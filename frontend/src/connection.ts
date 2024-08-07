import { CONFIG } from "./config";
import {
  decodeOneofMatchmakingUpdate,
  encodeOneofMatchmakingRequest,
  encodeOneofRequest,
  type OneofMatchmakingRequest,
} from "./Schema";

export class Connection {
  private socket: WebSocket;

  constructor(id: string) {
    this.socket = new WebSocket(CONFIG.WEBSOCKET_URL + "?id=" + id);

    this.socket.onopen = this.handleOpen.bind(this);
    this.socket.onmessage = this.handleMessage.bind(this);
    this.socket.onclose = this.handleClose.bind(this);
    this.socket.onerror = this.handleError.bind(this);
  }

  private handleOpen(event: Event): void {
    console.log("WebSocket connection established");
    this.socket.send("Hello, WebSocket server!");
  }

  private handleMessage(event: MessageEvent): void {
    console.log("Message received:", event.data);

    const reader = new FileReader();
    reader.onload = () => {
      if (reader.result instanceof ArrayBuffer) {
        const buffer = new Uint8Array(reader.result);
        const update = decodeOneofMatchmakingUpdate(buffer);

        console.log("Parsed update", update);
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

  sendMessage(message: OneofMatchmakingRequest) {
    console.log("Sending message", message);
    this.socket.send(encodeOneofMatchmakingRequest(message));
  }
}
