const shakeDampenPerS = 15;

export class Typeable {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private text: string;
  private progress: number;
  private boundHandleKeyDown: (event: KeyboardEvent) => void;
  private onComplete: () => void;
  private shakeMagnitude: number;

  constructor(
    text: string,
    onComplete: () => void,
    canvas: HTMLCanvasElement,
    ctx: CanvasRenderingContext2D
  ) {
    this.text = text;
    this.canvas = canvas;
    this.ctx = ctx;
    this.progress = 0;
    this.onComplete = onComplete;
    this.shakeMagnitude = 0;

    this.boundHandleKeyDown = this.handleKeyDown.bind(this);
    document.addEventListener("keydown", this.boundHandleKeyDown);
  }

  draw(x: number, y: number, deltaTime: number): void {
    this.ctx.save();
    this.ctx.textAlign = "start";

    const width = this.ctx.measureText(this.text).width;
    const baseX = x + Math.random() * this.shakeMagnitude - width / 2;
    const baseY = y + Math.random() * this.shakeMagnitude;

    const completedPart = this.text.substring(0, this.progress);
    this.ctx.fillText(completedPart, baseX, baseY);
    this.ctx.fillStyle = "#4a4b5b55";
    this.ctx.fillText(
      this.text.substring(this.progress),
      baseX + this.ctx.measureText(completedPart).width,
      baseY
    );

    if (this.shakeMagnitude > 0) {
      this.shakeMagnitude -= shakeDampenPerS * deltaTime;
      if (this.shakeMagnitude <= 0) {
        this.shakeMagnitude = 0;
      }
    }

    this.ctx.restore();
  }

  public removeEventListener(): void {
    document.removeEventListener("keydown", this.boundHandleKeyDown);
  }

  private handleKeyDown(event: KeyboardEvent) {
    if (this.text[this.progress] === event.key) {
      this.progress += 1;

      if (this.progress == this.text.length) {
        this.onComplete();
        this.progress = 0;
      }
    } else {
      if (this.progress > 0) {
        this.shakeMagnitude = 3.5;
      }

      this.progress = 0;
    }
  }
}
