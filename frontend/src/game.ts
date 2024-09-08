import { Connection } from "./connection.ts";
import {
 soldierColors,
 WORLD_TO_CANVAS,
} from "./constants.ts";
import {
 SoldierType,
 type AllKeepUpdates,
 type AllSoldierPositions,
 type GameFoundForPlayer,
 type InitialState,
 type KeepUpdate,
 type Oneof_GameServerToPlayer,
} from "./Schema.ts";
import { Typeable } from "./typeable.ts";
import {
 initialGameState,
 parseKeep,
 parseSoldier,
 updateSoldier,
 type GameState,
 type Keep,
} from "./types.ts";

export class Game {
 private canvas: HTMLCanvasElement;
 private ctx: CanvasRenderingContext2D;
 private connection: Connection | undefined;
 private gameState: GameState = initialGameState;
 private keepLabels: Map<number, Typeable> = new Map();
 private selectedKeep: number | null = null;

 constructor(
  canvas: HTMLCanvasElement,
  ctx: CanvasRenderingContext2D
 ) {
  this.canvas = canvas;
  this.ctx = ctx;
 }

 draw(dpr: number, deltaTime: number): void {
  this.ctx.save();

  this.ctx.font = "20px Arial";
  this.ctx.fillStyle = "black";
  this.ctx.textAlign = "center";

  this.drawKeeps(deltaTime);
  this.drawSoldiers();
  this.ctx.restore();
 }

 drawKeeps(deltaTime: number) {
  this.ctx.save();
  this.ctx.strokeStyle = "black";
  this.ctx.lineWidth = 2;

  const radius = 30;
  const innerRadius = radius * 0.8;
  const pieRadius = innerRadius * 0.8;

  this.gameState.keeps.forEach((keep) => {
   const x = keep.pos.x * WORLD_TO_CANVAS;
   const y = keep.pos.y * WORLD_TO_CANVAS;

   this.ctx.beginPath();
   this.ctx.arc(x, y, radius, 0, 2 * Math.PI);
   this.ctx.stroke();

   this.ctx.beginPath();
   this.ctx.arc(x, y, innerRadius, 0, 2 * Math.PI);
   this.ctx.stroke();

   this.drawPieChart(x, y, pieRadius, keep);

   this.ctx.save();
   this.ctx.textAlign = "center";
   this.ctx.font = "10px";
   this.ctx.fillStyle = "#4a4b5b";
   const totalCount = (
    keep.archer_count + keep.warrior_count
   ).toString();
   this.ctx.fillText(totalCount, x, y + 7);
   this.ctx.restore();

   this.keepLabels.get(keep.id)?.draw(x, y - 30, deltaTime);
  });

  this.ctx.restore();
 }

 drawPieChart(
  x: number,
  y: number,
  radius: number,
  keep: Keep
 ) {
  let angle = 0;
  const totalSoldiers =
   keep.archer_count + keep.warrior_count;
  let slice =
   (keep.warrior_count / totalSoldiers) * 2 * Math.PI;
  this.drawArc(
   x,
   y,
   radius,
   angle,
   slice,
   soldierColors[SoldierType.Warrior]
  );
  angle += slice;
  slice = (keep.archer_count / totalSoldiers) * 2 * Math.PI;
  this.drawArc(
   x,
   y,
   radius,
   angle,
   slice,
   soldierColors[SoldierType.Archer]
  );
 }

 drawArc(
  x: number,
  y: number,
  radius: number,
  startAngle: number,
  sliceAngle: number,
  color: string
 ) {
  this.ctx.beginPath();
  this.ctx.moveTo(x, y);
  this.ctx.arc(
   x,
   y,
   radius,
   startAngle,
   startAngle + sliceAngle
  );
  this.ctx.closePath();

  this.ctx.fillStyle = color;
  this.ctx.fill();
 }

 drawSoldiers() {
  this.ctx.save();
  this.ctx.strokeStyle = "black";
  this.ctx.lineWidth = 2;

  const radius = 4;
  this.gameState.soldiers.forEach((keep) => {
   const x = keep.pos.x * WORLD_TO_CANVAS;
   const y = keep.pos.y * WORLD_TO_CANVAS;

   this.ctx.beginPath();
   this.ctx.arc(x, y, radius, 0, 2 * Math.PI);
   this.ctx.stroke();
  });

  this.ctx.restore();
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
      () => this.handleTypeKeepName(id),
      this.canvas,
      this.ctx
     )
    );
   }
  });
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
  console.log("Keep udpates", msg.keep_updates);
  msg.keep_updates?.forEach((ku) => {
   const keepIndex = this.gameState.keeps.findIndex(
    (k) => k.id === ku.id
   );
   if (keepIndex >= 0) {
    this.gameState.keeps[keepIndex].archer_count =
     ku.archer_count || 0;
    this.gameState.keeps[keepIndex].warrior_count =
     ku.warrior_count || 0;
   }
  });
 }
}
