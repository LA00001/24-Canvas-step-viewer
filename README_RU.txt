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



## ИСТОРИЯ
### 1 ai-operations-ui-demo.zip — простой Blazor UI
### 2 topic-knowledge-base-demo.zip — справочник тем 101–115
### 3 docx-generator-demo.zip — отдельная генерация DOCX
### 4 workflow-pipeline-demo.zip — конвейер в консольном варианте
### 5 generated-files-api-demo.zip — API для выдачи файлов
### 6 ai-document-workflow-demo.zip — главный полный MVP с UI + конвейером
### 7 ai-document-workflow-devops-lab.zip — расширенная версия MVP с DevOps-обвязкой: health-check endpoints, Docker, Jenkinsfile, GitHub Actions CI и примеры AI infrastructure lab
### 8 ai-infra-kafka-s3-vault-dropapp-lab.zip — отдельный инфраструктурный lab: ASP.NET Core API + Redpanda/Kafka-compatible broker + MinIO/S3-compatible storage + HashiCorp Vault + dropapp-style manifest
### 9 ai-document-workflow-kafka-s3-vault-lab.zip — связка логики AI Document Workflow из проекта 6 с инфраструктурой Kafka/S3/Vault
### 10 ai-document-workflow-devops-ops-lab.zip — DevOps/Ops версия под Jenkins, Ansible, Groovy, Kafka, S3, Vault, Istio, dropapp-style, ИФТ/ПСИ и production-сопровождение
### 11 ai-document-workflow-observability-support-lab.zip — observability/support версия под сопровождение ПО: Prometheus, Grafana, Alertmanager, /metrics, healthz/readyz, readiness-проверки Kafka/S3/Vault, dashboard panels, alert rules, incident snapshot и runbook для диагностики инцидентов
### 12 ai-document-workflow-postgresql-support-lab.zip — PostgreSQL/support версия под сопровождение БД: PostgreSQL 16, Adminer, SQL schema/seed scripts, диагностические SQL-запросы, JOIN/GROUP BY/CTE, индексы, VIEW, PL/pgSQL functions, document health, incident diagnostics, release health и API endpoints для проверки состояния данных
### 13 ai-document-workflow-mvp-postgresql-support-lab.zip — гибрид проекта 6 и 12: главный MVP с UI + document workflow pipeline, объединённый с PostgreSQL/support-слоем для сопровождения БД: Docker Compose, PostgreSQL 16, Adminer, SQL schema/seed scripts, индексы, VIEW, PL/pgSQL functions, diagnostic SQL queries, document health, incident diagnostics, release health, /healthz, /readyz, /metrics, Prometheus, Grafana и Alertmanager
### 14 ai-document-workflow-business-data-analytics-lab.zip — Аналитический Blazor UI lab под роль аналитика: Visual Studio solution, запуск сценария анализа городских обращений, KPI по SLA/категориям/районам, BPMN/UML, business requirements, ТЗ, API/OpenAPI, SQL, Python ETL, dashboard spec и ML data preparation.
### 15-mortgage-vba-excel-sql-dashboard-lab.v2-blazor-ui-sln.zip — Excel/VBA/SQL lab под автоматизацию ипотечной отчётности: Visual Studio solution, Blazor UI для запуска сценария, ипотечный калькулятор, импорт CSV/SQL-выгрузок, VBA-модули, расчёт платежей/PTI/LTV, dashboard, data quality checks и регулярные отчёты.
### 16-ai-document-workflow-itil-change-management-lab.v1-html-ui-sln — ITIL/ITSM Change Management lab под инженера внедрения: Visual Studio solution, HTML UI с кнопкой формирования календаря изменений, ЗНИ/RFC, оценка рисков, поиск конфликтов, CAB checklist, планы внедрения и отката, связь с incident/problem management, KPI и SQL-отчётность.
### 17-ai-document-workflow-itsm-incident-analytics-lab.v2-blazor-ui-sln — ITSM Incident Analytics lab под роль аналитика в ITSM-системе: Blazor UI для анализа технологических инцидентов, KPI по SLA/MTTR/impact/root cause, контроль качества данных ITSM, аудит мероприятий, SQL-отчётность, RCA, dashboard spec и management report.
### 18-ml-model-validation-classic-ml-llm-lab.v5-russian-ui-comments.zip — Classic ML / LLM validation DS lab: Python pandas/sklearn, baseline logistic regression, challenger gradient boosting, ROC-AUC/Gini/KS/F1, backtest по историческим периодам, PSI/drift monitoring, LLM evaluation, generated CSV reports и Blazor UI-витрина.
### 19-Inventor-external-tool-MVP — Внешнее WinForms-приложение на .NET Framework для Autodesk Inventor. Поддерживает выбор тел в IPT рамкой, поиск внутренних и скрытых тел через RangeBox, редактируемые списки тел и элементов, копирование списков в буфер обмена, а также группировку компонентов в IAM-сборках.
### 20-Inventor-ipt-organizer — Внешний WinForms-инструмент для Autodesk Inventor: выбор тел IPT рамкой, поиск внутренних/скрытых тел через RangeBox, редактируемые списки объектов и создание папок в дереве Inventor.
### 21-Inventor-ipt-tree-editor — Внешний WinForms-инструмент для Autodesk Inventor. Работает с IPT через COM: выбор тел рамкой, поиск внутренних/скрытых тел через RangeBox, создание папок в дереве Inventor, редактирование имени/XYZ, JSON-экспорт дерева модели и логирование производительности.
### 22-Inventor-ipt-spatial-cube-selector — Внешний WinForms-инструмент для Autodesk Inventor IPT: spatial-cube индекс тел, быстрое выделение рамкой через RealCamera aspect-corrected projection, фильтрация body-box, временная подсветка кубов/следа и hide/show текущего visible-list.
### 23-Inventor-ipt-rangebox-tracebox-selector — Внешний WinForms-инструмент для Autodesk Inventor IPT: быстрый выбор тел через spatial-cube индекс и RealCamera aspect-corrected projection, режимы Cube hit all/filtered, строгая фильтрация SurfaceBody.RangeBox внутри Selection Trace Box, цветная ClientGraphics-подсветка RangeBox и hide/show visible-list.
### 24-Canvas-step-viewer — WinForms-инструмент для локального просмотра STEP в .ipt canvas: StepLite-парсер, software Z-buffer, непрозрачный True Front renderer, POLY_LOOP-сетка, исторические режимы рендера и FPS, backbuffer и интерактивное вращение. В GitHub-режиме показывается только canvas.


## License

Copyright (c) 2026 Андрей / LA00001

All rights reserved.

This repository is provided for portfolio and demonstration purposes only.
Copying, redistribution, modification, sublicensing, commercial use, or publication
of the source code is not permitted without prior written permission from the author.

---

Авторское право (c) 2026 Андрей / LA00001

Все права защищены.

Данный репозиторий предоставлен только для демонстрации в портфолио.
Копирование, распространение, изменение, сублицензирование, коммерческое использование
или публикация исходного кода не допускаются без предварительного письменного разрешения автора.
