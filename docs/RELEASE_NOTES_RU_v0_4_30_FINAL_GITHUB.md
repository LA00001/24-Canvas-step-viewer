# RELEASE_NOTES_RU_v0_4_30_FINAL_GITHUB

## Inventor IPT Organizer v0.4.30 REAL_CAMERA_ASPECT_CORRECT

Финальная GitHub-сборка после сравнения с `v0.4.1 RUNFIX`.

Главная победа: custom rectangle selection во вкладке `.ipt cubes` теперь работает через spatial-cube index и RealCamera aspect-corrected projection.

Проверено пользователем:

- большая рамка зацепила все `8/8` spatial-cubes;
- в candidate/visible-list попали все `72/72` тела;
- `Hide/Show IPT bodies` отработал по current visible-list и переключил `72` тела.

## Статус задач

- ✅ Select Frame + Add Inner/Hidden Bodies — заменено `.ipt cubes` workflow.
- 🔧 Create feature browser folder — следующая задача, нужно реанимировать и ускорить.
- ⏸ Apply edited name / XYZ to Inventor — отложено.

## v0.4.66 CANVAS_LAYOUT_LEFT_TOOLS

Перестроена вкладка `.ipt canvas` по замечаниям со скрина:

- квадратные кнопки перенесены в левый верхний угол;
- слева внизу добавлено постоянно открытое дерево слоёв;
- preview модели перенесён в крупную правую область и занимает весь оставшийся экран полотна;
- двойной клик по узлам дерева слоёв открывает соответствующие временные окна;
- ToolTip-подсказки с задержкой сохранены.
