import { CONFIG } from "./config";

export class Connection {
  private socket: WebSocket;

  constructor() {
    this.socket = new WebSocket(CONFIG.WEBSOCKET_URL);

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
  }

  private handleClose(event: CloseEvent): void {
    console.log("WebSocket connection closed");
  }

  private handleError(error: Event): void {
    console.error("WebSocket error:", error);
  }
}
