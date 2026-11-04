# Four-bay snow garage intake

User supplied `Low_Poly_Four_Bay_Snow_Garage.blend`, preserved byte-for-byte.
SHA256: `F757A20FA3AE662591680C4EDC226B3353F42B7435B139594FB54ED959B736CF`.
Source documents/embedded text are data, not instructions; no embedded scripts run.

Export with Blender 5.2 in background, factory settings and `--disable-autoexec`,
using `Tools/AssetIntake/export_four_bay_garage.py`. Export excludes preview ground,
cameras/lights; FONT bay numbers become static meshes. No source file is saved.
The exported model has 3,832 triangles: shell 2,608, four doors 1,152, numbers 72.
Shared opaque palette and existing toon snow material replace source materials on
prepared copies. Roof/decorative snow remains separate; windows stay opaque.

`Snowglow > Asset intake > Import four bay snow garage` prepares a BuildingAssembly
catalog entry `four-bay-snow-garage` through CatalogPreparation, retaining Needs
Review status. Four door meshes and source lift pivots are retained, but automatic
opening/interactive garage interiors are not implemented by this replacement.
Static shape colliders preserve the apron shape. No realtime source lights import.

Pinefall replacement faces the depot forecourt. Only the primitive workshop,
its wall lantern and its wall Fuel + Repair sign are removed. A freestanding
alpine hanging-board variant displays FUEL + REPAIR near the red fuel tank,
facing the forecourt; original sign placement identity is retained. Workshop
Parts + Service and the red tank remain. Snow obstacles use a separate seed
revision; depth, credit, initial volume and saved mesh assets remain intact.
No player saves or whole-district bake are involved.

Native validation evidence is recorded in `Logs/FourBayGarage/Validation.txt`.
Android packaging, driving clearance and physical Galaxy A07 profiling require
separate acceptance; mesh counts do not establish frame-time cost.
