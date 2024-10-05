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
 { numBumps: 6, radius: 20, bumpRadius: 12 },
 { numBumps: 5, radius: 13, bumpRadius: 8.8 },
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
 drawing: Drawing,
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
 drawing.drawCustom(style, (ctx) => {
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
 });
};

export function drawTree(
 drawing: Drawing,
 gridX: number,
 gridY: number,
 time: number
) {
 const center: Vector2 = {
  x: Math.round(gridX * TILE_SIZE) + HALF_T,
  y: Math.round(gridY * TILE_SIZE) + HALF_T,
 };
 const random = center.x * 17 + center.y * 31;
 center.x += Math.sin(random) * 6;
 center.y += Math.cos(random) * 6;
 const wind = (Math.sin(time + gridX + gridY) / 2 + 0.5) * 1.25;
 const bottomPoints = treePoints[0];
 drawTreePart(
  drawing,
  styles.treeBottom,
  bottomPoints,
  treeLayerConfigs[0].bumpRadius,
  center.x + -wind,
  center.y + wind
 );
 drawTreePart(
  drawing,
  styles.shadowBottom,
  bottomPoints,
  treeLayerConfigs[0].bumpRadius,
  center.x + -wind - SHADOW_OFFSET,
  center.y + wind + SHADOW_OFFSET
 );

 const topWind = wind * 2;
 const topPoints = treePoints[1];
 drawTreePart(
  drawing,
  styles.shadowTop,
  topPoints,
  treeLayerConfigs[1].bumpRadius,
  center.x + -topWind - 2,
  center.y + topWind + 2
 );
 drawTreePart(
  drawing,
  styles.treeTop,
  topPoints,
  treeLayerConfigs[1].bumpRadius,
  center.x + -topWind,
  center.y + topWind
 );
}
