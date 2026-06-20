# Анализ v0.5.10 — DRAG_DPI_CENTER_FIX

Пользовательский лог v0.5.09 сообщил `ScreenCenter=988,595.5`, `Pan=0,0`, однако screenshot всё ещё показывал модель в правом нижнем углу. Следовательно, ошибка находилась после projection fit.

Найдено два `DrawImageUnscaled`: показ основного backbuffer и показ software Z-buffer bitmap. После v0.5.08 рендер выполняется во внеэкранный bitmap, поэтому DPI/AutoScale могли применять к bitmap и control разные эффективные координаты. Изменение orbit pivot в v0.5.09 не могло исправить presentation mismatch.

Исправление v0.5.10:

- explicit destination/source rectangles с `GraphicsUnit.Pixel`;
- bitmap resolution соответствует `PaintEventArgs.Graphics.DpiX/DpiY`;
- off-screen Graphics: `PageUnit=Pixel`, `PageScale=1`;
- projected visual bounds центрируются отдельно от фиксированного 3D orbit pivot;
- одинаковое правило для full и 55% drag frame;
- диагностика mapping/center invariant.
