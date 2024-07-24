// shapes.js
export function drawPentagon(ctx, centerX, centerY, radius, fillStyle) {
  ctx.fillStyle = fillStyle;
  ctx.beginPath();
  for (let i = 0; i < 5; i++) {
    const angle = (i * 2 * Math.PI) / 5 - Math.PI / 2;
    const x = centerX + Math.cos(angle) * radius;
    const y = centerY + Math.sin(angle) * radius;
    ctx.lineTo(x, y);
  }
  ctx.closePath();
  ctx.fill();
}
