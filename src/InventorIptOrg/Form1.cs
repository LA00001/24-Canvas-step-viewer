// (C) Copyright 2011 Autodesk, Inc.
//
// Неформальный перевод уведомления Autodesk:
// разрешается использовать, копировать, изменять и распространять это ПО
// в объектном коде для любых целей и без оплаты при условии, что указанное
// уведомление об авторских правах сохраняется во всех копиях, а также
// присутствует в сопроводительной документации.
//
// AUTODESK ПРЕДОСТАВЛЯЕТ ЭТУ ПРОГРАММУ «КАК ЕСТЬ» И СО ВСЕМИ НЕДОСТАТКАМИ.
// AUTODESK ОТКАЗЫВАЕТСЯ ОТ ЛЮБЫХ ПОДРАЗУМЕВАЕМЫХ ГАРАНТИЙ,
// ВКЛЮЧАЯ ПРИГОДНОСТЬ ДЛЯ ПРОДАЖИ ИЛИ ДЛЯ КОНКРЕТНОЙ ЦЕЛИ.
// AUTODESK НЕ ГАРАНТИРУЕТ БЕСПЕРЕБОЙНУЮ ИЛИ БЕЗОШИБОЧНУЮ РАБОТУ ПРОГРАММЫ.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Inventor;
using MeshViewCore;
using StepLiteCore;

namespace InventorIptOrg
{
    public partial class Form1 : Form
    {
        // 13:05 17.06.2026 InventorIptOrg_v0_4_43_DESIGNER_NEWLINE_FIX
        // Сборочный FIX: Form1.Designer.cs ButtonIptCreateFeatureFolder2.Text использует escaped "\r\n", а не реальный newline внутри строки.

        private sealed class StepSurfaceDrawTriangle
        {
            public System.Drawing.PointF[] Poly { get; set; }
            public StepTriangle Triangle { get; set; }
            public double Depth { get; set; }
            public int Shade { get; set; }
            public int ShadeA { get; set; }
            public int ShadeB { get; set; }
            public int ShadeC { get; set; }
            public bool BackFacing { get; set; }
            public double NormalX { get; set; }
            public double NormalY { get; set; }
            public double NormalZ { get; set; }
            public string VertexKeyA { get; set; }
            public string VertexKeyB { get; set; }
            public string VertexKeyC { get; set; }
        }

        private sealed class StepNormalSample
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public double Weight { get; set; }
        }

        private sealed class StepFeatureEdgeInfo
        {
            public int A { get; set; }
            public int B { get; set; }
            public int Count { get; set; }
            public int FrontFacingCount { get; set; }
            public int BackFacingCount { get; set; }
            public StepSurfaceDrawTriangle FirstTriangle { get; set; }
            public StepSurfaceDrawTriangle SecondTriangle { get; set; }
        }

        private sealed class StepVisibleEdgeSegment
        {
            public int A { get; set; }
            public int B { get; set; }
            public int Style { get; set; }
            public List<int> SurfaceOwnerTriangleIds { get; set; }
        }

        private sealed class StepZBufferFrame
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public int[] Pixels { get; set; }
            public double[] Depth { get; set; }
            public int[] SurfaceOwnerTriangleIds { get; set; }
            public int SurfacePixelsWritten { get; set; }
            public long SurfacePixelsRejectedByDepth { get; set; }
            public int CandidateEdges { get; set; }
            public int CandidateMeshEdges { get; set; }
            public int VisibleEdges { get; set; }
            public int VisibleMeshEdges { get; set; }
            public int AcceptedMeshEdgesByRatio { get; set; }
            public int RejectedMeshEdgesByRatio { get; set; }
            public long VisibleEdgeSamples { get; set; }
            public long HiddenEdgeSamples { get; set; }
            public long VisibleMeshSamples { get; set; }
            public long HiddenMeshSamples { get; set; }
            public long HiddenByFrontDepth { get; set; }
            public long MeshOwnerMatchedSamples { get; set; }
            public long MeshOwnerRejectedSamples { get; set; }
            public int MeshEdgesWithoutSurfaceOwner { get; set; }
            public int OpaqueSurfacePixelCount { get; set; }
            public int NonOpaqueSurfacePixelCount { get; set; }
            public int DepthCuePixelsAdjusted { get; set; }
            public int NearestPositiveNormalZPixels { get; set; }
            public int NearestNegativeNormalZPixels { get; set; }
            public int LegacyRuleWouldCullNearestPixels { get; set; }
        }

        private sealed class StepRenderMethodProfile
        {
            public string MethodId { get; set; }
            public string MethodVersion { get; set; }
            public string DisplayName { get; set; }
            public string LightModel { get; set; }
            public string SurfacePalette { get; set; }
            public string ViewerProfile { get; set; }
            public string SurfaceFillMode { get; set; }
            public string GdiStrategy { get; set; }
            public int InputShadeMin { get; set; }
            public int InputShadeMax { get; set; }
            public int SurfaceShadeMin { get; set; }
            public int SurfaceShadeMax { get; set; }
            public int MeshLineRgb { get; set; }
            public int FeatureLineRgb { get; set; }
            public int SilhouetteLineRgb { get; set; }
            public double MeshDepthToleranceFactor { get; set; }
            public double FeatureDepthToleranceFactor { get; set; }
            public double SilhouetteDepthToleranceFactor { get; set; }
            public double MeshVisibleSampleRatioThreshold { get; set; }
            public int MeshDepthNeighborhoodRadius { get; set; }
            public int SurfaceAlpha { get; set; }
            public bool RequireMeshSurfaceOwnerMatch { get; set; }
            public int MeshOwnerNeighborhoodRadius { get; set; }
            public int DepthCueStrength { get; set; }
            public double DepthCueGamma { get; set; }
            public bool FrontFacingPositiveViewZ { get; set; }
            public bool RasterizeAllTrianglesForDepth { get; set; }
            public string Notes { get; set; }
        }

        // v0.5.16 GitHub presentation mode: only the local STEP canvas is exposed.
        // The visible workflow does not attach to or call Autodesk Inventor.
        // Legacy Inventor code remains in the historical source project but is not initialized from this build.
        // v0.5.17 warning-free build: this is runtime-readonly rather than const, so the legacy
        // fallback branches remain valid source code and the C# compiler no longer reports CS0162.
        private static readonly bool CanvasOnlyGitHubMode = true;

        private Inventor.Application _invApp;
        private bool _started;

        // v0.4.69 LIVE_3D_PREVIEW — experimental Win32 docking for Inventor ActiveView.
        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 0x40000000;
        private const int WS_VISIBLE = 0x10000000;
        private const int WS_CAPTION = 0x00C00000;
        private const int WS_THICKFRAME = 0x00040000;
        private const int WS_POPUP = unchecked((int)0x80000000);
        private const uint SWP_NOZORDER = 0x0004;
        private const uint SWP_NOACTIVATE = 0x0010;
        private const uint SWP_FRAMECHANGED = 0x0020;

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        // Эти поля ОБЯЗАТЕЛЬНО должны жить, пока Inventor ждёт рамочного выбора пользователя.
        // Если сделать их локальными переменными, COM-события может собрать GC, и OnSelect не сработает.
        private InteractionEvents _iptWindowInteractionEvents;
        private SelectEvents _iptWindowSelectEvents;
        private bool _iptWindowSelectionRunning;
        private bool _iptWindowSelectionWasHandled;
        private bool _originalShowPromptTooltips;
        private FormWindowState _windowStateBeforeSelection;

        // 19:05 16.06.2026 InventorIptOrg_v0_4_31_FEATURE_BROWSER_FOLDER_REANIMATE
        // FAST FEATURE-LIST CACHE:
        // v0.4.30 intentionally kept .ipt cubes as a fast visible-list and skipped feature/attribute refresh.
        // v0.4.31 reanimates the old Features list on demand from the current visible BodyListItem list.
        // The cache is per SurfaceBody COM identity and is used only inside the current application run.
        private readonly Dictionary<IntPtr, List<object>> _featureObjectsByBodyKey = new Dictionary<IntPtr, List<object>>();

        // 20:35 16.06.2026 InventorIptOrg_v0_4_33_ASSEMBLYINFO_PATH_FIX
        // Сборочный фикс после теста v0.4.32:
        // пакет с короткими путями потерял файл src\InventorIptOrg\Properties\AssemblyInfo.cs,
        // а .csproj корректно ожидал Compile Include="Properties\AssemblyInfo.cs".
        // v0.4.33 возвращает Properties/AssemblyInfo.cs и сохраняет короткую структуру путей.

        // 21:05 16.06.2026 InventorIptOrg_v0_4_34_TRACE_DEPTH_FIX
        // Визуальный FIX после теста: желто-зелёный trace box был слишком тонким по глубине,
        // потому что RealCamera branch строил box только как тонкий слой вокруг camera target plane.
        // Пользователь попросил: trace box должен занимать ПОЛНУЮ глубину всех задетых spatial-cubes.
        // v0.4.34 использует hidden-range по всем touched cells и для RealCamera branch после вычисления
        // экранного X/Y (или Y/Z, X/Z) расширяет скрытую ось до полного диапазона bounds задетых кубов.

        // 21:20 16.06.2026 InventorIptOrg_v0_4_35_BODY_RANGEBOXES
        // Сборочный FIX: v0.4.34 вызывал ExpandTraceBoxToHiddenRange, но helper не попал в класс Form1.
        // Функциональный FIX: добавлена кнопка Show/Hide body RangeBoxes.
        // Она рисует SurfaceBody.RangeBox текущего visible-list как временную ClientGraphics-графику Inventor
        // в полупрозрачных случайных цветах, без создания настоящих IPT-тел и без засорения browser tree.

        // 21:30 16.06.2026 InventorIptOrg_v0_4_36_BODY_RANGEBOXES_COMPILE_FIX
        // Сборочный FIX после теста v0.4.35:
        // 1) Environment.TickCount был неоднозначен между Inventor.Environment и System.Environment -> System.Environment.TickCount.
        // 2) TransientObjects.CreateColor требует byte, а random.Next возвращает int -> cast к byte.
        // 3) helper ExpandTraceBoxToHiddenRange реально добавлен в класс Form1 перед GetPlaneModelRanges.
        // Функция Show / Hide body RangeBoxes сохранена.

        // 21:45 16.06.2026 InventorIptOrg_v0_4_37_BODYBOX_COLOR_EDGES
        // Визуальный FIX после теста v0.4.36:
        // SurfaceBody.RangeBox был построен 8 раз, но в Inventor были видны в основном тонкие контуры,
        // а случайные полупрозрачные surface colors почти не различались.
        // v0.4.37 добавляет каждому RangeBox яркий толстый wireframe-контур по 12 рёбрам через CurveGraphics
        // и оставляет очень лёгкую face-заливку. Цвета берутся из high-contrast palette по индексу тела.
        // Также default body mode при запуске: Cube hit: filtered bodies.

        // 22:05 16.06.2026 InventorIptOrg_v0_4_38_RANGEBOX_INSIDE_TRACEBOX
        // Новый режим отбора тел:
        // 1. Cube hit: all bodies — все тела из задетых spatial-cubes.
        // 2. Cube hit: filtered bodies — кандидаты из кубиков, чьи BodyBox пересекаются с экранной рамкой.
        // 3. RangeBox fully inside TraceBox — кандидаты, чей SurfaceBody.RangeBox полностью лежит внутри
        // салатового Selection Trace Box / CustomRectTraceBox.
        // Этот режим нужен для строгого отбора только целиком попавших в 3D-параллелепипед рамки тел.

        // 00:05 17.06.2026 InventorIptOrg_v0_4_39_BROWSER_SUBFOLDERS
        // Новый workflow после успешного Create feature browser folder:
        // если пользователь уже создал BrowserFolder в дереве Inventor и выделил внутренние BrowserNode/Features,
        // кнопка Create subfolder from selected nodes создаёт новую BrowserFolder из этих выбранных внутренних узлов.
        // Основной путь: берём selected rows из нашей browser-tree grid или выделение Inventor SelectSet/BrowserPane,
        // собираем BrowserNode напрямую и вызываем BrowserPane.AddBrowserFolder без полного RefreshIptBrowserTree.

        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX =====================================================================================================
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Spatial Cubes Index / виртуальная база кубиков модели.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX По умолчанию режим выключен: выбор рамкой работает по старому полному foreach SurfaceBodies.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX После нажатия Build spatial cubes создаётся индекс 2x2x2 = 8 кубиков, и рамка берёт кандидатов из задетых кубиков.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX =====================================================================================================
        // 17:30 14.06.2026 InventorIptOrg_v0_4_11_SPATIAL_COMMENT_TIME_FIX =====================================================================================================
        // 17:30 14.06.2026 InventorIptOrg_v0_4_11_SPATIAL_COMMENT_TIME_FIX ВАЖНО: исторические performance-комментарии v0.4.5 восстановлены без подмены версии.
        // 17:30 14.06.2026 InventorIptOrg_v0_4_11_SPATIAL_COMMENT_TIME_FIX Spatial cubes code оставлен из v0.4.7; старые цифры v0.4.5 остаются OLD BASELINE.
        // 17:30 14.06.2026 InventorIptOrg_v0_4_11_SPATIAL_COMMENT_TIME_FIX В v0.4.11 уточнено различие: кубики тела != кубики, задетые текущей selectionLimits рамки.
        // 17:30 14.06.2026 InventorIptOrg_v0_4_11_SPATIAL_COMMENT_TIME_FIX =====================================================================================================
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT =====================================================================================================
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT Добавлен новый режим: CUSTOM CUBE RECT SELECT.
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT Это НЕ Inventor WindowSelect: программа ловит собственную экранную рамку поверх окна Inventor.
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT Направление протяжки больше не должно менять результат: left->right и right->left нормализуются в один Rectangle.
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT MVP-режим выбирает кубики по 2D-рамке и добавляет тела из этих кубиков в группу.
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT Старый Select Frame оставлен как исторический/резервный режим.
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT =====================================================================================================
        // 19:05 16.06.2026 InventorIptOrg_v0_4_31_FEATURE_BROWSER_FOLDER_REANIMATE ===============================================
        // ЗАМЕР НОВОГО РЕЖИМА / переход после v0.4.30:
        // v0.4.30 закрыл пункт Select Frame + Add Inner/Hidden Bodies через .ipt cubes и RealCamera aspect-corrected projection.
        // v0.4.31 начинает следующий пункт: Create feature browser folder.
        // Проблема: после быстрого .ipt cubes выбора listBoxIptFeatures пустой, потому что FAST HIDE READY path намеренно пропускал
        // AddBodyToGroup / attributes / features / SelectSet / Refresh списков.
        // Решение: добавлен on-demand быстрый сбор Features из текущего visible-list тел. Кнопка Create feature browser folder
        // сама строит feature-list, если он пустой, и создаёт BrowserFolder без полного RefreshIptBrowserTree после создания.
        // Apply edited name / XYZ to Inventor по-прежнему не трогаем.
        // Контрольные события: FEATURE_LIST_REANIMATE_FROM_VISIBLE_BODIES, FEATURE_BROWSER_FOLDER_FAST_CREATE_REQUEST,
        // FEATURE_BROWSER_FOLDER_FAST_CREATED_NO_TREE_REFRESH.
        // ==============================================================================================================================

        // 19:35 16.06.2026 InventorIptOrg_v0_4_32_FEATURE_BROWSER_NODES_FAST ===============================================
        // ИСПРАВЛЕНИЕ ПОСЛЕ ТЕСТА v0.4.31:
        // v0.4.31 строил feature-list только on-demand при нажатии Create feature browser folder, поэтому область Features
        // визуально оставалась пустой после .ipt cubes выбора и не закрывала пункт 2.
        // v0.4.32 строит Features/browser-nodes сразу после заполнения current visible-list.
        // Если Inventor не отдаёт CreatedByFeature / Face.CreatedByFeature для импортированных SurfaceBody, используется fallback:
        // BrowserPane.GetBrowserNodeFromObject(SurfaceBody) -> FeatureListItem с BrowserNode.
        // CreateBrowserFolderFromObjects теперь умеет принимать уже готовые BrowserNode напрямую, не пытаясь искать BrowserNode повторно.
        // Полный RefreshIptBrowserTree после создания feature folder по-прежнему пропускается ради скорости.
        // Контрольные события: FEATURE_LIST_AUTO_POPULATE_AFTER_CUBE_SELECTION, FEATURE_LIST_BODY_BROWSER_NODE_FALLBACK,
        // FEATURE_BROWSER_FOLDER_DIRECT_BROWSER_NODE_USED.
        // ==============================================================================================================================

        // 00:05 17.06.2026 InventorIptOrg_v0_4_39_BROWSER_SUBFOLDERS
        // Добавлена кнопка создания подпапки из уже выбранных BrowserNode.

        // 01:40 17.06.2026 InventorIptOrg_v0_4_42_FEATURE_FOLDER_NESTED
        // Упрощение workflow по просьбе пользователя:
        // обычная кнопка Create feature browser folder теперь при первом создании делает сразу две операции:
        // 1) создаёт внешнюю папку Selected_Features_yyyyMMdd_HHmmss;
        // 2) сразу внутри неё создаёт внутреннюю папку Selected_Features_Items_yyyyMMdd_HHmmss,
        //    и уже в неё помещает BrowserNode элементов/features.
        // Кнопка #2 остаётся копией той же кнопки и вызывает тот же обработчик.

        // 16:55 17.06.2026 InventorIptOrg_v0_4_47_LEGACY_BROWSER_TREE_AREA
        // Пользователь попросил НЕ заменять текущую редактируемую область Browser tree grid.
        // Нужно взять старую область TreeView из проекта:
        // MyFirstInventorPlugin_VS2017_Lesson5_CSharp_Tabs_IPT_IAM_BROWSER_TREE_EXPORT_FIX_IO_NAMES
        // и внедрить её рядом как отдельную legacy-область.
        // v0.4.47 добавляет отдельный TreeView: Legacy browser tree / старое дерево браузера Inventor,
        // со своими кнопками Refresh / Copy / Save JSON. Текущая DataGridView-область остаётся без замены.

        // 19:40 17.06.2026 InventorIptOrg_v0_4_48_REFRESH_BUTTON_TIMING
        // UI-FIX: кнопки Refresh в красной рамке теперь после каждого запуска показывают прямо в своём тексте
        // время последней отработки в секундах: last: X.XXX s.
        // Пока операция идёт, кнопка показывает working..., после завершения — постоянное последнее время.

        // 23:05 17.06.2026 InventorIptOrg_v0_4_51_SECOND_REFRESH_SPATIAL_BASE
        // ВАЖНО: база для этой версии — v0.4.48_REFRESH_BUTTON_TIMING.
        // v0.4.49 и v0.4.50 НЕ использовались как исходник.
        // Оригинальные кнопки и обработчики оставлены:
        // - ButtonIptRefreshBrowserTree_Click
        // - ButtonIptLegacyRefreshBrowserTree_Click
        // Рядом добавлены вторые кнопки #2, на которые вынесен новый подход через Fast build spatial cubes BASE.

        private bool RebuildSpatialCubesBaseForSecondRefresh(PartDocument partDoc, string source)
        {
            using (AppLogger.Scope("RebuildSpatialCubesBaseForSecondRefresh"))
            {
            if (partDoc == null)
            {
                return false;
            }

            try
            {
                int divisionsPerAxis = 2;
                double tolerance = 0.001;

                _spatialCubesIndex = SpatialCubesIndex.Build(
                    partDoc,
                    divisionsPerAxis,
                    tolerance,
                    GetComIdentityKey,
                    null);

                RefreshSpatialCubesTree();

                AppLogger.Log(
                    "SPATIAL_CUBES_FAST_BASE_REBUILT_FOR_CACHE_VIEW",
                    "RebuildSpatialCubesBaseForSecondRefresh",
                    "Source=" + (source ?? string.Empty) +
                    "; Bodies=" + (_spatialCubesIndex == null ? "0" : _spatialCubesIndex.Bodies.Count.ToString()) +
                    "; Cells=" + (_spatialCubesIndex == null ? "0" : _spatialCubesIndex.Cells.Count.ToString()) +
                    "; DivisionsPerAxis=" + divisionsPerAxis.ToString() +
                    "; BrowserPaneTraversal=False");

                return _spatialCubesIndex != null && _spatialCubesIndex.IsReady;
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem rebuilding spatial cubes BASE for #2 cache view");
                LoggedMessageBox.Show(ex.ToString());
                return false;
            }

            }}

        // 23:25 17.06.2026 InventorIptOrg_v0_4_52_SECOND_REFRESH_FULL_COUNT
        // FIX эксперимента #2:
        // v0.4.51 кнопки #2 строили только body-tree из spatial BASE: 72 тела + 2 служебные строки = 74.
        // Для чистого эксперимента нужно столько же строк, сколько у оригинального Refresh: 269 BrowserNode.
        // v0.4.52 #2 теперь:
        // 1) требует готовую Fast build spatial cubes BASE;
        // 2) делает полный обход BrowserPane только для формы дерева и имён;
        // 3) НЕ вытаскивает NativeObject/ObjectKind/XYZ для каждого узла;
        // 4) поэтому даёт такой же NodeCount/Rows как оригинал, но с облегчённой нагрузкой.

        // 23:45 17.06.2026 InventorIptOrg_v0_4_53_ONLINE_NAMES_CACHE
        // Замер нового режима:
        // v0.4.51 body-only BASE давал сладкие времена ~1.303 s / 0.815 s, но только 74 строки.
        // v0.4.52 full-count names-only дал честные 269 строк, но первый проход всё ещё около 7-8 s,
        // потому что остаётся BrowserPane traversal + GetBrowserNodeDisplayName на 269 узлов.
        // v0.4.53 добавляет онлайн-кэш names-only snapshot:
        // 1) первый #2 проход строит 269-узловый snapshot и кэширует его;
        // 2) последующие #2 обновления берут grid/tree из памяти без BrowserPane traversal;
        // 3) после изменений дерева cache инвалидируется.

        // 23:55 17.06.2026 InventorIptOrg_v0_4_54_PERF_COMMENTS_FROM_V52_LOG
        // 01:20 18.06.2026 InventorIptOrg_v0_4_56_DATAGRID_INPLACE_REFRESH
        // FIX производительности #2 BASE PreserveEditXyz:
        // быстрый #2 больше не должен пересоздавать DataGridView через Rows.Clear()+Rows.Add(),
        // если количество строк совпадает с текущей таблицей.
        // Теперь #2 BASE делает in-place refresh: обновляет Level/Type/Name/row.Tag/ReadOnly,
        // но не очищает X/Y/Z ячейки. Поэтому editable X/Y/Z сохраняются и не тратится
        // тяжёлое PopulateIptBrowserTreeGridFromItems на 269 строк.
        // Добавлены инженерные inline-комментарии из реального лога:
        // inventor_ipt_organizer_v0_4_52_20260617_233155_491.log.
        // Формат комментариев сохранён как в старых INLINE_PERF_COMMENTS:
        // полный цикл кнопки -> внутренние методы -> инженерный вывод.
        // ВАЖНО: copy-кнопки в этом конкретном логе не запускались, поэтому для них добавлен честный блок "замера нет".

        private SpatialCubesIndex _spatialCubesIndex;
        private TreeView treeViewIptSpatialCubes;
        private Label labelIptSpatialCubes;
        private Button ButtonIptBuildSpatialCubes;
        private Button ButtonIptPreviewSpatialCube;
        private Button ButtonIptClearSpatialCubePreview;
        private Button ButtonIptCustomRectSelectCubes;
        private Button ButtonIptToggleBodyRangeBoxes;
        private Label labelIptLegacyBrowserTree;
        private TreeView treeViewIptLegacyBrowserTree;
        private Button ButtonIptLegacyRefreshBrowserTree;
        private Button ButtonIptLegacyCopyBrowserTree;
        private Button ButtonIptLegacySaveBrowserTree;
        private Button ButtonIptLegacyRefreshBrowserTreeSpatialBase;
        private BrowserTreeNamesOnlySnapshot _browserTreeNamesOnlySnapshot;
        private ComboBox comboBoxIptCustomRectPlane;
        private ComboBox comboBoxIptCustomRectBodyMode;
        private const string SpatialCubePreviewGraphicsName = "InventorIptOrg_SpatialCubePreview";
        private const string CustomRectTraceGraphicsName = "InventorIptOrg_CustomRectTraceBox";
        private const string TouchedSpatialCubesPreviewGraphicsName = "InventorIptOrg_TouchedSpatialCubesPreview";
        private const string BodyRangeBoxesGraphicsName = "InventorIptOrg_BodyRangeBoxesPreview";

        // v0.4.64 LAYER_WINDOWS_CANVAS — отдельное полотно диагностики слоёв.
        private TabPage tabPageIptLayerCanvas;
        private ToolTip toolTipIptLayerCanvas;
        private Panel panelIptLayerCanvasRoot;
        private Panel panelIptLayerPreview;
        private Panel panelIptLayerPreviewDrawing;
        private Label labelIptLayerPreviewTitle;
        private Label labelIptLayerPreviewStatus;
        private FlowLayoutPanel flowLayoutPanelIptLayerButtons;
        private Label labelIptLayerTreeTitle;
        private TreeView treeViewIptLayerCanvasTree;
        private Label labelIptLayerCanvasHint;
        private Button ButtonIptOpenBrowserTruthWindow;
        private Button ButtonIptOpenSpatialCacheWindow;
        private Button ButtonIptOpenMergedCacheWindow;
        private Button ButtonIptOpenFastCacheWindow;
        private Button ButtonIptOpenLegacyCacheWindow;
        private Button ButtonIptOpenModelPreviewWindow;
        private Button ButtonIptCanvasFullRefresh;
        private Button ButtonIptCanvasBuildCubes;
        private Button ButtonIptCanvasRefreshLegacyTree;
        private ComboBox comboBoxIptViewerSource;
        private ComboBox comboBoxIptViewerRenderMode;
        private ComboBox comboBoxIptViewerFpsMode;
        private Button ButtonIptViewerOptions;
        private ContextMenuStrip contextMenuIptViewerOptions;
        private bool _viewerShowPointsOverlay;
        private bool _viewerShowBoxesOverlay;
        private bool _viewerDraftWhileDragging = true;
        private Button ButtonIptCanvasBuildMeshPreview;
        private Button ButtonIptCanvasOpenStepFile;
        private StepLiteScene _stepLitePreviewScene;
        private bool _stepLitePreviewEnabled;
        private string _stepLiteLastOpenStatus;
        // 17:45 19.06.2026 InventorIptOrg_v0_5_03_MESH_VISIBILITY_THRESHOLD
        // User requested historical method versions at the end of every Mode item, not the current app version.
        // The selected profile now logs AppVersion and MethodVersion separately.
        // Mesh Inventor Lab v0.5.03 adds front-depth neighborhood sampling plus a visible-sample ratio gate
        // so isolated depth-pass pixels cannot reveal mostly hidden/internal POLY_LOOP edges.
        // Silhouette and feature edges keep their independent depth tolerances and are not ratio-filtered.
        // New counters: AcceptedMeshEdgesByRatio, RejectedMeshEdgesByRatio and HiddenByFrontDepth.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_02_RENDER_METHOD_LAB_LOGGING — user Rubber Hose log:
        // Mesh Inventor Lab 0.130..0.154 s; surface pass 0.023..0.035 s; 2649 visible combined edges.
        // 01:55 20.06.2026 InventorIptOrg_v0_5_13_CURSOR_SAMPLED_DRAG_LOOP
        // ГЛУБОКИЙ РАЗБОР реального лога v0.5.12: reduced render уже быстрый, но FPS overlay смешивал две разные величины.
        // В первой сессии AverageFrameMs=26.629 ms (теоретическая render capacity ~37.6 FPS), но AverageFps=13.25.
        // Во второй сессии AverageFrameMs=31.689 ms (render capacity ~31.6 FPS), но AverageFps=9.58.
        // Причина: дорогой reduced render всё ещё выполнялся внутри WM_PAINT, а новая камера применялась только из MouseMove.
        // Пока UI thread рисовал 25..32 ms, Windows объединял mouse messages; после пауз completion-interval попадал в FPS EMA,
        // поэтому overlay показывал 8..16 FPS даже при кадре 24..31 ms и не отличал render throughput от presentation cadence.
        // v0.5.13 делает WM_PAINT presentation-only во время drag, а Timer с коротким интервалом сам читает Cursor.Position,
        // применяет накопившийся физический delta, синхронно строит reduced bitmap вне WM_PAINT, атомарно меняет backbuffer
        // и только после готового кадра вызывает Invalidate. Это убирает зависимость камеры от частоты MouseMove/paint queue.
        // Overlay и лог теперь отдельно показывают PresentFps и RenderCapacityFps=1000/EmaFrameMs.
        // ЗАМЕР НОВОГО РЕЖИМА: v0.5.12 ADAPTIVE_DRAG_DIRECT_SWAP -> v0.5.13 CURSOR_SAMPLED_DRAG_LOOP
        // InventorIptOrg_v0_5_12_ADAPTIVE_DRAG_DIRECT_SWAP — пользовательский Rubber Hose лог:
        // full=0.096..0.140 s; drag FrameMs=23.883..32.353 ms; FinalAdaptiveScale=0.57;
        // summary #1 AverageFrameMs=26.629, AverageFps=13.25; summary #2 AverageFrameMs=31.689, AverageFps=9.58.
        // InventorIptOrg_v0_5_13_CURSOR_SAMPLED_DRAG_LOOP — Visual Studio/Inventor timing pending.
        // 01:10 20.06.2026 InventorIptOrg_v0_5_12_ADAPTIVE_DRAG_DIRECT_SWAP
        // ГЛУБОКИЙ РАЗБОР реального лога v0.5.11: центрирование исправлено, но drag оставался CPU-bound.
        // При RenderScale=0.55 фактический EMA кадра был 74.509 ms, AverageFps=12.46, а full frame 0.187..0.306 s.
        // Причина: каждый drag-кадр после reduced render ещё создавал полный bitmap 920x641 и делал bilinear upscale,
        // затем Z-buffer выполнял две полные диагностические проходки по пикселям (nearest-facing и opaque/depth-cue),
        // хотя high-frequency логи были отключены, DepthCueStrength=0 и surface-owner matching=False.
        // v0.5.12 сохраняет reduced bitmap напрямую в atomic backbuffer и масштабирует только при presentation,
        // пропускает ненужные pixel diagnostics в reduced drag и адаптирует scale 0.32..0.58 к TargetFrameMs=40.
        // Overlay теперь рисуется в реальном visible viewport, поэтому его размер не зависит от reduced bitmap.
        // ЗАМЕР НОВОГО РЕЖИМА: v0.5.11 VISIBLE_VIEWPORT_CENTER_FIX -> v0.5.12 ADAPTIVE_DRAG_DIRECT_SWAP
        // InventorIptOrg_v0_5_11_VISIBLE_VIEWPORT_CENTER_FIX — пользовательский Rubber Hose лог:
        // initial full=0.091 s; drag frame 15=79.612 ms; frame 30=75.752 ms; AverageFps=12.46;
        // AverageFrameMs=72.643; FinalQualityRenderMs=193.088; RenderScale=0.55; CoalescedEvents=14.
        // InventorIptOrg_v0_5_12_ADAPTIVE_DRAG_DIRECT_SWAP — Visual Studio/Inventor timing pending.
        // 00:25 20.06.2026 InventorIptOrg_v0_5_11_VISIBLE_VIEWPORT_CENTER_FIX
        // FIX по реальному логу v0.5.10: panel ClientSize=1976x1167, но реальная видимая Paint-область была только 920x641.
        // Renderer центрировал модель в полном, частично скрытом ClientSize, поэтому в видимой части оставался только правый край модели.
        // v0.5.11 вычисляет реальный видимый client-viewport пересечением panel client rect со всеми parent client rect,
        // рендерит backbuffer именно в этот viewport и презентует bitmap в его локальные координаты.
        // Это возвращает принцип v0.4.72 VisibleClipFit, но без зависимости от случайно частичного ClipRectangle.
        // InventorIptOrg_v0_5_10_DRAG_DPI_CENTER_FIX — user log: ClientPixels=1976x1167; Clip=920x641; model clipped at right.
        // InventorIptOrg_v0_5_11_VISIBLE_VIEWPORT_CENTER_FIX — Visual Studio/Inventor timing pending.
        // ЗАМЕР НОВОГО РЕЖИМА: v0.5.10 DRAG_DPI_CENTER_FIX -> v0.5.11 VISIBLE_VIEWPORT_CENTER_FIX
        // Источник v0.5.10: inventor_ipt_organizer_v0_5_10_20260619_235221_024.log.
        // v0.5.10 full frame: STEP_LOCAL_DRAW_SECONDS=0.268 s; surface pass=0.207 s.
        // v0.5.10 drag first frame: FrameMs=244.369; TargetFps=30; actual first-frame FPS about 4.1.
        // Ключевая геометрия окна v0.5.10: Raw ClientSize=1976x1167, real Paint Clip=920x641.
        // v0.5.11 renders at EffectiveViewport and no longer spends pixels on the hidden 1056x526 tail.
        // Ожидаемое ускорение зависит от окна; реальный Visual Studio/Inventor замер v0.5.11 pending.
        // 00:35 20.06.2026 InventorIptOrg_v0_5_10_DRAG_DPI_CENTER_FIX
        // User test of v0.5.09 proved that changing the orbit/pivot math alone did not move the visible model:
        // the completed backbuffer was still presented with DrawImageUnscaled, while the WinForms paint surface
        // could be DPI-scaled. That allowed bitmap pixels and control coordinates to use different effective scales,
        // so the correctly computed viewport center was displayed near the lower-right and the model was clipped.
        // v0.5.10 presents every intermediate/final bitmap with an explicit destination rectangle and Pixel units,
        // carries the paint-surface DPI into created bitmaps, and restores projected visual-bounds centering while
        // keeping the 3D orbit pivot fixed at the model Bounds center. Final and 55% drag frames use the same mapping.
        // New diagnostics: VIEWER_BACKBUFFER_MAPPING and STEP_VIEW_CAMERA_CENTER with pixel mapping and correction.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_09_DRAG_CAMERA_CENTER_LOCK — user screenshot: model remained clipped at lower-right;
        // log reported ScreenCenter=988,595.5 and Pan=0,0, proving the projection math and visible presentation disagreed.
        // InventorIptOrg_v0_5_10_DRAG_DPI_CENTER_FIX — Visual Studio/Inventor timing pending.
        // 23:20 19.06.2026 InventorIptOrg_v0_5_09_DRAG_CAMERA_CENTER_LOCK
        // User test of v0.5.08 confirmed that flicker/backbuffer/FPS were nearly correct, but during LMB orbit
        // the model moved toward the mouse-side of the viewport instead of rotating around the visual center.
        // Root cause: PrepareStepLiteProjectionFit centered the current projected 2D bounding rectangle
        // (`rawCenterX/rawCenterY`) on every frame. For an asymmetric bent hose that projected-box center changes
        // with yaw/pitch, so the camera/orbit target drifted although the 3D model center itself was unchanged.
        // v0.5.09 uses a symmetric fit around the fixed 3D bounds center: extents are max(abs(min),abs(max)),
        // the model origin is mapped to the viewport center, mouse-down position never becomes an orbit pivot,
        // and reduced 55% drag frames use the same center after upscaling. Shift+LMB/RMB pan remains explicit.
        // New diagnostics: STEP_VIEW_CAMERA_CENTER with FitMode=SymmetricModelCenter, OrbitTarget and ScreenCenter.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_08_DRAG_BACKBUFFER_FPS — user screenshot: atomic shaded drag works,
        // but projected-bbox recentering moves the model far to the right during LMB orbit.
        // InventorIptOrg_v0_5_09_DRAG_CAMERA_CENTER_LOCK — Visual Studio/Inventor timing pending.
        // 22:30 19.06.2026 InventorIptOrg_v0_5_08_DRAG_BACKBUFFER_FPS
        // Root cause from the user's v0.5.07 drag log: every MouseMove synchronously invalidated the panel,
        // Panel.Paint cleared the visible surface, and FastDragWireframe rebuilt 4608 triangle edges on the UI thread.
        // Actual drag frames took 0.033..0.080 s (roughly 12.5..30 FPS) and the full quality True Front frame
        // took 0.161..0.198 s. The apparent blink was therefore a presentation/scheduling problem, not Z-buffer.
        // v0.5.08 adds an explicit 32-bpp backbuffer, OptimizedDoubleBuffer/AllPaintingInWmPaint/UserPaint,
        // 30 FPS mouse-move coalescing, reduced-resolution current-orientation shaded drag rendering,
        // atomic completed-frame swap, FPS/frame-time/dropped-input overlay and drag-session summary logging.
        // The stable TrueFrontFaceZBufferV0506 algorithm remains unchanged for final quality frames.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_07_TRUE_FRONT_FACE_COMPILE_FIX — user drag log: FastDragWireframe 0.033..0.080 s,
        // final Mesh True Front 0.161..0.198 s; every MouseMove generated a synchronous full wireframe repaint.
        // InventorIptOrg_v0_5_08_DRAG_BACKBUFFER_FPS — Visual Studio/Inventor timing pending.
        // 19:40 19.06.2026 InventorIptOrg_v0_5_07_TRUE_FRONT_FACE_COMPILE_FIX
        // Compile fix after the user's Visual Studio build of v0.5.06:
        // CS0136 at ComputeStepNormalShade line 4203 was caused by redeclaring local `brightness`
        // inside the TrueFrontFaceZBufferV0506 branch while the method already declared `double brightness;`.
        // The branch now assigns to the existing local, preserving the v0.5.06 renderer unchanged.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_06_TRUE_FRONT_FACE_ZBUFFER — Visual Studio: 3 projects succeeded,
        // InventorIptOrg failed with CS0136; Inventor runtime test could not start.
        // InventorIptOrg_v0_5_07_TRUE_FRONT_FACE_COMPILE_FIX — Visual Studio/Inventor timing pending.
        // 21:40 19.06.2026 InventorIptOrg_v0_5_06_TRUE_FRONT_FACE_ZBUFFER
        // DEEP ROOT-CAUSE RESULT from the user's v0.5.05 Rubber Hose run:
        // the pixels were alpha 255, but the visible-shell culling convention contradicted the Z-buffer.
        // ProjectPrimitivePointView uses greater view-Z as nearer (DepthRule=GreaterViewZIsNear), therefore
        // a camera-facing outward triangle has NormalZ > 0. Legacy code marked NormalZ > 0 as BackFacing
        // and culled the actual nearest shell. The rasterizer then filled the same screen silhouette with
        // the farther opposite/inner surface, which looked exactly like transparency despite alpha 255.
        // Offline reproduction on the supplied STEP at the default camera found 135012 covered pixels;
        // at all 135012 pixels the true nearest triangle had NormalZ > 0 and would have been legacy-culled.
        // v0.5.06 keeps old methods historically unchanged and adds a corrected method: positive view-Z
        // is front-facing, all triangles participate in the depth pass, nearest depth wins, and mesh edges
        // are tested against the corrected nearest surface without the destructive 3x3 max-depth sample.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_05_SOLID_FRONT_MESH_FIX — user log: 0.150 s, 1188 mesh edges,
        // OpaqueSurfacePixels=135012, NonOpaqueSurfacePixels=0, but legacy culling selected the far shell.
        // InventorIptOrg_v0_5_06_TRUE_FRONT_FACE_ZBUFFER — actual Visual Studio/Inventor timing pending.
        // 20:10 19.06.2026 InventorIptOrg_v0_5_05_SOLID_FRONT_MESH_FIX
        // Deep root-cause fix after the user's v0.5.04 Rubber Hose run:
        // 1) the surface was already truly opaque (SurfaceAlpha=255, SurfaceOpaque=True), so the
        //    perceived transparency was not alpha blending; it came from weak depth separation and
        //    a few surviving internal mesh fragments;
        // 2) v0.5.04 surface-owner gating was too strict for duplicated STEP POLY_LOOP vertices and
        //    removed the exterior grid: only 152 mesh edges survived versus 1188 in v0.5.03;
        // 3) the requested Inventor-style shading must use an explicit method-id branch rather than
        //    silently falling through to the older single-key equation.
        // v0.5.05 restores the proven v0.5.03 front-depth ratio mesh, disables hard owner gating,
        // adds an explicit solid-front key/fill/rim equation and a small opaque depth-cue pass.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_03_MESH_VISIBILITY_THRESHOLD — 1188 visible mesh edges, 0.117..0.155 s.
        // InventorIptOrg_v0_5_04_OPAQUE_BODY_MESH — 152 visible mesh edges, 0.138..0.150 s;
        // 2920 edges rejected, 1416 edges without a front surface owner.
        // InventorIptOrg_v0_5_05_SOLID_FRONT_MESH_FIX — actual Visual Studio/Inventor timing pending.
        // 19:20 19.06.2026 InventorIptOrg_v0_5_04_OPAQUE_BODY_MESH
        // v0.5.04 keeps the v0.5.03 ratio gate and adds surface-owner matching for ordinary POLY_LOOP mesh.
        // Each rasterized surface pixel stores the front triangle owner. A mesh sample is accepted only when
        // the front Z-buffer owner belongs to a triangle adjacent to that geometric mesh edge. This prevents
        // a rear/internal edge from leaking through an unrelated opaque front shell even when depths are close.
        // SurfaceAlpha remains explicitly 255; Ghost is still the only translucent surface mode.

        // InventorIptOrg_v0_5_03_MESH_VISIBILITY_THRESHOLD — user Rubber Hose log:
        // Mesh Inventor Lab 0.117..0.155 s; surface pass 0.033..0.051 s;
        // 1188 accepted mesh edges, 1884 rejected by ratio, 42956 samples hidden by front depth.
        // InventorIptOrg_v0_5_04_OPAQUE_BODY_MESH — same software Z-buffer plus surface-owner matching;
        // actual Visual Studio/Inventor timing is pending the user's run and is not claimed by this package.
        // 20:05 19.06.2026 InventorIptOrg_v0_5_02_RENDER_METHOD_LAB_LOGGING
        // User confirmed that v0.5.01 is still not visually final and requested new rendering methods
        // directly in the existing Mode combo so every switch and every resulting render is captured in the log.
        // New Surface+Mesh laboratories preserve the same STEP topology and software Z-buffer:
        // - Mesh Deep Clamp: removes the accidental v0.5.01 pre-clamp 145..214 and really uses 115..218;
        // - Mesh Two-Light: key + fill directional lighting;
        // - Mesh Hemisphere: hemisphere ambient + key light;
        // - Mesh Inventor Lab: stronger CAD key/fill/facing/rim profile with cleaner mesh depth bias.
        // VIEWER_RENDER_METHOD_SELECTED logs every method selection with all clamps, RGB values and depth factors.
        // STEP_ZBUFFER_* and STEP_LOCAL_DRAW_SECONDS repeat RenderMethod and profile parameters after rendering.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_01_DEEPER_SHADE_SPLIT_EDGE_BIAS — user Rubber Hose log:
        // Surface+Mesh 0.099 s and 0.111 s; surface pass 0.024..0.028 s; 2661 visible mesh edges.
        // InventorIptOrg_v0_5_02_RENDER_METHOD_LAB_LOGGING — same Z-buffer architecture;
        // actual Visual Studio/Inventor timing is pending the user's method-by-method run.
        // 19:05 19.06.2026 InventorIptOrg_v0_5_01_DEEPER_SHADE_SPLIT_EDGE_BIAS
        // Visual refinement after the confirmed v0.5.00 Rubber Hose run:
        // - deepen the neutral-gray surface range so the inner bore and lower bend read as real shadowed form;
        // - preserve the v0.5.00 Inventor-like blue-gray background and smooth Gouraud normals;
        // - make regular POLY_LOOP mesh lines slightly lighter so they stop dominating the shaded surface;
        // - keep the silhouette dark and give mesh, feature and silhouette edges separate Z-depth tolerances;
        // - reduce ordinary mesh tolerance to suppress crowded/over-permissive lines around the throat;
        // - preserve software Z-buffer, hidden-line removal, Surface+Mesh, no fan diagonals and FastDragWireframe.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_00_INVENTOR_STYLE_SHADED_MESH — user Rubber Hose log:
        // Surface+Mesh 0.100 s and 0.110 s; Z-buffer surface pass 0.025..0.031 s; 2667 visible mesh edges.
        // InventorIptOrg_v0_5_01_DEEPER_SHADE_SPLIT_EDGE_BIAS — same render architecture;
        // actual Visual Studio/Inventor timing is pending the user's run and is not claimed by this package.
        // 18:10 19.06.2026 InventorIptOrg_v0_5_00_INVENTOR_STYLE_SHADED_MESH
        // Visual calibration after the confirmed v0.4.99 Rubber Hose test:
        // - v0.4.99 geometry, Gouraud interpolation and hidden-line removal were correct and fast (~0.095 s),
        //   but the surface was too close to white and the visible POLY_LOOP mesh was too light;
        // - the Z-buffer surface now uses an Inventor-like neutral-gray contrast range;
        // - the directional/hemisphere light has stronger form separation without returning flat triangle bands;
        // - visible mesh, feature and silhouette pixels are darker for a Shaded-with-Edges CAD look;
        // - the canvas background uses a stronger blue-gray vertical gradient;
        // - software Z-buffer, Surface+Mesh, POLY_LOOP-only mesh, Gouraud normals and FastDragWireframe are preserved.
        // 16:35 19.06.2026 InventorIptOrg_v0_4_99_ZBUFFER_SMOOTH_SHADE_MESH
        // Improvement after v0.4.98:
        // v0.4.98 made the visible POLY_LOOP mesh correct and fast, but every triangle still had one flat shade.
        // v0.4.99 adds crease-aware Gouraud shading:
        // - face normals are accumulated by quantized 3D vertex coordinates;
        // - only neighbor normals with dot >= 0.72 are blended, preserving sharp edges;
        // - three vertex shades are barycentrically interpolated per pixel in the existing Z-buffer;
        // - visible mesh lines are lighter;
        // - silhouette overlay is one pixel and is drawn after mesh lines;
        // - edge depth tolerance is slightly increased to reduce dotted breaks.
        // 16:05 19.06.2026 InventorIptOrg_v0_4_98_ZBUFFER_VISIBLE_MESH
        // Improvement after v0.4.97:
        // Surface+Edges intentionally draws only silhouette/boundary/crease edges.
        // For smooth hose geometry that leaves only 126 visible feature edges and hides the useful surface grid.
        // v0.4.98 adds Mode=Surface+Mesh:
        // - uses original StepLiteScene.Edges from POLY_LOOP boundaries, not fan-triangle diagonals;
        // - every mesh edge passes through the existing software Z-buffer depth test;
        // - hidden mesh segments stay hidden;
        // - visible POLY_LOOP grid is drawn as a thin one-pixel CAD mesh;
        // - Surface+Edges, Surface, Wire, Ghost and Points remain available.
        // 15:45 19.06.2026 InventorIptOrg_v0_4_97_ADVANCED_FACE_ORIENTATION
        // STEP topology fix after v0.4.96:
        // Parse chain: ADVANCED_FACE -> FACE_OUTER_BOUND/FACE_BOUND -> POLY_LOOP.
        // Effective loop winding uses both FACE_BOUND.orientation and ADVANCED_FACE.same_sense.
        // If flags differ, the POLY_LOOP order is reversed before fan triangulation.
        // Z-buffer renderer and hidden-line removal remain unchanged.
        // 12:20 19.06.2026 InventorIptOrg_v0_4_96_ZBUFFER_HIDDEN_LINE
        // Новый renderer после v0.4.95:
        // v0.4.95 правильно нашёл silhouette/boundary/crease edges, но edge pass рисовал линии
        // поверх всей модели без проверки глубины, поэтому внутренние рёбра просвечивали как X-Ray.
        // v0.4.96:
        // - rasterizes visible shell triangles в software Z-buffer;
        // - хранит ближайшую view-depth для каждого пикселя;
        // - feature/silhouette edges рисуются только если проходят depth-test;
        // - Surface/Surface+Edges больше не используют Painter-only hidden line;
        // - Ghost и FastDrag сохраняют прежние облегчённые режимы.
        // 11:40 19.06.2026 InventorIptOrg_v0_4_95_SOFT_SHADE_SILHOUETTE
        // Улучшение после v0.4.94:
        // v0.4.94 построил 22232 BoundaryEdges и 0 CreaseEdges, потому что adjacency считалась
        // только по уже culled front-facing triangles и по vertex index.
        // v0.4.95:
        // - строит edge adjacency по ВСЕМ triangles до back-face culling;
        // - объединяет совпадающие edges по quantized 3D coordinates, а не только по index;
        // - отдельно рисует silhouette / crease / true boundary paths;
        // - добавляет мягкий diffuse shade и лёгкую projected contact shadow;
        // - сохраняет FastDragWireframe без тяжёлого edge/shadow прохода.
        // 00:30 19.06.2026 InventorIptOrg_v0_4_94_SURFACE_FEATURE_EDGES
        // Улучшение после v0.4.93:
        // v0.4.93 сделал opaque visible shell, но CAD-форма всё ещё слабо читается без рёбер.
        // v0.4.94 добавляет feature/boundary edges поверх visible shell:
        // - Surface+Edges теперь default;
        // - edges строятся не как вся triangle-сетка, а как boundary + crease edges;
        // - triangle normals сохраняются в StepSurfaceDrawTriangle;
        // - лог пишет STEP_FEATURE_EDGES_BUILT / DrawnFeatureEdges / BoundaryEdges / CreaseEdges.
        // 00:20 19.06.2026 InventorIptOrg_v0_4_93_SURFACE_VISIBLE_SHELL
        // Улучшение после v0.4.92:
        // v0.4.92 сделал depth-sort + shading, но Surface выглядел как полупрозрачный блок,
        // потому что рисовались и front-facing, и back-facing/внутренние triangles.
        // v0.4.93 делает Surface непрозрачным visible shell:
        // - Surface/Surface+Edges отбрасывают back-facing triangles;
        // - Ghost оставляет все triangles и остаётся прозрачным;
        // - Surface alpha = 255;
        // - лог пишет CulledBackFacingTriangles / DrawnFrontFacingTriangles / SurfaceOpaque=True.
        // 00:08 19.06.2026 InventorIptOrg_v0_4_92_SURFACE_DEPTH_SHADE
        // Улучшение после v0.4.91:
        // v0.4.91 впервые честно показал Surface через прямой FillPolygon, но картинка стала серым монолитом.
        // v0.4.92 добавляет Painter depth sort + pseudo shading по view-space normal/light.
        // FastDrag остаётся wireframe; full Surface получает сортировку дальние->ближние и оттенки серого.
        // 23:58 18.06.2026 InventorIptOrg_v0_4_91_SURFACE_SOLID_POLYGON_FILL
        // Fix после v0.4.90:
        // SurfaceFillMode=Winding логировался, DrawnTriangles=23696, но визуально снова был только bounding параллелепипед.
        // Значит общий GraphicsPath fill для STEP fan triangles всё равно не даёт видимой поверхности.
        // v0.4.91 добавляет прямой solid fill: Surface рисует каждый triangle через FillPolygon.
        // Это медленнее GraphicsPath, но должно честно показать поверхность. FastDrag остаётся быстрым wireframe.
        // 23:49 18.06.2026 InventorIptOrg_v0_4_90_SURFACE_FILL_WINDING_FIX
        // Fix после v0.4.89:
        // Mode=Surface был почти пустой/невидимый, хотя Wire показывал геометрию.
        // Причина наиболее вероятно в GDI+ GraphicsPath default FillMode.Alternate:
        // при большом наборе STEP fan triangles контуры могли взаимно гаситься odd-even заливкой.
        // Теперь Surface/Suface+Edges/Ghost используют FillMode.Winding и более плотную Surface-заливку.
        // Лог пишет SurfaceFillMode=Winding.
        // 23:38 18.06.2026 InventorIptOrg_v0_4_89_DEFAULT_CANVAS_TAB
        // Quality-of-life fix после v0.4.88:
        // .ipt canvas теперь выбирается вкладкой по умолчанию после InitializeLayerWindowsCanvasUi.
        // v0.4.88 оставлен как основа быстрого pleasant STEP viewer:
        // default Surface, простой FastDragWireframe, без v0.4.87 LOD/chunk overhead.
        // 23:30 18.06.2026 InventorIptOrg_v0_4_88_FAST_PLEASANT_VIEWER
        // Решение после экспериментов v0.4.85-v0.4.87:
        // chunks/LOD архитектурно интересны, но по ощущению скорости v0.4.87 стал хуже.
        // v0.4.88 возвращается к быстрой практичной базе v0.4.84:
        // - Mode по умолчанию = Surface, без ряби внутренних ребер;
        // - Surface+Edges остается отдельным режимом;
        // - drag использует простой FastDragWireframe без chunk/LOD overhead;
        // - FastDrag прорежен сильнее, чтобы вращение было легче;
        // - STEP render logs получают ViewerProfile=FastPleasant.
        // 22:36 18.06.2026 InventorIptOrg_v0_4_84_UNIFIED_VIEWER_MODES
        // NEW: единый viewer mode system без горы кнопок.
        // Добавлены compact controls: Source combo (Auto/STEP/Mesh/BoxGrid), Mode combo (Wire/Surface/Surface+Edges/Ghost/Points) и OPT popup.
        // Текущая поддержка сохранена: ST Step, MS Mesh, 8C Cubes, ActiveView preview и legacy actions.
        // RenderMode теперь управляет STEP viewer: можно убрать рябь каркаса через Surface/Surface+Edges/Ghost.
        // OPT содержит Show points, Show BoxGrid overlay и Draft while dragging.
        // 22:24 18.06.2026 InventorIptOrg_v0_4_83_STEP_FAST_GDI_PATHS
        // Fix по логу v0.4.82:
        // Local STEP parse/build = 0.901 с, но DrawStepLiteScenePreview = 27.101 с.
        // Причина: v0.4.82 делала десятки тысяч отдельных GDI+ вызовов FillPolygon/DrawPolygon.
        // Теперь triangles/edges собираются в GraphicsPath и рисуются батчем: FillPath + DrawPath.
        // Во время mouse drag используется FastDrag mode: прореженный wireframe без shaded fill,
        // чтобы даже малый поворот не заставлял ждать полный repaint.
        // 22:15 18.06.2026 InventorIptOrg_v0_4_82_STEP_POLYLOOP_MESH
        // Fix для STEP preview после v0.4.81:
        // v0.4.81 читал все CARTESIAN_POINT подряд и получил 58588 points / 0 edges,
        // поэтому на экране было "облако точек" и draw занимал 16+ секунд.
        // v0.4.82 читает POLY_LOOP и строит edges + triangles только из точек,
        // реально участвующих в гранях, затем рисует local shaded STEP mesh.
        // 22:00 18.06.2026 InventorIptOrg_v0_4_81_LOCAL_STEP_NO_MESSAGEBOX
        // Fix по логу пользователя v0.4.79:
        // StepLiteReader.Load занял 0.726 с, но OpenStepFileInLocalViewer висел 42+ секунды на USER_MESSAGE_SHOW -> USER_MESSAGE_RESULT.
        // Поэтому "зависание" было не чтением STEP, а модальным MessageBox с результатом.
        // Успешная загрузка STEP теперь не показывает MessageBox; статус пишется в левый статус/canvas и в лог.
        // Ошибки по-прежнему показываются через MessageBox.
        // 21:55 18.06.2026 InventorIptOrg_v0_4_80_NO_EXTERNAL_AUTOSTART
        // ВАЖНО: приложение больше не запускает внешний сеанс при старте.
        // До v0.4.79 конструктор делал:
        // Marshal.GetActiveObject(...) -> если не найдено -> runtime object creation(...) -> Visible=true.
        // Поэтому при запуске нашего окна автоматически поднимался внешний процесс.
        // Теперь старт только пробует подключиться к уже запущенному сеансу; если его нет, _invApp остаётся null.
        // Локальный ST Step / StepLiteCore может работать без внешнего сеанса.
        // 21:40 18.06.2026 InventorIptOrg_v0_4_79_LOCAL_STEP_VIEWER
        // IMPORTANT FIX: кнопка ST Step больше не открывает файл во внешнем сеансе через Documents.Open.
        // Теперь это локальный путь: OpenFileDialog -> StepLiteCore.StepLiteReader -> StepLiteScene -> отрисовка прямо в нашем .ipt canvas.
        // StepLiteCore читает CARTESIAN_POINT / VERTEX_POINT / EDGE_CURVE из STEP-текста.
        // Это ещё не full BRep tessellation, но уже не зависит от открытия STEP в стороннем окне.
        // 21:22 18.06.2026 InventorIptOrg_v0_4_78_STEP_OPEN_COMPILE_FIX
        // Compile fix после v0.4.77:
        // CS0104: File неоднозначен между Inventor.File и System.IO.File.
        // CS0104: Path неоднозначен между Inventor.Path и System.IO.Path.
        // Исправление: в STEP open button используем явные System.IO.File.Exists и System.IO.Path.GetExtension.
        // 21:25 18.06.2026 InventorIptOrg_v0_4_77_STEP_OPEN_BUTTON
        // NEW: добавлена кнопка ST Step на .ipt canvas.
        // Назначение: выбрать .stp/.step файл через OpenFileDialog и открыть его в активном сеансе через Documents.Open.
        // После открытия STEP пользователь может сразу нажимать 8C Cubes и MS Mesh для построения spatial BASE / mesh-preview.
        // Важно: STEP-файл не зашит в код; кнопка открывает любой выбранный пользователем файл.
        // 21:08 18.06.2026 InventorIptOrg_v0_4_76_MESH_FACETS_STRONG_CALL
        // Mesh fix по логу v0.4.75:
        // MESH_BODY_FACETS_FAILED для всех 72 тел: "ArgumentException: Не удалось преобразовать аргумент 2 для вызова на CalculateFacets".
        // Причина: dynamic вызов получал out-параметры как object и COM не мог привести аргумент VertexCount.
        // Исправление: основной путь теперь вызывает SurfaceBody.CalculateFacets строго типизированно:
        // int vertexCount, int facetCount, double[] vertexCoordinates, double[] normalVectors, int[] vertexIndices.
        // Fallback BoxMesh остаётся только если настоящий facets-вызов реально не вернул треугольники.
        // 20:58 18.06.2026 InventorIptOrg_v0_4_75_MESH_VIEW_CORE_COMPILE_FIX
        // Compile fix после v0.4.74: Form1.cs использует MeshScene/MeshBody/MeshTriangle,
        // но в опубликованном ZIP не было `using MeshViewCore;`.
        // Проектная ссылка на MeshViewCore.csproj уже была, поэтому исправление минимальное:
        // добавить namespace import без изменения mesh-preview логики.
        private MeshScene _meshPreviewScene;
        private bool _meshPreviewEnabled;
        private Button ButtonIptCanvasCaptureActiveView;
        private System.Drawing.Bitmap _layerCanvasPreviewBitmap;
        private string _layerCanvasPreviewMessage;
        private Button ButtonIptCanvasToggleLivePreview;
        private Button ButtonIptCanvasDockActiveView;
        private Timer _layerCanvasLivePreviewTimer;
        private bool _layerCanvasLivePreviewRunning;
        private bool _layerCanvasActiveViewDocked;
        private IntPtr _dockedActiveViewHwnd;
        private IntPtr _dockedActiveViewOriginalParent;
        private int _dockedActiveViewOriginalStyle;
        private bool _primitiveViewerDragging;
        private System.Drawing.Point _primitiveViewerLastMouse;
        private double _primitiveViewerYaw = -35.0;
        private double _primitiveViewerPitch = 22.0;
        private double _primitiveViewerZoom = 1.0;
        private bool _primitiveProjectionFitReady;
        private double _primitiveProjectionScale = 1.0;
        private double _primitiveProjectionOffsetX;
        private double _primitiveProjectionOffsetY;
        private double _primitiveProjectionRawMinX;
        private double _primitiveProjectionRawMaxX;
        private double _primitiveProjectionRawMinY;
        private double _primitiveProjectionRawMaxY;
        private double _primitiveViewerPanX;
        private double _primitiveViewerPanY;
        private bool _primitiveViewerPanning;

        // 17:55 20.06.2026 InventorIptOrg_v0_5_17_WARNING_FREE_CANVAS_BUILD
        // Исправлены три предупреждения CS0162 из подтверждённой пользовательской сборки v0.5.16.
        // CanvasOnlyGitHubMode заменён с compile-time const на runtime-readonly configuration flag.
        // Поведение canvas-only не изменено: .ipt canvas остаётся единственной видимой вкладкой,
        // Autodesk/Inventor workflow не инициализируется, проверенные Mode/FPS профили сохранены.

        // 20:30 20.06.2026 InventorIptOrg_v0_5_16_CANVAS_ONLY_GITHUB_PREP
        // GitHub presentation build: only .ipt canvas is visible; legacy .ipt/.iam/.ipt cubes tabs are hidden.
        // Visible toolbar exposes local STEP open, reset, options, render mode and FPS pipeline only.
        // Verified working profiles are marked with 🔥: Mesh True Front compile-fix v0.5.07 and Cursor Sampled Loop v0.5.13.
        // The visible startup/workflow does not attach to Inventor and does not call Autodesk APIs.
        // Historical legacy source and Autodesk interop reference remain in the repository for continuity;
        // extracting a physically dependency-free executable is documented separately and is not falsely claimed here.

        // 18:40 20.06.2026 InventorIptOrg_v0_5_15_RENDER_FPS_GROUP_FIX
        // ИСТОРИЧЕСКАЯ ГРУППИРОВКА ПРОВЕРЕНА ПО ПРИСЛАННЫМ ИСХОДНЫМ ZIP:
        // v0.5.06 = новый render method Mesh True Front; отдельного FPS/drag selector ещё нет.
        // v0.5.07 = только CS0136 compile fix того же Mesh True Front renderer; FPS/drag selector ещё нет.
        // v0.5.08 = первое появление backbuffer/FPS pipeline; v0.5.09..v0.5.13 = его последовательные поколения.
        // Поэтому Mode содержит v0.5.06 и отдельную compile-fix запись v0.5.07,
        // а FPS selector содержит ТОЛЬКО v0.5.08..v0.5.13.
        // ЗАМЕР НОВОГО РЕЖИМА / PERFORMANCE COMPARISON:
        // InventorIptOrg_v0_5_12_ADAPTIVE_DRAG_DIRECT_SWAP: AverageFrameMs=31.689 ms, AverageFps=9.58.
        // InventorIptOrg_v0_5_13_CURSOR_SAMPLED_DRAG_LOOP: AverageFrameMs=30.905 ms, RenderCapacityFps=32.36, AveragePresentFps=14.57.
        // v0.5.08 DRAG_BACKBUFFER_FPS — atomic viewer frame presentation and throttled interaction.
        private Timer _viewerDragRenderTimer;
        private System.Drawing.Bitmap _viewerBackBufferBitmap;
        private bool _viewerFrameRendering;
        private bool _viewerDragFramePending;
        private bool _viewerDragPaintScheduled;
        private int _viewerDragPendingInputEvents;
        private int _viewerDragInputEvents;
        private int _viewerDragFramesRendered;
        private int _viewerDragFramesCoalesced;
        private long _viewerDragSessionStartTimestamp;
        private long _viewerDragLastFrameCompletedTimestamp;
        private double _viewerDragFrameMsEma;
        private double _viewerDragFpsEma;
        private double _viewerDragRenderCapacityFps;
        private System.Drawing.Point _viewerDragCursorSampleScreen;
        private bool _viewerDragCursorSampleReady;
        private bool _viewerDragCameraDirty;
        private int _viewerDragCursorSamples;
        private int _viewerDragAppliedCursorDeltas;
        private int _viewerDragIdleTimerTicks;
        private double _viewerLastQualityFrameMs;
        private bool _viewerDragSummaryPending;
        private bool _viewerRenderingReducedDragFrame;
        private bool _viewerBackBufferDragFrame;
        private double _viewerBackBufferRenderScale = 1.0;
        private double _viewerDragAdaptiveScale = 0.45;
        private int _viewerDragAdaptiveScaleChanges;
        private const int ViewerDragTargetFps = 30;
        private const double ViewerDragInitialRenderScale = 0.50;
        private const double ViewerDragMinRenderScale = 0.30;
        private const double ViewerDragMaxRenderScale = 0.62;
        private const double ViewerDragTargetFrameMs = 30.0;
        private const int ViewerDragSchedulerIntervalMs = 1;


        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS =====================================================================================================
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ПРОФИЛЬ ПРОИЗВОДИТЕЛЬНОСТИ v0.4.5 INLINE_PERF_COMMENTS
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS -----------------------------------------------------------------------------------------------------
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Источник цифр: реальный лог v0.4.0 LOGGED_STABLE:
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS inventor_ipt_organizer_v0_4_0_20260613_215339_981.log
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Build token лога: IPT_ORGANIZER_v0.4.0_LOGGED_STABLE_2026-06-13
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Эти блоки специально оставлены прямо в коде рядом с функциями, чтобы при переключении методов
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS сразу было видно, сколько времени стоил каждый этап в контрольном тесте.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ВАЖНО:
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS 1. Это НЕ расчёт текущего запуска, а контрольная базовая карта времени из лога.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS 2. Вложенные строки НЕ нужно складывать как независимые этапы.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS 3. Большой родительский этап уже включает дочерние операции.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS 4. Текущие запуски продолжает измерять AppLogger в микросекундах.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS =====================================================================================================

        // 21:20 18.06.2026 InventorIptOrg_v0_4_74_MESH_VIEW_CORE
        // NEW: добавлена нейтральная DLL MeshViewCore и первый mesh-preview режим.
        // MeshViewCore хранит только MeshScene/MeshBody/MeshTriangle/MeshPoint3.
        // Адаптер приложения пробует получить facets/triangles из тел и рисует shaded mesh + wireframe overlay в canvas.
        // BoxGridCore остаётся для spatial grid; MeshViewCore отвечает только за mesh scene data.
        // 20:45 18.06.2026 InventorIptOrg_v0_4_72_PRIMITIVE_VISIBLE_CENTER_PAN
        // FIX primitive viewer по скрину пользователя: модель была в нижнем правом углу.
        // Fit теперь считается по реально видимому Paint ClipRectangle, а не по полному ClientSize.
        // Добавлен ручной pan: Shift+ЛКМ drag или ПКМ drag. Double-click сбрасывает rotation/zoom/pan.
        // Лог PRIMITIVE_VIEW_PROJECTION_FIT теперь пишет VisibleBounds/PanX/PanY/VisibleClipFit=True.
        // 20:25 18.06.2026 InventorIptOrg_v0_4_71_PRIMITIVE_VIEW_CENTER_FIT
        // FIX primitive viewer: модель смещалась в нижний правый угол.
        // Причина: старый viewer масштабировал от ModelBox max-size и центра панели, но не fit-ил реальную 2D-проекцию.
        // Теперь перед рисованием вычисляется raw projection bbox всех ModelBox/Cube/BodyBox углов,
        // затем scale/offset подбираются так, чтобы вся проекция была по центру правой области preview.
        // Drag/zoom/double-click сохранены.
        // 21:05 18.06.2026 InventorIptOrg_v0_4_70_PRIMITIVE_MOUSE_3D_VIEWER
        // UI-FIX: маленькая toolbar-панель 32x32 с программно нарисованными bitmap-значками.
        // NEW: локальный 3D viewer строит примитивную модель по Spatial BASE:
        // ModelBox + 8 cube bounds + SurfaceBody BodyBox primitives.
        // Мышь: ЛКМ drag = вращение, wheel = масштаб, double-click = сброс.
        // 20:35 18.06.2026 InventorIptOrg_v0_4_69_LIVE_3D_PREVIEW
        // Добавлены два режима "live 3D" на canvas:
        // LV Live — потоковое обновление preview через повторные Inventor.ActiveView.SaveAsBitmap по Timer.
        // DK Dock — экспериментальная попытка встроить HWND Inventor ActiveView в правую preview-панель через SetParent.
        // DK Dock является рискованным Win32-режимом: если Inventor не отдаёт HWND ActiveView, будет сообщение и fallback остаётся AV/LV.
        // 20:10 18.06.2026 InventorIptOrg_v0_4_68_ACTIVE_VIEW_PREVIEW
        // Canvas preview теперь может показывать всю модель как снимок текущего Inventor ActiveView.
        // Это не встраивание настоящего 3D viewport в WinForms, а быстрый bitmap-preview:
        // Inventor ActiveView.SaveAsBitmap -> _layerCanvasPreviewBitmap -> правый большой preview panel.
        // Добавлена квадратная кнопка AV Model.
        // 19:55 18.06.2026 InventorIptOrg_v0_4_67_CANVAS_LEFT_LEGACY_TREE
        // UI-FIX по скрину пользователя:
        // слева снизу вместо служебного Layer tree теперь постоянно открыто настоящее Legacy browser tree.
        // Это дерево клонируется из treeViewIptLegacyBrowserTree и должно выглядеть как старое дерево:
        // "Legacy browser tree / старое дерево браузера Inventor: 269".
        // Добавлена квадратная кнопка LR Refresh для обновления legacy дерева прямо из canvas.
        // 19:35 18.06.2026 InventorIptOrg_v0_4_66_CANVAS_LAYOUT_LEFT_TOOLS
        // UI-FIX по скрину пользователя:
        // - квадратные layer-кнопки перенесены в левый верхний угол;
        // - слева внизу добавлено постоянно открытое дерево слоёв;
        // - Model preview перенесён в крупную правую область и занимает весь оставшийся экран полотна;
        // - подсказки ToolTip и временные окна крупных областей сохранены.
        // 19:20 18.06.2026 InventorIptOrg_v0_4_65_LAYER_WINDOWS_COMPILE_FIX
        // COMPILE-FIX для v0.4.64:
        // В окне Browser Truth использовалось имя BrowserGridColCubeCount, которого в проекте нет.
        // Правильная существующая константа: BrowserGridColCubeCount.
        // Логика canvas/window output не менялась.
        // 18:10 18.06.2026 InventorIptOrg_v0_4_64_LAYER_WINDOWS_CANVAS
        // UI/архитектура диагностики:
        // крупные области теперь выводятся по запросу в отдельные временные окна, а не смешиваются в одну таблицу.
        // Добавлена вкладка ".ipt canvas": единое полотно, слева область превью модели, справа квадратные icon-buttons.
        // Каждая новая кнопка имеет ToolTip с задержкой наведения мыши:
        // Browser Truth / Spatial Cache / Merged View / Fast Cache / Legacy Tree / Model Preview.
        // Старые рабочие кнопки и v0.4.48 full refresh остаются на месте; canvas только показывает слои и открывает диагностические окна.

        public Form1()
        {
            using (AppLogger.Scope("Form1"))
            {
                InitializeComponent();
                this.Text = AppBuild.WindowTitle;
                AppLogger.Log("FORM_CONSTRUCTOR", nameof(Form1), "WindowTitle=" + this.Text + "; LogFilePath=" + AppLogger.LogFilePath);

                // GitHub canvas-only startup: do not initialize .ipt/.iam legacy UI, spatial-cube tools,
                // browser-tree COM paths, or automatic Inventor attachment. The local STEP viewer is
                // intentionally the only visible workflow in this build.
                InitializeLayerWindowsCanvasUi();
                ConfigureCanvasOnlyGitHubUi();
            }
        }

        private void TryAttachToRunningExternalSessionOnStartup()
        {
            using (AppLogger.Scope("TryAttachToRunningExternalSessionOnStartup"))
            {
            _invApp = null;
            _started = false;

            try
            {
                _invApp = (Inventor.Application)Marshal.GetActiveObject("Inventor.Application");

                AppLogger.Log(
                    "STARTUP_ATTACHED_TO_RUNNING_EXTERNAL_SESSION",
                    "TryAttachToRunningExternalSessionOnStartup",
                    "Attached=True; AutoStart=False");
            }
            catch (Exception ex)
            {
                _invApp = null;
                _started = false;

                AppLogger.Log(
                    "STARTUP_EXTERNAL_SESSION_NOT_FOUND",
                    "TryAttachToRunningExternalSessionOnStartup",
                    "Attached=False; AutoStart=False; Message=" + ex.Message);
            }

            }}

        private bool TryAttachToRunningExternalSessionForCommand()
        {
            using (AppLogger.Scope("TryAttachToRunningExternalSessionForCommand"))
            {
            if (_invApp != null)
            {
                return true;
            }

            try
            {
                _invApp = (Inventor.Application)Marshal.GetActiveObject("Inventor.Application");

                AppLogger.Log(
                    "COMMAND_ATTACHED_TO_RUNNING_EXTERNAL_SESSION",
                    "TryAttachToRunningExternalSessionForCommand",
                    "Attached=True; AutoStart=False");

                return true;
            }
            catch (Exception ex)
            {
                _invApp = null;

                AppLogger.Log(
                    "COMMAND_EXTERNAL_SESSION_NOT_FOUND",
                    "TryAttachToRunningExternalSessionForCommand",
                    "Attached=False; AutoStart=False; Message=" + ex.Message);

                LoggedMessageBox.Show(
                    "Внешний сеанс не запущен.\r\n\r\n" +
                    "Это приложение больше не запускает его автоматически.\r\n" +
                    "Для команд, которым нужен активный документ, сначала откройте внешний сеанс вручную.\r\n\r\n" +
                    "Локальная кнопка ST Step работает без внешнего сеанса.");

                return false;
            }

            }}

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            using (AppLogger.Scope("Form1_FormClosed"))
            {
                if (!CanvasOnlyGitHubMode)
                {
                    StopIptWindowSelection(false);
                    ClearSpatialCubePreviewSafe();
                    StopLayerCanvasLivePreview();
                    UndockActiveViewFromCanvas();

                    if (_started && _invApp != null)
                    {
                        _invApp.Quit();
                    }
                }

                _invApp = null;

                if (_layerCanvasPreviewBitmap != null)
                {
                    _layerCanvasPreviewBitmap.Dispose();
                    _layerCanvasPreviewBitmap = null;
                }

                if (_viewerDragRenderTimer != null)
                {
                    _viewerDragRenderTimer.Stop();
                    _viewerDragRenderTimer.Dispose();
                    _viewerDragRenderTimer = null;
                }

                if (_viewerBackBufferBitmap != null)
                {
                    _viewerBackBufferBitmap.Dispose();
                    _viewerBackBufferBitmap = null;
                }
            }
        }


        private void ConfigureCanvasOnlyGitHubUi()
        {
            if (!CanvasOnlyGitHubMode || tabControl1 == null || tabPageIptLayerCanvas == null)
            {
                return;
            }

            // A single visible tab makes the GitHub build self-explanatory and prevents accidental
            // access to historical Inventor/Autodesk-dependent controls.
            tabControl1.SuspendLayout();
            try
            {
                tabControl1.TabPages.Clear();
                tabControl1.TabPages.Add(tabPageIptLayerCanvas);
                tabControl1.SelectedTab = tabPageIptLayerCanvas;
            }
            finally
            {
                tabControl1.ResumeLayout(true);
            }

            AppLogger.Log(
                "GITHUB_CANVAS_ONLY_MODE",
                nameof(ConfigureCanvasOnlyGitHubUi),
                "VisibleTabs=.stp canvas; InventorAttach=False; AutodeskButtonsVisible=False; LocalStepOnly=True; LegacySourceRetained=True");
        }

        // ============================================================
        // v0.4.64 layer canvas / временные окна крупных областей
        // ============================================================
        private void InitializeLayerWindowsCanvasUi()
        {
            using (AppLogger.Scope("InitializeLayerWindowsCanvasUi"))
            {
            if (tabControl1 == null || tabPageIptLayerCanvas != null)
            {
                return;
            }

            toolTipIptLayerCanvas = new ToolTip();
            toolTipIptLayerCanvas.InitialDelay = 750;
            toolTipIptLayerCanvas.ReshowDelay = 250;
            toolTipIptLayerCanvas.AutoPopDelay = 18000;
            toolTipIptLayerCanvas.ShowAlways = true;

            tabPageIptLayerCanvas = new TabPage();
            tabPageIptLayerCanvas.Name = "tabPageIptLayerCanvas";
            tabPageIptLayerCanvas.Text = ".stp canvas";
            tabPageIptLayerCanvas.UseVisualStyleBackColor = true;
            tabPageIptLayerCanvas.AutoScroll = false;

            panelIptLayerCanvasRoot = new Panel();  
            panelIptLayerCanvasRoot.Dock = DockStyle.Fill;
            panelIptLayerCanvasRoot.BorderStyle = BorderStyle.FixedSingle;

            // LEFT TOP: square action buttons
            flowLayoutPanelIptLayerButtons = new FlowLayoutPanel();
            flowLayoutPanelIptLayerButtons.Location = new System.Drawing.Point(10, 10);
            flowLayoutPanelIptLayerButtons.Size = new System.Drawing.Size(404, 216);
            flowLayoutPanelIptLayerButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            flowLayoutPanelIptLayerButtons.WrapContents = true;
            flowLayoutPanelIptLayerButtons.AutoScroll = false;
            flowLayoutPanelIptLayerButtons.BorderStyle = BorderStyle.FixedSingle;

            ButtonIptOpenBrowserTruthWindow = CreateLayerSquareButton("BT", "Truth", "Открыть временное окно Browser Truth Cache: текущая полная таблица Browser tree после обычного Refresh.");
            ButtonIptOpenBrowserTruthWindow.Click += ButtonIptOpenBrowserTruthWindow_Click;

            ButtonIptCanvasOpenStepFile = CreateLayerSquareButton("ST", "Step", "Локально открыть .stp/.step в нашем canvas: StepLiteCore читает points/edges без открытия внешнего окна.");
            ButtonIptCanvasOpenStepFile.Click += delegate { ButtonIptCanvasOpenStepFile_Click(ButtonIptCanvasOpenStepFile, EventArgs.Empty); UpdateLayerCanvasPreviewStatus(); };

            ButtonIptOpenSpatialCacheWindow = CreateLayerSquareButton("SP", "Spatial", "Открыть временное окно Spatial Geometry Cache: 8 кубов, 72 тела, X/Y/Z, Cube IDs.");
            ButtonIptOpenSpatialCacheWindow.Click += ButtonIptOpenSpatialCacheWindow_Click;

            ButtonIptOpenMergedCacheWindow = CreateLayerSquareButton("MG", "Merged", "Открыть временное окно Merged View: Browser row + SurfaceBody + X/Y/Z + Cubes.");
            ButtonIptOpenMergedCacheWindow.Click += ButtonIptOpenMergedCacheWindow_Click;

            ButtonIptOpenFastCacheWindow = CreateLayerSquareButton("FC", "Fast", "Открыть временное окно Fast Cache View: cache-view для #2 BASE.");
            ButtonIptOpenFastCacheWindow.Click += ButtonIptOpenFastCacheWindow_Click;

            ButtonIptOpenLegacyCacheWindow = CreateLayerSquareButton("TR", "Legacy", "Открыть временное окно Legacy browser tree / старое дерево браузера Inventor.");
            ButtonIptOpenLegacyCacheWindow.Click += ButtonIptOpenLegacyCacheWindow_Click;

            ButtonIptOpenModelPreviewWindow = CreateLayerSquareButton("PV", "Preview", "Открыть временное окно preview/status модели.");
            ButtonIptOpenModelPreviewWindow.Click += ButtonIptOpenModelPreviewWindow_Click;

            ButtonIptCanvasCaptureActiveView = CreateLayerSquareButton("AV", "Model", "Снять текущий Inventor ActiveView и показать всю модель в большой области справа.");
            ButtonIptCanvasCaptureActiveView.Click += delegate { CaptureActiveInventorViewToLayerPreview(); UpdateLayerCanvasPreviewStatus(); };

            ButtonIptCanvasBuildMeshPreview = CreateLayerSquareButton("MS", "Mesh", "Построить mesh-preview: triangles/facets тел и показать shaded mesh + wireframe.");
            ButtonIptCanvasBuildMeshPreview.Click += delegate { ButtonIptCanvasBuildMeshPreview_Click(ButtonIptCanvasBuildMeshPreview, EventArgs.Empty); UpdateLayerCanvasPreviewStatus(); };

            ButtonIptCanvasToggleLivePreview = CreateLayerSquareButton("LV", "Live", "Запустить/остановить live-preview поток: периодически обновляет снимок Inventor ActiveView.");
            ButtonIptCanvasToggleLivePreview.Click += delegate { ToggleLayerCanvasLivePreview(); UpdateLayerCanvasPreviewStatus(); };

            ButtonIptCanvasDockActiveView = CreateLayerSquareButton("DK", "Dock", "Эксперимент: встроить настоящее окно Inventor ActiveView в правую preview-панель через Win32 SetParent.");
            ButtonIptCanvasDockActiveView.Click += delegate { ToggleDockActiveViewInCanvas(); UpdateLayerCanvasPreviewStatus(); };

            ButtonIptCanvasFullRefresh = CreateLayerSquareButton("RF", "Full", "Запустить канонический полный Refresh browser tree из v0.4.48. Медленно, но это источник правды.");
            ButtonIptCanvasFullRefresh.Click += delegate { ButtonIptRefreshBrowserTree_Click(ButtonIptRefreshBrowserTree, EventArgs.Empty); UpdateLayerCanvasPreviewStatus(); };

            ButtonIptCanvasBuildCubes = CreateLayerSquareButton("8C", "Cubes", "Построить Fast spatial cubes BASE: 2x2x2 = 8 кубов. Это геометрический слой, не Browser tree.");
            ButtonIptCanvasBuildCubes.Click += delegate { ButtonIptBuildSpatialCubes_Click(ButtonIptBuildSpatialCubes, EventArgs.Empty); UpdateLayerCanvasPreviewStatus(); };

            ButtonIptCanvasRefreshLegacyTree = CreateLayerSquareButton("LR", "Refresh", "Обновить постоянное Legacy browser tree в левом нижнем углу canvas.");
            ButtonIptCanvasRefreshLegacyTree.Click += delegate { ButtonIptLegacyRefreshBrowserTree_Click(ButtonIptLegacyRefreshBrowserTree, EventArgs.Empty); RefreshLayerCanvasTree(); UpdateLayerCanvasPreviewStatus(); };

            comboBoxIptViewerSource = CreateLayerViewerCombo(new string[] { "Source: Auto", "Source: STEP" }, "Источник геометрии для правого viewer. Auto выбирает STEP, потом Mesh, потом BoxGrid.");
            comboBoxIptViewerRenderMode = CreateLayerViewerCombo(
                new string[]
                {
                    "Mode: Wire — v0.4.84",
                    "Mode: Surface — v0.4.96",
                    "Mode: Surface+Edges — v0.4.96",
                    "Mode: Surface+Mesh — v0.4.99",
                    "Mode: Mesh Deep Clamp — v0.5.02",
                    "Mode: Mesh Two-Light — v0.5.02",
                    "Mode: Mesh Hemisphere — v0.5.02",
                    "Mode: Mesh Inventor Lab — v0.5.03",
                    "Mode: Mesh Opaque Body — v0.5.04",
                    "Mode: Mesh Solid Front — v0.5.05",
                    "Mode: Mesh True Front — v0.5.06",
                    "🔥Mode: Mesh True Front (compile fix) — v0.5.07",
                    "Mode: Ghost — v0.4.84",
                    "Mode: Points — v0.4.84"
                },
                "Версия в конце каждого пункта — историческая версия метода. Mesh True Front v0.5.06 вводит исправленный front-face Z-buffer. Отдельный пункт v0.5.07 воспроизводит тот же renderer после исправления CS0136; визуальный алгоритм между ними не менялся.");
            comboBoxIptViewerRenderMode.Size = new System.Drawing.Size(380, 24);
            comboBoxIptViewerRenderMode.DropDownWidth = 500;
            comboBoxIptViewerRenderMode.SelectedIndex = 11;

            comboBoxIptViewerFpsMode = CreateLayerViewerFpsCombo(
                new string[]
                {
                    "FPS: Drag Backbuffer — v0.5.08",
                    "FPS: Camera Center Lock — v0.5.09",
                    "FPS: DPI Center Fix — v0.5.10",
                    "FPS: Visible Viewport Center — v0.5.11",
                    "FPS: Adaptive Direct Swap — v0.5.12",
                    "🔥FPS: Cursor Sampled Loop — v0.5.13"
                },
                "Независимый выбор исторического drag/FPS pipeline. FPS-система впервые появилась в v0.5.08; v0.5.06 и v0.5.07 относятся только к render method и находятся в списке Mode. v0.5.08–v0.5.13 последовательно переключают backbuffer, center lock, DPI mapping, visible viewport, adaptive direct swap и cursor-sampled timer loop.");
            comboBoxIptViewerFpsMode.Size = new System.Drawing.Size(380, 24);
            comboBoxIptViewerFpsMode.DropDownWidth = 460;
            comboBoxIptViewerFpsMode.SelectedIndex = 5;

            ButtonIptViewerOptions = CreateLayerSquareButton("OP", "Opt", "Options popup: точки, BoxGrid overlay, shaded draft and historical FPS pipeline selection.");
            ButtonIptViewerOptions.Click += delegate { ShowViewerOptionsMenu(); };

            InitializeViewerOptionsMenu();

            Button buttonCanvasResetView = CreateLayerSquareButton("RS", "Reset", "Сбросить вращение, масштаб и pan локального STEP viewer.");
            buttonCanvasResetView.Click += delegate { ResetPrimitiveViewerCamera(); };

            // GitHub canvas-only toolbar. Inventor/Autodesk-dependent buttons are constructed only as
            // historical fields above but are intentionally not exposed in the visible UI.
            flowLayoutPanelIptLayerButtons.Controls.Add(ButtonIptCanvasOpenStepFile);
            flowLayoutPanelIptLayerButtons.Controls.Add(buttonCanvasResetView);
            flowLayoutPanelIptLayerButtons.Controls.Add(ButtonIptViewerOptions);
            flowLayoutPanelIptLayerButtons.Controls.Add(comboBoxIptViewerSource);
            flowLayoutPanelIptLayerButtons.Controls.Add(comboBoxIptViewerRenderMode);
            flowLayoutPanelIptLayerButtons.Controls.Add(comboBoxIptViewerFpsMode);

            // LEFT BOTTOM: always-open layer tree
            labelIptLayerTreeTitle = new Label();
            labelIptLayerTreeTitle.AutoSize = false;
            labelIptLayerTreeTitle.Location = new System.Drawing.Point(10, 236);
            labelIptLayerTreeTitle.Size = new System.Drawing.Size(404, 24);
            labelIptLayerTreeTitle.Text = "GitHub canvas-only / локальный STEP viewer";
            labelIptLayerTreeTitle.Font = new System.Drawing.Font("Segoe UI", 9.0F, System.Drawing.FontStyle.Bold);
            labelIptLayerTreeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            treeViewIptLayerCanvasTree = new TreeView();
            treeViewIptLayerCanvasTree.Location = new System.Drawing.Point(10, 264);
            treeViewIptLayerCanvasTree.Size = new System.Drawing.Size(404, 510);
            treeViewIptLayerCanvasTree.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            treeViewIptLayerCanvasTree.HideSelection = false;
            // Static information tree in canvas-only mode; no Inventor callbacks are attached.
            if (toolTipIptLayerCanvas != null)
            {
                toolTipIptLayerCanvas.SetToolTip(treeViewIptLayerCanvasTree, "Локальный STEP viewer. Видимый workflow не подключается к Autodesk Inventor.");
            }

            // RIGHT: large model preview takes the rest of the canvas
            panelIptLayerPreview = new Panel();
            panelIptLayerPreview.Location = new System.Drawing.Point(428, 10);
            panelIptLayerPreview.Size = new System.Drawing.Size(842, 764);
            panelIptLayerPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelIptLayerPreview.BorderStyle = BorderStyle.FixedSingle;
            panelIptLayerPreview.BackColor = System.Drawing.Color.White;

            labelIptLayerPreviewTitle = new Label();
            labelIptLayerPreviewTitle.AutoSize = false;
            labelIptLayerPreviewTitle.Location = new System.Drawing.Point(8, 8);
            labelIptLayerPreviewTitle.Size = new System.Drawing.Size(792, 34);
            labelIptLayerPreviewTitle.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            labelIptLayerPreviewTitle.Text = "Model preview / крупная область просмотра модели";
            labelIptLayerPreviewTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelIptLayerPreviewTitle.Font = new System.Drawing.Font("Segoe UI", 10.0F, System.Drawing.FontStyle.Bold);

            panelIptLayerPreviewDrawing = new BufferedViewerPanel();
            panelIptLayerPreviewDrawing.Location = new System.Drawing.Point(8, 48);
            panelIptLayerPreviewDrawing.Size = new System.Drawing.Size(822, 570);
            panelIptLayerPreviewDrawing.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelIptLayerPreviewDrawing.BorderStyle = BorderStyle.FixedSingle;
            panelIptLayerPreviewDrawing.BackColor = System.Drawing.Color.White;
            panelIptLayerPreviewDrawing.Paint += PanelIptLayerPreviewDrawing_Paint;
            panelIptLayerPreviewDrawing.Resize += PanelIptLayerPreviewDrawing_Resize;
            panelIptLayerPreviewDrawing.MouseDown += PanelIptLayerPreviewDrawing_MouseDown;
            panelIptLayerPreviewDrawing.MouseMove += PanelIptLayerPreviewDrawing_MouseMove;
            panelIptLayerPreviewDrawing.MouseUp += PanelIptLayerPreviewDrawing_MouseUp;
            panelIptLayerPreviewDrawing.MouseCaptureChanged += PanelIptLayerPreviewDrawing_MouseCaptureChanged;
            panelIptLayerPreviewDrawing.MouseWheel += PanelIptLayerPreviewDrawing_MouseWheel;
            panelIptLayerPreviewDrawing.MouseEnter += delegate { try { panelIptLayerPreviewDrawing.Focus(); } catch { } };
            panelIptLayerPreviewDrawing.DoubleClick += delegate { ResetPrimitiveViewerCamera(); };

            _viewerDragRenderTimer = new Timer();
            _viewerDragRenderTimer.Interval = ViewerDragSchedulerIntervalMs;
            _viewerDragRenderTimer.Tick += ViewerDragRenderTimer_Tick;

            labelIptLayerPreviewStatus = new Label();
            labelIptLayerPreviewStatus.AutoSize = false;
            labelIptLayerPreviewStatus.Location = new System.Drawing.Point(8, 626);
            labelIptLayerPreviewStatus.Size = new System.Drawing.Size(822, 120);
            labelIptLayerPreviewStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            labelIptLayerPreviewStatus.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            labelIptLayerPreviewStatus.BorderStyle = BorderStyle.FixedSingle;

            labelIptLayerCanvasHint = new Label();
            labelIptLayerCanvasHint.AutoSize = false;
            labelIptLayerCanvasHint.Location = new System.Drawing.Point(0, 0);
            labelIptLayerCanvasHint.Size = new System.Drawing.Size(0, 0);
            labelIptLayerCanvasHint.Visible = false;

            panelIptLayerPreview.Controls.Add(labelIptLayerPreviewTitle);
            panelIptLayerPreview.Controls.Add(panelIptLayerPreviewDrawing);
            panelIptLayerPreview.Controls.Add(labelIptLayerPreviewStatus);

            panelIptLayerCanvasRoot.Controls.Add(flowLayoutPanelIptLayerButtons);
            panelIptLayerCanvasRoot.Controls.Add(labelIptLayerTreeTitle);
            panelIptLayerCanvasRoot.Controls.Add(treeViewIptLayerCanvasTree);
            panelIptLayerCanvasRoot.Controls.Add(panelIptLayerPreview);
            panelIptLayerCanvasRoot.Controls.Add(labelIptLayerCanvasHint);

            tabPageIptLayerCanvas.Controls.Add(panelIptLayerCanvasRoot);
            tabControl1.Controls.Add(tabPageIptLayerCanvas);

            PopulateCanvasOnlyInfoTree();
            UpdateLayerCanvasPreviewStatus();
            SelectIptCanvasTabByDefault();

            AppLogger.Log(
                "LAYER_WINDOWS_CANVAS_READY",
                "InitializeLayerWindowsCanvasUi",
                "Layout=v0.5.17_CanvasOnlyWarningFree" +
                "; Buttons=3; SquareButtons=True; ViewerSelectors=True; SourceOptions=Auto|STEP; ToolTips=True; ToolTipInitialDelayMs=750" +
                "; InventorButtonsVisible=False; StaticInfoTree=True; PreviewPanelRightLarge=True; DefaultTab=.ipt canvas");

            }}

        private void PopulateCanvasOnlyInfoTree()
        {
            if (treeViewIptLayerCanvasTree == null)
            {
                return;
            }

            treeViewIptLayerCanvasTree.BeginUpdate();
            try
            {
                treeViewIptLayerCanvasTree.Nodes.Clear();
                TreeNode root = new TreeNode("GitHub canvas-only mode");
                root.Nodes.Add(new TreeNode("ST Step — открыть локальный .stp/.step"));
                root.Nodes.Add(new TreeNode("Software Z-buffer / True Front renderer"));
                root.Nodes.Add(new TreeNode("POLY_LOOP mesh без fan-triangle diagonals"));
                root.Nodes.Add(new TreeNode("LMB rotate; Shift+LMB/RMB pan; wheel zoom"));
                root.Nodes.Add(new TreeNode("Видимый workflow: без вызовов Inventor/Autodesk API"));
                root.Nodes.Add(new TreeNode("Legacy Inventor source сохранён, но UI скрыт"));
                treeViewIptLayerCanvasTree.Nodes.Add(root);
                root.ExpandAll();
            }
            finally
            {
                treeViewIptLayerCanvasTree.EndUpdate();
            }
        }

        private void SelectIptCanvasTabByDefault()
        {
            try
            {
                if (tabControl1 != null && tabPageIptLayerCanvas != null && tabControl1.TabPages.Contains(tabPageIptLayerCanvas))
                {
                    tabControl1.SelectedTab = tabPageIptLayerCanvas;
                    AppLogger.Log(
                        "DEFAULT_TAB_SELECTED",
                        "SelectIptCanvasTabByDefault",
                        "SelectedTab=.ipt canvas; Build=v0.5.17_WARNING_FREE_CANVAS_BUILD");
                }
            }
            catch (Exception ex)
            {
                AppLogger.Log(
                    "DEFAULT_TAB_SELECT_FAILED",
                    "SelectIptCanvasTabByDefault",
                    ex.GetType().Name + ": " + ex.Message);
            }
        }

        private void RefreshLayerCanvasTree()
        {
            if (treeViewIptLayerCanvasTree == null)
            {
                return;
            }

            treeViewIptLayerCanvasTree.BeginUpdate();
            try
            {
                treeViewIptLayerCanvasTree.Nodes.Clear();

                if (treeViewIptLegacyBrowserTree != null && treeViewIptLegacyBrowserTree.Nodes.Count > 0)
                {
                    foreach (TreeNode node in treeViewIptLegacyBrowserTree.Nodes)
                    {
                        treeViewIptLayerCanvasTree.Nodes.Add((TreeNode)node.Clone());
                    }

                    if (treeViewIptLayerCanvasTree.Nodes.Count > 0)
                    {
                        treeViewIptLayerCanvasTree.Nodes[0].Expand();
                    }
                }
                else
                {
                    TreeNode root = new TreeNode("Legacy browser tree is empty");
                    TreeNode refresh = new TreeNode("LR Refresh — построить legacy дерево") { Tag = "legacyRefresh" };
                    TreeNode open = new TreeNode("TR Legacy — открыть временное окно") { Tag = "legacy" };
                    TreeNode hint = new TreeNode("Ожидаемое дерево: Модель → папки → тела/элементы, 269 узлов");

                    root.Nodes.Add(refresh);
                    root.Nodes.Add(open);
                    root.Nodes.Add(hint);

                    treeViewIptLayerCanvasTree.Nodes.Add(root);
                    root.Expand();
                }

                AppLogger.Log(
                    "CANVAS_LEFT_LEGACY_TREE_REFRESHED",
                    "RefreshLayerCanvasTree",
                    "CanvasTreeNodes=" + CountTreeViewNodes(treeViewIptLayerCanvasTree.Nodes).ToString() +
                    "; SourceLegacyNodes=" + (treeViewIptLegacyBrowserTree == null ? "0" : CountTreeViewNodes(treeViewIptLegacyBrowserTree.Nodes).ToString()) +
                    "; ReplacesLayerTree=True");
            }
            finally
            {
                treeViewIptLayerCanvasTree.EndUpdate();
            }
        }

        private void TreeViewIptLayerCanvasTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string tag = e == null || e.Node == null ? string.Empty : Convert.ToString(e.Node.Tag);

            if (string.Equals(tag, "legacyRefresh", StringComparison.OrdinalIgnoreCase))
            {
                ButtonIptLegacyRefreshBrowserTree_Click(ButtonIptLegacyRefreshBrowserTree, EventArgs.Empty);
                RefreshLayerCanvasTree();
                UpdateLayerCanvasPreviewStatus();
            }
            else if (string.Equals(tag, "legacy", StringComparison.OrdinalIgnoreCase))
            {
                ButtonIptOpenLegacyCacheWindow_Click(sender, EventArgs.Empty);
            }
        }

        private Button CreateLayerSquareButton(string icon, string caption, string tooltip)
        {
            Button button = new Button();
            button.Size = new System.Drawing.Size(32, 32);
            button.Margin = new Padding(3);
            button.Text = string.Empty;
            button.Image = CreateLayerButtonIcon(icon, caption);
            button.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
            button.TextImageRelation = TextImageRelation.Overlay;
            button.UseVisualStyleBackColor = true;
            button.FlatStyle = FlatStyle.Standard;
            button.TabStop = false;

            if (toolTipIptLayerCanvas != null)
            {
                toolTipIptLayerCanvas.SetToolTip(button, icon + " " + caption + "\r\n\r\n" + tooltip);
            }

            return button;
        }

        private ComboBox CreateLayerViewerCombo(string[] items, string tooltip)
        {
            ComboBox combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Size = new System.Drawing.Size(128, 24);
            combo.Margin = new Padding(3);
            combo.TabStop = false;
            ConfigureColorFireCombo(combo);

            if (items != null)
            {
                combo.Items.AddRange(items);
            }

            if (combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }

            combo.SelectedIndexChanged += delegate
            {
                string selectedRenderMode = GetViewerRenderMode();
                StepRenderMethodProfile selectedProfile = GetStepRenderMethodProfile(selectedRenderMode);

                AppLogger.Log(
                    "VIEWER_MODE_CHANGED",
                    "CreateLayerViewerCombo",
                    "Source=" + GetViewerSourceMode() +
                    "; AppVersion=" + AppBuild.Version +
                    "; MethodVersion=" + selectedProfile.MethodVersion +
                    "; RenderMode=" + selectedRenderMode +
                    "; ModeItem=" + GetViewerRenderModeDisplayText() +
                    "; ShowPoints=" + _viewerShowPointsOverlay.ToString() +
                    "; ShowBoxes=" + _viewerShowBoxesOverlay.ToString() +
                    "; DraftWhileDragging=" + _viewerDraftWhileDragging.ToString() +
                    "; FpsModeItem=" + GetViewerFpsModeDisplayText() +
                    "; DragMethodVersion=" + GetViewerFpsModeVersion() +
                    "; DragPipeline=" + GetViewerFpsPipelineId());

                AppLogger.Log(
                    "VIEWER_RENDER_METHOD_SELECTED",
                    "CreateLayerViewerCombo",
                    BuildStepRenderMethodLogFields(selectedProfile));

                if (panelIptLayerPreviewDrawing != null)
                {
                    panelIptLayerPreviewDrawing.Invalidate();
                }

                UpdateLayerCanvasPreviewStatus();
            };

            if (toolTipIptLayerCanvas != null)
            {
                toolTipIptLayerCanvas.SetToolTip(combo, tooltip);
            }

            return combo;
        }


        private ComboBox CreateLayerViewerFpsCombo(string[] items, string tooltip)
        {
            ComboBox combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Size = new System.Drawing.Size(205, 24);
            combo.Margin = new Padding(3);
            combo.TabStop = false;
            ConfigureColorFireCombo(combo);

            if (items != null)
            {
                combo.Items.AddRange(items);
            }

            if (combo.Items.Count > 0)
            {
                combo.SelectedIndex = 0;
            }

            combo.SelectedIndexChanged += delegate
            {
                ResetViewerFpsPipelineAfterSelection();

                AppLogger.Log(
                    "VIEWER_FPS_MODE_CHANGED",
                    "CreateLayerViewerFpsCombo",
                    BuildViewerFpsModeLogFields());

                if (panelIptLayerPreviewDrawing != null)
                {
                    panelIptLayerPreviewDrawing.Invalidate();
                }

                UpdateLayerCanvasPreviewStatus();
            };

            if (toolTipIptLayerCanvas != null)
            {
                toolTipIptLayerCanvas.SetToolTip(combo, tooltip);
            }

            return combo;
        }

        private static void ConfigureColorFireCombo(ComboBox combo)
        {
            if (combo == null)
            {
                return;
            }

            combo.DrawMode = DrawMode.OwnerDrawFixed;
            combo.ItemHeight = Math.Max(22, combo.Font.Height + 6);
            combo.DrawItem += ColorFireCombo_DrawItem;
        }

        private static void ColorFireCombo_DrawItem(object sender, DrawItemEventArgs e)
        {
            ComboBox combo = sender as ComboBox;
            if (combo == null)
            {
                return;
            }

            int itemIndex = e.Index >= 0 ? e.Index : combo.SelectedIndex;
            if (itemIndex < 0 || itemIndex >= combo.Items.Count)
            {
                return;
            }

            string rawText = Convert.ToString(combo.Items[itemIndex], CultureInfo.InvariantCulture) ?? string.Empty;
            bool showFire = rawText.StartsWith("🔥", StringComparison.Ordinal);
            string visibleText = showFire ? rawText.Substring("🔥".Length) : rawText;

            e.DrawBackground();

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            System.Drawing.Color textColor = selected ? System.Drawing.SystemColors.HighlightText : combo.ForeColor;
            System.Drawing.Rectangle textBounds = e.Bounds;
            textBounds.X += 5;
            textBounds.Width = Math.Max(0, textBounds.Width - 8);

            if (showFire)
            {
                int iconSize = Math.Max(12, Math.Min(18, e.Bounds.Height - 4));
                System.Drawing.Rectangle iconBounds = new System.Drawing.Rectangle(
                    e.Bounds.Left + 4,
                    e.Bounds.Top + ((e.Bounds.Height - iconSize) / 2),
                    iconSize,
                    iconSize);

                DrawColorFireIcon(e.Graphics, iconBounds);
                textBounds.X = iconBounds.Right + 4;
                textBounds.Width = Math.Max(0, e.Bounds.Right - textBounds.X - 3);
            }

            TextRenderer.DrawText(
                e.Graphics,
                visibleText,
                combo.Font,
                textBounds,
                textColor,
                TextFormatFlags.Left |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine |
                TextFormatFlags.EndEllipsis |
                TextFormatFlags.NoPrefix);

            if ((e.State & DrawItemState.Focus) == DrawItemState.Focus)
            {
                e.DrawFocusRectangle();
            }
        }

        private static void DrawColorFireIcon(System.Drawing.Graphics graphics, System.Drawing.Rectangle bounds)
        {
            if (graphics == null || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return;
            }

            System.Drawing.Drawing2D.SmoothingMode previousSmoothing = graphics.SmoothingMode;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            try
            {
                System.Drawing.Rectangle glowBounds = new System.Drawing.Rectangle(
                    bounds.Left - 2,
                    bounds.Top - 1,
                    bounds.Width + 4,
                    bounds.Height + 3);

                using (System.Drawing.SolidBrush glowBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(55, 255, 128, 0)))
                {
                    graphics.FillEllipse(glowBrush, glowBounds);
                }

                float x = bounds.Left;
                float y = bounds.Top;
                float w = bounds.Width;
                float h = bounds.Height;

                using (System.Drawing.Drawing2D.GraphicsPath outerFlame = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    outerFlame.StartFigure();
                    outerFlame.AddBezier(x + (0.50f * w), y + h, x + (0.17f * w), y + (0.90f * h), x + (0.08f * w), y + (0.64f * h), x + (0.34f * w), y + (0.30f * h));
                    outerFlame.AddBezier(x + (0.34f * w), y + (0.30f * h), x + (0.42f * w), y + (0.20f * h), x + (0.48f * w), y + (0.35f * h), x + (0.52f * w), y + (0.45f * h));
                    outerFlame.AddBezier(x + (0.52f * w), y + (0.45f * h), x + (0.62f * w), y + (0.31f * h), x + (0.64f * w), y + (0.10f * h), x + (0.67f * w), y + (0.02f * h));
                    outerFlame.AddBezier(x + (0.67f * w), y + (0.02f * h), x + (0.94f * w), y + (0.31f * h), x + (0.96f * w), y + (0.72f * h), x + (0.50f * w), y + h);
                    outerFlame.CloseFigure();

                    using (System.Drawing.Drawing2D.LinearGradientBrush outerBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        bounds,
                        System.Drawing.Color.FromArgb(255, 255, 238, 0),
                        System.Drawing.Color.FromArgb(255, 235, 42, 16),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        graphics.FillPath(outerBrush, outerFlame);
                    }
                }

                using (System.Drawing.Drawing2D.GraphicsPath innerFlame = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    innerFlame.StartFigure();
                    innerFlame.AddBezier(x + (0.50f * w), y + (0.91f * h), x + (0.31f * w), y + (0.82f * h), x + (0.33f * w), y + (0.61f * h), x + (0.48f * w), y + (0.48f * h));
                    innerFlame.AddBezier(x + (0.48f * w), y + (0.48f * h), x + (0.51f * w), y + (0.61f * h), x + (0.57f * w), y + (0.66f * h), x + (0.62f * w), y + (0.45f * h));
                    innerFlame.AddBezier(x + (0.62f * w), y + (0.45f * h), x + (0.78f * w), y + (0.66f * h), x + (0.72f * w), y + (0.84f * h), x + (0.50f * w), y + (0.91f * h));
                    innerFlame.CloseFigure();

                    using (System.Drawing.Drawing2D.LinearGradientBrush innerBrush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        bounds,
                        System.Drawing.Color.White,
                        System.Drawing.Color.FromArgb(255, 255, 170, 0),
                        System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                    {
                        graphics.FillPath(innerBrush, innerFlame);
                    }
                }
            }
            finally
            {
                graphics.SmoothingMode = previousSmoothing;
            }
        }

        private void InitializeViewerOptionsMenu()
        {
            contextMenuIptViewerOptions = new ContextMenuStrip();

            ToolStripMenuItem showPoints = new ToolStripMenuItem("Show points overlay");
            showPoints.CheckOnClick = true;
            showPoints.Checked = _viewerShowPointsOverlay;
            showPoints.Click += delegate
            {
                _viewerShowPointsOverlay = showPoints.Checked;
                InvalidateViewerAfterOptionChange();
            };

            ToolStripMenuItem showBoxes = new ToolStripMenuItem("Show BoxGrid overlay");
            showBoxes.CheckOnClick = true;
            showBoxes.Checked = _viewerShowBoxesOverlay;
            showBoxes.Click += delegate
            {
                _viewerShowBoxesOverlay = showBoxes.Checked;
                InvalidateViewerAfterOptionChange();
            };

            ToolStripMenuItem draftDrag = new ToolStripMenuItem("30 FPS shaded draft while dragging");
            draftDrag.CheckOnClick = true;
            draftDrag.Checked = _viewerDraftWhileDragging;
            draftDrag.Click += delegate
            {
                _viewerDraftWhileDragging = draftDrag.Checked;
                InvalidateViewerAfterOptionChange();
            };

            contextMenuIptViewerOptions.Items.Add(showPoints);
            if (!CanvasOnlyGitHubMode)
            {
                contextMenuIptViewerOptions.Items.Add(showBoxes);
            }
            contextMenuIptViewerOptions.Items.Add(new ToolStripSeparator());
            contextMenuIptViewerOptions.Items.Add(draftDrag);
        }

        private void ShowViewerOptionsMenu()
        {
            if (contextMenuIptViewerOptions == null)
            {
                InitializeViewerOptionsMenu();
            }

            if (ButtonIptViewerOptions != null)
            {
                contextMenuIptViewerOptions.Show(ButtonIptViewerOptions, new System.Drawing.Point(0, ButtonIptViewerOptions.Height));
            }
        }

        private void InvalidateViewerAfterOptionChange()
        {
            AppLogger.Log(
                "VIEWER_OPTIONS_CHANGED",
                "InvalidateViewerAfterOptionChange",
                "Source=" + GetViewerSourceMode() +
                "; RenderMode=" + GetViewerRenderMode() +
                "; ShowPoints=" + _viewerShowPointsOverlay.ToString() +
                "; ShowBoxes=" + _viewerShowBoxesOverlay.ToString() +
                "; DraftWhileDragging=" + _viewerDraftWhileDragging.ToString() +
                "; DragMethodVersion=" + GetViewerFpsModeVersion() +
                "; DragPipeline=" + GetViewerFpsPipelineId());

            if (panelIptLayerPreviewDrawing != null)
            {
                panelIptLayerPreviewDrawing.Invalidate();
            }

            UpdateLayerCanvasPreviewStatus();
        }

        private string GetViewerSourceMode()
        {
            string text = comboBoxIptViewerSource == null || comboBoxIptViewerSource.SelectedItem == null
                ? "Source: Auto"
                : comboBoxIptViewerSource.SelectedItem.ToString();

            if (text.IndexOf("STEP", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "STEP";
            }

            if (text.IndexOf("Mesh", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Mesh";
            }

            if (text.IndexOf("BoxGrid", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "BoxGrid";
            }

            return "Auto";
        }

        private string GetViewerRenderMode()
        {
            string text = GetViewerRenderModeDisplayText();

            if (text.IndexOf("Mesh True Front (compile fix)", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceMeshTrueFrontCompileFix";
            }

            if (text.IndexOf("Mesh True Front", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceMeshTrueFront";
            }

            if (text.IndexOf("Mesh Solid Front", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceMeshSolidFront";
            }

            if (text.IndexOf("Mesh Opaque Body", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceMeshOpaqueBody";
            }

            if (text.IndexOf("Mesh Inventor Lab", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceMeshInventorLab";
            }

            if (text.IndexOf("Mesh Hemisphere", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceMeshHemisphere";
            }

            if (text.IndexOf("Mesh Two-Light", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceMeshTwoLight";
            }

            if (text.IndexOf("Mesh Deep Clamp", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceMeshDeepClamp";
            }

            if (text.IndexOf("Surface+Mesh", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceMesh";
            }

            if (text.IndexOf("Surface+Edges", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "SurfaceEdges";
            }

            if (text.IndexOf("Surface", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Surface";
            }

            if (text.IndexOf("Ghost", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Ghost";
            }

            if (text.IndexOf("Points", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "Points";
            }

            return "Wire";
        }

        private string GetViewerRenderModeDisplayText()
        {
            return comboBoxIptViewerRenderMode == null || comboBoxIptViewerRenderMode.SelectedItem == null
                ? "Mode: Surface+Mesh — v0.4.99"
                : comboBoxIptViewerRenderMode.SelectedItem.ToString();
        }


        private string GetViewerFpsModeDisplayText()
        {
            return comboBoxIptViewerFpsMode == null || comboBoxIptViewerFpsMode.SelectedItem == null
                ? "FPS: Cursor Sampled Loop — v0.5.13"
                : comboBoxIptViewerFpsMode.SelectedItem.ToString();
        }

        private int GetViewerFpsModeLevel()
        {
            string text = GetViewerFpsModeDisplayText();
            if (text.IndexOf("v0.5.08", StringComparison.OrdinalIgnoreCase) >= 0) { return 8; }
            if (text.IndexOf("v0.5.09", StringComparison.OrdinalIgnoreCase) >= 0) { return 9; }
            if (text.IndexOf("v0.5.10", StringComparison.OrdinalIgnoreCase) >= 0) { return 10; }
            if (text.IndexOf("v0.5.11", StringComparison.OrdinalIgnoreCase) >= 0) { return 11; }
            if (text.IndexOf("v0.5.12", StringComparison.OrdinalIgnoreCase) >= 0) { return 12; }
            return 13;
        }

        private string GetViewerFpsModeVersion()
        {
            return "v0.5." + GetViewerFpsModeLevel().ToString("00", CultureInfo.InvariantCulture);
        }

        private string GetViewerFpsPipelineId()
        {
            switch (GetViewerFpsModeLevel())
            {
                case 8: return "DragBackbufferFpsV0508";
                case 9: return "DragCameraCenterLockV0509";
                case 10: return "DragDpiCenterFixV0510";
                case 11: return "VisibleViewportCenterFixV0511";
                case 12: return "AdaptiveDragDirectSwapV0512";
                default: return "CursorSampledDragLoopV0513";
            }
        }

        private bool ViewerFpsUsesBackbuffer()
        {
            return GetViewerFpsModeLevel() >= 8;
        }

        private bool ViewerFpsUsesVisibleViewport()
        {
            return GetViewerFpsModeLevel() >= 11;
        }

        private bool ViewerFpsUsesAdaptiveScale()
        {
            return GetViewerFpsModeLevel() >= 12;
        }

        private bool ViewerFpsUsesCursorSampling()
        {
            return GetViewerFpsModeLevel() >= 13;
        }

        private bool ViewerFpsUsesDirectReducedSwap()
        {
            return GetViewerFpsModeLevel() >= 12;
        }

        private double GetViewerFpsInitialScale()
        {
            int level = GetViewerFpsModeLevel();
            if (level == 12) { return 0.45; }
            if (level >= 13) { return 0.50; }
            return 0.55;
        }

        private double GetViewerFpsMinScale()
        {
            int level = GetViewerFpsModeLevel();
            if (level == 12) { return 0.32; }
            if (level >= 13) { return 0.30; }
            return 0.55;
        }

        private double GetViewerFpsMaxScale()
        {
            int level = GetViewerFpsModeLevel();
            if (level == 12) { return 0.58; }
            if (level >= 13) { return 0.62; }
            return 0.55;
        }

        private double GetViewerFpsTargetFrameMs()
        {
            int level = GetViewerFpsModeLevel();
            if (level == 12) { return 40.0; }
            if (level >= 13) { return 30.0; }
            return 1000.0 / ViewerDragTargetFps;
        }

        private double GetViewerDragRenderScaleForSelectedMode()
        {
            if (!ViewerFpsUsesAdaptiveScale())
            {
                return 0.55;
            }

            return Math.Max(GetViewerFpsMinScale(), Math.Min(GetViewerFpsMaxScale(), _viewerDragAdaptiveScale));
        }

        private double GetViewerProjectionInteractiveScale()
        {
            if (!_viewerRenderingReducedDragFrame)
            {
                return 1.0;
            }

            return GetViewerDragRenderScaleForSelectedMode();
        }

        private string BuildViewerFpsModeLogFields()
        {
            int level = GetViewerFpsModeLevel();
            return "AppVersion=" + AppBuild.Version +
                "; DragMethodVersion=" + GetViewerFpsModeVersion() +
                "; FpsModeItem=" + GetViewerFpsModeDisplayText() +
                "; DragPipeline=" + GetViewerFpsPipelineId() +
                "; Backbuffer=" + ViewerFpsUsesBackbuffer().ToString() +
                "; PaintThreadRender=" + (level >= 8 && level <= 12).ToString() +
                "; TimerDrivenRender=" + (level >= 13).ToString() +
                "; CursorSampling=" + ViewerFpsUsesCursorSampling().ToString() +
                "; VisibleViewport=" + ViewerFpsUsesVisibleViewport().ToString() +
                "; AdaptiveScale=" + ViewerFpsUsesAdaptiveScale().ToString() +
                "; DirectReducedSwap=" + ViewerFpsUsesDirectReducedSwap().ToString() +
                "; InitialScale=" + GetViewerFpsInitialScale().ToString("0.00", CultureInfo.InvariantCulture) +
                "; MinScale=" + GetViewerFpsMinScale().ToString("0.00", CultureInfo.InvariantCulture) +
                "; MaxScale=" + GetViewerFpsMaxScale().ToString("0.00", CultureInfo.InvariantCulture);
        }

        private void ResetViewerFpsPipelineAfterSelection()
        {
            if (_viewerDragRenderTimer != null)
            {
                _viewerDragRenderTimer.Stop();
                _viewerDragRenderTimer.Interval = ViewerFpsUsesCursorSampling()
                    ? ViewerDragSchedulerIntervalMs
                    : Math.Max(1, 1000 / ViewerDragTargetFps);
            }

            _primitiveViewerDragging = false;
            _primitiveViewerPanning = false;
            if (panelIptLayerPreviewDrawing != null)
            {
                panelIptLayerPreviewDrawing.Capture = false;
            }
            _viewerFrameRendering = false;
            _viewerDragFramePending = false;
            _viewerDragPaintScheduled = false;
            _viewerDragCameraDirty = false;
            _viewerDragCursorSampleReady = false;
            _viewerDragSummaryPending = false;
            _viewerDragAdaptiveScale = GetViewerFpsInitialScale();
            _viewerDragAdaptiveScaleChanges = 0;
            _viewerDragFramesRendered = 0;
            _viewerDragFramesCoalesced = 0;
            _viewerDragFrameMsEma = 0.0;
            _viewerDragFpsEma = 0.0;
            _viewerDragRenderCapacityFps = 0.0;

            if (_viewerBackBufferBitmap != null)
            {
                _viewerBackBufferBitmap.Dispose();
                _viewerBackBufferBitmap = null;
            }
        }

        private System.Drawing.Rectangle GetViewerPaintViewportForSelectedFpsMode(PaintEventArgs e)
        {
            System.Drawing.Rectangle rawClient = panelIptLayerPreviewDrawing == null
                ? new System.Drawing.Rectangle(0, 0, 900, 560)
                : panelIptLayerPreviewDrawing.ClientRectangle;
            int level = GetViewerFpsModeLevel();

            if (level <= 7)
            {
                return GetPrimitiveVisiblePaintBounds(e, rawClient.Width, rawClient.Height);
            }

            if (level >= 11)
            {
                return GetEffectiveViewerViewport(panelIptLayerPreviewDrawing, e);
            }

            return new System.Drawing.Rectangle(rawClient.X, rawClient.Y, Math.Max(10, rawClient.Width), Math.Max(10, rawClient.Height));
        }

        private System.Drawing.Rectangle GetCurrentViewerViewportForSelectedFpsMode()
        {
            if (ViewerFpsUsesVisibleViewport())
            {
                return GetCurrentEffectiveViewerViewport(panelIptLayerPreviewDrawing);
            }

            System.Drawing.Rectangle rawClient = panelIptLayerPreviewDrawing == null
                ? new System.Drawing.Rectangle(0, 0, 900, 560)
                : panelIptLayerPreviewDrawing.ClientRectangle;
            return new System.Drawing.Rectangle(rawClient.X, rawClient.Y, Math.Max(10, rawClient.Width), Math.Max(10, rawClient.Height));
        }

        private static bool IsStepSurfaceMeshMode(string renderMode)
        {
            return string.Equals(renderMode, "SurfaceMesh", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(renderMode, "SurfaceMeshDeepClamp", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(renderMode, "SurfaceMeshTwoLight", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(renderMode, "SurfaceMeshHemisphere", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(renderMode, "SurfaceMeshInventorLab", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(renderMode, "SurfaceMeshOpaqueBody", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(renderMode, "SurfaceMeshSolidFront", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(renderMode, "SurfaceMeshTrueFront", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(renderMode, "SurfaceMeshTrueFrontCompileFix", StringComparison.OrdinalIgnoreCase);
        }

        private static StepRenderMethodProfile GetStepRenderMethodProfile(string renderMode)
        {
            StepRenderMethodProfile profile = new StepRenderMethodProfile();
            profile.MethodId = "SmoothVisibleMeshV0499";
            profile.MethodVersion = "v0.4.99";
            profile.DisplayName = "Surface+Mesh";
            profile.LightModel = "SingleKeyFacingTop";
            profile.SurfacePalette = "InventorDeeperNeutralGray";
            profile.ViewerProfile = "InventorDeeperGrayMesh";
            profile.SurfaceFillMode = "ZBufferDeeperGrayGouraudMesh";
            profile.GdiStrategy = "SoftwareZBufferSplitEdgeBias";
            profile.InputShadeMin = 145;
            profile.InputShadeMax = 214;
            profile.SurfaceShadeMin = 115;
            profile.SurfaceShadeMax = 218;
            profile.MeshLineRgb = 84;
            profile.FeatureLineRgb = 60;
            profile.SilhouetteLineRgb = 40;
            profile.MeshDepthToleranceFactor = 0.0025;
            profile.FeatureDepthToleranceFactor = 0.0032;
            profile.SilhouetteDepthToleranceFactor = 0.0040;
            profile.MeshVisibleSampleRatioThreshold = 0.0;
            profile.MeshDepthNeighborhoodRadius = 0;
            profile.SurfaceAlpha = 255;
            profile.RequireMeshSurfaceOwnerMatch = false;
            profile.MeshOwnerNeighborhoodRadius = 0;
            profile.DepthCueStrength = 0;
            profile.DepthCueGamma = 1.0;
            profile.FrontFacingPositiveViewZ = false;
            profile.RasterizeAllTrianglesForDepth = false;
            profile.Notes = "Smooth visible POLY_LOOP mesh lineage introduced in v0.4.99; current palette retains the v0.5.01 baseline parameters.";

            if (string.Equals(renderMode, "Wire", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(renderMode, "FastDragWireframe", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "WireDiagnostic";
                profile.MethodVersion = "v0.4.84";
                profile.DisplayName = "Wire";
                profile.LightModel = "None";
                profile.ViewerProfile = "WireDiagnostic";
                profile.SurfaceFillMode = "None";
                profile.GdiStrategy = "BatchedTriangleWire";
                profile.Notes = "Diagnostic triangle wireframe; no shaded surface pass.";
            }
            else if (string.Equals(renderMode, "Surface", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "SurfaceZBufferV0496";
                profile.MethodVersion = "v0.4.96";
                profile.DisplayName = "Surface";
                profile.Notes = "v0.5.01 shaded surface without edge overlay.";
            }
            else if (string.Equals(renderMode, "SurfaceEdges", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "SurfaceEdgesZBufferV0496";
                profile.MethodVersion = "v0.4.96";
                profile.DisplayName = "Surface+Edges";
                profile.Notes = "v0.5.01 shaded surface with feature/silhouette edges only.";
            }
            else if (string.Equals(renderMode, "Ghost", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "GhostPainter";
                profile.MethodVersion = "v0.4.84";
                profile.DisplayName = "Ghost";
                profile.LightModel = "GhostConstantAlpha";
                profile.ViewerProfile = "GhostPainter";
                profile.SurfaceFillMode = "PainterAscendingAlpha";
                profile.GdiStrategy = "GdiFillPolygonGhost";
                profile.Notes = "Transparent painter-sorted diagnostic shell.";
            }
            else if (string.Equals(renderMode, "Points", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "PointsOnly";
                profile.MethodVersion = "v0.4.84";
                profile.DisplayName = "Points";
                profile.LightModel = "None";
                profile.ViewerProfile = "PointsOnly";
                profile.SurfaceFillMode = "None";
                profile.GdiStrategy = "GdiPointCloud";
                profile.Notes = "Point-cloud diagnostic mode.";
            }
            else if (string.Equals(renderMode, "SurfaceMeshDeepClamp", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "DeepClamp115_218";
                profile.MethodVersion = "v0.5.02";
                profile.DisplayName = "Mesh Deep Clamp";
                profile.LightModel = "SingleKeyFacingTopCorrectedClamp";
                profile.SurfacePalette = "InventorDeepClampGray";
                profile.ViewerProfile = "RenderLabDeepClamp";
                profile.SurfaceFillMode = "ZBufferGouraudDeepClamp";
                profile.GdiStrategy = "SoftwareZBufferRenderMethodLab";
                profile.InputShadeMin = 115;
                profile.InputShadeMax = 218;
                profile.MeshLineRgb = 88;
                profile.MeshDepthToleranceFactor = 0.0022;
                profile.Notes = "Same v0.5.01 light equation, but removes the accidental 145..214 pre-clamp.";
            }
            else if (string.Equals(renderMode, "SurfaceMeshTwoLight", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "TwoLightKeyFill";
                profile.MethodVersion = "v0.5.02";
                profile.DisplayName = "Mesh Two-Light";
                profile.LightModel = "DirectionalKeyPlusFill";
                profile.SurfacePalette = "InventorTwoLightGray";
                profile.ViewerProfile = "RenderLabTwoLight";
                profile.SurfaceFillMode = "ZBufferGouraudTwoLight";
                profile.GdiStrategy = "SoftwareZBufferRenderMethodLab";
                profile.InputShadeMin = 105;
                profile.InputShadeMax = 224;
                profile.SurfaceShadeMin = 105;
                profile.SurfaceShadeMax = 224;
                profile.MeshLineRgb = 82;
                profile.FeatureLineRgb = 56;
                profile.SilhouetteLineRgb = 36;
                profile.MeshDepthToleranceFactor = 0.0020;
                profile.FeatureDepthToleranceFactor = 0.0030;
                profile.Notes = "Directional key and opposite fill light for deeper inner-bore separation.";
            }
            else if (string.Equals(renderMode, "SurfaceMeshHemisphere", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "HemisphereKey";
                profile.MethodVersion = "v0.5.02";
                profile.DisplayName = "Mesh Hemisphere";
                profile.LightModel = "HemisphereAmbientPlusKey";
                profile.SurfacePalette = "InventorHemisphereGray";
                profile.ViewerProfile = "RenderLabHemisphere";
                profile.SurfaceFillMode = "ZBufferGouraudHemisphere";
                profile.GdiStrategy = "SoftwareZBufferRenderMethodLab";
                profile.InputShadeMin = 108;
                profile.InputShadeMax = 220;
                profile.SurfaceShadeMin = 108;
                profile.SurfaceShadeMax = 220;
                profile.MeshLineRgb = 86;
                profile.FeatureLineRgb = 60;
                profile.SilhouetteLineRgb = 40;
                profile.MeshDepthToleranceFactor = 0.0022;
                profile.Notes = "Sky/ground hemisphere ambient plus key light for broad smooth CAD gradients.";
            }
            else if (string.Equals(renderMode, "SurfaceMeshInventorLab", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "InventorCadFrontVisibilityV0503";
                profile.MethodVersion = "v0.5.03";
                profile.DisplayName = "Mesh Inventor Lab";
                profile.LightModel = "KeyFillFacingTopRimGamma";
                profile.SurfacePalette = "InventorCadLabGray";
                profile.ViewerProfile = "RenderLabInventorCad";
                profile.SurfaceFillMode = "ZBufferGouraudInventorCadLab";
                profile.GdiStrategy = "SoftwareZBufferRenderMethodLab";
                profile.InputShadeMin = 98;
                profile.InputShadeMax = 224;
                profile.SurfaceShadeMin = 98;
                profile.SurfaceShadeMax = 224;
                profile.MeshLineRgb = 78;
                profile.FeatureLineRgb = 50;
                profile.SilhouetteLineRgb = 30;
                profile.MeshDepthToleranceFactor = 0.0012;
                profile.FeatureDepthToleranceFactor = 0.0028;
                profile.SilhouetteDepthToleranceFactor = 0.0042;
                profile.MeshVisibleSampleRatioThreshold = 0.65;
                profile.MeshDepthNeighborhoodRadius = 1;
                profile.GdiStrategy = "SoftwareZBufferVisibilityRatio";
                profile.Notes = "v0.5.03 CAD profile: key/fill/facing/top/rim plus 3x3 front-depth neighborhood and 65% visible-sample ratio gate for ordinary POLY_LOOP mesh edges.";
            }
            else if (string.Equals(renderMode, "SurfaceMeshOpaqueBody", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "OpaqueBodySurfaceOwnerV0504";
                profile.MethodVersion = "v0.5.04";
                profile.DisplayName = "Mesh Opaque Body";
                profile.LightModel = "KeyFillFacingTopRimGammaOpaque";
                profile.SurfacePalette = "InventorOpaqueBodyGray";
                profile.ViewerProfile = "OpaqueBodyMesh";
                profile.SurfaceFillMode = "ZBufferOpaqueOwnerGouraudMesh";
                profile.GdiStrategy = "SoftwareZBufferOpaqueSurfaceOwner";
                profile.InputShadeMin = 98;
                profile.InputShadeMax = 224;
                profile.SurfaceShadeMin = 98;
                profile.SurfaceShadeMax = 224;
                profile.SurfaceAlpha = 255;
                profile.MeshLineRgb = 78;
                profile.FeatureLineRgb = 50;
                profile.SilhouetteLineRgb = 30;
                profile.MeshDepthToleranceFactor = 0.0012;
                profile.FeatureDepthToleranceFactor = 0.0028;
                profile.SilhouetteDepthToleranceFactor = 0.0042;
                profile.MeshVisibleSampleRatioThreshold = 0.65;
                profile.MeshDepthNeighborhoodRadius = 1;
                profile.RequireMeshSurfaceOwnerMatch = true;
                profile.MeshOwnerNeighborhoodRadius = 1;
                profile.Notes = "v0.5.04 opaque body profile: preserves the v0.5.03 visual palette and 65% ratio gate, keeps alpha 255, and requires a front Z-buffer surface-owner match for every ordinary POLY_LOOP mesh sample. Rear/internal mesh cannot pass through an unrelated front shell.";
            }
            else if (string.Equals(renderMode, "SurfaceMeshSolidFront", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "InventorSolidFrontV0505";
                profile.MethodVersion = "v0.5.05";
                profile.DisplayName = "Mesh Solid Front";
                profile.LightModel = "ExplicitKeyFillFacingRimPlusOpaqueDepthCue";
                profile.SurfacePalette = "InventorSolidFrontGray";
                profile.ViewerProfile = "SolidFrontMesh";
                profile.SurfaceFillMode = "ZBufferOpaqueSolidFrontGouraudMesh";
                profile.GdiStrategy = "SoftwareZBufferSolidFrontRatio";
                profile.InputShadeMin = 102;
                profile.InputShadeMax = 218;
                profile.SurfaceShadeMin = 102;
                profile.SurfaceShadeMax = 218;
                profile.SurfaceAlpha = 255;
                profile.MeshLineRgb = 72;
                profile.FeatureLineRgb = 48;
                profile.SilhouetteLineRgb = 28;
                profile.MeshDepthToleranceFactor = 0.0012;
                profile.FeatureDepthToleranceFactor = 0.0028;
                profile.SilhouetteDepthToleranceFactor = 0.0042;
                profile.MeshVisibleSampleRatioThreshold = 0.65;
                profile.MeshDepthNeighborhoodRadius = 1;
                profile.RequireMeshSurfaceOwnerMatch = false;
                profile.MeshOwnerNeighborhoodRadius = 0;
                profile.DepthCueStrength = 12;
                profile.DepthCueGamma = 1.10;
                profile.Notes = "v0.5.05 solid-front fix: alpha stays 255, hard triangle-owner gating is disabled, the proven v0.5.03 65% front-depth ratio mesh is restored, and explicit key/fill/rim shading plus a subtle far-depth darkening separates the inner bore from the outer shell.";
            }
            else if (string.Equals(renderMode, "SurfaceMeshTrueFront", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "TrueFrontFaceZBufferV0506";
                profile.MethodVersion = "v0.5.06";
                profile.DisplayName = "Mesh True Front";
                profile.LightModel = "CameraCorrectPositiveZKeyFillFacing";
                profile.SurfacePalette = "InventorTrueFrontGray";
                profile.ViewerProfile = "TrueFrontFaceMesh";
                profile.SurfaceFillMode = "ZBufferAllTrianglesNearestOpaqueGouraud";
                profile.GdiStrategy = "SoftwareZBufferCameraFacingCullFix";
                profile.InputShadeMin = 165;
                profile.InputShadeMax = 220;
                profile.SurfaceShadeMin = 165;
                profile.SurfaceShadeMax = 220;
                profile.SurfaceAlpha = 255;
                profile.MeshLineRgb = 70;
                profile.FeatureLineRgb = 46;
                profile.SilhouetteLineRgb = 26;
                profile.MeshDepthToleranceFactor = 0.0025;
                profile.FeatureDepthToleranceFactor = 0.0030;
                profile.SilhouetteDepthToleranceFactor = 0.0042;
                profile.MeshVisibleSampleRatioThreshold = 0.55;
                profile.MeshDepthNeighborhoodRadius = 0;
                profile.RequireMeshSurfaceOwnerMatch = false;
                profile.MeshOwnerNeighborhoodRadius = 0;
                profile.DepthCueStrength = 0;
                profile.DepthCueGamma = 1.0;
                profile.FrontFacingPositiveViewZ = true;
                profile.RasterizeAllTrianglesForDepth = true;
                profile.Notes = "v0.5.06 root fix: GreaterViewZIsNear requires camera-facing triangles to use NormalZ > 0. Legacy NormalZ > 0 back-face culling removed the true nearest shell and displayed the far shell with alpha 255. This method corrects facing, rasterizes every triangle into the depth pass, keeps only the nearest opaque surface, and depth-tests POLY_LOOP mesh against that true front surface.";
            }
            else if (string.Equals(renderMode, "SurfaceMeshTrueFrontCompileFix", StringComparison.OrdinalIgnoreCase))
            {
                profile.MethodId = "TrueFrontFaceCompileFixV0507";
                profile.MethodVersion = "v0.5.07";
                profile.DisplayName = "Mesh True Front (compile fix)";
                profile.LightModel = "CameraCorrectPositiveZKeyFillFacing";
                profile.SurfacePalette = "InventorTrueFrontGray";
                profile.ViewerProfile = "TrueFrontFaceMeshCompileFix";
                profile.SurfaceFillMode = "ZBufferAllTrianglesNearestOpaqueGouraud";
                profile.GdiStrategy = "SoftwareZBufferCameraFacingCullFix";
                profile.InputShadeMin = 165;
                profile.InputShadeMax = 220;
                profile.SurfaceShadeMin = 165;
                profile.SurfaceShadeMax = 220;
                profile.SurfaceAlpha = 255;
                profile.MeshLineRgb = 70;
                profile.FeatureLineRgb = 46;
                profile.SilhouetteLineRgb = 26;
                profile.MeshDepthToleranceFactor = 0.0025;
                profile.FeatureDepthToleranceFactor = 0.0030;
                profile.SilhouetteDepthToleranceFactor = 0.0042;
                profile.MeshVisibleSampleRatioThreshold = 0.55;
                profile.MeshDepthNeighborhoodRadius = 0;
                profile.RequireMeshSurfaceOwnerMatch = false;
                profile.MeshOwnerNeighborhoodRadius = 0;
                profile.DepthCueStrength = 0;
                profile.DepthCueGamma = 1.0;
                profile.FrontFacingPositiveViewZ = true;
                profile.RasterizeAllTrianglesForDepth = true;
                profile.Notes = "v0.5.07 source-build profile: identical True Front rendering parameters to v0.5.06. The only historical change was CS0136 compile repair: the branch assigns to the existing brightness local instead of redeclaring it. No FPS/backbuffer pipeline existed yet.";
            }

            return profile;
        }

        private static string BuildStepRenderMethodLogFields(StepRenderMethodProfile profile)
        {
            if (profile == null)
            {
                return "RenderMethod=None";
            }

            return
                "AppVersion=" + AppBuild.Version +
                "; MethodVersion=" + profile.MethodVersion +
                "; RenderMethod=" + profile.MethodId +
                "; DisplayName=" + profile.DisplayName +
                "; LightModel=" + profile.LightModel +
                "; InputShadeClamp=" + profile.InputShadeMin.ToString() + ".." + profile.InputShadeMax.ToString() +
                "; SurfaceShadeClamp=" + profile.SurfaceShadeMin.ToString() + ".." + profile.SurfaceShadeMax.ToString() +
                "; SurfacePalette=" + profile.SurfacePalette +
                "; MeshLineRgb=" + profile.MeshLineRgb.ToString() +
                "; FeatureLineRgb=" + profile.FeatureLineRgb.ToString() +
                "; SilhouetteLineRgb=" + profile.SilhouetteLineRgb.ToString() +
                "; MeshDepthToleranceFactor=" + profile.MeshDepthToleranceFactor.ToString("0.0000", CultureInfo.InvariantCulture) +
                "; FeatureDepthToleranceFactor=" + profile.FeatureDepthToleranceFactor.ToString("0.0000", CultureInfo.InvariantCulture) +
                "; SilhouetteDepthToleranceFactor=" + profile.SilhouetteDepthToleranceFactor.ToString("0.0000", CultureInfo.InvariantCulture) +
                "; MeshVisibleSampleRatioThreshold=" + profile.MeshVisibleSampleRatioThreshold.ToString("0.00", CultureInfo.InvariantCulture) +
                "; MeshDepthNeighborhoodRadius=" + profile.MeshDepthNeighborhoodRadius.ToString() +
                "; SurfaceAlpha=" + profile.SurfaceAlpha.ToString() +
                "; RequireMeshSurfaceOwnerMatch=" + profile.RequireMeshSurfaceOwnerMatch.ToString() +
                "; MeshOwnerNeighborhoodRadius=" + profile.MeshOwnerNeighborhoodRadius.ToString() +
                "; DepthCueStrength=" + profile.DepthCueStrength.ToString() +
                "; DepthCueGamma=" + profile.DepthCueGamma.ToString("0.00", CultureInfo.InvariantCulture) +
                "; FrontFacingPositiveViewZ=" + profile.FrontFacingPositiveViewZ.ToString() +
                "; RasterizeAllTrianglesForDepth=" + profile.RasterizeAllTrianglesForDepth.ToString() +
                "; FrontFaceRule=" + (profile.FrontFacingPositiveViewZ ? "NormalZPositive" : "LegacyNormalZNegative") +
                "; ViewDepthNearRule=GreaterViewZIsNear" +
                "; Notes=" + profile.Notes;
        }

        private bool ShouldRenderViewerSource(string candidate)
        {
            string source = GetViewerSourceMode();

            if (string.Equals(source, candidate, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (!string.Equals(source, "Auto", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (string.Equals(candidate, "STEP", StringComparison.OrdinalIgnoreCase))
            {
                return _stepLitePreviewEnabled && _stepLitePreviewScene != null && _stepLitePreviewScene.PointCount > 0;
            }

            if (string.Equals(candidate, "Mesh", StringComparison.OrdinalIgnoreCase))
            {
                return !_stepLitePreviewEnabled && _meshPreviewEnabled && _meshPreviewScene != null && _meshPreviewScene.BodyCount > 0;
            }

            if (string.Equals(candidate, "BoxGrid", StringComparison.OrdinalIgnoreCase))
            {
                return !_stepLitePreviewEnabled &&
                       !_meshPreviewEnabled &&
                       _spatialCubesIndex != null &&
                       _spatialCubesIndex.IsReady &&
                       _spatialCubesIndex.ModelBox != null;
            }

            return false;
        }

        private System.Drawing.Bitmap CreateLayerButtonIcon(string icon, string caption)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(24, 24);

            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
            using (System.Drawing.Pen dark = new System.Drawing.Pen(System.Drawing.Color.FromArgb(45, 45, 45), 1.6F))
            using (System.Drawing.Pen mid = new System.Drawing.Pen(System.Drawing.Color.FromArgb(90, 90, 90), 1.2F))
            using (System.Drawing.Brush fill = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(230, 240, 255)))
            using (System.Drawing.Brush accent = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(70, 130, 220)))
            using (System.Drawing.Brush green = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(80, 170, 80)))
            using (System.Drawing.Brush orange = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(230, 150, 60)))
            using (System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
            using (System.Drawing.Font font = new System.Drawing.Font("Segoe UI", 6.4F, System.Drawing.FontStyle.Bold))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.Clear(System.Drawing.Color.Transparent);
                string key = (icon ?? string.Empty).Trim().ToUpperInvariant();

                if (key == "ST")
                {
                    g.FillRectangle(fill, 5, 5, 14, 14);
                    g.DrawRectangle(dark, 5, 5, 14, 14);
                    g.DrawLine(mid, 8, 9, 16, 9);
                    g.DrawLine(mid, 8, 13, 16, 13);
                    g.DrawString("S", font, textBrush, 8, 7);
                }
                else if (key == "BT")
                {
                    g.FillRectangle(fill, 4, 4, 16, 16);
                    g.DrawRectangle(dark, 4, 4, 16, 16);
                    g.DrawLine(mid, 7, 8, 17, 8); g.DrawLine(mid, 7, 12, 17, 12); g.DrawLine(mid, 7, 16, 14, 16);
                }
                else if (key == "SP" || key == "8C")
                {
                    for (int yy = 5; yy <= 13; yy += 8)
                    {
                        for (int xx = 5; xx <= 13; xx += 8)
                        {
                            g.FillRectangle(accent, xx, yy, 5, 5);
                            g.DrawRectangle(dark, xx, yy, 5, 5);
                        }
                    }
                    g.DrawString(key == "8C" ? "8" : "S", font, textBrush, 15, 13);
                }
                else if (key == "MG" || key == "MS")
                {
                    g.FillEllipse(accent, 4, 6, 9, 9);
                    g.FillEllipse(green, 11, 9, 9, 9);
                    g.DrawLine(dark, 10, 11, 15, 13);
                }
                else if (key == "FC" || key == "LV")
                {
                    System.Drawing.Point[] bolt = new System.Drawing.Point[] { new System.Drawing.Point(13, 3), new System.Drawing.Point(7, 13), new System.Drawing.Point(12, 13), new System.Drawing.Point(9, 21), new System.Drawing.Point(18, 10), new System.Drawing.Point(13, 10) };
                    g.FillPolygon(orange, bolt);
                    g.DrawPolygon(dark, bolt);
                }
                else if (key == "TR" || key == "LR")
                {
                    g.DrawLine(dark, 6, 5, 6, 19);
                    g.DrawLine(dark, 6, 8, 14, 8); g.DrawLine(dark, 6, 13, 14, 13); g.DrawLine(dark, 6, 18, 14, 18);
                    g.FillRectangle(fill, 14, 5, 6, 6); g.FillRectangle(fill, 14, 10, 6, 6); g.FillRectangle(fill, 14, 15, 6, 6);
                    g.DrawRectangle(dark, 14, 5, 6, 6); g.DrawRectangle(dark, 14, 10, 6, 6); g.DrawRectangle(dark, 14, 15, 6, 6);
                }
                else if (key == "PV" || key == "AV")
                {
                    g.FillRectangle(fill, 4, 6, 16, 12);
                    g.DrawRectangle(dark, 4, 6, 16, 12);
                    g.FillEllipse(accent, 8, 9, 7, 7);
                    g.DrawEllipse(dark, 8, 9, 7, 7);
                }
                else if (key == "DK")
                {
                    g.DrawRectangle(dark, 3, 5, 9, 12);
                    g.DrawRectangle(dark, 12, 7, 9, 12);
                    g.DrawLine(mid, 10, 11, 14, 11); g.DrawLine(mid, 10, 14, 14, 14);
                }
                else if (key == "RF")
                {
                    g.DrawArc(dark, 5, 5, 14, 14, 30, 270);
                    g.FillPolygon(green, new System.Drawing.Point[] { new System.Drawing.Point(17, 4), new System.Drawing.Point(20, 10), new System.Drawing.Point(14, 8) });
                }
                else if (key == "OP")
                {
                    g.FillEllipse(fill, 5, 5, 5, 5);
                    g.FillEllipse(fill, 14, 5, 5, 5);
                    g.FillEllipse(fill, 5, 14, 5, 5);
                    g.FillEllipse(fill, 14, 14, 5, 5);
                    g.DrawEllipse(dark, 5, 5, 5, 5);
                    g.DrawEllipse(dark, 14, 5, 5, 5);
                    g.DrawEllipse(dark, 5, 14, 5, 5);
                    g.DrawEllipse(dark, 14, 14, 5, 5);
                }
                else
                {
                    g.DrawRectangle(dark, 5, 5, 14, 14);
                    g.DrawString(key.Length > 0 ? key.Substring(0, 1) : "?", font, textBrush, 8, 7);
                }
            }

            return bitmap;
        }


private void PanelIptLayerPreviewDrawing_Paint(object sender, PaintEventArgs e)
{
    int fpsLevel = GetViewerFpsModeLevel();
    System.Drawing.Rectangle rawClient = panelIptLayerPreviewDrawing == null
        ? new System.Drawing.Rectangle(0, 0, 900, 560)
        : panelIptLayerPreviewDrawing.ClientRectangle;
    System.Drawing.Rectangle effectiveViewport = GetViewerPaintViewportForSelectedFpsMode(e);
    int width = Math.Max(1, effectiveViewport.Width);
    int height = Math.Max(1, effectiveViewport.Height);

    if (!AppLogger.SuppressHighFrequencyViewerLogs && !_primitiveViewerDragging)
    {
        System.Drawing.RectangleF visibleClip = e.Graphics == null
            ? System.Drawing.RectangleF.Empty
            : e.Graphics.VisibleClipBounds;

        AppLogger.Log(
            "VIEWER_BACKBUFFER_MAPPING",
            "PanelIptLayerPreviewDrawing_Paint",
            "RawClient=" + rawClient.X.ToString() + "," + rawClient.Y.ToString() +
            "," + rawClient.Width.ToString() + "," + rawClient.Height.ToString() +
            "; EffectiveViewport=" + effectiveViewport.X.ToString() + "," + effectiveViewport.Y.ToString() +
            "," + effectiveViewport.Width.ToString() + "," + effectiveViewport.Height.ToString() +
            "; Clip=" + e.ClipRectangle.X.ToString() + "," + e.ClipRectangle.Y.ToString() +
            "," + e.ClipRectangle.Width.ToString() + "," + e.ClipRectangle.Height.ToString() +
            "; VisibleClip=" + visibleClip.X.ToString("0.###", CultureInfo.InvariantCulture) +
            "," + visibleClip.Y.ToString("0.###", CultureInfo.InvariantCulture) +
            "," + visibleClip.Width.ToString("0.###", CultureInfo.InvariantCulture) +
            "," + visibleClip.Height.ToString("0.###", CultureInfo.InvariantCulture) +
            "; PaintDpi=" + e.Graphics.DpiX.ToString("0.###", CultureInfo.InvariantCulture) +
            "x" + e.Graphics.DpiY.ToString("0.###", CultureInfo.InvariantCulture) +
            "; DragMethodVersion=" + GetViewerFpsModeVersion() +
            "; DragPipeline=" + GetViewerFpsPipelineId() +
            "; PresentMode=" + (fpsLevel <= 9 ? "LegacyUnscaled" : (fpsLevel == 10 ? "ExplicitClientRectangle" : "VisibleViewportDestinationRectangle")));
    }

    // v0.5.06/v0.5.07: historical direct paint, no retained viewer backbuffer.
    if (fpsLevel <= 7)
    {
        Stopwatch directStopwatch = Stopwatch.StartNew();
        try
        {
            RenderLayerPreviewFrame(e.Graphics, width, height);
        }
        catch (Exception ex)
        {
            AppLogger.LogException("PanelIptLayerPreviewDrawing_Paint.Direct", ex);
        }
        directStopwatch.Stop();
        _viewerLastQualityFrameMs = directStopwatch.Elapsed.TotalMilliseconds;
        DrawViewerPerformanceOverlay(e.Graphics, effectiveViewport, _primitiveViewerDragging);
        return;
    }

    // v0.5.13: WM_PAINT only presents a completed frame while dragging.
    if (fpsLevel >= 13 && _primitiveViewerDragging && _viewerDraftWhileDragging)
    {
        DrawViewerBackBuffer(e.Graphics, effectiveViewport);
        _viewerDragPaintScheduled = false;
        return;
    }

    if (_viewerFrameRendering)
    {
        DrawViewerBackBuffer(e.Graphics, effectiveViewport);
        return;
    }

    _viewerFrameRendering = true;
    Stopwatch frameStopwatch = Stopwatch.StartNew();
    System.Drawing.Bitmap renderBitmap = null;
    System.Drawing.Bitmap completedBitmap = null;
    bool dragFrame = _primitiveViewerDragging && _viewerDraftWhileDragging;
    double dragRenderScale = dragFrame ? GetViewerDragRenderScaleForSelectedMode() : 1.0;

    try
    {
        int minWidth = fpsLevel <= 11 ? 320 : 280;
        int minHeight = fpsLevel <= 11 ? 220 : 190;
        int renderWidth = dragFrame
            ? Math.Max(minWidth, (int)Math.Round(width * dragRenderScale))
            : width;
        int renderHeight = dragFrame
            ? Math.Max(minHeight, (int)Math.Round(height * dragRenderScale))
            : height;

        renderWidth = Math.Min(width, renderWidth);
        renderHeight = Math.Min(height, renderHeight);

        renderBitmap = fpsLevel >= 10
            ? CreateViewerBitmap(renderWidth, renderHeight, e.Graphics.DpiX, e.Graphics.DpiY)
            : new System.Drawing.Bitmap(renderWidth, renderHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

        AppLogger.SuppressHighFrequencyViewerLogs = dragFrame;
        _viewerRenderingReducedDragFrame = dragFrame && (renderWidth != width || renderHeight != height);

        using (System.Drawing.Graphics frameGraphics = System.Drawing.Graphics.FromImage(renderBitmap))
        {
            if (fpsLevel >= 10)
            {
                ConfigureViewerPixelGraphics(frameGraphics);
            }
            frameGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            frameGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            frameGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            frameGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            RenderLayerPreviewFrame(frameGraphics, renderWidth, renderHeight);
        }

        if (dragFrame && fpsLevel <= 11 && (renderWidth != width || renderHeight != height))
        {
            completedBitmap = fpsLevel >= 10
                ? CreateViewerBitmap(width, height, e.Graphics.DpiX, e.Graphics.DpiY)
                : new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            using (System.Drawing.Graphics displayGraphics = System.Drawing.Graphics.FromImage(completedBitmap))
            {
                if (fpsLevel >= 10)
                {
                    ConfigureViewerPixelGraphics(displayGraphics);
                }
                displayGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                displayGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
                displayGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
                displayGraphics.DrawImage(
                    renderBitmap,
                    new System.Drawing.Rectangle(0, 0, width, height),
                    0,
                    0,
                    renderWidth,
                    renderHeight,
                    System.Drawing.GraphicsUnit.Pixel);
            }

            renderBitmap.Dispose();
            renderBitmap = null;
        }
        else
        {
            completedBitmap = renderBitmap;
            renderBitmap = null;
        }

        frameStopwatch.Stop();
        RecordViewerFrameCompleted(frameStopwatch.Elapsed.TotalMilliseconds, dragFrame, dragRenderScale);
        SwapViewerBackBuffer(completedBitmap, dragFrame, fpsLevel <= 11 ? 1.0 : dragRenderScale);
        completedBitmap = null;
    }
    catch (Exception ex)
    {
        AppLogger.LogException("PanelIptLayerPreviewDrawing_Paint", ex);
    }
    finally
    {
        AppLogger.SuppressHighFrequencyViewerLogs = false;
        _viewerRenderingReducedDragFrame = false;

        if (renderBitmap != null) { renderBitmap.Dispose(); }
        if (completedBitmap != null) { completedBitmap.Dispose(); }

        _viewerFrameRendering = false;
        _viewerDragPaintScheduled = false;
    }

    DrawViewerBackBuffer(e.Graphics, effectiveViewport);
}

        private static System.Drawing.Rectangle GetCurrentEffectiveViewerViewport(System.Windows.Forms.Control control)
        {
            System.Drawing.Rectangle fallback = control == null
                ? new System.Drawing.Rectangle(0, 0, 900, 560)
                : control.ClientRectangle;

            if (control == null || !control.IsHandleCreated)
            {
                return new System.Drawing.Rectangle(
                    fallback.X,
                    fallback.Y,
                    Math.Max(10, fallback.Width),
                    Math.Max(10, fallback.Height));
            }

            try
            {
                System.Drawing.Rectangle visibleScreen = control.RectangleToScreen(control.ClientRectangle);
                System.Windows.Forms.Control ancestor = control.Parent;

                while (ancestor != null)
                {
                    System.Drawing.Rectangle ancestorScreen = ancestor.RectangleToScreen(ancestor.ClientRectangle);
                    visibleScreen = System.Drawing.Rectangle.Intersect(visibleScreen, ancestorScreen);
                    if (visibleScreen.Width <= 0 || visibleScreen.Height <= 0)
                    {
                        break;
                    }

                    ancestor = ancestor.Parent;
                }

                if (visibleScreen.Width > 10 && visibleScreen.Height > 10)
                {
                    System.Drawing.Point localOrigin = control.PointToClient(visibleScreen.Location);
                    return new System.Drawing.Rectangle(
                        localOrigin.X,
                        localOrigin.Y,
                        visibleScreen.Width,
                        visibleScreen.Height);
                }
            }
            catch
            {
                // Use the normalized client rectangle below.
            }

            return new System.Drawing.Rectangle(
                fallback.X,
                fallback.Y,
                Math.Max(10, fallback.Width),
                Math.Max(10, fallback.Height));
        }

        private bool SampleAndApplyViewerDragCursor()
        {
            if (!_primitiveViewerDragging)
            {
                return false;
            }

            System.Drawing.Point currentScreen = System.Windows.Forms.Cursor.Position;
            _viewerDragCursorSamples++;

            if (!_viewerDragCursorSampleReady)
            {
                _viewerDragCursorSampleScreen = currentScreen;
                _viewerDragCursorSampleReady = true;
                return false;
            }

            int dx = currentScreen.X - _viewerDragCursorSampleScreen.X;
            int dy = currentScreen.Y - _viewerDragCursorSampleScreen.Y;
            _viewerDragCursorSampleScreen = currentScreen;

            if (dx == 0 && dy == 0)
            {
                return false;
            }

            if (_primitiveViewerPanning)
            {
                _primitiveViewerPanX += dx;
                _primitiveViewerPanY += dy;
            }
            else
            {
                _primitiveViewerYaw += dx * 0.55;
                _primitiveViewerPitch += dy * 0.45;

                if (_primitiveViewerPitch > 85.0) { _primitiveViewerPitch = 85.0; }
                if (_primitiveViewerPitch < -85.0) { _primitiveViewerPitch = -85.0; }
            }

            _primitiveViewerLastMouse = panelIptLayerPreviewDrawing == null
                ? System.Drawing.Point.Empty
                : panelIptLayerPreviewDrawing.PointToClient(currentScreen);
            _viewerDragAppliedCursorDeltas++;
            _viewerDragCameraDirty = true;
            return true;
        }


private void RenderViewerDragFrameDirect()
{
    if (!ViewerFpsUsesCursorSampling() || !_primitiveViewerDragging || !_viewerDraftWhileDragging ||
        _viewerFrameRendering || panelIptLayerPreviewDrawing == null)
    {
        return;
    }

    System.Drawing.Rectangle effectiveViewport = GetCurrentViewerViewportForSelectedFpsMode();
    int width = Math.Max(1, effectiveViewport.Width);
    int height = Math.Max(1, effectiveViewport.Height);
    double dragRenderScale = GetViewerDragRenderScaleForSelectedMode();
    int renderWidth = Math.Min(width, Math.Max(240, (int)Math.Round(width * dragRenderScale)));
    int renderHeight = Math.Min(height, Math.Max(160, (int)Math.Round(height * dragRenderScale)));

    int pendingEvents = _viewerDragPendingInputEvents;
    if (pendingEvents > 1) { _viewerDragFramesCoalesced += pendingEvents - 1; }
    _viewerDragPendingInputEvents = 0;
    _viewerDragFramePending = false;

    System.Drawing.Bitmap renderBitmap = null;
    System.Drawing.Bitmap completedBitmap = null;
    Stopwatch frameStopwatch = Stopwatch.StartNew();
    _viewerFrameRendering = true;

    try
    {
        renderBitmap = CreateViewerBitmap(renderWidth, renderHeight, 96.0F, 96.0F);
        AppLogger.SuppressHighFrequencyViewerLogs = true;
        _viewerRenderingReducedDragFrame = renderWidth != width || renderHeight != height;

        using (System.Drawing.Graphics frameGraphics = System.Drawing.Graphics.FromImage(renderBitmap))
        {
            ConfigureViewerPixelGraphics(frameGraphics);
            frameGraphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            frameGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            frameGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            frameGraphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;
            RenderLayerPreviewFrame(frameGraphics, renderWidth, renderHeight);
        }

        completedBitmap = renderBitmap;
        renderBitmap = null;
        frameStopwatch.Stop();
        RecordViewerFrameCompleted(frameStopwatch.Elapsed.TotalMilliseconds, true, dragRenderScale);
        SwapViewerBackBuffer(completedBitmap, true, dragRenderScale);
        completedBitmap = null;
        _viewerDragCameraDirty = false;

        _viewerDragPaintScheduled = true;
        panelIptLayerPreviewDrawing.Invalidate(effectiveViewport);
        panelIptLayerPreviewDrawing.Update();
    }
    catch (Exception ex)
    {
        _viewerDragCameraDirty = true;
        AppLogger.LogException("RenderViewerDragFrameDirect", ex);
    }
    finally
    {
        AppLogger.SuppressHighFrequencyViewerLogs = false;
        _viewerRenderingReducedDragFrame = false;
        _viewerFrameRendering = false;
        if (renderBitmap != null) { renderBitmap.Dispose(); }
        if (completedBitmap != null) { completedBitmap.Dispose(); }
    }
}

        private void RenderLayerPreviewFrame(System.Drawing.Graphics g, int width, int height)
        {
            g.Clear(System.Drawing.Color.White);

            using (System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
            using (System.Drawing.Font titleFont = new System.Drawing.Font("Segoe UI", 13.0F, System.Drawing.FontStyle.Bold))
            using (System.Drawing.Font infoFont = new System.Drawing.Font("Segoe UI", 9.0F))
            {
                if (_layerCanvasActiveViewDocked)
                {
                    g.DrawString("Inventor ActiveView is docked here", titleFont, textBrush, 24, 24);
                    g.DrawString("DK Dock mode: true live 3D window via Win32 SetParent.", infoFont, textBrush, 24, 56);
                    return;
                }

                if (_layerCanvasPreviewBitmap != null && !_primitiveViewerDragging)
                {
                    System.Drawing.Rectangle imageRect = CalculateFitRectangle(
                        _layerCanvasPreviewBitmap.Width,
                        _layerCanvasPreviewBitmap.Height,
                        new System.Drawing.Rectangle(12, 12, Math.Max(10, width - 24), Math.Max(10, height - 52)));

                    g.DrawImage(_layerCanvasPreviewBitmap, imageRect);

                    using (System.Drawing.Pen borderPen = new System.Drawing.Pen(System.Drawing.Color.DimGray, 1.0F))
                    {
                        g.DrawRectangle(borderPen, imageRect);
                    }

                    string message = string.IsNullOrWhiteSpace(_layerCanvasPreviewMessage)
                        ? "Inventor ActiveView preview"
                        : _layerCanvasPreviewMessage;

                    g.DrawString(message, infoFont, textBrush, 12, height - 32);
                    g.DrawString("Drag mouse to switch/rotate primitive viewer; double-click resets primitive view.", infoFont, textBrush, 12, height - 16);
                    return;
                }

                DrawPrimitiveModelPreview(g, new System.Drawing.Rectangle(0, 0, width, height));
            }
        }


private void DrawViewerBackBuffer(System.Drawing.Graphics graphics, System.Drawing.Rectangle effectiveViewport)
{
    if (graphics == null)
    {
        return;
    }

    int fpsLevel = GetViewerFpsModeLevel();
    System.Drawing.Rectangle destination = effectiveViewport.Width > 0 && effectiveViewport.Height > 0
        ? effectiveViewport
        : new System.Drawing.Rectangle(0, 0, 1, 1);

    if (fpsLevel >= 10)
    {
        ConfigureViewerPixelGraphics(graphics);
    }

    if (_viewerBackBufferBitmap != null)
    {
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;

        if (fpsLevel <= 9)
        {
            graphics.DrawImageUnscaled(_viewerBackBufferBitmap, destination.Left, destination.Top);
        }
        else
        {
            graphics.InterpolationMode = _viewerBackBufferRenderScale < 0.999
                ? System.Drawing.Drawing2D.InterpolationMode.Bilinear
                : System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            graphics.DrawImage(
                _viewerBackBufferBitmap,
                destination,
                0,
                0,
                _viewerBackBufferBitmap.Width,
                _viewerBackBufferBitmap.Height,
                System.Drawing.GraphicsUnit.Pixel);
        }

        DrawViewerPerformanceOverlay(graphics, destination, _viewerBackBufferDragFrame);
        return;
    }

    using (System.Drawing.Brush clearBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
    {
        graphics.FillRectangle(clearBrush, destination);
    }
}

        private static System.Drawing.Rectangle GetEffectiveViewerViewport(System.Windows.Forms.Control control, PaintEventArgs e)
        {
            System.Drawing.Rectangle fallback = control == null
                ? new System.Drawing.Rectangle(0, 0, 900, 560)
                : control.ClientRectangle;

            if (control == null || !control.IsHandleCreated)
            {
                return NormalizeViewerViewport(fallback, e);
            }

            try
            {
                System.Drawing.Rectangle visibleScreen = control.RectangleToScreen(control.ClientRectangle);
                System.Windows.Forms.Control ancestor = control.Parent;

                while (ancestor != null)
                {
                    System.Drawing.Rectangle ancestorScreen = ancestor.RectangleToScreen(ancestor.ClientRectangle);
                    visibleScreen = System.Drawing.Rectangle.Intersect(visibleScreen, ancestorScreen);
                    if (visibleScreen.Width <= 0 || visibleScreen.Height <= 0)
                    {
                        break;
                    }

                    ancestor = ancestor.Parent;
                }

                if (visibleScreen.Width > 10 && visibleScreen.Height > 10)
                {
                    System.Drawing.Point localOrigin = control.PointToClient(visibleScreen.Location);
                    System.Drawing.Rectangle ancestorVisible = new System.Drawing.Rectangle(
                        localOrigin.X,
                        localOrigin.Y,
                        visibleScreen.Width,
                        visibleScreen.Height);
                    return ConstrainViewerViewportByPaintExtent(ancestorVisible, e);
                }
            }
            catch
            {
                // Fall through to the Paint clip/client fallback.
            }

            return NormalizeViewerViewport(fallback, e);
        }


        private static System.Drawing.Rectangle ConstrainViewerViewportByPaintExtent(
            System.Drawing.Rectangle candidate,
            PaintEventArgs e)
        {
            if (e == null)
            {
                return candidate;
            }

            System.Drawing.Rectangle clip = e.ClipRectangle;

            // Full viewer invalidations in this application start at the local origin.
            // Use that full visible Paint extent when WinForms reports a much larger hidden ClientSize.
            // Tiny/offset partial invalidations are intentionally ignored so the camera center never jumps.
            bool fullOriginPaint = clip.X == candidate.X && clip.Y == candidate.Y &&
                clip.Width >= 320 && clip.Height >= 220;
            bool substantiallySmaller = clip.Width < candidate.Width * 0.90 ||
                clip.Height < candidate.Height * 0.90;

            if (fullOriginPaint && substantiallySmaller)
            {
                System.Drawing.Rectangle constrained = System.Drawing.Rectangle.Intersect(candidate, clip);
                if (constrained.Width > 10 && constrained.Height > 10)
                {
                    return constrained;
                }
            }

            return candidate;
        }

        private static System.Drawing.Rectangle NormalizeViewerViewport(System.Drawing.Rectangle fallback, PaintEventArgs e)
        {
            System.Drawing.Rectangle clip = e == null ? System.Drawing.Rectangle.Empty : e.ClipRectangle;
            if (clip.Width > 10 && clip.Height > 10 &&
                (fallback.Width <= 10 || fallback.Height <= 10 || clip.Width < fallback.Width || clip.Height < fallback.Height))
            {
                return clip;
            }

            return new System.Drawing.Rectangle(
                fallback.X,
                fallback.Y,
                Math.Max(10, fallback.Width),
                Math.Max(10, fallback.Height));
        }

        private static System.Drawing.Bitmap CreateViewerBitmap(int width, int height, float dpiX, float dpiY)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
                Math.Max(1, width),
                Math.Max(1, height),
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            float safeDpiX = dpiX > 1.0F && !float.IsNaN(dpiX) && !float.IsInfinity(dpiX) ? dpiX : 96.0F;
            float safeDpiY = dpiY > 1.0F && !float.IsNaN(dpiY) && !float.IsInfinity(dpiY) ? dpiY : 96.0F;
            bitmap.SetResolution(safeDpiX, safeDpiY);
            return bitmap;
        }

        private static void ConfigureViewerPixelGraphics(System.Drawing.Graphics graphics)
        {
            if (graphics == null)
            {
                return;
            }

            graphics.PageUnit = System.Drawing.GraphicsUnit.Pixel;
            graphics.PageScale = 1.0F;
        }


private void SwapViewerBackBuffer(System.Drawing.Bitmap completedBitmap, bool dragFrame, double renderScale)
{
    if (completedBitmap == null)
    {
        return;
    }

    System.Drawing.Bitmap oldBitmap = _viewerBackBufferBitmap;
    _viewerBackBufferBitmap = completedBitmap;
    _viewerBackBufferDragFrame = dragFrame;
    _viewerBackBufferRenderScale = dragFrame && ViewerFpsUsesDirectReducedSwap()
        ? Math.Max(GetViewerFpsMinScale(), Math.Min(1.0, renderScale))
        : 1.0;

    if (oldBitmap != null) { oldBitmap.Dispose(); }
}


private void DrawViewerPerformanceOverlay(System.Drawing.Graphics overlayGraphics, System.Drawing.Rectangle viewport, bool dragFrame)
{
    if (overlayGraphics == null || viewport.Width <= 0 || viewport.Height <= 0)
    {
        return;
    }

    int fpsLevel = GetViewerFpsModeLevel();
    string firstLine;
    string secondLine;
    string thirdLine = GetViewerFpsModeVersion() + "  " + GetViewerFpsPipelineId();

    if (fpsLevel <= 7)
    {
        firstLine = "DIRECT " + _viewerLastQualityFrameMs.ToString("0.0", CultureInfo.InvariantCulture) + " ms";
        secondLine = dragFrame ? "Legacy immediate paint" : "No retained backbuffer";
    }
    else if (dragFrame && fpsLevel >= 13)
    {
        firstLine = "Present " + _viewerDragFpsEma.ToString("0.0", CultureInfo.InvariantCulture) +
            "   Render " + _viewerDragRenderCapacityFps.ToString("0.0", CultureInfo.InvariantCulture) + " FPS";
        secondLine = "Frame " + _viewerDragFrameMsEma.ToString("0.0", CultureInfo.InvariantCulture) +
            " ms   Scale " + (_viewerBackBufferRenderScale * 100.0).ToString("0", CultureInfo.InvariantCulture) +
            "%   Drop " + _viewerDragFramesCoalesced.ToString(CultureInfo.InvariantCulture);
    }
    else if (dragFrame)
    {
        firstLine = "FPS " + _viewerDragFpsEma.ToString("0.0", CultureInfo.InvariantCulture) +
            "   Frame " + _viewerDragFrameMsEma.ToString("0.0", CultureInfo.InvariantCulture) + " ms";
        secondLine = (ViewerFpsUsesAdaptiveScale()
            ? "Adaptive " + (_viewerBackBufferRenderScale * 100.0).ToString("0", CultureInfo.InvariantCulture) + "%"
            : "Fixed 55%") + "   Drop " + _viewerDragFramesCoalesced.ToString(CultureInfo.InvariantCulture);
    }
    else
    {
        firstLine = "FULL " + _viewerLastQualityFrameMs.ToString("0.0", CultureInfo.InvariantCulture) + " ms";
        secondLine = _viewerDragFramesRendered > 0
            ? "Last P " + _viewerDragFpsEma.ToString("0.0", CultureInfo.InvariantCulture) +
              " / R " + _viewerDragRenderCapacityFps.ToString("0.0", CultureInfo.InvariantCulture) + " FPS"
            : (ViewerFpsUsesBackbuffer() ? "Backbuffer ready" : "Direct paint ready");
    }

    using (System.Drawing.Font overlayFont = new System.Drawing.Font("Segoe UI", 9.0F, System.Drawing.FontStyle.Bold))
    using (System.Drawing.Font smallFont = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Regular))
    using (System.Drawing.Brush backgroundBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(178, 25, 31, 39)))
    using (System.Drawing.Brush foregroundBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
    {
        System.Drawing.SizeF firstSize = overlayGraphics.MeasureString(firstLine, overlayFont);
        System.Drawing.SizeF secondSize = overlayGraphics.MeasureString(secondLine, overlayFont);
        System.Drawing.SizeF thirdSize = overlayGraphics.MeasureString(thirdLine, smallFont);
        int boxWidth = Math.Max(230, (int)Math.Ceiling(Math.Max(Math.Max(firstSize.Width, secondSize.Width), thirdSize.Width)) + 22);
        int boxHeight = 65;
        int x = Math.Max(viewport.Left + 8, viewport.Right - boxWidth - 10);
        int y = viewport.Top + 10;
        System.Drawing.Rectangle box = new System.Drawing.Rectangle(x, y, boxWidth, boxHeight);

        overlayGraphics.FillRectangle(backgroundBrush, box);
        overlayGraphics.DrawString(firstLine, overlayFont, foregroundBrush, x + 10, y + 5);
        overlayGraphics.DrawString(secondLine, overlayFont, foregroundBrush, x + 10, y + 24);
        overlayGraphics.DrawString(thirdLine, smallFont, foregroundBrush, x + 10, y + 45);
    }
}


private void RecordViewerFrameCompleted(double frameMilliseconds, bool dragFrame, double frameRenderScale)
{
    long now = Stopwatch.GetTimestamp();
    if (dragFrame)
    {
        _viewerDragFramesRendered++;
        _viewerDragFrameMsEma = _viewerDragFrameMsEma <= 0.0
            ? frameMilliseconds
            : (_viewerDragFrameMsEma * 0.80) + (frameMilliseconds * 0.20);
        _viewerDragRenderCapacityFps = _viewerDragFrameMsEma > 0.01 ? 1000.0 / _viewerDragFrameMsEma : 0.0;

        if (_viewerDragLastFrameCompletedTimestamp > 0)
        {
            double intervalMilliseconds = (now - _viewerDragLastFrameCompletedTimestamp) * 1000.0 / Stopwatch.Frequency;
            if (intervalMilliseconds > 0.01)
            {
                double activeGapLimitMs = Math.Max(120.0, GetViewerFpsTargetFrameMs() * 4.0);
                if (intervalMilliseconds <= activeGapLimitMs)
                {
                    double instantFps = 1000.0 / intervalMilliseconds;
                    _viewerDragFpsEma = _viewerDragFpsEma <= 0.0
                        ? instantFps
                        : (_viewerDragFpsEma * 0.70) + (instantFps * 0.30);
                }
                else
                {
                    _viewerDragFpsEma = _viewerDragRenderCapacityFps;
                }
            }
        }

        _viewerDragLastFrameCompletedTimestamp = now;
        UpdateViewerAdaptiveDragScale(frameRenderScale);

        if (_viewerDragFramesRendered == 1 || (_viewerDragFramesRendered % 15) == 0)
        {
            AppLogger.Log(
                "DRAG_FRAME_STATS",
                "RecordViewerFrameCompleted",
                "Frame=" + _viewerDragFramesRendered.ToString() +
                "; FrameMs=" + frameMilliseconds.ToString("0.000", CultureInfo.InvariantCulture) +
                "; EmaFrameMs=" + _viewerDragFrameMsEma.ToString("0.000", CultureInfo.InvariantCulture) +
                "; PresentFps=" + _viewerDragFpsEma.ToString("0.00", CultureInfo.InvariantCulture) +
                "; RenderCapacityFps=" + _viewerDragRenderCapacityFps.ToString("0.00", CultureInfo.InvariantCulture) +
                "; TargetFps=" + ViewerDragTargetFps.ToString() +
                "; RenderScale=" + frameRenderScale.ToString("0.00", CultureInfo.InvariantCulture) +
                "; NextRenderScale=" + _viewerDragAdaptiveScale.ToString("0.00", CultureInfo.InvariantCulture) +
                "; ScaleChanges=" + _viewerDragAdaptiveScaleChanges.ToString() +
                "; InputEvents=" + _viewerDragInputEvents.ToString() +
                "; CursorSamples=" + _viewerDragCursorSamples.ToString() +
                "; AppliedCursorDeltas=" + _viewerDragAppliedCursorDeltas.ToString() +
                "; IdleTimerTicks=" + _viewerDragIdleTimerTicks.ToString() +
                "; CoalescedEvents=" + _viewerDragFramesCoalesced.ToString() + "; " + BuildViewerFpsModeLogFields());
        }
    }
    else
    {
        _viewerLastQualityFrameMs = frameMilliseconds;
        if (_viewerDragSummaryPending)
        {
            double sessionMilliseconds = _viewerDragSessionStartTimestamp <= 0
                ? 0.0
                : (now - _viewerDragSessionStartTimestamp) * 1000.0 / Stopwatch.Frequency;

            AppLogger.Log(
                "DRAG_SESSION_SUMMARY",
                "RecordViewerFrameCompleted",
                "DurationMs=" + sessionMilliseconds.ToString("0.000", CultureInfo.InvariantCulture) +
                "; InputEvents=" + _viewerDragInputEvents.ToString() +
                "; FramesRendered=" + _viewerDragFramesRendered.ToString() +
                "; CoalescedEvents=" + _viewerDragFramesCoalesced.ToString() +
                "; AveragePresentFps=" + _viewerDragFpsEma.ToString("0.00", CultureInfo.InvariantCulture) +
                "; RenderCapacityFps=" + _viewerDragRenderCapacityFps.ToString("0.00", CultureInfo.InvariantCulture) +
                "; AverageFrameMs=" + _viewerDragFrameMsEma.ToString("0.000", CultureInfo.InvariantCulture) +
                "; FinalQualityRenderMs=" + frameMilliseconds.ToString("0.000", CultureInfo.InvariantCulture) +
                "; FinalAdaptiveScale=" + _viewerDragAdaptiveScale.ToString("0.00", CultureInfo.InvariantCulture) +
                "; ScaleChanges=" + _viewerDragAdaptiveScaleChanges.ToString() +
                "; CursorSamples=" + _viewerDragCursorSamples.ToString() +
                "; AppliedCursorDeltas=" + _viewerDragAppliedCursorDeltas.ToString() +
                "; IdleTimerTicks=" + _viewerDragIdleTimerTicks.ToString() +
                "; FinalMode=" + GetViewerRenderMode() + "; " + BuildViewerFpsModeLogFields());
            _viewerDragSummaryPending = false;
        }
    }
}



private void UpdateViewerAdaptiveDragScale(double frameRenderScale)
{
    if (!ViewerFpsUsesAdaptiveScale() || _viewerDragFramesRendered < 4 ||
        (_viewerDragFramesRendered % 4) != 0 || _viewerDragFrameMsEma <= 0.01)
    {
        return;
    }

    double minScale = GetViewerFpsMinScale();
    double maxScale = GetViewerFpsMaxScale();
    double targetFrameMs = GetViewerFpsTargetFrameMs();
    double currentScale = Math.Max(minScale, Math.Min(maxScale, _viewerDragAdaptiveScale));
    double desiredScale = currentScale * Math.Sqrt(targetFrameMs / _viewerDragFrameMsEma);
    desiredScale = Math.Max(minScale, Math.Min(maxScale, desiredScale));
    double minimumStepScale = Math.Max(minScale, currentScale - 0.055);
    double maximumStepScale = Math.Min(maxScale, currentScale + 0.025);
    desiredScale = Math.Max(minimumStepScale, Math.Min(maximumStepScale, desiredScale));
    double blend = desiredScale < currentScale ? 0.62 : 0.34;
    double nextScale = currentScale + (desiredScale - currentScale) * blend;
    nextScale = Math.Round(nextScale * 100.0) / 100.0;
    nextScale = Math.Max(minScale, Math.Min(maxScale, nextScale));
    if (Math.Abs(nextScale - currentScale) < 0.009) { return; }

    _viewerDragAdaptiveScale = nextScale;
    _viewerDragAdaptiveScaleChanges++;
    AppLogger.Log(
        "DRAG_ADAPTIVE_SCALE",
        "UpdateViewerAdaptiveDragScale",
        "Frame=" + _viewerDragFramesRendered.ToString() +
        "; FrameRenderScale=" + frameRenderScale.ToString("0.00", CultureInfo.InvariantCulture) +
        "; PreviousScale=" + currentScale.ToString("0.00", CultureInfo.InvariantCulture) +
        "; NextScale=" + nextScale.ToString("0.00", CultureInfo.InvariantCulture) +
        "; EmaFrameMs=" + _viewerDragFrameMsEma.ToString("0.000", CultureInfo.InvariantCulture) +
        "; TargetFrameMs=" + targetFrameMs.ToString("0.000", CultureInfo.InvariantCulture) +
        "; MinScale=" + minScale.ToString("0.00", CultureInfo.InvariantCulture) +
        "; MaxScale=" + maxScale.ToString("0.00", CultureInfo.InvariantCulture) +
        "; DragMethodVersion=" + GetViewerFpsModeVersion());
}

        private static System.Drawing.Rectangle GetPrimitiveVisiblePaintBounds(PaintEventArgs e, int fallbackWidth, int fallbackHeight)
        {
            System.Drawing.Rectangle clip = e == null ? System.Drawing.Rectangle.Empty : e.ClipRectangle;

            if (clip.Width <= 10 || clip.Height <= 10)
            {
                return new System.Drawing.Rectangle(0, 0, Math.Max(10, fallbackWidth), Math.Max(10, fallbackHeight));
            }

            return clip;
        }

        private void DrawPrimitiveModelPreview(System.Drawing.Graphics g, System.Drawing.Rectangle bounds)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            using (System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
            using (System.Drawing.Brush softTextBrush = new System.Drawing.SolidBrush(System.Drawing.Color.DimGray))
            using (System.Drawing.Font titleFont = new System.Drawing.Font("Segoe UI", 11.0F, System.Drawing.FontStyle.Bold))
            using (System.Drawing.Font infoFont = new System.Drawing.Font("Segoe UI", 8.5F))
            using (System.Drawing.Pen modelPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(80, 80, 80), 1.2F))
            using (System.Drawing.Pen cubePen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(120, 170, 220), 0.9F))
            using (System.Drawing.Pen bodyPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(40, 40, 40), 0.8F))
            {
                string status = (_spatialCubesIndex != null && _spatialCubesIndex.IsReady)
                    ? "Primitive 3D: " + _spatialCubesIndex.Bodies.Count.ToString() + " body RangeBoxes / " + _spatialCubesIndex.Cells.Count.ToString() + " cubes"
                    : "Primitive 3D: Spatial BASE is OFF";

                if (!_viewerRenderingReducedDragFrame)
                {
                    g.DrawString("Local primitive 3D viewer", titleFont, textBrush, 16, 14);
                    g.DrawString(status + " | LMB rotate | Shift+LMB/RMB pan | Wheel zoom | Double-click reset", infoFont, softTextBrush, 16, 40);
                }

                if (ShouldRenderViewerSource("STEP") && _stepLitePreviewScene != null && _stepLitePreviewScene.PointCount > 0)
                {
                    DrawStepLiteScenePreview(g, bounds);
                    return;
                }

                if (ShouldRenderViewerSource("Mesh") && _meshPreviewScene != null && _meshPreviewScene.BodyCount > 0)
                {
                    DrawMeshScenePreview(g, bounds);
                    return;
                }

                if (!ShouldRenderViewerSource("BoxGrid"))
                {
                    DrawPrimitivePlaceholder(g, bounds);
                    return;
                }

                if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady || _spatialCubesIndex.ModelBox == null)
                {
                    DrawPrimitivePlaceholder(g, bounds);
                    return;
                }

                SpatialBox modelBox = _spatialCubesIndex.ModelBox;
                double cx = (modelBox.MinX + modelBox.MaxX) * 0.5;
                double cy = (modelBox.MinY + modelBox.MaxY) * 0.5;
                double cz = (modelBox.MinZ + modelBox.MaxZ) * 0.5;
                double size = Math.Max(modelBox.SizeX, Math.Max(modelBox.SizeY, modelBox.SizeZ));

                if (size <= 0.000001)
                {
                    size = 1.0;
                }

                PreparePrimitiveProjectionFit(bounds, cx, cy, cz, size);

                DrawPrimitiveBox(g, modelBox, modelPen, bounds, cx, cy, cz, size, true);

                if (_spatialCubesIndex.Cells != null)
                {
                    foreach (SpatialCubeCell cell in _spatialCubesIndex.Cells)
                    {
                        if (cell != null)
                        {
                            DrawPrimitiveBox(g, cell.Bounds, cubePen, bounds, cx, cy, cz, size, false);
                        }
                    }
                }

                if (_spatialCubesIndex.Bodies != null)
                {
                    int bodyIndex = 0;

                    foreach (SpatialBodyRecord body in _spatialCubesIndex.Bodies)
                    {
                        bodyIndex++;

                        if (body == null || body.BodyBox == null)
                        {
                            continue;
                        }

                        bodyPen.Color = GetPrimitiveBodyColor(bodyIndex);
                        DrawPrimitiveBox(g, body.BodyBox, bodyPen, bounds, cx, cy, cz, size, false);
                    }
                }
            }
        }

        private void DrawStepLiteScenePreview(System.Drawing.Graphics g, System.Drawing.Rectangle bounds)
        {
            Stopwatch drawSw = Stopwatch.StartNew();

            using (System.Drawing.Drawing2D.LinearGradientBrush backgroundBrush =
                new System.Drawing.Drawing2D.LinearGradientBrush(
                    bounds,
                    System.Drawing.Color.FromArgb(186, 201, 222),
                    System.Drawing.Color.FromArgb(235, 241, 249),
                    90.0F))
            {
                g.FillRectangle(backgroundBrush, bounds);
            }

            if (_stepLitePreviewScene == null || _stepLitePreviewScene.Bounds == null || !_stepLitePreviewScene.Bounds.IsValid)
            {
                DrawPrimitivePlaceholder(g, bounds);
                return;
            }

            bool fastDragMode = _primitiveViewerDragging && _viewerDraftWhileDragging;
            string requestedRenderMode = GetViewerRenderMode();
            string renderMode = fastDragMode ? "FastDragSurface" : requestedRenderMode;
            StepRenderMethodProfile requestedRenderProfile = GetStepRenderMethodProfile(requestedRenderMode);

            g.SmoothingMode = fastDragMode
                ? System.Drawing.Drawing2D.SmoothingMode.None
                : System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighSpeed;

            if (!_viewerRenderingReducedDragFrame)
            {
                using (System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
                using (System.Drawing.Brush softTextBrush = new System.Drawing.SolidBrush(System.Drawing.Color.DimGray))
                using (System.Drawing.Font titleFont = new System.Drawing.Font("Segoe UI", 11.0F, System.Drawing.FontStyle.Bold))
                using (System.Drawing.Font infoFont = new System.Drawing.Font("Segoe UI", 8.5F))
                {
                    g.DrawString("Local STEP render-method laboratory viewer", titleFont, textBrush, 16, 14);
                    g.DrawString(
                        "Points=" + _stepLitePreviewScene.PointCount.ToString() +
                        " | Edges=" + _stepLitePreviewScene.EdgeCount.ToString() +
                        " | Triangles=" + _stepLitePreviewScene.TriangleCount.ToString() +
                        " | PolyLoops=" + _stepLitePreviewScene.PolyLoopEntityCount.ToString() +
                        " | Mode=" + renderMode +
                        " | Method=" + requestedRenderProfile.MethodId +
                        " (" + requestedRenderProfile.MethodVersion + ")" +
                        " | LMB rotate | Shift+LMB/RMB pan | Wheel zoom | Double-click reset",
                        infoFont,
                        softTextBrush,
                        16,
                        40);
                }
            }

            StepBox3 box = _stepLitePreviewScene.Bounds;
            double cx = (box.MinX + box.MaxX) * 0.5;
            double cy = (box.MinY + box.MaxY) * 0.5;
            double cz = (box.MinZ + box.MaxZ) * 0.5;
            double size = Math.Max(box.SizeX, Math.Max(box.SizeY, box.SizeZ));

            if (size <= 0.000001)
            {
                size = 1.0;
            }

            PrepareStepLiteProjectionFit(bounds, cx, cy, cz);

            System.Drawing.PointF[] projected = new System.Drawing.PointF[_stepLitePreviewScene.PointCount];
            double[] projectedDepths = new double[_stepLitePreviewScene.PointCount];

            for (int i = 0; i < _stepLitePreviewScene.Points.Count; i++)
            {
                StepPoint3 point = _stepLitePreviewScene.Points[i];
                projected[i] = ProjectPrimitivePoint(point.X, point.Y, point.Z, bounds, cx, cy, cz, size);

                double viewX;
                double viewY;
                double viewZ;
                ProjectPrimitivePointView(point.X, point.Y, point.Z, cx, cy, cz, out viewX, out viewY, out viewZ);
                projectedDepths[i] = viewZ;
            }

            int drawnTriangles = 0;
            int drawnEdges = 0;

            bool modePoints = string.Equals(renderMode, "Points", StringComparison.OrdinalIgnoreCase);
            bool modeWire = string.Equals(renderMode, "Wire", StringComparison.OrdinalIgnoreCase) ||
                            string.Equals(renderMode, "FastDragWireframe", StringComparison.OrdinalIgnoreCase);
            bool modeSurface = string.Equals(renderMode, "Surface", StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(renderMode, "FastDragSurface", StringComparison.OrdinalIgnoreCase);
            bool modeSurfaceEdges = string.Equals(renderMode, "SurfaceEdges", StringComparison.OrdinalIgnoreCase);
            bool modeSurfaceMesh = IsStepSurfaceMeshMode(renderMode);
            bool modeGhost = string.Equals(renderMode, "Ghost", StringComparison.OrdinalIgnoreCase);
            StepRenderMethodProfile renderProfile = requestedRenderProfile;

            if (_stepLitePreviewScene.TriangleCount > 0 && !modePoints)
            {
                if (modeWire)
                {
                    int maxFastEdges = fastDragMode ? 4200 : 90000;
                    int step = Math.Max(1, _stepLitePreviewScene.TriangleCount / maxFastEdges);

                    using (System.Drawing.Drawing2D.GraphicsPath edgePath = new System.Drawing.Drawing2D.GraphicsPath())
                    using (System.Drawing.Pen edgePen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(155, 70, 70, 70), fastDragMode ? 0.85F : 0.65F))
                    {
                        for (int i = 0; i < _stepLitePreviewScene.Triangles.Count; i += step)
                        {
                            StepTriangle triangle = _stepLitePreviewScene.Triangles[i];

                            if (!IsValidStepTriangle(triangle, projected.Length))
                            {
                                continue;
                            }

                            AddTriangleWireToPath(edgePath, projected, triangle);
                            drawnTriangles++;
                            drawnEdges += 3;
                        }

                        g.DrawPath(edgePen, edgePath);
                    }
                }
                else
                {
                    bool drawEdges = modeSurfaceEdges || modeSurfaceMesh || modeGhost || fastDragMode;
                    bool visibleShellMode = modeSurface || modeSurfaceEdges || modeSurfaceMesh;
                    int backFacingTriangles = 0;
                    int degenerateTriangles = 0;
                    int culledBackFacingTriangles = 0;
                    int smoothVertexKeys = 0;

                    List<StepSurfaceDrawTriangle> allSurfaceTriangles =
                        BuildStepSurfaceDrawTriangles(
                            projected,
                            cx,
                            cy,
                            cz,
                            renderProfile,
                            out backFacingTriangles,
                            out degenerateTriangles,
                            out smoothVertexKeys);

                    List<StepSurfaceDrawTriangle> surfaceTriangles = allSurfaceTriangles;

                    if (visibleShellMode)
                    {
                        List<StepSurfaceDrawTriangle> frontFacingTriangles = new List<StepSurfaceDrawTriangle>();

                        foreach (StepSurfaceDrawTriangle drawTriangle in allSurfaceTriangles)
                        {
                            if (drawTriangle == null)
                            {
                                continue;
                            }

                            if (drawTriangle.BackFacing)
                            {
                                culledBackFacingTriangles++;
                                continue;
                            }

                            frontFacingTriangles.Add(drawTriangle);
                        }

                        if (frontFacingTriangles.Count > Math.Max(16, allSurfaceTriangles.Count / 10))
                        {
                            surfaceTriangles = frontFacingTriangles;
                        }
                        else
                        {
                            culledBackFacingTriangles = 0;
                        }
                    }

                    List<StepSurfaceDrawTriangle> depthSurfaceTriangles =
                        renderProfile.RasterizeAllTrianglesForDepth
                            ? allSurfaceTriangles
                            : surfaceTriangles;

                    if (visibleShellMode)
                    {
                        int silhouetteEdges = 0;
                        int boundaryEdges = 0;
                        int creaseEdges = 0;

                        int meshEdges = 0;
                        List<StepVisibleEdgeSegment> edgeSegments;

                        int silhouetteOverlayEdges = 0;

                        if (modeSurfaceMesh)
                        {
                            edgeSegments = BuildStepVisibleMeshEdgeSegments(surfaceTriangles, out meshEdges);

                            int overlaySilhouetteEdges;
                            int overlayBoundaryEdges;
                            int overlayCreaseEdges;
                            List<StepVisibleEdgeSegment> featureOverlay =
                                BuildStepVisibleEdgeSegments(
                                    allSurfaceTriangles,
                                    out overlaySilhouetteEdges,
                                    out overlayBoundaryEdges,
                                    out overlayCreaseEdges);

                            foreach (StepVisibleEdgeSegment overlay in featureOverlay)
                            {
                                if (overlay != null && overlay.Style == 2)
                                {
                                    edgeSegments.Add(overlay);
                                    silhouetteOverlayEdges++;
                                }
                            }

                            silhouetteEdges = overlaySilhouetteEdges;
                            boundaryEdges = overlayBoundaryEdges;
                            creaseEdges = overlayCreaseEdges;
                        }
                        else if (drawEdges)
                        {
                            edgeSegments = BuildStepVisibleEdgeSegments(
                                allSurfaceTriangles,
                                out silhouetteEdges,
                                out boundaryEdges,
                                out creaseEdges);
                        }
                        else
                        {
                            edgeSegments = new List<StepVisibleEdgeSegment>();
                        }

                        Stopwatch zBufferSw = Stopwatch.StartNew();

                        StepZBufferFrame zFrame = RenderStepZBufferSurface(
                            depthSurfaceTriangles,
                            projected,
                            projectedDepths,
                            bounds,
                            edgeSegments,
                            size,
                            drawEdges,
                            renderProfile);

                        zBufferSw.Stop();

                        DrawStepZBufferFrame(g, bounds, zFrame);

                        drawnTriangles = depthSurfaceTriangles.Count;
                        drawnEdges = zFrame == null ? 0 : zFrame.VisibleEdges;

                        AppLogger.Log(
                            "STEP_ZBUFFER_SURFACE_STATS",
                            "DrawStepLiteScenePreview",
                            "Width=" + (zFrame == null ? 0 : zFrame.Width).ToString() +
                            "; Height=" + (zFrame == null ? 0 : zFrame.Height).ToString() +
                            "; SurfaceTriangles=" + depthSurfaceTriangles.Count.ToString() +
                            "; FrontFacingTriangles=" + surfaceTriangles.Count.ToString() +
                            "; DepthPassTriangles=" + depthSurfaceTriangles.Count.ToString() +
                            "; SurfacePixelsWritten=" + (zFrame == null ? 0 : zFrame.SurfacePixelsWritten).ToString() +
                            "; SurfacePixelsRejectedByDepth=" + (zFrame == null ? 0L : zFrame.SurfacePixelsRejectedByDepth).ToString() +
                            "; SmoothVertexKeys=" + smoothVertexKeys.ToString() +
                            "; ShadeInterpolation=GouraudBarycentric" +
                            "; NormalMerge=Quantized3DCoordinates" +
                            "; NormalCreaseDotThreshold=0.72" +
                            "; AppVersion=" + AppBuild.Version +
                            "; MethodVersion=" + renderProfile.MethodVersion +
                            "; RenderMethod=" + renderProfile.MethodId +
                            "; LightModel=" + renderProfile.LightModel +
                            "; InputShadeClamp=" + renderProfile.InputShadeMin.ToString() + ".." + renderProfile.InputShadeMax.ToString() +
                            "; SurfacePalette=" + renderProfile.SurfacePalette +
                            "; SurfaceShadeClamp=" + renderProfile.SurfaceShadeMin.ToString() + ".." + renderProfile.SurfaceShadeMax.ToString() +
                            "; SurfaceAlpha=" + renderProfile.SurfaceAlpha.ToString() +
                            "; OpaqueBody=" + (renderProfile.SurfaceAlpha >= 255).ToString() +
                            "; OpaqueSurfacePixels=" + (zFrame == null ? 0 : zFrame.OpaqueSurfacePixelCount).ToString() +
                            "; NonOpaqueSurfacePixels=" + (zFrame == null ? 0 : zFrame.NonOpaqueSurfacePixelCount).ToString() +
                            "; DepthCueStrength=" + renderProfile.DepthCueStrength.ToString() +
                            "; DepthCueGamma=" + renderProfile.DepthCueGamma.ToString("0.00", CultureInfo.InvariantCulture) +
                            "; DepthCuePixelsAdjusted=" + (zFrame == null ? 0 : zFrame.DepthCuePixelsAdjusted).ToString() +
                            "; BackgroundProfile=InventorBlueGray" +
                            "; ElapsedSeconds=" + zBufferSw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) +
                            "; DepthRule=GreaterViewZIsNear" +
                            "; FrontFaceRule=" + (renderProfile.FrontFacingPositiveViewZ ? "NormalZPositive" : "LegacyNormalZNegative") +
                            "; RasterizeAllTrianglesForDepth=" + renderProfile.RasterizeAllTrianglesForDepth.ToString() +
                            "; NearestPositiveNormalZPixels=" + (zFrame == null ? 0 : zFrame.NearestPositiveNormalZPixels).ToString() +
                            "; NearestNegativeNormalZPixels=" + (zFrame == null ? 0 : zFrame.NearestNegativeNormalZPixels).ToString() +
                            "; LegacyRuleWouldCullNearestPixels=" + (zFrame == null ? 0 : zFrame.LegacyRuleWouldCullNearestPixels).ToString());

                        AppLogger.Log(
                            "STEP_ZBUFFER_HIDDEN_LINE_STATS",
                            "DrawStepLiteScenePreview",
                            "CandidateEdges=" + (zFrame == null ? 0 : zFrame.CandidateEdges).ToString() +
                            "; VisibleEdges=" + (zFrame == null ? 0 : zFrame.VisibleEdges).ToString() +
                            "; VisibleEdgeSamples=" + (zFrame == null ? 0L : zFrame.VisibleEdgeSamples).ToString() +
                            "; HiddenEdgeSamples=" + (zFrame == null ? 0L : zFrame.HiddenEdgeSamples).ToString() +
                            "; SilhouetteEdges=" + silhouetteEdges.ToString() +
                            "; BoundaryEdges=" + boundaryEdges.ToString() +
                            "; CreaseEdges=" + creaseEdges.ToString() +
                            "; MeshEdges=" + meshEdges.ToString() +
                            "; EdgeSource=" + (modeSurfaceMesh ? "POLY_LOOP_BOUNDARIES" : "FEATURE_EDGES") +
                            "; AppVersion=" + AppBuild.Version +
                            "; MethodVersion=" + renderProfile.MethodVersion +
                            "; RenderMethod=" + renderProfile.MethodId +
                            "; MeshDepthToleranceFactor=" + renderProfile.MeshDepthToleranceFactor.ToString("0.0000", CultureInfo.InvariantCulture) +
                            "; FeatureDepthToleranceFactor=" + renderProfile.FeatureDepthToleranceFactor.ToString("0.0000", CultureInfo.InvariantCulture) +
                            "; SilhouetteDepthToleranceFactor=" + renderProfile.SilhouetteDepthToleranceFactor.ToString("0.0000", CultureInfo.InvariantCulture) +
                            "; MeshVisibleSampleRatioThreshold=" + renderProfile.MeshVisibleSampleRatioThreshold.ToString("0.00", CultureInfo.InvariantCulture) +
                            "; MeshDepthNeighborhoodRadius=" + renderProfile.MeshDepthNeighborhoodRadius.ToString() +
                            "; AcceptedMeshEdgesByRatio=" + (zFrame == null ? 0 : zFrame.AcceptedMeshEdgesByRatio).ToString() +
                            "; RejectedMeshEdgesByRatio=" + (zFrame == null ? 0 : zFrame.RejectedMeshEdgesByRatio).ToString() +
                            "; HiddenByFrontDepth=" + (zFrame == null ? 0L : zFrame.HiddenByFrontDepth).ToString() +
                            "; RequireMeshSurfaceOwnerMatch=" + renderProfile.RequireMeshSurfaceOwnerMatch.ToString() +
                            "; MeshOwnerNeighborhoodRadius=" + renderProfile.MeshOwnerNeighborhoodRadius.ToString() +
                            "; DepthCueStrength=" + renderProfile.DepthCueStrength.ToString() +
                            "; DepthCueGamma=" + renderProfile.DepthCueGamma.ToString("0.00", CultureInfo.InvariantCulture) +
                            "; MeshOwnerMatchedSamples=" + (zFrame == null ? 0L : zFrame.MeshOwnerMatchedSamples).ToString() +
                            "; MeshOwnerRejectedSamples=" + (zFrame == null ? 0L : zFrame.MeshOwnerRejectedSamples).ToString() +
                            "; MeshEdgesWithoutSurfaceOwner=" + (zFrame == null ? 0 : zFrame.MeshEdgesWithoutSurfaceOwner).ToString() +
                            "; HiddenLineRemoval=ZBufferDepthTest");

                        if (modeSurfaceMesh)
                        {
                            AppLogger.Log(
                                "STEP_ZBUFFER_VISIBLE_MESH_STATS",
                                "DrawStepLiteScenePreview",
                                "SceneMeshEdges=" + meshEdges.ToString() +
                                "; CandidateMeshEdges=" + (zFrame == null ? 0 : zFrame.CandidateMeshEdges).ToString() +
                                "; VisibleMeshEdges=" + (zFrame == null ? 0 : zFrame.VisibleMeshEdges).ToString() +
                                "; VisibleMeshSamples=" + (zFrame == null ? 0L : zFrame.VisibleMeshSamples).ToString() +
                                "; HiddenMeshSamples=" + (zFrame == null ? 0L : zFrame.HiddenMeshSamples).ToString() +
                                "; SilhouetteOverlayEdges=" + silhouetteOverlayEdges.ToString() +
                                "; AppVersion=" + AppBuild.Version +
                                "; MethodVersion=" + renderProfile.MethodVersion +
                                "; RenderMethod=" + renderProfile.MethodId +
                                "; MeshLineRgb=" + renderProfile.MeshLineRgb.ToString() +
                                "; SilhouetteLineRgb=" + renderProfile.SilhouetteLineRgb.ToString() +
                                "; MeshVisibleSampleRatioThreshold=" + renderProfile.MeshVisibleSampleRatioThreshold.ToString("0.00", CultureInfo.InvariantCulture) +
                                "; MeshDepthNeighborhoodRadius=" + renderProfile.MeshDepthNeighborhoodRadius.ToString() +
                                "; AcceptedMeshEdgesByRatio=" + (zFrame == null ? 0 : zFrame.AcceptedMeshEdgesByRatio).ToString() +
                                "; RejectedMeshEdgesByRatio=" + (zFrame == null ? 0 : zFrame.RejectedMeshEdgesByRatio).ToString() +
                                "; HiddenByFrontDepth=" + (zFrame == null ? 0L : zFrame.HiddenByFrontDepth).ToString() +
                                "; SurfaceAlpha=" + renderProfile.SurfaceAlpha.ToString() +
                                "; RequireMeshSurfaceOwnerMatch=" + renderProfile.RequireMeshSurfaceOwnerMatch.ToString() +
                                "; MeshOwnerNeighborhoodRadius=" + renderProfile.MeshOwnerNeighborhoodRadius.ToString() +
                                "; DepthCueStrength=" + renderProfile.DepthCueStrength.ToString() +
                                "; DepthCueGamma=" + renderProfile.DepthCueGamma.ToString("0.00", CultureInfo.InvariantCulture) +
                                "; OpaqueSurfacePixels=" + (zFrame == null ? 0 : zFrame.OpaqueSurfacePixelCount).ToString() +
                                "; NonOpaqueSurfacePixels=" + (zFrame == null ? 0 : zFrame.NonOpaqueSurfacePixelCount).ToString() +
                                "; DepthCuePixelsAdjusted=" + (zFrame == null ? 0 : zFrame.DepthCuePixelsAdjusted).ToString() +
                                "; MeshOwnerMatchedSamples=" + (zFrame == null ? 0L : zFrame.MeshOwnerMatchedSamples).ToString() +
                                "; MeshOwnerRejectedSamples=" + (zFrame == null ? 0L : zFrame.MeshOwnerRejectedSamples).ToString() +
                                "; MeshEdgesWithoutSurfaceOwner=" + (zFrame == null ? 0 : zFrame.MeshEdgesWithoutSurfaceOwner).ToString() +
                                "; MeshSource=StepLiteScene.Edges" +
                                "; TriangleDiagonalsIncluded=False" +
                                "; HiddenLineRemoval=ZBufferDepthTest");
                        }

                        AppLogger.Log(
                            "STEP_SURFACE_SHELL_STATS",
                            "DrawStepLiteScenePreview",
                            "SurfaceTriangles=" + depthSurfaceTriangles.Count.ToString() +
                            "; FrontFacingTriangles=" + surfaceTriangles.Count.ToString() +
                            "; DepthPassTriangles=" + depthSurfaceTriangles.Count.ToString() +
                            "; BackFacingTriangles=" + backFacingTriangles.ToString() +
                            "; CulledBackFacingTriangles=" + (renderProfile.RasterizeAllTrianglesForDepth ? 0 : culledBackFacingTriangles).ToString() +
                            "; BackFacingTrianglesExcludedFromMeshFrontList=" + culledBackFacingTriangles.ToString() +
                            "; DrawnFrontFacingTriangles=" + surfaceTriangles.Count.ToString() +
                            "; DrawnDepthPassTriangles=" + drawnTriangles.ToString() +
                            "; DrawnFeatureEdges=" + drawnEdges.ToString() +
                            "; SoftContactShadow=False" +
                            "; DegenerateTriangles=" + degenerateTriangles.ToString() +
                            "; SurfaceOpaque=" + (renderProfile.SurfaceAlpha >= 255).ToString() +
                            "; SurfaceAlpha=" + renderProfile.SurfaceAlpha.ToString() +
                            "; OpaqueSurfaceOwnerPass=" + renderProfile.RequireMeshSurfaceOwnerMatch.ToString() +
                            "; OpaqueSurfacePixels=" + (zFrame == null ? 0 : zFrame.OpaqueSurfacePixelCount).ToString() +
                            "; NonOpaqueSurfacePixels=" + (zFrame == null ? 0 : zFrame.NonOpaqueSurfacePixelCount).ToString() +
                            "; DepthCueStrength=" + renderProfile.DepthCueStrength.ToString() +
                            "; DepthCuePixelsAdjusted=" + (zFrame == null ? 0 : zFrame.DepthCuePixelsAdjusted).ToString() +
                            "; GhostMode=False" +
                            "; DepthSort=SoftwareZBuffer" +
                            "; FrontFaceRule=" + (renderProfile.FrontFacingPositiveViewZ ? "NormalZPositive" : "LegacyNormalZNegative") +
                            "; RasterizeAllTrianglesForDepth=" + renderProfile.RasterizeAllTrianglesForDepth.ToString() +
                            "; NearestPositiveNormalZPixels=" + (zFrame == null ? 0 : zFrame.NearestPositiveNormalZPixels).ToString() +
                            "; NearestNegativeNormalZPixels=" + (zFrame == null ? 0 : zFrame.NearestNegativeNormalZPixels).ToString() +
                            "; LegacyRuleWouldCullNearestPixels=" + (zFrame == null ? 0 : zFrame.LegacyRuleWouldCullNearestPixels).ToString() +
                            "; Shading=GouraudVertexNormals" +
                            "; AppVersion=" + AppBuild.Version +
                            "; MethodVersion=" + renderProfile.MethodVersion +
                            "; RenderMethod=" + renderProfile.MethodId +
                            "; LightModel=" + renderProfile.LightModel +
                            "; EdgeMode=" +
                            (modeSurfaceMesh
                                ? "ZBufferVisiblePolyLoopMesh"
                                : (drawEdges ? "ZBufferVisibleFeatureEdges" : "None")));
                    }
                    else
                    {
                        // Ghost intentionally keeps the translucent painter-style preview.
                        surfaceTriangles.Sort(delegate(StepSurfaceDrawTriangle a, StepSurfaceDrawTriangle b)
                        {
                            return a.Depth.CompareTo(b.Depth);
                        });

                        using (System.Drawing.Brush ghostBrush =
                            new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(42, 172, 176, 184)))
                        using (System.Drawing.Drawing2D.GraphicsPath ghostEdgePath =
                            new System.Drawing.Drawing2D.GraphicsPath())
                        using (System.Drawing.Pen ghostEdgePen =
                            new System.Drawing.Pen(System.Drawing.Color.FromArgb(72, 82, 86, 92), 0.38F))
                        {
                            foreach (StepSurfaceDrawTriangle drawTriangle in surfaceTriangles)
                            {
                                if (drawTriangle == null || drawTriangle.Poly == null || drawTriangle.Triangle == null)
                                {
                                    continue;
                                }

                                g.FillPolygon(ghostBrush, drawTriangle.Poly);
                                AddTriangleWireToPath(ghostEdgePath, projected, drawTriangle.Triangle);
                                drawnTriangles++;
                                drawnEdges += 3;
                            }

                            if (ghostEdgePath.PointCount > 0)
                            {
                                g.DrawPath(ghostEdgePen, ghostEdgePath);
                            }
                        }

                        AppLogger.Log(
                            "STEP_SURFACE_SHELL_STATS",
                            "DrawStepLiteScenePreview",
                            "SurfaceTriangles=" + depthSurfaceTriangles.Count.ToString() +
                            "; FrontFacingTriangles=" + surfaceTriangles.Count.ToString() +
                            "; DepthPassTriangles=" + depthSurfaceTriangles.Count.ToString() +
                            "; BackFacingTriangles=" + backFacingTriangles.ToString() +
                            "; CulledBackFacingTriangles=0" +
                            "; DrawnFrontFacingTriangles=" + surfaceTriangles.Count.ToString() +
                            "; DrawnDepthPassTriangles=" + drawnTriangles.ToString() +
                            "; DrawnFeatureEdges=" + drawnEdges.ToString() +
                            "; SoftContactShadow=False" +
                            "; DegenerateTriangles=" + degenerateTriangles.ToString() +
                            "; SurfaceOpaque=False" +
                            "; GhostMode=True" +
                            "; DepthSort=PainterAscending" +
                            "; Shading=GhostConstantAlpha" +
                            "; EdgeMode=GhostWire");
                    }
                }
            }
            else if (_stepLitePreviewScene.EdgeCount > 0 && !modePoints)
            {
                int maxEdgesToDraw = fastDragMode ? 12000 : 120000;
                int step = Math.Max(1, _stepLitePreviewScene.EdgeCount / maxEdgesToDraw);

                using (System.Drawing.Drawing2D.GraphicsPath edgePath = new System.Drawing.Drawing2D.GraphicsPath())
                using (System.Drawing.Pen edgePen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(150, 35, 35, 35), 0.85F))
                {
                    for (int i = 0; i < _stepLitePreviewScene.Edges.Count; i += step)
                    {
                        StepEdge edge = _stepLitePreviewScene.Edges[i];

                        if (edge == null || edge.A < 0 || edge.B < 0 ||
                            edge.A >= projected.Length || edge.B >= projected.Length)
                        {
                            continue;
                        }

                        edgePath.StartFigure();
                        edgePath.AddLine(projected[edge.A], projected[edge.B]);
                        drawnEdges++;
                    }

                    g.DrawPath(edgePen, edgePath);
                }
            }

            if (modePoints || _viewerShowPointsOverlay)
            {
                int maxPointsToDraw = fastDragMode ? 4000 : 12000;
                int pointStep = Math.Max(1, _stepLitePreviewScene.PointCount / maxPointsToDraw);

                using (System.Drawing.Brush pointBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(modePoints ? 180 : 90, 30, 90, 180)))
                {
                    for (int i = 0; i < projected.Length; i += pointStep)
                    {
                        System.Drawing.PointF pp = projected[i];
                        g.FillEllipse(pointBrush, pp.X - 1.5F, pp.Y - 1.5F, 3.0F, 3.0F);
                    }
                }
            }

            if (_viewerShowBoxesOverlay && _spatialCubesIndex != null && _spatialCubesIndex.IsReady && _spatialCubesIndex.ModelBox != null)
            {
                DrawBoxGridOverlayOnStepViewer(g, bounds);
            }

            drawSw.Stop();

            AppLogger.Log(
                "STEP_LOCAL_DRAW_SECONDS",
                "DrawStepLiteScenePreview",
                "ElapsedSeconds=" + drawSw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) +
                "; Mode=" + renderMode +
                "; Points=" + _stepLitePreviewScene.PointCount.ToString() +
                "; Edges=" + _stepLitePreviewScene.EdgeCount.ToString() +
                "; Triangles=" + _stepLitePreviewScene.TriangleCount.ToString() +
                "; PolyLoops=" + _stepLitePreviewScene.PolyLoopEntityCount.ToString() +
                "; DrawnTriangles=" + drawnTriangles.ToString() +
                "; DrawnEdges=" + drawnEdges.ToString() +
                "; AppVersion=" + AppBuild.Version +
                "; MethodVersion=" + renderProfile.MethodVersion +
                "; RenderMethod=" + renderProfile.MethodId +
                "; LightModel=" + renderProfile.LightModel +
                "; ViewerProfile=" + renderProfile.ViewerProfile +
                "; SurfaceFillMode=" + renderProfile.SurfaceFillMode +
                "; GdiStrategy=" + renderProfile.GdiStrategy);
        }

        private static bool IsValidStepTriangle(StepTriangle triangle, int pointCount)
        {
            if (triangle == null)
            {
                return false;
            }

            return triangle.A >= 0 && triangle.B >= 0 && triangle.C >= 0 &&
                   triangle.A < pointCount && triangle.B < pointCount && triangle.C < pointCount &&
                   triangle.A != triangle.B && triangle.B != triangle.C && triangle.A != triangle.C;
        }

        private static void AddTriangleWireToPath(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.PointF[] projected, StepTriangle triangle)
        {
            path.StartFigure();
            path.AddLine(projected[triangle.A], projected[triangle.B]);
            path.StartFigure();
            path.AddLine(projected[triangle.B], projected[triangle.C]);
            path.StartFigure();
            path.AddLine(projected[triangle.C], projected[triangle.A]);
        }

        private List<StepVisibleEdgeSegment> BuildStepVisibleMeshEdgeSegments(
            List<StepSurfaceDrawTriangle> surfaceTriangles,
            out int meshEdges)
        {
            meshEdges = 0;
            List<StepVisibleEdgeSegment> result = new List<StepVisibleEdgeSegment>();

            if (_stepLitePreviewScene == null ||
                _stepLitePreviewScene.Edges == null ||
                _stepLitePreviewScene.Points == null)
            {
                return result;
            }

            double modelSize = 1.0;

            if (_stepLitePreviewScene.Bounds != null && _stepLitePreviewScene.Bounds.IsValid)
            {
                modelSize = Math.Max(
                    _stepLitePreviewScene.Bounds.SizeX,
                    Math.Max(_stepLitePreviewScene.Bounds.SizeY, _stepLitePreviewScene.Bounds.SizeZ));
            }

            double quantizeTolerance = Math.Max(0.000001, modelSize * 0.000001);
            Dictionary<string, List<int>> surfaceOwnersByGeometry =
                BuildStepSurfaceEdgeOwnerMap(surfaceTriangles, quantizeTolerance);

            foreach (StepEdge edge in _stepLitePreviewScene.Edges)
            {
                if (edge == null ||
                    edge.A < 0 || edge.B < 0 ||
                    edge.A >= _stepLitePreviewScene.PointCount ||
                    edge.B >= _stepLitePreviewScene.PointCount ||
                    edge.A == edge.B)
                {
                    continue;
                }

                StepVisibleEdgeSegment segment = new StepVisibleEdgeSegment();
                segment.A = edge.A;
                segment.B = edge.B;
                segment.Style = 4; // Visible POLY_LOOP mesh: thin one-pixel line.

                string edgeKey = MakeStepGeometryEdgeKey(edge.A, edge.B, quantizeTolerance);
                List<int> ownerTriangleIds;

                if (!string.IsNullOrEmpty(edgeKey) &&
                    surfaceOwnersByGeometry.TryGetValue(edgeKey, out ownerTriangleIds))
                {
                    segment.SurfaceOwnerTriangleIds = new List<int>(ownerTriangleIds);
                }

                result.Add(segment);
                meshEdges++;
            }

            return result;
        }

        private Dictionary<string, List<int>> BuildStepSurfaceEdgeOwnerMap(
            List<StepSurfaceDrawTriangle> surfaceTriangles,
            double tolerance)
        {
            Dictionary<string, List<int>> result =
                new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);

            if (surfaceTriangles == null)
            {
                return result;
            }

            for (int triangleId = 0; triangleId < surfaceTriangles.Count; triangleId++)
            {
                StepSurfaceDrawTriangle drawTriangle = surfaceTriangles[triangleId];

                if (drawTriangle == null || drawTriangle.Triangle == null)
                {
                    continue;
                }

                StepTriangle triangle = drawTriangle.Triangle;
                AddStepSurfaceEdgeOwner(result, triangle.A, triangle.B, triangleId, tolerance);
                AddStepSurfaceEdgeOwner(result, triangle.B, triangle.C, triangleId, tolerance);
                AddStepSurfaceEdgeOwner(result, triangle.C, triangle.A, triangleId, tolerance);
            }

            return result;
        }

        private void AddStepSurfaceEdgeOwner(
            Dictionary<string, List<int>> ownersByGeometry,
            int a,
            int b,
            int triangleId,
            double tolerance)
        {
            if (ownersByGeometry == null)
            {
                return;
            }

            string edgeKey = MakeStepGeometryEdgeKey(a, b, tolerance);

            if (string.IsNullOrEmpty(edgeKey))
            {
                return;
            }

            List<int> owners;

            if (!ownersByGeometry.TryGetValue(edgeKey, out owners))
            {
                owners = new List<int>();
                ownersByGeometry[edgeKey] = owners;
            }

            if (!owners.Contains(triangleId))
            {
                owners.Add(triangleId);
            }
        }

        private string MakeStepGeometryEdgeKey(int a, int b, double tolerance)
        {
            if (_stepLitePreviewScene == null ||
                _stepLitePreviewScene.Points == null ||
                a < 0 || b < 0 ||
                a >= _stepLitePreviewScene.Points.Count ||
                b >= _stepLitePreviewScene.Points.Count)
            {
                return null;
            }

            StepPoint3 pa = _stepLitePreviewScene.Points[a];
            StepPoint3 pb = _stepLitePreviewScene.Points[b];

            if (pa == null || pb == null)
            {
                return null;
            }

            string keyA = MakeStepQuantizedPointKey(pa, tolerance);
            string keyB = MakeStepQuantizedPointKey(pb, tolerance);

            return string.CompareOrdinal(keyA, keyB) <= 0
                ? keyA + "|" + keyB
                : keyB + "|" + keyA;
        }

        private List<StepVisibleEdgeSegment> BuildStepVisibleEdgeSegments(
            List<StepSurfaceDrawTriangle> allTriangles,
            out int silhouetteEdges,
            out int boundaryEdges,
            out int creaseEdges)
        {
            silhouetteEdges = 0;
            boundaryEdges = 0;
            creaseEdges = 0;

            List<StepVisibleEdgeSegment> result = new List<StepVisibleEdgeSegment>();

            if (allTriangles == null || _stepLitePreviewScene == null ||
                _stepLitePreviewScene.Points == null)
            {
                return result;
            }

            Dictionary<string, StepFeatureEdgeInfo> edges =
                new Dictionary<string, StepFeatureEdgeInfo>(StringComparer.OrdinalIgnoreCase);

            double modelSize = 1.0;

            if (_stepLitePreviewScene.Bounds != null && _stepLitePreviewScene.Bounds.IsValid)
            {
                modelSize = Math.Max(
                    _stepLitePreviewScene.Bounds.SizeX,
                    Math.Max(_stepLitePreviewScene.Bounds.SizeY, _stepLitePreviewScene.Bounds.SizeZ));
            }

            double quantizeTolerance = Math.Max(0.000001, modelSize * 0.000001);

            foreach (StepSurfaceDrawTriangle triangle in allTriangles)
            {
                if (triangle == null || triangle.Triangle == null)
                {
                    continue;
                }

                AddStepFeatureEdgeCandidateByGeometry(
                    edges, triangle.Triangle.A, triangle.Triangle.B, triangle, quantizeTolerance);
                AddStepFeatureEdgeCandidateByGeometry(
                    edges, triangle.Triangle.B, triangle.Triangle.C, triangle, quantizeTolerance);
                AddStepFeatureEdgeCandidateByGeometry(
                    edges, triangle.Triangle.C, triangle.Triangle.A, triangle, quantizeTolerance);
            }

            foreach (StepFeatureEdgeInfo edge in edges.Values)
            {
                if (edge == null || edge.FrontFacingCount <= 0)
                {
                    continue;
                }

                int style = 0;

                if (edge.FrontFacingCount > 0 && edge.BackFacingCount > 0)
                {
                    silhouetteEdges++;
                    style = 2;
                }
                else if (edge.Count == 1)
                {
                    boundaryEdges++;
                    style = 1;
                }
                else if (edge.Count > 2)
                {
                    creaseEdges++;
                    style = 3;
                }
                else if (edge.FirstTriangle != null && edge.SecondTriangle != null &&
                         !edge.FirstTriangle.BackFacing && !edge.SecondTriangle.BackFacing)
                {
                    double dot =
                        edge.FirstTriangle.NormalX * edge.SecondTriangle.NormalX +
                        edge.FirstTriangle.NormalY * edge.SecondTriangle.NormalY +
                        edge.FirstTriangle.NormalZ * edge.SecondTriangle.NormalZ;

                    dot = Math.Abs(dot);

                    if (dot < 0.975)
                    {
                        creaseEdges++;
                        style = 3;
                    }
                }

                if (style == 0)
                {
                    continue;
                }

                StepVisibleEdgeSegment segment = new StepVisibleEdgeSegment();
                segment.A = edge.A;
                segment.B = edge.B;
                segment.Style = style;
                result.Add(segment);
            }

            return result;
        }

        private StepZBufferFrame RenderStepZBufferSurface(
            List<StepSurfaceDrawTriangle> triangles,
            System.Drawing.PointF[] projected,
            double[] projectedDepths,
            System.Drawing.Rectangle bounds,
            List<StepVisibleEdgeSegment> edges,
            double modelSize,
            bool drawEdges,
            StepRenderMethodProfile renderProfile)
        {
            if (triangles == null || projected == null || projectedDepths == null ||
                bounds.Width <= 0 || bounds.Height <= 0)
            {
                return null;
            }

            if (renderProfile == null)
            {
                renderProfile = GetStepRenderMethodProfile("SurfaceMesh");
            }

            bool reducedDragFrame = _viewerRenderingReducedDragFrame;
            bool needsSurfaceOwnerIds = !reducedDragFrame || renderProfile.RequireMeshSurfaceOwnerMatch;

            StepZBufferFrame frame = new StepZBufferFrame();
            frame.Width = bounds.Width;
            frame.Height = bounds.Height;
            frame.Pixels = new int[frame.Width * frame.Height];
            frame.Depth = new double[frame.Width * frame.Height];
            frame.SurfaceOwnerTriangleIds = needsSurfaceOwnerIds
                ? new int[frame.Width * frame.Height]
                : null;

            for (int i = 0; i < frame.Depth.Length; i++)
            {
                frame.Depth[i] = double.NegativeInfinity;
                if (frame.SurfaceOwnerTriangleIds != null)
                {
                    frame.SurfaceOwnerTriangleIds[i] = -1;
                }
            }

            for (int triangleId = 0; triangleId < triangles.Count; triangleId++)
            {
                StepSurfaceDrawTriangle drawTriangle = triangles[triangleId];
                if (drawTriangle == null || drawTriangle.Triangle == null)
                {
                    continue;
                }

                StepTriangle triangle = drawTriangle.Triangle;

                if (!IsValidStepTriangle(triangle, projected.Length) ||
                    triangle.A >= projectedDepths.Length ||
                    triangle.B >= projectedDepths.Length ||
                    triangle.C >= projectedDepths.Length)
                {
                    continue;
                }

                RasterizeStepZBufferTriangle(
                    frame,
                    projected[triangle.A],
                    projected[triangle.B],
                    projected[triangle.C],
                    projectedDepths[triangle.A],
                    projectedDepths[triangle.B],
                    projectedDepths[triangle.C],
                    drawTriangle.ShadeA,
                    drawTriangle.ShadeB,
                    drawTriangle.ShadeC,
                    bounds,
                    renderProfile.InputShadeMin,
                    renderProfile.InputShadeMax,
                    renderProfile.SurfaceShadeMin,
                    renderProfile.SurfaceShadeMax,
                    renderProfile.SurfaceAlpha,
                    triangleId);
            }

            // v0.5.12: the reduced drag path does not need expensive full-frame diagnostic scans.
            // True Front drag has DepthCueStrength=0 and owner matching disabled, so both scans were pure overhead.
            if (!reducedDragFrame)
            {
                AnalyzeStepNearestSurfaceFacing(frame, triangles);
                ApplyStepOpaqueDepthCue(frame, renderProfile);
            }
            else if (renderProfile.DepthCueStrength > 0)
            {
                ApplyStepOpaqueDepthCue(frame, renderProfile);
            }

            if (drawEdges && edges != null && edges.Count > 0)
            {
                frame.CandidateEdges = edges.Count;
                double meshDepthTolerance = Math.Max(0.000001, Math.Abs(modelSize) * renderProfile.MeshDepthToleranceFactor);
                double featureDepthTolerance = Math.Max(0.000001, Math.Abs(modelSize) * renderProfile.FeatureDepthToleranceFactor);
                double silhouetteDepthTolerance = Math.Max(0.000001, Math.Abs(modelSize) * renderProfile.SilhouetteDepthToleranceFactor);

                foreach (StepVisibleEdgeSegment edge in edges)
                {
                    if (edge == null ||
                        edge.A < 0 || edge.B < 0 ||
                        edge.A >= projected.Length || edge.B >= projected.Length ||
                        edge.A >= projectedDepths.Length || edge.B >= projectedDepths.Length)
                    {
                        continue;
                    }

                    if (edge.Style == 4)
                    {
                        frame.CandidateMeshEdges++;
                    }

                    double edgeDepthTolerance = edge.Style == 2
                        ? silhouetteDepthTolerance
                        : (edge.Style == 4 ? meshDepthTolerance : featureDepthTolerance);

                    bool visible = RasterizeStepZBufferEdge(
                        frame,
                        projected[edge.A],
                        projected[edge.B],
                        projectedDepths[edge.A],
                        projectedDepths[edge.B],
                        edge.Style,
                        bounds,
                        edgeDepthTolerance,
                        renderProfile,
                        edge.SurfaceOwnerTriangleIds);

                    if (visible)
                    {
                        frame.VisibleEdges++;
                    }
                }
            }

            return frame;
        }

        private static void AnalyzeStepNearestSurfaceFacing(
            StepZBufferFrame frame,
            List<StepSurfaceDrawTriangle> triangles)
        {
            if (frame == null || frame.SurfaceOwnerTriangleIds == null || triangles == null)
            {
                return;
            }

            for (int i = 0; i < frame.SurfaceOwnerTriangleIds.Length; i++)
            {
                int ownerId = frame.SurfaceOwnerTriangleIds[i];

                if (ownerId < 0 || ownerId >= triangles.Count)
                {
                    continue;
                }

                StepSurfaceDrawTriangle owner = triangles[ownerId];

                if (owner == null)
                {
                    continue;
                }

                if (owner.NormalZ > 0.0)
                {
                    frame.NearestPositiveNormalZPixels++;
                    // The legacy rule classified NormalZ > 0 as BackFacing and removed this surface.
                    frame.LegacyRuleWouldCullNearestPixels++;
                }
                else if (owner.NormalZ < 0.0)
                {
                    frame.NearestNegativeNormalZPixels++;
                }
            }
        }

        private static void ApplyStepOpaqueDepthCue(
            StepZBufferFrame frame,
            StepRenderMethodProfile renderProfile)
        {
            if (frame == null || frame.Pixels == null || frame.Depth == null)
            {
                return;
            }

            double minDepth = double.PositiveInfinity;
            double maxDepth = double.NegativeInfinity;

            for (int i = 0; i < frame.Depth.Length; i++)
            {
                if (double.IsNegativeInfinity(frame.Depth[i]))
                {
                    continue;
                }

                if (frame.Depth[i] < minDepth) minDepth = frame.Depth[i];
                if (frame.Depth[i] > maxDepth) maxDepth = frame.Depth[i];
            }

            double depthRange = maxDepth - minDepth;
            int cueStrength = renderProfile == null ? 0 : Math.Max(0, renderProfile.DepthCueStrength);
            double cueGamma = renderProfile == null ? 1.0 : Math.Max(0.1, renderProfile.DepthCueGamma);

            for (int i = 0; i < frame.Pixels.Length; i++)
            {
                if (double.IsNegativeInfinity(frame.Depth[i]))
                {
                    continue;
                }

                int pixel = frame.Pixels[i];
                int alpha = (int)((uint)pixel >> 24);

                if (alpha >= 255)
                {
                    frame.OpaqueSurfacePixelCount++;
                }
                else
                {
                    frame.NonOpaqueSurfacePixelCount++;
                }

                if (cueStrength <= 0 || depthRange <= 0.000000001)
                {
                    continue;
                }

                double nearAmount = (frame.Depth[i] - minDepth) / depthRange;
                nearAmount = Math.Max(0.0, Math.Min(1.0, nearAmount));
                double farAmount = Math.Pow(1.0 - nearAmount, cueGamma);
                int darken = (int)Math.Round(cueStrength * farAmount);

                if (darken <= 0)
                {
                    continue;
                }

                int red = Math.Max(0, ((pixel >> 16) & 255) - darken);
                int green = Math.Max(0, ((pixel >> 8) & 255) - darken);
                int blue = Math.Max(0, (pixel & 255) - darken);
                frame.Pixels[i] = MakeStepArgb(alpha, red, green, blue);
                frame.DepthCuePixelsAdjusted++;
            }
        }

        private static void RasterizeStepZBufferTriangle(
            StepZBufferFrame frame,
            System.Drawing.PointF p0Screen,
            System.Drawing.PointF p1Screen,
            System.Drawing.PointF p2Screen,
            double z0,
            double z1,
            double z2,
            int shade0,
            int shade1,
            int shade2,
            System.Drawing.Rectangle bounds,
            int inputShadeMin,
            int inputShadeMax,
            int outputShadeMin,
            int outputShadeMax,
            int surfaceAlpha,
            int triangleId)
        {
            if (frame == null || frame.Pixels == null || frame.Depth == null)
            {
                return;
            }

            double x0 = p0Screen.X - bounds.Left;
            double y0 = p0Screen.Y - bounds.Top;
            double x1 = p1Screen.X - bounds.Left;
            double y1 = p1Screen.Y - bounds.Top;
            double x2 = p2Screen.X - bounds.Left;
            double y2 = p2Screen.Y - bounds.Top;

            double area = StepEdgeFunction(x0, y0, x1, y1, x2, y2);

            if (Math.Abs(area) < 0.000001)
            {
                return;
            }

            int minX = Math.Max(0, (int)Math.Floor(Math.Min(x0, Math.Min(x1, x2))));
            int maxX = Math.Min(frame.Width - 1, (int)Math.Ceiling(Math.Max(x0, Math.Max(x1, x2))));
            int minY = Math.Max(0, (int)Math.Floor(Math.Min(y0, Math.Min(y1, y2))));
            int maxY = Math.Min(frame.Height - 1, (int)Math.Ceiling(Math.Max(y0, Math.Max(y1, y2))));

            if (minX > maxX || minY > maxY)
            {
                return;
            }

            double invArea = 1.0 / area;
            bool positiveArea = area > 0.0;
            int clampedShade0 = Math.Max(inputShadeMin, Math.Min(inputShadeMax, shade0));
            int clampedShade1 = Math.Max(inputShadeMin, Math.Min(inputShadeMax, shade1));
            int clampedShade2 = Math.Max(inputShadeMin, Math.Min(inputShadeMax, shade2));

            for (int y = minY; y <= maxY; y++)
            {
                double py = y + 0.5;

                for (int x = minX; x <= maxX; x++)
                {
                    double px = x + 0.5;
                    double e0 = StepEdgeFunction(x1, y1, x2, y2, px, py);
                    double e1 = StepEdgeFunction(x2, y2, x0, y0, px, py);
                    double e2 = StepEdgeFunction(x0, y0, x1, y1, px, py);

                    bool inside = positiveArea
                        ? e0 >= 0.0 && e1 >= 0.0 && e2 >= 0.0
                        : e0 <= 0.0 && e1 <= 0.0 && e2 <= 0.0;

                    if (!inside)
                    {
                        continue;
                    }

                    double w0 = e0 * invArea;
                    double w1 = e1 * invArea;
                    double w2 = e2 * invArea;
                    double depth = w0 * z0 + w1 * z1 + w2 * z2;
                    double interpolatedShade =
                        w0 * clampedShade0 +
                        w1 * clampedShade1 +
                        w2 * clampedShade2;
                    int shade = Math.Max(outputShadeMin, Math.Min(outputShadeMax, (int)Math.Round(interpolatedShade)));
                    int opaqueAlpha = Math.Max(0, Math.Min(255, surfaceAlpha));
                    int pixelColor = MakeStepArgb(opaqueAlpha, shade, shade, shade);
                    int index = y * frame.Width + x;

                    if (depth >= frame.Depth[index])
                    {
                        frame.Depth[index] = depth;
                        frame.Pixels[index] = pixelColor;

                        if (frame.SurfaceOwnerTriangleIds != null &&
                            index >= 0 && index < frame.SurfaceOwnerTriangleIds.Length)
                        {
                            frame.SurfaceOwnerTriangleIds[index] = triangleId;
                        }

                        frame.SurfacePixelsWritten++;
                    }
                    else
                    {
                        frame.SurfacePixelsRejectedByDepth++;
                    }
                }
            }
        }

        private static bool RasterizeStepZBufferEdge(
            StepZBufferFrame frame,
            System.Drawing.PointF aScreen,
            System.Drawing.PointF bScreen,
            double depthA,
            double depthB,
            int style,
            System.Drawing.Rectangle bounds,
            double depthTolerance,
            StepRenderMethodProfile renderProfile,
            List<int> surfaceOwnerTriangleIds)
        {
            if (frame == null || frame.Pixels == null || frame.Depth == null)
            {
                return false;
            }

            double x0 = aScreen.X - bounds.Left;
            double y0 = aScreen.Y - bounds.Top;
            double x1 = bScreen.X - bounds.Left;
            double y1 = bScreen.Y - bounds.Top;
            double dx = x1 - x0;
            double dy = y1 - y0;
            int steps = Math.Max(1, (int)Math.Ceiling(Math.Max(Math.Abs(dx), Math.Abs(dy))));
            bool anyVisible = false;

            if (renderProfile == null)
            {
                renderProfile = GetStepRenderMethodProfile("SurfaceMesh");
            }

            int color;
            int radius;

            if (style == 2)
            {
                color = MakeStepArgb(255, renderProfile.SilhouetteLineRgb, renderProfile.SilhouetteLineRgb, renderProfile.SilhouetteLineRgb);
                radius = 0;
            }
            else if (style == 1)
            {
                color = MakeStepArgb(255, renderProfile.FeatureLineRgb, renderProfile.FeatureLineRgb, renderProfile.FeatureLineRgb);
                radius = 0;
            }
            else if (style == 4)
            {
                // Ordinary visible POLY_LOOP mesh line. v0.5.03 can defer drawing until
                // the whole edge passes a front-depth visible-sample ratio threshold.
                color = MakeStepArgb(255, renderProfile.MeshLineRgb, renderProfile.MeshLineRgb, renderProfile.MeshLineRgb);
                radius = 0;
            }
            else
            {
                int defaultEdgeRgb = Math.Max(renderProfile.FeatureLineRgb, Math.Min(255, renderProfile.FeatureLineRgb + 16));
                color = MakeStepArgb(255, defaultEdgeRgb, defaultEdgeRgb, defaultEdgeRgb);
                radius = 0;
            }

            bool ratioFilterMesh =
                style == 4 &&
                renderProfile.MeshVisibleSampleRatioThreshold > 0.0;
            bool requireSurfaceOwnerMatch =
                style == 4 &&
                renderProfile.RequireMeshSurfaceOwnerMatch;

            if (requireSurfaceOwnerMatch &&
                (surfaceOwnerTriangleIds == null || surfaceOwnerTriangleIds.Count == 0))
            {
                frame.MeshEdgesWithoutSurfaceOwner++;
                frame.RejectedMeshEdgesByRatio++;
                return false;
            }

            List<System.Drawing.Point> deferredVisiblePixels = ratioFilterMesh
                ? new List<System.Drawing.Point>()
                : null;
            int inBoundsSamples = 0;
            int depthVisibleSamples = 0;

            for (int i = 0; i <= steps; i++)
            {
                double t = (double)i / (double)steps;
                int x = (int)Math.Round(x0 + dx * t);
                int y = (int)Math.Round(y0 + dy * t);

                if (x < 0 || y < 0 || x >= frame.Width || y >= frame.Height)
                {
                    continue;
                }

                inBoundsSamples++;
                double depth = depthA + (depthB - depthA) * t;
                double surfaceDepth = style == 4 && renderProfile.MeshDepthNeighborhoodRadius > 0
                    ? GetStepZBufferFrontDepth(frame, x, y, renderProfile.MeshDepthNeighborhoodRadius)
                    : frame.Depth[y * frame.Width + x];

                bool depthVisible =
                    double.IsNegativeInfinity(surfaceDepth) ||
                    depth >= surfaceDepth - depthTolerance;
                bool ownerVisible = !requireSurfaceOwnerMatch;

                if (depthVisible && requireSurfaceOwnerMatch)
                {
                    ownerVisible = HasStepZBufferSurfaceOwnerMatch(
                        frame,
                        x,
                        y,
                        renderProfile.MeshOwnerNeighborhoodRadius,
                        surfaceOwnerTriangleIds,
                        surfaceDepth,
                        depthTolerance);

                    if (ownerVisible)
                    {
                        frame.MeshOwnerMatchedSamples++;
                    }
                    else
                    {
                        frame.MeshOwnerRejectedSamples++;
                    }
                }

                if (depthVisible && ownerVisible)
                {
                    depthVisibleSamples++;

                    if (ratioFilterMesh)
                    {
                        deferredVisiblePixels.Add(new System.Drawing.Point(x, y));
                    }
                    else
                    {
                        SetStepZBufferPixel(frame, x, y, color, radius);
                        frame.VisibleEdgeSamples++;

                        if (style == 4)
                        {
                            frame.VisibleMeshSamples++;
                        }

                        anyVisible = true;
                    }
                }
                else
                {
                    frame.HiddenEdgeSamples++;

                    if (!depthVisible)
                    {
                        frame.HiddenByFrontDepth++;
                    }

                    if (style == 4)
                    {
                        frame.HiddenMeshSamples++;
                    }
                }
            }

            if (ratioFilterMesh)
            {
                double visibleRatio = inBoundsSamples <= 0
                    ? 0.0
                    : (double)depthVisibleSamples / (double)inBoundsSamples;

                if (depthVisibleSamples > 0 &&
                    visibleRatio >= renderProfile.MeshVisibleSampleRatioThreshold)
                {
                    foreach (System.Drawing.Point point in deferredVisiblePixels)
                    {
                        SetStepZBufferPixel(frame, point.X, point.Y, color, radius);
                    }

                    frame.VisibleEdgeSamples += deferredVisiblePixels.Count;
                    frame.VisibleMeshSamples += deferredVisiblePixels.Count;
                    frame.VisibleMeshEdges++;
                    frame.AcceptedMeshEdgesByRatio++;
                    return true;
                }

                frame.RejectedMeshEdgesByRatio++;
                return false;
            }

            if (anyVisible && style == 4)
            {
                frame.VisibleMeshEdges++;
            }

            return anyVisible;
        }

        private static bool HasStepZBufferSurfaceOwnerMatch(
            StepZBufferFrame frame,
            int centerX,
            int centerY,
            int radius,
            List<int> allowedOwnerTriangleIds,
            double frontDepth,
            double depthTolerance)
        {
            if (frame == null ||
                frame.Depth == null ||
                frame.SurfaceOwnerTriangleIds == null ||
                allowedOwnerTriangleIds == null ||
                allowedOwnerTriangleIds.Count == 0)
            {
                return false;
            }

            int safeRadius = Math.Max(0, radius);

            for (int y = Math.Max(0, centerY - safeRadius); y <= Math.Min(frame.Height - 1, centerY + safeRadius); y++)
            {
                for (int x = Math.Max(0, centerX - safeRadius); x <= Math.Min(frame.Width - 1, centerX + safeRadius); x++)
                {
                    int index = y * frame.Width + x;
                    int ownerTriangleId = frame.SurfaceOwnerTriangleIds[index];

                    if (ownerTriangleId < 0 || !allowedOwnerTriangleIds.Contains(ownerTriangleId))
                    {
                        continue;
                    }

                    double ownerDepth = frame.Depth[index];

                    if (double.IsNegativeInfinity(frontDepth) ||
                        ownerDepth >= frontDepth - depthTolerance)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static double GetStepZBufferFrontDepth(
            StepZBufferFrame frame,
            int centerX,
            int centerY,
            int radius)
        {
            if (frame == null || frame.Depth == null || frame.Width <= 0 || frame.Height <= 0)
            {
                return double.NegativeInfinity;
            }

            int safeRadius = Math.Max(0, radius);
            double frontDepth = double.NegativeInfinity;

            for (int y = Math.Max(0, centerY - safeRadius); y <= Math.Min(frame.Height - 1, centerY + safeRadius); y++)
            {
                for (int x = Math.Max(0, centerX - safeRadius); x <= Math.Min(frame.Width - 1, centerX + safeRadius); x++)
                {
                    double sampleDepth = frame.Depth[y * frame.Width + x];

                    if (sampleDepth > frontDepth)
                    {
                        frontDepth = sampleDepth;
                    }
                }
            }

            return frontDepth;
        }

        private static void SetStepZBufferPixel(
            StepZBufferFrame frame,
            int x,
            int y,
            int color,
            int radius)
        {
            if (frame == null || frame.Pixels == null)
            {
                return;
            }

            for (int oy = -radius; oy <= radius; oy++)
            {
                int py = y + oy;

                if (py < 0 || py >= frame.Height)
                {
                    continue;
                }

                for (int ox = -radius; ox <= radius; ox++)
                {
                    int px = x + ox;

                    if (px < 0 || px >= frame.Width)
                    {
                        continue;
                    }

                    frame.Pixels[py * frame.Width + px] = color;
                }
            }
        }

        private static double StepEdgeFunction(
            double ax,
            double ay,
            double bx,
            double by,
            double px,
            double py)
        {
            return (px - ax) * (by - ay) - (py - ay) * (bx - ax);
        }

        private static int MakeStepArgb(int a, int r, int g, int b)
        {
            return unchecked(
                (a << 24) |
                ((r & 255) << 16) |
                ((g & 255) << 8) |
                (b & 255));
        }

        private static void DrawStepZBufferFrame(
            System.Drawing.Graphics g,
            System.Drawing.Rectangle bounds,
            StepZBufferFrame frame)
        {
            if (g == null || frame == null || frame.Pixels == null ||
                frame.Width <= 0 || frame.Height <= 0)
            {
                return;
            }

            using (System.Drawing.Bitmap bitmap =
                new System.Drawing.Bitmap(
                    frame.Width,
                    frame.Height,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                System.Drawing.Rectangle bitmapBounds =
                    new System.Drawing.Rectangle(0, 0, frame.Width, frame.Height);

                System.Drawing.Imaging.BitmapData data = bitmap.LockBits(
                    bitmapBounds,
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                try
                {
                    Marshal.Copy(frame.Pixels, 0, data.Scan0, frame.Pixels.Length);
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }

                // v0.5.10: explicit pixel mapping prevents DPI metadata from scaling/offsetting the Z-buffer image.
                g.DrawImage(
                    bitmap,
                    new System.Drawing.Rectangle(bounds.Left, bounds.Top, frame.Width, frame.Height),
                    0,
                    0,
                    frame.Width,
                    frame.Height,
                    System.Drawing.GraphicsUnit.Pixel);
            }
        }

        private int AddStepSoftSilhouetteAndFeatureEdges(
            List<StepSurfaceDrawTriangle> allTriangles,
            System.Drawing.PointF[] projected,
            System.Drawing.Drawing2D.GraphicsPath silhouettePath,
            System.Drawing.Drawing2D.GraphicsPath creasePath,
            System.Drawing.Drawing2D.GraphicsPath boundaryPath,
            out int silhouetteEdges,
            out int boundaryEdges,
            out int creaseEdges)
        {
            silhouetteEdges = 0;
            boundaryEdges = 0;
            creaseEdges = 0;

            if (allTriangles == null || projected == null ||
                silhouettePath == null || creasePath == null || boundaryPath == null ||
                _stepLitePreviewScene == null || _stepLitePreviewScene.Points == null)
            {
                return 0;
            }

            Dictionary<string, StepFeatureEdgeInfo> edges =
                new Dictionary<string, StepFeatureEdgeInfo>(StringComparer.OrdinalIgnoreCase);

            double modelSize = 1.0;

            if (_stepLitePreviewScene.Bounds != null && _stepLitePreviewScene.Bounds.IsValid)
            {
                modelSize = Math.Max(
                    _stepLitePreviewScene.Bounds.SizeX,
                    Math.Max(_stepLitePreviewScene.Bounds.SizeY, _stepLitePreviewScene.Bounds.SizeZ));
            }

            double quantizeTolerance = Math.Max(0.000001, modelSize * 0.000001);

            foreach (StepSurfaceDrawTriangle triangle in allTriangles)
            {
                if (triangle == null || triangle.Triangle == null)
                {
                    continue;
                }

                AddStepFeatureEdgeCandidateByGeometry(
                    edges,
                    triangle.Triangle.A,
                    triangle.Triangle.B,
                    triangle,
                    quantizeTolerance);

                AddStepFeatureEdgeCandidateByGeometry(
                    edges,
                    triangle.Triangle.B,
                    triangle.Triangle.C,
                    triangle,
                    quantizeTolerance);

                AddStepFeatureEdgeCandidateByGeometry(
                    edges,
                    triangle.Triangle.C,
                    triangle.Triangle.A,
                    triangle,
                    quantizeTolerance);
            }

            int drawn = 0;

            foreach (StepFeatureEdgeInfo edge in edges.Values)
            {
                if (edge == null || edge.FrontFacingCount <= 0)
                {
                    continue;
                }

                System.Drawing.Drawing2D.GraphicsPath targetPath = null;

                if (edge.FrontFacingCount > 0 && edge.BackFacingCount > 0)
                {
                    silhouetteEdges++;
                    targetPath = silhouettePath;
                }
                else if (edge.Count == 1)
                {
                    boundaryEdges++;
                    targetPath = boundaryPath;
                }
                else if (edge.Count > 2)
                {
                    creaseEdges++;
                    targetPath = creasePath;
                }
                else if (edge.FirstTriangle != null && edge.SecondTriangle != null &&
                         !edge.FirstTriangle.BackFacing && !edge.SecondTriangle.BackFacing)
                {
                    double dot =
                        edge.FirstTriangle.NormalX * edge.SecondTriangle.NormalX +
                        edge.FirstTriangle.NormalY * edge.SecondTriangle.NormalY +
                        edge.FirstTriangle.NormalZ * edge.SecondTriangle.NormalZ;

                    dot = Math.Abs(dot);

                    if (dot < 0.975)
                    {
                        creaseEdges++;
                        targetPath = creasePath;
                    }
                }

                if (targetPath == null)
                {
                    continue;
                }

                if (edge.A < 0 || edge.B < 0 || edge.A >= projected.Length || edge.B >= projected.Length)
                {
                    continue;
                }

                targetPath.StartFigure();
                targetPath.AddLine(projected[edge.A], projected[edge.B]);
                drawn++;
            }

            return drawn;
        }

        private void AddStepFeatureEdgeCandidateByGeometry(
            Dictionary<string, StepFeatureEdgeInfo> edges,
            int a,
            int b,
            StepSurfaceDrawTriangle triangle,
            double tolerance)
        {
            if (edges == null || triangle == null ||
                _stepLitePreviewScene == null || _stepLitePreviewScene.Points == null ||
                a < 0 || b < 0 ||
                a >= _stepLitePreviewScene.Points.Count ||
                b >= _stepLitePreviewScene.Points.Count)
            {
                return;
            }

            StepPoint3 pa = _stepLitePreviewScene.Points[a];
            StepPoint3 pb = _stepLitePreviewScene.Points[b];

            if (pa == null || pb == null)
            {
                return;
            }

            string keyA = MakeStepQuantizedPointKey(pa, tolerance);
            string keyB = MakeStepQuantizedPointKey(pb, tolerance);

            int drawA = a;
            int drawB = b;
            string edgeKey;

            if (string.CompareOrdinal(keyA, keyB) <= 0)
            {
                edgeKey = keyA + "|" + keyB;
            }
            else
            {
                edgeKey = keyB + "|" + keyA;
                drawA = b;
                drawB = a;
            }

            StepFeatureEdgeInfo info;

            if (!edges.TryGetValue(edgeKey, out info))
            {
                info = new StepFeatureEdgeInfo();
                info.A = drawA;
                info.B = drawB;
                edges[edgeKey] = info;
            }

            info.Count++;

            if (triangle.BackFacing)
            {
                info.BackFacingCount++;
            }
            else
            {
                info.FrontFacingCount++;
            }

            if (info.FirstTriangle == null)
            {
                info.FirstTriangle = triangle;
            }
            else if (info.SecondTriangle == null)
            {
                info.SecondTriangle = triangle;
            }
        }

        private static string MakeStepQuantizedPointKey(StepPoint3 point, double tolerance)
        {
            if (point == null)
            {
                return "null";
            }

            double safeTolerance = Math.Max(0.000000001, tolerance);
            long qx = (long)Math.Round(point.X / safeTolerance);
            long qy = (long)Math.Round(point.Y / safeTolerance);
            long qz = (long)Math.Round(point.Z / safeTolerance);

            return qx.ToString(CultureInfo.InvariantCulture) + "," +
                   qy.ToString(CultureInfo.InvariantCulture) + "," +
                   qz.ToString(CultureInfo.InvariantCulture);
        }

        private void DrawStepSoftContactShadow(
            System.Drawing.Graphics g,
            System.Drawing.Rectangle bounds,
            double centerX,
            double centerY,
            double centerZ,
            double size)
        {
            if (g == null || _stepLitePreviewScene == null ||
                _stepLitePreviewScene.Bounds == null || !_stepLitePreviewScene.Bounds.IsValid)
            {
                return;
            }

            StepBox3 b = _stepLitePreviewScene.Bounds;
            System.Drawing.PointF[] footprint = new System.Drawing.PointF[]
            {
                ProjectPrimitivePoint(b.MinX, b.MinY, b.MinZ, bounds, centerX, centerY, centerZ, size),
                ProjectPrimitivePoint(b.MaxX, b.MinY, b.MinZ, bounds, centerX, centerY, centerZ, size),
                ProjectPrimitivePoint(b.MaxX, b.MinY, b.MaxZ, bounds, centerX, centerY, centerZ, size),
                ProjectPrimitivePoint(b.MinX, b.MinY, b.MaxZ, bounds, centerX, centerY, centerZ, size)
            };

            DrawStepSoftShadowLayer(g, footprint, 3.0F, 4.0F, 13);
            DrawStepSoftShadowLayer(g, footprint, 5.0F, 7.0F, 9);
            DrawStepSoftShadowLayer(g, footprint, 7.0F, 10.0F, 6);
            DrawStepSoftShadowLayer(g, footprint, 9.0F, 13.0F, 3);
        }

        private static void DrawStepSoftShadowLayer(
            System.Drawing.Graphics g,
            System.Drawing.PointF[] source,
            float offsetX,
            float offsetY,
            int alpha)
        {
            if (g == null || source == null || source.Length < 3)
            {
                return;
            }

            System.Drawing.PointF[] shadow = new System.Drawing.PointF[source.Length];

            for (int i = 0; i < source.Length; i++)
            {
                shadow[i] = new System.Drawing.PointF(
                    source[i].X + offsetX,
                    source[i].Y + offsetY);
            }

            using (System.Drawing.Brush shadowBrush =
                new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(alpha, 30, 35, 42)))
            {
                g.FillPolygon(shadowBrush, shadow);
            }
        }

        private List<StepSurfaceDrawTriangle> BuildStepSurfaceDrawTriangles(
            System.Drawing.PointF[] projected,
            double centerX,
            double centerY,
            double centerZ,
            StepRenderMethodProfile renderProfile,
            out int backFacingTriangles,
            out int degenerateTriangles,
            out int smoothVertexKeys)
        {
            backFacingTriangles = 0;
            degenerateTriangles = 0;
            smoothVertexKeys = 0;

            List<StepSurfaceDrawTriangle> result = new List<StepSurfaceDrawTriangle>();

            if (_stepLitePreviewScene == null ||
                _stepLitePreviewScene.Triangles == null ||
                _stepLitePreviewScene.Points == null ||
                projected == null)
            {
                return result;
            }

            double modelSize = 1.0;

            if (_stepLitePreviewScene.Bounds != null &&
                _stepLitePreviewScene.Bounds.IsValid)
            {
                modelSize = Math.Max(
                    _stepLitePreviewScene.Bounds.SizeX,
                    Math.Max(
                        _stepLitePreviewScene.Bounds.SizeY,
                        _stepLitePreviewScene.Bounds.SizeZ));
            }

            double quantizeTolerance = Math.Max(0.000001, modelSize * 0.000001);

            Dictionary<string, List<StepNormalSample>> normalsByVertexKey =
                new Dictionary<string, List<StepNormalSample>>(
                    StringComparer.OrdinalIgnoreCase);

            foreach (StepTriangle triangle in _stepLitePreviewScene.Triangles)
            {
                if (!IsValidStepTriangle(triangle, projected.Length))
                {
                    degenerateTriangles++;
                    continue;
                }

                StepPoint3 pa = _stepLitePreviewScene.Points[triangle.A];
                StepPoint3 pb = _stepLitePreviewScene.Points[triangle.B];
                StepPoint3 pc = _stepLitePreviewScene.Points[triangle.C];

                if (pa == null || pb == null || pc == null)
                {
                    degenerateTriangles++;
                    continue;
                }

                double ax;
                double ay;
                double az;
                double bx;
                double by;
                double bz;
                double cx;
                double cy;
                double cz;

                ProjectPrimitivePointView(
                    pa.X, pa.Y, pa.Z,
                    centerX, centerY, centerZ,
                    out ax, out ay, out az);

                ProjectPrimitivePointView(
                    pb.X, pb.Y, pb.Z,
                    centerX, centerY, centerZ,
                    out bx, out by, out bz);

                ProjectPrimitivePointView(
                    pc.X, pc.Y, pc.Z,
                    centerX, centerY, centerZ,
                    out cx, out cy, out cz);

                double ux = bx - ax;
                double uy = by - ay;
                double uz = bz - az;
                double vx = cx - ax;
                double vy = cy - ay;
                double vz = cz - az;

                double nx = uy * vz - uz * vy;
                double ny = uz * vx - ux * vz;
                double nz = ux * vy - uy * vx;
                double normalLen = Math.Sqrt(nx * nx + ny * ny + nz * nz);

                if (normalLen < 0.000000001)
                {
                    degenerateTriangles++;
                    continue;
                }

                // Camera convention: ProjectPrimitivePointView and Z-buffer use greater view-Z as nearer.
                // Historical methods keep their legacy rule. v0.5.06 explicitly fixes the convention:
                // NormalZ > 0 faces the camera, therefore only NormalZ < 0 is back-facing.
                bool backFacing = renderProfile != null && renderProfile.FrontFacingPositiveViewZ
                    ? nz < 0.0
                    : nz > 0.0;

                if (backFacing)
                {
                    backFacingTriangles++;
                }

                double invLen = 1.0 / normalLen;
                double nnx = nx * invLen;
                double nny = ny * invLen;
                double nnz = nz * invLen;

                string keyA = MakeStepQuantizedPointKey(pa, quantizeTolerance);
                string keyB = MakeStepQuantizedPointKey(pb, quantizeTolerance);
                string keyC = MakeStepQuantizedPointKey(pc, quantizeTolerance);

                AddStepNormalSample(
                    normalsByVertexKey,
                    keyA,
                    nnx,
                    nny,
                    nnz,
                    normalLen);

                AddStepNormalSample(
                    normalsByVertexKey,
                    keyB,
                    nnx,
                    nny,
                    nnz,
                    normalLen);

                AddStepNormalSample(
                    normalsByVertexKey,
                    keyC,
                    nnx,
                    nny,
                    nnz,
                    normalLen);

                StepSurfaceDrawTriangle drawTriangle =
                    new StepSurfaceDrawTriangle();

                drawTriangle.Triangle = triangle;
                drawTriangle.Poly = new System.Drawing.PointF[]
                {
                    projected[triangle.A],
                    projected[triangle.B],
                    projected[triangle.C]
                };
                drawTriangle.Depth = (az + bz + cz) / 3.0;
                drawTriangle.BackFacing = backFacing;
                drawTriangle.NormalX = nnx;
                drawTriangle.NormalY = nny;
                drawTriangle.NormalZ = nnz;
                drawTriangle.VertexKeyA = keyA;
                drawTriangle.VertexKeyB = keyB;
                drawTriangle.VertexKeyC = keyC;

                result.Add(drawTriangle);
            }

            smoothVertexKeys = normalsByVertexKey.Count;

            foreach (StepSurfaceDrawTriangle drawTriangle in result)
            {
                if (drawTriangle == null)
                {
                    continue;
                }

                drawTriangle.ShadeA = ComputeStepSmoothVertexShade(
                    normalsByVertexKey,
                    drawTriangle.VertexKeyA,
                    drawTriangle.NormalX,
                    drawTriangle.NormalY,
                    drawTriangle.NormalZ,
                    drawTriangle.BackFacing,
                    renderProfile);

                drawTriangle.ShadeB = ComputeStepSmoothVertexShade(
                    normalsByVertexKey,
                    drawTriangle.VertexKeyB,
                    drawTriangle.NormalX,
                    drawTriangle.NormalY,
                    drawTriangle.NormalZ,
                    drawTriangle.BackFacing,
                    renderProfile);

                drawTriangle.ShadeC = ComputeStepSmoothVertexShade(
                    normalsByVertexKey,
                    drawTriangle.VertexKeyC,
                    drawTriangle.NormalX,
                    drawTriangle.NormalY,
                    drawTriangle.NormalZ,
                    drawTriangle.BackFacing,
                    renderProfile);

                drawTriangle.Shade =
                    (drawTriangle.ShadeA +
                     drawTriangle.ShadeB +
                     drawTriangle.ShadeC) / 3;
            }

            return result;
        }

        private static void AddStepNormalSample(
            Dictionary<string, List<StepNormalSample>> normalsByVertexKey,
            string key,
            double x,
            double y,
            double z,
            double weight)
        {
            if (normalsByVertexKey == null || string.IsNullOrEmpty(key))
            {
                return;
            }

            List<StepNormalSample> samples;

            if (!normalsByVertexKey.TryGetValue(key, out samples))
            {
                samples = new List<StepNormalSample>();
                normalsByVertexKey[key] = samples;
            }

            StepNormalSample sample = new StepNormalSample();
            sample.X = x;
            sample.Y = y;
            sample.Z = z;
            sample.Weight = Math.Max(0.000001, weight);
            samples.Add(sample);
        }

        private static int ComputeStepSmoothVertexShade(
            Dictionary<string, List<StepNormalSample>> normalsByVertexKey,
            string key,
            double faceX,
            double faceY,
            double faceZ,
            bool backFacing,
            StepRenderMethodProfile renderProfile)
        {
            double sumX = 0.0;
            double sumY = 0.0;
            double sumZ = 0.0;
            double sumWeight = 0.0;
            const double creaseDotThreshold = 0.72;

            List<StepNormalSample> samples;

            if (normalsByVertexKey != null &&
                !string.IsNullOrEmpty(key) &&
                normalsByVertexKey.TryGetValue(key, out samples))
            {
                foreach (StepNormalSample sample in samples)
                {
                    if (sample == null)
                    {
                        continue;
                    }

                    double dot =
                        faceX * sample.X +
                        faceY * sample.Y +
                        faceZ * sample.Z;

                    if (dot < creaseDotThreshold)
                    {
                        continue;
                    }

                    double weight = Math.Max(0.000001, sample.Weight);
                    sumX += sample.X * weight;
                    sumY += sample.Y * weight;
                    sumZ += sample.Z * weight;
                    sumWeight += weight;
                }
            }

            if (sumWeight <= 0.000001)
            {
                sumX = faceX;
                sumY = faceY;
                sumZ = faceZ;
            }

            double length = Math.Sqrt(
                sumX * sumX +
                sumY * sumY +
                sumZ * sumZ);

            if (length <= 0.000000001)
            {
                sumX = faceX;
                sumY = faceY;
                sumZ = faceZ;
                length = Math.Sqrt(
                    sumX * sumX +
                    sumY * sumY +
                    sumZ * sumZ);
            }

            if (length > 0.000000001)
            {
                sumX /= length;
                sumY /= length;
                sumZ /= length;
            }

            return ComputeStepNormalShade(
                sumX,
                sumY,
                sumZ,
                backFacing,
                renderProfile);
        }

        private static int ComputeStepNormalShade(
            double normalX,
            double normalY,
            double normalZ,
            bool backFacing,
            StepRenderMethodProfile renderProfile)
        {
            if (renderProfile == null)
            {
                renderProfile = GetStepRenderMethodProfile("SurfaceMesh");
            }

            double brightness;

            if (string.Equals(renderProfile.MethodId, "TwoLightKeyFill", StringComparison.OrdinalIgnoreCase))
            {
                double keyDot = Math.Max(0.0, normalX * -0.42 + normalY * 0.52 + normalZ * -0.74);
                double fillDot = Math.Max(0.0, normalX * 0.58 + normalY * 0.18 + normalZ * -0.52);
                double facingDot = Math.Max(0.0, -normalZ);
                double topDot = Math.Max(0.0, normalY);

                brightness =
                    0.05 +
                    0.48 * keyDot +
                    0.18 * fillDot +
                    0.18 * facingDot +
                    0.11 * topDot;

                if (backFacing)
                {
                    brightness *= 0.58;
                }

                return (int)Math.Round(96.0 + ClampStepUnit(brightness) * 128.0);
            }

            if (string.Equals(renderProfile.MethodId, "HemisphereKey", StringComparison.OrdinalIgnoreCase))
            {
                double keyDot = Math.Max(0.0, normalX * -0.32 + normalY * 0.45 + normalZ * -0.83);
                double hemisphere = Math.Max(0.0, Math.Min(1.0, 0.5 + 0.5 * normalY));
                double facingDot = Math.Max(0.0, -normalZ);
                double topDot = Math.Max(0.0, normalY);

                brightness =
                    0.04 +
                    0.42 * keyDot +
                    0.30 * hemisphere +
                    0.18 * facingDot +
                    0.06 * topDot;

                if (backFacing)
                {
                    brightness *= 0.60;
                }

                return (int)Math.Round(100.0 + ClampStepUnit(brightness) * 120.0);
            }

            if (string.Equals(renderProfile.MethodId, "TrueFrontFaceZBufferV0506", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(renderProfile.MethodId, "TrueFrontFaceCompileFixV0507", StringComparison.OrdinalIgnoreCase))
            {
                // Correct camera-space convention: the camera is on positive view-Z and greater Z is nearer.
                // Therefore the facing and directional-light terms use +normalZ, not the legacy -normalZ.
                double keyDot = Math.Max(0.0, normalX * -0.38 + normalY * 0.54 + normalZ * 0.75);
                double fillDot = Math.Max(0.0, normalX * 0.50 + normalY * 0.10 + normalZ * 0.48);
                double facingDot = Math.Max(0.0, normalZ);
                double topDot = Math.Max(0.0, normalY);
                double rim = Math.Max(0.0, 1.0 - facingDot);
                rim *= rim;

                brightness =
                    0.065 +
                    0.43 * keyDot +
                    0.10 * fillDot +
                    0.25 * facingDot +
                    0.10 * topDot +
                    0.025 * rim;

                if (backFacing)
                {
                    brightness *= 0.50;
                }

                brightness = Math.Pow(ClampStepUnit(brightness), 1.04);
                return (int)Math.Round(165.0 + brightness * 65.0);
            }

            if (string.Equals(renderProfile.MethodId, "InventorSolidFrontV0505", StringComparison.OrdinalIgnoreCase))
            {
                double keyDot = Math.Max(0.0, normalX * -0.38 + normalY * 0.54 + normalZ * -0.75);
                double fillDot = Math.Max(0.0, normalX * 0.50 + normalY * 0.10 + normalZ * -0.48);
                double facingDot = Math.Max(0.0, -normalZ);
                double topDot = Math.Max(0.0, normalY);
                double rim = Math.Max(0.0, 1.0 - facingDot);
                rim *= rim;

                brightness =
                    0.075 +
                    0.43 * keyDot +
                    0.10 * fillDot +
                    0.24 * facingDot +
                    0.11 * topDot +
                    0.025 * rim;

                if (backFacing)
                {
                    brightness *= 0.56;
                }

                brightness = Math.Pow(ClampStepUnit(brightness), 1.06);
                return (int)Math.Round(102.0 + brightness * 116.0);
            }

            if (string.Equals(renderProfile.MethodId, "InventorCadKeyFillRim", StringComparison.OrdinalIgnoreCase))
            {
                double keyDot = Math.Max(0.0, normalX * -0.40 + normalY * 0.56 + normalZ * -0.72);
                double fillDot = Math.Max(0.0, normalX * 0.55 + normalY * 0.08 + normalZ * -0.50);
                double facingDot = Math.Max(0.0, -normalZ);
                double topDot = Math.Max(0.0, normalY);
                double rim = Math.Max(0.0, 1.0 - facingDot);
                rim *= rim;

                brightness =
                    0.035 +
                    0.46 * keyDot +
                    0.14 * fillDot +
                    0.21 * facingDot +
                    0.11 * topDot +
                    0.03 * rim;

                if (backFacing)
                {
                    brightness *= 0.54;
                }

                brightness = Math.Pow(ClampStepUnit(brightness), 1.15);
                return (int)Math.Round(94.0 + brightness * 130.0);
            }

            // v0.5.01 single-key equation. The DeepClamp method uses the same equation,
            // while its profile removes the accidental 145..214 raster pre-clamp.
            double lightX = -0.34;
            double lightY = 0.46;
            double lightZ = -0.82;

            double lightDot = Math.Max(
                0.0,
                normalX * lightX +
                normalY * lightY +
                normalZ * lightZ);

            double baselineFacingDot = Math.Max(0.0, -normalZ);
            double baselineTopDot = Math.Max(0.0, normalY);

            brightness =
                0.08 +
                0.54 * lightDot +
                0.22 * baselineFacingDot +
                0.16 * baselineTopDot;

            if (backFacing)
            {
                brightness *= 0.62;
            }

            return (int)Math.Round(106.0 + ClampStepUnit(brightness) * 112.0);
        }

        private static double ClampStepUnit(double value)
        {
            return Math.Max(0.0, Math.Min(1.0, value));
        }

        private void ProjectPrimitivePointView(
            double x,
            double y,
            double z,
            double centerX,
            double centerY,
            double centerZ,
            out double viewX,
            out double viewY,
            out double viewZ)
        {
            double px = x - centerX;
            double py = y - centerY;
            double pz = z - centerZ;

            double yaw = _primitiveViewerYaw * Math.PI / 180.0;
            double pitch = _primitiveViewerPitch * Math.PI / 180.0;
            double cosY = Math.Cos(yaw);
            double sinY = Math.Sin(yaw);
            double cosP = Math.Cos(pitch);
            double sinP = Math.Sin(pitch);

            double rx = px * cosY + pz * sinY;
            double rz = -px * sinY + pz * cosY;
            double ry = py * cosP - rz * sinP;
            double rz2 = py * sinP + rz * cosP;

            viewX = rx;
            viewY = ry;
            viewZ = rz2;
        }

        private void DrawStepLiteBoundsSilhouette(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, double centerX, double centerY, double centerZ, double size)
        {
            if (_stepLitePreviewScene == null || _stepLitePreviewScene.Bounds == null || !_stepLitePreviewScene.Bounds.IsValid)
            {
                return;
            }

            StepBox3 b = _stepLitePreviewScene.Bounds;
            using (System.Drawing.Pen silhouettePen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(75, 70, 70, 70), 0.75F))
            {
                System.Drawing.PointF[] p = new System.Drawing.PointF[]
                {
                    ProjectPrimitivePoint(b.MinX, b.MinY, b.MinZ, bounds, centerX, centerY, centerZ, size),
                    ProjectPrimitivePoint(b.MaxX, b.MinY, b.MinZ, bounds, centerX, centerY, centerZ, size),
                    ProjectPrimitivePoint(b.MaxX, b.MaxY, b.MinZ, bounds, centerX, centerY, centerZ, size),
                    ProjectPrimitivePoint(b.MinX, b.MaxY, b.MinZ, bounds, centerX, centerY, centerZ, size),
                    ProjectPrimitivePoint(b.MinX, b.MinY, b.MaxZ, bounds, centerX, centerY, centerZ, size),
                    ProjectPrimitivePoint(b.MaxX, b.MinY, b.MaxZ, bounds, centerX, centerY, centerZ, size),
                    ProjectPrimitivePoint(b.MaxX, b.MaxY, b.MaxZ, bounds, centerX, centerY, centerZ, size),
                    ProjectPrimitivePoint(b.MinX, b.MaxY, b.MaxZ, bounds, centerX, centerY, centerZ, size)
                };

                g.DrawLine(silhouettePen, p[0], p[1]);
                g.DrawLine(silhouettePen, p[1], p[2]);
                g.DrawLine(silhouettePen, p[2], p[3]);
                g.DrawLine(silhouettePen, p[3], p[0]);
                g.DrawLine(silhouettePen, p[4], p[5]);
                g.DrawLine(silhouettePen, p[5], p[6]);
                g.DrawLine(silhouettePen, p[6], p[7]);
                g.DrawLine(silhouettePen, p[7], p[4]);
                g.DrawLine(silhouettePen, p[0], p[4]);
                g.DrawLine(silhouettePen, p[1], p[5]);
                g.DrawLine(silhouettePen, p[2], p[6]);
                g.DrawLine(silhouettePen, p[3], p[7]);
            }
        }

        private void DrawBoxGridOverlayOnStepViewer(System.Drawing.Graphics g, System.Drawing.Rectangle bounds)
        {
            if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady || _spatialCubesIndex.ModelBox == null)
            {
                return;
            }

            SpatialBox modelBox = _spatialCubesIndex.ModelBox;
            double cx = (modelBox.MinX + modelBox.MaxX) * 0.5;
            double cy = (modelBox.MinY + modelBox.MaxY) * 0.5;
            double cz = (modelBox.MinZ + modelBox.MaxZ) * 0.5;
            double size = Math.Max(modelBox.SizeX, Math.Max(modelBox.SizeY, modelBox.SizeZ));

            if (size <= 0.000001)
            {
                return;
            }

            using (System.Drawing.Pen gridPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(95, 60, 130, 220), 0.8F))
            using (System.Drawing.Pen modelPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(130, 60, 130, 220), 1.2F))
            {
                DrawPrimitiveBox(g, modelBox, modelPen, bounds, cx, cy, cz, size, true);

                if (_spatialCubesIndex.Cells != null)
                {
                    foreach (SpatialCubeCell cell in _spatialCubesIndex.Cells)
                    {
                        if (cell != null && cell.Bounds != null)
                        {
                            DrawPrimitiveBox(g, cell.Bounds, gridPen, bounds, cx, cy, cz, size, false);
                        }
                    }
                }
            }
        }


private void PrepareStepLiteProjectionFit(System.Drawing.Rectangle bounds, double centerX, double centerY, double centerZ)
{
    int fpsLevel = GetViewerFpsModeLevel();
    if (fpsLevel <= 8)
    {
        PrepareStepLiteProjectionFitLegacyV0508(bounds, centerX, centerY, centerZ);
    }
    else if (fpsLevel == 9)
    {
        PrepareStepLiteProjectionFitCenterLockV0509(bounds, centerX, centerY, centerZ);
    }
    else
    {
        PrepareStepLiteProjectionFitVisualCenterV0510(bounds, centerX, centerY, centerZ);
    }
}

private void AccumulateStepLiteProjectionBounds(
    double centerX, double centerY, double centerZ,
    ref double minX, ref double maxX, ref double minY, ref double maxY, ref int pointCount)
{
    if (_stepLitePreviewScene == null || _stepLitePreviewScene.Points == null) { return; }
    foreach (StepPoint3 point in _stepLitePreviewScene.Points)
    {
        if (point == null) { continue; }
        AccumulatePrimitivePointProjection(point.X, point.Y, point.Z, centerX, centerY, centerZ,
            ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
    }
}

private void PrepareStepLiteProjectionFitLegacyV0508(System.Drawing.Rectangle bounds, double centerX, double centerY, double centerZ)
{
    _primitiveProjectionFitReady = false;
    double minX = double.MaxValue, maxX = double.MinValue, minY = double.MaxValue, maxY = double.MinValue;
    int pointCount = 0;
    AccumulateStepLiteProjectionBounds(centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
    if (pointCount <= 0) { return; }

    double rangeX = Math.Max(0.000001, maxX - minX);
    double rangeY = Math.Max(0.000001, maxY - minY);
    double interactiveScale = GetViewerFpsModeLevel() <= 7 ? 1.0 : GetViewerProjectionInteractiveScale();
    int marginLeft = Math.Max(8, (int)Math.Round(34.0 * interactiveScale));
    int marginTop = Math.Max(12, (int)Math.Round(74.0 * interactiveScale));
    int marginHorizontal = Math.Max(16, (int)Math.Round(68.0 * interactiveScale));
    int marginVertical = Math.Max(24, (int)Math.Round(124.0 * interactiveScale));
    System.Drawing.Rectangle viewport = new System.Drawing.Rectangle(
        bounds.Left + marginLeft, bounds.Top + marginTop,
        Math.Max(10, bounds.Width - marginHorizontal), Math.Max(10, bounds.Height - marginVertical));
    double scale = Math.Min(viewport.Width / rangeX, viewport.Height / rangeY) * 0.92 * _primitiveViewerZoom;
    double rawCenterX = (minX + maxX) * 0.5;
    double rawCenterY = (minY + maxY) * 0.5;
    _primitiveProjectionScale = scale;
    _primitiveProjectionOffsetX = viewport.Left + viewport.Width * 0.5 - rawCenterX * scale + _primitiveViewerPanX * interactiveScale;
    _primitiveProjectionOffsetY = viewport.Top + viewport.Height * 0.5 + rawCenterY * scale + _primitiveViewerPanY * interactiveScale;
    _primitiveProjectionFitReady = true;
}

private void PrepareStepLiteProjectionFitCenterLockV0509(System.Drawing.Rectangle bounds, double centerX, double centerY, double centerZ)
{
    _primitiveProjectionFitReady = false;
    double minX = double.MaxValue, maxX = double.MinValue, minY = double.MaxValue, maxY = double.MinValue;
    int pointCount = 0;
    AccumulateStepLiteProjectionBounds(centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
    if (pointCount <= 0) { return; }

    double extentX = Math.Max(0.000001, Math.Max(Math.Abs(minX), Math.Abs(maxX)));
    double extentY = Math.Max(0.000001, Math.Max(Math.Abs(minY), Math.Abs(maxY)));
    double interactiveScale = GetViewerProjectionInteractiveScale();
    int marginLeft = Math.Max(8, (int)Math.Round(34.0 * interactiveScale));
    int marginTop = Math.Max(12, (int)Math.Round(74.0 * interactiveScale));
    int marginHorizontal = Math.Max(16, (int)Math.Round(68.0 * interactiveScale));
    int marginVertical = Math.Max(24, (int)Math.Round(124.0 * interactiveScale));
    System.Drawing.Rectangle viewport = new System.Drawing.Rectangle(
        bounds.Left + marginLeft, bounds.Top + marginTop,
        Math.Max(10, bounds.Width - marginHorizontal), Math.Max(10, bounds.Height - marginVertical));
    double scale = Math.Min(viewport.Width / (extentX * 2.0), viewport.Height / (extentY * 2.0)) * 0.92 * _primitiveViewerZoom;
    double screenCenterX = viewport.Left + viewport.Width * 0.5;
    double screenCenterY = viewport.Top + viewport.Height * 0.5;
    _primitiveProjectionScale = scale;
    _primitiveProjectionOffsetX = screenCenterX + _primitiveViewerPanX * interactiveScale;
    _primitiveProjectionOffsetY = screenCenterY + _primitiveViewerPanY * interactiveScale;
    _primitiveProjectionRawMinX = minX; _primitiveProjectionRawMaxX = maxX;
    _primitiveProjectionRawMinY = minY; _primitiveProjectionRawMaxY = maxY;
    _primitiveProjectionFitReady = true;

    if (!AppLogger.SuppressHighFrequencyViewerLogs && !_viewerRenderingReducedDragFrame)
    {
        AppLogger.Log("STEP_VIEW_CAMERA_CENTER", "PrepareStepLiteProjectionFitCenterLockV0509",
            "FitMode=SymmetricModelCenter; OrbitTarget=" + centerX.ToString("0.###", CultureInfo.InvariantCulture) + "," +
            centerY.ToString("0.###", CultureInfo.InvariantCulture) + "," + centerZ.ToString("0.###", CultureInfo.InvariantCulture) +
            "; ScreenCenter=" + screenCenterX.ToString("0.###", CultureInfo.InvariantCulture) + "," + screenCenterY.ToString("0.###", CultureInfo.InvariantCulture) +
            "; Scale=" + scale.ToString("0.###", CultureInfo.InvariantCulture) + "; DragMethodVersion=" + GetViewerFpsModeVersion());
    }
}

private void PrepareStepLiteProjectionFitVisualCenterV0510(System.Drawing.Rectangle bounds, double centerX, double centerY, double centerZ)
{
    _primitiveProjectionFitReady = false;
    double minX = double.MaxValue, maxX = double.MinValue, minY = double.MaxValue, maxY = double.MinValue;
    int pointCount = 0;
    AccumulateStepLiteProjectionBounds(centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
    if (pointCount <= 0) { return; }

    double rangeX = Math.Max(0.000001, maxX - minX);
    double rangeY = Math.Max(0.000001, maxY - minY);
    double rawVisualCenterX = (minX + maxX) * 0.5;
    double rawVisualCenterY = (minY + maxY) * 0.5;
    double interactiveScale = GetViewerProjectionInteractiveScale();
    int marginLeft = Math.Max(8, (int)Math.Round(34.0 * interactiveScale));
    int marginTop = Math.Max(12, (int)Math.Round(74.0 * interactiveScale));
    int marginHorizontal = Math.Max(16, (int)Math.Round(68.0 * interactiveScale));
    int marginVertical = Math.Max(24, (int)Math.Round(124.0 * interactiveScale));
    System.Drawing.Rectangle viewport = new System.Drawing.Rectangle(
        bounds.Left + marginLeft, bounds.Top + marginTop,
        Math.Max(10, bounds.Width - marginHorizontal), Math.Max(10, bounds.Height - marginVertical));
    double scale = Math.Min(viewport.Width / rangeX, viewport.Height / rangeY) * 0.88 * _primitiveViewerZoom;
    double screenCenterX = viewport.Left + viewport.Width * 0.5;
    double screenCenterY = viewport.Top + viewport.Height * 0.5;
    double panX = _primitiveViewerPanX * interactiveScale;
    double panY = _primitiveViewerPanY * interactiveScale;
    _primitiveProjectionScale = scale;
    _primitiveProjectionOffsetX = screenCenterX - rawVisualCenterX * scale + panX;
    _primitiveProjectionOffsetY = screenCenterY + rawVisualCenterY * scale + panY;
    double screenMinX = _primitiveProjectionOffsetX + minX * scale;
    double screenMaxX = _primitiveProjectionOffsetX + maxX * scale;
    double screenMinY = _primitiveProjectionOffsetY - maxY * scale;
    double screenMaxY = _primitiveProjectionOffsetY - minY * scale;
    double projectedScreenCenterX = (screenMinX + screenMaxX) * 0.5;
    double projectedScreenCenterY = (screenMinY + screenMaxY) * 0.5;
    double correctionX = (screenCenterX + panX) - projectedScreenCenterX;
    double correctionY = (screenCenterY + panY) - projectedScreenCenterY;
    _primitiveProjectionOffsetX += correctionX;
    _primitiveProjectionOffsetY += correctionY;
    screenMinX += correctionX; screenMaxX += correctionX;
    screenMinY += correctionY; screenMaxY += correctionY;
    projectedScreenCenterX += correctionX; projectedScreenCenterY += correctionY;
    _primitiveProjectionRawMinX = minX; _primitiveProjectionRawMaxX = maxX;
    _primitiveProjectionRawMinY = minY; _primitiveProjectionRawMaxY = maxY;
    _primitiveProjectionFitReady = true;

    if (!AppLogger.SuppressHighFrequencyViewerLogs && !_viewerRenderingReducedDragFrame)
    {
        AppLogger.Log("STEP_VIEW_CAMERA_CENTER", "PrepareStepLiteProjectionFitVisualCenterV0510",
            "FitMode=ProjectedVisualCenterPixelLock; OrbitTarget=" + centerX.ToString("0.###", CultureInfo.InvariantCulture) + "," +
            centerY.ToString("0.###", CultureInfo.InvariantCulture) + "," + centerZ.ToString("0.###", CultureInfo.InvariantCulture) +
            "; ScreenTarget=" + (screenCenterX + panX).ToString("0.###", CultureInfo.InvariantCulture) + "," + (screenCenterY + panY).ToString("0.###", CultureInfo.InvariantCulture) +
            "; ProjectedScreenCenter=" + projectedScreenCenterX.ToString("0.###", CultureInfo.InvariantCulture) + "," + projectedScreenCenterY.ToString("0.###", CultureInfo.InvariantCulture) +
            "; ScreenBounds=" + screenMinX.ToString("0.###", CultureInfo.InvariantCulture) + ".." + screenMaxX.ToString("0.###", CultureInfo.InvariantCulture) +
            "," + screenMinY.ToString("0.###", CultureInfo.InvariantCulture) + ".." + screenMaxY.ToString("0.###", CultureInfo.InvariantCulture) +
            "; Scale=" + scale.ToString("0.###", CultureInfo.InvariantCulture) + "; VisibleViewport=" + ViewerFpsUsesVisibleViewport().ToString() +
            "; DragMethodVersion=" + GetViewerFpsModeVersion());
    }
}

        private void DrawMeshScenePreview(System.Drawing.Graphics g, System.Drawing.Rectangle bounds)
        {
            if (_meshPreviewScene == null || _meshPreviewScene.Bounds == null)
            {
                DrawPrimitivePlaceholder(g, bounds);
                return;
            }

            using (System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
            using (System.Drawing.Brush softTextBrush = new System.Drawing.SolidBrush(System.Drawing.Color.DimGray))
            using (System.Drawing.Font titleFont = new System.Drawing.Font("Segoe UI", 11.0F, System.Drawing.FontStyle.Bold))
            using (System.Drawing.Font infoFont = new System.Drawing.Font("Segoe UI", 8.5F))
            {
                g.DrawString("Mesh preview", titleFont, textBrush, 16, 14);
                g.DrawString(
                    "Bodies=" + _meshPreviewScene.BodyCount.ToString() +
                    " | Vertices=" + _meshPreviewScene.VertexCount.ToString() +
                    " | Triangles=" + _meshPreviewScene.TriangleCount.ToString() +
                    " | LMB rotate | Shift+LMB/RMB pan | Wheel zoom | Double-click reset",
                    infoFont,
                    softTextBrush,
                    16,
                    40);
            }

            MeshBox3 box = _meshPreviewScene.Bounds;
            double cx = (box.MinX + box.MaxX) * 0.5;
            double cy = (box.MinY + box.MaxY) * 0.5;
            double cz = (box.MinZ + box.MaxZ) * 0.5;
            double size = Math.Max(box.SizeX, Math.Max(box.SizeY, box.SizeZ));

            if (size <= 0.000001)
            {
                size = 1.0;
            }

            PrepareMeshProjectionFit(bounds, cx, cy, cz);

            int maxTrianglesToDraw = 30000;
            int drawnTriangles = 0;
            int bodyIndex = 0;

            foreach (MeshBody body in _meshPreviewScene.Bodies)
            {
                bodyIndex++;

                if (body == null || body.Vertices == null || body.Triangles == null)
                {
                    continue;
                }

                System.Drawing.Color baseColor = GetPrimitiveBodyColor(bodyIndex);

                using (System.Drawing.Brush fillBrush = new System.Drawing.SolidBrush(System.Drawing.Color.FromArgb(42, baseColor)))
                using (System.Drawing.Pen wirePen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(125, 40, 40, 40), 0.65F))
                {
                    foreach (MeshTriangle triangle in body.Triangles)
                    {
                        if (triangle == null)
                        {
                            continue;
                        }

                        if (triangle.A < 0 || triangle.B < 0 || triangle.C < 0 ||
                            triangle.A >= body.Vertices.Count ||
                            triangle.B >= body.Vertices.Count ||
                            triangle.C >= body.Vertices.Count)
                        {
                            continue;
                        }

                        MeshPoint3 a = body.Vertices[triangle.A];
                        MeshPoint3 b = body.Vertices[triangle.B];
                        MeshPoint3 c = body.Vertices[triangle.C];

                        System.Drawing.PointF[] points = new System.Drawing.PointF[]
                        {
                            ProjectPrimitivePoint(a.X, a.Y, a.Z, bounds, cx, cy, cz, size),
                            ProjectPrimitivePoint(b.X, b.Y, b.Z, bounds, cx, cy, cz, size),
                            ProjectPrimitivePoint(c.X, c.Y, c.Z, bounds, cx, cy, cz, size)
                        };

                        g.FillPolygon(fillBrush, points);
                        g.DrawPolygon(wirePen, points);

                        drawnTriangles++;

                        if (drawnTriangles >= maxTrianglesToDraw)
                        {
                            AppLogger.Log(
                                "MESH_VIEW_DRAW_LIMIT_REACHED",
                                "DrawMeshScenePreview",
                                "DrawnTriangles=" + drawnTriangles.ToString() +
                                "; TotalTriangles=" + _meshPreviewScene.TriangleCount.ToString());
                            return;
                        }
                    }
                }
            }
        }

        private void PrepareMeshProjectionFit(System.Drawing.Rectangle bounds, double centerX, double centerY, double centerZ)
        {
            _primitiveProjectionFitReady = false;

            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            int pointCount = 0;

            if (_meshPreviewScene != null && _meshPreviewScene.Bodies != null)
            {
                foreach (MeshBody body in _meshPreviewScene.Bodies)
                {
                    if (body == null || body.Vertices == null)
                    {
                        continue;
                    }

                    foreach (MeshPoint3 point in body.Vertices)
                    {
                        if (point == null)
                        {
                            continue;
                        }

                        AccumulatePrimitivePointProjection(point.X, point.Y, point.Z, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
                    }
                }
            }

            if (pointCount <= 0)
            {
                return;
            }

            double rangeX = Math.Max(0.000001, maxX - minX);
            double rangeY = Math.Max(0.000001, maxY - minY);

            System.Drawing.Rectangle viewport = new System.Drawing.Rectangle(
                bounds.Left + 34,
                bounds.Top + 74,
                Math.Max(10, bounds.Width - 68),
                Math.Max(10, bounds.Height - 124));

            double scaleX = viewport.Width / rangeX;
            double scaleY = viewport.Height / rangeY;
            double scale = Math.Min(scaleX, scaleY) * 0.92 * _primitiveViewerZoom;

            double rawCenterX = (minX + maxX) * 0.5;
            double rawCenterY = (minY + maxY) * 0.5;

            _primitiveProjectionScale = scale;
            _primitiveProjectionOffsetX = viewport.Left + viewport.Width * 0.5 - rawCenterX * scale + _primitiveViewerPanX;
            _primitiveProjectionOffsetY = viewport.Top + viewport.Height * 0.5 + rawCenterY * scale + _primitiveViewerPanY;
            _primitiveProjectionFitReady = true;
        }

        private void DrawPrimitivePlaceholder(System.Drawing.Graphics g, System.Drawing.Rectangle bounds)
        {
            using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.DimGray, 2.0F))
            using (System.Drawing.Pen lightPen = new System.Drawing.Pen(System.Drawing.Color.Silver, 1.0F))
            using (System.Drawing.Brush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black))
            using (System.Drawing.Font infoFont = new System.Drawing.Font("Segoe UI", 9.0F))
            {
                int boxW = Math.Max(180, Math.Min(bounds.Width / 2, 420));
                int boxH = Math.Max(130, Math.Min(bounds.Height / 2, 300));
                int x = Math.Max(40, (bounds.Width - boxW) / 2 - 40);
                int y = Math.Max(80, (bounds.Height - boxH) / 2);
                System.Drawing.Rectangle front = new System.Drawing.Rectangle(x, y, boxW, boxH);
                System.Drawing.Point offset = new System.Drawing.Point(Math.Max(38, boxW / 5), -Math.Max(28, boxH / 5));
                System.Drawing.Rectangle back = new System.Drawing.Rectangle(front.X + offset.X, front.Y + offset.Y, front.Width, front.Height);

                g.DrawRectangle(lightPen, back);
                g.DrawRectangle(pen, front);
                g.DrawLine(lightPen, front.Left, front.Top, back.Left, back.Top);
                g.DrawLine(lightPen, front.Right, front.Top, back.Right, back.Top);
                g.DrawLine(lightPen, front.Left, front.Bottom, back.Left, back.Bottom);
                g.DrawLine(lightPen, front.Right, front.Bottom, back.Right, back.Bottom);
                g.DrawString("Press 8C Cubes to build Spatial BASE. LMB rotate, Shift+LMB/RMB pan, wheel zoom.", infoFont, textBrush, 24, bounds.Height - 34);
            }
        }

        private void DrawPrimitiveBox(System.Drawing.Graphics g, SpatialBox box, System.Drawing.Pen pen, System.Drawing.Rectangle bounds, double centerX, double centerY, double centerZ, double modelSize, bool drawCenterCross)
        {
            if (box == null)
            {
                return;
            }

            System.Drawing.PointF[] p = new System.Drawing.PointF[8];

            p[0] = ProjectPrimitivePoint(box.MinX, box.MinY, box.MinZ, bounds, centerX, centerY, centerZ, modelSize);
            p[1] = ProjectPrimitivePoint(box.MaxX, box.MinY, box.MinZ, bounds, centerX, centerY, centerZ, modelSize);
            p[2] = ProjectPrimitivePoint(box.MaxX, box.MaxY, box.MinZ, bounds, centerX, centerY, centerZ, modelSize);
            p[3] = ProjectPrimitivePoint(box.MinX, box.MaxY, box.MinZ, bounds, centerX, centerY, centerZ, modelSize);
            p[4] = ProjectPrimitivePoint(box.MinX, box.MinY, box.MaxZ, bounds, centerX, centerY, centerZ, modelSize);
            p[5] = ProjectPrimitivePoint(box.MaxX, box.MinY, box.MaxZ, bounds, centerX, centerY, centerZ, modelSize);
            p[6] = ProjectPrimitivePoint(box.MaxX, box.MaxY, box.MaxZ, bounds, centerX, centerY, centerZ, modelSize);
            p[7] = ProjectPrimitivePoint(box.MinX, box.MaxY, box.MaxZ, bounds, centerX, centerY, centerZ, modelSize);

            DrawPrimitiveEdge(g, pen, p, 0, 1); DrawPrimitiveEdge(g, pen, p, 1, 2); DrawPrimitiveEdge(g, pen, p, 2, 3); DrawPrimitiveEdge(g, pen, p, 3, 0);
            DrawPrimitiveEdge(g, pen, p, 4, 5); DrawPrimitiveEdge(g, pen, p, 5, 6); DrawPrimitiveEdge(g, pen, p, 6, 7); DrawPrimitiveEdge(g, pen, p, 7, 4);
            DrawPrimitiveEdge(g, pen, p, 0, 4); DrawPrimitiveEdge(g, pen, p, 1, 5); DrawPrimitiveEdge(g, pen, p, 2, 6); DrawPrimitiveEdge(g, pen, p, 3, 7);

            if (drawCenterCross)
            {
                System.Drawing.PointF c = ProjectPrimitivePoint(centerX, centerY, centerZ, bounds, centerX, centerY, centerZ, modelSize);
                g.DrawLine(pen, c.X - 5, c.Y, c.X + 5, c.Y);
                g.DrawLine(pen, c.X, c.Y - 5, c.X, c.Y + 5);
            }
        }

        private static void DrawPrimitiveEdge(System.Drawing.Graphics g, System.Drawing.Pen pen, System.Drawing.PointF[] points, int a, int b)
        {
            g.DrawLine(pen, points[a], points[b]);
        }

        private void PreparePrimitiveProjectionFit(System.Drawing.Rectangle bounds, double centerX, double centerY, double centerZ, double modelSize)
        {
            _primitiveProjectionFitReady = false;

            double minX = double.MaxValue;
            double maxX = double.MinValue;
            double minY = double.MaxValue;
            double maxY = double.MinValue;
            int pointCount = 0;

            AccumulatePrimitiveBoxProjection(_spatialCubesIndex == null ? null : _spatialCubesIndex.ModelBox, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);

            if (_spatialCubesIndex != null && _spatialCubesIndex.Cells != null)
            {
                foreach (SpatialCubeCell cell in _spatialCubesIndex.Cells)
                {
                    if (cell != null)
                    {
                        AccumulatePrimitiveBoxProjection(cell.Bounds, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
                    }
                }
            }

            if (_spatialCubesIndex != null && _spatialCubesIndex.Bodies != null)
            {
                foreach (SpatialBodyRecord body in _spatialCubesIndex.Bodies)
                {
                    if (body != null)
                    {
                        AccumulatePrimitiveBoxProjection(body.BodyBox, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
                    }
                }
            }

            if (pointCount <= 0)
            {
                return;
            }

            double rangeX = Math.Max(0.000001, maxX - minX);
            double rangeY = Math.Max(0.000001, maxY - minY);

            System.Drawing.Rectangle viewport = new System.Drawing.Rectangle(
                bounds.Left + 34,
                bounds.Top + 74,
                Math.Max(10, bounds.Width - 68),
                Math.Max(10, bounds.Height - 124));

            double scaleX = viewport.Width / rangeX;
            double scaleY = viewport.Height / rangeY;
            double scale = Math.Min(scaleX, scaleY) * 0.92 * _primitiveViewerZoom;

            double rawCenterX = (minX + maxX) * 0.5;
            double rawCenterY = (minY + maxY) * 0.5;

            _primitiveProjectionScale = scale;
            _primitiveProjectionOffsetX = viewport.Left + viewport.Width * 0.5 - rawCenterX * scale + _primitiveViewerPanX;
            _primitiveProjectionOffsetY = viewport.Top + viewport.Height * 0.5 + rawCenterY * scale + _primitiveViewerPanY;
            _primitiveProjectionRawMinX = minX;
            _primitiveProjectionRawMaxX = maxX;
            _primitiveProjectionRawMinY = minY;
            _primitiveProjectionRawMaxY = maxY;
            _primitiveProjectionFitReady = true;

            AppLogger.Log(
                "PRIMITIVE_VIEW_PROJECTION_FIT",
                "PreparePrimitiveProjectionFit",
                "Points=" + pointCount.ToString() +
                "; RawX=" + minX.ToString("0.###", CultureInfo.InvariantCulture) + ".." + maxX.ToString("0.###", CultureInfo.InvariantCulture) +
                "; RawY=" + minY.ToString("0.###", CultureInfo.InvariantCulture) + ".." + maxY.ToString("0.###", CultureInfo.InvariantCulture) +
                "; Scale=" + scale.ToString("0.###", CultureInfo.InvariantCulture) +
                "; OffsetX=" + _primitiveProjectionOffsetX.ToString("0.###", CultureInfo.InvariantCulture) +
                "; OffsetY=" + _primitiveProjectionOffsetY.ToString("0.###", CultureInfo.InvariantCulture) +
                "; Zoom=" + _primitiveViewerZoom.ToString("0.###", CultureInfo.InvariantCulture) +
                "; VisibleBounds=" + viewport.Left.ToString() + "," + viewport.Top.ToString() + "," + viewport.Width.ToString() + "," + viewport.Height.ToString() +
                "; PanX=" + _primitiveViewerPanX.ToString("0.###", CultureInfo.InvariantCulture) +
                "; PanY=" + _primitiveViewerPanY.ToString("0.###", CultureInfo.InvariantCulture) +
                "; CenterFit=True; VisibleClipFit=True");
        }

        private void AccumulatePrimitiveBoxProjection(
            SpatialBox box,
            double centerX,
            double centerY,
            double centerZ,
            ref double minX,
            ref double maxX,
            ref double minY,
            ref double maxY,
            ref int pointCount)
        {
            if (box == null)
            {
                return;
            }

            AccumulatePrimitivePointProjection(box.MinX, box.MinY, box.MinZ, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
            AccumulatePrimitivePointProjection(box.MaxX, box.MinY, box.MinZ, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
            AccumulatePrimitivePointProjection(box.MaxX, box.MaxY, box.MinZ, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
            AccumulatePrimitivePointProjection(box.MinX, box.MaxY, box.MinZ, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
            AccumulatePrimitivePointProjection(box.MinX, box.MinY, box.MaxZ, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
            AccumulatePrimitivePointProjection(box.MaxX, box.MinY, box.MaxZ, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
            AccumulatePrimitivePointProjection(box.MaxX, box.MaxY, box.MaxZ, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
            AccumulatePrimitivePointProjection(box.MinX, box.MaxY, box.MaxZ, centerX, centerY, centerZ, ref minX, ref maxX, ref minY, ref maxY, ref pointCount);
        }

        private void AccumulatePrimitivePointProjection(
            double x,
            double y,
            double z,
            double centerX,
            double centerY,
            double centerZ,
            ref double minX,
            ref double maxX,
            ref double minY,
            ref double maxY,
            ref int pointCount)
        {
            double rawX;
            double rawY;

            ProjectPrimitivePointRaw(x, y, z, centerX, centerY, centerZ, out rawX, out rawY);

            minX = Math.Min(minX, rawX);
            maxX = Math.Max(maxX, rawX);
            minY = Math.Min(minY, rawY);
            maxY = Math.Max(maxY, rawY);
            pointCount++;
        }

        private void ProjectPrimitivePointRaw(
            double x,
            double y,
            double z,
            double centerX,
            double centerY,
            double centerZ,
            out double rawX,
            out double rawY)
        {
            double px = x - centerX;
            double py = y - centerY;
            double pz = z - centerZ;

            double yaw = _primitiveViewerYaw * Math.PI / 180.0;
            double pitch = _primitiveViewerPitch * Math.PI / 180.0;
            double cosY = Math.Cos(yaw);
            double sinY = Math.Sin(yaw);
            double cosP = Math.Cos(pitch);
            double sinP = Math.Sin(pitch);

            double rx = px * cosY + pz * sinY;
            double rz = -px * sinY + pz * cosY;
            double ry = py * cosP - rz * sinP;

            rawX = rx;
            rawY = ry;
        }

        private System.Drawing.PointF ProjectPrimitivePoint(
            double x,
            double y,
            double z,
            System.Drawing.Rectangle bounds,
            double centerX,
            double centerY,
            double centerZ,
            double modelSize)
        {
            double rawX;
            double rawY;

            ProjectPrimitivePointRaw(x, y, z, centerX, centerY, centerZ, out rawX, out rawY);

            if (_primitiveProjectionFitReady)
            {
                return new System.Drawing.PointF(
                    (float)(_primitiveProjectionOffsetX + rawX * _primitiveProjectionScale),
                    (float)(_primitiveProjectionOffsetY - rawY * _primitiveProjectionScale));
            }

            double viewScale = Math.Min(bounds.Width, bounds.Height) * 0.55 * _primitiveViewerZoom / Math.Max(modelSize, 0.000001);

            return new System.Drawing.PointF(
                (float)(bounds.Left + bounds.Width * 0.5 + rawX * viewScale),
                (float)(bounds.Top + bounds.Height * 0.55 - rawY * viewScale));
        }

        private static System.Drawing.Color GetPrimitiveBodyColor(int index)
        {
            System.Drawing.Color[] colors = new System.Drawing.Color[]
            {
                System.Drawing.Color.FromArgb(190, 60, 60),
                System.Drawing.Color.FromArgb(60, 120, 200),
                System.Drawing.Color.FromArgb(70, 150, 80),
                System.Drawing.Color.FromArgb(170, 110, 40),
                System.Drawing.Color.FromArgb(120, 80, 170),
                System.Drawing.Color.FromArgb(40, 150, 150)
            };

            return colors[Math.Abs(index) % colors.Length];
        }


private void PanelIptLayerPreviewDrawing_MouseDown(object sender, MouseEventArgs e)
{
    if (e.Button != MouseButtons.Left && e.Button != MouseButtons.Right)
    {
        return;
    }

    int fpsLevel = GetViewerFpsModeLevel();
    _primitiveViewerPanning = e.Button == MouseButtons.Right || (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
    _primitiveViewerDragging = true;
    _primitiveViewerLastMouse = e.Location;

    if (fpsLevel <= 7)
    {
        if (_layerCanvasPreviewBitmap != null && !_layerCanvasLivePreviewRunning)
        {
            _layerCanvasPreviewBitmap.Dispose();
            _layerCanvasPreviewBitmap = null;
        }
        if (panelIptLayerPreviewDrawing != null) { panelIptLayerPreviewDrawing.Invalidate(); }
        return;
    }

    if (fpsLevel >= 13)
    {
        _viewerDragCursorSampleScreen = System.Windows.Forms.Cursor.Position;
        _viewerDragCursorSampleReady = true;
        _viewerDragCameraDirty = false;
    }

    if (panelIptLayerPreviewDrawing != null)
    {
        panelIptLayerPreviewDrawing.Capture = true;
    }

    StartViewerDragSession();
    if (fpsLevel >= 8 && fpsLevel <= 11)
    {
        ScheduleViewerDragFrame(true);
    }
}


private void PanelIptLayerPreviewDrawing_MouseMove(object sender, MouseEventArgs e)
{
    if (!_primitiveViewerDragging)
    {
        return;
    }

    int fpsLevel = GetViewerFpsModeLevel();
    if (fpsLevel >= 13)
    {
        _viewerDragInputEvents++;
        _viewerDragPendingInputEvents++;
        if (SampleAndApplyViewerDragCursor())
        {
            ScheduleViewerDragFrame(true);
        }
        return;
    }

    int dx = e.X - _primitiveViewerLastMouse.X;
    int dy = e.Y - _primitiveViewerLastMouse.Y;
    if (dx == 0 && dy == 0)
    {
        return;
    }

    if (_primitiveViewerPanning)
    {
        _primitiveViewerPanX += dx;
        _primitiveViewerPanY += dy;
    }
    else
    {
        _primitiveViewerYaw += dx * 0.55;
        _primitiveViewerPitch += dy * 0.45;
        if (_primitiveViewerPitch > 85.0) { _primitiveViewerPitch = 85.0; }
        if (_primitiveViewerPitch < -85.0) { _primitiveViewerPitch = -85.0; }
    }

    _primitiveViewerLastMouse = e.Location;
    if (fpsLevel <= 7)
    {
        if (panelIptLayerPreviewDrawing != null) { panelIptLayerPreviewDrawing.Invalidate(); }
        return;
    }

    _viewerDragInputEvents++;
    _viewerDragPendingInputEvents++;
    ScheduleViewerDragFrame(fpsLevel == 12 && _viewerDragFramesRendered == 0);
}


private void PanelIptLayerPreviewDrawing_MouseUp(object sender, MouseEventArgs e)
{
    if (!_primitiveViewerDragging)
    {
        return;
    }

    int fpsLevel = GetViewerFpsModeLevel();
    if (fpsLevel >= 13)
    {
        SampleAndApplyViewerDragCursor();
    }

    _primitiveViewerDragging = false;
    _primitiveViewerPanning = false;

    if (fpsLevel <= 7)
    {
        if (_stepLitePreviewEnabled && panelIptLayerPreviewDrawing != null)
        {
            panelIptLayerPreviewDrawing.Invalidate();
        }
        return;
    }

    _viewerDragFramePending = false;
    _viewerDragCameraDirty = false;
    _viewerDragCursorSampleReady = false;
    _viewerDragPendingInputEvents = 0;
    _viewerDragSummaryPending = true;

    if (_viewerDragRenderTimer != null) { _viewerDragRenderTimer.Stop(); }
    if (panelIptLayerPreviewDrawing != null)
    {
        panelIptLayerPreviewDrawing.Capture = false;
        _viewerDragPaintScheduled = true;
        panelIptLayerPreviewDrawing.Invalidate();
    }
}


private void PanelIptLayerPreviewDrawing_MouseCaptureChanged(object sender, EventArgs e)
{
    if (GetViewerFpsModeLevel() <= 7 || !_primitiveViewerDragging || panelIptLayerPreviewDrawing == null || panelIptLayerPreviewDrawing.Capture)
    {
        return;
    }

    if (ViewerFpsUsesCursorSampling())
    {
        SampleAndApplyViewerDragCursor();
    }
    _primitiveViewerDragging = false;
    _primitiveViewerPanning = false;
    _viewerDragFramePending = false;
    _viewerDragCameraDirty = false;
    _viewerDragCursorSampleReady = false;
    _viewerDragPendingInputEvents = 0;
    _viewerDragSummaryPending = true;

    if (_viewerDragRenderTimer != null) { _viewerDragRenderTimer.Stop(); }
    _viewerDragPaintScheduled = true;
    panelIptLayerPreviewDrawing.Invalidate();

    AppLogger.Log(
        "DRAG_CAPTURE_LOST",
        "PanelIptLayerPreviewDrawing_MouseCaptureChanged",
        "Recovered=True; FinalQualityFrameScheduled=True; " + BuildViewerFpsModeLogFields());
}


private void StartViewerDragSession()
{
    int fpsLevel = GetViewerFpsModeLevel();
    _viewerDragFramePending = false;
    _viewerDragPaintScheduled = false;
    _viewerDragPendingInputEvents = 0;
    _viewerDragInputEvents = 0;
    _viewerDragFramesRendered = 0;
    _viewerDragFramesCoalesced = 0;
    _viewerDragFrameMsEma = 0.0;
    _viewerDragFpsEma = 0.0;
    _viewerDragRenderCapacityFps = 0.0;
    _viewerDragLastFrameCompletedTimestamp = 0;
    _viewerDragAdaptiveScaleChanges = 0;
    _viewerDragCursorSamples = 0;
    _viewerDragAppliedCursorDeltas = 0;
    _viewerDragIdleTimerTicks = 0;
    _viewerDragCameraDirty = false;
    _viewerDragCursorSampleScreen = System.Windows.Forms.Cursor.Position;
    _viewerDragCursorSampleReady = fpsLevel >= 13;

    double minScale = GetViewerFpsMinScale();
    double maxScale = GetViewerFpsMaxScale();
    if (_viewerDragAdaptiveScale < minScale || _viewerDragAdaptiveScale > maxScale || !ViewerFpsUsesAdaptiveScale())
    {
        _viewerDragAdaptiveScale = GetViewerFpsInitialScale();
    }

    _viewerDragSessionStartTimestamp = Stopwatch.GetTimestamp();
    _viewerDragSummaryPending = false;

    if (_viewerDragRenderTimer != null)
    {
        _viewerDragRenderTimer.Interval = fpsLevel >= 13
            ? ViewerDragSchedulerIntervalMs
            : Math.Max(1, 1000 / ViewerDragTargetFps);
        _viewerDragRenderTimer.Start();
    }

    AppLogger.Log(
        "DRAG_SESSION_START",
        "StartViewerDragSession",
        "TargetFps=" + ViewerDragTargetFps.ToString() +
        "; TargetFrameMs=" + GetViewerFpsTargetFrameMs().ToString("0.0", CultureInfo.InvariantCulture) +
        "; SchedulerIntervalMs=" + (_viewerDragRenderTimer == null ? 0 : _viewerDragRenderTimer.Interval).ToString() +
        "; InitialRenderScale=" + _viewerDragAdaptiveScale.ToString("0.00", CultureInfo.InvariantCulture) +
        "; RotationPivot=ModelBoundsCenter; ScreenCenterLocked=" + (fpsLevel >= 9).ToString() +
        "; MouseAnchorAffectsPivot=False; PanX=" + _primitiveViewerPanX.ToString("0.###", CultureInfo.InvariantCulture) +
        "; PanY=" + _primitiveViewerPanY.ToString("0.###", CultureInfo.InvariantCulture) +
        "; RequestedMode=" + GetViewerRenderMode() + "; " + BuildViewerFpsModeLogFields());
}


private void ScheduleViewerDragFrame(bool immediate)
{
    _viewerDragFramePending = true;
    int fpsLevel = GetViewerFpsModeLevel();

    if (fpsLevel >= 13)
    {
        _viewerDragCameraDirty = true;
        if (immediate && _primitiveViewerDragging && !_viewerFrameRendering)
        {
            RenderViewerDragFrameDirect();
        }
        return;
    }

    if (!immediate || panelIptLayerPreviewDrawing == null || _viewerDragPaintScheduled || _viewerFrameRendering)
    {
        return;
    }

    _viewerDragPaintScheduled = true;
    _viewerDragFramePending = false;
    panelIptLayerPreviewDrawing.Invalidate();
}


private void ViewerDragRenderTimer_Tick(object sender, EventArgs e)
{
    if (!_primitiveViewerDragging || !_viewerDraftWhileDragging)
    {
        return;
    }

    int fpsLevel = GetViewerFpsModeLevel();
    if (fpsLevel >= 13)
    {
        bool cursorMoved = SampleAndApplyViewerDragCursor();
        if (!cursorMoved && !_viewerDragCameraDirty)
        {
            _viewerDragIdleTimerTicks++;
            return;
        }

        if (_viewerFrameRendering)
        {
            _viewerDragFramePending = true;
            return;
        }

        RenderViewerDragFrameDirect();
        return;
    }

    if (!_viewerDragFramePending || _viewerDragPaintScheduled || _viewerFrameRendering)
    {
        return;
    }

    int pendingEvents = _viewerDragPendingInputEvents;
    if (pendingEvents > 1) { _viewerDragFramesCoalesced += pendingEvents - 1; }
    _viewerDragPendingInputEvents = 0;
    _viewerDragFramePending = false;
    _viewerDragPaintScheduled = true;
    if (panelIptLayerPreviewDrawing != null) { panelIptLayerPreviewDrawing.Invalidate(); }
}

        private void PanelIptLayerPreviewDrawing_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) { _primitiveViewerZoom *= 1.12; }
            else { _primitiveViewerZoom /= 1.12; }

            if (_primitiveViewerZoom < 0.15) { _primitiveViewerZoom = 0.15; }
            if (_primitiveViewerZoom > 8.0) { _primitiveViewerZoom = 8.0; }

            if (panelIptLayerPreviewDrawing != null)
            {
                panelIptLayerPreviewDrawing.Invalidate();
            }
        }

        private void ResetPrimitiveViewerCamera()
        {
            _primitiveViewerYaw = -35.0;
            _primitiveViewerPitch = 22.0;
            _primitiveViewerZoom = 1.0;
            _primitiveViewerPanX = 0.0;
            _primitiveViewerPanY = 0.0;

            if (_layerCanvasPreviewBitmap != null && !_layerCanvasLivePreviewRunning)
            {
                _layerCanvasPreviewBitmap.Dispose();
                _layerCanvasPreviewBitmap = null;
            }

            if (panelIptLayerPreviewDrawing != null)
            {
                panelIptLayerPreviewDrawing.Invalidate();
            }
        }

        private static System.Drawing.Rectangle CalculateFitRectangle(int imageWidth, int imageHeight, System.Drawing.Rectangle bounds)
        {
            if (imageWidth <= 0 || imageHeight <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                return bounds;
            }

            double scaleX = (double)bounds.Width / (double)imageWidth;
            double scaleY = (double)bounds.Height / (double)imageHeight;
            double scale = Math.Min(scaleX, scaleY);

            int width = Math.Max(1, (int)Math.Round(imageWidth * scale));
            int height = Math.Max(1, (int)Math.Round(imageHeight * scale));
            int x = bounds.X + (bounds.Width - width) / 2;
            int y = bounds.Y + (bounds.Height - height) / 2;

            return new System.Drawing.Rectangle(x, y, width, height);
        }

        private bool CaptureActiveInventorViewToLayerPreview()
        {
            return CaptureActiveInventorViewToLayerPreview(true);
        }

        private bool CaptureActiveInventorViewToLayerPreview(bool showErrors)
        {
            using (AppLogger.Scope("CaptureActiveInventorViewToLayerPreview"))
            {
            if (_invApp == null)
            {
                if (showErrors)
                {
                    LoggedMessageBox.Show("Inventor application is not available.");
                }

                return false;
            }

            try
            {
                int width = panelIptLayerPreviewDrawing == null ? 1280 : Math.Max(640, panelIptLayerPreviewDrawing.ClientSize.Width);
                int height = panelIptLayerPreviewDrawing == null ? 720 : Math.Max(480, panelIptLayerPreviewDrawing.ClientSize.Height);

                string fileName = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    "InventorIptOrg_active_view_preview_" + Guid.NewGuid().ToString("N") + ".png");

                dynamic activeView = _invApp.ActiveView;

                try
                {
                    activeView.Update();
                }
                catch
                {
                }

                activeView.SaveAsBitmap(fileName, width, height);

                System.Drawing.Bitmap bitmap;

                using (System.Drawing.Image image = System.Drawing.Image.FromFile(fileName))
                {
                    bitmap = new System.Drawing.Bitmap(image);
                }

                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch
                {
                }

                if (_layerCanvasPreviewBitmap != null)
                {
                    _layerCanvasPreviewBitmap.Dispose();
                }

                _layerCanvasPreviewBitmap = bitmap;
                _layerCanvasPreviewMessage =
                    (_layerCanvasLivePreviewRunning ? "LIVE bitmap stream: " : "Inventor ActiveView captured: ") +
                    bitmap.Width.ToString() + " x " + bitmap.Height.ToString() +
                    " | " + DateTime.Now.ToString("HH:mm:ss");

                if (panelIptLayerPreviewDrawing != null)
                {
                    panelIptLayerPreviewDrawing.Invalidate();
                }

                AppLogger.Log(
                    "CANVAS_ACTIVE_VIEW_PREVIEW_CAPTURED",
                    "CaptureActiveInventorViewToLayerPreview",
                    "Width=" + bitmap.Width.ToString() +
                    "; Height=" + bitmap.Height.ToString() +
                    "; Source=Inventor.ActiveView.SaveAsBitmap" +
                    "; LiveMode=" + _layerCanvasLivePreviewRunning.ToString());

                return true;
            }
            catch (Exception ex)
            {
                if (showErrors)
                {
                    LoggedMessageBox.Show("Не удалось снять Inventor ActiveView в preview.");
                    LoggedMessageBox.Show(ex.ToString());
                }

                AppLogger.Log(
                    "CANVAS_ACTIVE_VIEW_PREVIEW_CAPTURE_FAILED",
                    "CaptureActiveInventorViewToLayerPreview",
                    ex.ToString());

                return false;
            }

            }}

        private void ToggleLayerCanvasLivePreview()
        {
            using (AppLogger.Scope("ToggleLayerCanvasLivePreview"))
            {
            if (_layerCanvasActiveViewDocked)
            {
                UndockActiveViewFromCanvas();
            }

            if (_layerCanvasLivePreviewRunning)
            {
                StopLayerCanvasLivePreview();
                return;
            }

            if (_layerCanvasLivePreviewTimer == null)
            {
                _layerCanvasLivePreviewTimer = new Timer();
                _layerCanvasLivePreviewTimer.Interval = 500;
                _layerCanvasLivePreviewTimer.Tick += LayerCanvasLivePreviewTimer_Tick;
            }

            _layerCanvasLivePreviewRunning = true;
            _layerCanvasLivePreviewTimer.Start();
            SetButtonTextSafe(ButtonIptCanvasToggleLivePreview, "LV\r\nStop");
            CaptureActiveInventorViewToLayerPreview(false);

            AppLogger.Log(
                "CANVAS_LIVE_BITMAP_PREVIEW_STARTED",
                "ToggleLayerCanvasLivePreview",
                "IntervalMs=" + _layerCanvasLivePreviewTimer.Interval.ToString() +
                "; Source=Repeated ActiveView.SaveAsBitmap");

            }}

        private void StopLayerCanvasLivePreview()
        {
            using (AppLogger.Scope("StopLayerCanvasLivePreview"))
            {
            if (_layerCanvasLivePreviewTimer != null)
            {
                _layerCanvasLivePreviewTimer.Stop();
            }

            _layerCanvasLivePreviewRunning = false;
            SetButtonTextSafe(ButtonIptCanvasToggleLivePreview, "LV\r\nLive");

            AppLogger.Log(
                "CANVAS_LIVE_BITMAP_PREVIEW_STOPPED",
                "StopLayerCanvasLivePreview",
                "Stopped=True");

            }}

        private void LayerCanvasLivePreviewTimer_Tick(object sender, EventArgs e)
        {
            CaptureActiveInventorViewToLayerPreview(false);
        }

        private void ToggleDockActiveViewInCanvas()
        {
            using (AppLogger.Scope("ToggleDockActiveViewInCanvas"))
            {
            if (_layerCanvasActiveViewDocked)
            {
                UndockActiveViewFromCanvas();
                return;
            }

            StopLayerCanvasLivePreview();

            IntPtr hwnd;

            if (!TryGetActiveViewHwnd(out hwnd) || hwnd == IntPtr.Zero)
            {
                LoggedMessageBox.Show(
                    "Не удалось получить HWND Inventor ActiveView.\r\n\r\n" +
                    "DK Dock не поддерживается этим Inventor/Interop.\r\n" +
                    "Используйте AV Model или LV Live.");
                return;
            }

            _dockedActiveViewHwnd = hwnd;
            _dockedActiveViewOriginalParent = GetParent(hwnd);
            _dockedActiveViewOriginalStyle = GetWindowLong(hwnd, GWL_STYLE);

            int newStyle = (_dockedActiveViewOriginalStyle | WS_CHILD | WS_VISIBLE) & ~WS_CAPTION & ~WS_THICKFRAME & ~WS_POPUP;
            SetWindowLong(hwnd, GWL_STYLE, newStyle);
            SetParent(hwnd, panelIptLayerPreviewDrawing.Handle);
            _layerCanvasActiveViewDocked = true;
            SetButtonTextSafe(ButtonIptCanvasDockActiveView, "DK\r\nUndock");
            UpdateDockedActiveViewLayout();

            if (_layerCanvasPreviewBitmap != null)
            {
                _layerCanvasPreviewBitmap.Dispose();
                _layerCanvasPreviewBitmap = null;
            }

            _layerCanvasPreviewMessage = "Inventor ActiveView docked into canvas";
            panelIptLayerPreviewDrawing.Invalidate();

            AppLogger.Log(
                "CANVAS_ACTIVE_VIEW_DOCKED",
                "ToggleDockActiveViewInCanvas",
                "Hwnd=" + hwnd.ToInt64().ToString(CultureInfo.InvariantCulture) +
                "; OriginalParent=" + _dockedActiveViewOriginalParent.ToInt64().ToString(CultureInfo.InvariantCulture) +
                "; Mode=Win32_SetParent_Experimental");

            }}

        private void UndockActiveViewFromCanvas()
        {
            using (AppLogger.Scope("UndockActiveViewFromCanvas"))
            {
            if (!_layerCanvasActiveViewDocked || _dockedActiveViewHwnd == IntPtr.Zero)
            {
                return;
            }

            try
            {
                SetParent(_dockedActiveViewHwnd, _dockedActiveViewOriginalParent);
                SetWindowLong(_dockedActiveViewHwnd, GWL_STYLE, _dockedActiveViewOriginalStyle);
                SetWindowPos(_dockedActiveViewHwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
            }
            catch
            {
            }

            _layerCanvasActiveViewDocked = false;
            _dockedActiveViewHwnd = IntPtr.Zero;
            _dockedActiveViewOriginalParent = IntPtr.Zero;
            _dockedActiveViewOriginalStyle = 0;
            SetButtonTextSafe(ButtonIptCanvasDockActiveView, "DK\r\nDock");

            AppLogger.Log(
                "CANVAS_ACTIVE_VIEW_UNDOCKED",
                "UndockActiveViewFromCanvas",
                "Restored=True");

            }}

        private void UpdateDockedActiveViewLayout()
        {
            if (!_layerCanvasActiveViewDocked || _dockedActiveViewHwnd == IntPtr.Zero || panelIptLayerPreviewDrawing == null)
            {
                return;
            }

            System.Drawing.Rectangle rect = panelIptLayerPreviewDrawing.ClientRectangle;
            SetWindowPos(
                _dockedActiveViewHwnd,
                IntPtr.Zero,
                0,
                0,
                Math.Max(1, rect.Width),
                Math.Max(1, rect.Height),
                SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
        }

        private void PanelIptLayerPreviewDrawing_Resize(object sender, EventArgs e)
        {
            UpdateDockedActiveViewLayout();

            if (_viewerBackBufferBitmap != null)
            {
                _viewerBackBufferBitmap.Dispose();
                _viewerBackBufferBitmap = null;
            }

            if (panelIptLayerPreviewDrawing != null && !_viewerFrameRendering)
            {
                panelIptLayerPreviewDrawing.Invalidate();
            }
        }

        private bool TryGetActiveViewHwnd(out IntPtr hwnd)
        {
            hwnd = IntPtr.Zero;

            if (_invApp == null)
            {
                return false;
            }

            try
            {
                dynamic view = _invApp.ActiveView;
                object raw = null;

                try { raw = view.HWND; } catch { }
                if (raw == null) { try { raw = view.HWnd; } catch { } }
                if (raw == null) { try { raw = view.WindowHandle; } catch { } }

                hwnd = ConvertObjectToIntPtr(raw);
                return hwnd != IntPtr.Zero;
            }
            catch
            {
                hwnd = IntPtr.Zero;
                return false;
            }
        }

        private static IntPtr ConvertObjectToIntPtr(object value)
        {
            if (value == null)
            {
                return IntPtr.Zero;
            }

            if (value is IntPtr)
            {
                return (IntPtr)value;
            }

            try
            {
                long handle = Convert.ToInt64(value, CultureInfo.InvariantCulture);
                return new IntPtr(handle);
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        private void ButtonIptCanvasOpenStepFile_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptCanvasOpenStepFile_Click"))
            {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    dialog.Title = "Open STEP locally";
                    dialog.Filter = "STEP files (*.stp;*.step)|*.stp;*.step|All files (*.*)|*.*";
                    dialog.Multiselect = false;
                    dialog.CheckFileExists = true;
                    dialog.CheckPathExists = true;
                    dialog.RestoreDirectory = true;

                    Stopwatch dialogSw = Stopwatch.StartNew();
                    DialogResult dialogResult = dialog.ShowDialog(this);
                    dialogSw.Stop();

                    AppLogger.Log(
                        "STEP_LOCAL_FILE_DIALOG_SECONDS",
                        "ButtonIptCanvasOpenStepFile_Click",
                        "ElapsedSeconds=" + dialogSw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) +
                        "; Result=" + dialogResult.ToString());

                    if (dialogResult != DialogResult.OK)
                    {
                        AppLogger.Log(
                            "STEP_LOCAL_OPEN_CANCELLED",
                            "ButtonIptCanvasOpenStepFile_Click",
                            "UserCancelled=True");
                        return;
                    }

                    OpenStepFileInLocalViewer(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem opening STEP locally");
                LoggedMessageBox.Show(ex.ToString());

                AppLogger.Log(
                    "STEP_LOCAL_OPEN_FAILED",
                    "ButtonIptCanvasOpenStepFile_Click",
                    ex.ToString());
            }
            finally
            {
                sw.Stop();

                AppLogger.Log(
                    "STEP_LOCAL_OPEN_BUTTON_SECONDS",
                    "ButtonIptCanvasOpenStepFile_Click",
                    "ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));
            }

            }}

        private StepLiteScene OpenStepFileInLocalViewer(string stepFilePath)
        {
            using (AppLogger.Scope("OpenStepFileInLocalViewer"))
            {
            if (string.IsNullOrWhiteSpace(stepFilePath))
            {
                LoggedMessageBox.Show("STEP file path is empty.");
                return null;
            }

            if (!System.IO.File.Exists(stepFilePath))
            {
                LoggedMessageBox.Show("STEP file not found:\r\n" + stepFilePath);
                return null;
            }

            string extension = System.IO.Path.GetExtension(stepFilePath);

            if (!string.Equals(extension, ".stp", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(extension, ".step", StringComparison.OrdinalIgnoreCase))
            {
                DialogResult answer = MessageBox.Show(
                    this,
                    "Файл не похож на STEP:\r\n" + stepFilePath + "\r\n\r\nОткрыть локально всё равно?",
                    "Open STEP locally",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (answer != DialogResult.Yes)
                {
                    return null;
                }
            }

            Stopwatch sw = Stopwatch.StartNew();
            StepLiteReader reader = new StepLiteReader();
            StepLiteScene scene = reader.Load(stepFilePath);
            sw.Stop();

            _stepLitePreviewScene = scene;
            _stepLitePreviewEnabled = scene != null && scene.PointCount > 0;
            _meshPreviewEnabled = false;

            if (_layerCanvasPreviewBitmap != null)
            {
                _layerCanvasPreviewBitmap.Dispose();
                _layerCanvasPreviewBitmap = null;
            }

            if (panelIptLayerPreviewDrawing != null)
            {
                panelIptLayerPreviewDrawing.Invalidate();
            }

            AppLogger.Log(
                "STEP_LOCAL_SCENE_LOADED",
                "OpenStepFileInLocalViewer",
                "FileName=" + stepFilePath +
                "; Points=" + (scene == null ? 0 : scene.PointCount).ToString() +
                "; Edges=" + (scene == null ? 0 : scene.EdgeCount).ToString() +
                "; CartesianPoints=" + (scene == null ? 0 : scene.CartesianPointEntityCount).ToString() +
                "; VertexPoints=" + (scene == null ? 0 : scene.VertexPointEntityCount).ToString() +
                "; EdgeCurves=" + (scene == null ? 0 : scene.EdgeCurveEntityCount).ToString() +
                "; PolyLoops=" + (scene == null ? 0 : scene.PolyLoopEntityCount).ToString() +
                "; AdvancedFaces=" + (scene == null ? 0 : scene.AdvancedFaceEntityCount).ToString() +
                "; AdvancedFaceSameSenseTrue=" + (scene == null ? 0 : scene.AdvancedFaceSameSenseTrueCount).ToString() +
                "; AdvancedFaceSameSenseFalse=" + (scene == null ? 0 : scene.AdvancedFaceSameSenseFalseCount).ToString() +
                "; FaceBounds=" + (scene == null ? 0 : scene.FaceBoundEntityCount).ToString() +
                "; FaceOuterBounds=" + (scene == null ? 0 : scene.FaceOuterBoundEntityCount).ToString() +
                "; FaceInnerBounds=" + (scene == null ? 0 : scene.FaceInnerBoundEntityCount).ToString() +
                "; FaceBoundOrientationTrue=" + (scene == null ? 0 : scene.FaceBoundOrientationTrueCount).ToString() +
                "; FaceBoundOrientationFalse=" + (scene == null ? 0 : scene.FaceBoundOrientationFalseCount).ToString() +
                "; OrientedFaceLoops=" + (scene == null ? 0 : scene.OrientedFaceLoopCount).ToString() +
                "; ForwardFaceLoops=" + (scene == null ? 0 : scene.ForwardFaceLoopCount).ToString() +
                "; ReversedFaceLoops=" + (scene == null ? 0 : scene.ReversedFaceLoopCount).ToString() +
                "; OrphanPolyLoops=" + (scene == null ? 0 : scene.OrphanPolyLoopCount).ToString() +
                "; Triangles=" + (scene == null ? 0 : scene.TriangleCount).ToString() +
                "; Schema=" + (scene == null ? "" : (scene.FileSchema ?? "")) +
                "; OpensExternalSession=False" +
                "; ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));

            AppLogger.Log(
                "STEP_ADVANCED_FACE_ORIENTATION",
                "OpenStepFileInLocalViewer",
                "AdvancedFaceSameSenseTrue=" + (scene == null ? 0 : scene.AdvancedFaceSameSenseTrueCount).ToString() +
                "; AdvancedFaceSameSenseFalse=" + (scene == null ? 0 : scene.AdvancedFaceSameSenseFalseCount).ToString() +
                "; FaceBoundOrientationTrue=" + (scene == null ? 0 : scene.FaceBoundOrientationTrueCount).ToString() +
                "; FaceBoundOrientationFalse=" + (scene == null ? 0 : scene.FaceBoundOrientationFalseCount).ToString() +
                "; ForwardFaceLoops=" + (scene == null ? 0 : scene.ForwardFaceLoopCount).ToString() +
                "; ReversedFaceLoops=" + (scene == null ? 0 : scene.ReversedFaceLoopCount).ToString() +
                "; OrphanPolyLoops=" + (scene == null ? 0 : scene.OrphanPolyLoopCount).ToString() +
                "; UnresolvedFaceBounds=" + (scene == null ? 0 : scene.UnresolvedFaceBoundReferenceCount).ToString() +
                "; UnresolvedPolyLoops=" + (scene == null ? 0 : scene.UnresolvedPolyLoopReferenceCount).ToString() +
                "; InnerBoundsSkipped=" + (scene == null ? 0 : scene.InnerBoundTriangulationSkippedCount).ToString() +
                "; EffectiveForwardRule=BoundOrientationEqualsFaceSameSense");

            _stepLiteLastOpenStatus =
                "Loaded local STEP: " +
                (scene == null ? 0 : scene.PointCount).ToString() + " points / " +
                (scene == null ? 0 : scene.EdgeCount).ToString() + " edges / " +
                (scene == null ? 0 : scene.TriangleCount).ToString() + " triangles / schema " +
                (scene == null ? "" : (scene.FileSchema ?? ""));

            AppLogger.Log(
                "STEP_LOCAL_SCENE_STATUS_NON_BLOCKING",
                "OpenStepFileInLocalViewer",
                _stepLiteLastOpenStatus + "; MessageBoxShown=False");

            UpdateLayerCanvasPreviewStatus();

            return scene;

            }}

        private void ButtonIptCanvasBuildMeshPreview_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptCanvasBuildMeshPreview_Click"))
            {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                PartDocument partDoc;

                if (!TryGetActivePartDocument(out partDoc))
                {
                    return;
                }

                SetButtonTextSafe(ButtonIptCanvasBuildMeshPreview, "MS\r\n...");

                MeshScene scene = BuildMeshPreviewSceneFromPart(partDoc, 0.05);

                _meshPreviewScene = scene;
                _meshPreviewEnabled = scene != null && scene.BodyCount > 0;

                if (_layerCanvasPreviewBitmap != null)
                {
                    _layerCanvasPreviewBitmap.Dispose();
                    _layerCanvasPreviewBitmap = null;
                }

                if (panelIptLayerPreviewDrawing != null)
                {
                    panelIptLayerPreviewDrawing.Invalidate();
                }

                sw.Stop();

                AppLogger.Log(
                    "MESH_VIEW_SCENE_READY",
                    "ButtonIptCanvasBuildMeshPreview_Click",
                    "Bodies=" + (scene == null ? 0 : scene.BodyCount).ToString() +
                    "; Vertices=" + (scene == null ? 0 : scene.VertexCount).ToString() +
                    "; Triangles=" + (scene == null ? 0 : scene.TriangleCount).ToString() +
                    "; ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));

                LoggedMessageBox.Show(
                    "Mesh preview построен.\r\n\r\n" +
                    "Bodies: " + (scene == null ? 0 : scene.BodyCount).ToString() + "\r\n" +
                    "Vertices: " + (scene == null ? 0 : scene.VertexCount).ToString() + "\r\n" +
                    "Triangles: " + (scene == null ? 0 : scene.TriangleCount).ToString() + "\r\n\r\n" +
                    "Если FallbackBodies=0 в логе — это уже настоящий mesh, не кубики.");
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem building mesh preview");
                LoggedMessageBox.Show(ex.ToString());

                AppLogger.Log(
                    "MESH_VIEW_SCENE_FAILED",
                    "ButtonIptCanvasBuildMeshPreview_Click",
                    ex.ToString());
            }
            finally
            {
                SetButtonTextSafe(ButtonIptCanvasBuildMeshPreview, string.Empty);
            }

            }}

        private MeshScene BuildMeshPreviewSceneFromPart(PartDocument partDoc, double tolerance)
        {
            using (AppLogger.Scope("BuildMeshPreviewSceneFromPart"))
            {
            MeshScene scene = new MeshScene();
            scene.Name = partDoc == null ? string.Empty : SafeGetPartDisplayName(partDoc);

            if (partDoc == null)
            {
                return scene;
            }

            int bodyIndex = 0;
            int failedBodies = 0;
            int fallbackBodies = 0;

            foreach (SurfaceBody body in partDoc.ComponentDefinition.SurfaceBodies)
            {
                bodyIndex++;

                if (body == null)
                {
                    continue;
                }

                MeshBody meshBody;

                if (!TryBuildMeshBodyFromSurfaceBody(body, bodyIndex, tolerance, out meshBody))
                {
                    failedBodies++;

                    if (TryBuildBoxMeshBodyFromSurfaceBody(body, bodyIndex, out meshBody))
                    {
                        fallbackBodies++;
                    }
                }

                if (meshBody != null && meshBody.TriangleCount > 0)
                {
                    scene.AddBody(meshBody);
                }
            }

            AppLogger.Log(
                "MESH_VIEW_SCENE_BUILT",
                "BuildMeshPreviewSceneFromPart",
                "Bodies=" + scene.BodyCount.ToString() +
                "; Vertices=" + scene.VertexCount.ToString() +
                "; Triangles=" + scene.TriangleCount.ToString() +
                "; FailedBodies=" + failedBodies.ToString() +
                "; FallbackBodies=" + fallbackBodies.ToString() +
                "; Tolerance=" + tolerance.ToString(CultureInfo.InvariantCulture));

            return scene;

            }}

        private bool TryBuildMeshBodyFromSurfaceBody(SurfaceBody body, int bodyIndex, double tolerance, out MeshBody meshBody)
        {
            meshBody = null;

            try
            {
                int vertexCount;
                int facetCount;
                double[] vertexCoordinates;
                double[] normalVectors;
                int[] vertexIndices;

                // v0.4.76: строго типизированный COM-call.
                // v0.4.75 использовала dynamic + object out и падала на аргументе VertexCount.
                body.CalculateFacets(
                    tolerance,
                    out vertexCount,
                    out facetCount,
                    out vertexCoordinates,
                    out normalVectors,
                    out vertexIndices);

                MeshBody result;

                if (!TryCreateMeshBodyFromFacetArrays(
                    bodyIndex,
                    vertexCount,
                    facetCount,
                    vertexCoordinates,
                    vertexIndices,
                    out result))
                {
                    return false;
                }

                meshBody = result;

                AppLogger.Log(
                    "MESH_BODY_FACETS_OK",
                    "TryBuildMeshBodyFromSurfaceBody",
                    "BodyIndex=" + bodyIndex.ToString() +
                    "; VertexCount=" + vertexCount.ToString() +
                    "; FacetCount=" + facetCount.ToString() +
                    "; Vertices=" + result.VertexCount.ToString() +
                    "; Triangles=" + result.TriangleCount.ToString() +
                    "; Source=SurfaceBody.CalculateFacets_StrongTyped");

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.Log(
                    "MESH_BODY_FACETS_FAILED",
                    "TryBuildMeshBodyFromSurfaceBody",
                    "BodyIndex=" + bodyIndex.ToString() + "; " + ex.GetType().Name + ": " + ex.Message);

                return false;
            }
        }

        private bool TryCreateMeshBodyFromFacetArrays(
            int bodyIndex,
            int vertexCount,
            int facetCount,
            double[] vertexCoordinates,
            int[] vertexIndices,
            out MeshBody meshBody)
        {
            meshBody = null;

            if (vertexCount <= 0 || facetCount <= 0)
            {
                return false;
            }

            if (vertexCoordinates == null || vertexCoordinates.Length < vertexCount * 3)
            {
                return false;
            }

            if (vertexIndices == null || vertexIndices.Length < facetCount * 3)
            {
                return false;
            }

            MeshBody result = new MeshBody();
            result.Id = bodyIndex.ToString();
            result.Name = "MeshBody " + bodyIndex.ToString();

            for (int i = 0; i < vertexCount; i++)
            {
                int offset = i * 3;
                result.Vertices.Add(new MeshPoint3(
                    vertexCoordinates[offset],
                    vertexCoordinates[offset + 1],
                    vertexCoordinates[offset + 2]));
            }

            int minIndex = int.MaxValue;

            for (int i = 0; i < vertexIndices.Length; i++)
            {
                minIndex = Math.Min(minIndex, vertexIndices[i]);
            }

            int baseOffset = minIndex == 1 ? 1 : 0;

            for (int i = 0; i < facetCount; i++)
            {
                int offset = i * 3;
                int a = vertexIndices[offset] - baseOffset;
                int b = vertexIndices[offset + 1] - baseOffset;
                int c = vertexIndices[offset + 2] - baseOffset;

                if (a < 0 || b < 0 || c < 0 ||
                    a >= vertexCount || b >= vertexCount || c >= vertexCount)
                {
                    continue;
                }

                result.Triangles.Add(new MeshTriangle(a, b, c));
            }

            result.UpdateBounds();

            if (result.TriangleCount <= 0)
            {
                return false;
            }

            meshBody = result;
            return true;
        }

        private bool TryBuildBoxMeshBodyFromSurfaceBody(SurfaceBody body, int bodyIndex, out MeshBody meshBody)
        {
            meshBody = null;

            try
            {
                Box box = body.RangeBox;
                if (box == null)
                {
                    return false;
                }

                SpatialBox bodyBox = SpatialBox.FromInventorBox(box);
                MeshBody result = new MeshBody();
                result.Id = bodyIndex.ToString();
                result.Name = "BoxFallback " + bodyIndex.ToString();

                AddBoxMesh(result, bodyBox.MinX, bodyBox.MinY, bodyBox.MinZ, bodyBox.MaxX, bodyBox.MaxY, bodyBox.MaxZ);
                result.UpdateBounds();

                meshBody = result;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void AddBoxMesh(MeshBody body, double minX, double minY, double minZ, double maxX, double maxY, double maxZ)
        {
            int start = body.Vertices.Count;

            body.Vertices.Add(new MeshPoint3(minX, minY, minZ));
            body.Vertices.Add(new MeshPoint3(maxX, minY, minZ));
            body.Vertices.Add(new MeshPoint3(maxX, maxY, minZ));
            body.Vertices.Add(new MeshPoint3(minX, maxY, minZ));
            body.Vertices.Add(new MeshPoint3(minX, minY, maxZ));
            body.Vertices.Add(new MeshPoint3(maxX, minY, maxZ));
            body.Vertices.Add(new MeshPoint3(maxX, maxY, maxZ));
            body.Vertices.Add(new MeshPoint3(minX, maxY, maxZ));

            AddMeshQuad(body, start + 0, start + 1, start + 2, start + 3);
            AddMeshQuad(body, start + 4, start + 5, start + 6, start + 7);
            AddMeshQuad(body, start + 0, start + 1, start + 5, start + 4);
            AddMeshQuad(body, start + 1, start + 2, start + 6, start + 5);
            AddMeshQuad(body, start + 2, start + 3, start + 7, start + 6);
            AddMeshQuad(body, start + 3, start + 0, start + 4, start + 7);
        }

        private static void AddMeshQuad(MeshBody body, int a, int b, int c, int d)
        {
            body.Triangles.Add(new MeshTriangle(a, b, c));
            body.Triangles.Add(new MeshTriangle(a, c, d));
        }

        private static double[] ConvertToDoubleArray(object value)
        {
            if (value == null)
            {
                return new double[0];
            }

            double[] direct = value as double[];
            if (direct != null)
            {
                return direct;
            }

            Array array = value as Array;
            if (array == null)
            {
                return new double[0];
            }

            List<double> result = new List<double>();
            int lower = array.GetLowerBound(0);
            int upper = array.GetUpperBound(0);

            for (int i = lower; i <= upper; i++)
            {
                object item = array.GetValue(i);
                result.Add(Convert.ToDouble(item, CultureInfo.InvariantCulture));
            }

            return result.ToArray();
        }

        private static int[] ConvertToIntArray(object value)
        {
            if (value == null)
            {
                return new int[0];
            }

            int[] direct = value as int[];
            if (direct != null)
            {
                return direct;
            }

            Array array = value as Array;
            if (array == null)
            {
                return new int[0];
            }

            List<int> result = new List<int>();
            int lower = array.GetLowerBound(0);
            int upper = array.GetUpperBound(0);

            for (int i = lower; i <= upper; i++)
            {
                object item = array.GetValue(i);
                result.Add(Convert.ToInt32(item, CultureInfo.InvariantCulture));
            }

            return result.ToArray();
        }

        private void UpdateLayerCanvasPreviewStatus()
        {
            if (labelIptLayerPreviewStatus == null)
            {
                return;
            }

            if (CanvasOnlyGitHubMode)
            {
                labelIptLayerPreviewStatus.Text =
                    "GitHub canvas-only status:\r\n" +
                    "Local STEP: " + (_stepLitePreviewScene == null
                        ? "not loaded"
                        : (_stepLitePreviewScene.PointCount.ToString() + " points / " +
                           _stepLitePreviewScene.EdgeCount.ToString() + " edges / " +
                           _stepLitePreviewScene.TriangleCount.ToString() + " triangles")) + "\r\n" +
                    "Last STEP: " + (string.IsNullOrWhiteSpace(_stepLiteLastOpenStatus) ? "-" : _stepLiteLastOpenStatus) + "\r\n" +
                    "Render: " + GetViewerRenderModeDisplayText() + "\r\n" +
                    "FPS pipeline: " + GetViewerFpsModeDisplayText() + "\r\n" +
                    "Autodesk/Inventor calls from visible workflow: OFF\r\n" +
                    "LMB rotate | Shift+LMB/RMB pan | Wheel zoom | Double-click/RS reset";

                if (panelIptLayerPreviewDrawing != null)
                {
                    panelIptLayerPreviewDrawing.Invalidate();
                }
                return;
            }

            int gridRows = dataGridViewIptBrowserTree == null ? 0 : dataGridViewIptBrowserTree.Rows.Count;
            int legacyRows = treeViewIptLegacyBrowserTree == null ? 0 : CountTreeViewNodes(treeViewIptLegacyBrowserTree.Nodes);
            int cells = (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady) ? 0 : _spatialCubesIndex.Cells.Count;
            int bodies = (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady) ? 0 : _spatialCubesIndex.Bodies.Count;
            int cacheRows = (_browserTreeNamesOnlySnapshot == null || _browserTreeNamesOnlySnapshot.GridItems == null) ? 0 : _browserTreeNamesOnlySnapshot.GridItems.Count;

            labelIptLayerPreviewStatus.Text =
                "Layer status: Browser rows " + gridRows.ToString() +
                "; Legacy nodes " + legacyRows.ToString() +
                "; Cubes " + cells.ToString() +
                "; Bodies " + bodies.ToString() +
                "; Cache " + cacheRows.ToString();
        }


        private void ButtonIptOpenBrowserTruthWindow_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptOpenBrowserTruthWindow_Click"))
            {
            Form window = CreateLayerWindow("Browser Truth Cache", "Текущая таблица Browser tree. Источник: полный Refresh browser tree / v0.4.48 truth path.");
            DataGridView grid = CreateLayerGrid();
            CopyCurrentBrowserGridToLayerGrid(grid, "BrowserTruth");
            window.Controls.Add(grid);
            window.Show(this);
            UpdateLayerCanvasPreviewStatus();
            AppLogger.Log("LAYER_WINDOW_OPENED", "ButtonIptOpenBrowserTruthWindow_Click", "Layer=BrowserTruth; Rows=" + grid.Rows.Count.ToString());

            }}

        private void ButtonIptOpenSpatialCacheWindow_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptOpenSpatialCacheWindow_Click"))
            {
            Form window = CreateLayerWindow("Spatial Geometry Cache", "Spatial cubes BASE: кубы, тела, BodyBox center, Cube IDs.");
            DataGridView grid = CreateLayerGrid();
            FillSpatialCacheGrid(grid);
            window.Controls.Add(grid);
            window.Show(this);
            UpdateLayerCanvasPreviewStatus();
            AppLogger.Log("LAYER_WINDOW_OPENED", "ButtonIptOpenSpatialCacheWindow_Click", "Layer=SpatialCache; Rows=" + grid.Rows.Count.ToString());

            }}

        private void ButtonIptOpenMergedCacheWindow_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptOpenMergedCacheWindow_Click"))
            {
            Form window = CreateLayerWindow("Merged View Cache", "Текущая объединённая таблица: Browser row + editable flags + XYZ + Cubes.");
            DataGridView grid = CreateLayerGrid();
            FillMergedViewGrid(grid);
            window.Controls.Add(grid);
            window.Show(this);
            UpdateLayerCanvasPreviewStatus();
            AppLogger.Log("LAYER_WINDOW_OPENED", "ButtonIptOpenMergedCacheWindow_Click", "Layer=MergedView; Rows=" + grid.Rows.Count.ToString());

            }}

        private void ButtonIptOpenFastCacheWindow_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptOpenFastCacheWindow_Click"))
            {
            Form window = CreateLayerWindow("Fast Cache View", "BrowserTreeNamesOnlySnapshot / cache-view для вторых кнопок #2 BASE.");
            DataGridView grid = CreateLayerGrid();
            FillFastCacheGrid(grid);
            window.Controls.Add(grid);
            window.Show(this);
            UpdateLayerCanvasPreviewStatus();
            AppLogger.Log("LAYER_WINDOW_OPENED", "ButtonIptOpenFastCacheWindow_Click", "Layer=FastCache; Rows=" + grid.Rows.Count.ToString());

            }}

        private void ButtonIptOpenLegacyCacheWindow_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptOpenLegacyCacheWindow_Click"))
            {
            Form window = CreateLayerWindow("Legacy Browser Tree", "Отдельное временное окно старого TreeView.");
            TreeView tree = new TreeView();
            tree.Dock = DockStyle.Fill;
            tree.HideSelection = false;

            if (treeViewIptLegacyBrowserTree != null)
            {
                foreach (TreeNode node in treeViewIptLegacyBrowserTree.Nodes)
                {
                    tree.Nodes.Add((TreeNode)node.Clone());
                }
            }

            window.Controls.Add(tree);
            window.Show(this);
            UpdateLayerCanvasPreviewStatus();
            AppLogger.Log("LAYER_WINDOW_OPENED", "ButtonIptOpenLegacyCacheWindow_Click", "Layer=Legacy; Nodes=" + CountTreeViewNodes(tree.Nodes).ToString());

            }}

        private void ButtonIptOpenModelPreviewWindow_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptOpenModelPreviewWindow_Click"))
            {
            Form window = CreateLayerWindow("Model Preview / Status", "Статус текущей модели и кэшей без нового тяжёлого refresh.");
            DataGridView grid = CreateLayerGrid();
            FillModelPreviewGrid(grid);
            window.Controls.Add(grid);
            window.Show(this);
            UpdateLayerCanvasPreviewStatus();
            AppLogger.Log("LAYER_WINDOW_OPENED", "ButtonIptOpenModelPreviewWindow_Click", "Layer=ModelPreview; Rows=" + grid.Rows.Count.ToString());

            }}

        private Form CreateLayerWindow(string title, string description)
        {
            Form window = new Form();
            window.Text = AppBuild.WindowTitle + " — " + title;
            window.StartPosition = FormStartPosition.CenterParent;
            window.Size = new System.Drawing.Size(1120, 720);
            window.MinimizeBox = true;
            window.MaximizeBox = true;

            Label header = new Label();
            header.AutoSize = false;
            header.Dock = DockStyle.Top;
            header.Height = 54;
            header.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            header.Padding = new Padding(10, 0, 0, 0);
            header.Font = new System.Drawing.Font("Segoe UI", 9.0F, System.Drawing.FontStyle.Bold);
            header.Text = title + "\r\n" + description;

            window.Controls.Add(header);
            return window;
        }

        private DataGridView CreateLayerGrid()
        {
            DataGridView grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.BackgroundColor = System.Drawing.Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            return grid;
        }

        private static void AddLayerColumn(DataGridView grid, string name, string header)
        {
            if (grid.Columns.Contains(name))
            {
                return;
            }

            grid.Columns.Add(name, header);
        }

        private void CopyCurrentBrowserGridToLayerGrid(DataGridView target, string source)
        {
            AddLayerColumn(target, "Layer", "Layer");
            AddLayerColumn(target, "Level", "Level");
            AddLayerColumn(target, "Type", "Type");
            AddLayerColumn(target, "Name", "Name / имя");
            AddLayerColumn(target, "X", "X");
            AddLayerColumn(target, "Y", "Y");
            AddLayerColumn(target, "Z", "Z");
            AddLayerColumn(target, "Cubes", "Cubes");
            AddLayerColumn(target, "CubeIds", "Cube IDs");
            AddLayerColumn(target, "CanMove", "CanMove");
            AddLayerColumn(target, "CanRename", "CanRename");
            AddLayerColumn(target, "HasNativeObject", "HasNativeObject");

            if (dataGridViewIptBrowserTree == null)
            {
                return;
            }

            foreach (DataGridViewRow row in dataGridViewIptBrowserTree.Rows)
            {
                if (row == null || row.IsNewRow)
                {
                    continue;
                }

                BrowserTreeGridItem item = row.Tag as BrowserTreeGridItem;

                target.Rows.Add(
                    source,
                    GetGridCellText(row, BrowserGridColDepth),
                    GetGridCellText(row, BrowserGridColType),
                    GetGridCellText(row, BrowserGridColName),
                    GetGridCellText(row, BrowserGridColX),
                    GetGridCellText(row, BrowserGridColY),
                    GetGridCellText(row, BrowserGridColZ),
                    GetOptionalGridCellText(row, BrowserGridColCubeCount),
                    GetOptionalGridCellText(row, BrowserGridColCubeIds),
                    item == null ? "" : item.CanMove.ToString(),
                    item == null ? "" : item.CanRename.ToString(),
                    item == null ? "False" : (item.NativeObject != null).ToString());
            }
        }

        private string GetGridCellText(DataGridViewRow row, int columnIndex)
        {
            if (row == null || columnIndex < 0 || columnIndex >= row.Cells.Count)
            {
                return string.Empty;
            }

            return Convert.ToString(row.Cells[columnIndex].Value);
        }

        private string GetOptionalGridCellText(DataGridViewRow row, int columnIndex)
        {
            if (row == null || columnIndex < 0 || columnIndex >= row.Cells.Count)
            {
                return string.Empty;
            }

            return Convert.ToString(row.Cells[columnIndex].Value);
        }

        private void FillSpatialCacheGrid(DataGridView grid)
        {
            AddLayerColumn(grid, "Layer", "Layer");
            AddLayerColumn(grid, "Level", "Level");
            AddLayerColumn(grid, "CubeId", "Cube / ID");
            AddLayerColumn(grid, "BodyIndex", "Body index");
            AddLayerColumn(grid, "BodyName", "Body name");
            AddLayerColumn(grid, "X", "X");
            AddLayerColumn(grid, "Y", "Y");
            AddLayerColumn(grid, "Z", "Z");
            AddLayerColumn(grid, "CubeCount", "Cubes");
            AddLayerColumn(grid, "CubeIds", "Cube IDs");
            AddLayerColumn(grid, "Bounds", "Bounds");

            if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady)
            {
                grid.Rows.Add("Spatial", "0", "<OFF>", "", "Spatial BASE is not built", "", "", "", "", "", "");
                return;
            }

            grid.Rows.Add("Spatial", "0", "ROOT", "", _spatialCubesIndex.DocumentDisplayName, "", "", "", _spatialCubesIndex.Cells.Count.ToString(), "", FormatLayerSpatialBox(_spatialCubesIndex.ModelBox));

            foreach (SpatialCubeCell cell in _spatialCubesIndex.Cells)
            {
                grid.Rows.Add("Spatial", "1", cell.Id, "", "", "", "", "", cell.Bodies.Count.ToString(), "", FormatLayerSpatialBox(cell.Bounds));

                foreach (SpatialBodyRecord body in cell.Bodies)
                {
                    double x;
                    double y;
                    double z;
                    GetLayerSpatialBoxCenter(body == null ? null : body.BodyBox, out x, out y, out z);

                    grid.Rows.Add(
                        "Spatial",
                        "2",
                        cell.Id,
                        body == null ? "" : body.BodyIndex.ToString(),
                        body == null ? "" : body.DisplayName,
                        FormatLayerDouble(x),
                        FormatLayerDouble(y),
                        FormatLayerDouble(z),
                        body == null || body.CubeIds == null ? "0" : body.CubeIds.Count.ToString(),
                        body == null ? "" : body.CubeIdsText,
                        body == null ? "" : FormatLayerSpatialBox(body.BodyBox));
                }
            }
        }

        private void FillMergedViewGrid(DataGridView grid)
        {
            CopyCurrentBrowserGridToLayerGrid(grid, "Merged");
        }

        private void FillFastCacheGrid(DataGridView grid)
        {
            AddLayerColumn(grid, "Layer", "Layer");
            AddLayerColumn(grid, "Level", "Level");
            AddLayerColumn(grid, "Type", "Type");
            AddLayerColumn(grid, "Name", "Name / имя");
            AddLayerColumn(grid, "X", "X");
            AddLayerColumn(grid, "Y", "Y");
            AddLayerColumn(grid, "Z", "Z");
            AddLayerColumn(grid, "CanMove", "CanMove");
            AddLayerColumn(grid, "CanRename", "CanRename");
            AddLayerColumn(grid, "HasNativeObject", "HasNativeObject");

            if (_browserTreeNamesOnlySnapshot == null || _browserTreeNamesOnlySnapshot.GridItems == null || _browserTreeNamesOnlySnapshot.GridItems.Count == 0)
            {
                grid.Rows.Add("FastCache", "0", "<empty>", "Fast cache is empty. Use full Refresh browser tree first.", "", "", "", "", "", "");
                return;
            }

            foreach (BrowserTreeGridItem item in _browserTreeNamesOnlySnapshot.GridItems)
            {
                if (item == null)
                {
                    continue;
                }

                grid.Rows.Add(
                    "FastCache",
                    item.Depth.ToString(CultureInfo.InvariantCulture),
                    item.ObjectKind,
                    item.Name,
                    item.HasCenter ? FormatGridDouble(item.X) : "",
                    item.HasCenter ? FormatGridDouble(item.Y) : "",
                    item.HasCenter ? FormatGridDouble(item.Z) : "",
                    item.CanMove.ToString(),
                    item.CanRename.ToString(),
                    (item.NativeObject != null).ToString());
            }
        }

        private void FillModelPreviewGrid(DataGridView grid)
        {
            AddLayerColumn(grid, "Parameter", "Parameter");
            AddLayerColumn(grid, "Value", "Value");

            grid.Rows.Add("Version", AppBuild.Version + " — " + AppBuild.BuildName);
            grid.Rows.Add("Browser grid rows", dataGridViewIptBrowserTree == null ? "0" : dataGridViewIptBrowserTree.Rows.Count.ToString());
            grid.Rows.Add("Legacy nodes", treeViewIptLegacyBrowserTree == null ? "0" : CountTreeViewNodes(treeViewIptLegacyBrowserTree.Nodes).ToString());
            grid.Rows.Add("Spatial BASE ready", (_spatialCubesIndex != null && _spatialCubesIndex.IsReady).ToString());
            grid.Rows.Add("Spatial cubes", (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady) ? "0" : _spatialCubesIndex.Cells.Count.ToString());
            grid.Rows.Add("Spatial bodies", (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady) ? "0" : _spatialCubesIndex.Bodies.Count.ToString());
            grid.Rows.Add("Fast cache rows", (_browserTreeNamesOnlySnapshot == null || _browserTreeNamesOnlySnapshot.GridItems == null) ? "0" : _browserTreeNamesOnlySnapshot.GridItems.Count.ToString());

            PartDocument partDoc;
            if (TryGetActivePartDocument(out partDoc))
            {
                grid.Rows.Add("Active document", SafeGetPartDisplayName(partDoc));
            }
            else
            {
                grid.Rows.Add("Active document", "<no active PartDocument>");
            }
        }

        private static void GetLayerSpatialBoxCenter(SpatialBox box, out double x, out double y, out double z)
        {
            if (box == null)
            {
                x = 0.0;
                y = 0.0;
                z = 0.0;
                return;
            }

            x = (box.MinX + box.MaxX) * 0.5;
            y = (box.MinY + box.MaxY) * 0.5;
            z = (box.MinZ + box.MaxZ) * 0.5;
        }

        private static string FormatLayerSpatialBox(SpatialBox box)
        {
            if (box == null)
            {
                return string.Empty;
            }

            return "X[" + FormatLayerDouble(box.MinX) + ".." + FormatLayerDouble(box.MaxX) + "] " +
                   "Y[" + FormatLayerDouble(box.MinY) + ".." + FormatLayerDouble(box.MaxY) + "] " +
                   "Z[" + FormatLayerDouble(box.MinZ) + ".." + FormatLayerDouble(box.MaxZ) + "]";
        }

        private static string FormatLayerDouble(double value)
        {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        // ============================================================
        // Spatial cubes index / виртуальная база кубиков .ipt
        // ============================================================
        private void InitializeSpatialCubesUi()
        {
            using (AppLogger.Scope("InitializeSpatialCubesUi"))
            {
            AddSpatialColumnsToBrowserTreeGrid();

            TabPage tabPageCubes = new TabPage();
            tabPageCubes.Name = "tabPageIptSpatialCubes";
            tabPageCubes.Text = ".ipt cubes";
            tabPageCubes.UseVisualStyleBackColor = true;

            labelIptSpatialCubes = new Label();
            labelIptSpatialCubes.AutoSize = false;
            labelIptSpatialCubes.Location = new System.Drawing.Point(12, 12);
            labelIptSpatialCubes.Size = new System.Drawing.Size(1140, 44);
            labelIptSpatialCubes.Text = "Spatial cubes index: OFF. Нажмите Fast build spatial cubes BASE, чтобы быстро построить виртуальную базу кубиков модели.";

            ButtonIptBuildSpatialCubes = new Button();
            ButtonIptBuildSpatialCubes.Location = new System.Drawing.Point(12, 62);
            ButtonIptBuildSpatialCubes.Size = new System.Drawing.Size(190, 52);
            ButtonIptBuildSpatialCubes.Text = "Fast build spatial\r\ncubes BASE";
            ButtonIptBuildSpatialCubes.UseVisualStyleBackColor = true;
            ButtonIptBuildSpatialCubes.Click += ButtonIptBuildSpatialCubes_Click;

            ButtonIptPreviewSpatialCube = new Button();
            ButtonIptPreviewSpatialCube.Location = new System.Drawing.Point(214, 62);
            ButtonIptPreviewSpatialCube.Size = new System.Drawing.Size(220, 52);
            ButtonIptPreviewSpatialCube.Text = "Preview selected cube\r\nin Inventor";
            ButtonIptPreviewSpatialCube.UseVisualStyleBackColor = true;
            ButtonIptPreviewSpatialCube.Click += ButtonIptPreviewSpatialCube_Click;

            ButtonIptClearSpatialCubePreview = new Button();
            ButtonIptClearSpatialCubePreview.Location = new System.Drawing.Point(446, 62);
            ButtonIptClearSpatialCubePreview.Size = new System.Drawing.Size(190, 52);
            ButtonIptClearSpatialCubePreview.Text = "Clear cube\r\npreview";
            ButtonIptClearSpatialCubePreview.UseVisualStyleBackColor = true;
            ButtonIptClearSpatialCubePreview.Click += ButtonIptClearSpatialCubePreview_Click;

            ButtonIptCustomRectSelectCubes = new Button();
            ButtonIptCustomRectSelectCubes.Location = new System.Drawing.Point(648, 62);
            ButtonIptCustomRectSelectCubes.Size = new System.Drawing.Size(240, 52);
            ButtonIptCustomRectSelectCubes.Text = "Custom rect select\r\nASPECT CORRECT v0.4.69";
            ButtonIptCustomRectSelectCubes.UseVisualStyleBackColor = true;
            ButtonIptCustomRectSelectCubes.Click += ButtonIptCustomRectSelectCubes_Click;

            comboBoxIptCustomRectPlane = new ComboBox();
            comboBoxIptCustomRectPlane.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxIptCustomRectPlane.Location = new System.Drawing.Point(900, 68);
            comboBoxIptCustomRectPlane.Size = new System.Drawing.Size(250, 24);
            comboBoxIptCustomRectPlane.Items.Add("Front X/Y");
            comboBoxIptCustomRectPlane.Items.Add("Front X/Z");
            comboBoxIptCustomRectPlane.Items.Add("Side Y/Z");
            comboBoxIptCustomRectPlane.SelectedIndex = 0;

            comboBoxIptCustomRectBodyMode = new ComboBox();
            comboBoxIptCustomRectBodyMode.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxIptCustomRectBodyMode.Location = new System.Drawing.Point(900, 98);
            comboBoxIptCustomRectBodyMode.Size = new System.Drawing.Size(250, 24);
            comboBoxIptCustomRectBodyMode.Items.Add("1. Cube hit: all bodies");
            comboBoxIptCustomRectBodyMode.Items.Add("2. Cube hit: filtered bodies");
            comboBoxIptCustomRectBodyMode.Items.Add("3. RangeBox fully inside TraceBox");

            ButtonIptToggleBodyRangeBoxes = new Button();
            ButtonIptToggleBodyRangeBoxes.Location = new System.Drawing.Point(648, 120);
            ButtonIptToggleBodyRangeBoxes.Size = new System.Drawing.Size(240, 46);
            ButtonIptToggleBodyRangeBoxes.Text = "Show / Hide body\r\nRangeBoxes";
            ButtonIptToggleBodyRangeBoxes.UseVisualStyleBackColor = true;
            ButtonIptToggleBodyRangeBoxes.Click += ButtonIptToggleBodyRangeBoxes_Click;

            // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES
            // FROZEN UI MODES: old dropdown items are intentionally not added to the live ComboBox.
            // comboBoxIptCustomRectBodyMode.Items.Add("AUTO normal");
            // comboBoxIptCustomRectBodyMode.Items.Add("Whole selected cubes");
            // comboBoxIptCustomRectBodyMode.Items.Add("Precise body-box");
            comboBoxIptCustomRectBodyMode.SelectedIndex = 1;

            treeViewIptSpatialCubes = new TreeView();
            treeViewIptSpatialCubes.Location = new System.Drawing.Point(12, 176);
            treeViewIptSpatialCubes.Size = new System.Drawing.Size(1140, 592);
            treeViewIptSpatialCubes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeViewIptSpatialCubes.AfterSelect += TreeViewIptSpatialCubes_AfterSelect;

            tabPageCubes.Controls.Add(labelIptSpatialCubes);
            tabPageCubes.Controls.Add(ButtonIptBuildSpatialCubes);
            tabPageCubes.Controls.Add(ButtonIptPreviewSpatialCube);
            tabPageCubes.Controls.Add(ButtonIptClearSpatialCubePreview);
            tabPageCubes.Controls.Add(ButtonIptCustomRectSelectCubes);
            tabPageCubes.Controls.Add(ButtonIptToggleBodyRangeBoxes);
            tabPageCubes.Controls.Add(comboBoxIptCustomRectPlane);
            tabPageCubes.Controls.Add(comboBoxIptCustomRectBodyMode);
            tabPageCubes.Controls.Add(treeViewIptSpatialCubes);

            tabControl1.Controls.Add(tabPageCubes);
        
            }}

        // ============================================================
        // v0.4.47: Legacy Browser Tree area from old project
        // MyFirstInventorPlugin_VS2017_Lesson5_CSharp_Tabs_IPT_IAM_BROWSER_TREE_EXPORT_FIX_IO_NAMES
        // This is an additional TreeView area. It does NOT replace the current editable DataGridView Browser tree.
        // ============================================================
        private void InitializeLegacyBrowserTreeAreaUi()
        {
            using (AppLogger.Scope("InitializeLegacyBrowserTreeAreaUi"))
            {
            if (tabPageIpt == null)
            {
                return;
            }

            tabPageIpt.AutoScroll = true;

            labelIptLegacyBrowserTree = new Label();
            labelIptLegacyBrowserTree.AutoSize = true;
            labelIptLegacyBrowserTree.Location = new System.Drawing.Point(320, 790);
            labelIptLegacyBrowserTree.Name = "labelIptLegacyBrowserTree";
            labelIptLegacyBrowserTree.Size = new System.Drawing.Size(390, 16);
            labelIptLegacyBrowserTree.TabIndex = 100;
            labelIptLegacyBrowserTree.Text = "Legacy browser tree / старое дерево браузера Inventor: 0";

            treeViewIptLegacyBrowserTree = new TreeView();
            treeViewIptLegacyBrowserTree.Location = new System.Drawing.Point(323, 812);
            treeViewIptLegacyBrowserTree.Name = "treeViewIptLegacyBrowserTree";
            treeViewIptLegacyBrowserTree.Size = new System.Drawing.Size(820, 220);
            treeViewIptLegacyBrowserTree.TabIndex = 101;
            treeViewIptLegacyBrowserTree.HideSelection = false;
            treeViewIptLegacyBrowserTree.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;

            ButtonIptLegacyRefreshBrowserTree = new Button();
            ButtonIptLegacyRefreshBrowserTree.Location = new System.Drawing.Point(323, 1045);
            ButtonIptLegacyRefreshBrowserTree.Name = "ButtonIptLegacyRefreshBrowserTree";
            ButtonIptLegacyRefreshBrowserTree.Size = new System.Drawing.Size(149, 52);
            ButtonIptLegacyRefreshBrowserTree.TabIndex = 102;
            ButtonIptLegacyRefreshBrowserTree.Text = "Refresh legacy\r\nbrowser tree";
            ButtonIptLegacyRefreshBrowserTree.UseVisualStyleBackColor = true;
            ButtonIptLegacyRefreshBrowserTree.Click += ButtonIptLegacyRefreshBrowserTree_Click;

            ButtonIptLegacyRefreshBrowserTreeSpatialBase = new Button();
            ButtonIptLegacyRefreshBrowserTreeSpatialBase.Location = new System.Drawing.Point(484, 1045);
            ButtonIptLegacyRefreshBrowserTreeSpatialBase.Name = "ButtonIptLegacyRefreshBrowserTreeSpatialBase";
            ButtonIptLegacyRefreshBrowserTreeSpatialBase.Size = new System.Drawing.Size(170, 52);
            ButtonIptLegacyRefreshBrowserTreeSpatialBase.TabIndex = 105;
            ButtonIptLegacyRefreshBrowserTreeSpatialBase.Text = "Refresh legacy #2\r\nfrom spatial BASE";
            ButtonIptLegacyRefreshBrowserTreeSpatialBase.UseVisualStyleBackColor = true;
            ButtonIptLegacyRefreshBrowserTreeSpatialBase.Click += ButtonIptLegacyRefreshBrowserTreeSpatialBase_Click;

            ButtonIptLegacyCopyBrowserTree = new Button();
            ButtonIptLegacyCopyBrowserTree.Location = new System.Drawing.Point(665, 1045);
            ButtonIptLegacyCopyBrowserTree.Name = "ButtonIptLegacyCopyBrowserTree";
            ButtonIptLegacyCopyBrowserTree.Size = new System.Drawing.Size(141, 52);
            ButtonIptLegacyCopyBrowserTree.TabIndex = 103;
            ButtonIptLegacyCopyBrowserTree.Text = "Copy legacy tree\r\nto clipboard";
            ButtonIptLegacyCopyBrowserTree.UseVisualStyleBackColor = true;
            ButtonIptLegacyCopyBrowserTree.Click += ButtonIptLegacyCopyBrowserTree_Click;

            ButtonIptLegacySaveBrowserTree = new Button();
            ButtonIptLegacySaveBrowserTree.Location = new System.Drawing.Point(812, 1045);
            ButtonIptLegacySaveBrowserTree.Name = "ButtonIptLegacySaveBrowserTree";
            ButtonIptLegacySaveBrowserTree.Size = new System.Drawing.Size(155, 52);
            ButtonIptLegacySaveBrowserTree.TabIndex = 104;
            ButtonIptLegacySaveBrowserTree.Text = "Save legacy tree\r\nJSON file";
            ButtonIptLegacySaveBrowserTree.UseVisualStyleBackColor = true;
            ButtonIptLegacySaveBrowserTree.Click += ButtonIptLegacySaveBrowserTree_Click;

            tabPageIpt.Controls.Add(labelIptLegacyBrowserTree);
            tabPageIpt.Controls.Add(treeViewIptLegacyBrowserTree);
            tabPageIpt.Controls.Add(ButtonIptLegacyRefreshBrowserTree);
            tabPageIpt.Controls.Add(ButtonIptLegacyRefreshBrowserTreeSpatialBase);
            tabPageIpt.Controls.Add(ButtonIptLegacyCopyBrowserTree);
            tabPageIpt.Controls.Add(ButtonIptLegacySaveBrowserTree);

            UpdateIptLegacyBrowserTreeCaption();

            AppLogger.Log(
                "LEGACY_BROWSER_TREE_AREA_ADDED",
                "InitializeLegacyBrowserTreeAreaUi",
                "SourceProject=MyFirstInventorPlugin_VS2017_Lesson5_CSharp_Tabs_IPT_IAM_BROWSER_TREE_EXPORT_FIX_IO_NAMES; DoesNotReplaceEditableGrid=True");

            }}

        private void AddSpatialColumnsToBrowserTreeGrid()
        {
            using (AppLogger.Scope("AddSpatialColumnsToBrowserTreeGrid"))
            {
            if (dataGridViewIptBrowserTree == null)
            {
                return;
            }

            if (!dataGridViewIptBrowserTree.Columns.Contains("ColumnIptTreeCubeCount"))
            {
                DataGridViewTextBoxColumn colCount = new DataGridViewTextBoxColumn();
                colCount.Name = "ColumnIptTreeCubeCount";
                colCount.HeaderText = "Cubes";
                colCount.Width = 60;
                colCount.ReadOnly = true;
                dataGridViewIptBrowserTree.Columns.Add(colCount);
            }

            if (!dataGridViewIptBrowserTree.Columns.Contains("ColumnIptTreeCubeIds"))
            {
                DataGridViewTextBoxColumn colIds = new DataGridViewTextBoxColumn();
                colIds.Name = "ColumnIptTreeCubeIds";
                colIds.HeaderText = "Cube IDs";
                colIds.Width = 240;
                colIds.ReadOnly = true;
                dataGridViewIptBrowserTree.Columns.Add(colIds);
            }
        
            }}

        private void ButtonIptBuildSpatialCubes_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptBuildSpatialCubes_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            try
            {
                ClearSpatialCubePreview(partDoc);

                // Первый безопасный режим: 2 x 2 x 2 = 8 виртуальных кубиков.
                // Потом это число можно вынести в NumericUpDown и менять без переписывания алгоритма.
                int divisionsPerAxis = 2;
                double tolerance = 0.001;

                // 20:30 15.06.2026 InventorIptOrg_v0_4_21_CUSTOM_RECT_CACHED_BODY_BOX_HIT
                // FAST SPATIAL BASE BUILD: не вызываем GetBodyDisplayName для каждого тела и не запускаем полный RefreshIptBrowserTree.
                // БАЗА кубиков должна строиться один раз быстро: body.RangeBox -> cached SpatialBodyRecord.BodyBox -> cells.
                // Имена тел в fast-base режиме технические: SurfaceBody N. Это осознанная замена дорогому COM-поиску отображаемых имён.
                _spatialCubesIndex = SpatialCubesIndex.Build(
                    partDoc,
                    divisionsPerAxis,
                    tolerance,
                    GetComIdentityKey,
                    null);

                RefreshSpatialCubesTree();
                AppLogger.Log(
                    "SPATIAL_CUBES_FAST_BASE_READY",
                    "ButtonIptBuildSpatialCubes_Click",
                    "DisplayNameProvider=GenericSurfaceBodyNames; RefreshIptBrowserTreeSkipped=True; Bodies=" + _spatialCubesIndex.Bodies.Count.ToString() + "; Cells=" + _spatialCubesIndex.Cells.Count.ToString());

                LoggedMessageBox.Show(
                    "FAST spatial cubes BASE построена.\r\n\r\n" +
                    "Кубиков: " + _spatialCubesIndex.Cells.Count.ToString() + "\r\n" +
                    "Тел: " + _spatialCubesIndex.Bodies.Count.ToString() + "\r\n" +
                    "Разбиение: " + divisionsPerAxis.ToString() + " x " + divisionsPerAxis.ToString() + " x " + divisionsPerAxis.ToString() + "\r\n" +
                    "Tolerance: " + tolerance.ToString(CultureInfo.InvariantCulture) + "\r\n\r\n" +
                    "FAST BASE: GetBodyDisplayName и RefreshIptBrowserTree пропущены.\r\n" +
                    "Теперь рамочный выбор работает по cached БАЗЕ кубиков и cached BodyBox.");
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem building spatial cubes index");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        private void RefreshSpatialCubesTree()
        {
            using (AppLogger.Scope("RefreshSpatialCubesTree"))
            {
            if (treeViewIptSpatialCubes == null)
            {
                return;
            }

            treeViewIptSpatialCubes.BeginUpdate();

            try
            {
                treeViewIptSpatialCubes.Nodes.Clear();

                if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady)
                {
                    TreeNode offNode = new TreeNode("Spatial cubes index: OFF / кубики ещё не построены");
                    treeViewIptSpatialCubes.Nodes.Add(offNode);
                    UpdateIptSpatialCubesCaption();
                    return;
                }

                TreeNode root = new TreeNode(
                    "Spatial cubes index: ON | cells=" + _spatialCubesIndex.Cells.Count.ToString() +
                    " | bodies=" + _spatialCubesIndex.Bodies.Count.ToString() +
                    " | created=" + _spatialCubesIndex.CreatedAt.ToString("HH:mm:ss dd.MM.yyyy"));

                root.Nodes.Add("Document: " + _spatialCubesIndex.DocumentDisplayName);
                root.Nodes.Add("Model bounds db: " + FormatSpatialBox(_spatialCubesIndex.ModelBox) + " | volume(db^3)=" + FormatSpatialDouble(_spatialCubesIndex.ModelBox.Volume));

                foreach (SpatialCubeCell cell in _spatialCubesIndex.Cells)
                {
                    TreeNode cellNode = new TreeNode(
                        cell.Id +
                        " | bodies=" + cell.Bodies.Count.ToString() +
                        " | volume(db^3)=" + FormatSpatialDouble(cell.Volume) +
                        " | " + FormatSpatialBox(cell.Bounds));

                    cellNode.Tag = cell;

                    foreach (SpatialBodyRecord record in cell.Bodies)
                    {
                        TreeNode bodyNode = new TreeNode(
                            record.DisplayName +
                            " | cubes=" + (record.CubeIds == null ? "0" : record.CubeIds.Count.ToString()) +
                            " | " + record.CubeIdsText);
                        bodyNode.Tag = record;
                        cellNode.Nodes.Add(bodyNode);
                    }

                    root.Nodes.Add(cellNode);
                }

                treeViewIptSpatialCubes.Nodes.Add(root);
                root.Expand();
                UpdateIptSpatialCubesCaption();
            }
            finally
            {
                treeViewIptSpatialCubes.EndUpdate();
            }
        
            }}

        private void UpdateIptSpatialCubesCaption()
        {
            using (AppLogger.Scope("UpdateIptSpatialCubesCaption"))
            {
            if (labelIptSpatialCubes == null)
            {
                return;
            }

            if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady)
            {
                labelIptSpatialCubes.Text = "Spatial cubes index: OFF. Рамочный выбор работает по старому полному перебору SurfaceBodies.";
                return;
            }

            labelIptSpatialCubes.Text =
                "Spatial cubes index: ON | cells=" + _spatialCubesIndex.Cells.Count.ToString() +
                " | bodies=" + _spatialCubesIndex.Bodies.Count.ToString() +
                " | divisions=" + _spatialCubesIndex.DivisionsPerAxis.ToString() + "x" + _spatialCubesIndex.DivisionsPerAxis.ToString() + "x" + _spatialCubesIndex.DivisionsPerAxis.ToString() +
                " | tolerance=" + _spatialCubesIndex.Tolerance.ToString(CultureInfo.InvariantCulture) +
                " | document=" + _spatialCubesIndex.DocumentDisplayName;
        
            }}

        private void TreeViewIptSpatialCubes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            using (AppLogger.Scope("TreeViewIptSpatialCubes_AfterSelect"))
            {
            SpatialCubeCell cell = e.Node == null ? null : e.Node.Tag as SpatialCubeCell;
            if (cell == null || labelIptSpatialCubes == null)
            {
                UpdateIptSpatialCubesCaption();
                return;
            }

            labelIptSpatialCubes.Text =
                "Selected cube: " + cell.Id +
                " | bodies=" + cell.Bodies.Count.ToString() +
                " | volume(db^3)=" + FormatSpatialDouble(cell.Volume) +
                " | " + FormatSpatialBox(cell.Bounds);
        
            }}

        private void ButtonIptPreviewSpatialCube_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptPreviewSpatialCube_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            SpatialCubeCell cell;
            if (!TryGetSelectedSpatialCubeCell(out cell))
            {
                LoggedMessageBox.Show("Выберите кубик в дереве .ipt cubes, затем нажмите Preview selected cube in Inventor.");
                return;
            }

            ShowSpatialCubePreview(partDoc, cell);
        
            }}

        private void ButtonIptClearSpatialCubePreview_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptClearSpatialCubePreview_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            ClearSpatialCubePreview(partDoc);
        
            }}

        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT =====================================================================================================
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT CUSTOM RECT SELECT / новый режим выбора кубиков собственной экранной рамкой.
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT Это не Inventor WindowSelect: Inventor больше не решает, какие SurfaceBody вернуть при drag left/right.
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT Программа показывает прозрачный overlay, ловит две экранные точки, нормализует Rectangle и выбирает кубики по пересечению 2D-проекций.
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT MVP рассчитан на ортографический вид спереди/сбоку и выбранную плоскость из comboBox: Front X/Y, Front X/Z, Side Y/Z.
        // 10:10 15.06.2026 InventorIptOrg_v0_4_12_CUSTOM_CUBE_RECT_SELECT =====================================================================================================
        // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT =====================================================================================================
        // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT CUSTOM RECT CENTER HIT / уточнение custom-рамки v0.4.12.
        // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT v0.4.12 выбирал кубик при любом пересечении 2D-прямоугольника кубика с экранной рамкой.
        // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT На границах это было слишком чувствительно: рамка цепляла соседние сектора и могла дать 4 или 8 кубиков.
        // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT v0.4.13 выбирает кубик только если центр его 2D-проекции попал внутрь экранной рамки.
        // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT Ожидаемый эффект: меньше случайных соседних кубиков при небольшом заходе рамки за границу сектора.
        // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT =====================================================================================================
        // 17:25 15.06.2026 InventorIptOrg_v0_4_14_CUSTOM_RECT_BODY_FILTER =====================================================================================================
        // 17:25 15.06.2026 InventorIptOrg_v0_4_14_CUSTOM_RECT_BODY_FILTER CUSTOM RECT BODY FILTER / уточнение custom-рамки v0.4.13.
        // 17:25 15.06.2026 InventorIptOrg_v0_4_14_CUSTOM_RECT_BODY_FILTER v0.4.13 точно выбирал кубики, но дальше добавлял ВСЕ тела из выбранных кубиков.
        // 17:25 15.06.2026 InventorIptOrg_v0_4_14_CUSTOM_RECT_BODY_FILTER Это было корректно для режима "whole cell", но тяжело и не совпадало с OLD-логикой, где кубики дают кандидатов, а не финальный список.
        // 17:25 15.06.2026 InventorIptOrg_v0_4_14_CUSTOM_RECT_BODY_FILTER v0.4.14 оставляет выбор кубиков по центру кубика, затем фильтрует тела внутри выбранных кубиков.
        // 17:25 15.06.2026 InventorIptOrg_v0_4_14_CUSTOM_RECT_BODY_FILTER Body filter: тело проходит, если центр 2D-проекции его RangeBox попал внутрь пользовательской экранной рамки.
        // 17:25 15.06.2026 InventorIptOrg_v0_4_14_CUSTOM_RECT_BODY_FILTER Ожидаемый эффект: выбранные кубики остаются стабильными, но в группу передаются не все тела кубика, а только тела из области рамки.
        // 17:25 15.06.2026 InventorIptOrg_v0_4_14_CUSTOM_RECT_BODY_FILTER =====================================================================================================
        // 17:45 15.06.2026 InventorIptOrg_v0_4_15_CUSTOM_RECT_FAST_BODY_ONLY =====================================================================================================
        // 17:45 15.06.2026 InventorIptOrg_v0_4_15_CUSTOM_RECT_FAST_BODY_ONLY FAST BODY ONLY / удаление тяжёлой механики после быстрого custom-выбора.
        // 17:45 15.06.2026 InventorIptOrg_v0_4_15_CUSTOM_RECT_FAST_BODY_ONLY v0.4.14 уже быстро строит выбор из готовой БАЗЫ кубиков: frame -> cells -> candidate bodies -> body-filter.
        // 17:45 15.06.2026 InventorIptOrg_v0_4_15_CUSTOM_RECT_FAST_BODY_ONLY Дальше тормозил не выбор, а старый хвост: AddFeaturesForBodyToGroup/GetFeaturesForBody, SelectSet.Select и Refresh списков.
        // 17:45 15.06.2026 InventorIptOrg_v0_4_15_CUSTOM_RECT_FAST_BODY_ONLY В этом режиме custom-рамка добавляет только тела в myBodyGroup и локальный список формы.
        // 17:45 15.06.2026 InventorIptOrg_v0_4_15_CUSTOM_RECT_FAST_BODY_ONLY Авто-поиск features, UI SelectSet и полный Refresh списков намеренно пропущены.
        // 17:45 15.06.2026 InventorIptOrg_v0_4_15_CUSTOM_RECT_FAST_BODY_ONLY =====================================================================================================
        // 18:35 15.06.2026 InventorIptOrg_v0_4_16_FAST_CUBE_BODIES_HOTFIX =====================================================================================================
        // 18:35 15.06.2026 InventorIptOrg_v0_4_16_FAST_CUBE_BODIES_HOTFIX Startup hotfix: shortened executable/assembly namespace to avoid CLR 8007007a startup crash.
        // 18:35 15.06.2026 InventorIptOrg_v0_4_16_FAST_CUBE_BODIES_HOTFIX Custom rectangle whole-cell mode: selected cube centers -> ALL distinct bodies from selected cells -> AddBodyToGroup only.
        // 18:35 15.06.2026 InventorIptOrg_v0_4_16_FAST_CUBE_BODIES_HOTFIX Body-filter, GetFeaturesForBody, AddFeaturesForBodyToGroup, SelectSet.Select, and full Refresh lists are skipped.
        // 18:35 15.06.2026 InventorIptOrg_v0_4_16_FAST_CUBE_BODIES_HOTFIX =====================================================================================================
        // 18:55 15.06.2026 InventorIptOrg_v0_4_17_CUSTOM_RECT_LIST_ONLY =====================================================================================================
        // 18:55 15.06.2026 InventorIptOrg_v0_4_17_CUSTOM_RECT_LIST_ONLY FAST LIST ONLY / следующий шаг после v0.4.16.
        // 18:55 15.06.2026 InventorIptOrg_v0_4_17_CUSTOM_RECT_LIST_ONLY v0.4.16 уже не запускал features/SelectSet/Refresh, но всё ещё писал myBodyGroup AttributeSet через AddBodyToGroup.
        // 18:55 15.06.2026 InventorIptOrg_v0_4_17_CUSTOM_RECT_LIST_ONLY Запись атрибутов в Inventor для 51 тела занимала секунды, хотя выбор из spatial BASE уже занимал миллисекунды.
        // 18:55 15.06.2026 InventorIptOrg_v0_4_17_CUSTOM_RECT_LIST_ONLY v0.4.17 не пишет myBodyGroup и не вызывает AddBodyToGroup: только заменяет видимый список формы телами из выбранных кубиков.
        // 18:55 15.06.2026 InventorIptOrg_v0_4_17_CUSTOM_RECT_LIST_ONLY Это режим проверки/просмотра результата рамки: frame -> cells -> ALL distinct bodies -> visible list only.
        // 18:55 15.06.2026 InventorIptOrg_v0_4_17_CUSTOM_RECT_LIST_ONLY =====================================================================================================
        // 19:10 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY =====================================================================================================
        // 19:10 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY FAST HIDE READY / исправление ограничения v0.4.17.
        // 19:10 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY v0.4.17 был очень быстрым, но только показывал тела в списке формы и не писал myBodyGroup Attributes.
        // 19:10 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY Поэтому старый Toggle/Hide, который искал тела через AttributeManager.FindObjects("myBodyGroup", ...), не видел выбранные рамкой тела.
        // 19:10 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY v0.4.18 оставляет быстрый LIST ONLY path, но Toggle/Hide теперь сначала работает напрямую по текущему visible-list BodyListItem.
        // 19:10 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY Если список формы пустой, остаётся fallback к старому атрибутному myBodyGroup режиму.
        // 19:10 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY =====================================================================================================
        // 21:05 15.06.2026 InventorIptOrg_v0_4_22_CUSTOM_RECT_NORMAL_SELECT =====================================================================================================
        // 21:05 15.06.2026 InventorIptOrg_v0_4_22_CUSTOM_RECT_NORMAL_SELECT NORMAL SELECT / единая нормальная логика custom-рамки.
        // 21:05 15.06.2026 InventorIptOrg_v0_4_22_CUSTOM_RECT_NORMAL_SELECT Кубики снова являются ускоряющей БАЗОЙ кандидатов, а не единственным финальным смыслом выбора.
        // 21:05 15.06.2026 InventorIptOrg_v0_4_22_CUSTOM_RECT_NORMAL_SELECT AUTO normal: большая рамка по сектору = Whole selected cubes, маленькая рамка по кусочку = Precise body-box hit.
        // 21:05 15.06.2026 InventorIptOrg_v0_4_22_CUSTOM_RECT_NORMAL_SELECT В обоих случаях результат попадает в текущий visible-list / Bodies in current group, Hide/Show работает по нему напрямую.
        // 21:05 15.06.2026 InventorIptOrg_v0_4_22_CUSTOM_RECT_NORMAL_SELECT Тяжёлые операции AddBodyToGroup/attributes, GetFeaturesForBody, SelectSet.Select и полный Refresh списков не запускаются.
        // 21:05 15.06.2026 InventorIptOrg_v0_4_22_CUSTOM_RECT_NORMAL_SELECT =====================================================================================================
        // 22:55 15.06.2026 InventorIptOrg_v0_4_23_CUSTOM_RECT_NORMAL_SELECT_FIX =====================================================================================================
        // 22:55 15.06.2026 InventorIptOrg_v0_4_23_CUSTOM_RECT_NORMAL_SELECT_FIX AUTO SAFE: AUTO normal больше не режет тела через Precise body-box автоматически.
        // 22:55 15.06.2026 InventorIptOrg_v0_4_23_CUSTOM_RECT_NORMAL_SELECT_FIX В AUTO normal кубики остаются главной безопасной БАЗОЙ выбора: выбраны кубики -> все тела выбранных кубиков в visible-list.
        // 22:55 15.06.2026 InventorIptOrg_v0_4_23_CUSTOM_RECT_NORMAL_SELECT_FIX Precise body-box оставлен только как ручной экспериментальный режим, когда пользователь сам хочет уточнять по 2D BodyBox.
        // 22:55 15.06.2026 InventorIptOrg_v0_4_23_CUSTOM_RECT_NORMAL_SELECT_FIX Это исправляет потерю тел вроде SurfaceBody 6, которые принадлежат выбранным кубикам, но отбрасывались AUTO-фильтром v0.4.22.
        // 22:55 15.06.2026 InventorIptOrg_v0_4_23_CUSTOM_RECT_NORMAL_SELECT_FIX =====================================================================================================
        // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES =====================================================================================================
        // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES TWO LIVE MODES / пользовательские определения #1 и #2.
        // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES #1 Cube hit: all bodies = рамка зацепила кубики -> из них быстро выгружаются ВСЕ тела без исключения в visible-list.
        // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES #2 Cube hit: filtered bodies = рамка зацепила кубики -> из них быстро выгружаются кандидаты -> по этой же рамке фильтруются cached BodyBox тел.
        // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES Старые пункты UI AUTO normal / Whole selected cubes / Precise body-box заморожены и не добавляются в dropdown.
        // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES ЗАМЕР НОВОГО РЕЖИМА: сравнение InventorIptOrg_v0_4_23_CUSTOM_RECT_NORMAL_SELECT_FIX -> InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES.
        // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES Контрольный лог InventorIptOrg_v0_4_23_CUSTOM_RECT_NORMAL_SELECT_FIX: SpatialCubesIndex.Build = 3970443 us, Bodies=72, Cells=8, DivisionsPerAxis=2.
        // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES Для v0.4.24 сравнивать события CUSTOM_RECT_CUBE_HIT_FILTERED_SELECTION_QUERY и CUSTOM_RECT_CUBE_HIT_FILTERED_SELECTION_APPLIED; реальные числа появятся после тестового запуска.
        // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES =====================================================================================================
        // 14:45 16.06.2026 InventorIptOrg_v0_4_25_CUSTOM_RECT_TRACE_BOX =====================================================================================================
        // 14:45 16.06.2026 InventorIptOrg_v0_4_25_CUSTOM_RECT_TRACE_BOX Добавлен SELECTION TRACE BOX: после custom rectangle select остаётся временный полупрозрачный marker/parallelepiped в Inventor ClientGraphics.
        // 14:45 16.06.2026 InventorIptOrg_v0_4_25_CUSTOM_RECT_TRACE_BOX Это НЕ настоящее тело IPT: в дереве модели ничего не создаётся, файл не засоряется, очистка идёт через Clear cube preview.
        // 14:45 16.06.2026 InventorIptOrg_v0_4_25_CUSTOM_RECT_TRACE_BOX ЗАМЕР НОВОГО РЕЖИМА: сравнение InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES -> InventorIptOrg_v0_4_25_CUSTOM_RECT_TRACE_BOX.
        // 14:45 16.06.2026 InventorIptOrg_v0_4_25_CUSTOM_RECT_TRACE_BOX Для v0.4.25 сравнивать событие CUSTOM_RECT_TRACE_BOX_PREVIEW_BUILT; выбор тел остаётся на событиях v0.4.24.
        // 14:45 16.06.2026 InventorIptOrg_v0_4_25_CUSTOM_RECT_TRACE_BOX =====================================================================================================
        // 15:15 16.06.2026 InventorIptOrg_v0_4_26_TOUCHED_CUBES_PREVIEW =====================================================================================================
        // 15:15 16.06.2026 InventorIptOrg_v0_4_26_TOUCHED_CUBES_PREVIEW Добавлен TOUCHED CUBES PREVIEW: после custom rectangle select все задетые spatial-cubes подсвечиваются отдельной янтарной временной ClientGraphics-графикой.
        // 15:15 16.06.2026 InventorIptOrg_v0_4_26_TOUCHED_CUBES_PREVIEW Это НЕ настоящие IPT-тела: графика не попадает в дерево модели и очищается через Clear cube preview вместе с синим selected-cube preview и зелёным trace box.
        // 15:15 16.06.2026 InventorIptOrg_v0_4_26_TOUCHED_CUBES_PREVIEW ЗАМЕР НОВОГО РЕЖИМА: сравнение InventorIptOrg_v0_4_25_CUSTOM_RECT_TRACE_BOX -> InventorIptOrg_v0_4_26_TOUCHED_CUBES_PREVIEW.
        // 15:15 16.06.2026 InventorIptOrg_v0_4_26_TOUCHED_CUBES_PREVIEW Для v0.4.26 сравнивать событие CUSTOM_RECT_TOUCHED_CUBES_PREVIEW_BUILT; выбор тел остаётся на событиях v0.4.24, trace box — на событиях v0.4.25.
        // 15:15 16.06.2026 InventorIptOrg_v0_4_26_TOUCHED_CUBES_PREVIEW =====================================================================================================
        // 16:15 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT =====================================================================================================
        // 16:15 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT ЗАМЕР НОВОГО РЕЖИМА: сравнение InventorIptOrg_v0_4_26_TOUCHED_CUBES_PREVIEW -> InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT.
        // 16:15 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT Главная правка: 2D-проекция кубиков и cached BodyBox строится через реальную камеру Inventor ActiveView.Camera, а не через грубую линейную ModelBox-формулу.
        // 16:15 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT База v0.4.27: 2D-проекция кубиков и cached BodyBox строится через реальную камеру Inventor ActiveView.Camera.
        // 17:10 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT Зелёный 3D trace box пересчитан через RealCamera screen unprojection, чтобы совпадать с экранной рамкой в orthographic Front view.
        // 17:10 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT Контрольные события: REAL_CAMERA_SCREEN_PROJECTOR_READY, CUSTOM_RECT_REAL_CAMERA_SELECTION_QUERY, CUSTOM_RECT_TRACE_BOX_RESTORED_REAL_CAMERA_ASPECT_CORRECT_MARKER.
        // 16:15 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT =====================================================================================================

        // 18:20 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT =====================================================================================================
        // 18:20 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT ЗАМЕР НОВОГО РЕЖИМА: сравнение InventorIptOrg_v0_4_29_REAL_CAMERA_TRACE_EXACT -> InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT.
        // 18:20 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT Проблема v0.4.29: при ViewRect W1065 H560 Camera.GetExtents давал aspect около 1.09 вместо viewport aspect около 1.90; зелёный trace box и RealCamera projector были сжаты по X.
        // 18:20 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT Решение v0.4.30: вводится EffectiveExtents. Для wide viewport сохраняем raw Height и считаем effective Width = raw Height * ViewportAspect.
        // 18:20 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT Важно: EffectiveExtents применяются в обе стороны — box->screen для touched cubes/body filter и screen->box для зелёного trace marker.
        // 18:20 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT Новые диагностические поля: RawExtents, ViewportAspect, CameraExtentsAspect, AspectCorrection, Effective Extents в строке Extents.
        // 18:20 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT =====================================================================================================

        // 18:45 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT_GITHUB_FINAL ==========================================
        // GITHUB FINAL ASSEMBLY / сборка для загрузки поверх текущей GitHub-базы InventorIptOrg_v0_4_1_RUNFIX.
        // СТАТУС 1: Select Frame + Add Inner/Hidden Bodies заменён рабочей вкладкой .ipt cubes: spatial-cube index + custom rectangle + RealCamera aspect-corrected projection.
        // СТАТУС 2: Create feature browser folder — следующая активная задача; старую область получения elements/features/browser nodes нужно реанимировать и ускорить.
        // СТАТУС 3: Apply edited name / XYZ to Inventor — отложено намеренно, в этой сборке не трогать.
        // Контроль v0.4.30: full-frame test выбрал 8/8 spatial cubes, 72/72 SurfaceBodies; Hide/Show работает напрямую по current visible-list.
        // ==============================================================================================================================
        private void ButtonIptCustomRectSelectCubes_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptCustomRectSelectCubes_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady || !_spatialCubesIndex.MatchesDocument(partDoc))
            {
                LoggedMessageBox.Show("Сначала откройте .ipt модель и нажмите Build spatial cubes index на вкладке .ipt cubes.");
                return;
            }

            CustomCubeRectPlane plane = GetSelectedCustomCubeRectPlane();
            string planeText = GetCustomCubeRectPlaneText(plane);
            CustomRectBodySelectionMode bodyMode = GetSelectedCustomRectBodySelectionMode();
            string bodyModeText = GetCustomRectBodySelectionModeText(bodyMode);

            DialogResult answer = LoggedMessageBox.Show(
                "Новый режим выберет КУБИКИ собственной экранной рамкой, а не Inventor WindowSelect.\r\n\r\n" +
                "1) Поставьте нужный вид Inventor, например Front.\r\n" +
                "2) Выберите плоскость в списке справа от кнопки. Сейчас: " + planeText + "\r\n" +
                "3) Выберите режим тела. Сейчас: " + bodyModeText + "\r\n" +
                "4) Нажмите OK.\r\n" +
                "5) На прозрачном overlay протяните рамку над графическим окном Inventor.\r\n\r\n" +
                "CUBE HIT + FEATURE BROWSER NODES FAST v0.4.32: после .ipt cubes выбора Features/browser-nodes строятся сразу из current visible-list; fallback использует BrowserNode SurfaceBody, если CreatedByFeature пустой.\r\n" +
                "#1 all bodies: зацепили кубики -> быстро показали все тела из этих кубиков.\r\n" +
                "#2 filtered bodies: зацепили кубики -> быстро получили кандидатов -> отфильтровали их по этой рамке через cached BodyBox.\r\n" +
                "REAL CAMERA ASPECT CORRECT: все задетые рамкой spatial-cubes подсветятся янтарным ClientGraphics.\r\n" +
                "TRACE BOX: зелёный 3D marker строится через RealCamera + viewport aspect correction, чтобы совпадать с экранной рамкой.\r\n" +
                "FAST HIDE READY: AddBodyToGroup/attributes, features/GetFeaturesForBody, SelectSet.Select и Refresh списков НЕ запускаются.\r\n" +
                "Hide/Show IPT bodies теперь работает напрямую по текущему списку рамки.\r\n" +
                "Направление протяжки не важно: слева-направо и справа-налево дадут один нормализованный Rectangle.",
                "Custom cube rectangle select",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (answer != DialogResult.OK)
            {
                return;
            }

            System.Drawing.Rectangle viewRect;
            if (!TryGetInventorViewScreenRectangle(out viewRect))
            {
                LoggedMessageBox.Show("Не удалось получить экранный прямоугольник окна Inventor. Попробуйте сделать Inventor активным окном и повторить.");
                return;
            }

            FormWindowState oldState = this.WindowState;
            try
            {
                this.WindowState = FormWindowState.Minimized;
            }
            catch
            {
            }

            CustomRectangleOverlay overlay = new CustomRectangleOverlay(viewRect, "Drag custom cube rectangle over Inventor view. Esc = cancel.");
            DialogResult overlayResult = overlay.ShowDialog();

            try
            {
                this.WindowState = oldState;
                this.Activate();
            }
            catch
            {
            }

            if (overlayResult != DialogResult.OK || overlay.SelectedRectangle.Width < 4 || overlay.SelectedRectangle.Height < 4)
            {
                LoggedMessageBox.Show("Custom rectangle selection отменён или рамка слишком маленькая.");
                return;
            }

            SpatialQueryResult screenQuery = BuildSpatialQueryFromScreenRectangle(overlay.SelectedRectangle, viewRect, plane, bodyMode);
            if (screenQuery == null || screenQuery.TouchedCells == null || screenQuery.TouchedCells.Count == 0)
            {
                LoggedMessageBox.Show("Экранная рамка не задела ни одного кубика. Проверьте вид Inventor и выбранную плоскость: " + planeText);
                return;
            }

            int selectedInInventorCount = 0;
            int addedCount = AddSpatialQueryBodiesToListOnly(partDoc, screenQuery, out selectedInInventorCount);

            bool touchedCubesPreviewShown = ShowTouchedSpatialCubesPreview(partDoc, screenQuery.TouchedCells, bodyMode, overlay.SelectedRectangle, viewRect, plane);

            // 17:10 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT =====================================================================================================
            // Возвращён зелёный SELECTION TRACE BOX из v0.4.25-v0.4.26, потому что без него невозможно глазами понять, где программа оставила след рамочного выбора.
            // ВАЖНО: выбор кубиков и body-filter остаются на real-camera 2D screen projection из v0.4.27. Зелёный trace box — визуальный ориентир/след, а не источник истины для отбора тел.
            // Контрольные события: CUSTOM_RECT_TRACE_BOX_CALCULATED, CUSTOM_RECT_TRACE_BOX_PREVIEW_BUILT, CUSTOM_RECT_TRACE_BOX_RESTORED_REAL_CAMERA_ASPECT_CORRECT_MARKER.
            // ============================================================================================================================================================================
            SpatialBox traceBox = BuildCustomRectTraceBoxFromScreenRectangle(overlay.SelectedRectangle, viewRect, plane, screenQuery);
            bool traceBoxPreviewShown = ShowCustomRectTraceBoxPreview(partDoc, traceBox, overlay.SelectedRectangle, viewRect, plane, screenQuery);
            AppLogger.Log(
                "CUSTOM_RECT_TRACE_BOX_RESTORED_REAL_CAMERA_ASPECT_CORRECT_MARKER",
                "ButtonIptCustomRectSelectCubes_Click",
                "Shown=" + traceBoxPreviewShown.ToString() +
                "; TraceBox=" + FormatSpatialBox(traceBox) +
                "; SelectionUsesRealCameraProjection=True" +
                "; TraceBoxIsRealCameraAspectCorrectMarker=True" +
                "; RealCameraProjection=" + (screenQuery == null ? "" : screenQuery.ProjectionMode) +
                "; SelectionRect=" + FormatScreenRectangle(overlay.SelectedRectangle) +
                "; ViewRect=" + FormatScreenRectangle(viewRect));

            // 18:55 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY
            // FAST HIDE READY: intentionally skip AddBodyToGroup/attributes, RefreshIptGroupList, RefreshIptFeatureList, ShowSpatialCubePreview.
            // The visible list is replaced directly from SpatialBodyRecord.DisplayName. No Inventor document attributes are modified.

            LoggedMessageBox.Show(
                "Custom cube rectangle selection выполнен.\r\n\r\n" +
                "Режим: наша экранная рамка, НЕ Inventor WindowSelect.\r\n" +
                "Hit mode: real-camera 2D screen projection; центр кубика внутри рамки; small-area fallback: центр рамки внутри кубика.\r\n" +
                "Body mode: " + screenQuery.ResolvedBodySelectionMode + "\r\n" +
                "Mode #1: Cube hit: all bodies = все тела из задетых кубиков без body-filter.\r\n" +
                "Mode #2: Cube hit: filtered bodies = кандидаты из кубиков + фильтр по пересечению cached BodyBox с рамкой.\r\n" +
                "Mode #3: RangeBox fully inside TraceBox = только тела, чей SurfaceBody.RangeBox полностью внутри салатового Selection Trace Box.\r\n" +
                "Body filter: " + screenQuery.BodyFilterMode + "\r\n" +
                "FAST HIDE READY: AddBodyToGroup/attributes / features / SelectSet / Refresh списков пропущены.\r\n" +
                "Hide/Show IPT bodies: READY, работает напрямую по текущему visible-list.\r\n" +
                "Touched cubes preview: " + (touchedCubesPreviewShown ? "ON, янтарные задетые кубики созданы" : "OFF, задетые кубики не подсвечены") + "\r\n" +
                "Selection trace box: " + (traceBoxPreviewShown ? "ON, зелёный 3D marker рассчитан через RealCamera + aspect correction и растянут на полную глубину задетых кубиков" : "OFF, зелёный marker не удалось создать") + "\r\n" +
                "Trace note: выбор тел и зелёный marker теперь строятся от одной RealCamera aspect-corrected projection\r\n" +
                "Плоскость UI: " + planeText + " (для real-camera режима оставлена как подпись/лог, расчёт идёт от ActiveView.Camera)\r\n" +
                "Режим выбора тел: " + bodyModeText + "\r\n" +
                "Экранная рамка: " + FormatScreenRectangle(overlay.SelectedRectangle) + "\r\n" +
                "Inventor view rect: " + FormatScreenRectangle(viewRect) + "\r\n" +
                "Projection mode: " + (screenQuery == null ? "" : screenQuery.ProjectionMode) + "\r\n" +
                "Projection details: " + (screenQuery == null ? "" : screenQuery.ProjectionDetails) + "\r\n\r\n" +
                "Задето кубиков по 2D-рамке: " + screenQuery.TouchedCells.Count.ToString() + "\r\n" +
                "Кандидатов из выбранных кубиков до body-filter: " + screenQuery.CandidateBodiesFromCellsBeforeBodyFilter.ToString() + "\r\n" +
                "Отклонено body-filter: " + screenQuery.RejectedBodiesByBodyFilter.ToString() + "\r\n" +
                "Кандидатов после final body mode: " + screenQuery.CandidateBodies.Count.ToString() + "\r\n" +
                "Тел показано в visible-list: " + addedCount.ToString() + "\r\n" +
                "Запись myBodyGroup Attributes: SKIPPED\r\n" +
                "Hide/Show из текущего списка: READY\r\n" +
                "UI Inventor SelectSet пропущен: " + selectedInInventorCount.ToString() + "\r\n\r\n" +
                BuildSpatialQueryInfoText(screenQuery));
            }}

        private bool TryGetSelectedSpatialCubeCell(out SpatialCubeCell cell)
        {
            using (AppLogger.Scope("TryGetSelectedSpatialCubeCell"))
            {
            cell = null;

            if (treeViewIptSpatialCubes == null || treeViewIptSpatialCubes.SelectedNode == null)
            {
                return false;
            }

            cell = treeViewIptSpatialCubes.SelectedNode.Tag as SpatialCubeCell;
            return cell != null;
        
            }}

        private void ShowSpatialCubePreview(PartDocument partDoc, SpatialCubeCell cell)
        {
            using (AppLogger.Scope("ShowSpatialCubePreview"))
            {
            if (partDoc == null || cell == null || cell.Bounds == null)
            {
                return;
            }

            ClearSpatialCubePreview(partDoc);

            try
            {
                dynamic compDef = partDoc.ComponentDefinition;
                dynamic clientGraphicsCollection = compDef.ClientGraphicsCollection;
                dynamic clientGraphics = clientGraphicsCollection.Add(SpatialCubePreviewGraphicsName);
                dynamic graphicsNode = clientGraphics.AddNode(1);

                dynamic transientGeometry = _invApp.TransientGeometry;
                dynamic inventorBox = transientGeometry.CreateBox();
                dynamic minPoint = transientGeometry.CreatePoint(cell.Bounds.MinX, cell.Bounds.MinY, cell.Bounds.MinZ);
                dynamic maxPoint = transientGeometry.CreatePoint(cell.Bounds.MaxX, cell.Bounds.MaxY, cell.Bounds.MaxZ);

                try
                {
                    inventorBox.PutBoxData(minPoint, maxPoint);
                }
                catch
                {
                    try
                    {
                        inventorBox.MinPoint = minPoint;
                        inventorBox.MaxPoint = maxPoint;
                    }
                    catch
                    {
                    }
                }

                dynamic transientBRep = _invApp.TransientBRep;
                dynamic previewBody = transientBRep.CreateSolidBlock(inventorBox);
                dynamic surfaceGraphics = graphicsNode.AddSurfaceGraphics(previewBody);

                // Полупрозрачность в разных версиях Inventor API может открываться через разные свойства.
                // Поэтому здесь best-effort: если прозрачность не применится, кубик всё равно будет создан как временная графика.
                try
                {
                    dynamic color = _invApp.TransientObjects.CreateColor(0, 170, 255);
                    color.Opacity = 0.18;
                    surfaceGraphics.Color = color;
                }
                catch
                {
                }

                try
                {
                    surfaceGraphics.Translucent = true;
                }
                catch
                {
                }

                try
                {
                    clientGraphics.Selectable = false;
                }
                catch
                {
                }

                try
                {
                    partDoc.Update();
                }
                catch
                {
                }
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Не удалось создать временный полупрозрачный кубик в Inventor. Индекс кубиков построен, но preview-графика не сработала в этой версии Inventor API.");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        private void ClearSpatialCubePreviewSafe()
        {
            using (AppLogger.Scope("ClearSpatialCubePreviewSafe"))
            {
            try
            {
                if (_invApp == null || _invApp.ActiveDocument == null)
                {
                    return;
                }

                PartDocument partDoc = _invApp.ActiveDocument as PartDocument;
                if (partDoc != null)
                {
                    ClearSpatialCubePreview(partDoc);
                }
            }
            catch
            {
            }
        
            }}

        private void ClearSpatialCubePreview(PartDocument partDoc)
        {
            using (AppLogger.Scope("ClearSpatialCubePreview"))
            {
            if (partDoc == null)
            {
                return;
            }

            try
            {
                dynamic compDef = partDoc.ComponentDefinition;
                dynamic clientGraphicsCollection = compDef.ClientGraphicsCollection;

                ClearClientGraphicsByName(clientGraphicsCollection, SpatialCubePreviewGraphicsName);
                ClearClientGraphicsByName(clientGraphicsCollection, CustomRectTraceGraphicsName);
                ClearClientGraphicsByName(clientGraphicsCollection, TouchedSpatialCubesPreviewGraphicsName);
                ClearClientGraphicsByName(clientGraphicsCollection, BodyRangeBoxesGraphicsName);

                try
                {
                    partDoc.Update();
                }
                catch
                {
                }
            }
            catch
            {
            }
        
            }}

        private void ClearCustomRectTracePreview(PartDocument partDoc)
        {
            using (AppLogger.Scope("ClearCustomRectTracePreview"))
            {
            if (partDoc == null)
            {
                return;
            }

            try
            {
                dynamic compDef = partDoc.ComponentDefinition;
                dynamic clientGraphicsCollection = compDef.ClientGraphicsCollection;
                ClearClientGraphicsByName(clientGraphicsCollection, CustomRectTraceGraphicsName);

                try
                {
                    partDoc.Update();
                }
                catch
                {
                }
            }
            catch
            {
            }
        
            }}

        private void ClearTouchedSpatialCubesPreview(PartDocument partDoc)
        {
            using (AppLogger.Scope("ClearTouchedSpatialCubesPreview"))
            {
            if (partDoc == null)
            {
                return;
            }

            try
            {
                dynamic compDef = partDoc.ComponentDefinition;
                dynamic clientGraphicsCollection = compDef.ClientGraphicsCollection;
                ClearClientGraphicsByName(clientGraphicsCollection, TouchedSpatialCubesPreviewGraphicsName);

                try
                {
                    partDoc.Update();
                }
                catch
                {
                }
            }
            catch
            {
            }
        
            }}

        private static void ClearClientGraphicsByName(dynamic clientGraphicsCollection, string graphicsName)
        {
            if (clientGraphicsCollection == null || string.IsNullOrWhiteSpace(graphicsName))
            {
                return;
            }

            try
            {
                dynamic oldGraphics = clientGraphicsCollection.Item(graphicsName);
                oldGraphics.Delete();
                return;
            }
            catch
            {
            }

            try
            {
                dynamic oldGraphics = clientGraphicsCollection[graphicsName];
                oldGraphics.Delete();
            }
            catch
            {
            }
        }

        private static bool ClientGraphicsExistsByName(dynamic clientGraphicsCollection, string graphicsName)
        {
            if (clientGraphicsCollection == null || string.IsNullOrWhiteSpace(graphicsName))
            {
                return false;
            }

            try
            {
                dynamic graphics = clientGraphicsCollection.Item(graphicsName);
                return graphics != null;
            }
            catch
            {
            }

            try
            {
                dynamic graphics = clientGraphicsCollection[graphicsName];
                return graphics != null;
            }
            catch
            {
                return false;
            }
        }

        private void ButtonIptToggleBodyRangeBoxes_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptToggleBodyRangeBoxes_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            try
            {
                dynamic compDef = partDoc.ComponentDefinition;
                dynamic clientGraphicsCollection = compDef.ClientGraphicsCollection;

                if (ClientGraphicsExistsByName(clientGraphicsCollection, BodyRangeBoxesGraphicsName))
                {
                    ClearBodyRangeBoxesPreview(partDoc);
                    LoggedMessageBox.Show("Body RangeBoxes preview cleared.");
                    return;
                }

                int shown = ShowBodyRangeBoxesPreview(partDoc);
                LoggedMessageBox.Show("Body RangeBoxes preview shown: " + shown.ToString());
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem toggling body RangeBoxes preview");
                LoggedMessageBox.Show(ex.ToString());
            }

            }}

        private void ClearBodyRangeBoxesPreview(PartDocument partDoc)
        {
            using (AppLogger.Scope("ClearBodyRangeBoxesPreview"))
            {
            if (partDoc == null)
            {
                return;
            }

            try
            {
                dynamic compDef = partDoc.ComponentDefinition;
                dynamic clientGraphicsCollection = compDef.ClientGraphicsCollection;
                ClearClientGraphicsByName(clientGraphicsCollection, BodyRangeBoxesGraphicsName);

                try
                {
                    partDoc.Update();
                }
                catch
                {
                }

                AppLogger.Log("BODY_RANGEBOXES_PREVIEW_CLEARED", "ClearBodyRangeBoxesPreview", "ClientGraphics=True");
            }
            catch
            {
            }

            }}

        private int ShowBodyRangeBoxesPreview(PartDocument partDoc)
        {
            using (AppLogger.Scope("ShowBodyRangeBoxesPreview"))
            {
            if (partDoc == null || listBoxIptBodies == null || listBoxIptBodies.Items.Count == 0)
            {
                LoggedMessageBox.Show("Список тел пустой. Сначала выберите тела рамкой или через .ipt cubes.");
                return 0;
            }

            ClearBodyRangeBoxesPreview(partDoc);

            int builtCount = 0;
            int skippedCount = 0;
            int edgeGraphicsCount = 0;
            int edgeGraphicsFailedCount = 0;

            try
            {
                dynamic compDef = partDoc.ComponentDefinition;
                dynamic clientGraphicsCollection = compDef.ClientGraphicsCollection;
                dynamic clientGraphics = clientGraphicsCollection.Add(BodyRangeBoxesGraphicsName);

                dynamic transientBRep = _invApp.TransientBRep;

                HashSet<IntPtr> bodyKeys = new HashSet<IntPtr>();

                foreach (object obj in listBoxIptBodies.Items)
                {
                    BodyListItem item = obj as BodyListItem;
                    if (item == null || item.Body == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    IntPtr key = item.IdentityKey;
                    if (key == IntPtr.Zero)
                    {
                        key = GetComIdentityKey(item.Body);
                    }

                    if (key != IntPtr.Zero && bodyKeys.Contains(key))
                    {
                        skippedCount++;
                        continue;
                    }

                    if (key != IntPtr.Zero)
                    {
                        bodyKeys.Add(key);
                    }

                    Box rangeBox = null;
                    try
                    {
                        rangeBox = item.Body.RangeBox;
                    }
                    catch
                    {
                        rangeBox = null;
                    }

                    if (rangeBox == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    int paletteIndex = builtCount;
                    byte red;
                    byte green;
                    byte blue;
                    GetHighContrastBodyBoxColor(paletteIndex, out red, out green, out blue);

                    dynamic graphicsNode = clientGraphics.AddNode(builtCount + 1);

                    // Light face fill: useful only as a soft volume hint. The important visible layer is the colored edge wireframe below.
                    try
                    {
                        dynamic previewBody = transientBRep.CreateSolidBlock(rangeBox);
                        dynamic surfaceGraphics = graphicsNode.AddSurfaceGraphics(previewBody);
                        dynamic faceColor = _invApp.TransientObjects.CreateColor(red, green, blue);
                        faceColor.Opacity = 0.06;
                        surfaceGraphics.Color = faceColor;

                        try
                        {
                            surfaceGraphics.Translucent = true;
                        }
                        catch
                        {
                        }
                    }
                    catch
                    {
                    }

                    int addedEdges;
                    bool edgeOk = TryAddRangeBoxColoredEdges(graphicsNode, rangeBox, red, green, blue, 3.5, out addedEdges);
                    edgeGraphicsCount += addedEdges;
                    if (!edgeOk)
                    {
                        edgeGraphicsFailedCount++;
                    }

                    builtCount++;
                }

                try
                {
                    clientGraphics.Selectable = false;
                }
                catch
                {
                }

                try
                {
                    partDoc.Update();
                }
                catch
                {
                }

                AppLogger.Log(
                    "BODY_RANGEBOXES_PREVIEW_BUILT",
                    "ShowBodyRangeBoxesPreview",
                    "VisibleBodyListItems=" + listBoxIptBodies.Items.Count.ToString() +
                    "; BuiltRangeBoxes=" + builtCount.ToString() +
                    "; Skipped=" + skippedCount.ToString() +
                    "; Source=SurfaceBody.RangeBox" +
                    "; VisualMode=HighContrastColoredEdgesAndLightFaces" +
                    "; ColorMode=HighContrastPaletteByBodyIndex" +
                    "; FaceOpacity=0.06" +
                    "; EdgeOpacity=1.0" +
                    "; EdgeLineWeight=3.5" +
                    "; EdgeGraphics=" + edgeGraphicsCount.ToString() +
                    "; EdgeGraphicsFailedBodies=" + edgeGraphicsFailedCount.ToString() +
                    "; IsInventorBody=False; ClientGraphics=True; Selectable=False");

                return builtCount;
            }
            catch (Exception ex)
            {
                AppLogger.LogException("ShowBodyRangeBoxesPreview", ex);
                LoggedMessageBox.Show("Не удалось создать временную графику SurfaceBody.RangeBox.");
                LoggedMessageBox.Show(ex.ToString());
                return builtCount;
            }

            }}

        private static void GetHighContrastBodyBoxColor(int index, out byte red, out byte green, out byte blue)
        {
            // High-contrast palette: deliberately saturated colors, much easier to distinguish than low-opacity random fills.
            byte[,] palette = new byte[,]
            {
                { 255, 0, 0 },
                { 0, 180, 255 },
                { 0, 220, 70 },
                { 255, 0, 220 },
                { 255, 210, 0 },
                { 140, 80, 255 },
                { 0, 255, 220 },
                { 255, 120, 0 },
                { 80, 255, 0 },
                { 255, 80, 150 },
                { 70, 140, 255 },
                { 255, 255, 255 }
            };

            int count = palette.GetLength(0);
            int safeIndex = index % count;
            if (safeIndex < 0)
            {
                safeIndex += count;
            }

            red = palette[safeIndex, 0];
            green = palette[safeIndex, 1];
            blue = palette[safeIndex, 2];
        }

        private bool TryAddRangeBoxColoredEdges(dynamic graphicsNode, Box rangeBox, byte red, byte green, byte blue, double lineWeight, out int addedEdges)
        {
            using (AppLogger.Scope("TryAddRangeBoxColoredEdges"))
            {
            addedEdges = 0;

            if (graphicsNode == null || rangeBox == null)
            {
                return false;
            }

            try
            {
                Inventor.Point minPoint = rangeBox.MinPoint;
                Inventor.Point maxPoint = rangeBox.MaxPoint;

                dynamic transientGeometry = _invApp.TransientGeometry;

                dynamic p000 = transientGeometry.CreatePoint(minPoint.X, minPoint.Y, minPoint.Z);
                dynamic p100 = transientGeometry.CreatePoint(maxPoint.X, minPoint.Y, minPoint.Z);
                dynamic p010 = transientGeometry.CreatePoint(minPoint.X, maxPoint.Y, minPoint.Z);
                dynamic p110 = transientGeometry.CreatePoint(maxPoint.X, maxPoint.Y, minPoint.Z);
                dynamic p001 = transientGeometry.CreatePoint(minPoint.X, minPoint.Y, maxPoint.Z);
                dynamic p101 = transientGeometry.CreatePoint(maxPoint.X, minPoint.Y, maxPoint.Z);
                dynamic p011 = transientGeometry.CreatePoint(minPoint.X, maxPoint.Y, maxPoint.Z);
                dynamic p111 = transientGeometry.CreatePoint(maxPoint.X, maxPoint.Y, maxPoint.Z);

                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p000, p100, red, green, blue, lineWeight);
                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p010, p110, red, green, blue, lineWeight);
                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p001, p101, red, green, blue, lineWeight);
                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p011, p111, red, green, blue, lineWeight);

                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p000, p010, red, green, blue, lineWeight);
                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p100, p110, red, green, blue, lineWeight);
                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p001, p011, red, green, blue, lineWeight);
                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p101, p111, red, green, blue, lineWeight);

                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p000, p001, red, green, blue, lineWeight);
                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p100, p101, red, green, blue, lineWeight);
                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p010, p011, red, green, blue, lineWeight);
                addedEdges += AddOneBodyRangeBoxEdge(graphicsNode, transientGeometry, p110, p111, red, green, blue, lineWeight);

                return addedEdges > 0;
            }
            catch (Exception ex)
            {
                AppLogger.LogException("TryAddRangeBoxColoredEdges", ex);
                return false;
            }

            }}

        private int AddOneBodyRangeBoxEdge(dynamic graphicsNode, dynamic transientGeometry, dynamic p1, dynamic p2, byte red, byte green, byte blue, double lineWeight)
        {
            try
            {
                dynamic line = transientGeometry.CreateLineSegment(p1, p2);
                dynamic curveGraphics = graphicsNode.AddCurveGraphics(line);
                dynamic edgeColor = _invApp.TransientObjects.CreateColor(red, green, blue);
                edgeColor.Opacity = 1.0;
                curveGraphics.Color = edgeColor;

                try
                {
                    curveGraphics.LineWeight = lineWeight;
                }
                catch
                {
                }

                return 1;
            }
            catch
            {
                return 0;
            }
        }

        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / GetBodiesForIptSelection — входная точка выбора кандидатов для OnSelect.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Контрольный участок получения кандидатов через spatial cubes index — 0.035 с.
        private List<SurfaceBody> GetBodiesForIptSelection(PartDocument partDoc, BoxLimits selectionLimits, out SpatialQueryResult spatialQuery)
        {
            using (AppLogger.Scope("GetBodiesForIptSelection"))
            {
            spatialQuery = null;
            List<SurfaceBody> result = new List<SurfaceBody>();

            if (partDoc == null)
            {
                return result;
            }

            // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / проверка индекса: SpatialCubesIndex.MatchesDocument — 0.000649 с.
            if (_spatialCubesIndex != null && _spatialCubesIndex.IsReady && _spatialCubesIndex.MatchesDocument(partDoc))
            {
                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / BoxLimits.ToSpatialBox — 0.000244 с.
                SpatialBox selectionBox = selectionLimits == null ? null : selectionLimits.ToSpatialBox();
                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / SpatialCubesIndex.Query — 0.001361 с.
                spatialQuery = _spatialCubesIndex.Query(selectionBox);

                if (spatialQuery != null)
                {
                    foreach (SpatialBodyRecord record in spatialQuery.CandidateBodies)
                    {
                        if (record != null && record.Body != null)
                        {
                            result.Add(record.Body);
                        }
                    }
                }

                AppLogger.Log(
                    "SPATIAL_CUBES_SELECTION_CANDIDATES",
                    "GetBodiesForIptSelection",
                    "TouchedCells=" + (spatialQuery == null || spatialQuery.TouchedCells == null ? "0" : spatialQuery.TouchedCells.Count.ToString()) +
                    "; CandidateBodies=" + result.Count.ToString());

                return result;
            }

            // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS OLD MODE / если spatial cubes index OFF, используется старый полный foreach SurfaceBodies.
            foreach (SurfaceBody body in partDoc.ComponentDefinition.SurfaceBodies)
            {
                if (body != null)
                {
                    result.Add(body);
                }
            }

            AppLogger.Log("SPATIAL_CUBES_SELECTION_OLD_MODE", "GetBodiesForIptSelection", "CandidateBodies=" + result.Count.ToString());
            return result;
        
            }}

        private string BuildSpatialQueryInfoText(SpatialQueryResult spatialQuery)
        {
            using (AppLogger.Scope("BuildSpatialQueryInfoText"))
            {
            if (spatialQuery == null)
            {
                return "Spatial cubes index: OFF / использован старый полный перебор SurfaceBodies.";
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("Spatial cubes index: ON\r\n");
            sb.Append("Hit mode: ");
            sb.Append(string.IsNullOrEmpty(spatialQuery.HitMode) ? "ProjectedCenterInsideSelectionRect" : spatialQuery.HitMode);
            sb.Append("\r\nRequested body mode: ");
            sb.Append(string.IsNullOrEmpty(spatialQuery.RequestedBodySelectionMode) ? "Cube hit: all bodies" : spatialQuery.RequestedBodySelectionMode);
            sb.Append("\r\nResolved body mode: ");
            sb.Append(string.IsNullOrEmpty(spatialQuery.ResolvedBodySelectionMode) ? "Cube hit: all bodies" : spatialQuery.ResolvedBodySelectionMode);
            sb.Append("\r\nBody filter: ");
            sb.Append(string.IsNullOrEmpty(spatialQuery.BodyFilterMode) ? "NormalAuto" : spatialQuery.BodyFilterMode);
            sb.Append("\r\nProjection mode: ");
            sb.Append(string.IsNullOrEmpty(spatialQuery.ProjectionMode) ? "LegacyModelBoxLinearFallback" : spatialQuery.ProjectionMode);
            if (!string.IsNullOrEmpty(spatialQuery.ProjectionDetails))
            {
                sb.Append("\r\nProjection details: ");
                sb.Append(spatialQuery.ProjectionDetails);
            }
            sb.Append("\r\nProjected cube rects: ");
            sb.Append(spatialQuery.ProjectedCubeRectCount.ToString());
            sb.Append("; projected body rects: ");
            sb.Append(spatialQuery.ProjectedBodyRectCount.ToString());
            sb.Append("; fallback rects: ");
            sb.Append(spatialQuery.ProjectionFallbackCount.ToString());
            sb.Append("\r\nAuto whole-cell coverage: ");
            sb.Append(spatialQuery.AutoWholeCellCoveragePercent.ToString("0.0", CultureInfo.InvariantCulture));
            sb.Append("%\r\n");
            // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX ВАЖНО: это количество кубиков, которые задела именно текущая selectionLimits рамки.
            // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Это не равно количеству кубиков, в которых может числиться конкретное тело.
            sb.Append("Задето кубиков: ");
            sb.Append(spatialQuery.TouchedCells == null ? "0" : spatialQuery.TouchedCells.Count.ToString());
            sb.Append("\r\nКандидатов из выбранных кубиков: ");
            sb.Append(spatialQuery.CandidateBodiesFromCellsBeforeBodyFilter.ToString());
            sb.Append("\r\nОтклонено body-filter: ");
            sb.Append(spatialQuery.RejectedBodiesByBodyFilter.ToString());
            sb.Append("\r\nКандидатов после final body mode: ");
            sb.Append(spatialQuery.CandidateBodies == null ? "0" : spatialQuery.CandidateBodies.Count.ToString());

            if (spatialQuery.TouchedCells != null && spatialQuery.TouchedCells.Count > 0)
            {
                sb.Append("\r\nКубики: ");
                for (int i = 0; i < spatialQuery.TouchedCells.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }

                    sb.Append(spatialQuery.TouchedCells[i].Id);
                }
            }

            return sb.ToString();
        
            }}

        private enum CustomCubeRectPlane
        {
            FrontXY,
            FrontXZ,
            SideYZ
        }

        private enum CustomRectBodySelectionMode
        {
            CubeHitAllBodies,
            CubeHitFilteredBodies,
            RangeBoxFullyInsideTraceBox

            // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES
            // FROZEN HISTORICAL MODES: keep the old names as comments only, not as live options.
            // AutoNormal,
            // WholeSelectedCubes,
            // PreciseBodyBox
        }

        private CustomCubeRectPlane GetSelectedCustomCubeRectPlane()
        {
            using (AppLogger.Scope("GetSelectedCustomCubeRectPlane"))
            {
            int index = 0;

            try
            {
                if (comboBoxIptCustomRectPlane != null && comboBoxIptCustomRectPlane.SelectedIndex >= 0)
                {
                    index = comboBoxIptCustomRectPlane.SelectedIndex;
                }
            }
            catch
            {
                index = 0;
            }

            if (index == 1)
            {
                return CustomCubeRectPlane.FrontXZ;
            }

            if (index == 2)
            {
                return CustomCubeRectPlane.SideYZ;
            }

            return CustomCubeRectPlane.FrontXY;
            }}

        private static string GetCustomCubeRectPlaneText(CustomCubeRectPlane plane)
        {
            if (plane == CustomCubeRectPlane.FrontXZ)
            {
                return "Front X/Z";
            }

            if (plane == CustomCubeRectPlane.SideYZ)
            {
                return "Side Y/Z";
            }

            return "Front X/Y";
        }

        private CustomRectBodySelectionMode GetSelectedCustomRectBodySelectionMode()
        {
            using (AppLogger.Scope("GetSelectedCustomRectBodySelectionMode"))
            {
            int index = 0;

            try
            {
                if (comboBoxIptCustomRectBodyMode != null && comboBoxIptCustomRectBodyMode.SelectedIndex >= 0)
                {
                    index = comboBoxIptCustomRectBodyMode.SelectedIndex;
                }
            }
            catch
            {
                index = 0;
            }

            if (index == 1)
            {
                return CustomRectBodySelectionMode.CubeHitFilteredBodies;
            }

            if (index == 2)
            {
                return CustomRectBodySelectionMode.RangeBoxFullyInsideTraceBox;
            }

            // FROZEN historical indices:
            // old index == 1 used to be Whole selected cubes.
            // old index == 2 used to be Precise body-box.
            // They are not live dropdown choices anymore.
            return CustomRectBodySelectionMode.CubeHitAllBodies;
            }}

        private static string GetCustomRectBodySelectionModeText(CustomRectBodySelectionMode mode)
        {
            if (mode == CustomRectBodySelectionMode.CubeHitFilteredBodies)
            {
                return "2. Cube hit: filtered bodies";
            }

            if (mode == CustomRectBodySelectionMode.RangeBoxFullyInsideTraceBox)
            {
                return "3. RangeBox fully inside TraceBox";
            }

            return "1. Cube hit: all bodies";
        }

        private SpatialQueryResult BuildSpatialQueryFromScreenRectangle(System.Drawing.Rectangle selectionRect, System.Drawing.Rectangle viewRect, CustomCubeRectPlane plane, CustomRectBodySelectionMode bodyMode)
        {
            using (AppLogger.Scope("BuildSpatialQueryFromScreenRectangle"))
            {
            SpatialQueryResult result = new SpatialQueryResult();
            result.TouchedCells = new List<SpatialCubeCell>();
            result.CandidateBodies = new List<SpatialBodyRecord>();
            result.CandidateBodiesFromCellsBeforeBodyFilter = 0;
            result.RejectedBodiesByBodyFilter = 0;
            result.HitMode = "CubeCenterOrSelectionCenterInsideCellRect";
            result.BodyFilterMode = "Pending";
            result.RequestedBodySelectionMode = GetCustomRectBodySelectionModeText(bodyMode);
            result.ResolvedBodySelectionMode = result.RequestedBodySelectionMode;
            result.AutoWholeCellCoveragePercent = 0.0;
            result.ProjectionMode = "Pending";
            result.ProjectionDetails = string.Empty;
            result.ProjectionFallbackCount = 0;
            result.ProjectedCubeRectCount = 0;
            result.ProjectedBodyRectCount = 0;

            if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady || _spatialCubesIndex.ModelBox == null)
            {
                return result;
            }

            System.Drawing.Rectangle clipped = System.Drawing.Rectangle.Intersect(selectionRect, viewRect);
            if (clipped.Width <= 0 || clipped.Height <= 0)
            {
                return result;
            }

            int bodyEntryCountBeforeDistinct = 0;
            int rejectedCellsByHitMode = 0;
            int fallbackCellsBySelectionCenter = 0;
            int rejectedBodiesByBodyFilter = 0;
            double maxCellCoverageRatio = 0.0;
            System.Drawing.Point selectionCenter = GetRectangleCenter(clipped);
            HashSet<int> bodyIndexes = new HashSet<int>();
            List<SpatialBodyRecord> bodyCandidatesFromCells = new List<SpatialBodyRecord>();

            InventorScreenProjector screenProjector = null;
            string projectorDetails;
            bool useRealCameraProjector = TryCreateInventorScreenProjector(viewRect, out screenProjector, out projectorDetails);
            result.ProjectionMode = useRealCameraProjector ? "RealCameraActiveViewOrthographicAspectCorrected" : "LegacyModelBoxLinearFallback";
            result.ProjectionDetails = projectorDetails;
            result.HitMode = useRealCameraProjector ? "RealCameraAspectCorrectedProjectedBoxCenterOrSelectionCenterInsideBoxRect" : "LegacyModelBoxLinear_CubeCenterOrSelectionCenterInsideCellRect";

            foreach (SpatialCubeCell cell in _spatialCubesIndex.Cells)
            {
                if (cell == null || cell.Bounds == null)
                {
                    continue;
                }

                System.Drawing.Rectangle cellScreenRect;
                if (useRealCameraProjector && screenProjector != null && screenProjector.TryProjectBoxToScreenRectangle(cell.Bounds, out cellScreenRect))
                {
                    result.ProjectedCubeRectCount++;
                }
                else
                {
                    cellScreenRect = ProjectSpatialBoxToScreenRectangle(cell.Bounds, _spatialCubesIndex.ModelBox, viewRect, plane);
                    result.ProjectionFallbackCount++;
                }

                System.Drawing.Point cellCenter = GetRectangleCenter(cellScreenRect);
                double cellCoverageRatio = RectangleIntersectionAreaRatio(clipped, cellScreenRect);

                // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT HitMode=ProjectedCenterInsideSelectionRect.
                // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT Кубик выбирается не по любому касанию 2D-рамки, а только если центр 2D-проекции кубика попал внутрь рамки.
                // 16:10 15.06.2026 InventorIptOrg_v0_4_13_CUSTOM_RECT_CENTER_HIT Это уменьшает "прилипание" соседних кубиков, когда пользователь слегка задел границу сектора.
                // 19:30 15.06.2026 InventorIptOrg_v0_4_19_CUSTOM_RECT_SMALL_AREA_HIT =====================================================================================================
                // 19:30 15.06.2026 InventorIptOrg_v0_4_19_CUSTOM_RECT_SMALL_AREA_HIT Маленькая рамка внутри одного кубика не содержит центр кубика, поэтому v0.4.18 возвращала 0 кубиков.
                // 19:30 15.06.2026 InventorIptOrg_v0_4_19_CUSTOM_RECT_SMALL_AREA_HIT Новый гибридный HitMode:
                // 19:30 15.06.2026 InventorIptOrg_v0_4_19_CUSTOM_RECT_SMALL_AREA_HIT 1) основной режим: центр 2D-проекции кубика внутри рамки;
                // 19:30 15.06.2026 InventorIptOrg_v0_4_19_CUSTOM_RECT_SMALL_AREA_HIT 2) small-area fallback: центр рамки внутри 2D-прямоугольника кубика.
                // 19:30 15.06.2026 InventorIptOrg_v0_4_19_CUSTOM_RECT_SMALL_AREA_HIT Это позволяет выбирать маленький оранжевый участок внутри большого кубика и всё равно получить этот кубик.
                // 19:30 15.06.2026 InventorIptOrg_v0_4_19_CUSTOM_RECT_SMALL_AREA_HIT =====================================================================================================
                // 20:05 15.06.2026 InventorIptOrg_v0_4_20_CUSTOM_RECT_CACHED_BODY_FILTER =====================================================================================================
                // 20:05 15.06.2026 InventorIptOrg_v0_4_20_CUSTOM_RECT_CACHED_BODY_FILTER v0.4.19 выбирала маленькой рамкой кубик, но затем брала ВСЕ тела выбранного кубика.
                // 20:05 15.06.2026 InventorIptOrg_v0_4_20_CUSTOM_RECT_CACHED_BODY_FILTER Новый режим оставляет быстрый hit-test кубиков, но включает дешёвый cached body-filter внутри выбранных кубиков.
                // 20:05 15.06.2026 InventorIptOrg_v0_4_20_CUSTOM_RECT_CACHED_BODY_FILTER Фильтр НЕ вызывает Inventor COM: используется уже сохранённый SpatialBodyRecord.BodyBox из БАЗЫ кубиков.
                // 20:05 15.06.2026 InventorIptOrg_v0_4_20_CUSTOM_RECT_CACHED_BODY_FILTER Тело проходит, если центр 2D-проекции cached BodyBox попал внутрь экранной рамки.
                // 20:05 15.06.2026 InventorIptOrg_v0_4_20_CUSTOM_RECT_CACHED_BODY_FILTER =====================================================================================================
                // 20:35 15.06.2026 InventorIptOrg_v0_4_21_CUSTOM_RECT_CACHED_BODY_BOX_HIT =====================================================================================================
                // 20:35 15.06.2026 InventorIptOrg_v0_4_21_CUSTOM_RECT_CACHED_BODY_BOX_HIT Дополнение к v0.4.20: центр body-box оказался слишком строгим для маленькой рамки.
                // 20:35 15.06.2026 InventorIptOrg_v0_4_21_CUSTOM_RECT_CACHED_BODY_BOX_HIT Теперь тело проходит, если его cached 2D BodyBox rectangle пересекается с экранной рамкой.
                // 20:35 15.06.2026 InventorIptOrg_v0_4_21_CUSTOM_RECT_CACHED_BODY_BOX_HIT Дополнительно Build spatial cubes BASE ускорен: пропущены GetBodyDisplayName и RefreshIptBrowserTree.
                // 20:35 15.06.2026 InventorIptOrg_v0_4_21_CUSTOM_RECT_CACHED_BODY_BOX_HIT =====================================================================================================
                bool hitByCellCenter = RectangleContainsPointInclusive(clipped, cellCenter);
                bool hitBySelectionCenter = RectangleContainsPointInclusive(cellScreenRect, selectionCenter);

                if (!hitByCellCenter && !hitBySelectionCenter)
                {
                    rejectedCellsByHitMode++;
                    continue;
                }

                if (!hitByCellCenter && hitBySelectionCenter)
                {
                    fallbackCellsBySelectionCenter++;
                }

                if (cellCoverageRatio > maxCellCoverageRatio)
                {
                    maxCellCoverageRatio = cellCoverageRatio;
                }

                result.TouchedCells.Add(cell);

                foreach (SpatialBodyRecord record in cell.Bodies)
                {
                    if (record == null)
                    {
                        continue;
                    }

                    bodyEntryCountBeforeDistinct++;
                    if (bodyIndexes.Add(record.BodyIndex))
                    {
                        bodyCandidatesFromCells.Add(record);
                    }
                }
            }

            result.CandidateBodiesFromCellsBeforeBodyFilter = bodyCandidatesFromCells.Count;
            result.AutoWholeCellCoveragePercent = maxCellCoverageRatio * 100.0;

            bool useCubeHitAllBodies = bodyMode == CustomRectBodySelectionMode.CubeHitAllBodies;
            bool useRangeBoxFullyInsideTraceBox = bodyMode == CustomRectBodySelectionMode.RangeBoxFullyInsideTraceBox;

            // 21:05 15.06.2026 InventorIptOrg_v0_4_22_CUSTOM_RECT_NORMAL_SELECT
            // FROZEN historical AUTO logic: AutoWholeCubeCoverageThreshold = 0.25 used to switch AUTO normal between whole cubes and Precise body-box.
            // 22:55 15.06.2026 InventorIptOrg_v0_4_23_CUSTOM_RECT_NORMAL_SELECT_FIX froze AUTO normal into safe whole-cubes behavior.
            // 13:45 16.06.2026 InventorIptOrg_v0_4_24_CUBE_HIT_FILTERED_BODIES exposes two explicit live modes instead of hidden AUTO switching.
            // 22:05 16.06.2026 InventorIptOrg_v0_4_38_RANGEBOX_INSIDE_TRACEBOX adds strict 3D containment:
            // SurfaceBody.RangeBox must be fully inside the green Selection Trace Box.

            if (useCubeHitAllBodies)
            {
                result.BodyFilterMode = "CubeHitAllBodies_NoBodyFilter";
                result.ResolvedBodySelectionMode = "1. Cube hit: all bodies";
                result.CandidateBodies.AddRange(bodyCandidatesFromCells);
                rejectedBodiesByBodyFilter = 0;
            }
            else if (useRangeBoxFullyInsideTraceBox)
            {
                SpatialBox selectionTraceBox = BuildCustomRectTraceBoxFromScreenRectangle(selectionRect, viewRect, plane, result);
                double containmentTolerance = _spatialCubesIndex == null ? 0.001 : _spatialCubesIndex.Tolerance;

                result.BodyFilterMode = "RangeBoxFullyInsideTraceBox_CachedBodyBox3DContainment";
                result.ResolvedBodySelectionMode = "3. RangeBox fully inside TraceBox";

                foreach (SpatialBodyRecord record in bodyCandidatesFromCells)
                {
                    if (record == null || record.BodyBox == null || selectionTraceBox == null)
                    {
                        rejectedBodiesByBodyFilter++;
                        continue;
                    }

                    if (SpatialBoxFullyInsideInclusive(record.BodyBox, selectionTraceBox, containmentTolerance))
                    {
                        result.CandidateBodies.Add(record);
                    }
                    else
                    {
                        rejectedBodiesByBodyFilter++;
                    }
                }

                AppLogger.Log(
                    "RANGEBOX_FULLY_INSIDE_TRACEBOX_FILTER_USED",
                    "BuildSpatialQueryFromScreenRectangle",
                    "SelectionTraceBox=" + FormatSpatialBox(selectionTraceBox) +
                    "; CandidateBodiesFromCellsBeforeBodyFilter=" + bodyCandidatesFromCells.Count.ToString() +
                    "; CandidateBodiesAfterFinalBodyMode=" + result.CandidateBodies.Count.ToString() +
                    "; RejectedBodiesByBodyFilter=" + rejectedBodiesByBodyFilter.ToString() +
                    "; ContainmentTolerance=" + containmentTolerance.ToString("0.######", CultureInfo.InvariantCulture) +
                    "; FullContainmentAxes=XYZ");
            }
            else
            {
                result.BodyFilterMode = useRealCameraProjector ? "CubeHitFilteredBodies_CachedRealCameraBodyBoxIntersectsSelectionRect" : "CubeHitFilteredBodies_CachedLegacyProjectedBodyBoxIntersectsSelectionRect";
                result.ResolvedBodySelectionMode = "2. Cube hit: filtered bodies";

                foreach (SpatialBodyRecord record in bodyCandidatesFromCells)
                {
                    if (record == null || record.BodyBox == null)
                    {
                        rejectedBodiesByBodyFilter++;
                        continue;
                    }

                    System.Drawing.Rectangle bodyScreenRect;
                    if (useRealCameraProjector && screenProjector != null && screenProjector.TryProjectBoxToScreenRectangle(record.BodyBox, out bodyScreenRect))
                    {
                        result.ProjectedBodyRectCount++;
                    }
                    else
                    {
                        bodyScreenRect = ProjectSpatialBoxToScreenRectangle(record.BodyBox, _spatialCubesIndex.ModelBox, viewRect, plane);
                        result.ProjectionFallbackCount++;
                    }

                    // 20:35 15.06.2026 InventorIptOrg_v0_4_21_CUSTOM_RECT_CACHED_BODY_BOX_HIT
                    // v0.4.20 проверял только cached center тела. Маленькая рамка могла попасть в часть тела,
                    // но центр тела оставался вне рамки, поэтому отбрасывались все 51 кандидатов.
                    // v0.4.21 проверяет пересечение 2D-прямоугольника cached BodyBox с экранной рамкой.
                    // COM-вызовов здесь нет: BodyBox уже лежит в SpatialBodyRecord из FAST spatial cubes BASE.
                    if (RectanglesIntersectInclusive(clipped, bodyScreenRect))
                    {
                        result.CandidateBodies.Add(record);
                    }
                    else
                    {
                        rejectedBodiesByBodyFilter++;
                    }
                }
            }

            result.RejectedBodiesByBodyFilter = rejectedBodiesByBodyFilter;

            AppLogger.Log(
                "CUSTOM_RECT_REAL_CAMERA_SELECTION_QUERY",
                "BuildSpatialQueryFromScreenRectangle",
                "HitMode=" + result.HitMode +
                "; RequestedBodyMode=" + result.RequestedBodySelectionMode +
                "; ResolvedBodyMode=" + result.ResolvedBodySelectionMode +
                "; BodyFilter=" + result.BodyFilterMode +
                "; AutoWholeCellCoveragePercent=" + result.AutoWholeCellCoveragePercent.ToString("0.0", CultureInfo.InvariantCulture) +
                "; Plane=" + GetCustomCubeRectPlaneText(plane) +
                "; SelectionRect=" + FormatScreenRectangle(selectionRect) +
                "; ClippedSelectionRect=" + FormatScreenRectangle(clipped) +
                "; ViewRect=" + FormatScreenRectangle(viewRect) +
                "; ProjectionMode=" + result.ProjectionMode +
                "; ProjectionDetails=" + result.ProjectionDetails +
                "; ProjectedCubeRects=" + result.ProjectedCubeRectCount.ToString() +
                "; ProjectedBodyRects=" + result.ProjectedBodyRectCount.ToString() +
                "; ProjectionFallbackCount=" + result.ProjectionFallbackCount.ToString() +
                "; TouchedCells=" + result.TouchedCells.Count.ToString() +
                "; RejectedCellsByHitMode=" + rejectedCellsByHitMode.ToString() +
                "; FallbackCellsBySelectionCenter=" + fallbackCellsBySelectionCenter.ToString() +
                "; BodyEntriesBeforeDistinct=" + bodyEntryCountBeforeDistinct.ToString() +
                "; CandidateBodiesFromCellsBeforeBodyFilter=" + result.CandidateBodiesFromCellsBeforeBodyFilter.ToString() +
                "; RejectedBodiesByBodyFilter=" + result.RejectedBodiesByBodyFilter.ToString() +
                "; CandidateBodiesAfterFinalBodyMode=" + result.CandidateBodies.Count.ToString() +
                "; CellIds=" + BuildCellIdsText(result.TouchedCells));

            return result;
            }}

        private int AddSpatialQueryBodiesToListOnly(PartDocument partDoc, SpatialQueryResult screenQuery, out int selectedInInventorCount)
        {
            using (AppLogger.Scope("AddSpatialQueryBodiesToListOnly"))
            {
            selectedInInventorCount = 0;

            if (screenQuery == null || screenQuery.CandidateBodies == null || listBoxIptBodies == null)
            {
                return 0;
            }

            int shownCount = 0;
            int skippedNull = 0;

            // 18:55 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY
            // FAST HIDE READY: do not write myBodyGroup AttributeSet into Inventor bodies.
            // This intentionally skips AddBodyToGroup, AttributeSetNameIsUsed and all document mutation.
            // The result of the spatial-cubes BASE query is shown as a virtual/current list in the form.
            listBoxIptBodies.BeginUpdate();
            try
            {
                listBoxIptBodies.Items.Clear();

                foreach (SpatialBodyRecord record in screenQuery.CandidateBodies)
                {
                    if (record == null || record.Body == null)
                    {
                        skippedNull++;
                        continue;
                    }

                    string displayName = record.DisplayName;
                    if (string.IsNullOrWhiteSpace(displayName))
                    {
                        displayName = "SurfaceBody " + record.BodyIndex.ToString();
                    }

                    listBoxIptBodies.Items.Add(new BodyListItem(record.Body, displayName, record.IdentityKey));
                    shownCount++;
                }
            }
            finally
            {
                listBoxIptBodies.EndUpdate();
            }

            if (labelIptGroupList != null)
            {
                labelIptGroupList.Text = "Bodies in current group / Custom rectangle visible-list, Hide-ready: " + shownCount.ToString();
            }

            int autoFeatureItems = 0;
            if (shownCount > 0)
            {
                // 19:35 16.06.2026 InventorIptOrg_v0_4_32_FEATURE_BROWSER_NODES_FAST
                // Auto-reanimate the Features area immediately after .ipt cubes selection.
                // This makes the red Features block visibly non-empty before the user presses Create feature browser folder.
                autoFeatureItems = BuildFeatureListFromCurrentVisibleBodiesFast(partDoc, false);

                AppLogger.Log(
                    "FEATURE_LIST_AUTO_POPULATE_AFTER_CUBE_SELECTION",
                    "AddSpatialQueryBodiesToListOnly",
                    "ShownBodies=" + shownCount.ToString() +
                    "; AutoFeatureItems=" + autoFeatureItems.ToString());
            }

            AppLogger.Log(
                "CUSTOM_RECT_REAL_CAMERA_SELECTION_APPLIED",
                "AddSpatialQueryBodiesToListOnly",
                "CandidateBodiesFromCellsBeforeBodyFilter=" + screenQuery.CandidateBodiesFromCellsBeforeBodyFilter.ToString() +
                "; RejectedBodiesByBodyFilter=" + screenQuery.RejectedBodiesByBodyFilter.ToString() +
                "; RequestedBodyMode=" + screenQuery.RequestedBodySelectionMode +
                "; ResolvedBodyMode=" + screenQuery.ResolvedBodySelectionMode +
                "; BodyFilterMode=" + screenQuery.BodyFilterMode +
                "; AutoWholeCellCoveragePercent=" + screenQuery.AutoWholeCellCoveragePercent.ToString("0.0", CultureInfo.InvariantCulture) +
                "; ProjectionMode=" + screenQuery.ProjectionMode +
                "; ProjectedCubeRects=" + screenQuery.ProjectedCubeRectCount.ToString() +
                "; ProjectedBodyRects=" + screenQuery.ProjectedBodyRectCount.ToString() +
                "; ProjectionFallbackCount=" + screenQuery.ProjectionFallbackCount.ToString() +
                "; CandidateBodiesAfterFinalBodyMode=" + screenQuery.CandidateBodies.Count.ToString() +
                "; ShownInVisibleList=" + shownCount.ToString() +
                "; AutoFeatureItems=" + autoFeatureItems.ToString() +
                "; SkippedNullRecords=" + skippedNull.ToString() +
                "; CachedBodyBoxHit=" + (screenQuery.BodyFilterMode != null && screenQuery.BodyFilterMode.IndexOf("BodyBox", StringComparison.OrdinalIgnoreCase) >= 0).ToString() +
                "; AddBodyToGroupSkipped=True; AttributesSkipped=True; FeaturesSkipped=True; SelectSetSkipped=True; RefreshListsSkipped=True; HideReadyVisibleList=True");

            return shownCount;
            }}

        private bool AppendSpatialBodyRecordToVisibleBodyListFast(SpatialBodyRecord record)
        {
            using (AppLogger.Scope("AppendSpatialBodyRecordToVisibleBodyListFast"))
            {
            if (record == null || record.Body == null || listBoxIptBodies == null)
            {
                return false;
            }

            if (this.InvokeRequired)
            {
                bool added = false;
                RunOnUiThread(delegate { added = AppendSpatialBodyRecordToVisibleBodyListFast(record); });
                return added;
            }

            IntPtr key = record.IdentityKey;
            if (key != IntPtr.Zero)
            {
                foreach (object obj in listBoxIptBodies.Items)
                {
                    BodyListItem item = obj as BodyListItem;
                    if (item != null && item.IdentityKey == key)
                    {
                        return false;
                    }
                }
            }

            string displayName = record.DisplayName;
            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = "SurfaceBody " + record.BodyIndex.ToString();
            }

            listBoxIptBodies.Items.Add(new BodyListItem(record.Body, displayName, key));
            return true;
            }}

        private bool TryCreateInventorScreenProjector(System.Drawing.Rectangle viewRect, out InventorScreenProjector projector, out string details)
        {
            using (AppLogger.Scope("TryCreateInventorScreenProjector"))
            {
            projector = null;
            details = "NotStarted";

            if (_invApp == null || viewRect.Width <= 10 || viewRect.Height <= 10)
            {
                details = "NoInventorApplicationOrInvalidViewRect";
                return false;
            }

            try
            {
                dynamic activeView = _invApp.ActiveView;
                dynamic camera = activeView.Camera;

                dynamic eyeObj = camera.Eye;
                dynamic targetObj = camera.Target;
                dynamic upObj = camera.UpVector;

                double eyeX = ToInvariantDouble(eyeObj.X);
                double eyeY = ToInvariantDouble(eyeObj.Y);
                double eyeZ = ToInvariantDouble(eyeObj.Z);
                double targetX = ToInvariantDouble(targetObj.X);
                double targetY = ToInvariantDouble(targetObj.Y);
                double targetZ = ToInvariantDouble(targetObj.Z);
                double upX = ToInvariantDouble(upObj.X);
                double upY = ToInvariantDouble(upObj.Y);
                double upZ = ToInvariantDouble(upObj.Z);

                double extentWidth = 0.0;
                double extentHeight = 0.0;
                bool gotExtents = false;

                try
                {
                    camera.GetExtents(out extentWidth, out extentHeight);
                    gotExtents = true;
                }
                catch
                {
                    gotExtents = false;
                }

                if (!gotExtents || extentWidth <= 0.000000001 || extentHeight <= 0.000000001)
                {
                    details = "CameraGetExtentsFailedOrInvalid";
                    AppLogger.Log(
                        "REAL_CAMERA_SCREEN_PROJECTOR_FALLBACK",
                        "TryCreateInventorScreenProjector",
                        "Reason=" + details + "; ViewRect=" + FormatScreenRectangle(viewRect));
                    return false;
                }

                bool perspective = false;
                string perspectiveText = "Unknown";
                try
                {
                    perspective = Convert.ToBoolean(camera.Perspective);
                    perspectiveText = perspective.ToString();
                }
                catch
                {
                    perspective = false;
                }

                // v0.4.30 intentionally treats the current рабочий вид Спереди/orthographic as the main case.
                // If Inventor exposes Perspective=True here, we still build an orthographic projector and log it clearly;
                // the next version can add perspective division if the user needs perspective view selection.
                projector = new InventorScreenProjector(
                    viewRect,
                    new CameraPoint3D(eyeX, eyeY, eyeZ),
                    new CameraPoint3D(targetX, targetY, targetZ),
                    new CameraVector3D(upX, upY, upZ),
                    extentWidth,
                    extentHeight,
                    perspective);

                if (!projector.IsReady)
                {
                    details = "ProjectorMathNotReady_" + projector.StatusText;
                    AppLogger.Log(
                        "REAL_CAMERA_SCREEN_PROJECTOR_FALLBACK",
                        "TryCreateInventorScreenProjector",
                        "Reason=" + details + "; ViewRect=" + FormatScreenRectangle(viewRect));
                    projector = null;
                    return false;
                }

                details = projector.StatusText;
                AppLogger.Log(
                    "REAL_CAMERA_SCREEN_PROJECTOR_READY",
                    "TryCreateInventorScreenProjector",
                    "ViewRect=" + FormatScreenRectangle(viewRect) +
                    "; Eye=" + FormatCameraPoint(eyeX, eyeY, eyeZ) +
                    "; Target=" + FormatCameraPoint(targetX, targetY, targetZ) +
                    "; Up=" + FormatCameraPoint(upX, upY, upZ) +
                    "; ExtentWidth=" + extentWidth.ToString("0.########", CultureInfo.InvariantCulture) +
                    "; ExtentHeight=" + extentHeight.ToString("0.########", CultureInfo.InvariantCulture) +
                    "; Perspective=" + perspectiveText +
                    "; Projector=" + details);

                return true;
            }
            catch (Exception ex)
            {
                details = "Exception_" + ex.GetType().Name;
                AppLogger.LogException("TryCreateInventorScreenProjector", ex);
                return false;
            }
        
            }}

        private static double ToInvariantDouble(object value)
        {
            if (value == null)
            {
                return 0.0;
            }

            return Convert.ToDouble(value, CultureInfo.InvariantCulture);
        }

        private static string FormatCameraPoint(double x, double y, double z)
        {
            return "(" +
                   x.ToString("0.###", CultureInfo.InvariantCulture) + "," +
                   y.ToString("0.###", CultureInfo.InvariantCulture) + "," +
                   z.ToString("0.###", CultureInfo.InvariantCulture) + ")";
        }

        private sealed class InventorScreenProjector
        {
            private readonly System.Drawing.Rectangle viewRect;
            private readonly CameraPoint3D eye;
            private readonly CameraPoint3D target;
            private readonly CameraVector3D right;
            private readonly CameraVector3D up;
            private readonly CameraVector3D forward;
            private readonly double rawExtentWidth;
            private readonly double rawExtentHeight;
            private readonly double extentWidth;
            private readonly double extentHeight;
            private readonly double viewAspect;
            private readonly double rawAspect;
            private readonly string aspectCorrectionMode;
            private readonly bool perspective;

            public InventorScreenProjector(System.Drawing.Rectangle viewRect, CameraPoint3D eye, CameraPoint3D target, CameraVector3D upVector, double extentWidth, double extentHeight, bool perspective)
            {
                this.viewRect = viewRect;
                this.eye = eye;
                this.target = target;
                this.rawExtentWidth = extentWidth;
                this.rawExtentHeight = extentHeight;
                this.viewAspect = viewRect.Height > 0 ? ((double)viewRect.Width / (double)viewRect.Height) : 0.0;
                this.rawAspect = extentHeight > 0.000000001 ? (extentWidth / extentHeight) : 0.0;

                double effectiveExtentWidth = extentWidth;
                double effectiveExtentHeight = extentHeight;
                string correction = "OFF";

                // 18:20 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT =====================================================================================================
                // FIX: Inventor Camera.GetExtents can return a camera extents pair whose aspect does not match the visible viewport.
                // In the user's Front X/Y tests Y matched visually, while X was compressed by about 1.7x.
                // Therefore we keep height as the stable axis when viewport is wider, and derive width from ViewRect aspect.
                // For a tall/narrow viewport, do the symmetrical correction from width.
                // This effective extent is used by BOTH directions: 3D box -> screen and screen rectangle -> green trace box.
                if (this.viewAspect > 0.000000001 && this.rawAspect > 0.000000001)
                {
                    double relativeAspectError = Math.Abs(this.rawAspect - this.viewAspect) / this.viewAspect;
                    if (relativeAspectError > 0.01)
                    {
                        if (this.rawAspect < this.viewAspect)
                        {
                            effectiveExtentHeight = extentHeight;
                            effectiveExtentWidth = extentHeight * this.viewAspect;
                            correction = "WidthFromHeightByViewportAspect";
                        }
                        else
                        {
                            effectiveExtentWidth = extentWidth;
                            effectiveExtentHeight = extentWidth / this.viewAspect;
                            correction = "HeightFromWidthByViewportAspect";
                        }
                    }
                }

                this.extentWidth = effectiveExtentWidth;
                this.extentHeight = effectiveExtentHeight;
                this.aspectCorrectionMode = correction;
                this.perspective = perspective;

                forward = CameraVector3D.Normalize(CameraVector3D.FromPoints(eye, target));
                up = CameraVector3D.Normalize(upVector);
                right = CameraVector3D.Normalize(CameraVector3D.Cross(forward, up));

                if (right.Length < 0.000000001)
                {
                    // Fallback if camera UpVector is accidentally parallel to forward.
                    right = CameraVector3D.Normalize(CameraVector3D.Cross(forward, new CameraVector3D(0.0, 0.0, 1.0)));
                    if (right.Length < 0.000000001)
                    {
                        right = CameraVector3D.Normalize(CameraVector3D.Cross(forward, new CameraVector3D(0.0, 1.0, 0.0)));
                    }
                }

                // Re-orthogonalize up to make the screen basis stable.
                up = CameraVector3D.Normalize(CameraVector3D.Cross(right, forward));
            }

            public bool IsReady
            {
                get
                {
                    return viewRect.Width > 0 && viewRect.Height > 0 &&
                           extentWidth > 0.000000001 && extentHeight > 0.000000001 &&
                           right.Length > 0.000000001 && up.Length > 0.000000001 && forward.Length > 0.000000001;
                }
            }

            public string StatusText
            {
                get
                {
                    return "OrthographicCameraBasisAspectCorrected" +
                           "; Extents=" + extentWidth.ToString("0.###", CultureInfo.InvariantCulture) + "x" + extentHeight.ToString("0.###", CultureInfo.InvariantCulture) +
                           "; RawExtents=" + rawExtentWidth.ToString("0.###", CultureInfo.InvariantCulture) + "x" + rawExtentHeight.ToString("0.###", CultureInfo.InvariantCulture) +
                           "; ViewportAspect=" + viewAspect.ToString("0.###", CultureInfo.InvariantCulture) +
                           "; CameraExtentsAspect=" + rawAspect.ToString("0.###", CultureInfo.InvariantCulture) +
                           "; AspectCorrection=" + aspectCorrectionMode +
                           "; PerspectiveFlag=" + perspective.ToString() +
                           "; Right=" + right.ToCompactText() +
                           "; Up=" + up.ToCompactText() +
                           "; Forward=" + forward.ToCompactText();
                }
            }

            public bool TryBuildTraceBoxFromScreenRectangle(System.Drawing.Rectangle selectionRect, double halfThickness, out SpatialBox traceBox)
            {
                traceBox = null;
                if (!IsReady || selectionRect.Width <= 0 || selectionRect.Height <= 0)
                {
                    return false;
                }

                System.Drawing.Rectangle clipped = System.Drawing.Rectangle.Intersect(selectionRect, viewRect);
                if (clipped.Width <= 0 || clipped.Height <= 0)
                {
                    return false;
                }

                if (halfThickness <= 0.000000001)
                {
                    halfThickness = 0.5;
                }

                CameraPoint3D[] points = new CameraPoint3D[]
                {
                    UnprojectScreenPointToWorld(clipped.Left, clipped.Top, -halfThickness),
                    UnprojectScreenPointToWorld(clipped.Right, clipped.Top, -halfThickness),
                    UnprojectScreenPointToWorld(clipped.Left, clipped.Bottom, -halfThickness),
                    UnprojectScreenPointToWorld(clipped.Right, clipped.Bottom, -halfThickness),
                    UnprojectScreenPointToWorld(clipped.Left, clipped.Top, halfThickness),
                    UnprojectScreenPointToWorld(clipped.Right, clipped.Top, halfThickness),
                    UnprojectScreenPointToWorld(clipped.Left, clipped.Bottom, halfThickness),
                    UnprojectScreenPointToWorld(clipped.Right, clipped.Bottom, halfThickness)
                };

                double minX = points[0].X;
                double maxX = points[0].X;
                double minY = points[0].Y;
                double maxY = points[0].Y;
                double minZ = points[0].Z;
                double maxZ = points[0].Z;

                for (int i = 1; i < points.Length; i++)
                {
                    minX = Math.Min(minX, points[i].X);
                    maxX = Math.Max(maxX, points[i].X);
                    minY = Math.Min(minY, points[i].Y);
                    maxY = Math.Max(maxY, points[i].Y);
                    minZ = Math.Min(minZ, points[i].Z);
                    maxZ = Math.Max(maxZ, points[i].Z);
                }

                traceBox = new SpatialBox(minX, minY, minZ, maxX, maxY, maxZ);
                traceBox.EnsureNonZeroSize(halfThickness);
                return true;
            }

            private CameraPoint3D UnprojectScreenPointToWorld(double screenX, double screenY, double depthOffsetAlongForward)
            {
                double cameraX = (((screenX - (double)viewRect.Left) / (double)viewRect.Width) - 0.5) * extentWidth;
                double cameraY = (0.5 - ((screenY - (double)viewRect.Top) / (double)viewRect.Height)) * extentHeight;

                return new CameraPoint3D(
                    target.X + (right.X * cameraX) + (up.X * cameraY) + (forward.X * depthOffsetAlongForward),
                    target.Y + (right.Y * cameraX) + (up.Y * cameraY) + (forward.Y * depthOffsetAlongForward),
                    target.Z + (right.Z * cameraX) + (up.Z * cameraY) + (forward.Z * depthOffsetAlongForward));
            }

            public bool TryProjectBoxToScreenRectangle(SpatialBox box, out System.Drawing.Rectangle rect)
            {
                rect = System.Drawing.Rectangle.Empty;
                if (box == null || !IsReady)
                {
                    return false;
                }

                CameraPoint3D[] points = new CameraPoint3D[]
                {
                    new CameraPoint3D(box.MinX, box.MinY, box.MinZ),
                    new CameraPoint3D(box.MinX, box.MinY, box.MaxZ),
                    new CameraPoint3D(box.MinX, box.MaxY, box.MinZ),
                    new CameraPoint3D(box.MinX, box.MaxY, box.MaxZ),
                    new CameraPoint3D(box.MaxX, box.MinY, box.MinZ),
                    new CameraPoint3D(box.MaxX, box.MinY, box.MaxZ),
                    new CameraPoint3D(box.MaxX, box.MaxY, box.MinZ),
                    new CameraPoint3D(box.MaxX, box.MaxY, box.MaxZ)
                };

                bool any = false;
                double minX = 0.0;
                double maxX = 0.0;
                double minY = 0.0;
                double maxY = 0.0;

                for (int i = 0; i < points.Length; i++)
                {
                    double screenX;
                    double screenY;
                    if (!TryProjectPointToScreen(points[i], out screenX, out screenY))
                    {
                        continue;
                    }

                    if (!any)
                    {
                        minX = maxX = screenX;
                        minY = maxY = screenY;
                        any = true;
                    }
                    else
                    {
                        minX = Math.Min(minX, screenX);
                        maxX = Math.Max(maxX, screenX);
                        minY = Math.Min(minY, screenY);
                        maxY = Math.Max(maxY, screenY);
                    }
                }

                if (!any)
                {
                    return false;
                }

                int left = (int)Math.Floor(minX);
                int rightInt = (int)Math.Ceiling(maxX);
                int top = (int)Math.Floor(minY);
                int bottom = (int)Math.Ceiling(maxY);

                if (rightInt <= left)
                {
                    rightInt = left + 1;
                }

                if (bottom <= top)
                {
                    bottom = top + 1;
                }

                rect = System.Drawing.Rectangle.FromLTRB(left, top, rightInt, bottom);
                return rect.Width > 0 && rect.Height > 0;
            }

            private bool TryProjectPointToScreen(CameraPoint3D point, out double screenX, out double screenY)
            {
                screenX = 0.0;
                screenY = 0.0;

                CameraVector3D fromTarget = CameraVector3D.FromPoints(target, point);
                double cameraX = CameraVector3D.Dot(fromTarget, right);
                double cameraY = CameraVector3D.Dot(fromTarget, up);

                // Orthographic mapping: v0.4.30 uses aspect-corrected effective extents, not raw Camera.GetExtents X/Y directly.
                screenX = viewRect.Left + ((cameraX / extentWidth) + 0.5) * viewRect.Width;
                screenY = viewRect.Top + (0.5 - (cameraY / extentHeight)) * viewRect.Height;

                return !double.IsNaN(screenX) && !double.IsNaN(screenY) && !double.IsInfinity(screenX) && !double.IsInfinity(screenY);
            }
        }

        private sealed class CameraPoint3D
        {
            public CameraPoint3D(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public double X { get; private set; }
            public double Y { get; private set; }
            public double Z { get; private set; }
        }

        private sealed class CameraVector3D
        {
            public CameraVector3D(double x, double y, double z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public double X { get; private set; }
            public double Y { get; private set; }
            public double Z { get; private set; }

            public double Length
            {
                get { return Math.Sqrt((X * X) + (Y * Y) + (Z * Z)); }
            }

            public static CameraVector3D FromPoints(CameraPoint3D a, CameraPoint3D b)
            {
                return new CameraVector3D(b.X - a.X, b.Y - a.Y, b.Z - a.Z);
            }

            public static CameraVector3D Normalize(CameraVector3D value)
            {
                if (value == null)
                {
                    return new CameraVector3D(0.0, 0.0, 0.0);
                }

                double length = value.Length;
                if (length < 0.000000001)
                {
                    return new CameraVector3D(0.0, 0.0, 0.0);
                }

                return new CameraVector3D(value.X / length, value.Y / length, value.Z / length);
            }

            public static double Dot(CameraVector3D a, CameraVector3D b)
            {
                if (a == null || b == null)
                {
                    return 0.0;
                }

                return (a.X * b.X) + (a.Y * b.Y) + (a.Z * b.Z);
            }

            public static CameraVector3D Cross(CameraVector3D a, CameraVector3D b)
            {
                if (a == null || b == null)
                {
                    return new CameraVector3D(0.0, 0.0, 0.0);
                }

                return new CameraVector3D(
                    (a.Y * b.Z) - (a.Z * b.Y),
                    (a.Z * b.X) - (a.X * b.Z),
                    (a.X * b.Y) - (a.Y * b.X));
            }

            public string ToCompactText()
            {
                return "(" +
                       X.ToString("0.###", CultureInfo.InvariantCulture) + "," +
                       Y.ToString("0.###", CultureInfo.InvariantCulture) + "," +
                       Z.ToString("0.###", CultureInfo.InvariantCulture) + ")";
            }
        }

        private static System.Drawing.Rectangle ProjectSpatialBoxToScreenRectangle(SpatialBox box, SpatialBox modelBox, System.Drawing.Rectangle viewRect, CustomCubeRectPlane plane)
        {
            double hMin;
            double hMax;
            double vMin;
            double vMax;
            double modelHMin;
            double modelHMax;
            double modelVMin;
            double modelVMax;

            GetPlaneRanges(box, modelBox, plane, out hMin, out hMax, out vMin, out vMax, out modelHMin, out modelHMax, out modelVMin, out modelVMax);

            double hSize = modelHMax - modelHMin;
            double vSize = modelVMax - modelVMin;

            if (Math.Abs(hSize) < 0.000000001)
            {
                hSize = 1.0;
            }

            if (Math.Abs(vSize) < 0.000000001)
            {
                vSize = 1.0;
            }

            double leftD = viewRect.Left + ((hMin - modelHMin) / hSize) * viewRect.Width;
            double rightD = viewRect.Left + ((hMax - modelHMin) / hSize) * viewRect.Width;

            // Экранная Y-координата растёт вниз, поэтому max модели рисуется выше.
            double topD = viewRect.Top + (1.0 - ((vMax - modelVMin) / vSize)) * viewRect.Height;
            double bottomD = viewRect.Top + (1.0 - ((vMin - modelVMin) / vSize)) * viewRect.Height;

            int left = (int)Math.Floor(Math.Min(leftD, rightD));
            int right = (int)Math.Ceiling(Math.Max(leftD, rightD));
            int top = (int)Math.Floor(Math.Min(topD, bottomD));
            int bottom = (int)Math.Ceiling(Math.Max(topD, bottomD));

            if (right <= left)
            {
                right = left + 1;
            }

            if (bottom <= top)
            {
                bottom = top + 1;
            }

            return System.Drawing.Rectangle.FromLTRB(left, top, right, bottom);
        }

        private bool ShowTouchedSpatialCubesPreview(PartDocument partDoc, List<SpatialCubeCell> touchedCells, CustomRectBodySelectionMode bodyMode, System.Drawing.Rectangle selectionRect, System.Drawing.Rectangle viewRect, CustomCubeRectPlane plane)
        {
            using (AppLogger.Scope("ShowTouchedSpatialCubesPreview"))
            {
            if (partDoc == null || touchedCells == null || touchedCells.Count == 0)
            {
                return false;
            }

            ClearTouchedSpatialCubesPreview(partDoc);

            try
            {
                dynamic compDef = partDoc.ComponentDefinition;
                dynamic clientGraphicsCollection = compDef.ClientGraphicsCollection;
                dynamic clientGraphics = clientGraphicsCollection.Add(TouchedSpatialCubesPreviewGraphicsName);

                dynamic transientGeometry = _invApp.TransientGeometry;
                dynamic transientBRep = _invApp.TransientBRep;

                int builtCount = 0;
                int skippedCount = 0;

                foreach (SpatialCubeCell cell in touchedCells)
                {
                    if (cell == null || cell.Bounds == null)
                    {
                        skippedCount++;
                        continue;
                    }

                    dynamic graphicsNode = clientGraphics.AddNode(builtCount + 1);
                    dynamic inventorBox = transientGeometry.CreateBox();
                    dynamic minPoint = transientGeometry.CreatePoint(cell.Bounds.MinX, cell.Bounds.MinY, cell.Bounds.MinZ);
                    dynamic maxPoint = transientGeometry.CreatePoint(cell.Bounds.MaxX, cell.Bounds.MaxY, cell.Bounds.MaxZ);

                    try
                    {
                        inventorBox.PutBoxData(minPoint, maxPoint);
                    }
                    catch
                    {
                        try
                        {
                            inventorBox.MinPoint = minPoint;
                            inventorBox.MaxPoint = maxPoint;
                        }
                        catch
                        {
                        }
                    }

                    dynamic previewBody = transientBRep.CreateSolidBlock(inventorBox);
                    dynamic surfaceGraphics = graphicsNode.AddSurfaceGraphics(previewBody);

                    // v0.4.30: touched cubes preview uses amber/orange, not the blue selected-cube color.
                    try
                    {
                        dynamic color = _invApp.TransientObjects.CreateColor(255, 170, 0);
                        color.Opacity = 0.16;
                        surfaceGraphics.Color = color;
                    }
                    catch
                    {
                    }

                    try
                    {
                        surfaceGraphics.Translucent = true;
                    }
                    catch
                    {
                    }

                    builtCount++;
                }

                try
                {
                    clientGraphics.Selectable = false;
                }
                catch
                {
                }

                try
                {
                    partDoc.Update();
                }
                catch
                {
                }

                AppLogger.Log(
                    "CUSTOM_RECT_REAL_CAMERA_ASPECT_CORRECT_BUILT",
                    "ShowTouchedSpatialCubesPreview",
                    "TouchedCellsInput=" + touchedCells.Count.ToString() +
                    "; BuiltPreviewBoxes=" + builtCount.ToString() +
                    "; SkippedCells=" + skippedCount.ToString() +
                    "; BodyMode=" + GetCustomRectBodySelectionModeText(bodyMode) +
                    "; Plane=" + GetCustomRectTracePlaneTextSafe(plane) +
                    "; SelectionRect=" + FormatScreenRectangle(selectionRect) +
                    "; ViewRect=" + FormatScreenRectangle(viewRect) +
                    "; Color=Amber_255_170_0" +
                    "; Opacity=0.16" +
                    "; CellIds=" + BuildCellIdsText(touchedCells) +
                    "; IsInventorBody=False; ClientGraphics=True; Selectable=False");

                return builtCount > 0;
            }
            catch (Exception ex)
            {
                AppLogger.LogException("ShowTouchedSpatialCubesPreview", ex);
                return false;
            }
        
            }}

        private bool ShowCustomRectTraceBoxPreview(PartDocument partDoc, SpatialBox traceBox, System.Drawing.Rectangle selectionRect, System.Drawing.Rectangle viewRect, CustomCubeRectPlane plane, SpatialQueryResult screenQuery)
        {
            using (AppLogger.Scope("ShowCustomRectTraceBoxPreview"))
            {
            if (partDoc == null || traceBox == null)
            {
                return false;
            }

            ClearCustomRectTracePreview(partDoc);

            try
            {
                dynamic compDef = partDoc.ComponentDefinition;
                dynamic clientGraphicsCollection = compDef.ClientGraphicsCollection;
                dynamic clientGraphics = clientGraphicsCollection.Add(CustomRectTraceGraphicsName);
                dynamic graphicsNode = clientGraphics.AddNode(1);

                dynamic transientGeometry = _invApp.TransientGeometry;
                dynamic inventorBox = transientGeometry.CreateBox();
                dynamic minPoint = transientGeometry.CreatePoint(traceBox.MinX, traceBox.MinY, traceBox.MinZ);
                dynamic maxPoint = transientGeometry.CreatePoint(traceBox.MaxX, traceBox.MaxY, traceBox.MaxZ);

                try
                {
                    inventorBox.PutBoxData(minPoint, maxPoint);
                }
                catch
                {
                    try
                    {
                        inventorBox.MinPoint = minPoint;
                        inventorBox.MaxPoint = maxPoint;
                    }
                    catch
                    {
                    }
                }

                dynamic transientBRep = _invApp.TransientBRep;
                dynamic previewBody = transientBRep.CreateSolidBlock(inventorBox);
                dynamic surfaceGraphics = graphicsNode.AddSurfaceGraphics(previewBody);

                try
                {
                    dynamic color = _invApp.TransientObjects.CreateColor(0, 255, 80);
                    color.Opacity = 0.28;
                    surfaceGraphics.Color = color;
                }
                catch
                {
                }

                try
                {
                    surfaceGraphics.Translucent = true;
                }
                catch
                {
                }

                try
                {
                    clientGraphics.Selectable = false;
                }
                catch
                {
                }

                try
                {
                    partDoc.Update();
                }
                catch
                {
                }

                AppLogger.Log(
                    "CUSTOM_RECT_TRACE_BOX_PREVIEW_BUILT",
                    "ShowCustomRectTraceBoxPreview",
                    "TraceBox=" + FormatSpatialBox(traceBox) +
                    "; Plane=" + GetCustomRectTracePlaneTextSafe(plane) +
                    "; SelectionRect=" + FormatScreenRectangle(selectionRect) +
                    "; ViewRect=" + FormatScreenRectangle(viewRect) +
                    "; TouchedCells=" + (screenQuery == null || screenQuery.TouchedCells == null ? "0" : screenQuery.TouchedCells.Count.ToString()) +
                    "; IsInventorBody=False; ClientGraphics=True; Selectable=False");

                return true;
            }
            catch (Exception ex)
            {
                AppLogger.LogException("ShowCustomRectTraceBoxPreview", ex);
                return false;
            }
        
            }}

        private SpatialBox BuildCustomRectTraceBoxFromScreenRectangle(System.Drawing.Rectangle selectionRect, System.Drawing.Rectangle viewRect, CustomCubeRectPlane plane, SpatialQueryResult screenQuery)
        {
            using (AppLogger.Scope("BuildCustomRectTraceBoxFromScreenRectangle"))
            {
            if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady || _spatialCubesIndex.ModelBox == null)
            {
                return null;
            }

            System.Drawing.Rectangle clipped = System.Drawing.Rectangle.Intersect(selectionRect, viewRect);
            if (clipped.Width <= 0 || clipped.Height <= 0 || viewRect.Width <= 0 || viewRect.Height <= 0)
            {
                return null;
            }

            SpatialBox modelBox = _spatialCubesIndex.ModelBox;
            double hiddenMin;
            double hiddenMax;
            GetCustomRectTraceHiddenRange(modelBox, plane, screenQuery, out hiddenMin, out hiddenMax);

            // 17:35 16.06.2026 InventorIptOrg_v0_4_30_REAL_CAMERA_ASPECT_CORRECT =====================================================================================================
            // v0.4.29 показала, что RealCamera без viewport aspect correction сжимает зелёный 3D след по X.
            // v0.4.30 строит green trace box из той же ActiveView.Camera/GetExtents + viewport-aspect-corrected математики, что и реальный выбор кубиков/body-box.
            // Для рабочего orthographic Front view экранная рамка -> camera target plane -> тонкий axis-aligned trace box.
            // Контрольное событие: CUSTOM_RECT_TRACE_BOX_REAL_CAMERA_CALCULATED. В ProjectionDetails должны появиться RawExtents, ViewportAspect, CameraExtentsAspect, AspectCorrection.
            // ============================================================================================================================================================================
            InventorScreenProjector traceProjector = null;
            string traceProjectorDetails = "NotStarted";
            if (TryCreateInventorScreenProjector(viewRect, out traceProjector, out traceProjectorDetails) && traceProjector != null)
            {
                SpatialBox realCameraTraceBox = null;
                double halfThickness = GetSafeTraceFallbackSize(modelBox);
                if (traceProjector.TryBuildTraceBoxFromScreenRectangle(clipped, halfThickness, out realCameraTraceBox) && realCameraTraceBox != null)
                {
                    realCameraTraceBox = ExpandTraceBoxToHiddenRange(realCameraTraceBox, plane, hiddenMin, hiddenMax);
                    realCameraTraceBox.EnsureNonZeroSize(GetSafeTraceFallbackSize(modelBox));
                    AppLogger.Log(
                        "CUSTOM_RECT_TRACE_BOX_REAL_CAMERA_CALCULATED",
                        "BuildCustomRectTraceBoxFromScreenRectangle",
                        "TraceBox=" + FormatSpatialBox(realCameraTraceBox) +
                        "; ProjectionMode=RealCameraActiveViewOrthographicAspectCorrected" +
                        "; ProjectionDetails=" + traceProjectorDetails +
                        "; SelectionRect=" + FormatScreenRectangle(selectionRect) +
                        "; ClippedSelectionRect=" + FormatScreenRectangle(clipped) +
                        "; ViewRect=" + FormatScreenRectangle(viewRect) +
                        "; HalfThickness=" + halfThickness.ToString("0.###", CultureInfo.InvariantCulture) +
                        "; HiddenRange=" + hiddenMin.ToString("0.###", CultureInfo.InvariantCulture) + ".." + hiddenMax.ToString("0.###", CultureInfo.InvariantCulture) +
                        "; HiddenRangeMode=TouchedCubesFullDepth" +
                        "; TraceBoxMode=RealCameraAspectCorrectedScreenRectangleUnprojectedToTargetPlaneAxisAlignedBoxExpandedToTouchedCubeDepth");
                    return realCameraTraceBox;
                }
            }

            AppLogger.Log(
                "CUSTOM_RECT_TRACE_BOX_REAL_CAMERA_FALLBACK_TO_LEGACY",
                "BuildCustomRectTraceBoxFromScreenRectangle",
                "Reason=RealCameraTraceBoxBuildFailed" +
                "; ProjectionDetails=" + traceProjectorDetails +
                "; SelectionRect=" + FormatScreenRectangle(selectionRect) +
                "; ViewRect=" + FormatScreenRectangle(viewRect));

            double modelHMin;
            double modelHMax;
            double modelVMin;
            double modelVMax;
            GetPlaneModelRanges(modelBox, plane, out modelHMin, out modelHMax, out modelVMin, out modelVMax);

            double hSize = modelHMax - modelHMin;
            double vSize = modelVMax - modelVMin;
            if (Math.Abs(hSize) < 0.000000001)
            {
                hSize = 1.0;
            }

            if (Math.Abs(vSize) < 0.000000001)
            {
                vSize = 1.0;
            }

            double leftRatio = Clamp01(((double)clipped.Left - (double)viewRect.Left) / (double)viewRect.Width);
            double rightRatio = Clamp01(((double)clipped.Right - (double)viewRect.Left) / (double)viewRect.Width);
            double topRatio = Clamp01(((double)clipped.Top - (double)viewRect.Top) / (double)viewRect.Height);
            double bottomRatio = Clamp01(((double)clipped.Bottom - (double)viewRect.Top) / (double)viewRect.Height);

            double h1 = modelHMin + leftRatio * hSize;
            double h2 = modelHMin + rightRatio * hSize;

            // Экранная Y-координата растёт вниз, поэтому top даёт верхнюю/max модельную координату.
            double vTop = modelVMin + (1.0 - topRatio) * vSize;
            double vBottom = modelVMin + (1.0 - bottomRatio) * vSize;

            double hMin = Math.Min(h1, h2);
            double hMax = Math.Max(h1, h2);
            double vMin = Math.Min(vTop, vBottom);
            double vMax = Math.Max(vTop, vBottom);

            SpatialBox traceBox;
            if (plane == CustomCubeRectPlane.FrontXZ)
            {
                traceBox = new SpatialBox(hMin, hiddenMin, vMin, hMax, hiddenMax, vMax);
            }
            else if (plane == CustomCubeRectPlane.SideYZ)
            {
                traceBox = new SpatialBox(hiddenMin, hMin, vMin, hiddenMax, hMax, vMax);
            }
            else
            {
                traceBox = new SpatialBox(hMin, vMin, hiddenMin, hMax, vMax, hiddenMax);
            }

            traceBox.EnsureNonZeroSize(GetSafeTraceFallbackSize(modelBox));
            AppLogger.Log(
                "CUSTOM_RECT_TRACE_BOX_CALCULATED",
                "BuildCustomRectTraceBoxFromScreenRectangle",
                "TraceBox=" + FormatSpatialBox(traceBox) +
                "; Plane=" + GetCustomRectTracePlaneTextSafe(plane) +
                "; SelectionRect=" + FormatScreenRectangle(selectionRect) +
                "; ClippedSelectionRect=" + FormatScreenRectangle(clipped) +
                "; HiddenAxisMode=OneSpatialCubeLayerOrModelCenterFallback");

            return traceBox;
            }}

        private static SpatialBox ExpandTraceBoxToHiddenRange(SpatialBox baseTraceBox, CustomCubeRectPlane plane, double hiddenMin, double hiddenMax)
        {
            if (baseTraceBox == null)
            {
                return null;
            }

            if (plane == CustomCubeRectPlane.FrontXZ)
            {
                return new SpatialBox(
                    baseTraceBox.MinX,
                    hiddenMin,
                    baseTraceBox.MinZ,
                    baseTraceBox.MaxX,
                    hiddenMax,
                    baseTraceBox.MaxZ);
            }

            if (plane == CustomCubeRectPlane.SideYZ)
            {
                return new SpatialBox(
                    hiddenMin,
                    baseTraceBox.MinY,
                    baseTraceBox.MinZ,
                    hiddenMax,
                    baseTraceBox.MaxY,
                    baseTraceBox.MaxZ);
            }

            return new SpatialBox(
                baseTraceBox.MinX,
                baseTraceBox.MinY,
                hiddenMin,
                baseTraceBox.MaxX,
                baseTraceBox.MaxY,
                hiddenMax);
        }

        private static void GetPlaneModelRanges(SpatialBox modelBox, CustomCubeRectPlane plane, out double modelHMin, out double modelHMax, out double modelVMin, out double modelVMax)
        {
            if (plane == CustomCubeRectPlane.FrontXZ)
            {
                modelHMin = modelBox.MinX;
                modelHMax = modelBox.MaxX;
                modelVMin = modelBox.MinZ;
                modelVMax = modelBox.MaxZ;
                return;
            }

            if (plane == CustomCubeRectPlane.SideYZ)
            {
                modelHMin = modelBox.MinY;
                modelHMax = modelBox.MaxY;
                modelVMin = modelBox.MinZ;
                modelVMax = modelBox.MaxZ;
                return;
            }

            modelHMin = modelBox.MinX;
            modelHMax = modelBox.MaxX;
            modelVMin = modelBox.MinY;
            modelVMax = modelBox.MaxY;
        }

        private void GetCustomRectTraceHiddenRange(SpatialBox modelBox, CustomCubeRectPlane plane, SpatialQueryResult screenQuery, out double hiddenMin, out double hiddenMax)
        {
            hiddenMin = 0.0;
            hiddenMax = 1.0;

            if (modelBox == null)
            {
                return;
            }

            double modelMin;
            double modelMax;
            double modelSize;

            if (plane == CustomCubeRectPlane.FrontXZ)
            {
                modelMin = modelBox.MinY;
                modelMax = modelBox.MaxY;
                modelSize = modelBox.SizeY;
            }
            else if (plane == CustomCubeRectPlane.SideYZ)
            {
                modelMin = modelBox.MinX;
                modelMax = modelBox.MaxX;
                modelSize = modelBox.SizeX;
            }
            else
            {
                modelMin = modelBox.MinZ;
                modelMax = modelBox.MaxZ;
                modelSize = modelBox.SizeZ;
            }

            bool hasTouchedDepth = false;
            double touchedMin = 0.0;
            double touchedMax = 0.0;

            if (screenQuery != null && screenQuery.TouchedCells != null)
            {
                foreach (SpatialCubeCell cell in screenQuery.TouchedCells)
                {
                    if (cell == null || cell.Bounds == null)
                    {
                        continue;
                    }

                    double cellMin;
                    double cellMax;
                    if (plane == CustomCubeRectPlane.FrontXZ)
                    {
                        cellMin = cell.Bounds.MinY;
                        cellMax = cell.Bounds.MaxY;
                    }
                    else if (plane == CustomCubeRectPlane.SideYZ)
                    {
                        cellMin = cell.Bounds.MinX;
                        cellMax = cell.Bounds.MaxX;
                    }
                    else
                    {
                        cellMin = cell.Bounds.MinZ;
                        cellMax = cell.Bounds.MaxZ;
                    }

                    if (!hasTouchedDepth)
                    {
                        touchedMin = cellMin;
                        touchedMax = cellMax;
                        hasTouchedDepth = true;
                    }
                    else
                    {
                        touchedMin = Math.Min(touchedMin, cellMin);
                        touchedMax = Math.Max(touchedMax, cellMax);
                    }
                }
            }

            if (hasTouchedDepth)
            {
                hiddenMin = touchedMin;
                hiddenMax = touchedMax;
                return;
            }

            double layerSize = modelSize / Math.Max(1, _spatialCubesIndex == null ? 1 : _spatialCubesIndex.DivisionsPerAxis);
            if (Math.Abs(layerSize) < 0.000000001)
            {
                layerSize = GetSafeTraceFallbackSize(modelBox);
            }

            double center = (modelMin + modelMax) * 0.5;
            hiddenMin = center - layerSize * 0.5;
            hiddenMax = center + layerSize * 0.5;

            if (hiddenMin < modelMin)
            {
                hiddenMin = modelMin;
                hiddenMax = Math.Min(modelMax, hiddenMin + layerSize);
            }

            if (hiddenMax > modelMax)
            {
                hiddenMax = modelMax;
                hiddenMin = Math.Max(modelMin, hiddenMax - layerSize);
            }
        }

        private static double GetSafeTraceFallbackSize(SpatialBox modelBox)
        {
            if (modelBox == null)
            {
                return 1.0;
            }

            double maxSize = Math.Max(modelBox.SizeX, Math.Max(modelBox.SizeY, modelBox.SizeZ));
            if (Math.Abs(maxSize) < 0.000000001)
            {
                return 1.0;
            }

            return maxSize * 0.01;
        }

        private static double Clamp01(double value)
        {
            if (value < 0.0)
            {
                return 0.0;
            }

            if (value > 1.0)
            {
                return 1.0;
            }

            return value;
        }

        private static string GetCustomRectTracePlaneTextSafe(CustomCubeRectPlane plane)
        {
            return GetCustomCubeRectPlaneText(plane);
        }

        private static System.Drawing.Point GetRectangleCenter(System.Drawing.Rectangle rect)
        {
            return new System.Drawing.Point(
                rect.Left + (rect.Width / 2),
                rect.Top + (rect.Height / 2));
        }

        private static bool RectangleContainsPointInclusive(System.Drawing.Rectangle rect, System.Drawing.Point point)
        {
            return point.X >= rect.Left &&
                   point.X <= rect.Right &&
                   point.Y >= rect.Top &&
                   point.Y <= rect.Bottom;
        }

        private static bool RectanglesIntersectInclusive(System.Drawing.Rectangle a, System.Drawing.Rectangle b)
        {
            if (a.Width <= 0 || a.Height <= 0 || b.Width <= 0 || b.Height <= 0)
            {
                return false;
            }

            return a.Right >= b.Left &&
                   b.Right >= a.Left &&
                   a.Bottom >= b.Top &&
                   b.Bottom >= a.Top;
        }

        private static bool SpatialBoxFullyInsideInclusive(SpatialBox inner, SpatialBox outer, double tolerance)
        {
            if (inner == null || outer == null)
            {
                return false;
            }

            if (tolerance < 0.0)
            {
                tolerance = 0.0;
            }

            return inner.MinX >= outer.MinX - tolerance &&
                   inner.MaxX <= outer.MaxX + tolerance &&
                   inner.MinY >= outer.MinY - tolerance &&
                   inner.MaxY <= outer.MaxY + tolerance &&
                   inner.MinZ >= outer.MinZ - tolerance &&
                   inner.MaxZ <= outer.MaxZ + tolerance;
        }

        private static double RectangleIntersectionAreaRatio(System.Drawing.Rectangle inner, System.Drawing.Rectangle outer)
        {
            if (inner.Width <= 0 || inner.Height <= 0 || outer.Width <= 0 || outer.Height <= 0)
            {
                return 0.0;
            }

            System.Drawing.Rectangle intersection = System.Drawing.Rectangle.Intersect(inner, outer);
            if (intersection.Width <= 0 || intersection.Height <= 0)
            {
                return 0.0;
            }

            double intersectionArea = (double)intersection.Width * (double)intersection.Height;
            double outerArea = (double)outer.Width * (double)outer.Height;
            if (outerArea <= 0.0)
            {
                return 0.0;
            }

            return intersectionArea / outerArea;
        }

        private static void GetPlaneRanges(
            SpatialBox box,
            SpatialBox modelBox,
            CustomCubeRectPlane plane,
            out double hMin,
            out double hMax,
            out double vMin,
            out double vMax,
            out double modelHMin,
            out double modelHMax,
            out double modelVMin,
            out double modelVMax)
        {
            if (plane == CustomCubeRectPlane.FrontXZ)
            {
                hMin = box.MinX;
                hMax = box.MaxX;
                vMin = box.MinZ;
                vMax = box.MaxZ;
                modelHMin = modelBox.MinX;
                modelHMax = modelBox.MaxX;
                modelVMin = modelBox.MinZ;
                modelVMax = modelBox.MaxZ;
                return;
            }

            if (plane == CustomCubeRectPlane.SideYZ)
            {
                hMin = box.MinY;
                hMax = box.MaxY;
                vMin = box.MinZ;
                vMax = box.MaxZ;
                modelHMin = modelBox.MinY;
                modelHMax = modelBox.MaxY;
                modelVMin = modelBox.MinZ;
                modelVMax = modelBox.MaxZ;
                return;
            }

            hMin = box.MinX;
            hMax = box.MaxX;
            vMin = box.MinY;
            vMax = box.MaxY;
            modelHMin = modelBox.MinX;
            modelHMax = modelBox.MaxX;
            modelVMin = modelBox.MinY;
            modelVMax = modelBox.MaxY;
        }

        private bool TryGetInventorViewScreenRectangle(out System.Drawing.Rectangle rect)
        {
            using (AppLogger.Scope("TryGetInventorViewScreenRectangle"))
            {
            rect = System.Drawing.Rectangle.Empty;

            IntPtr hwnd = IntPtr.Zero;

            try
            {
                dynamic dynApp = _invApp;
                dynamic activeView = dynApp.ActiveView;
                object viewHwnd = activeView.HWND;
                hwnd = ToIntPtrFromComHandle(viewHwnd);
            }
            catch
            {
                hwnd = IntPtr.Zero;
            }

            if (hwnd == IntPtr.Zero)
            {
                try
                {
                    dynamic dynApp = _invApp;
                    object frameHwnd = dynApp.MainFrameHWND;
                    hwnd = ToIntPtrFromComHandle(frameHwnd);
                }
                catch
                {
                    hwnd = IntPtr.Zero;
                }
            }

            if (hwnd == IntPtr.Zero)
            {
                return false;
            }

            NativeRect nativeRect;
            if (!GetWindowRect(hwnd, out nativeRect))
            {
                return false;
            }

            rect = System.Drawing.Rectangle.FromLTRB(nativeRect.Left, nativeRect.Top, nativeRect.Right, nativeRect.Bottom);
            AppLogger.Log("CUSTOM_CUBE_RECT_VIEW_RECT", "TryGetInventorViewScreenRectangle", "Hwnd=" + hwnd.ToString() + "; Rect=" + FormatScreenRectangle(rect));

            return rect.Width > 10 && rect.Height > 10;
            }}

        private static IntPtr ToIntPtrFromComHandle(object handleValue)
        {
            if (handleValue == null)
            {
                return IntPtr.Zero;
            }

            if (handleValue is IntPtr)
            {
                return (IntPtr)handleValue;
            }

            try
            {
                return new IntPtr(Convert.ToInt64(handleValue, CultureInfo.InvariantCulture));
            }
            catch
            {
                return IntPtr.Zero;
            }
        }

        private static string FormatScreenRectangle(System.Drawing.Rectangle rect)
        {
            return "L" + rect.Left.ToString(CultureInfo.InvariantCulture) +
                   " T" + rect.Top.ToString(CultureInfo.InvariantCulture) +
                   " R" + rect.Right.ToString(CultureInfo.InvariantCulture) +
                   " B" + rect.Bottom.ToString(CultureInfo.InvariantCulture) +
                   " W" + rect.Width.ToString(CultureInfo.InvariantCulture) +
                   " H" + rect.Height.ToString(CultureInfo.InvariantCulture);
        }

        private static string BuildCellIdsText(List<SpatialCubeCell> cells)
        {
            if (cells == null || cells.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < cells.Count; i++)
            {
                if (i > 0)
                {
                    sb.Append(", ");
                }

                sb.Append(cells[i].Id);
            }

            return sb.ToString();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct NativeRect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out NativeRect lpRect);

        private void ApplySpatialCubesInfoToBrowserTreeRow(DataGridViewRow row, BrowserTreeGridItem item)
        {
            using (AppLogger.Scope("ApplySpatialCubesInfoToBrowserTreeRow"))
            {
            if (row == null || item == null || dataGridViewIptBrowserTree.Columns.Count <= BrowserGridColCubeIds)
            {
                return;
            }

            row.Cells[BrowserGridColCubeCount].Value = string.Empty;
            row.Cells[BrowserGridColCubeIds].Value = string.Empty;

            if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady)
            {
                return;
            }

            SurfaceBody body = item.NativeObject as SurfaceBody;
            if (body == null)
            {
                return;
            }

            SpatialBodyRecord record = _spatialCubesIndex.FindBodyRecord(GetComIdentityKey(body));
            if (record == null)
            {
                return;
            }

            // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Здесь показывается принадлежность самого тела к кубикам индекса.
            // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Пример: Тело 11 может лежать сразу в 2 кубиках: CUBE_X0_Y0_Z1 и CUBE_X0_Y1_Z1.
            // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Это отдельная информация и она не равна списку кубиков, которые задела текущая рамка selectionLimits.
            row.Cells[BrowserGridColCubeCount].Value = record.CubeIds == null ? "0" : record.CubeIds.Count.ToString();
            row.Cells[BrowserGridColCubeIds].Value = record.CubeIdsText;
        
            }}

        private static string FormatSpatialBox(SpatialBox box)
        {
            using (AppLogger.Scope("FormatSpatialBox"))
            {
            if (box == null)
            {
                return "<null>";
            }

            return "X[" + FormatSpatialDouble(box.MinX) + ".." + FormatSpatialDouble(box.MaxX) + "] " +
                   "Y[" + FormatSpatialDouble(box.MinY) + ".." + FormatSpatialDouble(box.MaxY) + "] " +
                   "Z[" + FormatSpatialDouble(box.MinZ) + ".." + FormatSpatialDouble(box.MaxZ) + "]";
        
            }}

        private static string FormatSpatialDouble(double value)
        {
            using (AppLogger.Scope("FormatSpatialDouble"))
            {
            return value.ToString("0.####", CultureInfo.InvariantCulture);
        
            }}

        // ============================================================
        // Вкладка .iam
        // Исходная логика из Lesson 5: работа с компонентами сборки.
        // ============================================================
        private void Button1_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("Button1_Click"))
            {
            if (!TryGetActiveAssemblyDocument(out AssemblyDocument asmDoc))
            {
                return;
            }

            if (asmDoc.SelectSet.Count == 0)
            {
                LoggedMessageBox.Show("Need to select a Part or Sub Assembly");
                return;
            }

            SelectSet selSet = asmDoc.SelectSet;

            try
            {
                foreach (object obj in selSet)
                {
                    ComponentOccurrence compOcc = (ComponentOccurrence)obj;
                    Debug.Print(compOcc.Name);
                    // compOcc.Visible = false; // пример: скрыть компонент

                    AttributeSets attbSets = compOcc.AttributeSets;

                    // Добавляем атрибуты к ComponentOccurrence.
                    // В VB это было: If Not attbSets.NameIsUsed("myPartGroup") Then
                    if (!AttributeSetNameIsUsed(attbSets, "myPartGroup"))
                    {
                        AttributeSet attbSet = attbSets.Add("myPartGroup");

                        Inventor.Attribute attb = attbSet.Add(
                            "PartGroup1",
                            ValueTypeEnum.kStringType,
                            "Group1");
                    }
                }

                LoggedMessageBox.Show("Selected assembly components were added to group.");
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Is the selected item a Component?");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        private void Button2_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("Button2_Click"))
            {
            if (!TryGetActiveAssemblyDocument(out AssemblyDocument asmDoc))
            {
                return;
            }

            try
            {
                AttributeManager attbMan = asmDoc.AttributeManager;

                ObjectCollection objsCol = attbMan.FindObjects(
                    "myPartGroup",
                    "PartGroup1",
                    "Group1");

                int count = 0;

                foreach (object obj in objsCol)
                {
                    ComponentOccurrence compOcc = (ComponentOccurrence)obj;

                    // Переключаем видимость ComponentOccurrence.
                    compOcc.Visible = !compOcc.Visible;
                    count++;
                }

                LoggedMessageBox.Show("Assembly components toggled: " + count.ToString());
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem hiding component");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        // ============================================================
        // Вкладка .ipt
        // Работа с телами детали. Можно выбрать грань или тело.
        // Если выбрана грань, код добавляет/переключает всё тело, которому принадлежит эта грань.
        // ============================================================
        private void ButtonIptAdd_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptAdd_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            if (partDoc.SelectSet.Count == 0)
            {
                LoggedMessageBox.Show("Select a solid body or a face in the part");
                return;
            }

            try
            {
                int count = 0;

                foreach (object obj in partDoc.SelectSet)
                {
                    SurfaceBody body = GetSurfaceBodyFromSelectedObject(obj);

                    if (body == null)
                    {
                        continue;
                    }

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 1 / найденное тело: AddBodyToGroup для 2 тел — 0.201 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 1 / найденное тело: AddFeaturesForBodyToGroup для 2 тел — 0.172 с.
                    AddBodyToGroup(body);
                    AddFeaturesForBodyToGroup(body);
                    count++;
                }

                if (count == 0)
                {
                    LoggedMessageBox.Show("No solid bodies found in the current selection. Select a face or a solid body.");
                    return;
                }

                RefreshIptGroupList(partDoc);
                RefreshIptFeatureList(partDoc);

                LoggedMessageBox.Show("Selected IPT bodies were added to group: " + count.ToString());
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem adding selected IPT bodies to group");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        // Новый интерактивный сценарий:
        // 1) Нажать кнопку.
        // 2) Пользователь получает приглашение.
        // 3) Пользователь тянет рамку ЛКМ в окне Inventor.
        // 4) После отпускания ЛКМ срабатывает OnSelect.
        // 5) Строим RangeBox только по выбранным видимым ТЕЛАМ.
        // 6) Проходим по ВСЕМ SurfaceBodies в .ipt и добавляем каждое тело, чей RangeBox пересекает эту область.
        // Важно: НЕ добавлять здесь kPartFaceFilter. Он может вернуть тысячи граней и сильно замедлить выбор.

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS =====================================================================================================
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS 1. Select Frame + Add Inner/Hidden Bodies
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS -----------------------------------------------------------------------------------------------------
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Select Frame — полный цикл: 5.571 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS └─ Нажатие кнопки / запуск режима выбора — 1.563 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    └─ Inventor возвращает видимые SurfaceBody из рамки — точного отдельного замера нет
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS       └─ Пользователь тянул рамку + Inventor вызвал OnSelect — 2.867 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          └─ IptWindowSelectEvents_OnSelect — 2.141 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS             ├─ Начальная подготовка OnSelect — 0.046 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS             ├─ построить общий RangeBox по видимым телам — 0.003689 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS             │  └─ вместе с GetComIdentityKey — 0.004106 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS             └─ пройти по всем SurfaceBodies детали — 0.189 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS                ├─ для каждого тела: Intersects(body.RangeBox)
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS                │  └─ 72 проверки Intersects — 0.189 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS                └─ если тело попало:
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS                   ├─ AddBodyToGroup
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS                   │  └─ 2 тела — 0.201 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS                   └─ AddFeaturesForBodyToGroup
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS                      └─ поиск features только для найденных тел
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS                         └─ 2 тела — 0.172 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS             ├─ RefreshIptGroupList — 0.063 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS             ├─ RefreshIptFeatureList — 0.056 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS             └─ MessageBox "Объекты добавлены в группу" — 1.316 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Инженерный вывод:
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS сам перебор тел быстрый: 72 Intersects = 0.189 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Основная стоимость шага — действия пользователя и MessageBox, а не алгоритм RangeBox.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS =====================================================================================================

        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX =====================================================================================================
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX 1. Select Frame + Add Inner/Hidden Bodies — SPATIAL CUBES INDEX MODE
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX -----------------------------------------------------------------------------------------------------
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Select Frame — полный цикл: 20.462 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX └─ Нажатие кнопки / запуск режима выбора — 11.255 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX    ├─ TryGetActivePartDocument — 0.032 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX    ├─ MessageBox-инструкция выбора рамкой — 10.856 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX    ├─ TryAddSelectionFilter — 0.004 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX    └─ прочая подготовка InteractionEvents / UI — около 0.363 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX       └─ Пользователь тянул рамку + Inventor вызвал OnSelect — 2.576 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX          └─ IptWindowSelectEvents_OnSelect — 6.631 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             ├─ TryGetActivePartDocument — 0.009 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             ├─ Начальная подготовка OnSelect до первого GetComIdentityKey — 0.012 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             ├─ построить общий RangeBox по видимым телам — 0.003662 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  └─ вместе с GetComIdentityKey — 0.004041 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             ├─ получить тела-кандидаты через spatial cubes index — 0.035 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  ├─ SpatialCubesIndex.MatchesDocument — 0.000649 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  ├─ BoxLimits.ToSpatialBox — 0.000244 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  ├─ SpatialCubesIndex.Query — 0.001361 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  └─ результат индекса:
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │     ├─ Spatial cubes index: ON
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │     ├─ Задето кубиков selectionLimits рамки: 1
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │     ├─ Кубик, который вернул Query: CUBE_X0_Y1_Z1
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │     ├─ Кандидатов из кубиков: 37
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │     └─ Важно: это кубики, задетые текущей рамкой, а не полный список кубиков каждого найденного тела.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             ├─ пройти по телам-кандидатам из кубика — около 0.451 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  ├─ финальная проверка Intersects по кандидатам
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  │  └─ 37 вызовов Intersects — 0.123 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  │     ├─ среднее — 0.003324 с на тело-кандидат
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  │     ├─ минимум — 0.001680 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  │     └─ максимум — 0.013159 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │  └─ если тело прошло финальный Intersects:
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │     ├─ AddBodyToGroup
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │     │  └─ 2 тела — 0.080 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │     └─ AddFeaturesForBodyToGroup
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │        └─ 2 тела — 0.133 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │           ├─ GetFeaturesForBody — 0.086 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             │           └─ AddFeatureToGroup — 0.043 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             ├─ RefreshIptGroupList — 0.050 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             ├─ RefreshIptFeatureList — 0.061 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX             └─ MessageBox "Объекты добавлены в группу" — 6.111 с
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Результат выбора:
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Видимых тел, попавших в рамку Inventor: 1
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Проверено тел-кандидатов: 37
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Всего тел добавлено по RangeBox, включая внутренние/скрытые: 2
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Также выделено в UI Inventor: 2
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Spatial cubes index: ON
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Задето кубиков selectionLimits рамки: 1
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Кандидатов из задетого кубика: 37
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Кубики, которые реально вернул Query для рамки: CUBE_X0_Y1_Z1
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Уточнение по принадлежности тел к кубикам:
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Тело 11 лежит в 2 кубиках:
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX ├─ CUBE_X0_Y0_Z1
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX └─ CUBE_X0_Y1_Z1
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Но конкретный selectionLimits от рамки в логе 17:02 задел только:
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX └─ CUBE_X0_Y1_Z1
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Инженерный вывод:
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Новый режим НЕ перебирал все 72 SurfaceBodies на финальной проверке.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Spatial index сузил список с 72 тел до 37 тел-кандидатов из кубика, который задел selectionLimits: CUBE_X0_Y1_Z1.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Финальный Intersects выполнился 37 раз вместо 72.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Алгоритмическая часть OnSelect до MessageBox заняла около 0.519 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Основная видимая длительность этого конкретного запуска — пользовательские MessageBox: инструкция 10.856 с и итоговое окно 6.111 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX =====================================================================================================

        private void ButtonIptAddInsideBox_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptAddInsideBox_Click"))
            {
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / запуск рамочного выбора — 11.255 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Внутри: TryGetActivePartDocument — 0.032 с; MessageBox-инструкция — 10.856 с; TryAddSelectionFilter — 0.004 с.
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            if (_iptWindowSelectionRunning)
            {
                LoggedMessageBox.Show("Window selection is already running. Press Esc in Inventor to cancel it.");
                return;
            }

        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / MessageBox-инструкция выбора рамкой — 10.856 с в контрольном запуске.
            DialogResult result = LoggedMessageBox.Show(
                "Выберите ТЕЛА рамкой в окне Inventor.\r\n\r\n" +
                "1) Нажмите OK.\r\n" +
                "2) В Inventor зажмите ЛКМ.\r\n" +
                "3) Протяните рамку.\r\n" +
                "4) Отпустите ЛКМ.\r\n\r\n" +
                "После отпускания ЛКМ программа добавит в группу найденные тела, включая внутренние/скрытые по RangeBox.\r\n\r\n" +
                "Оптимизировано: выбираем только SurfaceBody, без граней.",
                "IPT window selection",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Information);

            if (result != DialogResult.OK)
            {
                return;
            }

            try
            {
                partDoc.SelectSet.Clear();

                _iptWindowSelectionWasHandled = false;
                _iptWindowSelectionRunning = true;
                _windowStateBeforeSelection = this.WindowState;

                // Создаём настоящую интерактивную команду выбора Inventor.
                _iptWindowInteractionEvents = _invApp.CommandManager.CreateInteractionEvents();
                _iptWindowInteractionEvents.InteractionDisabled = false;
                _iptWindowInteractionEvents.StatusBarText = "Выберите рамкой ТЕЛА в .ipt. Отпустите ЛКМ для завершения. Esc - отмена.";
                _iptWindowInteractionEvents.OnTerminate += IptWindowInteractionEvents_OnTerminate;

                _iptWindowSelectEvents = _iptWindowInteractionEvents.SelectEvents;

                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / TryAddSelectionFilter — 0.004 с.
                // Быстрый режим: выбираем только твёрдые/поверхностные тела.
                // НЕ добавлять здесь kPartFaceFilter: на сложных моделях он может вернуть тысячи граней.
                // Нам нужны только видимые выбранные тела для построения RangeBox, дальше все тела просматриваем сами.
                TryAddSelectionFilter(_iptWindowSelectEvents, SelectionFilterEnum.kPartBodyFilter);

                // Это ключевой флаг Inventor API, который включает выбор рамкой.
                _iptWindowSelectEvents.WindowSelectEnabled = true;

                // Не включаем одиночный выбор: рамка может вернуть несколько объектов.
                try
                {
                    _iptWindowSelectEvents.SingleSelectEnabled = false;
                }
                catch
                {
                    // В некоторых версиях Interop это может быть открыто иначе; главное здесь — WindowSelectEnabled.
                }

                // Эти обработчики намеренно не пустые: без OnSelect/OnPreSelect
                // некоторые версии Inventor ненадёжно подсвечивают/выбирают объекты во время InteractionEvents.
                _iptWindowSelectEvents.OnPreSelect += IptWindowSelectEvents_OnPreSelect;
                _iptWindowSelectEvents.OnSelect += IptWindowSelectEvents_OnSelect;

                _originalShowPromptTooltips = _invApp.GeneralOptions.ShowCommandPromptTooltips;
                _invApp.GeneralOptions.ShowCommandPromptTooltips = true;

                // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / прочая подготовка InteractionEvents и UI — около 0.363 с.
                // Уводим окно WinForms в сторону, чтобы оно не закрывало вид Inventor.
                this.WindowState = FormWindowState.Minimized;

                _iptWindowInteractionEvents.Start();
            }
            catch (Exception ex)
            {
                StopIptWindowSelection(true);
                LoggedMessageBox.Show("Problem starting IPT window selection");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        private void IptWindowSelectEvents_OnPreSelect(
            ref object PreSelectEntity,
            out bool DoHighlight,
            ref ObjectCollection MorePreSelectEntities,
            SelectionDeviceEnum SelectionDevice,
            Inventor.Point ModelPosition,
            Point2d ViewPosition,
            Inventor.View View)
        {
            using (AppLogger.Scope("IptWindowSelectEvents_OnPreSelect"))
            {
            // Быстрый режим: подсвечиваем/выбираем только тела, не грани.
            // Выбор граней может создать тысячи выбранных объектов и стать медленным на больших деталях.
            DoHighlight = PreSelectEntity is SurfaceBody;
        
            }}


        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS -----------------------------------------------------------------------------------------------------
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Детализация OnSelect для шага 1:
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS OnSelect начинается после того, как пользователь отпустил ЛКМ, а Inventor вызвал наше событие.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS В этом методе строится RangeBox выбранных видимых тел, затем проверяются все SurfaceBodies детали.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Контрольная стоимость всего метода: 2.141 с, из них финальный MessageBox занял 1.316 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS -----------------------------------------------------------------------------------------------------

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS INLINE MAP: метод OnSelect контрольной выборки занял 2.141 с, включая финальный MessageBox 1.316 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX INLINE MAP: новый OnSelect в режиме spatial cubes занял 6.631 с, включая финальный MessageBox 6.111 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Алгоритмическая часть OnSelect до MessageBox — около 0.519 с; финальный Intersects: 37 кандидатов вместо 72 SurfaceBodies.
        private void IptWindowSelectEvents_OnSelect(
            ObjectsEnumerator JustSelectedEntities,
            SelectionDeviceEnum SelectionDevice,
            Inventor.Point ModelPosition,
            Point2d ViewPosition,
            Inventor.View View)
        {
            using (AppLogger.Scope("IptWindowSelectEvents_OnSelect"))
            {
            if (!_iptWindowSelectionRunning)
            {
                return;
            }

            try
            {
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 1 / OnSelect: начальная подготовка после отпускания ЛКМ — 0.046 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Сюда входит вход в OnSelect и получение активного PartDocument.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / TryGetActivePartDocument внутри OnSelect — 0.009 с; подготовка до первого GetComIdentityKey — 0.012 с.
                if (!TryGetActivePartDocument(out PartDocument partDoc))
                {
                    StopIptWindowSelection(true);
                    return;
                }

                BoxLimits selectionLimits = new BoxLimits();
                HashSet<IntPtr> selectedBodyKeys = new HashSet<IntPtr>();
                int selectedBodyCount = 0;

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 1 / RangeBox: построить общий RangeBox по видимым телам — 0.003689 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 1 / RangeBox: вместе с GetComIdentityKey — 0.004106 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Inventor уже вернул видимые SurfaceBody из рамки; точного отдельного замера возврата нет.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / построить общий RangeBox по видимым телам — 0.003662 с; вместе с GetComIdentityKey — 0.004041 с.
                // Это только видимые/выбираемые ТЕЛА, которые вернул настоящий рамочный выбор Inventor.
                // Используем их только для определения 3D-области RangeBox.
                foreach (object obj in JustSelectedEntities)
                {
                    SurfaceBody selectedBody = obj as SurfaceBody;

                    if (selectedBody == null)
                    {
                        continue;
                    }

                    IntPtr key = GetComIdentityKey(selectedBody);

                    if (key != IntPtr.Zero && selectedBodyKeys.Contains(key))
                    {
                        continue;
                    }

                    if (key != IntPtr.Zero)
                    {
                        selectedBodyKeys.Add(key);
                    }

                    selectionLimits.Include(selectedBody.RangeBox);
                    selectedBodyCount++;
                }

                if (!selectionLimits.HasValue)
                {
                    _iptWindowSelectionWasHandled = true;
                    StopIptWindowSelection(true);
                    LoggedMessageBox.Show("Рамка не захватила ни одного тела. Попробуйте захватить хотя бы одно видимое тело.");
                    return;
                }

                partDoc.SelectSet.Clear();

                int addedCount = 0;
                int selectedInInventorCount = 0;

        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX ЭТАП 1 / выбор кандидатов для проверки.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Если spatial cubes index ещё не построен кнопкой, работаем по-старому: полный foreach SurfaceBodies.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Если индекс построен, сначала берём только тела из кубиков, которые пересекла selectionLimits.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / получить тела-кандидаты через spatial cubes index — 0.035 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX В контрольном запуске Query для selectionLimits вернул 1 задетый кубик CUBE_X0_Y1_Z1, кандидатов — 37.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX При этом отдельное тело может числиться в нескольких кубиках: например, Тело 11 лежит в CUBE_X0_Y0_Z1 и CUBE_X0_Y1_Z1.
                SpatialQueryResult spatialQuery = null;
                List<SurfaceBody> bodiesToCheck = GetBodiesForIptSelection(partDoc, selectionLimits, out spatialQuery);
                int bodiesCheckedCount = 0;

        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / пройти по телам-кандидатам из кубика — около 0.451 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Финальная проверка Intersects выполняется только по bodiesToCheck, а не по всем 72 SurfaceBodies.
                foreach (SurfaceBody body in bodiesToCheck)
                {
                    if (body == null)
                    {
                        continue;
                    }

                    bodiesCheckedCount++;

                    // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX ВАЖНО: body.RangeBox теперь вынесен отдельно от Intersects.
                    // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Так в следующих логах можно будет честно разделить: получение RangeBox от Inventor и математический тест коробок.
                    Box bodyRangeBox = null;

                    try
                    {
                        bodyRangeBox = body.RangeBox;
                    }
                    catch
                    {
                        bodyRangeBox = null;
                    }

                    if (bodyRangeBox == null)
                    {
                        continue;
                    }

                    // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / финальный Intersects по кандидатам: 37 вызовов — 0.123 с.
                    // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Среднее — 0.003324 с; минимум — 0.001680 с; максимум — 0.013159 с.
                    // Режим пересекающей рамки: включается каждое тело, чей RangeBox пересекает выбранную область.
                    // Если нужны только тела полностью внутри области, замените Intersects на Contains.
                    if (!selectionLimits.Intersects(bodyRangeBox, 0.001))
                    {
                        continue;
                    }

                    // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / найденное тело: AddBodyToGroup для 2 тел — 0.080 с.
                    // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / найденное тело: AddFeaturesForBodyToGroup для 2 тел — 0.133 с.
                    // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX Внутри AddFeatures: GetFeaturesForBody — 0.086 с; AddFeatureToGroup — 0.043 с.
                    AddBodyToGroup(body);
                    AddFeaturesForBodyToGroup(body);
                    addedCount++;

                    try
                    {
                        partDoc.SelectSet.Select(body);
                        selectedInInventorCount++;
                    }
                    catch
                    {
                        // Невидимые/специальные тела можно пометить атрибутами, но они могут не подсветиться в UI Inventor.
                    }
                }

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 1 / обновление UI-списков: RefreshIptGroupList — 0.063 с; RefreshIptFeatureList — 0.056 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / обновление UI-списков: RefreshIptGroupList — 0.050 с; RefreshIptFeatureList — 0.061 с.
                // Здесь не нужен partDoc.Update(): добавление атрибутов не требует перестроения модели, а Update может быть медленным.
                RefreshIptGroupList(partDoc);
                RefreshIptFeatureList(partDoc);

                _iptWindowSelectionWasHandled = true;
                StopIptWindowSelection(true);

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 1 / пользовательская обратная связь: MessageBox "Объекты добавлены в группу" — 1.316 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX NEW MODE / пользовательская обратная связь: MessageBox "Объекты добавлены в группу" — 6.111 с.
        // 17:02 14.06.2026 InventorIptOrg_v0_4_7_SPATIAL_CUBES_COMPILE_FIX В итоговом окне фиксируем: index ON, 1 кубик, 37 кандидатов, 2 добавленных тела.
                LoggedMessageBox.Show(
                    "Объекты добавлены в группу.\r\n\r\n" +
                    "Видимых тел, попавших в рамку Inventor: " + selectedBodyCount.ToString() + "\r\n" +
                    "Проверено тел-кандидатов: " + bodiesCheckedCount.ToString() + "\r\n" +
                    "Всего тел добавлено по RangeBox, включая внутренние/скрытые: " + addedCount.ToString() + "\r\n" +
                    "Также выделено в UI Inventor: " + selectedInInventorCount.ToString() + "\r\n\r\n" +
                    BuildSpatialQueryInfoText(spatialQuery));
            }
            catch (Exception ex)
            {
                _iptWindowSelectionWasHandled = true;
                StopIptWindowSelection(true);
                LoggedMessageBox.Show("Problem processing IPT window selection");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        private void IptWindowInteractionEvents_OnTerminate()
        {
            using (AppLogger.Scope("IptWindowInteractionEvents_OnTerminate"))
            {
            bool showCancelMessage = _iptWindowSelectionRunning && !_iptWindowSelectionWasHandled;

            CleanupIptWindowSelection(true);

            if (showCancelMessage)
            {
                LoggedMessageBox.Show("Выделение рамкой отменено.");
            }
        
            }}

        private void StopIptWindowSelection(bool restoreForm)
        {
            using (AppLogger.Scope("StopIptWindowSelection"))
            {
            if (_iptWindowInteractionEvents != null)
            {
                try
                {
                    _iptWindowInteractionEvents.Stop();
                }
                catch
                {
                    CleanupIptWindowSelection(restoreForm);
                }
            }
            else
            {
                CleanupIptWindowSelection(restoreForm);
            }
        
            }}

        private void CleanupIptWindowSelection(bool restoreForm)
        {
            using (AppLogger.Scope("CleanupIptWindowSelection"))
            {
            try
            {
                if (_invApp != null)
                {
                    _invApp.GeneralOptions.ShowCommandPromptTooltips = _originalShowPromptTooltips;
                }
            }
            catch
            {
            }

            if (_iptWindowSelectEvents != null)
            {
                try { _iptWindowSelectEvents.OnPreSelect -= IptWindowSelectEvents_OnPreSelect; } catch { }
                try { _iptWindowSelectEvents.OnSelect -= IptWindowSelectEvents_OnSelect; } catch { }
            }

            if (_iptWindowInteractionEvents != null)
            {
                try { _iptWindowInteractionEvents.OnTerminate -= IptWindowInteractionEvents_OnTerminate; } catch { }
            }

            _iptWindowSelectEvents = null;
            _iptWindowInteractionEvents = null;
            _iptWindowSelectionRunning = false;

            if (restoreForm)
            {
                RunOnUiThread(RestoreFormAfterSelection);
            }
        
            }}

        private void RestoreFormAfterSelection()
        {
            using (AppLogger.Scope("RestoreFormAfterSelection"))
            {
            if (this.IsDisposed)
            {
                return;
            }

            try
            {
                this.WindowState = _windowStateBeforeSelection;
                this.Activate();
            }
            catch
            {
            }
        
            }}

        private void RunOnUiThread(Action action)
        {
            using (AppLogger.Scope("RunOnUiThread"))
            {
            if (action == null || this.IsDisposed)
            {
                return;
            }

            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(action);
                    return;
                }

                action();
            }
            catch
            {
                // Форма может закрываться в момент, когда COM-события Inventor ещё завершаются.
            }
        
            }}

        private void ButtonIptToggle_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptToggle_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            try
            {
                // 19:10 15.06.2026 InventorIptOrg_v0_4_18_CUSTOM_RECT_FAST_HIDE_READY
                // Fast hide-ready path: if the visible IPT body list contains BodyListItem objects,
                // toggle these exact SurfaceBody references directly. This makes v0.4.17-style
                // list-only custom rectangle results hideable without writing myBodyGroup Attributes.
                int count = ToggleVisibleIptBodyListDirect(partDoc);
                if (count > 0)
                {
                    LoggedMessageBox.Show("IPT bodies toggled from current visible list: " + count.ToString());
                    return;
                }

                // Fallback: historical attribute-based group toggle for bodies that were really added
                // through AddBodyToGroup / myBodyGroup Attributes.
                AttributeManager attbMan = partDoc.AttributeManager;

                ObjectCollection objsCol = attbMan.FindObjects(
                    "myBodyGroup",
                    "BodyGroup1",
                    "Group1");

                foreach (object obj in objsCol)
                {
                    SurfaceBody body = obj as SurfaceBody;

                    if (body == null)
                    {
                        continue;
                    }

                    body.Visible = !body.Visible;
                    count++;
                }

                partDoc.Update();
                RefreshIptGroupList(partDoc);
                RefreshIptFeatureList(partDoc);

                LoggedMessageBox.Show("IPT bodies toggled: " + count.ToString());
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem hiding/showing IPT bodies");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        private int ToggleVisibleIptBodyListDirect(PartDocument partDoc)
        {
            using (AppLogger.Scope("ToggleVisibleIptBodyListDirect"))
            {
            if (partDoc == null || listBoxIptBodies == null || listBoxIptBodies.Items.Count == 0)
            {
                return 0;
            }

            int count = 0;
            int skippedNull = 0;
            HashSet<IntPtr> toggledKeys = new HashSet<IntPtr>();

            foreach (object obj in listBoxIptBodies.Items)
            {
                BodyListItem item = obj as BodyListItem;
                if (item == null || item.Body == null)
                {
                    skippedNull++;
                    continue;
                }

                IntPtr key = item.IdentityKey;
                if (key == IntPtr.Zero)
                {
                    key = GetComIdentityKey(item.Body);
                }

                if (key != IntPtr.Zero)
                {
                    if (toggledKeys.Contains(key))
                    {
                        continue;
                    }
                    toggledKeys.Add(key);
                }

                item.Body.Visible = !item.Body.Visible;
                count++;
            }

            if (count > 0)
            {
                partDoc.Update();
            }

            AppLogger.Log(
                "FAST_HIDE_READY_TOGGLE_VISIBLE_LIST",
                "ToggleVisibleIptBodyListDirect",
                "VisibleListItems=" + listBoxIptBodies.Items.Count.ToString() +
                "; ToggledBodies=" + count.ToString() +
                "; SkippedNullItems=" + skippedNull.ToString() +
                "; AttributesSkipped=True; FeaturesSkipped=True; RefreshListsSkipped=True");

            return count;
            }}

        private bool TryGetActivePartDocument(out PartDocument partDoc)
        {
            using (AppLogger.Scope("TryGetActivePartDocument"))
            {
            partDoc = null;

            if (_invApp == null)
            {
                LoggedMessageBox.Show("Unable to get or start Inventor");
                return false;
            }

            if (_invApp.Documents.Count == 0)
            {
                LoggedMessageBox.Show("Need to open a Part document");
                return false;
            }

            if (_invApp.ActiveDocument.DocumentType != DocumentTypeEnum.kPartDocumentObject)
            {
                LoggedMessageBox.Show("Need to have a Part document active (.ipt)");
                return false;
            }

            partDoc = (PartDocument)_invApp.ActiveDocument;
            return true;
        
            }}

        private bool TryGetActiveAssemblyDocument(out AssemblyDocument asmDoc)
        {
            using (AppLogger.Scope("TryGetActiveAssemblyDocument"))
            {
            asmDoc = null;

            if (_invApp == null)
            {
                LoggedMessageBox.Show("Unable to get or start Inventor");
                return false;
            }

            if (_invApp.Documents.Count == 0)
            {
                LoggedMessageBox.Show("Need to open an Assembly document");
                return false;
            }

            if (_invApp.ActiveDocument.DocumentType != DocumentTypeEnum.kAssemblyDocumentObject)
            {
                LoggedMessageBox.Show("Need to have an Assembly document active");
                return false;
            }

            asmDoc = (AssemblyDocument)_invApp.ActiveDocument;
            return true;
        
            }}

        private static void TryAddSelectionFilter(SelectEvents selectEvents, SelectionFilterEnum filter)
        {
            using (AppLogger.Scope("TryAddSelectionFilter"))
            {
            try
            {
                selectEvents.AddSelectionFilter(filter);
            }
            catch
            {
                // Продолжаем работу. Разные версии Inventor/Interop могут по-разному вести себя с некоторыми фильтрами.
            }
        
            }}

        private static bool AddBodyToGroup(SurfaceBody body)
        {
            using (AppLogger.Scope("AddBodyToGroup"))
            {
            AttributeSets attbSets = body.AttributeSets;

            if (AttributeSetNameIsUsed(attbSets, "myBodyGroup"))
            {
                return false;
            }

            AttributeSet attbSet = attbSets.Add("myBodyGroup");

            Inventor.Attribute attb = attbSet.Add(
                "BodyGroup1",
                ValueTypeEnum.kStringType,
                "Group1");

            return true;
        
            }}

        private static bool RemoveBodyFromGroup(SurfaceBody body)
        {
            using (AppLogger.Scope("RemoveBodyFromGroup"))
            {
            if (body == null)
            {
                return false;
            }

            AttributeSets attbSets = body.AttributeSets;

            if (!AttributeSetNameIsUsed(attbSets, "myBodyGroup"))
            {
                return false;
            }

            try
            {
                dynamic sets = attbSets;
                AttributeSet attbSet = sets.Item("myBodyGroup");
                attbSet.Delete();
                return true;
            }
            catch
            {
                try
                {
                    // Запасной вариант для версий Interop, где Item открыт как индексатор.
                    dynamic sets = attbSets;
                    AttributeSet attbSet = sets["myBodyGroup"];
                    attbSet.Delete();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        
            }}

        private void RefreshIptGroupList(PartDocument partDoc)
        {
            using (AppLogger.Scope("RefreshIptGroupList"))
            {
            if (partDoc == null || this.IsDisposed)
            {
                return;
            }

            // Inventor InteractionEvents может вызвать OnSelect из COM-потока, который не является
            // UI-потоком WinForms. Поэтому все изменения ListBox/Label нужно передавать
            // обратно в поток формы, иначе WinForms выдаст InvalidOperationException.
            if (this.InvokeRequired)
            {
                RunOnUiThread(delegate { RefreshIptGroupList(partDoc); });
                return;
            }

            bool beginUpdateStarted = false;

            try
            {
                listBoxIptBodies.BeginUpdate();
                beginUpdateStarted = true;
                listBoxIptBodies.Items.Clear();

                AttributeManager attbMan = partDoc.AttributeManager;
                ObjectCollection objsCol = attbMan.FindObjects(
                    "myBodyGroup",
                    "BodyGroup1",
                    "Group1");

                HashSet<IntPtr> bodyKeys = new HashSet<IntPtr>();
                int index = 1;

                foreach (object obj in objsCol)
                {
                    SurfaceBody body = obj as SurfaceBody;

                    if (body == null)
                    {
                        continue;
                    }

                    IntPtr key = GetComIdentityKey(body);

                    if (key != IntPtr.Zero && bodyKeys.Contains(key))
                    {
                        continue;
                    }

                    if (key != IntPtr.Zero)
                    {
                        bodyKeys.Add(key);
                    }

                    listBoxIptBodies.Items.Add(new BodyListItem(body, GetBodyDisplayName(body, index), key));
                    index++;
                }
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem refreshing IPT body list");
                LoggedMessageBox.Show(ex.ToString());
            }
            finally
            {
                if (beginUpdateStarted)
                {
                    listBoxIptBodies.EndUpdate();
                }

                UpdateIptGroupListCaption();
            }
        
            }}

        private void UpdateIptGroupListCaption()
        {
            using (AppLogger.Scope("UpdateIptGroupListCaption"))
            {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                RunOnUiThread(UpdateIptGroupListCaption);
                return;
            }

            labelIptGroupList.Text = "Bodies in current group / список тел в группе: " + listBoxIptBodies.Items.Count.ToString();
        
            }}

        private static string GetBodyDisplayName(SurfaceBody body, int index)
        {
            using (AppLogger.Scope("GetBodyDisplayName"))
            {
            string name = null;

            try
            {
                dynamic dynBody = body;
                name = dynBody.Name as string;
            }
            catch
            {
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "SurfaceBody";
            }

            string visibilityText = string.Empty;

            try
            {
                visibilityText = body.Visible ? "" : " [hidden]";
            }
            catch
            {
            }

            return index.ToString("000") + " - " + name + visibilityText;
        
            }}

        private void ButtonIptClearList_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptClearList_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            DialogResult result = LoggedMessageBox.Show(
                "Очистить списки и удалить группы myBodyGroup / myFeatureGroup из текущей .ipt?",
                "Clear IPT lists and groups",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (result != DialogResult.OK)
            {
                return;
            }

            try
            {
                int removedBodyCount = ClearGroupFromPartDocument(partDoc, "myBodyGroup", "BodyGroup1", "Group1");
                int removedFeatureCount = ClearGroupFromPartDocument(partDoc, "myFeatureGroup", "FeatureGroup1", "Group1");

                listBoxIptBodies.Items.Clear();
                listBoxIptFeatures.Items.Clear();
                UpdateIptGroupListCaption();
                UpdateIptFeatureListCaption();

                LoggedMessageBox.Show(
                    "Списки очищены.\r\n\r\n" +
                    "Удалено из группы тел: " + removedBodyCount.ToString() + "\r\n" +
                    "Удалено из группы элементов: " + removedFeatureCount.ToString());
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem clearing IPT lists");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        private void ButtonIptCopyList_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptCopyList_Click"))
            {
            if (listBoxIptBodies.Items.Count == 0)
            {
                LoggedMessageBox.Show("Список пустой.");
                return;
            }

            StringBuilder sb = new StringBuilder();

            foreach (object obj in listBoxIptBodies.Items)
            {
                sb.AppendLine(obj.ToString());
            }

            Clipboard.SetText(sb.ToString());
            LoggedMessageBox.Show("Список скопирован в буфер обмена: " + listBoxIptBodies.Items.Count.ToString());
        
            }}

        private void ButtonIptCreateBodyFolder_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptCreateBodyFolder_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            if (listBoxIptBodies.Items.Count == 0)
            {
                LoggedMessageBox.Show("Список тел пустой. Сначала добавьте тела рамкой или обычным выделением.");
                return;
            }

            try
            {
                List<object> objectsForFolder = new List<object>();

                foreach (object obj in listBoxIptBodies.Items)
                {
                    BodyListItem item = obj as BodyListItem;

                    if (item != null && item.Body != null)
                    {
                        objectsForFolder.Add(item.Body);
                    }
                }

                CreateBrowserFolderFromObjects(
                    partDoc,
                    objectsForFolder,
                    "Selected_Bodies",
                    "тел");
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem creating browser folder from IPT body list");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        private void ButtonIptCopyFeatureList_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptCopyFeatureList_Click"))
            {
            if (listBoxIptFeatures.Items.Count == 0)
            {
                LoggedMessageBox.Show("Список элементов пустой.");
                return;
            }

            StringBuilder sb = new StringBuilder();

            foreach (object obj in listBoxIptFeatures.Items)
            {
                sb.AppendLine(obj.ToString());
            }

            Clipboard.SetText(sb.ToString());
            LoggedMessageBox.Show("Список элементов скопирован в буфер обмена: " + listBoxIptFeatures.Items.Count.ToString());
        
            }}


        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS =====================================================================================================
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS 2. Create feature browser folder
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS -----------------------------------------------------------------------------------------------------
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Create feature browser folder — полный цикл: 37.900 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS └─ ButtonIptCreateFeatureFolder_Click — 37.900 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    └─ TryGetActivePartDocument — 0.032 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS       └─ CreateBrowserFolderFromObjects — 37.862 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          ├─ получить Model BrowserPane — 0.709 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          │  ├─ TryGetBrowserPaneByName — 0.006 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          │  └─ IsUsableBrowserPane — 0.008 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          ├─ поиск BrowserNode для объектов из списка — 0.023 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          │  ├─ TryGetBrowserNodeFromObject #1 — 0.012846 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          │  ├─ GetComIdentityKey #1 — 0.000184 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          │  ├─ TryGetBrowserNodeFromObject #2 — 0.010381 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          │  └─ GetComIdentityKey #2 — 0.000213 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          ├─ создание папки в дереве Inventor — точного отдельного замера нет
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          │  └─ оценочно вместе с подготовкой сообщения — около 0.264 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          ├─ MessageBox "Папка создана" — 2.885 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS          └─ RefreshIptBrowserTree — 33.943 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS             └─ BuildBrowserTreeGridItems / обход дерева Inventor — почти всё это время
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Инженерный вывод:
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS создание папки и поиск BrowserNode быстрые.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Основная стоимость — последующее полное обновление дерева Inventor через RefreshIptBrowserTree.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS =====================================================================================================

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS INLINE MAP: Create feature browser folder — полный цикл 37.900 с.
        private void ButtonIptCreateFeatureFolder_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptCreateFeatureFolder_Click"))
            {
        // 19:05 16.06.2026 InventorIptOrg_v0_4_31_FEATURE_BROWSER_FOLDER_REANIMATE ЭТАП 2 / подготовка:
        // Если старый Features-list пустой после .ipt cubes fast visible-list, то собираем элементы on-demand из текущего списка тел.
        // Это закрывает проблему v0.4.30: кнопка Create feature browser folder не могла работать, потому что listBoxIptFeatures был пуст.
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            try
            {
                if (listBoxIptFeatures.Items.Count == 0)
                {
                    int builtCount = BuildFeatureListFromCurrentVisibleBodiesFast(partDoc, false);

                    if (builtCount == 0)
                    {
                        LoggedMessageBox.Show(
                            "Список элементов пустой, и из текущего списка тел не удалось получить связанные Features.\r\n\r\n" +
                            "Сначала выберите тела рамкой во вкладке .ipt cubes или обычной рамкой, чтобы список тел был не пустой.");
                        return;
                    }
                }

                List<object> objectsForFolder = new List<object>();

                foreach (object obj in listBoxIptFeatures.Items)
                {
                    FeatureListItem item = obj as FeatureListItem;

                    if (item != null && item.Feature != null)
                    {
                        objectsForFolder.Add(item.Feature);
                    }
                }

                AppLogger.Log(
                    "FEATURE_BROWSER_FOLDER_FAST_CREATE_REQUEST",
                    "ButtonIptCreateFeatureFolder_Click",
                    "FeatureItems=" + listBoxIptFeatures.Items.Count.ToString() +
                    "; ObjectsForFolder=" + objectsForFolder.Count.ToString() +
                    "; Source=CurrentFeatureListOrReanimatedVisibleBodies; RefreshBrowserTreeAfterCreate=False" +
                    "; CreateNestedInnerFolderWithItems=True");

        // 19:05 16.06.2026 InventorIptOrg_v0_4_31_FEATURE_BROWSER_FOLDER_REANIMATE ЭТАП 2 / ускорение:
        // Исторически CreateBrowserFolderFromObjects тратил большую часть времени на RefreshIptBrowserTree после создания папки.
        // Для закрытия пункта Create feature browser folder создаём папку и НЕ делаем полный обход дерева автоматически.
                CreateBrowserFolderFromObjects(
                    partDoc,
                    objectsForFolder,
                    "Selected_Features",
                    "элементов",
                    false,
                    true);
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem creating browser folder from IPT feature list");
                LoggedMessageBox.Show(ex.ToString());
            }

            }}

        private void ButtonIptCreateBrowserSubfolder_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptCreateBrowserSubfolder_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            BrowserPane modelPane = GetModelBrowserPane(partDoc);
            if (modelPane == null)
            {
                LoggedMessageBox.Show(
                    "Не удалось получить Model BrowserPane текущей детали.\r\n\r\n" +
                    "Список BrowserPanes, который вернул Inventor:\r\n" +
                    GetBrowserPaneDebugList(partDoc));
                return;
            }

            try
            {
                string subfolderNodeSource = "InventorBrowserSelectionOrInternalBrowserTreeGrid";
                List<BrowserNode> selectedNodes = GetBrowserNodesForSubfolderCreation(partDoc, modelPane);
                selectedNodes = NormalizeBrowserNodeListForSubfolder(selectedNodes);

                if (selectedNodes.Count == 0)
                {
                    string featureListSource;
                    List<BrowserNode> featureListNodes = GetBrowserNodesForSubfolderFromFeatureList(modelPane, out featureListSource);
                    selectedNodes = NormalizeBrowserNodeListForSubfolder(featureListNodes);
                    subfolderNodeSource = featureListSource;
                }

                if (selectedNodes.Count == 0)
                {
                    LoggedMessageBox.Show(
                        "Не удалось получить BrowserNode для подпапки.\r\n\r\n" +
                        "Как использовать теперь:\r\n" +
                        "1) Основной вариант: выделите строки в списке Features / элементы и нажмите эту кнопку.\r\n" +
                        "2) Если строки Features не выделены, кнопка попробует взять весь текущий Features-list.\r\n" +
                        "3) Дополнительно можно нажать Refresh browser tree и выделить строки во внутренней таблице Browser tree.\r\n\r\n" +
                        "Примечание: Inventor не всегда отдаёт выделение из левого дерева Model Browser через COM, поэтому добавлен fallback через текущий Features-list.");
                    return;
                }

                ObjectCollection browserNodes = _invApp.TransientObjects.CreateObjectCollection();
                foreach (BrowserNode node in selectedNodes)
                {
                    browserNodes.Add(node);
                }

                string folderName = "Sub_Features_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                BrowserFolder folder = null;

                try
                {
                    folder = modelPane.AddBrowserFolder(folderName, browserNodes);
                }
                catch
                {
                    try
                    {
                        dynamic dynPane = modelPane;
                        folder = (BrowserFolder)dynPane.AddBrowserFolder(folderName, browserNodes);
                    }
                    catch (Exception ex)
                    {
                        LoggedMessageBox.Show(
                            "Inventor не смог создать подпапку Browser Folder из выбранных узлов.\r\n\r\n" +
                            "Проверьте, что выбраны внутренние элементы/features одной существующей папки, а не сама папка.\r\n\r\n" +
                            ex.ToString());
                        return;
                    }
                }

                try
                {
                    dynamic dynPaneUpdate = modelPane;
                    dynPaneUpdate.Update();
                }
                catch
                {
                }

                AppLogger.Log(
                    "BROWSER_SUBFOLDER_CREATED_FROM_SELECTED_NODES",
                    "ButtonIptCreateBrowserSubfolder_Click",
                    "FolderName=" + folderName +
                    "; SelectedBrowserNodes=" + selectedNodes.Count.ToString() +
                    "; BrowserNodesInCollection=" + browserNodes.Count.ToString() +
                    "; RefreshBrowserTreeAfterCreate=False" +
                    "; Source=" + subfolderNodeSource);

                LoggedMessageBox.Show(
                    "Подпапка создана в дереве Inventor.\r\n\r\n" +
                    "Имя подпапки: " + folderName + "\r\n" +
                    "Выбранных BrowserNode: " + selectedNodes.Count.ToString() + "\r\n" +
                    "Источник узлов: " + subfolderNodeSource + "\r\n" +
                    "Полный Refresh browser tree после создания: SKIPPED for speed");

                UpdateIptBrowserTreeCaption();
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem creating browser subfolder from selected nodes");
                LoggedMessageBox.Show(ex.ToString());
            }

            }}

        private List<BrowserNode> GetBrowserNodesForSubfolderCreation(PartDocument partDoc, BrowserPane modelPane)
        {
            using (AppLogger.Scope("GetBrowserNodesForSubfolderCreation"))
            {
            List<BrowserNode> nodes = new List<BrowserNode>();

            AddBrowserNodesFromInternalGridSelection(nodes);

            if (nodes.Count == 0)
            {
                AddBrowserNodesFromInventorSelectSet(partDoc, modelPane, nodes);
            }

            if (nodes.Count == 0)
            {
                AddBrowserNodesFromBrowserPaneSelection(modelPane, nodes);
            }

            if (nodes.Count == 0 && modelPane != null)
            {
                try
                {
                    NodeCounter counter = new NodeCounter(5000);
                    AddSelectedBrowserNodesByTreeScan(modelPane.TopNode, nodes, counter, 0);
                }
                catch
                {
                }
            }

            AppLogger.Log(
                "BROWSER_SUBFOLDER_SELECTED_NODE_SOURCE_SCAN",
                "GetBrowserNodesForSubfolderCreation",
                "FoundNodes=" + nodes.Count.ToString() +
                "; GridSelectedRows=" + (dataGridViewIptBrowserTree == null ? 0 : dataGridViewIptBrowserTree.SelectedRows.Count).ToString());

            return nodes;

            }}

        private List<BrowserNode> GetBrowserNodesForSubfolderFromFeatureList(BrowserPane modelPane, out string featureListSource)
        {
            using (AppLogger.Scope("GetBrowserNodesForSubfolderFromFeatureList"))
            {
            featureListSource = "FeatureListFallbackNone";
            List<BrowserNode> nodes = new List<BrowserNode>();

            if (modelPane == null || listBoxIptFeatures == null || listBoxIptFeatures.Items.Count == 0)
            {
                AppLogger.Log(
                    "BROWSER_SUBFOLDER_FEATURELIST_FALLBACK_EMPTY",
                    "GetBrowserNodesForSubfolderFromFeatureList",
                    "Reason=NoModelPaneOrEmptyFeatureList");
                return nodes;
            }

            List<object> sourceItems = new List<object>();
            bool usedSelectedItems = false;

            try
            {
                if (listBoxIptFeatures.SelectedItems != null && listBoxIptFeatures.SelectedItems.Count > 0)
                {
                    foreach (object selectedItem in listBoxIptFeatures.SelectedItems)
                    {
                        sourceItems.Add(selectedItem);
                    }

                    usedSelectedItems = true;
                    featureListSource = "SelectedFeaturesListItems";
                }
            }
            catch
            {
            }

            if (sourceItems.Count == 0)
            {
                foreach (object item in listBoxIptFeatures.Items)
                {
                    sourceItems.Add(item);
                }

                featureListSource = "AllCurrentFeaturesListItems";
            }

            int featureItems = 0;
            int directBrowserNodes = 0;
            int resolvedBrowserNodes = 0;
            int unresolvedFeatures = 0;

            foreach (object obj in sourceItems)
            {
                FeatureListItem featureItem = obj as FeatureListItem;
                object featureObject = featureItem == null ? obj : featureItem.Feature;

                if (featureObject == null)
                {
                    unresolvedFeatures++;
                    continue;
                }

                featureItems++;

                BrowserNode directNode = featureObject as BrowserNode;
                if (directNode != null)
                {
                    nodes.Add(directNode);
                    directBrowserNodes++;
                    continue;
                }

                BrowserNode node;
                if (TryGetBrowserNodeFromObject(modelPane, featureObject, out node) && node != null)
                {
                    nodes.Add(node);
                    resolvedBrowserNodes++;
                }
                else
                {
                    AddBrowserNodeCandidate(modelPane, featureObject, nodes);
                    if (nodes.Count > directBrowserNodes + resolvedBrowserNodes)
                    {
                        resolvedBrowserNodes++;
                    }
                    else
                    {
                        unresolvedFeatures++;
                    }
                }
            }

            nodes = NormalizeBrowserNodeListForSubfolder(nodes);

            AppLogger.Log(
                "BROWSER_SUBFOLDER_FEATURELIST_FALLBACK_SCAN",
                "GetBrowserNodesForSubfolderFromFeatureList",
                "FeatureListSource=" + featureListSource +
                "; UsedSelectedItems=" + usedSelectedItems.ToString() +
                "; FeatureItemsChecked=" + featureItems.ToString() +
                "; DirectBrowserNodes=" + directBrowserNodes.ToString() +
                "; ResolvedBrowserNodes=" + resolvedBrowserNodes.ToString() +
                "; UnresolvedFeatures=" + unresolvedFeatures.ToString() +
                "; ResultBrowserNodes=" + nodes.Count.ToString());

            return nodes;
            }}

        private void AddBrowserNodesFromInternalGridSelection(List<BrowserNode> nodes)
        {
            if (nodes == null || dataGridViewIptBrowserTree == null || dataGridViewIptBrowserTree.SelectedRows.Count == 0)
            {
                return;
            }

            foreach (DataGridViewRow row in dataGridViewIptBrowserTree.SelectedRows)
            {
                BrowserTreeGridItem item = row == null ? null : row.Tag as BrowserTreeGridItem;
                if (item != null && item.Node != null)
                {
                    nodes.Add(item.Node);
                }
            }
        }

        private void AddBrowserNodesFromInventorSelectSet(PartDocument partDoc, BrowserPane modelPane, List<BrowserNode> nodes)
        {
            if (partDoc == null || modelPane == null || nodes == null)
            {
                return;
            }

            try
            {
                SelectSet selectSet = partDoc.SelectSet;
                if (selectSet == null || selectSet.Count == 0)
                {
                    return;
                }

                foreach (object selectedObject in selectSet)
                {
                    AddBrowserNodeCandidate(modelPane, selectedObject, nodes);
                }
            }
            catch
            {
            }
        }

        private void AddBrowserNodesFromBrowserPaneSelection(BrowserPane modelPane, List<BrowserNode> nodes)
        {
            if (modelPane == null || nodes == null)
            {
                return;
            }

            string[] selectionPropertyNames = new string[]
            {
                "SelectedBrowserNodes",
                "SelectedNodes",
                "SelectedNode",
                "Selection",
                "Selected"
            };

            foreach (string propertyName in selectionPropertyNames)
            {
                object selectionObject;
                if (TryGetLateBoundProperty(modelPane, propertyName, out selectionObject) && selectionObject != null)
                {
                    AddBrowserNodeCandidate(modelPane, selectionObject, nodes);
                    if (nodes.Count > 0)
                    {
                        return;
                    }
                }
            }
        }

        private void AddSelectedBrowserNodesByTreeScan(BrowserNode node, List<BrowserNode> nodes, NodeCounter counter, int depth)
        {
            if (node == null || nodes == null || counter == null || counter.Count >= counter.MaxCount || depth > 30)
            {
                return;
            }

            counter.Count++;

            object selectedValue;
            if (TryGetLateBoundProperty(node, "Selected", out selectedValue))
            {
                try
                {
                    if (Convert.ToBoolean(selectedValue))
                    {
                        nodes.Add(node);
                    }
                }
                catch
                {
                }
            }

            try
            {
                dynamic dynNode = node;
                dynamic children = dynNode.BrowserNodes;
                int childCount = (int)children.Count;

                for (int i = 1; i <= childCount; i++)
                {
                    BrowserNode child = null;
                    try
                    {
                        child = (BrowserNode)children.Item(i);
                    }
                    catch
                    {
                        try
                        {
                            child = (BrowserNode)children[i];
                        }
                        catch
                        {
                            child = null;
                        }
                    }

                    if (child != null)
                    {
                        AddSelectedBrowserNodesByTreeScan(child, nodes, counter, depth + 1);
                    }

                    if (counter.Count >= counter.MaxCount)
                    {
                        break;
                    }
                }
            }
            catch
            {
            }
        }

        private void AddBrowserNodeCandidate(BrowserPane modelPane, object candidate, List<BrowserNode> nodes)
        {
            if (candidate == null || nodes == null)
            {
                return;
            }

            BrowserNode directNode = candidate as BrowserNode;
            if (directNode != null)
            {
                nodes.Add(directNode);
                return;
            }

            if (TryAddBrowserNodesFromObjectCollection(modelPane, candidate, nodes))
            {
                return;
            }

            BrowserNode node;
            if (modelPane != null && TryGetBrowserNodeFromObject(modelPane, candidate, out node) && node != null)
            {
                nodes.Add(node);
                return;
            }
        }

        private bool TryAddBrowserNodesFromObjectCollection(BrowserPane modelPane, object collectionCandidate, List<BrowserNode> nodes)
        {
            if (collectionCandidate == null || nodes == null)
            {
                return false;
            }

            object countValue;
            if (!TryGetLateBoundProperty(collectionCandidate, "Count", out countValue))
            {
                return false;
            }

            int count;
            try
            {
                count = Convert.ToInt32(countValue);
            }
            catch
            {
                return false;
            }

            bool addedAny = false;
            for (int i = 1; i <= count; i++)
            {
                object item = null;
                try
                {
                    dynamic dynCollection = collectionCandidate;
                    item = dynCollection.Item(i);
                }
                catch
                {
                    try
                    {
                        dynamic dynCollection = collectionCandidate;
                        item = dynCollection[i];
                    }
                    catch
                    {
                        item = null;
                    }
                }

                if (item != null)
                {
                    int before = nodes.Count;
                    AddBrowserNodeCandidate(modelPane, item, nodes);
                    if (nodes.Count > before)
                    {
                        addedAny = true;
                    }
                }
            }

            return addedAny;
        }

        private List<BrowserNode> NormalizeBrowserNodeListForSubfolder(List<BrowserNode> nodes)
        {
            List<BrowserNode> result = new List<BrowserNode>();
            HashSet<IntPtr> keys = new HashSet<IntPtr>();

            if (nodes == null)
            {
                return result;
            }

            foreach (BrowserNode node in nodes)
            {
                if (node == null)
                {
                    continue;
                }

                IntPtr key = GetComIdentityKey(node);
                if (key != IntPtr.Zero && keys.Contains(key))
                {
                    continue;
                }

                if (key != IntPtr.Zero)
                {
                    keys.Add(key);
                }

                result.Add(node);
            }

            return result;
        }

        private static bool TryGetLateBoundProperty(object source, string propertyName, out object value)
        {
            value = null;

            if (source == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            try
            {
                value = source.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, source, null);
                return true;
            }
            catch
            {
                return false;
            }
        }


        // 19:05 16.06.2026 InventorIptOrg_v0_4_31_FEATURE_BROWSER_FOLDER_REANIMATE
        // Reanimate Features area from the current visible-list.
        // The .ipt cubes selection path is intentionally list-only and does not write myFeatureGroup attributes.
        // This method keeps that fast path intact and builds a virtual FeatureListItem list only when the user needs feature folders.
        private int BuildFeatureListFromCurrentVisibleBodiesFast(PartDocument partDoc, bool showMessage)
        {
            using (AppLogger.Scope("BuildFeatureListFromCurrentVisibleBodiesFast"))
            {
            if (partDoc == null || listBoxIptBodies == null || listBoxIptFeatures == null)
            {
                return 0;
            }

            if (listBoxIptBodies.Items.Count == 0)
            {
                if (showMessage)
                {
                    LoggedMessageBox.Show("Список тел пустой. Сначала выберите тела рамкой или через .ipt cubes.");
                }

                return 0;
            }

            Stopwatch sw = Stopwatch.StartNew();
            int scannedBodies = 0;
            int bodyCacheHits = 0;
            int bodyCacheMisses = 0;
            int directFeatureBodies = 0;
            int faceFallbackBodies = 0;
            int browserNodeFallbackBodies = 0;
            int browserNodeFallbackFailures = 0;
            int nullBodyItems = 0;
            int duplicateFeatureCount = 0;

            BrowserPane modelPane = null;
            try
            {
                modelPane = GetModelBrowserPane(partDoc);
            }
            catch
            {
                modelPane = null;
            }

            HashSet<IntPtr> addedFeatureKeys = new HashSet<IntPtr>();
            List<FeatureListItem> featureItems = new List<FeatureListItem>();

            foreach (object obj in listBoxIptBodies.Items)
            {
                BodyListItem bodyItem = obj as BodyListItem;
                if (bodyItem == null || bodyItem.Body == null)
                {
                    nullBodyItems++;
                    continue;
                }

                scannedBodies++;

                IntPtr bodyKey = bodyItem.IdentityKey;
                if (bodyKey == IntPtr.Zero)
                {
                    bodyKey = GetComIdentityKey(bodyItem.Body);
                }

                List<object> featuresForBody = null;

                if (bodyKey != IntPtr.Zero && _featureObjectsByBodyKey.TryGetValue(bodyKey, out featuresForBody))
                {
                    bodyCacheHits++;
                }
                else
                {
                    bool usedFaceFallback;
                    bool usedDirectFeature;
                    featuresForBody = GetFeaturesForBodyFast(bodyItem.Body, out usedFaceFallback, out usedDirectFeature);

                    if (featuresForBody == null)
                    {
                        featuresForBody = new List<object>();
                    }

                    if (featuresForBody.Count == 0)
                    {
                        BrowserNode bodyBrowserNode;
                        if (modelPane != null && TryGetBrowserNodeFromObject(modelPane, bodyItem.Body, out bodyBrowserNode) && bodyBrowserNode != null)
                        {
                            featuresForBody.Add(bodyBrowserNode);
                            browserNodeFallbackBodies++;

                            AppLogger.Log(
                                "FEATURE_LIST_BODY_BROWSER_NODE_FALLBACK",
                                "BuildFeatureListFromCurrentVisibleBodiesFast",
                                "Body=" + bodyItem.DisplayName +
                                "; BrowserNode=" + GetBrowserNodeDisplayName(bodyBrowserNode));
                        }
                        else
                        {
                            browserNodeFallbackFailures++;
                        }
                    }

                    if (bodyKey != IntPtr.Zero)
                    {
                        _featureObjectsByBodyKey[bodyKey] = featuresForBody;
                    }

                    bodyCacheMisses++;

                    if (usedDirectFeature)
                    {
                        directFeatureBodies++;
                    }

                    if (usedFaceFallback)
                    {
                        faceFallbackBodies++;
                    }
                }

                if (featuresForBody == null)
                {
                    continue;
                }

                foreach (object feature in featuresForBody)
                {
                    if (feature == null)
                    {
                        continue;
                    }

                    IntPtr featureKey = GetComIdentityKey(feature);

                    if (featureKey != IntPtr.Zero && addedFeatureKeys.Contains(featureKey))
                    {
                        duplicateFeatureCount++;
                        continue;
                    }

                    if (featureKey != IntPtr.Zero)
                    {
                        addedFeatureKeys.Add(featureKey);
                    }

                    int displayIndex = featureItems.Count + 1;
                    featureItems.Add(new FeatureListItem(feature, GetFeatureDisplayName(feature, displayIndex), featureKey));
                }
            }

            listBoxIptFeatures.BeginUpdate();
            try
            {
                listBoxIptFeatures.Items.Clear();

                foreach (FeatureListItem item in featureItems)
                {
                    listBoxIptFeatures.Items.Add(item);
                }
            }
            finally
            {
                listBoxIptFeatures.EndUpdate();
            }

            UpdateIptFeatureListCaption();

            sw.Stop();

            AppLogger.Log(
                "FEATURE_LIST_REANIMATE_FROM_VISIBLE_BODIES",
                "BuildFeatureListFromCurrentVisibleBodiesFast",
                "ScannedBodies=" + scannedBodies.ToString() +
                "; NullBodyItems=" + nullBodyItems.ToString() +
                "; FeatureItems=" + featureItems.Count.ToString() +
                "; DuplicateFeaturesSkipped=" + duplicateFeatureCount.ToString() +
                "; BodyCacheHits=" + bodyCacheHits.ToString() +
                "; BodyCacheMisses=" + bodyCacheMisses.ToString() +
                "; DirectFeatureBodies=" + directFeatureBodies.ToString() +
                "; FaceFallbackBodies=" + faceFallbackBodies.ToString() +
                "; BrowserNodeFallbackBodies=" + browserNodeFallbackBodies.ToString() +
                "; BrowserNodeFallbackFailures=" + browserNodeFallbackFailures.ToString() +
                "; ModelBrowserPaneAvailable=" + (modelPane != null).ToString() +
                "; ElapsedMs=" + sw.ElapsedMilliseconds.ToString() +
                "; AttributeWriteSkipped=True; RefreshBrowserTreeSkipped=True");

            if (showMessage)
            {
                LoggedMessageBox.Show(
                    "Features/browser-nodes построены из текущего списка тел.\r\n\r\n" +
                    "Тел просмотрено: " + scannedBodies.ToString() + "\r\n" +
                    "Строк в Features: " + featureItems.Count.ToString() + "\r\n" +
                    "BrowserNode fallback: " + browserNodeFallbackBodies.ToString() + "\r\n" +
                    "Время: " + sw.ElapsedMilliseconds.ToString() + " ms");
            }

            return featureItems.Count;

            }}

        private static List<object> GetFeaturesForBodyFast(SurfaceBody body, out bool usedFaceFallback, out bool usedDirectFeature)
        {
            using (AppLogger.Scope("GetFeaturesForBodyFast"))
            {
            usedFaceFallback = false;
            usedDirectFeature = false;

            List<object> result = new List<object>();
            HashSet<IntPtr> featureKeys = new HashSet<IntPtr>();

            Action<object> addFeature = delegate (object feature)
            {
                if (feature == null)
                {
                    return;
                }

                IntPtr key = GetComIdentityKey(feature);

                if (key != IntPtr.Zero && featureKeys.Contains(key))
                {
                    return;
                }

                if (key != IntPtr.Zero)
                {
                    featureKeys.Add(key);
                }

                result.Add(feature);
            };

            if (body == null)
            {
                return result;
            }

            try
            {
                dynamic dynBody = body;
                object directFeature = dynBody.CreatedByFeature;
                addFeature(directFeature);
            }
            catch
            {
            }

            if (result.Count > 0)
            {
                usedDirectFeature = true;
                return result;
            }

            // Fallback only when the SurfaceBody itself does not expose CreatedByFeature.
            // This is slower than the direct path, but it is still limited to the current visible-list,
            // not the full document tree.
            try
            {
                foreach (Face face in body.Faces)
                {
                    try
                    {
                        dynamic dynFace = face;
                        object faceFeature = dynFace.CreatedByFeature;
                        addFeature(faceFeature);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            if (result.Count > 0)
            {
                usedFaceFallback = true;
            }

            return result;

            }}


        private void ButtonIptClearFeatureList_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptClearFeatureList_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            DialogResult result = LoggedMessageBox.Show(
                "Очистить только список элементов и удалить группу myFeatureGroup?",
                "Clear IPT feature group",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question);

            if (result != DialogResult.OK)
            {
                return;
            }

            try
            {
                int removedFeatureCount = ClearGroupFromPartDocument(partDoc, "myFeatureGroup", "FeatureGroup1", "Group1");
                listBoxIptFeatures.Items.Clear();
                _featureObjectsByBodyKey.Clear();
                UpdateIptFeatureListCaption();
                LoggedMessageBox.Show("Список элементов очищен. Удалено из группы элементов: " + removedFeatureCount.ToString());
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem clearing IPT feature list");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        private void ButtonIptRemoveSelectedFeature_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptRemoveSelectedFeature_Click"))
            {
            RemoveSelectedIptFeaturesFromListAndGroup();
        
            }}

        private void listBoxIptFeatures_KeyDown(object sender, KeyEventArgs e)
        {
            using (AppLogger.Scope("listBoxIptFeatures_KeyDown"))
            {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedIptFeaturesFromListAndGroup();
                e.Handled = true;
            }
        
            }}

        private void ButtonIptRemoveSelected_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptRemoveSelected_Click"))
            {
            RemoveSelectedIptBodiesFromListAndGroup();
        
            }}

        private void listBoxIptBodies_KeyDown(object sender, KeyEventArgs e)
        {
            using (AppLogger.Scope("listBoxIptBodies_KeyDown"))
            {
            if (e.KeyCode == Keys.Delete)
            {
                RemoveSelectedIptBodiesFromListAndGroup();
                e.Handled = true;
            }
        
            }}

        private void RemoveSelectedIptBodiesFromListAndGroup()
        {
            using (AppLogger.Scope("RemoveSelectedIptBodiesFromListAndGroup"))
            {
            if (listBoxIptBodies.SelectedItems.Count == 0)
            {
                LoggedMessageBox.Show("Выберите одну или несколько строк списка для удаления.");
                return;
            }

            List<BodyListItem> selectedItems = new List<BodyListItem>();

            foreach (object obj in listBoxIptBodies.SelectedItems)
            {
                BodyListItem item = obj as BodyListItem;

                if (item != null)
                {
                    selectedItems.Add(item);
                }
            }

            int removedCount = 0;

            foreach (BodyListItem item in selectedItems)
            {
                if (RemoveBodyFromGroup(item.Body))
                {
                    removedCount++;
                }

                listBoxIptBodies.Items.Remove(item);
            }

            UpdateIptGroupListCaption();
            LoggedMessageBox.Show("Удалено из списка и группы: " + removedCount.ToString());
        
            }}

        private void RefreshIptFeatureList(PartDocument partDoc)
        {
            using (AppLogger.Scope("RefreshIptFeatureList"))
            {
            if (partDoc == null || this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                RunOnUiThread(delegate { RefreshIptFeatureList(partDoc); });
                return;
            }

            bool beginUpdateStarted = false;

            try
            {
                listBoxIptFeatures.BeginUpdate();
                beginUpdateStarted = true;
                listBoxIptFeatures.Items.Clear();

                AttributeManager attbMan = partDoc.AttributeManager;
                ObjectCollection objsCol = attbMan.FindObjects(
                    "myFeatureGroup",
                    "FeatureGroup1",
                    "Group1");

                HashSet<IntPtr> featureKeys = new HashSet<IntPtr>();
                int index = 1;

                foreach (object obj in objsCol)
                {
                    if (obj == null)
                    {
                        continue;
                    }

                    IntPtr key = GetComIdentityKey(obj);

                    if (key != IntPtr.Zero && featureKeys.Contains(key))
                    {
                        continue;
                    }

                    if (key != IntPtr.Zero)
                    {
                        featureKeys.Add(key);
                    }

                    listBoxIptFeatures.Items.Add(new FeatureListItem(obj, GetFeatureDisplayName(obj, index), key));
                    index++;
                }
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem refreshing IPT feature list");
                LoggedMessageBox.Show(ex.ToString());
            }
            finally
            {
                if (beginUpdateStarted)
                {
                    listBoxIptFeatures.EndUpdate();
                }

                UpdateIptFeatureListCaption();
            }
        
            }}

        private void UpdateIptFeatureListCaption()
        {
            using (AppLogger.Scope("UpdateIptFeatureListCaption"))
            {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                RunOnUiThread(UpdateIptFeatureListCaption);
                return;
            }

            labelIptFeatureList.Text = "Features / элементы, связанные с телами: " + listBoxIptFeatures.Items.Count.ToString();
        
            }}

        private static int AddFeaturesForBodyToGroup(SurfaceBody body)
        {
            using (AppLogger.Scope("AddFeaturesForBodyToGroup"))
            {
            if (body == null)
            {
                return 0;
            }

            List<object> features = GetFeaturesForBody(body);
            int added = 0;

            foreach (object feature in features)
            {
                if (AddFeatureToGroup(feature))
                {
                    added++;
                }
            }

            return added;
        
            }}

        private static List<object> GetFeaturesForBody(SurfaceBody body)
        {
            using (AppLogger.Scope("GetFeaturesForBody"))
            {
            List<object> result = new List<object>();
            HashSet<IntPtr> featureKeys = new HashSet<IntPtr>();

            Action<object> addFeature = delegate (object feature)
            {
                if (feature == null)
                {
                    return;
                }

                IntPtr key = GetComIdentityKey(feature);

                if (key != IntPtr.Zero && featureKeys.Contains(key))
                {
                    return;
                }

                if (key != IntPtr.Zero)
                {
                    featureKeys.Add(key);
                }

                result.Add(feature);
            };

            // Лучший случай: некоторые объекты Inventor напрямую отдают элемент, создавший всё тело.
            try
            {
                dynamic dynBody = body;
                addFeature(dynBody.CreatedByFeature);
            }
            catch
            {
            }

            // Надёжный запасной вариант: пройти по граням тела и собрать элементы, которые создали эти грани.
            // Это делается только для сгруппированных тел, а не для каждой грани во всём документе.
            try
            {
                foreach (Face face in body.Faces)
                {
                    try
                    {
                        dynamic dynFace = face;
                        addFeature(dynFace.CreatedByFeature);
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            return result;
        
            }}

        private static bool AddFeatureToGroup(object feature)
        {
            using (AppLogger.Scope("AddFeatureToGroup"))
            {
            AttributeSets attbSets = GetAttributeSetsFromInventorObject(feature);

            if (attbSets == null)
            {
                return false;
            }

            if (AttributeSetNameIsUsed(attbSets, "myFeatureGroup"))
            {
                return false;
            }

            AttributeSet attbSet = attbSets.Add("myFeatureGroup");

            Inventor.Attribute attb = attbSet.Add(
                "FeatureGroup1",
                ValueTypeEnum.kStringType,
                "Group1");

            return true;
        
            }}

        private static bool RemoveFeatureFromGroup(object feature)
        {
            using (AppLogger.Scope("RemoveFeatureFromGroup"))
            {
            return RemoveAttributeGroupFromInventorObject(feature, "myFeatureGroup");
        
            }}

        private void RemoveSelectedIptFeaturesFromListAndGroup()
        {
            using (AppLogger.Scope("RemoveSelectedIptFeaturesFromListAndGroup"))
            {
            if (listBoxIptFeatures.SelectedItems.Count == 0)
            {
                LoggedMessageBox.Show("Выберите одну или несколько строк списка элементов для удаления.");
                return;
            }

            List<FeatureListItem> selectedItems = new List<FeatureListItem>();

            foreach (object obj in listBoxIptFeatures.SelectedItems)
            {
                FeatureListItem item = obj as FeatureListItem;

                if (item != null)
                {
                    selectedItems.Add(item);
                }
            }

            int removedCount = 0;

            foreach (FeatureListItem item in selectedItems)
            {
                if (RemoveFeatureFromGroup(item.Feature))
                {
                    removedCount++;
                }

                listBoxIptFeatures.Items.Remove(item);
            }

            UpdateIptFeatureListCaption();
            LoggedMessageBox.Show("Удалено из списка элементов и группы: " + removedCount.ToString());
        
            }}

        private static string GetFeatureDisplayName(object feature, int index)
        {
            using (AppLogger.Scope("GetFeatureDisplayName"))
            {
            BrowserNode browserNode = feature as BrowserNode;
            if (browserNode != null)
            {
                string nodeName = GetBrowserNodeDisplayName(browserNode);
                return index.ToString("000") + " - BrowserNode - " + nodeName;
            }

            string name = null;
            string typeText = null;
            string suppressedText = string.Empty;

            try
            {
                dynamic dynFeature = feature;
                name = dynFeature.Name as string;
            }
            catch
            {
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "PartFeature";
            }

            try
            {
                dynamic dynFeature = feature;
                typeText = Convert.ToString(dynFeature.Type);
            }
            catch
            {
            }

            try
            {
                dynamic dynFeature = feature;
                bool suppressed = Convert.ToBoolean(dynFeature.Suppressed);
                suppressedText = suppressed ? " [suppressed]" : string.Empty;
            }
            catch
            {
            }

            if (!string.IsNullOrWhiteSpace(typeText))
            {
                return index.ToString("000") + " - " + name + " [" + typeText + "]" + suppressedText;
            }

            return index.ToString("000") + " - " + name + suppressedText;
        
            }}

        private static AttributeSets GetAttributeSetsFromInventorObject(object inventorObject)
        {
            using (AppLogger.Scope("GetAttributeSetsFromInventorObject"))
            {
            if (inventorObject == null)
            {
                return null;
            }

            try
            {
                dynamic dynObj = inventorObject;
                return (AttributeSets)dynObj.AttributeSets;
            }
            catch
            {
                return null;
            }
        
            }}

        private static bool RemoveAttributeGroupFromInventorObject(object inventorObject, string setName)
        {
            using (AppLogger.Scope("RemoveAttributeGroupFromInventorObject"))
            {
            if (inventorObject == null)
            {
                return false;
            }

            AttributeSets attbSets = GetAttributeSetsFromInventorObject(inventorObject);

            if (attbSets == null || !AttributeSetNameIsUsed(attbSets, setName))
            {
                return false;
            }

            try
            {
                dynamic sets = attbSets;
                AttributeSet attbSet = sets.Item(setName);
                attbSet.Delete();
                return true;
            }
            catch
            {
                try
                {
                    dynamic sets = attbSets;
                    AttributeSet attbSet = sets[setName];
                    attbSet.Delete();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        
            }}

        private static int ClearGroupFromPartDocument(PartDocument partDoc, string setName, string attributeName, string attributeValue)
        {
            using (AppLogger.Scope("ClearGroupFromPartDocument"))
            {
            AttributeManager attbMan = partDoc.AttributeManager;
            ObjectCollection objsCol = attbMan.FindObjects(setName, attributeName, attributeValue);

            List<object> objectsToRemove = new List<object>();

            foreach (object obj in objsCol)
            {
                if (obj != null)
                {
                    objectsToRemove.Add(obj);
                }
            }

            int removedCount = 0;

            foreach (object obj in objectsToRemove)
            {
                if (RemoveAttributeGroupFromInventorObject(obj, setName))
                {
                    removedCount++;
                }
            }

            return removedCount;
        
            }}


        // -----------------------------------------------------------------------------------------------------
        // Метод CreateBrowserFolderFromObjects:
        // в контрольном логе весь метод занял 37.862 с.
        // Важный нюанс: сама работа с BrowserPane/BrowserNode/созданием папки заняла около 1 с,
        // а большая часть времени ушла в вызванный после этого RefreshIptBrowserTree — 33.943 с.
        // -----------------------------------------------------------------------------------------------------

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS INLINE MAP: CreateBrowserFolderFromObjects — 37.862 с в контрольном логе.
        private void CreateBrowserFolderFromObjects(PartDocument partDoc, List<object> nativeObjects, string folderPrefix, string itemKindText, bool refreshBrowserTreeAfterCreate = true, bool createInnerFolderWithItems = false)
        {
            using (AppLogger.Scope("CreateBrowserFolderFromObjects"))
            {
            if (nativeObjects == null || nativeObjects.Count == 0)
            {
                LoggedMessageBox.Show("Нет объектов для создания папки.");
                return;
            }

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 2 / BrowserPane: получить Model BrowserPane — 0.709 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Внутри: TryGetBrowserPaneByName — 0.006 с; IsUsableBrowserPane — 0.008 с.
            BrowserPane modelPane = GetModelBrowserPane(partDoc);

            if (modelPane == null)
            {
                LoggedMessageBox.Show(
                    "Не удалось получить Model BrowserPane текущей детали.\r\n\r\n" +
                    "Я попробовал имена Model/Модель, ActivePane и перебор всех BrowserPanes.\r\n\r\n" +
                    "Список BrowserPanes, который вернул Inventor:\r\n" +
                    GetBrowserPaneDebugList(partDoc));
                return;
            }

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 2 / BrowserNode: поиск BrowserNode для объектов из списка — 0.023 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Контрольные детали: TryGetBrowserNodeFromObject #1 — 0.012846 с; #2 — 0.010381 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS GetComIdentityKey #1 — 0.000184 с; #2 — 0.000213 с.
            ObjectCollection browserNodes = _invApp.TransientObjects.CreateObjectCollection();
            List<BrowserNode> resolvedBrowserNodeList = new List<BrowserNode>();
            HashSet<IntPtr> addedNodeKeys = new HashSet<IntPtr>();
            int missedCount = 0;

            foreach (object nativeObject in nativeObjects)
            {
                BrowserNode node = nativeObject as BrowserNode;

                if (node != null)
                {
                    AppLogger.Log(
                        "FEATURE_BROWSER_FOLDER_DIRECT_BROWSER_NODE_USED",
                        "CreateBrowserFolderFromObjects",
                        "Node=" + GetBrowserNodeDisplayName(node));
                }
                else if (!TryGetBrowserNodeFromObject(modelPane, nativeObject, out node))
                {
                    missedCount++;
                    continue;
                }

                IntPtr nodeKey = GetComIdentityKey(node);

                if (nodeKey != IntPtr.Zero && addedNodeKeys.Contains(nodeKey))
                {
                    continue;
                }

                if (nodeKey != IntPtr.Zero)
                {
                    addedNodeKeys.Add(nodeKey);
                }

                browserNodes.Add(node);
                resolvedBrowserNodeList.Add(node);
            }

            if (browserNodes.Count == 0)
            {
                LoggedMessageBox.Show(
                    "Не удалось получить BrowserNode ни для одного объекта.\r\n\r\n" +
                    "Для некоторых версий Inventor API папки в .ipt могут работать ограниченно. " +
                    "Попробуйте кнопку папки для элементов/features, если папка для тел не создаётся.");
                return;
            }

            string folderTimestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string folderName = folderPrefix + "_" + folderTimestamp;
            string innerFolderName = folderPrefix + "_Items_" + folderTimestamp;
            BrowserFolder folder = null;
            BrowserFolder innerFolder = null;
            BrowserNode innerFolderBrowserNode = null;
            BrowserNode outerFolderBrowserNode = null;
            int innerFolderNodeCount = 0;
            bool innerFolderCreateFailed = false;
            bool outerFolderCreateFailed = false;
            bool trueNestedCreated = false;
            bool createdInnerOnlyFallback = false;
            string innerFolderError = string.Empty;
            string outerFolderError = string.Empty;

            // 13:35 17.06.2026 InventorIptOrg_v0_4_45_TRUE_NESTED_NO_DUPLICATES
            // ВАЖНО: для настоящей структуры "папка в папке" нельзя создавать внешнюю папку из тех же feature nodes,
            // а потом внутреннюю из тех же feature nodes. Это даёт визуальные дубликаты.
            // Правильный порядок:
            // 1) inner folder from feature nodes;
            // 2) BrowserNode(inner folder);
            // 3) outer folder from that one inner folder node.
            if (createInnerFolderWithItems)
            {
                try
                {
                    innerFolderNodeCount = browserNodes.Count;
                    innerFolder = CreateBrowserFolderWithFallback(modelPane, innerFolderName, browserNodes);

                    try
                    {
                        dynamic dynPaneUpdateInner = modelPane;
                        dynPaneUpdateInner.Update();
                    }
                    catch
                    {
                    }

                    if (TryGetBrowserNodeForBrowserFolder(modelPane, innerFolder, innerFolderName, out innerFolderBrowserNode) && innerFolderBrowserNode != null)
                    {
                        ObjectCollection outerBrowserNodes = _invApp.TransientObjects.CreateObjectCollection();
                        outerBrowserNodes.Add(innerFolderBrowserNode);

                        folder = CreateBrowserFolderWithFallback(modelPane, folderName, outerBrowserNodes);
                        trueNestedCreated = true;

                        try
                        {
                            dynamic dynPaneUpdateOuter = modelPane;
                            dynPaneUpdateOuter.Update();
                        }
                        catch
                        {
                        }

                        TryGetBrowserNodeForBrowserFolder(modelPane, folder, folderName, out outerFolderBrowserNode);

                        AppLogger.Log(
                            "FEATURE_BROWSER_FOLDER_TRUE_NESTED_CREATED",
                            "CreateBrowserFolderFromObjects",
                            "OuterFolderName=" + folderName +
                            "; InnerFolderName=" + innerFolderName +
                            "; InnerFolderNodeCount=" + innerFolderNodeCount.ToString() +
                            "; OuterFolderSource=InnerFolderBrowserNode; DuplicateFeatureNodesAvoided=True");
                    }
                    else
                    {
                        createdInnerOnlyFallback = true;
                        AppLogger.Log(
                            "FEATURE_BROWSER_FOLDER_TRUE_NESTED_INNER_ONLY_FALLBACK",
                            "CreateBrowserFolderFromObjects",
                            "InnerFolderName=" + innerFolderName +
                            "; InnerFolderNodeCount=" + innerFolderNodeCount.ToString() +
                            "; Reason=CannotResolveInnerFolderBrowserNode; DuplicateFeatureNodesAvoided=True");
                    }
                }
                catch (Exception ex)
                {
                    innerFolderCreateFailed = innerFolder == null;
                    outerFolderCreateFailed = innerFolder != null && folder == null && !createdInnerOnlyFallback;
                    if (innerFolder == null)
                    {
                        innerFolderError = ex.Message;
                    }
                    else
                    {
                        outerFolderError = ex.Message;
                    }

                    AppLogger.LogException("CreateBrowserFolderFromObjects.TrueNestedNoDuplicates", ex);

                    if (innerFolder == null)
                    {
                        LoggedMessageBox.Show(
                            "Inventor не смог создать внутреннюю Browser Folder.\r\n\r\n" +
                            ex.ToString());
                        return;
                    }

                    createdInnerOnlyFallback = true;
                }
            }
            else
            {
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 2 / создание папки: точного отдельного замера нет; оценка с подготовкой сообщения около 0.264 с.
                try
                {
                    folder = CreateBrowserFolderWithFallback(modelPane, folderName, browserNodes);
                }
                catch (Exception ex)
                {
                    LoggedMessageBox.Show(
                        "Inventor не смог создать Browser Folder.\r\n\r\n" +
                        "Возможно, эта версия Inventor ограничивает папки в .ipt для выбранного типа объектов.\r\n" +
                        "Попробуйте создать папку по списку элементов/features.\r\n\r\n" +
                        ex.ToString());
                    return;
                }

                try
                {
                    dynamic dynPaneUpdate = modelPane;
                    dynPaneUpdate.Update();
                }
                catch
                {
                }
            }

            PopulateBrowserTreePreviewAfterFolderCreate(
                partDoc,
                folderName,
                innerFolderName,
                outerFolderBrowserNode,
                innerFolderBrowserNode,
                resolvedBrowserNodeList,
                createInnerFolderWithItems,
                trueNestedCreated,
                createdInnerOnlyFallback);

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 2 / пользовательская обратная связь: MessageBox "Папка создана" — 2.885 с.
            LoggedMessageBox.Show(
                "Папка создана в дереве Inventor.\r\n\r\n" +
                (createInnerFolderWithItems ? "Режим: TRUE nested no-duplicates\r\n" : string.Empty) +
                (createInnerFolderWithItems ? "Внешняя папка: " + (trueNestedCreated ? folderName : (createdInnerOnlyFallback ? "не создана — fallback" : "FAILED")) + "\r\n" : "Папка: " + folderName + "\r\n") +
                (createInnerFolderWithItems ? "Внутренняя папка с элементами: " + (innerFolderCreateFailed ? "FAILED" : innerFolderName) + "\r\n" : string.Empty) +
                "Добавлено BrowserNode: " + browserNodes.Count.ToString() + "\r\n" +
                (createInnerFolderWithItems ? "BrowserNode во внутренней папке: " + innerFolderNodeCount.ToString() + "\r\n" : string.Empty) +
                (createInnerFolderWithItems ? "True nested создан: " + trueNestedCreated.ToString() + "\r\n" : string.Empty) +
                (createdInnerOnlyFallback ? "Fallback: создана только внутренняя папка, чтобы не плодить дубли.\r\n" : string.Empty) +
                (innerFolderCreateFailed ? "Ошибка внутренней папки: " + innerFolderError + "\r\n" : string.Empty) +
                (outerFolderCreateFailed ? "Ошибка внешней папки: " + outerFolderError + "\r\n" : string.Empty) +
                "Не удалось найти узел для объектов: " + missedCount.ToString() + "\r\n" +
                "Тип списка: " + itemKindText + "\r\n" +
                "Полный Refresh browser tree после создания: " + (refreshBrowserTreeAfterCreate ? "ON" : "SKIPPED for speed"));

            AppLogger.Log(
                refreshBrowserTreeAfterCreate ? "BROWSER_FOLDER_CREATED_WITH_TREE_REFRESH" : "FEATURE_BROWSER_FOLDER_FAST_CREATED_NO_TREE_REFRESH",
                "CreateBrowserFolderFromObjects",
                "FolderName=" + folderName +
                "; BrowserNodes=" + browserNodes.Count.ToString() +
                "; MissedObjects=" + missedCount.ToString() +
                "; ItemKind=" + itemKindText +
                "; RefreshBrowserTreeAfterCreate=" + refreshBrowserTreeAfterCreate.ToString() +
                "; CreateInnerFolderWithItems=" + createInnerFolderWithItems.ToString() +
                "; InnerFolderName=" + (createInnerFolderWithItems ? innerFolderName : string.Empty) +
                "; InnerFolderCreateFailed=" + innerFolderCreateFailed.ToString() +
                "; InnerFolderNodeCount=" + innerFolderNodeCount.ToString() +
                "; TrueNestedCreated=" + trueNestedCreated.ToString() +
                "; CreatedInnerOnlyFallback=" + createdInnerOnlyFallback.ToString() +
                "; DuplicateFeatureNodesAvoided=" + createInnerFolderWithItems.ToString());

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 2 / дорогая часть: RefreshIptBrowserTree — 33.943 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Практически всё это время — BuildBrowserTreeGridItems / обход дерева Inventor.
            InvalidateBrowserTreeNamesOnlyCache("CreateBrowserFolderFromObjects");

            if (refreshBrowserTreeAfterCreate)
            {
                RefreshIptBrowserTree(partDoc);
            }
            else
            {
                UpdateIptBrowserTreeCaption();
            }

            }}

        private BrowserFolder CreateBrowserFolderWithFallback(BrowserPane modelPane, string folderName, ObjectCollection browserNodes)
        {
            if (modelPane == null)
            {
                throw new InvalidOperationException("modelPane is null");
            }

            try
            {
                return modelPane.AddBrowserFolder(folderName, browserNodes);
            }
            catch
            {
                dynamic dynPane = modelPane;
                return (BrowserFolder)dynPane.AddBrowserFolder(folderName, browserNodes);
            }
        }

        private bool TryGetBrowserNodeForBrowserFolder(BrowserPane modelPane, BrowserFolder folder, string expectedFolderName, out BrowserNode node)
        {
            using (AppLogger.Scope("TryGetBrowserNodeForBrowserFolder"))
            {
            node = null;

            if (modelPane == null || folder == null)
            {
                return false;
            }

            object value;
            if (TryGetLateBoundProperty(folder, "BrowserNode", out value))
            {
                node = value as BrowserNode;
                if (node != null)
                {
                    return true;
                }
            }

            if (TryGetLateBoundProperty(folder, "Node", out value))
            {
                node = value as BrowserNode;
                if (node != null)
                {
                    return true;
                }
            }

            if (TryGetLateBoundProperty(folder, "FolderNode", out value))
            {
                node = value as BrowserNode;
                if (node != null)
                {
                    return true;
                }
            }

            try
            {
                if (TryGetBrowserNodeFromObject(modelPane, folder, out node) && node != null)
                {
                    return true;
                }
            }
            catch
            {
            }

            try
            {
                NodeCounter counter = new NodeCounter(8000);
                if (TryFindBrowserNodeByDisplayName(modelPane.TopNode, expectedFolderName, out node, counter, 0) && node != null)
                {
                    return true;
                }
            }
            catch
            {
            }

            AppLogger.Log(
                "FEATURE_BROWSER_FOLDER_INNER_NODE_RESOLVE_FAILED",
                "TryGetBrowserNodeForBrowserFolder",
                "ExpectedFolderName=" + expectedFolderName);

            return false;
            }}

        private bool TryFindBrowserNodeByDisplayName(BrowserNode currentNode, string expectedDisplayName, out BrowserNode foundNode, NodeCounter counter, int depth)
        {
            foundNode = null;

            if (currentNode == null || counter == null || counter.Count >= counter.MaxCount || depth > 40)
            {
                return false;
            }

            counter.Count++;

            string displayName = GetBrowserNodeDisplayName(currentNode);
            if (string.Equals(displayName, expectedDisplayName, StringComparison.OrdinalIgnoreCase))
            {
                foundNode = currentNode;
                return true;
            }

            try
            {
                dynamic dynNode = currentNode;
                dynamic children = dynNode.BrowserNodes;
                int childCount = (int)children.Count;

                for (int i = 1; i <= childCount; i++)
                {
                    BrowserNode child = null;
                    try
                    {
                        child = (BrowserNode)children.Item(i);
                    }
                    catch
                    {
                        try
                        {
                            child = (BrowserNode)children[i];
                        }
                        catch
                        {
                            child = null;
                        }
                    }

                    if (child != null && TryFindBrowserNodeByDisplayName(child, expectedDisplayName, out foundNode, counter, depth + 1))
                    {
                        return true;
                    }

                    if (counter.Count >= counter.MaxCount)
                    {
                        break;
                    }
                }
            }
            catch
            {
            }

            return false;
        }

        // ============================================================
        // Область дерева браузера .ipt
        // Показывает дерево браузера модели Inventor как редактируемую таблицу.
        // Имя можно передать обратно в Inventor. X/Y/Z — это центр RangeBox
        // в текущих единицах длины документа. Для строк SurfaceBody
        // редактирование X/Y/Z создаёт MoveFeature через AddFreeDrag.
        // ============================================================
        private const int BrowserGridColDepth = 0;
        private const int BrowserGridColType = 1;
        private const int BrowserGridColName = 2;
        private const int BrowserGridColX = 3;
        private const int BrowserGridColY = 4;
        private const int BrowserGridColZ = 5;
        private const int BrowserGridColCubeCount = 6;
        private const int BrowserGridColCubeIds = 7;

        private void ButtonIptBuildVisibleListTree_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptBuildVisibleListTree_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            List<BrowserTreeGridItem> items = new List<BrowserTreeGridItem>();

            BrowserTreeGridItem root = new BrowserTreeGridItem();
            root.Depth = 0;
            root.Name = "Current visible-list";
            root.OriginalName = root.Name;
            root.ObjectKind = "VirtualRoot";
            root.CanRename = false;
            root.CanMove = false;
            items.Add(root);

            if (listBoxIptBodies != null && listBoxIptBodies.Items.Count > 0)
            {
                BrowserTreeGridItem bodiesRoot = new BrowserTreeGridItem();
                bodiesRoot.Depth = 1;
                bodiesRoot.Name = "Bodies: " + listBoxIptBodies.Items.Count.ToString();
                bodiesRoot.OriginalName = bodiesRoot.Name;
                bodiesRoot.ObjectKind = "VirtualGroup";
                bodiesRoot.CanRename = false;
                bodiesRoot.CanMove = false;
                items.Add(bodiesRoot);

                foreach (object obj in listBoxIptBodies.Items)
                {
                    BodyListItem bodyItem = obj as BodyListItem;
                    SurfaceBody body = bodyItem == null ? null : bodyItem.Body;
                    BrowserTreeGridItem item = new BrowserTreeGridItem();
                    item.Depth = 2;
                    item.NativeObject = body;
                    item.Name = bodyItem == null ? Convert.ToString(obj) : bodyItem.DisplayName;
                    item.OriginalName = item.Name;
                    item.ObjectKind = "SurfaceBody";
                    double x;
                    double y;
                    double z;
                    item.HasCenter = TryGetObjectCenterInDocumentUnits(partDoc, body, out x, out y, out z);
                    item.X = x;
                    item.Y = y;
                    item.Z = z;
                    item.CanMove = body != null;
                    item.CanRename = body != null;
                    items.Add(item);
                }
            }

            if (listBoxIptFeatures != null && listBoxIptFeatures.Items.Count > 0)
            {
                BrowserTreeGridItem featuresRoot = new BrowserTreeGridItem();
                featuresRoot.Depth = 1;
                featuresRoot.Name = "Features: " + listBoxIptFeatures.Items.Count.ToString();
                featuresRoot.OriginalName = featuresRoot.Name;
                featuresRoot.ObjectKind = "VirtualGroup";
                featuresRoot.CanRename = false;
                featuresRoot.CanMove = false;
                items.Add(featuresRoot);

                foreach (object obj in listBoxIptFeatures.Items)
                {
                    FeatureListItem featureItem = obj as FeatureListItem;
                    object featureObject = featureItem == null ? obj : featureItem.Feature;
                    BrowserTreeGridItem item = new BrowserTreeGridItem();
                    item.Depth = 2;
                    item.NativeObject = featureObject;
                    item.Name = featureItem == null ? Convert.ToString(obj) : featureItem.DisplayName;
                    item.OriginalName = item.Name;
                    item.ObjectKind = SafeGetNativeObjectKind(featureObject, null);
                    double x;
                    double y;
                    double z;
                    item.HasCenter = TryGetObjectCenterInDocumentUnits(partDoc, featureObject, out x, out y, out z);
                    item.X = x;
                    item.Y = y;
                    item.Z = z;
                    item.CanMove = false;
                    item.CanRename = featureObject != null;
                    items.Add(item);
                }
            }

            PopulateIptBrowserTreeGridFromItems(items, "CurrentVisibleListsFastPreview");
            LoggedMessageBox.Show("Browser tree preview restored from current visible-list: " + items.Count.ToString() + " rows.");
            }}

        private void ResetBrowserRefreshButtonTimingCaptions()
        {
            using (AppLogger.Scope("ResetBrowserRefreshButtonTimingCaptions"))
            {
            SetTimedRefreshButtonText(ButtonIptRefreshBrowserTree, "Refresh browser tree", null);
            SetTimedRefreshButtonText(ButtonIptRefreshBrowserTreeSpatialBase, "Refresh tree #2 BASE", null);
            SetTimedRefreshButtonText(ButtonIptLegacyRefreshBrowserTree, "Refresh legacy", null);
            SetTimedRefreshButtonText(ButtonIptLegacyRefreshBrowserTreeSpatialBase, "Refresh legacy #2 BASE", null);
            }}

        private void SetTimedRefreshButtonText(Button button, string baseText, TimeSpan? elapsed)
        {
            if (button == null)
            {
                return;
            }

            string suffix = elapsed.HasValue
                ? elapsed.Value.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture) + " s"
                : "-- s";

            SetButtonTextSafe(button, baseText + "\r\nlast: " + suffix);
        }

        private void SetButtonTextSafe(Button button, string text)
        {
            if (button == null || button.IsDisposed)
            {
                return;
            }

            if (button.InvokeRequired)
            {
                button.BeginInvoke(new Action(delegate
                {
                    if (!button.IsDisposed)
                    {
                        button.Text = text;
                    }
                }));
                return;
            }

            button.Text = text;
        }

        // 23:05 17.06.2026 InventorIptOrg_v0_4_51_SECOND_REFRESH_SPATIAL_BASE
        // SECOND BUTTON #2: новый быстрый путь рядом со старой кнопкой.
        // Старый ButtonIptRefreshBrowserTree_Click НЕ трогаем.
        // 23:33 17.06.2026 InventorIptOrg_v0_4_52_SECOND_REFRESH_FULL_COUNT — замер кнопки Refresh tree #2 BASE
        // Источник: inventor_ipt_organizer_v0_4_52_20260617_233155_491.log
        // Refresh tree #2 BASE — полный цикл кнопки: 8.126 с
        // ├─ ButtonIptRefreshBrowserTreeSpatialBase_Click scope — 8.126723 с
        // ├─ TryGetActivePartDocument — 0.027071 с
        // ├─ EnsureSpatialBaseReadyForSecondRefresh — 0.002901 с
        // └─ BuildBrowserTreeGridFullCountFromSpatialBaseSecondButton — 8.093756 с
        //    ├─ GetModelBrowserPane — 0.214927 с
        //    ├─ GetBrowserNodeDisplayName: 269 вызовов — 3.491734 с
        //    ├─ PopulateIptBrowserTreeGridFromItems — 0.148436 с
        //    └─ UpdateIptBrowserTreeCaption — около 0.000 с
        // Инженерный вывод:
        // #2 full-count уже честно даёт 269 строк и не тратит время на NativeObject/ObjectKind/XYZ.
        // Остаточная цена 7-8 с — это BrowserPane traversal + получение имён 269 узлов.
        // Именно поэтому v0.4.53 добавляет online names-only snapshot cache.
        private void ButtonIptRefreshBrowserTreeSpatialBase_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptRefreshBrowserTreeSpatialBase_Click"))
            {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                SetButtonTextSafe(ButtonIptRefreshBrowserTreeSpatialBase, "Refresh tree #2\r\nworking...");

                if (!TryGetActivePartDocument(out PartDocument partDoc))
                {
                    return;
                }

                if (!EnsureSpatialBaseReadyForSecondRefresh(partDoc))
                {
                    return;
                }

                BuildBrowserTreeGridFullCountFromSpatialBaseSecondButton(partDoc);
            }
            finally
            {
                sw.Stop();
                SetTimedRefreshButtonText(ButtonIptRefreshBrowserTreeSpatialBase, "Refresh tree #2 BASE", sw.Elapsed);
                AppLogger.Log(
                    "BROWSER_TREE_REFRESH_2_SPATIAL_BASE_SECONDS",
                    "ButtonIptRefreshBrowserTreeSpatialBase_Click",
                    "ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));
            }

            }}

        // 23:05 17.06.2026 InventorIptOrg_v0_4_51_SECOND_REFRESH_SPATIAL_BASE
        // SECOND BUTTON #2 для старого legacy TreeView.
        // Старый ButtonIptLegacyRefreshBrowserTree_Click НЕ трогаем.
        // 23:33 17.06.2026 InventorIptOrg_v0_4_52_SECOND_REFRESH_FULL_COUNT — замер кнопки Refresh legacy #2 BASE
        // Источник: inventor_ipt_organizer_v0_4_52_20260617_233155_491.log
        // Refresh legacy #2 BASE — полный цикл кнопки: 7.202 с
        // ├─ ButtonIptLegacyRefreshBrowserTreeSpatialBase_Click scope — 7.202516 с
        // ├─ TryGetActivePartDocument — 0.026451 с
        // ├─ EnsureSpatialBaseReadyForSecondRefresh — 0.003474 с
        // └─ BuildLegacyBrowserTreeFullCountFromSpatialBaseSecondButton — 7.168412 с
        //    ├─ GetModelBrowserPane — 0.232706 с
        //    ├─ GetBrowserNodeDisplayName: 269 вызовов — 3.139893 с
        //    └─ UpdateIptLegacyBrowserTreeCaption — около 0.000 с
        // Инженерный вывод:
        // legacy #2 показывает цену чистого full-count names-only дерева без XYZ/NativeObject.
        // Следующий шаг для “почти онлайн” — кэшировать результат первого прохода и затем отдавать дерево из памяти.
        private void ButtonIptLegacyRefreshBrowserTreeSpatialBase_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptLegacyRefreshBrowserTreeSpatialBase_Click"))
            {
            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                SetButtonTextSafe(ButtonIptLegacyRefreshBrowserTreeSpatialBase, "Refresh legacy #2\r\nworking...");

                if (!TryGetActivePartDocument(out PartDocument partDoc))
                {
                    return;
                }

                if (!EnsureSpatialBaseReadyForSecondRefresh(partDoc))
                {
                    return;
                }

                BuildLegacyBrowserTreeFullCountFromSpatialBaseSecondButton(partDoc);
            }
            finally
            {
                sw.Stop();
                SetTimedRefreshButtonText(ButtonIptLegacyRefreshBrowserTreeSpatialBase, "Refresh legacy #2 BASE", sw.Elapsed);
                AppLogger.Log(
                    "LEGACY_BROWSER_TREE_REFRESH_2_SPATIAL_BASE_SECONDS",
                    "ButtonIptLegacyRefreshBrowserTreeSpatialBase_Click",
                    "ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));
            }

            }}

        private bool EnsureSpatialBaseReadyForSecondRefresh(PartDocument partDoc)
        {
            using (AppLogger.Scope("EnsureSpatialBaseReadyForSecondRefresh"))
            {
            if (_spatialCubesIndex != null && _spatialCubesIndex.IsReady && _spatialCubesIndex.MatchesDocument(partDoc))
            {
                return true;
            }

            return RebuildSpatialCubesBaseForSecondRefresh(partDoc, "EnsureSpatialBaseReadyForSecondRefresh");

            }}

        // 23:25 17.06.2026 InventorIptOrg_v0_4_52_SECOND_REFRESH_FULL_COUNT
        // Новый #2 режим: тот же BrowserPane node count, что у оригинального Refresh,
        // но без дорогого NativeObject/ObjectKind/XYZ для каждого BrowserNode.
        private void BuildBrowserTreeGridFullCountFromSpatialBaseSecondButton(PartDocument partDoc)
        {
            using (AppLogger.Scope("BuildBrowserTreeGridFullCountFromSpatialBaseSecondButton"))
            {
            Stopwatch sw = Stopwatch.StartNew();

            BrowserTreeNamesOnlySnapshot snapshot = TryGetFastCacheViewSnapshot(partDoc, "Refresh tree #2 BASE");

            if (snapshot == null)
            {
                LoggedMessageBox.Show(
                    "FAST CACHE не готов.\r\n\r\n" +
                    "Кнопка Refresh tree #2 BASE в v0.4.62 больше НЕ запускает живой обход дерева Inventor.\r\n\r\n" +
                    "Сначала нужно один раз получить cache:\r\n" +
                    "1) обычным Refresh browser tree, или\r\n" +
                    "2) предыдущим rebuild/sync режимом.\r\n\r\n" +
                    "Spatial cubes BASE можно пересчитать, но она не заменяет 269-строчный Browser tree cache.");
                return;
            }

            int spatialRowsRefreshed;
            int spatialCentersRefreshed;
            RefreshFastCacheSpatialDataFromSpatialBase(partDoc, snapshot, out spatialRowsRefreshed, out spatialCentersRefreshed);

            List<BrowserTreeGridItem> previousEditableItems = CaptureCurrentBrowserTreeGridItemsForSecondButtonMerge();
            List<BrowserTreeGridItem> items = CloneBrowserTreeGridItems(snapshot.GridItems);

            int editableMergedRows;
            int editableMergedCenters;
            int editableMergedMovableRows;
            int editableMismatchRows;
            MergeEditableBrowserTreeDataByRow(
                items,
                previousEditableItems,
                out editableMergedRows,
                out editableMergedCenters,
                out editableMergedMovableRows,
                out editableMismatchRows);

            bool inPlaceGridUpdated = PopulateIptBrowserTreeGridInPlaceFromItems(
                items,
                "SecondButtonFastCacheViewInPlace");

            if (!inPlaceGridUpdated)
            {
                PopulateIptBrowserTreeGridFromItems(items, "SecondButtonFastCacheViewFallback");
            }

            sw.Stop();

            AppLogger.Log(
                "BROWSER_TREE_REFRESH_2_FAST_CACHE_VIEW",
                "BuildBrowserTreeGridFullCountFromSpatialBaseSecondButton",
                "Rows=" + items.Count.ToString() +
                "; InPlaceGridUpdated=" + inPlaceGridUpdated.ToString() +
                "; SnapshotNodes=" + snapshot.NodeCount.ToString() +
                "; SnapshotNativeObjectRows=" + snapshot.NativeObjectRows.ToString() +
                "; SnapshotSpatialBodyRows=" + snapshot.SpatialBodyRows.ToString() +
                "; SnapshotSpatialCenterRows=" + snapshot.SpatialCenterRows.ToString() +
                "; FastCacheOnly=True" +
                "; BrowserPaneTraversal=False" +
                "; LiveNameSync=False" +
                "; GetOrBuildBrowserTreeNamesOnlySnapshotCalled=False" +
                "; SpatialRowsRefreshed=" + spatialRowsRefreshed.ToString() +
                "; SpatialCentersRefreshed=" + spatialCentersRefreshed.ToString() +
                "; EditableMergeRows=" + editableMergedRows.ToString() +
                "; EditableMergeCenters=" + editableMergedCenters.ToString() +
                "; EditableMergeMovableRows=" + editableMergedMovableRows.ToString() +
                "; EditableMismatchRows=" + editableMismatchRows.ToString() +
                "; PreserveEditXyz=True" +
                "; OriginalButtonPreserved=True" +
                "; ExpectedSameRowCountAsOriginal=True" +
                "; ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));

            }}

        private void AppendBrowserTreeGridItemsFullCountSpatialBaseNamesOnly(BrowserNode browserNode, int depth, List<BrowserTreeGridItem> items, NodeCounter counter)
        {
            using (AppLogger.Scope("AppendBrowserTreeGridItemsFullCountSpatialBaseNamesOnly"))
            {
            if (browserNode == null || counter.Count >= counter.MaxCount)
            {
                return;
            }

            counter.Count++;

            string name = GetBrowserNodeDisplayName(browserNode);

            BrowserTreeGridItem item = new BrowserTreeGridItem();
            item.Node = browserNode;
            item.NativeObject = null;
            item.Depth = depth;
            item.Name = name;
            item.OriginalName = name;
            item.ObjectKind = depth == 0 ? "BrowserNode #2 names-only root" : "BrowserNode #2 names-only";
            item.HasCenter = false;
            item.CanMove = false;
            item.CanRename = browserNode != null;
            items.Add(item);

            if (depth > 25 || counter.Count >= counter.MaxCount)
            {
                return;
            }

            try
            {
                dynamic dynNode = browserNode;
                dynamic children = dynNode.BrowserNodes;
                int childCount = (int)children.Count;

                for (int i = 1; i <= childCount; i++)
                {
                    BrowserNode child = null;

                    try
                    {
                        child = (BrowserNode)children.Item(i);
                    }
                    catch
                    {
                        try
                        {
                            child = (BrowserNode)children[i];
                        }
                        catch
                        {
                            child = null;
                        }
                    }

                    if (child != null)
                    {
                        AppendBrowserTreeGridItemsFullCountSpatialBaseNamesOnly(child, depth + 1, items, counter);
                    }

                    if (counter.Count >= counter.MaxCount)
                    {
                        break;
                    }
                }
            }
            catch
            {
            }

            }}

        // 23:25 17.06.2026 InventorIptOrg_v0_4_52_SECOND_REFRESH_FULL_COUNT
        // Legacy #2: тот же BrowserPane node count, но TreeView строится names-only.
        private void BuildLegacyBrowserTreeFullCountFromSpatialBaseSecondButton(PartDocument partDoc)
        {
            using (AppLogger.Scope("BuildLegacyBrowserTreeFullCountFromSpatialBaseSecondButton"))
            {
            Stopwatch sw = Stopwatch.StartNew();

            if (treeViewIptLegacyBrowserTree == null)
            {
                return;
            }

            BrowserTreeNamesOnlySnapshot snapshot = TryGetFastCacheViewSnapshot(partDoc, "Refresh legacy #2 BASE");

            if (snapshot == null || snapshot.RootNode == null)
            {
                LoggedMessageBox.Show(
                    "FAST CACHE не готов для Legacy #2.\r\n\r\n" +
                    "В v0.4.62 legacy #2 тоже не запускает живой обход BrowserPane.");
                return;
            }

            BrowserTreeNode root = CloneBrowserTreeNode(snapshot.RootNode);

            treeViewIptLegacyBrowserTree.BeginUpdate();
            try
            {
                treeViewIptLegacyBrowserTree.Nodes.Clear();

                TreeNode rootNode = CreateTreeViewNode(root);
                treeViewIptLegacyBrowserTree.Nodes.Add(rootNode);
                rootNode.Expand();
            }
            finally
            {
                try
                {
                    treeViewIptLegacyBrowserTree.EndUpdate();
                }
                catch
                {
                }
            }

            UpdateIptLegacyBrowserTreeCaption();
            sw.Stop();

            AppLogger.Log(
                "LEGACY_BROWSER_TREE_REFRESH_2_FAST_CACHE_VIEW",
                "BuildLegacyBrowserTreeFullCountFromSpatialBaseSecondButton",
                "TreeViewNodes=" + CountTreeViewNodes(treeViewIptLegacyBrowserTree.Nodes).ToString() +
                "; SnapshotNodes=" + snapshot.NodeCount.ToString() +
                "; SnapshotNativeObjectRows=" + snapshot.NativeObjectRows.ToString() +
                "; SnapshotSpatialBodyRows=" + snapshot.SpatialBodyRows.ToString() +
                "; SnapshotSpatialCenterRows=" + snapshot.SpatialCenterRows.ToString() +
                "; FastCacheOnly=True" +
                "; BrowserPaneTraversal=False" +
                "; LiveNameSync=False" +
                "; GetOrBuildBrowserTreeNamesOnlySnapshotCalled=False" +
                "; OriginalButtonPreserved=True" +
                "; ExpectedSameNodeCountAsOriginal=True" +
                "; ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));

            }}

        private BrowserTreeNamesOnlySnapshot TryGetFastCacheViewSnapshot(PartDocument partDoc, string source)
        {
            using (AppLogger.Scope("TryGetFastCacheViewSnapshot"))
            {
            string documentKey = GetBrowserTreeSnapshotDocumentKey(partDoc);

            if (_browserTreeNamesOnlySnapshot != null &&
                string.Equals(_browserTreeNamesOnlySnapshot.DocumentKey, documentKey, StringComparison.OrdinalIgnoreCase) &&
                _browserTreeNamesOnlySnapshot.GridItems != null &&
                _browserTreeNamesOnlySnapshot.RootNode != null)
            {
                AppLogger.Log(
                    "BROWSER_TREE_FAST_CACHE_VIEW_HIT",
                    "TryGetFastCacheViewSnapshot",
                    "Source=" + (source ?? string.Empty) +
                    "; DocumentKey=" + documentKey +
                    "; Nodes=" + _browserTreeNamesOnlySnapshot.NodeCount.ToString() +
                    "; BrowserPaneTraversal=False");
                return _browserTreeNamesOnlySnapshot;
            }

            BrowserTreeNamesOnlySnapshot gridSnapshot = TryBuildBrowserTreeSnapshotFromCurrentGridOnly(partDoc, source);

            if (gridSnapshot != null)
            {
                _browserTreeNamesOnlySnapshot = gridSnapshot;
                AppLogger.Log(
                    "BROWSER_TREE_FAST_CACHE_VIEW_BUILT_FROM_CURRENT_GRID",
                    "TryGetFastCacheViewSnapshot",
                    "Source=" + (source ?? string.Empty) +
                    "; DocumentKey=" + documentKey +
                    "; Nodes=" + gridSnapshot.NodeCount.ToString() +
                    "; BrowserPaneTraversal=False" +
                    "; SourceGridRows=" + gridSnapshot.GridItems.Count.ToString());
                return _browserTreeNamesOnlySnapshot;
            }

            AppLogger.Log(
                "BROWSER_TREE_FAST_CACHE_VIEW_MISS",
                "TryGetFastCacheViewSnapshot",
                "Source=" + (source ?? string.Empty) +
                "; DocumentKey=" + documentKey +
                "; BrowserPaneTraversal=False" +
                "; CurrentGridRows=" + (dataGridViewIptBrowserTree == null ? "0" : dataGridViewIptBrowserTree.Rows.Count.ToString()));

            return null;

            }}

        private BrowserTreeNamesOnlySnapshot TryBuildBrowserTreeSnapshotFromCurrentGridOnly(PartDocument partDoc, string source)
        {
            using (AppLogger.Scope("TryBuildBrowserTreeSnapshotFromCurrentGridOnly"))
            {
            List<BrowserTreeGridItem> items = CaptureCurrentBrowserTreeGridItemsForSecondButtonMerge();

            if (items == null || items.Count == 0)
            {
                return null;
            }

            BrowserTreeNamesOnlySnapshot snapshot = new BrowserTreeNamesOnlySnapshot();
            snapshot.DocumentKey = GetBrowserTreeSnapshotDocumentKey(partDoc);
            snapshot.BuiltAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            snapshot.GridItems.AddRange(CloneBrowserTreeGridItems(items));
            snapshot.NodeCount = snapshot.GridItems.Count;
            snapshot.RootNode = BuildBrowserTreeNodeFromGridItems(snapshot.GridItems);

            CountSnapshotRows(snapshot);

            AppLogger.Log(
                "BROWSER_TREE_FAST_CACHE_SNAPSHOT_FROM_GRID_BUILT",
                "TryBuildBrowserTreeSnapshotFromCurrentGridOnly",
                "Source=" + (source ?? string.Empty) +
                "; Nodes=" + snapshot.NodeCount.ToString() +
                "; NativeObjectRows=" + snapshot.NativeObjectRows.ToString() +
                "; SpatialBodyRows=" + snapshot.SpatialBodyRows.ToString() +
                "; SpatialCenterRows=" + snapshot.SpatialCenterRows.ToString() +
                "; BrowserPaneTraversal=False");

            return snapshot;

            }}

        private static BrowserTreeNode BuildBrowserTreeNodeFromGridItems(List<BrowserTreeGridItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return null;
            }

            BrowserTreeNode root = null;
            List<BrowserTreeNode> stack = new List<BrowserTreeNode>();

            foreach (BrowserTreeGridItem item in items)
            {
                if (item == null)
                {
                    continue;
                }

                BrowserTreeNode node = new BrowserTreeNode();
                node.Name = item.Name;
                node.ObjectKind = item.ObjectKind;
                node.HasCenter = item.HasCenter;
                node.X = item.X;
                node.Y = item.Y;
                node.Z = item.Z;

                int depth = Math.Max(0, item.Depth);

                while (stack.Count <= depth)
                {
                    stack.Add(null);
                }

                if (depth == 0 || root == null)
                {
                    if (root == null)
                    {
                        root = node;
                    }
                    else
                    {
                        root.Children.Add(node);
                    }
                }
                else
                {
                    BrowserTreeNode parent = null;

                    for (int d = depth - 1; d >= 0; d--)
                    {
                        if (d < stack.Count && stack[d] != null)
                        {
                            parent = stack[d];
                            break;
                        }
                    }

                    if (parent == null)
                    {
                        if (root == null)
                        {
                            root = node;
                        }
                        else
                        {
                            root.Children.Add(node);
                        }
                    }
                    else
                    {
                        parent.Children.Add(node);
                    }
                }

                stack[depth] = node;

                for (int d = depth + 1; d < stack.Count; d++)
                {
                    stack[d] = null;
                }
            }

            return root;
        }

        private static void CountSnapshotRows(BrowserTreeNamesOnlySnapshot snapshot)
        {
            if (snapshot == null || snapshot.GridItems == null)
            {
                return;
            }

            int nativeRows = 0;
            int bodyRows = 0;
            int centerRows = 0;

            foreach (BrowserTreeGridItem item in snapshot.GridItems)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.NativeObject != null)
                {
                    nativeRows++;
                }

                if (item.NativeObject is SurfaceBody)
                {
                    bodyRows++;
                }

                if (item.HasCenter)
                {
                    centerRows++;
                }
            }

            snapshot.NativeObjectRows = nativeRows;
            snapshot.SpatialBodyRows = bodyRows;
            snapshot.SpatialCenterRows = centerRows;
        }

        private void RefreshFastCacheSpatialDataFromSpatialBase(
            PartDocument partDoc,
            BrowserTreeNamesOnlySnapshot snapshot,
            out int spatialRowsRefreshed,
            out int spatialCentersRefreshed)
        {
            using (AppLogger.Scope("RefreshFastCacheSpatialDataFromSpatialBase"))
            {
            spatialRowsRefreshed = 0;
            spatialCentersRefreshed = 0;

            if (snapshot == null || snapshot.GridItems == null)
            {
                return;
            }

            if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady || !_spatialCubesIndex.MatchesDocument(partDoc))
            {
                RebuildSpatialCubesBaseForSecondRefresh(partDoc, "RefreshFastCacheSpatialDataFromSpatialBase");
            }

            if (_spatialCubesIndex == null || !_spatialCubesIndex.IsReady || _spatialCubesIndex.Bodies == null)
            {
                return;
            }

            UnitsOfMeasure uom = null;
            try
            {
                uom = partDoc == null ? null : partDoc.UnitsOfMeasure;
            }
            catch
            {
                uom = null;
            }

            foreach (BrowserTreeGridItem item in snapshot.GridItems)
            {
                if (item == null || !(item.NativeObject is SurfaceBody))
                {
                    continue;
                }

                SpatialBodyRecord record;

                if (!TryFindSpatialBodyRecordByNativeObject(item.NativeObject, out record))
                {
                    continue;
                }

                spatialRowsRefreshed++;

                double x;
                double y;
                double z;

                if (TryGetSpatialBoxCenterInDocumentUnits(uom, record.BodyBox, out x, out y, out z))
                {
                    item.HasCenter = true;
                    item.X = x;
                    item.Y = y;
                    item.Z = z;
                    item.CanMove = true;
                    item.CanRename = true;
                    item.ObjectKind = "SurfaceBody (FAST CACHE + Spatial BASE)";
                    spatialCentersRefreshed++;
                }
            }

            CountSnapshotRows(snapshot);

            AppLogger.Log(
                "BROWSER_TREE_FAST_CACHE_SPATIAL_DATA_REFRESHED",
                "RefreshFastCacheSpatialDataFromSpatialBase",
                "Rows=" + snapshot.GridItems.Count.ToString() +
                "; SpatialRowsRefreshed=" + spatialRowsRefreshed.ToString() +
                "; SpatialCentersRefreshed=" + spatialCentersRefreshed.ToString() +
                "; SpatialBodies=" + _spatialCubesIndex.Bodies.Count.ToString() +
                "; Cells=" + _spatialCubesIndex.Cells.Count.ToString() +
                "; BrowserPaneTraversal=False");

            }}

        private void SyncBrowserTreeNamesOnlySnapshotNamesFromLiveBrowserNodes(
            BrowserTreeNamesOnlySnapshot snapshot,
            string source,
            out int readCount,
            out int changedCount)
        {
            using (AppLogger.Scope("SyncBrowserTreeNamesOnlySnapshotNamesFromLiveBrowserNodes"))
            {
            Stopwatch sw = Stopwatch.StartNew();
            readCount = 0;
            changedCount = 0;

            if (snapshot == null || snapshot.GridItems == null)
            {
                return;
            }

            foreach (BrowserTreeGridItem item in snapshot.GridItems)
            {
                if (item == null || item.Node == null)
                {
                    continue;
                }

                readCount++;

                string liveName = GetBrowserNodeDisplayName(item.Node);

                if (string.IsNullOrWhiteSpace(liveName))
                {
                    continue;
                }

                liveName = liveName.Trim();

                if (!string.Equals(item.Name, liveName, StringComparison.Ordinal))
                {
                    item.Name = liveName;
                    item.OriginalName = liveName;
                    changedCount++;
                }
            }

            int treeIndex = 0;
            ApplySnapshotGridNamesToBrowserTreeNode(snapshot.RootNode, snapshot.GridItems, ref treeIndex);

            sw.Stop();

            AppLogger.Log(
                "BROWSER_TREE_NAMES_ONLY_CACHE_LIVE_NAME_SYNCED",
                "SyncBrowserTreeNamesOnlySnapshotNamesFromLiveBrowserNodes",
                "Source=" + (source ?? string.Empty) +
                "; NodesRead=" + readCount.ToString() +
                "; NamesChanged=" + changedCount.ToString() +
                "; TreeNamesApplied=" + treeIndex.ToString() +
                "; NativeObjectSkipped=True" +
                "; ObjectKindSkipped=True" +
                "; XyzSkipped=True" +
                "; ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));

            }}

        private static void ApplySnapshotGridNamesToBrowserTreeNode(
            BrowserTreeNode treeNode,
            List<BrowserTreeGridItem> gridItems,
            ref int index)
        {
            if (treeNode == null || gridItems == null || index >= gridItems.Count)
            {
                return;
            }

            BrowserTreeGridItem item = gridItems[index];

            if (item != null)
            {
                treeNode.Name = item.Name;
                treeNode.ObjectKind = item.ObjectKind;
                treeNode.HasCenter = item.HasCenter;
                treeNode.X = item.X;
                treeNode.Y = item.Y;
                treeNode.Z = item.Z;
            }

            index++;

            foreach (BrowserTreeNode child in treeNode.Children)
            {
                ApplySnapshotGridNamesToBrowserTreeNode(child, gridItems, ref index);
            }
        }

        private BrowserTreeNamesOnlySnapshot GetOrBuildBrowserTreeNamesOnlySnapshot(PartDocument partDoc, out bool cacheHit)
        {
            using (AppLogger.Scope("GetOrBuildBrowserTreeNamesOnlySnapshot"))
            {
            cacheHit = false;
            string documentKey = GetBrowserTreeSnapshotDocumentKey(partDoc);

            if (_browserTreeNamesOnlySnapshot != null &&
                string.Equals(_browserTreeNamesOnlySnapshot.DocumentKey, documentKey, StringComparison.OrdinalIgnoreCase) &&
                _browserTreeNamesOnlySnapshot.GridItems != null &&
                _browserTreeNamesOnlySnapshot.RootNode != null)
            {
                cacheHit = true;
                AppLogger.Log(
                    "BROWSER_TREE_NAMES_ONLY_CACHE_HIT",
                    "GetOrBuildBrowserTreeNamesOnlySnapshot",
                    "DocumentKey=" + documentKey +
                    "; Nodes=" + _browserTreeNamesOnlySnapshot.NodeCount.ToString() +
                    "; BuiltAt=" + _browserTreeNamesOnlySnapshot.BuiltAt);
                return _browserTreeNamesOnlySnapshot;
            }

            Stopwatch sw = Stopwatch.StartNew();
            BrowserPane modelPane = GetModelBrowserPane(partDoc);

            if (modelPane == null)
            {
                LoggedMessageBox.Show(
                    "Не удалось получить Model BrowserPane текущей детали.\r\n\r\n" +
                    "Список BrowserPanes, который вернул Inventor:\r\n" +
                    GetBrowserPaneDebugList(partDoc));
                return null;
            }

            BrowserTreeNamesOnlySnapshot snapshot = new BrowserTreeNamesOnlySnapshot();
            snapshot.DocumentKey = documentKey;
            snapshot.BuiltAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            UnitsOfMeasure uom = null;
            try
            {
                uom = partDoc == null ? null : partDoc.UnitsOfMeasure;
            }
            catch
            {
                uom = null;
            }

            int spatialBodyRows = 0;
            int spatialCenterRows = 0;
            int nativeObjectRows = 0;
            snapshot.RootNode = BuildBrowserTreeUnifiedSnapshotNode(
                modelPane.TopNode,
                0,
                new NodeCounter(5000),
                snapshot,
                uom,
                ref nativeObjectRows,
                ref spatialBodyRows,
                ref spatialCenterRows);
            snapshot.NodeCount = snapshot.GridItems.Count;
            snapshot.NativeObjectRows = nativeObjectRows;
            snapshot.SpatialBodyRows = spatialBodyRows;
            snapshot.SpatialCenterRows = spatialCenterRows;

            _browserTreeNamesOnlySnapshot = snapshot;
            sw.Stop();

            AppLogger.Log(
                "BROWSER_TREE_NAMES_ONLY_CACHE_MISS_BUILT",
                "GetOrBuildBrowserTreeNamesOnlySnapshot",
                "DocumentKey=" + documentKey +
                "; Nodes=" + snapshot.NodeCount.ToString() +
                "; NativeObjectRows=" + snapshot.NativeObjectRows.ToString() +
                "; SpatialBodyRows=" + snapshot.SpatialBodyRows.ToString() +
                "; SpatialCenterRows=" + snapshot.SpatialCenterRows.ToString() +
                "; BrowserPaneTraversal=TrueUnifiedRowModel" +
                "; NativeObjectSkipped=True" +
                "; ObjectKindSkipped=True" +
                "; XyzSkipped=True" +
                "; ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));

            return snapshot;

            }}

        private BrowserTreeNode BuildBrowserTreeUnifiedSnapshotNode(
            BrowserNode browserNode,
            int depth,
            NodeCounter counter,
            BrowserTreeNamesOnlySnapshot snapshot,
            UnitsOfMeasure uom,
            ref int nativeObjectRows,
            ref int spatialBodyRows,
            ref int spatialCenterRows)
        {
            using (AppLogger.Scope("BuildBrowserTreeUnifiedSnapshotNode"))
            {
            BrowserTreeNode result = new BrowserTreeNode();

            if (browserNode == null)
            {
                result.Name = "<null BrowserNode>";
                result.ObjectKind = "BrowserNode #2 unified null";
                return result;
            }

            if (counter.Count >= counter.MaxCount)
            {
                result.Name = "<tree truncated: node limit reached>";
                result.ObjectKind = "BrowserNode #2 unified truncated";
                return result;
            }

            counter.Count++;
            string name = GetBrowserNodeDisplayName(browserNode);
            string kind = depth == 0 ? "BrowserNode #2 unified root" : "BrowserNode #2 unified";

            BrowserTreeGridItem item = new BrowserTreeGridItem();
            item.Node = browserNode;
            item.NativeObject = null;
            item.Depth = depth;
            item.Name = name;
            item.OriginalName = name;
            item.ObjectKind = kind;
            item.HasCenter = false;
            item.CanMove = false;
            item.CanRename = browserNode != null;

            bool attachedSpatialBody = TryAttachSpatialBaseBodyDataToBrowserTreeItem(
                browserNode,
                item,
                uom,
                out bool attachedCenter);

            if (item.NativeObject != null)
            {
                nativeObjectRows++;
            }

            if (attachedSpatialBody)
            {
                spatialBodyRows++;
            }

            if (attachedCenter)
            {
                spatialCenterRows++;
            }

            snapshot.GridItems.Add(item);

            result.Name = item.Name;
            result.ObjectKind = item.ObjectKind;
            result.HasCenter = item.HasCenter;
            result.X = item.X;
            result.Y = item.Y;
            result.Z = item.Z;

            if (depth > 25 || counter.Count >= counter.MaxCount)
            {
                return result;
            }

            try
            {
                dynamic dynNode = browserNode;
                dynamic children = dynNode.BrowserNodes;
                int childCount = (int)children.Count;

                for (int i = 1; i <= childCount; i++)
                {
                    if (counter.Count >= counter.MaxCount)
                    {
                        BrowserTreeNode truncated = new BrowserTreeNode();
                        truncated.Name = "<tree truncated: node limit reached>";
                        truncated.ObjectKind = "BrowserNode #2 unified truncated";
                        result.Children.Add(truncated);
                        break;
                    }

                    BrowserNode child = null;

                    try
                    {
                        child = (BrowserNode)children.Item(i);
                    }
                    catch
                    {
                        try
                        {
                            child = (BrowserNode)children[i];
                        }
                        catch
                        {
                            child = null;
                        }
                    }

                    if (child != null)
                    {
                        result.Children.Add(BuildBrowserTreeUnifiedSnapshotNode(
                            child,
                            depth + 1,
                            counter,
                            snapshot,
                            uom,
                            ref nativeObjectRows,
                            ref spatialBodyRows,
                            ref spatialCenterRows));
                    }
                }
            }
            catch
            {
            }

            return result;

            }}

        private bool TryAttachSpatialBaseBodyDataToBrowserTreeItem(
            BrowserNode browserNode,
            BrowserTreeGridItem item,
            UnitsOfMeasure uom,
            out bool attachedCenter)
        {
            using (AppLogger.Scope("TryAttachSpatialBaseBodyDataToBrowserTreeItem"))
            {
            attachedCenter = false;

            if (browserNode == null || item == null || _spatialCubesIndex == null || !_spatialCubesIndex.IsReady || _spatialCubesIndex.Bodies == null)
            {
                return false;
            }

            object nativeObject = TryGetNativeObjectFromBrowserNode(browserNode);

            if (!(nativeObject is SurfaceBody))
            {
                return false;
            }

            item.NativeObject = nativeObject;
            item.CanMove = true;
            item.CanRename = true;

            SpatialBodyRecord record;

            if (!TryFindSpatialBodyRecordByNativeObject(nativeObject, out record))
            {
                item.ObjectKind = "SurfaceBody (BrowserNode native)";
                return true;
            }

            item.ObjectKind = "SurfaceBody (Unified BASE model)";

            double x;
            double y;
            double z;

            if (TryGetSpatialBoxCenterInDocumentUnits(uom, record.BodyBox, out x, out y, out z))
            {
                item.HasCenter = true;
                item.X = x;
                item.Y = y;
                item.Z = z;
                attachedCenter = true;
            }

            return true;

            }}

        private bool TryFindSpatialBodyRecordByNativeObject(object nativeObject, out SpatialBodyRecord record)
        {
            record = null;

            if (nativeObject == null || _spatialCubesIndex == null || _spatialCubesIndex.Bodies == null)
            {
                return false;
            }

            IntPtr identityKey = IntPtr.Zero;

            try
            {
                identityKey = GetComIdentityKey(nativeObject);
            }
            catch
            {
                identityKey = IntPtr.Zero;
            }

            foreach (SpatialBodyRecord candidate in _spatialCubesIndex.Bodies)
            {
                if (candidate == null)
                {
                    continue;
                }

                if (identityKey != IntPtr.Zero && candidate.IdentityKey == identityKey)
                {
                    record = candidate;
                    return true;
                }

                if (candidate.Body != null && object.ReferenceEquals(candidate.Body, nativeObject))
                {
                    record = candidate;
                    return true;
                }
            }

            return false;
        }

        private BrowserTreeNode BuildBrowserTreeNamesOnlySnapshotNode(BrowserNode browserNode, int depth, NodeCounter counter, BrowserTreeNamesOnlySnapshot snapshot)
        {
            using (AppLogger.Scope("BuildBrowserTreeNamesOnlySnapshotNode"))
            {
            BrowserTreeNode result = new BrowserTreeNode();

            if (browserNode == null)
            {
                result.Name = "<null BrowserNode>";
                result.ObjectKind = "BrowserNode #2 cache names-only null";
                return result;
            }

            if (counter.Count >= counter.MaxCount)
            {
                result.Name = "<tree truncated: node limit reached>";
                result.ObjectKind = "BrowserNode #2 cache names-only truncated";
                return result;
            }

            counter.Count++;
            string name = GetBrowserNodeDisplayName(browserNode);
            string kind = depth == 0 ? "BrowserNode #2 cache root" : "BrowserNode #2 cache names-only";

            result.Name = name;
            result.ObjectKind = kind;
            result.HasCenter = false;

            BrowserTreeGridItem item = new BrowserTreeGridItem();
            item.Node = browserNode;
            item.NativeObject = null;
            item.Depth = depth;
            item.Name = name;
            item.OriginalName = name;
            item.ObjectKind = kind;
            item.HasCenter = false;
            item.CanMove = false;
            item.CanRename = browserNode != null;
            snapshot.GridItems.Add(item);

            if (depth > 25 || counter.Count >= counter.MaxCount)
            {
                return result;
            }

            try
            {
                dynamic dynNode = browserNode;
                dynamic children = dynNode.BrowserNodes;
                int childCount = (int)children.Count;

                for (int i = 1; i <= childCount; i++)
                {
                    if (counter.Count >= counter.MaxCount)
                    {
                        BrowserTreeNode truncated = new BrowserTreeNode();
                        truncated.Name = "<tree truncated: node limit reached>";
                        truncated.ObjectKind = "BrowserNode #2 cache names-only truncated";
                        result.Children.Add(truncated);
                        break;
                    }

                    BrowserNode child = null;

                    try
                    {
                        child = (BrowserNode)children.Item(i);
                    }
                    catch
                    {
                        try
                        {
                            child = (BrowserNode)children[i];
                        }
                        catch
                        {
                            child = null;
                        }
                    }

                    if (child != null)
                    {
                        result.Children.Add(BuildBrowserTreeNamesOnlySnapshotNode(child, depth + 1, counter, snapshot));
                    }
                }
            }
            catch
            {
            }

            return result;

            }}

        private static List<BrowserTreeGridItem> CloneBrowserTreeGridItems(List<BrowserTreeGridItem> source)
        {
            List<BrowserTreeGridItem> result = new List<BrowserTreeGridItem>();

            if (source == null)
            {
                return result;
            }

            foreach (BrowserTreeGridItem item in source)
            {
                if (item == null)
                {
                    continue;
                }

                BrowserTreeGridItem copy = new BrowserTreeGridItem();
                copy.Node = item.Node;
                copy.NativeObject = item.NativeObject;
                copy.Depth = item.Depth;
                copy.Name = item.Name;
                copy.OriginalName = item.OriginalName;
                copy.ObjectKind = item.ObjectKind;
                copy.HasCenter = item.HasCenter;
                copy.X = item.X;
                copy.Y = item.Y;
                copy.Z = item.Z;
                copy.CanMove = item.CanMove;
                copy.CanRename = item.CanRename;
                result.Add(copy);
            }

            return result;
        }

        private List<BrowserTreeGridItem> CaptureCurrentBrowserTreeGridItemsForSecondButtonMerge()
        {
            using (AppLogger.Scope("CaptureCurrentBrowserTreeGridItemsForSecondButtonMerge"))
            {
            List<BrowserTreeGridItem> result = new List<BrowserTreeGridItem>();

            if (this.IsDisposed || dataGridViewIptBrowserTree == null || this.InvokeRequired)
            {
                return result;
            }

            foreach (DataGridViewRow row in dataGridViewIptBrowserTree.Rows)
            {
                BrowserTreeGridItem source = row == null ? null : row.Tag as BrowserTreeGridItem;

                if (source == null)
                {
                    continue;
                }

                BrowserTreeGridItem copy = CloneBrowserTreeGridItem(source);

                string visibleName = Convert.ToString(row.Cells[BrowserGridColName].Value);
                if (!string.IsNullOrWhiteSpace(visibleName))
                {
                    copy.Name = visibleName.Trim();
                }

                double x;
                double y;
                double z;

                if (TryParseGridDouble(row.Cells[BrowserGridColX].Value, out x) &&
                    TryParseGridDouble(row.Cells[BrowserGridColY].Value, out y) &&
                    TryParseGridDouble(row.Cells[BrowserGridColZ].Value, out z))
                {
                    copy.HasCenter = true;
                    copy.X = x;
                    copy.Y = y;
                    copy.Z = z;
                }

                result.Add(copy);
            }

            AppLogger.Log(
                "BROWSER_TREE_EDITABLE_GRID_SNAPSHOT_CAPTURED",
                "CaptureCurrentBrowserTreeGridItemsForSecondButtonMerge",
                "Rows=" + result.Count.ToString());

            return result;

            }}

        private static BrowserTreeGridItem CloneBrowserTreeGridItem(BrowserTreeGridItem item)
        {
            if (item == null)
            {
                return null;
            }

            BrowserTreeGridItem copy = new BrowserTreeGridItem();
            copy.Node = item.Node;
            copy.NativeObject = item.NativeObject;
            copy.Depth = item.Depth;
            copy.Name = item.Name;
            copy.OriginalName = item.OriginalName;
            copy.ObjectKind = item.ObjectKind;
            copy.HasCenter = item.HasCenter;
            copy.X = item.X;
            copy.Y = item.Y;
            copy.Z = item.Z;
            copy.CanMove = item.CanMove;
            copy.CanRename = item.CanRename;
            return copy;
        }

        private void MergeEditableBrowserTreeDataByRow(
            List<BrowserTreeGridItem> targetItems,
            List<BrowserTreeGridItem> previousItems,
            out int mergedRows,
            out int mergedCenters,
            out int mergedMovableRows,
            out int mismatchRows)
        {
            using (AppLogger.Scope("MergeEditableBrowserTreeDataByRow"))
            {
            mergedRows = 0;
            mergedCenters = 0;
            mergedMovableRows = 0;
            mismatchRows = 0;

            if (targetItems == null || previousItems == null || targetItems.Count == 0 || previousItems.Count == 0)
            {
                return;
            }

            int count = Math.Min(targetItems.Count, previousItems.Count);

            for (int i = 0; i < count; i++)
            {
                BrowserTreeGridItem target = targetItems[i];
                BrowserTreeGridItem previous = previousItems[i];

                if (target == null || previous == null)
                {
                    mismatchRows++;
                    continue;
                }

                if (!IsLikelySameBrowserTreeGridRow(target, previous))
                {
                    mismatchRows++;
                    continue;
                }

                if (previous.NativeObject != null)
                {
                    target.NativeObject = previous.NativeObject;
                }

                if (previous.HasCenter)
                {
                    target.HasCenter = true;
                    target.X = previous.X;
                    target.Y = previous.Y;
                    target.Z = previous.Z;
                    mergedCenters++;
                }

                target.CanMove = previous.CanMove;
                target.CanRename = previous.CanRename || target.CanRename;

                if (!string.IsNullOrWhiteSpace(previous.OriginalName))
                {
                    target.OriginalName = previous.OriginalName;
                }

                mergedRows++;

                if (target.CanMove && target.NativeObject is SurfaceBody)
                {
                    mergedMovableRows++;
                }
            }

            if (targetItems.Count != previousItems.Count)
            {
                mismatchRows += Math.Abs(targetItems.Count - previousItems.Count);
            }

            AppLogger.Log(
                "BROWSER_TREE_EDITABLE_GRID_MERGED_INTO_BASE2",
                "MergeEditableBrowserTreeDataByRow",
                "TargetRows=" + targetItems.Count.ToString() +
                "; PreviousRows=" + previousItems.Count.ToString() +
                "; MergedRows=" + mergedRows.ToString() +
                "; MergedCenters=" + mergedCenters.ToString() +
                "; MergedMovableRows=" + mergedMovableRows.ToString() +
                "; MismatchRows=" + mismatchRows.ToString());

            }}

        private static bool IsLikelySameBrowserTreeGridRow(BrowserTreeGridItem a, BrowserTreeGridItem b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            if (a.Depth != b.Depth)
            {
                return false;
            }

            return string.Equals(
                NormalizeBrowserTreeGridNameForMerge(a.Name),
                NormalizeBrowserTreeGridNameForMerge(b.Name),
                StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeBrowserTreeGridNameForMerge(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            return value.Trim();
        }

        private static BrowserTreeNode CloneBrowserTreeNode(BrowserTreeNode source)
        {
            if (source == null)
            {
                return null;
            }

            BrowserTreeNode copy = new BrowserTreeNode();
            copy.Name = source.Name;
            copy.ObjectKind = source.ObjectKind;
            copy.HasCenter = source.HasCenter;
            copy.X = source.X;
            copy.Y = source.Y;
            copy.Z = source.Z;

            foreach (BrowserTreeNode child in source.Children)
            {
                BrowserTreeNode childCopy = CloneBrowserTreeNode(child);

                if (childCopy != null)
                {
                    copy.Children.Add(childCopy);
                }
            }

            return copy;
        }

        private static string GetBrowserTreeSnapshotDocumentKey(PartDocument partDoc)
        {
            string fullFileName = SafeGetPartFullFileName(partDoc);

            if (!string.IsNullOrWhiteSpace(fullFileName))
            {
                return fullFileName.Trim().ToLowerInvariant();
            }

            return SafeGetPartDisplayName(partDoc).Trim().ToLowerInvariant();
        }

        private void InvalidateBrowserTreeNamesOnlyCache(string reason)
        {
            using (AppLogger.Scope("InvalidateBrowserTreeNamesOnlyCache"))
            {
            bool hadCache = _browserTreeNamesOnlySnapshot != null;
            _browserTreeNamesOnlySnapshot = null;

            AppLogger.Log(
                "BROWSER_TREE_NAMES_ONLY_CACHE_INVALIDATED",
                "InvalidateBrowserTreeNamesOnlyCache",
                "HadCache=" + hadCache.ToString() +
                "; Reason=" + (reason ?? string.Empty));

            }}

        private BrowserTreeNode BuildBrowserTreeNodeFullCountSpatialBaseNamesOnly(BrowserNode browserNode, int depth, NodeCounter counter)
        {
            using (AppLogger.Scope("BuildBrowserTreeNodeFullCountSpatialBaseNamesOnly"))
            {
            BrowserTreeNode result = new BrowserTreeNode();

            if (browserNode == null)
            {
                result.Name = "<null BrowserNode>";
                return result;
            }

            counter.Count++;
            result.Name = GetBrowserNodeDisplayName(browserNode);
            result.ObjectKind = depth == 0 ? "BrowserNode #2 names-only root" : "BrowserNode #2 names-only";

            if (depth > 25 || counter.Count >= counter.MaxCount)
            {
                return result;
            }

            try
            {
                dynamic dynNode = browserNode;
                dynamic children = dynNode.BrowserNodes;
                int childCount = (int)children.Count;

                for (int i = 1; i <= childCount; i++)
                {
                    if (counter.Count >= counter.MaxCount)
                    {
                        BrowserTreeNode truncated = new BrowserTreeNode();
                        truncated.Name = "<tree truncated: node limit reached>";
                        result.Children.Add(truncated);
                        break;
                    }

                    BrowserNode child = null;

                    try
                    {
                        child = (BrowserNode)children.Item(i);
                    }
                    catch
                    {
                        try
                        {
                            child = (BrowserNode)children[i];
                        }
                        catch
                        {
                            child = null;
                        }
                    }

                    if (child != null)
                    {
                        result.Children.Add(BuildBrowserTreeNodeFullCountSpatialBaseNamesOnly(child, depth + 1, counter));
                    }
                }
            }
            catch
            {
            }

            return result;

            }}

        private void BuildBrowserTreeGridFromSpatialBaseSecondButton(PartDocument partDoc)
        {
            using (AppLogger.Scope("BuildBrowserTreeGridFromSpatialBaseSecondButton"))
            {
            Stopwatch sw = Stopwatch.StartNew();

            List<BrowserTreeGridItem> items = new List<BrowserTreeGridItem>();

            BrowserTreeGridItem root = new BrowserTreeGridItem();
            root.Node = null;
            root.NativeObject = null;
            root.Depth = 0;
            root.Name = string.IsNullOrWhiteSpace(_spatialCubesIndex.DocumentDisplayName)
                ? SafeGetPartDisplayName(partDoc)
                : _spatialCubesIndex.DocumentDisplayName;
            root.OriginalName = root.Name;
            root.ObjectKind = "Spatial BASE document";
            root.HasCenter = false;
            root.CanMove = false;
            root.CanRename = false;
            items.Add(root);

            BrowserTreeGridItem group = new BrowserTreeGridItem();
            group.Node = null;
            group.NativeObject = null;
            group.Depth = 1;
            group.Name = "Bodies from spatial BASE (" + _spatialCubesIndex.Bodies.Count.ToString() + ")";
            group.OriginalName = group.Name;
            group.ObjectKind = "Spatial BASE bodies";
            group.HasCenter = false;
            group.CanMove = false;
            group.CanRename = false;
            items.Add(group);

            UnitsOfMeasure uom = null;
            try
            {
                uom = partDoc == null ? null : partDoc.UnitsOfMeasure;
            }
            catch
            {
                uom = null;
            }

            int bodyRows = 0;
            int rowsWithoutBox = 0;

            foreach (SpatialBodyRecord record in _spatialCubesIndex.Bodies)
            {
                if (record == null)
                {
                    continue;
                }

                BrowserTreeGridItem item = new BrowserTreeGridItem();
                item.Node = null;
                item.NativeObject = record.Body;
                item.Depth = 2;
                item.Name = string.IsNullOrWhiteSpace(record.DisplayName)
                    ? "SurfaceBody " + record.BodyIndex.ToString()
                    : record.DisplayName;
                item.OriginalName = item.Name;
                item.ObjectKind = "SurfaceBody (Spatial BASE)";
                item.CanMove = record.Body != null;
                item.CanRename = record.Body != null;

                double x;
                double y;
                double z;
                if (TryGetSpatialBoxCenterInDocumentUnits(uom, record.BodyBox, out x, out y, out z))
                {
                    item.HasCenter = true;
                    item.X = x;
                    item.Y = y;
                    item.Z = z;
                }
                else
                {
                    rowsWithoutBox++;
                }

                items.Add(item);
                bodyRows++;
            }

            PopulateIptBrowserTreeGridFromItems(items, "SecondButtonSpatialBaseFastBodyTree");
            sw.Stop();

            AppLogger.Log(
                "BROWSER_TREE_REFRESH_2_SPATIAL_BASE_BODYONLY_BUILT",
                "BuildBrowserTreeGridFromSpatialBaseSecondButton",
                "Rows=" + items.Count.ToString() +
                "; BodyRows=" + bodyRows.ToString() +
                "; RowsWithoutBox=" + rowsWithoutBox.ToString() +
                "; SpatialBodies=" + _spatialCubesIndex.Bodies.Count.ToString() +
                "; Cells=" + _spatialCubesIndex.Cells.Count.ToString() +
                "; FullBrowserPaneTraversal=False" +
                "; OriginalButtonPreserved=True" +
                "; Source=Fast build spatial cubes BASE" +
                "; ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));

            }}

        private void BuildLegacyBrowserTreeFromSpatialBaseSecondButton(PartDocument partDoc)
        {
            using (AppLogger.Scope("BuildLegacyBrowserTreeFromSpatialBaseSecondButton"))
            {
            Stopwatch sw = Stopwatch.StartNew();

            if (treeViewIptLegacyBrowserTree == null)
            {
                return;
            }

            BrowserTreeNode root = BuildSpatialBaseBrowserTreeNode(partDoc);

            treeViewIptLegacyBrowserTree.BeginUpdate();
            try
            {
                treeViewIptLegacyBrowserTree.Nodes.Clear();
                TreeNode rootNode = CreateTreeViewNode(root);
                treeViewIptLegacyBrowserTree.Nodes.Add(rootNode);
                rootNode.Expand();
            }
            finally
            {
                try
                {
                    treeViewIptLegacyBrowserTree.EndUpdate();
                }
                catch
                {
                }
            }

            UpdateIptLegacyBrowserTreeCaption();
            sw.Stop();

            AppLogger.Log(
                "LEGACY_BROWSER_TREE_REFRESH_2_SPATIAL_BASE_BODYONLY_BUILT",
                "BuildLegacyBrowserTreeFromSpatialBaseSecondButton",
                "TreeViewNodes=" + CountTreeViewNodes(treeViewIptLegacyBrowserTree.Nodes).ToString() +
                "; SpatialBodies=" + _spatialCubesIndex.Bodies.Count.ToString() +
                "; Cells=" + _spatialCubesIndex.Cells.Count.ToString() +
                "; FullBrowserPaneTraversal=False" +
                "; OriginalButtonPreserved=True" +
                "; Source=Fast build spatial cubes BASE" +
                "; ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));

            }}

        private BrowserTreeNode BuildSpatialBaseBrowserTreeNode(PartDocument partDoc)
        {
            BrowserTreeNode root = new BrowserTreeNode();
            root.Name = string.IsNullOrWhiteSpace(_spatialCubesIndex.DocumentDisplayName)
                ? SafeGetPartDisplayName(partDoc)
                : _spatialCubesIndex.DocumentDisplayName;
            root.ObjectKind = "Spatial BASE document";

            BrowserTreeNode group = new BrowserTreeNode();
            group.Name = "Bodies from spatial BASE (" + _spatialCubesIndex.Bodies.Count.ToString() + ")";
            group.ObjectKind = "Spatial BASE bodies";
            root.Children.Add(group);

            UnitsOfMeasure uom = null;
            try
            {
                uom = partDoc == null ? null : partDoc.UnitsOfMeasure;
            }
            catch
            {
                uom = null;
            }

            foreach (SpatialBodyRecord record in _spatialCubesIndex.Bodies)
            {
                if (record == null)
                {
                    continue;
                }

                BrowserTreeNode bodyNode = new BrowserTreeNode();
                bodyNode.Name = string.IsNullOrWhiteSpace(record.DisplayName)
                    ? "SurfaceBody " + record.BodyIndex.ToString()
                    : record.DisplayName;
                bodyNode.ObjectKind = "SurfaceBody (Spatial BASE)";

                double x;
                double y;
                double z;
                if (TryGetSpatialBoxCenterInDocumentUnits(uom, record.BodyBox, out x, out y, out z))
                {
                    bodyNode.HasCenter = true;
                    bodyNode.X = x;
                    bodyNode.Y = y;
                    bodyNode.Z = z;
                }

                group.Children.Add(bodyNode);
            }

            return root;
        }

        private static bool TryGetSpatialBoxCenterInDocumentUnits(UnitsOfMeasure uom, SpatialBox box, out double x, out double y, out double z)
        {
            x = 0;
            y = 0;
            z = 0;

            if (box == null)
            {
                return false;
            }

            double centerX = (box.MinX + box.MaxX) / 2.0;
            double centerY = (box.MinY + box.MaxY) / 2.0;
            double centerZ = (box.MinZ + box.MaxZ) / 2.0;

            try
            {
                if (uom != null)
                {
                    x = uom.ConvertUnits(centerX, UnitsTypeEnum.kDatabaseLengthUnits, uom.LengthUnits);
                    y = uom.ConvertUnits(centerY, UnitsTypeEnum.kDatabaseLengthUnits, uom.LengthUnits);
                    z = uom.ConvertUnits(centerZ, UnitsTypeEnum.kDatabaseLengthUnits, uom.LengthUnits);
                    return true;
                }
            }
            catch
            {
            }

            x = centerX;
            y = centerY;
            z = centerZ;
            return true;
        }

        // 23:31 17.06.2026 InventorIptOrg_v0_4_52_SECOND_REFRESH_FULL_COUNT — замер кнопки Refresh browser tree
        // Источник: inventor_ipt_organizer_v0_4_52_20260617_233155_491.log
        // Refresh browser tree — полный цикл кнопки: 39.458 с
        // ├─ ButtonIptRefreshBrowserTree_Click scope — 39.542327 с
        // ├─ TryGetActivePartDocument — 0.077734 с
        // └─ RefreshIptBrowserTree — 39.456587 с
        //    ├─ BuildBrowserTreeGridItems — 39.065874 с
        //    │  ├─ GetModelBrowserPane — 0.514207 с
        //    │  ├─ TryGetNativeObjectFromBrowserNode: 269 вызовов — 3.388043 с
        //    │  ├─ GetBrowserNodeDisplayName: 269 вызовов — 4.023515 с
        //    │  ├─ SafeGetNativeObjectKind: 269 вызовов — 5.393711 с
        //    │  ├─ TryGetObjectCenterInDocumentUnits: 269 вызовов — 17.299796 с
        //    │  │  └─ ConvertDatabaseLengthToDocumentUnits: 654 вызова — 3.551396 с
        //    │  └─ AppendBrowserTreeGridItems: 269 рекурсивных scope; сумма scope перекрывается рекурсией
        //    ├─ PopulateIptBrowserTreeGridFromItems — 0.377967 с
        //    └─ UpdateIptBrowserTreeCaption — около 0.002 с
        // Инженерный вывод:
        // обычный Refresh browser tree всё ещё тормозит из-за тяжёлого COM-чтения каждого BrowserNode:
        // NativeObject + ObjectKind + особенно XYZ/RangeBox/ConvertUnits. DataGridView-заполнение занимает меньше 0.4 с.
        // Поэтому правильный путь оптимизации — отдельные режимы names-only/cache/#2, а не косметика UI.
                // 17:55 18.06.2026 InventorIptOrg_v0_4_63_FULL_REFRESH_V48_RESTORED
        // ВОССТАНОВЛЕН ИСТОЧНИК ПРАВДЫ:
        // ButtonIptRefreshBrowserTree_Click и его полный путь RefreshIptBrowserTree ->
        // BuildBrowserTreeGridItems -> AppendBrowserTreeGridItems взяты из
        // InventorIptOrg_v0_4_48_RefreshButtonTiming(2).zip.
        // Это тот самый честный медленный режим ~44.582 с:
        // BrowserNode -> NativeObject -> ObjectKind -> X/Y/Z -> реальные имена и структура Inventor.
        // Быстрые #2 BASE режимы остаются вспомогательными cache-view экспериментами,
        // но каноническая кнопка Refresh browser tree снова считается основной истиной.
private void ButtonIptRefreshBrowserTree_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptRefreshBrowserTree_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                SetButtonTextSafe(ButtonIptRefreshBrowserTree, "Refresh browser tree\r\nworking...");
                RefreshIptBrowserTree(partDoc);
            }
            finally
            {
                sw.Stop();
                SetTimedRefreshButtonText(ButtonIptRefreshBrowserTree, "Refresh browser tree", sw.Elapsed);
                AppLogger.Log(
                    "BROWSER_TREE_REFRESH_BUTTON_SECONDS",
                    "ButtonIptRefreshBrowserTree_Click",
                    "ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));
            }

            }}

        // 23:31-23:33 17.06.2026 InventorIptOrg_v0_4_52_SECOND_REFRESH_FULL_COUNT — замер кнопки Copy tree to clipboard
        // Источник: inventor_ipt_organizer_v0_4_52_20260617_233155_491.log
        // ButtonIptCopyBrowserTree_Click — точного замера в этом логе нет.
        // ENTER/EXIT для ButtonIptCopyBrowserTree_Click: 0 / 0.
        // События clipboard/copy в логе: не найдены.
        // Инженерный вывод:
        // этот лог пригоден для Refresh / #2 Refresh замеров, но не для оценки Copy tree.
        // Для честного блока нужно отдельное нажатие Copy tree после заполнения 269 строк.
        private void ButtonIptCopyBrowserTree_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptCopyBrowserTree_Click"))
            {
            if (dataGridViewIptBrowserTree.Rows.Count == 0)
            {
                LoggedMessageBox.Show("Дерево пустое. Сначала нажмите Refresh browser tree.");
                return;
            }

            string text = GetBrowserTreeGridText();

            if (string.IsNullOrWhiteSpace(text))
            {
                LoggedMessageBox.Show("Не удалось получить текст дерева.");
                return;
            }

            Clipboard.SetText(text);
            LoggedMessageBox.Show("Дерево с координатами скопировано в буфер обмена.");
        
            }}

        private void ButtonIptSaveBrowserTree_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptSaveBrowserTree_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            try
            {
                BrowserTreeExport export = BuildBrowserTreeExport(partDoc);

                if (export == null || export.Root == null)
                {
                    LoggedMessageBox.Show("Не удалось построить дерево браузера Inventor.");
                    return;
                }

                string defaultFileName = "inventor_ipt_tree.json";
                string initialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);

                try
                {
                    if (!string.IsNullOrWhiteSpace(partDoc.FullFileName))
                    {
                        initialDirectory = System.IO.Path.GetDirectoryName(partDoc.FullFileName);
                        defaultFileName = System.IO.Path.GetFileNameWithoutExtension(partDoc.FullFileName) + ".tree.json";
                    }
                }
                catch
                {
                }

                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Title = "Save IPT browser tree JSON";
                    dialog.Filter = "JSON tree file (*.json)|*.json|Text tree file (*.txt)|*.txt|All files (*.*)|*.*";
                    dialog.FileName = defaultFileName;
                    dialog.InitialDirectory = initialDirectory;
                    dialog.OverwritePrompt = true;

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    string extension = System.IO.Path.GetExtension(dialog.FileName).ToLowerInvariant();

                    if (extension == ".txt")
                    {
                        System.IO.File.WriteAllText(dialog.FileName, BuildPlainTextTree(export), Encoding.UTF8);
                    }
                    else
                    {
                        System.IO.File.WriteAllText(dialog.FileName, BuildJsonTree(export), Encoding.UTF8);
                    }

                    LoggedMessageBox.Show(
                        "Дерево сохранено с координатами X/Y/Z.\r\n\r\n" +
                        "Теперь можно передавать два файла:\r\n" +
                        "1) исходный .ipt\r\n" +
                        "2) файл дерева: " + System.IO.Path.GetFileName(dialog.FileName));
                }
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem saving IPT browser tree");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}


        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS =====================================================================================================
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS 3. Apply edited name / XYZ to Inventor
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS -----------------------------------------------------------------------------------------------------
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Apply edited name / XYZ to Inventor — полный цикл: 127.339 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS └─ ButtonIptApplyBrowserTreeEdits_Click — 127.339 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    ├─ TryGetActivePartDocument — 0.031 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    ├─ MessageBox подтверждения Yes/No — 2.179 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │  └─ пользователь нажал Yes
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    ├─ подготовка / поиск / чтение изменённых строк до первого Move — 1.910 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │  ├─ чтение строк таблицы
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │  ├─ парсинг X/Y/Z
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │  ├─ проверка изменённых значений
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │  └─ подготовка тел к перемещению
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    ├─ блок перемещения тел — 20.370 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │  ├─ MoveSurfaceBodyByDatabaseVector
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │  │  └─ 15 тел — 6.578 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │  └─ служебные операции вокруг перемещения — около 13.792 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    ├─ RefreshIptGroupList — 0.562 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    ├─ RefreshIptFeatureList — 0.154 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    ├─ RefreshIptBrowserTree — 97.030 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │  └─ BuildBrowserTreeGridItems — 96.664 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │     └─ AppendBrowserTreeGridItems — 96.230 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │        ├─ TryGetNativeObjectFromBrowserNode
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │        │  └─ 270 вызовов — 4.320 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │        ├─ GetBrowserNodeDisplayName
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │        │  └─ 270 вызовов — 11.886 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    │        └─ остальная работа обхода дерева / координаты / строки таблицы — около 80 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS    └─ MessageBox "Изменения переданы в Inventor" — 5.046 с
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Инженерный вывод:
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS перемещение тел работает и не является главным узким местом.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Реальный MoveSurfaceBodyByDatabaseVector для 15 тел занял 6.578 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Основная стоимость полного шага — RefreshIptBrowserTree после применения изменений: 97.030 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS =====================================================================================================

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS INLINE MAP: Apply edited name / XYZ to Inventor — полный цикл 127.339 с.
        private void ButtonIptApplyBrowserTreeEdits_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptApplyBrowserTreeEdits_Click"))
            {
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 3 / подготовка: TryGetActivePartDocument — 0.031 с.
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            if (dataGridViewIptBrowserTree.Rows.Count == 0)
            {
                LoggedMessageBox.Show("Таблица дерева пустая. Сначала нажмите Refresh browser tree.");
                return;
            }

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 3 / пользовательское подтверждение: MessageBox Yes/No — 2.179 с.
            DialogResult answer = LoggedMessageBox.Show(
                "Передать изменения имени и координат X/Y/Z обратно в Inventor?\r\n\r\n" +
                "Важно: изменение координат для тел создаёт MoveFeature в истории детали.",
                "Apply edited tree to Inventor",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
            {
                return;
            }

            int renamedCount = 0;
            int movedCount = 0;
            int failedCount = 0;

            try
            {
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 3 / подготовка перед Move: чтение строк, парсинг X/Y/Z, проверка изменений — 1.910 с.
                dataGridViewIptBrowserTree.EndEdit();

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 3 / перебор строк таблицы: здесь ищутся изменённые Name и X/Y/Z.
                foreach (DataGridViewRow row in dataGridViewIptBrowserTree.Rows)
                {
                    BrowserTreeGridItem item = row.Tag as BrowserTreeGridItem;

                    if (item == null)
                    {
                        continue;
                    }

                    string newName = Convert.ToString(row.Cells[BrowserGridColName].Value);
                    newName = newName == null ? string.Empty : newName.Trim();

                    if (!string.IsNullOrWhiteSpace(newName) && newName != item.OriginalName)
                    {
                        if (TrySetInventorObjectName(item.NativeObject, item.Node, newName))
                        {
                            renamedCount++;
                            item.OriginalName = newName;
                        }
                        else
                        {
                            failedCount++;
                        }
                    }

                    if (item.CanMove && item.NativeObject is SurfaceBody)
                    {
                        double targetX;
                        double targetY;
                        double targetZ;

                        if (!TryParseGridDouble(row.Cells[BrowserGridColX].Value, out targetX) ||
                            !TryParseGridDouble(row.Cells[BrowserGridColY].Value, out targetY) ||
                            !TryParseGridDouble(row.Cells[BrowserGridColZ].Value, out targetZ))
                        {
                            continue;
                        }

                        double currentX;
                        double currentY;
                        double currentZ;

                        if (!TryGetObjectCenterInDocumentUnits(partDoc, item.NativeObject, out currentX, out currentY, out currentZ))
                        {
                            continue;
                        }

                        double dxDoc = targetX - currentX;
                        double dyDoc = targetY - currentY;
                        double dzDoc = targetZ - currentZ;

                        if (Math.Abs(dxDoc) < 0.000001 && Math.Abs(dyDoc) < 0.000001 && Math.Abs(dzDoc) < 0.000001)
                        {
                            continue;
                        }

                        double dxDb = ConvertDocumentLengthToDatabaseUnits(partDoc, dxDoc);
                        double dyDb = ConvertDocumentLengthToDatabaseUnits(partDoc, dyDoc);
                        double dzDb = ConvertDocumentLengthToDatabaseUnits(partDoc, dzDoc);

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 3 / перемещение: весь блок перемещения тел — 20.370 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Чистые MoveSurfaceBodyByDatabaseVector для 15 тел — 6.578 с; служебное окружение вокруг Move — около 13.792 с.
                        if (MoveSurfaceBodyByDatabaseVector(partDoc, (SurfaceBody)item.NativeObject, dxDb, dyDb, dzDb))
                        {
                            movedCount++;
                        }
                        else
                        {
                            failedCount++;
                        }
                    }
                }

                try
                {
                    partDoc.Update();
                }
                catch
                {
                }

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 3 / обновление UI: RefreshIptGroupList — 0.562 с; RefreshIptFeatureList — 0.154 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 3 / самая дорогая часть: RefreshIptBrowserTree — 97.030 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Внутри: BuildBrowserTreeGridItems — 96.664 с; AppendBrowserTreeGridItems — 96.230 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS TryGetNativeObjectFromBrowserNode: 270 вызовов — 4.320 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS GetBrowserNodeDisplayName: 270 вызовов — 11.886 с.
        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS Остальная работа обхода дерева / координаты / строки таблицы — около 80 с.
                RefreshIptGroupList(partDoc);
                RefreshIptFeatureList(partDoc);
                RefreshIptBrowserTree(partDoc);

        // 00:45 14.06.2026 InventorIptOrg_v0_4_5_INLINE_PERF_COMMENTS ЭТАП 3 / пользовательская обратная связь: MessageBox "Изменения переданы в Inventor" — 5.046 с.
                LoggedMessageBox.Show(
                    "Изменения переданы в Inventor.\r\n\r\n" +
                    "Переименовано: " + renamedCount.ToString() + "\r\n" +
                    "Перемещено тел: " + movedCount.ToString() + "\r\n" +
                    "Ошибок/пропусков: " + failedCount.ToString());
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem applying edited tree to Inventor");
                LoggedMessageBox.Show(ex.ToString());
            }
        
            }}

        // 23:32 17.06.2026 InventorIptOrg_v0_4_52_SECOND_REFRESH_FULL_COUNT — замер кнопки Refresh legacy browser tree
        // Источник: inventor_ipt_organizer_v0_4_52_20260617_233155_491.log
        // Refresh legacy browser tree — полный цикл кнопки: 32.778 с
        // ├─ ButtonIptLegacyRefreshBrowserTree_Click scope — 32.801997 с
        // ├─ TryGetActivePartDocument — 0.022130 с
        // └─ RefreshIptLegacyBrowserTree — 32.776927 с
        //    ├─ BuildBrowserTreeExport — 32.756575 с
        //    │  ├─ GetModelBrowserPane — 0.201337 с
        //    │  ├─ TryGetNativeObjectFromBrowserNode: 538 вызовов — 1.268609 с
        //    │  ├─ GetBrowserNodeDisplayName: 269 вызовов — 4.394074 с
        //    │  ├─ SafeGetNativeObjectKind: 269 вызовов — 0.897150 с
        //    │  ├─ TryGetObjectCenterInDocumentUnits: 269 вызовов — 20.444139 с
        //    │  │  └─ ConvertDatabaseLengthToDocumentUnits: 654 вызова — 6.237197 с
        //    │  └─ BuildBrowserTreeNode: 269 рекурсивных scope; сумма scope перекрывается рекурсией
        //    └─ UpdateIptLegacyBrowserTreeCaption — около 0.002 с
        // Инженерный вывод:
        // legacy TreeView быстрее UI-частью, но главный тормоз тот же: COM-центр/XYZ и конвертация единиц.
        // Ускорение должно идти через отключение XYZ или через кэш/snapshot, а не через замену TreeView/DataGridView.
        private void ButtonIptLegacyRefreshBrowserTree_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptLegacyRefreshBrowserTree_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                SetButtonTextSafe(ButtonIptLegacyRefreshBrowserTree, "Refresh legacy\r\nworking...");
                RefreshIptLegacyBrowserTree(partDoc);
            }
            finally
            {
                sw.Stop();
                SetTimedRefreshButtonText(ButtonIptLegacyRefreshBrowserTree, "Refresh legacy", sw.Elapsed);
                AppLogger.Log(
                    "LEGACY_BROWSER_TREE_REFRESH_BUTTON_SECONDS",
                    "ButtonIptLegacyRefreshBrowserTree_Click",
                    "ElapsedSeconds=" + sw.Elapsed.TotalSeconds.ToString("0.000", CultureInfo.InvariantCulture));
            }

            }}

        // 23:31-23:33 17.06.2026 InventorIptOrg_v0_4_52_SECOND_REFRESH_FULL_COUNT — замер кнопки Copy legacy tree to clipboard
        // Источник: inventor_ipt_organizer_v0_4_52_20260617_233155_491.log
        // ButtonIptLegacyCopyBrowserTree_Click — точного замера в этом логе нет.
        // ENTER/EXIT для ButtonIptLegacyCopyBrowserTree_Click: 0 / 0.
        // События clipboard/copy в логе: не найдены.
        // Инженерный вывод:
        // legacy copy пока не оценён этим логом. Для отдельного perf-блока нужно нажать Copy legacy tree на 269 узлах.
        private void ButtonIptLegacyCopyBrowserTree_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptLegacyCopyBrowserTree_Click"))
            {
            if (treeViewIptLegacyBrowserTree == null || treeViewIptLegacyBrowserTree.Nodes.Count == 0)
            {
                LoggedMessageBox.Show("Legacy tree пустое. Сначала нажмите Refresh legacy browser tree.");
                return;
            }

            string text = GetTreeViewText(treeViewIptLegacyBrowserTree);

            if (string.IsNullOrWhiteSpace(text))
            {
                LoggedMessageBox.Show("Не удалось получить текст legacy дерева.");
                return;
            }

            Clipboard.SetText(text);
            LoggedMessageBox.Show("Legacy дерево скопировано в буфер обмена.");

            }}

        private void ButtonIptLegacySaveBrowserTree_Click(object sender, EventArgs e)
        {
            using (AppLogger.Scope("ButtonIptLegacySaveBrowserTree_Click"))
            {
            if (!TryGetActivePartDocument(out PartDocument partDoc))
            {
                return;
            }

            try
            {
                BrowserTreeExport export = BuildBrowserTreeExport(partDoc);

                if (export == null || export.Root == null)
                {
                    LoggedMessageBox.Show("Не удалось построить legacy дерево браузера Inventor.");
                    return;
                }

                string defaultFileName = "inventor_ipt_legacy_tree.json";
                string initialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.DesktopDirectory);

                try
                {
                    if (!string.IsNullOrWhiteSpace(partDoc.FullFileName))
                    {
                        initialDirectory = System.IO.Path.GetDirectoryName(partDoc.FullFileName);
                        defaultFileName = System.IO.Path.GetFileNameWithoutExtension(partDoc.FullFileName) + ".legacy.tree.json";
                    }
                }
                catch
                {
                }

                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.Title = "Save IPT legacy browser tree JSON";
                    dialog.Filter = "JSON tree file (*.json)|*.json|Text tree file (*.txt)|*.txt|All files (*.*)|*.*";
                    dialog.FileName = defaultFileName;
                    dialog.InitialDirectory = initialDirectory;
                    dialog.OverwritePrompt = true;

                    if (dialog.ShowDialog(this) != DialogResult.OK)
                    {
                        return;
                    }

                    string extension = System.IO.Path.GetExtension(dialog.FileName).ToLowerInvariant();

                    if (extension == ".txt")
                    {
                        System.IO.File.WriteAllText(dialog.FileName, BuildPlainTextTree(export), Encoding.UTF8);
                    }
                    else
                    {
                        System.IO.File.WriteAllText(dialog.FileName, BuildJsonTree(export), Encoding.UTF8);
                    }

                    LoggedMessageBox.Show(
                        "Legacy дерево сохранено.\r\n\r\n" +
                        "Теперь можно передавать два файла:\r\n" +
                        "1) исходный .ipt\r\n" +
                        "2) файл legacy дерева: " + System.IO.Path.GetFileName(dialog.FileName));
                }
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem saving IPT legacy browser tree");
                LoggedMessageBox.Show(ex.ToString());
            }

            }}

        private void RefreshIptLegacyBrowserTree(PartDocument partDoc)
        {
            using (AppLogger.Scope("RefreshIptLegacyBrowserTree"))
            {
            if (partDoc == null || this.IsDisposed || treeViewIptLegacyBrowserTree == null)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                RunOnUiThread(delegate { RefreshIptLegacyBrowserTree(partDoc); });
                return;
            }

            try
            {
                BrowserTreeExport export = BuildBrowserTreeExport(partDoc);

                treeViewIptLegacyBrowserTree.BeginUpdate();
                treeViewIptLegacyBrowserTree.Nodes.Clear();

                if (export != null && export.Root != null)
                {
                    TreeNode rootNode = CreateTreeViewNode(export.Root);
                    treeViewIptLegacyBrowserTree.Nodes.Add(rootNode);
                    rootNode.Expand();
                }

                AppLogger.Log(
                    "LEGACY_BROWSER_TREE_REFRESHED",
                    "RefreshIptLegacyBrowserTree",
                    "TreeViewNodes=" + CountTreeViewNodes(treeViewIptLegacyBrowserTree.Nodes).ToString() +
                    "; SourceProject=MyFirstInventorPlugin_VS2017_Lesson5_CSharp_Tabs_IPT_IAM_BROWSER_TREE_EXPORT_FIX_IO_NAMES");
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem refreshing IPT legacy browser tree");
                LoggedMessageBox.Show(ex.ToString());
            }
            finally
            {
                try
                {
                    treeViewIptLegacyBrowserTree.EndUpdate();
                }
                catch
                {
                }

                UpdateIptLegacyBrowserTreeCaption();
                RefreshLayerCanvasTree();
            }

            }}

        private void UpdateIptLegacyBrowserTreeCaption()
        {
            using (AppLogger.Scope("UpdateIptLegacyBrowserTreeCaption"))
            {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                RunOnUiThread(UpdateIptLegacyBrowserTreeCaption);
                return;
            }

            int nodeCount = treeViewIptLegacyBrowserTree == null ? 0 : CountTreeViewNodes(treeViewIptLegacyBrowserTree.Nodes);

            if (labelIptLegacyBrowserTree != null)
            {
                labelIptLegacyBrowserTree.Text = "Legacy browser tree / старое дерево браузера Inventor: " + nodeCount.ToString();
            }

            }}

        private void RefreshIptBrowserTree(PartDocument partDoc)
        {
            using (AppLogger.Scope("RefreshIptBrowserTree"))
            {
            if (partDoc == null || this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                RunOnUiThread(delegate { RefreshIptBrowserTree(partDoc); });
                return;
            }

            try
            {
                List<BrowserTreeGridItem> items = BuildBrowserTreeGridItems(partDoc, 5000);
                PopulateIptBrowserTreeGridFromItems(items, "FullInventorBrowserRefresh");
            }
            catch (Exception ex)
            {
                LoggedMessageBox.Show("Problem refreshing IPT browser tree table");
                LoggedMessageBox.Show(ex.ToString());
            }
            finally
            {
                dataGridViewIptBrowserTree.ResumeLayout();
                UpdateIptBrowserTreeCaption();
            }
        
            }}

        private void PopulateBrowserTreePreviewAfterFolderCreate(
            PartDocument partDoc,
            string folderName,
            string innerFolderName,
            BrowserNode outerFolderBrowserNode,
            BrowserNode innerFolderBrowserNode,
            List<BrowserNode> featureNodes,
            bool createInnerFolderWithItems,
            bool trueNestedCreated,
            bool createdInnerOnlyFallback)
        {
            using (AppLogger.Scope("PopulateBrowserTreePreviewAfterFolderCreate"))
            {
            try
            {
                List<BrowserTreeGridItem> previewItems = new List<BrowserTreeGridItem>();

                if (createInnerFolderWithItems)
                {
                    if (trueNestedCreated)
                    {
                        previewItems.Add(CreateBrowserTreeGridItemFromNodeOrSynthetic(partDoc, outerFolderBrowserNode, folderName, "BrowserFolder", 0));
                        previewItems.Add(CreateBrowserTreeGridItemFromNodeOrSynthetic(partDoc, innerFolderBrowserNode, innerFolderName, "BrowserFolder", 1));

                        foreach (BrowserNode node in featureNodes)
                        {
                            previewItems.Add(CreateBrowserTreeGridItemFromNodeOrSynthetic(partDoc, node, GetBrowserNodeDisplayName(node), "BrowserNode", 2));
                        }
                    }
                    else
                    {
                        previewItems.Add(CreateBrowserTreeGridItemFromNodeOrSynthetic(partDoc, innerFolderBrowserNode, innerFolderName, "BrowserFolder", 0));

                        foreach (BrowserNode node in featureNodes)
                        {
                            previewItems.Add(CreateBrowserTreeGridItemFromNodeOrSynthetic(partDoc, node, GetBrowserNodeDisplayName(node), "BrowserNode", 1));
                        }
                    }
                }
                else
                {
                    previewItems.Add(CreateBrowserTreeGridItemFromNodeOrSynthetic(partDoc, outerFolderBrowserNode, folderName, "BrowserFolder", 0));

                    foreach (BrowserNode node in featureNodes)
                    {
                        previewItems.Add(CreateBrowserTreeGridItemFromNodeOrSynthetic(partDoc, node, GetBrowserNodeDisplayName(node), "BrowserNode", 1));
                    }
                }

                PopulateIptBrowserTreeGridFromItems(previewItems, "CreatedFolderPreview");

                AppLogger.Log(
                    "BROWSER_TREE_RESTORED_FROM_CREATED_FOLDER_PREVIEW",
                    "PopulateBrowserTreePreviewAfterFolderCreate",
                    "Rows=" + previewItems.Count.ToString() +
                    "; CreateInnerFolderWithItems=" + createInnerFolderWithItems.ToString() +
                    "; TrueNestedCreated=" + trueNestedCreated.ToString() +
                    "; CreatedInnerOnlyFallback=" + createdInnerOnlyFallback.ToString() +
                    "; FullRefreshSkipped=True");
            }
            catch (Exception ex)
            {
                AppLogger.LogException("PopulateBrowserTreePreviewAfterFolderCreate", ex);
            }
            }}

        private bool PopulateIptBrowserTreeGridInPlaceFromItems(List<BrowserTreeGridItem> items, string source)
        {
            using (AppLogger.Scope("PopulateIptBrowserTreeGridInPlaceFromItems"))
            {
            if (items == null)
            {
                items = new List<BrowserTreeGridItem>();
            }

            if (this.IsDisposed || dataGridViewIptBrowserTree == null)
            {
                return false;
            }

            if (this.InvokeRequired)
            {
                bool result = false;
                RunOnUiThread(delegate { result = PopulateIptBrowserTreeGridInPlaceFromItems(items, source); });
                return result;
            }

            int existingRows = dataGridViewIptBrowserTree.Rows.Count;

            if (existingRows != items.Count)
            {
                AppLogger.Log(
                    "BROWSER_TREE_GRID_INPLACE_REFRESH_SKIPPED",
                    "PopulateIptBrowserTreeGridInPlaceFromItems",
                    "Reason=RowCountMismatch" +
                    "; ExistingRows=" + existingRows.ToString() +
                    "; Items=" + items.Count.ToString() +
                    "; Source=" + source);
                return false;
            }

            dataGridViewIptBrowserTree.SuspendLayout();

            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    BrowserTreeGridItem item = items[i];
                    DataGridViewRow row = dataGridViewIptBrowserTree.Rows[i];

                    if (item == null || row == null)
                    {
                        continue;
                    }

                    // In-place update intentionally does NOT clear X/Y/Z cells.
                    // X/Y/Z remain editable values from the previous full refresh or user's edit.
                    row.Cells[BrowserGridColDepth].Value = item.Depth.ToString(CultureInfo.InvariantCulture);
                    row.Cells[BrowserGridColType].Value = item.ObjectKind;
                    row.Cells[BrowserGridColName].Value = item.Name;

                    row.Tag = item;
                    ApplySpatialCubesInfoToBrowserTreeRow(row, item);
                    ApplyEditableStateToBrowserTreeRow(row, item, true);
                }
            }
            finally
            {
                dataGridViewIptBrowserTree.ResumeLayout();
                UpdateIptBrowserTreeCaption();
            }

            AppLogger.Log(
                "BROWSER_TREE_GRID_INPLACE_REFRESHED",
                "PopulateIptBrowserTreeGridInPlaceFromItems",
                "Rows=" + dataGridViewIptBrowserTree.Rows.Count.ToString() +
                "; Source=" + source +
                "; PreserveXyzCellValues=True" +
                "; RowsClear=False" +
                "; RowsAdd=False");

            return true;

            }}

        private void ApplyEditableStateToBrowserTreeRow(DataGridViewRow row, BrowserTreeGridItem item, bool preserveXyzCellValues)
        {
            if (row == null || item == null)
            {
                return;
            }

            bool canRename = item.CanRename;
            bool canMove = item.CanMove && item.NativeObject is SurfaceBody;

            row.Cells[BrowserGridColName].ReadOnly = !canRename;
            row.Cells[BrowserGridColName].Style.BackColor = canRename ? System.Drawing.Color.White : System.Drawing.Color.Gainsboro;

            row.Cells[BrowserGridColX].ReadOnly = !canMove;
            row.Cells[BrowserGridColY].ReadOnly = !canMove;
            row.Cells[BrowserGridColZ].ReadOnly = !canMove;

            row.Cells[BrowserGridColX].Style.BackColor = canMove ? System.Drawing.Color.White : System.Drawing.Color.Gainsboro;
            row.Cells[BrowserGridColY].Style.BackColor = canMove ? System.Drawing.Color.White : System.Drawing.Color.Gainsboro;
            row.Cells[BrowserGridColZ].Style.BackColor = canMove ? System.Drawing.Color.White : System.Drawing.Color.Gainsboro;

            if (!preserveXyzCellValues)
            {
                row.Cells[BrowserGridColX].Value = item.HasCenter ? FormatGridDouble(item.X) : string.Empty;
                row.Cells[BrowserGridColY].Value = item.HasCenter ? FormatGridDouble(item.Y) : string.Empty;
                row.Cells[BrowserGridColZ].Value = item.HasCenter ? FormatGridDouble(item.Z) : string.Empty;
            }
            else if (item.HasCenter)
            {
                if (string.IsNullOrWhiteSpace(Convert.ToString(row.Cells[BrowserGridColX].Value)))
                {
                    row.Cells[BrowserGridColX].Value = FormatGridDouble(item.X);
                }

                if (string.IsNullOrWhiteSpace(Convert.ToString(row.Cells[BrowserGridColY].Value)))
                {
                    row.Cells[BrowserGridColY].Value = FormatGridDouble(item.Y);
                }

                if (string.IsNullOrWhiteSpace(Convert.ToString(row.Cells[BrowserGridColZ].Value)))
                {
                    row.Cells[BrowserGridColZ].Value = FormatGridDouble(item.Z);
                }
            }
        }

        private void PopulateIptBrowserTreeGridFromItems(List<BrowserTreeGridItem> items, string source)
        {
            using (AppLogger.Scope("PopulateIptBrowserTreeGridFromItems"))
            {
            if (items == null)
            {
                items = new List<BrowserTreeGridItem>();
            }

            if (this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                RunOnUiThread(delegate { PopulateIptBrowserTreeGridFromItems(items, source); });
                return;
            }

            dataGridViewIptBrowserTree.SuspendLayout();
            try
            {
                dataGridViewIptBrowserTree.Rows.Clear();

                foreach (BrowserTreeGridItem item in items)
                {
                    int rowIndex = dataGridViewIptBrowserTree.Rows.Add(
                        item.Depth.ToString(CultureInfo.InvariantCulture),
                        item.ObjectKind,
                        item.Name,
                        item.HasCenter ? FormatGridDouble(item.X) : string.Empty,
                        item.HasCenter ? FormatGridDouble(item.Y) : string.Empty,
                        item.HasCenter ? FormatGridDouble(item.Z) : string.Empty);

                    DataGridViewRow row = dataGridViewIptBrowserTree.Rows[rowIndex];
                    row.Tag = item;
                    ApplySpatialCubesInfoToBrowserTreeRow(row, item);

                    if (!item.CanRename)
                    {
                        row.Cells[BrowserGridColName].ReadOnly = true;
                        row.Cells[BrowserGridColName].Style.BackColor = System.Drawing.Color.Gainsboro;
                    }

                    if (!item.CanMove)
                    {
                        row.Cells[BrowserGridColX].ReadOnly = true;
                        row.Cells[BrowserGridColY].ReadOnly = true;
                        row.Cells[BrowserGridColZ].ReadOnly = true;
                        row.Cells[BrowserGridColX].Style.BackColor = System.Drawing.Color.Gainsboro;
                        row.Cells[BrowserGridColY].Style.BackColor = System.Drawing.Color.Gainsboro;
                        row.Cells[BrowserGridColZ].Style.BackColor = System.Drawing.Color.Gainsboro;
                    }
                }
            }
            finally
            {
                dataGridViewIptBrowserTree.ResumeLayout();
                UpdateIptBrowserTreeCaption();
            }

            AppLogger.Log(
                "BROWSER_TREE_GRID_POPULATED",
                "PopulateIptBrowserTreeGridFromItems",
                "Rows=" + dataGridViewIptBrowserTree.Rows.Count.ToString() +
                "; Source=" + source);
            }}

        private BrowserTreeGridItem CreateBrowserTreeGridItemFromNodeOrSynthetic(PartDocument partDoc, BrowserNode node, string fallbackName, string fallbackKind, int depth)
        {
            BrowserTreeGridItem item = new BrowserTreeGridItem();
            item.Node = node;
            item.NativeObject = node == null ? null : TryGetNativeObjectFromBrowserNode(node);
            item.Depth = depth;
            item.Name = node == null ? fallbackName : GetBrowserNodeDisplayName(node);
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                item.Name = fallbackName;
            }
            item.OriginalName = item.Name;
            item.ObjectKind = node == null ? fallbackKind : SafeGetNativeObjectKind(item.NativeObject, node);
            if (string.IsNullOrWhiteSpace(item.ObjectKind))
            {
                item.ObjectKind = fallbackKind;
            }

            double x;
            double y;
            double z;
            item.HasCenter = TryGetObjectCenterInDocumentUnits(partDoc, item.NativeObject, out x, out y, out z);
            item.X = x;
            item.Y = y;
            item.Z = z;
            item.CanMove = item.NativeObject is SurfaceBody;
            item.CanRename = node != null || item.NativeObject != null;
            return item;
        }

        private void UpdateIptBrowserTreeCaption()
        {
            using (AppLogger.Scope("UpdateIptBrowserTreeCaption"))
            {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.InvokeRequired)
            {
                RunOnUiThread(UpdateIptBrowserTreeCaption);
                return;
            }

            labelIptBrowserTree.Text = "Browser tree / дерево браузера Inventor: " + dataGridViewIptBrowserTree.Rows.Count.ToString() +
                "   | редактируемые колонки: Name, X, Y, Z";
        
            }}

        private List<BrowserTreeGridItem> BuildBrowserTreeGridItems(PartDocument partDoc, int maxCount)
        {
            using (AppLogger.Scope("BuildBrowserTreeGridItems"))
            {
            List<BrowserTreeGridItem> items = new List<BrowserTreeGridItem>();
            BrowserPane modelPane = GetModelBrowserPane(partDoc);

            if (modelPane == null)
            {
                LoggedMessageBox.Show(
                    "Не удалось получить Model BrowserPane текущей детали.\r\n\r\n" +
                    "Список BrowserPanes, который вернул Inventor:\r\n" +
                    GetBrowserPaneDebugList(partDoc));
                return items;
            }

            AppendBrowserTreeGridItems(partDoc, modelPane.TopNode, 0, items, new NodeCounter(maxCount));
            return items;
        
            }}

        private void AppendBrowserTreeGridItems(PartDocument partDoc, BrowserNode browserNode, int depth, List<BrowserTreeGridItem> items, NodeCounter counter)
        {
            using (AppLogger.Scope("AppendBrowserTreeGridItems"))
            {
            if (browserNode == null || counter.Count >= counter.MaxCount)
            {
                return;
            }

            counter.Count++;

            object nativeObject = TryGetNativeObjectFromBrowserNode(browserNode);
            string name = GetBrowserNodeDisplayName(browserNode);
            string kind = SafeGetNativeObjectKind(nativeObject, browserNode);

            double x;
            double y;
            double z;
            bool hasCenter = TryGetObjectCenterInDocumentUnits(partDoc, nativeObject, out x, out y, out z);

            BrowserTreeGridItem item = new BrowserTreeGridItem();
            item.Node = browserNode;
            item.NativeObject = nativeObject;
            item.Depth = depth;
            item.Name = name;
            item.OriginalName = name;
            item.ObjectKind = kind;
            item.HasCenter = hasCenter;
            item.X = x;
            item.Y = y;
            item.Z = z;
            item.CanMove = nativeObject is SurfaceBody;
            item.CanRename = nativeObject != null || browserNode != null;
            items.Add(item);

            if (depth > 25 || counter.Count >= counter.MaxCount)
            {
                return;
            }

            try
            {
                dynamic dynNode = browserNode;
                dynamic children = dynNode.BrowserNodes;
                int childCount = (int)children.Count;

                for (int i = 1; i <= childCount; i++)
                {
                    BrowserNode child = null;

                    try
                    {
                        child = (BrowserNode)children.Item(i);
                    }
                    catch
                    {
                        try
                        {
                            child = (BrowserNode)children[i];
                        }
                        catch
                        {
                            child = null;
                        }
                    }

                    if (child != null)
                    {
                        AppendBrowserTreeGridItems(partDoc, child, depth + 1, items, counter);
                    }

                    if (counter.Count >= counter.MaxCount)
                    {
                        break;
                    }
                }
            }
            catch
            {
            }
        
            }}

        private BrowserTreeExport BuildBrowserTreeExport(PartDocument partDoc)
        {
            using (AppLogger.Scope("BuildBrowserTreeExport"))
            {
            BrowserPane modelPane = GetModelBrowserPane(partDoc);

            if (modelPane == null)
            {
                LoggedMessageBox.Show(
                    "Не удалось получить Model BrowserPane текущей детали.\r\n\r\n" +
                    "Список BrowserPanes, который вернул Inventor:\r\n" +
                    GetBrowserPaneDebugList(partDoc));
                return null;
            }

            BrowserTreeExport export = new BrowserTreeExport();
            export.Format = "InventorIptBrowserTree";
            export.Version = "0.4.12";
            export.ExportedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            export.DocumentFullFileName = SafeGetPartFullFileName(partDoc);
            export.DocumentDisplayName = SafeGetPartDisplayName(partDoc);
            export.BrowserPaneName = SafeGetBrowserPaneName(modelPane);
            export.LengthUnits = SafeGetPartLengthUnits(partDoc);
            export.Root = BuildBrowserTreeNode(partDoc, modelPane.TopNode, 0, new NodeCounter(5000));
            return export;
        
            }}

        private static BrowserTreeNode BuildBrowserTreeNode(PartDocument partDoc, BrowserNode browserNode, int depth, NodeCounter counter)
        {
            using (AppLogger.Scope("BuildBrowserTreeNode"))
            {
            BrowserTreeNode result = new BrowserTreeNode();

            if (browserNode == null)
            {
                result.Name = "<null BrowserNode>";
                return result;
            }

            counter.Count++;
            result.Name = GetBrowserNodeDisplayName(browserNode);
            result.ObjectKind = SafeGetNativeObjectKind(TryGetNativeObjectFromBrowserNode(browserNode), browserNode);

            object nativeObject = TryGetNativeObjectFromBrowserNode(browserNode);
            double x;
            double y;
            double z;
            result.HasCenter = TryGetObjectCenterInDocumentUnits(partDoc, nativeObject, out x, out y, out z);
            result.X = x;
            result.Y = y;
            result.Z = z;

            if (depth > 25 || counter.Count >= counter.MaxCount)
            {
                return result;
            }

            try
            {
                dynamic dynNode = browserNode;
                dynamic children = dynNode.BrowserNodes;
                int childCount = (int)children.Count;

                for (int i = 1; i <= childCount; i++)
                {
                    if (counter.Count >= counter.MaxCount)
                    {
                        BrowserTreeNode truncated = new BrowserTreeNode();
                        truncated.Name = "<tree truncated: node limit reached>";
                        result.Children.Add(truncated);
                        break;
                    }

                    BrowserNode child = null;

                    try
                    {
                        child = (BrowserNode)children.Item(i);
                    }
                    catch
                    {
                        try
                        {
                            child = (BrowserNode)children[i];
                        }
                        catch
                        {
                            child = null;
                        }
                    }

                    if (child != null)
                    {
                        result.Children.Add(BuildBrowserTreeNode(partDoc, child, depth + 1, counter));
                    }
                }
            }
            catch
            {
            }

            return result;
        
            }}

        private static string GetBrowserNodeDisplayName(BrowserNode node)
        {
            using (AppLogger.Scope("GetBrowserNodeDisplayName"))
            {
            if (node == null)
            {
                return "<null>";
            }

            string text = null;

            try
            {
                dynamic dynNode = node;
                dynamic definition = dynNode.BrowserNodeDefinition;
                text = Convert.ToString(definition.Label);
            }
            catch
            {
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    dynamic dynNode = node;
                    dynamic definition = dynNode.BrowserNodeDefinition;
                    text = Convert.ToString(definition.DisplayName);
                }
                catch
                {
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    dynamic dynNode = node;
                    text = Convert.ToString(dynNode.FullPath);
                }
                catch
                {
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    dynamic dynNode = node;
                    text = Convert.ToString(dynNode.Name);
                }
                catch
                {
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    object nativeObject = TryGetNativeObjectFromBrowserNode(node);
                    dynamic nativeDyn = nativeObject;
                    text = Convert.ToString(nativeDyn.Name);
                }
                catch
                {
                }
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                text = "<BrowserNode>";
            }

            return text;
        
            }}

        private string GetBrowserTreeGridText()
        {
            using (AppLogger.Scope("GetBrowserTreeGridText"))
            {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Level\tType\tName\tX\tY\tZ\tCubeCount\tCubeIds");

            foreach (DataGridViewRow row in dataGridViewIptBrowserTree.Rows)
            {
                sb.Append(Convert.ToString(row.Cells[BrowserGridColDepth].Value));
                sb.Append('\t');
                sb.Append(Convert.ToString(row.Cells[BrowserGridColType].Value));
                sb.Append('\t');
                sb.Append(Convert.ToString(row.Cells[BrowserGridColName].Value));
                sb.Append('\t');
                sb.Append(Convert.ToString(row.Cells[BrowserGridColX].Value));
                sb.Append('\t');
                sb.Append(Convert.ToString(row.Cells[BrowserGridColY].Value));
                sb.Append('\t');
                sb.Append(Convert.ToString(row.Cells[BrowserGridColZ].Value));
                sb.Append('\t');
                sb.Append(dataGridViewIptBrowserTree.Columns.Count > BrowserGridColCubeCount ? Convert.ToString(row.Cells[BrowserGridColCubeCount].Value) : string.Empty);
                sb.Append('\t');
                sb.AppendLine(dataGridViewIptBrowserTree.Columns.Count > BrowserGridColCubeIds ? Convert.ToString(row.Cells[BrowserGridColCubeIds].Value) : string.Empty);
            }

            return sb.ToString();
        
            }}

        private static TreeNode CreateTreeViewNode(BrowserTreeNode node)
        {
            string text = node == null ? "<null>" : node.Name;

            if (node != null && !string.IsNullOrWhiteSpace(node.ObjectKind))
            {
                text += " [" + node.ObjectKind + "]";
            }

            if (node != null && node.HasCenter)
            {
                text += " (" +
                    node.X.ToString("0.###", CultureInfo.InvariantCulture) + ", " +
                    node.Y.ToString("0.###", CultureInfo.InvariantCulture) + ", " +
                    node.Z.ToString("0.###", CultureInfo.InvariantCulture) + ")";
            }

            TreeNode treeNode = new TreeNode(text);

            if (node != null)
            {
                foreach (BrowserTreeNode child in node.Children)
                {
                    treeNode.Nodes.Add(CreateTreeViewNode(child));
                }
            }

            return treeNode;
        }

        private static int CountTreeViewNodes(TreeNodeCollection nodes)
        {
            if (nodes == null)
            {
                return 0;
            }

            int count = 0;

            foreach (TreeNode node in nodes)
            {
                count++;
                count += CountTreeViewNodes(node.Nodes);
            }

            return count;
        }

        private static string GetTreeViewText(TreeView treeView)
        {
            StringBuilder sb = new StringBuilder();

            if (treeView == null)
            {
                return string.Empty;
            }

            foreach (TreeNode node in treeView.Nodes)
            {
                AppendTreeNodeText(sb, node, 0);
            }

            return sb.ToString();
        }

        private static void AppendTreeNodeText(StringBuilder sb, TreeNode node, int depth)
        {
            if (node == null)
            {
                return;
            }

            sb.Append(new string(' ', depth * 2));
            sb.AppendLine(node.Text);

            foreach (TreeNode child in node.Nodes)
            {
                AppendTreeNodeText(sb, child, depth + 1);
            }
        }

        private static string BuildPlainTextTree(BrowserTreeExport export)
        {
            using (AppLogger.Scope("BuildPlainTextTree"))
            {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Inventor IPT Browser Tree");
            sb.AppendLine("Version: " + export.Version);
            sb.AppendLine("ExportedAt: " + export.ExportedAt);
            sb.AppendLine("Document: " + export.DocumentFullFileName);
            sb.AppendLine("BrowserPane: " + export.BrowserPaneName);
            sb.AppendLine("LengthUnits: " + export.LengthUnits);
            sb.AppendLine();
            AppendBrowserTreeNodeText(sb, export.Root, 0);
            return sb.ToString();
        
            }}

        private static void AppendBrowserTreeNodeText(StringBuilder sb, BrowserTreeNode node, int depth)
        {
            using (AppLogger.Scope("AppendBrowserTreeNodeText"))
            {
            if (node == null)
            {
                return;
            }

            sb.Append(new string(' ', depth * 2));
            sb.Append(node.Name);
            sb.Append(" [");
            sb.Append(node.ObjectKind);
            sb.Append(']');

            if (node.HasCenter)
            {
                sb.Append("  XYZ=(");
                sb.Append(FormatGridDouble(node.X));
                sb.Append(", ");
                sb.Append(FormatGridDouble(node.Y));
                sb.Append(", ");
                sb.Append(FormatGridDouble(node.Z));
                sb.Append(')');
            }

            sb.AppendLine();

            foreach (BrowserTreeNode child in node.Children)
            {
                AppendBrowserTreeNodeText(sb, child, depth + 1);
            }
        
            }}

        private static string BuildJsonTree(BrowserTreeExport export)
        {
            using (AppLogger.Scope("BuildJsonTree"))
            {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            AppendJsonProperty(sb, 1, "format", export.Format, true);
            AppendJsonProperty(sb, 1, "version", export.Version, true);
            AppendJsonProperty(sb, 1, "exportedAt", export.ExportedAt, true);
            AppendJsonProperty(sb, 1, "documentDisplayName", export.DocumentDisplayName, true);
            AppendJsonProperty(sb, 1, "documentFullFileName", export.DocumentFullFileName, true);
            AppendJsonProperty(sb, 1, "browserPaneName", export.BrowserPaneName, true);
            AppendJsonProperty(sb, 1, "lengthUnits", export.LengthUnits, true);
            sb.AppendLine("  \"tree\":");
            AppendJsonBrowserTreeNode(sb, export.Root, 1);
            sb.AppendLine();
            sb.AppendLine("}");
            return sb.ToString();
        
            }}

        private static void AppendJsonProperty(StringBuilder sb, int indent, string name, string value, bool comma)
        {
            using (AppLogger.Scope("AppendJsonProperty"))
            {
            sb.Append(new string(' ', indent * 2));
            sb.Append('"');
            sb.Append(JsonEscape(name));
            sb.Append("\": ");
            sb.Append('"');
            sb.Append(JsonEscape(value));
            sb.Append('"');

            if (comma)
            {
                sb.Append(',');
            }

            sb.AppendLine();
        
            }}

        private static void AppendJsonNumberOrNullProperty(StringBuilder sb, int indent, string name, bool hasValue, double value, bool comma)
        {
            using (AppLogger.Scope("AppendJsonNumberOrNullProperty"))
            {
            sb.Append(new string(' ', indent * 2));
            sb.Append('"');
            sb.Append(JsonEscape(name));
            sb.Append("\": ");

            if (hasValue)
            {
                sb.Append(value.ToString("0.######", CultureInfo.InvariantCulture));
            }
            else
            {
                sb.Append("null");
            }

            if (comma)
            {
                sb.Append(',');
            }

            sb.AppendLine();
        
            }}

        private static void AppendJsonBoolProperty(StringBuilder sb, int indent, string name, bool value, bool comma)
        {
            using (AppLogger.Scope("AppendJsonBoolProperty"))
            {
            sb.Append(new string(' ', indent * 2));
            sb.Append('"');
            sb.Append(JsonEscape(name));
            sb.Append("\": ");
            sb.Append(value ? "true" : "false");

            if (comma)
            {
                sb.Append(',');
            }

            sb.AppendLine();
        
            }}

        private static void AppendJsonBrowserTreeNode(StringBuilder sb, BrowserTreeNode node, int indent)
        {
            using (AppLogger.Scope("AppendJsonBrowserTreeNode"))
            {
            if (node == null)
            {
                sb.Append("null");
                return;
            }

            sb.Append(new string(' ', indent * 2));
            sb.AppendLine("{");
            AppendJsonProperty(sb, indent + 1, "name", node.Name, true);
            AppendJsonProperty(sb, indent + 1, "type", node.ObjectKind, true);
            AppendJsonBoolProperty(sb, indent + 1, "hasCenter", node.HasCenter, true);
            AppendJsonNumberOrNullProperty(sb, indent + 1, "x", node.HasCenter, node.X, true);
            AppendJsonNumberOrNullProperty(sb, indent + 1, "y", node.HasCenter, node.Y, true);
            AppendJsonNumberOrNullProperty(sb, indent + 1, "z", node.HasCenter, node.Z, true);
            sb.Append(new string(' ', (indent + 1) * 2));
            sb.AppendLine("\"children\": [");

            for (int i = 0; i < node.Children.Count; i++)
            {
                AppendJsonBrowserTreeNode(sb, node.Children[i], indent + 2);

                if (i < node.Children.Count - 1)
                {
                    sb.Append(',');
                }

                sb.AppendLine();
            }

            sb.Append(new string(' ', (indent + 1) * 2));
            sb.AppendLine("]");
            sb.Append(new string(' ', indent * 2));
            sb.Append('}');
        
            }}

        private static string JsonEscape(string value)
        {
            using (AppLogger.Scope("JsonEscape"))
            {
            if (value == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();

            foreach (char c in value)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '"': sb.Append("\\\""); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (char.IsControl(c))
                        {
                            sb.Append("\\u");
                            sb.Append(((int)c).ToString("x4"));
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }

            return sb.ToString();
        
            }}

        private static string SafeGetPartFullFileName(PartDocument partDoc)
        {
            using (AppLogger.Scope("SafeGetPartFullFileName"))
            {
            try
            {
                return partDoc.FullFileName;
            }
            catch
            {
                return string.Empty;
            }
        
            }}

        private static string SafeGetPartDisplayName(PartDocument partDoc)
        {
            using (AppLogger.Scope("SafeGetPartDisplayName"))
            {
            try
            {
                return partDoc.DisplayName;
            }
            catch
            {
                return string.Empty;
            }
        
            }}

        private static string SafeGetBrowserPaneName(BrowserPane pane)
        {
            using (AppLogger.Scope("SafeGetBrowserPaneName"))
            {
            try
            {
                return pane.Name;
            }
            catch
            {
                return string.Empty;
            }
        
            }}

        private static string SafeGetPartLengthUnits(PartDocument partDoc)
        {
            using (AppLogger.Scope("SafeGetPartLengthUnits"))
            {
            try
            {
                UnitsOfMeasure uom = partDoc.UnitsOfMeasure;
                return Convert.ToString(uom.LengthUnits);
            }
            catch
            {
                return "document units";
            }
        
            }}

        private static object TryGetNativeObjectFromBrowserNode(BrowserNode node)
        {
            using (AppLogger.Scope("TryGetNativeObjectFromBrowserNode"))
            {
            if (node == null)
            {
                return null;
            }

            try
            {
                dynamic dynNode = node;
                object nativeObject = dynNode.NativeObject;

                if (nativeObject != null)
                {
                    return nativeObject;
                }
            }
            catch
            {
            }

            try
            {
                dynamic dynNode = node;
                dynamic definition = dynNode.BrowserNodeDefinition;
                object nativeObject = definition.NativeObject;

                if (nativeObject != null)
                {
                    return nativeObject;
                }
            }
            catch
            {
            }

            return null;
        
            }}

        private static string SafeGetNativeObjectKind(object nativeObject, BrowserNode node)
        {
            using (AppLogger.Scope("SafeGetNativeObjectKind"))
            {
            if (nativeObject == null)
            {
                return "BrowserNode";
            }

            if (nativeObject is SurfaceBody)
            {
                return "SurfaceBody";
            }

            try
            {
                dynamic dynObj = nativeObject;
                string typeText = Convert.ToString(dynObj.Type);

                if (!string.IsNullOrWhiteSpace(typeText))
                {
                    return typeText;
                }
            }
            catch
            {
            }

            try
            {
                return nativeObject.GetType().Name;
            }
            catch
            {
                return "InventorObject";
            }
        
            }}

        private static bool TryGetObjectCenterInDocumentUnits(PartDocument partDoc, object nativeObject, out double x, out double y, out double z)
        {
            using (AppLogger.Scope("TryGetObjectCenterInDocumentUnits"))
            {
            x = 0;
            y = 0;
            z = 0;

            if (partDoc == null || nativeObject == null)
            {
                return false;
            }

            try
            {
                dynamic dynObj = nativeObject;
                Box box = (Box)dynObj.RangeBox;

                if (box == null)
                {
                    return false;
                }

                double centerX = (box.MinPoint.X + box.MaxPoint.X) / 2.0;
                double centerY = (box.MinPoint.Y + box.MaxPoint.Y) / 2.0;
                double centerZ = (box.MinPoint.Z + box.MaxPoint.Z) / 2.0;

                x = ConvertDatabaseLengthToDocumentUnits(partDoc, centerX);
                y = ConvertDatabaseLengthToDocumentUnits(partDoc, centerY);
                z = ConvertDatabaseLengthToDocumentUnits(partDoc, centerZ);
                return true;
            }
            catch
            {
                return false;
            }
        
            }}

        private static double ConvertDatabaseLengthToDocumentUnits(PartDocument partDoc, double databaseValue)
        {
            using (AppLogger.Scope("ConvertDatabaseLengthToDocumentUnits"))
            {
            try
            {
                UnitsOfMeasure uom = partDoc.UnitsOfMeasure;
                return uom.ConvertUnits(databaseValue, UnitsTypeEnum.kDatabaseLengthUnits, uom.LengthUnits);
            }
            catch
            {
                return databaseValue;
            }
        
            }}

        private static double ConvertDocumentLengthToDatabaseUnits(PartDocument partDoc, double documentValue)
        {
            using (AppLogger.Scope("ConvertDocumentLengthToDatabaseUnits"))
            {
            try
            {
                UnitsOfMeasure uom = partDoc.UnitsOfMeasure;
                return uom.ConvertUnits(documentValue, uom.LengthUnits, UnitsTypeEnum.kDatabaseLengthUnits);
            }
            catch
            {
                return documentValue;
            }
        
            }}

        private bool MoveSurfaceBodyByDatabaseVector(PartDocument partDoc, SurfaceBody body, double dx, double dy, double dz)
        {
            using (AppLogger.Scope("MoveSurfaceBodyByDatabaseVector"))
            {
            if (partDoc == null || body == null)
            {
                return false;
            }

            if (Math.Abs(dx) < 0.0000001 && Math.Abs(dy) < 0.0000001 && Math.Abs(dz) < 0.0000001)
            {
                return true;
            }

            try
            {
                ObjectCollection bodies = _invApp.TransientObjects.CreateObjectCollection();
                bodies.Add(body);

                dynamic compDef = partDoc.ComponentDefinition;
                dynamic moveDef = compDef.Features.MoveFeatures.CreateMoveDefinition(bodies);
                moveDef.AddFreeDrag(dx, dy, dz);
                compDef.Features.MoveFeatures.Add(moveDef);
                return true;
            }
            catch
            {
                return false;
            }
        
            }}

        private static bool TrySetInventorObjectName(object nativeObject, BrowserNode node, string newName)
        {
            using (AppLogger.Scope("TrySetInventorObjectName"))
            {
            if (string.IsNullOrWhiteSpace(newName))
            {
                return false;
            }

            if (nativeObject != null)
            {
                try
                {
                    dynamic dynObj = nativeObject;
                    dynObj.Name = newName;
                    return true;
                }
                catch
                {
                }
            }

            if (node != null)
            {
                try
                {
                    dynamic dynNode = node;
                    dynamic definition = dynNode.BrowserNodeDefinition;
                    definition.Label = newName;
                    return true;
                }
                catch
                {
                }

                try
                {
                    dynamic dynNode = node;
                    dynNode.Name = newName;
                    return true;
                }
                catch
                {
                }
            }

            return false;
        
            }}

        private static string FormatGridDouble(double value)
        {
            using (AppLogger.Scope("FormatGridDouble"))
            {
            return value.ToString("0.###", CultureInfo.InvariantCulture);
        
            }}

        private static bool TryParseGridDouble(object value, out double result)
        {
            using (AppLogger.Scope("TryParseGridDouble"))
            {
            result = 0;
            string text = Convert.ToString(value);

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            text = text.Trim();

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out result))
            {
                return true;
            }

            text = text.Replace(',', '.');
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
        
            }}

        private class BrowserTreeNamesOnlySnapshot
        {
            public BrowserTreeNamesOnlySnapshot()
            {
                GridItems = new List<BrowserTreeGridItem>();
                BuiltAt = string.Empty;
                DocumentKey = string.Empty;
            }

            public string DocumentKey { get; set; }
            public string BuiltAt { get; set; }
            public int NodeCount { get; set; }
            public int NativeObjectRows { get; set; }
            public int SpatialBodyRows { get; set; }
            public int SpatialCenterRows { get; set; }
            public List<BrowserTreeGridItem> GridItems { get; private set; }
            public BrowserTreeNode RootNode { get; set; }
        }

        private class BrowserTreeExport
        {
            public string Format { get; set; }
            public string Version { get; set; }
            public string ExportedAt { get; set; }
            public string DocumentDisplayName { get; set; }
            public string DocumentFullFileName { get; set; }
            public string BrowserPaneName { get; set; }
            public string LengthUnits { get; set; }
            public BrowserTreeNode Root { get; set; }
        }

        private class BrowserTreeNode
        {
            public BrowserTreeNode()
            {
                using (AppLogger.Scope("BrowserTreeNode"))
                {
                Children = new List<BrowserTreeNode>();
                ObjectKind = string.Empty;
            
                }}

            public string Name { get; set; }
            public string ObjectKind { get; set; }
            public bool HasCenter { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public List<BrowserTreeNode> Children { get; private set; }
        }

        private class BrowserTreeGridItem
        {
            public BrowserNode Node { get; set; }
            public object NativeObject { get; set; }
            public int Depth { get; set; }
            public string Name { get; set; }
            public string OriginalName { get; set; }
            public string ObjectKind { get; set; }
            public bool HasCenter { get; set; }
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
            public bool CanMove { get; set; }
            public bool CanRename { get; set; }
        }

        private class NodeCounter
        {
            public NodeCounter(int maxCount)
            {
                using (AppLogger.Scope("NodeCounter"))
                {
                MaxCount = maxCount;
            
                }}

            public int Count { get; set; }
            public int MaxCount { get; private set; }
        }

        private static BrowserPane GetModelBrowserPane(PartDocument partDoc)
        {
            using (AppLogger.Scope("GetModelBrowserPane"))
            {
            if (partDoc == null)
            {
                return null;
            }

            dynamic panes = null;

            try
            {
                panes = partDoc.BrowserPanes;
            }
            catch
            {
                return null;
            }

            // В английском Inventor панель браузера модели обычно называется "Model".
            // В локализованных версиях Inventor видимое имя может быть переведено,
            // поэтому сначала пробуем распространённые имена, затем ActivePane и в конце
            // перебираем все панели браузера.
            string[] preferredNames = new string[]
            {
                "Model",
                "Модель",
                "Modell",
                "Modelo",
                "Modèle"
            };

            foreach (string paneName in preferredNames)
            {
                BrowserPane paneByName = TryGetBrowserPaneByName(panes, paneName);

                if (IsUsableBrowserPane(paneByName))
                {
                    return paneByName;
                }
            }

            try
            {
                BrowserPane activePane = (BrowserPane)panes.ActivePane;

                if (IsUsableBrowserPane(activePane))
                {
                    return activePane;
                }
            }
            catch
            {
            }

            try
            {
                int count = (int)panes.Count;

                for (int i = 1; i <= count; i++)
                {
                    BrowserPane paneByIndex = null;

                    try
                    {
                        paneByIndex = (BrowserPane)panes.Item(i);
                    }
                    catch
                    {
                        try
                        {
                            paneByIndex = (BrowserPane)panes[i];
                        }
                        catch
                        {
                            paneByIndex = null;
                        }
                    }

                    if (IsUsableBrowserPane(paneByIndex))
                    {
                        return paneByIndex;
                    }
                }
            }
            catch
            {
            }

            return null;
        
            }}

        private static BrowserPane TryGetBrowserPaneByName(dynamic panes, string paneName)
        {
            using (AppLogger.Scope("TryGetBrowserPaneByName"))
            {
            if (panes == null || string.IsNullOrEmpty(paneName))
            {
                return null;
            }

            try
            {
                return (BrowserPane)panes.Item(paneName);
            }
            catch
            {
                try
                {
                    return (BrowserPane)panes[paneName];
                }
                catch
                {
                    return null;
                }
            }
        
            }}

        private static bool IsUsableBrowserPane(BrowserPane pane)
        {
            using (AppLogger.Scope("IsUsableBrowserPane"))
            {
            if (pane == null)
            {
                return false;
            }

            try
            {
                BrowserNode topNode = pane.TopNode;
                return topNode != null;
            }
            catch
            {
                return false;
            }
        
            }}

        private static string GetBrowserPaneDebugList(PartDocument partDoc)
        {
            using (AppLogger.Scope("GetBrowserPaneDebugList"))
            {
            if (partDoc == null)
            {
                return "partDoc is null";
            }

            try
            {
                dynamic panes = partDoc.BrowserPanes;
                int count = (int)panes.Count;
                StringBuilder sb = new StringBuilder();

                for (int i = 1; i <= count; i++)
                {
                    try
                    {
                        BrowserPane pane = (BrowserPane)panes.Item(i);
                        string name = "";

                        try { name = pane.Name; } catch { name = "<no name>"; }

                        sb.AppendLine(i.ToString() + ": " + name);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine(i.ToString() + ": <error> " + ex.Message);
                    }
                }

                if (sb.Length == 0)
                {
                    return "BrowserPanes collection is empty";
                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        
            }}

        private static bool TryGetBrowserNodeFromObject(BrowserPane modelPane, object nativeObject, out BrowserNode node)
        {
            using (AppLogger.Scope("TryGetBrowserNodeFromObject"))
            {
            node = null;

            if (modelPane == null || nativeObject == null)
            {
                return false;
            }

            try
            {
                node = modelPane.GetBrowserNodeFromObject(nativeObject);
                return node != null;
            }
            catch
            {
                try
                {
                    dynamic dynPane = modelPane;
                    node = (BrowserNode)dynPane.GetBrowserNodeFromObject(nativeObject);
                    return node != null;
                }
                catch
                {
                    node = null;
                    return false;
                }
            }
        
            }}

        private class BodyListItem
        {
            public BodyListItem(SurfaceBody body, string displayName, IntPtr identityKey)
            {
                using (AppLogger.Scope("BodyListItem"))
                {
                Body = body;
                DisplayName = displayName;
                IdentityKey = identityKey;
            
                }}

            public SurfaceBody Body { get; private set; }
            public string DisplayName { get; private set; }
            public IntPtr IdentityKey { get; private set; }

            public override string ToString()
            {
                using (AppLogger.Scope("ToString"))
                {
                return DisplayName;
            
                }}
        }

        private class FeatureListItem
        {
            public FeatureListItem(object feature, string displayName, IntPtr identityKey)
            {
                using (AppLogger.Scope("FeatureListItem"))
                {
                Feature = feature;
                DisplayName = displayName;
                IdentityKey = identityKey;
            
                }}

            public object Feature { get; private set; }
            public string DisplayName { get; private set; }
            public IntPtr IdentityKey { get; private set; }

            public override string ToString()
            {
                using (AppLogger.Scope("ToString"))
                {
                return DisplayName;
            
                }}
        }

        private class BoxLimits
        {
            public bool HasValue { get; private set; }

            private double _minX;
            private double _minY;
            private double _minZ;
            private double _maxX;
            private double _maxY;
            private double _maxZ;

            public void Include(Box box)
            {
                using (AppLogger.Scope("Include"))
                {
                Point min = box.MinPoint;
                Point max = box.MaxPoint;

                if (!HasValue)
                {
                    _minX = min.X;
                    _minY = min.Y;
                    _minZ = min.Z;
                    _maxX = max.X;
                    _maxY = max.Y;
                    _maxZ = max.Z;
                    HasValue = true;
                    return;
                }

                _minX = Math.Min(_minX, min.X);
                _minY = Math.Min(_minY, min.Y);
                _minZ = Math.Min(_minZ, min.Z);
                _maxX = Math.Max(_maxX, max.X);
                _maxY = Math.Max(_maxY, max.Y);
                _maxZ = Math.Max(_maxZ, max.Z);
            
                }}

            public SpatialBox ToSpatialBox()
            {
                using (AppLogger.Scope("BoxLimits.ToSpatialBox"))
                {
                if (!HasValue)
                {
                    return null;
                }

                return new SpatialBox(_minX, _minY, _minZ, _maxX, _maxY, _maxZ);
            
                }}

            public bool Intersects(Box box, double tolerance)
            {
                using (AppLogger.Scope("Intersects"))
                {
                Point min = box.MinPoint;
                Point max = box.MaxPoint;

                return !(max.X < _minX - tolerance || min.X > _maxX + tolerance ||
                         max.Y < _minY - tolerance || min.Y > _maxY + tolerance ||
                         max.Z < _minZ - tolerance || min.Z > _maxZ + tolerance);
            
                }}

            public bool Contains(Box box, double tolerance)
            {
                using (AppLogger.Scope("Contains"))
                {
                Point min = box.MinPoint;
                Point max = box.MaxPoint;

                return min.X >= _minX - tolerance && max.X <= _maxX + tolerance &&
                       min.Y >= _minY - tolerance && max.Y <= _maxY + tolerance &&
                       min.Z >= _minZ - tolerance && max.Z <= _maxZ + tolerance;
            
                }}
        }

        private static SurfaceBody GetSurfaceBodyFromSelectedObject(object selectedObject)
        {
            using (AppLogger.Scope("GetSurfaceBodyFromSelectedObject"))
            {
            SurfaceBody body = selectedObject as SurfaceBody;
            if (body != null)
            {
                return body;
            }

            Face face = selectedObject as Face;
            if (face != null)
            {
                return face.SurfaceBody;
            }

            // Дополнительный запасной вариант для разных оболочек Inventor Interop.
            // Некоторые выбранные объекты имеют SurfaceBody, но здесь напрямую не приводятся к Face.
            try
            {
                dynamic dynObj = selectedObject;
                return dynObj.SurfaceBody as SurfaceBody;
            }
            catch
            {
                return null;
            }
        
            }}

        private static IntPtr GetComIdentityKey(object comObject)
        {
            using (AppLogger.Scope("GetComIdentityKey"))
            {
            if (comObject == null)
            {
                return IntPtr.Zero;
            }

            IntPtr unknown = IntPtr.Zero;

            try
            {
                unknown = Marshal.GetIUnknownForObject(comObject);
                return unknown;
            }
            catch
            {
                return IntPtr.Zero;
            }
            finally
            {
                if (unknown != IntPtr.Zero)
                {
                    Marshal.Release(unknown);
                }
            }
        
            }}

        private static bool AttributeSetNameIsUsed(AttributeSets attributeSets, string name)
        {
            using (AppLogger.Scope("AttributeSetNameIsUsed"))
            {
            // Позднее связывание сохраняет совместимость этого C#-перевода с версиями Inventor Interop,
            // где VB-вызов NameIsUsed("...") в C# выглядит иначе.
            dynamic sets = attributeSets;
            return sets.NameIsUsed(name);
        
            }}
    }

    internal sealed class BufferedViewerPanel : Panel
    {
        public BufferedViewerPanel()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            DoubleBuffered = true;
            UpdateStyles();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // v0.5.08: the completed explicit backbuffer covers the full client area.
            // Suppressing WM_ERASEBKGND removes the white flash between interactive frames.
        }
    }

    internal sealed class CustomRectangleOverlay : Form
    {
        private readonly System.Drawing.Rectangle inventorViewRectangle;
        private readonly string helpText;
        private bool dragging;
        private System.Drawing.Point dragStart;
        private System.Drawing.Point dragCurrent;

        public CustomRectangleOverlay(System.Drawing.Rectangle inventorViewRectangle, string helpText)
        {
            this.inventorViewRectangle = inventorViewRectangle;
            this.helpText = helpText ?? string.Empty;
            this.SelectedRectangle = System.Drawing.Rectangle.Empty;

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.Bounds = SystemInformation.VirtualScreen;
            this.TopMost = true;
            this.ShowInTaskbar = false;
            this.KeyPreview = true;
            this.BackColor = System.Drawing.Color.Black;
            this.Opacity = 0.12;
            this.Cursor = Cursors.Cross;
            this.DoubleBuffered = true;
        }

        public System.Drawing.Rectangle SelectedRectangle { get; private set; }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            dragging = true;
            dragStart = e.Location;
            dragCurrent = e.Location;
            SelectedRectangle = System.Drawing.Rectangle.Empty;
            Capture = true;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!dragging)
            {
                return;
            }

            dragCurrent = e.Location;
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (!dragging || e.Button != MouseButtons.Left)
            {
                return;
            }

            dragging = false;
            Capture = false;
            dragCurrent = e.Location;
            SelectedRectangle = NormalizeRectangle(ToScreenPoint(dragStart), ToScreenPoint(dragCurrent));
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                using (System.Drawing.Pen viewPen = new System.Drawing.Pen(System.Drawing.Color.DodgerBlue, 2.0f))
                using (System.Drawing.Pen selectionPen = new System.Drawing.Pen(System.Drawing.Color.Lime, 3.0f))
                using (System.Drawing.SolidBrush textBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                using (System.Drawing.Font font = new System.Drawing.Font("Segoe UI", 12.0f, System.Drawing.FontStyle.Bold))
                {
                    System.Drawing.Rectangle localViewRect = ToLocalRectangle(inventorViewRectangle);
                    e.Graphics.DrawRectangle(viewPen, localViewRect);

                    string text = helpText + "\r\nBlue = Inventor view. Green = custom rectangle.";
                    e.Graphics.DrawString(text, font, textBrush, localViewRect.Left + 12, Math.Max(10, localViewRect.Top + 12));

                    if (dragging)
                    {
                        System.Drawing.Rectangle screenRect = NormalizeRectangle(ToScreenPoint(dragStart), ToScreenPoint(dragCurrent));
                        System.Drawing.Rectangle localRect = ToLocalRectangle(screenRect);
                        e.Graphics.DrawRectangle(selectionPen, localRect);
                    }
                }
            }
            catch
            {
            }
        }

        private System.Drawing.Point ToScreenPoint(System.Drawing.Point localPoint)
        {
            return new System.Drawing.Point(this.Left + localPoint.X, this.Top + localPoint.Y);
        }

        private System.Drawing.Rectangle ToLocalRectangle(System.Drawing.Rectangle screenRect)
        {
            return new System.Drawing.Rectangle(
                screenRect.Left - this.Left,
                screenRect.Top - this.Top,
                screenRect.Width,
                screenRect.Height);
        }

        private static System.Drawing.Rectangle NormalizeRectangle(System.Drawing.Point a, System.Drawing.Point b)
        {
            int left = Math.Min(a.X, b.X);
            int right = Math.Max(a.X, b.X);
            int top = Math.Min(a.Y, b.Y);
            int bottom = Math.Max(a.Y, b.Y);

            return System.Drawing.Rectangle.FromLTRB(left, top, right, bottom);
        }
    }
}
