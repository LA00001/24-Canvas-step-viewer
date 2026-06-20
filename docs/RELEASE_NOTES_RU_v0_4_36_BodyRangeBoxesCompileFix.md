# Inventor IPT Organizer v0.4.36 — BodyRangeBoxesCompileFix

## Что исправлено

Исправлены ошибки компиляции из v0.4.35:

- `Environment` был неоднозначным между `Inventor.Environment` и `System.Environment`;
- `CreateColor` требовал `byte`, а random RGB были `int`;
- helper `ExpandTraceBoxToHiddenRange` не был объявлен в классе `Form1`.

## Что сохранено

- `Show / Hide body RangeBoxes`;
- временная ClientGraphics-графика для `SurfaceBody.RangeBox`;
- короткие пути проекта;
- `AssemblyInfo.cs` в пакете.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
