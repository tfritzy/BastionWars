import { drawPentagon, drawGrid } from "./rendering.js";
import { CanvasControls } from "./controls.js";
import { Connection } from "./connection.js";
import { Performance } from "./performance.js";
import { MainMenu } from "./mainMenu.js";

export class Game {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private controls: CanvasControls;
  private connection: Connection;
  private performance: Performance;
  private mainMenu: MainMenu;
  private dpr: number;
  private lastTime: number;

  constructor() {
    this.canvas = document.getElementById("myCanvas") as HTMLCanvasElement;
    this.ctx = this.canvas.getContext("2d")!;
    this.controls = new CanvasControls(this.canvas, this.draw.bind(this));
    this.connection = new Connection("tkn_001");
    this.performance = new Performance();
    this.mainMenu = new MainMenu(this.canvas, this.ctx);
    this.dpr = window.devicePixelRatio || 1;
    this.lastTime = performance.now();

    this.resizeCanvas();
    this.addEventListeners();
    this.update();
  }

  private resizeCanvas(): void {
    const { width, height } = this.calculateCanvasSize();
    this.canvas.width = width;
    this.canvas.height = height;
    this.canvas.style.width = `${width / this.dpr}px`;
    this.canvas.style.height = `${height / this.dpr}px`;
    this.draw();
  }

  private calculateCanvasSize(): { width: number; height: number } {
    const maxSize = 16384; // Maximum canvas size for most browsers
    let width = window.innerWidth * this.dpr;
    let height = window.innerHeight * this.dpr;

    if (width > maxSize || height > maxSize) {
      const ratio = Math.min(maxSize / width, maxSize / height);
      width *= ratio;
      height *= ratio;
    }

    return { width: Math.floor(width), height: Math.floor(height) };
  }

  private draw(): void {
    const { scale, offsetX, offsetY } = this.controls.getTransform();
    const deltaTime = (performance.now() - this.lastTime) / 1000;
    this.lastTime = performance.now();

    this.ctx.save();
    this.ctx.scale(this.dpr, this.dpr);
    this.ctx.clearRect(
      0,
      0,
      this.canvas.width / this.dpr,
      this.canvas.height / this.dpr
    );
    this.ctx.scale(scale, scale);
    this.ctx.translate(offsetX, offsetY);

    this.ctx.textRendering = "geometricPrecision";
    this.ctx.imageSmoothingEnabled = false;
    drawGrid(
      this.ctx,
      this.controls,
      this.canvas.width / this.dpr,
      this.canvas.height / this.dpr
    );
    this.mainMenu.draw(this.dpr, deltaTime);
    this.drawShapes();

    this.ctx.restore();
  }

  private update(currentTime?: number): void {
    requestAnimationFrame((time) => this.update(time));
    this.performance.start();
    this.draw();
    this.performance.stop();
    this.performance.draw(this.ctx, this.canvas);
  }

  private drawShapes(): void {
    drawPentagon(this.ctx, 300, 300, 50, "rgba(255, 0, 255, 0.5)");
  }

  private addEventListeners(): void {
    window.addEventListener("resize", this.resizeCanvas.bind(this));
  }
}
