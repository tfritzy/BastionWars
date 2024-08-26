import type { Connection } from "./connection.ts";
import { Constants } from "./constants.ts";
import {
 decodeOneof_MatchMakerToPlayer,
 encodeOneof_PlayerToMatchmaker,
 type Oneof_PlayerToMatchmaker,
 type SearchForGame,
} from "./Schema.ts";
import { Typeable } from "./typeable.ts";
// import "dotenv/config";

export class MainMenu {
 private canvas: HTMLCanvasElement;
 private ctx: CanvasRenderingContext2D;
 private buttons: Array<Typeable>;
 private connection: Connection;

 constructor(
  canvas: HTMLCanvasElement,
  ctx: CanvasRenderingContext2D,
  connection: Connection
 ) {
  this.canvas = canvas;
  this.ctx = ctx;
  this.connection = connection;

  this.buttons = [
   new Typeable(
    "Start",
    () => this.findGame(),
    canvas,
    ctx
   ),
   new Typeable(
    "Options",
    () => console.log("Options complete"),
    canvas,
    ctx
   ),
   new Typeable(
    "Exit",
    () => console.log("Exit complete"),
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

  this.ctx.restore();
 }

 private async findGame() {
  const searchForGame: SearchForGame = {
   ranked: true,
  };

  const request: Oneof_PlayerToMatchmaker = {
   player_id: "plyr_001",
   search_for_game: searchForGame,
  };

  try {
   const matchmakerAddress = Constants.MATCHMAKING_URL;
   const response = await fetch(
    `${matchmakerAddress}/search-for-game`,
    {
     method: "POST",
     headers: {
      "Content-Type": "application/x-protobuf",
     },
     body: encodeOneof_PlayerToMatchmaker(request),
    }
   );

   if (!response.ok) {
    throw new Error(
     `Failed to connect with matchmaking server. status: ${response.status}`
    );
   }

   const responseData = await response.arrayBuffer();
   const responseMsg = decodeOneof_MatchMakerToPlayer(
    new Uint8Array(responseData)
   );
   console.log("Game found:", responseMsg);
  } catch (error) {
   console.error("Error during game search:", error);
  }
 }
}
