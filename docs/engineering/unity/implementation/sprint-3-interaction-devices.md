## Sprint 3 — Interaction + Device MVPs (3 weeks)

Objective: Implement core interaction (ray + prompts + toolbelt) and two device MVPs (Monitor, Oxygen delivery) wired through the patient adapter.

### Deliverables

- [ ] Interaction system with prompts and toolbelt inventory
- [ ] Device: Patient Monitor (HR, RR, SpO2, NIBP trigger)
- [ ] Device: Oxygen delivery (NC/non‑rebreather, flow rates)
- [ ] Materials library for medical plastics/metals/fabrics; decal MVP

### Tasks

Interaction

- [ ] Input actions: gameplay and UI maps
- [ ] Ray interaction with highlight and context prompts
- [ ] Toolbelt inventory: equip/unequip interactions

Devices

- [ ] Device API contracts and event model
- [ ] Patient Monitor UI (placeholder art) + vitals binding
- [ ] NIBP measurement start/stop logic; simulated cuff animation placeholder
- [ ] Oxygen device with flow selection and mask type
- [ ] Adapter hooks to translate device intents to model inputs

Graphics — Materials & Decals

- [ ] Author base materials (PBR) with consistent rough/metal ranges
- [ ] Decal projector setup; create blood/wear decal atlas MVP

Testing

- [ ] PlayMode: interaction prompts correctness and device action flows
- [ ] Contract tests: device → adapter → model I/O mappings

Docs

- [ ] Update `architecture.md` Interaction and Devices sections
- [ ] Expand `graphics-spec.md` with material guidelines and decal usage

### Acceptance Criteria

- [ ] Both devices affect patient state through adapter; UI reflects changes
- [ ] Interaction is discoverable and responsive; no input conflicts
- [ ] Materials and decals meet readability and performance budgets

### Risks & Mitigations

- UI readability under lighting → emissive calibration pass; HDR clamp
- Over‑complex device logic → MVP scope; defer advanced features to flags
