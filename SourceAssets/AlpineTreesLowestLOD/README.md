# Lowest-detail alpine tree and wood pack

Supplied 4 November 2026: Alpine_Trees_26_Variants_4_LODs.blend, preserved outside
Assets (5,141,872 bytes). SHA256:
895E555B6AE1371E639B207FFD301E5BD8B8D936E99C2FBFD0D4D3FB5425F94D.

Blender 5.2.2 inspected all four sets with factory startup and autoexecution off.
Embedded READ ME and START HERE texts are source data, not executed instructions.
LOD3 was verified to have the minimum triangle count among LOD0/1/2/3 for every
variant. Only the 26 LOD3 collections were exported. Presentation objects and all
LOD0/1/2 geometry are excluded from the new prepared FBXs; the original .blend
retains them as supplied. No LOD switching or LODGroup is requested or needed.

Exports: Assets/Snowglow/Catalog/Sources/AlpineTreesLowestLOD. ExportMetrics.json
records each included object, LOD3 collection, geometry and excluded presentation.
The original LOD0 mounting origin/floor and axis conversion are reused so scene
transforms and collision proxies can remain unchanged. The source's existing face
colors were checked against the previous palette, and the same bark/snow materials
are reused. Snow caps stay separate and removable. No new materials or textures.

Existing catalog IDs alpine-tree-01..26 and prefab paths/GUIDs are updated in place
with LOD3 mesh references. Colliders, DistanceVisibility, placement IDs, positions,
rotation and scale stay in place. Original Blender, old FBXs and original meshes
remain preserved; prefab backups are under ignored Logs/AlpineTreesLowestLOD.
The regular tree-intake command forwards to the lowest-detail intake once active,
so it does not silently restore the old higher-detail visuals.

Menu: Snowglow > Asset intake > Use lowest LOD3 for all alpine trees and wood.
Scripts: Tools/AssetIntake/inspect_alpine_tree_lods.py,
export_alpine_trees_lowest_lod.py; Assets/Snowglow/Editor/LowestTreeLODIntake.cs.
Source inspection: Inspection.json. Native validation/previews: Logs/AlpineTreesLowestLOD.

The exported set has 8,266 visible triangles in 36 mesh groups. Based on saved
placement counts, the 97-instance forest is expected to change from 96,243 to
40,939 triangles (57.46% fewer), retaining 145 renderers. Unity native import/save checks confirmed these figures; triangle reduction alone
is not an FPS measurement.


Unity 6000.3.8f1 compiled, imported all 26 LOD3 FBXs, updated existing prepared
prefabs in place and saved/reopened Pinefall. All 97 forest instances reference
only the new LOD3 meshes; no old forest mesh remains elsewhere in the scene.
Existing catalog IDs, prefab GUIDs, complete placement transforms, collision
shapes and distance culling matched before/after. Original Terrain/snow seed/mesh
dependency hashes matched. Scene validation passed. Six native prefab previews
and a forest view rendered; snowy tree, log and scene views visually inspected.
No LODGroup, runtime switching, snow-mask mutation, rebake or player-save write.
Entries remain Needs Review. Live traversal and A07 performance remain untested.

| Variant | LOD3 triangles |
| --- | ---: |
| Tower Spruce | 270 |
| Broad Fir | 208 |
| Slender Spruce | 282 |
| Irregular Drooping Fir | 234 |
| Wind Bent Pine | 192 |
| Open Alpine Larch | 588 |
| Young Fir | 126 |
| Spruce Sapling | 116 |
| Snowy Tower Spruce | 644 |
| Snowy Broad Fir | 489 |
| Snowy Slender Spruce | 679 |
| Snowy Irregular Drooping Fir | 550 |
| Snowy Wind Bent Pine | 445 |
| Snowy Open Alpine Larch | 1,315 |
| Snowy Young Fir | 321 |
| Snowy Spruce Sapling | 288 |
| Tall Dead Snag | 316 |
| Thin Bare Snag | 262 |
| Broken Alpine Trunk | 224 |
| Bare Young Sapling | 168 |
| Low Cut Stump | 66 |
| Tall Cut Stump | 66 |
| Split Cut Stump | 74 |
| Snow Capped Stump | 113 |
| Fallen Cut Log | 68 |
| Snowy Broken Log | 162 |
