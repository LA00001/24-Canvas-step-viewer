# v0.4.84 — UnifiedViewerModes

## Что сделано

Добавлена единая система режимов viewer без добавления горы кнопок.

## UI

На `.ipt canvas` добавлены компактные controls:

- `Source` combo:
  - Auto
  - STEP
  - Mesh
  - BoxGrid

- `Mode` combo:
  - Wire
  - Surface
  - Surface+Edges
  - Ghost
  - Points

- `OPT` popup:
  - Show points overlay
  - Show BoxGrid overlay
  - Draft while dragging

## Что сохранено

Старые action-кнопки сохранены:

- ST Step
- MS Mesh
- 8C Cubes
- RF Full
- LR Refresh
- остальные layer windows

## Render

STEP viewer теперь уважает `Mode`:

- Wire — только каркас
- Surface — лёгкая поверхность без рёбер
- Surface+Edges — поверхность + контур/рёбра
- Ghost — полупрозрачная поверхность
- Points — точки

Во время вращения при включённом Draft while dragging используется быстрый wireframe.
