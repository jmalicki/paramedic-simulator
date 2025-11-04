## Sprint 5 — Generative + Art Production (3 weeks)

Objective: Operationalize the generative pipeline for textures/decals/material states, train lightweight LoRAs for style consistency, and integrate proxy 3D → retopo → Unity imports with validations.

### Deliverables
- [ ] LoRAs trained for device/environ style
- [ ] Batch texture gen pipelines with automated validation
- [ ] Decal sheets (blood/wear) integrated; runtime material state swaps
- [ ] Proxy 3D assist flow finalized (TripoSR → retopo/UV/bake)

### Tasks
Style Consistency
- [ ] Curate reference datasets; annotate style
- [ ] Train LoRAs; evaluate FID/LPIPS; pick best checkpoints
- [ ] Version LoRAs and prompts; document usage

Batch Texture Generation
- [ ] ComfyUI batch graphs with seed sweeps and CFG matrices
- [ ] Perceptual auto‑ranking; export top‑k; human curation step
- [ ] Normal/rough/metal derivation and PBR range checks
- [ ] Unity import processors enforce sizes, compression, color space

Decals & States
- [ ] Generate blood/wear atlases; color/roughness clamps for plausibility
- [ ] Device and prop material states (Clean/Used/Contam) with runtime swap logic

Proxy 3D Workflow
- [ ] Capture photo refs; run TripoSR; generate coarse meshes
- [ ] Manual retopo and UV; bake maps; replace proxies
- [ ] LODs and import rules applied; addressable grouping

Testing
- [ ] Automated PBR validation suite; seam and tiling checks
- [ ] Runtime perf test scenes with target draw calls and ms budgets

Docs
- [ ] Update graphics specs with measured performance
- [ ] Asset provenance sidecars filled with prompts/seeds/model hashes

### Acceptance Criteria
- [ ] Generated textures pass PBR and tiling checks; integrate without rework in Unity
- [ ] Decal and state systems function visually and within perf budgets
- [ ] Proxy 3D aids production without shipping raw AI meshes

### Risks & Mitigations
- Inconsistent outputs → prompt templates + LoRAs + human curation
- Perf regressions → budget gates and profile scenes in CI (where feasible)


