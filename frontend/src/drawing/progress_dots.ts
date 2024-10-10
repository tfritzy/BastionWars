import { Layer } from "../constants";
import type { Drawing, DrawStyle } from "../drawing";

const unfilledDot: DrawStyle = {
  layer: Layer.UI,
  fill_style: "white",
  should_fill: true,
  stroke_style: "#444444",
  should_stroke: true,
  line_width: 0.5,
};
const filledDot: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#444444",
  should_fill: true,
};
const dotRadius = 2;
const stepSize = 5;

export function drawDots(
  drawing: Drawing,
  x: number,
  y: number,
  progress: number,
  max: number
) {
  for (let i = 0; i < max; i++) {
    const isFilled = progress > i;
    const width = (max - 1) * stepSize;
    const dotX = x - width / 2 + i * stepSize;
    if (isFilled) {
      drawing.drawCustom(filledDot, (ctx) => {
        ctx.moveTo(dotX + dotRadius, y);
        ctx.arc(dotX, y, dotRadius, 0, Math.PI * 2);
      });
    } else {
      drawing.drawCustom(unfilledDot, (ctx) => {
        ctx.moveTo(dotX + dotRadius, y);
        ctx.arc(dotX, y, dotRadius, 0, Math.PI * 2);
      });
    }
  }
}
