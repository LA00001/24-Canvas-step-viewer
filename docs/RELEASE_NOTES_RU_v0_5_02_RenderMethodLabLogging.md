# v0.5.02 — RenderMethodLabLogging

Build name: `RENDER_METHOD_LAB_LOGGING`

## Цель

Не угадывать один следующий профиль освещения, а дать несколько методов в том же выпадающем списке `Mode`, чтобы пользователь мог быстро переключить их на одной геометрии и прислать один лог с объективным сравнением.

## Новые пункты Mode

1. `Mode: Surface+Mesh` — точная база v0.5.01.
2. `Mode: Mesh Deep Clamp` — исправленный диапазон 115..218 без старого pre-clamp 145..214.
3. `Mode: Mesh Two-Light` — основной и заполняющий направленные источники.
4. `Mode: Mesh Hemisphere` — hemisphere ambient + key light.
5. `Mode: Mesh Inventor Lab` — контрастный CAD-профиль key/fill/facing/top/rim + gamma.

Базовые `Wire`, `Surface`, `Surface+Edges`, `Ghost` и `Points` сохранены. По умолчанию выбран `Mesh Inventor Lab`.

## Логирование

При каждом переключении создаётся запись:

`VIEWER_RENDER_METHOD_SELECTED`

В неё входят:

- `RenderMethod`;
- `DisplayName`;
- `LightModel`;
- `InputShadeClamp`;
- `SurfaceShadeClamp`;
- `SurfacePalette`;
- `MeshLineRgb`;
- `FeatureLineRgb`;
- `SilhouetteLineRgb`;
- `MeshDepthToleranceFactor`;
- `FeatureDepthToleranceFactor`;
- `SilhouetteDepthToleranceFactor`;
- пояснение `Notes`.

После фактической перерисовки `RenderMethod` повторяется во всех основных `STEP_ZBUFFER_*`, `STEP_SURFACE_SHELL_STATS` и `STEP_LOCAL_DRAW_SECONDS`.

## Исправленная диагностическая ошибка v0.5.01

В v0.5.01 был заявлен финальный surface clamp `115..218`, но перед barycentric interpolation сохранялся старый vertex pre-clamp `145..214`. Это не ломало renderer, однако ограничивало реальную глубину теней.

В v0.5.02:

- `Surface+Mesh` намеренно сохраняет это поведение как точный baseline;
- `Mesh Deep Clamp` использует input/output clamp `115..218` и показывает реальный эффект исправления;
- остальные новые методы имеют собственные диапазоны и формулы.

## Сохранено

- software Z-buffer;
- hidden-line removal;
- POLY_LOOP mesh из `StepLiteScene.Edges`;
- `TriangleDiagonalsIncluded=False`;
- AdvancedFace orientation;
- Gouraud interpolation и quantized normal merge;
- FastDragWireframe;
- отсутствие автозапуска Inventor;
- `.ipt canvas` по умолчанию;
- `Cube hit: filtered bodies` по умолчанию.

## Контрольный тест

Открыть Rubber Hose и последовательно выбрать:

`Surface+Mesh → Mesh Deep Clamp → Mesh Two-Light → Mesh Hemisphere → Mesh Inventor Lab`

На каждом пункте дождаться полной перерисовки. Затем прислать один скрин наиболее удачного метода и один полный лог v0.5.02.

Visual Studio и Inventor при подготовке пакета не запускались; выполнены статические проверки исходников и ZIP.
