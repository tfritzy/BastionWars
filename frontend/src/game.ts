import { Connection } from "./connection.ts";
import type { GameFoundForPlayer } from "./Schema.ts";
import { Typeable } from "./typeable.ts";

export class Game {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private buttons: Array<Typeable>;
  private connection: Connection | undefined;

  constructor(canvas: HTMLCanvasElement, ctx: CanvasRenderingContext2D) {
    this.canvas = canvas;
    this.ctx = ctx;

    this.buttons = [
      new Typeable("In game now", () => console.log("Whatever"), canvas, ctx),
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

  connectToGameServer(details: GameFoundForPlayer) {
    this.connection = new Connection(details.address!);
  }
}
