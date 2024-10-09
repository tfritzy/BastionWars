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

const keepShadowStyle: DrawStyle = {
 layer: Layer.KeepShadows,
 fill_style: SHADOW_COLOR,
 should_fill: true,
};

const keepBaseStyle: DrawStyle = {
 layer: Layer.KeepBase,
 fill_style: "#eeeeef",
 should_fill: true,
 should_stroke: true,
 stroke_style: KEEP_LINE_STYLE,
 line_width: KEEP_LINE_WIDTH,
};

const keepDrawStyle: DrawStyle = {
 layer: Layer.KeepTowers,
 fill_style: "white",
 should_fill: true,
 should_stroke: true,
 stroke_style: KEEP_LINE_STYLE,
 line_width: KEEP_LINE_WIDTH,
};

const keepCenterShadowStyle: DrawStyle = {
 layer: Layer.KeepCenter,
 fill_style: "#dddddd",
 should_fill: true,
 should_stroke: true,
 stroke_style: KEEP_LINE_STYLE,
 line_width: KEEP_LINE_WIDTH,
};

const keepCenterStyle: DrawStyle = {
 layer: Layer.KeepCenter,
 fill_style: "white",
 should_fill: true,
};

export function drawKeep(drawing: Drawing, keep: Keep, deltaTime: number) {
 const x = Math.round(keep.pos.x * WORLD_TO_CANVAS);
 const y = Math.round(keep.pos.y * WORLD_TO_CANVAS);
 const stepSize = Math.PI / 5;

 drawing.drawCustom(keepBaseStyle, (ctx) => {
  drawCircle(ctx, x, y, 25);
 });

 drawing.drawCustom(keepDrawStyle, (ctx) => {
  ctx.moveTo(x, y);
  for (let i = 0; i < 10; i += 2) {
   ctx.arc(x, y, 25, stepSize * i, stepSize * (i + 1));
   ctx.closePath();
  }
 });

 drawing.drawCustom(keepCenterShadowStyle, (ctx) => {
  drawCircle(ctx, x, y, 20);
 });

 drawing.drawCustom(keepCenterStyle, (ctx) => {
  drawCircle(ctx, x - 0.6, y + 0.6, 18);
 });

 drawing.drawCustom(keepShadowStyle, (ctx) => {
  drawCircle(ctx, x - SHADOW_OFFSET, y + SHADOW_OFFSET, 24);
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
