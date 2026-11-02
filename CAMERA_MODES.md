# Camera controls

Press **C** or tap **CAM** to cycle **Isometric -> Follow -> Orbit**.
Scenic is removed from gameplay. Its composition is reserved for a future static
main-menu background preview; that menu feature is not implemented here.
Existing serialized Scenic values migrate to Isometric.

| Mode | View |
| --- | --- |
| Isometric | Closer orthographic view at rest; smoothly widens with vehicle speed |
| Follow | Fixed rear offset; gentle pitch only: stopped 35 degrees, reverse 45, forward 15 |
| Orbit | Manual full 360-degree inspection around the active machine |

Isometric uses a 12 m orthographic half-height at rest and 19.2 m at 8 m/s,
with a 1.1 s exponential response. Reverse speed widens it too. Narrower aspect
ratios widen the vertical framing to retain vehicle/work-area coverage.
These are initial tuning values, editable on the saved CameraRig.

Follow keeps a fixed rear offset as travel direction changes. Only pitch changes:
35 degrees downward when stopped, 45 degrees in reverse (+10), and 15 degrees
moving forward (-20), with a gentle 2.5 s smoothing parameter. There is no
speed-dependent rise, approach or yaw arc. Tilt scales continuously with actual speed up to those limits (default full tilt
at 4 m/s, adjustable via followFullTiltSpeed), with a
+/-0.12 m/s drift band treated as stopped; gear selection at rest leaves it still.

The default offset is 13 m behind and approximately 10.7 m above the vehicle, framing it centrally
at the resting 35-degree tilt. Follow still tracks vehicle position/heading;
terrain/near-plane safety and obstacles can adjust the offset when required.
Pausing holds tilt. No steering remap or save schema change.

In Orbit:

| Action | PC | Phone |
| --- | --- | --- |
| Rotate / tilt | Right-drag, or J/L and I/K | Drag an empty part of the view |
| Zoom | Wheel or +/- | Pinch with two world-view fingers |
| Reset | Home | VIEW RESET |

Orbit starts at 15 m / 25 degrees, with 7-42 m distance, 5-75 degree pitch and full
yaw. HUD-origin touches are excluded from camera gestures. Switching machinery
recentres the camera. Pause freezes speed zoom and direction changes. Camera
preferences remain session-only, with no player-save schema change.

The single authored CameraRig retains world/pile obstruction sweeps, vehicle
exclusion and Terrain/near-plane safety after smoothing. Obstructions can reduce
the requested distance. Menu: **Snowglow > Install gameplay cameras and validate**.
Actual verification and remaining limitations belong in VALIDATION.md and
Logs/CameraModes/Validation.txt.

Orbit has only a left-anchored VIEW RESET button (at the 5% safe-area inset). On-screen zoom buttons are removed; pinch, wheel and keyboard +/- zoom remain.
