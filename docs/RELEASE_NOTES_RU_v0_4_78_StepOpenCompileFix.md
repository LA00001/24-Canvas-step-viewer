# v0.4.78 — StepOpenCompileFix

## Исправлено

Ошибка сборки:

`CS0104 File / Path ambiguous`

## Причина

Namespace `Inventor` содержит собственные `File` и `Path`, поэтому короткие имена конфликтовали с `System.IO`.

## Изменение

В коде STEP-кнопки используются явные имена:

- `System.IO.File.Exists`
- `System.IO.Path.GetExtension`
