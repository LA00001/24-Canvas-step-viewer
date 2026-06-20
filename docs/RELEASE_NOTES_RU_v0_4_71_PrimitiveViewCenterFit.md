# Inventor IPT Organizer v0.4.71 — PrimitiveViewCenterFit

## Исправлено

Primitive 3D viewer больше не должен улетать в нижний правый угол.

## Как работает центрирование

Перед рисованием строится 2D bounding box всей projected-модели:

- ModelBox
- spatial cubes
- body RangeBoxes

После этого вычисляются scale и offset, чтобы вся модель была по центру preview.

## Лог

Добавлено событие:

`PRIMITIVE_VIEW_PROJECTION_FIT`
