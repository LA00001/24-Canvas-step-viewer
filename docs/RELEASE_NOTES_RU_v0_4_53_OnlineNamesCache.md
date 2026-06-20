# Inventor IPT Organizer v0.4.53 — OnlineNamesCache

## Что добавлено

Добавлен in-memory names-only snapshot cache для кнопок `#2 BASE`.

## Как работает

1. Нажать `.ipt cubes` → `Fast build spatial cubes BASE`.
2. Первый запуск `Refresh tree #2 BASE` или `Refresh legacy #2 BASE` строит полный 269-узловый snapshot.
3. Snapshot сохраняется в `_browserTreeNamesOnlySnapshot`.
4. Следующие `#2` запуски используют этот snapshot из памяти.

## При cache hit пропускается

- BrowserPane traversal
- NativeObject
- ObjectKind
- XYZ

## Когда cache инвалидируется

При создании BrowserFolder через `CreateBrowserFolderFromObjects`.

## Новые события лога

- `BROWSER_TREE_NAMES_ONLY_CACHE_MISS_BUILT`
- `BROWSER_TREE_NAMES_ONLY_CACHE_HIT`
- `BROWSER_TREE_REFRESH_2_ONLINE_CACHE_HIT`
- `LEGACY_BROWSER_TREE_REFRESH_2_ONLINE_CACHE_HIT`
- `BROWSER_TREE_NAMES_ONLY_CACHE_INVALIDATED`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
