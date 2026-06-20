# Inventor IPT Organizer v0.4.40 — SubfolderFeatureListFallback

## Что исправлено

В v0.4.39 Inventor не отдавал выбранные `BrowserNode` из левого дерева Model Browser через COM, поэтому кнопка `Create subfolder from selected nodes` показывала сообщение, что выбранные узлы не найдены.

## Что сделано

Добавлен fallback:

1. Сначала программа пробует старые источники: internal browser grid, SelectSet, BrowserPane selection, tree scan.
2. Если найдено 0 BrowserNode, программа берёт выбранные строки из `Features / элементы`.
3. Если в `Features / элементы` ничего не выделено, программа берёт весь текущий Features-list.
4. Для каждого FeatureListItem снова ищется BrowserNode через ModelBrowserPane.
5. Из найденных BrowserNode создаётся `Sub_Features_yyyyMMdd_HHmmss`.

## Новые события лога

- `BROWSER_SUBFOLDER_FEATURELIST_FALLBACK_SCAN`
- `BROWSER_SUBFOLDER_FEATURELIST_FALLBACK_EMPTY`

## Практический сценарий

После `Custom rect select` и `Create feature browser folder` список `Features / элементы` уже содержит нужные внутренние объекты. Теперь можно выделить нужные строки в этом списке и нажать `Create subfolder from selected nodes`.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
