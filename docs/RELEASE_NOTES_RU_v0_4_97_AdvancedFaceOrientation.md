# v0.4.97 — AdvancedFaceOrientation

## Что добавлено

StepLiteCore теперь читает topology chain:

`ADVANCED_FACE -> FACE_OUTER_BOUND / FACE_BOUND -> POLY_LOOP`

Для каждого loop учитываются два STEP-флага:

- `FACE_BOUND.orientation`
- `ADVANCED_FACE.same_sense`

Если значения флагов различаются, порядок точек `POLY_LOOP` разворачивается до triangulation.

## Важное ограничение

Текущие тестовые файлы IceCore Mini и Rubber Hose содержат только `.T.` для обоих флагов.
Поэтому для них ожидается:

- `ReversedFaceLoops=0`
- визуальная геометрия может не измениться.

Версия нужна для корректной обработки STEP-файлов, где встречаются `.F.` orientation flags.

## Новые логи

- `STEP_ADVANCED_FACE_ORIENTATION`
- `AdvancedFaceSameSenseTrue/False`
- `FaceBoundOrientationTrue/False`
- `ForwardFaceLoops`
- `ReversedFaceLoops`
- `OrphanPolyLoops`
- `UnresolvedFaceBounds`
- `UnresolvedPolyLoops`

Z-buffer renderer v0.4.96 сохранён без изменений.
