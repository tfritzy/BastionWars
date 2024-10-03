import type { Vector2 } from "./types";

enum PathType {
 Straight,
 Diagonal,
 Circular,
 CornerTurn,
}

const cornerTurnRadius = 0.5;

const PathLengths: Record<PathType, number> = {
 [PathType.Straight]: 1.0,
 [PathType.Diagonal]: 1.414, // Approximately sqrt(2)
 [PathType.Circular]: 1.571, // Approximately pi/2
 [PathType.CornerTurn]: 1 - cornerTurnRadius + 0.5 * Math.PI * cornerTurnRadius, // Precise calculation
};

function determinePathType(from: Vector2, to: Vector2): PathType {
 const dx = Math.abs(to.x - from.x);
 const dy = Math.abs(to.y - from.y);

 if (dx === 0 && dy === 0) return PathType.Straight; // No movement
 if ((dx === 1 && dy === 0) || (dx === 0 && dy === 1)) return PathType.Straight;
 if (dx === 1 && dy === 1) return PathType.Diagonal;
 if (dx + dy === 1) return PathType.Circular;
 if (dx + dy === 2 && (dx === 1 || dy === 1)) return PathType.CornerTurn;

 throw new Error(
  "Invalid path segment: movement greater than one tile is not supported."
 );
}

function determinePathPos(
 source: Vector2,
 target: Vector2,
 distanceAlong: number
): Vector2 {
 const type = determinePathType(source, target);
 const direction = normalizeVector({
  x: target.x - source.x,
  y: target.y - source.y,
 });
 const normalizedDistance = Math.min(distanceAlong / PathLengths[type], 1);
 const basePosition = { x: source.x + 0.5, y: source.y + 0.5 }; // Center of the source tile

 switch (type) {
  case PathType.Straight:
  case PathType.Diagonal:
   return {
    x: basePosition.x + direction.x * normalizedDistance,
    y: basePosition.y + direction.y * normalizedDistance,
   };
  case PathType.Circular:
   return calculateCircularPosition(
    basePosition,
    direction,
    normalizedDistance
   );
  case PathType.CornerTurn:
   return calculateCornerTurnPosition(
    basePosition,
    direction,
    normalizedDistance
   );
  default:
   throw new Error("Invalid path type");
 }
}

function calculateCircularPosition(
 basePosition: Vector2,
 direction: Vector2,
 normalizedDistance: number
): Vector2 {
 const angle = (normalizedDistance * Math.PI) / 2;
 const rotatedDirection = {
  x: direction.x * Math.cos(angle) - direction.y * Math.sin(angle),
  y: direction.x * Math.sin(angle) + direction.y * Math.cos(angle),
 };
 return {
  x: basePosition.x + rotatedDirection.x,
  y: basePosition.y + rotatedDirection.y,
 };
}

function calculateCornerTurnPosition(
 basePosition: Vector2,
 direction: Vector2,
 normalizedDistance: number
): Vector2 {
 const straightPart = 1 - cornerTurnRadius;
 const arcLength = 0.5 * Math.PI * cornerTurnRadius;
 const totalLength = straightPart + arcLength;

 if (normalizedDistance <= straightPart / totalLength) {
  // On the straight part
  return {
   x: basePosition.x + direction.x * (normalizedDistance * totalLength),
   y: basePosition.y + direction.y * (normalizedDistance * totalLength),
  };
 } else {
  // On the curved part
  const arcDistance = normalizedDistance * totalLength - straightPart;
  const angle = arcDistance / cornerTurnRadius;
  const cornerCenter = {
   x: basePosition.x + direction.x * straightPart,
   y: basePosition.y + direction.y * straightPart,
  };
  const perpDirection = { x: -direction.y, y: direction.x };
  return {
   x:
    cornerCenter.x +
    direction.x * cornerTurnRadius * Math.sin(angle) +
    perpDirection.x * cornerTurnRadius * (1 - Math.cos(angle)),
   y:
    cornerCenter.y +
    direction.y * cornerTurnRadius * Math.sin(angle) +
    perpDirection.y * cornerTurnRadius * (1 - Math.cos(angle)),
  };
 }
}

function normalizeVector(vector: Vector2): Vector2 {
 const magnitude = Math.sqrt(vector.x * vector.x + vector.y * vector.y);
 return {
  x: vector.x / magnitude,
  y: vector.y / magnitude,
 };
}

export function updateSoldierPosition(
 path: Vector2[],
 prog: number,
 subProg: number,
 deltaTime: number
): void {
 if (path.length < 2) {
  return; // No update needed if the path is too short
 }

 let currentSegment = Math.floor(prog);
 let nextSegment = currentSegment + 1;

 if (nextSegment >= path.length) {
  return; // No update needed if we've reached the end of the path
 }

 const source = path[currentSegment];
 const target = path[nextSegment];

 subProg += deltaTime;
 const pathType = determinePathType(source, target);
 const segmentLength = PathLengths[pathType];

 if (subProg >= segmentLength) {
  // Move to the next segment
  prog += 1;
  subProg -= segmentLength;
 }

 // The function doesn't return anything as per the provided signature
 // The caller is expected to use the updated prog and subProg values
 // to update the soldier's position externally
}
