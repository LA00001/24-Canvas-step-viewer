# Package manifest — InventorIptOrg v0.5.06

Build name: `TRUE_FRONT_FACE_ZBUFFER`

Archive name: `InventorIptOrg_v0_5_06_TrueFrontFaceZBuffer.zip`

Основные изменения:

- новый default mode `Mesh True Front — v0.5.06`;
- исправленный camera-facing rule `NormalZPositive`;
- all-triangle nearest Z-buffer depth pass;
- alpha 255 и отсутствие painter transparency;
- camera-correct lighting;
- corrected POLY_LOOP mesh depth test;
- сохранены все исторические Mode-пункты и версии методов;
- Inventor autostart отсутствует;
- `.ipt canvas` остаётся default tab;
- `Cube hit: filtered bodies` остаётся default cube mode.

Visual Studio и Inventor при формировании пакета не запускались. Выполнены статические проверки, численное воспроизведение supplied STEP и ZIP integrity test.
