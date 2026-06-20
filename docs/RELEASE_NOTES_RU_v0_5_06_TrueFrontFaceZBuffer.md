# v0.5.06 — TrueFrontFaceZBuffer

Build name: `TRUE_FRONT_FACE_ZBUFFER`

Новый режим по умолчанию:

`Mode: Mesh True Front — v0.5.06`

## Найденная причина

Лог v0.5.05 показывал:

- `SurfaceAlpha=255`;
- `OpaqueSurfacePixels=135012`;
- `NonOpaqueSurfacePixels=0`.

Следовательно, эффект прозрачности не мог быть alpha blending.

В `ProjectPrimitivePointView` большее `viewZ` находится ближе к камере, и Z-buffer использует `GreaterViewZIsNear`. Значит наружная грань, направленная к камере, имеет `NormalZ > 0`. Legacy renderer делал наоборот:

```text
NormalZ > 0 => BackFacing => culled
```

В результате настоящая ближайшая оболочка удалялась, а тот же экранный силуэт заполнялся более дальней противоположной/внутренней оболочкой. Пиксели были непрозрачными, но геометрически показывалась неправильная сторона тела.

## Исправление

- `FrontFaceRule=NormalZPositive`;
- `RasterizeAllTrianglesForDepth=True`;
- nearest depth выигрывает независимо от порядка triangles;
- `SurfaceAlpha=255`;
- camera-facing light terms используют `+normalZ`;
- `DepthCueStrength=0`;
- mesh depth neighborhood `0`, чтобы соседний более близкий пиксель не вырезал наружный каркас;
- mesh visible ratio `0.55`;
- mesh tolerance `0.0025 × modelSize`.

## Новые поля лога

- `FrontFacingPositiveViewZ`;
- `RasterizeAllTrianglesForDepth`;
- `FrontFaceRule`;
- `ViewDepthNearRule`;
- `FrontFacingTriangles`;
- `DepthPassTriangles`;
- `NearestPositiveNormalZPixels`;
- `NearestNegativeNormalZPixels`;
- `LegacyRuleWouldCullNearestPixels`.

## Численная проверка supplied Rubber Hose STEP

Default camera `yaw=-35°, pitch=22°`:

- covered pixels: `135012`;
- nearest surface with `NormalZ > 0`: `135012`;
- nearest surface with `NormalZ < 0`: `0`;
- legacy rule would cull nearest pixels: `135012`;
- predicted visible POLY_LOOP edges with new depth rule: approximately `1098`.

Проверены дополнительные camera angles: `(0,0)`, `(45,20)`, `(90,0)`, `(-90,30)`, `(180,-15)`, `(20,60)`. Во всех случаях каждый покрытый nearest pixel принадлежал triangle с `NormalZ > 0`.

Финальный camera-correct gray profile использует clamp `165..220`: передняя оболочка остаётся светлой как в Inventor, а реальная внутренняя полость затемняется направлением нормали, а не прозрачностью или искусственным depth cue.
