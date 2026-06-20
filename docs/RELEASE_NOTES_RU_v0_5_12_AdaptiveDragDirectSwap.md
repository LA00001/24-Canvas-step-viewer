# v0.5.12 — AdaptiveDragDirectSwap

После исправления центра v0.5.11 оставалось около `12.46 FPS` при ЛКМ. v0.5.12 ускоряет именно интерактивный drag-путь:

- adaptive render scale `0.32..0.58` вместо фиксированных `0.55`;
- целевой бюджет кадра `40 ms`;
- reduced bitmap больше не копируется в полный bitmap перед swap;
- diagnostic pixel scans отключены только на reduced drag;
- нулевой reduced-кадр на MouseDown больше не строится;
- FPS overlay рисуется в реальном visible viewport;
- финальный `Mesh True Front — v0.5.06` не изменён.
