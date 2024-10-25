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
 font: "mono 14px Roboto",
 should_fill: true,
 text_align: "center",
};

const keyRectSize = 18;

export class ResourceLabel {
 public text: string;
 private drawing: Drawing;

 constructor(
  text: string,
  drawing: Drawing,
  keepPos: Vector2,
  onComplete: () => void,
  progress: number
 ) {
  this.text = text.toUpperCase();
  this.drawing = drawing;
 }

 draw(x: number, y: number, deltaTime: number): void {
  // Letter rect
  // this.drawing.drawCustom(letterRect, (ctx) => {
  //  ctx.roundRect(
  //   x - keyRectSize / 2 - 5,
  //   y - keyRectSize / 2 - 4,
  //   5 * this.text.length + 10,
  //   keyRectSize,
  //   2
  //  );
  // });

  // Letter
  this.drawing.drawCustom(style, (ctx) => {
   ctx.fillText(this.text, x, y);
  });

  // drawDots(this.drawing, x, y + 10, 0, this.text.length);
 }
}
