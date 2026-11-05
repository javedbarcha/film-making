# Verification record

## Reference-based tractor and loader audio - 5 November 2026

User supplied four machinery MP3 references, emphasizing the excavator recording.
Preserved originals/hashes under SourceAssets/MachineryAudio/References. Current
profiles replace earlier procedural operating loops with six short sample-derived
engine/work/drivetrain loops: tractor uses the three tractor recordings; loader
uses excavator sections. Band shaping, 250 ms loop crossfades, gentle compression
and 0.65 peak headroom; mono 24 kHz PCM. Exact source ranges/process recorded in
RecordedManifest.json and source README. Original samples/procedural assets remain.

Saved MachineryAudioProfile bindings switch tractor/loader clips at runtime,
without creating sources/clips. Engine pitch range is restrained for recordings;
speed/load/wheel/slip/bucket/throttle response remains. Added a 0.65 s reference-
textured pneumatic brake cue, once on a moving brake press with 1 s cooldown;
its game use does not assert the original recorded event identity. Existing
reverse alarm is kept separately controlled, not embedded as a whole recording
sequence. Pause/unavailable/parked/fuel/condition/disable stop sound; mute includes
the new brake source. No physics, UI, save schema or district changes.

Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall and passed 878
assertions including recorded profile swaps, PCM/loop/headroom, runtime mix,
brake edge/held/cooldown and mute/state gates. Scene validation and 19 snow checks
passed; seed hash and fleet/catalog transforms preserved. Offline 41 s tractor/
loader listening preview has peak 0.295 (not a live playthrough). Native checks
do not establish audible balance, phone speakers, Android build or A07 costs.
Report: Logs/MachineryAudio/Validation.txt. Preview:
Logs/MachineryAudio/ReferenceAnalysis/Machinery-recorded-preview.wav.


## Load-responsive machinery audio - 5 November 2026

Replaced speed-only motor gain with three Editor-authored operating layers:
original procedural diesel base, loaded diesel/growl and transmission whine.
Mono 24 kHz PCM16 loops (4/4/2 seconds), generated offline by
Tools/AssetIntake/generate_machinery_audio.py; no external recordings or runtime
clip/source factories. Original motor.wav retained. Source/provenance/metrics:
SourceAssets/MachineryAudio. MachineryAudioAuthoring binds saved sources and
PrototypeAuthoring reuses it on explicit future bakes.

SynthAudio reads actual road speed, wheel RPM, throttle/brake, snow resistance,
slip, bucket load and low-speed driving demand. Resting active machinery has an
idle; speed raises engine tone and gearbox whine; struggle raises growl/volume
with mild tone droop. Loader pitch is lower/heavier than tractor. These are
presentation curves, not a new engine/gearing simulation or physics input.
Attack/release smoothing is 0.18/0.5 s. Menus/pause, parked machines, empty fuel,
zero condition and disable stop loops. Existing mute controls all sources; the
reverse warning retains its clip/volume and follows operating/reverse state.
No new UI, purchases or save schema changes.

Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall and passed 846
assertions: PCM headroom/loop boundaries, curve ranges, both machines' runtime
idle/driving/stationary struggle, load droop, loader tuning, mute/unmute and
pause/fuel/condition/parked/disable behavior. Scene validation and 19 snow checks
passed; snow hash and fleet/catalog poses unchanged. No player-save writes or
district rebake. No audible listening/live driving, Android packaging or A07
speaker/performance acceptance claimed. Audition in Play and on phone for final
mix balance. Evidence: Logs/MachineryAudio/Validation.txt. Editor menu:
Snowglow > Audio > Install load responsive machinery audio.


## Three imported snowmen - 5 November 2026

Imported the supplied Three_Low_Poly_Snowmen_1000_Triangles.blend through
CatalogPreparation as three reusable Needs Review Landscape entries snowman-01,
02 and 03. Original source/hash/provenance in SourceAssets/ThreeSnowmen. Classic
top hat, knit cap and earmuffs each retain exactly 1,000 triangles: 3,000 total,
six renderers, two shared materials (opaque clothing palette/existing toon snow).
Source staging offsets, studio ground, cameras and lights excluded; embedded
scripts never executed. Snow bodies are decorative, with no snow-volume effect.

One of each is saved in Pinefall, facing southwest: pinefall-snowman-01 at
(-53.25, 5.01, 48), 02 at (14.54, 5.01, 84.44), 03 at (83, 5.01, -18).
Terrain-grounded placements avoid roads by 2 m, entrance anchors by 4 m, existing
colliders and seeded interactive snow. Narrow body capsule collision avoids
broad invisible twig-arm boxes. Repeat intake preserves placements/no duplicates.
Menu: Snowglow > Asset intake > Import three snowmen.

Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall and passed scene
validation plus 19 snow checks. Three native previews visually inspected. A
separate seed revision reconciled 169 obstacle flags; snow quantities and
existing catalog poses preserved. No district rebake or player-save writes.
Live driving, Android packaging and Galaxy A07 costs remain untested. Evidence:
Logs/ThreeSnowmen/Validation.txt and Snowman-1/2/3.png.


## Rear service sign captions - 5 November 2026

Unity 6000.3.8f1 / StandaloneWindows64 compiled, saved/reopened Pinefall; scene
validation and 19 snow checks passed. FUEL + REPAIR and SNOW DEPOT have saved
rear-facing captions using existing depth-tested text/atlas bindings. A 12-triangle
blue depot rear panel clears the centre support, with existing palette and no
collider/light. Paired native front/back renders inspected; pixel comparisons
prove rear captions visible from behind and invisible from the front. Existing
front captions and current catalog/fleet poses retained; snow seed hash unchanged.
No player-save writes or district rebake. Backup/report/images: Logs/ServiceSignBacks.
Live driving, touch readability, Android packaging and A07 costs untested.

## Road paint, separator and legacy resumed snow - 5 November 2026

Unity 6000.3.8f1 / StandaloneWindows64 compiled, saved/reopened Pinefall; layout,
scene and 19 snow checks passed. Authored yellow centre dashes use one combined
renderer/shared material across six through-roads and parking access, with clear
junction gaps. Depot Contract 0 boundary removed; two low concrete curb spans
retain the 10 m north entry and clear 4.4 m sampled route. Facility/fleet/road
poses and original snow/Terrain asset hashes retained. New seed adds layout
version only; starting snow quantities unchanged. Native depot top/entry and
yard-context renders inspected. Synthetic old JSON restored via SnowField.Initialize
clears obsolete disposal/depot access snow exactly once, adjusts initial volume,
preserves outside snow/credit/disposed/activity revision, and retains later snow
after serialization and another restore. No actual profile writes during tests.
This supersedes the earlier statement that old jobs retain redundant access snow.
Reports/backups/renders: Logs/RoadPresentation. Full live profile/driving testing,
Android packaging and Galaxy A07 performance remain untested.

## Depot approach road/snow cleanup - 5 November 2026

Unity 6000.3.8f1 / StandaloneWindows64 compiled, saved/reopened and passed layout,
scene and 19 snow checks. Per-cell comparisons verify only marked starting snow
changed: 4,040 cells / 257.937 m3 removed, initial volume reduced accordingly,
disposed/revision retained. Original snow/Terrain assets and outside depth/credit
unchanged. Removed east depot spur/collider only; north/east through-roads retained.
Facility/fleet poses unchanged. Sixteen affected chunks rebuilt into a separate
7.95 MB asset; no district rebake or player-save migration. Native depot top/entry
views inspected. Evidence: Logs/DepotRoadCleanup/Validation.txt and After images.
Existing saved gameplay snow remains unchanged. Depot-access contract target now
starts clear; future core-loop tuning must revisit that objective. Driving,
Android packaging and Galaxy A07 performance remain untested.

## Manual facility access - 5 November 2026

Unity 6000.3.8f1 / StandaloneWindows64 compiled, saved/reopened Pinefall and
passed 1,010 facility/preservation assertions, existing isometric layout/scene
validation and 19 snow checks. Garage/fuel/yard manual poses and fleet retained
exactly. Depot uses the red-marked northern edge from the z=-65 road; yard keeps
its north-facing Entrance anchor. Both sampled static routes clear at 4.4 m
width. Snow Depot sign arrow/read position now indicates the north entry.
Native depot/yard entry renders inspected. This supersedes the earlier southern
approach checks. Logs/FacilityAccess contains before-scene backup, native views
and Validation.txt. A separate seed changes 655 obstacle flags only; original
seed/mesh/Terrain hashes and all snow quantities/revision retained. No player-save
writes or district rebake. Physical driving/dumping/refuelling, Android build and
Galaxy A07 performance remain untested.

## Isometric-facing arrangement - 5 November 2026

Unity 6000.3.8f1 / StandaloneWindows64 compiled, saved/reopened Pinefall and passed
scene validation plus 19 snow checks. Initial arrangement: 5,062 assertions;
final repeated pass after rear-panel correction: 4,802. Checks cover home/road
footprints, front/side directions, garage/fleet penetration, disposal accounting
footprint, sign reading sightlines/arrows, identities, unaffected poses and snow
values/hashes. Final sampled approach sweeps are 4.4 m wide; no live driving claim.
Repetition retained placements instead of turning lots again.

Native isometric before/after, seven home variants, warning front/back close-ups
and garage Follow/Orbit views inspected. Posts obscured first rear text copies;
source-derived backing boards resolve it. Twelve additions total 3,654 triangles
and twelve opaque renderers, shared existing palette, no extra lights/colliders.
Thirty signs retained. Original Terrain/snow assets, quantities/revision and
player saves preserved; separate seed changes 5,862 obstacle flags. No district
rebake/camera change. Pre-save failures reverted; backups/reports/renders:
Logs/IsometricLayout. See [ISOMETRIC_LAYOUT.md](ISOMETRIC_LAYOUT.md). Actual
driving/dumping/service, touch readability, Android packaging and A07 costs untested.

## Isometric roof clipping and tracking correction - 5 November 2026

Isometric no longer shares perspective obstacle pull-in. That sweep shortened
the camera offset near canopy/roof collision, placing its wide orthographic
near plane through geometry and overriding smoothing. The camera now smooths
its focus once, holds 41-degree pitch/45-degree yaw, and sits at least 60 m back
along the view axis (farther for large orthographic framing). Terrain safety
retreats along the same axis, preserving composition. Orthographic size remains
12 m at rest / 19.2 m at full speed, 1.1 s response. Follow/Orbit collision paths
remain in place. No roof hiding, new user controls, scene rebuild or save migration.

Unity 6000.3.8f1 / Android compiled and passed 1,728 assertions: stationary canopy
cannot pull the camera inward, 600-frame 2 m/s crossing with maximum per-frame
step 0.03334 m, fixed orientation/centred subject, near/far and paused speed zoom,
four aspect-ratio terrain/near-plane checks, and garage/station bounds beyond
the near plane. Three native depot renders inspected; scene validation and
19 snow checks passed. Saved Pinefall bytes unchanged; unsaved scene preserved
separately when present. No player-save writes. These sampled checks are not
a full live driving test or Android/A07 acceptance. Evidence:
Logs/IsometricRoof/Validation.txt and Fuel-canopy/Garage-roof/Tractor-isometric.png.


## Imported snow machinery fuel station - 5 November 2026

Imported the supplied Low_Poly_Snow_Machinery_Fuel_Station.blend through
CatalogPreparation, replacing only the primitive red Fuel tank. Reusable
BuildingAssembly `snow-machinery-fuel-station` remains Needs Review. Source has
3,088 triangles excluding studio ground; prepared output has 3,040 after removing
48 zero-area faces (no visible geometry removed), three renderers and two shared
materials. Separate Body, opaque Luminous lenses and removable decorative
SnowCap groups; no new real-time lights or hose/nozzle interaction. Original
blend/hash/provenance preserved in SourceAssets/SnowMachineryFuelStation.

Saved placement `pinefall-fuel-station` at (-110, 5.13, -85) is 2 m inward from
the old tank centre, fitting the larger canopy beside the standing Fuel + Repair
sign. Sign and garage remain unchanged, with nonintersecting bounds. Station
stays within the existing 25 m depot service zone; servicing mechanics unchanged.
Static shape collision leaves drive-under space open. Repeat intake retains
manual placement; explicit future district baking reuses the prepared station.
Garage intake also recognizes the new fuel facility instead of requiring a tank.

Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall and passed scene
validation plus 19 snow checks. Front/rear/depot-context renders inspected. A
separate seed revision updated 62 obstacle flags; depth, credit, initial/disposed
volumes, original snow meshes and other catalog transforms were preserved. No
player-save writes or district rebake. Physical driving/refuelling, Android
packaging and Galaxy A07 performance remain untested. Evidence:
Logs/FuelStation/Validation.txt. Menu: Snowglow > Asset intake > Import snow
machinery fuel station.


## Imported snow disposal yard - 5 November 2026

Replaced Pinefall's primitive disposal walls/markers and old asphalt pad with
the supplied Snow_Disposal_Yard_Only.blend through CatalogPreparation. Reusable
BuildingAssembly `snow-disposal-yard` remains Needs Review: 972 triangles, three
renderers, two shared materials. Studio Plane/cameras/lights excluded; embedded
text never executed. Original source/hash/provenance in SourceAssets/SnowDisposalYard.
Pure snow/cyan caps remain separately removable decorative geometry. No runtime
assembly or added real-time lights. Static pad/wall shape collision keeps the
entrance open rather than blocking the whole yard with an aggregate box.

Saved placement `pinefall-disposal-yard` is centred at (94, 5.13, -85), uniform
scale 1.4, entrance facing north. Existing disposal processing rectangle and
runtime accounting remain unchanged. Redundant west road/snow strip stays removed.
Repeat intake retains placement; future district baking uses the prepared yard
when present in the catalog. Current intake performed no district rebake.

Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall and passed scene
validation plus 19 snow checks. Front/rear/top native renders inspected. Updated
obstacles use a separate seed revision; depth, credit, initial/disposed volumes,
original snow meshes and all existing catalog transforms were preserved. No
player-save writes. Physical driving/dumping, Android packaging and Galaxy A07
performance remain untested. Evidence: Logs/DisposalYard/Validation.txt.
Use Snowglow > Asset intake > Import snow disposal yard.


## Tractor left rear tyre guard - 5 November 2026

Corrected inward winding on the mirrored rear fender in prepared TractorBody.fbx.
Source left guard had negative signed volume while its opposite was outward.
Exporter now repairs prepared rear-guard normals and asserts positive volume;
`--body-only` preserves separate blade/beacon files. Source blend, metas, shape,
placement and wheel physics remain unchanged. Body catalog remains 5,548 triangles.
Unity 6000.3.8f1 / Android reimported, passed scene validation and 19 snow checks;
native left/right starter-prefab renders inspected with solid outward guards.
No scene/prefab reconstruction or player-save writes; phone driving untested.
Evidence: Logs/TractorGuard/Validation.txt and paired Tractor-left/right.png.


## Four-bay garage and standing fuel sign - 5 November 2026

Imported the supplied low-poly garage through CatalogPreparation as the reusable
BuildingAssembly `four-bay-snow-garage` (Needs Review). Its 3,832 triangles include
four separate doors and mesh bay numbers; preview ground/cameras/lights excluded.
Original blend preserved with SHA256/provenance in SourceAssets/FourBaySnowGarage.
Opaque palette/windows and removable toon snow retain the project style. Static
shape collision follows the model; automatic garage-door operation is not added.

Pinefall's primitive workshop and old wall lantern were removed. The facade
Fuel + Repair board was replaced by a standing imported hanging-board variant
beside the red fuel tank, facing the depot forecourt. Placement ID
`pinefall-sign-10` is retained; the sign now uses `alpine-sign-10`. The tank and
Workshop Parts + Service board remain. Sign wayfinding intent was updated.
Garage intake is repeat-safe and preserves subsequent garage/sign transforms.

Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall and passed scene
validation plus 19 snow checks. Garage front/rear, fuel-sign and combined depot-context renders inspected.
Snow depth, credit and volume preserved; obstacle updates use a separate seed
revision. Original seed/meshes and player saves untouched; no district rebake.
Driving clearance, Android packaging and Galaxy A07 costs remain untested.
Evidence: Logs/FourBayGarage/Validation.txt. Reimporting older environment sets
may retain older validation assumptions about the removed facade fixture.


## Disposal access snow strip correction - 4 November 2026

User clarified that the unwanted strip was interactive snow extending into the
yard, remaining after the asphalt removal. Removed only its 1,000 authored seed
cells (x > 70 and x < 95, abs(z + 85) < 5), retaining the main north/south road.
Revised starting volume by 65.851 m3 and rebuilt only two affected mesh chunks
into new assets. Original seed/meshes retained; outside depth/credit unchanged.
SnowField.Seed no longer generates dumpLink. This is an authored starting-layout
change, not gameplay disposal; player saves untouched. Unity saved/reopened
Pinefall; scene validation and 19 snow checks passed. Phone playtest untested.
Evidence: Logs/DisposalAccess/SnowValidation.txt.


## Disposal yard west access removed - 4 November 2026

Removed the redundant 30 x 10 m road segment entering the disposal yard from
the west at (78, -85), including its collider. Yard, other roads and snow seed
remain unchanged. DistrictFactory no longer creates this spur; no district rebake
was performed. Unity saved/reopened Pinefall, passed scene validation and all 19
snow checks. Phone playtesting remains untested. Evidence:
`Logs/DisposalAccess/Validation.txt`.


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

## Editor asset recovery and supplied light icons - 4 November 2026

The Editor reported an unreadable GoldenTractor-Starter.prefab and a placeholder
machine while reopening Pinefall. The 214,607-byte prefab contained only zero
bytes. Restored the intact staged Git blob, preserving the prefab GUID and all
other scene/model work. Corrupt/restored copies are under Logs/EditorRecovery_20261104.
Scanned Assets/ProjectSettings/Packages for remaining zero-filled and NUL-containing
text assets: none remained. A generated empty LayerSettings JSON was already
recreated by Unity. Earlier Unity Remote failures reported no USB device; ADB
currently lists no devices. No C# compilation error was found in those logs.

Native Unity 6000.3.8f1 reimported the restored prefab, reopened existing Pinefall
without saving placeholder damage, and passed both fleet bindings, scene
validation and 19 snow-ledger checks. Any unsaved scene is preserved as a recovery
copy before reopening; no district rebake/player-save write. Evidence:
Logs/EditorRecovery_20261104/Validation.txt.

Both supplied teal PNGs are preserved intact and imported as cropped sprites.
The saved right-center button is 48x48, with hidden text and untinted on/off
artwork. Native direct command checks passed sprite swaps for both vehicles and
paused input; saved/reopened sprite references/listener and size/anchor checks
passed. Inspected the rendered HUD. Both beacon lenses retain constant amber
through eight animation samples while halo visibility flashes fully on/off;
rotation, shaders and all lighting checks passed. Evidence: Logs/VehicleLights/
Validation.txt, AmberBeacon.txt and LightsButton.png. The current validation run
used StandaloneWindows64; Android builds, physical touch size and A07 profiling
remain untested. Earlier Console entries are retained as historical evidence.

## Solid lamp-box geometry - 4 November 2026

Tractor rear lamps now have explicit solid white 25x19x10 cm housings outside
the ballast. Both machines have orange 26x23x10 cm front boxes and white rear
boxes, permanently visible with illumination off. Counts are four boxes on
tractor and six on loader. All are Editor-authored prefab geometry; original
model meshes, rigs and collision setup remain unchanged. Native checks inspect
counts, material colors and renderer visibility when switched off; renders
include `Tractor/Loader-front-boxes-off.png` and `rear-boxes-off.png` under
Logs/VehicleLights. Results are recorded separately in `LampBoxes.txt` after
the installer completes. Live driving and phone appearance remain untested.

Completed: Unity 6000.3.8f1 saved/reopened both updated prefabs and Pinefall;
LampBoxes.txt passed counts, exact colors and persistent visibility with lamps
off. Inspected all four front/rear off renders: orange front housings and white
rear boxes are visible on both vehicles. Existing light-button, independent
beacon, paired pixel, shader, scene and 19 snow-ledger checks also passed.

## Light switch, warm front and white rear glow - 4 November 2026

- Added a 96x64 button centered vertically at the safe-area right controls, with
  persistent onClick binding. Saved/reopened the UI in Pinefall and both updated
  machine prefabs. Direct button-command fixtures exercise both selections and
  paused input; the selected machine's label/state tracks its own session toggle.
- Lamp-off checks confirm front/rear beams, surfaces and halos are disabled while
  the red beacon and its halo remain enabled and its rotor advances. Pause,
  parked, empty-fuel, broken-condition and disable behavior remain checked.
- Front beams are warmer and intensity 10 (previously 7). Rear surfaces are white
  in both gears. Four tractor and six loader lamp glow quads are authored in
  addition to each existing beacon halo, without extra real-time lights.
- Unity 6000.3.8f1 / Android compiled and passed scene validation, both shader
  checks, paired lamp pixel checks, button/state checks and 19 snow-ledger checks.
  Sixteen native vehicle renders and one HUD render were generated. Inspected
  the button placement, tractor/loader front glow, loader rear-white glow and
  beacon-on/lamps-off view. Fleet/environment poses, IDs and seed hash preserved.
- Report: `Logs/VehicleLights/Validation.txt`; HUD: `LightsButton.png`.
  Touch clicks, phone ergonomics, all views during continuous driving, Android
  packaging and Galaxy A07 costs remain untested. No player-save schema/write.

## Paired lamps and rotating red beacons - 4 November 2026

User screenshots exposed mismatched paired lamps and static roof beacons. The
lamp exporter created a new material per part, leaving extra imported slots that
the controller did not update. Export now shares one material; authoring binds
exactly one slot and checks one submesh per group. Prepared closed lamp surfaces
also receive outward-normal validation. Original blends/body meshes are preserved.

The rear pair now switches together from red running lamps to white in reverse.
Each red roof lens rotates around its own center, with an opaque sweeping highlight
and one small additive billboard halo. No beacon point lights, global bloom or HDR
were added. Pause/park/engine-unavailable behavior remains off. Two front spots and
the mobile URP light budget remain unchanged. Native evidence and render results
are recorded in `Logs/VehicleLights/Validation.txt`; device play/profiling remains
untested.

Unity 6000.3.8f1 / Android saved/reopened the updated prefabs and scene. Fourteen
native renders were produced and front/rear views inspected: both front lamps
are white, both rear lamps red in forward and white in reverse, and low/bright
red beacon phases have distinct glow. Pixel samples independently check both
front lamps and both rear lamps/colors. Rotation, pulse variation, pause freeze,
single-material bindings and both beacon shader compilations passed. Scene and
19 snow-ledger checks passed; environment/fleet poses, save IDs and seed hashes
were retained. Rear shells moved 6 cm behind the source ballast surface to avoid
occlusion. An Editor-only SendMessage assertion in the test harness was replaced
with a direct shutdown-helper call; normal runtime disabling delegates to it.

## Operating vehicle lights - 4 November 2026

- Unity 6000.3.8f1 / Android compiled the runtime controller and Editor installer.
  Both equipped prefabs and Pinefall saved/reopened with three source-derived
  luminous groups and two shadowless spots per machine. Original rig meshes,
  vehicle transforms/save IDs and snow seed remained unchanged.
- Native state checks passed for operating, menu/pause, parked selection, empty
  fuel, zero condition, disable handler and return to operation. These exercise
  authored bindings directly in Edit mode, not a continuous driving playtest.
  Scene validation and all 19 snow-ledger checks passed; no player save writes.
- Mobile URP additional-light limit increased from zero to two so beams can
  illuminate surfaces. Rear lamps/beacons add no real-time lights. Beam range is
  22 m with shadows off. Device performance is not established by these limits.
- Report: `Logs/VehicleLights/Validation.txt`; controls and authoring details:
  `VEHICLE_LIGHTS.md`. Live driving, pause/machine-switch playthroughs, Android
  packaging and Galaxy A07 light/frame-time profiling remain untested.
- Four native on/off preview renders were generated. Inspected both operating
  views and the loader's off view: forward ground illumination and amber beacon
  visibility are present. This isolated dim-light fixture does not change the
  district's daytime lighting or prove visibility/clearance from every angle.

## Selected mountain refitting - 3 November 2026

- Added **Snowglow > Environment > Refit selected mountains to terrain** for
  selected lot groups, mountain roots or mesh children. No Pinefall placements
  were automatically refitted. Scene saving remains a manual authoring action.
- Native Unity 6000.3.8f1 / Android compiled and passed disposable fixture checks
  for all five variants on uneven Terrain, parent/child selection/deduplication,
  yaw/pitch and uneven ancestor scale, preserved X/Z/rotation/scale, buried bases,
  unchanged upper peaks/geometry counts, collider coverage, repeat fitting,
  full Undo/Redo and scene save/reopen with persistent mesh references.
- Checks compare original catalog dependency hashes and Pinefall scene bytes;
  both remained unchanged. No player save was read/written, Terrain/snow data
  changed or district rebaked. Fixture scene, terrain and meshes are cleaned up.
- The checks exposed and fixed tilted-base seating and conservative physics-box
  fitting under sheared ancestor transforms. Meshes are rebuilt from sources
  into independent persistent revisions so Undo does not overwrite old geometry.
- Report: `Logs/SelectedMountainRefit/Validation.txt`. Menu to rerun:
  **Snowglow > Environment > Run selected mountain refit checks**.
  Rendered seam inspection after manual placement, manual menu interaction,
  Android build and Galaxy A07 profiling remain untested. Sampled edge clearance
  does not establish exact contact along every terrain triangle.

## Mountain terrain seating correction - 29 October 2026

- The previous lowest-footprint seating rule left too much of some models below higher terrain. Replaced it with Editor-authored terrain-conforming mesh copies for all 12 placements. Each source vertex keeps its height above the model base, added to local Terrain height; the mountain is then buried by a small uniform depth. This preserves the complete upper features above the underlying ground instead of accepting only one visible peak vertex.
- Base side-wall edges are sampled at 33 positions to prevent a straight edge bridging above terrain hollows. Burial is 2.00-4.13 m for the current placements (roughly 2.4-6.1% of model height). All cap vertices are above terrain, with minimum cap relief 13.02-33.38 m and maximum relief 42.86-122.67 m. All source vertex/index counts are unchanged. Normals/bounds and conservative background box proxies are refreshed.
- Saved 24 scene-specific Rock/SnowCap mesh assets under `Assets/Snowglow/Catalog/SceneMeshes/AlpineMountains`; raw FBXs, catalog prefab geometry and original Blender source remain unchanged. Source-relative fitting always reads the original catalog mesh, avoiding cumulative deformation. Future full district authoring and the mountain intake invoke the same Editor-only fitting path. No runtime mesh authoring was added.
- Unity 6000.3.8f1 / Android compiled, fitted, saved/reopened Pinefall and verified persistent fitted mesh references, unchanged vertex/index counts, buried base vertices, every cap vertex above Terrain and over 85% height relief at each peak. Scene validation and all 19 snow-ledger checks passed. Several fitted mountains and their bases were visually inspected in Scene view.
- Existing X/Z positions, yaw, scale, variant mix, homes/fleet, snow and Terrain assets are preserved. No terrain/snow/district rebake or player-save write. Reports: `Logs/AlpineMountains/TerrainFit.txt`, `TerrainFitValidation.txt`, `UnityIntake.txt`; scene backup: `Pinefall-before-terrain-fit.unity`. Full perimeter/collision walkthrough, current Android appearance and Galaxy A07 performance remain untested.


## Five alpine mountains intake - 29 October 2026

- Preserved `Five_Alpine_Mountains.blend` byte-for-byte under `SourceAssets/FiveAlpineMountains`; source/copy SHA256 `382B0050FB4E9ACB40D17B4904C4B35950DCF2400A42D0F5863F47574CFCBF73`. Inspected every named collection and all five source renders with embedded-script autoexecution disabled; embedded text was not executed. Part maps account for each model object and exclude presentation ground, labels, cameras, lights and movement empties.
- Exported five metre-scale FBXs with separate Rock/SnowCap meshes and a shared rock palette. All ten closed solids passed outward-winding checks; no repairs or extra geometry were needed. Prepared stable catalog IDs `alpine-mountain-01` through `05` remain Needs Review. URP Simple Lit approximates source rock node graphs; snow uses the existing opaque white/cyan toon material. Conservative background box collision proxies are not detailed climbable surfaces.
- Unity 6000.3.8f1 / Android compiled the intake, prepared/validated all five reusable prefabs, replaced all 12 sphere mountains and old caps, saved/reopened Pinefall, and passed scene validation plus all 19 snow-ledger checks. Balanced Editor shuffle seed 291029 places Needle Peak/Twin Summits three times each and Sawtooth Ridge/Glacier Massif/Alpine Cirque twice each. Random yaw and uniform scale fit each old horizontal footprint; X/Z positions are preserved. Bases sit two metres below the lowest 9x9 sampled terrain height in those footprints.
- Repeated native intake preserved the saved placement mix/transforms. Editor mesh readback (without enabling player CPU readability) checked cap vertices against Terrain heights: each placement has cap vertices 18.97-101.11 m above terrain, so none is fully buried. Five native prefab renders were visually inspected and one placed mountain was inspected in Scene view. This is not a full seam/sightline walkthrough.
- Native snapshots verified all 16 enlarged home IDs/assets/transforms, fleet bindings and snow-seed dependency hash unchanged. Independent file/YAML checks verified 12 unique mountain IDs, no primitive mountain groups/caps, preserved home lots, source hash/complete maps and all new files below 90 MB. Existing terrain, snow geometry, vehicle/home prefabs and player saves were not regenerated. Full district bake was not run; its Editor path now reuses the prepared mountain set.
- Report/previews: `Logs/AlpineMountains/UnityIntake.txt`, `NN-unity.png`; pre-change scene backup: `Pinefall-before.unity` in that folder. Whole-layout sightlines, terrain seam/collision walkthrough, current Android packaging/shader appearance and Galaxy A07 sustained timings remain untested.


## All houses replaced and enlarged - 29 October 2026

- Replaced every prototype cottage: all 16 saved lots now use imported homes. Editor-only balanced shuffling (seed 291026) places Red Cottage and Green Porch three times each; Garage Home, White Colonial, Storybook Cottage, Twin A Frame and Brick Barn twice each. Every variant is used; no runtime randomization/construction was added.
- Houses are uniformly enlarged 1.5x relative to the previous per-lot fitting calculation. Foundation X/Z footprints increase exactly 1.5x; height/ground level and fences remain unchanged. Catalog maximum placement scale is 1.5. Existing home IDs are preserved, new IDs are unique, and repeat intake preserves the saved layout without cumulative scaling. Future full bakes use this all-lot path.
- Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall, confirmed all 16 placements and seven variants, passed house/road and house/house footprint overlap preflight, scene structural validation and all 19 snow-ledger checks. Independent saved-YAML checks verified 16 unique home IDs, no primitive cottage groups, and all 16 foundations' exact horizontal size change with unchanged height. District overview inspected in Scene view. Report: `Logs/WinterHomes/UnityIntake.txt`.
- No snow seed/geometry, terrain, vehicle prefab or player save was regenerated. Pre-change scene backup: `Logs/WinterHomes/Pinefall-before-all-homes.unity`. Live vehicle clearance, snow interaction around larger foundations, current Android build/appearance and sustained Galaxy A07 profiling remain untested.


## Wall winding correction - 29 October 2026

The right wing of Red Cottage, living wing of Garage Home and cross wing of White
Colonial had inward-facing closed wall shells (seven polygons each). Prepared
exports now recalculate consistent outward face winding per closed source solid
before joining; intentionally open facade panels retain their authored direction.
All 958 closed solids passed outward-volume checks, and a Blender FBX round trip
found zero inward closed shells across all seven exports. NormalValidation.json
records the three repairs. Tools/AssetIntake/check_winter_home_normals.py repeats
the exported-shell check with Blender --background --factory-startup --disable-autoexec.

Unity reimported the FBXs, saved/reopened Pinefall, passed structural validation
and all 19 snow-ledger checks, and regenerated the seven prefab previews. The three
corrected facades were visually inspected. Triangle/vertex counts, mesh/material
GUIDs, original .blend, existing placements, removable snow and opaque windows
are preserved. No district or snow rebuild was performed. Android/device rendering
and continuous vehicle clearance remain untested.

## Seven winter homes intake - completed 29 October 2026

- Preserved the supplied Blender file byte-for-byte (SHA256 `2AD3BEC380E5EE05A400561C0E9CA5445607C4D389EBFBF46EF4E4DECB9F8CDE`). Inspected all seven collections and rendered their source views with embedded-script autoexecution disabled. Complete part maps account for every house-collection object without duplicates; display tiles, fonts, cameras and lights are excluded.
- Prepared seven catalog entries/prefabs, IDs `winter-home-01` through `winter-home-07`. Four merged renderers per house retain architecture, opaque warm windows, detachable roof snow and ground drifts. Ground drift bases are clipped/capped on prepared copies; source geometry stays intact. Compound wall/deck/step colliders and entrance anchors are saved for review.
- Fixed the initial triangle-count assertion: six FBX architecture meshes contain eight zero-area triangles each, verified by Blender round-trip inspection. Export metrics now record these explicitly; native Unity counts must equal export triangles minus zero-area triangles. Actual prepared geometry is 1,002-2,848 triangles / 2,078-5,820 imported vertices per home. See `MOBILE_ASSET_BUDGET.md` for individual counts.
- Unity 6000.3.8f1 with Android selected compiled the Editor intake, prepared/validated all seven prefabs, replaced seven cottage buildings near the depot, saved and reopened Pinefall, and passed scene structural validation plus all 19 snow-ledger checks. Foundations/fences, fleet bindings and snow seed dependency hash were preserved. Existing generated snow assets, terrain and vehicle prefabs were not regenerated; player saves were not loaded or written.
- Rendered seven native Unity prefab previews and inspected the saved cottage in Scene view. Preview key-light direction was corrected to illuminate the visible facades; all seven corrected previews were inspected. Repeated intake preserved exactly seven placements and passed the same saved-scene checks. Entries remain Needs Review; live vehicle/house clearance, full placement/collider walkthrough, Android packaging, shader appearance on phone and sustained Galaxy A07 timings remain untested. Reports/previews: `Logs/WinterHomes/UnityIntake.txt`, `NN-unity.png`; scene backup: `Pinefall-before.unity` in the same ignored folder.

## Keyboard / loaded-bucket control fixes - 28 October 2026

- Prevent arrow/Space gameplay input from also navigating/submitting selected HUD buttons; restore UI keyboard navigation in menus. This addresses a confirmed input-routing conflict, but the user's complete keyboard outage has not yet been reproduced and its Editor/build context is awaiting clarification.
- Move the loader's 92%-full Auto carry check outside the moving/collecting branch, covering stationary/restored loads. Preserve a higher lift and the dump pose. Explicit lift/tilt axes immediately enter Manual; lowering into the work pose re-enables collection. Snow transfer arithmetic and saved equipment geometry are unchanged.
- Runtime and Editor sources (including the new regression helper) compiled with Unity 6000.3.8f1's Roslyn compiler and the project's existing Unity assembly/reference response files. Output is isolated in ignored `Logs/ControlsCompile`, not installed into the running Editor. Editor compile has one pre-existing `AssetWorkshopWindow.title` hiding warning. This is a C# compile check, not a completed Unity scene/play test.
- Added **Snowglow > Run driving input and loaded bucket checks**, queued with `Logs/DrivingControls.request`. It exercises injected keyboard events, HUD navigation routing, actual imported loader mouth elevation/full/partial/manual/dump poses and the 19 snow ledger checks without loading/writing player saves. Execution is pending refresh of the existing Editor, whose main window was unavailable to automation. Read a fresh `Logs/DrivingControls.txt` before claiming these checks passed. Physical keyboard/Game view focus, continuous driving/loading and Android device testing remain pending.

## Soft matte toon snow - 28 October 2026

- Reference revision: user requested pure-white tops and pale cyan shading from a snowy mountain image. Updated mesh palette to `#FFFFFF` / `#BFE7EE`, added upward-normal whitening and reset terrain layer tint to white. Unity imported the shader and passed the material/binding/seed/scene checks again (`Logs/SoftSnowMaterial.txt`). Final reference revision was not visually inspected because the Unity window closed before capture; Android appearance remains untested. The off-white palette below records the earlier iteration.

- Applied off-white `#F1F3EE` and icy-blue shade `#AFCBE2` to the shared opaque snow mesh material using a lightweight URP shader. This covers plowable snow, piles, snow caps and carried snow through existing palette bindings. Terrain retains supported URP Terrain/Lit with its snow layer tinted off-white and metallic/smoothness zero. An experimental custom terrain pass failed visual review and was removed.
- Unity 6000.3.8f1 / Android compiled Editor code and imported the mesh shader without reported shader errors. Material GUIDs, seed dependency hash, chunk bindings and terrain instancing were preserved; Pinefall scene structural validation passed. Visually inspected snow surfaces, roof caps and restored terrain rendering in Unity Scene view. See `Logs/SoftSnowMaterial.txt`. Future Editor district bakes apply the same defaults; a full district rebake was not run for this material change.
- Snow geometry, volume mechanics, vehicle physics and scenery layout are unchanged. Android player shader compilation, current gameplay with the new material and Galaxy A07 appearance/frame timing remain untested. Task 2 (finished scenery asset) is deferred.

## Bright opaque vehicle windows - 28 October 2026

- Replaced transparent glazing on both imported vehicles with the shared `Snowglow/Toon Vehicle Windows` URP shader: bright cyan, pale diagonal painted reflection streaks, opaque geometry queue, solid alpha and depth writes. Object-space projection works with both existing combined meshes without new UVs, textures, triangles or runtime construction. Imported single-plane windows remain visible from both sides.
- Updated the two existing shared window materials in place, preserving prefab/scene references, plus future model-intake material creation. The shader is unlit and does not sample lighting, reflection probes or textures. This removes transparent window blending/sorting; it does not establish a measured FPS improvement.
- Unity 6000.3.8f1 / Android compiled runtime/Editor code and the Editor shader, verified opaque material state, both saved equipped prefab bindings and the current Pinefall scene. Inspected the tractor's opaque cyan windows and diagonal highlights in Unity. See `Logs/ToonWindows.txt`. Android player shader compilation and Galaxy A07 timings remain untested.

## Loader tyre clearance and raised tread repair - 27 October 2026

- Corrected the bucket/tyre overlap by extending the prepared boom reach 0.8 m, moving its bucket pin accordingly, and reducing the working boom pitch from 18 to 9 degrees. The boom mesh stretches with the new pin; bucket-local shape, intake and snow-load anchors are unchanged. Updated both catalog body and equipped prefab, its equipment COM and Pinefall in Edit mode.
- Found all loader tread block faces pointing inward, plus inconsistent winding on tractor chevron grips. Recalculated outward faces on prepared mesh copies: 66 blocks per loader wheel and 40/32 per rear/front tractor wheel. Loader tread tips already protrude to 0.73 m over the 0.68 m tyre carcass, so no geometry was added. Source .blend files remain untouched. Export scripts assert outward orientation for every block; saved `TreadValidation.json` files record the results.
- Unity 6000.3.8f1 / Android saved/reopened the scene and passed 441 lift/tilt samples against a conservative tyre envelope allowing for steering. Minimum longitudinal gap was 0.239 m (required >0.08 m). Work/carry/dump ground clearance, load display, machine identity/controller bindings and all 19 snow checks passed. See `Logs/LoaderIntake/ClearanceChecks.txt` and `UnityValidation.txt`.
- Inspected the corrected loader in Unity: bucket stands clear of the tyres and grips have solid outward-facing surfaces. Triangle counts remain 8,424 for the loader including snow load and 7,274 for the tractor. Continuous motion, suspension/body contacts, live driving and phone profiling remain untested; the sampled geometry check does not replace a full playtest.

## Imported snow loader replacement - 27 October 2026

- Preserved `snow_loader_lp.blend` byte-for-byte in `SourceAssets/LowPolySnowLoader` (SHA256 `365804EDC406BD3D021C8156956C984815D4567F30ABAA58D2F29ED993467327`). Inspected named parts without executing embedded text. Exported evaluated meshes/curve hoses into body, four wheels, boom and bucket modules; excluded presentation objects.
- Saved two Needs Review catalog entries, a fitting/recipe and `ImportedSnowLoader.prefab`. Replaced Pinefall Fleet[1], retaining the `wheeled-loader-01` save ID, 1,400 price, initial unowned state and existing upgrade/ownership lookup. The imported free tractor, scenery and saved snow assets are unchanged. Full district Editor baking now uses both imported machinery prefabs.
- Authored separate boom and bucket tilt pivots, connection/intake/load anchors, wheel rig, collision proxy and snow spray. The low working pose and full-lift automatic dump passed geometric shell/edge clearance checks. Added per-rig minimum dump lift; existing machines retain 0.55, imported loader uses 1. Hydraulics/chassis articulation remain prototype approximations.
- Unity 6000.3.8f1 / Android compiled, saved and reopened Pinefall; passed persistent meshes, rig/controller initialization, carry/dump/load display, drive/park, purchase/save identity and all 19 snow checks. The 39 workshop checks also passed. Fixed catalog prototype reimport overwriting an existing wheel entry's physics from the newly replaced fleet. Palette colors and rest alignment were inspected in Unity; the source render was also inspected.
- Loader geometry: 7,656 triangles before carried snow; 8,424 triangles / 15,095 imported vertices / 8 renderers / 9 material slots including its inactive load visual, excluding particles. Shared opaque palette and separate glass reduce color-related submissions. Vertex splits explain the difference from Blender's 4,740 evaluated vertices. See `MOBILE_ASSET_BUDGET.md` and `Logs/LoaderIntake/UnityValidation.txt`.
- Catalog now warns at provisional vehicle review thresholds of 15,000 triangles / 30,000 imported vertices, alongside material-slot warnings. These do not establish device performance. Live driving/snow interaction, all manual poses, hydraulic linkage fidelity, APK and sustained Galaxy A07 measurements remain untested. No player save was modified.

## Imported tractor as free starter - 27 October 2026

- Replaced only Pinefall's first fleet machine with the saved `GoldenTractor-Starter.prefab`. Its physics definition costs zero, uses the existing `municipal-plow-01` save ID, and is owned initially. Session initialization now guarantees starter ownership even if an old profile recorded it as unowned. Existing coins, upgrades, active selection and loader purchase behavior are preserved.
- Bound the imported model's four wheel pivots to WheelColliders with separate front/rear radii, and connected its fitted blade working edge, existing work/lift/angle controls and preauthored snow spray. Mass includes the beacon and plow. Full district Editor baking also uses this starter prefab. Catalog source entries retain their review status; this is an explicit prototype integration, not a claim of production approval.
- Unity 6000.3.8f1 / Android compiled and saved/reopened Pinefall. Checked connected starter prefab in Fleet[0], zero price/owned state, legacy ID, unchanged loader prefab, unequal wheel radii, equipment compatibility, controller initialization, work/raise/angle commands and drive/park activation. Scene validation and all 19 snow checks passed. No player save was loaded or modified by these checks. Report: `Logs/StarterPlowValidation.txt`.
- The blade/lift hardware moves as one rigid module under the prototype controls. Faithful hydraulic linkage, motion clearance, live driving/snow interaction, Android packaging and Galaxy A07 performance remain untested. The scene replacement did not regenerate terrain, snow, scenery or the loader.

## Supplied Blender tractor intake - 27 October 2026

- Inspected `low_poly_snow_plower.blend` using Blender 5.2 with embedded script autoexecution disabled. Preserved a byte-identical original under `SourceAssets/LowPolySnowPlower`; source/copy SHA256 is `8F48568B0051F2379707F15809558090DE7128C23637BAD6E6A8774BA815B85C`. The embedded generator was not executed.
- Accounted for all 209 named vehicle meshes in a saved part map. Excluded the unrelated cube, studio floor, cameras and lights. Exported body/four wheels, plow/lift hardware and beacon as separate FBX modules. Rigid modules are combined while the original Blender parts remain editable.
- Unity 6000.3.8f1 with Android selected compiled the intake and imported metre-scale, Y-up/Z-forward meshes. Verified all four wheel pivots and fitted front/roof sockets. Prepared three Needs Review catalog entries, two unapproved fittings, a recipe and `GoldenTractor-REVIEW.prefab`. All geometry and materials are persisted through Editor authoring; no runtime construction or Pinefall/fleet replacement was added.
- Model-specific structural checks passed: prefab save/reopen, persistent meshes, wheel bindings, front/roof compatibility and refusal to bake unapproved assets. Source render and the combined Unity prefab were visually inspected at rest. Original toon materials are approximated with a URP palette; this does not establish shading fidelity in gameplay.
- Reran all 39 Asset Workshop acceptance checks, saved Pinefall validation and all 19 snow checks successfully. Reports: `Logs/PlowerIntake/UnityIntake.txt`, `Logs/AssetWorkshopChecks.txt`, `Logs/SceneValidation.txt`. Saved Pinefall and Generated assets have no changes.
- Remaining: author/review lift linkage and yaw behavior, full movement/clipping clearance, physical mass/COM/suspension tuning, driving and snow interaction, Android build and Galaxy A07 profiling. The source has unequal front/rear wheels; the uniform wheel-set socket is omitted. The review preview has no MachineController and is not a drivable machine. Total 7,274 triangles / 7 renderers / 30 material slots; phone performance is unmeasured. See `SourceAssets/LowPolySnowPlower/README.md`.

## Asset Workshop phase - 26 October 2026

- Added Editor-only model preparation, source-only prompt/image/script drafts, a categorized prefab catalog, explicit review/Ready publication, fitting profiles, modular vehicle assembly and a Ready-only JSON manifest/scene-plan workflow. Nine prototype catalog entries are saved as separate prefab copies and remain Needs Review.
- Preparation preserves imported sources, proposes collision and four-wheel rigs, records provenance and metrics, normalizes roots, and copies Standard/legacy materials to URP Simple Lit. Fused wheels, absent loader pivots and missing equipment anchors block publication. Stable IDs are read-only in the custom entry inspector.
- Approval fingerprints cover prefab dependencies, physics and constraints; fitting fingerprints additionally cover socket and mount transforms. Assembly baking supports blades, buckets, wheel sets and accessory mass/modifiers, retains authored rig rest transforms, and returns registered outputs to review after rebaking.
- Scene plans resolve approved IDs and road connections before authoring. Validation rejects invalid transforms/bounds/scales, overlaps, unknown/unapproved/stale entries and missing/cyclic road targets. New scenes are persisted in Edit mode; Pinefall and existing scene files cannot be overwritten by this workflow. Generated layouts do not automatically add contracts, snow coverage or machines to the current session fleet.
- Unity 6000.3.8f1 / Android compiled all runtime/Editor scripts and passed **39 actual Editor acceptance checks**, including source preservation, draft completion, material conversion, incompatible/overweight parts, changed approvals, plow/loader baking, wheel physics, catalog identity updates, connected roads, scene save/reopen and fixture cleanup. Pinefall scene validation and all 19 snow checks also passed. See `Logs/AssetWorkshopChecks.txt`.
- Verified `Assets/Scenes/Pinefall.unity` and `Assets/Snowglow/Generated` have no changes from this phase. Existing vehicle defaults retain grip 1 and the original dry centre of mass; new rig behavior is opt-in.
- The workshop window's visual layout/drag interaction, imported production models, manual clipping/movement review, live driving of newly assembled machines and sustained Galaxy A07 performance have not been tested. No new APK was built. Current player shop/ownership/save UI is unchanged; broader catalog-driven garage integration is a later step.
- Full instructions and scene-plan schema example are in `ASSET_WORKSHOP.md` and `Assets/Snowglow/Catalog/Examples/DistrictLayout.json`.

## Git-friendly snow asset split - 26 October 2026

- Split `Snow50cm_20261026_085523_745.asset` (170,024,383 bytes) into its existing seed asset plus 32 native mesh-part assets. Kept the seed GUID, scenery, vehicles and authoritative snow data; rewired saved visual/collision mesh references in Pinefall.
- Both snow rebuild and full district bake now group eight chunks per mesh file, enforcing a 90,000,000 byte per-file safety budget. Earlier unrelated generated assets are unchanged.
- Unity 6000.3.8f1 / Android saved and reopened the scene, passed reference/rig/asset validation and all 19 ledger checks, and confirmed the snow volume was unchanged. See `Logs/SnowAssetSplit.txt` and `Logs/SceneValidation.txt`. A local copy of the original asset and scene is kept in ignored `Logs/SnowAssetSplitBackup/`.

## Cohesive pile spreading - 26 October 2026

- Replaced centre-first stacking with lower-cell deposition weighted by distance, so piles grow outward as well as upward. Added bounded eight-neighbor slumping with a tuned slope of about 32 degrees, 3 cm cohesion allowance and gradual transfer rate. The 50 cm grid and current visual representation are unchanged.
- Existing and newly modified piles settle during gameplay. Pause freezes settling; blocked cells and field edges prevent invalid transfers; passive movement cannot generate work credit. Per-frame settling and mesh work remain budgeted.
- All 19 actual C# ledger checks passed under Mono (Unity logging/editor wrappers stubbed), then passed in Unity 6000.3.8f1 with Android selected after successful compilation. Seven new cases cover mound footprint, pause, downhill movement, volume conservation, resting slope, credit, obstacles and field edges. See `Logs/PileChecks.txt`.
- No rendered gameplay, handling or sustained Galaxy A07 performance measurement was performed for this change.

## Half-size snow cells - 26 October 2026

- The current checked-out source and scene used the original 1 m cells. Reduced width/length to 50 cm: four times the horizontal resolution over the same 256 m working field. Kept the existing combined-column representation.
- Updated cell-area volume accounting, coordinate conversion, working footprint, disposal bounds, scene validation and legacy save/seed migration. The snow-only editor rebuild retains vehicles, scenery and previous snow assets.
- Unity 6000.3.8f1 / Android compiled, rebuilt the saved snow, reopened Pinefall and passed scene validation plus all 12 ledger checks. The same checks also passed independently under Mono. `Logs/SceneValidation.txt` records the latest result.
- Visual gameplay and sustained Galaxy A07 performance at this resolution remain untested.

## Unity verification - 22 October 2026

- Read README.md, ARCHITECTURE.md and this checklist before changes.
- Used installed Unity 6000.3.8f1 with the project targeting Android; URP 17.3.0, Input System 1.18.0 and uGUI 2.0.0 imported.
- Fixed package resolution: removed the nonexistent `com.unity.modules.textrendering` dependency and enabled `com.unity.modules.vehicles`, which supplies WheelCollider. Both runtime and editor assemblies compiled in Unity after the fix.
- Executed the Edit-mode authoring workflow. Unity logged authoring completion and saved the populated `Assets/Scenes/Pinefall.unity`, generated assets and connected machinery prefabs under `Assets/Snowglow/Generated/Bake_20261022_111754_769/`.
- Added `Snowglow > Validate saved district and snow ledger`. It reopens the scene and checks missing scripts, persisted rigs/definitions, compatibility, loader anchors, 256 snow chunks and their saved meshes/colliders, terrain, materials, URP and build-scene inclusion. Results go to `Logs/SceneValidation.txt`.
- Scene save failures now stop the bake; the ready marker is written only after asset/rig validation. Automatic first-import baking is disabled in batch mode.
- Headless Unity compilation and saved-scene reopening passed in an isolated project copy with the Android target. All scene validation checks and all nine snow ledger invariants passed. See `Logs/UnityValidation.log` and `Logs/SceneValidation.txt`. The copy preserved the open editor session and original authored content.
- A subsequent fresh-bake recheck was interrupted during compilation; `Logs/UnityBakeValidation.log` does not establish a completed second bake.
- The temporary `.validation-project` copy was removed at the user's request. Keep one Unity project at this root; use the Snowglow validation menu here for future checks. Validation logs remain under `Logs/`.

## Historical source checks (before Unity verification)

- Parsed all 24 C# source files with the tree-sitter C# grammar: no syntax-error nodes.
- Checked player-side code after excluding UNITY_EDITOR blocks: no `new GameObject`, primitive scene creation or Terrain factory calls. All authored scene construction is confined to Editor code.
- Checked the authoring source declares a 1000 × 1000 m Terrain, persistent terrain/material/definition/mesh assets, connected machinery prefabs and an explicit scene save.
- Checked URP/Input System/uGUI and Terrain/physics/text/audio module declarations.
- Verified all 10 synthesized WAV files are readable mono 16-bit PCM.
- Geometric checks: depot-access, parking and main-street contract footprints contain 248, 744 and 560 cells respectively. Road-based objectives lie within the intended road geometry.
- Checked ZIP integrity after packaging.

These checks do not establish Unity compilation or runtime correctness.

## Repeatable mechanics checks

`Snowglow → Run snow ledger checks` exercises the actual engine-independent C# volume model in Edit mode. Nine invariants cover pushing, source reward credit, recycled snow, carrying, dumping, rejected out-of-bounds/full destinations, randomized transfers and blocked cells. This command requires the scripts to compile in Unity first.

## Required first-import/editor checks

1. Open in Unity 6.3 LTS and resolve package import. No script/compiler errors.
2. Execute the Edit-mode bake if the first-import attempt does not run. No authoring exceptions, missing material resources or invalid mesh/collider errors.
3. With Play stopped, inspect the concrete Terrain, colored scenery, truck and loader, UI, snow and generated asset folder. Save, close and reopen: all remain present without rebuilding.
4. Run the ledger checks. Inspect attachment mount IDs, dry/loaded mass and width compatibility ratings.
5. Enter Play. Inspect camera framing, world labels, HUD scaling and touch controls.
6. Drive and brake on asphalt, snow and the ice strip; test slope/curb behavior and assistance on/off. Tune suspension, grip and torque using the Galaxy A07.
7. Push snow with the blade down, angle it both directions, then raise it. Observe pile obstruction, traction loss and load resistance. Check no snow enters solid structures.
8. Use the loader intake, lift, carry, curl/tilt and dump. Verify loaded mass/center of mass, finite capacity and conservation through partial/rejected dumping.
9. Switch vehicles only when stopped at the depot. Worksite, piles, loads, fuel and condition must remain.
10. Complete/abandon/restart jobs; confirm late pay reduction, original-work-only credit and no repeated completion grant. Test simulated success/cancel/failure/duplicate purchase delivery.
11. Save during a job with a loaded bucket and displaced snow. Restart: state restores consistently. Check backup recovery after an interrupted write.
12. Build Android, test all multitouch combinations and safe-area layouts, then run at least 15 minutes on the Galaxy A07. Measure CPU/GPU/physics, memory, heat, loading and save hitches. 30 FPS is a target, not a measured result.

## Known prototype limits

- Unity and Android Build Support are installed, and the district has been baked. Android player compilation, APK packaging and connected-phone validation have not been performed.
- No rendered screenshot, Unity playtest, sound playback test or APK has been produced. Computer-use capture could not inspect the editor because it reported `no monitor found for window`; headless checks do not verify appearance.
- Handling, snow forces, collider cooking and mobile draw cost require tuning in Unity. The source is a mechanics prototype, not a production-ready release.
- Approximately 250 m square has interactive snow; the full 1 km Terrain supplies the surrounding landscape. Terrain itself is not deformed by plowing.
- Snow is a depth/volume grid with blocky combined-cube visuals and substantial-pile collision proxies. No granular flow, compaction simulation, melt, active accumulation or snowblower behavior.
- Loader boom/bucket are kinematic articulated visuals coupled to load/volume mechanics, not simulated hydraulic joints. Only the two supplied equipment fits are authored.
- Low/standard quality controls affect rendering. Galaxy A07 performance has not been established.
- Three contract footprints, one district, two cameras, basic XP/levels, tyre/engine upgrades and local saving are implemented in source. Regional unlock maps, alternative attachment purchasing, tracked locomotion, advanced challenge rules, monetization SDKs and cloud services remain future work.

## Residential block expansion - 30 October 2026

Expanded Pinefall from 16 to **40 houses** in ten saved Editor-authored groups:
four central blocks of six, two northern blocks of four and four outer groups of
two. All seven imported variants are used (Red 6, Green 7, Garage 6, White 5,
Storybook 5, Twin A Frame 6, Brick Barn 5). Existing 16 home prefab-instance
records remain byte-identical; their IDs, variants, enlarged scales and poses are
preserved. Added 24 connected prefab lots with matching foundations and fences.

`Editor/TownBlockAuthoring.cs` exposes **Snowglow > Environment > Expand town into
2, 4 and 6 house blocks**. Repeating it retains 40 homes without duplicate IDs or
cumulative scaling. Future full authoring invokes the same Editor placement helper;
no full district bake was performed for this change and no runtime factory added.

Unity 6000.3.8f1 / Android compiled, saved/reopened the scene and passed native
scene checks and all 19 snow-ledger checks. Repeating the actual menu command also
passed, adding zero new obstacle cells. World-space footprint checks verify at
least 1 m between houses and neighboring houses/roads. An initial checker bug mixed
local house bounds with world road bounds; corrected before the scene was saved.
The expanded layout was inspected from above in Scene view.

The scene references `Catalog/SceneData/TownBlocksSnowSeed.asset`, a 5.23 MB copy
of the original seed with 6,188 additional conservative house-proxy obstacle cells.
All depth, credit and volume-accounting values remain identical. The original
seed/GUID, saved snow meshes, Terrain, mountains, fleet and player saves are
preserved. No snow geometry was rebuilt. Saved original house prefab-instance
records were independently compared with the pre-edit scene and are identical.

House meshes total **72,552 triangles and 160 renderers**, before foundations,
fences and shadow passes. This expansion needs new phone profiling; counts do not
prove frame times. Full vehicle/yard clearance walkthrough, Android appearance/
packaging and sustained Galaxy A07 performance remain untested. See native report
`Logs/TownBlocks/Validation.txt` and backup `Logs/TownBlocks/Pinefall-before.unity`.


## Twelve alpine street lights - 30 October 2026

Prepared all twelve collections from the user's ChatGPT-generated
`Alpine_Street_Lights_12_Variants.blend`. Preserved source/copy SHA256
`50F565BAE9ECF8CEA58338E271A939C914EAB6C6FD2FBCD6567F6356286EC88D`.
The embedded START HERE text was treated as data and not executed. Inspected all
source renders, exported copies with Blender autoexecution disabled and retained
complete part/light/presentation maps under `Catalog/Sources/AlpineStreetLights`.

Stable catalog IDs `alpine-street-light-01` through `12` are Needs Review. Each
prefab has separate Frame, opaque unlit Luminous and removable SnowCap meshes,
sharing three materials and one point-filtered palette across the set. Original
pole/base mounting origins and a facade connection anchor for Wall Lantern are
preserved. Narrow pole/base boxes avoid invisible full-width barriers below arms;
these are proposed proxies, not exact collision meshes. All 274 closed mesh
components passed winding checks without flips. Visible Unity geometry matches
export metrics after stripping recorded zero-area LED triangles.

Replaced all nine prototype poles at their original horizontal positions with
nine pole variants. Added two depot bollards, two neighborhood globes and a wall
lantern seated on the workshop facade. All twelve styles are used across fourteen
saved prefab instances (`pinefall-light-01` to `14`). Optional light counts/positions/
colors and Blender watts are recorded; 21 shadowless point-light components exist
under inactive `Optional lights | enable in Editor (mobile default off)` groups.
Opaque luminous meshes remain visible without real-time local lights. Intensity/
range values are URP proposals, not numerical conversions from Blender watts.

Unity 6000.3.8f1 / Android compiled, prepared all twelve prefabs, saved/reopened
Pinefall and passed scene validation plus all 19 snow-ledger checks. All twelve
native prefab previews were rendered and inspected; one placed pole was inspected
in Scene view. Repeated native intake preserves all fourteen IDs/transforms and
changes zero obstacle cells. Clearance checks synchronize physics transforms before
checking pole proxies against roads and house colliders; the wall anchor is checked
against the facade. Reopening the scene unloads unused catalog ScriptableObjects,
so intake reacquires catalog handles before final checks/previews.

The active snow seed is now `Catalog/SceneData/StreetLightsSnowSeed.asset` (5.23 MB),
a separate copy of the town seed. Refreshing static obstacle flags added 16,341
and removed 1,161 obsolete cells (17,502 changed); this incorporates current house
foundations and the replacement poles. Every depth/credit/accounting field remains
identical. Original seed assets/GUIDs, saved snow meshes, Terrain and player saves
were preserved. All 40 home and 12 mountain prefab-instance records are independently
byte-identical to the pre-intake scene. Fleet bindings/poses and all existing
placement poses are preserved by native snapshots. Town-block authoring now extends
a newer scene-specific mask instead of reverting it to the earlier town-only seed;
that inter-tool repeat was verified natively with zero changes.

Menu: **Snowglow > Asset intake > Import twelve alpine street lights**. Future full
baking uses the prepared placement helper; no district/snow geometry rebake or
runtime fixture construction was performed. Native report and previews are under
`Logs/AlpineStreetLights`; pre-intake backup is `Pinefall-before.unity`. Source intake
notes and controls are in `SourceAssets/AlpineStreetLights/README.md`.

Fourteen fixtures total **8,804 triangles / 42 renderers and material slots**, with
three shared materials. Per-variant geometry is 228-1,290 triangles; Twin/Triple
heritage fixtures exceed the small-prop 1,000-triangle working target slightly,
with multiple lantern heads. No decimation was needed for this intake. Counts do
not establish frame times. Full driving/collision walkthrough, optional night-light
appearance, Android packaging/rendering and Galaxy A07 performance remain untested.

## Twenty-six alpine tree variants - 30 October 2026

Replaced all **97 primitive tree roots** in Pinefall with prepared models from
`Alpine_Trees_26_Variants.blend`. Original preserved under SourceAssets/AlpineTrees,
SHA256 `C8FA426AEAB4D38B8FFBCD378212D1376E139CA831F8437676D6F3720FBFE06F`.
Embedded START HERE text was treated as data and never executed. All 26 numbered
collections/source previews were inspected: eight live forms, eight snowy forms,
four bare/dead forms, four stumps and two fallen logs. All 700 closed mesh components
passed winding checks without reversals; open surfaces retain their direction.
No source overwrite, decimation or transparent foliage cards.

Scripts `Tools/AssetIntake/inspect_alpine_trees.py` and `export_alpine_trees.py`
produce the source inventory, 26 FBXs, point-filtered 16x1 palette, full part map,
metrics and source renders with Blender autoexecution disabled. Stable catalog
IDs `alpine-tree-01` through `26` remain Needs Review. Each uses one opaque TreeWood
mesh/material plus an independently removable SnowCap where supplied (09-16, 24,
26); two shared materials across the set. Source Tree or Wood was already a joined
rigid assembly, which is retained. Snow-free source variants stay snow-free.

`Editor/AlpineTreesIntake.cs` authors a deterministic mix/yaw (seed 301026), using
every variant and weighting remaining slots toward winter evergreens. Six forest
floor variants each occur once. Original X/Z positions and uniform scales are
preserved, with 8 cm buried foot probes; stumps/logs align with local Terrain normal.
Native read-only vertex checks verify sufficient relief above terrain without
changing shared meshes. Narrow proposed trunk capsules avoid canopy-wide invisible
barriers; stumps/logs use compact box proxies. Existing 230 m DistanceVisibility is
retained. No wind, runtime assembly, interactive tree cutting, new snow simulation
or plowable decorative snow was introduced.

Unity 6000.3.8f1 / Android compiled, prepared all 26 prefabs, saved/reopened Pinefall
and passed scene validation plus all 19 snow-ledger checks. All 26 native prefab
previews were rendered and inspected, and a placed Snowy Tower Spruce was inspected
in Scene view. Preview cameras fit the full projected bounds (including wide stump
roots/log ends). Repeated actual menu intake passed with 97 unique placement IDs,
unchanged saved mix/poses/scales and zero obstacle changes. After scene reopening,
unused catalog asset handles are reacquired before native prefab checks/previews.
Failed unsaved placement validation restores the original saved scene, avoiding a
partial replacement pass.

The new forest totals **96,243 mesh triangles / 145 renderer-material slots**, down
from **454,736 / 679** for the original primitive forest inventory (about 79% fewer
triangles/renderers). These are complete placement inventories, not visible-frame,
shadow-pass, batching or FPS measurements. Per-variant counts are 220-2,633 triangles;
Open Alpine Larch (1,966) and Snowy Open Alpine Larch (2,633) are the heaviest, with
two and eight placements respectively. Review their device cost before denser use.
No Android build or phone-performance claim follows from these counts.

All 66 existing home/mountain/light prefab-instance records are independently
byte-identical to the pre-intake scene. Fleet bindings, Terrain and saved snow mesh
assets are preserved by native snapshots. Nearby trunk checks changed **zero** snow
obstacle cells; the current street-light seed reference and data stay unchanged,
and no tree-specific seed asset was needed. No district/snow rebake or player-save
writes. Unrelated user edits to build-version settings were left untouched.

Menu: **Snowglow > Asset intake > Import twenty six alpine trees**. The future
full district bake calls PlacePreparedTrees when all 26 prepared entries exist;
that full bake was not run. Reports/previews and the pre-edit scene copy are under
`Logs/AlpineTrees` (`UnityIntake.txt`, `NN-source.png`, `NN-unity.png`,
`Pinefall-before.unity`). Source details: `SourceAssets/AlpineTrees/README.md`.
Full forest/collision walkthrough, runtime culling behavior, Android rendering/
packaging and sustained Galaxy A07 profiling remain untested in this task.


## Orbit and Scenic camera modes - 30 October 2026

Added two views alongside the existing Isometric and Follow modes. **C / CAM** now
cycles Isometric -> Follow -> Orbit -> Scenic. Orbit provides full 360-degree
inspection and front/rear access; Scenic provides a wider diagonal panorama with
slow automatic revolution. Native scenic-preview review led to a lower 18-degree,
45-degree-diagonal default and 65-degree FOV so town/mountains appear behind the
vehicle instead of the depot roof dominating the image. Orbit starts at 15 m / 25
degrees; Scenic at 40 m. Both allow 5-75 degree tilt, free yaw, and bounded zoom
(Orbit 7-42 m, Scenic 22-65 m). Obstructions may shorten actual camera distance.

PC: right-drag or J/L + I/K; wheel or +/- zoom; Home resets. Phone: drag an empty
view, pinch two world-view fingers, or use saved zoom/reset buttons. Fingers that
start on HUD/driving controls are excluded even after leaving the control. Existing
W/S/arrows/steering remain vehicle-relative. Scenic turns at four degrees/second;
manual input holds automatic movement for six seconds, and menus/pause freeze it.
Machinery switching recentres the active view; camera preferences are session-only,
with no save-schema or player-save writes.

CameraRig still moves the one authored gameplay Camera. Terrain height and all four
near-plane corners are constrained, including after smoothing; nonallocating sphere
sweeps against world/pile colliders pull the camera inward, ignoring the active
vehicle. No new runtime camera, UI, environment or machinery construction. Input
System 1.18's normalized wheel range is respected, including Windows platform-range
fallback. Legacy serialized Follow settings migrate through FormerlySerializedAs.

CameraModesAuthoring saves the terrain binding, mode label, help and reset/zoom
buttons into existing Pinefall. Future full district/HUD authoring includes these
bindings; no district/snow rebake was run. Menu: **Snowglow > Install orbit and scenic
cameras and validate**. CameraModesChecks uses disposable native fixtures, synthetic
keyboard/mouse/touch input, a rendered UI fixture and an actual obstruction. Its
Input System player-update test hook and manually simulated Editor UI lifecycles
are restored/cleaned up; no project Input Settings or real input devices are changed.
Initial Editor-only edge-event/UI-registration fixture issues were corrected rather
than altering gameplay UI exclusion to accommodate an uninitialized fixture.

Unity 6000.3.8f1 / Android compiled, saved/reopened the Camera/HUD and passed
**10,827 assertions including 2,160 terrain/near-plane poses**. Cases cover all four
modes, flat/sloping terrain, 4:3 and 20:9 aspect ratios, 5/45/75-degree poses, multiple
yaw angles and every intermediate smoothing step sampled; keyboard C/W/S/A, mouse
orbit/wheel, touch drag/pinch and HUD exclusions, zoom/pitch clamps, Scenic auto/
pause/manual-hold/reset, obstruction pull-in and front-of-vehicle reachability.
Scene validation and all 19 snow-ledger checks passed. Native Orbit front/rear and
Scenic-wide scene renders were inspected under `Logs/CameraModes`. The scenic
render visibly includes vehicle, housing, trees and mountain backdrop.

All existing environment catalog IDs/poses and Terrain/snow assets were protected
by native snapshots. An independent saved-scene record comparison confirms all 163
home/mountain/light/tree prefab instances unchanged. Only the Camera/HUD authoring
and runtime camera/input presentation were changed; vehicle/snow physics and player
saves were untouched. A pre-edit scene backup is `Logs/CameraModes/Pinefall-before.unity`.
Controls/limits are documented in CAMERA_MODES.md; report is
`Logs/CameraModes/Validation.txt`. These are Editor fixture/render results, not a
continuous physical Game-view or Android playtest. Live steering/touch ergonomics,
every possible obstacle/pile arrangement, Android packaging and Galaxy A07 frame
times remain untested. Wide views can increase visible geometry/shadow work.


## Even residential spacing - 31 October 2026

Redistributed the existing 40 complete house lots to fill the western gaps shown
in the user's marked screenshot. Central pairs now span X -46/-22 and 22/46;
western lots form pairs at X -102/-82 across Z -44, -20, 20 and 44.
Northern pairs sit at Z 82/102. House IDs, variants, scales, foundations and fences
are retained; this explicitly supersedes the previous requirement to keep the
original 16 house positions. No homes were added.

Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall and passed house/road
and house/house footprint checks with a 1 m margin, scene validation and all 19
snow-ledger checks. EvenTownSnowSeed.asset is a separate scene-specific seed:
30,706 obstacle flags changed, including cleared former footprints; depth,
credit and volume accounting were unchanged. Existing seeds, snow meshes,
Terrain, mountains, fleet and player saves were preserved. No district rebake.
Menu: Snowglow > Environment > Redistribute homes evenly. Native report:
Logs/TownBlocks/Validation.txt. Live driving, rendered appearance and phone
performance were not tested for this adjustment.


## Vehicle tyre impressions - 31 October 2026

Added an Editor-authored SnowTyreTracks renderer to Pinefall, bound to both
machines and all eight WheelColliders, with widths fitted from the wheel visuals.
Grounded moving wheels stamp pale cyan herringbone bars onto interactive snow;
steering and reverse travel retain wheel direction. Bare ground, blocked cells,
lost contact, stationary tyres and recovery discontinuities do not draw continuous
tracks. Marks are cosmetic shading, not physical ruts: no compaction, snow-volume
transfer, tyre-grip change, clearing credit or reward change.

A shared transparent unlit/fog-aware material and mutable mesh draw a bounded
2048-stamp ring (4096 quads / 8192 triangles maximum), refreshed at most 10 times
per active-play second. Marks fade over 100 active-play seconds, freeze with
pause and clear on worksite/grid reset; they are not persisted in player saves.
Cleared/reshaped snow invalidates marks, avoiding floating trails. Runtime creates
only the mutable mesh/data; renderer, component, material and bindings exist before
Play. PrototypeAuthoring includes the same Editor placement helper for future
full bakes; this task did not rebake the district or snow geometry.

Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall and passed scene
validation, 11 track fixtures and all 19 snow-ledger checks. Fixtures cover
stationary/moving tyres, unchanged accounting, teleports, contact gaps, reverse,
steered marks, capacity, cleared/blocked snow and expiry. The native daylight
synthetic-contact preview was rendered and inspected at Logs/TyreTracks/Preview.png.
Existing catalog placement IDs/poses and seed dependency hash remain unchanged.
Menu: Snowglow > Install snow tyre impressions and validate. Native report and
pre-edit scene backup are under Logs/TyreTracks. Continuous physical driving,
live contact/turn appearance, Android rendering and A07 frame times remain untested.

Repeated native tyre-track installation passed with one renderer and no duplicate bindings. Independent scene-record comparison confirms all pre-existing object and prefab records unchanged; only five new track records and the root list were added.


## Compact driving information - 31 October 2026

At the user's request, the three driving readouts are now text only across the
top: vehicle status on the left, notice/equipment in the centre, contract/wallet
on the right. Removed the Top HUD and Equipment status background Images;
readouts use 14 px design-space text, with best-fit down to 12 px and a small
white glyph outline for contrast. They do not intercept input. Driving buttons
and menu layouts are retained. This is a temporary readability/scenery change
before the later production UI redesign.

CompactHUDAuthoring applies the same layout to the existing saved scene and
future HUDFactory builds. Unity 6000.3.8f1 compiled, saved/reopened Pinefall and
passed scene validation. The 1280x720 native UI preview was rendered and inspected;
report, preview and scene backup are under Logs/CompactHUD. No continuous
Game-view playtest or physical-phone readability test was performed.

## Gameplay camera revision - 31 October 2026

Scenic is removed from C/CAM cycling and reserved for a future static main-menu preview. Isometric now closes at low speed and widens smoothly across 0-8 m/s (11-18 m orthographic half-height). Follow smoothly arcs to the front for actual reversing, returns behind for forward travel and holds direction inside a +/-0.35 m/s dead band. Pause freezes speed framing and directional state. Orbit controls and steering semantics remain.

Unity 6000.3.8f1 / Android compiled, saved/reopened Pinefall and passed 11,132 assertions including 2,220 terrain/near-plane poses, plus all 19 snow-ledger checks. New cases cover three-mode cycling, legacy Scenic migration, speed zoom in both directions, slowing, pause, smooth reverse onset, front view, stop retention and return behind. Existing input, UI exclusion and obstruction checks passed. The native Isometric-close preview was inspected; Orbit front/rear renders were saved under Logs/CameraModes.

Authoring verified unchanged catalog IDs/poses and terrain/snow dependency hashes. Unrelated Android build/settings changes were retained. These are Editor fixture/render results; continuous physical driving, reverse-transition comfort, Android appearance and A07 performance remain untested. See CAMERA_MODES.md.


Isometric framing adjustment (31 October 2026): user requested about 2x closer. Halved the saved scene and default orthographic size range from 11-18 m to 5.5-9 m; speed zoom timing/range in speed units is unchanged. Updated the existing speed-zoom assertion threshold for the smaller range. Source/serialized-value inspection only for this tuning edit; no new Unity render or phone playtest.


Isometric framing adjustment (31 October): reduced the current 5.5/9 m near/far orthographic half-heights to 4.4/7.2 m (20% tighter framing, approximately 25% larger vehicle). Updated both saved Pinefall values and CameraRig defaults. Speed thresholds and smoothing are unchanged. Source/scene values checked; no Unity render or device test was run for this tuning-only change.

Isometric follow-up tuning (31 October): the user found 4.4/7.2 m too close. Increased saved scene and default orthographic half-heights to 5/8.2 m, between the last two settings. Speed response remains unchanged. Source/scene values checked; Unity rendering and device verification remain pending.

Isometric wider-framing revision (31 October): increased near/far orthographic half-heights from 5/8.2 m to 6.5/10.5 m after the user still found the view too close. Saved Pinefall values, defaults and camera documentation agree. Speed response unchanged. Unity visual/device verification remains pending.

Isometric further widening (31 October): user requested another step outward; near/far half-heights are now 8/13 m in the saved scene and defaults. Speed response unchanged; visual/device verification pending.

Isometric further widening (31 October): near/far half-heights increased to 10/16 m at user request. Saved scene, defaults and camera documentation updated; speed response unchanged. Visual/device verification pending.

Isometric additional outward step (31 October): near/far half-heights increased from 10/16 m to 12/19.2 m (20% wider framing) at user request. Saved scene, defaults and camera documentation agree; speed response unchanged. Visual/device verification pending.


## Simplified driving buttons - 1 November 2026

Saved/reopened Pinefall with separate LEFT/RIGHT hold buttons; removed Lift/Tilt
and Auto/Manual buttons and manual keyboard operation. Full loader buckets now
always auto-carry, including old saved Manual state. F/R is 150x64 with 50 design
units of clearance above Drive. RECOVER TO DEPOT has an explicit persistent
UICommand.Click binding to the existing recovery action. Its underlying method
was exercised on a disposable Rigidbody fixture, retaining the bucket load.

Driving HUD now stretches inside safeArea rather than the centred menu design
canvas. Left/right groups anchor at 5%/95%, with uniform size fitting for narrower
screens; equipment actions remain centred. No runtime UI construction was added.
Unity 6000.3.8f1 / Android compiled, saved/reopened the layout, passed scene checks,
steering hold/release/full-bucket/recovery fixtures and all 19 snow-ledger checks.
Native UI preview was rendered and inspected (Logs/DrivingButtons/Preview.png).

The updated DrivingControlsChecks also passed with the actual imported loader:
stationary full-load mouth elevation, partial work, higher-boom preservation,
legacy Manual normalization, ignored R/F/T/G keys, dump pose and normal driving/
UI keyboard routing. Its Editor EventSystem fixture needed explicit OnEnable/
OnDisable registration; the initial failure was a fixture setup issue. Reports:
Logs/DrivingButtons/Validation.txt and Logs/DrivingControls.txt. No player saves
were written by fixtures. Continuous Game-view touch/driving/recovery interaction,
physical-phone ergonomics and A07 performance remain untested.

Driving button spacing adjustment (1 November): increased LEFT/RIGHT separation from 16 to 36 design units; reduced F/R width from 150 to 118 to match Drive, retaining 64 height. Updated saved scene and Editor authoring values. Structural values checked; no new Unity render or phone test for this layout-only adjustment.

Orbit UI simplification (1 November): remove on-screen Zoom In/Out buttons and retain only VIEW RESET, anchored to the left controls group at the 5% safe-area inset. Mouse wheel, keyboard +/- and two-finger pinch remain supported. Future HUD/camera authoring follows this layout.

Orbit reset-only follow-up: saved scene structurally checked with zero camera:in/out commands and one camera:reset command, left-aligned within the 5% inset group. Code compiled; Unity assembly reload was still in progress at the saved-data check. Native refresh/visual verification remains pending. Backup: Logs/OrbitReset/Pinefall-before.unity.


## Photoshop UI kit intake - 1 November 2026

Imported the user-supplied Snow_District_UI_Buttons.psd through a scoped extension
of Asset Workshop: appended UI category and PhotoshopDocument provenance without
renumbering existing enum values. Original 22,701,282-byte PSD is preserved under
SourceAssets/SnowDistrictUI with SHA256
8e38b85f54187061130352c043f55401da74be31ba58157c4940282f14a9e9b9,
verified equal to the supplied file. Layer text/names/references were treated as
data; no embedded instructions/scripts/actions were executed.

Exported 32 independent button layers plus three Play state layers with alpha,
excluding header/background/labels/hidden references from runtime sprites.
Catalog/Sources/SnowDistrictUI holds transparent PNGs, a shared 2048x1024 atlas and
complete export manifest. Stable IDs snow-district-ui-01 through -32 have reusable
UI prefabs and sprite references; all remain Needs Review. UI bounds use canvas
pixels; validation checks persistent sprites/graphics and excludes world physics.
Ready world-layout manifests omit UI entries, and world scene plans reject them.
Existing 3D approval fingerprint format remains unchanged.

Skinned 65 matching existing buttons across driving and menus, retaining hit areas,
actions, side anchors, steering gap and F/R size. Labels stay separate. Source Play
Normal/Pressed/Disabled artwork is retained on the reusable prefab and mapped Play
actions; other controls use color feedback. Work artwork follows plow versus loader
selection via saved sprite bindings; no manual lift/tilt controls are restored.
View Reset uses Camera art with its explicit label. No zoom, Auto/Manual, Districts
or Challenge features were enabled by importing their reusable artwork.

Unity 6000.3.8f1 compiled, prepared/validated 32 entries and prefabs, saved/reopened
35 sprites and 65 UI bindings, and passed catalogue/scene checks. Native driving
preview rendered and inspected under Logs/SnowDistrictUI. Repeat intake retained
32 stable entries and 65 mapped buttons. World catalog IDs/poses and snow seed hash
were checked unchanged. No district/snow rebake or player-save writes. Source PSD
is outside Assets; runtime atlas raw RGBA8 size is 8 MiB before platform compression.
Physical touch/state interaction, phone legibility and sustained A07 performance
remain untested. Menu: Snowglow > Asset intake > Import Photoshop UI button kit.
Source notes: SourceAssets/SnowDistrictUI/README.md. Reports/backups/previews:
Logs/SnowDistrictUI.


## Abandon icon and icon-only driving controls - 1 November 2026

Added derived UI catalogue entry snow-district-ui-33 (Needs Review) and reusable
prefab for Abandon Job. Built-in imagegen edited the supplied Career clipboard:
checkmark replaced by an amber X, retaining the snowy petrol-teal/ivory style.
Original generated PNG and exact prompt are preserved under
SourceAssets/SnowDistrictUI/Derived; runtime import is capped at 256px.
The original PSD and its 32 source button assets remain unchanged.

Driving-button labels are hidden, not deleted; non-button information readouts
and menu labels remain. Abandon Job stays labelled in the pause menu. Driving
artwork is centred with a common 196:166 RectTransform aspect and preserveAspect,
without changing touch rectangles, spacing, actions or 5% anchors. Runtime camera
label text may still update its inactive Text component; it remains invisible.

Unity 6000.3.8f1 compiled, validated 33 UI entries, saved/reopened 66 matched button
bindings, checked hidden driving labels/common aspect ratio and visible Abandon
menu label/artwork. Scene/catalogue validation passed; world IDs/poses and snow
seed hash unchanged. Native icon-only driving preview rendered and inspected at
Logs/SnowDistrictUI/Driving-preview.png. No live touch, continuous gameplay or phone
performance claim. Existing menu/drive actions and snow mechanics unchanged.


## Depot recovery snap-back fix - 1 November 2026

RecoverToDepot previously moved only the Transform while toggling Rigidbody
kinematic state. With interpolation and autoSyncTransforms disabled, the physics
pose could retain the pre-recovery location and overwrite the visible teleport.
Recovery now temporarily disables interpolation, clears body momentum, assigns
Rigidbody.position/rotation and synchronizes the Transform before restoring the
active/parked state and interpolation. Wheel drive torque, steering and transient
motion readings reset; attachment sweep resets while bucket load remains intact.
Ordinary vehicle motion is unchanged and still comes from Rigidbody/WheelColliders.

Unity 6000.3.8f1 compiled and passed 496 assertions on the actual tractor/loader
clones in an isolated Editor preview physics scene: active/inactive recovery,
interpolation, autoSyncTransforms=false, load retention, 120 subsequent physics
steps per case and reactivation without stale momentum. Scene validation and all
19 snow-ledger checks passed. No scene assets or player saves were written by the
fixture. The fixture verifies its physics scene is separate from the default
world and restores global auto-sync state. Initial harness use of a Play-only
scene API was replaced with the correct Editor preview API before the passing run.
Menu: Snowglow > Run recovery physics checks. Report:
Logs/RecoveryPhysics/Validation.txt. Continuous live recovery-button driving and
Android/device validation remain untested.


## Gear-driven Follow camera - 2 November 2026

Follow now reads PrototypeInput.Reverse, the same selected gear used by the drive
controls. It starts changing sides on gear selection, without waiting for actual
backward/forward velocity. Reverse arc time advances at 0.25x the forward rate;
gear changes discard old angular momentum. Position tracking, look smoothing,
terrain/near-plane clearance and obstruction response retain normal responsiveness.
Isometric speed zoom still uses actual vehicle speed. No scene rebake or save
schema change was needed.

Unity 6000.3.8f1 compiled and passed 14,637 camera assertions covering 2,920
terrain/near-plane poses, keyboard and F/R gear selection at rest, opposite vehicle
motion, quarter-rate arc timing, pause, Orbit gestures/UI exclusion and obstruction
pull-in. Scene validation and all 19 snow-ledger checks passed. Report:
Logs/CameraModes/Validation.txt. Existing Pinefall was reopened after disposable
fixtures; no scene assets or player saves were written. Continuous live driving,
physical touch ergonomics, Android packaging and Galaxy A07 performance remain
untested.


### Gentler symmetric Follow arcs - 2 November 2026

User reported discomfort from rapid direction changes. Both gear-switch arcs now
run at half the previous reverse speed (0.125x the original forward rate), with
the same speed in both directions. Selected gear still starts the transition
immediately; old angular momentum is cleared when the gear changes. Vehicle
tracking and ground protection remain responsive. This supersedes the asymmetric
quarter-speed reverse tuning above.

Unity 6000.3.8f1 compiled and passed 18,238 camera assertions, including 3,620
terrain/near-plane poses, equal-duration arc symmetry, reduced arc progress and
100 rapid gear toggles. Scene validation and 19 snow-ledger checks passed.
Report: Logs/CameraModes/Validation.txt. No scene or player-save changes.
Live comfort/continuous driving and Galaxy A07 performance remain untested.


### Rear Follow with reverse overhead view - 2 November 2026

Supersedes both gear-switch arc revisions: user requested a static initial rear
Follow view, rising gently over the vehicle and looking down during backward
travel, returning to its original pose at a complete stop, and lowering while
moving forward. No front/rear yaw arc remains. Actual signed travel speed now
selects the posture; gear changes at rest leave it still. A +/-0.12 m/s drift
band counts as stopped. Defaults: resting 13 m rear / 6 m high, reversing 2 m
rear / 16 m high, forward 13 m rear / 4.5 m high; 2.5 s pose smoothing.
Ground/near-plane and obstruction protections remain active. No scene rebake or
player-save changes.

Unity 6000.3.8f1 compiled and passed 29,838 camera assertions including 5,920
terrain/near-plane poses: stationary gear selection, gentle reverse rise,
downward view, rear-side retention, forward lowering, stop restoration from
both directions, drift tolerance, pause and rapid travel reversals. Scene
validation and all 19 snow-ledger checks passed. Report:
Logs/CameraModes/Validation.txt. Live driving comfort, physical touch and
Galaxy A07 performance remain untested.


### Fixed Follow offset and tilt-only direction response - 2 November 2026

Supersedes the previous reverse overhead movement. Follow retains a fixed rear
offset and gently changes pitch only: stopped 45 degrees, reverse 60 (+15),
forward 30 (-15). Actual travel and the existing drift dead band select tilt;
2.5 s smoothing remains. Default rear distance is 13 m and height is 14.6 m,
centering the vehicle at rest. Vehicle position/heading tracking continues;
terrain/obstacle safety can still adjust placement. No direction-dependent
translation or yaw arc. No scene rebake or player-save changes.

Unity 6000.3.8f1 compilation and 29,941 camera assertions passed, including
5,920 terrain/near-plane poses, exact settled pitch checks, unchanged camera
position across travel states, gentle rapid reversals, pause and stop reset.
Scene validation and 19 snow-ledger checks passed. Report:
Logs/CameraModes/Validation.txt. Live comfort and phone performance untested.


Follow resting-angle adjustment: stopped tilt is now 35 degrees, retaining the
+/-15 degree response (reverse 50, forward 20). Fixed rear offset height now
matches 35-degree resting framing (~10.7 m above at 13 m rear distance).
Unity compilation and all 29,941 camera assertions passed with the updated
angles, including 5,920 ground/near-plane poses; scene/19 snow checks passed.
No scene or player-save writes. Live comfort and phone performance untested.


Follow angle tuning: stopped 40 degrees; reverse offset -10 gives 30 degrees;
forward offset +20 gives 60 degrees. Fixed offset height matches resting framing
(~12.5 m above at 13 m rear distance). Gentle tilt-only response retained.
Unity compilation and 29,941 camera assertions passed for 40/30/60 degrees,
including 5,920 terrain/near-plane poses; scene and 19 snow-ledger checks passed.
No scene/player-save writes. Live comfort and phone performance untested.


Latest Follow tuning: stopped 35 degrees; reverse +10 gives 45 degrees;
forward -20 gives 15 degrees. Fixed rear offset and gentle tilt-only response
remain, with resting height ~10.7 m at 13 m rear distance. Unity compilation
and 29,941 camera assertions passed for 35/45/15 degrees, including 5,920
terrain/near-plane poses; scene and 19 snow checks passed. No scene/save writes.
Live comfort and phone performance remain untested.


Latest Follow angles: stopped 30 degrees; reverse +20 gives 50 degrees;
forward -15 gives 15 degrees. Fixed rear offset height matches resting framing
(~9.1 m above at 13 m rear distance); gentle tilt-only transitions retained.
Unity compilation and 29,941 camera assertions passed for 30/50/15 degrees,
including 5,920 terrain/near-plane poses; scene and 19 snow checks passed.
No scene/player-save writes. Live comfort and phone performance untested.


Latest Follow angles: stopped 35 degrees; reverse +10 gives 45 degrees;
forward -25 gives 10 degrees. Fixed rear offset height matches resting framing
(~10.7 m above at 13 m rear distance); gentle tilt-only transitions retained.
Unity compilation and 29,941 camera assertions passed for 35/45/10 degrees,
including 5,920 terrain/near-plane poses; scene and 19 snow checks passed.
No scene/player-save writes. Live comfort and phone performance untested.


Current Follow tuning: stopped 35 degrees; reverse +10 gives 45 degrees;
forward -20 gives 15 degrees. Fixed rear offset and gentle transitions retained.
Unity compilation and 29,941 camera assertions passed for 35/45/15 degrees;
scene and 19 snow checks passed. No scene/player-save writes. Live comfort
and phone performance remain untested.


Speed-proportional Follow tilt: actual signed travel speed now scales tilt
continuously from stopped 35 degrees toward reverse 45 / forward 15. Full tilt
is reached at 4 m/s (CameraRig.followFullTiltSpeed), with limits clamped above
that speed; the 0.12 m/s drift dead band returns to 35 degrees. Existing 2.5 s
smoothing keeps acceleration, braking and stops gentle. Fixed offset retained.
Unity 6000.3.8f1 compilation and 43,947 assertions passed, including intermediate
speed angles, maximum clamps, smooth stop return, pause and 8,720 ground poses.
Scene validation and 19 snow checks passed. No scene/player-save writes.
Live comfort and Galaxy A07 performance remain untested.


## Central intersection invisible obstacle - 2 November 2026

User marked the X0/Z0 town intersection as blocked. Native collider audit found
all twelve alpine mountain box proxies at the origin instead of their visible
mountains. AlpineTerrainFitting had treated CatalogValidation.BoundsOf's
placement-local bounds as world bounds, then converted/scaled them again.
The fitting helper now converts fitted mesh-bound corners directly into each
box's local space and records prefab overrides; validation requires proxies to
cover their mountains outside the interactive worksite.

Editor-only IntersectionObstacleAuthoring repaired the twelve saved scene
proxies without refitting meshes or rebaking Pinefall. A new scene-specific
CentralIntersectionSnowSeed.asset clears 2,108 stale local obstacle flags;
all depth, credit, initial-volume/disposal/revision values and original seed
assets are preserved. SnowField already uses authored seed obstacle flags on
restore, so existing player saves need no rewrite. The scene was saved/reopened.
All catalog placement IDs/transforms, Terrain and mesh dependency hashes matched.
Unity also serialized the current CameraRig fields in place of its removed legacy
fields; current camera behavior was retained.

Unity 6000.3.8f1 compiled. Central overlap and four vehicle-sized directional
box sweeps passed after reopen; every mountain proxy covers its own mesh bounds.
Scene validation and all 19 snow-ledger checks passed. No player-save writes.
Live continuous driving, mountain perimeter traversal and Android/device testing
remain untested. Report and backup: Logs/IntersectionObstacle/Validation.txt and
Pinefall-before.unity. Menu: Snowglow > Environment > Repair central intersection
mountain proxies. Audit command is available alongside the repair.

## Angular cloud import and scatter - 3 November 2026

Blender 5.2.2 inspected/exported all eight supplied meshes with autoexecution off.
Source SHA256: 8AE32B6DE44F38348DB0D38BD745FA0C274D657D6E0E7B7D0A6D3318BEE681E9.
Unity 6000.3.8f1 / Android compiled AngularCloudsIntake, prepared eight Needs Review
catalog prefabs and saved/reopened 40 sky instances, five each. Native counts:
332 triangles and one renderer per variant, 13,280 triangles / 40 renderers total,
one shared opaque material. Unique IDs, persistent prefab instances, absent
cloud physics and disabled shadows verified. Existing non-cloud catalog poses
and all existing mesh/Terrain/snow hashes matched. Scene validation passed.
Four 1280x720 upward horizon renders produced; north/east visually inspected.
No rebake, runtime factory, weather, snow mutation or player-save writes. Live
all-camera comfort, Android packaging and A07 performance untested. Details:
SourceAssets/AngularClouds/README.md; report/previews/backup: Logs/AngularClouds.

## Additional roadside fixtures

Unity 6000.3.8f1 compiled RoadsideLightsAuthoring and saved/reopened 14 new fixture
prefab instances, 28 total. Added geometry 8,810 triangles / 42 renderers. Pole
proxies passed road/driveway/scenery clearance with optional point lights off;
enabled light count unchanged. Existing catalog IDs/transforms and original
mesh/Terrain/seed hashes matched. New seed copy changes 56 local obstacle flags;
every snow depth/credit/volume/accounting value retained. Scene validation and
19 snow ledger checks passed. Native 1600x900 overview rendered and inspected,
with fog/distance limiting small fixture detail. Serialized before/after comparison
found no removed old records, 30 added records, and only SnowField seed reference
and SceneRoots changed among preexisting records. No district rebake or player-save
writes. Continuous driving, night illumination and A07 performance untested.
Report/backup/preview: Logs/RoadsideLights. Source details: SourceAssets/AlpineStreetLights/README.md.

## Reverse warning alarm - 4 November 2026

Unity 6000.3.8f1 compiled SynthAudio, PrototypeSession and ReverseAlarmAuthoring.
Imported original mono 22,050Hz PCM reverse-alarm.wav, 0.85s cycle. Native authoring
saved/reopened Pinefall and verified persistent looping 2D source, playOnAwake off,
mono duration, mute/unmute and scene validation. Static wiring uses shared reverse
gear state for either active machine only in running gameplay; menus stop the
source immediately. Generator/WAV checks confirmed a 0.36s tone followed by 0.49s
silence, zero-valued loop boundaries and no PCM clipping. Existing catalog poses
and snow seed dependency hash matched; no rebake or player-save writes. Audible
Play-mode, forward/reverse driving transitions and phone-speaker acceptance remain
untested. Report/backup: Logs/ReverseAlarm; source notes: SourceAssets/ReverseAlarm.

## Lowest tree and wood LOD replacement - 4 November 2026

Blender 5.2.2 inspected all four LOD sets with autoexecution disabled and verified
LOD3 is the minimum triangle count for every variant. Preserved source SHA256:
895E555B6AE1371E639B207FFD301E5BD8B8D936E99C2FBFD0D4D3FB5425F94D.
Only the 26 LOD3 FBXs were exported/imported; material colors matched the existing
palette, mounting basis/floor retained and source/presentation objects excluded.
Unity 6000.3.8f1 compiled LowestTreeLODIntake, updated existing prefabs and saved/
reopened Pinefall. All 97 forest instances use only new LOD3 meshes, no old forest
mesh remains elsewhere in the scene, and no LODGroups were introduced. Native
counts: 96,243 -> 40,939 triangles, 145 renderers unchanged. Existing catalog IDs,
prefab GUIDs, complete placement matrices, collision proxy shapes and culling
settings retained; Terrain/snow seed/mesh hashes matched. Scene validation passed.
Six native prefab previews and a 1280x720 forest view rendered; snowy tree, log and
forest views visually inspected. No district rebake, snow-mask or player-save
writes. Native import/save evidence does not establish live collision traversal,
Android rendering or A07 frame-time gains. Report, original prefab/scene backups
and previews: Logs/AlpineTreesLowestLOD. Durable details and metrics:
SourceAssets/AlpineTreesLowestLOD/README.md. Entries remain Needs Review.

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

## Alpine sign-board intake - 4 November 2026

User supplied `Alpine_Mobile_Sign_Boards_12_Variants.blend` and authorized pipeline
preparation, replacement of primitive signs, more placements and every variant
used at least once. Source is preserved byte-identically under
SourceAssets/AlpineSignBoards (SHA256 A533FFDEB2781572932C7FAC07F0FC6312B600D50133BD23607DAF399DD22A12).
Embedded READ ME is source data only; Blender ran with autoexecution disabled.

SignBoardsIntake uses CatalogPreparation to create 12 Needs Review scenery entries
`alpine-sign-01` through `12` with stable prefab/entry GUIDs. Evaluated supplied
FONT lettering remains static mesh art; decorative SnowCap is separate. One
opaque palette material and existing toon snow are shared. No new lights or
runtime spawning. Menu: Snowglow > Asset intake > Import twelve alpine sign boards.

Pinefall has 16 connected Editor-authored sign instances: all four primitive signs
replaced, plus 12 additions using every variant. Replacement destination labels
are retained on blank boards with saved TextMesh children. Variant 06 mounts on
the workshop facade; freestanding signs clear roads and seat to Terrain. Scene
geometry is 6,117 triangles plus five destination labels; each variant is 200-1,037
triangles including supplied mesh lettering and snow. Conservative envelope
colliders remain subject to driving/handling review. Entries are not Ready.

Unity 6000.3.8f1 / Android compiled and saved/reopened Pinefall; scene and 19 snow
ledger checks passed. Non-sign catalog poses, fleet and protected Terrain/snow
hashes matched. A separate SignBoardsSnowSeed copy changes 171 local obstacle
flags and preserves all depth/credit/volume. No district rebake or player-save
writes. Native reports/backups/previews: Logs/AlpineSignBoards. Continuous driving,
touch readability, Android package and Galaxy A07 costs remain untested.

Sign intake handoff: repeat native intake retained all 16 placements, existing
catalog IDs/poses and seed (zero additional mask changes). All 12 front-face
prefab renders and four in-scene renders were inspected. These revealed retained
TextMesh destination labels wider than the smaller imported boards. FitLabels
now measures and fits those five labels to the board face and validates their
bounds. This final source change is queued via Logs/ImportSignBoards.request,
but has not yet compiled/run: Unity needs focus and Ctrl+R. Both computer-control
connections failed (timeout / native pipe unavailable), so no successful final
label-fit render or compile is claimed. The import/layout itself is saved and
passed the preceding native checks. Run the pending pass and inspect updated
Logs/AlpineSignBoards/scene-*.png before calling label sizing verified.

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

Parked vehicle completion (5 November): Unity / Android saved/reopened five
static replacements and passed scene/19 snow checks; all eight source variant
previews and parking row inspected. 13,344 placed triangles; 80 obstacle flags
updated, quantities/poses/fleet retained. Logs/SnowTownVehicles/Validation.txt.
Driving, Android packaging and A07 costs untested.
