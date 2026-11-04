# Snowglow Asset Workshop

Open **Snowglow > Asset Workshop**. This phase adds Editor tools and reusable content; it does not replace Pinefall, add runtime spawning, or change the current garage screens.

## Catalog supplied with the project

`Assets/Snowglow/Catalog/SnowglowCatalog.asset` contains prototype vehicle bodies, matching blade/bucket, a wheel set, cottage/fence lot, tree, depot and reusable road. They are separate prefab copies. All start **Needs Review**, because structural checks do not establish visual fit or mobile performance.

Catalog entries have stable IDs, categories, source/provenance notes, dimensions, geometry/material counts, constraints and preparation status. Drag a row into the Scene view or Hierarchy to place its prefab. Search covers names, categories and tags. Draft/manual placement is useful for review; AI scene plans accept only approved Ready entries.

## Prepare an imported model

1. Import the model and its textures into the Unity Project window. FBX/OBJ work through Unity's installed importers; other formats need an appropriate importer or conversion in Blender. A `.blend` file also depends on Unity's Blender import setup. The workshop accepts an imported GameObject, prefab or scene assembly, rather than executing an external file.
2. In **Prepare asset**, drag the model into the drop area. Select the category, title, source type, uniform scale and orientation correction.
3. Add provenance and assumptions. A reference image, generator script or other project asset can be recorded in the source-document field. For prompt/image/script work without a model yet, use **Create source-only draft**. Select that Draft and prepare its model later to retain the same entry ID.
4. Choose **Prepare separate game-ready prefab**. The original model, import settings and source materials are preserved. The tool creates a wrapper prefab, normalizes its root, centres the geometry on the ground (or on the axle for wheels), proposes collision/physics setup and copies Standard/legacy materials to URP Simple Lit. Inspect material conversion fidelity.
5. Review the entry and open its prefab. Adjust collision proxies and map missing anchors. Edit its associated vehicle/equipment definition in the Inspector as needed. The report distinguishes structural errors from review/performance concerns.
6. Confirm **Visual Reviewed**, then press **Publish Ready**. Errors block publication. Changed prefab dependencies, physics settings or placement constraints invalidate approval and require republication; fitting profiles also need approval again after their dependencies change.

Raw intake expects visual models without gameplay scripts. The **Import prototype building blocks** command handles the supplied authored machinery. Generated source code is recorded as provenance and is never automatically executed by the workshop.

### Vehicle conventions

- Units are metres, Y is up and Z is forward. Keep the prepared root at scale `(1,1,1)`; correct scale on the visual child.
- Name separate wheel objects `wheel_fl`, `wheel_fr`, `wheel_rl`, `wheel_rr` for initial mapping, or bind the four wheel visual anchors manually in `PreparedVehicleRig`.
- Generated wheel centres, radius, mass, spring settings and chassis box are starting proposals. Fused wheels must be separated before the rig can publish.
- The vehicle definition supplies mass, torque, wheel dimensions and supported equipment interface. `AssetSocket` supplies the actual connection point, accepted part category, width/mass limits, hydraulics and fitting-angle envelope.
- A loader must have its actual boom pivot assigned. Mechanical animation remains transform-driven, as in the original prototype; arbitrary imported animation clips/joint simulation are not automatically retargeted.

### Attachment and wheel conventions

- An attachment needs `AssetAnchor` components for **Connection** and **WorkingEdge**, plus a valid `AttachmentDefinition`. A bucket additionally needs a **LoadVisual** anchor on its preplaced snow-load visual.
- The connection anchor identifies the part's mounting frame. Fitting offsets are applied relative to the vehicle socket. The tool does not stretch parts to make them fit.
- Wheel catalog entries represent one wheel reused at four rig locations. Their dimensions, radius, mass and grip multiplier drive the physical setup; right-side mirroring is configurable. Socket and rig clearance limits reject oversized wheel sets.
- Accessories use their matching sockets; added mass, mount position, torque and grip modifiers are incorporated when an assembly is baked. Engine-like upgrades can use an accessory entry with suitable metadata/socket; this is not a generic transmission simulator.
- Existing blade and bucket behaviors are supported. Blowers, tracks and new hydraulic mechanisms still require their own gameplay implementations.

## Assemble a vehicle

1. Publish the prepared **body** and selected parts after reviewing them.
2. In **Vehicle assembly**, create a recipe and drag a body entry into **Vehicle body**. Already equipped assemblies are separate output assets and cannot be used as bodies.
3. Add a fitting row and drag a part into it. Set the socket ID (`front` or `wheels` for supplied bodies). An `AssetSocket` can be added/configured in prefab mode for other accessories.
4. Adjust mounting position and rotation. **Refresh assembly preview** provides a rotatable preview. Review clearance, pivot alignment and the full intended operating envelope; automatic checks do not guarantee absence of mesh clipping.
5. Approve the fitting after inspection. Changes clear its approval. Interface, category, full operating mass, working width, hydraulics, wheel clearance and supported behavior checks must pass.
6. **Bake complete drivable prefab** saves a new physics definition and an equipped prefab with Rigidbody, wheels, equipment anchors and preauthored effects. The current prototype requires exactly one blade or bucket per complete machine. Baked rigs retain authored attachment rest transforms rather than forcing the original primitive mounting offsets.
7. **Register assembled vehicle in catalog**, review and publish it. The output can then be placed by a scene plan.

Placement does not grant player ownership or automatically add a machine to `PrototypeSession.Fleet`. Add a complete machine to a session's fleet during Editor authoring to use the existing controls. A broader catalog-driven shop/equipment save UI is a later integration; no transactions or player loadouts are silently changed in this phase.

## Reusable environment assemblies

Prepare a complete house lot, tree group, depot or road as one catalog entry. Review the footprint of automatically generated aggregate box colliders: entrances and driveways often need smaller compound proxies rather than a single blocking box. The supplied prototype assemblies retain their existing authored colliders.

Roads need at least two **RoadConnection** anchors with matching interface IDs. Other anchor roles identify entrances, service/disposal zones and parking points for future scene/gameplay authoring. These markers describe intent; they do not automatically implement contract logic or service actions.

## AI scene plans

1. Publish the entries the layout should use.
2. Export the **Ready catalog manifest** from **AI scene plan**. It includes IDs, categories, sizes, tags, placement scale ranges, connection frames and socket capabilities. Give this file and your level brief to the AI.
3. Have the AI produce version-1 JSON using only those IDs. See `Assets/Snowglow/Catalog/Examples/DistrictLayout.json` for the schema. The supplied example requires its prototype entries to be reviewed/published first.
4. Load/paste the plan and press **Validate plan**. Validation rejects unknown/unapproved/stale entries, duplicate IDs, invalid transforms/scales, out-of-district placements, solid AABB footprint overlaps, incompatible road anchors and missing/cyclic connections. It resolves the whole plan before creating anything.
5. **Build new scene from plan** writes a new scene under `Assets/Scenes`. Existing scenes, including Pinefall, cannot be overwritten. Optionally it creates a saved flat 1 km Terrain. All prefab instances and metadata exist before Play.

This produces an editable **layout**, not an automatically complete career district. Contract rules, interactive snow coverage, terrain shaping and session fleets remain explicit authoring tasks. AABB checks are conservative; visual review and navigation/handling checks are still required. Plans are currently limited to 1,000 placements and 2 MB of JSON to keep Editor operations bounded.

## Future asset requests in chat

Provide the actual model/imported prefab or the generation script plus dependencies, and any reference images. The preparation conventions above are the contract for future AI-assisted intake. Scripts and references belong in `Catalog/Sources` (or another preserved source folder); prepared output belongs in `Catalog/Prefabs` and `Catalog/Entries`. Inference must be recorded, and uncertain results stay Draft/Needs Review.

## Verification

The first supplied Blender tractor has been prepared as three Needs Review entries
(body/four wheels, curved plow/lift frame, roof beacon), with a saved recipe and review
prefab. Use **Snowglow > Asset intake > Open tractor review prefab** to inspect it.
See [the intake record](SourceAssets/LowPolySnowPlower/README.md) for part mapping,
source preservation, wheel differences and the required articulation work before
gameplay approval.

Run **Snowglow > Run asset workshop checks** in Edit mode. Results go to `Logs/AssetWorkshopChecks.txt`. Checks exercise actual Unity prefab preparation, source preservation, approval invalidation, fitting, wheel upgrades and scene serialization using disposable fixtures. They restore Pinefall and remove their fixtures. Rendering, imported production model quality, touch handling and Galaxy A07 performance require separate testing.


The twelve imported alpine street-light entries are reusable Needs Review scenery
prefabs. Their Frame, Luminous and SnowCap groups remain independently editable;
remove/disable the cap in the Editor for a snow-free variant. Optional point lights
are inactive by default under the clearly named group. Wall Lantern requires a
facade mount; the other eleven variants retain their original ground/pole origin.
See SourceAssets/AlpineStreetLights/README.md for provenance, counts and controls.


The 26 alpine tree/wood catalog entries remain Needs Review. Each is an opaque
TreeWood assembly plus independently removable SnowCap only where supplied. Tags
distinguish evergreen, deadwood and forest-floor variants. Existing 230 m distance
visibility is saved on prepared prefabs; trunk capsules and log/stump boxes require
collision review. See SourceAssets/AlpineTrees/README.md for provenance and metrics.


## Photoshop UI kits

UI is a separate catalogue category with canvas-pixel bounds and persistent sprite
references. Use **Snowglow > Asset intake > Import Photoshop UI button kit** for the
preserved Snow District kit. Source layers, hashes and export mapping live under
SourceAssets/SnowDistrictUI and Catalog/Sources/SnowDistrictUI. Thirty-two UI button
prefabs remain Needs Review; the extra three Play states are sprite variants on
Play. The generic model-preparation path redirects UI intake rather than proposing
world colliders. UI entries cannot be placed through the 3D district scene planner.
Do not publish entries Ready solely because they were integrated into Pinefall.

PSDUIIntake reapplies the skin through Editor authoring; runtime only switches the
already-bound Work icon for plow/loader. Existing button actions and touch targets
remain. Artwork for future or removed controls is inventory, not authorization to
reintroduce those controls. See SourceAssets/SnowDistrictUI/README.md for provenance,
re-export commands and the mobile atlas budget.

The alpine sign-board intake adds 12 Needs Review landscape entries with static
mesh lettering, separate removable snow and explicit ground/wall mount anchors.
Blank replacement boards retain destination labels as scene-authored TextMesh
children. See SourceAssets/AlpineSignBoards/README.md for provenance, counts,
repeat-safe import menu and acceptance limits.

Sign export now recalculates outward winding per closed source solid before
separating material groups. Open source lettering is preserved. Use the sign
normals/four-sided-render check menu after re-export; keep backface culling on.
