# v0.5.05 — SolidFrontMeshFix

Build name: `SOLID_FRONT_MESH_FIX`

Новый режим по умолчанию:

`Mode: Mesh Solid Front — v0.5.05`

## Глубокая причина проблемы v0.5.04

Пользовательский лог v0.5.04 доказал, что поверхность уже была математически непрозрачной:

- `SurfaceAlpha=255`;
- `SurfaceOpaque=True`;
- `OpaqueBody=True`.

Поэтому ощущение прозрачности создавалось не alpha-blending. Одновременно жёсткий surface-owner gate удалил почти весь внешний каркас:

- v0.5.03: `1188` принятых mesh-рёбер;
- v0.5.04: только `152` принятых mesh-рёбер;
- v0.5.04: `2920` отклонённых рёбер;
- v0.5.04: `1416` рёбер без surface owner.

Причина: STEP POLY_LOOP использует дублированные вершины граней, а пиксель на геометрической границе может принадлежать соседнему front triangle. Требование точного triangle-owner совпадения оказалось слишком строгим для регулярной surface mesh.

Дополнительно новый профиль получил отдельную ветку освещения. Он больше не зависит от legacy single-key fallback.

## Изменения

- добавлен `Mesh Solid Front — v0.5.05`;
- режим выбран по умолчанию;
- `SurfaceAlpha=255` сохранён;
- hard surface-owner gate отключён для нового режима;
- возвращён проверенный front-depth ratio gate v0.5.03:
  - threshold `0.65`;
  - depth neighborhood `3x3`;
  - mesh depth tolerance `0.0012 * modelSize`;
- добавлено явное key/fill/facing/rim освещение;
- добавлен небольшой opaque depth cue: strength `12`, gamma `1.10`;
- mesh line RGB `72`, silhouette RGB `28`;
- сохранены software Z-buffer, Gouraud interpolation, POLY_LOOP mesh и отсутствие fan diagonals.

## Новая диагностика

Лог содержит:

- `OpaqueSurfacePixels`;
- `NonOpaqueSurfacePixels`;
- `DepthCueStrength`;
- `DepthCueGamma`;
- `DepthCuePixelsAdjusted`;
- `RequireMeshSurfaceOwnerMatch=False`;
- `AcceptedMeshEdgesByRatio`;
- `RejectedMeshEdgesByRatio`.

Visual Studio и Inventor в среде подготовки пакета не запускались. Выполнены статическая проверка исходников, численное воспроизведение STEP/Z-buffer на Rubber Hose и проверка целостности ZIP.
