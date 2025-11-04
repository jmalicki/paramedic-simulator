## Lane A — Core Simulation + Patient Adapter

Repo: https://github.com/jmalicki/paramedic-simulator
Branch: lane-a-sim-adapter

Context
- Implement deterministic sim loop and Patient Adapter per `docs/engineering/unity/system-architecture.md` and `docs/engineering/unity/implementation/sprint-1-foundations.md`.

Tasks
- [ ] Fixed-step SimulationClock (pause, 0.5×/1×/2×), single-writer updates
- [ ] Define `PatientStateRequest/Response v0.1` (SI units, timestamps)
- [ ] Patient Adapter transport: in-process call OR localhost gRPC/HTTP with timeout/retry/circuit breaker
- [ ] Seeded RNG per session; serialize seed in telemetry
- [ ] Golden JSON/proto samples and contract tests

Constraints
- [ ] Deterministic; avoid `Time.deltaTime` in sim state transitions
- [ ] Pure functions for transitions where possible; versioned contracts; no PII

Deliverables
- [ ] Runtime code + unit tests
- [ ] Golden files + brief README

Definition of Done
- [ ] EditMode tests green; adapter is swappable without changing gameplay

References
- `docs/engineering/unity/system-architecture.md`
- `docs/engineering/unity/tech-stack-and-versions.md`
- `docs/engineering/unity/implementation/sprint-1-foundations.md`

