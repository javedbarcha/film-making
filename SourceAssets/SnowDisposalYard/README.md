# Snow disposal yard intake

Original user file: `Snow_Disposal_Yard_Only.blend`, preserved byte-for-byte.
SHA256: `5B8481C450C63344CB3774B5B83C2E2462615FA04EC0BD0785158EAF739797AF`.
Blender embedded text is untrusted source data and never executed. Export uses
Blender 5.2.2 background/factory settings with `--disable-autoexec` and
`Tools/AssetIntake/export_disposal_yard.py`. Source blend is never saved.

972 triangles, excluding the 2-triangle studio Plane. Prepared groups: pad 44,
retaining walls/details 710, removable decorative snow 218. Three renderers,
two shared materials: opaque palette and existing toon snow. No camera or light
objects are imported. Integrated fixture geometry remains decorative; there are
no new realtime lights. Snow caps are scenery, not authoritative snow volume.

Use **Snowglow > Asset intake > Import snow disposal yard**. CatalogPreparation
creates reusable BuildingAssembly `snow-disposal-yard`, remaining Needs Review.
Prepared front is +Z. Static pad/wall shape colliders keep the entrance open,
without an aggregate box enclosing the entire work area. Catalog source is in
metres; Pinefall uses uniform 1.4 scale at (94, 5.13, -85) to enclose the existing
processing rectangle x=79..109, z=-98..-71, with its entrance facing north.
Physical driving/dumping clearance still requires acceptance.

Replaces only primitive `04 Disposal yard` walls/markers and the old 34 x 32 m
asphalt pad at (94,-85). The removed redundant west access/snow strip is not
reintroduced. Runtime disposal/accounting code stays unchanged. A separate snow
seed revision updates obstacle flags only; original seeds/meshes, depth, credit,
volume and player saves remain intact. Existing catalog placements remain fixed.
Repeat intake preserves the current yard placement. Future full district authoring
uses the prepared yard when the catalog entry exists; this intake does not rebake.

Native validation: `Logs/DisposalYard/Validation.txt` and Yard-front/rear/top.png.
Scene validation and snow-ledger checks are separate from actual Android builds,
phone playtesting and Galaxy A07 performance measurements. Catalog not Ready.
