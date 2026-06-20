# v0.4.83 — StepFastGdiPaths

## Что исправлено

В v0.4.82 STEP загружался быстро:

- scene build: 0.901 s
- full ST click: 2.770 s

Но первая отрисовка занимала:

- DrawStepLiteScenePreview: 27.101 s

И при любом вращении всё перерисовывалось заново.

## Причина

v0.4.82 рисовала каждый triangle отдельно:

- FillPolygon на каждый triangle
- DrawPolygon на каждый triangle

При 23696 triangles это десятки тысяч GDI+ calls.

## Изменение

v0.4.83:

- собирает triangles/edges в GraphicsPath;
- рисует батчем:
  - FillPath
  - DrawPath
- во время mouse drag использует FastDragWireframe:
  - без shaded fill
  - прореженный wireframe
  - быстрый интерактивный поворот

## Логи

`STEP_LOCAL_DRAW_SECONDS` теперь пишет:

- Mode
- DrawnTriangles
- DrawnEdges
- GdiStrategy=GraphicsPathBatch
