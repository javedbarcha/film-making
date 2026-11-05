# Detailed faces in Pinefall

The 5 November 2026 arrangement exposes decorated fronts and sides to the fixed
southwest isometric camera. Camera behaviour is unchanged.

| Saved content | Arrangement |
| --- | --- |
| 40 home lots | Whole lots turn 90 degrees: entrances west, decorated sides south, fences behind. Foundations and fences turn together; centres, scales, variants and IDs remain. |
| Four-bay garage | User's later manual centre (-81.76, 5.13, -93.74), facade west. Closed doors remain static. |
| Disposal yard | Original centre (94, 5.13, -85) and 1.4 scale; user's later north-facing entrance retained. Pad still covers the existing disposal accounting rectangle. |
| Directory / Alpine Lodge | Faces southwest. Lodge arrow and distance use the current home-40 entrance. |
| Snow Depot | User subsequently repositioned the board; preserve the current scene pose. SNOW DEPOT is now readable on both faces. |
| Town Center | At (7.5, 12), facing west, arrow toward the junction. |
| Western/southern warnings | Six Road Closed and six Alpine Danger boards retain their road-facing fronts and gain matching rear panels/artwork. |

The saved layout retains **30 signs**. The manually absent Workshop Parts +
Service board is not recreated. Fuel + Repair, Alpine Market, the manually moved
fuel station, machinery and other scenery placements are preserved.

The user's later manual facility arrangement supersedes the initial southern
approaches. Depot access crosses the existing north pad edge at (-97.5, -70)
from the z=-65 road; disposal access follows its north-facing Entrance anchor.
Garage and fuel station face west. Sampled **4.4 m-wide** approach sweeps passed;
these are not actual vehicle-driving tests. Depot service rules and the disposal
processing rectangle are unchanged.

Later red/yellow road cleanup removed the redundant east depot connector and
its collider. The northern road's west arm and the adjacent southwest north/south
road remain, with their interactive starting snow cleared. Only 16 affected snow
chunks were revised; original assets and user placements remain. Starting volume
decreased by 257.937 m3, with no disposal/reward entry. Existing gameplay saves
were initially not migrated. The subsequent road-presentation fix now migrates
old resumed access snow once using authoredLayoutVersion; later pushed snow is
retained. See Logs/DepotRoadCleanup and Logs/RoadPresentation.

All six through-roads and the parking connector now have yellow dashed centre
paint, leaving intersections clear. Paint is authored into one shared mesh and
is covered by snow until plowed. The yellow Contract 0 boundary rectangle is
removed. Low concrete curbs distinguish the depot north edge from the adjacent
road while preserving the 10 m entry opening. No facility/road transforms changed.

## Authored rear artwork

Fuel + Repair also has a matching rear caption, preserving its current front and
manual placement. A small blue rear caption panel on Snow Depot clears its centre
post (12 triangles, existing palette, no collider/light). Both captions use the
existing depth-tested text binding; native paired views and pixel checks are in
`Logs/ServiceSignBacks`. They are saved scene content, with no runtime assembly.

The original sign blend is preserved. `Tools/AssetIntake/export_sign_rear_panels.py`
prepares upper board solids and lettering for variants 04 and 05. Posts, footings
and decorative snow are excluded. Rear boards sit 25 mm beyond original supports,
so posts do not obscure the text. Two shared FBXs under
`Catalog/Sources/AlpineSignBoards` supply twelve saved instances:
**3,654 added triangles / twelve renderers**, existing opaque palette, no new
materials, textures, lights, colliders or runtime assembly. Rear additions do not
cast shadows.

## Editor workflow and preservation

`Snowglow > Environment > Arrange detailed faces for isometric` is repeat-safe.
The saved `Presentation | isometric-facing layout v1` marker prevents another
quarter-turn or placement reset. Repetition validates the arrangement and refreshes
review renders; existing rear groups are reused. Backups are under
`Logs/IsometricLayout`. Saved sign-reading positions support the revised layout;
sign intake recognizes the marker rather than restoring the older 31-sign set.

After manually moving facilities, use `Snowglow > Environment > Reconcile manual
facility access` to refresh obstacle flags and validate the saved entrances. It
preserves facility poses; the saved `Facility access | manual layout v1` marker
and depot RoadConnection anchor override the earlier isometric-front constraint.
The disposal route derives from its existing Entrance anchor. Current reconciliation
changed 655 obstacle flags in a further separate seed, with quantities unchanged.
Backup and native depot/yard views: `Logs/FacilityAccess`. Unity saved/reopened and
passed 1,010 preservation/access assertions, layout/scene validation and 19 snow
checks; this was StandaloneWindows64, not an Android/device acceptance pass.

Moved collisions required **5,862 obstacle-flag changes** in the separate
`IsometricLayoutSnowObstacles.asset` seed. Original seeds and snow meshes remain;
depth, work credit, initial/disposed volume and revision were preserved. No player
save was written, source prefab replaced or district rebaked.

## Evidence and limits

Unity **6000.3.8f1 / StandaloneWindows64** compiled, saved/reopened Pinefall and
passed scene validation and all 19 snow checks. Initial layout: 5,062 assertions;
final repeated pass: 4,802, including the wider approach sweeps. Repeated passes
are not additive coverage totals. Native isometric comparisons, all seven home
variants, warning front/back close-ups and garage Follow/Orbit views were inspected.
Inspection caught posts obscuring the first lettering-only copies; final supported
rear panels resolve it. Pre-save guard failures reverted without saving.

Reports/renders: `Logs/IsometricLayout/Validation.txt`, `Buildings-comparison.png`,
`Homes-comparison.png`, `Signs-comparison.png`, `Warnings-comparison.png` and
`RearPanel-*-front/back.png`. Initial audit: `Logs/IsometricAudit`.

Downward-facing lamps remain underside details; rearrangement cannot expose all
of them to an overhead camera. Text remains small at normal isometric framing.
Live driving/dumping/service routes, touch readability, Android packaging and
sustained Galaxy A07 performance remain untested.
