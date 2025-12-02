## Sprint 4 — Authoring + Assessment + Generative Setup (3 weeks)

Objective: Enable non‑engineers to author scenarios, implement the scoring rubric and human‑readable reports, and stand up local generative tooling on the 48 GB GPU.

### Deliverables

- [ ] Scenario authoring via ScriptableObjects + JSON import/export
- [ ] Grading rubric engine with weighted criteria and reports
- [ ] Generative stack installed (ComfyUI/InvokeAI) with saved graphs

### Tasks

Authoring

- [ ] ScriptableObjects for scenario metadata, objectives, triggers
- [ ] Validators: missing assets, invalid thresholds, time conflicts
- [ ] JSON export/import with schema versioning and migrations

Assessment

- [ ] Rubric engine: weights, thresholds, rationale strings
- [ ] Session report generation (JSON + human‑readable Markdown)
- [ ] Hook to telemetry to extract key actions and times

Generative Setup

- [ ] Install ComfyUI with SDXL 1.0 + Refiner; ControlNet Tile/Canny/Depth/Normal
- [ ] Create and commit graphs: texture tiling, decal sheets, inpaint, upscale
- [ ] Lock prompt templates and negative lists for clinical realism
- [ ] Set output directories and naming; seed logging

Testing

- [ ] Import/export round‑trip test with diffs
- [ ] Scoring unit tests for common rubric rules
- [ ] Smoke test: run each ComfyUI graph and validate outputs meet size and color space

Docs

- [ ] Update `generative-graphics-pipeline.md` with exact model hashes
- [ ] Authoring workflow guide for non‑engineers

### Acceptance Criteria

- [ ] A scenario can be authored, exported, imported, and run end‑to‑end
- [ ] Reports list objectives with rationales and scores; stable across replays
- [ ] Generative graphs run locally reproducibly and produce usable assets

### Risks & Mitigations

- Schema churn → migrations; version contracts; golden test assets
- Generative variance → lock seeds; post‑curation; SMEs in review loop
