import { colors } from "./colors";
import {
  colorForAlliance,
  HALF_T,
  KEEP_LINE_STYLE,
  KEEP_LINE_WIDTH,
  Layer,
  SHADOW_COLOR,
  UNIT_AREA,
  WORLD_TO_CANVAS,
} from "./constants";
import { drawCircle, type Drawing, type DrawStyle } from "./drawing";
import type { Keep } from "./types";

const SHADOW_OFFSET = 4;

const keepLabel: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#444444",
  should_fill: true,
  font: "1.2em monospace",
  text_align: "center",
};
const archerBubbleStyle: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#d1fae5",
  stroke_style: "#064e3b",
  should_fill: true,
  should_stroke: true,
  line_width: 0.5,
};
const warriorBubbleStyle: DrawStyle = {
  layer: Layer.UI,
  fill_style: "#ffe4e6",
  stroke_style: "#881337",
  should_fill: true,
  should_stroke: true,
  line_width: 0.5,
};

const shadowStyle: DrawStyle = {
  layer: Layer.Keep,
  should_fill: true,
  fill_style: SHADOW_COLOR,
};

const centerBodyStyle: DrawStyle = {
  layer: Layer.Keep,
  should_fill: true,
  fill_style: "white",
  should_stroke: true,
  stroke_style: colors.slate[400],
};

const innerArea: DrawStyle = {
  layer: Layer.KeepTop,
  should_fill: true,
  fill_style: "white",
};

const innerShadow: DrawStyle = {
  layer: Layer.KeepTop,
  should_fill: true,
  fill_style: colors.slate[200],
  should_stroke: true,
  stroke_style: colors.slate[400],
};

export function drawKeep(drawing: Drawing, keep: Keep, deltaTime: number) {
  const x = Math.round(keep.pos.x * WORLD_TO_CANVAS);
  const y = Math.round(keep.pos.y * WORLD_TO_CANVAS);
  const stepSize = Math.PI / 5;

  // shadow
  drawing.drawCustom(shadowStyle, (ctx) => {
    drawCircle(ctx, x - SHADOW_OFFSET, y + SHADOW_OFFSET, 18);
  });

  // center body
  drawing.drawCustom(centerBodyStyle, (ctx) => {
    drawCircle(ctx, x, y, 18.75);
  });

  // arrow blockey thingies.
  drawing.drawCustom(
    {
      layer: Layer.Keep,
      should_fill: true,
      fill_style: colorForAlliance(keep.alliance)[300],
      should_stroke: true,
      stroke_style: colorForAlliance(keep.alliance)[500],
    },
    (ctx) => {
      ctx.moveTo(x, y);
      for (let i = 0; i < 10; i += 2) {
        ctx.arc(x, y, 18.75, stepSize * i, stepSize * (i + 1));
        ctx.closePath();
      }
    }
  );

  // Center shadow area.
  drawing.drawCustom(innerShadow, (ctx) => {
    drawCircle(ctx, x, y, 15);
  });

  drawing.drawCustom(innerArea, (ctx) => {
    drawCircle(ctx, x - 0.6, y + 0.6, 13.5);
  });

  //   if (keep.archer_count > keep.warrior_count) {
  //     drawing.drawCustom(archerBubbleStyle, (ctx) => {
  //       const r = Math.sqrt(
  //         ((keep.archer_count + keep.warrior_count) * UNIT_AREA) / Math.PI
  //       );
  //       drawCircle(ctx, x, y, r);
  //     });
  //     drawing.drawCustom(warriorBubbleStyle, (ctx) => {
  //       const r = Math.sqrt((keep.warrior_count * UNIT_AREA) / Math.PI);
  //       drawCircle(ctx, x, y, r);
  //     });
  //   } else {
  //     drawing.drawCustom(warriorBubbleStyle, (ctx) => {
  //       const r = Math.sqrt(
  //         ((keep.warrior_count + keep.archer_count) * UNIT_AREA) / Math.PI
  //       );
  //       drawCircle(ctx, x, y, r);
  //     });
  //     drawing.drawCustom(archerBubbleStyle, (ctx) => {
  //       const r = Math.sqrt((keep.archer_count * UNIT_AREA) / Math.PI);
  //       drawCircle(ctx, x, y, r);
  //     });
  //   }

  drawing.drawCustom(keepLabel, (ctx) => {
    ctx.fillText(keep.name, x, y - 30);
  });
}
