# Inventor IPT Organizer v0.4.54 — PerfCommentsFromV52Log

## Что сделано

В код добавлены инженерные perf-comment blocks из лога:

`inventor_ipt_organizer_v0_4_52_20260617_233155_491.log`

## Методы

- `ButtonIptRefreshBrowserTree_Click`
- `ButtonIptLegacyRefreshBrowserTree_Click`
- `ButtonIptRefreshBrowserTreeSpatialBase_Click`
- `ButtonIptLegacyRefreshBrowserTreeSpatialBase_Click`
- `ButtonIptCopyBrowserTree_Click`
- `ButtonIptLegacyCopyBrowserTree_Click`

## Важно

Для copy-кнопок в этом конкретном логе нет ENTER/EXIT событий, поэтому в коде честно указано, что замера нет и нужен отдельный прогон.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
