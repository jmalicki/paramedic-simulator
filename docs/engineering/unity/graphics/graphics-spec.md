## Graphics & Visual Direction — Paramedic Simulator

### Visual Goals

- Realistic-but-performant clinical visuals focused on readability and training clarity.
- Prioritize legibility of medical devices, patient cues (skin, breathing, cyanosis), and environment hazards.
- Target 60 FPS on mid‑range laptop with URP.

### Rendering Stack

- Pipeline: URP (Forward+ when available in Unity 6 LTS).
- Post‑processing: Bloom (subtle), Color Adjustments, Vignette (light), Depth of Field for focus hints only in menus/cinematics.
- Anti‑aliasing: TAA (URP) with per‑object motion vectors where helpful; fallback to SMAA.
- Shadows: Mixed lighting; baked lights for interiors; real‑time key lights with shadow distance budget.

### Lighting

- Author scenes with one primary key (directional/area), fill, and accent lights near points of care.
- Use Light Probes and Reflection Probes; bake GI for interiors.
- Physically‑based values where practical; maintain exposure consistency across scenes.

### Materials & Shaders

- PBR standard materials for most assets; mobile‑friendly variants for UI‑heavy scenes.
- Custom shaders (Shader Graph) for:
  - Patient skin: sub‑surface scattering approximation (SSS), sweat sheen mask.
  - Medical devices: emissive displays with scanline/fresnel options; CRT/LED variants.
  - Decals: blood, dirt, scuffs via URP decal projector for storytelling and wear.
  - Transparent items: IV bags, tubing with refraction-lite and thickness maps.

### Characters & Animation

- Patient: single detailed base with blendshapes for breathing effort, cyanosis tint mask, pain/grimace, facial pallor.
- Rig: Humanoid; retargetable; additive breathing layer; procedural micro‑motion for unconscious states.
- Staff/NPCs (later): low animation variety; IK for hand placement on props.

### Environments

- Modular kits for: ambulance, apartment, street exterior, ER bay.
- Trim sheets + atlas textures to control draw calls; vertex density budget aligned with mid‑range GPUs.
- Decal pass for storytelling (blood drops, footprints, equipment wear).

### UI & In‑World Displays

- Medical monitors and devices as in‑world meshes with emissive screens; HDR emissive used sparingly.
- Screen content via RenderTextures or procedural UI to keep crisp at all distances.
- Colorblind‑safe palettes and strong contrast for vitals; alarm hierarchies communicated with light and sound.

### Performance Budgets (Initial)

- CPU: simulation < 2 ms; rendering main thread < 8 ms; total < 16 ms/frame.
- GPU: 6–8 ms/frame on target; draw calls < 1500 (desktop mid‑range), triangles < 4–6M in view.
- Post‑FX: <= 1.5 ms total; shadow cascades <= 4; shadow distance tuned per scene.

### Asset Quality Targets

- Hero first‑person props (BVM, laryngoscope, IV start kit): 5–15k tris; 2k textures.
- Common small props (syringes, meds, tape): 500–2k tris; 512–1k textures; atlas friendly.
- Rooms/modules: 20–60k tris per module; 2k trim sheets; shared materials.

### LOD & Impostors

- LOD0/1/2 for props > 5k tris; screen‑space metrics tuned in Profiled scenes.
- Impostors/billboards for far environment dressing if needed.

### Effects (VFX)

- Particle systems for oxygen vapor, dust motes, subtle blood mist in trauma; GPU instancing where possible.
- Screen‑space indicators for critical states (tunnel vision for hypoxia optional, accessibility‑toggled).

### Art Direction Notes

- Color: neutral clinical base; accent with medical device hues; avoid over‑saturated tones.
- Tone: grounded realism; convey urgency through lighting/audio, not exaggerated visuals.
