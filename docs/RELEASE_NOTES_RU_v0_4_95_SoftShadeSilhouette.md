# v0.4.95 — SoftShadeSilhouette

## Что исправляем после v0.4.94

В логе v0.4.94:

- `DrawnFeatureEdges=22232`
- `BoundaryEdges=22232`
- `CreaseEdges=0`

Причина: edge adjacency строилась только по culled front-facing triangles и по vertex index.

## Что сделано

- Edge adjacency строится по всем triangles до back-face culling.
- Совпадающие edges объединяются по quantized 3D coordinates.
- Отдельные проходы:
  - silhouette edges;
  - crease edges;
  - true boundary edges.
- Добавлен мягкий directional shading.
- Добавлен лёгкий projected contact shadow.
- Добавлен светлый CAD-style фон.
- FastDrag остаётся простым wireframe.

## Новые логи

- `STEP_SOFT_SILHOUETTE_EDGES_BUILT`
- `SilhouetteEdges`
- `BoundaryEdges`
- `CreaseEdges`
- `EdgeKey=Quantized3DCoordinates`
- `SoftContactShadow=True`
- `SurfaceFillMode=SoftShadeOpaqueShell`
- `GdiStrategy=SoftShadeSilhouetteContactShadow`
