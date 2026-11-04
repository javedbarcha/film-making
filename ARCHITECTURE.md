# Prototype structure and extension boundaries

`Editor/SelectedMountainRefit` is a selection-only authoring command separate
from whole-set mountain intake. It precomputes source-derived meshes, preserves
horizontal transforms, fits the lower skirt and creates persistent mesh revisions
before assigning them in one Undo group with root seating and collider updates.
Upper peaks remain source-shaped. Old mesh assets remain intact for Undo and
existing scenes. Scene saving stays with the author; no runtime component,
district rebuild, Terrain mutation or snow-mask update is introduced. Selections
inside the interactive snow field are rejected to keep that mask consistent.

## Asset Workshop phase

`CatalogAsset`/`AssetCatalog` store stable content IDs, provenance, approval state, measured bounds, constraints and prefab references. `AssetAnchor`, `AssetSocket` and `PreparedVehicleRig` describe authored connection and physical bindings. `FittingProfile` and `AssemblyRecipe` keep mounting adjustments and loadout composition separate from imported visuals. The importer preserves originals and creates reviewable prefab copies with suggested proxies/rigs; Ready approval includes a dependency/physics fingerprint.

All construction remains under Editor code: `CatalogPreparation`, `CatalogAssembly`, `CatalogBootstrap` and `CatalogScenePlans`. The latter exports a Ready-only manifest and resolves declarative JSON before writing a new layout scene. It does not execute model generators, generate runtime objects, replace existing contracts, or automatically register new machines in the current session fleet. `AssetWorkshopWindow` exposes the workflows; `CatalogChecks` exercises preparation, fitting, approval invalidation and scene persistence with disposable fixtures. See ASSET_WORKSHOP.md.

Vehicle definitions now optionally provide base grip and dry centre of mass, with defaults matching the original machines. Assembly baking derives these from approved parts. Attachment controllers optionally retain authored mount/boom/bucket rest transforms; the legacy primitive rigs retain their existing behavior.

## Authored content lifetime

`Editor/SnowMeshAssets` persists visual/collision meshes in native mesh-part files, grouping eight snow chunks per part. The SnowSeed stores only volume data. Scene MeshFilter/MeshCollider references point to those saved mesh assets. Both full district baking and snow-only rebuilding use this writer, which rejects snow files at or above 90 MB. Splitting an existing bundled seed keeps its GUID and snow data, saves new mesh references before removing old subassets, and validates the reopened scene.

`Editor/PrototypeAuthoring` orchestrates `DistrictFactory`, `MachineFactory` and `HUDFactory` in Edit mode. It writes native assets and the populated Pinefall scene. Generated objects are not recreated by runtime callbacks. Re-running the command intentionally replaces the scene and creates a fresh timestamped asset folder; earlier folders are retained.

`VehicleDefinition`, `AttachmentDefinition`, `PrototypePalette` and `SnowSeed` are immutable authored inputs. Scene references point directly to saved machinery, controls, camera, audio and snow chunks. Wheel/mount/body visual anchors are separate from chassis and wheel physics.

`Editor/SceneValidation` checks persisted machine rigs, attachment compatibility, snow seed/chunk assets, terrain, shared materials, URP and build-scene inclusion. Its menu command reopens the saved scene and runs the nine snow ledger invariants. Baking checks the scene before writing the ready marker and treats scene-save failure as an error. First-import baking is disabled in batch mode so validation never silently regenerates content.

## Runtime mechanics

`VehicleOperatingLights` reads the selected machine's operating/session state and
updates Editor-authored beam/lamp bindings. It creates no runtime objects or
materials. `VehicleLightsAuthoring` prepares source-derived lamp surfaces and two
shadowless spots per equipped prefab; future machine builds reuse those bindings.
The mobile URP additional-light budget is two per object. Lamp surfaces use shared
opaque unlit materials; bloom/day-night lighting remains separate future work.
The paired-lamp revision enforces one material/submesh per group, switches rear
running lamps to white in reverse, and animates authored red beacon rotors with
an opaque sweep shader plus one small additive camera-facing halo per machine.
Runtime uses reused material property blocks; no per-frame material copies or
new light objects are created. Beacon animation stops with operating lights off.

- `MachineController`: Rigidbody/WheelCollider movement, surface-dependent tyre friction, assistance, fuel, condition and carried-load mass/center of mass.
- `AttachmentController`: blade/bucket state, Auto/Manual input, working-edge queries, processing budget, carried volume and dumping.
- `SnowVolumeGrid`: engine-independent, authoritative volume and original-work-credit accounting. It refuses full/out-of-bounds/static-obstacle deposition.
- `SnowField`: field queries, chunk visual/collision refresh, target accessibility and disposal processing. Baked assets are cloned into transient mutable meshes so Play does not overwrite source assets.
- `PrototypeInput` / `TouchControl`: keyboard and multi-pointer touch input. Camera modes do not change steering interpretation.
- `CameraRig`, `SynthAudio`, `PrototypeHUD`, `UICommand`: presentation and player commands.
- `PrototypeSession`: small prototype coordinator for contracts, fleet selection, economy, XP, settings and pause states.
- `SaveStore`: versioned snapshots, safe file replacement and backup load.

The prototype coordinator intentionally keeps the first district compact. Before adding multiple regions or production billing, extract the economy, contract and progression sections into dedicated services while retaining stable definition IDs and the existing volume model. Do not add content by extending conditional chains indefinitely.

## Compatibility contract

Vehicles declare mount identity, hydraulics, permitted equipment mass and working width. Equipment declares requirements. Bucket compatibility includes rated full snow mass at 250 kg/m³. The two rigs are explicitly authored; matching strings alone does not create a new mechanical rig. Future blower/tracked implementations must supply the locomotion/attachment behavior and a fitted visual rig before compatibility is enabled.

## Expansion order

1. Prove/tune the two supplied machines on the phone.
2. Add authored fitting profiles and a genuine equipment inventory when introducing alternative attachments.
3. Extract contract/economy/progression services; add district and region content definitions.
4. Add a second district to validate content boundaries and stable save IDs.
5. Implement tracked locomotion or blower processing as distinct behaviors.
6. Introduce platform services/real purchasing behind tested transaction boundaries.

The primitive presentation can be replaced throughout this sequence. Preserve metre scale, Y-up/Z-forward axes, wheel pivots, attachment mounts, working-edge anchors and collision clearances.


Street-light intake preserves original mounting origins, separate decorative snow
and opaque luminous mesh groups. Optional shadowless point lights are saved under
inactive prefab groups and enabled deliberately in the Editor. StreetLightsIntake
places catalog prefabs before Play, refreshing only a scene-specific snow obstacle
mask while preserving volumes and original seeds. TownBlockAuthoring extends a
newer scene-specific seed rather than reverting another intake's obstacle mask.


AlpineTreesIntake replaces only saved primitive forest roots with prepared catalog
prefabs. It retains horizontal positions/scales and existing DistanceVisibility;
Editor-authored foot-probe seating and terrain-normal orientation for logs/stumps
preserve shared meshes. Decorative caps remain separate from the snow ledger. Snow
obstacle flags are considered only near new trunk proxies; no-change intake retains
the current seed reference rather than creating another asset. No tree factories
or new runtime rendering behavior are added.


CameraRig now cycles Isometric, Follow, Orbit and Scenic on the same authored Camera.
It owns mouse/touch orbit input, UI exclusion, session-only angle/zoom and terrain/
near-plane constraints. Sphere sweeps use world/pile layers and exclude the active
vehicle; clearance is enforced after smoothing too. Scenic auto yaw pauses under
manual input or gameplay menus. CameraModesAuthoring adds persistent zoom/reset/help
UI to existing Pinefall; no runtime camera/UI construction or player-save change.
CameraModesChecks uses disposable Editor fixtures, synthetic player-style Input
System updates and a rendered UI fixture; its temporary package hook is restored.


AlpineTerrainFitting converts fitted mesh-bound corners directly into each
collision proxy's local space; CatalogValidation.BoundsOf returns root-local
bounds and must not be interpreted as world coordinates. Scene proxy repair
updates only affected snow obstacle flags in a new seed copy while preserving
depth/credit/accounting and saved scenery. Runtime save restoration already
uses the authored seed mask.

LowestTreeLODIntake replaces only mesh references inside the 26 existing alpine
forest prefabs, retaining their IDs/GUIDs, hierarchy, collision proxies and culling.
Source LOD3 exports use the same mounting basis and palette. All 97 saved forest
instances therefore use one fixed lowest-detail mesh set, without LODGroup or
runtime switching. Legacy tree-intake installation forwards to this intake while
the lowest-detail entries are active. No scene regeneration or snow changes.

Forest density/seating authoring (4 November 2026): `ForestSeatingAuthoring` is an
Editor-only, repeat-safe repair of the saved Pinefall forest (146 LOD3 instances).
It samples actual mesh support geometry and visible vertices against Terrain,
excludes complete mountain horizontal bounds, and saves placements before Play.
Only affected snow obstacle flags change in a separate seed copy; authoritative
snow depth, credit and volume remain unchanged. No runtime forest spawning.

Visible-forest revision: forest placement is bounded to 280 m around the district
and rejected if any center/corner district-facing segment crosses a mountain
horizontal envelope (3 m margin). Tree terrain-normal tilt is capped at 12 degrees;
wood takes full terrain alignment. At least 34 trees and six wood pieces have
5-degree-or-steeper slope sites. Support and visible-vertex seating remain checked.

The marked outer-boundary revision supersedes the 280 m cap: forest full bounds
now fit within 470 m, with half the existing instances assigned beyond 340 m.
Mountain-facing exclusion and all slope/support rules remain; count and meshes
are unchanged. Native overview renders temporarily disable fog and restore it
before reopening the saved scene; gameplay fog is unchanged.

SignBoardsIntake prepares 12 Blender sign variants through CatalogPreparation and
saves 16 Pinefall placements before Play, preserving other scene work. Body and
evaluated FONT artwork use a shared opaque palette; snow is separate. Variant 06
has a wall mount; blank replacement boards have authored destination TextMesh
children. Only nearby snow obstacle flags change in a separate seed copy. Runtime
does not build signs, fonts or colliders. Repeat intake retains scene placements.

Latest sign curation supersedes all-variant scene placement: SignWayfindingAuthoring
authors 31 approved-purpose signs through SignBoardsIntake (seven destination/service
boards, twelve outer road-end closures, twelve inward town-boundary warnings).
A versioned scene marker prevents repeated imports from recreating removed types.
Road/junction readers, real destination targets and boundary-ring checks validate
facing and arrows. Caption overrides leave catalog/source lettering untouched.
Moved/deleted footprints are reconciled against authored scene colliders in a
separate obstacle-mask seed; snow depth/credit/volume remain unchanged.

`SnowDisposalYardIntake` prepares the supplied yard as a reusable building,
replaces only the primitive yard/pad in the Editor, and revises obstacle flags
without changing snow quantities or the existing runtime processing rectangle.
PrototypeAuthoring reuses its prepared prefab on future explicit district bakes.
