# 24-Inventor-ipt-canvas-step-viewer

**Build:** `Inventor IPT Organizer — v0.5.17 — WARNING_FREE_CANVAS_BUILD`

GitHub-oriented presentation build of the local `.ipt canvas` STEP viewer. Only the `.ipt canvas` tab is visible. The verified profiles are highlighted in the selectors:

- `🔥Mode: Mesh True Front (compile fix) — v0.5.07`
- `🔥FPS: Cursor Sampled Loop — v0.5.13`

The Mode and FPS selectors are wider so the full historical labels are visible. The visible toolbar contains only local STEP open, camera reset, viewer options and the render/FPS selectors. Legacy `.ipt`, `.iam`, `.ipt cubes` tabs and Inventor-dependent buttons are hidden. Startup no longer attaches to a running Inventor session.

## v0.5.17 warning-free build

The three `CS0162: Unreachable code detected` warnings reported by the confirmed v0.5.16 Visual Studio build are fixed without changing canvas-only behavior. `CanvasOnlyGitHubMode` is now a runtime-readonly configuration flag instead of a compile-time constant, so the retained legacy fallback branches remain compilable and no longer trigger unreachable-code diagnostics.

## Autodesk dependency status

The **visible canvas workflow** uses `StepLiteCore`, `System.Drawing` and WinForms only; it does not call Inventor/Autodesk APIs. The historical legacy source is still retained in `src/InventorIptOrg`, so that project still references `Autodesk.Inventor.Interop.dll` at compile time. This package does not falsely claim that the whole legacy executable is already dependency-free. A fully standalone project requires extracting the canvas form/renderer into a separate project.

## Repository description

WinForms-инструмент для локального просмотра STEP в .ipt canvas: StepLite-парсер, software Z-buffer, непрозрачный True Front renderer, POLY_LOOP-сетка, исторические режимы рендера и FPS, backbuffer и интерактивное вращение. В GitHub-режиме показывается только canvas, а Inventor-зависимые кнопки скрыты.

---

# Inventor IPT Organizer v0.5.15 — RENDER_FPS_GROUP_FIX

Historical grouping was re-audited directly against the user-supplied source ZIP archives. `v0.5.06` introduced the Mesh True Front renderer. `v0.5.07` only fixed CS0136 in that same renderer and did not introduce FPS/backbuffer behavior. The FPS selector therefore starts at `v0.5.08` and contains only v0.5.08–v0.5.13.

Mode now contains both auditable source-build profiles:

- `Mode: Mesh True Front — v0.5.06`
- `Mode: Mesh True Front (compile fix) — v0.5.07`

The two profiles intentionally render identically; v0.5.07 differs only by the source-level compile repair.

---

# Inventor IPT Organizer v0.5.14 — FPS_MODE_SELECTOR (superseded grouping)

This build introduced the second selector, but its historical grouping was later found to be wrong. v0.5.06 and v0.5.07 were render-source builds without an FPS/backbuffer subsystem. v0.5.15 corrects the groups and starts FPS at v0.5.08.

# Inventor IPT Organizer v0.5.13 — CURSOR_SAMPLED_DRAG_LOOP

Current build: `Inventor IPT Organizer — v0.5.13 — CURSOR_SAMPLED_DRAG_LOOP`

Stable final-quality renderer: `Mode: Mesh True Front — v0.5.06`.

This build decouples drag rendering from WinForms WM_PAINT and from MouseMove delivery. A short timer samples the physical cursor, applies the complete accumulated delta, renders a reduced True Front frame into an off-screen bitmap, atomically swaps it, and immediately presents the completed backbuffer. The overlay now shows separate presentation cadence and renderer capacity.

---

# Inventor IPT Organizer v0.5.12 — ADAPTIVE_DRAG_DIRECT_SWAP

Current build: `Inventor IPT Organizer — v0.5.12 — ADAPTIVE_DRAG_DIRECT_SWAP`

Stable final-quality renderer: `Mode: Mesh True Front — v0.5.06`.

This build keeps the v0.5.11 visible-viewport center fix and improves LMB orbit performance. Reduced drag frames are stored directly in the atomic backbuffer instead of being copied into a full-size bitmap. Expensive per-pixel diagnostics are skipped only during reduced drag, and render scale adapts from 0.32 to 0.58 toward a 40 ms frame budget. The unchanged full-quality backbuffer stays visible on MouseDown; the first reduced frame is rendered only after actual camera movement. Full-quality True Front rendering is unchanged.

---



# Inventor IPT Organizer v0.4.30 FINAL GitHub Assembly

This package is the current stable GitHub-ready baseline after comparing the uploaded GitHub base `v0.4.1 RUNFIX` with the working `v0.4.30 REAL_CAMERA_ASPECT_CORRECT` build.

Current status:

- Done: Select Frame + Add Inner/Hidden Bodies is replaced by `.ipt cubes` spatial-cube rectangle selection.
- Next: reanimate and accelerate Create feature browser folder.
- Deferred: Apply edited name / XYZ to Inventor.

See:

- `PROJECT_STATUS_v0_4_30_FINAL.md`
- `COMPARISON_v0_4_1_RUNFIX_to_v0_4_30_REAL_CAMERA_ASPECT_CORRECT.md`
- `NEXT_TASK_v0_4_31_FEATURE_BROWSER_FOLDER_REANIMATE.md`

---

# InventorIptOrg v0.4.30 REAL_CAMERA_ASPECT_CORRECT

This build fixes the horizontal scale error found in v0.4.29.

The RealCamera projector now uses aspect-corrected effective extents instead of raw `Camera.GetExtents` X/Y directly. In the user's test, Inventor view rect was `W1065 H560`, while raw camera extents were about `44.242 x 40.724`; that raw aspect compressed X. v0.4.30 keeps the stable height and derives the effective width from the viewport aspect.

Build token:
`IPT_ORGANIZER_v0.4.30_REAL_CAMERA_ASPECT_CORRECT_2026-06-16`


## v0.4.31 FEATURE_BROWSER_FOLDER_REANIMATE

После v0.4.30 продолжена работа: кнопка Create feature browser folder теперь автоматически строит список Features из текущего visible-list тел, если старый feature-list пустой, и создаёт BrowserFolder без полного Refresh browser tree после создания.

## v0.4.32 FEATURE_BROWSER_NODES_FAST

После выбора тел через `.ipt cubes` список `Features / элементы` теперь заполняется сразу. Если Inventor не отдаёт CreatedByFeature, используется BrowserNode fallback по SurfaceBody. Создание feature browser folder работает без полного Refresh browser tree после создания.

## v0.4.34 TRACE_DEPTH_FIX

Исправлен сборочный сбой CS2001: в пакет добавлен `src/InventorIptOrg/Properties/AssemblyInfo.cs`. Короткая структура путей сохранена.

## v0.4.34 TRACE_DEPTH_FIX

Желто-зелёный trace box после custom rectangle selection больше не рисуется тонким слоем около target plane. Теперь скрытая ось trace box растягивается на полную глубину всех touched spatial-cubes.

## v0.4.35 BODY_RANGEBOXES

Исправлен compile error v0.4.34: добавлен отсутствующий helper `ExpandTraceBoxToHiddenRange`.
Добавлена кнопка `Show / Hide body RangeBoxes`, которая рисует `SurfaceBody.RangeBox` текущего visible-list временной ClientGraphics-графикой в случайных полупрозрачных цветах.

## v0.4.36 BODY_RANGEBOXES_COMPILE_FIX

Исправлены compile errors v0.4.35:
- `Environment.TickCount` заменён на `System.Environment.TickCount`;
- random RGB значения приводятся к `byte` для `CreateColor`;
- добавлен отсутствующий helper `ExpandTraceBoxToHiddenRange`.

Кнопка `Show / Hide body RangeBoxes` сохранена.

## v0.4.37 BODYBOX_COLOR_EDGES

Исправлена видимость `Show / Hide body RangeBoxes`: вместо слабой случайной полупрозрачной заливки теперь у каждого `SurfaceBody.RangeBox` есть яркий толстый wireframe-контур по 12 рёбрам и лёгкая face-заливка. Цвета берутся из high-contrast palette по индексу тела. По умолчанию `.ipt cubes` запускается в режиме `Cube hit: filtered bodies`.

## v0.4.38 RANGEBOX_INSIDE_TRACEBOX

Добавлен третий режим `.ipt cubes`: `3. RangeBox fully inside TraceBox`.
Он оставляет из кандидатов только те тела, чей `SurfaceBody.RangeBox` полностью лежит внутри зелёного `Selection Trace Box`.

Dropdown теперь:
- `1. Cube hit: all bodies`
- `2. Cube hit: filtered bodies`
- `3. RangeBox fully inside TraceBox`

По умолчанию выбран режим `2. Cube hit: filtered bodies`.

## v0.4.39 BROWSER_SUBFOLDERS

Добавлена кнопка `Create subfolder from selected nodes`: из выбранных внутренних BrowserNode/features уже созданной папки Inventor создаётся новая BrowserFolder-подпапка без полного Refresh browser tree после создания. Источники выбора: выделенные строки внутренней Browser tree grid, Inventor SelectSet или выделение BrowserPane.

## v0.4.40 SUBFOLDER_FEATURELIST_FALLBACK

Исправлена кнопка `Create subfolder from selected nodes`: если Inventor не отдаёт выделение BrowserNode из левого дерева Model Browser, программа берёт выделенные строки из `Features / элементы`; если там ничего не выделено — берёт весь текущий Features-list. Это позволяет создавать подпапку из объектов, уже найденных после `Custom rect select` и `Create feature browser folder`.

## v0.4.43 DESIGNER_NEWLINE_FIX

`Create feature browser folder` теперь сразу делает вложенную структуру: сначала внешнюю папку `Selected_Features_yyyyMMdd_HHmmss`, затем внутреннюю папку `Selected_Features_Items_yyyyMMdd_HHmmss`, и уже в неё перемещаются BrowserNode элементов/features. Кнопка `#2` остаётся копией той же кнопки и вызывает тот же обработчик.

## v0.4.43 DESIGNER_NEWLINE_FIX

Исправлен compile error CS1010/CS1002/CS1040 в `Form1.Designer.cs`: текст кнопки `Create feature browser folder #2` теперь записан как escaped `\r\n`, без реального переноса строки внутри C# string literal. Функциональность v0.4.42 FeatureFolderNested сохранена.

## v0.4.44 FEATURELIST_NODE_FIX

Исправлен compile error CS0103: добавлен отсутствующий helper `GetBrowserNodesForSubfolderFromFeatureList`.
Кнопка subfolder теперь может брать BrowserNode из выбранных строк `Features / элементы`, а если там ничего не выделено — из всего текущего feature-list. Сохранены nested feature folder v0.4.42 и Designer newline fix v0.4.43.

## v0.4.45 TRUE_NESTED_NO_DUPLICATES

Исправлена логика nested feature folder. Раньше внешняя и внутренняя папки создавались из одного и того же набора feature nodes, из-за чего в дереве появлялись две папки с одинаковыми элементами. Теперь при nested-режиме сначала создаётся внутренняя папка с элементами, затем внешняя папка создаётся из BrowserNode этой внутренней папки. Если BrowserNode внутренней папки получить не удаётся, создаётся только внутренняя папка, чтобы не плодить дубли.

## v0.4.46 BROWSER_TREE_RESTORE

Восстановлена область `Browser tree / дерево браузера Inventor` без возврата тяжёлого auto-refresh. После создания feature browser folder таблица заполняется быстрым preview созданной структуры. Добавлена кнопка `Build Browser tree from current visible-list`, которая строит быстрый preview из текущих Bodies/Features списков. Полный `Refresh browser tree` остаётся ручным.

## v0.4.47 LEGACY_BROWSER_TREE_AREA

Добавлена новая область из старого проекта `MyFirstInventorPlugin_VS2017_Lesson5_CSharp_Tabs_IPT_IAM_BROWSER_TREE_EXPORT_FIX_IO_NAMES`: `Legacy browser tree / старое дерево браузера Inventor`.

Важно: существующая новая область `Browser tree / дерево браузера Inventor` с редактируемой DataGridView не заменена и остаётся на месте. Legacy-область добавлена ниже на `.ipt` вкладке через AutoScroll.

Кнопки legacy-области:
- `Refresh legacy browser tree`
- `Copy legacy tree to clipboard`
- `Save legacy tree JSON file`

## v0.4.48 REFRESH_BUTTON_TIMING

Кнопки обновления деревьев теперь показывают время последней отработки прямо в названии кнопки:
- `Refresh browser tree / last: X.XXX s`
- `Refresh legacy / last: X.XXX s`

Во время выполнения кнопка временно показывает `working...`.

## v0.4.51 SECOND_REFRESH_SPATIAL_BASE

База сборки: `InventorIptOrg_v0_4_48_RefreshButtonTiming.zip`.

Важно: v0.4.49 и v0.4.50 не использовались как исходник.

Оригинальные кнопки и обработчики оставлены без изменения:
- `ButtonIptRefreshBrowserTree_Click`
- `ButtonIptLegacyRefreshBrowserTree_Click`

Рядом с ними добавлены вторые кнопки #2:
- `Refresh tree #2 from spatial BASE`
- `Refresh legacy #2 from spatial BASE`

Новый быстрый способ через `Fast build spatial cubes BASE` применён только к кнопкам #2.

## v0.4.52 SECOND_REFRESH_FULL_COUNT

Исправлен эксперимент с кнопками `#2`.

В v0.4.51 кнопки `#2` строили только body-tree из spatial BASE: 72 тела + 2 служебные строки = 74. Для чистого эксперимента нужно столько же строк/узлов, сколько у оригинальных кнопок.

Теперь `#2` кнопки:

- требуют готовую `Fast build spatial cubes BASE`;
- обходят тот же `BrowserPane`, чтобы получить тот же node count / row count;
- строят names-only дерево;
- пропускают массовые `NativeObject`, `ObjectKind`, `XYZ`;
- оригинальные кнопки и обработчики оставлены без изменений.

Новые события:

- `BROWSER_TREE_REFRESH_2_SPATIAL_BASE_FULLCOUNT_BUILT`
- `LEGACY_BROWSER_TREE_REFRESH_2_SPATIAL_BASE_FULLCOUNT_BUILT`

## v0.4.53 ONLINE_NAMES_CACHE

Добавлен онлайн-кэш для кнопок `#2 BASE`.

1. Первый `#2` проход строит полный 269-узловый names-only snapshot через BrowserPane.
2. Snapshot сохраняется в памяти.
3. Следующие `#2` обновления строят DataGridView/TreeView из памяти, без повторного BrowserPane traversal.
4. При создании BrowserFolder cache инвалидируется.

Новые логи:
- `BROWSER_TREE_NAMES_ONLY_CACHE_MISS_BUILT`
- `BROWSER_TREE_NAMES_ONLY_CACHE_HIT`
- `BROWSER_TREE_REFRESH_2_ONLINE_CACHE_HIT`
- `LEGACY_BROWSER_TREE_REFRESH_2_ONLINE_CACHE_HIT`
- `BROWSER_TREE_NAMES_ONLY_CACHE_INVALIDATED`

## v0.4.54 PERF_COMMENTS_FROM_V52_LOG

Добавлены inline perf-comment blocks из реального лога:

`inventor_ipt_organizer_v0_4_52_20260617_233155_491.log`

Блоки добавлены перед методами:

- `ButtonIptRefreshBrowserTree_Click`
- `ButtonIptLegacyRefreshBrowserTree_Click`
- `ButtonIptRefreshBrowserTreeSpatialBase_Click`
- `ButtonIptLegacyRefreshBrowserTreeSpatialBase_Click`
- `ButtonIptCopyBrowserTree_Click`
- `ButtonIptLegacyCopyBrowserTree_Click`

Для copy-кнопок в этом логе запусков не было, поэтому добавлены честные блоки "замера нет".

## v0.4.55 PRESERVE_EDIT_XYZ_IN_BASE2

Исправление для `Refresh tree #2 BASE`: быстрый names-only/cache refresh больше не должен стирать редактируемые X/Y/Z, если перед ним уже был обычный `Refresh browser tree`.

Как работает:

1. Перед `#2 BASE` снимается snapshot текущего DataGridView.
2. Быстро строятся 269 строк из names-only/cache.
3. В совпавшие по порядку и имени строки возвращаются `NativeObject`, `CanMove`, `X`, `Y`, `Z`.
4. Поэтому `Apply edited name / XYZ to Inventor` остаётся рабочим для body rows, которые уже были известны после полного refresh.

Новые логи:

- `BROWSER_TREE_EDITABLE_GRID_SNAPSHOT_CAPTURED`
- `BROWSER_TREE_EDITABLE_GRID_MERGED_INTO_BASE2`

## v0.4.56 DATAGRID_INPLACE_REFRESH

Добавлен in-place refresh для `Refresh tree #2 BASE`.

- если количество строк совпадает, `Rows.Clear()` и `Rows.Add()` не вызываются;
- обновляются Level / Type / Name / row.Tag / ReadOnly;
- X/Y/Z не очищаются и остаются редактируемыми;
- если количество строк не совпало, используется безопасный fallback на полное заполнение.

Новые логи:

- `BROWSER_TREE_GRID_INPLACE_REFRESHED`
- `BROWSER_TREE_GRID_INPLACE_REFRESH_SKIPPED`
- `InPlaceGridUpdated=True/False`

## v0.4.57 DATAGRID_INPLACE_COMPILE_FIX

Исправлены ошибки компиляции v0.4.56:

- восстановлены helper-методы `CaptureCurrentBrowserTreeGridItemsForSecondButtonMerge`
  и `MergeEditableBrowserTreeDataByRow`;
- `BrowserGridColLevel` заменён на существующий `BrowserGridColDepth`.

Логика v0.4.56 сохранена: `Refresh tree #2 BASE` пытается обновлять DataGridView in-place,
не очищая X/Y/Z.

## v0.4.58 CACHE_NAME_SYNC

Исправлен stale-cache случай для `#2 BASE`.

Проблема: если переименовать BrowserFolder/узел прямо в дереве Inventor, затем нажать `Refresh tree #2 BASE`, UI показывал старое имя из cached names-only snapshot.

Новая логика:

- на cache hit `#2 BASE` синхронизирует имена из живых `BrowserNode` ссылок;
- структура дерева не перестраивается;
- `NativeObject`, `ObjectKind`, `XYZ` по-прежнему не читаются;
- `X/Y/Z` и editable-данные сохраняются через предыдущий merge/in-place механизм.

Новый лог:

- `BROWSER_TREE_NAMES_ONLY_CACHE_LIVE_NAME_SYNCED`

## v0.4.59 CACHE_NAME_SYNC_COMPILE_FIX

Исправлена ошибка компиляции v0.4.58:

- добавлен отсутствующий метод `PopulateIptBrowserTreeGridInPlaceFromItems`;
- добавлен helper `ApplyEditableStateToBrowserTreeRow`;
- сохранены `CacheNameSync`, `PreserveEditXyz` и in-place DataGrid refresh.

Ошибка, которую исправляет версия:

`CS0103: Имя "PopulateIptBrowserTreeGridInPlaceFromItems" не существует в текущем контексте.`

## v0.4.61 UNIFIED_BROWSER_ROW_MODEL

Переписана логика `#2 BASE`: теперь это единая модель строк, а не цепочка patch/merge.

Новая архитектура:

- первый `#2 BASE` cache miss строит `BrowserTreeNamesOnlySnapshot` как row-model;
- каждая строка BrowserNode получает имя и, если это `SurfaceBody`, stable link через `NativeObject -> COM identity -> SpatialBodyRecord`;
- body rows сразу получают `NativeObject`, `CanMove`, `X/Y/Z`, `Cubes`, `Cube IDs`;
- cache hit больше не восстанавливает X/Y/Z по кускам, а использует готовый row-model;
- live name sync обновляет только имена, не ломая spatial/body данные.

Новые логи/поля:

- `BuildBrowserTreeUnifiedSnapshotNode`
- `TryAttachSpatialBaseBodyDataToBrowserTreeItem`
- `TryFindSpatialBodyRecordByNativeObject`
- `SnapshotNativeObjectRows`
- `SnapshotSpatialBodyRows`
- `SnapshotSpatialCenterRows`

## v0.4.62 FAST_CACHE_VIEW

`Refresh tree #2 BASE` теперь является чистым cache-view режимом.

Главное изменение:

- кнопка `#2 BASE` больше не вызывает `GetOrBuildBrowserTreeNamesOnlySnapshot`;
- не запускает `GetModelBrowserPane`;
- не делает live BrowserPane traversal;
- не читает 269 BrowserNode имён;
- не делает live-name sync;
- показывает уже готовую row-model из памяти.

Если cache отсутствует, `#2 BASE` пробует построить snapshot только из текущего DataGridView без COM-обхода BrowserPane.  
Если и текущей таблицы нет, кнопка честно сообщает, что FAST CACHE не готов.

Spatial cubes BASE при необходимости пересчитывается отдельно как 2x2x2 = 8 кубиков; это не BrowserPane traversal.

Новые логи:

- `BROWSER_TREE_REFRESH_2_FAST_CACHE_VIEW`
- `LEGACY_BROWSER_TREE_REFRESH_2_FAST_CACHE_VIEW`
- `BROWSER_TREE_FAST_CACHE_VIEW_HIT`
- `BROWSER_TREE_FAST_CACHE_VIEW_BUILT_FROM_CURRENT_GRID`
- `BROWSER_TREE_FAST_CACHE_VIEW_MISS`
- `BROWSER_TREE_FAST_CACHE_SPATIAL_DATA_REFRESHED`
- `SPATIAL_CUBES_FAST_BASE_REBUILT_FOR_CACHE_VIEW`

## v0.4.63 FULL_REFRESH_V48_RESTORED

Возвращён канонический полный refresh из `InventorIptOrg_v0_4_48_RefreshButtonTiming(2).zip`.

Восстановлены/зафиксированы как источник правды методы:

- `ButtonIptRefreshBrowserTree_Click`
- `RefreshIptBrowserTree`
- `BuildBrowserTreeGridItems`
- `AppendBrowserTreeGridItems`

Назначение:

- снова считать обычную кнопку `Refresh browser tree` главным честным режимом;
- она читает живой Inventor BrowserPane и строит правильную связь `BrowserNode -> NativeObject -> ObjectKind -> X/Y/Z`;
- быстрые `#2 BASE` остаются вспомогательными cache-view экспериментами, но не заменяют источник правды.

## v0.4.64 LAYER_WINDOWS_CANVAS

Новая цель: крупные области выводятся по запросу в отдельные временные окна.

Добавлена вкладка `.ipt canvas`:

- слева область `Model preview / превью модели`;
- справа квадратные кнопки со значками;
- у кнопок есть подсказки `ToolTip` с задержкой наведения мыши;
- старые рабочие кнопки и канонический v0.4.48 full refresh не удалены.

Новые временные окна:

- Browser Truth Cache
- Spatial Geometry Cache
- Merged View Cache
- Fast Cache View
- Legacy Browser Tree
- Model Preview / Status

Новые логи:

- `LAYER_WINDOWS_CANVAS_READY`
- `LAYER_WINDOW_OPENED`

## v0.4.65 LAYER_WINDOWS_COMPILE_FIX

Исправлена ошибка компиляции v0.4.64:

`CS0103: Имя "BrowserGridColCubes" не существует в текущем контексте.`

Правка:

- `BrowserGridColCubes` заменён на существующий `BrowserGridColCubeCount`.

Функциональная логика v0.4.64 сохранена:

- вкладка `.ipt canvas`;
- квадратные кнопки со значками;
- ToolTip-подсказки с задержкой;
- временные окна крупных областей;
- слева область preview модели.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.

## v0.4.67 CANVAS_LEFT_LEGACY_TREE

Исправлена левая нижняя область `.ipt canvas`:

- вместо служебного `Layer tree / дерево слоёв` теперь показывается настоящее `Legacy browser tree`;
- дерево клонируется из `treeViewIptLegacyBrowserTree`;
- добавлена кнопка `LR Refresh` для построения legacy дерева прямо из canvas;
- если legacy дерево ещё пустое, показывается служебный узел с подсказкой;
- после `Refresh legacy` canvas-дерево обновляется автоматически.

## v0.4.68 ACTIVE_VIEW_PREVIEW

Добавлена кнопка `AV Model` на `.ipt canvas`.

Она переносит текущий вид модели из Autodesk Inventor в правую крупную область preview:

- источник: `Inventor.ActiveView.SaveAsBitmap`;
- результат: bitmap-снимок текущего 3D вида;
- это не настоящий live 3D viewport, а быстрый preview-кадр;
- после поворота/масштаба в Inventor нужно снова нажать `AV Model`.

## v0.4.69 LIVE_3D_PREVIEW

Добавлены два режима live-preview:

- `LV Live`: потоковое обновление bitmap-preview через Timer + `Inventor.ActiveView.SaveAsBitmap`.
- `DK Dock`: экспериментальное встраивание окна `Inventor.ActiveView` в preview panel через Win32 `SetParent`.

`DK Dock` — настоящий live-window режим, но он зависит от того, отдаёт ли Inventor/Interop HWND текущего ActiveView. Если HWND недоступен, используйте `LV Live`.

## v0.4.70 PRIMITIVE_MOUSE_3D_VIEWER

Изменения по canvas:

- кнопки уменьшены до toolbar-формата 32x32;
- подписи убраны с кнопок, смысл находится в ToolTip;
- для кнопок генерируются bitmap-значки в коде;
- справа добавлен локальный primitive 3D viewer.

Primitive viewer строится по данным spatial BASE:

- `SpatialBox ModelBox`;
- `SpatialCubeCell.Bounds`;
- `SpatialBodyRecord.BodyBox`.

Управление мышью:

- ЛКМ + drag: вращение;
- mouse wheel: масштаб;
- double-click: сброс вида.

Это собственная быстрая визуализация RangeBox/BodyBox-примитивов, а не BRep-геометрия Inventor.

## v0.4.71 PRIMITIVE_VIEW_CENTER_FIT

Исправлено смещение primitive viewer в нижний правый угол.

Теперь viewer перед рисованием:

- собирает все углы `ModelBox`, 8 `SpatialCubeCell.Bounds` и 72 `SpatialBodyRecord.BodyBox`;
- считает реальный 2D bounding box после yaw/pitch-проекции;
- подбирает scale/offset по viewport;
- центрирует всю модель в правой preview-области.

Мышь сохранена:

- ЛКМ drag — вращение;
- колесо — zoom;
- double-click — сброс.

## v0.4.72 PRIMITIVE_VISIBLE_CENTER_PAN

Исправлено положение primitive viewer:

- fit считается по `PaintEventArgs.ClipRectangle`, то есть по реально видимой части preview;
- добавлен ручной сдвиг:
  - `Shift + ЛКМ drag` — двигать модель;
  - `ПКМ drag` — двигать модель;
- `ЛКМ drag` без Shift — вращение;
- `wheel` — zoom;
- `double-click` — сброс rotation/zoom/pan.

Лог `PRIMITIVE_VIEW_PROJECTION_FIT` теперь пишет `VisibleBounds`, `PanX`, `PanY`, `VisibleClipFit=True`.

## v0.4.73 BOXGRID_CORE_DLL

Добавлен отдельный проект DLL:

`src/BoxGridCore/BoxGridCore.csproj`

Назначение DLL:

- нейтральное 3D box-grid ядро;
- построение сетки по набору 3D boxes;
- привязка items к ячейкам;
- быстрый query по touched cells / intersecting items / fully-inside items.

Основное приложение теперь вызывает `BoxGridCore` из адаптера `SpatialCubesIndex.BuildCellsWithBoxGridCore`.

Новые логи:

- `BOXGRID_CORE_DLL_USED`

Важно: DLL `BoxGridCore` не содержит ссылок на сторонние CAD/COM-типы и работает только с нейтральными `Box3`, `BoxGridItemInput`, `BoxGrid`, `BoxGridCell`.

## v0.4.74 MESH_VIEW_CORE

Добавлен отдельный нейтральный DLL-проект:

`src/MeshViewCore/MeshViewCore.csproj`

Внутри:

- `MeshPoint3`
- `MeshBox3`
- `MeshTriangle`
- `MeshBody`
- `MeshScene`

На `.ipt canvas` добавлена кнопка `MS Mesh`.

Она строит mesh-preview:

- получает facets/triangles тел через адаптер приложения;
- складывает данные в `MeshViewCore.MeshScene`;
- рисует справа `shaded mesh + wireframe overlay`.

Если facets тела получить не удалось, используется fallback box-mesh по RangeBox, чтобы viewer не оставался пустым.

Новые логи:

- `MESH_VIEW_SCENE_BUILT`
- `MESH_VIEW_SCENE_READY`
- `MESH_BODY_FACETS_FAILED`
- `MESH_VIEW_DRAW_LIMIT_REACHED`

## v0.4.75 MESH_VIEW_CORE_COMPILE_FIX

Исправлена ошибка сборки v0.4.74 в основном проекте:

`CS0246: MeshScene / MeshBody не найден`

Причина:

- `MeshViewCore.dll` собиралась успешно;
- `InventorIptOrg.csproj` имел ProjectReference на `MeshViewCore`;
- но в `Form1.cs` отсутствовал `using MeshViewCore;`.

Исправление:

- добавлен `using MeshViewCore;`;
- mesh-preview логика не менялась.

## v0.4.76 MESH_FACETS_STRONG_CALL

Исправление для mesh-preview после теста v0.4.75.

По логу v0.4.75:

- `MESH_BODY_FACETS_FAILED` сработал для всех 72 тел;
- ошибка: `ArgumentException: Не удалось преобразовать аргумент 2 для вызова на CalculateFacets`;
- итог: `FailedBodies=72; FallbackBodies=72`;
- поэтому viewer показывал не настоящий mesh, а box-mesh fallback.

Изменение:

- `SurfaceBody.CalculateFacets` теперь вызывается строго типизированно:
  - `int vertexCount`
  - `int facetCount`
  - `double[] vertexCoordinates`
  - `double[] normalVectors`
  - `int[] vertexIndices`

Новые/обновлённые логи:

- `MESH_BODY_FACETS_OK`
- `MESH_VIEW_SCENE_BUILT` теперь пишет `RealFacetBodies`.

## v0.4.77 STEP_OPEN_BUTTON

Добавлена новая квадратная кнопка на `.ipt canvas`:

`ST Step`

Назначение:

- выбрать `.stp/.step` через OpenFileDialog;
- открыть файл в активном сеансе через `Documents.Open(path, true)`;
- после открытия можно запускать `8C Cubes` и `MS Mesh`.

Новые логи:

- `STEP_FILE_OPENED`
- `STEP_OPEN_FAILED`
- `STEP_OPEN_CANCELLED`
- `STEP_OPEN_BUTTON_SECONDS`

## v0.4.78 STEP_OPEN_COMPILE_FIX

Исправлена ошибка сборки v0.4.77 в `ST Step`:

- `CS0104: File` неоднозначен между `Inventor.File` и `System.IO.File`;
- `CS0104: Path` неоднозначен между `Inventor.Path` и `System.IO.Path`.

Исправление:

- `File.Exists(...)` заменено на `System.IO.File.Exists(...)`;
- `Path.GetExtension(...)` заменено на `System.IO.Path.GetExtension(...)`.

Логика кнопки `ST Step` не менялась.

## v0.4.79 LOCAL_STEP_VIEWER

Изменена логика кнопки `ST Step`.

Было в v0.4.77/v0.4.78:

- выбрать STEP;
- открыть его во внешнем сеансе через `Documents.Open`.

Стало:

- выбрать STEP;
- прочитать его локально через новый нейтральный проект `StepLiteCore`;
- показать points/edges прямо в `.ipt canvas`;
- внешний сеанс не используется.

Новый проект:

`src/StepLiteCore/StepLiteCore.csproj`

Новые логи:

- `STEP_LOCAL_SCENE_LOADED`
- `STEP_LOCAL_OPEN_FAILED`
- `STEP_LOCAL_OPEN_CANCELLED`
- `STEP_LOCAL_OPEN_BUTTON_SECONDS`

## v0.4.80 NO_EXTERNAL_AUTOSTART

Изменено поведение запуска приложения.

Было:

- при старте `Form1` пытался подключиться к уже запущенному внешнему сеансу;
- если он не найден, вызывался `runtime object creation(...)`;
- из-за этого внешний процесс запускался автоматически.

Стало:

- при старте только `Marshal.GetActiveObject(...)`;
- если активного сеанса нет, приложение продолжает работу с `_invApp = null`;
- автозапуск отключён;
- локальная кнопка `ST Step` продолжает работать без внешнего сеанса через `StepLiteCore`.

Новые логи:

- `STARTUP_ATTACHED_TO_RUNNING_EXTERNAL_SESSION`
- `STARTUP_EXTERNAL_SESSION_NOT_FOUND`
- `COMMAND_ATTACHED_TO_RUNNING_EXTERNAL_SESSION`
- `COMMAND_EXTERNAL_SESSION_NOT_FOUND`

## v0.4.81 LOCAL_STEP_NO_MESSAGEBOX

Исправлено ощущение "долгого зависания" после `ST Step`.

В v0.4.79 успешная локальная загрузка STEP показывала модальный MessageBox.
Лог пользователя показал, что чтение STEP заняло 0.726 с, а ожидание OK в MessageBox заняло больше 42 секунд.

Теперь успешная загрузка не показывает MessageBox:

- результат пишется в левую статусную область;
- пишется `STEP_LOCAL_SCENE_STATUS_NON_BLOCKING`;
- `ST Step` не блокирует UI после загрузки.

Добавлены логи:

- `STEP_LOCAL_FILE_DIALOG_SECONDS`
- `STEP_LOCAL_SCENE_STATUS_NON_BLOCKING`
- `STEP_LOCAL_DRAW_SECONDS`

## v0.4.82 STEP_POLYLOOP_MESH

Исправлен локальный STEP preview.

Проблема v0.4.81:

- читались все `CARTESIAN_POINT`;
- `EDGE_CURVE` в тестовом STEP отсутствовали;
- получалось `58588 points / 0 edges`;
- на экране было облако точек.

Изменение v0.4.82:

- `StepLiteCore` читает `POLY_LOOP`;
- использует только точки из loops;
- строит edges по контуру loop;
- строит triangles через fan triangulation;
- canvas рисует `Local STEP mesh viewer`.

Обновлены логи:

- `STEP_LOCAL_SCENE_LOADED` теперь пишет `PolyLoops`, `AdvancedFaces`, `Triangles`;
- `STEP_LOCAL_DRAW_SECONDS` теперь пишет `Triangles` и `PolyLoops`.

## v0.4.83 STEP_FAST_GDI_PATHS

Исправлена тяжёлая отрисовка локального STEP preview.

Проблема v0.4.82:

- STEP scene строилась быстро;
- но `DrawStepLiteScenePreview` занимал около 27 секунд;
- при любом вращении модели всё перерисовывалось и снова приходилось ждать.

Изменение:

- вместо тысяч отдельных `FillPolygon/DrawPolygon` используется `GraphicsPath`;
- triangles/edges рисуются батчем через `FillPath`/`DrawPath`;
- во время mouse drag включается режим `FastDragWireframe`:
  - без заливки;
  - прореженный wireframe;
  - быстрый поворот.

Обновлён лог:

`STEP_LOCAL_DRAW_SECONDS` теперь пишет `Mode` и `GdiStrategy=GraphicsPathBatch`.

## v0.4.84 UNIFIED_VIEWER_MODES

Добавлена единая система управления viewer без размножения кнопок.

Новые controls на `.ipt canvas`:

- `Source` dropdown: `Auto / STEP / Mesh / BoxGrid`
- `Mode` dropdown: `Wire / Surface / Surface+Edges / Ghost / Points`
- `OPT` popup: `Show points overlay`, `Show BoxGrid overlay`, `Draft while dragging`

Текущая поддержка сохранена:

- `ST Step`
- `MS Mesh`
- `8C Cubes`
- `RF Full`
- `LR Refresh`
- слой временных окон

STEP render теперь переключается режимами:

- Wire
- Surface
- Surface+Edges
- Ghost
- Points

BoxGridCore пока подключён как optional overlay: `OPT -> Show BoxGrid overlay`.

## v0.4.88 FAST_PLEASANT_VIEWER

Практический rollback от тяжёлого LOD-эксперимента v0.4.87 к быстрой viewer-базе v0.4.84.

Изменения:

- default `Mode` = `Surface`;
- `Surface` убирает рябь внутренних рёбер;
- `Surface+Edges` оставлен как отдельный режим;
- drag использует простой `FastDragWireframe` без chunk/LOD overhead;
- FastDrag дополнительно прорежен;
- STEP render log пишет `ViewerProfile=FastPleasant`.

Цель: быстрый и приятный viewer, без ухудшения ощущения вращения.

## v0.4.89 DEFAULT_CANVAS_TAB

Quality-of-life сборка поверх v0.4.88 FAST_PLEASANT_VIEWER.

Изменение:

- после инициализации UI автоматически выбирается вкладка `.ipt canvas`;
- добавлен лог `DEFAULT_TAB_SELECTED`.

Сохранено:

- default `Mode = Surface`;
- быстрый `FastDragWireframe`;
- `ViewerProfile=FastPleasant`;
- без v0.4.87 LOD/chunk overhead.

## v0.4.90 SURFACE_FILL_WINDING_FIX

Исправление Surface mode поверх v0.4.89 DEFAULT_CANVAS_TAB.

Проблема:

- `Mode=Surface` выглядел почти пустым;
- `Mode=Wire` показывал геометрию.

Изменение:

- Surface/Suface+Edges/Ghost используют `GraphicsPath(FillMode.Winding)`;
- Surface-заливка сделана плотнее;
- чистый Surface получает лёгкий bounds silhouette;
- лог пишет `SurfaceFillMode=Winding`.

Сохранено:

- `.ipt canvas` по умолчанию;
- default `Mode = Surface`;
- быстрый `FastDragWireframe`;
- без v0.4.87 LOD/chunk overhead.

## v0.4.91 SURFACE_SOLID_POLYGON_FILL

Исправление после v0.4.90.

Проблема:

- лог показывал `DrawnTriangles=23696`;
- `SurfaceFillMode=Winding` был включён;
- визуально оставался только bounding параллелепипед.

Изменение:

- Surface больше не использует один общий `GraphicsPath.FillPath`;
- каждый triangle рисуется напрямую через `g.FillPolygon`;
- Surface+Edges использует прямую заливку + edge path;
- лог пишет `SurfaceFillMode=SolidFillPolygon`;
- FastDrag остаётся быстрым wireframe.

Цель: сначала получить видимую поверхность, затем уже улучшать shading.

## v0.4.92 SURFACE_DEPTH_SHADE

Улучшение Surface после v0.4.91.

Изменения:

- `Mode=Surface` теперь строит draw-list треугольников;
- добавлен painter depth sort;
- добавлено pseudo-shading по view-space normal/light;
- `Surface+Edges` использует тот же shaded surface;
- FastDrag остаётся быстрым wireframe;
- добавлен лог `STEP_SURFACE_SHADE_STATS`.

Лог рендера:

- `SurfaceFillMode=DepthSortedShadedPolygons`
- `GdiStrategy=DepthSortedFillPolygonPseudoShade`

## v0.4.93 SURFACE_VISIBLE_SHELL

Улучшение Surface после v0.4.92.

Изменения:

- `Surface` / `Surface+Edges` отбрасывают back-facing triangles;
- обычный Surface стал opaque (`alpha=255`);
- `Ghost` оставлен прозрачным отдельным режимом;
- убран bounding-box silhouette из Surface draw path;
- усилен pseudo-shading;
- добавлен лог `STEP_SURFACE_SHELL_STATS`.

Лог рендера:

- `SurfaceFillMode=OpaqueVisibleShell`
- `GdiStrategy=VisibleShellDepthSortedPseudoShade`
- `CulledBackFacingTriangles`
- `DrawnFrontFacingTriangles`
- `SurfaceOpaque=True`

## v0.4.94 SURFACE_FEATURE_EDGES

Улучшение Surface после v0.4.93.

Изменения:

- `Surface+Edges` стал default mode;
- добавлен feature-edge overlay поверх visible shell;
- рисуются не все triangle edges, а boundary + crease edges;
- triangle normals сохраняются в `StepSurfaceDrawTriangle`;
- добавлен лог `STEP_FEATURE_EDGES_BUILT`.

Лог рендера:

- `STEP_FEATURE_EDGES_BUILT`
- `DrawnFeatureEdges`
- `BoundaryEdges`
- `CreaseEdges`
- `SurfaceFillMode=OpaqueVisibleShellFeatureEdges`
- `GdiStrategy=VisibleShellFeatureEdgesPseudoShade`

## v0.4.95 SOFT_SHADE_SILHOUETTE

Улучшение CAD-вида после v0.4.94.

Изменения:

- исправлена edge adjacency: все triangles + quantized 3D coordinate keys;
- silhouette, crease и boundary edges рисуются разными тонкими стилями;
- добавлен мягкий directional shade;
- добавлена лёгкая contact shadow;
- добавлен светлый CAD-style background;
- FastDrag остаётся быстрым wireframe.

Новые логи:

- `STEP_SOFT_SILHOUETTE_EDGES_BUILT`
- `SilhouetteEdges`
- `BoundaryEdges`
- `CreaseEdges`
- `SoftContactShadow=True`
- `SurfaceFillMode=SoftShadeOpaqueShell`
- `GdiStrategy=SoftShadeSilhouetteContactShadow`

## v0.4.96 ZBUFFER_HIDDEN_LINE

Первый software Z-buffer renderer для локального STEP viewer.

Изменения:

- Surface triangles растеризуются в depth-buffer;
- ближайшая view-depth хранится для каждого пикселя;
- silhouette/boundary/crease edges проходят depth-test;
- скрытые участки линий не рисуются;
- contact shadow временно отключена;
- Ghost и FastDrag сохранены отдельными лёгкими режимами.

Новые логи:

- `STEP_ZBUFFER_SURFACE_STATS`
- `STEP_ZBUFFER_HIDDEN_LINE_STATS`
- `SurfaceFillMode=ZBufferOpaqueShell`
- `GdiStrategy=SoftwareZBufferHiddenLine`

## v0.4.97 ADVANCED_FACE_ORIENTATION

Добавлена orientation-aware цепочка STEP topology:

- `ADVANCED_FACE`
- `FACE_OUTER_BOUND / FACE_BOUND`
- `POLY_LOOP`

Порядок loop разворачивается, когда:

`FACE_BOUND.orientation != ADVANCED_FACE.same_sense`

Добавлен лог `STEP_ADVANCED_FACE_ORIENTATION`.

Важно: в текущих тестовых IceCore Mini и Rubber Hose все orientation flags равны `.T.`,
поэтому ожидается `ReversedFaceLoops=0`. Z-buffer v0.4.96 сохранён.
 
## v0.4.98 ZBUFFER_VISIBLE_MESH

Добавлен режим `Surface+Mesh`.

- Surface строится существующим software Z-buffer.
- Mesh берётся из `StepLiteScene.Edges`, то есть из границ `POLY_LOOP`.
- Диагонали fan-triangulation не рисуются.
- Каждое mesh-ребро проходит Z-buffer depth-test.
- Скрытые линии не отображаются.
- `Surface+Mesh` выбран default mode для теста.

Новый лог:

- `STEP_ZBUFFER_VISIBLE_MESH_STATS`
- `MeshSource=StepLiteScene.Edges`
- `TriangleDiagonalsIncluded=False`

## v0.4.99 ZBUFFER_SMOOTH_SHADE_MESH

Добавлено crease-aware Gouraud shading поверх рабочего Z-buffer v0.4.98.

- vertex normals объединяются по quantized 3D coordinates;
- sharp edges сохраняются порогом `dot >= 0.72`;
- три vertex shades интерполируются на каждый pixel;
- mesh lines стали светлее;
- silhouette overlay стал однопиксельным;
- depth tolerance слегка увеличен против пунктирных разрывов.

Новые логи:

- `ShadeInterpolation=GouraudBarycentric`
- `SmoothVertexKeys`
- `SilhouetteOverlayEdges`
- `SurfaceFillMode=ZBufferGouraudSmoothMesh`
- `GdiStrategy=SoftwareZBufferGouraudMesh`

## v0.5.00 INVENTOR_STYLE_SHADED_MESH

Визуальная калибровка рабочего renderer v0.4.99 по эталонному виду Rubber Hose в Autodesk Inventor.

- surface palette затемнена до нейтрального CAD-gray;
- сохранено Gouraud smoothing без flat-shading полос;
- POLY_LOOP mesh и silhouette стали тёмными и читаемыми;
- фон viewer стал более выраженным blue-gray gradient;
- software Z-buffer, hidden-line removal и `TriangleDiagonalsIncluded=False` сохранены.

Новые логи:

- `SurfacePalette=InventorNeutralGray`
- `SurfaceShadeClamp=145..214`
- `BackgroundProfile=InventorBlueGray`
- `MeshLineRgb=68`
- `SilhouetteLineRgb=44`
- `ViewerProfile=InventorStyleGrayMesh`
- `SurfaceFillMode=ZBufferInventorGrayGouraudMesh`
- `GdiStrategy=SoftwareZBufferInventorStyleMesh`
## v0.5.01 DEEPER_SHADE_SPLIT_EDGE_BIAS

Точечная доводка стабильной v0.5.00 по реальному Rubber Hose тесту:

- более глубокий Gouraud gray clamp `115..218`;
- обычная POLY_LOOP-сетка осветлена до `RGB 84`;
- silhouette сохранён тёмным `RGB 40`;
- отдельные Z-depth tolerance для mesh (`0.0025`), feature (`0.0032`) и silhouette (`0.0040`);
- software Z-buffer, hidden-line removal и отсутствие fan-triangle diagonals сохранены.

Базовый пользовательский замер v0.5.00: Surface+Mesh 0.100–0.110 s. Фактический замер v0.5.01 ожидается после запуска.

## v0.5.02 RENDER_METHOD_LAB_LOGGING

Добавлена лаборатория методов рендера прямо в существующий список `Mode`:

- `Surface+Mesh` — точная визуальная база v0.5.01;
- `Mesh Deep Clamp` — тот же свет, но без скрытого pre-clamp `145..214`;
- `Mesh Two-Light` — directional key + fill;
- `Mesh Hemisphere` — hemisphere ambient + key;
- `Mesh Inventor Lab` — key/fill/facing/top/rim с gamma и более строгим mesh depth bias.

Каждое переключение пишет `VIEWER_RENDER_METHOD_SELECTED` с полным профилем: LightModel, input/output shade clamps, RGB линий и три depth-tolerance factor. После перерисовки выбранный `RenderMethod` также повторяется в `STEP_ZBUFFER_SURFACE_STATS`, `STEP_ZBUFFER_HIDDEN_LINE_STATS`, `STEP_ZBUFFER_VISIBLE_MESH_STATS`, `STEP_SURFACE_SHELL_STATS` и `STEP_LOCAL_DRAW_SECONDS`.

Выявлена важная деталь v0.5.01: перед финальным clamp `115..218` vertex shades всё ещё предварительно ограничивались диапазоном `145..214`, поэтому визуальное углубление теней было слабее ожидаемого. Исходный вид сохранён как baseline, а исправленный вариант вынесен в отдельный `Mesh Deep Clamp` для прямого сравнения.

Пользовательский замер v0.5.01 на Rubber Hose: `Surface+Mesh` 0.099 s и 0.111 s; surface pass 0.024–0.028 s; 2661 видимое mesh-ребро.


## v0.5.03 MESH_VISIBILITY_THRESHOLD

Версии в выпадающем списке `Mode` теперь обозначают историческую версию метода, а не текущую версию приложения. Например: `Surface+Mesh — v0.4.99`, `Mesh Deep Clamp — v0.5.02`, `Mesh Inventor Lab — v0.5.03`.

`Mesh Inventor Lab — v0.5.03` добавляет фильтр видимости обычных POLY_LOOP-рёбер:

- front-depth берётся из окрестности 3×3 пикселя, чтобы рёбра не просачивались через raster gaps;
- mesh-ребро рисуется только при доле depth-visible samples не ниже `0.65`;
- silhouette и feature edges сохраняют отдельные depth tolerance и не проходят ratio-фильтр;
- в логах разделены `AppVersion` и `MethodVersion`;
- добавлены `AcceptedMeshEdgesByRatio`, `RejectedMeshEdgesByRatio`, `HiddenByFrontDepth`, `MeshVisibleSampleRatioThreshold`, `MeshDepthNeighborhoodRadius`.

Сохранены software Z-buffer, Gouraud shading, AdvancedFace orientation, `StepLiteScene.Edges`, `TriangleDiagonalsIncluded=False`, FastDragWireframe и локальный STEP viewer без запуска Inventor.

База сравнения по пользовательскому логу v0.5.02 на Rubber Hose: Mesh Inventor Lab `0.130–0.154 s`, surface pass `0.023–0.035 s`, 2649 видимых combined edges. Пользовательский замер v0.5.03 на Rubber Hose: Mesh Inventor Lab 0.117–0.155 s; surface pass 0.033–0.051 s; 1188 принятых mesh-рёбер и 1884 отклонённых по ratio.
## v0.5.04 OPAQUE_BODY_MESH

Добавлен новый исторический режим:

`Mode: Mesh Opaque Body — v0.5.04`

Он сохраняет визуальную палитру и 65% visible-sample gate из `Mesh Inventor Lab — v0.5.03`, но дополнительно связывает каждое обычное POLY_LOOP-ребро с треугольниками поверхности, которым оно геометрически принадлежит.

Новый opaque-owner алгоритм:

- surface pass по-прежнему пишет полностью непрозрачные пиксели `SurfaceAlpha=255`;
- Z-buffer теперь хранит не только depth, но и ID ближайшего front-surface triangle для каждого пикселя;
- для каждого POLY_LOOP edge строится список допустимых surface-owner triangles по quantized 3D edge geometry;
- mesh sample проходит только тогда, когда front Z-buffer owner совпадает с одним из владельцев этого ребра;
- дальнее или внутреннее ребро не может просочиться через несвязанную переднюю оболочку даже при близких depth;
- silhouette и feature edges сохраняют отдельные правила и не требуют mesh-owner match;
- `Ghost` остаётся единственным намеренно прозрачным surface-режимом.

Новые поля логов:

- `SurfaceAlpha`
- `OpaqueBody`
- `OpaqueSurfaceOwnerPass`
- `RequireMeshSurfaceOwnerMatch`
- `MeshOwnerNeighborhoodRadius`
- `MeshOwnerMatchedSamples`
- `MeshOwnerRejectedSamples`
- `MeshEdgesWithoutSurfaceOwner`

Пользовательский замер v0.5.03 на Rubber Hose сохранён в performance comments: `Mesh Inventor Lab` 0.117–0.155 s, surface pass 0.033–0.051 s, 1188 принятых mesh-рёбер, 1884 отклонённых по ratio и 42956 samples, скрытых front depth. Фактический замер v0.5.04 ожидается после запуска.


## v0.5.05 — Mesh Solid Front

Новый режим по умолчанию: `Mode: Mesh Solid Front — v0.5.05`.

Он сохраняет полностью непрозрачную поверхность (`SurfaceAlpha=255`), возвращает внешний POLY_LOOP-каркас после слишком строгого v0.5.04 owner-gate и применяет явное solid-front освещение с небольшим depth cue. Старые методы остаются в списке с историческими версиями.

## v0.5.06 — Mesh True Front

Новый default mode:

`Mode: Mesh True Front — v0.5.06`

Это исправление не меняет alpha: предыдущий лог уже доказал, что поверхность была полностью непрозрачной. Исправлена настоящая причина эффекта прозрачности — обратное camera-space правило front/back face. При `GreaterViewZIsNear` ближайшая наружная поверхность имеет `NormalZ > 0`, а legacy renderer удалял именно её.

v0.5.06 выполняет полный nearest-surface depth pass по всем triangles, использует правильный `NormalZPositive` front-face rule, camera-correct освещение и снова накладывает только mesh, прошедший depth test настоящей ближайшей оболочки.

## v0.5.07 — True Front Face compile fix

Исправлена блокирующая сборку ошибка `CS0136`: в ветке метода `TrueFrontFaceZBufferV0506` теперь используется ранее объявленная переменная `brightness`. Визуальный алгоритм v0.5.06 сохранён без изменений.

## v0.5.08 — Drag backbuffer and FPS

Interactive LMB rotation now uses an atomic off-screen backbuffer instead of clearing the visible panel and drawing a dense diagnostic wireframe directly on it. MouseMove events are coalesced to a 30 FPS timer. During drag, the current camera orientation is rendered as a reduced-resolution shaded True Front surface; on mouse release, one full-quality frame is generated. An FPS/frame-time/coalesced-input overlay is shown in the upper-right corner.

The historical final-quality method remains `Mode: Mesh True Front — v0.5.06`.


## v0.5.09 — Drag camera center lock

- Replaced projected-bounding-box recentering with `FitMode=SymmetricModelCenter` for STEP preview.
- The 3D model-bounds center is mapped to the viewport center for every yaw/pitch.
- Mouse-down position does not become an orbit pivot.
- Reduced 55% drag frames and full frames use the same visual center after scaling.
- Explicit Shift+LMB/RMB pan remains unchanged.
- Added `STEP_VIEW_CAMERA_CENTER` diagnostics and drag-session pivot fields.
- Preserved v0.5.08 atomic backbuffer/FPS and historical `Mesh True Front — v0.5.06`.


## v0.5.10 — Drag DPI center fix

- Replaced `DrawImageUnscaled` for viewer and Z-buffer presentation with explicit destination/source pixel rectangles.
- Backbuffer bitmaps inherit the active paint-surface DPI and all intermediate Graphics objects use `GraphicsUnit.Pixel`.
- Restored projected visual-bounds centering while keeping the 3D orbit pivot fixed at the model Bounds center.
- Added an exact screen-space center correction and reduced fit factor `0.88` for safe margins.
- Added `VIEWER_BACKBUFFER_MAPPING` and expanded `STEP_VIEW_CAMERA_CENTER` diagnostics.
- Preserved v0.5.08 backbuffer/FPS and historical `Mesh True Front — v0.5.06`.




## ИСТОРИЯ
### 1 ai-operations-ui-demo.zip — простой Blazor UI
### 2 topic-knowledge-base-demo.zip — справочник тем 101–115
### 3 docx-generator-demo.zip — отдельная генерация DOCX
### 4 workflow-pipeline-demo.zip — конвейер в консольном варианте
### 5 generated-files-api-demo.zip — API для выдачи файлов
### 6 ai-document-workflow-demo.zip — главный полный MVP с UI + конвейером
### 7 ai-document-workflow-devops-lab.zip — расширенная версия MVP с DevOps-обвязкой: health-check endpoints, Docker, Jenkinsfile, GitHub Actions CI и примеры AI infrastructure lab
### 8 ai-infra-kafka-s3-vault-dropapp-lab.zip — отдельный инфраструктурный lab: ASP.NET Core API + Redpanda/Kafka-compatible broker + MinIO/S3-compatible storage + HashiCorp Vault + dropapp-style manifest
### 9 ai-document-workflow-kafka-s3-vault-lab.zip — связка логики AI Document Workflow из проекта 6 с инфраструктурой Kafka/S3/Vault
### 10 ai-document-workflow-devops-ops-lab.zip — DevOps/Ops версия под Jenkins, Ansible, Groovy, Kafka, S3, Vault, Istio, dropapp-style, ИФТ/ПСИ и production-сопровождение
### 11 ai-document-workflow-observability-support-lab.zip — observability/support версия под сопровождение ПО: Prometheus, Grafana, Alertmanager, /metrics, healthz/readyz, readiness-проверки Kafka/S3/Vault, dashboard panels, alert rules, incident snapshot и runbook для диагностики инцидентов
### 12 ai-document-workflow-postgresql-support-lab.zip — PostgreSQL/support версия под сопровождение БД: PostgreSQL 16, Adminer, SQL schema/seed scripts, диагностические SQL-запросы, JOIN/GROUP BY/CTE, индексы, VIEW, PL/pgSQL functions, document health, incident diagnostics, release health и API endpoints для проверки состояния данных
### 13 ai-document-workflow-mvp-postgresql-support-lab.zip — гибрид проекта 6 и 12: главный MVP с UI + document workflow pipeline, объединённый с PostgreSQL/support-слоем для сопровождения БД: Docker Compose, PostgreSQL 16, Adminer, SQL schema/seed scripts, индексы, VIEW, PL/pgSQL functions, diagnostic SQL queries, document health, incident diagnostics, release health, /healthz, /readyz, /metrics, Prometheus, Grafana и Alertmanager
### 14 ai-document-workflow-business-data-analytics-lab.zip — Аналитический Blazor UI lab под роль аналитика: Visual Studio solution, запуск сценария анализа городских обращений, KPI по SLA/категориям/районам, BPMN/UML, business requirements, ТЗ, API/OpenAPI, SQL, Python ETL, dashboard spec и ML data preparation.
### 15-mortgage-vba-excel-sql-dashboard-lab.v2-blazor-ui-sln.zip — Excel/VBA/SQL lab под автоматизацию ипотечной отчётности: Visual Studio solution, Blazor UI для запуска сценария, ипотечный калькулятор, импорт CSV/SQL-выгрузок, VBA-модули, расчёт платежей/PTI/LTV, dashboard, data quality checks и регулярные отчёты.
### 16-ai-document-workflow-itil-change-management-lab.v1-html-ui-sln — ITIL/ITSM Change Management lab под инженера внедрения: Visual Studio solution, HTML UI с кнопкой формирования календаря изменений, ЗНИ/RFC, оценка рисков, поиск конфликтов, CAB checklist, планы внедрения и отката, связь с incident/problem management, KPI и SQL-отчётность.
### 17-ai-document-workflow-itsm-incident-analytics-lab.v2-blazor-ui-sln — ITSM Incident Analytics lab под роль аналитика в ITSM-системе: Blazor UI для анализа технологических инцидентов, KPI по SLA/MTTR/impact/root cause, контроль качества данных ITSM, аудит мероприятий, SQL-отчётность, RCA, dashboard spec и management report.
### 18-ml-model-validation-classic-ml-llm-lab.v5-russian-ui-comments.zip — Classic ML / LLM validation DS lab: Python pandas/sklearn, baseline logistic regression, challenger gradient boosting, ROC-AUC/Gini/KS/F1, backtest по историческим периодам, PSI/drift monitoring, LLM evaluation, generated CSV reports и Blazor UI-витрина.
### 19-Inventor-external-tool-MVP — Внешнее WinForms-приложение на .NET Framework для Autodesk Inventor. Поддерживает выбор тел в IPT рамкой, поиск внутренних и скрытых тел через RangeBox, редактируемые списки тел и элементов, копирование списков в буфер обмена, а также группировку компонентов в IAM-сборках.
### 20-Inventor-ipt-organizer — Внешний WinForms-инструмент для Autodesk Inventor: выбор тел IPT рамкой, поиск внутренних/скрытых тел через RangeBox, редактируемые списки объектов и создание папок в дереве Inventor.
### 21-Inventor-ipt-tree-editor — Внешний WinForms-инструмент для Autodesk Inventor. Работает с IPT через COM: выбор тел рамкой, поиск внутренних/скрытых тел через RangeBox, создание папок в дереве Inventor, редактирование имени/XYZ, JSON-экспорт дерева модели и логирование производительности.
### 22-Inventor-ipt-spatial-cube-selector — Внешний WinForms-инструмент для Autodesk Inventor IPT: spatial-cube индекс тел, быстрое выделение рамкой через RealCamera aspect-corrected projection, фильтрация body-box, временная подсветка кубов/следа и hide/show текущего visible-list.
### 23-Inventor-ipt-rangebox-tracebox-selector — Внешний WinForms-инструмент для Autodesk Inventor IPT: быстрый выбор тел через spatial-cube индекс и RealCamera aspect-corrected projection, режимы Cube hit all/filtered, строгая фильтрация SurfaceBody.RangeBox внутри Selection Trace Box, цветная ClientGraphics-подсветка RangeBox и hide/show visible-list.
### 24-Canvas-step-viewer — WinForms-инструмент для локального просмотра STEP в .ipt canvas: StepLite-парсер, software Z-buffer, непрозрачный True Front renderer, POLY_LOOP-сетка, исторические режимы рендера и FPS, backbuffer и интерактивное вращение. В GitHub-режиме показывается только canvas.


## License

Copyright (c) 2026 Андрей / LA00001

All rights reserved.

This repository is provided for portfolio and demonstration purposes only.
Copying, redistribution, modification, sublicensing, commercial use, or publication
of the source code is not permitted without prior written permission from the author.

---

Авторское право (c) 2026 Андрей / LA00001

Все права защищены.

Данный репозиторий предоставлен только для демонстрации в портфолио.
Копирование, распространение, изменение, сублицензирование, коммерческое использование
или публикация исходного кода не допускаются без предварительного письменного разрешения автора.