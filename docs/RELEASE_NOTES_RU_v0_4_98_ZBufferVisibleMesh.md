# v0.4.98 — ZBufferVisibleMesh

## Зачем нужен новый режим

В v0.4.97:

- `Surface+Edges` показывал только 126 feature edges;
- `Wire` показывал все 4608 triangle edges, включая скрытые;
- для плавного резинового отвода отсутствовал промежуточный CAD-режим.

## Что сделано

Добавлен новый режим:

`Mode: Surface+Mesh`

Pipeline:

`Z-buffer surface -> StepLiteScene.Edges -> edge depth-test -> visible mesh only`

Особенности:

- используются исходные границы `POLY_LOOP`;
- fan-triangulation diagonals не включаются;
- скрытые участки mesh-рёбер отбрасываются Z-buffer;
- видимая сетка рисуется тонкой однопиксельной линией;
- `Surface+Edges` сохраняет только silhouette/boundary/crease edges;
- `Wire` остаётся диагностическим режимом насквозь.

## Новый лог

`STEP_ZBUFFER_VISIBLE_MESH_STATS`

Поля:

- `SceneMeshEdges`
- `CandidateMeshEdges`
- `VisibleMeshEdges`
- `VisibleMeshSamples`
- `HiddenMeshSamples`
- `MeshSource=StepLiteScene.Edges`
- `TriangleDiagonalsIncluded=False`

`Surface+Mesh` выбран режимом по умолчанию для проверки.
