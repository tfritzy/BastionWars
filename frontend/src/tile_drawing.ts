import {
  HALF_T,
  FULL_T,
  QUARTER_T,
  BOUNDARY_LINE_STYLE,
  BOUNDARY_LINE_WIDTH,
  LAND_LINE_STYLE,
  LAND_LINE_WIDTH,
  THREE_Q_T,
} from "./constants";
import type { Drawing } from "./drawing";
import type { Vector2 } from "./types";

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
  drawing.drawFillable(owner1_color, (ctx) => {
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
  drawing.drawStrokeable(BOUNDARY_LINE_STYLE, BOUNDARY_LINE_WIDTH, (ctx) => {
    const p0 = rotate(p, { x: p.x, y: p.y + HALF_T }, rotation);
    ctx.moveTo(land_mid.x, land_mid.y);
    ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p0.x, p0.y);
  });

  // bottom owner area
  drawing.drawFillable(owner2_color, (ctx) => {
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
  drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, (ctx) => {
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
  drawing.drawFillable(owner1_color, (ctx) => {
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
  drawing.drawStrokeable(BOUNDARY_LINE_STYLE, BOUNDARY_LINE_WIDTH, (ctx) => {
    const p0 = rotate(p, { x: p.x + HALF_T, y: p.y + FULL_T }, rotation);
    ctx.moveTo(land_mid.x, land_mid.y);
    ctx.quadraticCurveTo(p.x + HALF_T, p.y + HALF_T, p0.x, p0.y);
  });

  // bottom right owner area
  drawing.drawFillable(owner2_color, (ctx) => {
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
  drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, (ctx) => {
    const p0 = rotate(p, { x: p.x + HALF_T, y: p.y }, rotation);
    const p1 = rotate(p, { x: p.x + FULL_T, y: p.y + HALF_T }, rotation);
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
      [x, y] = [y, -x];
      break;
    case 2: // 180 degrees
      [x, y] = [-x, -y];
      break;
    case 3: // 270 degrees clockwise (or 90 degrees counterclockwise)
      [x, y] = [-y, x];
      break;
  }

  // Translate back to tile coordinates
  return {
    x: x + tile_top_left.x + HALF_T,
    y: y + tile_top_left.y + HALF_T,
  };
}
