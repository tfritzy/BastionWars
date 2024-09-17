import { SoldierType } from "./Schema";

export const Constants = {
  MATCHMAKING_URL: "http://localhost:7249",
};

export const WORLD_TO_CANVAS = 64;
export const TILE_SIZE = WORLD_TO_CANVAS;
export const HALF_T = TILE_SIZE / 2;
export const QUARTER_T = TILE_SIZE / 4;
export const CORNER_RADIUS = TILE_SIZE / 4;

export const soldierColors: { [key: string]: string } = {
  [SoldierType.Archer]: "#86efacaa",
  [SoldierType.Warrior]: "#fb7185aa",
};

export const keepColors = [
  "#fecaca",
  "#fed7aa",
  "#fde68a",
  "#fef08a",
  "#d9f99d",
  "#bbf7d0",
  "#99f6e4",
  "#a5f3fc",
  "#bae6fd",
  "#bfdbfe",
  "#c7d2fe",
  "#e9d5ff",
  "#f5d0fe",
  "#fbcfe8",
  "#fecdd3",
];
export const BOUNDARY_LINE_STYLE = "black";
export const BOUNDARY_LINE_WIDTH = 1;
export const KEEP_LINE_STYLE = "black";
export const KEEP_LINE_WIDTH = 1;
