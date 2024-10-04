import { TILE_SIZE } from "./constants";
import type { Soldier, Vector2 } from "./types";

enum PathType {
 Straight,
 Circular,
}

const cornerTurnRadius = 0.5;
const PathLengths: Record<PathType, number> = {
 [PathType.Straight]: 1.0,
 [PathType.Circular]: 1.571,
};

function determinePathType(path: Vector2[], pathIndex: number): PathType {
 if (pathIndex >= path.length - 1) {
  throw new Error("Invalid path index");
 }

 if (pathIndex === 0 || pathIndex === path.length - 2) {
  return PathType.Straight;
 }

 const prev = path[pathIndex - 1];
 const current = path[pathIndex];
 const next = path[pathIndex + 1];

 const prevDirection = {
  x: current.x - prev.x,
  y: current.y - prev.y,
 };
 const nextDirection = {
  x: next.x - current.x,
  y: next.y - current.y,
 };

 return prevDirection.x !== nextDirection.x ||
  prevDirection.y !== nextDirection.y
  ? PathType.Circular
  : PathType.Straight;
}

export function determinePathPos(
 path: Vector2[],
 pathIndex: number,
 distanceAlong: number
): Vector2 {
 const type = determinePathType(path, pathIndex);
 const source = path[pathIndex];
 const target = path[pathIndex + 1];
 const percentAlong = distanceAlong / PathLengths[type];
 const center = {
  x: source.x + 0.5,
  y: source.y + 0.5,
 };
 const delta = {
  x: target.x - source.x,
  y: target.y - source.y,
 };
 const reverseDir = {
  x: -delta.x / 2,
  y: -delta.y / 2,
 };
 const origin = {
  x: center.x + reverseDir.x,
  y: center.y + reverseDir.y,
 };
 console.log(center, reverseDir, origin);

 switch (type) {
  case PathType.Straight:
   return {
    x: delta.x * percentAlong + origin.x,
    y: delta.y * percentAlong + origin.y,
   };
  case PathType.Circular:
   return { x: 1, y: 1 };
  //    return calculateCircularPosition(basePosition, direction, distanceAlong);
  default:
   throw new Error("Invalid path type");
 }
}

function calculateCircularPosition(
 basePosition: Vector2,
 direction: Vector2,
 distanceAlong: number
): Vector2 {
 const percentComplete = distanceAlong / PathLengths[PathType.Circular];
 const angle = (percentComplete * Math.PI) / 2;
 const rotatedDirection = {
  x: direction.x * Math.cos(angle) - direction.y * Math.sin(angle),
  y: direction.x * Math.sin(angle) + direction.y * Math.cos(angle),
 };
 return {
  x: basePosition.x + rotatedDirection.x,
  y: basePosition.y + rotatedDirection.y,
 };
}

function normalizeVector(vector: Vector2): Vector2 {
 const magnitude = Math.sqrt(vector.x * vector.x + vector.y * vector.y);
 return {
  x: vector.x / magnitude,
  y: vector.y / magnitude,
 };
}

export function updateSoldierPathProgress(
 soldier: Soldier,
 path: Vector2[],
 deltaTime: number
): void {
 const step = getCurrentPathSteps(path, soldier.pathIndex);
 if (!step) return;

 soldier.subPathProgress += soldier.movementSpeed * deltaTime;
 const pathType = determinePathType(path, soldier.pathIndex);
 const segmentLength = PathLengths[pathType];

 if (soldier.subPathProgress >= segmentLength) {
  soldier.pathIndex += 1;
  soldier.subPathProgress -= segmentLength;
  console.log("Completed path");
 }
}

type PathStep = {
 source: Vector2;
 target: Vector2;
};

export function getCurrentPathSteps(
 path: Vector2[],
 prog: number
): PathStep | null {
 if (path.length < 2) {
  return null;
 }

 const currentSegment = Math.floor(prog);
 const nextSegment = currentSegment + 1;

 if (nextSegment >= path.length) {
  return null;
 }

 return {
  source: path[currentSegment],
  target: path[nextSegment],
 };
}
