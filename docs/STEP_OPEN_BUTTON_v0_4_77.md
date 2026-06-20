# v0.4.77 STEP Open Button

Добавлена кнопка `ST Step` на вкладке `.ipt canvas`.

## Workflow

1. Нажать `ST Step`.
2. Выбрать `.stp` или `.step`.
3. Файл открывается в активном сеансе через `Documents.Open(path, true)`.
4. Если документ открылся как Part document:
   - `8C Cubes` строит spatial BASE;
   - `MS Mesh` строит mesh-preview.

## Логи

- `STEP_FILE_OPENED`
- `STEP_OPEN_FAILED`
- `STEP_OPEN_CANCELLED`
- `STEP_OPEN_BUTTON_SECONDS`

STEP-файл не зашит в код и не хранится внутри DLL.
