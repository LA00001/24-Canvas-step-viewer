# Deep root-cause analysis — v0.5.06

## Симптом

Тело выглядело прозрачным, хотя лог стабильно показывал alpha 255.

## Почему предыдущие попытки не могли исправить причину

- v0.5.04 owner gate влиял только на линии и удалил почти весь внешний mesh.
- v0.5.05 depth cue менял яркость уже выбранной поверхности, но не менял выбор ближайшей стороны closed shell.
- alpha уже был 255, поэтому дальнейшее усиление opacity не могло изменить результат.

## Геометрическое противоречие

Renderer использовал одновременно:

```text
DepthRule: GreaterViewZIsNear
Legacy back face: NormalZ > 0
```

Эти два правила несовместимы. При камере на положительной стороне view-Z camera-facing outward normal имеет положительный Z. То есть legacy culling удалял именно nearest surface.

## Воспроизведение

Supplied STEP был разобран теми же правилами:

- 3072 points;
- 3072 POLY_LOOP edges;
- 1536 triangles;
- 768 advanced faces;
- closed shell / manifold solid BREP.

При default camera front-only legacy pass и all-triangle pass имели одинаковую silhouette mask `135012` pixels, но owner/depth различались на каждом covered pixel. All-triangle nearest owner во всех `135012` pixels имел `NormalZ > 0`; legacy renderer удалял его и показывал более дальнюю поверхность. Средняя разница глубины составляла около `8.13` model units, медиана `6.57`, максимум `31.66`.

## Решение

Новый режим сохраняет исторические режимы нетронутыми и применяет:

```text
FrontFaceRule = NormalZPositive
DepthPass = AllTriangles
DepthWinner = GreatestViewZ
SurfaceAlpha = 255
MeshDepthNeighborhoodRadius = 0
MeshVisibleSampleRatioThreshold = 0.55
```
