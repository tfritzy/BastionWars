import {
  BOUNDARY_LINE_STYLE,
  BOUNDARY_LINE_WIDTH,
  Layer,
  TILE_SIZE,
} from "./constants";
import { setDpr } from "./helpers";

type DrawFunction = (ctx: CanvasRenderingContext2D) => void;

export function drawCircle(
  ctx: CanvasRenderingContext2D,
  x: number,
  y: number,
  radius: number
) {
  ctx.moveTo(x + radius, y);
  ctx.arc(x, y, radius, 0, 2 * Math.PI);
}

export type DrawStyle = {
  should_fill?: boolean | undefined;
  should_stroke?: boolean | undefined;
  layer: number;
  stroke_style?: string | undefined;
  fill_style?: string | undefined;
  line_width?: number | undefined;
  text_align?: CanvasTextAlign | undefined;
  font?: string | undefined;
};

function drawStyleHashcode(drawStyle: DrawStyle): number {
  let hash = 17;

  hash = hash * 31 + (drawStyle.should_fill ? 1 : 0);
  hash = hash * 31 + (drawStyle.should_stroke ? 1 : 0);

  hash = hash * 31 + (drawStyle.layer ?? 0);
  hash = hash * 31 + (drawStyle.line_width ?? 0);

  hash = hash * 31 + (drawStyle.stroke_style?.length ?? 0);
  hash = hash * 31 + (drawStyle.fill_style?.length ?? 0);
  hash = hash * 31 + (drawStyle.text_align?.length ?? 0);
  hash = hash * 31 + (drawStyle.font?.length ?? 0);

  return hash;
}

function drawStyleEquals(
  drawStyle1: DrawStyle,
  drawStyle2: DrawStyle
): boolean {
  return (
    drawStyle1.should_fill === drawStyle2.should_fill &&
    drawStyle1.should_stroke === drawStyle2.should_stroke &&
    drawStyle1.layer === drawStyle2.layer &&
    drawStyle1.stroke_style === drawStyle2.stroke_style &&
    drawStyle1.fill_style === drawStyle2.fill_style &&
    drawStyle1.line_width === drawStyle2.line_width &&
    drawStyle1.text_align === drawStyle2.text_align &&
    drawStyle1.font === drawStyle2.font
  );
}

export class Drawing {
  private draw_queue: Map<number, [DrawStyle, DrawFunction[]][]>[] = [];
  private offscreen_canvases: Map<
    string,
    { canvas: HTMLCanvasElement; ctx: CanvasRenderingContext2D }
  > = new Map<
    string,
    { canvas: HTMLCanvasElement; ctx: CanvasRenderingContext2D }
  >();

  constructor() {}

  addToDrawQueue(style: DrawStyle, func: DrawFunction) {
    if (!this.draw_queue[style.layer]) {
      this.draw_queue[style.layer] = new Map<
        number,
        [DrawStyle, DrawFunction[]][]
      >();
      console.log(this.draw_queue);
    }

    const hashCode = drawStyleHashcode(style);
    if (!this.draw_queue[style.layer].has(hashCode)) {
      return this.draw_queue[style.layer].set(hashCode, [[style, [func]]]);
    } else {
      const tuple = this.draw_queue[style.layer]
        .get(hashCode)!
        .find((t) => drawStyleEquals(t[0], style));

      if (!tuple) {
        this.draw_queue[style.layer].get(hashCode)?.push([style, [func]]);
      } else {
        tuple[1].push(func);
      }
    }
  }

  drawWithOffscreen(
    canvasKey: string,
    targetX: number,
    targetY: number,
    layer: number,
    steps: (ctx: CanvasRenderingContext2D) => void
  ) {
    if (!this.offscreen_canvases.has(canvasKey)) {
      const canvas = document.createElement("canvas");
      canvas.id = canvasKey;
      document.body.appendChild(canvas);
      const ctx = canvas.getContext("2d")!;
      setDpr(canvas, ctx);
      this.offscreen_canvases.set(canvasKey, { canvas, ctx });
      steps(ctx);
    }

    this.addToDrawQueue({ layer }, (ctx) => {
      ctx.drawImage(
        this.offscreen_canvases.get(canvasKey)!.canvas,
        0,
        0,
        TILE_SIZE * (window.devicePixelRatio || 1),
        TILE_SIZE * (window.devicePixelRatio || 1),
        targetX,
        targetY,
        TILE_SIZE,
        TILE_SIZE
      );
    });
  }

  drawCustom(style: DrawStyle, steps: DrawFunction) {
    this.addToDrawQueue(style, steps);
  }

  drawFillable(style: string, layer: number, steps: DrawFunction) {
    if (!style) return;

    const drawStyle: DrawStyle = {
      fill_style: style,
      layer: layer,
      should_fill: true,
    };
    this.addToDrawQueue(drawStyle, steps);
  }

  drawBoundary(steps: DrawFunction) {
    const drawStyle: DrawStyle = {
      stroke_style: BOUNDARY_LINE_STYLE,
      line_width: BOUNDARY_LINE_WIDTH,
      layer: Layer.Map,
      should_stroke: true,
    };
    this.addToDrawQueue(drawStyle, steps);
  }

  drawStrokeable(
    style: string,
    line_width: number,
    layer: number,
    steps: DrawFunction
  ) {
    if (!style) return;

    const drawStyle: DrawStyle = {
      stroke_style: style,
      line_width: line_width,
      layer: layer,
      should_stroke: true,
    };
    this.addToDrawQueue(drawStyle, steps);
  }

  drawText(
    style: string,
    strokeStyle: string | undefined,
    line_width: number,
    align: CanvasTextAlign,
    font: string,
    steps: DrawFunction
  ) {
    const drawStyle: DrawStyle = {
      fill_style: style,
      stroke_style: strokeStyle,
      line_width: line_width,
      layer: Layer.UI,
      should_fill: true,
      should_stroke: !!strokeStyle,
      text_align: align,
      font: font,
    };
    this.addToDrawQueue(drawStyle, steps);
  }

  draw(ctx: CanvasRenderingContext2D) {
    for (const layer of this.draw_queue) {
      if (!layer) continue;
      for (const [_, drawTuples] of layer) {
        for (const drawTuple of drawTuples) {
          ctx.save();
          if (drawTuple[0].fill_style) ctx.fillStyle = drawTuple[0].fill_style;
          if (drawTuple[0].stroke_style)
            ctx.strokeStyle = drawTuple[0].stroke_style;
          if (drawTuple[0].line_width) ctx.lineWidth = drawTuple[0].line_width;
          if (drawTuple[0].text_align) ctx.textAlign = drawTuple[0].text_align;
          if (drawTuple[0].font) ctx.font = drawTuple[0].font;

          ctx.beginPath();
          drawTuple[1].forEach((func) => {
            func(ctx);
          });

          if (drawTuple[0].should_fill) ctx.fill();

          if (drawTuple[0].should_stroke) ctx.stroke();
          drawTuple[1].length = 0;
          ctx.restore();
        }
      }
    }
  }
}
