# v0.4.93 — SurfaceVisibleShell

## Что исправляем

v0.4.92 добавил depth sort и shading, но Surface выглядел полупрозрачным/слоёным,
потому что рисовались и front-facing, и back-facing triangles.

## Что сделано

- `Surface` и `Surface+Edges` теперь работают как opaque visible shell.
- Back-facing triangles отбрасываются.
- `Ghost` остаётся прозрачным и рисует все triangles.
- `Surface` больше не рисует bounding-box silhouette поверх модели.
- Усилен контраст pseudo-shading.
- FastDrag остаётся быстрым wireframe.

## Новые логи

- `STEP_SURFACE_SHELL_STATS`
- `CulledBackFacingTriangles`
- `DrawnFrontFacingTriangles`
- `SurfaceOpaque=True`
- `SurfaceFillMode=OpaqueVisibleShell`
- `GdiStrategy=VisibleShellDepthSortedPseudoShade`
