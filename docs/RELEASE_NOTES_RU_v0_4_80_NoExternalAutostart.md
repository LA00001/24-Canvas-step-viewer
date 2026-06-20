# v0.4.80 — NoExternalAutostart

## Исправлено

При запуске нашего приложения больше не запускается внешний процесс автоматически.

## Причина старого поведения

В конструкторе формы был код:

`Marshal.GetActiveObject(...)`

а если активный процесс не найден:

`runtime object creation(...)`

Именно `runtime object creation` запускал внешний процесс.

## Новое поведение

На старте:

- приложение только пытается подключиться к уже запущенному процессу;
- если процесса нет, `_invApp = null`;
- автозапуска нет;
- `ST Step` работает локально через `StepLiteCore`.

## Новые логи

- `STARTUP_ATTACHED_TO_RUNNING_EXTERNAL_SESSION`
- `STARTUP_EXTERNAL_SESSION_NOT_FOUND`
- `COMMAND_ATTACHED_TO_RUNNING_EXTERNAL_SESSION`
- `COMMAND_EXTERNAL_SESSION_NOT_FOUND`
