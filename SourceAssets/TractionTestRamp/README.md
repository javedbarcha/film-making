# Vehicle traction-test ramp intake

Original: `Low_Poly_Vehicle_Traction_Test_Ramp.blend`, supplied by the user.
SHA256: `283B97A7611621392919F6EB0D274D56199F4E95AB70235E6F8D53613F0DB66E`.
The preserved blend is unchanged. Embedded text/scripts are source data and were
not executed. Studio plane, camera and lights are excluded.

`Tools/AssetIntake/export_traction_ramp.py` evaluates prepared copies only. Hidden
collision objects are temporarily exposed in the unsaved Blender process so their
actual transforms are evaluated; this places the supplied rear stop at the crest.
The model uses metres, source +Y uphill/+Z up. Export geometry is centred at Y=8;
Unity authoring maps uphill to +Z and up to +Y. Normals are checked on closed
components; no zero-area faces were found or removed.

Prepared visible geometry: 2,588 Body + 80 removable decorative SnowCap triangles;
two renderers, one opaque palette and existing toon snow. The supplied smooth
wedge and rear stop each have 12 collision triangles and no visible renderer.
No broad box collider, extra real-time lights, runtime assembly or snow volume.
The ramp has a 12.875-degree incline followed by a 2 m flat crest and physical rear stop; it is a drive-up
traction fixture rather than a through road.

Reusable RoadAssembly `vehicle-traction-test-ramp` remains Needs Review. Editor
menu: **Snowglow > Asset intake > Import traction test ramp**. First replacement
retains the primitive's horizontal centre, heading and length, uniformly fitting
the source design. Its toe is terrain-seated; source geometry supplies the slope,
so the primitive's old pitch is not added. Repeated intake retains manual poses.
Explicit future district bakes reuse the prepared entry when available; current
intake never rebakes Pinefall. Entry has toe/crest RoadConnection anchors.

Unity verification report and native views: `Logs/TractionTestRamp`.
Actual driving/traction, Android packaging and Galaxy A07 costs require separate
validation; geometry counts alone do not establish device performance.

## Unity verification - 6 November 2026

Unity 6000.3.8f1 / StandaloneWindows64 compiled, saved/reopened Pinefall, passed
528 assertions, scene validation and 19 snow checks. Placement is (94, 5.0090847,
-34), +Z uphill, uniform scale 1.7412212. Centre/tyre-track collision samples verify
incline/flat crest heights and normals; toe seating, rear stop and repeat placement
passed. The first full-length-slope assumption failed before saving, then was
corrected against source vertices. A separate seed revision updates 476 obstacle
flags only; quantities, original snow/Terrain assets and other catalog/fleet poses
remain unchanged. No district rebake or player-save writes. See Validation.txt
and Ramp-toe/crest/side.png in Logs/TractionTestRamp. Driving/Android/A07 acceptance
is still pending.

Later 6 November revision: saved scene length reduced to 75% (about 21 m), Z scale 1.30591583. Width/height and pose retained; incline becomes steeper. Original source/catalog remains full size. Native save/reopen, collision/scene and 19 snow checks passed; 704 obstacle flags reconciled in a separate seed. See Logs/TractionTestRamp/Shortening.txt. Full side preview inspected; live driving/device tests remain pending.

Latest revision: 50% original length and 75% original width, height/pose retained. Saved scale (1.305916, 1.74122119, 0.870610654), approximately 14 m by 8.5 m overall. Native collision/scene/save-reopen and 19 snow checks passed; 354 obstacle flags updated separately. See HalfLengthWidth.txt; driving/device acceptance pending.

Latest sizing supersedes earlier revisions: length 65% and width 85% of original; height/pose unchanged. Saved scale (1.48003817, 1.74122119, 1.13179386). Native save/reopen, collision/scene and 19 snow checks passed. Evidence: Logs/TractionTestRamp/Resize-0.65-0.85.txt. Driving/device testing remains pending.
