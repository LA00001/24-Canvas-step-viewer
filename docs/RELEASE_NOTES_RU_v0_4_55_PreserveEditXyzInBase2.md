# Inventor IPT Organizer v0.4.55 — PreserveEditXyzInBase2

## Проблема

`Refresh tree #2 BASE` быстрый, но names-only/cache режим стирал редактируемые X/Y/Z и делал строки не movable.

## Исправление

Перед #2 refresh сохраняем текущие строки DataGridView, затем после быстрого построения 269 строк возвращаем в совпавшие строки:

- `NativeObject`
- `CanMove`
- `CanRename`
- `HasCenter`
- `X`
- `Y`
- `Z`

## Рабочий сценарий

1. Нажать обычный `Refresh browser tree`, чтобы получить полные SurfaceBody/XYZ.
2. Потом нажимать `Refresh tree #2 BASE`.
3. X/Y/Z должны оставаться видимыми и редактируемыми для совпавших body rows.

## Новые события лога

- `BROWSER_TREE_EDITABLE_GRID_SNAPSHOT_CAPTURED`
- `BROWSER_TREE_EDITABLE_GRID_MERGED_INTO_BASE2`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
