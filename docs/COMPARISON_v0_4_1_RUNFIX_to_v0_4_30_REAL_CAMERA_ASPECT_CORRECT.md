# COMPARISON_v0_4_1_RUNFIX_to_v0_4_30_REAL_CAMERA_ASPECT_CORRECT

GitHub-база, загруженная пользователем: `InventorIptOrg_v0_4_1_RUNFIX`  
Новая рабочая сборка: `InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT`

## Что было в v0.4.1 RUNFIX

- WinForms внешнее приложение для Autodesk Inventor через COM.
- Старые workflows Select Frame / Inner Hidden Bodies / RangeBox.
- Списки IPT bodies/features/browser items.
- Начальная логика папок/дерева Inventor.
- Apply edited name / XYZ существовал как направление, но сейчас отложен.

## Что добавлено к v0.4.30

### Spatial cubes

- Добавлен `SpatialCubesIndex.cs`.
- Добавлена вкладка `.ipt cubes`.
- Добавлен быстрый spatial-cube BASE для `SurfaceBodies`.
- Добавлено дерево cube-cells: `CUBE_X*_Y*_Z*`.

### Новое выделение рамкой

- Добавлена собственная custom screen rectangle рамка поверх окна Inventor.
- Выбор больше не зависит от старого Inventor `WindowSelect`.
- Добавлены два живых режима:
  - `Cube hit: all bodies`;
  - `Cube hit: filtered bodies`.
- Старые режимы `AUTO normal`, `Whole selected cubes`, `Precise body-box` заморожены/оставлены комментариями.

### RealCamera aspect correction

- Добавлен projector через `ActiveView.Camera`.
- Исправлен X-scale bug через viewport aspect correction.
- Для wide viewport используется:

```text
EffectiveWidth = RawHeight * ViewportAspect
EffectiveHeight = RawHeight
```

- Одна и та же aspect-corrected система используется для:
  - box -> screen projection;
  - touched cubes;
  - body-box filtering;
  - screen rectangle -> green 3D trace marker.

### Preview / visual debug

- Синий preview выбранного одного cube-cell из дерева сохранён.
- Янтарная подсветка всех задетых рамкой кубиков добавлена.
- Зелёный trace marker рамки восстановлен и пересчитан через RealCamera aspect-corrected projection.

### Hide/Show acceleration

- Hide/Show работает напрямую по текущему visible-list.
- Старый путь через `myBodyGroup` attributes остаётся резервным/fallback.
- Быстрый путь не обязан писать attributes и не обязан запускать тяжёлый refresh всех списков.

## Что не входит в победу v0.4.30

Следующее ещё не сделано и должно идти отдельно:

```text
v0.4.31_FEATURE_BROWSER_FOLDER_REANIMATE
```

Задача: реанимировать получение elements/features/browser nodes и вернуть создание browser-folder в Inventor, не ломая `.ipt cubes`.

```text
Apply edited name / XYZ to Inventor
```

Отложено. Пока не трогать.
