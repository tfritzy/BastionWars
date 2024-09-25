export class Performance {
  private frameCount: number;
  private lastFpsUpdateTime: number;
  private fps: number;

  constructor() {
    this.frameCount = 0;
    this.lastFpsUpdateTime = 0;
    this.fps = 0;
  }

  public start(): void {
    this.frameCount++;

    const currentTime = performance.now();
    if (currentTime - this.lastFpsUpdateTime >= 1000) {
      this.fps = this.frameCount;
      this.frameCount = 0;
      this.lastFpsUpdateTime = currentTime;
    }
  }

  public stop(): void {
    // This method is no longer needed for FPS calculation
  }

  public draw(ctx: CanvasRenderingContext2D, canvas: HTMLCanvasElement): void {
    ctx.save();
    ctx.resetTransform(); // Reset any transformations
    ctx.font = "14px Arial";
    ctx.fillStyle = "black";
    ctx.textAlign = "right";
    ctx.textBaseline = "top";
    const metrics = `${this.fps} FPS`;
    ctx.fillText(metrics, canvas.width - 10, 10);
    ctx.restore();
  }
}
