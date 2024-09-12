import { Connection } from "./connection.ts";
import { soldierColors, WORLD_TO_CANVAS } from "./constants.ts";
import { drawLandTile } from "./grid_drawing.ts";
import { drawKeep } from "./rendering.ts";
import {
  RenderTileType,
  SoldierType,
  type AllKeepUpdates,
  type AllSoldierPositions,
  type GameFoundForPlayer,
  type InitialState,
  type KeepUpdate,
  type Oneof_GameServerToPlayer,
} from "./Schema.ts";
import { Typeable } from "./typeable.ts";
import {
  initialGameState,
  parseKeep,
  parseSoldier,
  updateSoldier,
  type GameState,
  type Keep,
} from "./types.ts";

export class Game {
  private canvas: HTMLCanvasElement;
  private ctx: CanvasRenderingContext2D;
  private connection: Connection | undefined;
  private gameState: GameState = initialGameState;
  private keepLabels: Map<number, Typeable> = new Map();
  private selectedKeep: number | null = null;

  constructor(canvas: HTMLCanvasElement, ctx: CanvasRenderingContext2D) {
    this.canvas = canvas;
    this.ctx = ctx;
  }

  draw(dpr: number, deltaTime: number): void {
    this.ctx.save();

    this.drawLandAndWater();
    this.drawKeeps(deltaTime);
    this.drawSoldiers();

    this.ctx.restore();
  }

  drawKeeps(deltaTime: number) {
    this.ctx.save();

    this.gameState.keeps.forEach((keep) => {
      const x = keep.pos.x * WORLD_TO_CANVAS;
      const y = keep.pos.y * WORLD_TO_CANVAS;
      drawKeep(this.ctx, keep, deltaTime);
      this.keepLabels.get(keep.id)?.draw(x, y - 25, deltaTime);
    });

    this.ctx.restore();
  }

  drawLandAndWater() {
    this.ctx.save();

    const tileSize = WORLD_TO_CANVAS;
    const cornerRadius = tileSize / 8;

    this.gameState.renderTiles.forEach((tileType, index) => {
      const x =
        (index % (this.gameState.mapWidth + 1)) * tileSize - tileSize / 2;
      const y =
        Math.floor(index / (this.gameState.mapWidth + 1)) * tileSize -
        tileSize / 2;

      drawLandTile(this.ctx, x, y, tileSize, cornerRadius, tileType);
    });

    this.ctx.restore();
  }

  drawSoldiers() {
    this.ctx.save();

    const radius = 4;
    this.gameState.soldiers.forEach((keep) => {
      const x = keep.pos.x * WORLD_TO_CANVAS;
      const y = keep.pos.y * WORLD_TO_CANVAS;

      this.ctx.beginPath();
      this.ctx.arc(x, y, radius, 0, 2 * Math.PI);
      this.ctx.stroke();
    });

    this.ctx.restore();
  }

  connectToGameServer(details: GameFoundForPlayer) {
    this.connection = new Connection(
      details.address!,
      details.player_id!,
      details.auth_token!,
      this.onMessage
    );
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
      this.selectedKeep = null;
    }
  };

  onMessage = (message: Oneof_GameServerToPlayer) => {
    if (message.initial_state) {
      this.handleInitialStateMsg(message.initial_state);
    } else if (message.all_soldier_positions) {
      this.handleSoldierPositionsMsg(message.all_soldier_positions);
    } else if (message.keep_updates) {
      this.handleKeepUpdates(message.keep_updates);
    }
  };

  handleInitialStateMsg(msg: InitialState) {
    console.log("initial state", msg);
    msg.keeps?.forEach((k) => {
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

    this.gameState.tiles = msg.tiles || [];
    this.gameState.renderTiles = msg.render_tiles || [];
    this.gameState.mapHeight = msg.map_height || 0;
    this.gameState.mapWidth = msg.map_width || 0;
  }

  handleSoldierPositionsMsg(msg: AllSoldierPositions) {
    const currentTime = Date.now();
    msg.soldier_positions?.forEach((s) => {
      const existingSoldier = s.id && this.gameState.soldiers.get(s.id);
      if (!existingSoldier) {
        const soldier = parseSoldier(s, currentTime);
        if (soldier) {
          this.gameState.soldiers.set(soldier.id, soldier);
        }
      } else {
        updateSoldier(existingSoldier, s, currentTime);
      }
    });

    for (let [key, value] of this.gameState.soldiers) {
      if (value.lastUpdated !== currentTime) {
        this.gameState.soldiers.delete(key);
      }
    }
  }

  handleKeepUpdates(msg: AllKeepUpdates) {
    msg.keep_updates?.forEach((ku) => {
      const keepIndex = this.gameState.keeps.findIndex((k) => k.id === ku.id);
      if (keepIndex >= 0) {
        this.gameState.keeps[keepIndex].archer_count = ku.archer_count || 0;
        this.gameState.keeps[keepIndex].warrior_count = ku.warrior_count || 0;
      }
    });
  }
}
