import { Connection } from "./connection.ts";
import { WORLD_TO_CANVAS } from "./constants.ts";
import type {
  GameFoundForPlayer,
  Long,
  Oneof_GameServerToPlayer,
} from "./Schema.ts";
import { Typeable } from "./typeable.ts";
import { initialGameState, parseKeep, type GameState } from "./types.ts";

export class Game {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private buttons: Array<Typeable>;
  private connection: Connection | undefined;
  private gameState: GameState = initialGameState;
  private keepLabels: Map<number, Typeable> = new Map();
  private selectedKeep: number | null = null;

  constructor(canvas: HTMLCanvasElement, ctx: CanvasRenderingContext2D) {
    this.canvas = canvas;
    this.ctx = ctx;

    this.buttons = [
      new Typeable("In game now", () => console.log("Whatever"), canvas, ctx),
    ];
  }

  draw(dpr: number, deltaTime: number): void {
    this.ctx.save();

    this.ctx.font = "30px Arial";
    this.ctx.fillStyle = "black";
    this.ctx.textAlign = "center";

    const canvasLogicalHeight = this.canvas.height / dpr;
    const canvasLogicalWidth = this.canvas.width / dpr;

    this.buttons.forEach((button, index) => {
      const x = canvasLogicalWidth / 2 - 60;
      const y = canvasLogicalHeight / 2 + index * 60 - 60;
      button.draw(x, y, deltaTime);
    });

    this.drawKeeps(deltaTime);
    this.ctx.restore();
  }

  drawKeeps(deltaTime: number) {
    this.ctx.save();
    this.ctx.strokeStyle = "black";
    this.ctx.lineWidth = 2;

    const radius = 20;
    const innerRadius = radius * 0.7;

    this.gameState.keeps.forEach((keep) => {
      const x = keep.pos.x * WORLD_TO_CANVAS;
      const y = keep.pos.y * WORLD_TO_CANVAS;

      this.ctx.beginPath();
      this.ctx.arc(x, y, radius, 0, 2 * Math.PI);
      this.ctx.stroke();

      this.ctx.beginPath();
      this.ctx.arc(x, y, innerRadius, 0, 2 * Math.PI);
      this.ctx.stroke();

      if (!this.keepLabels.has(keep.id)) {
      }

      this.keepLabels.get(keep.id)?.draw(x, y, deltaTime);
    });

    this.ctx.restore();
  }

  connectToGameServer(details: GameFoundForPlayer) {
    this.connection = new Connection(details.address!, this.onMessage);
  }

  handleTypeKeepName = (id: number) => {
    if (this.selectedKeep == null) {
      this.selectedKeep = id;
    } else {
      this.connection?.sendMessage({
        sender_id: "Something I guess",
        issue_deployment_order: {
          source_keep: this.selectedKeep,
          target_keep: id,
        },
      });
    }
  };

  onMessage = (message: Oneof_GameServerToPlayer) => {
    if (message.initial_state) {
      message.initial_state.keeps?.forEach((k) => {
        var keep = parseKeep(k);
        if (keep != null) {
          this.gameState.keeps.push(keep);
          const id = keep.id;
          this.keepLabels.set(
            keep.id,
            new Typeable(
              keep.name,
              () => this.handleTypeKeepName(id),
              this.canvas,
              this.ctx
            )
          );
        }
      });
      console.log(
        "Handled initial state. Keeps are this now",
        this.gameState.keeps
      );
    }
  };
}
