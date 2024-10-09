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
