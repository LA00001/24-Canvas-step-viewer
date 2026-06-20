# Inventor IPT Organizer v0.4.32 — FEATURE_BROWSER_NODES_FAST

## Исправление после v0.4.31

v0.4.31 не закрыла пункт 2 визуально: блок `Features / элементы` оставался пустым после выбора тел через `.ipt cubes`.

## Что изменено

- После `.ipt cubes` выбора Features/browser-nodes строятся сразу из текущего visible-list тел.
- Если `CreatedByFeature` и `Face.CreatedByFeature` ничего не дают, используется fallback:
  `BrowserPane.GetBrowserNodeFromObject(SurfaceBody)`.
- `CreateBrowserFolderFromObjects` теперь умеет принимать уже готовый `BrowserNode` напрямую.
- После создания папки полный `Refresh browser tree` пропускается для скорости.
- `Apply edited name / XYZ to Inventor` не трогался.

## Новые события лога

- `FEATURE_LIST_AUTO_POPULATE_AFTER_CUBE_SELECTION`
- `FEATURE_LIST_BODY_BROWSER_NODE_FALLBACK`
- `FEATURE_BROWSER_FOLDER_DIRECT_BROWSER_NODE_USED`
- `FEATURE_BROWSER_FOLDER_FAST_CREATED_NO_TREE_REFRESH`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
