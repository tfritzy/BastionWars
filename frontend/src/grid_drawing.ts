import {
  HALF_T,
  keepColors,
  FULL_T,
  Layer,
  colorForAlliance,
} from "./constants.ts";
import type { Drawing } from "./drawing.ts";
import { RenderAllianceCase, type RenderTile } from "./Schema.ts";
import {
  draw_land_0111_ownership_x111,
  draw_land_0111_ownership_x121,
  draw_land_0111_ownership_x211,
  draw_land_1000_ownership_1xxx,
  draw_land_1001_ownership_1xx1,
  draw_land_1001_ownership_1xx2,
  draw_land_1011_ownership_1x12,
  draw_land_1011_ownership_1x22,
  draw_land_1011_ownership_1x23,
  draw_land_1100_ownership_11xx,
  draw_land_1100_ownership_12xx,
  draw_land_1101_ownership_12x1,
  draw_land_1111_ownership_1222,
} from "./tile_drawing.ts";
import type { GameState } from "./types.ts";

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
}

export function drawLandTile(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  //  drawing.drawStrokeable("black", 0.5, Layer.UI, (ctx) => {
  //   ctx.rect(x, y, FULL_T, FULL_T);
  //  });

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
      renderTwoOppositeOneOwner(drawing, x, y, tile);
      break;
    case RenderAllianceCase.TwoOpposite_TwoOwners:
      renderTwoOppositeTwoOwners(drawing, x, y, tile);
      break;

    case RenderAllianceCase.SingleCorner_OneOwner:
      renderSingleCornerOneOwner(drawing, x, y, tile);
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
  drawing.drawFillable(styleForCorner(tile, 0), Layer.Map, (ctx) =>
    ctx.rect(x, y, HALF_T, HALF_T)
  );
  drawing.drawFillable(styleForCorner(tile, 1), Layer.Map, (ctx) =>
    ctx.rect(x + HALF_T, y, HALF_T, HALF_T)
  );
  drawing.drawFillable(styleForCorner(tile, 2), Layer.Map, (ctx) =>
    ctx.rect(x, y + HALF_T, HALF_T, HALF_T)
  );
  drawing.drawFillable(styleForCorner(tile, 3), Layer.Map, (ctx) =>
    ctx.rect(x + HALF_T, y + HALF_T, HALF_T, HALF_T)
  );

  if (tile.corner_alliance[0] !== tile.corner_alliance[1]) {
    drawing.drawBoundary((ctx) => {
      ctx.moveTo(x + HALF_T, y + HALF_T);
      ctx.lineTo(x + HALF_T, y);
    });
  }

  if (tile.corner_alliance[1] !== tile.corner_alliance[3]) {
    drawing.drawBoundary((ctx) => {
      ctx.moveTo(x + HALF_T, y + HALF_T);
      ctx.lineTo(x + FULL_T, y + HALF_T);
    });
  }

  if (tile.corner_alliance[3] !== tile.corner_alliance[2]) {
    drawing.drawBoundary((ctx) => {
      ctx.moveTo(x + HALF_T, y + HALF_T);
      ctx.lineTo(x + HALF_T, y + FULL_T);
    });
  }

  if (tile.corner_alliance[0] !== tile.corner_alliance[2]) {
    drawing.drawBoundary((ctx) => {
      ctx.moveTo(x + HALF_T, y + HALF_T);
      ctx.lineTo(x, y + HALF_T);
    });
  }
}

function renderFullLandOneOwner(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  drawing.drawFillable(styleForCorner(tile, 0), Layer.Map, (ctx) =>
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
    drawing.drawFillable(styleForCorner(tile, 0), Layer.Map, (ctx) =>
      ctx.rect(x, y, FULL_T, HALF_T)
    );
    drawing.drawFillable(styleForCorner(tile, 2), Layer.Map, (ctx) =>
      ctx.rect(x, y + HALF_T, FULL_T, HALF_T)
    );
    drawing.drawBoundary((ctx) => {
      ctx.moveTo(x, y + HALF_T);
      ctx.lineTo(x + FULL_T, y + HALF_T);
    });
  } else {
    drawing.drawFillable(styleForCorner(tile, 0), Layer.Map, (ctx) =>
      ctx.rect(x, y, HALF_T, FULL_T)
    );
    drawing.drawFillable(styleForCorner(tile, 1), Layer.Map, (ctx) =>
      ctx.rect(x + HALF_T, y, HALF_T, FULL_T)
    );
    drawing.drawBoundary((ctx) => {
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
  if (!tile.corner_alliance) return;

  let firstCount = 1;
  for (let i = 1; i < tile.corner_alliance.length; i++) {
    if (tile.corner_alliance[i] === tile.corner_alliance[0]) {
      firstCount += 1;
    }
  }

  let oddCorner;
  if (firstCount === 1) {
    oddCorner = 0;
  } else {
    oddCorner = tile.corner_alliance.findIndex(
      (a) => a !== tile.corner_alliance![0]
    );
  }

  switch (oddCorner) {
    case 0: // odd corner top left
      draw_land_1111_ownership_1222(
        drawing,
        { x, y },
        0,
        styleForCorner(tile, 0),
        styleForCorner(tile, 1)
      );
      break;
    case 1: // odd corner top right
      draw_land_1111_ownership_1222(
        drawing,
        { x, y },
        1,
        styleForCorner(tile, 1),
        styleForCorner(tile, 2)
      );
      break;
    case 2: // odd corner bottom left
      draw_land_1111_ownership_1222(
        drawing,
        { x, y },
        3,
        styleForCorner(tile, 2),
        styleForCorner(tile, 1)
      );
      break;
    case 3: // odd corner bottom right
      draw_land_1111_ownership_1222(
        drawing,
        { x, y },
        2,
        styleForCorner(tile, 3),
        styleForCorner(tile, 1)
      );
      break;
    default:
      throw tile.tile_case + " shouldn't be a three corners case 😕";
  }
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
        styleForCorner(tile, 3)
      );
      break;
    case 11: // empty corner top right
      draw_land_0111_ownership_x111(
        drawing,
        { x, y },
        1,
        styleForCorner(tile, 2)
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
      if (
        tile.corner_alliance[0] === -1 &&
        tile.corner_alliance[1] === tile.corner_alliance[2]
      ) {
        draw_land_1101_ownership_12x1(
          drawing,
          { x, y },
          1,
          styleForCorner(tile, 3),
          styleForCorner(tile, 1)
        );
      } else if (
        tile.corner_alliance[0] === -1 &&
        tile.corner_alliance[2] === tile.corner_alliance[3]
      ) {
        draw_land_0111_ownership_x211(
          drawing,
          { x, y },
          0,
          styleForCorner(tile, 1),
          styleForCorner(tile, 2)
        );
      } else if (
        tile.corner_alliance[0] === -1 &&
        tile.corner_alliance[1] === tile.corner_alliance[3]
      ) {
        draw_land_0111_ownership_x121(
          drawing,
          { x, y },
          0,
          styleForCorner(tile, 2),
          styleForCorner(tile, 1)
        );
      } else if (tile.corner_alliance[1] !== tile.corner_alliance[3]) {
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
      if (
        tile.corner_alliance[1] === -1 &&
        tile.corner_alliance[0] === tile.corner_alliance[3]
      ) {
        draw_land_1101_ownership_12x1(
          drawing,
          { x, y },
          2,
          styleForCorner(tile, 2),
          styleForCorner(tile, 0)
        );
      } else if (
        tile.corner_alliance[1] === -1 &&
        tile.corner_alliance[0] === tile.corner_alliance[2]
      ) {
        draw_land_0111_ownership_x211(
          drawing,
          { x, y },
          1,
          styleForCorner(tile, 3),
          styleForCorner(tile, 0)
        );
      } else if (
        tile.corner_alliance[1] === -1 &&
        tile.corner_alliance[2] === tile.corner_alliance[3]
      ) {
        draw_land_0111_ownership_x121(
          drawing,
          { x, y },
          1,
          styleForCorner(tile, 0),
          styleForCorner(tile, 2)
        );
      } else if (tile.corner_alliance[0] !== tile.corner_alliance[2]) {
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
      if (
        tile.corner_alliance[2] === -1 &&
        tile.corner_alliance[0] === tile.corner_alliance[3]
      ) {
        draw_land_1101_ownership_12x1(
          drawing,
          { x, y },
          0,
          styleForCorner(tile, 1),
          styleForCorner(tile, 0)
        );
      } else if (
        tile.corner_alliance[2] === -1 &&
        tile.corner_alliance[1] === tile.corner_alliance[3]
      ) {
        draw_land_0111_ownership_x211(
          drawing,
          { x, y },
          3,
          styleForCorner(tile, 0),
          styleForCorner(tile, 1)
        );
      } else if (
        tile.corner_alliance[2] === -1 &&
        tile.corner_alliance[0] === tile.corner_alliance[1]
      ) {
        draw_land_0111_ownership_x121(
          drawing,
          { x, y },
          3,
          styleForCorner(tile, 3),
          styleForCorner(tile, 1)
        );
      } else if (tile.corner_alliance[0] !== tile.corner_alliance[1]) {
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
      if (
        tile.corner_alliance[3] === -1 &&
        tile.corner_alliance[1] === tile.corner_alliance[2]
      ) {
        draw_land_1101_ownership_12x1(
          drawing,
          { x, y },
          0,
          styleForCorner(tile, 0),
          styleForCorner(tile, 1)
        );
      } else if (
        tile.corner_alliance[2] === 3 &&
        tile.corner_alliance[0] === tile.corner_alliance[1]
      ) {
        draw_land_0111_ownership_x211(
          drawing,
          { x, y },
          2,
          styleForCorner(tile, 2),
          styleForCorner(tile, 0)
        );
      } else if (
        tile.corner_alliance[2] === -1 &&
        tile.corner_alliance[0] === tile.corner_alliance[2]
      ) {
        draw_land_0111_ownership_x121(
          drawing,
          { x, y },
          2,
          styleForCorner(tile, 1),
          styleForCorner(tile, 0)
        );
      } else if (tile.corner_alliance[0] !== tile.corner_alliance[1]) {
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

function renderTwoOppositeOneOwner(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  switch (tile.tile_case) {
    case 6:
      draw_land_1001_ownership_1xx1(
        drawing,
        { x, y },
        1,
        styleForCorner(tile, 1)
      );
      break;
    case 9:
      draw_land_1001_ownership_1xx1(
        drawing,
        { x, y },
        0,
        styleForCorner(tile, 0)
      );
      break;
    default:
      throw tile.tile_case + " shouldn't be a two opp corners case 😕";
  }
}
function renderTwoOppositeTwoOwners(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  switch (tile.tile_case) {
    case 6:
      draw_land_1001_ownership_1xx2(
        drawing,
        { x, y },
        1,
        styleForCorner(tile, 1),
        styleForCorner(tile, 2)
      );
      break;
    case 9:
      draw_land_1001_ownership_1xx2(
        drawing,
        { x, y },
        0,
        styleForCorner(tile, 0),
        styleForCorner(tile, 3)
      );
      break;
    default:
      throw tile.tile_case + " shouldn't be a two opp corners case 😕";
  }
}

function renderSingleCornerOneOwner(
  drawing: Drawing,
  x: number,
  y: number,
  tile: RenderTile
) {
  if (!tile.corner_alliance) return;

  switch (tile.tile_case) {
    case 1: // bottom right
      draw_land_1000_ownership_1xxx(
        drawing,
        { x, y },
        2,
        styleForCorner(tile, 3)
      );
      break;
    case 2: // bottom left
      draw_land_1000_ownership_1xxx(
        drawing,
        { x, y },
        3,
        styleForCorner(tile, 2)
      );
      break;
    case 4: // top right
      draw_land_1000_ownership_1xxx(
        drawing,
        { x, y },
        1,
        styleForCorner(tile, 1)
      );
      break;
    case 8: // top left
      draw_land_1000_ownership_1xxx(
        drawing,
        { x, y },
        0,
        styleForCorner(tile, 0)
      );
      break;
    default:
      throw tile.tile_case + " shouldn't be a two opp corners case 😕";
  }
}
function renderFullWater() {}

function styleForCorner(tile: RenderTile, i: number) {
  const alliance = tile.corner_alliance![i];

  if (alliance <= 0) {
    return "";
  }

  return colorForAlliance(alliance)[100];
}
