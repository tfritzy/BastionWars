import { SoldierType } from "./Schema";

export const Layer = {
  Map: 1,
  Grass: 2,
  ProjectilesOnGround: 3,
  Units: 4,
  Keeps: 5,
  Trees: 6,
  Projectiles: 7,
  UI: 8,
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
  [SoldierType.Archer]: "#86efacaa",
  [SoldierType.Warrior]: "#fb7185aa",
};

export const keepColors = [
  "#fecaca",
  "#bfdbfe",
  "#fef08a",
  "#d9f99d",
  "#fde68a",
  "#bbf7d0",
  "#99f6e4",
  "#a5f3fc",
  "#bae6fd",
  "#fed7aa",
  "#c7d2fe",
  "#e9d5ff",
  "#f5d0fe",
  "#fbcfe8",
  "#fecdd3",
];

export const BOUNDARY_LINE_STYLE = "#222222";
export const BOUNDARY_LINE_WIDTH = 1;
export const BOUNDARY_LINE_DASH = [4, 4];

export const KEEP_FILL_STYLE = "#e2e8f0";
export const KEEP_LINE_STYLE = "black";
export const KEEP_LINE_WIDTH = 1;
export const KEEP_LABEL_FONT = "bold 18pt Arial";
export const KEEP_LABEL_STROKE = "black";
export const KEEP_LABEL_COMPLETED_COLOR = "#6ee7b7";
export const KEEP_LABEL_REMAINING_COLOR = "white";
export const KEEP_LABEL_OUTLINE_COLOR = "#475569";

export const LAND_LINE_STYLE = "black";
export const LAND_LINE_WIDTH = 1;
