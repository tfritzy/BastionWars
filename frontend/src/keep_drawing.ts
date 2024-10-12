import {
 HALF_T,
 KEEP_LINE_STYLE,
 KEEP_LINE_WIDTH,
 Layer,
 SHADOW_COLOR,
 soldierColors,
 soldierOutlineColors,
 WORLD_TO_CANVAS,
} from "./constants";
import { drawCircle, type Drawing, type DrawStyle } from "./drawing";
import { SoldierType } from "./Schema";
import type { Keep } from "./types";

const KEEP_RADIUS = 20;
const KEEP_INNER_RADIUS = KEEP_RADIUS * 0.8;
const PIE_RADIUS = KEEP_RADIUS;
const SHADOW_OFFSET = 7;

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
   drawCircle(ctx, x - SHADOW_OFFSET, y + SHADOW_OFFSET, 24);
   ctx.fill();

   // center body
   ctx.beginPath();
   ctx.fillStyle = "#eeeeef";
   drawCircle(ctx, x, y, 25);
   ctx.fill();
   ctx.stroke();

   // arrow blockey thingies.
   ctx.beginPath();
   ctx.fillStyle = "white";
   ctx.moveTo(x, y);
   for (let i = 0; i < 10; i += 2) {
    ctx.arc(x, y, 25, stepSize * i, stepSize * (i + 1));
    ctx.closePath();
   }
   ctx.fill();
   ctx.stroke();

   // Center shadow area.
   ctx.beginPath();
   ctx.fillStyle = "#dddddd";
   ctx.strokeStyle = KEEP_LINE_STYLE;
   drawCircle(ctx, x, y, 20);
   ctx.fill();
   ctx.stroke();

   // ?
   ctx.beginPath();
   ctx.fillStyle = "white";
   drawCircle(ctx, x - 0.6, y + 0.6, 18);
   ctx.fill();
  }
 );

 drawing.drawCustom(archerBubbleStyle, (ctx) => {
  ctx.moveTo(x + keep.archer_count, y);
  ctx.arc(x, y, keep.archer_count, 0, Math.PI * 2);
 });
 drawing.drawCustom(warriorBubbleStyle, (ctx) => {
  ctx.moveTo(x + keep.warrior_count, y);
  ctx.arc(x, y, keep.warrior_count, 0, Math.PI * 2);
 });
}

export function drawPieChart(
 drawing: Drawing,
 x: number,
 y: number,
 radius: number,
 keep: Keep
) {
 let angle = 0;
 const totalSoldiers = keep.archer_count + keep.warrior_count;
 let slice = (keep.warrior_count / totalSoldiers) * 2 * Math.PI;
 drawArc(drawing, x, y, radius, angle, slice, soldierPieStyle);
 angle += slice;
 slice = (keep.archer_count / totalSoldiers) * 2 * Math.PI;
 drawArc(drawing, x, y, radius, angle, slice, archerPieStyle);
}

const soldierPieStyle: DrawStyle = {
 layer: Layer.UI,
 should_fill: true,
 fill_style: soldierColors[SoldierType.Warrior],
 stroke_style: soldierOutlineColors[SoldierType.Warrior],
 line_width: 0.5,
};

const archerPieStyle: DrawStyle = {
 layer: Layer.UI,
 should_fill: true,
 fill_style: soldierColors[SoldierType.Archer],
};
export function drawArc(
 drawing: Drawing,
 x: number,
 y: number,
 radius: number,
 startAngle: number,
 sliceAngle: number,
 style: DrawStyle
) {
 drawing.drawCustom(style, (ctx) => {
  ctx.moveTo(x, y);
  ctx.arc(x, y, radius, startAngle, startAngle + sliceAngle);
  ctx.closePath();
 });
}
