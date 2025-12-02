## Sprint 1 — Foundations (2 weeks)

Objective: Stand up the Unity project, establish deterministic simulation scaffolding, define model contracts, and set up CI, profiling scenes, and URP baseline.

### Deliverables

- [ ] Unity project (6000 LTS, URP) committed with baseline settings
- [ ] Deterministic fixed‑step loop with time controls
- [ ] Model contract schemas (proto/JSON) and client shim
- [ ] CI pipelines for tests and builds with artifacts
- [ ] Graphics baseline: URP renderer asset, post‑FX profile, profiling scenes

### Tasks

Project bootstrap

- [ ] Create `UnityProject` with URP template
- [ ] Configure Project Settings (Linear, forward renderer, incremental GC, 50 Hz fixed timestep)
- [ ] Add packages: Input System, Addressables, TextMeshPro, Localization, Test Framework, Cinemachine
- [ ] Namespace policy and analyzers enabled; nullable reference types on

Repo & CI

- [ ] Create `.github/workflows/unity-ci.yml` for test + build
- [ ] Cache Library where safe; upload JUnit and coverage artifacts
- [ ] Secret handling documented for license and future notarization

Simulation core

- [ ] Implement `SimulationClock` with pause/speed (0.5×/1×/2×) and single update event
- [ ] Author `SimulationState` POCO and transition interface (pure function where possible)
- [ ] Add seeded RNG service, session seed serialization

Model integration groundwork

- [ ] Define `patient_state_request` and `patient_state_response` schemas (units in SI)
- [ ] Implement Patient Adapter transport: EITHER in‑process runner call OR localhost gRPC/HTTP client with timeout, retry, and circuit breaker policy
- [ ] Add JSON/proto golden samples and contract tests

Graphics baseline

- [ ] Create URP Renderer asset and Quality settings profiles (Dev/Release)
- [ ] Post‑FX: Bloom, Color Adjustments, Vignette profiles (disabled by default)
- [ ] Profiling scenes: Empty, 1k props, shadows on/off; capture baseline fps

Testing

- [ ] EditMode tests for SimulationClock and state transitions
- [ ] PlayMode bootstrap test for project sanity (scene loads headless)

Docs

- [ ] Update `tech-stack-and-versions.md` with exact package versions used
- [ ] Record baseline performance numbers in `graphics-spec.md`

### Acceptance Criteria

- [ ] CI green: tests run on PR and produce artifacts; dev builds for Win/macOS
- [ ] Sim loop deterministic over replay of recorded inputs (unit test verifies)
- [ ] Golden schema round‑trips pass; breaking changes flagged in tests
- [ ] Profiling scenes render within budget on target dev machine

### Risks & Mitigations

- Package version friction → lock versions; document in tech stack
- License automation issues → fallback manual until license resolved; keep build job optional
