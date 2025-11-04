## Generative Graphics Pipeline (Local GPU, 48 GB VRAM)

Goal: accelerate art production while preserving clinical realism and performance. All tooling runs locally where feasible, using the 48 GB VRAM GPU for high‑res diffusion, ControlNet stacks, and batching.

### Use Cases
- Texture synthesis and upscaling (albedo/roughness/normal from refs)
- Decals (blood, dirt, wear) and environment dressing masks
- Material variant generation (clean/used/bloody states) with consistent style
- 2D concept → proxy 3D for block‑outs (then retopo)
- Pose/motion generation for brief vignettes and micro‑motions

### Model Choices (Local)
- Image Generation: SDXL 1.0 + Refiner; LoRAs for clinical equipment style; ControlNet (Canny, Depth, Normal, Tile)
- Texture/Mat Gen: SDXL + ControlNet Tile for seamless; Normal map derivation via AI normal estimators; optional Substance‑like AI with open tools
- Inpainting/Outpainting: SDXL Inpaint for blood decals, wear, label fixes
- Super‑Resolution: 4x‑SR models (Real‑ESRGAN variants) for UI/emissive screens
- 3D from Images (assistive): TripoSR for coarse mesh; use as proxy, then manual retopo/UV
- Motion: MDM or similar diffusion motion models for short loops; export as FBX via retarget

### Workflow
1) Concept → Moodboards
  - Generate SDXL boards with strict prompt templates and negative lists for realism
  - Approve palettes and material references (medical plastics, metals, fabrics)

2) Texture Authoring
  - Start from scan/photo or base material; generate albedo variants with SDXL (Tile)
  - Derive roughness/metalness heuristically; generate normals from depth/photometric cues
  - Validate seams, tileability, and PBR ranges; bake to 1k/2k atlases

3) Decals & States
  - Generate blood/dirt/wear as decal sheets; ensure physically plausible color and gloss
  - Author state machine: clean → used → contaminated; swap materials/decals at runtime

4) Equipment Screens & Labels
  - Generate high‑res UI backgrounds/textures with SDXL; vectorize critical glyphs
  - Super‑resolve and sharpen; export to 1k–2k atlases; ensure legibility at 1–2 meters

5) Proxy 3D & Retopo
  - For simple props, use TripoSR on reference photos to get a coarse mesh
  - Manual retopo in DCC; UV unwrap; bake maps; replace proxy with production mesh

6) Motion/Poses
  - Generate short breathing/pain micro‑motions via motion diffusion; retarget to Humanoid
  - Blend as additive layers; ensure loopability and subtlety

### Performance/Quality Guards
- Style Consistency: Train lightweight LoRAs for devices/environments; version control them
- Prompt Templates: Locked templates per asset family; negative prompts against non‑clinical artifacts
- Validation: Automated checks for PBR value ranges, texture sizes, compression, tile seams
- Privacy/Licensing: Use only self‑created or licensed refs; record provenance in metadata

### Tooling & Automation
- Invoke models via InvokeAI/ComfyUI graphs; saved, versioned graph files per asset type
- Batch jobs: seed sweeps, CFG/steps matrices; auto‑pick top‑k via simple perceptual metrics
- Unity import processors enforce texture settings, compression, and naming conventions

### Risks
- Uncanny outputs for skin → use generative only for masks/tints, not final albedo
- Inaccurate labels → require human review by SME; keep vector sources
- 3D generative artifacts → always retopo/UV and bake; never ship raw AI meshes


