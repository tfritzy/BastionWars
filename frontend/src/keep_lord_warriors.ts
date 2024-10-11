import { CanvasControls } from "./controls.js";
import { Performance } from "./performance.js";
import { MainMenu } from "./mainMenu.js";
import { Game } from "./game.js";
import type { GameFoundForPlayer } from "./Schema.js";
import { SHADOW_COLOR } from "./constants.js";
import { calculateCanvasSize, setDpr } from "./helpers.js";

export class KeepLordWarriors {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private controls: CanvasControls;
  private performance: Performance;
  private mainMenu: MainMenu;
  private game: Game | undefined;
  private dpr: number;
  private lastTime: number;
  private state: "main_menu" | "in_game" = "main_menu";

  constructor() {
    this.dpr = window.devicePixelRatio || 1;
    this.canvas = document.getElementById("myCanvas") as HTMLCanvasElement;
    this.ctx = this.initializeContext();
    this.controls = new CanvasControls(this.canvas, this.draw.bind(this));
    this.performance = new Performance();
    this.mainMenu = new MainMenu(this.canvas, this.ctx, this.enterGame);
    this.lastTime = performance.now();

    this.resizeCanvas();
    this.addEventListeners();
    this.update();
  }

  private initializeContext(): CanvasRenderingContext2D {
    var ctx = this.canvas.getContext("2d", {
      willReadFrequently: true,
    })!;
    ctx.imageSmoothingEnabled = false;
    return ctx;
  }

  private resizeCanvas(): void {
    const { width, height } = calculateCanvasSize();
    this.canvas.width = width;
    this.canvas.height = height;
    this.canvas.style.width = `${width / this.dpr}px`;
    this.canvas.style.height = `${height / this.dpr}px`;
    setDpr(this.canvas, this.ctx);
  }

  private draw(): void {
    const { offsetX, offsetY } = this.controls.getTransform();
    const deltaTime = (performance.now() - this.lastTime) / 1000;
    this.lastTime = performance.now();

    this.ctx.save();
    this.ctx.clearRect(
      0,
      0,
      this.canvas.width / this.dpr,
      this.canvas.height / this.dpr
    );
    this.ctx.translate(offsetX, offsetY);

    if (this.state === "in_game") {
      this.game!.draw(this.dpr, deltaTime);
    } else {
      this.mainMenu.draw(this.dpr, deltaTime);
    }

    this.ctx.restore();
  }

  private enterGame = (details: GameFoundForPlayer): void => {
    console.log("Entering game", details);
    this.game = new Game(this.canvas, this.ctx);
    this.game.connectToGameServer(details);
    this.state = "in_game";
  };

  private update(currentTime?: number): void {
    requestAnimationFrame((time) => this.update(time));
    this.performance.start();
    this.draw();
    this.performance.stop();
    this.performance.draw(this.ctx, this.canvas);
  }

  private addEventListeners(): void {
    window.addEventListener("resize", this.resizeCanvas.bind(this));
  }
}
