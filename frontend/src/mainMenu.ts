import type { Connection } from "./connection.ts";
import type { OneofMatchmakingRequest, SearchForGame } from "./Schema.ts";
import { Typeable } from "./typeable.ts";

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

    this.ctx.restore();
  }

  private findGame() {
    const searchForGame: SearchForGame = {
      ranked: true,
    };

    const request: OneofMatchmakingRequest = {
      sender_id: "tkn_001",
      search_for_game: searchForGame,
    };

    this.connection.sendMessage(request);
  }
}
