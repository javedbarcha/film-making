# Eight angular low-poly clouds

Supplied 3 November 2026: `Eight_Angular_Low_Poly_Clouds.blend`, preserved
byte-for-byte from the user's Downloads file (1,373,168 bytes).
SHA256: `8AE32B6DE44F38348DB0D38BD745FA0C274D657D6E0E7B7D0A6D3318BEE681E9`.

Blender 5.2.2 inspected/exported with `--factory-startup --disable-autoexec`.
The file contains eight mesh objects and no embedded texts, cameras or lights.
Each variant has 168 source vertices and 332 triangles. Original layout positions
are presentation only; exported copies are horizontally centered with base Y=0
in Unity. Source shape/proportions remain unchanged, flat face normals are
recalculated, and the three source face colors are packed into one opaque palette.
No original Blender data is saved over or executed.

| Catalog ID | Variant | Triangles |
| --- | --- | ---: |
| angular-cloud-01 | Alpine Puff | 332 |
| angular-cloud-02 | Wide Cumulus | 332 |
| angular-cloud-03 | Tall Cluster | 332 |
| angular-cloud-04 | Long Cloud Bank | 332 |
| angular-cloud-05 | Asymmetric Puff | 332 |
| angular-cloud-06 | Stepped Cluster | 332 |
| angular-cloud-07 | Compact Crown | 332 |
| angular-cloud-08 | Broad Alpine Bank | 332 |

Prepared FBXs, palette and object inclusion/metrics manifest:
`Assets/Snowglow/Catalog/Sources/AngularClouds`.
Editor intake: `AngularCloudsIntake.cs`; menu **Snowglow > Asset intake > Import
and scatter eight angular clouds**. All catalog entries remain Needs Review.

The scene scatter uses 40 saved prefab instances, five of every variant, under
**Sky clouds | 40 angular low-poly clouds**. Random seed 311103 produces jittered
angular sectors, radii 260-620 m, base altitudes 195-260 m, random yaw and uniform
scales fitting source width to 45-85 m. Cloud/mountain bounds receive clearance
checks. Repeat intake preserves existing cloud poses instead of reshuffling or
duplicating them. Existing skybox and camera background settings are left unchanged.

Total added geometry: 13,280 triangles, 40 renderers/material slots, one shared
opaque URP Unlit palette material. Clouds have no colliders, Rigidbody, lights,
shadow casting, wind animation or runtime construction. They do not modify snow.
Unlit face colors match the current daytime setting; coordinated day/night cloud
response would be separate future work. These counts are not device timings.

Inspection/export scripts: `Tools/AssetIntake/inspect_angular_clouds.py` and
`export_angular_clouds.py`. Native report/previews and original scene backup:
`Logs/AngularClouds` (ignored). Android/device performance is not established.

Unity 6000.3.8f1 / Android compiled, imported and validated all eight catalog
prefabs, then saved/reopened the 40 placements. Native metrics match 332 triangles
and one renderer/material slot per variant. Five of each variant, unique placement
IDs, prefab bindings, absent physics and disabled shadow casting were verified.
Existing non-cloud catalog identities/transforms and all existing mesh/Terrain/
snow dependency hashes matched before/after. No district rebake or player-save
write. Four native 1280x720 sky/horizon views rendered; north/east views were
visually inspected. Scene validation passed. Continuous live-camera inspection
and Android/device profiling remain untested. Entries remain Needs Review.
