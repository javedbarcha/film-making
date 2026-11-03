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

## Forest density and terrain seating ? 4 November 2026

User requested approximately 50% more trees/wood and repair of floating, deeply
buried or mountain-overlapping placements. Pinefall now has **146 instances**
(97 original + 49), all supplied LOD3; **62,496 triangles**. All 26 variants remain.
The Editor-only `ForestSeatingAuthoring` menu expands by the original population
once and repeats terrain repair without duplicates. Original IDs/variants remain;
87 original objects moved horizontally to suitable ground, with all original
heights corrected. Initial support audit found 47 floating, 48 excessively buried
and 31 overlapping conservative mountain horizontal bounds (categories overlap).
Trees stay upright; wood aligns to terrain slope. Mesh support samples are seated
with bounded shallow burial; all visible mesh vertices pass terrain penetration
limits. Full canopy/wood bounds clear mountain XZ envelopes by 3 m, preventing
placement beneath mountains. New variants follow existing proportions with varied
scale/yaw and spacing, outside the central worksite.

Unity 6000.3.8f1 compiled, saved/reopened Pinefall and passed scene validation and
19 snow ledger checks. A second run retained exactly 146 instances, required no
further movement and found zero floating/deep support or mountain overlaps. Three
native forest renders were visually inspected. Original Terrain, shared mesh and
snow seed hashes and non-forest catalog poses remained unchanged. Ten nearby snow
obstacle flags were updated in separate `SeatedForestSnowSeed.asset`; all depth,
credit and volume stayed unchanged. No district rebake or player-save writes.
Continuous driving and A07 performance were not tested. Reports, placement CSV,
scene backup and three renders are under ignored `Logs/ForestSeating`.

## Visible forest and slope placement - 4 November 2026

The latest user request supersedes upright-only trees and scattering over the
entire outer terrain. All 146 LOD3 tree/wood instances now stay within 280 m of
the district center, before the first mountain horizontal envelope along rays
from the district center to their full bounds. Full bounds also retain 3 m
mountain clearance; nothing is placed beneath or beyond mountains along these
rays. This is a conservative district-facing placement rule, not visibility from
every possible camera angle.

Tree roots tilt toward the terrain normal by up to 12 degrees, giving trunks a
slight slope-aware lean. Wood/stumps/log ends rotate fully with the terrain.
At least 34 trees and all six wood pieces occupy slopes of at least 5 degrees.
Actual mesh support and visible-vertex terrain checks still prevent floating or
excessive burial. The existing Editor menu applies these rules repeatably.

Native Unity 6000.3.8f1 compiled, saved/reopened Pinefall and validated all 146
placements, LOD3 bindings, mountain exclusion, terrain support and snow accounting.
86 existing pieces moved horizontally in this revision; geometry stays at 62,496
triangles / 219 renderers. Ten nearby obstacle flags changed in a new separate
seed revision; all snow depth, credit and volume were preserved. Terrain, shared
meshes, prior seed assets, non-forest catalog poses and player saves are preserved.
Scene checks and 19 snow ledger checks passed. No district rebake or runtime
spawning. Android performance and continuous driving remain untested.

Final repeat run confirmed **91 trees and all six wood pieces** on slopes of at
least 5 degrees, with all 146 positions stable (zero further relocations), matching
saved slope rotations and zero additional snow-mask changes. Three native views
were inspected, including a slope tree view and a close stump view.

## User-marked outer forest boundary - 4 November 2026

The annotated overhead reference supersedes the previous 280 m limit. The forest
now extends toward the mountain ring within a 470 m maximum full-bounds radius.
73 of the existing 146 LOD3 pieces were redistributed beyond 340 m; the farthest
root is 467.9 m from town. The other half retains inner woodland coverage. There
are no new objects or meshes: 62,496 triangles / 219 renderers remain. Full bounds
still clear mountain envelopes by 3 m and remain before the first mountain along
district-facing rays, with terrain seating and slope alignment preserved.
111 trees and all six wood pieces occupy slopes of at least 5 degrees.

Unity 6000.3.8f1 compiled, saved/reopened Pinefall and passed all placement,
rotation, visible-vertex/support, scene and 19 snow-ledger checks. No snow-mask
changes were needed; prior seed/Terrain/shared-mesh hashes and non-forest catalog
poses matched. Player saves are untouched. The Editor-only forest menu retains
this wider distribution. Reports and native previews: Logs/ForestSeating.
Android performance and continuous driving remain untested.

The wider layout passed a repeat native run with zero further relocations and
zero snow-mask changes. A fog-free native overhead render was visually inspected
against the supplied reference; temporary preview fog changes were restored.
