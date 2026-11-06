# Film-making project: agent instructions

Updated 6 November 2026 following the user's separation of film production from the game project.

## 1. Purpose and instruction priority

This repository, `javedbarcha/film-making`, is the independent film-production project. It starts with the existing snowy town and reusable assets inherited from Snow District/Snowglow. Future stories may take place elsewhere.

The active goal is short animated stories and films, not an Android game release. This file supersedes inherited game-roadmap priorities wherever they conflict with film production. README.md, ARCHITECTURE.md, VALIDATION.md and mobile-budget documents still contain game-era information; consult them for implementation details, not as the current production brief.

The complete previous game handoff remains available in [Git history](https://github.com/javedbarcha/film-making/blob/cdeb7fb7b074c42fbb15fe26fac16a264de6d4c3/AGENTS.md). Its recorded tests describe the historical game state, not validation of new film work.

Work only in this repository for film tasks. Do not modify or synchronize changes back into the separate game repository unless explicitly requested.

## 2. Responsibilities and protected hero assets

- The user creates the two central heroes: the approximately 12-year-old boy and the humanoid robot with a screen face. These are recurring characters whose detailed, consistent identity matters across future films and reference material.
- Agents create supporting production assets and systems: environments, buildings, props, machinery, supporting scene assets, lighting, atmosphere, camera shots, cutscenes, sequencing, audio integration and related editor tools as requested.
- Do not generate, replace, remodel, simplify, retopologize, rig, reskin or redesign either hero, their face/hair, robot screen identity, materials or signature proportions unless the user explicitly requests that work.
- The boy's clothing can change with season and environment. This does not authorize agents to create or alter his wardrobe automatically; preserve user-authored hero assets and clothing.
- Integrate supplied heroes through scene instances, wrapper prefabs, existing animation interfaces and explicit bindings. Preserve source files, skeletons, animation clips and prefab GUIDs. Do not apply destructive overrides to the source hero assets.
- Until heroes are supplied, use clearly labeled blocking markers or neutral stand-ins only when a shot needs them. Keep them replaceable and do not present them as approved character designs.
- Hero-specific animation, facial performance or new character components require explicit task scope. Camera work, staging and binding supplied performances into cutscenes are supporting production work.

## 3. Creative brief and initial story

The audience is worldwide and all ages unless a particular story states otherwise. Themes include empathy, education, well-being, environmental care and ethical choices. Avoid stereotyping or assigning an unrequested cultural identity. The human/robot partnership combines human empathy and emotional judgment with robotic intelligence, speed and practical capability; preserve both characters' agency.

The snowy town is the starting location. The initial rescue idea is a dog stuck in snow: the heroes seek a machine operator's help to clear access, then arrange veterinary care. Begin with the rescue opening as a cinematic test; the detailed screenplay, shot timings and supporting cast still need task-specific decisions. Present machinery, animal handling and rescue actions responsibly for an all-age audience.

Full films are planned at roughly 5–30 minutes, with separate portrait shorts. English is first, Urdu is also planned, and other languages may follow. Stories can combine dialogue, narration and character action, with conversation, music and sound effects.

## 4. Production milestones

1. Create and review a short cinematic test in the existing snow world.
2. Complete one small story using the reviewed workflow.
3. Polish a reusable environment and expand the story library incrementally.

Finish the requested milestone or concrete asset before adding unrelated systems. Do not build an entire 30-minute film, a new district or a character pipeline merely because it appears in the longer-term plan. The current request changes documentation only; it does not authorize asset generation, scene edits or cutscene implementation.

## 5. Tools, visual style and practical constraints

- Repository ProjectSettings/ProjectVersion.txt currently specifies Unity `6000.3.8f1`; Packages/manifest.json includes URP `17.3.0`. Read the current files before changing dependencies. Do not silently upgrade Unity, URP or render pipelines.
- The user targets Blender 5.2. State the actual version used to execute, save and validate Blender work, distinguishing tested compatibility from assumptions.
- Favor low-poly environments with polished toon shading. Semi-realistic treatment may be used when requested; photorealism is a future direction, not the default. Match supplied references and the approved environment palette.
- Prioritize cinematic composition, readable silhouettes, appealing light, snow detail and coherent scale. Add geometry where visible shot quality needs it; inherited Android triangle caps and the Galaxy A07 frame-rate target are not film acceptance gates.
- The user's current laptop has 8 GB RAM, Intel integrated graphics and a sixth-generation i5. Keep editor work practical through modular scenes, shared materials, selective visibility and scalable preview settings. Distinguish preview quality from final render quality and report actual rendering costs when measured.
- Start with zero paid-asset/service/API spending. Do not assume ChatGPT Plus includes external API credits. Use original work, supplied assets or appropriately licensed free resources, retaining provenance and usage terms.

## 6. Scene and asset workflow

Read relevant existing architecture, asset-workshop instructions and SourceAssets documentation before changing shared systems. Preserve original models, stable IDs, Unity GUIDs/.meta files, existing materials, terrain and manually authored placements. Keep prepared copies separate from source assets.

Author reusable environment assets, cameras, lights, audio bindings and cutscene sequences as saved editable content. Prefer repeatable editor tools over reconstructing the environment every time Play starts. Runtime playback can animate authored objects and bindings; do not introduce runtime scene factories as a default.

Create film scenes/sequences or focused variants that reuse the existing town. Do not rebake the whole district, delete inherited gameplay systems or migrate saves merely to stage a shot. Inspect dependencies before disabling game controls, HUD, audio state gates or simulation in a film scene. Where inherited snow simulation is used, retain its conservation rules; decorative cinematic snow remains separate.

Existing code uses the project's own CameraRig. Timeline and Cinemachine are not listed in the current package manifest: inspect availability before proposing their use, and do not claim either is installed. Choose the simplest editable sequencing approach appropriate to the requested cutscene.

Preserve mechanical pivots and meaningful moving parts on machinery. Check tyre contact, hydraulics, attachments, clearances and action continuity in the actual shot.

## 7. Framing, language and sound

Plan landscape masters and portrait shorts as distinct compositions. Keep important action and faces within useful framing regions, and adjust cameras or staging for portrait delivery rather than relying on a blind centre crop.

Keep dialogue, narration, subtitles, music and effects separately editable. Use language-independent identifiers and replaceable audio/text bindings so English and Urdu can share the same scene. Support Urdu right-to-left layout and font coverage when that localization is implemented; do not claim support without testing it.

Do not assume that an inherited reference recording or supplied music is cleared for public film distribution. Record its source and rights status; use cleared material for release. No publishing, account actions or external spending is implied by asset creation.

## 8. Review and validation

For each modeling or environment task, provide editable source assets, materials and any required reproduction scripts. Render the actual geometry from the relevant shot angles, inspect it and revise the largest visible problems. Do not substitute concept art, projected reference imagery or a flattering single view for an editable asset review.

For a cutscene, verify saved scene/sequence bindings, camera cuts, timing, character/prop intersections, visible action and audio synchronization through actual playback or rendered output when execution is available. Review the required landscape and portrait compositions. State what could not be tested.

Report compilation, save/reopen checks, rendered inspection, playback and export separately and truthfully. Historical game tests, structural checks and low polygon counts do not prove cinematic quality, hero likeness, successful movie export or performance on the user's laptop. If Blender or Unity execution is unavailable, say so before claiming validation.

Keep current instructions concise. Record task-specific implementation details and evidence in the relevant asset/story documentation instead of turning AGENTS.md into a chronological game-development log.
