import { Layer } from "./constants";
import type { Drawing, DrawStyle } from "./drawing";
import { drawProgressBar } from "./drawing/progress_bar";
import { drawWheat } from "./drawing/wheat";

const style: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#444444",
  font: "bold 12px Verdena",
  should_fill: true,
  text_align: "center",
};

const borderStyle: DrawStyle = {
  layer: Layer.UI,
  stroke_style: "#444444",
  should_stroke: true,
  line_width: 1,
};

const width = 35;
const height = 40;
const padding = 4;

export class ResourceLabel {
  private text: string;
  private progress: number;
  private boundHandleKeyDown: (event: KeyboardEvent) => void;
  private onComplete: () => void;
  private drawing: Drawing;

  constructor(text: string, drawing: Drawing, onComplete: () => void) {
    this.text = text.toUpperCase();
    this.progress = 0;
    this.drawing = drawing;
    this.onComplete = onComplete;

    this.boundHandleKeyDown = this.handleKeyDown.bind(this);
    document.addEventListener("keydown", this.boundHandleKeyDown);
  }

  draw(x: number, y: number, deltaTime: number): void {
    this.drawing.drawCustom(style, (ctx) => {
      ctx.fillText(this.text[this.progress], x, y);
    });

    const progress = this.progress / this.text.length;
    drawProgressBar(
      this.drawing,
      x - width / 2 + padding,
      y + 8,
      width - padding * 2,
      5,
      2.75,
      progress
    );
  }

  public removeEventListener(): void {
    document.removeEventListener("keydown", this.boundHandleKeyDown);
  }

  private handleKeyDown(event: KeyboardEvent) {
    if (this.text[this.progress].toLowerCase() === event.key.toLowerCase()) {
      this.progress += 1;

      if (this.progress == this.text.length) {
        this.onComplete();
        this.progress = 0;
      }
    }
  }
}
