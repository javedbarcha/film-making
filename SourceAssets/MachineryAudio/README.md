# Reference-based machinery audio

Current operating sounds use the four user-supplied MP3s preserved unchanged in
References. Original names and SHA256 hashes are recorded in RecordedManifest.json.
Filenames identify supplied provenance, not verified licensing terms. Full MP3s
remain outside Assets; only short processed WAVs are referenced by the game.
No embedded document instructions were executed and no external service used.

Run `Tools/AssetIntake/prepare_recorded_machinery_audio.py` through Blender with
background/factory settings and `--disable-autoexec` (aud MP3 decoding and numpy).
Six mono 24 kHz PCM16 loops are saved in Assets/Snowglow/Audio/Machinery/Recorded:

| Game layer | Supplied recording | Selected range | Loop |
| --- | --- | --- | --- |
| Tractor idle | tractor-engine-27641 | 11.75?17.00 s | 5 s |
| Tractor work | diesel-tractor-2-55064 | 33.25?37.50 s | 4 s |
| Tractor drive texture | spinopel-tractor-290811 | 13.50?16.75 s | 3 s |
| Loader idle | excavator-19118 | 41.75?47.00 s | 5 s |
| Loader work | excavator-19118 | 286.25?290.50 s | 4 s |
| Loader drive texture | excavator-19118 | 220.25?223.50 s | 3 s |

Band shaping, DC removal, gentle compression, peak headroom and 250 ms overlap
crossfades prepare the loops. Peaks stay at 0.65. The 0.65 s brake-air cue uses
high-frequency texture at 67.50 s in the excavator recording with a shaped envelope.
Its name describes game use; the source event was not independently identified
as an actual brake. It fires only on a moving brake press, with a 1 s cooldown;
held brake, idle, pause and disabled machinery do not continuously hiss.
Existing dedicated reverse alarm/cadence is retained and controlled separately.
No recorded whole sequence is played blindly as a gameplay loop.

MachineryAudioProfile holds saved tractor/loader clip references. Runtime swaps
only these references and updates existing AudioSource playback/pitch/gain. It
creates no source/clip assets. Read-only speed, wheel RPM, throttle/brake, snow
resistance, slip, carried load and low-speed demand inform the smoothed mix.
Engine pitch uses a restrained recording-friendly range; struggling increases
volume/growl with mild tone droop. Presentation does not simulate new gears or
change machinery physics. Pause/menus, parked/unavailable machines, empty fuel,
zero condition and disable stop loops/cues. All sources follow existing mute.

Use **Snowglow > Audio > Install load responsive machinery audio**. Factory binds
profiles and sources before Play on explicit bakes. Seven processed clips total
24.65 s: approximately 2.37 MB decoded float samples before Unity overhead.
Older original procedural diesel-base/diesel-load/transmission WAVs and generator
remain preserved as historical assets and are no longer the operating profile.

Native evidence: Logs/MachineryAudio/Validation.txt (878 assertions plus scene/
19 snow checks; saved/reopened). Numeric loop, mix and event checks passed; no
live audible/phone listening or sustained performance acceptance is claimed.
Listening demo: Logs/MachineryAudio/ReferenceAnalysis/Machinery-recorded-preview.wav.
41 s: tractor then loader, each idle/acceleration/cruise/struggle/brake/reverse.
The offline preview is a demonstration, not a recorded live playthrough.
