# v0.4.99 — ZBufferSmoothShadeMesh

## Что улучшено после v0.4.98

v0.4.98 правильно показывал видимую POLY_LOOP-сетку через Z-buffer,
но каждый triangle имел один постоянный оттенок. На плавных трубах были заметны полосы.

## Что сделано

- Добавлены сглаженные vertex normals.
- Нормали объединяются по quantized 3D coordinates.
- Соседние normals смешиваются только при `dot >= 0.72`, поэтому резкие грани сохраняются.
- Для каждого triangle вычисляются `ShadeA`, `ShadeB`, `ShadeC`.
- Shade интерполируется внутри triangle barycentric-методом вместе с depth.
- Видимая mesh-сетка сделана светлее.
- Silhouette overlay сделан однопиксельным.
- Depth tolerance увеличен с `0.0025` до `0.0040` размера модели для уменьшения пунктирных разрывов.

## Новые признаки в логах

- `ShadeInterpolation=GouraudBarycentric`
- `NormalMerge=Quantized3DCoordinates`
- `NormalCreaseDotThreshold=0.72`
- `SmoothVertexKeys`
- `SilhouetteOverlayEdges`
- `SurfaceFillMode=ZBufferGouraudSmoothMesh`
- `GdiStrategy=SoftwareZBufferGouraudMesh`

Z-buffer, hidden-line removal, AdvancedFaceOrientation и Surface+Mesh сохранены.
