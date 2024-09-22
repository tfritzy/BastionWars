import {
  HALF_T,
  KEEP_FILL_STYLE,
  KEEP_LINE_STYLE,
  KEEP_LINE_WIDTH,
  soldierColors,
  WORLD_TO_CANVAS,
} from "./constants";
import type { CanvasControls } from "./controls";
import type { Drawing } from "./drawing";
import { SoldierType } from "./Schema";
import type { Keep } from "./types";

export function drawPentagon(
  ctx: CanvasRenderingContext2D,
  centerX: number,
  centerY: number,
  radius: number,
  fillStyle: string | CanvasGradient | CanvasPattern
): void {
  ctx.fillStyle = fillStyle;
  ctx.beginPath();
  for (let i = 0; i < 5; i++) {
    const angle = (i * 2 * Math.PI) / 5 - Math.PI / 2;
    const x = centerX + Math.cos(angle) * radius;
    const y = centerY + Math.sin(angle) * radius;
    if (i === 0) {
      ctx.moveTo(x, y);
    } else {
      ctx.lineTo(x, y);
    }
  }
  ctx.closePath();
  ctx.fill();
}

export function drawGrid(
  ctx: CanvasRenderingContext2D,
  controls: CanvasControls,
  width: number,
  height: number
) {
  const { offsetX, offsetY } = controls.getTransform();
  ctx.fillStyle = "#e6eeed";
  const gridSize = 20;
  const roundedX = Math.trunc(offsetX / gridSize) * gridSize;
  const roundedY = Math.trunc(offsetY / gridSize) * gridSize;
  for (let x = -roundedX; x < width - roundedX; x += gridSize) {
    for (let y = -roundedY; y < height - roundedY; y += gridSize) {
      ctx.beginPath();
      ctx.arc(x, y, 1, 0, Math.PI * 2);
      ctx.fill();
    }
  }
}

function drawRoundedRect(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  width: number,
  height: number,
  radius: number
) {
  ctx.moveTo(x + radius, y);
  ctx.arcTo(x + width, y, x + width, y + height, radius);
  ctx.arcTo(x + width, y + height, x, y + height, radius);
  ctx.arcTo(x, y + height, x, y, radius);
  ctx.arcTo(x, y, x + width, y, radius);
}

const KEEP_RADIUS = 20;
const KEEP_INNER_RADIUS = KEEP_RADIUS * 0.8;
const KEEP_MIDDLE_RADIUS =
  KEEP_INNER_RADIUS + (KEEP_RADIUS - KEEP_INNER_RADIUS) / 2;
const PIE_RADIUS = KEEP_INNER_RADIUS;
const BLOCK_COUNT = 8;
const BLOCK_RADIUS = 4;
function drawBlockRing(drawing: Drawing, x: number, y: number, radius: number) {
  const angleStep = (2 * Math.PI) / BLOCK_COUNT;

  let angle = angleStep;
  for (let i = 0; i < BLOCK_COUNT; i++) {
    const endAngle = i * angleStep;
    const blockX = x + Math.cos(angle) * KEEP_MIDDLE_RADIUS;
    const blockY = y + Math.sin(angle) * KEEP_MIDDLE_RADIUS;
    const blockEndX = x + Math.cos(endAngle) * KEEP_MIDDLE_RADIUS;
    const blockEndY = y + Math.sin(endAngle) * KEEP_MIDDLE_RADIUS;
    angle = endAngle;

    drawing.drawStrokeable(KEEP_LINE_STYLE, KEEP_LINE_WIDTH, (ctx) => {
      ctx.moveTo(blockX, y);
      ctx.quadraticCurveTo(x, y, blockEndX, blockEndY);
    });
    drawing.drawFillable(KEEP_FILL_STYLE, (ctx) => {
      ctx.moveTo(blockX + BLOCK_RADIUS, y);
      ctx.arc(blockX, blockY, BLOCK_RADIUS, 0, 2 * Math.PI);
    });
  }
}

export function drawKeep(drawing: Drawing, keep: Keep, deltaTime: number) {
  const x = Math.round(keep.pos.x * WORLD_TO_CANVAS);
  const y = Math.round(keep.pos.y * WORLD_TO_CANVAS);

  drawBlockRing(drawing, x, y, KEEP_RADIUS);

  // // Draw main keep circle
  // drawing.drawStrokeable(KEEP_LINE_STYLE, KEEP_LINE_WIDTH, (ctx) => {
  //   ctx.moveTo(x + KEEP_RADIUS, y);
  //   ctx.arc(x, y, KEEP_RADIUS, 0, 2 * Math.PI);
  // });

  // // Draw inner keep circle
  // drawing.drawStrokeable(KEEP_LINE_STYLE, KEEP_LINE_WIDTH, (ctx) => {
  //   ctx.moveTo(x + KEEP_INNER_RADIUS, y);
  //   ctx.arc(x, y, KEEP_INNER_RADIUS, 0, 2 * Math.PI);
  // });

  // Draw block ring

  // Draw pie chart (assuming this function exists)
  drawPieChart(drawing, x, y, PIE_RADIUS, keep);

  // Draw total count
  const totalCount = (keep.archer_count + keep.warrior_count).toString();
  drawing.drawText("#4a4b5b", "center", (ctx) => {
    ctx.fillText(totalCount, x, y + 4);
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
  drawArc(
    drawing,
    x,
    y,
    radius,
    angle,
    slice,
    soldierColors[SoldierType.Warrior]
  );
  angle += slice;
  slice = (keep.archer_count / totalSoldiers) * 2 * Math.PI;
  drawArc(
    drawing,
    x,
    y,
    radius,
    angle,
    slice,
    soldierColors[SoldierType.Archer]
  );
}

export function drawArc(
  drawing: Drawing,
  x: number,
  y: number,
  radius: number,
  startAngle: number,
  sliceAngle: number,
  color: string
) {
  drawing.drawFillable(color, (ctx) => {
    ctx.moveTo(x, y);
    ctx.arc(x, y, radius, startAngle, startAngle + sliceAngle);
  });
}
