# Historical Mode/FPS grouping audit — v0.5.15

The user-supplied project ZIPs were unpacked and compared directly.

| Build | Verified historical role | UI group in v0.5.15 |
|---|---|---|
| v0.5.06 TRUE_FRONT_FACE_ZBUFFER | Introduced `Mode: Mesh True Front — v0.5.06`; no backbuffer/FPS subsystem | Mode |
| v0.5.07 TRUE_FRONT_FACE_COMPILE_FIX | Only fixes CS0136 (`double brightness` redeclaration); renderer algorithm unchanged; no backbuffer/FPS subsystem | Mode |
| v0.5.08 DRAG_BACKBUFFER_FPS | First explicit backbuffer, atomic swap, 30 FPS timer/coalescing and FPS overlay | FPS |
| v0.5.09 DRAG_CAMERA_CENTER_LOCK | FPS pipeline generation with orbit-center lock | FPS |
| v0.5.10 DRAG_DPI_CENTER_FIX | FPS pipeline generation with explicit pixel/DPI presentation | FPS |
| v0.5.11 VISIBLE_VIEWPORT_CENTER_FIX | FPS pipeline generation using effective visible viewport | FPS |
| v0.5.12 ADAPTIVE_DRAG_DIRECT_SWAP | FPS pipeline generation with adaptive reduced scale/direct swap | FPS |
| v0.5.13 CURSOR_SAMPLED_DRAG_LOOP | FPS pipeline generation with timer-driven physical cursor sampling | FPS |

## Important v0.5.06 / v0.5.07 distinction

The v0.5.07 Form1 source differs functionally from v0.5.06 only in the compile repair:

```csharp
// v0.5.06 (CS0136)
double brightness = ...;

// v0.5.07
brightness = ...;
```

Therefore the two Mode entries intentionally produce the same image. Their separate entries exist for source-build provenance and logging, not because v0.5.07 introduced a new lighting/Z-buffer algorithm.
