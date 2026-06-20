# v0.5.11 — VisibleViewportCenterFix

Исправлено оставшееся смещение модели вправо-вниз.

Причина подтверждена реальным логом v0.5.10: control сообщал `ClientSize=1976x1167`, а видимая Paint-область была `920x641`. Проекция центрировалась в скрытой части control.

В v0.5.11:

- вычисляется реальный видимый viewport через пересечение client rect viewer со всеми parent client rect;
- full frame, reduced drag frame, backbuffer и FPS overlay используют одинаковые размеры;
- частичные маленькие invalidation не влияют на центр камеры;
- `Mesh True Front — v0.5.06`, fixed orbit pivot, atomic swap и throttling сохранены.
