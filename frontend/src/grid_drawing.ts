import {
  BOUNDARY_LINE_STYLE,
  BOUNDARY_LINE_WIDTH,
  HALF_T,
  keepColors,
  QUARTER_T,
  FULL_T,
} from "./constants.ts";
import type { Drawing } from "./drawing.ts";
import { RenderAllianceCase, type RenderTile } from "./Schema.ts";
import {
  draw_land_0111_ownership_x111,
  draw_land_1011_ownership_1x12,
  draw_land_1011_ownership_1x22,
  draw_land_1011_ownership_1x23,
  draw_land_1100_ownership_11xx,
  draw_land_1100_ownership_12xx,
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
    corner_alliance: [0, 1, 1, 1],
    tile_case: 7,
  });
  drawLandTile(drawing, -8 * FULL_T - FULL_T / 2, 13 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [0, 1, 2, 1],
    tile_case: 7,
  });
  drawLandTile(drawing, -8 * FULL_T - FULL_T / 2, 15 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [0, 1, 2, 2],
    tile_case: 7,
  });
  drawLandTile(drawing, -8 * FULL_T - FULL_T / 2, 17 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_ThreeOwners,
    corner_alliance: [0, 1, 3, 2],
    tile_case: 7,
  });
  drawLandTile(drawing, -8 * FULL_T - FULL_T / 2, 19 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.TwoAdjacent_OneOwner,
    corner_alliance: [0, 0, 1, 1],
    tile_case: 3,
  });
  drawLandTile(drawing, -8 * FULL_T - FULL_T / 2, 21 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.TwoAdjacent_TwoOwners,
    corner_alliance: [0, 0, 2, 1],
    tile_case: 3,
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
    corner_alliance: [2, 0, 2, 1],
    tile_case: 11,
  });
  drawLandTile(drawing, -6 * FULL_T - FULL_T / 2, 17 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_ThreeOwners,
    corner_alliance: [3, 0, 2, 1],
    tile_case: 11,
  });
  drawLandTile(drawing, -6 * FULL_T - FULL_T / 2, 19 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.TwoAdjacent_OneOwner,
    corner_alliance: [0, 1, 0, 1],
    tile_case: 5,
  });
  drawLandTile(drawing, -6 * FULL_T - FULL_T / 2, 21 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.TwoAdjacent_TwoOwners,
    corner_alliance: [0, 1, 0, 2],
    tile_case: 5,
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
    corner_alliance: [1, 2, 0, 2],
    tile_case: 13,
  });
  drawLandTile(drawing, -4 * FULL_T - FULL_T / 2, 17 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_ThreeOwners,
    corner_alliance: [1, 2, 0, 3],
    tile_case: 13,
  });
  drawLandTile(drawing, -4 * FULL_T - FULL_T / 2, 19 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.TwoAdjacent_OneOwner,
    corner_alliance: [1, 0, 1, 0],
    tile_case: 10,
  });
  drawLandTile(drawing, -4 * FULL_T - FULL_T / 2, 21 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.TwoAdjacent_TwoOwners,
    corner_alliance: [2, 0, 1, 0],
    tile_case: 10,
  });

  drawLandTile(drawing, -2 * FULL_T - FULL_T / 2, 11 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_OneOwner,
    corner_alliance: [2, 1, 1, 0],
    tile_case: 14,
  });
  drawLandTile(drawing, -2 * FULL_T - FULL_T / 2, 13 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [2, 1, 2, 0],
    tile_case: 14,
  });
  drawLandTile(drawing, -2 * FULL_T - FULL_T / 2, 15 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_TwoOwners,
    corner_alliance: [1, 1, 2, 0],
    tile_case: 14,
  });
  drawLandTile(drawing, -2 * FULL_T - FULL_T / 2, 17 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.ThreeCorners_ThreeOwners,
    corner_alliance: [2, 3, 1, 0],
    tile_case: 14,
  });
  drawLandTile(drawing, -2 * FULL_T - FULL_T / 2, 19 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.TwoAdjacent_OneOwner,
    corner_alliance: [1, 1, 0, 0],
    tile_case: 12,
  });
  drawLandTile(drawing, -2 * FULL_T - FULL_T / 2, 21 * FULL_T - FULL_T / 2, {
    alliance_case: RenderAllianceCase.TwoAdjacent_TwoOwners,
    corner_alliance: [1, 2, 0, 0],
    tile_case: 12,
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
      renderThreeCornersThreeOwners(drawing, x, y, tile);
      break;

    case RenderAllianceCase.TwoAdjacent_TwoOwners:
      renderTwoAdjacentTwoOwners(drawing, x, y, tile);
      break;
    case RenderAllianceCase.TwoAdjacent_OneOwner:
      renderTwoAdjacentOneOwner(drawing, x, y, tile);
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
      draw_land_0111_ownership_x111(
        drawing,
        { x, y },
        0,
        styleForCorner(tile, 1)
      );
      break;
    case 11: // empty corner top right
      draw_land_0111_ownership_x111(
        drawing,
        { x, y },
        1,
        styleForCorner(tile, 1)
      );
      break;
    case 13: // empty corner bottom left
      draw_land_0111_ownership_x111(
        drawing,
        { x, y },
        3,
        styleForCorner(tile, 1)
      );
      break;
    case 14: // empty corner bottom right
      draw_land_0111_ownership_x111(
        drawing,
        { x, y },
        2,
        styleForCorner(tile, 1)
      );
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
      if (tile.corner_alliance[1] !== tile.corner_alliance[3]) {
        draw_land_1011_ownership_1x22(
          drawing,
          { x, y },
          3,
          styleForCorner(tile, 1),
          styleForCorner(tile, 2)
        );
      } else {
        draw_land_1011_ownership_1x12(
          drawing,
          { x, y },
          3,
          styleForCorner(tile, 1),
          styleForCorner(tile, 2)
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
      if (tile.corner_alliance[0] !== tile.corner_alliance[1]) {
        draw_land_1011_ownership_1x22(
          drawing,
          { x, y },
          2,
          styleForCorner(tile, 0),
          styleForCorner(tile, 3)
        );
      } else {
        draw_land_1011_ownership_1x12(
          drawing,
          { x, y },
          2,
          styleForCorner(tile, 0),
          styleForCorner(tile, 3)
        );
      }
      break;
    case 14: // empty corner bottom right
      if (tile.corner_alliance[0] !== tile.corner_alliance[1]) {
        draw_land_1011_ownership_1x22(
          drawing,
          { x, y },
          1,
          styleForCorner(tile, 2),
          styleForCorner(tile, 1)
        );
      } else {
        draw_land_1011_ownership_1x12(
          drawing,
          { x, y },
          1,
          styleForCorner(tile, 2),
          styleForCorner(tile, 1)
        );
      }

      break;
    default:
      throw tile.tile_case + " shouldn't be a three corners case 😕";
  }
}
function renderThreeCornersThreeOwners(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  switch (tile.tile_case) {
    case 7: // empty corner top left
      draw_land_1011_ownership_1x23(
        drawing,
        { x, y },
        3,
        styleForCorner(tile, 2),
        styleForCorner(tile, 3),
        styleForCorner(tile, 1)
      );
      break;
    case 11: // empty corner top right
      draw_land_1011_ownership_1x23(
        drawing,
        { x, y },
        0,
        styleForCorner(tile, 0),
        styleForCorner(tile, 2),
        styleForCorner(tile, 3)
      );
      break;
    case 13: // empty corner bottom left
      draw_land_1011_ownership_1x23(
        drawing,
        { x, y },
        2,
        styleForCorner(tile, 3),
        styleForCorner(tile, 1),
        styleForCorner(tile, 0)
      );
      break;
    case 14: // empty corner bottom right
      draw_land_1011_ownership_1x23(
        drawing,
        { x, y },
        1,
        styleForCorner(tile, 1),
        styleForCorner(tile, 0),
        styleForCorner(tile, 2)
      );
      break;
    default:
      throw tile.tile_case + " shouldn't be a three corners case 😕";
  }
}

function renderTwoAdjacentOneOwner(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  switch (tile.tile_case) {
    case 3: // empty top
      draw_land_1100_ownership_11xx(
        drawing,
        { x, y },
        2,
        styleForCorner(tile, 2)
      );
      break;
    case 5: // empty left side
      draw_land_1100_ownership_11xx(
        drawing,
        { x, y },
        1,
        styleForCorner(tile, 1)
      );
      break;
    case 10: // empty right side
      draw_land_1100_ownership_11xx(
        drawing,
        { x, y },
        3,
        styleForCorner(tile, 0)
      );
      break;
    case 12: // empty bottom
      draw_land_1100_ownership_11xx(
        drawing,
        { x, y },
        0,
        styleForCorner(tile, 0)
      );
      break;
    default:
      throw tile.tile_case + " shouldn't be a three corners case 😕";
  }
}
function renderTwoAdjacentTwoOwners(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  switch (tile.tile_case) {
    case 3: // empty top
      draw_land_1100_ownership_12xx(
        drawing,
        { x, y },
        2,
        styleForCorner(tile, 3),
        styleForCorner(tile, 2)
      );
      break;
    case 5: // empty left side
      draw_land_1100_ownership_12xx(
        drawing,
        { x, y },
        1,
        styleForCorner(tile, 1),
        styleForCorner(tile, 3)
      );
      break;
    case 10: // empty right side
      draw_land_1100_ownership_12xx(
        drawing,
        { x, y },
        3,
        styleForCorner(tile, 2),
        styleForCorner(tile, 0)
      );
      break;
    case 12: // empty bottom
      draw_land_1100_ownership_12xx(
        drawing,
        { x, y },
        0,
        styleForCorner(tile, 0),
        styleForCorner(tile, 1)
      );
      break;
    default:
      throw tile.tile_case + " shouldn't be a three corners case 😕";
  }
}

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
