# v0.5.09 — DragCameraCenterLock

Build name: `DRAG_CAMERA_CENTER_LOCK`

Исправлено смещение модели во время вращения ЛКМ. STEP viewer больше не центрирует меняющийся projected bbox; вместо этого фиксированный 3D центр модели остаётся в центре viewport. Положение нажатия мыши не влияет на pivot. Ручной pan и весь backbuffer/FPS pipeline v0.5.08 сохранены.
