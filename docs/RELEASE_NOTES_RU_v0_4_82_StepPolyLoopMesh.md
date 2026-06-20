# v0.4.82 — StepPolyLoopMesh

## Что исправлено

v0.4.81 локально читал все `CARTESIAN_POINT`, но не находил `EDGE_CURVE`.

Для тестового STEP это давало:

- 58588 points
- 0 edges
- облако точек
- draw около 16 секунд

## Новое поведение

`StepLiteCore` теперь читает:

- `POLY_LOOP`
- `ADVANCED_FACE` count
- точки, реально участвующие в `POLY_LOOP`
- edges из loop-последовательности
- triangles через fan-triangulation loop polygon

## Результат

Вместо хаотичного облака всех координат теперь строится local STEP mesh из loop/faces.
