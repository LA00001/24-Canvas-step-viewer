# Inventor IPT Organizer v0.4.69 — Live3DPreview

## Новые кнопки

- `LV Live` — запускает/останавливает live bitmap stream.
- `DK Dock` — экспериментально встраивает Inventor ActiveView в правую preview область.

## Режимы

### AV Model

Один снимок.

### LV Live

Повторные снимки по Timer. Это live-поток кадров, но не настоящий 3D-control.

### DK Dock

Экспериментальный настоящий live 3D: Win32 `SetParent` для HWND Inventor ActiveView.

Если Inventor не отдаёт HWND ActiveView, появится сообщение, а рабочим вариантом остаётся `LV Live`.
