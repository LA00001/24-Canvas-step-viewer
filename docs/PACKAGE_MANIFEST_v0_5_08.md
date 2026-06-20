# Package manifest — InventorIptOrg v0.5.08

Build name: `DRAG_BACKBUFFER_FPS`

Archive name: `InventorIptOrg_v0_5_08_DragBackbufferFps.zip`

## Изменения

- app/assembly version: `v0.5.08` / `0.5.8.0`;
- explicit viewer backbuffer;
- WinForms optimized double buffering;
- 30 FPS drag timer;
- MouseMove coalescing;
- 55% reduced-resolution shaded True Front drag frame;
- atomic frame swap;
- FPS/frame-time/dropped-input overlay;
- final full-quality render after mouse release;
- high-frequency drag log suppression plus summary events;
- historical `Mesh True Front — v0.5.06` algorithm preserved.

## Проверки пакета

- C# lexical brace/parenthesis/bracket balance;
- solution/project references;
- Compile Include existence;
- version/build markers;
- ZIP integrity;
- maximum internal path length.

Visual Studio и Inventor в среде подготовки пакета не запускались.
