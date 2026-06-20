# Analysis v0.5.11 — visible viewport center

## Confirmed evidence

The v0.5.10 runtime log reported:

- drawing control ClientSize: `1976x1167`;
- Paint clip / visible surface: `920x641`;
- projection target: approximately `(988, 595.5)`.

The projection was mathematically centered in the full hidden ClientSize, but the user could see only the left/top `920x641` portion. The model therefore appeared at the lower-right edge. `DrawImageUnscaled`, bitmap DPI and the 3D orbit pivot were not the remaining cause.

## Fix

`GetEffectiveViewerViewport` converts the viewer client rectangle to screen coordinates, intersects it with every ancestor client rectangle, converts the result back to viewer-local coordinates, and applies a guarded full-origin Paint extent when WinForms still reports an oversized hidden client. Full and reduced frames are rendered at the resulting dimensions and presented into the same local rectangle.

Tiny or offset partial invalidations are ignored so they cannot recenter the camera.
