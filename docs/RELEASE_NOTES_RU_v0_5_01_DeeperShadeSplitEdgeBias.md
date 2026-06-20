# v0.5.01 — DeeperShadeSplitEdgeBias

## Цель

Продолжение удачной базы v0.5.00. По реальному Rubber Hose тесту геометрия, Gouraud shading, Z-buffer и скорость уже стабильны, но внутренняя полость всё ещё светлее Inventor, а плотная POLY_LOOP-сетка местами доминирует над объёмом.

## Что изменено

- Поверхность получила более глубокий нейтрально-серый диапазон:
  - raster clamp `115..218` вместо `145..214`;
  - уменьшен ambient fill;
  - усилен directional light и разделение свет/тень;
  - back-facing contribution снижен без возврата flat-shading полос.
- Обычная видимая POLY_LOOP-сетка осветлена до `RGB 84`, чтобы shading оставался главным.
- Silhouette overlay сохранён тёмным: `RGB 40`.
- Edge depth tolerance разделён по типам:
  - mesh: `0.0025 * modelSize`;
  - feature: `0.0032 * modelSize`;
  - silhouette: `0.0040 * modelSize`.
- Уменьшенный mesh bias должен убрать лишнюю плотность и слишком разрешённые линии около горловины, не ломая silhouette.

## Сохранено без отката

- software Z-buffer и hidden-line removal;
- Gouraud barycentric interpolation;
- quantized normal merge, crease threshold `0.72`;
- `Surface+Mesh` по умолчанию;
- `StepLiteScene.Edges` / POLY_LOOP mesh;
- `TriangleDiagonalsIncluded=False`;
- `AdvancedFaceOrientation`;
- `FastDragWireframe`;
- Inventor не запускается автоматически;
- `.ipt canvas` открывается по умолчанию;
- `Cube hit: filtered bodies` остаётся режимом по умолчанию.

## Контрольные лог-поля

- `SurfacePalette=InventorDeeperNeutralGray`
- `SurfaceShadeClamp=115..218`
- `MeshLineRgb=84`
- `SilhouetteLineRgb=40`
- `MeshDepthToleranceFactor=0.0025`
- `FeatureDepthToleranceFactor=0.0032`
- `SilhouetteDepthToleranceFactor=0.0040`
- `ViewerProfile=InventorDeeperGrayMesh`
- `SurfaceFillMode=ZBufferDeeperGrayGouraudMesh`
- `GdiStrategy=SoftwareZBufferSplitEdgeBias`

## Базовый замер

Пользовательский лог v0.5.00 для Rubber Hose: `Surface+Mesh` 0.100–0.110 s, surface pass 0.025–0.031 s, 2667 видимых mesh-рёбер. Архитектура рендера в v0.5.01 сохранена, но фактический замер новой версии должен быть получен после запуска пользователем.

Visual Studio и Autodesk Inventor в этой среде не запускались. Выполнены статические проверки исходников и целостности ZIP.
