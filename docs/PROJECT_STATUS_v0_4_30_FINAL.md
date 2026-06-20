# PROJECT_STATUS_v0_4_30_FINAL

Дата сборки: 2026-06-16  
Рабочая версия: `Inventor IPT Organizer — v0.4.30 — REAL_CAMERA_ASPECT_CORRECT`

## Текущая карта проекта

### ✅ 1. Select Frame + Add Inner/Hidden Bodies

Старая логика Select Frame / Add Inner Hidden Bodies заменена рабочим workflow во вкладке `.ipt cubes`.

Текущий стабильный путь:

```text
Build spatial cubes BASE
-> Custom rect select ASPECT CORRECT v0.4.30
-> Cube hit: all bodies / Cube hit: filtered bodies
-> current visible-list
-> Hide/Show IPT bodies directly from current visible-list
```

Главные элементы:

- `SpatialCubesIndex.cs` — spatial-cube индекс `SurfaceBodies`.
- `Cube hit: all bodies` — задетые кубики -> все тела из них.
- `Cube hit: filtered bodies` — задетые кубики -> кандидаты -> RealCamera BodyBox filter.
- RealCamera orthographic projection с aspect correction.
- Янтарная временная подсветка задетых cubе-cells.
- Зелёный временный trace marker выбранной рамки.
- Hide/Show работает напрямую по текущему visible-list, без обязательной записи `myBodyGroup` attributes.

Контрольный тест v0.4.30:

```text
Projection mode: RealCameraActiveViewOrthographicAspectCorrected
AspectCorrection: WidthFromHeightByViewportAspect
Задето кубиков: 8/8
Кандидатов из выбранных кубиков: 72/72
Отклонено body-filter: 0
Тел показано в visible-list: 72
IPT bodies toggled from current visible-list: 72
```

### 🔧 2. Create feature browser folder

Следующая активная задача.

Сейчас старая область получения elements/features/browser nodes считается нерабочей. Её нужно реанимировать отдельно от уже победившей `.ipt cubes`-логики.

План следующей версии:

```text
v0.4.31_FEATURE_BROWSER_FOLDER_REANIMATE
```

Что нужно сделать:

1. Найти и восстановить рабочий путь получения элементов/features/browser nodes из Inventor IPT.
2. Ускорить получение списка, чтобы оно не тормозило после рамочного выбора.
3. Вернуть создание папки в browser tree Inventor.
4. Не ломать текущую `.ipt cubes`-логику v0.4.30.

### ⏸ 3. Apply edited name / XYZ to Inventor

Отложено. В текущей сборке не трогать.

Причина: сначала нужно стабилизировать быстрый workflow выбора тел и browser-folder creation. Редактирование имени/XYZ будет отдельной задачей позже.
