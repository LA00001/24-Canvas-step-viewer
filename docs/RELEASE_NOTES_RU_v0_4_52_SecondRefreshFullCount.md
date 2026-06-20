# Inventor IPT Organizer v0.4.52 — SecondRefreshFullCount

## Что исправлено

v0.4.51 давал 74 строки в кнопках `#2`, потому что строил только body-tree:

- root
- group
- 72 SurfaceBody

Для чистого эксперимента нужно столько же строк/узлов, сколько у оригинального Browser tree refresh.

## Новая логика #2

Кнопки `#2` теперь делают:

1. Проверяют, что `Fast build spatial cubes BASE` готова.
2. Обходят тот же Inventor `BrowserPane`, чтобы получить тот же node count.
3. Для каждого узла берут только имя/форму дерева.
4. Не вытаскивают тяжёлые данные:
   - `NativeObject`
   - `ObjectKind`
   - `XYZ`

## Что сохранено

Оригинальные кнопки не тронуты:

- `ButtonIptRefreshBrowserTree_Click`
- `ButtonIptLegacyRefreshBrowserTree_Click`

## Ожидаемый результат

Если оригинальная кнопка показывает 269 строк, кнопка `#2` тоже должна показать 269 строк/узлов.

## Новые логи

- `BROWSER_TREE_REFRESH_2_SPATIAL_BASE_FULLCOUNT_BUILT`
- `LEGACY_BROWSER_TREE_REFRESH_2_SPATIAL_BASE_FULLCOUNT_BUILT`

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
