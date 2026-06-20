# v0.4.74 — MeshViewCore

## Что сделано

Добавлен второй нейтральный DLL-проект:

`MeshViewCore`

Он хранит mesh-scene данные:

- MeshPoint3
- MeshBox3
- MeshTriangle
- MeshBody
- MeshScene

## Что добавлено в приложение

На `.ipt canvas` добавлена кнопка:

`MS Mesh`

Она строит mesh-preview и показывает справа shaded mesh + wireframe.

## Важно

`MeshViewCore` не привязан к конкретному источнику данных. Адаптер приложения получает triangles/facets и передаёт в нейтральные структуры.
