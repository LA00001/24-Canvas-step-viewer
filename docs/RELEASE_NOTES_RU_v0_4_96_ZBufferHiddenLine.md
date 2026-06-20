# v0.4.96 — ZBufferHiddenLine

## Почему понадобилась новая версия

v0.4.95 уже правильно разделял:

- silhouette edges;
- boundary edges;
- crease edges.

Но edge pass рисовал линии поверх готовой поверхности без проверки глубины.
Из-за этого внутренние рёбра просвечивали через внешние панели.

## Что сделано

- Добавлен software Z-buffer для `Surface` и `Surface+Edges`.
- Каждый пиксель хранит ближайшую view-depth.
- Visible-shell triangles растеризуются с barycentric depth interpolation.
- Feature/silhouette edges проходят отдельный depth-test.
- Скрытые участки рёбер не рисуются.
- Контактная псевдотень отключена на время проверки Z-buffer.
- `Ghost` сохраняет прозрачный painter-preview.
- `FastDragWireframe` остаётся быстрым.

## Новые логи

- `STEP_ZBUFFER_SURFACE_STATS`
- `STEP_ZBUFFER_HIDDEN_LINE_STATS`
- `SurfacePixelsWritten`
- `SurfacePixelsRejectedByDepth`
- `CandidateEdges`
- `VisibleEdges`
- `VisibleEdgeSamples`
- `HiddenEdgeSamples`
- `HiddenLineRemoval=ZBufferDepthTest`
- `SurfaceFillMode=ZBufferOpaqueShell`
- `GdiStrategy=SoftwareZBufferHiddenLine`
