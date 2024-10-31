export function getTableText(headers: string[], rows: string[][]): string[] {
  // Calculate the maximum width needed for each column
  const colWidths = headers.map((header, colIndex) => {
    const columnValues = [header, ...rows.map((row) => row[colIndex] || "")];
    return Math.max(
      ...columnValues.map((val) => (val || "").toString().length)
    );
  });

  // Create the header row with padding
  const headerRow = headers
    .map((header, i) => header.padEnd(colWidths[i]))
    .join(" | ");

  // Create the separator line
  const separator = colWidths.map((width) => "-".repeat(width)).join("-+-");

  // Create each data row with padding
  const dataRows = rows.map((row) =>
    row
      .map((cell, i) => (cell || "").toString().padEnd(colWidths[i]))
      .join(" | ")
  );

  // Combine all parts
  return [headerRow, separator, ...dataRows];
}
