# Snowglow mobile art budget

Permanent colored lamp housings add four boxes / 48 triangles to tractor and
six boxes / 72 triangles to loader. Orange front and white rear materials are
shared across the fleet. Housings stay visible when lamps are off; no colliders
or additional lights. These counts exclude the existing glow/lens geometry and
do not establish device frame time.

Lights toggle/glow revision (4 November): four tractor and six loader lamp halo
quads join the existing one-per-machine beacon halo. These are small additive
billboards, enabled with that machine's light switch. Beacon glow remains enabled
independently during operation. Front spot intensity is 10 rather than 7; there
are still only two enabled shadowless spots on the selected machine. Phone
profiling must include these small transparent surfaces; geometry counts do not
establish their overdraw cost.

Operating vehicle lamps (4 November): two shadowless 22 m spotlights on the
selected operating machine; parked machinery has no enabled beams. URP now
allows two additional lights per object, previously zero. Three extra opaque
lamp-surface renderers are enabled on that machine; rear/beacon surfaces add
no point lights. Native ground-illumination previews passed, but A07 cost and
daytime readability across all gameplay cameras still need device validation.
The subsequent beacon revision adds one small additive halo quad per vehicle,
enabled only with that machine's operating lights, plus a rotating opaque red
lens shader. No additional point lights. Paired lamp groups use one shared
material slot each; phone profiling remains required.

Reference: Samsung Galaxy A07, Android URP, 30 FPS (33.3 ms/frame). These are
conservative starting budgets for this mid low-poly game, not device limits or
evidence that most phones run it smoothly. Profile representative older/lower-end
phones and the A07 with snow simulation, UI, shadows and saving enabled.

| Content | Suggested working target | Review trigger |
| --- | --- | --- |
| Complete nearby vehicle, including wheels and equipment | 5,000–10,000 triangles | Above 15,000 triangles or 30,000 imported vertices |
| Small scenery prop | 50–1,000 triangles | Repeated instances or tiny screen footprint justify less detail |
| House/building assembly | 500–3,000 triangles | Above 5,000; consider repeated windows/trim and LOD |
| Vehicle materials | 1–4 shared materials, preferably one opaque palette plus opaque toon windows | Above 12 material slots; count slots across every renderer |
| Vehicle renderers | About 6–10 for separate moving modules | Many tiny renderers; combine rigid parts in the Editor |
| Visible scene geometry | Initially aim around 100,000–200,000 triangles | Includes snow/terrain; count extra shadow passes separately |

Numerical targets above are Snowglow recommendations. They are advisory: screen
coverage, transparency/overdraw, shadow passes, shader cost, CPU draw submission,
mesh updates and physics can dominate at a low triangle count. Use Unity's device
Profiler and Frame Debugger for actual timings and submissions. The SRP Batcher
reduces submission/setup work; it does not make every material slot a single draw.

The seven saved winter-home prefabs include detachable roof/ground snow:

| House | Unity triangles | Imported vertices |
| --- | ---: | ---: |
| Red Cottage | 1,724 | 3,566 |
| Green Porch | 1,944 | 3,960 |
| Garage Home | 1,828 | 3,762 |
| White Colonial | 1,728 | 3,568 |
| Storybook Cottage | 1,748 | 3,600 |
| Twin A Frame | 1,002 | 2,078 |
| Brick Barn | 2,848 | 5,820 |

Each is within the project's 500-3,000 building working band. No decimation was
needed. Four renderers/material slots per house use three shared materials across
the set (opaque palette, unlit warm windows and snow). Six exports contain eight
zero-area architecture triangles each, verified in Blender; Unity removes them.
Export metadata records these separately so import checks compare visible geometry.
Unity prepared and saved all seven and reopened their Pinefall placements on
29 October 2026 (`Logs/WinterHomes/UnityIntake.txt`). These counts do not establish
Android frame times, overdraw, draw submissions or sustained phone performance.

Five alpine mountains use 690 / 726 / 1,016 / 930 / 974 triangles and
1,524 / 1,601 / 2,380 / 2,041 / 2,247 Unity-imported vertices respectively
(Needle Peak, Twin Summits, Sawtooth Ridge, Glacier Massif, Alpine Cirque).
Each has two renderers/material slots, sharing rock and snow materials across the
set. Twelve saved placements total 10,088 mesh triangles, before shadow passes.
Geometry is modest for background assets and did not require decimation. Terrain,
view coverage, fog and shadow costs still need device profiling; these counts are
not frame-time or draw-submission measurements.

The new loader is within the vehicle geometry target: 7,656 vehicle triangles,
8,424 including its preauthored carried-snow visual. The complete prefab has
15,095 imported vertices, eight renderers and nine material slots / three distinct
materials. The tractor is also within the geometry target (7,274 triangles), but
its 30 material slots should be consolidated into a shared palette in a future
optimization pass. No triangle decimation is justified for these vehicles yet.

At the saved depot camera, an Editor frustum-bounds inventory found 69,770 mesh
triangles, including 54,432 snow triangles, and 69 material slots. This is only a
candidate inventory: it excludes Terrain, UI, particles and shadow passes, and
does not model batching or occlusion. Other camera positions and tall snow piles
can cost more. It is not a rendered-frame or FPS measurement.

Future catalog vehicle checks now warn above 15,000 triangles / 30,000 imported
vertices. Existing material-slot warnings remain active. Recheck whole equipped
vehicles: reviewing body, wheels and equipment separately can hide their total.
Both vehicles now use opaque toon windows to avoid transparent window blending
and sorting. Use LOD/culling for distant/repeated assets, limit transparency, and
avoid adding tiny unseen bevels/bolts. Preserve each original source before any
optimization; author prepared meshes and proxies in the Editor.

Primary guidance: [Android geometry optimization](https://developer.android.com/games/optimize/geometry),
[Unity mobile graphics/art optimization](https://unity.com/how-to/mobile-game-optimization-tips-part-2).

The 30 October residential expansion has 40 homes: 72,552 house mesh triangles
and 160 house renderers, before foundations/fences/shadow passes. Original
16 homes remain; ten blocks contain 2, 4 or 6 homes each. Native Editor count and
clearance checks passed; profile the denser layout again on A07 before judging
mobile frame times. No new per-house materials, textures or real-time lights.

The twelve alpine street-light prefabs use three shared opaque materials and
three renderer groups each. Native imported geometry (including removable snow):

| Fixture | Triangles | Unity vertices |
| --- | ---: | ---: |
| Path Bollard | 228 | 396 |
| Park Globe | 600 | 1,092 |
| Compact Village Lantern | 630 | 1,128 |
| Heritage Single | 630 | 1,128 |
| Heritage Twin | 1,084 | 1,980 |
| Wall Lantern | 414 | 796 |
| Alpine Hanging Lantern | 684 | 1,244 |
| Heritage Triple | 1,290 | 2,384 |
| Residential Curved LED | 438 | 780 |
| Main Road Outreach | 438 | 780 |
| Dual Median LED | 604 | 1,092 |
| Four Way Area Light | 936 | 1,724 |

Fourteen scene fixtures total 8,804 triangles / 42 renderer-material slots, before
shadow passes. Twin/Triple heritage lamps slightly exceed the small-prop 1,000
triangle working target because they have multiple heads; no decimation was needed.
All 21 placed optional point lights are inactive by default and shadowless when
enabled; use their small proposed ranges deliberately and profile night scenes.
Opaque unlit luminous surfaces avoid transparent glazing. Native save/reopen and
structural checks do not establish A07 frame times or a safe active-light count.

The 26 alpine tree/wood prefabs use two shared opaque materials across the set,
one body renderer and one additional renderer on supplied snowy variants:

| Variant | Triangles | Unity vertices |
| --- | ---: | ---: |
| Tower Spruce | 904 | 1,590 |
| Broad Fir | 694 | 1,224 |
| Slender Spruce | 942 | 1,600 |
| Irregular Drooping Fir | 780 | 1,338 |
| Wind Bent Pine | 646 | 1,130 |
| Open Alpine Larch | 1,966 | 3,460 |
| Young Fir | 426 | 724 |
| Spruce Sapling | 388 | 650 |
| Snowy Tower Spruce | 1,290 | 2,280 |
| Snowy Broad Fir | 981 | 1,745 |
| Snowy Slender Spruce | 1,361 | 2,303 |
| Snowy Irregular Drooping Fir | 1,100 | 1,882 |
| Snowy Wind Bent Pine | 893 | 1,567 |
| Snowy Open Alpine Larch | 2,633 | 4,523 |
| Snowy Young Fir | 647 | 1,121 |
| Snowy Spruce Sapling | 576 | 986 |
| Tall Dead Snag | 1,054 | 1,988 |
| Thin Bare Snag | 874 | 1,628 |
| Broken Alpine Trunk | 750 | 1,394 |
| Bare Young Sapling | 562 | 1,028 |
| Low Cut Stump | 220 | 357 |
| Tall Cut Stump | 220 | 357 |
| Split Cut Stump | 252 | 417 |
| Snow Capped Stump | 223 | 362 |
| Fallen Cut Log | 232 | 300 |
| Snowy Broken Log | 318 | 464 |

Ninety-seven placed forest assemblies total 96,243 triangles / 145 renderer-material
slots, versus 454,736 / 679 in the original primitive forest. Existing 230 m distance
visibility is retained. These are whole-placement inventories, excluding extra
shadow passes, and do not establish active-frame costs or A07 performance. The
Larch / Snowy Larch are heaviest (1,966 / 2,633 triangles, placed 2 / 8 times);
review these in device profiling before increasing forest density. No transparent
foliage cards, per-tree material copies, wind animation or per-instance mesh copies.

Orbit/Scenic camera modes (30 October) use the same single gameplay Camera and add
no environment geometry or enabled lights. Camera safety uses bounded, nonallocating
sphere sweeps and Terrain samples. Wider/rotating viewpoints can reveal more assets
and shadows than the original camera, so the existing geometry counts do not predict
A07 timings. Editor input/terrain checks and three native renders passed; profile
all four views during actual driving and snow work on the phone.

The roadside fixture expansion adds 14 prepared lamps (28 total): +8,810 triangles
and +42 renderers/material slots. Total fixture geometry is now 17,614 triangles /
84 renderers, sharing the existing materials. New optional point lights remain off;
there is no enabled real-time light-count increase. Native save/reopen, clearance
and snow-accounting checks passed. Counts do not establish device frame-time cost.

Before density expansion, supplied LOD3 reduced the 97-instance forest from
96,243 to 40,939 triangles / 145 renderers (57.46% fewer triangles). Native
mesh references and saved/reopened scene verified. No LOD0/1/2 from the new pack
are imported and no LODGroup switching is added. Material slots/culling are
unchanged; render-submission and FPS improvements are not measured. See
SourceAssets/AlpineTreesLowestLOD/README.md for per-variant LOD3 counts.

4 November 2026 density/seating update: current forest is 146 instances,
62,496 triangles / 219 renderers, still LOD3 only. The 49 additions contribute
21,557 triangles. Terrain seating and conservative mountain exclusion passed native
save/reopen checks. Geometry counts are not measured A07 performance.
