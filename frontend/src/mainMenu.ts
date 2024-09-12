import type { Connection } from "./connection.ts";
import { Constants } from "./constants.ts";
import { drawPentagon } from "./rendering.ts";
import {
  decodeOneof_MatchMakerToPlayer,
  encodeOneof_PlayerToMatchmaker,
  type GameFoundForPlayer,
  type Oneof_PlayerToMatchmaker,
  type SearchForGame,
} from "./Schema.ts";
import { Typeable } from "./typeable.ts";

export class MainMenu {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private buttons: Array<Typeable>;
  private enterGame: (details: GameFoundForPlayer) => void;

  constructor(
    canvas: HTMLCanvasElement,
    ctx: CanvasRenderingContext2D,
    enterGame: (details: GameFoundForPlayer) => void
  ) {
    this.canvas = canvas;
    this.ctx = ctx;
    this.enterGame = enterGame;

    this.buttons = [
      new Typeable("Start", () => this.findGame(), canvas, ctx),
      new Typeable(
        "Options",
        () => console.log("Options complete"),
        canvas,
        ctx
      ),
      new Typeable("Exit", () => console.log("Exit complete"), canvas, ctx),
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

    this.drawShapes();

    this.ctx.restore();
  }

  private drawShapes(): void {
    drawPentagon(this.ctx, 300, 300, 50, "rgba(255, 0, 255, 0.5)");
  }

  private async findGame() {
    const searchForGame: SearchForGame = {
      ranked: true,
    };

    const request: Oneof_PlayerToMatchmaker = {
      player_id: "plyr_" + Math.floor(Math.random() * 1000),
      search_for_game: searchForGame,
    };

    const matchmakerAddress = Constants.MATCHMAKING_URL;
    const response = await fetch(`${matchmakerAddress}/search-for-game`, {
      method: "POST",
      headers: {
        "Content-Type": "application/x-protobuf",
      },
      body: encodeOneof_PlayerToMatchmaker(request),
    });

    if (!response.ok) {
      console.error(
        `Failed to connect with matchmaking server. status: ${response.status}. TODO: Show toast or something.`
      );
      return;
    }

    const responseData = await response.arrayBuffer();
    const responseMsg = decodeOneof_MatchMakerToPlayer(
      new Uint8Array(responseData)
    )?.found_game;
    console.log("Game found (stringified):", JSON.stringify(responseMsg));
    console.log("Game found (object):", responseMsg);

    if (responseMsg) {
      console.log("game_id:", responseMsg.game_id);
      console.log("player_id:", responseMsg.player_id);
      console.log("address:", responseMsg.address);
    }

    if (!responseMsg) {
      console.error(
        "Invalid response from matchmaking server. TODO show toast or something."
      );
      return;
    }

    this.enterGame(responseMsg);
  }
}
