# Operating machinery lights

Latest button appearance: the supplied teal on/off artwork replaces the text
button at **48x48** design units, remaining at safe-area right-center. Each state
uses its own saved sprite; the old text is hidden and artwork remains untinted.
See SourceAssets/VehicleLightButton/README.md for preserved originals/import crop.

Latest beacon behavior: both solid lenses stay the same tractor-yellow/amber
throughout operation. Only the lens rotor moves and the separate glow flashes
fully on/off. Runtime explicitly binds the shared amber color so an old red
material cannot reappear on the loader. Earlier red/sweeping-color notes below
are historical and superseded.

Solid geometry revision: both machines now have permanent **orange front lamp
boxes** and **white rear lamp boxes**, visible even when lamps are switched off.
The tractor has two explicit 25x19x10 cm rear housings attached to the ballast,
not only luminous surface copies. Front boxes are 26x23x10 cm: two on the tractor
and four on the loader. Colored Simple Lit materials keep the boxes readable as
solid objects. Warm front/white rear illuminated faces, glow, light switch and
independent red beacon retain their existing behavior. All boxes are saved in
equipped prefabs during Editor authoring; they add no colliders or runtime spawn.

Latest revision (4 November): **LIGHTS ON/OFF** is a small 96x64 design-unit
button at the safe-area right side, vertically centered. It controls only the
selected machine's front/rear lamps, beams and their glow halos. The roof beacon
stays lit and rotating while that machine operates, regardless of this switch.
Menus/pause and parked/engine-unavailable states still stop its operating lights.

Front lamps and illumination are warm; beam intensity is 10, increased from 7.
Rear lamps now remain **white in both gears**, superseding the red/white gear
behavior described below. Both vehicles have authored front and rear glow halos,
not only bright lamp surfaces. They use the same small additive billboard shader
as the beacon glow; no extra real-time lights or scene-wide bloom were added.
The switch defaults on and retains its state per machine during the current
session; it is not a new player-save field. The button shows the selected
machine's state after a machine switch. All UI and glow objects are saved before
Play; runtime only updates existing bindings.

Both current machines switch their front/work lamps, rear lamps and amber beacon
surfaces on automatically while selected during active play. They remain on while
stopped with the engine available. Menus/pause, parked machinery, empty fuel,
zero condition and disabling the controller switch them off. No extra button or
night cycle is required. Rear lamps are paired red running lamps, switching
together to white when reverse gear is selected. Roof beacons now use red rotating
lenses, a sweeping bright highlight and a gently pulsing small halo, following the
user's emergency-beacon reference. Animation freezes and glow switches off on pause.

`VehicleOperatingLights` updates saved Light/Renderer/rotor bindings only. Each equipped
prefab contains three opaque luminous mesh groups and two shadowless spotlights.
The loader's upper work lamps carry its beams; its lower front lamps also light
visually. Rear lamps and beacons use bright opaque surfaces without real-time
point lights. Each beacon adds one small camera-facing additive halo quad. This
local glow does not require scene-wide bloom or HDR; bloom is not enabled.

The prepared luminous surfaces are copies of named lamp parts from the preserved
Blender sources, offset 4 mm along their normals to avoid coincident faces with
the original opaque lens surfaces. Normals are recalculated on prepared copies.
Rear shells are offset 6 cm backward to sit outside the source ballast skin,
which otherwise hides the rear lenses even with the correct material.
Each combined pair uses exactly one material and submesh: the first exporter had
created separate per-lens materials, so changing only slot zero left one grey or
the wrong color. That slot mismatch is now rejected by validation. Export script:
`Tools/AssetIntake/export_vehicle_lamps.py`. Parts and local beam positions:
`Assets/Snowglow/Catalog/Sources/VehicleLights/LampBindings.json`.
Source files, vehicle rigs, wheels, equipment and original model meshes remain
unchanged. Lamp objects are parented to the body mesh's authored coordinate frame.

The mobile URP asset now permits two additional lights per object, previously
zero. Only the selected operating machine enables its two beams; beam range is
22 m, with no shadows. Existing Low/High render-scale/shadow settings remain.
Actual phone cost must be profiled; this budget is not a performance measurement.

Editor installer: **Snowglow > Install operating vehicle lights and validate**.
Stop Play and save Pinefall first. Installation updates the two equipped prefabs
in place, preserves their GUIDs and save IDs, and saves/reopens Pinefall. Future
machine/district authoring includes the same saved bindings. No runtime assembly,
new gameplay forces, snow changes or player-save migration is involved.

Native evidence and remaining limits: `Logs/VehicleLights/Validation.txt` and
`VALIDATION.md`. Original scene backup: `Logs/VehicleLights/Pinefall-before.unity`.

Front housing color correction: yellow (RGB 1, 0.83, 0.025) on both vehicles, superseding orange. The existing material path/GUID is retained; rear housings remain white.

Beacon color correction: retain the original amber/orange roof-beacon appearance during operation; the rotating lens and halo no longer switch to red. Rotation, glow and independence from the lamp button remain.
