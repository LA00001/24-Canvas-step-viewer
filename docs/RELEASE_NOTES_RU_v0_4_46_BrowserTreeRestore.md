# Inventor IPT Organizer v0.4.46 — BrowserTreeRestore

## Что восстановлено

Область `Browser tree / дерево браузера Inventor` снова заполняется без обязательного полного обхода всего дерева Inventor.

## Новое поведение

После `Create feature browser folder` таблица Browser tree получает быстрый preview созданной структуры:

`Selected_Features -> Selected_Features_Items -> feature nodes`

Это не запускает дорогой `RefreshIptBrowserTree`.

## Новая кнопка

`Build Browser tree from current visible-list`

Она строит быстрый preview из текущих списков Bodies/Features.

## Полный refresh

Кнопка `Refresh browser tree` сохранена как ручной полный обход дерева Inventor.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
