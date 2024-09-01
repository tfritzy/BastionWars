export class CanvasControls {
  private canvas: HTMLCanvasElement;
  private draw: () => void;
  private offsetX: number;
  private offsetY: number;
  private isDragging: boolean;
  private lastX: number;
  private lastY: number;

  constructor(canvas: HTMLCanvasElement, draw: () => void) {
    this.canvas = canvas;
    this.draw = draw;
    this.offsetX = 0;
    this.offsetY = 0;
    this.isDragging = false;
    this.lastX = 0;
    this.lastY = 0;
    this.setupEventListeners();
  }

  private setupEventListeners(): void {
    this.canvas.addEventListener("mousedown", this.startDrag.bind(this));
    this.canvas.addEventListener("mousemove", this.drag.bind(this));
    this.canvas.addEventListener("mouseup", this.endDrag.bind(this));
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
      this.offsetX += deltaX;
      this.offsetY += deltaY;
      this.lastX = event.clientX;
      this.lastY = event.clientY;
      this.draw();
    }
  }

  private endDrag(): void {
    this.isDragging = false;
    this.canvas.style.cursor = "default";
  }

  public getTransform(): { offsetX: number; offsetY: number } {
    return {
      offsetX: this.offsetX,
      offsetY: this.offsetY,
    };
  }
}
