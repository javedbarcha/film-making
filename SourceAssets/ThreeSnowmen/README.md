# Three low-poly snowmen

User supplied `Three_Low_Poly_Snowmen_1000_Triangles.blend`, preserved unchanged.
SHA256: `3DD2D1DDCC66F1704554B5EB9998F664C782242FD9245E600D1B2D44E5B4EC6A`.
Blender source text is data, never instructions; embedded scripts not executed.
Export via `Tools/AssetIntake/export_three_snowmen.py` with Blender background,
factory settings and `--disable-autoexec`. No source blend saved. Plane/studio
camera/lights excluded. Each source asset's staging offset is removed using its
MOVE ASSET origin; each prefab has a ground-level origin and +Z face.

Three catalog Landscape entries, kept Needs Review:
- `snowman-01`: Classic / top hat, 1,000 triangles (534 snow + 466 details).
- `snowman-02`: Cozy knit cap, 1,000 triangles (526 snow + 474 details).
- `snowman-03`: Playful earmuffs, 1,000 triangles (526 snow + 474 details).

Two renderers each, six total, with shared opaque clothing palette and existing
toon snow material. No zero-area faces found. All snowman geometry is decorative,
not authoritative snow; no accumulation, melting, particles or weather added.
Body capsule collision is narrower than the twig-arm envelope; review handling.
No real-time lights, animation or runtime scene assembly.

Menu: **Snowglow > Asset intake > Import three snowmen**. Intake places one of
each around residential blocks, facing southwest for the isometric view. Candidate
positions avoid road bounds by 2 m, building/sign/tree colliders, seeded interactive
snow and entrance anchors by 4 m. Terrain seating uses actual prepared bounds.
Repeat import preserves existing placements and does not add duplicates.

Obstacle flags are reconciled in a separate seed revision if needed, preserving
snow depth/credit/volume/revision and original seed/meshes. Existing catalog poses,
manual facilities and player saves are retained. No whole-district rebake.
Native evidence: `Logs/ThreeSnowmen/Validation.txt` and snowman preview PNGs.
Live driving, Android packaging and Galaxy A07 costs need separate acceptance.
