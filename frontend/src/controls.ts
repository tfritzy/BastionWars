export class CanvasControls {
  private canvas: HTMLCanvasElement;
  private draw: () => void;
  private scale: number;
  private offsetX: number;
  private offsetY: number;
  private isDragging: boolean;
  private lastX: number;
  private lastY: number;

  constructor(canvas: HTMLCanvasElement, draw: () => void) {
    this.canvas = canvas;
    this.draw = draw;
    this.scale = 1;
    this.offsetX = 0;
    this.offsetY = 0;
    this.isDragging = false;
    this.lastX = 0;
    this.lastY = 0;
    this.setupEventListeners();
  }

  private setupEventListeners(): void {
    this.canvas.addEventListener("wheel", this.zoom.bind(this));
    this.canvas.addEventListener("mousedown", this.startDrag.bind(this));
    this.canvas.addEventListener("mousemove", this.drag.bind(this));
    this.canvas.addEventListener("mouseup", this.endDrag.bind(this));
  }

  private zoom(event: WheelEvent): void {
    event.preventDefault();
    const rect = this.canvas.getBoundingClientRect();
    const mouseX = event.clientX - rect.left;
    const mouseY = event.clientY - rect.top;
    const wheel = event.deltaY < 0 ? 1 : -1;
    const zoom = Math.exp(wheel * 0.1);
    const newScale = this.scale * zoom;
    if (newScale > 0.1 && newScale < 10) {
      const focusX = mouseX / this.scale - this.offsetX;
      const focusY = mouseY / this.scale - this.offsetY;
      this.offsetX -= mouseX / newScale - focusX;
      this.offsetY -= mouseY / newScale - focusY;
      this.scale = newScale;
      this.draw();
    }
  }

  private startDrag(event: MouseEvent): void {
    this.isDragging = true;
    this.lastX = event.clientX;
    this.lastY = event.clientY;
    this.canvas.style.cursor = "grabbing";
  }

  private drag(event: MouseEvent): void {
    if (this.isDragging) {
      const deltaX = event.clientX - this.lastX;
      const deltaY = event.clientY - this.lastY;
      this.offsetX += deltaX / this.scale;
      this.offsetY += deltaY / this.scale;
      this.lastX = event.clientX;
      this.lastY = event.clientY;
      this.draw();
    }
  }

  private endDrag(): void {
    this.isDragging = false;
    this.canvas.style.cursor = "default";
  }

  public getTransform(): { scale: number; offsetX: number; offsetY: number } {
    return {
      scale: this.scale,
      offsetX: this.offsetX,
      offsetY: this.offsetY,
    };
  }
}
