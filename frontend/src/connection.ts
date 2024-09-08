import { CONFIG } from "./config";
import {
  decodeOneof_GameServerToPlayer,
  encodeOneof_PlayerToGameServer,
  type Oneof_GameServerToPlayer,
  type Oneof_PlayerToGameServer,
} from "./Schema";

export class Connection {
  private socket: WebSocket;
  private onMessage: (message: Oneof_GameServerToPlayer) => void;
  private playerId: string;
  private authToken: string;

  constructor(
    address: string,
    playerId: string,
    token: string,
    onMessage: (message: Oneof_GameServerToPlayer) => void
  ) {
    console.log("Attempting to open a connection", address);
    this.socket = new WebSocket(
      `${address}?playerId=${playerId}&authToken=${token}`
    );
    this.playerId = playerId;
    this.authToken = token;
    this.onMessage = onMessage;

    this.socket.onopen = this.handleOpen.bind(this);
    this.socket.onmessage = this.handleMessage.bind(this);
    this.socket.onclose = this.handleClose.bind(this);
    this.socket.onerror = this.handleError.bind(this);
  }

  private handleOpen(event: Event): void {
    console.log("WebSocket connection established");
    this.socket.send("Hello, game server!");
  }

  private handleMessage(event: MessageEvent): void {
    const reader = new FileReader();
    reader.onload = () => {
      if (reader.result instanceof ArrayBuffer) {
        const buffer = new Uint8Array(reader.result);
        const update = decodeOneof_GameServerToPlayer(buffer);

        this.onMessage(update);
      }
    };
    reader.readAsArrayBuffer(event.data);
  }

  private handleClose(event: CloseEvent): void {
    console.log("WebSocket connection closed");
  }

  private handleError(error: Event): void {
    console.error("WebSocket error:", error);
  }

  sendMessage(message: Oneof_PlayerToGameServer) {
    console.log("Sending message", message);
    message.sender_id = this.playerId;
    message.auth_token = this.authToken;
    this.socket.send(encodeOneof_PlayerToGameServer(message));
  }
}
