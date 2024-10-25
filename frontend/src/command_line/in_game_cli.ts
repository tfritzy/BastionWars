import type { Connection } from "../connection";
import type { ClientState, GameState } from "../types";
import { CommandLine } from "./command_line";

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
    return this.attack(split);
   case "goto":
    return this.goto(split);
   case "harvest":
    return this.harvest(split);
   case "fields":
    return this.fields(split);
   case "keeps":
    return this.keeps(split);
   default:
    return `Unknown command: ${split[0]}\nType 'help' for available commands`;
  }
 };

 attack = (command: string[]): string => {
  if (command.length < 3) {
   return "attack needs 2 arguments. \nExample usage: attack {source_keep} {target_keep}";
  }

  const source_keep = this.gameState.keepNameToId.get(command[1]);
  if (!source_keep) {
   return `Unable to find source_keep: ${command[1]}`;
  }

  const target_keep = this.gameState.keepNameToId.get(command[2]);
  if (!target_keep) {
   return `Unable to find target_keep: ${command[1]}`;
  }

  this.clientState.connection?.sendMessage({
   issue_deployment_order: {
    source_keep: source_keep,
    target_keep: target_keep,
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

  if (this.clientState.selectedKeep === keepId) {
   return `Already viewing '${command[1]}'`;
  }

  this.clientState.selectedKeep = keepId;
  return "";
 };

 fields = (command: string[]): string => {
  return this.gameState.harvestables
   .map((h) => `(${h.pos.x}, ${h.pos.y}): 34%`)
   .join("\n");
 };

 keeps = (command: string[]): string => {
  const header = "name\twarriors\tarchers\n";
  return (
   header +
   Array.from(this.gameState.keeps.values())
    .map((k) => `${k.name}\t${k.warrior_count}\t${k.archer_count}`)
    .join("\n")
  );
 };

 harvest = (command: string[]): string => {
  if (command.length < 2) {
   return "harvest needs one argument. \nExample usage: harvest xyz";
  }

  const typed = command[1];
  if (!this.gameState.harvestables.some((h) => h.text === typed)) {
   return `No field found with id: '${typed}'`;
  }

  this.clientState.connection?.sendMessage({
   harvest_field: {
    text: command[1],
   },
  });
  return "";
 };
}
