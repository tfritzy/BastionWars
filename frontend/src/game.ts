import { Connection } from "./connection.ts";
import {
 ARROW_COLOR,
 ARROW_LENGTH,
 KEEP_LABEL_FONT,
 KEEP_LABEL_STROKE,
 Layer,
 UNIT_COLOR,
 UNIT_OUTLINE_COLOR,
 UNIT_OUTLINE_WIDTH,
 UNIT_RADIUS,
 WORLD_TO_CANVAS,
} from "./constants.ts";
import { Drawing } from "./drawing.ts";
import { drawMap } from "./grid_drawing.ts";
import { drawKeep } from "./keep_drawing.ts";
import {
 TileType,
 type AllKeepUpdates,
 type AllSoldierPositions,
 type GameFoundForPlayer,
 type InitialState,
 type NewProjectiles,
 type Oneof_GameServerToPlayer,
 type RenderTileUpdates,
} from "./Schema.ts";
import { drawTree, drawTreeLayer } from "./tree_drawing.ts";
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
 updateSoldier,
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

 constructor(
  canvas: HTMLCanvasElement,
  ctx: CanvasRenderingContext2D
 ) {
  this.canvas = canvas;
  this.ctx = ctx;
  this.drawing = new Drawing();
 }

 draw(dpr: number, deltaTime: number): void {
  drawMap(this.drawing, this.gameState);
  this.drawKeeps(deltaTime);
  this.drawSoldiers();
  this.drawing.draw(this.ctx);
  this.drawKeepLables(deltaTime);
  this.drawProjectiles(deltaTime);
  this.drawTrees(deltaTime);
 }

 drawKeeps(deltaTime: number) {
  this.gameState.keeps.forEach((keep) => {
   const x = keep.pos.x * WORLD_TO_CANVAS;
   const y = keep.pos.y * WORLD_TO_CANVAS;
   drawKeep(this.drawing, keep, deltaTime);
  });
 }

 drawTrees(deltaTime: number) {
  this.gameState.tiles.forEach((tile, i) => {
   if (tile == TileType.Tree) {
    const x = Math.floor(i % this.gameState.mapWidth);
    const y = Math.floor(i / this.gameState.mapWidth);
    drawTree(this.drawing, x, y, deltaTime);
   }
  });
 }

 drawKeepLables(deltaTime: number) {
  this.gameState.keeps.forEach((keep) => {
   const x = keep.pos.x * WORLD_TO_CANVAS;
   const y = keep.pos.y * WORLD_TO_CANVAS;
   this.keepLabels.get(keep.id)?.draw(x, y - 25, deltaTime);
  });
 }

 soldierStyle = {
  layer: Layer.Units,
  fill_style: UNIT_COLOR,
  line_width: UNIT_OUTLINE_WIDTH,
  should_fill: true,
  should_stroke: false,
  stroke_style: UNIT_OUTLINE_COLOR,
 };
 drawSoldiers() {
  this.gameState.soldiers.forEach((soldier) => {
   this.drawing.drawCustom(this.soldierStyle, (ctx) => {
    const x = soldier.pos.x * WORLD_TO_CANVAS;
    const y = soldier.pos.y * WORLD_TO_CANVAS;

    ctx.moveTo(x + UNIT_RADIUS, y);
    ctx.arc(x, y, UNIT_RADIUS, 0, 2 * Math.PI);
   });
  });
 }

 drawProjectiles(deltaTime: number) {
  for (
   let i = this.gameState.projectiles.length - 1;
   i >= 0;
   i--
  ) {
   const proj = this.gameState.projectiles[i];
   proj.remaining_life -= deltaTime;
   proj.remaining_movement_time -= deltaTime;

   if (proj.remaining_life <= 0) {
    const lastIndex = this.gameState.projectiles.length - 1;
    if (i !== lastIndex) {
     this.gameState.projectiles[i] =
      this.gameState.projectiles[lastIndex];
    }
    this.gameState.projectiles.pop();
    continue;
   }

   const hasLanded = proj.remaining_movement_time <= 0;
   if (!hasLanded) {
    proj.current_velocity.z -=
     proj.gravitational_force * deltaTime;
    const deltaV = cloneMultiplyV3(
     proj.current_velocity,
     deltaTime
    );
    addV3(proj.current_pos, deltaV);
   }

   const layer = hasLanded
    ? Layer.ProjectilesOnGround
    : Layer.Projectiles;
   this.drawing.drawStrokeable(
    ARROW_COLOR,
    1,
    layer,
    (ctx) => {
     const x = proj.current_pos.x * WORLD_TO_CANVAS;
     const y = proj.current_pos.y * WORLD_TO_CANVAS;
     const xy = {
      x: proj.current_velocity.x,
      y: proj.current_velocity.y,
     };
     const xyMag = magnitude(xy);
     const theta = Math.atan(
      proj.current_velocity.z / xyMag
     );
     const arrowVisibleLength =
      Math.cos(theta) * ARROW_LENGTH;
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
  } else if (message.all_soldier_positions) {
   this.handleSoldierPositionsMsg(
    message.all_soldier_positions
   );
  } else if (message.keep_updates) {
   this.handleKeepUpdates(message.keep_updates);
  } else if (message.new_projectiles) {
   this.handleNewProjectiles(message.new_projectiles);
  } else if (message.render_tile_updates) {
   this.handleRenderTileUpdates(
    message.render_tile_updates
   );
  }
 };

 handleInitialStateMsg(msg: InitialState) {
  console.log("initial state", msg);
  msg.keeps?.forEach((k) => {
   var keep = parseKeep(k);
   if (keep != null) {
    this.gameState.keeps.push(keep);
    const id = keep.id;
    this.keepLabels.set(
     keep.id,
     new Typeable(
      keep.name,
      KEEP_LABEL_FONT,
      () => this.handleTypeKeepName(id),
      this.drawing,
      KEEP_LABEL_STROKE
     )
    );
   }
  });

  this.gameState.tiles = msg.tiles || [];
  this.gameState.renderTiles = msg.render_tiles || [];
  this.gameState.mapHeight = msg.map_height || 0;
  this.gameState.mapWidth = msg.map_width || 0;
 }

 handleSoldierPositionsMsg(msg: AllSoldierPositions) {
  const currentTime = Date.now();
  msg.soldier_positions?.forEach((s) => {
   const existingSoldier =
    s.id && this.gameState.soldiers.get(s.id);
   if (!existingSoldier) {
    const soldier = parseSoldier(s, currentTime);
    if (soldier) {
     this.gameState.soldiers.set(soldier.id, soldier);
    }
   } else {
    updateSoldier(existingSoldier, s, currentTime);
   }
  });

  for (let [key, value] of this.gameState.soldiers) {
   if (value.lastUpdated !== currentTime) {
    this.gameState.soldiers.delete(key);
   }
  }
 }

 handleKeepUpdates(msg: AllKeepUpdates) {
  msg.keep_updates?.forEach((ku) => {
   const keepIndex = this.gameState.keeps.findIndex(
    (k) => k.id === ku.id
   );
   if (keepIndex >= 0) {
    this.gameState.keeps[keepIndex].archer_count =
     ku.archer_count || 0;
    this.gameState.keeps[keepIndex].warrior_count =
     ku.warrior_count || 0;
    this.gameState.keeps[keepIndex].alliance =
     ku.alliance || 0;
   }
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
    (rt.pos.x || 0) +
    (rt.pos.y || 0) * (this.gameState.mapWidth + 1);
   this.gameState.renderTiles[index] = rt.render_tile;
  });
 }
}
