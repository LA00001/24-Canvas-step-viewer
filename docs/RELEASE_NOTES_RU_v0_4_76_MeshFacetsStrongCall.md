# v0.4.76 — MeshFacetsStrongCall

## Исправлено

v0.4.75 показывала кубики, потому что все вызовы mesh facets падали:

`ArgumentException: Не удалось преобразовать аргумент 2 для вызова на CalculateFacets`

## Изменение

Вызов `CalculateFacets` заменён с dynamic/object-out на строго типизированный COM-call.

## Ожидаемый результат

В логе должно появиться:

`MESH_BODY_FACETS_OK`

А в `MESH_VIEW_SCENE_BUILT` должно быть:

`FallbackBodies=0` или хотя бы меньше 72.
