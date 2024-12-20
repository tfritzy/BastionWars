import { InGameCli } from "./command_line/in_game_cli.ts";
import { Connection } from "./connection.ts";
import {
  ARROW_COLOR,
  ARROW_LENGTH,
  ARROW_LINE_WIDTH,
  HALF_T,
  Layer,
  UNIT_RADIUS,
  WORLD_TO_CANVAS,
  TILE_SIZE,
  colorForAlliance,
} from "./constants.ts";
import { Drawing } from "./drawing.ts";
import { drawProgressBar } from "./drawing/progress_bar.ts";
import { drawMap } from "./grid_drawing.ts";
import {
  addV3,
  cloneMultiplyV3,
  deleteBySwap,
  divide,
  magnitude,
} from "./helpers.ts";
import { drawKeep } from "./keep_drawing.ts";
import {
  adjustPosForRowOffset,
  determinePathPos,
  updateSoldierPathProgress,
} from "./pathing.ts";
import {
  TileType,
  type AllKeepUpdates,
  type FieldVisibilityChanges,
  type GameFoundForPlayer,
  type HarvestedFields,
  type InitialState,
  type NewGrownFields,
  type NewProjectiles,
  type NewSoldiers,
  type Oneof_GameServerToPlayer,
  type RemovedSoldiers,
  type RenderTileUpdates,
} from "./Schema.ts";
import { drawTree } from "./tree_drawing.ts";
import { Typeable } from "./typeable.ts";
import {
  initialGameState,
  parseKeep,
  parseProjectile,
  parseSoldier,
  parseField,
  type GameState,
  type ClientState,
} from "./types.ts";

export class Game {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private gameState: GameState = initialGameState;
  private clientState: ClientState;
  private drawing: Drawing;
  private time: number = 0;
  private zoom: number;
  private cli: InGameCli;

  constructor(canvas: HTMLCanvasElement, ctx: CanvasRenderingContext2D) {
    this.canvas = canvas;
    this.ctx = ctx;
    this.drawing = new Drawing();
    this.zoom = this.ctx.getTransform().a;
    this.clientState = {
      connection: undefined,
      camPos: { x: 0, y: 0 },
      targetCamPos: { x: 0, y: 0 },
    };
    this.cli = new InGameCli(this.clientState, this.gameState);
  }

  draw(dpr: number, deltaTime: number): void {
    this.time += deltaTime;
    this.scrollTowardsTargetPos(deltaTime);
    drawMap(this.drawing, this.gameState);
    this.drawKeeps(deltaTime);
    this.drawSoldiers(deltaTime);
    this.drawProjectiles(deltaTime);
    this.drawTrees(this.time);
    this.drawFields(deltaTime);
    this.drawing.draw(this.ctx);
  }

  scrollTowardsTargetPos(deltaTime: number) {
    if (
      this.clientState.camPos.x != this.clientState.targetCamPos.x ||
      this.clientState.camPos.y != this.clientState.targetCamPos.y
    ) {
      const delta = {
        x:
          (-this.clientState.targetCamPos.x * TILE_SIZE * this.zoom +
            this.canvas.width / 2 -
            this.clientState.camPos.x) /
          10,
        y:
          (-this.clientState.targetCamPos.y * TILE_SIZE * this.zoom +
            this.canvas.height * 0.35 -
            this.clientState.camPos.y) /
          10,
      };
      this.clientState.camPos.x += delta.x;
      this.clientState.camPos.y += delta.y;

      this.ctx.setTransform(
        this.zoom,
        0,
        0,
        this.zoom,
        this.clientState.camPos.x,
        this.clientState.camPos.y
      );
    }
  }

  drawKeeps(deltaTime: number) {
    this.gameState.keeps.forEach((keep) => {
      drawKeep(this.drawing, keep, deltaTime);
    });
  }

  drawFields(deltaTime: number) {
    for (let i = 0; i < this.gameState.harvestables.length; i++) {
      const h = this.gameState.harvestables[i];
      const x = h.pos.x * TILE_SIZE + HALF_T;
      const y = h.pos.y * TILE_SIZE + HALF_T;
      h.remainingGrowth -= deltaTime;
      h.remainingGrowth = Math.max(0, h.remainingGrowth);
      const progress =
        (h.totalGrowthTime - h.remainingGrowth) / h.totalGrowthTime;

      if (progress < 1) {
        drawProgressBar(this.drawing, x - 10, y, 20, 3, 1.5, progress);
      } else {
        h.resource.draw(x, y, deltaTime);
        drawProgressBar(this.drawing, x - 10, y + 6, 20, 3, 1.5, 1);
      }
    }
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

      this.drawing.drawCustom(
        {
          layer: Layer.Units,
          fill_style: colorForAlliance(sKeep?.alliance || 0)[300],
          should_fill: true,
          line_width: 0.25,
          should_stroke: true,
          stroke_style: colorForAlliance(sKeep?.alliance || 0)[700],
        },
        (ctx) => {
          const x = soldier.pos.x * WORLD_TO_CANVAS;
          const y = soldier.pos.y * WORLD_TO_CANVAS;

          ctx.moveTo(x + UNIT_RADIUS, y);
          ctx.arc(x, y, UNIT_RADIUS, 0, 2 * Math.PI);
        }
      );
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
    this.clientState.connection = new Connection(
      details.address!,
      details.player_id!,
      details.auth_token!,
      this.onMessage
    );
  }

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
    } else if (message.new_grown_fields) {
      this.handleNewGrownFields(message.new_grown_fields);
    } else if (message.harvested_fields) {
      this.handleHarvestedFields(message.harvested_fields);
    } else if (message.field_visibility_changes) {
      this.handleFieldVisibilityChanges(message.field_visibility_changes);
    }
  };

  handleInitialStateMsg(msg: InitialState) {
    console.log("initial state", msg);
    msg.keeps?.forEach((k) => {
      var keep = parseKeep(k);
      if (keep != null) {
        this.gameState.keeps.set(keep.id, keep);
        this.gameState.keepNameToId.set(keep.name, keep.id);
      }
    });
    this.gameState.harvestables = [];
    msg.grown_fields?.forEach((w) => {
      const parsed = parseField(w, this.drawing);
      if (!parsed) return;
      this.gameState.harvestables.push(parsed);
    });

    this.gameState.tiles = msg.tiles || [];
    this.gameState.renderTiles = msg.render_tiles || [];
    this.gameState.mapHeight = msg.map_height || 0;
    this.gameState.mapWidth = msg.map_width || 0;
    this.gameState.ownAlliance = msg.own_alliance || 0;

    const k = Array.from(this.gameState.keeps.values()).find(
      (k) => k.alliance === this.gameState.ownAlliance
    );
    if (k) {
      this.clientState.camPos = {
        x: -k.pos.x * TILE_SIZE * this.zoom + this.canvas.width / 2,
        y: -k.pos.y * TILE_SIZE * this.zoom + this.canvas.height * 0.35,
      };
      this.clientState.targetCamPos = { x: k.pos.x, y: k.pos.y };
    }
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

  handleNewGrownFields(msg: NewGrownFields) {
    msg.fields?.forEach((f) => {
      const i = this.gameState.harvestables.findIndex(
        (h) =>
          h.pos.x === (f.grid_pos?.x || 0) && h.pos.y === (f.grid_pos?.y || 0)
      );

      if (i >= 0) {
        this.gameState.harvestables[i].text = f.text || "";
        this.gameState.harvestables[i].remainingGrowth = 0;
        this.gameState.harvestables[i].resource.text = f.text || "";
      }
    });
  }

  handleHarvestedFields(msg: HarvestedFields) {
    msg.fields?.forEach((f) => {
      const i = this.gameState.harvestables.findIndex(
        (h) => h.pos.x === (f.pos?.x || 0) && h.pos.y === (f.pos?.y || 0)
      );

      if (i >= 0) {
        this.gameState.harvestables[i].remainingGrowth =
          f.remainingGrowthTime || 0;
        this.gameState.harvestables[i].totalGrowthTime = f.totalGrowthTime || 0;
      }
    });
  }

  handleFieldVisibilityChanges(msg: FieldVisibilityChanges) {
    msg.new_values?.forEach((f) => {
      const i = this.gameState.harvestables.findIndex(
        (h) =>
          h.pos.x === (f.grid_pos?.x || 0) && h.pos.y === (f.grid_pos?.y || 0)
      );

      if (f.visible) {
        if (i >= 0) {
          this.gameState.harvestables[i].remainingGrowth =
            f.remainingGrowthTime || 0;
          this.gameState.harvestables[i].text = f.text || "";
          this.gameState.harvestables[i].totalGrowthTime =
            f.totalGrowthTime || 1;
        } else {
          const field = parseField(f, this.drawing);
          if (field) {
            this.gameState.harvestables.push(field);
          }
        }
      } else if (i >= 0) {
        deleteBySwap(this.gameState.harvestables, i);
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
      this.gameState.renderTiles[index] = {
        tile_case: rt.render_tile.tile_case || 0,
        corner_alliance: rt.render_tile.corner_alliance || [],
        alliance_case: rt.render_tile.alliance_case,
      };
    });
  }
}
