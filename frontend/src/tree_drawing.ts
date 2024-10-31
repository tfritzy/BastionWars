import {
 HALF_T,
 Layer,
 SHADOW_COLOR,
 TILE_SIZE,
 TREE_LINE_COLOR,
 TREE_LINE_WIDTH,
} from "./constants";
import { type Drawing, type DrawStyle } from "./drawing";
import { type Vector2 } from "./types";

const SHADOW_OFFSET = 3;

const treeLayerConfigs = [
 { numBumps: 6, radius: 10, bumpRadius: 6 },
 { numBumps: 5, radius: 6.5, bumpRadius: 4.4 },
] as const;

const createTreeStyle = (layer: number): DrawStyle => ({
 layer,
 fill_style: "white",
 line_width: TREE_LINE_WIDTH,
 should_stroke: true,
 should_fill: true,
 stroke_style: TREE_LINE_COLOR,
});

const createShadowStyle = (layer: number, fillStyle: string): DrawStyle => ({
 layer,
 fill_style: fillStyle,
 should_fill: true,
});

const styles = {
 treeBottom: createTreeStyle(Layer.TreeBottoms),
 treeTop: createTreeStyle(Layer.TreeTops),
 shadowBottom: createShadowStyle(Layer.TreeShadows, SHADOW_COLOR),
 shadowTop: createShadowStyle(Layer.TreeTops, "#dddddd"),
};

const treeTopStyle: DrawStyle = {
 layer: Layer.TreeTops,
 line_width: 0.5,
 should_stroke: true,
 stroke_style: "black",
};

const circlePoint = (
 theta: number,
 center: Vector2,
 radius: number
): Vector2 => ({
 x: Math.cos(theta) * radius + center.x,
 y: Math.sin(theta) * radius + center.y,
});

const generateTreePoints = (
 center: Vector2,
 config: (typeof treeLayerConfigs)[number],
 offset: boolean
) => {
 const { numBumps, radius, bumpRadius } = config;
 const peakRadius = radius + bumpRadius;
 const stepSize = (2 * Math.PI) / numBumps;
 const halfStep = stepSize / 2;
 let theta = offset ? stepSize / 2 : 0;
 theta += (center.x * 31 + center.y * 17) % (2 * Math.PI);

 const points: Vector2[] = [];
 const controlPoints: Vector2[] = [];
 const start = circlePoint(theta, center, radius);

 for (let i = 1; i < numBumps; i++) {
  theta += stepSize;
  points.push(circlePoint(theta, center, radius));
  controlPoints.push(circlePoint(theta - halfStep, center, peakRadius));
 }

 theta += stepSize;
 const endControlPoint = circlePoint(theta - halfStep, center, peakRadius);

 return { start, points, controlPoints, endControlPoint };
};

const treePoints = [
 generateTreePoints({ x: 0, y: 0 }, treeLayerConfigs[0], true),
 generateTreePoints({ x: 0, y: 0 }, treeLayerConfigs[1], true),
];

const drawTreePart = (
 ctx: CanvasRenderingContext2D,
 style: DrawStyle,
 {
  start,
  points,
  controlPoints,
  endControlPoint,
 }: ReturnType<typeof generateTreePoints>,
 bumpRadius: number,
 xOffset: number,
 yOffset: number
) => {
 ctx.lineWidth = style.line_width || 0;
 ctx.fillStyle = style.fill_style || "";
 ctx.strokeStyle = style.stroke_style || "";
 ctx.beginPath();
 ctx.moveTo(start.x + xOffset, start.y + yOffset);
 for (let i = 0; i < points.length; i++) {
  ctx.arcTo(
   controlPoints[i].x + xOffset,
   controlPoints[i].y + yOffset,
   points[i].x + xOffset,
   points[i].y + yOffset,
   bumpRadius
  );
 }
 ctx.arcTo(
  endControlPoint.x + xOffset,
  endControlPoint.y + yOffset,
  start.x + xOffset,
  start.y + yOffset,
  bumpRadius
 );
 ctx.closePath();
 if (style.should_fill) ctx.fill();
 if (style.should_stroke) ctx.stroke();
};

export function drawTree(
 drawing: Drawing,
 gridX: number,
 gridY: number,
 time: number
) {
 drawing.drawWithOffscreen(
  "tree",
  Math.round(gridX * TILE_SIZE),
  Math.round(gridY * TILE_SIZE),
  Layer.TreeBottoms,
  (ctx) => {
   const random = HALF_T * 17 + HALF_T * 31;
   const bottomPoints = treePoints[0];

   drawTreePart(
    ctx,
    styles.shadowBottom,
    bottomPoints,
    treeLayerConfigs[0].bumpRadius,
    HALF_T - SHADOW_OFFSET,
    HALF_T + SHADOW_OFFSET
   );

   drawTreePart(
    ctx,
    styles.treeBottom,
    bottomPoints,
    treeLayerConfigs[0].bumpRadius,
    HALF_T,
    HALF_T
   );

   const topPoints = treePoints[1];
   drawTreePart(
    ctx,
    styles.shadowTop,
    topPoints,
    treeLayerConfigs[1].bumpRadius,
    HALF_T - 2,
    HALF_T + 2
   );
   drawTreePart(
    ctx,
    styles.treeTop,
    topPoints,
    treeLayerConfigs[1].bumpRadius,
    HALF_T,
    HALF_T
   );
  }
 );
}
