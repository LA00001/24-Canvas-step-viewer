# v0.4.75 — MeshViewCoreCompileFix

## Исправлено

Ошибка сборки:

`CS0246 MeshScene / MeshBody`

## Причина

В `Form1.cs` не был подключён namespace:

`using MeshViewCore;`

## Изменение

Добавлен импорт namespace.  
ProjectReference на `MeshViewCore.csproj` уже был и оставлен без изменений.
