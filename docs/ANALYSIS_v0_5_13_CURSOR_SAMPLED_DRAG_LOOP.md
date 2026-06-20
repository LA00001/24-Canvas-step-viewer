# v0.5.13 Cursor-sampled drag loop analysis

Source: real user log `inventor_ipt_organizer_v0_5_12_20260620_002733_734.log`.

## What the v0.5.12 measurements actually showed

The reduced renderer itself was no longer the main reason for the low number shown in the overlay:

- session 1: `AverageFrameMs=26.629` ms, which is about 37.6 render FPS, while the overlay/session value was only `AverageFps=13.25`;
- session 2: `AverageFrameMs=31.689` ms, which is about 31.6 render FPS, while the overlay/session value was only `AverageFps=9.58`;
- individual frames at 57% were commonly 23.9–32.4 ms.

The old `FPS` field measured time between completed frames. That interval also contained idle cursor periods, queued MouseMove delivery, delayed WM_PAINT dispatch and any time in which no new frame was requested. It therefore did not represent pure renderer throughput.

## Remaining pipeline problem

v0.5.12 still performed the expensive reduced render from `WM_PAINT`. Camera deltas were primarily applied from `MouseMove`. While the UI thread was rasterizing a frame, Windows could coalesce mouse messages. A pending paint could also be overtaken by more input/timer messages. The completed bitmap was correct, but camera sampling and presentation cadence were coupled to the WinForms paint queue.

## v0.5.13 change

- `WM_PAINT` is presentation-only while dragging.
- A short-interval WinForms timer samples the physical `Cursor.Position`.
- The timer applies the complete accumulated cursor delta, renders one reduced True Front frame into an off-screen bitmap, atomically swaps it and immediately performs the cheap backbuffer blit.
- MouseMove is retained for diagnostics but is no longer the only source of camera motion.
- Adaptive target is 30 ms with a 0.30–0.62 scale range.
- Overlay/logging separates `PresentFps` from `RenderCapacityFps = 1000 / EmaFrameMs`.

The final-quality historical renderer remains `Mesh True Front — v0.5.06`.
