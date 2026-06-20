# v0.5.00 — InventorStyleShadedMesh

## Цель

После реального теста v0.4.99 на Rubber Hose геометрия, Z-buffer и видимая POLY_LOOP-сетка работали правильно и быстро, но поверхность выглядела почти белой, а сетка была слишком светлой по сравнению с Autodesk Inventor.

## Что изменено

- Сохранены Gouraud normals, quantized normal merge и порог crease `dot >= 0.72`.
- Диапазон поверхности откалиброван под нейтральный Inventor-подобный серый:
  - средняя яркость снижена;
  - усилено разделение освещённых и затенённых участков;
  - raster clamp установлен `145..214`.
- Видимая POLY_LOOP-сетка затемнена до `RGB 68`.
- Silhouette overlay затемнён до `RGB 44`.
- Feature edges используют более тёмные CAD-оттенки.
- Фон STEP viewer заменён на более выраженный сине-серый вертикальный градиент.
- Исправлен повреждённый хвост `InventorIptOrg.sln`; solution снова содержит ровно четыре проекта и корректный `EndGlobal`.

## Сохранено без отката

- software Z-buffer;
- hidden-line depth-test;
- `Surface+Mesh` по умолчанию;
- `StepLiteScene.Edges` без fan-triangle diagonals;
- `TriangleDiagonalsIncluded=False`;
- `AdvancedFaceOrientation`;
- `FastDragWireframe`;
- отсутствие автоматического запуска Inventor;
- `.ipt canvas` по умолчанию.

## Новые признаки в логах

- `SurfacePalette=InventorNeutralGray`
- `SurfaceShadeClamp=145..214`
- `BackgroundProfile=InventorBlueGray`
- `MeshLineRgb=68`
- `SilhouetteLineRgb=44`
- `ViewerProfile=InventorStyleGrayMesh`
- `SurfaceFillMode=ZBufferInventorGrayGouraudMesh`
- `GdiStrategy=SoftwareZBufferInventorStyleMesh`

Visual Studio и Inventor должны быть проверены на стороне пользователя. В пакете выполнены статические проверки исходников и целостности ZIP.
