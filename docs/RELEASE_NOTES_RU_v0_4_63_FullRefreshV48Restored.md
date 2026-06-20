# Inventor IPT Organizer v0.4.63 — FullRefreshV48Restored

## Что сделано

Возвращён/зафиксирован полный refresh-путь из версии:

`InventorIptOrg_v0_4_48_RefreshButtonTiming(2).zip`

## Восстановленные методы

- `ButtonIptRefreshBrowserTree_Click`
- `RefreshIptBrowserTree`
- `BuildBrowserTreeGridItems`
- `AppendBrowserTreeGridItems`

## Почему

После экспериментов с `#2 BASE`, cache-view и spatial row-model стало ясно, что единственный полностью корректный источник правды — полный обход живого дерева Inventor.

v0.4.63 снова явно фиксирует:

`Refresh browser tree = truth builder`

А `#2 BASE` остаётся только вспомогательным быстрым cache-view, не заменяющим полный refresh.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
