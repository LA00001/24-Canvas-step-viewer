# Inventor IPT Organizer v0.4.31 — FEATURE_BROWSER_FOLDER_REANIMATE

## Что исправлено

v0.4.30 закрыл быстрый выбор тел через `.ipt cubes` и `RealCamera aspect-corrected projection`, но старый блок `Features / элементы, связанные с телами` оставался пустым после быстрого выбора.

В v0.4.31 кнопка **Create feature browser folder** больше не упирается в пустой feature-list.

## Новая логика

1. Пользователь выбирает тела через `.ipt cubes`.
2. В `Bodies in current group / Custom rectangle visible-list` появляются `SurfaceBody`.
3. Если `Features / элементы` пустой, кнопка **Create feature browser folder** автоматически строит список Features из текущего visible-list тел.
4. После этого создаётся BrowserFolder в дереве Inventor.
5. Полный `Refresh browser tree` после создания папки пропускается для скорости.

## Важно

`Apply edited name / XYZ to Inventor` пока не трогался.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
