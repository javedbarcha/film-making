# Snowglow — editable primitive district prototype

Unity 6.3 LTS / URP / Android / offline. Reference device: Samsung Galaxy A07. Target: 30 FPS; device performance is not yet measured.

Latest [isometric-facing scene arrangement](ISOMETRIC_LAYOUT.md): home entrances
and decorated sides face west/south; the later manual garage faces west and disposal entrance north,
destination boards favour isometric, and twelve warning boards have matching rear
panels. Camera behaviour is unchanged. Current saved layout retains 30 signs;
this supersedes the older 31-sign arrangement described below.

The active fleet now uses the imported Golden tractor (free starter) and Golden snow loader (1,400 coins). Both are saved Editor-authored prefabs. See [mobile asset budgets](MOBILE_ASSET_BUDGET.md) for measured geometry and future intake thresholds.

Snow now uses pure-white tops with pale cyan toon shading, following the supplied reference. See [snow material authoring](SNOW_MATERIAL.md) for the palette, Editor command and validation limits.

## Asset preparation and reusable catalogs

Open **Snowglow > Asset Workshop** for model intake, draft/review/Ready catalog entries, drag-and-drop vehicle fittings, equipped prefab baking and validated AI scene plans. The supplied prototype building blocks are separate catalog copies and require review before publication. See [ASSET_WORKSHOP.md](ASSET_WORKSHOP.md) for the complete workflow and current boundaries. Pinefall's gameplay and player inventory are not replaced by catalog authoring.

All [seven winter homes](SourceAssets/SevenWinterHomes/README.md) are saved as reusable catalog prefabs and placed across 40 Pinefall house lots in ten blocks: four groups of 6, two of 4 and four of 2. All seven variants are used. The original 16 homes retain their identities and transforms. Homes are 1.5x the previous lot-fitted size; foundation footprints expand horizontally to match. Roof/ground snow stays separately removable in the Editor and warm windows are opaque. **Snowglow > Asset intake > Import seven winter homes** installs the set without a district rebake and preserves existing placements when repeated. Unity compilation, scene save/reopen, structural checks and 19 snow-ledger checks passed; Android performance and continuous vehicle clearance remain untested. Entries remain Needs Review. **Snowglow > Environment > Expand town into 2, 4 and 6 house blocks** repeats the saved 40-home layout without duplicates. The expanded scene uses a separate snow-seed copy with updated house obstacles and unchanged snow volume; original seed and snow meshes are retained.

All 12 former primitive mountains now use a shuffled mix of the [five imported alpine variants](SourceAssets/FiveAlpineMountains/README.md), with every variant used at least twice. They are saved Editor-authored catalog instances with separate snow caps; original horizontal locations are preserved. Editor-authored mesh copies follow the local terrain with shallow buried bases, keeping the mountains above ground. Use **Snowglow > Asset intake > Import five alpine mountains** for repeat-safe installation.

All [12 alpine street-light variants](SourceAssets/AlpineStreetLights/README.md) are prepared as reusable catalog prefabs and used across 28 Pinefall fixtures, replacing the nine primitive poles and adding depot bollards, neighborhood globes and a workshop wall lantern. Frame, opaque luminous surfaces and removable snow caps are separate. Optional shadowless point lights are saved under an inactive **Optional lights | enable in Editor (mobile default off)** group. Use **Snowglow > Asset intake > Import twelve alpine street lights** for repeat-safe installation. Unity save/reopen, scene checks and 19 snow checks passed; night lighting and device performance remain untested.

The forest now contains **146 tree/wood instances** (97 + 49), using only the
lowest supplied **LOD3** from the [four-LOD alpine pack](SourceAssets/AlpineTreesLowestLOD/README.md).
All 26 variants remain represented. Terrain seating uses a slight tree-trunk tilt (up to 12 degrees) and fully
slope-aligned wood. All forest bounds stay within 470 m of town, on the near side
of mountain envelopes with 3 m clearance. Following the marked overhead reference,
73 pieces extend into the outer band beyond 340 m; the farthest root is 467.9 m away.
111 trees and all six wood pieces occupy slopes of 5 degrees or more.
Forest geometry is **62,496 triangles / 219 renderers**. Removable snow,
collision proxies and 230 m distance culling remain; no LODGroup switching.
**Snowglow > Environment > Expand forest by half and repair all terrain seating**
repeats the repair without adding duplicates. Unity compilation, save/reopen,
support and visible-vertex checks, scene validation and 19 snow checks passed.
Three native forest views were inspected; phone performance remains untested.
Original sources, Terrain, snow depth/credit/volume and player saves are preserved.

The [alpine sign-board kit](SourceAssets/AlpineSignBoards/README.md) keeps all 12
catalog variants. Pinefall uses the user's curated **31-sign** layout: seven
single-purpose destination/service boards, 12 Road Closed signs at outer road
ends, and 12 inward-facing Alpine Danger signs around the town boundary.
Faces and arrows use actual roads, junctions and destinations; scene captions
fit the boards. **Snowglow > Asset intake > Import twelve alpine sign boards**
retains this curated layout on repeat. Native save/reopen, scene and snow checks
passed; phone performance remains untested. All 12 sign variants also passed
closed-solid winding and normal checks after repairing reversed arrow/warning
faces; backface culling stays enabled. Catalog entries stay Needs Review.

## Important: how the concrete scene is authored

The district has now been baked with **Unity 6000.3.8f1**. `Assets/Scenes/Pinefall.unity` contains the concrete terrain, colored primitive scenery, machinery prefab instances and snow chunks. Keep `Assets/Snowglow/Generated/` with the project. The editor tool creates this content **in Edit mode, before Play**. Nothing calls the district or vehicle factories at runtime.

After baking, all objects are present in the Hierarchy, inspectable and editable with Play stopped. Generated assets remain on disk and travel with the project. You can change them manually without rerunning the baker. Only gameplay state, wheel animation, bucket movement, and mutable snow geometry change at runtime.

Snow volume data lives in the seed asset; visual and collision meshes are saved separately in its sibling `_Meshes` folder, with eight chunks per native `.asset` part. Keep that folder and its `.meta` files with the scene. Editor authoring enforces a 90 MB per-file safety budget for snow assets so they can be committed to ordinary Git without Git LFS.

## Open and bake

For manual mountain placement, select the changed **Mountain lot** group(s) and
choose **Snowglow > Environment > Refit selected mountains to terrain**, with
Play stopped. It preserves X/Z, rotation and scale, seats the lower slopes and
updates rock, snow caps and collision proxies. Undo/Redo is supported; save the
scene after review. See [mountain refitting](SourceAssets/FiveAlpineMountains/README.md#refit-mountains-after-manual-placement---3-november-2026).

1. Extract the ZIP and add the `Snowglow` folder in Unity Hub.
2. Open in **Unity 6000.3.8f1** with Android Build Support installed (the version recorded in ProjectSettings).
3. Allow URP 17.3, Input System 1.18 and uGUI dependencies to import. The built-in Vehicles module supplies WheelCollider; do not remove it. Accept any requested input-system editor restart.
4. The first-import authoring tool attempts to bake the district in Edit mode. If it does not, choose **Snowglow → Bake editable test district**. Stop Play mode first. Baking can take time because it writes terrain and snow geometry assets.
5. Open `Assets/Scenes/Pinefall.unity`. Inspect the concrete objects in Scene view. The editor frames the plow near the depot; select the Terrain and press F to inspect the kilometre-wide world.
6. Press Play. Choose a contract from the dashboard. Use the on-screen touch controls or keyboard.

The saved scene is populated and included in Build Settings. Use **Snowglow > Validate saved district and snow ledger** to reopen it and check persisted rigs/assets and volume invariants. This writes `Logs/SceneValidation.txt`. Manual baking replaces the layout; use it only when you intend to author a fresh district.

## What the bake saves

- A **1000 × 1000 m Unity Terrain**, with a flat central worksite and rising surrounding terrain.
- A compact working district within approximately 250 × 250 m. The whole kilometre is landscape, not densely populated city content.
- Grey cube roads, parking area, service depot, fuel tank, retaining walls, curbs, slope and ice test lanes.
- All 40 house lots use seven imported winter-home variants at the enlarged 1.5x fitting convention, with removable decorative snow, opaque warm windows, expanded foundation footprints and preserved fences.
- Ninety-seven imported alpine tree/wood assemblies using all 26 variants, including removable snowy trees, bare snags, stumps and logs.
- Twelve imported low-poly mountains using five variants with separate white/cyan snow caps, plus a primitive roadside boulder.
- Fourteen imported alpine light fixtures using all twelve variants, plus primitive signs and parked cars.
- A detailed orange municipal plow and yellow wheeled loader, saved as connected prefabs. Tyres are cylinders with hub cylinders and individual tread cubes; cabs, mirrors, lights, exhausts, steps, and working equipment are separate objects.
- URP pipeline, renderer, shared colored materials, terrain layer and texture, vehicle/equipment definitions.
- Initial snow volume data, combined-cube chunk meshes and substantial-pile collision meshes.
- A preauthored mobile Canvas, menus, touch controls, camera, audio sources and imported synth WAV files.

Snow columns use 50 cm cells, half the width and length of this project's original 1 m cells, giving four times the horizontal resolution over the same 256 m working field. They are combined into chunk meshes rather than individual GameObjects. Transfers remain cubic metres, and old 1 m seeds/saves retain their total volume when upgraded. To update older authored snow without replacing the scenery or machinery, stop Play and choose **Snowglow > Rebuild snow at 50 cm resolution**. The carried bucket load is a preplaced sphere whose size changes. Snow particles are visual feedback only.

Piles spread as they grow. Deposits prefer lower nearby cells within a mound-shaped support envelope; steep sides then slump gradually toward a tuned 32 degree resting slope with a small cohesive lip. Settling conserves volume, avoids static obstacles, freezes while paused and creates no work credit. Loaded piles resume settling when simulation starts. The simulation processes at most 512 active cells per frame and retains the existing mesh-update budget; live feel and phone performance still require testing.

## First playthrough

- The imported **Golden snow-plowing tractor** is the free, already-owned starting machine. Its blade and beacon are fitted in the saved scene. It retains the previous starter save ID so upgrades carry over. The loader remains a separate purchase.

- Start **Depot Access**. The target is the outlined strip at **X −65 / Z −90**; drive east from the depot and then along that access road.
- Blade starts lowered. Drive forward to push snow; use ANGLE L/C/R to discharge to one side. Raise the blade for travel.
- Clear 90% of the selected contract area. Pushing snow back into it blocks progress again. Only original work in that contract area builds the combo/reward.
- Parking Lot is at **X94 / Z21**; Main Street is centered at **X0 / Z0**. Coordinates appear in the optional debug overlay.
- Buy the loader in the machinery shop (1,400 coins; the prototype starts with 1,800). Stop at the depot and select it in Garage.
- Loader WORK lowers the bucket; CARRY lifts it. In Auto mode a nearly full bucket lifts automatically, including after restoring a full bucket. DUMP lifts/tips and deposits from the bucket mouth. Lift/tilt keys or buttons immediately take manual control; R/F raises/lowers even when previously in Auto.
- The storage yard is at **X94 / Z−85**. Deposited snow remains visible up to a retained depth; excess is processed in bounded batches and recorded as disposed volume.
- Contract deadlines reduce pay by 30% when late, rather than fail the job. Fuel and collision damage are functional. Refuel/repair at the depot. Free recovery preserves the worksite and bucket contents.
- A fresh contract resets the snow to the authored initial state and returns both machines to the depot with empty buckets. Machine switching within one contract preserves the snow and loads.
- Use the dummy store to obtain test currency. All transactions are explicitly simulated; no billing or network access.

## Controls

| Control | Touch | Keyboard |
| --- | --- | --- |
| Steer | Drag the steering panel left/right | A/D or left/right |
| Drive forward | DRIVE with F selected | W/up |
| Drive reverse | DRIVE with R selected | S/down |
| Brake | BRAKE | Space |
| Forward/reverse | F/R | X |
| Work / raise / carry | WORK/CARRY | E |
| Blade angle | ANGLE L/C/R | Q |
| Dump bucket | DUMP | B |
| Cycle three camera modes | CAM | C |
| Orbit / tilt (Orbit or Scenic) | Drag empty view | Right-drag or J/L and I/K |
| Camera zoom (Orbit or Scenic) | Pinch or ZOOM +/- | Wheel or +/- |
| Reset camera view | VIEW RESET | Home |
| Recover | RECOVER | H |
| Pause | PAUSE | Escape |

The steering panel commands vehicle steering, not screen-direction movement. Camera changes do not remap steering.

Camera order is **Isometric ? Follow ? Orbit ? Scenic**. Orbit provides full 360-degree vehicle inspection; Scenic gives a wider, slowly revolving panorama with manual control. Terrain/near-plane clearance and obstruction sweeps keep the camera above ground. See [camera controls and limits](CAMERA_MODES.md). The camera tools are saved in the scene; no district rebake is required.

## Physics and machinery

Use **LIGHTS ON/OFF** at the right-center of the screen for the selected machine's
warm front and white rear lamps. Both machines have visible lamp glow; the red
roof beacon continues rotating when those lamps are switched off. Settings are
per-machine for the current play session. See [vehicle lights](VEHICLE_LIGHTS.md).

Both machines now have [automatic operating lights](VEHICLE_LIGHTS.md): front/work
beams, paired rear lamps and rotating red roof beacons turn on during active play on the
selected fueled, serviceable machine. Parked machines and menus/pause turn them
off. Two shadowless spotlights illuminate ahead; no manual light control is needed.

Both machines use a Rigidbody, four WheelColliders, powered wheels, front-wheel steering, tuned suspension, tyre surface grip and torque/braking. Assistance provides steering moderation, traction assistance and anti-roll/upright torque. It does not directly translate the machine or guarantee forward speed. Load adds 250 kg per cubic metre to mass and shifts the calculated center of mass.

The loader uses a rigid chassis and front-wheel steering. Boom/bucket movement is transform-driven equipment articulation, not a solver-based hydraulic-joint simulation. Snow is a conservative depth/volume field with pile collision proxies, not granular snow physics. Thin snow affects resistance/grip; substantial piles have collider geometry. These are prototype approximations requiring playtesting.

## Editing assets and compatibility

Generated assets are under `Assets/Snowglow/Generated/Bake_<timestamp>/`. Keep this folder when moving the finished project.

- Vehicle definitions contain stable IDs, mount IDs, mass, wheel geometry, torque, speed and load ratings.
- Attachment definitions validate mount ID, mass, width, hydraulics and machine rig type.
- Replace meshes under `VisualRoot`, preserving wheel, mount, bucket and working-edge anchors. Adjust the physical rig for changed real dimensions.
- The supplied truck/blade and loader/bucket are the supported pairings. There is no generic visual fitting solver. Additional attachment behaviors or rig types need implementation as well as matching definitions.
- Tyre and engine upgrades are functional (three levels). Alternate blade widths, adapter equipment, tracked locomotion and snowblower behavior are future work.
- A manual bake creates a **new** timestamped asset folder and replaces the test scene after the save prompt. It does not overwrite your earlier generated assets. Do not rebake after hand-editing the scene unless you intend to replace its layout.

## Save, pause and menus

A versioned local JSON file and backup are stored at `Application.persistentDataPath/snowglow-v2.json`. It stores wallet, XP, owned machines, upgrades, fuel, condition, equipment state, active contract, positions, snow/piles and the last rewarded/simulated transaction IDs. Menus and application suspension pause simulation and contract time. It autosaves every 30 seconds and at important actions.

Menus include dashboard, three contract choices, garage, machinery shop, parts, service, dummy store, settings, pause and results. Settings expose low/standard rendering, sound, assistance and debug. The first region is one district; a globe screen and other regions are not implemented in this build.

## Android build

Bake and test in Editor first. In Build Profiles select Android and include `Assets/Scenes/Pinefall.unity`. The authoring command assigns `com.snowglow.prototype`, landscape orientation, and the mobile URP asset. Install Android SDK/NDK/OpenJDK through Unity Hub and use a Development Build for initial Galaxy A07 profiling. No APK was built here.

## Verification

Read `VALIDATION.md` for current evidence, known limits and the required Unity/device acceptance checklist. Unity editor compilation and the Edit-mode bake have now run. Physics tuning, rendered scene inspection, touch behavior and phone performance still require testing. No APK has been built.

Residential spacing update (31 October): the same 40 complete lots are now spread across the western gaps and wider central rows. House identities, variants and sizes remain; their former positions are superseded. Use **Snowglow > Environment > Redistribute homes evenly**. Unity save/reopen, 1 m house/road and house/house footprint checks and all 19 snow checks passed. See VALIDATION.md.

Both vehicles now leave cosmetic cyan tyre-tread impressions on interactive snow while their wheels are grounded and moving. Marks follow steering/reverse, fade during active play and disappear when snow is cleared or reshaped; they do not alter snow volume, grip or rewards and are not saved. Editor installer: **Snowglow > Install snow tyre impressions and validate**. Native preview/fixture results are under Logs/TyreTracks; continuous driving and phone performance remain untested.


Camera revision (31 October): C/CAM now cycles Isometric -> Follow -> Orbit.
Isometric is closer at rest and widens smoothly with speed. Follow smoothly moves
to the front during actual reversing and returns behind for forward travel.
Scenic is deferred to a future static main-menu background preview. See
CAMERA_MODES.md for tuning and controls.


## Simplified driving buttons - 1 November 2026

Current controls supersede earlier manual equipment instructions: Lift/Tilt buttons
and R/F/T/G manual operation are removed, as is Auto/Manual and the V shortcut.
Loader buckets automatically lift/curl to carry at 92% capacity, including legacy
saved Manual states; WORK/CARRY and DUMP remain. Separate held LEFT/RIGHT buttons
replace drag steering; opposite simultaneous buttons cancel and releases do not
cancel the other held button. Keyboard A/D/arrows remain.

Driving side groups are anchored at 5% and 95% of the safe-area width, independently
of the centred menu canvas. F/R is enlarged to 150x64 design units and separated
from Drive by a 50-unit vertical gap. Recover is explicitly bound to the same
Recover-to-depot action as service recovery and relabelled RECOVER TO DEPOT.
UI and loader checks/results are recorded in Logs/DrivingButtons/Validation.txt
and Logs/DrivingControls.txt. Physical-phone ergonomics remain to be tested.

All eight angular cloud variants are prepared in the catalog and scattered as
40 saved sky objects in Pinefall, using one shared opaque palette with no cloud
colliders or shadows. See SourceAssets/AngularClouds/README.md for source details,
counts and the repeat-safe Editor command. Native save/reopen and scene validation
passed; Android performance remains untested.

The [four-bay snow garage](SourceAssets/FourBaySnowGarage/README.md) is a reusable
Needs Review building assembly, now replacing the primitive Pinefall workshop.
Its old wall lantern is removed; Fuel + Repair is a standing sign near the red
tank. Use **Snowglow > Asset intake > Import four bay snow garage**.

The [imported snow disposal yard](SourceAssets/SnowDisposalYard/README.md) replaces
Pinefall?s primitive yard and pad. Its reusable catalog assembly remains Needs
Review; use **Snowglow > Asset intake > Import snow disposal yard**.

The [snow machinery fuel station](SourceAssets/SnowMachineryFuelStation/README.md)
replaces the primitive red tank. Its catalog assembly remains Needs Review; use
**Snowglow > Asset intake > Import snow machinery fuel station**. Existing depot
service behavior and standing Fuel + Repair sign are preserved.

All [three imported snowmen](SourceAssets/ThreeSnowmen/README.md) are placed
around Pinefall residential blocks. Their reusable catalog entries remain Needs
Review; use **Snowglow > Asset intake > Import three snowmen**.
