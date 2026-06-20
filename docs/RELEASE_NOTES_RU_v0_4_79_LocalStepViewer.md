# v0.4.79 — LocalStepViewer

## Исправлено назначение ST Step

`ST Step` больше не открывает файл во внешнем сеансе.

Теперь workflow:

1. `ST Step`
2. выбрать `.stp/.step`
3. `StepLiteCore` читает файл как текст
4. preview строится прямо в `.ipt canvas`

## Новый проект

`src/StepLiteCore/StepLiteCore.csproj`

## Что читает StepLiteCore

- `CARTESIAN_POINT`
- `VERTEX_POINT`
- `EDGE_CURVE`

Это lightweight-local STEP preview.  
Это ещё не full BRep tessellation, но уже не зависит от открытия файла во внешнем окне.

## Логи

- `STEP_LOCAL_SCENE_LOADED`
- `STEP_LOCAL_OPEN_FAILED`
- `STEP_LOCAL_OPEN_CANCELLED`
- `STEP_LOCAL_OPEN_BUTTON_SECONDS`
