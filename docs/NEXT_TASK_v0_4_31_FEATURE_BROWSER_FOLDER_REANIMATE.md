# NEXT_TASK_v0_4_31_FEATURE_BROWSER_FOLDER_REANIMATE

Цель следующей версии: вернуть рабочую область `Create feature browser folder`.

## Нельзя ломать

Рабочая база v0.4.30 должна остаться стабильной:

- `.ipt cubes` tab;
- `SpatialCubesIndex`;
- `Cube hit: all bodies`;
- `Cube hit: filtered bodies`;
- RealCamera aspect-corrected projection;
- amber touched cubes preview;
- green trace marker;
- Hide/Show from current visible-list.

## Что реанимировать

1. Быстрое получение Inventor browser/features/elements.
2. Заполнение списка features/browser items без тяжёлого полного refresh после каждого действия.
3. Создание папки в дереве Inventor browser.
4. Логирование времени получения списка и создания папки.

## Предлагаемый build-name

```text
v0.4.31 FEATURE_BROWSER_FOLDER_REANIMATE
```

## Важно

`Apply edited name / XYZ to Inventor` пока не трогать.
