# v0.5.10 — DragDpiCenterFix

Build name: `DRAG_DPI_CENTER_FIX`

- Исправлено смещение backbuffer в правый нижний угол на DPI-scaled WinForms surface.
- `DrawImageUnscaled` заменён явным pixel mapping.
- Visual bounds точно центрируются; 3D model Bounds center остаётся orbit pivot.
- Backbuffer/FPS и `Mesh True Front — v0.5.06` сохранены.
