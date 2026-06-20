# Canvas / Autodesk dependency audit — v0.5.16

## Confirmed local path

The visible `.ipt canvas` workflow opens `.stp/.step` through `OpenFileDialog`, parses it with `StepLiteCore.StepLiteReader`, and renders it with the local software Z-buffer. The visible controls do not call Inventor COM or Autodesk APIs. Startup attachment was removed from the v0.5.16 constructor.

## Hidden paths

The following legacy features are not added to the visible toolbar: ActiveView capture, live preview, docked ActiveView, mesh extraction from `SurfaceBody`, spatial cubes, browser refresh/tree windows and legacy cache tools. The `.ipt`, `.iam` and `.ipt cubes` tabs are removed from `tabControl1` after canvas initialization.

## Remaining compile-time dependency

`src/InventorIptOrg/InventorIptOrg.csproj` still references `Autodesk.Inventor.Interop.dll`, because the historical legacy code remains in the same project. Therefore the whole legacy project is not yet a physically dependency-free build. The honest next architecture step is a new standalone project containing only the canvas UI, StepLiteCore and the renderer.
