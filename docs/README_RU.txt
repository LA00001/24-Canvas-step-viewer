24-Inventor-ipt-canvas-step-viewer

Сборка: Inventor IPT Organizer — v0.5.17 — WARNING_FREE_CANVAS_BUILD

Показывается только вкладка .ipt canvas. Рабочие режимы отмечены огнём:
- 🔥Mode: Mesh True Front (compile fix) — v0.5.07
- 🔥FPS: Cursor Sampled Loop — v0.5.13

Комбобоксы расширены. В видимой панели оставлены только локальное открытие STEP, сброс камеры, настройки и выбор рендера/FPS. Вкладки .ipt, .iam, .ipt cubes и Inventor-зависимые кнопки скрыты. Автоподключение к Inventor при запуске отключено.


v0.5.17: исправлены три предупреждения CS0162 из подтверждённой сборки v0.5.16. CanvasOnlyGitHubMode теперь runtime-readonly, а не compile-time const; поведение canvas-only не изменено.

Важно: видимый canvas workflow не вызывает Autodesk/Inventor API, но исторический проект src/InventorIptOrg всё ещё содержит legacy-код и compile-time ссылку Autodesk.Inventor.Interop.dll. Полностью автономный executable потребует отдельного проекта с вынесенным canvas renderer.

Описание репозитория (до 350 символов):
WinForms-инструмент для локального просмотра STEP в .ipt canvas: StepLite-парсер, software Z-buffer, непрозрачный True Front renderer, POLY_LOOP-сетка, исторические режимы рендера и FPS, backbuffer и интерактивное вращение. В GitHub-режиме показывается только canvas, а Inventor-зависимые кнопки скрыты.

---

Inventor IPT Organizer v0.5.15 — RENDER_FPS_GROUP_FIX

Группировка перепроверена по присланным исходным ZIP.
v0.5.06 добавила render method Mesh True Front.
v0.5.07 исправила только CS0136 в том же renderer и ещё не содержала FPS/backbuffer pipeline.
Поэтому список FPS теперь начинается строго с v0.5.08 и заканчивается v0.5.13.
В Mode добавлены обе проверяемые сборки: Mesh True Front v0.5.06 и Mesh True Front (compile fix) v0.5.07.

---

Inventor IPT Organizer v0.5.14 — FPS_MODE_SELECTOR

В v0.5.14 был добавлен второй список, но историческая группировка оказалась неверной: v0.5.06 и v0.5.07 ещё не содержали FPS/backbuffer. v0.5.15 исправляет группы: эти сборки находятся в Mode, а FPS начинается с v0.5.08.

Inventor IPT Organizer v0.5.13 — CURSOR_SAMPLED_DRAG_LOOP

Текущая сборка: Inventor IPT Organizer — v0.5.13 — CURSOR_SAMPLED_DRAG_LOOP

Стабильный финальный метод: Mode: Mesh True Front — v0.5.06.

v0.5.13 выносит reduced drag render из WM_PAINT. Короткий Timer считывает физический Cursor.Position, применяет весь накопившийся delta, строит reduced True Front bitmap вне paint callback, атомарно меняет backbuffer и сразу выполняет дешёвый present. Overlay отдельно показывает Present FPS и реальную Render capacity FPS. Адаптивный scale работает в диапазоне 0.30..0.62 под бюджет 30 ms. Финальный True Front не изменён.

---

Inventor IPT Organizer v0.5.12 — ADAPTIVE_DRAG_DIRECT_SWAP

Текущая сборка: Inventor IPT Organizer — v0.5.12 — ADAPTIVE_DRAG_DIRECT_SWAP

Стабильный финальный метод: Mode: Mesh True Front — v0.5.06.

v0.5.12 сохраняет правильный visible viewport v0.5.11 и ускоряет вращение ЛКМ. Уменьшенный drag-кадр теперь сразу становится atomic backbuffer без промежуточного полного bitmap и bilinear-copy. В reduced drag отключены только ненужные диагностические проходы по всем пикселям. Масштаб рендера автоматически меняется в диапазоне 0.32..0.58 под бюджет 40 ms. На MouseDown сохраняется последний full-quality backbuffer; первый reduced-кадр строится только после реального движения камеры. Финальный True Front не изменён.
