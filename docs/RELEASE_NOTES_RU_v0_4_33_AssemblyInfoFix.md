# Inventor IPT Organizer v0.4.33 — AssemblyInfoFix

## Что исправлено

В v0.4.32 при переходе на короткую структуру путей в архив не попал файл:

`src/InventorIptOrg/Properties/AssemblyInfo.cs`

При этом `InventorIptOrg.csproj` ссылался на него:

`<Compile Include="Properties\AssemblyInfo.cs" />`

Из-за этого Visual Studio выдавала CS2001.

## Что сделано

- Добавлен `src/InventorIptOrg/Properties/AssemblyInfo.cs`.
- Версия обновлена до `v0.4.33`.
- Короткая структура путей сохранена:
  - `InventorIptOrg.sln`
  - `src/InventorIptOrg/InventorIptOrg.csproj`

Функциональные правки v0.4.32 сохранены.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
