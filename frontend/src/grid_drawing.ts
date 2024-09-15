import {
  keepColors,
  QUARTER_T,
  TILE_SIZE,
  WORLD_TO_CANVAS,
} from "./constants.ts";
import { RenderAllianceCase, type RenderTile } from "./Schema.ts";
import type { GameState } from "./types.ts";

export function drawMap(ctx: CanvasRenderingContext2D, gameState: GameState) {
  ctx.save();

  gameState.renderTiles.forEach((tile, index) => {
    const x = (index % (gameState.mapWidth + 1)) * TILE_SIZE - TILE_SIZE / 2;
    const y =
      Math.floor(index / (gameState.mapWidth + 1)) * TILE_SIZE - TILE_SIZE / 2;

    drawLandTile(ctx, x, y, tile);
  });

  ctx.fill();
  ctx.restore();
}

export function drawLandTile(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  tile: RenderTile
) {
  ctx.lineWidth = 1;
  ctx.fillStyle = "black";

  switch (tile.alliance_case) {
    case RenderAllianceCase.FullLand_IndividualCorners:
      renderFullLandIndividualCorners(ctx, x, y, tile);
      break;
    case RenderAllianceCase.FullLand_OneOwner:
      renderFullLandOneOwner(ctx, x, y, tile);
      break;
    case RenderAllianceCase.FullLand_SplitDownMiddle:
      renderFullLandSplitDownMiddle();
      break;
    case RenderAllianceCase.FullLand_SingleRoundedCorner:
      renderFullLandSingleRoundedCorner();
      break;

    case RenderAllianceCase.ThreeCorners_OneOwner:
      renderThreeCornersOneOwner();
      break;
    case RenderAllianceCase.ThreeCorners_TwoOwners:
      renderThreeCornersTwoOwners();
      break;
    case RenderAllianceCase.ThreeCorners_ThreeOwners:
      renderThreeCornersThreeOwners();
      break;

    case RenderAllianceCase.TwoAdjacent_TwoOwners:
      renderTwoAdjacentTwoOwners();
      break;
    case RenderAllianceCase.TwoAdjacent_OneOwner:
      renderTwoAdjacentOneOwner();
      break;

    case RenderAllianceCase.TwoOpposite_OneOwner:
      renderTwoOppositeOneOwner();
      break;
    case RenderAllianceCase.TwoOpposite_TwoOwners:
      renderTwoOppositeTwoOwners();
      break;

    case RenderAllianceCase.SingleCorner_OneOwner:
      renderSingleCornerOneOwner();
      break;

    case RenderAllianceCase.FullWater_NoOnwer:
      renderFullWater();
      break;
    default:
      throw "Unknown RenderAllianceCase: " + tile.alliance_case;
  }
}

function renderFullLandIndividualCorners(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;
  setColorForAlliance(ctx, tile.corner_alliance[0]);
  ctx.rect(x, y, QUARTER_T, QUARTER_T);
  setColorForAlliance(ctx, tile.corner_alliance[1]);
  ctx.rect(x + QUARTER_T, y, QUARTER_T, QUARTER_T);
  setColorForAlliance(ctx, tile.corner_alliance[2]);
  ctx.rect(x, y + QUARTER_T, QUARTER_T, QUARTER_T);
  setColorForAlliance(ctx, tile.corner_alliance[3]);
  ctx.rect(x + QUARTER_T, y + QUARTER_T, QUARTER_T, QUARTER_T);
  // ctx.fill();
}
function renderFullLandOneOwner(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;
  setColorForAlliance(ctx, tile.corner_alliance[0]);
  ctx.rect(x, y, TILE_SIZE, TILE_SIZE);
  // ctx.fill();
}
function renderFullLandSplitDownMiddle() {}
function renderFullLandSingleRoundedCorner() {}

function renderThreeCornersOneOwner() {}
function renderThreeCornersTwoOwners() {}
function renderThreeCornersThreeOwners() {}

function renderTwoAdjacentOneOwner() {}
function renderTwoAdjacentTwoOwners() {}

function renderTwoOppositeOneOwner() {}
function renderTwoOppositeTwoOwners() {}

function renderSingleCornerOneOwner() {}
function renderFullWater() {}

function setColorForAlliance(ctx: CanvasRenderingContext2D, alliance: number) {
  if (alliance != 0) {
    ctx.fillStyle = keepColors[alliance % keepColors.length];
  } else {
    ctx.fillStyle = "";
  }
}

// export function drawOneLandCorner(
//   ctx: CanvasRenderingContext2D,
//   x: number,
//   y: number,
//   size: number,
//   corners: boolean[]
// ) {
//   const cornerIndex = corners.indexOf(true);
//   const halfSize = size / 2;

//   ctx.beginPath();

//   switch (cornerIndex) {
//     case 0: // Top-left corner
//       ctx.moveTo(x, y + halfSize);
//       ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y);
//       // ctx.lineTo(x, y);
//       // ctx.lineTo(x, y + halfSize);
//       break;
//     case 1: // Top-right corner
//       ctx.moveTo(x + halfSize, y);
//       ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + size, y + halfSize);
//       // ctx.lineTo(x + size, y);
//       // ctx.lineTo(x + halfSize, y);
//       break;
//     case 2: // Bottom-left corner
//       ctx.moveTo(x + halfSize, y + size);
//       ctx.quadraticCurveTo(x + halfSize, y + halfSize, x, y + halfSize);
//       // ctx.lineTo(x, y + size);
//       // ctx.lineTo(x + halfSize, y + size);
//       break;
//     case 3: // Bottom-right corner
//       ctx.moveTo(x + size, y + halfSize);
//       ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y + size);
//       // ctx.lineTo(x + size, y + size);
//       // ctx.lineTo(x + size, y + halfSize);
//       break;
//   }

//   ctx.stroke();
// }

// export function drawTwoAdjacentLandCorners(
//   ctx: CanvasRenderingContext2D,
//   x: number,
//   y: number,
//   size: number,
//   corners: boolean[]
// ) {
//   const halfSize = size / 2;
//   ctx.beginPath();

//   if (corners[0] && corners[1]) {
//     // Top half
//     ctx.moveTo(x, y + halfSize);
//     ctx.lineTo(x + size, y + halfSize);
//   } else if (corners[1] && corners[3]) {
//     // Right half
//     ctx.moveTo(x + halfSize, y);
//     ctx.lineTo(x + halfSize, y + size);
//   } else if (corners[2] && corners[3]) {
//     // Bottom half
//     ctx.moveTo(x, y + halfSize);
//     ctx.lineTo(x + size, y + halfSize);
//   } else if (corners[0] && corners[2]) {
//     // Left half
//     ctx.moveTo(x + halfSize, y);
//     ctx.lineTo(x + halfSize, y + size);
//   }

//   ctx.stroke();
// }

// export function drawTwoOppositeLandCorners(
//   ctx: CanvasRenderingContext2D,
//   x: number,
//   y: number,
//   size: number,
//   radius: number,
//   corners: boolean[]
// ) {
//   const halfSize = size / 2;
//   ctx.beginPath();

//   corners.forEach((isLand, i) => {
//     if (isLand) {
//       switch (i) {
//         case 0:
//           ctx.moveTo(x + halfSize, y);
//           ctx.quadraticCurveTo(
//             x + halfSize,
//             y + halfSize,
//             x + size,
//             y + halfSize
//           );
//           break;
//         case 1:
//           ctx.moveTo(x + size, y + halfSize);
//           ctx.quadraticCurveTo(
//             x + halfSize,
//             y + halfSize,
//             x + halfSize,
//             y + size
//           );
//           break;
//         case 2:
//           ctx.moveTo(x, y + halfSize);
//           ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y);
//           break;
//         case 3:
//           ctx.moveTo(x, y + halfSize);
//           ctx.quadraticCurveTo(
//             x + halfSize,
//             y + halfSize,
//             x + halfSize,
//             y + size
//           );
//           break;
//       }
//     }
//   });

//   ctx.stroke();
// }

// export function drawThreeLandCorners(
//   ctx: CanvasRenderingContext2D,
//   x: number,
//   y: number,
//   size: number,
//   radius: number,
//   corners: boolean[]
// ) {
//   const halfSize = size / 2;
//   const waterCorner = corners.indexOf(false);
//   const [cx, cy] = getCornerCoordinates(x, y, size, waterCorner);
//   ctx.beginPath();
//   switch (waterCorner) {
//     case 0:
//       ctx.moveTo(x + halfSize, y);
//       ctx.quadraticCurveTo(x + halfSize, y + halfSize, x, y + halfSize);
//       // ctx.lineTo(x, y + size);
//       // ctx.lineTo(x + size, y + size);
//       // ctx.lineTo(x + size, y);
//       break;
//     case 1:
//       ctx.moveTo(cx, cy + halfSize);
//       ctx.quadraticCurveTo(cx - halfSize, cy + halfSize, cx - halfSize, cy);
//       // ctx.lineTo(cx - size, cy);
//       // ctx.lineTo(cx - size, cy + size);
//       // ctx.lineTo(cx, cy + size);
//       break;
//     case 2:
//       ctx.moveTo(x, y + halfSize);
//       ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y + size);
//       // ctx.lineTo(x + size, y + size);
//       // ctx.lineTo(x + size, y);
//       // ctx.lineTo(x, y);
//       // ctx.lineTo(x, y + halfSize);
//       break;
//     case 3:
//       ctx.moveTo(x + size, y + halfSize);
//       ctx.quadraticCurveTo(x + halfSize, y + halfSize, x + halfSize, y + size);
//       // ctx.lineTo(x, y + size);
//       // ctx.lineTo(x, y);
//       // ctx.lineTo(x + size, y);
//       // ctx.lineTo(x + size, y + halfSize);
//       break;
//   }
//   ctx.stroke();
// }

// export function drawFullLand(
//   ctx: CanvasRenderingContext2D,
//   x: number,
//   y: number,
//   size: number,
//   radius: number,
//   renderTile: RenderTile
// ) {
//   const quarterSize = size / 4;
//   ctx.beginPath();

//   if (renderTile.num_alliances == 1) {
//     // fill single color
//     ctx.fillStyle = keepColors[renderTile.corner_alliance![0]];
//     ctx.rect(x, y, size, size);
//   } else if (renderTile.num_alliances == 2) {
//     if (isSplitDownMiddleCase(renderTile)) {
//       // Split down middle.
//     } else {
//       // Single arching corner.
//     }
//   } else {
//     ctx.fillStyle = keepColors[renderTile.corner_alliance![0]];
//     ctx.rect(x, y, quarterSize, quarterSize);
//     ctx.fillStyle = keepColors[renderTile.corner_alliance![1]];
//     ctx.rect(x + quarterSize, y, quarterSize, quarterSize);
//     ctx.fillStyle = keepColors[renderTile.corner_alliance![2]];
//     ctx.rect(x, y + quarterSize, quarterSize, quarterSize);
//     ctx.fillStyle = keepColors[renderTile.corner_alliance![3]];
//     ctx.rect(x + quarterSize, y + quarterSize, quarterSize, quarterSize);
//   }

//   ctx.fill();
// }

// function isSplitDownMiddleCase(renderTile: RenderTile) {
//   if (!renderTile.corner_alliance) {
//     return false;
//   }

//   let count = 1;
//   for (let i = 1; i < renderTile.corner_alliance.length; i++) {
//     if (renderTile.corner_alliance[i] === renderTile.corner_alliance[0]) {
//       count += 1;
//     }
//   }

//   return count === 2;
// }

// function findOddCornerIndex(renderTile: RenderTile): number {
//   if (!renderTile.corner_alliance) {
//     return -1;
//   }

//   let startValue = renderTile.corner_alliance[0];
//   for (let i = 1; i < renderTile.corner_alliance.length; i++) {
//     if (renderTile.corner_alliance[i] != startValue) {
//       if (i == 1) {
//         return 0;
//       } else {
//         return i;
//       }
//     }
//   }

//   return -1;
// }

// export function getCornerCoordinates(
//   x: number,
//   y: number,
//   size: number,
//   corner: number
// ): [number, number] {
//   switch (corner) {
//     case 0:
//       return [x, y];
//     case 1:
//       return [x + size, y];
//     case 2:
//       return [x + size, y + size];
//     case 3:
//       return [x, y + size];
//     default:
//       throw "F***";
//   }
// }

// export function isAdjacentCorners(corners: boolean[]): boolean {
//   return (
//     (corners[0] && corners[1]) ||
//     (corners[1] && corners[3]) ||
//     (corners[3] && corners[2]) ||
//     (corners[2] && corners[0])
//   );
// }

// export function interpretTileType(tileType: RenderTile): boolean[] {
//   switch (tileType.tile_case) {
//     case 0:
//       return [false, false, false, false];
//     case 1:
//       return [false, false, false, true];
//     case 2:
//       return [false, false, true, false];
//     case 3:
//       return [false, false, true, true];
//     case 4:
//       return [false, true, false, false];
//     case 5:
//       return [false, true, false, true];
//     case 6:
//       return [false, true, true, false];
//     case 7:
//       return [false, true, true, true];
//     case 8:
//       return [true, false, false, false];
//     case 9:
//       return [true, false, false, true];
//     case 10:
//       return [true, false, true, false];
//     case 11:
//       return [true, false, true, true];
//     case 12:
//       return [true, true, false, false];
//     case 13:
//       return [true, true, false, true];
//     case 14:
//       return [true, true, true, false];
//     case 15:
//       return [true, true, true, true];
//     default:
//       return [false, false, false, false];
//   }
// }
