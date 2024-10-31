import type { Connection } from "../connection";
import { TILE_SIZE } from "../constants";
import { deleteBySwap } from "../helpers";
import type { ClientState, GameState } from "../types";
import { CommandLine } from "./command_line";
import { getTableText } from "./helpers";

export class InGameCli {
 private cli: CommandLine;
 private clientState: ClientState;
 private gameState: GameState;

 constructor(clientState: ClientState, gameState: GameState) {
  this.cli = new CommandLine(this.handleCommand);
  this.clientState = clientState;
  this.gameState = gameState;
 }

 handleCommand = (command: string): string => {
  const split = command.split(" ");
  switch (split[0]) {
   case "attack":
   case "a":
    return this.attack(split);
   case "goto":
   case "g":
    return this.goto(split);
   case "harvest":
   case "h":
    return this.harvest(split);
   case "fields":
    return this.fields(split);
   case "keeps":
    return this.keeps(split);
   case "scroll":
   case "s":
    return this.scroll(split);
   default:
    return `Unknown command: ${split[0]}\nType 'help' for available commands`;
  }
 };

 attack = (command: string[]): string => {
  if (command.length < 3) {
   return "attack needs at least 2 arguments. \nExample usage: attack {source_keep} {target_keep} 50";
  }

  const source_keep = this.gameState.keepNameToId.get(command[1]);
  if (!source_keep) {
   return `Unable to find source_keep: ${command[1]}`;
  }

  const target_keep = this.gameState.keepNameToId.get(command[2]);
  if (!target_keep) {
   return `Unable to find target_keep: ${command[2]}`;
  }

  const percent = Number(command[3]);
  if (command[3] && !Number.isInteger(percent)) {
   return "percent should be an integer.\nEg '43' to send 43% of troops";
  }

  this.clientState.connection?.sendMessage({
   issue_deployment_order: {
    source_keep: source_keep,
    target_keep: target_keep,
    percent: percent ? percent / 100 : 1,
   },
  });
  return "";
 };

 goto = (command: string[]): string => {
  if (command.length < 2) {
   return "goto needs one argument. \nExample usage: goto {keep}";
  }

  const keepId = this.gameState.keepNameToId.get(command[1]);
  if (!keepId) {
   return `Unable to find keep: '${command[1]}'`;
  }

  const pos = {
   x: this.gameState.keeps.get(keepId)!.pos.x,
   y: this.gameState.keeps.get(keepId)!.pos.y,
  };
  this.clientState.targetCamPos = pos;
  return "";
 };

 fields = (command: string[]): string => {
  return getTableText(
   ["id", "location", "growth"],
   this.gameState.harvestables.map((h) => [
    h.text,
    `(${h.pos.x}, ${h.pos.y})`,
    (((h.totalGrowthTime - h.remainingGrowth) / h.totalGrowthTime) * 100)
     .toFixed(0)
     .toString() + "%",
   ])
  );
 };

 keeps = (command: string[]): string => {
  return getTableText(
   ["Name", "Lord", "Warriors", "Archers"],
   Array.from(this.gameState.keeps.values()).map((k) => [
    k.name,
    k.alliance.toString(),
    k.warrior_count.toString(),
    k.archer_count.toString(),
   ])
  );
 };

 scroll = (command: string[]): string => {
  if (command.length < 2) {
   return "scroll requires at least 1 argument.\nExample usage: scroll r 3";
  }

  const dir = command[1];
  const amount = Number(command[2]) || 1;
  if (!Number.isInteger(amount)) {
   return "second argument must be valid integer";
  }

  switch (dir) {
   case "left":
   case "l":
    this.clientState.targetCamPos.x -= amount;
    break;
   case "right":
   case "r":
    this.clientState.targetCamPos.x += amount;
    break;
   case "up":
   case "u":
    this.clientState.targetCamPos.y -= amount;
    break;
   case "down":
   case "d":
    this.clientState.targetCamPos.y += amount;
    break;
   default:
    return "first argument must be a valid direction.\nValid options: l, r, u, d, left, right, up, down";
  }

  return "";
 };

 harvest = (command: string[]): string => {
  if (command.length < 2) {
   return "harvest needs at least one argument. \nExample usage: harvest xyz abc";
  }

  const typed = command.slice(1);
  const invalid = [];
  for (let i = 0; i < typed.length; i++) {
   const t = typed[i];
   if (!this.gameState.harvestables.some((h) => h.text === t)) {
    deleteBySwap(typed, i);
    invalid.push(t);
    i -= 1;
   }
  }

  if (typed.length > 0) {
   this.clientState.connection?.sendMessage({
    harvest_fields: {
     text: typed,
    },
   });
  }

  if (invalid.length > 0) {
   return "Invalid values: " + invalid.join(", ");
  } else {
   return "";
  }
 };
}
