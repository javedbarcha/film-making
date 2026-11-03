# Five alpine mountains intake

Supplied 29 October 2026: `Five_Alpine_Mountains.blend`.
Preserved original SHA256:
`382B0050FB4E9ACB40D17B4904C4B35950DCF2400A42D0F5863F47574CFCBF73`.

The five collection-based models are prepared as reusable Landscape catalog entries.
The presentation collection (ground, labels, cameras and lights) is excluded.
Embedded `START HERE | Five Alpine Mountains` text is data and was not executed.
Original source meshes, materials and node graphs remain intact in this file.

| Catalog ID | Variant | Triangles | Unity vertices | Saved instances |
| --- | --- | ---: | ---: | ---: |
| alpine-mountain-01 | Needle Peak | 690 | 1,524 | 3 |
| alpine-mountain-02 | Twin Summits | 726 | 1,601 | 3 |
| alpine-mountain-03 | Sawtooth Ridge | 1,016 | 2,380 | 2 |
| alpine-mountain-04 | Glacier Massif | 930 | 2,041 | 2 |
| alpine-mountain-05 | Alpine Cirque | 974 | 2,247 | 2 |

Each prefab has two renderers/material slots. Rock uses a shared point-sampled
palette and URP Simple Lit; snow uses the existing opaque white/cyan toon material.
The separate SnowCap mesh remains removable in the Editor. Blender shader-node
ramps are approximated with these mobile materials, not translated literally.
All ten source mesh solids are closed and have consistent outward winding;
preparation checks/recalculates them before export. No triangles were added.

The equipped scenery prefabs are under
`Assets/Snowglow/Catalog/Prefabs/alpine-mountain-01.prefab` through `05`.
Catalog entries retain stable IDs and Needs Review status. Their box colliders
are conservative background proxies, not climbable mountain collision meshes.

Use **Snowglow > Asset intake > Import five alpine mountains**, with saved Pinefall
open and Play stopped. The command replaces all 12 old sphere mountain locations.
A balanced bag shuffled with seed 291029 guarantees every variant is used, with
random yaw and uniform scaling fitted inside each old horizontal footprint.
X/Z locations are preserved. The Editor fits separate scene mesh copies to local
Terrain, retaining each source vertex's height above the model base. Bases are
buried 2.00-4.13 m after sampling side-wall base edges for gaps. Most mountain
height stays above ground and all cap vertices remain visible above the Terrain.
Shared source/catalog meshes remain unchanged. The 24 fitted meshes are saved at
`Assets/Snowglow/Catalog/SceneMeshes/AlpineMountains`; normals/bounds and background
box proxies are refreshed. Fitting reads original catalog meshes on every run,
preventing cumulative deformation. There is no runtime mesh generation.
Repeated intake preserves existing placements; future full district authoring uses
the prepared set. No construction/randomization occurs during Play.

Unity 6000.3.8f1 / Android compiled, prepared/validated the five prefabs, saved and
reopened all 12 placements, and passed scene checks and all 19 snow-ledger checks.
Homes, vehicle fleet, snow and terrain assets were preserved; no player save was
loaded or written. Five Blender source views and Unity prefab previews were
rendered/inspected; one placed mountain was inspected in Scene view. Native cap-vertex
readback initially checked peak visibility only; the terrain-fit correction now checks
every cap vertex and buried base vertices, with peak relief 42.86-122.67 m. A repeat
intake preserved the saved mix, rotations and scales.
Live sightlines, complete terrain-seam walkthrough, current Android packaging and
sustained Galaxy A07 performance remain untested.

Inspection/export scripts: `Tools/AssetIntake/inspect_alpine_mountains.py` and
`export_alpine_mountains.py`. Run with Blender
`--background --factory-startup --disable-autoexec --python <script>`.
Prepared FBX/palette/part map/geometry and normal checks:
`Assets/Snowglow/Catalog/Sources/FiveAlpineMountains`.
Native report: `Logs/AlpineMountains/UnityIntake.txt`; previews: `NN-unity.png`.
Pre-change scene backup: `Logs/AlpineMountains/Pinefall-before.unity`.
Logs are ignored; durable results belong in this document and VALIDATION.md.

Terrain fitting correction (29 October): Unity compiled, fitted all 12 mountains,
saved/reopened persistent mesh references, passed source geometry-count and terrain
seating checks plus scene/19 snow-ledger checks. Reports: `TerrainFit.txt` and
`TerrainFitValidation.txt` under `Logs/AlpineMountains`. Scene backup before fitting:
`Pinefall-before-terrain-fit.unity`. Terrain itself and snow volume data are unchanged.


Collision correction (2 November): the original terrain-fit proxy update mixed
placement-local and world bounds, placing all twelve boxes at X0/Z0. Repaired
scene overrides now match fitted mountain mesh corners in each box's local space.
Future fitting uses the corrected helper and checks proxy coverage/location.
No source/catalog meshes or placements changed. Native saved/reopened overlap
and intersection sweeps passed; 2,108 stale snow-blocking cells were cleared in
a new scene-specific seed without changing snow volume/credit. See
Logs/IntersectionObstacle/Validation.txt and VALIDATION.md.

## Refit mountains after manual placement - 3 November 2026

With Play stopped, select one or more **Mountain lot** groups in the Hierarchy,
move/rotate/resize them, then choose **Snowglow > Environment > Refit selected
mountains to terrain**. Selecting a mountain mesh child also works; selecting a
larger group includes its mountain descendants, with duplicates removed. Save
the scene after reviewing the result. Ctrl+Z / Ctrl+Y undo/redo the fitting.

The command preserves horizontal placement, rotation and scale. It adjusts the
lot's world Y to seat the source geometry above the highest sampled ground, then
fits the lower 55% of the model to the terrain with a smooth blend. The upper
45% retains its original shape. Base edges are sampled and buried at least
0.75 m at sampled positions (base vertices at least 2 m); this is a sampled seating
check, not an exact terrain/triangle intersection solver. Rock and decorative
snow are fitted together, with normals, bounds and conservative box proxies
updated. Uneven parent scaling and rotated boxes use the actual physics frame.

Every fit starts from the original catalog meshes. New scene-specific mesh
revisions are saved under `Assets/Snowglow/Catalog/SceneMeshes/MountainRefits`;
prior revisions remain available for Undo and existing references. Do not delete
them blindly. Source Blender/FBX files and shared catalog prefabs are unchanged.
Unselected mountains, Terrain, snow data and player saves are not edited. This
command does not save/reopen your working scene or rebake the district.

Use positive scales, keep the whole mountain inside the scene's single Terrain,
and keep background mountains/collision proxies outside the 256 m interactive
snow field. Invalid selections are rejected before fitting; application failures
roll back the scene operation. Prefab Mode and Play mode are unsupported.

The older **Import five alpine mountains** command still refits the complete set
using its original terrain-conforming workflow; use the selected command for
manual placement work. Native checks: **Snowglow > Environment > Run selected
mountain refit checks**, report `Logs/SelectedMountainRefit/Validation.txt`.
