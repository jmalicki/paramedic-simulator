## Paramedic Simulator — Unity Delivery Roadmap

Audience: engineering, production, and design.
Scope: shipping a single‑player desktop build that integrates the generative patient model, with strong foundations for authoring, testing, and observability. VR/multiplayer follow after foundations are stable.

### Guiding Principles
- **Foundations first**: deterministic sim loop, data contracts, tooling, automated tests.
- **Authoring over hardcoding**: scenarios and devices authorable via ScriptableObjects + JSON.
- **Observability**: record every decision and physiological state change for replay/debug.
- **Performance budgets**: 60 FPS on mid‑range laptop; sim loop < 2 ms/frame.

### Phase 0 — Bootstrapping (1–2 weeks)
- Create Unity project (Unity 6000 LTS, URP). Repo layout and CI skeleton.
- Define data contracts to the generative patient model (HTTP/gRPC; versioned JSON schema).
- Spike: call model with stub patient and stream vitals into UI mock.
- Acceptance: CI builds EditMode/PlayMode tests, lints, and produces dev builds for Win/macOS.

### Phase 1 — Core Simulation and Contracts (3–4 weeks)
- Deterministic simulation loop (fixed‑step) with time dilation and pause.
- Patient Model Adapter: request/response orchestration, caching, timeout/retry policies.
- Scenario Engine v1: state machine, objectives, scoring rubric, timers.
- Telemetry & replay: structured logs, session artifacts, anonymized payloads.
- Acceptance: scripted scenario runs end‑to‑end without manual intervention; replay works.

### Phase 2 — Interaction, Devices, and UI (4–6 weeks)
- Interaction System: ray + proximity interaction, toolbelt, inventory.
- Medical Devices v1: monitor, BP cuff, pulse ox, ECG, IV/IO, oxygen delivery.
- UI/UX: HUD for vitals, orders, checklists, timers; pause/menu; localization hooks.
- Acceptance: all devices functionally operate and affect patient state through adapter.

### Phase 3 — Authoring Tools and Assessment (3–5 weeks)
- Scenario Authoring Tooling: inspectors, validators, soft constraints, seed management.
- Assessment/Grading: rubric engine linked to scenario objectives, human‑readable reports.
- Save/Load of authored scenarios and session replays; schema migration strategy.
- Acceptance: non‑engineers can create a scenario and produce a graded session report.

### Phase 4 — Performance, Polish, and Release Prep (2–4 weeks)
- Performance profiling and optimization; memory and GC tuning.
- Accessibility: remappable controls, colorblind‑safe palettes, subtitles.
- Security & Privacy: PII avoidance, offline mode, model request redaction.
- Acceptance: stable 60 FPS on target hardware; no PII in logs; green CI.

### Risks and Mitigations
- Model latency/availability → caching, predictive prefetch, offline fallback tables.
- Nondeterminism across frames → pure functions, fixed update ordering, seed management.
- Scope creep in devices → strict device MVPs, feature flags, backlog triage.

### Milestones (High‑Level)
- M0: Project + CI up; stubbed sim loop and model call.
- M1: Deterministic loop + scenario engine + telemetry.
- M2: Interaction + device MVPs + HUD.
- M3: Authoring + rubric + reports.
- M4: Optimized, shippable desktop build.


