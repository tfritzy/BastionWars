import type { KeepState, SoldierState, V2 } from "./Schema";
import { Typeable } from "./typeable";

type Vector2 = {
  x: number;
  y: number;
};
export type Keep = {
  name: string;
  id: number;
  pos: Vector2;
  warrior_count: number;
  archer_count: number;
};
export type Soldier = {
  id: number;
  pos: Vector2;
  lastUpdated: number;
};

export const initialGameState: GameState = {
  keeps: [],
  soldiers: new Map(),
};

export type GameState = {
  keeps: Keep[];
  soldiers: Map<number, Soldier>;
};

export const parseKeep: (keep: KeepState | undefined) => Keep | null = (
  keep
) => {
  if (!keep || !keep.id || !keep.name || !keep.pos) {
    return null;
  }

  return {
    alliance: keep.alliance || 0,
    id: keep.id,
    name: keep.name,
    pos: parseV2(keep.pos),
    warrior_count: keep.warrior_count || 0,
    archer_count: keep.archer_count || 0,
  };
};

export function parseSoldier(
  soldier: SoldierState | undefined,
  time: number
): Soldier | null {
  if (!soldier || !soldier.id || !soldier.pos) {
    return null;
  }

  return {
    id: soldier.id,
    pos: parseV2(soldier.pos),
    lastUpdated: time,
  };
}

export function updateSoldier(
  soldier: Soldier,
  soldierState: SoldierState | undefined,
  time: number
) {
  if (soldierState?.pos) {
    soldier.pos = parseV2(soldierState?.pos);
  }
  soldier.lastUpdated = time;
}

export function parseV2(v2: V2) {
  return {
    x: v2.x || 0,
    y: v2.y || 0,
  };
}
