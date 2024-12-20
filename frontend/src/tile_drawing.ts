import {
 HALF_T,
 FULL_T,
 QUARTER_T,
 LAND_LINE_STYLE,
 LAND_LINE_WIDTH,
 THREE_Q_T,
 Layer,
} from "./constants";
import type { Drawing } from "./drawing";
import type { Vector2 } from "./types";

export function draw_land_1111_ownership_1222(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner1_color: string,
 owner2_color: string
) {
 drawing.drawFillable(owner1_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
 });

 drawing.drawFillable(owner2_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);
  const p3 = rotate(p, { x: p.x, y: p.y + FULL_T }, rotation);
  const p4 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p5 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.lineTo(p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
  ctx.lineTo(p4.x, p4.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p5.x, p5.y);
 });

 drawing.drawBoundary((ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
}

export function draw_land_0111_ownership_x111(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner_color: string
) {
 drawing.drawFillable(owner_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);
  const p3 = rotate(p, { x: p.x, y: p.y + FULL_T }, rotation);
  const p4 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p5 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.lineTo(p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
  ctx.lineTo(p4.x, p4.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p5.x, p5.y);
 });

 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
}

export function draw_land_0111_ownership_x121(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner1_color: string,
 owner2_color: string
) {
 // Curve of the land
 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });

 // Adjacent owner corner
 drawing.drawBoundary((ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
 drawing.drawFillable(owner1_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  const p2 = rotate(p, { x: p.x, y: p.y + FULL_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
  ctx.lineTo(p2.x, p2.y);
 });

 // Remaining ownership land
 drawing.drawFillable(owner2_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p2 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  const p3 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);
  const p4 = rotate(p, { x: p.x + FULL_T, y: p.y }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
  ctx.lineTo(p4.x, p4.y);
 });
}

export function draw_land_0111_ownership_x211(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner1_color: string,
 owner2_color: string
) {
 // Curve of the land
 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });

 // Adjacent owner corner
 drawing.drawBoundary((ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
 drawing.drawFillable(owner1_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
  ctx.lineTo(p2.x, p2.y);
 });

 // Remaining ownership land
 drawing.drawFillable(owner2_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const p3 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);
  const p4 = rotate(p, { x: p.x, y: p.y + FULL_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
  ctx.lineTo(p4.x, p4.y);
 });
}

export function draw_land_1101_ownership_12x1(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner1_color: string,
 owner2_color: string
) {
 // Curve of the land
 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });

 // top right owner area
 drawing.drawFillable(owner1_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + FULL_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
 });
 drawing.drawBoundary((ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });

 // Remaining area
 drawing.drawFillable(owner2_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const p3 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);
  const p4 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  const p5 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
  ctx.lineTo(p4.x, p4.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p5.x, p5.y);
 });
}

export function draw_land_1011_ownership_1x22(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner1_color: string,
 owner2_color: string
) {
 const land_mid = rotate(
  p,
  getCenterOfCurve(
   p.x + HALF_T,
   p.y,
   p.x + HALF_T,
   p.y + HALF_T,
   p.x + FULL_T,
   p.y + HALF_T
  ),
  rotation
 );

 // Top left owner area
 drawing.drawFillable(owner1_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = land_mid;
  const cp0 = rotate(p, { x: p.x + HALF_T, y: p.y + QUARTER_T }, rotation);
  const p3 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const cp1 = rotate(p, { x: p.x + HALF_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(cp0.x, cp0.y, p2.x, p2.y);
  ctx.quadraticCurveTo(cp1.x, cp1.y, p3.x, p3.y);
 });

 // bondary between owned lands
 drawing.drawBoundary((ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  ctx.moveTo(land_mid.x, land_mid.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p0.x, p0.y);
 });

 // bottom owner area
 drawing.drawFillable(owner2_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + FULL_T }, rotation);
  const p1 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p2 = land_mid;
  const p3 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const cp1 = rotate(p, { x: p.x + THREE_Q_T, y: p.y + HALF_T }, rotation);
  const p4 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
  ctx.quadraticCurveTo(cp1.x, cp1.y, p3.x, p3.y);
  ctx.lineTo(p4.x, p4.y);
 });

 // Curve of the land
 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
}

export function draw_land_1011_ownership_1x12(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner1_color: string,
 owner2_color: string
) {
 const land_mid = rotate(
  p,
  getCenterOfCurve(
   p.x + HALF_T,
   p.y,
   p.x + HALF_T,
   p.y + HALF_T,
   p.x + FULL_T,
   p.y + HALF_T
  ),
  rotation
 );

 // Top left owner area
 drawing.drawFillable(owner1_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = land_mid;
  const cp0 = rotate(p, { x: p.x + HALF_T, y: p.y + QUARTER_T }, rotation);
  const p3 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  const cp1 = rotate(p, { x: p.x + HALF_T, y: p.y + HALF_T }, rotation);
  const p4 = rotate(p, { x: p.x, y: p.y + FULL_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(cp0.x, cp0.y, p2.x, p2.y);
  ctx.quadraticCurveTo(cp1.x, cp1.y, p3.x, p3.y);
  ctx.lineTo(p4.x, p4.y);
 });

 // bondary between owned lands
 drawing.drawBoundary((ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  ctx.moveTo(land_mid.x, land_mid.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p0.x, p0.y);
 });

 // bottom right owner area
 drawing.drawFillable(owner2_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  const p2 = land_mid;
  const p3 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const cp1 = rotate(p, { x: p.x + THREE_Q_T, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
  ctx.quadraticCurveTo(cp1.x, cp1.y, p3.x, p3.y);
 });

 // Curve of the land
 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
}

export function draw_land_1011_ownership_1x23(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner1_color: string,
 owner2_color: string,
 owner3_color: string
) {
 const land_mid = rotate(
  p,
  getCenterOfCurve(
   p.x + HALF_T,
   p.y,
   p.x + HALF_T,
   p.y + HALF_T,
   p.x + FULL_T,
   p.y + HALF_T
  ),
  rotation
 );

 // Top left owner area
 drawing.drawFillable(owner1_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = land_mid;
  const cp0 = rotate(p, { x: p.x + HALF_T, y: p.y + QUARTER_T }, rotation);
  const p3 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const cp1 = rotate(p, { x: p.x + HALF_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(cp0.x, cp0.y, p2.x, p2.y);
  ctx.quadraticCurveTo(cp1.x, cp1.y, p3.x, p3.y);
 });

 // bondary between owned lands
 drawing.drawBoundary((ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  ctx.moveTo(land_mid.x, land_mid.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p0.x, p0.y);
 });

 // bondary between owned lands
 drawing.drawBoundary((ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  ctx.moveTo(land_mid.x, land_mid.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p0.x, p0.y);
 });

 // bottom right owner area
 drawing.drawFillable(owner3_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  const p2 = land_mid;
  const p3 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const cp1 = rotate(p, { x: p.x + THREE_Q_T, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
  ctx.quadraticCurveTo(cp1.x, cp1.y, p3.x, p3.y);
 });

 drawing.drawFillable(owner2_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = land_mid;
  const p2 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  const p3 = rotate(p, { x: p.x, y: p.y + FULL_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
 });

 // Curve of the land
 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
}

export function draw_land_1100_ownership_11xx(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner_color: string
) {
 drawing.drawFillable(owner_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const p3 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.lineTo(p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
 });

 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
 });
}

export function draw_land_1100_ownership_12xx(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner1_color: string,
 owner2_color: string
) {
 drawing.drawFillable(owner1_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x + HALF_T, y: p.y + HALF_T }, rotation);
  const p3 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.lineTo(p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
 });

 drawing.drawFillable(owner2_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const p3 = rotate(p, { x: p.x + HALF_T, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.lineTo(p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
 });

 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
 });

 drawing.drawBoundary((ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
 });
}

export function draw_land_1001_ownership_1xx1(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner_color: string
) {
 drawing.drawFillable(owner_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const p3 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);
  const p4 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  const p5 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
  ctx.lineTo(p4.x, p4.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p5.x, p5.y);
 });

 // Drawing land
 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });

 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
}

export function draw_land_1001_ownership_1xx2(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner1_color: string,
 owner2_color: string
) {
 const land_top_mid = rotate(
  p,
  getCenterOfCurve(
   p.x + HALF_T,
   p.y,
   p.x + HALF_T,
   p.y + HALF_T,
   p.x + FULL_T,
   p.y + HALF_T
  ),
  rotation
 );
 const land_bottom_mid = rotate(
  p,
  getCenterOfCurve(
   p.x,
   p.y + HALF_T,
   p.x + HALF_T,
   p.y + HALF_T,
   p.x + HALF_T,
   p.y + FULL_T
  ),
  rotation
 );

 // Top left part
 drawing.drawFillable(owner1_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = land_top_mid;
  const cp2 = rotate(p, { x: p.x + HALF_T, y: p.y + QUARTER_T }, rotation);
  const p3 = land_bottom_mid;
  const p4 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const cp3 = rotate(p, { x: p.x + QUARTER_T, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(cp2.x, cp2.y, p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
  ctx.quadraticCurveTo(cp3.x, cp3.y, p4.x, p4.y);
 });

 // bottom right part
 drawing.drawFillable(owner2_color, Layer.Map, (ctx) => {
  const p0 = land_top_mid;
  const cp1 = rotate(p, { x: p.x + THREE_Q_T, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  const p2 = rotate(p, { x: p.x + FULL_T, y: p.y + FULL_T }, rotation);
  const p3 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  const p4 = land_bottom_mid;
  const cp4 = rotate(p, { x: p.x + HALF_T, y: p.y + THREE_Q_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(cp1.x, cp1.y, p1.x, p1.y);
  ctx.lineTo(p2.x, p2.y);
  ctx.lineTo(p3.x, p3.y);
  ctx.quadraticCurveTo(cp4.x, cp4.y, p4.x, p4.y);
 });

 // bounds between
 drawing.drawBoundary((ctx) => {
  ctx.moveTo(land_top_mid.x, land_top_mid.y);
  ctx.lineTo(land_bottom_mid.x, land_bottom_mid.y);
 });

 // Drawing land
 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });

 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
}

export function draw_land_1000_ownership_1xxx(
 drawing: Drawing,
 p: Vector2,
 rotation: number,
 owner_color: string
) {
 drawing.drawFillable(owner_color, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p2 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.lineTo(p1.x, p1.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p2.x, p2.y);
 });

 drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, Layer.Map, (ctx) => {
  const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
  const p1 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);

  ctx.moveTo(p0.x, p0.y);
  ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p1.x, p1.y);
 });
}

function getCenterOfCurve(
 x0: number,
 y0: number,
 cpx: number,
 cpy: number,
 x1: number,
 y1: number
): Vector2 {
 const mx1 = (x0 + cpx) / 2;
 const my1 = (y0 + cpy) / 2;

 const mx2 = (cpx + x1) / 2;
 const my2 = (cpy + y1) / 2;

 const midX = (mx1 + mx2) / 2;
 const midY = (my1 + my2) / 2;

 return { x: midX, y: midY };
}

function rotate(tile_top_left: Vector2, point: Vector2, iterations: number) {
 // Normalize the point to the center of the tile
 let x = point.x - (tile_top_left.x + HALF_T);
 let y = point.y - (tile_top_left.y + HALF_T);

 // Calculate the number of 90-degree rotations (0 to 3)
 const rotations = ((iterations % 4) + 4) % 4;

 // Perform rotation based on the number of 90-degree turns
 switch (rotations) {
  case 0: // No rotation
   break;
  case 1: // 90 degrees clockwise
   [x, y] = [-y, x]; // Corrected for clockwise rotation
   break;
  case 2: // 180 degrees
   [x, y] = [-x, -y];
   break;
  case 3: // 270 degrees clockwise (or 90 degrees counterclockwise)
   [x, y] = [y, -x]; // Corrected for clockwise rotation
   break;
 }

 // Translate back to tile coordinates
 return {
  x: x + tile_top_left.x + HALF_T,
  y: y + tile_top_left.y + HALF_T,
 };
}
