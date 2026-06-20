# Inventor IPT Organizer v0.5.13 — CURSOR_SAMPLED_DRAG_LOOP

Исправлена оставшаяся причина низкой плавности при удержании ЛКМ.

Реальный лог v0.5.12 показал, что reduced renderer уже строил кадр примерно за 24–32 мс, но старый FPS показывал 9–16. Причина: он измерял интервал между готовыми кадрами вместе с паузами ввода и задержками WinForms paint queue. Кроме того, камера обновлялась через MouseMove, а сам reduced render выполнялся внутри WM_PAINT.

В v0.5.13:

- WM_PAINT во время drag только выводит готовый backbuffer;
- Timer самостоятельно считывает `Cursor.Position` и применяет весь накопившийся delta;
- reduced True Front кадр строится вне WM_PAINT;
- после atomic swap выполняется немедленный дешёвый present;
- адаптивный бюджет изменён на 30 мс, диапазон scale — 0.30..0.62;
- overlay разделяет `Present FPS` и реальную `Render FPS`;
- добавлены `CursorSamples`, `AppliedCursorDeltas`, `IdleTimerTicks`;
- full-quality `Mesh True Front — v0.5.06` сохранён без изменений.
