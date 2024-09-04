import type { KeepState, V2 } from "./Schema";

type RecursiveNonNullable<T> = T extends (infer U)[]
  ? RecursiveNonNullable<U>[]
  : T extends object
  ? {
      [P in keyof T]-?: RecursiveNonNullable<NonNullable<T[P]>>;
    }
  : NonNullable<T>;

export type Keep = RecursiveNonNullable<KeepState>;

export const initialGameState: GameState = {
  keeps: [],
};

export type GameState = {
  keeps: Keep[];
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

export function parseV2(v2: V2) {
  return {
    x: v2.x || 0,
    y: v2.y || 0,
  };
}
