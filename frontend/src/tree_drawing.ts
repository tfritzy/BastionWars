import {
 HALF_T,
 Layer,
 SHADOW_COLOR,
 TILE_SIZE,
 WORLD_TO_CANVAS,
} from "./constants";
import { drawCircle, type Drawing, type DrawStyle } from "./drawing";
import { seededRandom } from "./random";
import { addV3, type Vector2 } from "./types";

const NUM_BUMPS = [9, 7, 5];
const RADIUS = [20, 14, 9];
const BUMP_R = [8, 7, 5.8];

const treeStyle: DrawStyle = {
 layer: Layer.TreeBottoms,
 fill_style: "white",
 line_width: 0.5,
 should_stroke: true,
 should_fill: true,
 stroke_style: "black",
};

const treeTopStyle: DrawStyle = {
 ...treeStyle,
 layer: Layer.TreeTops,
};

const baseShadowStyle: DrawStyle = {
 layer: Layer.TreeShadows,
 fill_style: SHADOW_COLOR,
 should_fill: true,
};

const topShadowStyle: DrawStyle = {
 layer: Layer.TreeTopShadows,
 fill_style: "#eeeeef",
 should_fill: true,
};

function circlePoint(theta: number, x: number, y: number, radius: number) {
 return {
  x: Math.cos(theta) * radius + x,
  y: Math.sin(theta) * radius + y,
 };
}

export function drawTree(
 drawing: Drawing,
 grid_x: number,
 grid_y: number,
 deltaTime: number
) {
 const pos = {
  x: Math.round(grid_x * TILE_SIZE) + HALF_T,
  y: Math.round(grid_y * TILE_SIZE) + HALF_T,
 };

 for (let i = 0; i < 2; i++) {
  drawTreeLayer(
   drawing,
   pos,
   RADIUS[i],
   NUM_BUMPS[i],
   BUMP_R[i],
   i % 2 == 0,
   i === 0,
   deltaTime
  );
 }
}

export function drawTreeLayer(
 drawing: Drawing,
 pos: Vector2,
 radius: number,
 num_bumps: number,
 bump_r: number,
 offset: boolean,
 is_bottom: boolean,
 deltaTime: number
) {
 const peak_r = radius + bump_r;
 const step_size = (2 * Math.PI) / num_bumps;
 const half_step = step_size / 2;

 let theta = offset ? step_size / 2 : 0;

 let points = [];
 let control_points = [];

 const start = circlePoint(theta, pos.x, pos.y, radius);
 for (let i = 1; i < num_bumps; i++) {
  theta += step_size;
  points.push(circlePoint(theta, pos.x, pos.y, radius));
  control_points.push(circlePoint(theta - half_step, pos.x, pos.y, peak_r));
 }
 theta += step_size;
 const end_cp = circlePoint(theta - half_step, pos.x, pos.y, peak_r);

 const bodyStyle = is_bottom ? treeStyle : treeTopStyle;
 drawing.drawCustom(bodyStyle, (ctx) => {
  ctx.moveTo(start.x, start.y);
  for (let i = 0; i < points.length; i++) {
   ctx.arcTo(
    control_points[i].x,
    control_points[i].y,
    points[i].x,
    points[i].y,
    bump_r
   );
  }
  ctx.arcTo(end_cp.x, end_cp.y, start.x, start.y, bump_r);
  ctx.closePath();
 });

 const shadowStyle = is_bottom ? baseShadowStyle : topShadowStyle;
 drawing.drawCustom(shadowStyle, (ctx) => {
  ctx.moveTo(start.x - 5, start.y + 5);
  for (let i = 0; i < points.length; i++) {
   ctx.arcTo(
    control_points[i].x - 5,
    control_points[i].y + 5,
    points[i].x - 5,
    points[i].y + 5,
    bump_r
   );
  }
  ctx.arcTo(end_cp.x - 5, end_cp.y + 5, start.x - 5, start.y + 5, bump_r);
  ctx.closePath();
 });
}
