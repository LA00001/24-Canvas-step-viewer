# v0.5.17 — WARNING_FREE_CANVAS_BUILD

Подтверждённая пользователем сборка v0.5.16 завершалась успешно, но Visual Studio сообщала три предупреждения CS0162 в `Form1.cs`: строки исходной сборки 1036, 1605 и 7508.

Причина: `CanvasOnlyGitHubMode` был объявлен как `const bool = true`. Компилятор заранее вычислял условия `if (!CanvasOnlyGitHubMode)` и код после `if (CanvasOnlyGitHubMode) { ... return; }` как недостижимый.

Исправление:

```csharp
private static readonly bool CanvasOnlyGitHubMode = true;
```

Флаг остаётся неизменяемым во время работы приложения, но больше не является compile-time constant. Поэтому сохранённые legacy fallback-ветки остаются корректным исходным кодом и не вызывают CS0162.

Поведение не изменено:

- показывается только `.ipt canvas`;
- проверенные режимы с 🔥 сохранены;
- startup attach к Inventor отключён;
- видимый STEP workflow не вызывает Autodesk/Inventor API;
- True Front renderer и Cursor Sampled Loop не изменены.
