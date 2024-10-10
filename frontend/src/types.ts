import {
  GROUND_WORD_COMPLETED_STYLE,
  GROUND_WORD_REMAINING_STYLE,
} from "./constants";
import type { Drawing } from "./drawing";
import { ResourceLabel } from "./resource_label";
import type {
  KeepState,
  NewProjectile,
  RenderTile,
  NewSoldier,
  TileType,
  V2,
  V3,
  WalkPathType,
  NewWords,
  NewWord,
  V2Int,
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

export type Keep = {
  name: string;
  id: number;
  pos: Vector2;
  warrior_count: number;
  archer_count: number;
  alliance: number;
  pathMap: Map<number, Vector2[]>;
  walkMap: Map<number, WalkPathType[]>;
};

export type Soldier = {
  id: number;
  pos: Vector2;
  nonOffsetPos: Vector2;
  sourceKeepId: number;
  targetKeepId: number;
  pathIndex: number;
  subPathProgress: number;
  movementSpeed: number;
  rowOffset: number;
};

export type Projectile = {
  id: number;
  remaining_life: number;
  remaining_movement_time: number;
  current_pos: Vector3;
  current_velocity: Vector3;
  gravitational_force: number;
};

export type Harvestable = {
  pos: Vector2;
  text: string;
  resource: ResourceLabel;
};

export type GameState = {
  keeps: Map<number, Keep>;
  soldiers: Map<number, Soldier>;
  renderTiles: RenderTile[];
  tiles: TileType[];
  mapWidth: number;
  mapHeight: number;
  projectiles: Projectile[];
  harvestables: Harvestable[];
};

export const initialGameState: GameState = {
  keeps: new Map<number, Keep>(),
  soldiers: new Map(),
  renderTiles: [],
  tiles: [],
  mapWidth: 0,
  mapHeight: 0,
  projectiles: [],
  harvestables: [],
};

export const parseKeep: (keep: KeepState | undefined) => Keep | null = (
  keep
) => {
  if (
    !keep ||
    !keep.id ||
    !keep.name ||
    !keep.pos ||
    !keep.alliance ||
    !keep.paths
  ) {
    return null;
  }

  const pathMap: Map<number, Vector2[]> = new Map();
  const walkMap: Map<number, WalkPathType[]> = new Map();
  keep.paths.forEach((path) => {
    if (!path.target_id || !path.path || !path.walk_types) return;

    pathMap.set(
      path.target_id,
      path.path?.map((p) => parseV2(p))
    );
    walkMap.set(path.target_id, path.walk_types);
  });

  return {
    alliance: keep.alliance,
    id: keep.id,
    name: keep.name,
    pos: parseV2(keep.pos),
    warrior_count: keep.warrior_count || 0,
    archer_count: keep.archer_count || 0,
    pathMap: pathMap,
    walkMap: walkMap,
  };
};

export function parseSoldier(soldier: NewSoldier | undefined): Soldier | null {
  if (
    !soldier ||
    !soldier.id ||
    !soldier.source_keep_id ||
    !soldier.target_keep_id ||
    !soldier.movement_speed
  ) {
    return null;
  }

  return {
    id: soldier.id,
    pos: { x: 0, y: 0 },
    nonOffsetPos: { x: 0, y: 0 },
    sourceKeepId: soldier.source_keep_id,
    targetKeepId: soldier.target_keep_id,
    pathIndex: 0,
    subPathProgress: 0.5,
    movementSpeed: soldier.movement_speed,
    rowOffset: soldier.row_offset || 0,
  };
}

export function parseWord(
  msgWord: NewWord | undefined,
  drawing: Drawing,
  handleComplete: (resource_pos: V2Int, resource_text: string) => void
): Harvestable | null {
  if (msgWord?.grid_pos && msgWord?.text) {
    const pos = parseV2(msgWord.grid_pos);
    const text = msgWord.text;
    return {
      pos: pos,
      text: msgWord.text,
      resource: new ResourceLabel(
        text,
        drawing,
        parseV2(msgWord.owning_keep_pos!),
        () => handleComplete(pos, text)
      ),
    };
  }

  return null;
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
