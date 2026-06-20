# Анализ смещения камеры при LMB — v0.5.09

## Наблюдение

Пользователь подтвердил, что v0.5.08 почти устранила мерцание, но при вращении ЛКМ модель уходила вправо от ожидаемого визуального центра.

## Причина

`PrepareStepLiteProjectionFit` вычислял `rawCenterX/rawCenterY` из текущего 2D projected bbox и каждый кадр переносил этот центр в viewport center. У асимметричного изогнутого отвода центр projected bbox меняется при yaw/pitch. Это превращало автоматический fit в плавающий camera target.

## Исправление

- orbit target = фиксированный центр 3D bounds;
- projected extents = `max(abs(min), abs(max))` по X/Y;
- scale рассчитывается по симметричному диапазону;
- raw origin `(0,0)` отображается в визуальный центр viewport;
- pan добавляется только явно через Shift+LMB/RMB;
- reduced drag frame использует те же пропорции центра после upscale.

## Сохранено

- Mesh True Front v0.5.06;
- 32-bpp atomic backbuffer;
- 30 FPS coalescing;
- FPS overlay;
- full-quality render after mouse release.
