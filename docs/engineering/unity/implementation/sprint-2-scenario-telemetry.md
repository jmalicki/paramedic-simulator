## Sprint 2 — Scenario + Telemetry (2 weeks)

Objective: Implement the scenario engine (states, timers, triggers) and full session telemetry/replay, and expand graphics with lighting templates.

### Deliverables

- [ ] Scenario Engine v1 with objectives and timers
- [ ] Telemetry recorder (JSONL) and replay runner
- [ ] HUD prototype: vitals, objectives, timer (placeholder art okay)
- [ ] Graphics: lighting templates, probe volumes, baked GI test scene

### Tasks

Scenario Engine

- [ ] State machine: Initial → Running → Success/Fail/Timeout/Aborted
- [ ] Triggers: time‑based, action‑based, threshold‑based
- [ ] Objectives: weighted criteria, text for UI, event hooks
- [ ] Rubric: produces per‑objective score and rationale strings

Telemetry & Replay

- [ ] Define JSONL schema: tick, inputs, state deltas, model I/O
- [ ] Implement writer with rolling files and size limits
- [ ] Implement replay runner that replaces live inputs with recorded ones
- [ ] Privacy scrubbing rules and opt‑in encryption

HUD Prototype

- [ ] Vitals panel (HR, RR, SpO2, BP placeholder)
- [ ] Objectives list with completion states
- [ ] Session timer with pause/speed indicators

Graphics — Lighting

- [ ] Key/fill/accent template prefabs
- [ ] Light Probe + Reflection Probe setups
- [ ] Bake GI for interior test scene; record settings in doc

Testing

- [ ] EditMode: scenario state transitions
- [ ] PlayMode: replay determinism test (output parity across runs)

Docs

- [ ] Update `architecture.md` Scenario and Telemetry sections
- [ ] Document lighting practices in `graphics-spec.md`

### Acceptance Criteria

- [ ] A scripted scenario runs to completion and yields a score report
- [ ] Replay faithfully reproduces UI and state outputs
- [ ] Baked GI scene meets visual goals and performance budget

### Risks & Mitigations

- Replay drift → tighten determinism; record seeds; minimize non‑deterministic APIs
- Log volume → compress and chunk; cap per‑session size
