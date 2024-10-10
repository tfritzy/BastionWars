import type { Vector2, Vector3 } from "./types";

export function deleteBySwap(arr: any[], index: number) {
  if (index < 0 || index >= arr.length) {
    throw new Error("Index out of bounds");
  }

  // Swap the element at `index` with the last element
  [arr[index], arr[arr.length - 1]] = [arr[arr.length - 1], arr[index]];

  // Remove the last element (which is now the element we want to delete)
  arr.pop();

  return arr;
}

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

export function addV2(v1: Vector2, v2: Vector2) {
  v1.x += v2.x;
  v1.y += v2.y;
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

  return { x: v.x / length, y: v.y / length };
}
