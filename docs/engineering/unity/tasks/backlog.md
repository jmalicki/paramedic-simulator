## Unity Backlog — Seed

### Epics
- Core Simulation & Contracts
- Interaction & Devices
- UI/UX & Localization
- Authoring & Assessment
- Telemetry & Replay
- Build/CI & Tooling
- Graphics & Rendering
- Generative Graphics Pipeline

### Sprint 1 — Foundations
- Create Unity project (6000 LTS, URP) and repo layout.
- Add Input System, Addressables, TextMeshPro; configure namespaces and analyzers.
- Implement deterministic fixed‑step loop with time controls.
- Define model contracts (proto/JSON schema) and HTTP/gRPC client shim.
- CI: GitHub Actions with Unity Builder; run EditMode tests; produce dev artifacts.
- Graphics: URP baseline renderer, post‑FX profile, profile scenes.

### Sprint 2 — Scenario + Telemetry
- Scenario Engine v1: states, timers, triggers, soft fail conditions.
- Session Telemetry: JSONL writer, replay reader, privacy scrubbing.
- HUD prototype: vitals panel, objectives, timer.
- Graphics: lighting pass templates, probe volumes, baked GI test scene.

### Sprint 3 — Interaction + Device MVPs
- Ray‑based interaction + prompts; toolbelt inventory.
- Device: Patient Monitor (HR, RR, SpO2, NIBP start/stop).
- Device: Oxygen delivery (NC/non‑rebreather) with flow rates affecting vitals.
- Graphics: medical materials library (plastics, metals, fabrics); decal system MVP.

### Sprint 4 — Authoring + Assessment
- Scenario authoring via ScriptableObjects + JSON export/import.
- Grading rubric engine; human‑readable reports.
- Generative: SDXL + ControlNet local setup; prompt templates; texture gen pipeline.

### Sprint 5 — Generative & Art Production
- Train LoRAs for device style; asset provenance sidecars; batch texture generation.
- Decal sheets for blood/wear; import processors; automated PBR validation.
- Proxy 3D via TripoSR for simple props; retopo/UV/bake workflow finalized.


### Non‑Functional Tasks
- Performance budget enforcement; memory/GC profiling.
- Accessibility: remap controls, color palettes, subtitles.
- Security: offline mode, allowlist hosts, payload redaction.


