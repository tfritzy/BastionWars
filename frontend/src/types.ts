import type {
  KeepState,
  NewProjectile,
  RenderTile,
  SoldierState,
  TileType,
  V2,
  V3,
} from "./Schema";
import { Typeable } from "./typeable";

export type Vector2 = {
  x: number;
  y: number;
};

export type Vector3 = {
  x: number;
  y: number;
  z: number;
};

export function cloneMultiplyV3(v: Vector3, multiple: number) {
  return {
    x: v.x * multiple,
    y: v.y * multiple,
    z: v.z * multiple,
  };
}

export function multiplyV3(v: Vector3, multiple: number) {
  v.x *= multiple;
  v.y *= multiple;
  v.z *= multiple;
}

export function addV3(v1: Vector3, v2: Vector3) {
  v1.x += v2.x;
  v1.y += v2.y;
  v1.z += v2.z;
}

export function divide(v: Vector2, multiple: number) {
  v.x /= multiple;
  v.y /= multiple;
}

export function magnitude(v: Vector2) {
  return Math.sqrt(v.x * v.x + v.y * v.y);
}

export function normalize(v: Vector2) {
  const length = Math.sqrt(v.x * v.x + v.y * v.y);
  if (length === 0) {
    v.x = 0;
    v.y = 0;
    return v;
  }
  v.x /= length;
  v.y /= length;

  return v;
}

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

export type Projectile = {
  id: number;
  remaining_life: number;
  remaining_movement_time: number;
  current_pos: Vector3;
  current_velocity: Vector3;
  gravitational_force: number;
};

export const initialGameState: GameState = {
  keeps: [],
  soldiers: new Map(),
  renderTiles: [],
  tiles: [],
  mapWidth: 0,
  mapHeight: 0,
  projectiles: [],
};

export type GameState = {
  keeps: Keep[];
  soldiers: Map<number, Soldier>;
  renderTiles: RenderTile[];
  tiles: TileType[];
  mapWidth: number;
  mapHeight: number;
  projectiles: Projectile[];
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

export function parseV2(v2: V2): Vector2 {
  return {
    x: v2.x || 0,
    y: v2.y || 0,
  };
}

export function parseV3(v3: V3): Vector3 {
  return {
    x: v3.x || 0,
    y: v3.y || 0,
    z: v3.z || 0,
  };
}

export function parseProjectile(
  projectile: NewProjectile | undefined
): Projectile | null {
  if (
    !projectile ||
    !projectile.id ||
    !projectile.start_pos ||
    !projectile.time_will_land ||
    !projectile.initial_velocity ||
    !projectile.birth_time ||
    !projectile.gravitational_force
  ) {
    return null;
  }

  return {
    remaining_life: projectile.time_will_land - projectile.birth_time + 2,
    remaining_movement_time: projectile.time_will_land - projectile.birth_time,
    id: projectile.id,
    current_pos: parseV3(projectile.start_pos),
    current_velocity: parseV3(projectile.initial_velocity),
    gravitational_force: projectile.gravitational_force,
  };
}
