# Package manifest — InventorIptOrg v0.5.00

Build name: `INVENTOR_STYLE_SHADED_MESH`

Base: `InventorIptOrg_v0_4_99_ZBufferSmoothShadeMesh`

## Main change

The confirmed Rubber Hose test showed correct geometry, Gouraud interpolation, Z-buffer hidden-line removal and approximately 0.095 s `Surface+Mesh` rendering, but the local viewer surface was too close to white and its mesh lines were too light compared with Autodesk Inventor.

v0.5.00 keeps the renderer architecture and recalibrates its visual profile:

- neutral-gray surface range;
- darker POLY_LOOP mesh and silhouette;
- stronger blue-gray background gradient;
- preserved software Z-buffer and hidden-line depth test;
- preserved `TriangleDiagonalsIncluded=False`;
- preserved `.ipt canvas` and `Surface+Mesh` defaults.

## Static verification

- App version marker: OK
- Build name marker: OK
- Assembly versions: OK (`0.5.0.0`)
- Four project files parsed as XML: OK
- Missing `Compile` includes: none
- Missing `ProjectReference` files: none
- C# lexical brace balance: OK
- Solution project count: 4
- Solution tail / `EndGlobal`: OK
- ZIP integrity: checked after packaging

Visual Studio and Autodesk Inventor were not run in this environment.
