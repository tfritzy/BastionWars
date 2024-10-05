import { WalkPathType } from "./Schema";
import type { Soldier, Vector2 } from "./types";

function getPathLength(walkType: WalkPathType) {
 if (
  walkType == WalkPathType.StraightDown ||
  walkType == WalkPathType.StraightToRight ||
  walkType == WalkPathType.StraightUp ||
  walkType == WalkPathType.StraightLeft
 ) {
  return 1;
 } else {
  return 1.571;
 }
}

const A_270 = Math.PI + Math.PI / 2;
const A_180 = Math.PI;
const A_90 = Math.PI / 2;
const A_0 = 0;

export function determinePathPos(
 p: Vector2,
 walkType: WalkPathType,
 distanceAlong: number
): Vector2 {
 switch (walkType) {
  case WalkPathType.StraightToRight:
   return { x: p.x + distanceAlong, y: p.y + 0.5 };
  case WalkPathType.StraightDown:
   return { x: p.x + 0.5, y: p.y + distanceAlong };
  case WalkPathType.StraightUp:
   return { x: p.x + 0.5, y: p.y + 1 - distanceAlong };
  case WalkPathType.StraightLeft:
   return { x: p.x + 1 - distanceAlong, y: p.y + 0.5 };
  case WalkPathType.CircularLeftDown:
   return determineCircularPos(p.x, p.y + 1, A_90, true, distanceAlong);
  case WalkPathType.CircularLeftUp:
   return determineCircularPos(p.x, p.y, A_270, false, distanceAlong);
  case WalkPathType.CircularDownLeft:
   return determineCircularPos(p.x, p.y + 1, A_0, false, distanceAlong);
  case WalkPathType.CircularDownRight:
   return determineCircularPos(p.x + 1, p.y + 1, A_180, true, distanceAlong);
  case WalkPathType.CircularRightDown:
   return determineCircularPos(p.x + 1, p.y + 1, A_90, false, distanceAlong);
  case WalkPathType.CircularRightUp:
   return determineCircularPos(p.x + 1, p.y, A_270, true, distanceAlong);
  case WalkPathType.CircularUpRight:
   return determineCircularPos(p.x + 1, p.y, A_180, false, distanceAlong);
  case WalkPathType.CircularUpLeft:
   return determineCircularPos(p.x, p.y, A_0, true, distanceAlong);
  default:
   console.error("Uknown walk path type", walkType);
   return { x: 0, y: 0 };
 }
}

function determineCircularPos(
 circleCenterX: number,
 circleCenterY: number,
 startAngle: number,
 clockwise: boolean,
 distanceAlong: number
): Vector2 {
 const angleTraversed =
  (distanceAlong / (0.5 * 2 * Math.PI)) * (0.5 * 2 * Math.PI);

 let finalAngle = clockwise
  ? startAngle - angleTraversed
  : startAngle + angleTraversed;

 finalAngle = finalAngle % (2 * Math.PI);
 if (finalAngle < 0) finalAngle += 2 * Math.PI;

 const x = circleCenterX + 0.5 * Math.cos(finalAngle);
 const y = circleCenterY - 0.5 * Math.sin(finalAngle);

 return { x, y };
}

export function updateSoldierPathProgress(
 soldier: Soldier,
 walkPath: WalkPathType[],
 deltaTime: number
): void {
 if (soldier.pathIndex < 0 || soldier.pathIndex >= walkPath.length) {
  return;
 }

 soldier.subPathProgress += soldier.movementSpeed * deltaTime;
 const segmentLength = getPathLength(walkPath[soldier.pathIndex]);
 if (soldier.subPathProgress >= segmentLength) {
  soldier.pathIndex += 1;
  soldier.subPathProgress -= segmentLength;
 }
}
