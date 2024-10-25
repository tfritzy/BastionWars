import { Layer } from "./constants";
import type { Drawing, DrawStyle } from "./drawing";
import { drawDots } from "./drawing/progress_dots";
import { type Vector2 } from "./types";

const letterRect: DrawStyle = {
  layer: Layer.UI,
  fill_style: "white",
  should_fill: true,
  should_stroke: true,
  stroke_style: "#444444",
  line_width: 0.5,
};

const style: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#444444",
  font: "bold 12px Verdena sans-serif",
  should_fill: true,
  text_align: "center",
};

const keyRectSize = 18;

export class ResourceLabel {
  public text: string;
  public progress: number;
  private lerpedProgress: number;
  private boundHandleKeyDown: (event: KeyboardEvent) => void;
  private onCharComplete: () => void;
  private drawing: Drawing;

  constructor(
    text: string,
    drawing: Drawing,
    keepPos: Vector2,
    onComplete: () => void,
    progress: number
  ) {
    this.text = text.toUpperCase();
    this.progress = progress;
    this.lerpedProgress = 0;
    this.drawing = drawing;
    this.onCharComplete = onComplete;

    this.boundHandleKeyDown = this.handleKeyDown.bind(this);
    document.addEventListener("keydown", this.boundHandleKeyDown);
  }

  draw(x: number, y: number, deltaTime: number): void {
    if (this.lerpedProgress < this.progress) {
      this.lerpedProgress += deltaTime * 5;
    }

    // Letter rect
    this.drawing.drawCustom(letterRect, (ctx) => {
      ctx.roundRect(
        x - keyRectSize / 2,
        y - keyRectSize / 2 - 4,
        keyRectSize,
        keyRectSize,
        2
      );
    });

    // Letter
    if (this.progress < this.text.length) {
      this.drawing.drawCustom(style, (ctx) => {
        ctx.fillText(this.text[this.progress], x, y);
      });
    }

    drawDots(this.drawing, x, y + 10, this.progress, this.text.length);
  }

  public removeEventListener(): void {
    document.removeEventListener("keydown", this.boundHandleKeyDown);
  }

  private handleKeyDown(event: KeyboardEvent) {
    if (this.progress >= this.text.length) {
      this.removeEventListener();
      return;
    }

    if (this.text[this.progress].toLowerCase() === event.key.toLowerCase()) {
      this.progress += 1;
      this.onCharComplete();

      if (this.progress == this.text.length) {
        this.removeEventListener();
      }
    }
  }
}
