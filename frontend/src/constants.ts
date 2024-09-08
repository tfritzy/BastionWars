import { SoldierType } from "./Schema";

export const Constants = {
  MATCHMAKING_URL: "http://localhost:7249",
};

export const WORLD_TO_CANVAS = 50;

export const soldierColors: { [key: string]: string } = {
  [SoldierType.Archer]: "#86efac",
  [SoldierType.Warrior]: "#fb7185",
};
