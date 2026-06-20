# Package manifest — InventorIptOrg v0.5.01

Build name: `DEEPER_SHADE_SPLIT_EDGE_BIAS`

Base: `InventorIptOrg_v0_5_00_InventorStyleShadedMesh`

## Main change

The stable v0.5.00 software Z-buffer renderer is retained. v0.5.01 deepens the Gouraud gray range and separates edge depth tolerances so the shaded form reads more like Autodesk Inventor while ordinary POLY_LOOP mesh lines are less dominant.

- surface clamp `115..218`;
- mesh RGB `84`;
- silhouette RGB `40`;
- mesh depth factor `0.0025`;
- feature depth factor `0.0032`;
- silhouette depth factor `0.0040`;
- preserved `TriangleDiagonalsIncluded=False`;
- preserved `.ipt canvas`, `Surface+Mesh`, and filtered-cube defaults.

## Static verification

- App version marker: OK
- Build name marker: OK
- Assembly versions: OK (`0.5.1.0`)
- Project files parsed as XML: OK
- Missing `Compile` includes: none
- Missing `ProjectReference` files: none
- C# lexical brace balance: OK
- Solution project count: 4
- ZIP integrity: checked after packaging

Visual Studio and Autodesk Inventor were not run in this environment.
