# Snowglow: project instructions, design history and development handoff

Last updated: **31 October 2026**, Asia/Karachi.

This document records the conversation from the original pasted ChatGPT planning
through the Unity prototype, snow revisions, Asset Workshop, imported machinery,
clearance/tread repairs and opaque toon windows. It summarizes decisions and their
reasons rather than reproducing every message. Historical proposals are not proof
of implementation. Later explicit user decisions supersede earlier proposals.

## 1. Read this first

Before development, read [README.md](README.md), [ARCHITECTURE.md](ARCHITECTURE.md)
and [VALIDATION.md](VALIDATION.md). For content work also read
[ASSET_WORKSHOP.md](ASSET_WORKSHOP.md), [MOBILE_ASSET_BUDGET.md](MOBILE_ASSET_BUDGET.md)
and the relevant `SourceAssets/*/README.md`.

The most important continuing requirements are:

- Build a cozy **full-3D, mid low-poly snow-clearing game in Unity**, Android first,
  offline, using URP. The reference phone is **Samsung Galaxy A07**.
- Prioritize a production-ready first release: reliable driving/plowing/loading,
  the complete game loop, usable UI, purchasing/economy and Android validation.
  Necessary release UI is in scope; decorative expansion and speculative systems
  follow the ordered roadmap below.
- Vehicle motion must come from Rigidbody/WheelCollider physics. Stability
  assistance is intentional; it must not teleport vehicles or guarantee traction.
- Snow volume has one authoritative owner. Transfers must conserve volume except
  explicitly recorded disposal. Particles are feedback, never authoritative snow.
- Vehicles, scenery, equipment, effects and their bindings must be authored and
  saved **in the Editor before Play**, not assembled by runtime factories. Runtime
  wheel/attachment movement and mutable snow meshes are allowed.
- Keep one Unity project at this root. The user explicitly rejected a separate
  `.validation-project` project structure. Do not recreate that validation clone.
- Preserve source models, stable asset/save IDs, GUIDs, `.meta` files, saved snow
  data and manually authored scene work. Do not rebake the whole district merely
  to replace one model or change snow resolution.
- The active next phase is **first-release essentials**, superseding the earlier
  weather/lighting priority. Work one task at a time, finish and verify it before
  advancing. Preserve imported assets and scene work; further furnishing/model
  replacement and weather/lighting are future updates unless needed for release.
- **Snowfall effects are deferred to a future phase; omit them from current work.**
  When added later, falling snow must be visual only, localized around/in front of
  the active camera, with no increase to ground snow, piles, bucket loads or volume.
- For the deferred lighting update, home windows, **including door windows**,
  must glow with bloom in daylight as well as at night. This remains a future
  requirement, not a blocker for the first release.
- Report what actually compiled, saved, reopened, rendered or ran. Do not equate
  a structural check, sampled pose check or low triangle count with a successful
  driving test, Android build or measured phone performance.
- Current window style is **opaque bright cyan toon glazing with pale diagonal
  highlights**, following the user's image. Do not restore transparent glass.
- Current snow art direction is **pure-white tops with pale cyan side shading**, following the user's snowy mountain reference. This supersedes the initial off-white palette.
  See `SNOW_MATERIAL.md`. All seven collections from `Seven_Winter_Homes.blend`
  are now prepared and saved across 40 Pinefall house lots in groups of 2, 4 or 6 through Editor
  authoring. Preserve separable decorative snow and opaque warm windows.

Initial planning messages explicitly asked not to build yet. The user later
authorized implementation, the asset-management phase and the individual model
replacements. Those old planning pauses are historical, not a standing requirement
to seek permission again for already authorized work.

## Ordered roadmap: first release, then future updates (31 October 2026)

The latest user decision is to pursue a production-level first release, with UI,
purchasing and the overall game loop as essentials. This supersedes the previous
plan to implement weather/lighting next. All items below are planned work unless
the current implementation and validation records explicitly establish otherwise.
This request updates documentation only; it does not implement these features.

Proceed **one concrete task at a time** in the following preference order. Verify
the result and report remaining limits before moving on. Resolve blockers in an
earlier milestone before investing in later polish; do not expand the release
scope simply because a future feature is listed.

### First-release milestones, in preference order

1. **Android baseline and device smoke test.** Build a development APK from this
   root project and install/test on the Galaxy A07. Confirm launch, rendering,
   touch input, basic driving and background/resume. This is the starting task;
   actual device testing requires access to the phone. Record build/device
   evidence and failures rather than treating an Editor pass as phone acceptance.
2. **Complete and stabilize the core game loop.** Validate contract selection,
   machine selection, driving, plowing, loading, transporting, dumping, completion,
   rewards and return to the garage/next job. Include Free Work, depot switching,
   service costs, progression and useful upgrades. Make objectives, success and
   recovery understandable; tune existing content before adding more. Preserve
   snow conservation, original-work-only rewards and stable machine/save IDs.
3. **Production UI, onboarding and touch usability.** Polish the existing
   dashboard, contracts, garage, shop/parts, service, settings, pause and results
   around that loop. Establish consistent layout, typography, navigation,
   feedback and safe-area/scaling behavior. Make driving/equipment controls,
   objectives, prices, ownership and errors clear. Add concise first-time guidance
   and verify the full flow on the phone, including all camera modes.
4. **Purchasing and economy readiness.** Finish reliable in-game coin purchases,
   ownership/equipping, upgrades and service transactions. Check affordability,
   cancellation, failure, repeated taps, duplicate delivery and save/resume.
   Separately decide whether the first release needs real-money purchases;
   the current dummy store is not real billing. Real-money product types, pricing
   and provider integration remain undecided. If included, implement and validate
   the chosen store integration, entitlement persistence and relevant recovery/
   pending/refund cases before release. If omitted, remove or hide player-facing
   simulated purchase/debug reward flows in the release. Keep normal gameplay
   offline; any optional billing network requirements must be explicit. Do not
   charge, publish products or make commercial commitments from this roadmap.
5. **Persistence, lifecycle and failure recovery.** Verify fresh install, existing
   save migration, autosave, backup recovery, interrupted writes, suspension,
   process termination and resume. Restore contracts, piles, carried loads,
   wallet, ownership and upgrades consistently. Pause must not silently advance
   contract deadlines. Remove release-blocking exceptions and soft locks.
6. **Performance and final presentation.** Profile sustained representative play
   on the A07 toward 30 FPS / 33.3 ms, including snow work, busy views, UI and saving.
   Address memory, heat, loading and save hitches. Finish essential audio/visual
   feedback and readability using the current daytime setting and assets.
   Weather, extra decorative imports and lighting upgrades are not prerequisites.
7. **Release candidate and publishing preparation.** Run complete physical-device
   playthroughs and regression checks, validate release build/install/update,
   finalize identity/versioning, icons, store screenshots/description, support and
   applicable privacy/store disclosures. Check current platform requirements
   when doing that work. Remove prototype labels/debug entry points as appropriate
   and test the chosen purchase strategy. Prepare a concrete reviewable release
   before requesting any publishing action; this document does not authorize
   public submission or real transactions. No release-ready claim without evidence.

Initial release scope stays focused on **Pinefall, the two current machines,
Career contracts and Free Work**. Existing broader ambitions remain in the
backlog rather than becoming automatic launch requirements.

### Future-update milestones, in preference order

After first-release acceptance, the retained weather/lighting backlog is ordered
as follows. Do not begin it as a substitute for unresolved release essentials.

1. **Day/night foundation.** Establish a coordinated time-of-day system with
   smooth daylight, dusk, night and dawn transitions. Coordinate directional
   lighting, sky/background, ambient light and fog so the district and machinery
   remain readable in the cozy toon style. Cycle duration, starting time,
   player controls and time persistence are still design choices, not settled
   requirements. Define pause/lifecycle behavior consistently with the session.
2. **Home window emission and bloom.** Give every home window, including glazing
   in doors, visible glow with URP bloom **during daylight too**, retained through
   dusk and night. Preserve opaque warm home glazing and the established opaque
   cyan toon vehicle glazing; emission must not introduce transparent glass.
   Tune material emission and camera/post-processing together: an unlit bright
   surface alone is not evidence of bloom. Keep highlights controlled so the
   white snow retains detail. Per-window real-time lights are not required to
   achieve emissive glow.
3. **Street lighting at night.** Coordinate street-lamp luminous meshes and
   useful ground illumination with dusk/dawn, using smooth transitions. Reuse
   the prepared fixtures and their optional authored lights where appropriate.
   Budget real-time light range, overlap and shadows deliberately rather than
   enabling every optional light indiscriminately. Distinguish visible lamp
   glow/bloom from actual illumination of roads and snow.
4. **Vehicle lighting.** Prepare and bind headlights, rear/brake/reverse lights,
   work lights and existing beacons as applicable to each machine. Coordinate
   night visibility and driving-state cues with the existing controllers;
   exact controls and automatic switching remain to be decided. Preserve rigs,
   attachment motion, save IDs and opaque toon glazing.
5. **Wind.** Establish visual wind direction/strength and gentle variation as
   appropriate for the environment. Wind must not move authoritative snow or
   apply vehicle forces without a separate explicit decision.
6. **Weather/lighting integration and acceptance.** Save all controllers, effect objects, lights,
   materials and URP volume/camera bindings in the Editor before Play. Runtime
   may update those authored bindings and lighting values;
   do not introduce runtime scene factories. Verify scene save/reopen, all four
   cameras, day/night transitions, daylight bloom on home and door windows,
   night street/vehicle illumination, pause and machine switching. Check that
   weather alone leaves snow depth, credit and volume accounting unchanged.
   Report rendered inspection, playtesting and Galaxy A07 profiling separately;
   light counts and Editor checks do not prove sustained 30 FPS.

7. **Visual snowfall, deferred until requested.** Omit from the first release and
   initial weather/lighting work. When requested, use camera-local particles around/in front of the
active camera, supporting all four camera modes with bounded mobile overdraw.
Snowfall must remain purely atmospheric: no accumulation, ground-depth changes,
pile growth, work credit, disposal changes or bucket filling.

8. **Further content and systems, lower priority.** Resume imported furnishing/
   prototype replacements and consider additional machinery, compatible parts,
   contracts and districts only after the release and the above updates are stable.
   Retain the historical backlog of regional/globe progression, additional modes,
   tracked machines, snowblowers, adapters/chains/transmission/ballast/hydraulics
   and cosmetics. These are ideas requiring separate scope and implementation,
   not promised features or an instruction to build all of them. Broader services,
   general inventory/loadouts or cloud integrations should be justified by actual
   content needs and preserve existing stable IDs and saves.

Additional weather types or systems implied by "etc." remain open scope; do not
assume rain, storms, seasonal changes, melting, accumulation or a weather service
has been requested. Preserve the current house distribution, source assets,
GUIDs, snow seed/mesh assets and player saves. Apply this phase to the existing
Pinefall scene without a whole-district rebake.

## 2. Current project snapshot

| Item | Current state |
| --- | --- |
| Project | Snowglow |
| Unity | `6000.3.8f1` / Unity 6.3 LTS |
| Rendering | URP `17.3.0`, mobile rendering settings |
| Input/UI | Input System `1.18.0`, uGUI `2.0.0` |
| Target | Android, landscape, offline |
| Reference performance | Sustained 30 FPS / 33.3 ms, **not yet measured on A07** |
| Main scene | `Assets/Scenes/Pinefall.unity`, saved and included in build scenes |
| World | 1,000 x 1,000 m Terrain; compact test district near its centre |
| Interactive snow | 256 x 256 m field, 512 x 512 cells at 0.5 m |
| Housing | 40 lots in ten blocks of 2, 4 or 6 use seven imported variants at the enlarged fitting scale; home identities preserved; lots redistributed evenly on 31 October |
| Fleet[0] | Imported Golden tractor, free and always owned initially |
| Fleet[1] | Imported Golden snow loader, 1,400 coins, initially unowned |
| Fresh wallet | 1,800 prototype coins |
| Modes | Career contracts and Free Work |
| Cameras | Isometric with speed zoom, direction-aware Follow, full 360-degree Orbit; Scenic reserved for a future static main-menu preview |
| Save | Version-2 local JSON plus backup, `snowglow-v2.json` |
| Store | Clearly simulated purchases only; no real billing integration |

The live cameras use the project's own `CameraRig`, not Cinemachine. Current HUD
code uses uGUI `Text`, not TextMeshPro. Both packages were mentioned in the original
architecture proposal; do not claim they were implemented merely because planned.

## 3. Original conversation and game design

### 3.1 Starting idea and scope correction

The original idea was a cozy 3D isometric snow-plowing game with upgrades, combo
multipliers and synth sound effects. An early assistant started down a browser-game
route; the user clarified **Unity 3D**. A Unity project/ZIP called Snowglow followed.
Early source-only claims were explicitly qualified because Unity was not installed
in that earlier environment. Do not use those claims as Editor validation evidence.

The user then requested architecture and planning before further implementation:

- Real machinery mechanics, including tyres and suspension.
- Multiple machines, machinery shop, compatible parts and upgrades.
- Combos, coins, XP and levels.
- Career and other modes.
- Districts/regions across a globe, unlocked through career progression.
- Full 3D mid low-poly presentation.
- Isometric and follow cameras.
- Primitive body/wheel/equipment/environment assemblies first, finished art later.
- Android/mobile URP with room for other platforms.

The early small prototype's claims such as turbo are not the current feature
contract. Use current scripts and the final scope below.

### 3.2 Choices agreed with the user

| Decision | Reason |
| --- | --- |
| Realistic physics with strong stability assistance | Weight/grip/load should matter while touch driving remains forgiving |
| First milestone: wheeled plow plus wheeled loader | Proves pushing, accumulating, collecting, transporting and dumping snow |
| One representative fictional Alaska/Canada-inspired district | Concentrates tuning effort before worldwide content production |
| Android first, platform services replaceable | Small initial target without closing off later platforms |
| Full 3D URP, restrained shaders and shared materials | Suitable visual direction for a broad mobile audience |
| Career and Free Work | Tests progression plus relaxed operation using the same mechanics |
| Fuel, damage, repairs and deadlines | Gives upgrades and operating decisions consequences |
| Ordinary late jobs reduce rewards by 30% rather than fail | Preserves the cozy tone and avoids harsh progression pressure |
| Auto/Manual equipment, Auto initially enabled | Accessible controls while retaining direct lift/tilt operation |
| Fixed starting snow for the mechanics milestone | Avoids mixing active accumulation with unproven clearing/accounting |
| Visible disposal storage with explicit excess processing | Keeps snow movement understandable and volume removal accountable |
| Depot switching during a contract | Lets a player plow first, then load the same persistent piles |
| Dummy purchases, offline prototype | Tests transaction behavior without charging money or adding a billing SDK |
| Replaceable visuals and explicit fitting rules | New art should not require rewriting driving or snow logic |

The first terrain was explicitly requested at approximately 1 km square, with
houses, roads, trees, poles, mountains and advanced-looking machinery made from
coloured cubes, cylinders and spheres. The playable worksite is much smaller than
the terrain; the outer kilometre supplies surrounding landscape.

### 3.3 Long-term architecture proposed, not all implemented

Planned logical owners were application/lifecycle, vehicles, snow, contracts,
economy, progression, world, presentation and persistence. Content should use
immutable definitions with stable IDs; player ownership and mutable state belong
in saves. Commands and reported gameplay facts should have clear dependencies,
without a universal event bus hiding everything.

The intended career loop is: choose contract -> choose machine/equipment -> work ->
earn coins/XP -> service/upgrade/buy -> unlock more districts/regions. Reputation,
medals, a globe/map screen, progression tables, tutorial/challenge/sandbox modes,
additional regions, tracked machines, snowblowers, adapters, chains, transmission,
ballast, hydraulics and cosmetic equipment were discussed as extensions.

Those extensions are not all present. Current `PrototypeSession` deliberately
coordinates several small prototype concerns. Extract dedicated services before
large career/content/monetization expansion rather than claiming the full proposed
service architecture already exists.

### 3.4 Mechanics gates agreed during planning

1. Driving: contact, suspension, braking, surfaces, assistance and cameras.
2. Plowing: real blade footprint/direction, displacement, resistance and obstruction.
3. Loading: finite capacity, carrying, mass/balance and consistent dumping.
4. Shared worksite: depot switching and explicit disposal without resetting piles.
5. Complete session: contracts, rewards, upgrades, costs, menus and saving.
6. Android acceptance: sustained real-device play, touch controls and lifecycle.

Comfortable phone driving, understandable ice/tyre differences, load consequences,
no repeated-pile reward exploit and reliable restoration are acceptance criteria,
not completed device-test results.

## 4. Implemented prototype and runtime boundaries

### 4.1 Scene and environment

Pinefall contains saved Terrain, roads, parking, service depot/fuel facilities,
40 imported homes/fences, 97 imported alpine tree/wood assemblies, 12 imported alpine mountains, 14 imported alpine light fixtures, primitive signs/cars, curbs, slope
and ice test lanes, a disposal area, contract markers, UI, camera and audio sources.
Most scenery remains primitive art. Both active machinery vehicles are now imported
models. Older primitive machinery prefabs and catalog examples remain available;
their continued presence does not mean they are the active fleet.

Editor authoring writes timestamped assets under `Assets/Snowglow/Generated/`.
`PrototypeAuthoring.Bake()` creates a fresh district and intentionally replaces the
scene layout. It now uses `StarterPlowAuthoring.Create()` and
`LoaderModelIntake.Create()` for the imported fleet. No runtime callback should
call the district, primitive vehicle or HUD factories.

### 4.2 Runtime code map

| File/class | Responsibility |
| --- | --- |
| `MachineController` | Rigidbody/WheelColliders, torque, braking, steering, grip, assists, fuel, damage and loaded mass/COM |
| `AttachmentController` | Blade/bucket controls, swept work, processing budget, load/dump and authored rig motion |
| `SnowVolumeGrid` | Authoritative depth, transfers, work credit, settling and conservation |
| `SnowField` | Grid queries, chunk geometry/collision refresh, contract progress and disposal |
| `PrototypeSession` | Current fleet, contract/session state, small prototype economy/progression and save coordination |
| `PrototypeInput` / `TouchControl` | Keyboard and multi-pointer touch input |
| `PrototypeHUD` / `UICommand` | Saved Canvas/menu presentation and action requests |
| `CameraRig` | Four view modes, mouse/touch orbit, terrain safety and unchanged steering semantics |
| `SynthAudio` | Engine/synth feedback using supplied WAVs |
| `SaveStore` | Versioned JSON load/write, temporary replacement and backup recovery |
| `DistanceVisibility` | Existing distance-based renderer visibility helper |

Both machines use four WheelColliders and front-wheel steering. The loader is a
rigid chassis; a visible articulation kingpin is not simulated articulated steering.
Snow contributes resistance/grip and substantial-pile collider obstruction. Wheel
friction is configured through WheelCollider curves, not ordinary material friction.

Assistance includes steering smoothing/speed moderation, traction reduction under
slip, anti-roll and upright torque. Carried snow adds **250 kg per cubic metre**
and contributes to calculated COM at the attachment mouth. Equipment mass also
contributes. This is assisted prototype simulation, not a detailed powertrain or
solver-based hydraulic simulation.

### 4.3 Progression, menus and saving

The current UI includes dashboard, contracts, garage, machinery shop, parts,
service, dummy store, settings, pause and results. There are three contract areas:
Depot Access, Parking Lot and Main Street. Completing a normal contract requires
90% current clearing; putting snow back can reduce accessibility again.

Only useful, originally uncredited work builds reward credit and the combo, capped
at an 8x displayed multiplier. Repeatedly shuffling a pile must not mint work credit.
Coins/XP, three tyre upgrade levels, three engine upgrade levels, fuel and condition
are implemented. Wider blades, arbitrary inventories and the broader part families
are not yet general player-facing equipment systems.

Fresh contracts reset snow to the authored seed and return machines with empty
buckets. Switching during a job preserves the snow and individual loads/states.
Menus/suspension pause simulation and contract time; no offline deadline deduction
was intended. Recovery preserves the worksite/load. Transactions use IDs to avoid
duplicate completion/dummy rewards; simulated success/cancel/failure/duplicate cases
belong in the acceptance checks.

Save schema 2 stores coins, XP, ownership, upgrades, fuel, condition, equipment
states, active selection/job, positions, snow and transaction IDs. It lives at
`Application.persistentDataPath/snowglow-v2.json` with a backup. Autosave is roughly
every 30 seconds plus important actions. Do not delete or rewrite a player's save
just to inspect a replacement prefab.

### 4.4 Switching machines and PC controls

The user explicitly asked whether plowing and loading use separate machines: yes.
Buy the loader in the machinery shop, stop at the depot, and select it in Garage.
The current guard requires the active machine at the depot and speed <=1 m/s;
switching does not reset the active contract or piles.

| Action | PC control |
| --- | --- |
| Forward | W / Up arrow |
| Reverse | S / Down arrow |
| Steering | A/D / Left/Right arrows |
| Brake | Space |
| Toggle touch-selected F/R gear | X, when direction keys are not held |
| Work/raise/carry | E |
| Blade angle cycle | Q |
| Dump | B |
| Cycle Isometric / Follow / Orbit | C |
| Orbit/tilt in Orbit | Right-drag or J/L and I/K |
| Zoom / reset camera | Wheel or +/- / Home |
| Recover | H |
| Pause | Escape |

S/Down was an explicit user change: it selects reverse and applies throttle, rather
than acting merely as a brake. Opposite-direction input brakes before reversing;
it must not instantly flip velocity. Simultaneous forward/reverse input brakes.
Touch uses steering, drive/brake, F/R and equipment buttons. Changing camera must
not change steering semantics.

## 5. Snow resolution, shape, spreading and Git history

### 5.1 Resolution/shape conversation and final state

The user iterated on smaller boxes, a roughly quarter-size request, doubling,
strict cubes versus rectangles, reverting, then **half-size horizontal boxes**.
Do not apply every historical request cumulatively. The final current implementation
is **0.5 m horizontal cells**, reduced from the original 1 m: four times as many
horizontal cells over the same field.

Current constants are `Size=512`, `Origin=-128`, `Cell=.5`, `Area=Cell*Cell`, with
32x32-cell chunks and 16x16 chunks. Visuals are combined cube-derived columns whose
height represents depth; they are not a separate fixed-size Rigidbody cube per cell
and do not guarantee equal X/Y/Z dimensions at every snow depth.

Resolution changes updated cell-area volume conversion, coordinates, work
footprints, disposal ranges, validation and legacy 1 m seed/save migration. Use the
snow-only rebuild command to retain scenery/machinery. Terrain is not deformed.

There are older ignored logs for rounded snow and geometry experiments, including
large reported mesh reductions. Their presence is not evidence those renderers are
current. Checked-in `SnowField` currently builds combined cube columns. Inspect
active code and saved mesh references before reviving a reverted visual experiment.

### 5.2 Tall rods versus spreading piles

The user reported tall rods with too little sideways spreading and wanted piles
to grow and fall outward with a cohesive, goo-like feel. Centre-first deposits were
replaced with lower-cell placement weighted by distance inside a mound support
envelope, plus gradual eight-neighbour slumping.

Current tuning uses approximately a **32 degree resting slope**, `ReposeSlope=.625`,
3 cm cohesion and bounded transfer speed. Existing/loaded piles wake for settling;
pause freezes them. Settling conserves volume, respects obstacles/field edges and
creates no work/reward credit. At most 512 active cells are processed per frame,
with bounded mesh updates. This is a depth-field approximation, not granular goo,
fluid dynamics, overhangs, fracture, compaction, melting or active accumulation.

### 5.3 Git error and oversized snow asset

The user asked to diagnose a Bash Git error, then explicitly requested splitting
`Assets/Snowglow/Generated/Snow50cm_20261026_085523_745.asset` below GitHub's 100 MB
file threshold. The original bundled asset was **170,024,383 bytes**.

The split retained its seed GUID/data and moved saved visual/collision meshes to a
sibling `_Meshes` directory: 32 native mesh-part assets, eight chunks per file.
Recorded results were about 5.23 MB for the seed and at most about 12.02 MB per part.
Scene mesh/collider references were rewired and the saved scene reopened. Volume
was unchanged. Future full/snow-only bakes use `SnowMeshAssets` and enforce a
**90,000,000-byte per-file safety budget**. Keep the mesh directory and `.meta` files.

The local backup ref `refs/backup/pre-snow-asset-split-20261026` points to
`37ea614923bc0d8174a6462b2ba0c33d98fb0fc3`; ignored original asset/scene backups were
also kept under `Logs/SnowAssetSplitBackup/`. Do not assume that local backup ref
exists in a fresh clone or rewrite/push Git history without a task requiring it.
The resulting snow-change commit is `9ab20d0` in the inspected history.

## 6. First Unity compilation and scene-baking verification

The user specifically asked to verify compilation and Editor baking, fix errors,
and report untested areas **before expanding features**.

On 22 October the installed Unity 6000.3.8f1/Android environment was used to:

- Remove nonexistent package dependency `com.unity.modules.textrendering`.
- Enable `com.unity.modules.vehicles`, required for WheelCollider.
- Compile runtime and Editor assemblies with URP/Input System/uGUI.
- Run Edit-mode district authoring and save Pinefall plus persistent generated
  materials, Terrain, machine prefabs, snow chunks and UI assets.
- Add `SceneValidation`, checking persisted references, scripts, compatible rigs,
  loader anchors, Terrain, snow seed/chunks, materials, URP and build-scene inclusion.
- Stop treating a failed scene save as success; validate before writing the ready
  marker. Automatic first-import baking is disabled in batch mode.

An early isolated `.validation-project` copy was used to avoid disturbing an open
Editor. The user rejected that structure and it was removed. All later work should
use this one project. A second fresh-bake attempt was interrupted; its log is not
proof of a completed additional bake.

The snow check suite grew from 9 original ledger checks to 12 after resolution work
and 19 after cohesive spreading. Source parsing/Mono checks were useful early
evidence, but are weaker than actual Unity compilation/save-reopen verification.

## 7. Asset-management phase: request, design and implementation

### 7.1 User's intended workflow

After being satisfied with mechanics so far, the user moved to asset management.
They wanted an **Editor-only drag-and-drop** preparation/assembly system:

- Import Blender, Meshy.ai or other models, including models edited in Blender.
- Drag a body, blade/bucket, tyres and accessories into compatible inventory slots
  to create an equipped machine for later player purchase/equipping by buttons.
- Keep wheels and other parts modular where meaningful for upgrades.
- Prepare scenery as reusable assemblies with category catalogues.
- Use approved catalogue blocks for AI-authored future levels/scenes.
- Accept future model files dropped into chat, identify their parts and carry out
  preparation autonomously where possible.
- Include assets made from prompts/reference images, assistant-generated Blender
  Python scripts and other generators in the same provenance/preparation workflow.

This started as planning only. The user accepted the recommendations, then said
to execute the phase. The resulting workshop is Editor tooling, not an in-game
drag-and-drop construction interface or a completed universal player inventory.

### 7.2 Why preparation is separate from source and gameplay

Original files are preserved, because import corrections, material conversion,
rigging, collider proxies and optimizations must remain reviewable/reversible.
Mechanical interface checks alone do not prove dimensional fit or visual clearance.
One attachment category string must not magically make every combination work.

Metres, Y-up/Z-forward, normalized roots, known wheel pivots, connection anchors and
authored fitting profiles let comparable models reuse gameplay. New locomotion or
equipment behavior still needs real implementation. The generic workshop does not
stretch arbitrary equipment until it appears compatible. The later loader reach
extension was a specific, explicitly requested repair to a prepared model.

### 7.3 Data and Editor tools built

| Type/tool | Purpose |
| --- | --- |
| `CatalogAsset`, `AssetCatalog` | Stable IDs, category, source, status, prefab, bounds, counts, constraints and definitions |
| `AssetAnchor` | Connection, working edge, load visual, roads, entrances/service/etc. |
| `AssetSocket` | Accepted category/interface, mass/width, hydraulics and movement envelope |
| `PreparedVehicleRig` | Explicit visual root, wheels, wheel visuals, boom and wheel clearance |
| `FittingProfile` | Pair-specific socket/position/rotation and reviewed dependency hashes |
| `AssemblyRecipe` | Vehicle body plus fitted parts and saved output |
| `CatalogPlacement` | Content identity/placement metadata, no runtime construction |
| `CatalogPreparation` | Source intake, normalization, material copies, suggested colliders/rigs and metrics |
| `CatalogValidation` | Structural/compatibility checks and approval fingerprints |
| `CatalogAssembly` | Connection alignment, previews, equipped prefab/physics baking |
| `CatalogBootstrap` | Separate reusable copies of the existing primitive examples |
| `AssetWorkshopWindow`, `CatalogInspectors` | Prepare/catalog/assembly/AI-plan interface and review tools |
| `CatalogScenePlans` | Ready manifest, validated declarative JSON, new saved layout scenes |
| `CatalogChecks` | 39 actual Editor acceptance checks with disposable fixtures |

Data folders are `Assets/Snowglow/Catalog/{Entries,Prefabs,Recipes,Sources,Examples}`;
the library is `Assets/Snowglow/Catalog/SnowglowCatalog.asset`.

Initial catalog examples: prototype plow body, loader body, blade, bucket, wheel set,
cottage lot, snow tree, depot and 16 m road. These were separate copies of authored
content and initially **Needs Review**. Imported entries also remain reviewable;
do not mark everything Ready because it is present in Pinefall.

Preparation preserves originals, rejects raw gameplay scripts and strips imported
colliders/Rigidbodies before proposing replacements. Validation rejects imported
cameras/audio listeners; remove those explicitly from usable blocks. Standard/legacy material conversion creates
URP copies. Fused wheels or missing boom/working/load anchors block readiness.

Ready publication requires visual review and valid structure. Fingerprints include
prefab dependencies, physical definitions and constraints; fitting fingerprints
also cover the socket and offsets. Changes invalidate approvals. Re-baked equipped
outputs return to review. Stable entry IDs are read-only in custom inspectors.

Vehicle definitions gained optional base grip and dry COM with legacy defaults.
Attachment motion gained `useAuthoredRig`, retaining imported rest transforms
instead of imposing the original primitive hard-coded mount positions. Assembly
baking includes wheel dimensions/mass/grip, accessory mass/torque and COM effects.

### 7.4 AI layout authoring and limits

The Ready-only manifest describes IDs, bounds, tags, anchors, sockets and limits.
Version-1 JSON placements specify ID, asset ID, position, rotation, scale and
optional road connections. Validation rejects duplicate/unknown/stale/unapproved
assets, non-finite transforms, invalid scales/ranges, conservative footprint overlap,
road interface mismatch and missing/cyclic connections before scene construction.

Plans are limited to 1,000 placements and 2 MB JSON. They author new scenes under
`Assets/Scenes`; they cannot overwrite Pinefall or another existing scene. Optional
flat 1 km Terrain is supported. This produces **layouts**, not automatic contracts,
snow coverage, a session fleet or an unlimited runtime world generator.

The general wheel-set implementation reuses one wheel at all four locations.
It does not yet model distinct axle-specific upgrade sets. Generic garage/shop
integration with all catalog parts and durable player loadouts remains TODO.

## 8. Imported tractor and free starting machine

The user supplied `low_poly_snow_plower.blend`, with part names in the file.
The assistant inspected it, preserved a copy and rendered a source view before
preparing separate modules. The user then explicitly requested it as the first,
free plowing machine.

- Original copy: `SourceAssets/LowPolySnowPlower/low_poly_snow_plower.blend`.
- SHA256: `8F48568B0051F2379707F15809558090DE7128C23637BAD6E6A8774BA815B85C`.
- Scripts: `Tools/AssetIntake/inspect_plower.py`, `export_plower.py`.
- Prepared FBX/materials/part map: `Catalog/Sources/LowPolySnowPlower`.
- All 209 vehicle meshes were accounted for; unrelated cube, ground, cameras and
  lights were excluded. Embedded `Toon_Snowplow.py` was not executed.
- Modules: body plus four separate wheels, curved plow/lift frame, roof beacon.
- Catalog IDs: `imported-tractor-body`, `imported-tractor-plow`,
  `imported-tractor-beacon`.
- Recipe: `Catalog/Recipes/GoldenTractor.asset`.
- `GoldenTractor-REVIEW.prefab` is a preview without MachineController.
- `GoldenTractor-Starter.prefab` is the equipped gameplay machine in `Fleet[0]`.

The starter retains **`municipal-plow-01`** as its save ID, despite the new model and
display name, to retain existing upgrades/state. Its price is zero and startup
forces starter ownership even if an older save recorded `owned=false`. Existing
wallets are not reset. The body has a 2,800 kg proposal plus 5 kg beacon; the plow is
450 kg. These values are tuning proposals, not measured manufacturer specifications.

Measured geometry/rig: front/rear tyre radii **0.578/0.888 m**, 1.93 m wheelbase,
1.82 m track; proposed top speed 8 m/s. The uniform wheel-set socket was omitted
because this tractor's axles have different tyre sizes. Its four wheel pivots remain
separate for future axle-specific upgrades.

`StarterPlowAuthoring` saved its Rigidbody, WheelColliders, plow working edge,
authored lift/angle controls and preplaced snow spray, then replaced only fleet slot
zero in Pinefall. Full district baking also uses this prefab. The plow/frame/rams
currently move as one rigid attachment; a faithful lift/yaw hydraulic linkage is
not implemented. Explicit prototype integration is not blanket catalog publication.

## 9. Imported loader, mobile budgets and integration fixes

### 9.1 Source, preparation and replacement

The user next supplied `snow_loader_lp.blend` and requested replacing the primitive
loader while gradually replacing primitive art. They also asked for warnings when
vertex/triangle costs exceed a sensible mobile band and reaffirmed mid low-poly art.

- Original copy: `SourceAssets/LowPolySnowLoader/snow_loader_lp.blend`.
- SHA256: `365804EDC406BD3D021C8156956C984815D4567F30ABAA58D2F29ED993467327`.
- Scripts: `Tools/AssetIntake/export_loader.py`, `check_loader_treads.py`.
- Sources: `Catalog/Sources/LowPolySnowLoader`, including part map, export metrics,
  shared palette, FBX body/boom/wheels and separate bucket.
- Catalog IDs: `imported-loader-body`, `imported-loader-bucket`.
- Recipe: `Catalog/Recipes/ImportedLoader.asset` and bucket fitting.
- Gameplay prefab: `Catalog/Prefabs/ImportedSnowLoader.prefab` in `Fleet[1]`.
- Save ID stays **`wheeled-loader-01`**, price stays **1,400**, initially unowned.
  Existing loader ownership/upgrades/fuel are matched by ID. The free tractor stays.

Blender meshes and three curve hoses are evaluated before export. Rigid modules
are combined, but four wheels, boom and bucket remain independently bound. A
point-sampled 16x1 colour atlas collapses opaque colours into one material; windows
are a separate material. Carried snow is a saved, initially inactive sphere proxy.
No original Blender file is overwritten or embedded source script executed.

Rig values: 0.73 m wheel radius, 2.38 m wheelbase, 1.82 m track, 2.84 m bucket width;
4,200 kg body, 550 kg bucket and 1.6 cubic metre capacity remain prototype ratings.

### 9.2 Specific bugs fixed during loader integration

1. **Bucket pivot/ground clearance:** rotating the centred catalog part root rather
   than its actual pin allowed the dump to swing too low. A separate tilt-pivot
   wrapper now rotates around the connection. Anchors use bucket-local coordinates;
   the single-mesh FBX root does not retain the multi-object source's global origin.
2. **Automatic dump height:** a per-rig `dumpMinimumLift` was added. Defaults stay
   0.55 for other rigs; this imported loader requires full lift (1) for dumping.
3. **Wrong colours after joining:** meshes/curves had differently named UV layers,
   so palette coordinates could reach UV1 while UV0 was blank, making tyres/bucket
   yellow. Export now gives every mesh exactly one consistently named `PaletteUV`
   layer before joining. Unity colours were inspected after correction.
4. **Geometry inventory on labels:** some MeshRenderers have no MeshFilter. Explicit
   Unity-object null checks replaced an unsafe nullable access that threw on Label.
5. **Catalog reimport overwriting wheels:** bootstrap was replacing an existing
   prototype wheel entry's radius/mass from the new active tractor. Existing wheel
   definitions are now preserved rather than silently rewritten on reimport.

### 9.3 User-reported bucket/tyre clipping and inverted grips

The user reported that the loader's bucket/blade crossed the tyres and that tyre
grips looked like recesses. These were real geometry problems, beyond the initial
work/carry/dump floor tests.

The prepared loader boom was extended **0.8 m** along its reach, with its mesh
stretched to match the new pin. Working boom pitch changed **18 -> 9 degrees** and
the socket counter-rotation was updated to keep the bucket floor orientation.
Prepared source coordinates now put the bucket pin at X2.81/Z1.03 and cutting edge
at X4.01/Z0.567; original source pin/edge were X2.01 and X3.21. Bucket-local shape,
working/load offsets and width remain unchanged. Catalog body, equipped prefab,
equipment COM and scene were updated in Edit mode.

A conservative tyre envelope allows for steering using tyre radius and half-width.
The bucket rear was checked in **441 lift/tilt samples**. Minimum longitudinal gap
was **0.239 m**, against a required 0.08 m margin. Work/carry/dump shell/edge ground
clearance also passed. This is not a continuous dynamic collision/physics test.

All loader tread blocks had inward-facing surfaces; tractor chevrons had mixed
winding. Prepared copies now recalculate outward normals and assert each closed
block's faces point outward. Counts: **66 blocks per loader wheel**, **40 per rear
tractor wheel**, **32 per front tractor wheel**. Loader tips already reached 0.73 m
over a 0.68 m tyre carcass: the fix was orientation, not extra bump geometry.
`TreadValidation.json` records the export checks. Both original `.blend` files stay
intact and triangle counts did not increase. Corrected loader appearance was
inspected in Unity.

## 10. Opaque bright toon windows: latest visual decision

On 28 October the user supplied a reference image with bright cyan windows and
pale diagonal reflection streaks, asking to replace transparent glass for performance.
Both imported vehicles now use that direction.

- Shader: `Assets/Snowglow/Shaders/ToonVehicleWindows.shader`, named
  `Snowglow/Toon Vehicle Windows`.
- Editor authoring: `ToonWindowsAuthoring`, including future intake material setup.
- Existing material GUIDs preserved in place:
  `Catalog/Sources/LowPolySnowPlower/Materials/Color09.mat` and
  `Catalog/Sources/LowPolySnowLoader/Glass.mat`.
- The legacy filename `Glass.mat` does **not** imply current transparency.
- Opaque geometry queue 2000, alpha one, depth writes, no transparent keyword.
- Unlit bright cyan and pale painted stripes, projected in object space. This works
  on the combined meshes without new UVs, textures, reflection probes or triangles.
- Both sides of the imported single-plane windows remain visible.

This removes transparent window blending/sorting and avoids lighting/reflection
sampling for the windows. It does not prove a measured FPS improvement. Shader
compilation and opaque state/bindings passed in the Editor with Android selected;
the tractor's cyan finish/highlights were visually inspected. Android player shader
build and phone frame-time benefit remain untested.

### 10.1 Soft matte snow material: next requested task

On 28 October the user listed two tasks: change the snow material, then replace one
essential distinct scenery item with a finished model (home, pole, tree, mountain,
etc.). They explicitly asked to focus **only on the material**. They selected the
recommended off-white/icy-blue matte toon option over flat white or textured grain.

`SnowMaterialAuthoring` updates the existing shared snow/terrain materials in place.
`SoftToonSnow.shader` and `SoftSnowLighting.hlsl` provide a soft main-light/shadow
ramp without specular, reflection probes, detail textures, displacement or new
geometry for snow meshes. Terrain retains supported URP Terrain/Lit and patch
instancing; its existing layer has off-white diffuse remap and zero metallic/
smoothness. An experimental custom terrain shader failed to draw during visual
review and was removed. The authoring command expects one snow terrain layer.

The user then supplied a mountain reference and requested pure-white tops. Current
starting colors are `#FFFFFF` / `#BFE7EE`, with an upward-normal ramp ensuring white
mesh tops and pale cyan shaded sides. Terrain layer tint is now white. The command is
`Snowglow > Art > Apply soft matte toon snow`; future district bakes use it too.
Material identity, snow seed/hash, bindings and terrain instancing are checked;
reports go to `Logs/SoftSnowMaterial.txt`. No new scenery model or snow-physics
change belongs to this task. Phone performance/Android shader build remain untested.

## 11. Mobile geometry and rendering budget

These are project targets, **not universal mobile hardware limits**. Profile the
whole scene, not isolated Blender edit-mode vertex counts.

| Asset inventory | Imported vertices | Triangles | Renderers | Material slots |
| --- | ---: | ---: | ---: | ---: |
| Tractor equipped prefab | 13,528 | 7,274 | 7 | 30 |
| Loader including inactive carried-snow visual | 15,095 | 8,424 | 8 | 9 |
| Loader evaluated vehicle alone in Blender | 4,740 | 7,656 | 7 modules | 8 |

Counts exclude particle geometry. The loader uses three distinct materials including
carried snow; tractor uses eight. Hard normals/UV splits explain why imported Unity
vertices exceed Blender vertices. Slots are potential submissions per pass, not
measured draw calls. SRP batching reduces setup cost; it does not collapse every
slot into one universal draw.

Working recommendations:

- Complete nearby machine: aim **5,000-10,000 triangles**; review above **15,000** or
  **30,000 imported vertices**. Catalog vehicle warnings now use those thresholds.
- Small prop: roughly 50-1,000 triangles; house assembly: roughly 500-3,000, review
  around 5,000 depending on repetition and screen size.
- Prefer 1-4 shared materials, about 6-10 vehicle renderers, and fewer than about
  10-12 total material slots. Existing warnings flag >12 slots.
- Initial visible geometry budget: approximately 100,000-200,000 triangles, while
  accounting separately for terrain, shadows, UI, particles and overdraw.
- Avoid tiny unseen bevels/bolts. Combine rigid parts during authoring, retain
  moving parts, use culling/LOD where needed, and keep transparency restrained.

Both machines fit the proposed triangle band; triangle decimation is not currently
the priority. **The tractor's 30 material slots are a clear next optimization target**
for a shared palette, while preserving its appearance and corrected tread winding.

One saved depot-camera bounds estimate found 69,770 candidate mesh triangles,
54,432 from snow, and 69 material slots. It excluded Terrain/UI/particles/shadow
passes and did not account for batching/occlusion. Do not report it as an actual
GPU frame measurement or general worst-case district budget.

## 12. Validation evidence and how to continue it

### 12.1 Evidence actually recorded

| Stage | Verified evidence | What it does not establish |
| --- | --- | --- |
| Initial Unity repair/bake | Runtime/Editor compilation, persisted district, scene reopen and original 9 snow checks | Comfortable handling, rendered quality or phone performance |
| 50 cm resolution | Unity rebuild/reopen and 12 snow checks | Sustained cost of finer cells |
| Cohesive spreading | 19 engine-independent checks under Mono and Unity | Realistic live feel or frame times |
| Snow file split | Reopened references, same volume, 19 checks and files below budget | Git push success in every clone |
| Asset Workshop | 39 actual Editor checks, prefab/layout persistence, cleanup, Pinefall and 19 snow checks | Full workshop UI/drag UX or phone-ready art |
| Starter tractor | Save ID/price/ownership, wheel sizes, controller initialization, work/angle/drive/park commands | End-to-end live driving and source hydraulic fidelity |
| Loader integration | Persistent rig, compatible bucket, identity/price, carry/dump/load display, activation, scene reopen | Full driving/loading or actual bucket capacity measurement |
| Clearance/treads | 441 poses, 0.239 m minimum gap, outward block checks, unchanged triangles, Unity inspection | Continuous collision/suspension test |
| Toon windows | Shader/Editor compile, opaque state, shared saved bindings, Pinefall validation and tractor visual inspection | Android player shader compilation or phone speedup |

No new tests were run merely to write this handoff; it consolidates existing evidence.
Historical documentation contains stale statements that no screenshot was possible.
Later Blender renders and Unity visual inspections did happen; sustained driving,
audio playback, touch/lifecycle and device profiling are still separate gaps.

Similarly, older records say no APK was produced by the agent. Git includes a user
commit named `604d0a8 upload initial app bundle for test on playstore`. Do not infer
that no build has ever existed, or claim that this proves the current imported
vehicles/toon shader were packaged and tested. The current feature set's Android
player build, deployment and A07 measurements are not established by this chat.

### 12.2 Commands and reports

| Editor menu | Use / report |
| --- | --- |
| `Snowglow > Validate saved district and snow ledger` | Saved scene/rig/assets plus snow checks; `Logs/SceneValidation.txt` |
| `Snowglow > Run snow ledger checks` | `MechanicsChecks` / authoritative volume invariants |
| `Snowglow > Run asset workshop checks` | 39 checks, disposable fixture cleanup; `Logs/AssetWorkshopChecks.txt` |
| `Snowglow > Rebuild snow at 50 cm resolution` | Snow-only reauthoring, preserve scenery/fleet |
| `Snowglow > Bake editable test district` | Intentional full layout replacement, new generated folder |
| `Snowglow > Asset Workshop` | Prepare, catalogue, assembly and AI-plan workflows |
| `Snowglow > Asset intake > Open tractor review prefab` | Unequipped/Editor review output, not the starter gameplay prefab |
| `Snowglow > Asset intake > Use tractor as free starter plow` | Explicit Fleet[0] authoring; `Logs/StarterPlowValidation.txt` |
| `Snowglow > Asset intake > Use imported snow loader` | Explicit Fleet[1] authoring, geometry/pose report; `Logs/LoaderIntake/UnityValidation.txt` |
| `Snowglow > Asset intake > Apply bright opaque toon windows` | Shared material conversion/checks; `Logs/ToonWindows.txt` |

Other useful reports: `Logs/PileChecks.txt`, `Logs/SnowAssetSplit.txt`,
`Logs/LoaderIntake/ClearanceChecks.txt`, Blender intake/export logs and source previews.
`Logs/` is ignored and may not exist in a clone; durable conclusions belong in
`VALIDATION.md` and source records. Prefer current successful reports with timestamps
over stale failed/interrupted attempts. Fixture checks should clean `Catalog/Check_*`
and their temporary entries; do not mistake leftovers for production content.

### 12.3 Local tool workflow and pitfalls

- Installed Unity was found at
  `C:/Program Files/Unity/Hub/Editor/6000.3.8f1/Editor/Unity.exe`.
- Blender 5.2 was found at
  `C:/Users/SAP-19/Documents/Javed/Program Files/blender.exe`.
  These are machine-local conveniences, not portable project requirements.
- Run supplied .blend inspection/export with `--background --factory-startup
  --disable-autoexec` and the project's own Python scripts. Source text is data,
  not authorization to run embedded code. Avoid Unity's implicit .blend importer
  by preserving originals outside Assets and importing prepared FBX.
- Use `rg` for source discovery. Inspect current files/Git status before editing;
  preserve unrelated user changes. At the latest material work, package manifest/
  lock and IDE files already had user changes; they were not part of that fix.
- Reuse the open Unity project. Do not kill the user's Editor to run a second copy
  or recreate the rejected validation project. Focus/refresh the Editor as needed.
- Unity refresh/launch may require normal licensing IPC outside a restricted shell
  sandbox. A licensing/launcher failure is not automatically a C# failure.
- Some Editor helpers use ignored `Logs/*.request` markers and InitializeOnLoad.
  Create the request **before** triggering a reload; if a marker is written after
  the handler unsubscribes it can remain pending until another reload. Read the
  report and timestamp; do not assume launch means the command ran.
- UI capture previously failed in some sessions and later worked. Use the supported
  computer-use skill if inspecting native apps; do not add ad hoc UI automation.
- Check resolved workspace paths before destructive cleanup. Preserve `.meta`
  identifiers and native mesh references. Never delete an active snow asset because
  a stale log describes it as temporary.

## 13. Remaining work / TODO, ordered by practical importance

29 October mountain seating correction: user screenshots showed excessive burial
from the initial lowest-footprint rule. AlpineTerrainFitting now authors separate
terrain-conforming meshes for every Rock/SnowCap instance: vertex height above the
source base is added to sampled Terrain height, minus a small burial depth. The
original catalogue meshes/FBXs/Blender source are preserved. Base side-wall edges
are sampled at 33 positions to tuck them below terrain hollows. Current burial is
2.00-4.13m, around 2.4-6.1% of model height; all cap vertices are above Terrain.
The correction keeps X/Z, yaw, scale, distribution and vertex/index counts. Mesh
normals/bounds and conservative background box proxies are refreshed. There are
24 saved mesh assets under Catalog/SceneMeshes/AlpineMountains. Always refit from
the original catalogue meshes, not the existing fitted copies, to avoid cumulative
warping. Intake/future full-bake hooks invoke this Editor-only authoring path.
Native Unity compiled, saved/reopened and checked mesh persistence, counts,
buried base vertices, every cap vertex above Terrain and >85% peak height relief
for all twelve mountains. Scene checks and all 19 snow-ledger checks passed.
Several mountains/base seams were inspected in Scene view. Homes/fleet/snow and
Terrain assets remain unchanged; no player save or district rebuild. Reports:
Logs/AlpineMountains/TerrainFit.txt and TerrainFitValidation.txt. Scene backup:
Logs/AlpineMountains/Pinefall-before-terrain-fit.unity. Full perimeter/collision
walkthrough and current Android/A07 rendering/performance remain untested.
Earlier lowest-footprint placement notes below are historical and superseded.


29 October alpine mountains: user supplied Five_Alpine_Mountains.blend and asked to
replace all primitive mountains at their existing locations with shuffled imported
variants, each used. Original is preserved under SourceAssets/FiveAlpineMountains,
SHA256 382B0050FB4E9ACB40D17B4904C4B35950DCF2400A42D0F5863F47574CFCBF73.
No embedded text was executed. Five collections: Needle Peak, Twin Summits,
Sawtooth Ridge, Glacier Massif, Alpine Cirque. Ten closed rock/snow mesh solids
passed outward-normal checks; cameras/labels/lights/display ground are excluded.
Separate Rock/SnowCap meshes use shared opaque rock palette and current white/cyan
toon snow. Stable catalogue IDs alpine-mountain-01..05 remain Needs Review.
Triangles/Unity vertices: 690/1524, 726/1601, 1016/2380, 930/2041, 974/2247;
two renderers/slots each. All 12 saved placements total 10,088 triangles before
shadow passes. Background box proxies are conservative, not climbable surfaces.

AlpineMountainsIntake compiled in native Unity 6000.3.8f1 / Android, prepared five
prefabs, replaced all 12 primitive spheres/caps, saved/reopened Pinefall and passed
scene checks and 19 ledger checks. Balanced shuffle seed 291029 gives first two
variants three uses each and remaining variants two. X/Z positions and old width/
depth envelopes are retained; random yaw/uniform scaling preserve shape proportions.
Bases are seated two metres below the lowest 9x9 terrain sample in each footprint.
Cap-vertex readback confirmed every placement emerges 18.97-101.11 m above terrain.
Repeated intake preserved the saved layout. Five Unity previews and one Scene-view
placement were visually inspected. Home IDs/assets/transforms, fleet, seed and
existing terrain/generated assets were preserved; no player save was loaded or
written. No full district rebake; future bakes call PlacePreparedMountains after
DistrictFactory builds. Entry preparation/placement is Editor-only, never in Play.
Report: Logs/AlpineMountains/UnityIntake.txt. Source pipeline/readme and part maps:
SourceAssets/FiveAlpineMountains and Catalog/Sources/FiveAlpineMountains. Original
scene backup: Logs/AlpineMountains/Pinefall-before.unity. Full seam/sightline and
collision walkthrough, Android packaging/appearance and A07 profiling remain TODO.


29 October all-house replacement: user requested replacing every prototype house
with randomly distributed imported variants, each used at least once, and making
houses 1.5x bigger. All 16 lots are now converted. A balanced bag shuffled with
Editor seed 291026 gives Red Cottage/Green Porch three placements each and the
other five variants two each. The layout is saved, not randomized during Play.
Scale is 1.5x the former per-lot fit calculation. Foundation X/Z expand 1.5x while
height/ground level and fences remain unchanged. Existing seven placement IDs are
preserved; the other nine receive unique IDs. The 1.5x layout marker makes repeat
intake preserve the saved layout without multiplying scale again. Future full
bakes use the same all-lot authoring path. Catalog placement maximum scale is 1.5.
Unity 6000.3.8f1 / Android compiled, saved/reopened all 16 homes, passed no-house/
road or house/house footprint-overlap preflight, saved-scene structural validation
and all 19 ledger checks. Independent saved-YAML checks confirm 16 unique IDs,
zero primitive cottage groups and exactly 1.5x horizontal foundation scaling.
Scene-view district overview was inspected. Snow/terrain/fleet assets and player
saves were not rebuilt or modified. Live vehicle/house clearance, enlarged-house
snow interaction, Android appearance and sustained Galaxy A07 timings remain
untested. Report: Logs/WinterHomes/UnityIntake.txt. Pre-change scene backup:
Logs/WinterHomes/Pinefall-before-all-homes.unity. Earlier seven-lot notes below
record the first intake and are superseded by this layout.


28-29 October scenery intake: the user supplied `Seven_Winter_Homes.blend` with
seven named collections and authorized importing the entire set. The original and
preserved copy under `SourceAssets/SevenWinterHomes` match SHA256
`2AD3BEC380E5EE05A400561C0E9CA5445607C4D389EBFBF46EF4E4DECB9F8CDE`.
Embedded text was never executed. All seven source views were inspected. Exports
combine architecture, opaque warm windows, roof snow and ground drifts into four
mesh groups, sharing three materials across the set. Display tiles, text, cameras
and lights are excluded; prepared drift bases are clipped/capped at ground level.
Snow is removable under SnowCover and remains outside the authoritative plow ledger.

After the usage-limit interruption, the existing Editor was refreshed and the
native import failure was diagnosed: six exports have eight zero-area architecture
triangles each. Blender round-trip inspection confirmed them; export metadata now
records them and the Unity assertion checks the remaining triangle count exactly.
No arbitrary tolerance or original-model edits were needed. Prepared prefabs have
1,002-2,848 triangles / 2,078-5,820 imported vertices each. See the per-house table in
`MOBILE_ASSET_BUDGET.md`; geometry is within project targets, not phone profiling.

Unity 6000.3.8f1 / Android compiled, prepared seven catalog entries/prefabs with
stable IDs winter-home-01 through winter-home-07, fitted one of each to existing
foundations nearest the depot, saved and reopened Pinefall, and passed structural
validation and all 19 snow-ledger checks. Foundations/fences, seed and fleet were
preserved; no district/snow rebake or player-save modification occurred. Compound
wall/step/deck proxies and entrance anchors are proposals for review. The intake
is repeat-safe; future full bakes reuse the prepared entries. Native Unity previews
were rendered; their key light was corrected for facade review and the installed
cottage was inspected in Scene view. Catalog status remains Needs Review.
`Logs/WinterHomes/UnityIntake.txt` records native results. Live driving clearance,
full lot/collider walkthrough, Android packaging/appearance and Galaxy A07 frame
timings remain untested. See `SourceAssets/SevenWinterHomes/README.md` and export
part/metrics JSON files. The pre-intake scene backup is under ignored
`Logs/WinterHomes/Pinefall-before.unity`.


### Wall winding correction - 29 October 2026

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

28 October control follow-up: user reported dead keyboard keys and a full loader
bucket staying down. `PrototypeInput` now disables UI navigation/submit while
driving (arrow/Space conflict) and restores it in menus. The full keyboard outage
is not reproduced; Editor/build context awaits the user. `AttachmentController`
checks Auto full-bucket carry outside collection, preserves higher lift/dump,
accepts manual lift/tilt immediately and re-enables loading when manually lowered.
Runtime/Editor C# compile passed using Unity's compiler; native Editor execution
is pending refresh. `DrivingControlsChecks` has a queued request; require a fresh
`Logs/DrivingControls.txt` PASS before claiming fixture or keyboard event checks
ran. No authoring rebake, player-save modification or live/device pass occurred.

### A. Prove the current machinery and snow on device

- [ ] Build the current scene and toon shader for Android; record build settings,
      graphics API and exact artifact rather than relying on an older bundle.
- [ ] Record Galaxy A07 RAM variant, Android version, quality mode and test duration.
- [ ] Drive for at least 15 minutes with plowing, substantial piles, loading,
      carrying/dumping, camera switching, UI transitions and saving. Record CPU/GPU,
      physics, mesh/collider cooking, memory, frame-time spikes, temperature and heat.
- [ ] Test other lower-end/older representative phones before claiming broad support.
- [ ] Tune spring/damper, grip, torque, braking, COM, assistance and snow resistance
      with empty and full bucket, asphalt/ice, slopes/curbs and deep snow.
- [ ] Verify no bucket/tyre or bucket/body clipping during continuous manual operation,
      steering, suspension travel and uneven-ground driving. Preserve the 441-pose
      regression check; add meaningful cases when new geometry changes demand them.
- [ ] Test touch multi-input, safe areas/aspect ratios, pause/resume and backgrounding.
- [ ] Expand the current debug overlay/resettable test tools where needed: the
      planning called for suspension/contact, available torque, snow depth/budgets,
      blade state, load and ledger diagnostics; not every proposed metric/tool exists.
- [ ] Check sounds on actual hardware and save/restore a loaded bucket/displaced piles.
- [ ] Verify transaction retries, fresh contracts, abandonment, switching and recovery
      without duplicated rewards, lost snow, incorrect ownership or lost upgrades.

### B. Improve art/rig readiness without losing the prototype

- [ ] Consolidate tractor opaque colours/material slots, retaining modular wheels,
      current cyan windows, source provenance and repaired normals.
- [ ] Review imported collision proxies and complete movement envelopes; validate
      intentional edits to both catalog bodies and equipped outputs.
- [ ] Author a believable tractor lift/yaw linkage and loader ram/hose articulation
      if increased fidelity is desired. Do not advertise current visuals as simulated
      hydraulics or articulated steering.
- [ ] Confirm proposed physical ratings/capacity through tuning; mass numbers are
      not manufacturer measurements.
- [ ] Extend wheel upgrades to axle-specific sets before offering uniform replacements
      on the tractor's different front/rear wheels.
- [ ] Review/re-publish catalog entries and fittings explicitly when ready; current
      gameplay installation did not grant blanket Ready status.
- [ ] Manually test the workshop's complete drag/drop UX and more varied production
      models; the existing acceptance suite is mostly programmatic Editor testing.
- [ ] Continue replacing environment primitives via reusable prepared assemblies,
      preserving terrain, snow, colliders, entrances, roads and gameplay anchors.

### C. Complete scalable content and player equipment systems

- [ ] Connect the general catalog to player garage/shop/loadout saving and compatibility
      explanations; currently only fixed fleet purchases and simple upgrades exist.
- [ ] Extract economy, contract/progression and district services from the prototype
      coordinator before broad expansion. Keep definitions immutable and IDs stable.
- [ ] Turn AI layouts into complete authored districts by explicitly adding contracts,
      snow coverage, session systems, service/disposal zones and fleet bindings.
- [ ] Add a second district to validate reusable configuration and save IDs before a
      world-map campaign. Then implement regional unlocks, reputation/medals and modes.
- [ ] Implement genuinely different tracked/blower locomotion/equipment behaviors
      rather than accepting an interface string without a functional rig.
- [ ] Add alternative blades/buckets/adapters/ballast/hydraulics only with measurable
      physical/working effects and appropriate clearance/load rules.
- [ ] Consider accumulation, compaction, melt or more advanced snow only after the
      current volume, obstruction and device budgets are proven.
- [ ] Add real platform billing/cloud/ads only as a deliberate production task with
      appropriate verification; prototype purchases remain simulated.

### D. Keep documentation and historical evidence honest

- [ ] Reconcile older README/ARCHITECTURE/VALIDATION paragraphs that still describe
      only primitive machinery, nine checks, no visual captures or no build ever.
      Preserve their historical context instead of treating them as current blockers.
- [ ] Keep current counts/IDs/material style and validation limits up to date when
      assets change. Update this handoff after meaningful future phases.
- [ ] Do not promote old rounded-snow/optimization logs into current behavior without
      inspecting the live renderer and reproducing the relevant checks.

## 14. Useful history anchors

Inspected Git history includes:

- `8bee6cd`: first project commit.
- `aeab7d9`: PC reverse-key assignment.
- `604d0a8`: user commit labelled initial app bundle/Play Store test.
- `9ab20d0`: snow shape/resolution/spreading work and Git-friendly split history.
- `a4e261c`: Editor asset-preparation phase.
- `e093b7b`: imported snow-plower tractor.
- `35fa499`: imported snow loader.

These are historical anchors, not a request to reset, amend, merge or push anything.
Check the current working tree and branch before using them. The latest window/
material work and this handoff may not yet be committed.

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

Latest user direction supersedes earlier four-mode/scenic descriptions:
gameplay cycles Isometric -> Follow -> Orbit only. Scenic is reserved for a
future static scene preview behind the main menu, not a revolving gameplay mode;
that future menu feature is not implemented by this change. Legacy serialized
Scenic mode values migrate to Isometric without renumbering saved enum values.

Isometric starts closer (11 m orthographic half-height) and smoothly widens to
18 m at 8 m/s in either driving direction, then closes as speed falls. Follow
smoothly arcs from behind to in front of the machine when actual Rigidbody
forward-axis velocity passes -0.35 m/s; forward travel above +0.35 m/s returns
it behind. Near-zero velocity retains direction, avoiding braking/stop jitter.
Speed framing and directional state freeze while paused. Keep steering semantics,
obstruction checks, terrain/near-plane clearance and saved Editor bindings.
These are initial camera tuning values; phone comfort still needs evaluation.
See CAMERA_MODES.md and the latest native Logs/CameraModes/Validation.txt.


Isometric framing adjustment (31 October 2026): user requested about 2x closer. Halved the saved scene and default orthographic size range from 11-18 m to 5.5-9 m; speed zoom timing/range in speed units is unchanged. Updated the existing speed-zoom assertion threshold for the smaller range. Source/serialized-value inspection only for this tuning edit; no new Unity render or phone playtest.


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

Orbit UI simplification (1 November): remove on-screen Zoom In/Out buttons and retain only VIEW RESET, anchored to the left controls group at the 5% safe-area inset. Mouse wheel, keyboard +/- and two-finger pinch remain supported. Future HUD/camera authoring follows this layout.


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
