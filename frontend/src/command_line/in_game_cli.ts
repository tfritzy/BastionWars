import { SoldierType } from "../Schema";
import type { ClientState, GameState, Keep } from "../types";
import { CommandLine } from "./command_line";
import { getKeepTableText } from "./helpers";

export class InGameCli {
 private cli: CommandLine;
 private clientState: ClientState;
 private gameState: GameState;

 constructor(clientState: ClientState, gameState: GameState) {
  this.cli = new CommandLine(this.handleCommand);
  this.clientState = clientState;
  this.gameState = gameState;
 }

 handleCommand = (fullCommand: string): string => {
  const commands = fullCommand.split("|");
  let prevOutput: string = "";
  const fullOutput = [];

  for (let i = 0; i < commands.length; i++) {
   const command = commands[i];
   const split = command.trim().split(" ");
   const commandName = split.splice(0, 1)[0];

   if (prevOutput) {
    split.splice(0, 0, prevOutput);
   }

   const output = this.getOutputOfCommand(commandName, split);

   if (typeof output[0] !== "string") {
    prevOutput = (output as Keep[]).map((k) => k.name).join(",");
    if (i === commands.length - 1) {
     fullOutput.push(...getKeepTableText(output as Keep[]));
    }
   } else {
    fullOutput.push(...output);
   }
  }

  return fullOutput.join("\n");
 };

 getOutputOfCommand(command: string, args: string[]): string[] | Keep[] {
  switch (command) {
   case "attack":
   case "a":
    return this.attack(args);
   case "goto":
   case "g":
    return [this.goto(args)];
   case "keeps":
    return this.keeps(args);
   case "scroll":
   case "s":
    return [this.scroll(args)];
   default:
    return [`Unknown command: ${command}\nType 'help' for available commands`];
  }
 }

 attack = (args: string[]): string[] => {
  let soldierType = undefined;

  const hasArcherFlag = args.includes("--archers");
  const hasWarriorFlag = args.includes("--warriors");
  if (hasArcherFlag && !hasWarriorFlag) {
   soldierType = SoldierType.Archer;
  } else if (!hasArcherFlag && hasWarriorFlag) {
   soldierType = SoldierType.Warrior;
  }

  if (args.length < 2) {
   return [
    "attack needs at least 2 arguments.",
    "Example usage: attack {source_keep} {target_keep} 50",
   ];
  }

  const target_keep = this.gameState.keepNameToId.get(args[1]);
  if (!target_keep) {
   return [`Unable to find target_keep: ${args[1]}`];
  }

  let percent = Number(args[2]);
  if (args[3] && !Number.isInteger(percent)) {
   return ["percent should be an integer.", "Eg '43' to send 43% of troops"];
  }
  percent ||= 100;

  const output = [];
  const sources = args[0].split(",");
  for (const source of sources) {
   const source_keep = this.gameState.keepNameToId.get(source);
   if (!source_keep) {
    output.push(`Unable to find source_keep: ${source}`);
    continue;
   }

   const troopString = soldierType ? soldierType.toString() : "troop";
   output.push(
    `${source} sending ${percent}% of its ${troopString}s to ${args[1]}`
   );
   this.clientState.connection?.sendMessage({
    issue_deployment_order: {
     source_keep: source_keep,
     target_keep: target_keep,
     percent: percent / 100,
     soldier_type: soldierType,
    },
   });
  }

  return output;
 };

 goto = (args: string[]): string => {
  if (args.length < 1) {
   return "goto needs one argument. \nExample usage: goto {keep}";
  }

  const keepId = this.gameState.keepNameToId.get(args[0]);
  if (!keepId) {
   return `Unable to find keep: '${args[0]}'`;
  }

  const pos = {
   x: this.gameState.keeps.get(keepId)!.pos.x,
   y: this.gameState.keeps.get(keepId)!.pos.y,
  };
  this.clientState.targetCamPos = pos;
  return "";
 };

 keeps = (args: string[]): Keep[] => {
  let keeps = Array.from(this.gameState.keeps.values());

  const mineFlag = args.includes("--mine");
  if (mineFlag) {
   keeps = keeps.filter((k) => k.alliance === this.gameState.ownAlliance);
  }

  return keeps;
 };

 scroll = (args: string[]): string => {
  if (args.length < 1) {
   return "scroll requires at least 1 argument.\nExample usage: scroll r 3";
  }

  const dir = args[0];
  const amount = Number(args[1]) || 5;
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
}
