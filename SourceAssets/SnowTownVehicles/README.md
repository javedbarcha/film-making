# Snow town parked vehicle pack

User file `Snow_Town_Eight_Low_Poly_Vehicles.blend`, preserved unchanged.
SHA256: `DB3F55EEF9EF1AC5A73BE398DB2C4EA26C6780EFA75678B6BC208FEB07404A62`.
Embedded text/scripts are source data and never executed. Export with Blender
background/factory settings, `--disable-autoexec` and
`Tools/AssetIntake/export_parking_vehicles.py`. No source blend saved.
Studio Plane/cameras/lights excluded. Vehicle staging offsets removed using each
MOVE VEHICLE root. Source body headlamp surfaces establish the front (-Y source,
+Z prepared). Closed prepared solids get outward normals before material splitting.
Explicit triangulation removes zero-area faces only; no visible surface removed.

| Variant | Source triangles | Prepared triangles |
| --- | ---: | ---: |
| 01 Snow Hatchback | 2650 | 2602 |
| 02 Family Sedan | 2650 | 2602 |
| 03 Utility Pickup | 2900 | 2852 |
| 04 Mountain Rescue Pickup | 2900 | 2852 |
| 05 Alpine SUV | 2690 | 2642 |
| 06 Town Patrol SUV | 2846 | 2798 |
| 07 Snow Town Ambulance | 2900 | 2788 |
| 08 Town Taxi | 2694 | 2646 |

Each prepared vehicle has three renderers: static Body (including wheels), Snow
and Glazing. Original blend retains individual wheel objects. Three shared
materials: opaque palette, existing toon snow, and opaque cyan toon windows with
pale diagonal highlights. No realtime lights, physics driving or runtime factories.
Static body box collision requires handling review. Decorative snow is outside
the authoritative grid. No player ownership/garage fleet entries added.

All eight are reusable Needs Review Landscape entries `parked-town-vehicle-01`
through `08`. Landscape is deliberate: these are parked scenery, not prepared
player vehicle bodies with Rigidbody/WheelCollider rigs. Use **Snowglow > Asset
intake > Import eight parked town vehicles**.

Only the existing five primitive cars are replaced: hatchback, sedan, utility
pickup, Alpine SUV and taxi (01,02,03,05,08). Original X/Z/headings retained,
with tyre bottoms seated on the parking asphalt. Original poses recorded in
Logs/SnowTownVehicles/OriginalParkingPoses.json. Other variants remain available
in the catalog. Repeated intake retains the saved replacements without duplicates.
Future explicit district authoring uses these prepared replacements when available.

Five placements add 13,344 triangles / 15 renderers, with three shared materials.
A separate seed revision refreshes obstacle flags if needed; snow depth/credit/
volume/revision and original assets remain intact. Active machinery fleet,
manual layout and player saves preserved. No district rebake.
Native evidence: Logs/SnowTownVehicles/Validation.txt, Vehicle-01..08.png and
Parking-row.png. Actual driving/Android build/A07 performance remain untested.

Native completion: Unity 6000.3.8f1 / Android saved/reopened Pinefall, passed
scene/19 snow checks, reconciled 80 obstacle flags, preserved quantities/poses
and fleet. All eight variant previews and row render inspected. Physical
driving/phone acceptance remains untested.
