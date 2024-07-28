export const CONFIG = {
  WEBSOCKET_URL:
    location.hostname === "localhost" || location.hostname === "127.0.0.1"
      ? "ws://localhost:7249/ws"
      : "wss://prod-websocket-url",
};
