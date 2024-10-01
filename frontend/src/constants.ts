import { SoldierType } from "./Schema";

export const Layer = {
  Map: 1,
  Grass: 2,
  ProjectilesOnGround: 3,
  Units: 4,
  UnitOutlines: 5,
  KeepShadows: 6,
  TreeShadows: 7,
  KeepBase: 8,
  KeepTowers: 9,
  KeepCenter: 10,
  TreeBottoms: 11,
  TreeTopShadows: 12,
  TreeTops: 13,
  Projectiles: 14,
  UI: 15,
};

export const Constants = {
  MATCHMAKING_URL: "http://localhost:7249",
};

export const ARROW_LENGTH = 6;

export const WORLD_TO_CANVAS = 64;
export const TILE_SIZE = WORLD_TO_CANVAS;
export const FULL_T = TILE_SIZE;
export const HALF_T = TILE_SIZE / 2;
export const QUARTER_T = TILE_SIZE / 4;
export const CORNER_RADIUS = TILE_SIZE / 4;
export const THREE_Q_T = HALF_T + QUARTER_T;

export const soldierColors: { [key: string]: string } = {
  [SoldierType.Archer]: "#bbf7d0",
  [SoldierType.Warrior]: "#fde68a",
};

export const soldierOutlineColors: {
  [key: string]: string;
} = {
  [SoldierType.Archer]: "#166534",
  [SoldierType.Warrior]: "#92400e",
};

export const keepColors = [
  "#ecfccb",
  "#ffe4e6",
  "#d1fae5",
  "#fee2e2",
  "#dbeafe",
  "#cffafe",
  "#ede9fe",
  "#fef3c7",
  "#fae8ff",
];

export const SHADOW_COLOR = "#00000033";

export const BOUNDARY_LINE_STYLE = "#222222";
export const BOUNDARY_LINE_WIDTH = 1;
export const BOUNDARY_LINE_DASH = [4, 4];

export const KEEP_FILL_STYLE = "#e2e8f0";
export const KEEP_LINE_STYLE = "black";
export const KEEP_LINE_WIDTH = 0.5;
export const KEEP_LABEL_FONT = "bold 18pt Arial";
export const KEEP_LABEL_STROKE = "black";
export const KEEP_LABEL_COMPLETED_COLOR = "#6ee7b7";
export const KEEP_LABEL_REMAINING_COLOR = "white";
export const KEEP_LABEL_OUTLINE_COLOR = "#475569";
export const ARROW_COLOR = "#475569";

export const LAND_LINE_STYLE = "black";
export const LAND_LINE_WIDTH = 1;

export const UNIT_COLOR = "#9d4343";
export const UNIT_OUTLINE_COLOR = "#813645";
export const UNIT_OUTLINE_WIDTH = 0.25;
export const UNIT_RADIUS = 2;
