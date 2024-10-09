import { Layer, TILE_SIZE } from "./constants";
import type { Drawing, DrawStyle } from "./drawing";
import { drawProgressBar } from "./drawing/progress_bar";
import { deleteBySwap } from "./helpers";
import { normalize, type Vector2 } from "./types";

const style: DrawStyle = {
 layer: Layer.UI,
 fill_style: "#444444",
 font: "bold 12px Verdena",
 should_fill: true,
 text_align: "center",
};

const sphereStyle: DrawStyle = {
 layer: Layer.UI,
 fill_style: "white",
 stroke_style: "#333333",
 should_fill: true,
 should_stroke: true,
};

const width = 35;
const padding = 4;

export class ResourceLabel {
 private text: string;
 private progress: number;
 private lerpedProgress: number;
 private boundHandleKeyDown: (event: KeyboardEvent) => void;
 private onComplete: () => void;
 private drawing: Drawing;
 private timeSinceCompleted: number;
 private spherePositions: Vector2[];
 private sphereVelocities: Vector2[];
 private keepPos: Vector2;
 private sphereSpeed: number;

 constructor(
  text: string,
  drawing: Drawing,
  keepPos: Vector2,
  onComplete: () => void
 ) {
  this.text = text.toUpperCase();
  this.progress = 0;
  this.lerpedProgress = 0;
  this.timeSinceCompleted = -1;
  this.drawing = drawing;
  this.onComplete = onComplete;
  this.spherePositions = [];
  this.sphereVelocities = [];
  this.keepPos = keepPos;
  this.sphereSpeed = 0;

  this.boundHandleKeyDown = this.handleKeyDown.bind(this);
  document.addEventListener("keydown", this.boundHandleKeyDown);
 }

 draw(x: number, y: number, deltaTime: number): void {
  if (this.lerpedProgress < this.progress) {
   this.lerpedProgress += deltaTime * 5;
  }

  this.drawing.drawCustom(style, (ctx) => {
   ctx.fillText(this.text[this.progress], x, y);
  });

  const progress = this.progress / this.text.length;
  const lerpedProgress = this.lerpedProgress / this.text.length;
  drawProgressBar(
   this.drawing,
   x - width / 2 + padding,
   y + 8,
   width - padding * 2,
   5,
   2.75,
   progress,
   lerpedProgress
  );

  if (this.timeSinceCompleted >= 0) {
   this.timeSinceCompleted += deltaTime;
   this.sphereSpeed += deltaTime * 200;
   this.sphereSpeed = Math.min(this.sphereSpeed, 200);

   for (let i = 0; i < this.spherePositions.length; i++) {
    const delta = {
     x: this.keepPos.x * TILE_SIZE - (x + this.spherePositions[i].x),
     y: this.keepPos.y * TILE_SIZE - (y + this.spherePositions[i].y),
    };

    if (Math.abs(delta.x) < 10 && Math.abs(delta.y) < 10) {
     deleteBySwap(this.spherePositions, i);
     deleteBySwap(this.sphereVelocities, i);

     if (this.spherePositions.length == 0) {
      this.timeSinceCompleted = -1;
     }

     return;
    }

    const dir = normalize(delta);
    this.spherePositions[i].x += dir.x * this.sphereSpeed * deltaTime;
    this.spherePositions[i].y += dir.y * this.sphereSpeed * deltaTime;
   }
   if (this.timeSinceCompleted > 5) {
    this.timeSinceCompleted = -1;
   }

   for (let i = 0; i < this.spherePositions.length; i++) {
    this.drawing.drawCustom(sphereStyle, (ctx) => {
     ctx.moveTo(
      x + this.spherePositions[i].x + 3,
      y + this.spherePositions[i].y
     );
     ctx.arc(
      x + this.spherePositions[i].x,
      y + this.spherePositions[i].y,
      3,
      0,
      Math.PI * 2
     );
    });
   }
  }
 }

 public removeEventListener(): void {
  document.removeEventListener("keydown", this.boundHandleKeyDown);
 }

 private handleKeyDown(event: KeyboardEvent) {
  if (this.text[this.progress].toLowerCase() === event.key.toLowerCase()) {
   this.progress += 1;

   if (this.progress == this.text.length) {
    this.onComplete();
    this.timeSinceCompleted = 0;

    const arcSize = (Math.PI * 2) / this.text.length;
    for (let i = 0; i < 1; i++) {
     this.spherePositions.push({ x: 0, y: 0 });
     this.sphereVelocities.push({
      x: Math.cos(arcSize * i) * 50,
      y: Math.sin(arcSize * i) * 50,
     });
    }
   }
  }
 }
}
