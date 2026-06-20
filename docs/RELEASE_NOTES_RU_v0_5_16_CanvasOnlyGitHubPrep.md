# v0.5.16 CANVAS_ONLY_GITHUB_PREP

Сборка подготовлена для репозитория №24. Показывается только `.ipt canvas`; старые `.ipt`, `.iam`, `.ipt cubes` скрыты. Комбобоксы Mode/FPS расширены. Огнём отмечены только проверенные на Rubber Hose режимы v0.5.07 и v0.5.13.

В видимой панели оставлены ST Step, Reset, Options, Source, Mode и FPS. Кнопки, зависящие от Inventor/Autodesk, не добавляются в UI. При запуске программа не подключается к Inventor.

Важно: локальный canvas workflow не вызывает Autodesk API, но legacy-проект всё ещё содержит ссылку на Autodesk.Inventor.Interop для сохранённого исторического кода.
