# Inventor IPT Organizer v0.5.15 — RENDER_FPS_GROUP_FIX

Исправлена историческая группировка двух независимых списков.

## Mode

- `Mode: Mesh True Front — v0.5.06` — происхождение правильного True Front алгоритма.
- `Mode: Mesh True Front (compile fix) — v0.5.07` — тот же алгоритм после исправления CS0136.

## FPS

FPS/backbuffer появился только в v0.5.08, поэтому второй список содержит строго:

- v0.5.08 Drag Backbuffer;
- v0.5.09 Camera Center Lock;
- v0.5.10 DPI Center Fix;
- v0.5.11 Visible Viewport Center;
- v0.5.12 Adaptive Direct Swap;
- v0.5.13 Cursor Sampled Loop.

Пункты FPS v0.5.06 и v0.5.07 удалены как исторически неверные.
