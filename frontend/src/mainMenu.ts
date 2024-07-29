import { Typeable } from "./typeable.ts";

export class MainMenu {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private buttons: Array<Typeable>;

  constructor(canvas: HTMLCanvasElement, ctx: CanvasRenderingContext2D) {
    this.canvas = canvas;
    this.ctx = ctx;
    this.buttons = [
      new Typeable("Start", () => console.log("Start complete"), canvas, ctx),
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
}
