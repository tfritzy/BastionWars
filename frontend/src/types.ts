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
};

export const initialGameState: GameState = {
 keeps: [],
 soldiers: new Map(),
};

export type GameState = {
 keeps: Keep[];
 soldiers: Map<number, Soldier>;
};

export const parseKeep: (
 keep: KeepState | undefined
) => Keep | null = (keep) => {
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

export const parseSoldier: (
 soldier: SoldierState | undefined
) => Soldier | null = (sol) => {
 if (!sol || !sol.id || !sol.pos) {
  return null;
 }

 return {
  id: sol.id,
  pos: parseV2(sol.pos),
 };
};

export function updateSoldier(
 soldier: Soldier,
 soldierState: SoldierState | undefined
) {
 if (soldierState?.pos)
  soldier.pos = parseV2(soldierState?.pos);
 if (soldierState?.velocity)
  soldier.velocity = parseV2(soldierState?.velocity);
}

export function parseV2(v2: V2) {
 return {
  x: v2.x || 0,
  y: v2.y || 0,
 };
}
