## Lane E — Generative Graphics Setup (Local 48GB VRAM)

Repo: https://github.com/jmalicki/paramedic-simulator
Branch: lane-e-generative-graphics

Context
- Local SDXL + ControlNet pipeline for textures, decals, inpaint, upscale; reproducible graphs and provenance per `docs/engineering/unity/graphics/generative-graphics-pipeline.md`.

Tasks
- [ ] Install ComfyUI; add graphs for tiled textures, decals, inpaint, upscale; commit graph files
- [ ] Lock prompt templates/negatives; log seeds; record model/version hashes
- [ ] Batch runs (seed sweeps, CFG matrices) with auto-ranking; export top-k; human curation
- [ ] Optional: Train lightweight LoRAs for device/environment styles

Constraints
- [ ] Provenance sidecars; PBR/tiling validation; no raw AI meshes shipped

Deliverables
- [ ] Graphs, scripts, sample assets, docs

Definition of Done
- [ ] Each graph runs locally and outputs assets passing validation checks

References
- `docs/engineering/unity/graphics/generative-graphics-pipeline.md`

