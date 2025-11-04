## Lane C — Scenario Engine + Telemetry/Replay

Repo: https://github.com/jmalicki/paramedic-simulator
Branch: lane-c-scenario-telemetry

Context
- Implement scenario engine (states, triggers, objectives) and full session telemetry/replay per `docs/engineering/unity/implementation/sprint-2-scenario-telemetry.md`.

Tasks
- [ ] Scenario states: Initial → Running → Success/Fail/Timeout/Aborted
- [ ] Triggers: time/action/threshold; weighted objectives and rubric stub
- [ ] Telemetry JSONL (tick, inputs, state deltas, model I/O); privacy scrubbing; optional encryption
- [ ] Deterministic replay runner; HUD prototype (vitals/objectives/timer)

Constraints
- [ ] Deterministic replay; versioned schemas; no PII

Deliverables
- [ ] Runtime + tests + docs updates

Definition of Done
- [ ] Scripted scenario runs end-to-end; replay parity verified

References
- `docs/engineering/unity/implementation/sprint-2-scenario-telemetry.md`

