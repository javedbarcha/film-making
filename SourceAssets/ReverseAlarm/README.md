# Reverse warning alarm

Original synthesized heavy-machinery backup warning, created 4 November 2026.
No third-party recording is used. Generator: Tools/Audio/generate_reverse_alarm.py.
Prepared clip: Assets/Snowglow/Audio/reverse-alarm.wav, mono 16-bit PCM, 22,050Hz,
0.85-second cycle: 0.36s of a 780Hz tone with a restrained second harmonic and
soft attack/release, followed by 0.49s silence. Source gain is 0.35 in Unity.

ReverseAlarmAuthoring creates and saves the looping AudioSource in the Editor,
with playOnAwake disabled, under the existing SynthAudio session object. It also
binds future full district bakes. Runtime only plays/stops this existing source.
Both machines use the same warning through the current active-machine/input state.
It plays while reverse gear is selected during running gameplay, including when
stationary in reverse, matching a conventional machinery backup warning. Forward,
menus/pause and component disable stop it; the existing sound toggle mutes it.
This is an audible indication, with no driving/physics/save/economy changes.

Menu: Snowglow > Audio > Install reverse warning alarm. Original scene backup
and native installation report: Logs/ReverseAlarm. Phone loudness and audible
Play-mode verification must be reported separately from binding checks.

Unity 6000.3.8f1 compiled the runtime and Editor changes, imported the clip as mono
PCM with preload enabled, saved/reopened Pinefall and verified the looping source,
clip duration, playOnAwake off, 2D playback, mute/unmute and scene validation.
Waveform integrity checks confirmed tone/silence cadence, zero loop boundaries and
no PCM clipping. Existing catalog poses and snow seed hash matched. No player-save
writes or district rebake. Audible Play-mode and device speaker checks remain
untested. Native report: Logs/ReverseAlarm/Validation.txt.
