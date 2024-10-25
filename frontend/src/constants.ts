import type { DrawStyle } from "./drawing";
import { SoldierType } from "./Schema";

export const Layer = {
  Map: 1,
  Grass: 2,
  ProjectilesOnGround: 3,
  UnitShadows: 4,
  Units: 5,
  UnitOutlines: 6,
  Keep: 7,
  TreeShadows: 8,
  TreeBottoms: 9,
  TreeTops: 10,
  Projectiles: 11,
  UI: 12,
};

export const Constants = {
  MATCHMAKING_URL: "http://localhost:7249",
};

export const ARROW_LENGTH = 5;
export const ARROW_LINE_WIDTH = 0.75;

export const WORLD_TO_CANVAS = 64;
export const TILE_SIZE = WORLD_TO_CANVAS;
export const FULL_T = TILE_SIZE;
export const HALF_T = TILE_SIZE / 2;
export const QUARTER_T = TILE_SIZE / 4;
export const CORNER_RADIUS = TILE_SIZE / 4;
export const THREE_Q_T = HALF_T + QUARTER_T;

export const SCROLL_SPEED = 100;

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

// Shadows
export const SHADOW_COLOR = "#00000033";

// Ownership boundary
export const BOUNDARY_LINE_STYLE = "#222222";
export const BOUNDARY_LINE_WIDTH = 1;
export const BOUNDARY_LINE_DASH = [4, 4];

// Keeps
export const KEEP_FILL_STYLE = "#e2e8f0";
export const KEEP_LINE_STYLE = "black";
export const KEEP_LINE_WIDTH = 0.5;
export const ARROW_COLOR = "#475569";
export const KEEP_LABEL_COMPLETED_STYLE: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#6ee7b7",
  font: "bold 10pt Verdana",
  should_fill: true,
};
export const KEEP_LABEL_REMAINING_STYLE: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#444444",
  font: "bold 10pt Verdana",
  should_fill: true,
};

// Land
export const LAND_LINE_STYLE = "black";
export const LAND_LINE_WIDTH = 1;

// Units
export const UNIT_COLOR = "white";
export const UNIT_OUTLINE_COLOR = "black";
export const UNIT_OUTLINE_WIDTH = 0.5;
export const UNIT_RADIUS = 2.5;
export const UNIT_SHADOW_OFFSET = 1.5;
export const UNIT_AREA = 5;

// Trees
export const TREE_LINE_COLOR = "#333333";
export const TREE_LINE_WIDTH = 0.5;

// Ground words
export const GROUND_WORD_COMPLETED_STYLE: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#333333",
  font: "semibold 10px Verdana",
  should_fill: true,
  text_align: "start",
};
export const GROUND_WORD_REMAINING_STYLE: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#00000066",
  font: "semibold 10px Verdana",
  should_fill: true,
  text_align: "start",
};
