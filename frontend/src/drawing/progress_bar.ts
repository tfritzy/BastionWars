import { Layer } from "../constants";
import type { Drawing, DrawStyle } from "../drawing";

function drawRoundedRect(
 ctx: CanvasRenderingContext2D,
 x: number,
 y: number,
 width: number,
 height: number,
 radius: number
) {
 ctx.moveTo(x + radius, y); // Move to the top-left corner, offset by radius
 ctx.lineTo(x + width - radius, y); // Draw a line to the top-right corner
 ctx.quadraticCurveTo(x + width, y, x + width, y + radius); // Draw the top-right rounded corner
 ctx.lineTo(x + width, y + height - radius); // Line down to bottom-right corner
 ctx.quadraticCurveTo(x + width, y + height, x + width - radius, y + height); // Bottom-right corner
 ctx.lineTo(x + radius, y + height); // Line to the bottom-left corner
 ctx.quadraticCurveTo(x, y + height, x, y + height - radius); // Bottom-left corner
 ctx.lineTo(x, y + radius); // Line up to the top-left corner
 ctx.quadraticCurveTo(x, y, x + radius, y); // Top-left rounded corner
 ctx.closePath();
}

const filledPart: DrawStyle = {
 layer: Layer.UI,
 fill_style: "#333333",
 should_fill: true,
};
const unFilledPart: DrawStyle = {
 layer: Layer.UI,
 stroke_style: "#333333",
 should_stroke: true,
 line_width: 0.5,
 //  fill_style: "white",
 //  should_fill: true,
};
export function drawProgressBar(
 drawing: Drawing,
 x: number,
 y: number,
 width: number,
 height: number,
 radius: number,
 progress: number
) {
 if (progress > 0) {
  drawing.drawCustom(filledPart, (ctx) => {
   ctx.roundRect(x, y, width * progress, height, radius);
  });
 }

 drawing.drawCustom(unFilledPart, (ctx) => {
  ctx.roundRect(x, y, width, height, radius);
 });
}
