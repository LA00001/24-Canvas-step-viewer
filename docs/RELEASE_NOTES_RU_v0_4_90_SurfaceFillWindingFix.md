# v0.4.90 — SurfaceFillWindingFix

## Что исправлено

В v0.4.89 `Mode=Surface` мог выглядеть почти пустым, хотя `Mode=Wire` показывал геометрию.

## Исправление

- Surface/Suface+Edges/Ghost теперь используют `GraphicsPath(FillMode.Winding)`.
- Surface-заливка стала плотнее.
- Для чистого Surface добавлен лёгкий bounds silhouette, чтобы форма читалась даже без внутренних рёбер.
- В лог добавлено `SurfaceFillMode=Winding`.

## Что сохранено

- `.ipt canvas` по умолчанию.
- Default `Mode = Surface`.
- Быстрый `FastDragWireframe`.
- Без v0.4.87 LOD/chunk overhead.
