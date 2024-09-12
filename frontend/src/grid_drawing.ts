import { RenderTileType } from "./Schema.ts";

export function drawLandTile(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  size: number,
  radius: number,
  tileType: RenderTileType
) {
  const corners = interpretTileType(tileType);
  const landCount = corners.filter(Boolean).length;

  ctx.lineWidth = 1;
  ctx.fillStyle = "black";

  switch (landCount) {
    case 0:
      // Full water, do nothing
      break;
    case 1:
      drawOneLandCorner(ctx, x, y, size, corners);
      break;
    case 2:
      if (isAdjacentCorners(corners)) {
        drawTwoAdjacentLandCorners(ctx, x, y, size, corners);
      } else {
        drawTwoOppositeLandCorners(ctx, x, y, size, radius, corners);
      }
      break;
    case 3:
      drawThreeLandCorners(ctx, x, y, size, radius, corners);
      break;
    case 4:
      // ctx.beginPath();
      // ctx.rect(x, y, size, size);
      // ctx.stroke();
      break;
  }
}

export function drawOneLandCorner(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  size: number,
  corners: boolean[]
) {
  const cornerIndex = corners.indexOf(true);
  const halfSize = size / 2;

  ctx.beginPath();

  switch (cornerIndex) {
    case 0: // Top-left corner
      ctx.moveTo(x, y + halfSize);
      ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y);
      // ctx.lineTo(x, y);
      // ctx.lineTo(x, y + halfSize);
      break;
    case 1: // Top-right corner
      ctx.moveTo(x + halfSize, y);
      ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + size, y + halfSize);
      // ctx.lineTo(x + size, y);
      // ctx.lineTo(x + halfSize, y);
      break;
    case 2: // Bottom-left corner
      ctx.moveTo(x + halfSize, y + size);
      ctx.quadraticCurveTo(x + halfSize, y + halfSize, x, y + halfSize);
      // ctx.lineTo(x, y + size);
      // ctx.lineTo(x + halfSize, y + size);
      break;
    case 3: // Bottom-right corner
      ctx.moveTo(x + size, y + halfSize);
      ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y + size);
      // ctx.lineTo(x + size, y + size);
      // ctx.lineTo(x + size, y + halfSize);
      break;
  }

  ctx.stroke();
}

export function drawTwoAdjacentLandCorners(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  size: number,
  corners: boolean[]
) {
  const halfSize = size / 2;
  ctx.beginPath();

  if (corners[0] && corners[1]) {
    // Top half
    ctx.moveTo(x, y + halfSize);
    ctx.lineTo(x + size, y + halfSize);
  } else if (corners[1] && corners[3]) {
    // Right half
    ctx.moveTo(x + halfSize, y);
    ctx.lineTo(x + halfSize, y + size);
  } else if (corners[2] && corners[3]) {
    // Bottom half
    ctx.moveTo(x, y + halfSize);
    ctx.lineTo(x + size, y + halfSize);
  } else if (corners[0] && corners[2]) {
    // Left half
    ctx.moveTo(x + halfSize, y);
    ctx.lineTo(x + halfSize, y + size);
  }

  ctx.stroke();
}

export function drawTwoOppositeLandCorners(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  size: number,
  radius: number,
  corners: boolean[]
) {
  const halfSize = size / 2;
  ctx.beginPath();

  corners.forEach((isLand, i) => {
    if (isLand) {
      switch (i) {
        case 0:
          ctx.moveTo(x + halfSize, y);
          ctx.quadraticCurveTo(
            x + halfSize,
            y + halfSize,
            x + size,
            y + halfSize
          );
          break;
        case 1:
          ctx.moveTo(x + size, y + halfSize);
          ctx.quadraticCurveTo(
            x + halfSize,
            y + halfSize,
            x + halfSize,
            y + size
          );
          break;
        case 2:
          ctx.moveTo(x, y + halfSize);
          ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y);
          break;
        case 3:
          ctx.moveTo(x, y + halfSize);
          ctx.quadraticCurveTo(
            x + halfSize,
            y + halfSize,
            x + halfSize,
            y + size
          );
          break;
      }
    }
  });

  ctx.stroke();
}

export function drawThreeLandCorners(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  size: number,
  radius: number,
  corners: boolean[]
) {
  const halfSize = size / 2;
  const waterCorner = corners.indexOf(false);
  const [cx, cy] = getCornerCoordinates(x, y, size, waterCorner);
  ctx.beginPath();
  switch (waterCorner) {
    case 0:
      ctx.moveTo(x + halfSize, y);
      ctx.quadraticCurveTo(x + halfSize, y + halfSize, x, y + halfSize);
      // ctx.lineTo(x, y + size);
      // ctx.lineTo(x + size, y + size);
      // ctx.lineTo(x + size, y);
      break;
    case 1:
      ctx.moveTo(cx, cy + halfSize);
      ctx.quadraticCurveTo(cx - halfSize, cy + halfSize, cx - halfSize, cy);
      // ctx.lineTo(cx - size, cy);
      // ctx.lineTo(cx - size, cy + size);
      // ctx.lineTo(cx, cy + size);
      break;
    case 2:
      ctx.moveTo(x, y + halfSize);
      ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y + size);
      // ctx.lineTo(x + size, y + size);
      // ctx.lineTo(x + size, y);
      // ctx.lineTo(x, y);
      // ctx.lineTo(x, y + halfSize);
      break;
    case 3:
      ctx.moveTo(x + size, y + halfSize);
      ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y + size);
      // ctx.lineTo(x, y + size);
      // ctx.lineTo(x, y);
      // ctx.lineTo(x + size, y);
      // ctx.lineTo(x + size, y + halfSize);
      break;
  }
  ctx.stroke();
}

export function getCornerCoordinates(
  x: number,
  y: number,
  size: number,
  corner: number
): [number, number] {
  switch (corner) {
    case 0:
      return [x, y];
    case 1:
      return [x + size, y];
    case 2:
      return [x + size, y + size];
    case 3:
      return [x, y + size];
    default:
      throw "F***";
  }
}

export function isAdjacentCorners(corners: boolean[]): boolean {
  return (
    (corners[0] && corners[1]) ||
    (corners[1] && corners[3]) ||
    (corners[3] && corners[2]) ||
    (corners[2] && corners[0])
  );
}

export function interpretTileType(tileType: RenderTileType): boolean[] {
  switch (tileType) {
    case RenderTileType.FullWater:
      return [false, false, false, false];
    case RenderTileType.L_1000:
      return [true, false, false, false];
    case RenderTileType.L_0100:
      return [false, true, false, false];
    case RenderTileType.L_1100:
      return [true, true, false, false];
    case RenderTileType.L_0010:
      return [false, false, true, false];
    case RenderTileType.L_1010:
      return [true, false, true, false];
    case RenderTileType.L_0110:
      return [false, true, true, false];
    case RenderTileType.L_1110:
      return [true, true, true, false];
    case RenderTileType.L_0001:
      return [false, false, false, true];
    case RenderTileType.L_1001:
      return [true, false, false, true];
    case RenderTileType.L_0101:
      return [false, true, false, true];
    case RenderTileType.L_1101:
      return [true, true, false, true];
    case RenderTileType.L_0011:
      return [false, false, true, true];
    case RenderTileType.L_1011:
      return [true, false, true, true];
    case RenderTileType.L_0111:
      return [false, true, true, true];
    case RenderTileType.FullLand:
      return [true, true, true, true];
    default:
      return [false, false, false, false];
  }
}
