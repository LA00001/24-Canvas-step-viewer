# Inventor IPT Organizer v0.4.58 — CacheNameSync

## Что исправлено

`Refresh tree #2 BASE` теперь обновляет имена, если узел был переименован прямо в Browser tree Inventor.

## Почему было старое имя

v0.4.57 использовал in-memory names-only snapshot. При cache hit он не ходил в Inventor BrowserNode и поэтому не видел внешние rename.

## Что изменено

На cache hit:

- проход по cached `BrowserTreeGridItem.Node`;
- чтение только live display name через `GetBrowserNodeDisplayName`;
- обновление `snapshot.GridItems`;
- обновление legacy `snapshot.RootNode`.

Не читается:

- `NativeObject`
- `ObjectKind`
- `XYZ`

## Новый лог

`BROWSER_TREE_NAMES_ONLY_CACHE_LIVE_NAME_SYNCED`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
