import { Layer } from "./constants";
import type { Drawing, DrawStyle } from "./drawing";
import { drawProgressBar } from "./drawing/progress_bar";
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
  font: "1.2em monospace",
  should_fill: true,
  text_align: "center",
};

export class ResourceLabel {
  public text: string;
  private drawing: Drawing;

  constructor(text: string, drawing: Drawing) {
    this.text = text;
    this.drawing = drawing;
  }

  draw(x: number, y: number, deltaTime: number): void {
    // Letter
    this.drawing.drawCustom(style, (ctx) => {
      ctx.fillText(this.text, x, y);
    });

    // drawDots(this.drawing, x, y + 10, 0, this.text.length);
  }
}
