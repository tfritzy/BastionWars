import { Layer } from "./constants";
import type { Drawing, DrawStyle } from "./drawing";

const style: DrawStyle = {
 layer: Layer.UI,
 fill_style: "#444444",
 font: "bold 12px Verdena",
 should_fill: true,
 text_align: "center",
};

export class ResourceLabel {
 private text: string;
 private progress: number;
 private boundHandleKeyDown: (event: KeyboardEvent) => void;
 private onComplete: () => void;
 private drawing: Drawing;

 constructor(text: string, drawing: Drawing, onComplete: () => void) {
  this.text = text;
  this.progress = 0;
  this.drawing = drawing;
  this.onComplete = onComplete;

  this.boundHandleKeyDown = this.handleKeyDown.bind(this);
  document.addEventListener("keydown", this.boundHandleKeyDown);
 }

 draw(x: number, y: number, deltaTime: number): void {
  this.drawing.drawCustom(style, (ctx) => {
   ctx.fillText(this.text[this.progress], x, y);
  });
 }

 public removeEventListener(): void {
  document.removeEventListener("keydown", this.boundHandleKeyDown);
 }

 private handleKeyDown(event: KeyboardEvent) {
  if (this.text[this.progress] === event.key) {
   this.progress += 1;

   if (this.progress == this.text.length) {
    this.onComplete();
    this.progress = 0;
   }
  }
 }
}
