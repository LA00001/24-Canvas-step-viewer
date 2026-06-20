# v0.4.81 — LocalStepNoMessageBox

## Что исправлено

После локальной загрузки STEP больше не показывается модальный MessageBox с результатом.

## Причина "зависания" в v0.4.79

По логу:

- `StepLiteReader.Load` / `STEP_LOCAL_SCENE_LOADED` занял 0.726 с;
- затем был показан MessageBox;
- пользователь нажал OK примерно через 42 с;
- из-за этого общий `STEP_LOCAL_OPEN_BUTTON_SECONDS` стал 51.608 с.

## Новое поведение

Успешная загрузка STEP:

- пишет статус в левую статусную область;
- пишет лог `STEP_LOCAL_SCENE_STATUS_NON_BLOCKING`;
- не блокирует UI сообщением.

## Новые/уточнённые логи

- `STEP_LOCAL_FILE_DIALOG_SECONDS`
- `STEP_LOCAL_SCENE_STATUS_NON_BLOCKING`
- `STEP_LOCAL_DRAW_SECONDS` если draw дольше 0.250 с
