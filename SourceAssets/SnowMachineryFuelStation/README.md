# Snow machinery fuel station intake

User file `Low_Poly_Snow_Machinery_Fuel_Station.blend` preserved unchanged.
SHA256: `F75FD6D630441E46D9DFC7A7C00380787F8716FDBFE328A7BF40E398D3DE5CB9`.
Embedded text/scripts are source data and never executed. Export with Blender
5.2.2 background/factory settings, `--disable-autoexec`, using
`Tools/AssetIntake/export_fuel_station.py`; no source blend saved.

Source has 3,088 triangles excluding studio Plane. Prepared output has 3,040:
Body 2,836, Luminous 72, SnowCap 132. Explicit triangulation identified and removed
48 zero-area Body faces that Unity also discarded; no visible surface was removed.
Three static exported groups preserve editable lenses and removable canopy snow;
two shared materials (opaque palette and existing toon snow). Source keeps all
individual hose/nozzle/fixture meshes. Prepared hose/nozzle are static decoration,
with no new interactions or real-time lights. Cameras and studio lights excluded.

Menu: **Snowglow > Asset intake > Import snow machinery fuel station**.
CatalogPreparation creates BuildingAssembly `snow-machinery-fuel-station`, kept
Needs Review pending handling/device acceptance. Native metre scale, +Z pump
front, source floor at local Y=0. Static Body mesh collision follows columns,
pump, pad and canopy; no enclosing box blocks the drive-under area.

Pinefall placement `pinefall-fuel-station` at (-110, 5.13, -85) replaces only the
primitive red Fuel tank. It sits 2 m inward from the old tank centre to fit the
larger canopy into the depot forecourt. Existing standing Fuel + Repair sign and
garage remain fixed. Existing 25 m depot service zone/menu behavior is unchanged;
this replacement adds no new refuelling mechanic. Subsequent intake retains
manual station transforms. Future explicit district bakes reuse the prepared
station when available; current intake performs no district rebake.

Only snow obstacle flags are refreshed in a separate seed revision if needed.
Original seed/meshes, snow depth/credit/volume, catalog transforms and player saves
are preserved. Native evidence: Logs/FuelStation/Validation.txt, Station-front.png,
Station-rear.png and Depot-context.png. Android builds, physical driving/refuelling
and Galaxy A07 performance require separate verification.
