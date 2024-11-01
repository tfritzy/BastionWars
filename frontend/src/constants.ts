import { colors } from "./colors";
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
  KeepTop: 8,
  TreeShadows: 9,
  TreeBottoms: 10,
  TreeTops: 11,
  Projectiles: 12,
  UI: 13,
};

export const Constants = {
  MATCHMAKING_URL: "http://localhost:7249",
};

export const ARROW_LENGTH = 5;
export const ARROW_LINE_WIDTH = 0.75;

export const WORLD_TO_CANVAS = 48;
export const TILE_SIZE = WORLD_TO_CANVAS;
export const FULL_T = TILE_SIZE;
export const HALF_T = TILE_SIZE / 2;
export const QUARTER_T = TILE_SIZE / 4;
export const CORNER_RADIUS = TILE_SIZE / 4;
export const THREE_Q_T = HALF_T + QUARTER_T;

export const SCROLL_SPEED = 100;

export const keepColors = [
  colors.red,
  colors.orange,
  colors.amber,
  colors.yellow,
  colors.lime,
  colors.green,
  colors.emerald,
  colors.teal,
  colors.cyan,
  colors.sky,
  colors.blue,
  colors.indigo,
  colors.violet,
  colors.purple,
  colors.fuchsia,
  colors.pink,
  colors.rose,
];

export function colorForAlliance(alliance: number) {
  return keepColors[alliance % keepColors.length];
}

// Shadows
export const SHADOW_COLOR = colors.slate[800] + "22";

// Keeps
export const KEEP_FILL_STYLE = colors.slate[100];
export const KEEP_LINE_STYLE = colors.slate[600];
export const KEEP_LINE_WIDTH = 0.5;
export const ARROW_COLOR = "#475569";

// Land
export const LAND_LINE_STYLE = colors.slate[500];
export const LAND_LINE_WIDTH = 0.5;

// Units
export const UNIT_RADIUS = 3;
export const UNIT_AREA = 5;

// Trees
export const TREE_LINE_COLOR = "#999999";
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
