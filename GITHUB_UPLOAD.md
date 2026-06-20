# GitHub upload — v0.5.17

## Repository

`24-Inventor-ipt-canvas-step-viewer`

## Description (303 characters)

WinForms-инструмент для локального просмотра STEP в .ipt canvas: StepLite-парсер, software Z-buffer, непрозрачный True Front renderer, POLY_LOOP-сетка, исторические режимы рендера и FPS, backbuffer и интерактивное вращение. В GitHub-режиме показывается только canvas, а Inventor-зависимые кнопки скрыты.

## Release ZIP

`InventorIptOrg_v0_5_17_WarningFreeCanvasBuild.zip`

## Warning-free correction

The user-confirmed v0.5.16 build completed successfully but reported three CS0162 warnings. v0.5.17 removes those warnings by changing the canvas-only configuration flag from compile-time `const` to runtime `static readonly`; visible behavior is unchanged.

## Visible UI

- only `.ipt canvas` tab;
- wider Mode and FPS selectors;
- `🔥Mode: Mesh True Front (compile fix) — v0.5.07`;
- `🔥FPS: Cursor Sampled Loop — v0.5.13`;
- local STEP open, reset, options only;
- Inventor-dependent buttons and legacy tabs hidden;
- startup Inventor attachment disabled.

## Dependency note

The visible local STEP workflow does not call Autodesk/Inventor APIs. The historical main project still references `Autodesk.Inventor.Interop.dll` because legacy code is retained for continuity. Do not describe the entire solution as dependency-free until the canvas renderer is extracted into its own project.

Visual Studio and Inventor were not run in the packaging environment. Static package/source checks are in `docs/STATIC_CHECK_v0_5_17.txt`.
