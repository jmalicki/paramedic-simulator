## Lane F — Interaction + Device MVPs

Repo: https://github.com/jmalicki/paramedic-simulator
Branch: lane-f-interaction-devices

Context
- Build interaction system and two device MVPs (Monitor, Oxygen) wired through the Patient Adapter per `docs/engineering/unity/implementation/sprint-3-interaction-devices.md`.

Tasks
- [ ] Input actions (gameplay/UI), ray-based interaction, prompts; toolbelt inventory
- [ ] Device API contracts and event model
- [ ] Patient Monitor UI (HR/RR/SpO2 + NIBP trigger); Oxygen delivery (NC/non-rebreather) with flow rates
- [ ] Bindings: device intents → Adapter → state; UI reflects changes

Constraints
- [ ] UI legibility under lighting; emissive calibration; feature-flag advanced features

Deliverables
- [ ] Runtime + PlayMode tests; docs updates

Definition of Done
- [ ] Devices affect patient state through adapter; interaction UX is responsive and discoverable

References
- `docs/engineering/unity/implementation/sprint-3-interaction-devices.md`

