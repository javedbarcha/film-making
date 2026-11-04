# Alpine sign-board intake

Supplied 4 November 2026: `Alpine_Mobile_Sign_Boards_12_Variants.blend`.
Preserved source: 1,489,920 bytes; SHA256
`A533FFDEB2781572932C7FAC07F0FC6312B600D50133BD23607DAF399DD22A12`.
The embedded `READ ME | Alpine Sign Kit` is untrusted source data, not an
instruction or script to execute. Blender 5.2.2 ran with factory startup and
`--disable-autoexec`. No source blend was saved or modified.

Twelve numbered collections contain a joined geometry object and mounting empty;
some include FONT artwork. Preparation evaluates supplied FONT objects into static
mesh lettering and splits decorative snow by source material. Original text and
all modular authoring remain in the source. Studio ground, camera and lights are
excluded. See Inspection.json and the exported PartMap.json for the mapping.
Exports: `Assets/Snowglow/Catalog/Sources/AlpineSignBoards`.
Scripts: `Tools/AssetIntake/inspect_sign_boards.py`, `export_sign_boards.py`.

Stable catalog IDs: `alpine-sign-01` through `alpine-sign-12`. Prepared assets use
the project's CatalogPreparation pipeline, stay Needs Review and are not Ready
publication. Body and source lettering share an opaque palette; SnowCap uses the
existing pure-white/cyan toon snow material and stays independently removable.
No lights, runtime scene factories, additional snow volume or save schema changes.

Menu: **Snowglow > Asset intake > Import twelve alpine sign boards**.
Pinefall target: 16 persistent connected prefab instances, replacing four primitive
signs and adding one of every variant. Blank replacement boards preserve the
original destination labels with Editor-authored TextMesh children. Variant 06
mounts to the workshop facade; all other variants are ground-mounted. Full sign
collision envelopes are conservative and need handling review. Repeating intake
retains existing sign placements and does not add duplicates.

Native Unity 6000.3.8f1 / Android compiled, imported and saved/reopened Pinefall.
All 12 variants have connected saved scene instances; four primitive signs were
replaced and 12 added (16 total). All variants remain Needs Review. Full sign
bounds clear asphalt roads; ground origins seat to Terrain and the wall mount
matches the workshop facade. Scene and 19 snow-ledger checks passed. Original
Terrain/snow asset hashes, non-sign catalog poses and fleet bindings matched.
171 local obstacle flags changed in a separate SignBoardsSnowSeed copy; snow
depth, credit and volume stayed unchanged. Player saves were untouched.

| ID suffix | Variant | Triangles including source lettering/snow |
| --- | --- | ---: |
| 01 | Trail directory four routes | 854 |
| 02 | Single green trail board | 290 |
| 03 | Blue direction arrow | 231 |
| 04 | Alpine hazard warning | 524 |
| 05 | Road closed warning | 421 |
| 06 | Blank wall timber board | 200 |
| 07 | Large twin post noticeboard | 332 |
| 08 | Wooden arrow right | 240 |
| 09 | Twin wooden arrows | 224 |
| 10 | Blank hanging board | 392 |
| 11 | Festive hanging board | 1037 |
| 12 | Small blank timber post | 236 |

One of each variant totals 4,981 triangles. Scene sign geometry is 6,117 triangles
(excludes five retained destination TextMesh labels). Body/lettering share one
opaque palette material; removable snow shares the existing snow material.
Blender FONT artwork is static mesh geometry, not missing runtime fonts.
Reports/backups/native renders: Logs/AlpineSignBoards. Continuous driving, touch
readability, Android packaging and A07 timings remain untested.

The former label-fit handoff is resolved: native bounds checks and rendered
scene captions passed. The latest curated layout below supersedes the original
all-variant placement plan.

## Curated road-facing sign layout - 4 November 2026

Latest explicit user list supersedes the request to place all twelve variants and
the earlier 16/14-sign layouts. Only these **31** boards are authored in Pinefall:
one Snow Depot at the depot driveway; one Fuel + Repair on the workshop facade;
one Workshop Parts + Service beside the workshop; one Alpine Lodge beside an
existing home; one Town Center near the central junction; one Alpine Market by
the northern house block; one stacked directory at the central junction; twelve
Road Closed boards at the outer ends of the six through-roads; twelve Alpine
Danger boards around the built-town edge, facing inward. Service-yard/parking
accesses remain open. Danger spacing is approximately 30 degrees at 165 m radius,
with bounded small adjustments for terrain/clearance. Other board placements are
removed; all twelve source/catalog variants and their GUIDs remain available.

SignWayfindingAuthoring configures the saved layout through SignBoardsIntake.
Faces aim toward actual road/junction reading positions; arrow checks use real
fleet-depot, town-center and existing house-entrance targets (home-40 in this
scene). Lodge distance uses that entrance rather than the supplied fictitious
450 m. Scene-only captions identify real local services; original source mesh
lettering is retained in catalog prefabs. Alpine Market artwork is restored.
Destination labels now fit their board faces. Repeated import validates the
curated layout without resurrecting removed boards or adding duplicates.

Unity 6000.3.8f1 / Android compiled, saved/reopened the curated 31-sign layout,
validated road clearance, ground/wall seating, facing, arrows, sightlines,
boundary count/radius/spacing, scene bindings and 19 snow-ledger checks. Native
road-reader views were rendered and inspected. Terrain/original snow asset
hashes, non-sign catalog poses and fleet references matched. Moved/deleted sign
obstacles are reconciled in a separate seed revision, retaining all depth,
credit and volume. No district rebake or player-save writes. Sources, catalog
entries and earlier seed revisions remain preserved. Phone performance, actual
driving and touch readability are untested. Reports/renders/backups:
Logs/AlpineSignBoards; Wayfinding.txt records each reading position and target.

Final curated-layout evidence: the canopy-safe version saved/reopened and passed
a repeat native run with 31 signs and zero further obstacle-mask changes. All
31 road/town-reader renders were produced; representative destination, workshop,
road-end and boundary views were inspected. Depot/workshop reading views are
separate, Town Center is beside the central junction, and tree canopy bounds
clear the warnings and their reading sightlines. Prepared geometry totals 14,584
triangles (hidden source lettering included; eight authored caption objects are
additional). Native angle/spacing checks passed. Actual driving/device tests
remain unperformed. The earlier pending label-fit handoff is resolved.

## Sign-board winding and normals repair - 4 November 2026

User reported culling faults and requested all board normals checked. The original
blend audit found five inward closed solids / 31 reversed polygon faces across
four variants: Snow Depot arrow (03, seven faces), Alpine Danger (04, ten faces
across two solids), Town Center arrow (08, seven faces), and twin arrows (09,
seven faces). The other eight variants passed. The preserved source blend is
unchanged (original SHA256 still matches); embedded text remains unexecuted.

The exporter now orients every connected closed solid outward before splitting
Body/Lettering/SnowCap, preserving open lettering and palette UVs. Only the four
affected FBXs were re-exported in this repair, with their existing .meta/GUIDs.
All 198 closed source solids then passed, with zero inward solids. Triangle
counts, dimensions, mounting origins, snow groups and shared materials remain.
Backface culling stays enabled; no double-sided material workaround was applied.
Future full or selected exports run the same normal repair automatically.

Unity 6000.3.8f1 / Android compiled and synchronously reimported those four FBXs.
SignNormalsChecks audited all twelve catalog variants / 198 native closed shells:
no inward shells and no vertex normals opposing triangle winding. Existing
zero-area glyph triangles are skipped only for orientation math and retained
in geometry totals (4,981 across the catalog). All prefab/scene mesh references
remained valid. Forty-eight native front/back/side previews were rendered and
visually inspected through three contact sheets; solid boards are visible from
each exterior side and source lettering remains intentionally front-only.

The curated 31-sign layout, placement matrices, Pinefall file bytes and protected
Terrain/snow hashes were unchanged. Scene validation and 19 snow-ledger checks
passed. No scene save/rebake, player-save writes or runtime authoring. This is
Editor mesh/render evidence, not an Android package or A07 acceptance test.
Menu: Snowglow > Asset intake > Validate all sign normals and four-sided renders.
Report/renders: Logs/AlpineSignBoards/Normals/Validation.txt; source audit:
SourceAssets/AlpineSignBoards/NormalsAudit-before.json; corrected audit:
Assets/Snowglow/Catalog/Sources/AlpineSignBoards/NormalsAudit.json. Previous FBX
bytes remain under ignored Logs/AlpineSignBoards/Normals-before.

## Edited sign caption depth correction - 4 November 2026

The apparent open backs on the four edited signs were captions rendering through
solid boards. Eight TextMesh captions now use the authored `WorldSignText.mat`
and `Snowglow/World Sign Text` shader with depth testing (`ZTest LEqual`) and
backface culling. `WorldSignLabel` keeps the authored font-atlas binding current
on enable and font rebuild; it creates no runtime assets or geometry. Sign intake
applies this binding after fitting captions. Preserve the edited wording/layout.

Unity 6000.3.8f1 / Android compiled, saved and reopened Pinefall. Paired renders
with captions enabled/disabled proved front visibility and zero rear caption
pixels on the stacked directory, lodge, fuel/repair and workshop boards. All eight
front/rear renders were visually inspected. Scene validation and 19 snow checks
passed; board geometry, placement and snow seed remained unchanged. This fixes
text rendering separately from the earlier source-normal repairs. No Android
package, device driving or phone performance was tested. Evidence:
`Logs/AlpineSignBoards/LabelDepth/Validation.txt` and `Inspection.png`.

