# v0.5.03 — MeshVisibilityThreshold

Build name: `MESH_VISIBILITY_THRESHOLD`

## Главное

1. Каждый пункт `Mode` получил суффикс с исторической версией метода:
   - `Wire — v0.4.84`
   - `Surface — v0.4.96`
   - `Surface+Edges — v0.4.96`
   - `Surface+Mesh — v0.4.99`
   - `Mesh Deep Clamp — v0.5.02`
   - `Mesh Two-Light — v0.5.02`
   - `Mesh Hemisphere — v0.5.02`
   - `Mesh Inventor Lab — v0.5.03`
   - `Ghost — v0.4.84`
   - `Points — v0.4.84`

2. Логи разделяют:
   - `AppVersion=v0.5.03`
   - `MethodVersion=<историческая версия выбранного метода>`

3. `Mesh Inventor Lab — v0.5.03` фильтрует просачивающиеся внутренние рёбра:
   - `MeshDepthNeighborhoodRadius=1` — front depth проверяется по окрестности 3×3;
   - `MeshVisibleSampleRatioThreshold=0.65`;
   - обычное POLY_LOOP-ребро сначала полностью тестируется, затем рисуется только при достаточной доле видимых samples;
   - silhouette и feature edges ratio-фильтром не подавляются.

## Новые поля логов

- `AppVersion`
- `MethodVersion`
- `MeshVisibleSampleRatioThreshold`
- `MeshDepthNeighborhoodRadius`
- `AcceptedMeshEdgesByRatio`
- `RejectedMeshEdgesByRatio`
- `HiddenByFrontDepth`

## Сохранено

- software Z-buffer;
- Gouraud vertex normals;
- hidden-line removal;
- AdvancedFace orientation;
- `StepLiteScene.Edges`;
- `TriangleDiagonalsIncluded=False`;
- FastDragWireframe;
- `.ipt canvas` по умолчанию;
- отсутствие Inventor autostart.

Visual Studio и Autodesk Inventor при подготовке пакета не запускались. Выполнены статические проверки исходников и целостности ZIP.
