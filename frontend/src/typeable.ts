import type { Drawing } from "./drawing";

const shakeDampenPerS = 15;

export class Typeable {
  private text: string;
  private progress: number;
  private boundHandleKeyDown: (event: KeyboardEvent) => void;
  private onComplete: () => void;
  private shakeMagnitude: number;
  private drawing: Drawing;
  private font: string;

  constructor(
    text: string,
    font: string,
    onComplete: () => void,
    drawing: Drawing
  ) {
    this.text = text;
    this.progress = 0;
    this.drawing = drawing;
    this.onComplete = onComplete;
    this.shakeMagnitude = 0;
    this.font = font;

    this.boundHandleKeyDown = this.handleKeyDown.bind(this);
    document.addEventListener("keydown", this.boundHandleKeyDown);
  }

  draw(x: number, y: number, deltaTime: number): void {
    const completedPart = this.text.substring(0, this.progress);

    this.drawing.drawText("#4a4b5b", "start", this.font, (ctx) => {
      const width = ctx.measureText(this.text).width;
      const baseX = x + Math.random() * this.shakeMagnitude - width / 2;
      const baseY = y + Math.random() * this.shakeMagnitude;

      ctx.fillText(completedPart, baseX, baseY);
    });
    this.drawing.drawText("#4a4b5b88", "start", this.font, (ctx) => {
      const width = ctx.measureText(this.text).width;
      const baseX = x + Math.random() * this.shakeMagnitude - width / 2;
      const baseY = y + Math.random() * this.shakeMagnitude;
      ctx.fillText(
        this.text.substring(this.progress),
        baseX + ctx.measureText(completedPart).width,
        baseY
      );
    });

    if (this.shakeMagnitude > 0) {
      this.shakeMagnitude -= shakeDampenPerS * deltaTime;
      if (this.shakeMagnitude <= 0) {
        this.shakeMagnitude = 0;
      }
    }
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
