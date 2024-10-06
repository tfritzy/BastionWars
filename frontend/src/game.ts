import { Connection } from "./connection.ts";
import {
  ARROW_COLOR,
  ARROW_LENGTH,
  ARROW_LINE_WIDTH,
  HALF_T,
  Layer,
  SHADOW_COLOR,
  UNIT_SHADOW_OFFSET,
  UNIT_COLOR,
  UNIT_OUTLINE_COLOR,
  UNIT_OUTLINE_WIDTH,
  UNIT_RADIUS,
  WORLD_TO_CANVAS,
  TILE_SIZE,
  KEEP_LABEL_COMPLETED_STYLE,
  KEEP_LABEL_REMAINING_STYLE,
} from "./constants.ts";
import { Drawing } from "./drawing.ts";
import { drawMap } from "./grid_drawing.ts";
import { drawKeep } from "./keep_drawing.ts";
import {
  adjustPosForRowOffset,
  determinePathPos,
  updateSoldierPathProgress,
} from "./pathing.ts";
import {
  TileType,
  type AllKeepUpdates,
  type GameFoundForPlayer,
  type InitialState,
  type NewProjectiles,
  type NewSoldiers,
  type Oneof_GameServerToPlayer,
  type RemovedSoldiers,
  type RenderTileUpdates,
} from "./Schema.ts";
import { drawTree } from "./tree_drawing.ts";
import { Typeable } from "./typeable.ts";
import {
  addV3,
  cloneMultiplyV3,
  divide,
  initialGameState,
  magnitude,
  parseKeep,
  parseProjectile,
  parseSoldier,
  parseWords,
  type GameState,
} from "./types.ts";

export class Game {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private connection: Connection | undefined;
  private gameState: GameState = initialGameState;
  private keepLabels: Map<number, Typeable> = new Map();
  private selectedKeep: number | null = null;
  private drawing: Drawing;
  private time: number = 0;

  constructor(canvas: HTMLCanvasElement, ctx: CanvasRenderingContext2D) {
    this.canvas = canvas;
    this.ctx = ctx;
    this.drawing = new Drawing();
  }

  draw(dpr: number, deltaTime: number): void {
    this.time += deltaTime;
    drawMap(this.drawing, this.gameState);
    this.drawKeeps(deltaTime);
    this.drawSoldiers(deltaTime);
    this.drawing.draw(this.ctx);
    this.drawKeepLables(deltaTime);
    this.drawProjectiles(deltaTime);
    this.drawTrees(this.time);
    this.drawWords(deltaTime);
  }

  drawKeeps(deltaTime: number) {
    this.gameState.keeps.forEach((keep) => {
      drawKeep(this.drawing, keep, deltaTime);
    });
  }

  drawWords(deltaTime: number) {
    this.gameState.words.forEach((word, i) => {
      const x = word.pos.x * TILE_SIZE + HALF_T;
      const y = word.pos.y * TILE_SIZE + HALF_T;
      word.typeable.draw(x, y, deltaTime);
    });
  }

  drawTrees(time: number) {
    this.gameState.tiles.forEach((tile, i) => {
      if (tile == TileType.Tree) {
        const x = Math.floor(i % this.gameState.mapWidth);
        const y = Math.floor(i / this.gameState.mapWidth);
        drawTree(this.drawing, x, y, time);
      }
    });
  }

  drawKeepLables(deltaTime: number) {
    this.gameState.keeps.forEach((keep) => {
      const x = keep.pos.x * WORLD_TO_CANVAS + HALF_T;
      const y = keep.pos.y * WORLD_TO_CANVAS + HALF_T;
      this.keepLabels.get(keep.id)?.draw(x, y - 35, deltaTime);
    });
  }

  soldierShadowOffset = 2;
  soldierShadowStyle = {
    layer: Layer.UnitShadows,
    fill_style: SHADOW_COLOR,
    should_fill: true,
  };

  soldierStyle = {
    layer: Layer.Units,
    fill_style: UNIT_COLOR,
    line_width: UNIT_OUTLINE_WIDTH,
    should_fill: true,
    should_stroke: true,
    stroke_style: UNIT_OUTLINE_COLOR,
  };
  drawSoldiers(deltaTime: number) {
    this.gameState.soldiers.forEach((soldier) => {
      const sKeep = this.gameState.keeps.get(soldier.sourceKeepId);
      const path = sKeep!.pathMap.get(soldier.targetKeepId)!;
      const walkPath = sKeep!.walkMap.get(soldier.targetKeepId)!;
      updateSoldierPathProgress(soldier, walkPath, deltaTime);
      if (soldier.pathIndex >= 0 && soldier.pathIndex < path.length) {
        const nextPos = determinePathPos(
          path[soldier.pathIndex],
          walkPath[soldier.pathIndex],
          soldier.subPathProgress
        );
        soldier.pos = adjustPosForRowOffset(
          soldier.nonOffsetPos,
          nextPos,
          soldier.rowOffset
        );
        soldier.nonOffsetPos = nextPos;
      }

      this.drawing.drawCustom(this.soldierShadowStyle, (ctx) => {
        const x = soldier.pos.x * WORLD_TO_CANVAS;
        const y = soldier.pos.y * WORLD_TO_CANVAS;

        ctx.moveTo(
          x + UNIT_RADIUS - UNIT_SHADOW_OFFSET,
          y + UNIT_SHADOW_OFFSET
        );
        ctx.arc(
          x - UNIT_SHADOW_OFFSET,
          y + UNIT_SHADOW_OFFSET,
          UNIT_RADIUS,
          0,
          2 * Math.PI
        );
      });

      this.drawing.drawCustom(this.soldierStyle, (ctx) => {
        const x = soldier.pos.x * WORLD_TO_CANVAS;
        const y = soldier.pos.y * WORLD_TO_CANVAS;

        ctx.moveTo(x + UNIT_RADIUS, y);
        ctx.arc(x, y, UNIT_RADIUS, 0, 2 * Math.PI);
      });
    });
  }

  drawProjectiles(deltaTime: number) {
    for (let i = this.gameState.projectiles.length - 1; i >= 0; i--) {
      const proj = this.gameState.projectiles[i];
      proj.remaining_life -= deltaTime;
      proj.remaining_movement_time -= deltaTime;

      if (proj.remaining_life <= 0) {
        const lastIndex = this.gameState.projectiles.length - 1;
        if (i !== lastIndex) {
          this.gameState.projectiles[i] = this.gameState.projectiles[lastIndex];
        }
        this.gameState.projectiles.pop();
        continue;
      }

      const hasLanded = proj.remaining_movement_time <= 0;
      if (!hasLanded) {
        proj.current_velocity.z -= proj.gravitational_force * deltaTime;
        const deltaV = cloneMultiplyV3(proj.current_velocity, deltaTime);
        addV3(proj.current_pos, deltaV);
      }

      const layer = hasLanded ? Layer.ProjectilesOnGround : Layer.Projectiles;
      this.drawing.drawStrokeable(
        ARROW_COLOR,
        ARROW_LINE_WIDTH,
        layer,
        (ctx) => {
          const x = proj.current_pos.x * WORLD_TO_CANVAS;
          const y = proj.current_pos.y * WORLD_TO_CANVAS;
          const xy = {
            x: proj.current_velocity.x,
            y: proj.current_velocity.y,
          };
          const xyMag = magnitude(xy);
          const theta = Math.atan(proj.current_velocity.z / xyMag);
          const arrowVisibleLength = Math.cos(theta) * ARROW_LENGTH;
          const halfArrow = arrowVisibleLength / 2;
          divide(xy, xyMag);

          ctx.moveTo(x + xy.x * halfArrow, y + xy.y * halfArrow);
          ctx.lineTo(x - xy.x * halfArrow, y - xy.y * halfArrow);
        }
      );
    }
  }

  connectToGameServer(details: GameFoundForPlayer) {
    this.connection = new Connection(
      details.address!,
      details.player_id!,
      details.auth_token!,
      this.onMessage
    );
  }

  handleTypeKeepName = (id: number) => {
    if (this.selectedKeep == null) {
      this.selectedKeep = id;
    } else {
      this.connection?.sendMessage({
        sender_id: "Something I guess",
        issue_deployment_order: {
          source_keep: this.selectedKeep,
          target_keep: id,
        },
      });
      this.selectedKeep = null;
    }
  };

  onMessage = (message: Oneof_GameServerToPlayer) => {
    if (message.initial_state) {
      this.handleInitialStateMsg(message.initial_state);
    } else if (message.new_soldiers) {
      this.handleNewSoldiersMsg(message.new_soldiers);
    } else if (message.removed_soldiers) {
      this.handleRemovedSoldiersMsg(message.removed_soldiers);
    } else if (message.keep_updates) {
      this.handleKeepUpdates(message.keep_updates);
    } else if (message.new_projectiles) {
      this.handleNewProjectiles(message.new_projectiles);
    } else if (message.render_tile_updates) {
      this.handleRenderTileUpdates(message.render_tile_updates);
    }
  };

  handleInitialStateMsg(msg: InitialState) {
    console.log("initial state", msg);
    msg.keeps?.forEach((k) => {
      var keep = parseKeep(k);
      if (keep != null) {
        this.gameState.keeps.set(keep.id, keep);
        const id = keep.id;
        this.keepLabels.set(
          keep.id,
          new Typeable(
            keep.name,
            KEEP_LABEL_COMPLETED_STYLE,
            KEEP_LABEL_REMAINING_STYLE,
            this.drawing,
            () => this.handleTypeKeepName(id)
          )
        );
      }
    });

    this.gameState.tiles = msg.tiles || [];
    this.gameState.renderTiles = msg.render_tiles || [];
    this.gameState.mapHeight = msg.map_height || 0;
    this.gameState.mapWidth = msg.map_width || 0;
    this.gameState.words = parseWords(msg, this.drawing);
  }

  handleNewSoldiersMsg(msg: NewSoldiers) {
    msg.soldiers?.forEach((s) => {
      const soldier = parseSoldier(s);
      if (!soldier) return;
      this.gameState.soldiers.set(soldier.id, soldier);
    });
  }

  handleRemovedSoldiersMsg(msg: RemovedSoldiers) {
    msg.soldier_ids?.forEach((s) => {
      if (this.gameState.soldiers.has(s)) {
        const sol = this.gameState.soldiers.get(s);
        console.log(`Soldier ${sol?.id} died at ${sol?.pos.x}, ${sol?.pos.y}`);

        this.gameState.soldiers.delete(s);
      }
    });
  }

  handleKeepUpdates(msg: AllKeepUpdates) {
    msg.keep_updates?.forEach((ku) => {
      if (!ku.id) return;
      const keep = this.gameState.keeps.get(ku.id);
      if (!keep) return;
      keep.archer_count = ku.archer_count || 0;
      keep.warrior_count = ku.warrior_count || 0;
      keep.alliance = ku.alliance || 0;
    });
  }

  handleNewProjectiles(msg: NewProjectiles) {
    msg.projectiles?.forEach((p) => {
      const proj = parseProjectile(p);
      if (proj) {
        this.gameState.projectiles.push(proj);
      }
    });
  }

  handleRenderTileUpdates(msg: RenderTileUpdates) {
    msg.render_tile_updates?.forEach((rt) => {
      if (!rt || !rt.pos || !rt.render_tile) {
        return;
      }

      const index =
        (rt.pos.x || 0) + (rt.pos.y || 0) * (this.gameState.mapWidth + 1);
      this.gameState.renderTiles[index] = rt.render_tile;
    });
  }
}
