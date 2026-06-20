# v0.5.07 — TrueFrontFaceCompileFix

Build name: `TRUE_FRONT_FACE_COMPILE_FIX`

Исправлена ошибка Visual Studio `CS0136` в `Form1.cs`, метод `ComputeStepNormalShade`.

Причина: метод заранее объявлял `double brightness;`, а ветка `TrueFrontFaceZBufferV0506` повторно объявляла `double brightness = ...` во вложенной области. C# запрещает такое затенение локальной переменной.

Исправление: повторное объявление заменено на `brightness = ...`.

Рендер-метод `Mode: Mesh True Front — v0.5.06` сохранён без изменений; версия приложения и заголовка — v0.5.07.
