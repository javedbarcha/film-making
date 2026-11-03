# Alpine street lights intake

Supplied 30 October 2026: `Alpine_Street_Lights_12_Variants.blend`.
Preserved byte-identical source SHA256:
`50F565BAE9ECF8CEA58338E271A939C914EAB6C6FD2FBCD6567F6356286EC88D`.
The user identifies the model as generated in ChatGPT. Its embedded `START HERE |
Alpine Street Lights` text is source data, never an instruction or executed script.
Original source is outside Assets; prepared FBXs are imported independently.

All twelve numbered collections have two source mesh objects, Fixture and Snow,
plus a movement empty and one or more optional point lights. Presentation ground,
labels, cameras, demonstration wall and studio lights are excluded. A complete
object inventory is `Inspection.json`; prepared included/excluded mapping is
`Assets/Snowglow/Catalog/Sources/AlpineStreetLights/PartMap.json`.

| Catalog ID suffix | Fixture | Visible triangles | Optional point lights |
| --- | --- | ---: | ---: |
| 01 | Path Bollard | 228 | 1 |
| 02 | Park Globe | 600 | 1 |
| 03 | Compact Village Lantern | 630 | 1 |
| 04 | Heritage Single | 630 | 1 |
| 05 | Heritage Twin | 1,084 | 2 |
| 06 | Wall Lantern | 414 | 1 |
| 07 | Alpine Hanging Lantern | 684 | 1 |
| 08 | Heritage Triple | 1,290 | 3 |
| 09 | Residential Curved LED | 438 | 1 |
| 10 | Main Road Outreach | 438 | 1 |
| 11 | Dual Median LED | 604 | 2 |
| 12 | Four Way Area Light | 936 | 4 |

Stable IDs are `alpine-street-light-01` through `12`. Each prepared prefab has
three renderer/material groups: Frame, opaque unlit Luminous, and independently
removable SnowCap. All variants share three URP materials and a point-filtered
16x1 color palette. Snow follows the project's white-top/cyan-side toon material.
No transparent glass or new texture-per-fixture is introduced. Rigid metal pieces
were already joined in the source Fixture object; preparation preserves that
source and separates luminous faces without pretending each metal part is modular.

Ground fixtures retain the original pole/base mounting origin, not the visual
bounds centre of outreach arms. Wall Lantern has an explicit wall-mount anchor;
it must be attached to a facade rather than placed as a freestanding pole. Narrow
pole/base boxes avoid a full-width invisible obstruction below overhead arms.
The wall variant has a compact fixture proxy. Collision proxies require driving
review and are not exact mesh collision surfaces.

`Optional lights | enable in Editor (mobile default off)` is an inactive group in
every prepared prefab. Enable it or individual light components deliberately in
the Editor for a night-scene review. Sources' positions, warm colors, watt values
and counts are preserved in ExportMetrics.json; native point lights are shadowless
with proposed small ranges/intensities. Blender watts are not numerically equivalent
to Unity intensity. Luminous meshes remain visible with these optional lights off.
Removable decorative snow never enters the authoritative plowing volume ledger.

`Tools/AssetIntake/inspect_street_lights.py` inventories the preserved file.
`export_street_lights.py` exports normalized copies, split mesh groups, palette,
source previews, normals audit and light metadata. Run Blender with
`--background --factory-startup --disable-autoexec --python <script>`.
All 274 closed connected solids were checked; no faces needed reversal. Open
surfaces retain their winding. LED fixtures contain zero-area source triangles
(16/16/32/64 for variants 09/10/11/12); visible counts above exclude these because
Unity strips them. No decimation or source overwrite was performed.

Editor command: **Snowglow > Asset intake > Import twelve alpine street lights**.
It installs twelve Needs Review catalog prefabs, replaces nine old street lamps,
and adds two depot bollards, two neighborhood globes and one workshop wall lantern.
All twelve variants are represented across fourteen placements. Placement IDs
are `pinefall-light-01` through `14`; repeated intake preserves the saved layout.
A scene-specific `Catalog/SceneData/StreetLightsSnowSeed.asset` refreshes static
obstacle flags without changing depth/credit/accounting or the original seed.
Terrain, snow meshes, existing homes, mountains, fleet and player saves stay intact.
Future full district authoring calls the same placement helper in Edit mode.

Read a fresh `Logs/AlpineStreetLights/UnityIntake.txt` for native compilation,
save/reopen, metric, scene and snow-ledger verification evidence; source previews
are `NN-source.png` and native prefab previews are `NN-unity.png`. Pre-intake scene
backup is `Logs/AlpineStreetLights/Pinefall-before.unity`. Catalog status remains
Needs Review until visual clearance and device checks justify Ready publication.
Low polygon counts do not establish Galaxy A07 performance. Optional night lighting,
continuous driving clearance and Android appearance need their own checks.

Completed in Unity 6000.3.8f1 / Android on 30 October 2026: all twelve catalog
prefabs and fourteen placements saved/reopened, imported counts matched, scene
checks and 19 snow-ledger checks passed. All twelve native previews were inspected.
Forty home and twelve mountain prefab-instance records remain byte-identical.
The static-mask refresh changed 17,502 cells (16,341 added, 1,161 removed), preserving
every snow depth/credit/accounting value and the original seeds. Repeating intake
changes zero mask cells. See VALIDATION.md for the limits and correction details.

## Roadside density expansion

The user's request for about 1x more street lights adds 14 ground-mounted prepared
fixtures along the six main-road shoulders, bringing Pinefall to 28 fixtures.
Original 14 IDs/poses remain; new IDs are pinefall-road-light-01 through -14.
The group Additional roadside lights | 14 prepared fixtures is saved in the scene.
Largest-gap placement spreads the additions, with outreach arms toward roads and
pole proxies clear of asphalt, driveways, houses and existing scenery. Styles
reuse variants 03/04/05/07/08/09/10/11 and existing shared materials, emission and
removable snow. New optional point lights remain off; enabled light count unchanged.

Added inventory: 8,810 triangles / 42 renderers; total fixtures 17,614 triangles /
84 renderers. A separate RoadsideLightsSnowSeed.asset updates 56 local obstacle
flags, preserving all depth, credit, volume/accounting and original seed assets.
No full-district or snow-geometry rebuild. Menu: Snowglow > Environment > Add
fourteen roadside street lights. Repeating it preserves the existing additions.

Unity 6000.3.8f1 compiled, saved/reopened all 28 fixtures, checked new pole clearance
and persistent prefab/light settings, and passed scene validation and 19 snow
ledger checks. Existing catalog poses and original mesh/Terrain/seed hashes matched.
Serialized comparison confirmed no preexisting scene records were removed; only
the SnowField seed reference and SceneRoots changed among preexisting records.
An overview rendered and was inspected, although fog/distance limit fixture detail.
Continuous driving clearance, night lighting and A07 profiling remain untested.
Report/preview/backup: Logs/RoadsideLights. No player-save writes.
