type DrawFunction = (ctx: CanvasRenderingContext2D) => void;

export class Drawing {
  private fill_queue = new Map<string, DrawFunction[]>();
  private stroke_queue = new Map<string, DrawFunction[]>();

  constructor() {}

  drawFillable(style: string, steps: DrawFunction) {
    if (!this.fill_queue.has(style)) this.fill_queue.set(style, []);
    this.fill_queue.get(style)!.push(steps);
  }

  drawStrokeable(style: string, line_width: number, steps: DrawFunction) {
    if (!this.stroke_queue.has(style)) this.stroke_queue.set(style, []);
    this.stroke_queue.get(style)!.push((ctx) => {
      ctx.lineWidth = line_width;
      steps(ctx);
    });
  }

  drawText(style: string, align: CanvasTextAlign, steps: DrawFunction) {
    this.drawFillable(style, (ctx) => {
      ctx.textAlign = align;
      steps(ctx);
    });
  }

  draw(ctx: CanvasRenderingContext2D) {
    ctx.save();
    for (const [fill, seq] of this.fill_queue) {
      if (fill) {
        ctx.fillStyle = fill;
        ctx.beginPath();
        seq?.forEach((func) => {
          func(ctx);
        });

        ctx.fill();
      }

      // Clear list without garbage collecting.
      this.fill_queue.get(fill)!.length = 0;
    }
    ctx.restore();

    ctx.save();
    for (const [stroke, seq] of this.stroke_queue) {
      ctx.strokeStyle = stroke;
      ctx.beginPath();
      seq?.forEach((func) => {
        func(ctx);
      });
      ctx.stroke();

      // Clear list without garbage collecting.
      this.stroke_queue.get(stroke)!.length = 0;
    }
    ctx.restore();
  }
}
