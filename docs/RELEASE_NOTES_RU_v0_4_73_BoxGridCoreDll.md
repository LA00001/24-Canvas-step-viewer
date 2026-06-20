# v0.4.73 — BoxGridCore DLL

## Что сделано

Добавлен отдельный DLL-проект:

`BoxGridCore`

## Что внутри

- `Box3`
- `BoxGridItemInput`
- `BoxGridItem`
- `BoxGridCell`
- `BoxGridQueryResult`
- `BoxGrid`
- `BoxGridBuilder`

## Как приложение использует DLL

`SpatialCubesIndex.Build` по-прежнему читает данные из текущего приложения, но разбиение на сетку и привязка items к ячейкам теперь выполняются через:

`BoxGridBuilder.Build(...)`

После этого адаптер копирует результат обратно в текущие структуры приложения.

## Новый лог

`BOXGRID_CORE_DLL_USED`
