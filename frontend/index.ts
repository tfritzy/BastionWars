import { serve } from "bun";
import { join } from "path";

serve({
  port: 7250,
  fetch(req) {
    const url = new URL(req.url);
    let path = url.pathname;

    if (path === "/") {
      path = "/index.html";
    }

    let filePath;
    if (path.startsWith("/dist/")) {
      // Serve compiled JS files from dist
      filePath = join(".", path);
    } else {
      // Serve other static files from public
      filePath = join("public", path);
    }

    try {
      const file = Bun.file(filePath);
      return new Response(file);
    } catch (error) {
      return new Response("Not Found", { status: 404 });
    }
  },
});

console.log(`Listening on http://localhost:7250`);
