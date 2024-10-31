import {
 HALF_T,
 KEEP_LINE_STYLE,
 KEEP_LINE_WIDTH,
 Layer,
 SHADOW_COLOR,
 UNIT_AREA,
 WORLD_TO_CANVAS,
} from "./constants";
import { drawCircle, type Drawing, type DrawStyle } from "./drawing";
import type { Keep } from "./types";

const SHADOW_OFFSET = 4;

const keepLabel: DrawStyle = {
 layer: Layer.UI,
 fill_style: "#444444",
 should_fill: true,
 font: "1.2em monospace",
 text_align: "center",
};
const archerBubbleStyle: DrawStyle = {
 layer: Layer.UI,
 fill_style: "#d1fae5",
 stroke_style: "#064e3b",
 should_fill: true,
 should_stroke: true,
 line_width: 0.5,
};
const warriorBubbleStyle: DrawStyle = {
 layer: Layer.UI,
 fill_style: "#ffe4e6",
 stroke_style: "#881337",
 should_fill: true,
 should_stroke: true,
 line_width: 0.5,
};

export function drawKeep(drawing: Drawing, keep: Keep, deltaTime: number) {
 const x = Math.round(keep.pos.x * WORLD_TO_CANVAS);
 const y = Math.round(keep.pos.y * WORLD_TO_CANVAS);
 const stepSize = Math.PI / 5;

 drawing.drawWithOffscreen(
  "keep",
  x - HALF_T,
  y - HALF_T,
  Layer.Keep,
  (ctx: CanvasRenderingContext2D) => {
   const x = HALF_T;
   const y = HALF_T;
   ctx.strokeStyle = KEEP_LINE_STYLE;
   ctx.lineWidth = KEEP_LINE_WIDTH;

   // shadow
   ctx.beginPath();
   ctx.fillStyle = SHADOW_COLOR;
   drawCircle(ctx, x - SHADOW_OFFSET, y + SHADOW_OFFSET, 18);
   ctx.fill();

   // center body
   ctx.beginPath();
   ctx.fillStyle = "#eeeeef";
   drawCircle(ctx, x, y, 18.75);
   ctx.fill();
   ctx.stroke();

   // arrow blockey thingies.
   ctx.beginPath();
   ctx.fillStyle = "white";
   ctx.moveTo(x, y);
   for (let i = 0; i < 10; i += 2) {
    ctx.arc(x, y, 18.75, stepSize * i, stepSize * (i + 1));
    ctx.closePath();
   }
   ctx.fill();
   ctx.stroke();

   // Center shadow area.
   ctx.beginPath();
   ctx.fillStyle = "#dddddd";
   ctx.strokeStyle = KEEP_LINE_STYLE;
   drawCircle(ctx, x, y, 15);
   ctx.fill();
   ctx.stroke();

   // ?
   ctx.beginPath();
   ctx.fillStyle = "white";
   drawCircle(ctx, x - 0.6, y + 0.6, 13.5);
   ctx.fill();
  }
 );

 drawing.drawCustom(archerBubbleStyle, (ctx) => {
  const r = Math.sqrt((keep.archer_count * UNIT_AREA) / Math.PI);
  drawCircle(ctx, x, y, r);
 });
 drawing.drawCustom(warriorBubbleStyle, (ctx) => {
  const r = Math.sqrt((keep.warrior_count * UNIT_AREA) / Math.PI);
  drawCircle(ctx, x, y, r);
 });

 drawing.drawCustom(keepLabel, (ctx) => {
  ctx.fillText(keep.name, x, y - 22);
 });
}
