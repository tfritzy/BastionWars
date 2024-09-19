import {
  BOUNDARY_LINE_STYLE,
  BOUNDARY_LINE_WIDTH,
  HALF_T,
  keepColors,
  QUARTER_T,
  LAND_LINE_STYLE,
  LAND_LINE_WIDTH,
  FULL_T,
} from "./constants.ts";
import type { Drawing } from "./drawing.ts";
import { RenderAllianceCase, type RenderTile } from "./Schema.ts";
import {
  draw_land_1011_ownership_1x12,
  draw_land_1011_ownership_1x22,
} from "./tile_drawing.ts";
import type { GameState } from "./types.ts";

type RectParams = {
  x: number;
  y: number;
  width: number;
  height: number;
};

type LineParams = {
  start_x: number;
  start_y: number;
  end_x: number;
  end_y: number;
};

type ArcParams = {
  p0_x: number;
  p0_y: number;
  p1_x: number;
  p1_y: number;
  cp_x: number;
  cp_y: number;
  p2_x: number;
  p2_y: number;
};

export function drawMap(drawing: Drawing, gameState: GameState) {
  gameState.renderTiles.forEach((tile, index) => {
    const x = Math.floor(
      (index % (gameState.mapWidth + 1)) * FULL_T - FULL_T / 2
    );
    const y = Math.floor(
      Math.floor(index / (gameState.mapWidth + 1)) * FULL_T - FULL_T / 2
    );

    drawLandTile(drawing, x, y, tile);
  });

  drawLandTile(drawing, -8 * FULL_T - FULL_T / 2, 11 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_OneOwner,
    corner_alliance: [0, 2, 1, 1],
    tile_case: 7,
  });
  drawLandTile(drawing, -8 * FULL_T - FULL_T / 2, 13 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [0, 2, 1, 1],
    tile_case: 7,
  });
  drawLandTile(drawing, -8 * FULL_T - FULL_T / 2, 15 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [0, 2, 2, 1],
    tile_case: 7,
  });

  drawLandTile(drawing, -6 * FULL_T - FULL_T / 2, 11 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_OneOwner,
    corner_alliance: [1, 1, 1, 1],
    tile_case: 11,
  });
  drawLandTile(drawing, -6 * FULL_T - FULL_T / 2, 13 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [2, 0, 1, 1],
    tile_case: 11,
  });
  drawLandTile(drawing, -6 * FULL_T - FULL_T / 2, 15 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [2, 0, 1, 2],
    tile_case: 11,
  });

  drawLandTile(drawing, -4 * FULL_T - FULL_T / 2, 11 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_OneOwner,
    corner_alliance: [1, 1, 0, 2],
    tile_case: 13,
  });
  drawLandTile(drawing, -4 * FULL_T - FULL_T / 2, 13 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [1, 1, 0, 2],
    tile_case: 13,
  });
  drawLandTile(drawing, -4 * FULL_T - FULL_T / 2, 15 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [2, 1, 0, 2],
    tile_case: 13,
  });

  drawLandTile(drawing, -2 * FULL_T - FULL_T / 2, 11 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_OneOwner,
    corner_alliance: [2, 1, 1, 0],
    tile_case: 14,
  });
  drawLandTile(drawing, -2 * FULL_T - FULL_T / 2, 13 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [2, 1, 1, 0],
    tile_case: 14,
  });
  drawLandTile(drawing, -2 * FULL_T - FULL_T / 2, 15 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [2, 2, 1, 0],
    tile_case: 14,
  });
}

export function drawLandTile(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  switch (tile.alliance_case) {
    case RenderAllianceCase.FullLand_IndividualCorners:
      renderFullLandIndividualCorners(drawing, x, y, tile);
      break;
    case RenderAllianceCase.FullLand_OneOwner:
      renderFullLandOneOwner(drawing, x, y, tile);
      break;
    case RenderAllianceCase.FullLand_SplitDownMiddle:
      renderFullLandSplitDownMiddle(drawing, x, y, tile);
      break;
    case RenderAllianceCase.FullLand_SingleRoundedCorner:
      renderFullLandSingleRoundedCorner(drawing, x, y, tile);
      break;

    case RenderAllianceCase.ThreeCorners_OneOwner:
      renderThreeCornersOneOwner(drawing, x, y, tile);
      break;
    case RenderAllianceCase.ThreeCorners_TwoOwners:
      renderThreeCornersTwoOwners(drawing, x, y, tile);
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
      console.log(tile);
      throw "Unknown RenderAllianceCase: " + tile.alliance_case;
  }

  // for (const [fill, rects] of rect_queue) {
  //   rects?.forEach((r) => {
  //     drawing.drawFillable(fill, () => {
  //       ctx.rect(r.x, r.y, r.width, r.height);
  //     });
  //   });
  // }

  // ctx.save();
  // ctx.lineWidth = BOUNDARY_LINE_WIDTH;
  // for (const [stroke, lines] of line_queue) {
  //   ctx.beginPath();
  //   ctx.strokeStyle = stroke;
  //   lines?.forEach((l) => {
  //     ctx.moveTo(l.start_x, l.start_y);
  //     ctx.lineTo(l.end_x, l.end_y);
  //   });
  //   ctx.stroke();

  //   // Clear list without garbage collecting.
  //   line_queue.get(stroke)!.length = 0;
  // }
  // ctx.restore();

  // ctx.save();
  // ctx.lineWidth = BOUNDARY_LINE_WIDTH;
  // ctx.strokeStyle = BOUNDARY_LINE_STYLE;
  // for (const [fill, arcs] of arc_queue) {
  //   ctx.fillStyle = fill;
  //   ctx.beginPath();
  //   arcs?.forEach((a) => {
  //     ctx.moveTo(a.p0_x, a.p0_y);
  //     ctx.lineTo(a.p1_x, a.p1_y);
  //     ctx.quadraticCurveTo(a.cp_x, a.cp_y, a.p2_x, a.p2_y);
  //   });
  //   ctx.fill();

  //   ctx.beginPath();
  //   arcs?.forEach((a) => {
  //     ctx.moveTo(a.p1_x, a.p1_y);
  //     ctx.quadraticCurveTo(a.cp_x, a.cp_y, a.p2_x, a.p2_y);
  //   });
  //   ctx.stroke();

  //   // Clear list without garbage collecting.
  //   arc_queue.get(fill)!.length = 0;
  // }
  // ctx.restore();
}

function renderFullLandIndividualCorners(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;
  drawing.drawFillable(styleForCorner(tile, 0), (ctx) =>
    ctx.rect(x, y, QUARTER_T, QUARTER_T)
  );
  drawing.drawFillable(styleForCorner(tile, 1), (ctx) =>
    ctx.rect(x + QUARTER_T, y, QUARTER_T, QUARTER_T)
  );
  drawing.drawFillable(styleForCorner(tile, 2), (ctx) =>
    ctx.rect(x, y + QUARTER_T, QUARTER_T, QUARTER_T)
  );
  drawing.drawFillable(styleForCorner(tile, 3), (ctx) =>
    ctx.rect(x + QUARTER_T, y + QUARTER_T, QUARTER_T, QUARTER_T)
  );
}

function renderFullLandOneOwner(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  drawing.drawFillable(styleForCorner(tile, 0), (ctx) =>
    ctx.rect(x, y, FULL_T, FULL_T)
  );
}

function renderFullLandSplitDownMiddle(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;
  const isHorizontal = tile.corner_alliance[0] === tile.corner_alliance[1];
  if (isHorizontal) {
    drawing.drawFillable(styleForCorner(tile, 0), (ctx) =>
      ctx.rect(x, y, FULL_T, HALF_T)
    );
    drawing.drawFillable(styleForCorner(tile, 2), (ctx) =>
      ctx.rect(x, y + HALF_T, FULL_T, HALF_T)
    );
    drawing.drawStrokeable(BOUNDARY_LINE_STYLE, BOUNDARY_LINE_WIDTH, (ctx) => {
      ctx.moveTo(x, y + HALF_T);
      ctx.lineTo(x + FULL_T, y + HALF_T);
    });
  } else {
    drawing.drawFillable(styleForCorner(tile, 0), (ctx) =>
      ctx.rect(x, y, HALF_T, FULL_T)
    );
    drawing.drawFillable(styleForCorner(tile, 1), (ctx) =>
      ctx.rect(x + HALF_T, y, HALF_T, FULL_T)
    );
    drawing.drawStrokeable(BOUNDARY_LINE_STYLE, BOUNDARY_LINE_WIDTH, (ctx) => {
      ctx.moveTo(x + HALF_T, y);
      ctx.lineTo(x + HALF_T, y + FULL_T);
    });
  }
}
function renderFullLandSingleRoundedCorner(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  drawing.drawFilledArc(
    x,
    y,
    x + HALF_T,
    y,
    x + HALF_T,
    y + HALF_T,
    x,
    y + HALF_T,
    styleForCorner(tile, 0),
    BOUNDARY_LINE_STYLE,
    BOUNDARY_LINE_WIDTH
  );
}

function renderThreeCornersOneOwner(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  switch (tile.tile_case) {
    case 7: // empty corner top left
      drawing.drawFillable(styleForCorner(tile, 1), (ctx) => {
        ctx.moveTo(x + HALF_T, y);
        ctx.lineTo(x + FULL_T, y);
        ctx.lineTo(x + FULL_T, y + FULL_T);
        ctx.lineTo(x, y + FULL_T);
        ctx.lineTo(x, y + HALF_T);
        ctx.quadraticCurveTo(x + HALF_T, y + HALF_T, x + HALF_T, y);
      });

      drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, (ctx) => {
        ctx.moveTo(x, y + HALF_T);
        ctx.quadraticCurveTo(x + HALF_T, y + HALF_T, x + HALF_T, y);
      });
      break;
    case 11: // empty corner top right
      drawing.drawFillable(styleForCorner(tile, 1), (ctx) => {
        ctx.moveTo(x + HALF_T, y);
        ctx.lineTo(x, y);
        ctx.lineTo(x, y + FULL_T);
        ctx.lineTo(x + FULL_T, y + FULL_T);
        ctx.lineTo(x + FULL_T, y + HALF_T);
        ctx.quadraticCurveTo(x + HALF_T, y + HALF_T, x + HALF_T, y);
      });

      drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, (ctx) => {
        ctx.moveTo(x + HALF_T, y);
        ctx.quadraticCurveTo(x + HALF_T, y + HALF_T, x + FULL_T, y + HALF_T);
      });
      break;
    case 13: // empty corner bottom left
      drawing.drawFillable(styleForCorner(tile, 1), (ctx) => {
        ctx.moveTo(x, y + HALF_T);
        ctx.lineTo(x, y);
        ctx.lineTo(x + FULL_T, y);
        ctx.lineTo(x + FULL_T, y + FULL_T);
        ctx.lineTo(x + HALF_T, y + FULL_T);
        ctx.quadraticCurveTo(x + HALF_T, y + HALF_T, x, y + HALF_T);
      });

      drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, (ctx) => {
        ctx.moveTo(x, y + HALF_T);
        ctx.quadraticCurveTo(x + HALF_T, y + HALF_T, x + HALF_T, y + FULL_T);
      });
      break;
    case 14: // empty corner bottom right
      drawing.drawFillable(styleForCorner(tile, 1), (ctx) => {
        ctx.moveTo(x + FULL_T, y + HALF_T);
        ctx.lineTo(x + FULL_T, y);
        ctx.lineTo(x, y);
        ctx.lineTo(x, y + FULL_T);
        ctx.lineTo(x + HALF_T, y + FULL_T);
        ctx.quadraticCurveTo(x + HALF_T, y + HALF_T, x + FULL_T, y + HALF_T);
      });

      drawing.drawStrokeable(LAND_LINE_STYLE, LAND_LINE_WIDTH, (ctx) => {
        ctx.moveTo(x + HALF_T, y + FULL_T);
        ctx.quadraticCurveTo(x + HALF_T, y + HALF_T, x + FULL_T, y + HALF_T);
      });
      break;
    default:
      throw tile.tile_case + " shouldn't be a three corners case 😕";
  }
}
function renderThreeCornersTwoOwners(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  switch (tile.tile_case) {
    case 7: // empty corner top left
      if (tile.corner_alliance[1] !== tile.corner_alliance[2]) {
        draw_land_1011_ownership_1x22(
          drawing,
          { x, y },
          3,
          styleForCorner(tile, 2),
          styleForCorner(tile, 1)
        );
      } else {
        draw_land_1011_ownership_1x12(
          drawing,
          { x, y },
          3,
          styleForCorner(tile, 2),
          styleForCorner(tile, 1)
        );
      }
      break;
    case 11: // empty corner top right
      if (tile.corner_alliance[0] !== tile.corner_alliance[2]) {
        draw_land_1011_ownership_1x22(
          drawing,
          { x, y },
          0,
          styleForCorner(tile, 0),
          styleForCorner(tile, 3)
        );
      } else {
        draw_land_1011_ownership_1x12(
          drawing,
          { x, y },
          0,
          styleForCorner(tile, 0),
          styleForCorner(tile, 3)
        );
      }
      break;
    case 13: // empty corner bottom left
      if (tile.corner_alliance[0] !== tile.corner_alliance[3]) {
        draw_land_1011_ownership_1x22(
          drawing,
          { x, y },
          2,
          styleForCorner(tile, 3),
          styleForCorner(tile, 0)
        );
      } else {
        draw_land_1011_ownership_1x12(
          drawing,
          { x, y },
          2,
          styleForCorner(tile, 3),
          styleForCorner(tile, 0)
        );
      }
      break;
    case 14: // empty corner bottom right
      if (tile.corner_alliance[0] !== tile.corner_alliance[1]) {
        draw_land_1011_ownership_1x22(
          drawing,
          { x, y },
          1,
          styleForCorner(tile, 1),
          styleForCorner(tile, 2)
        );
      } else {
        draw_land_1011_ownership_1x12(
          drawing,
          { x, y },
          1,
          styleForCorner(tile, 1),
          styleForCorner(tile, 2)
        );
      }

      break;
    default:
      throw tile.tile_case + " shouldn't be a three corners case 😕";
  }
}
function renderThreeCornersThreeOwners() {}

function renderTwoAdjacentOneOwner() {}
function renderTwoAdjacentTwoOwners() {}

function renderTwoOppositeOneOwner() {}
function renderTwoOppositeTwoOwners() {}

function renderSingleCornerOneOwner() {}
function renderFullWater() {}

function styleForCorner(tile: RenderTile, i: number) {
  const alliance = tile.corner_alliance![i];
  if (alliance != 0) {
    return keepColors[alliance % keepColors.length];
  } else {
    return "";
  }
}

function getCenterOfCurve(
  x0: number,
  y0: number,
  cpx: number,
  cpy: number,
  x1: number,
  y1: number
) {
  const mx1 = (x0 + cpx) / 2;
  const my1 = (y0 + cpy) / 2;

  const mx2 = (cpx + x1) / 2;
  const my2 = (cpy + y1) / 2;

  const midX = (mx1 + mx2) / 2;
  const midY = (my1 + my2) / 2;

  return { x: midX, y: midY };
}

// function drawRect(
//   x: number,
//   y: number,
//   width: number,
//   height: number,
//   fill_style: string
// ) {
//   if (!rect_queue.has(fill_style)) rect_queue.set(fill_style, []);
//   rect_queue.get(fill_style)!.push({ x, y, width, height });
// }

// function drawLine(
//   start_x: number,
//   start_y: number,
//   end_x: number,
//   end_y: number,
//   stroke_style: string
// ) {
//   if (!line_queue.has(stroke_style)) line_queue.set(stroke_style, []);
//   line_queue.get(stroke_style)!.push({ start_x, start_y, end_x, end_y });
// }

// function drawArc(
//   p0_x: number,
//   p0_y: number,
//   p1_x: number,
//   p1_y: number,
//   cp_x: number,
//   cp_y: number,
//   p2_x: number,
//   p2_y: number,
//   fill_style: string
// ) {
//   if (!arc_queue.has(fill_style)) arc_queue.set(fill_style, []);
//   arc_queue
//     .get(fill_style)!
//     .push({ p0_x, p0_y, p1_x, p1_y, cp_x, cp_y, p2_x, p2_y });
// }

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
