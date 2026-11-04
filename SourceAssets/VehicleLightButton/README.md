# Supplied lights button icons

Original user PNGs are preserved byte-for-byte as LightsOn.png and LightsOff.png.
On: Cozy teal headlights-on button.png; SHA256
`3039D01C193C1E4654A6F4C6935B72079BD3D8D9B712DD4905E2E93B752EFB31`.
Off: Teal headlights button, off state.png; SHA256
`CCEE97D9469912FD669317B99D27046E2B536333E0814717AED5325F9589A8D4`.

Unity copies are under Catalog/Sources/VehicleLightButton. Both source images
are 1254x1254 RGBA. Their sprite metadata uses the same rectangle
(184,188,874,876), in Unity bottom-left pixel coordinates, to exclude the large
transparent margins and insignificant stray-alpha pixels. No bitmap recoloring,
resampling or background removal was performed. Bilinear/clamped, no mipmaps,
256 maximum imported texture size. Source files retain their full resolution.

The existing right-center button is now 48x48 design units. Both sprite references
and the click listener are saved in Pinefall. VehicleLightsButton swaps the image
for the selected machine's session light state, without tinting the supplied
artwork. The previous text label is hidden when icons are available. Rotation
and flashing of the constant amber roof lens remain independent of this button.

Native Unity 6000.3.8f1 verified sprite swapping for both machine selections,
paused input and scene save/reopen. The rendered HUD was inspected. This run's
active platform was StandaloneWindows64; Android build, A07 touch size and
performance were not tested. Use the existing vehicle-light installer to repeat
authoring. See VALIDATION.md for the separate tractor asset recovery during import.
