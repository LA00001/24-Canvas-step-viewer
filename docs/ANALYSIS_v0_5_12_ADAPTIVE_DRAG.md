# Analysis v0.5.12 — adaptive drag direct swap

## Runtime evidence from v0.5.11

The supplied Rubber Hose log confirms that centering is fixed. The remaining bottleneck is the reduced drag pipeline:

- `RenderScale=0.55`;
- frame 15: `79.612 ms`;
- frame 30: `75.752 ms`;
- session average: `72.643 ms`, `12.46 FPS`;
- final quality render: `193.088 ms`.

## Root causes

1. Every reduced frame was enlarged into a newly allocated full-size bitmap before atomic swap.
2. The reduced Z-buffer still ran full-frame nearest-facing and opaque/depth-cue diagnostic scans although high-frequency logging was suppressed, depth cue was zero, and owner matching was disabled.
3. The first reduced frame was rendered immediately on MouseDown with `InputEvents=0`, before the camera changed.
4. Fixed 55% resolution could not react to viewport size or CPU load.

## v0.5.12 fix

- direct reduced bitmap swap; scaling occurs once at Paint presentation;
- adaptive scale range `0.32..0.58`, target `40 ms`;
- scale changes are damped and logged as `DRAG_ADAPTIVE_SCALE`;
- reduced drag skips only diagnostic scans that do not affect image output;
- MouseDown keeps the completed full frame; reduced rendering starts on the first real movement;
- overlay is drawn in visible-viewport coordinates;
- final True Front renderer is unchanged.
