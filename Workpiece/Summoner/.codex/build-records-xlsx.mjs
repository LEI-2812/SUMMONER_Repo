import fs from "node:fs/promises";
import path from "node:path";
import { createRequire } from "node:module";

const require = createRequire(import.meta.url);
const { SpreadsheetFile, Workbook } = require("@oai/artifact-tool");

const rootDir = process.cwd();
const recordsDir = path.join(rootDir, ".codex", "records");
const outputPath = path.join(rootDir, "records.xlsx");

function parseCsv(text) {
  const rows = [];
  let row = [];
  let value = "";
  let inQuotes = false;

  for (let index = 0; index < text.length; index += 1) {
    const char = text[index];
    const next = text[index + 1];

    if (char === '"' && inQuotes && next === '"') {
      value += '"';
      index += 1;
      continue;
    }

    if (char === '"') {
      inQuotes = !inQuotes;
      continue;
    }

    if (char === "," && !inQuotes) {
      row.push(value);
      value = "";
      continue;
    }

    if ((char === "\n" || char === "\r") && !inQuotes) {
      if (char === "\r" && next === "\n") {
        index += 1;
      }
      row.push(value);
      if (row.some((cell) => cell.length > 0)) {
        rows.push(row);
      }
      row = [];
      value = "";
      continue;
    }

    value += char;
  }

  if (value.length > 0 || row.length > 0) {
    row.push(value);
    if (row.some((cell) => cell.length > 0)) {
      rows.push(row);
    }
  }

  return rows;
}

async function csvRowsLoad(fileName) {
  const text = await fs.readFile(path.join(recordsDir, fileName), "utf8");
  return parseCsv(text);
}

function sheetWrite(sheet, rows) {
  const rowCount = rows.length;
  const colCount = rows[0]?.length ?? 0;
  const range = sheet.getRangeByIndexes(0, 0, rowCount, colCount);
  range.values = rows;

  const header = sheet.getRangeByIndexes(0, 0, 1, colCount);
  header.format = {
    fill: "#1F4E79",
    font: { bold: true, color: "#FFFFFF" },
  };

  range.format.borders = { preset: "all", style: "thin", color: "#D9D9D9" };
  range.format.wrapText = true;

  sheet.freezePanes.freezeRows(1);

  const widths = [90, 95, 90, 220, 420, 420, 360, 100, 280];
  for (let col = 0; col < colCount; col += 1) {
    sheet.getRangeByIndexes(0, col, rowCount, 1).format.columnWidthPx = widths[col] ?? 180;
  }
}

function columnNameGet(columnNumber) {
  let name = "";
  let value = columnNumber;
  while (value > 0) {
    const mod = (value - 1) % 26;
    name = String.fromCharCode(65 + mod) + name;
    value = Math.floor((value - mod) / 26);
  }
  return name;
}

const workbook = Workbook.create();
const devRows = await csvRowsLoad("dev-records.csv");
const qaRows = await csvRowsLoad("qa-records.csv");

const devSheet = workbook.worksheets.add("DEV Records");
sheetWrite(devSheet, devRows);

const qaSheet = workbook.worksheets.add("QA Records");
sheetWrite(qaSheet, qaRows);

await workbook.inspect({
  kind: "table",
  range: "'DEV Records'!A1:H4",
  include: "values",
  tableMaxRows: 4,
  tableMaxCols: 8,
});

await workbook.inspect({
  kind: "table",
  range: "'QA Records'!A1:I3",
  include: "values",
  tableMaxRows: 3,
  tableMaxCols: 9,
});

const output = await SpreadsheetFile.exportXlsx(workbook);
await output.save(outputPath);

console.log(outputPath);
