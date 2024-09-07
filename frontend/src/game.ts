import { Connection } from "./connection.ts";
import { WORLD_TO_CANVAS } from "./constants.ts";
import type {
 AllSoldierPositions,
 GameFoundForPlayer,
 InitialState,
 Long,
 Oneof_GameServerToPlayer,
 SoldierType,
} from "./Schema.ts";
import { Typeable } from "./typeable.ts";
import {
 initialGameState,
 parseKeep,
 parseSoldier,
 updateSoldier,
 type GameState,
} from "./types.ts";

export class Game {
 private canvas: HTMLCanvasElement;
 private ctx: CanvasRenderingContext2D;
 private buttons: Array<Typeable>;
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

  this.buttons = [
   new Typeable(
    "In game now",
    () => console.log("Whatever"),
    canvas,
    ctx
   ),
  ];
 }

 draw(dpr: number, deltaTime: number): void {
  this.ctx.save();

  this.ctx.font = "30px Arial";
  this.ctx.fillStyle = "black";
  this.ctx.textAlign = "center";

  const canvasLogicalHeight = this.canvas.height / dpr;
  const canvasLogicalWidth = this.canvas.width / dpr;

  this.buttons.forEach((button, index) => {
   const x = canvasLogicalWidth / 2 - 60;
   const y = canvasLogicalHeight / 2 + index * 60 - 60;
   button.draw(x, y, deltaTime);
  });

  this.drawKeeps(deltaTime);
  this.drawSoldiers();
  this.ctx.restore();
 }

 drawKeeps(deltaTime: number) {
  this.ctx.save();
  this.ctx.strokeStyle = "black";
  this.ctx.lineWidth = 2;

  const radius = 20;
  const innerRadius = radius * 0.7;

  this.gameState.keeps.forEach((keep) => {
   const x = keep.pos.x * WORLD_TO_CANVAS;
   const y = keep.pos.y * WORLD_TO_CANVAS;

   this.ctx.beginPath();
   this.ctx.arc(x, y, radius, 0, 2 * Math.PI);
   this.ctx.stroke();

   this.ctx.beginPath();
   this.ctx.arc(x, y, innerRadius, 0, 2 * Math.PI);
   this.ctx.stroke();

   this.keepLabels.get(keep.id)?.draw(x, y, deltaTime);
  });

  this.ctx.restore();
 }

 drawSoldiers() {
  this.ctx.save();
  this.ctx.strokeStyle = "black";
  this.ctx.lineWidth = 2;

  const radius = 10;
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
  }
 };

 onMessage = (message: Oneof_GameServerToPlayer) => {
  if (message.initial_state) {
   this.handleInitialStateMsg(message.initial_state);
  } else if (message.all_soldier_positions) {
   this.handleSoldierPositionsMsg(
    message.all_soldier_positions
   );
  }
 };

 handleInitialStateMsg(msg: InitialState) {
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
  msg.soldier_positions?.forEach((s) => {
   const existingSoldier =
    s.id && this.gameState.soldiers.get(s.id);
   if (!existingSoldier) {
    const soldier = parseSoldier(s);
    if (soldier) {
     this.gameState.soldiers.set(soldier.id, soldier);
    }
   } else {
    updateSoldier(existingSoldier, s);
   }
  });
 }
}
