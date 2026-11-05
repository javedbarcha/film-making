# Exhaust tip correction and quantity

Measurements from preserved tractor/loader source blends, read using Blender
background/factory settings and `--disable-autoexec`; embedded text not executed.
`Tools/AssetIntake/inspect_exhaust_outlets.py` records evaluated mesh bounds/top
centres in SourceOutlets.json. Source blends are never saved or replaced.

Source +X forward/+Z up maps to Unity (-sourceY, sourceZ, sourceX). The earlier
smoke positions used the opposite lateral sign. Source-body-space pipe surfaces:
tractor (-0.42, 2.795, 0.46), loader (-0.38, 2.75, -1.30). The emitter centre sits
5 mm above each surface, mapped through the actual fitted Body transform into
the machine root. This retains correct placement if the prepared body is offset.
ExhaustSmokeAuthoring uses this mapping for future explicit authoring.

The focused correction menu changes only emitter position on both existing saved
prefabs and scene instances. It does not rerun full effect/material authoring or
reset the user's appearance. Emission coefficients double from 0.25/1.35/0.8 to
0.5/2.7/1.6 particles/s, with cap 4.8 instead of 2.4. Idle and every throttle/load
combination are exactly 2x. The current 24-particle capacity accommodates the new
peak with existing 1.7?2.3 s lifetime, so it is left unchanged. Start speed,
simulation speed, velocity/drift, lifetime, size, cone, mesh and material stay as
supplied by the existing effect. Parked/pause/unavailable gates remain in place.

Menu: Snowglow > Effects > Correct exhaust tips and double smoke quantity.
Evidence/report/backups and four native views: Logs/ExhaustSmoke/TipCorrection.
No source model, vehicle physics, snow quantities or player saves are modified.
Runtime still adjusts only the already-authored particle emission and playback.
Physical moving-smoke inspection and Galaxy A07 costs remain separate acceptance.

## Denser exhaust and initial vertical rise - 6 November 2026

User requested another 2x quantity increase and about 0.5 m of upward travel
before drifting. Idle/throttle/load coefficients are now 1/5.4/3.2 particles/s,
capped at 9.6/s: exactly twice the previous mix and four times the original.
Saved motion curves hold 0.65 m/s upward velocity with zero sideways drift for
the first 42% of the existing 1.7-2.3 s lifetime (0.46-0.63 m), then ease upward
speed down and introduce gentle random lateral drift. Initial cone launch speed
is zero, avoiding the former immediate forward kick. Particle size, fade,
mesh/material, pipe-tip placement and 24-particle capacity remain unchanged.
Runtime still only controls authored emission/playback; existing pause, parked,
selection, fuel and condition gates remain. Focused Editor menu:
Snowglow > Effects > Apply doubled smoke and vertical rise. Evidence/backups:
Logs/ExhaustSmoke/Rise. Live driving and Galaxy A07 acceptance remain separate.


Unity 6000.3.8f1 / Android saved/reloaded both prefabs and saved/reopened Pinefall.
1,185 assertions passed, including native particle trajectories across three
lifetimes/eight seeds, 121 rate combinations, outlet checks and state gates.
Scene validation and all 19 snow checks passed; snow hash and fleet/catalog
poses preserved. Live moving appearance, Android packaging and A07 cost untested.

