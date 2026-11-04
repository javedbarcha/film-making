# Supplied snow-plowing tractor

Original: `low_poly_snow_plower.blend`, supplied 27 October 2026.
SHA256: `8F48568B0051F2379707F15809558090DE7128C23637BAD6E6A8774BA815B85C`.
The preserved copy is outside Assets so Unity does not invoke Blender's automatic importer.
Embedded `Toon_Snowplow.py` was treated as source data and was not executed.

The model is a four-wheel tractor with a curved orange front blade, unequal-size
front/rear tyres, rigid lift hardware, a glazed cab and an amber roof beacon.
Source units are metres; source axes are Z-up and X-forward.

Preparation scripts are in `Tools/AssetIntake`. Run Blender with
`--background --factory-startup --disable-autoexec` and the supplied file followed
by `--python Tools/AssetIntake/export_plower.py` from the project root.
This exports separate FBX modules into `Assets/Snowglow/Catalog/Sources/LowPolySnowPlower`.
No source .blend is saved or overwritten. Studio floor, default cube, cameras and
lights are excluded. All 209 vehicle meshes are accounted for by `PartMap.json`.
Named source meshes are combined per rigid module, retaining material slots and
four wheel-centred pivots; the original Blender file keeps each original part editable.

Prepared tyre grips have corrected outward winding (40 blocks per rear wheel,
32 per front wheel). The original source had mixed face directions, producing
recessed-looking bumps. Export asserts each repaired block's outward normals and
saves `TreadValidation.json`; no vertices or triangles were added.

Unity menu: **Snowglow > Asset intake**. Inspect can be rerun; Prepare deliberately
refuses duplicate catalog IDs. Review/edit existing entries for subsequent adjustments.

Prepared catalog entries:

- `imported-tractor-body`: 5,548 triangles; body plus four independently bound wheels.
- `imported-tractor-plow`: 1,654 triangles; blade, cutting edge, markers and lift frame.
- `imported-tractor-beacon`: 72 triangles; beacon and base, static appearance.

Recipe: `Assets/Snowglow/Catalog/Recipes/GoldenTractor.asset`.
Combined preview: `Assets/Snowglow/Catalog/Prefabs/GoldenTractor-REVIEW.prefab`.
Open it with **Snowglow > Asset intake > Open tractor review prefab**.
It is an Editor assembly preview without MachineController, not a playable machine.
The three catalog entries and fittings remain Needs Review/unapproved. The original
catalog sources remain separate from gameplay integration.

Following the request to use it as the free starter, Pinefall now uses a separate
`GoldenTractor-Starter.prefab` with MachineController, authored blade controls and
snow spray. It costs zero, occupies Fleet[0], and keeps the old `municipal-plow-01`
save ID. **Snowglow > Asset intake > Use tractor as free starter plow** reapplies this
scene substitution; the regular district baker also uses the saved starter prefab.
The loader is unchanged. Starter initialization and saved scene checks passed; the
rigid lift/yaw remains the prototype approximation described below.

Physical proposals: 2,800 kg dry body, 450 kg plow, 5 kg beacon, front/rear wheel radii
0.578/0.888 m including treads, 1.93 m wheelbase, 1.82 m track, speed limit 8 m/s.
These are starting values, not measured specifications. The source has no mechanical
animation rig or explicit blade-yaw joint. Lift arms, rams and blade are currently one
rigid module; they require an authored linkage and clearance review before gameplay
publication. The generic controller's vertical lift/yaw is not a faithful simulation
of these hydraulics. Uniform four-wheel replacement is omitted because the current
wheel-set workflow cannot describe unequal axles. Four wheel meshes/pivots remain
separate for future axle-specific upgrade support.

Source-render preview was visually inspected. Unity import confirmed metre scale,
Y-up/Z-forward axes and all four pivots. Unity structural checks passed for saved/reopened
prefabs, persistent mesh references, fittings and rejection of unapproved gameplay baking.
URP Simple Lit palette is copied from source colors. Windows now use opaque,
double-sided cyan toon shading with pale diagonal reflections (28 October 2026);
the original Blender toon shading is approximated. 7,274 total triangles, 7 renderers
and 30 material slots require device profiling; the renderer reduction is not evidence
of acceptable phone performance. Driving, lift/angle clipping, snow interaction, Android
build and sustained Galaxy A07 performance remain untested.

## Rear tyre guard winding correction - 5 November 2026

The left rear guard was mirrored across source Y with unchanged face winding.
Its signed volume was -0.13640273 m3 versus +0.13640273 on the opposite guard,
causing inward-facing surfaces and apparent gaps under backface culling.
`export_plower.py` now recalculates outward normals on prepared rear-fender
copies and asserts positive volume. Source blend, vertex positions, guard shape,
wheel physics and triangle count are unchanged. `--body-only` skips the source
render and exports only TractorBody.fbx; plow/beacon exports and all metas remain
intact. Previous body bytes: Logs/TractorGuard/TractorBody-before.fbx. Native
validation and paired side previews: Logs/TractorGuard. No source file saved.
