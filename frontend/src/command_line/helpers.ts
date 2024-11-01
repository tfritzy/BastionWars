import type { Keep, Vector2 } from "../types";

// Define which columns we want to display and their headers
const DISPLAY_COLUMNS = {
 name: "Name",
 pos: "Position",
 warrior_count: "Warriors",
 archer_count: "Archers",
 alliance: "Alliance",
} as const;

type DisplayColumns = keyof typeof DISPLAY_COLUMNS;

export function getKeepTableText(keeps: Keep[]): string[] {
 if (!keeps.length) return [];

 // Get headers from our display columns
 const headers = Object.values(DISPLAY_COLUMNS);

 // Format cell value based on column type
 const formatCellValue = (keep: Keep, col: DisplayColumns): string => {
  const value = keep[col];
  if (col === "pos") {
   return `(${(value as Vector2).x},${(value as Vector2).y})`;
  }
  return value.toString();
 };

 // Calculate the maximum width needed for each column
 const colWidths: number[] = Object.entries(DISPLAY_COLUMNS).map(
  ([col, header]) => {
   const columnValues = [
    header,
    ...keeps.map((keep) => formatCellValue(keep, col as DisplayColumns)),
   ];
   return Math.max(...columnValues.map((val) => val.length));
  }
 );

 // Create the header row with padding
 const headerRow = headers
  .map((header, i) => header.padEnd(colWidths[i]))
  .join(" | ");

 // Create the separator line
 const separator = colWidths.map((width) => "-".repeat(width)).join("-+-");

 // Create each data row with padding
 const dataRows = keeps.map((keep) =>
  Object.keys(DISPLAY_COLUMNS)
   .map((col, i) => {
    const value = formatCellValue(keep, col as DisplayColumns);
    return value.padEnd(colWidths[i]);
   })
   .join(" | ")
 );

 // Combine all parts
 return [headerRow, separator, ...dataRows];
}
