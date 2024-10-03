import type { Soldier, Vector2 } from "./types";

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
  [PathType.CornerTurn]:
    1 - cornerTurnRadius + 0.5 * Math.PI * cornerTurnRadius, // Precise calculation
};

function determinePathType(from: Vector2, to: Vector2): PathType {
  const dx = Math.abs(to.x - from.x);
  const dy = Math.abs(to.y - from.y);

  if (dx === 0 && dy === 0) return PathType.Straight; // No movement
  if ((dx === 1 && dy === 0) || (dx === 0 && dy === 1))
    return PathType.Straight;
  if (dx === 1 && dy === 1) return PathType.Diagonal;
  if (dx + dy === 1) return PathType.Circular;
  if (dx + dy === 2 && (dx === 1 || dy === 1)) return PathType.CornerTurn;

  throw new Error(
    "Invalid path segment: movement greater than one tile is not supported."
  );
}

export function determinePathPos(
  source: Vector2,
  target: Vector2,
  distanceAlong: number
): Vector2 {
  const type = determinePathType(source, target);
  const direction = normalizeVector({
    x: target.x - source.x,
    y: target.y - source.y,
  });
  const basePosition = { x: source.x + 0.5, y: source.y + 0.5 }; // Center of the source tile

  switch (type) {
    case PathType.Straight:
    case PathType.Diagonal:
      return {
        x: basePosition.x + direction.x * distanceAlong,
        y: basePosition.y + direction.y * distanceAlong,
      };
    case PathType.Circular:
      return calculateCircularPosition(basePosition, direction, distanceAlong);
    case PathType.CornerTurn:
      return calculateCornerTurnPosition(
        basePosition,
        direction,
        distanceAlong
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

export function updateSoldierPathProgress(
  soldier: Soldier,
  path: Vector2[],
  deltaTime: number
): void {
  const step = getCurrentPathSteps(path, soldier.pathIndex);
  if (!step) return;

  soldier.subPathProgress += soldier.movementSpeed * deltaTime;
  const pathType = determinePathType(step.source, step.target);
  const segmentLength = PathLengths[pathType];

  if (soldier.subPathProgress >= segmentLength) {
    // Move to the next segment
    soldier.pathIndex += 1;
    soldier.subPathProgress -= segmentLength;
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
    return null; // No update needed if the path is too short
  }

  let currentSegment = Math.floor(prog);
  let nextSegment = currentSegment + 1;

  if (nextSegment >= path.length) {
    return null; // No update needed if we've reached the end of the path
  }

  return {
    source: path[currentSegment],
    target: path[nextSegment],
  };
}
