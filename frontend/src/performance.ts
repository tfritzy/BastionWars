export class Performance {
  private start_time: number;
  private frame_time: number;

  constructor() {
    this.start_time = 0;
    this.frame_time = 0;
  }

  public start(): void {
    this.start_time = performance.now();
  }

  public stop(): void {
    this.frame_time = performance.now() - this.start_time;
  }

  public draw(ctx: CanvasRenderingContext2D, canvas: HTMLCanvasElement): void {
    ctx.save();
    ctx.resetTransform(); // Reset any transformations
    ctx.font = "14px Arial";
    ctx.fillStyle = "black";
    ctx.textAlign = "right";
    ctx.textBaseline = "top";
    const metrics = `${this.frame_time.toFixed(0)} ms`;
    ctx.fillText(metrics, canvas.width - 10, 10);
    ctx.restore();
  }
}
