# Alpine trees intake

Supplied 30 October 2026: `Alpine_Trees_26_Variants.blend`.
Preserved SHA256: `C8FA426AEAB4D38B8FFBCD378212D1376E139CA831F8437676D6F3720FBFE06F`.
The original stays outside Assets and is never overwritten. Embedded `START HERE |
Alpine Trees` text is source data, not instructions or executed code. Source
inspection and export run Blender with `--disable-autoexec`.

The 26 collections include eight live-tree forms, eight separate snowy counterparts,
four bare/dead forms, four stumps and two fallen logs. Tree or Wood is already a
joined rigid assembly in the source; preparation retains it as one opaque palette
mesh. Snow meshes exist only in variants 09-16, 24 and 26 and remain independently
removable. Snow-free variants are not given invented snow geometry.

| Catalog suffix | Variant | Visible export triangles | Snow mesh |
| --- | --- | ---: | --- |
| 01 | Tower Spruce | 904 | No |
| 02 | Broad Fir | 694 | No |
| 03 | Slender Spruce | 942 | No |
| 04 | Irregular Drooping Fir | 780 | No |
| 05 | Wind Bent Pine | 646 | No |
| 06 | Open Alpine Larch | 1,966 | No |
| 07 | Young Fir | 426 | No |
| 08 | Spruce Sapling | 388 | No |
| 09 | Snowy Tower Spruce | 1,290 | Yes |
| 10 | Snowy Broad Fir | 981 | Yes |
| 11 | Snowy Slender Spruce | 1,361 | Yes |
| 12 | Snowy Irregular Drooping Fir | 1,100 | Yes |
| 13 | Snowy Wind Bent Pine | 893 | Yes |
| 14 | Snowy Open Alpine Larch | 2,633 | Yes |
| 15 | Snowy Young Fir | 647 | Yes |
| 16 | Snowy Spruce Sapling | 576 | Yes |
| 17 | Tall Dead Snag | 1,054 | No |
| 18 | Thin Bare Snag | 874 | No |
| 19 | Broken Alpine Trunk | 750 | No |
| 20 | Bare Young Sapling | 562 | No |
| 21 | Low Cut Stump | 220 | No |
| 22 | Tall Cut Stump | 220 | No |
| 23 | Split Cut Stump | 252 | No |
| 24 | Snow Capped Stump | 223 | Yes |
| 25 | Fallen Cut Log | 232 | No |
| 26 | Snowy Broken Log | 318 | Yes |

Stable IDs: `alpine-tree-01` through `26`. Prepared FBXs, a point-filtered 16x1
palette, ExportMetrics.json and complete PartMap.json are under
`Assets/Snowglow/Catalog/Sources/AlpineTrees`. Presentation display bases, labels,
cameras/lights and movement empties are excluded. The prepared tree has one body
renderer, plus one removable SnowCap renderer where supplied; the whole set uses
two shared opaque materials (URP Simple Lit palette and existing white/cyan toon
snow). There are no transparent foliage cards or new runtime model factories.

Normalized ground floor and source horizontal mounting origin are preserved.
Tall trees use narrow proposed trunk capsules, not full-canopy blocking boxes;
stumps/logs use compact boxes. Source meshes are shared across instances and not
terrain-deformed. Terrain seating probes are buried by 8 cm, and stumps/logs align
with the sampled Terrain normal. Native validation checks both buried probes and
remaining geometry above terrain. Existing 230 m DistanceVisibility is preserved.
Collision/terrain fitting still requires a complete forest walkthrough.

`Tools/AssetIntake/inspect_alpine_trees.py` records all source objects/materials.
`export_alpine_trees.py` exports evaluated copies and checks connected closed
solids before export. All 700 closed solids passed without reversed faces; open
surfaces retain authored winding. Source preview cameras are reframed to show
complete long logs at portrait aspect; the original .blend is never saved.
No decimation is applied. The 1,966-triangle Larch and 2,633-triangle Snowy Larch
need particular attention when repeated or distant; low counts do not prove FPS.

Editor menu: **Snowglow > Asset intake > Import twenty six alpine trees**.
It prepares 26 Needs Review catalog prefabs and replaces all original primitive
forest tree positions, using every variant. Deterministic seed 301026 weights
additional placements toward winter evergreens; rare stumps/logs each occur once.
Original X/Z positions and uniform scales remain, while yaw and ground seating are
authored in Edit mode. Repeated intake preserves the saved mix and IDs. Future full
district authoring calls the same placement helper. No whole-district rebake is
needed to replace the existing forest.

The tool snapshots all existing catalog placements, fleet, Terrain and snow assets,
saves/reopens Pinefall, checks persistent meshes/rigs/materials and runs the 19 snow
ledger checks. It reacquires catalog assets after OpenScene unloads unused Editor
assets. Failed placement preflight discards only the tool's unsaved placement pass,
starting from the required saved scene. Original source/GUIDs/player saves stay
intact. Only nearby interactive-field trunk obstacle flags are considered; if no
flags change, the current seed is preserved byte-for-byte. Otherwise a separate
`Catalog/SceneData/AlpineTreesSnowSeed.asset` preserves all depth/credit/accounting.

Reports/previews: `Logs/AlpineTrees/UnityIntake.txt`, `NN-source.png`, `NN-unity.png`.
Pre-intake scene copy: `Logs/AlpineTrees/Pinefall-before.unity`. Read a fresh PASS
report before claiming native compilation, placement or save/reopen success.

Completed in Unity 6000.3.8f1 / Android on 30 October 2026: all 26 catalog prefabs
and all 97 replacement placements saved/reopened. All variants are used. Native
geometry/material/terrain checks, scene validation and 19 snow-ledger checks passed.
All 26 prepared prefab previews were inspected, along with one placed snowy spruce.
Repeating the actual import menu preserves the mix, transforms and scales. Preview
framing includes whole wide stumps/logs. Entries remain Needs Review.

Forest inventory: 96,243 mesh triangles / 145 renderer-material slots versus
454,736 / 679 for the old primitive set. Counts exclude extra shadow passes and
are not rendered-frame or phone-performance results. All 66 home/mountain/light
prefab-instance records stay byte-identical; fleet, Terrain and saved snow meshes
are preserved. No nearby snow obstacle flags changed; the original active seed
reference and complete data remain unchanged. No tree-specific seed was created.
No district rebake or player-save write. Full forest/collision walkthrough, runtime
culling, Android rendering/build and Galaxy A07 timings remain untested.

## Superseded active visuals: lowest supplied LOD3

The user's four-LOD pack replaces the active tree/wood visuals with LOD3 only.
Current source/preparation/validation: SourceAssets/AlpineTreesLowestLOD/README.md.
The 26 existing IDs and prefab GUIDs are retained, with 97 scene instances at
40,939 triangles / 145 renderers. The older geometry counts above describe this
preserved original source, not the current active forest. The regular intake
forwards to LowestTreeLODIntake while lod3-only entries are active. No LOD0/1/2
geometry from the new pack is imported; no LODGroup or runtime switching is used.
Original pose, collider, palette/snow and culling bindings remain unchanged.
