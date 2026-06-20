# v0.5.04 — OpaqueBodyMesh

Build name: `OPAQUE_BODY_MESH`

## Главное

Добавлен новый режим:

`Mode: Mesh Opaque Body — v0.5.04`

Визуальные параметры `Mesh Inventor Lab — v0.5.03` сохранены, но обычные POLY_LOOP-рёбра теперь проходят дополнительную проверку владельца передней поверхности.

## Surface-owner Z-buffer

1. Surface pass записывает:
   - непрозрачный ARGB с `SurfaceAlpha=255`;
   - ближайший depth;
   - ID front-surface triangle, который победил в данном пикселе.
2. Для каждого POLY_LOOP edge строится список triangle-owner ID по quantized 3D geometry.
3. Mesh sample принимается только когда:
   - он проходит front-depth test;
   - front owner в окрестности принадлежит этому ребру;
   - всё ребро проходит сохранённый ratio threshold `0.65`.
4. Rear/internal edge не может рисоваться поверх несвязанной передней оболочки.

Silhouette и feature edges сохраняют свои независимые depth tolerance. `Ghost` остаётся намеренно прозрачным режимом.

## Новые поля логов

- `SurfaceAlpha=255`
- `OpaqueBody=True`
- `OpaqueSurfaceOwnerPass=True`
- `RequireMeshSurfaceOwnerMatch=True`
- `MeshOwnerNeighborhoodRadius=1`
- `MeshOwnerMatchedSamples`
- `MeshOwnerRejectedSamples`
- `MeshEdgesWithoutSurfaceOwner`

## Сохранено

- software Z-buffer;
- Gouraud vertex normals;
- historical method versions в списке Mode;
- `AppVersion` и `MethodVersion` в логах;
- AdvancedFace orientation;
- `StepLiteScene.Edges`;
- `TriangleDiagonalsIncluded=False`;
- FastDragWireframe;
- `.ipt canvas` по умолчанию;
- отсутствие Inventor autostart.

Пользовательский замер v0.5.03 на Rubber Hose сохранён как база сравнения: `Mesh Inventor Lab` 0.117–0.155 s, surface pass 0.033–0.051 s, 1188 accepted mesh edges, 1884 rejected by ratio, 42956 samples hidden by front depth.

Visual Studio и Autodesk Inventor при подготовке пакета не запускались. Выполнены статические проверки исходников и целостности ZIP.
