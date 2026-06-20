# Inventor IPT Organizer v0.4.51 — SecondRefreshSpatialBase

## Исходник

Эта сборка сделана от:

`InventorIptOrg_v0_4_48_RefreshButtonTiming.zip`

v0.4.49 и v0.4.50 не использовались как база.

## Что сохранено

Оригинальные кнопки и обработчики остались:

- `ButtonIptRefreshBrowserTree_Click`
- `ButtonIptLegacyRefreshBrowserTree_Click`

## Что добавлено

Рядом с оригинальными кнопками добавлены вторые кнопки:

- `Refresh tree #2 from spatial BASE`
- `Refresh legacy #2 from spatial BASE`

## Как работает новый путь #2

1. Сначала нажать `.ipt cubes` → `Fast build spatial cubes BASE`.
2. Потом нажать одну из кнопок `#2`.

Кнопки #2 строят дерево из `_spatialCubesIndex.Bodies`, без полного BrowserPane traversal.

## Новые логи

- `BROWSER_TREE_REFRESH_2_SPATIAL_BASE_BUILT`
- `LEGACY_BROWSER_TREE_REFRESH_2_SPATIAL_BASE_BUILT`
- `BROWSER_TREE_REFRESH_2_SPATIAL_BASE_SECONDS`
- `LEGACY_BROWSER_TREE_REFRESH_2_SPATIAL_BASE_SECONDS`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
