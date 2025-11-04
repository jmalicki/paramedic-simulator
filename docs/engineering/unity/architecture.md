## Unity Architecture — Paramedic Simulator

### Project Structure
```
UnityProject/
  Assets/
    Scripts/
      Runtime/
        Core/                // Game loop, time, DI, services
        Simulation/          // Deterministic sim systems, patient gateway
        Scenario/            // Objectives, scoring, triggers
        Interaction/         // Input, raycasts, toolbelt, inventory
        Devices/             // Monitor, BP, ECG, oxygen, lines
        UI/                  // HUD, menus, localizable strings
        Audio/
        Persistence/         // Save/Load, replay, telemetry
        Integrations/        // HTTP/gRPC client to model service
      Editor/
    Art/
    Addressables/
  ProjectSettings/
  Packages/
```

### Core Systems
- Simulation Core
  - Fixed‑step deterministic loop; single writer principle for patient state.
  - Time controls: pause, speed 0.5×/1×/2×; step‑through for teaching.

- Patient Model Adapter
  - Transport: HTTP/2 gRPC preferred; HTTP/JSON fallback; exponential backoff.
  - Contracts: versioned JSON schema, explicit units (SI), timestamps, seed.
  - Caching: memoize recent requests by input hash; TTL by policy.
  - Offline: rules table fallback for critical vitals when model unavailable.

- Scenario Engine
  - State machine per scenario: initial, running, success, fail, timeout, aborted.
  - Triggers: time‑based, action‑based, physiological thresholds.
  - Rubric: weighted criteria; produces assessment with human‑readable rationales.

- Interaction System
  - Input System package; action maps for gameplay/UI; interaction rays + prompts.
  - Toolbelt inventory; contextual affordances; simple IK for hands optional later.

- Devices
  - Each device encapsulates UI + logic; communicates via Device API to Adapter.
  - Device actions emit events (e.g., start NIBP measurement) consumed by sim.

- UI/UX
  - HUD: vitals, timers, objectives; device UIs in-world or screen‑space overlay.
  - Localization with Smart Strings; high contrast and colorblind‑safe palettes.

- Persistence & Telemetry
  - Session recording: every tick input, state, and model I/O to a compact JSONL.
  - Replay: deterministic re‑execution consuming recorded inputs.
  - Privacy: scrub identifiers; optional encryption of artifacts at rest.

### Data Flow
1. Player action → Interaction System emits command.
2. Device or Scenario translates command → Simulation intent.
3. Patient Model Adapter builds request from current state + intents.
4. Adapter requests model; merges response into authoritative state.
5. Scenario evaluates objectives; UI updates; telemetry logs tick.

### Determinism Strategy
- Single authoritative patient state updated only in the sim step.
- Pure functions for state transition where possible; seeded RNG per session.
- Explicit unit conversions; no reliance on `Time.deltaTime` inside sim code.

### Error Handling & Reliability
- Unified Result type and guardrails around network/IO with retries and circuit breakers.
- Health checks for model service; degrade gracefully to offline rules.

### Performance Targets
- 60 FPS on mid‑range hardware; CPU for sim < 2 ms; GC allocs < 1 KB/frame avg.
- Use Addressables for content; pool common prefabs; bake lighting.

### Security & Privacy
- No PII in logs; redact payloads; optional offline‑only mode.
- All outbound traffic behind a single client with allowlisted hosts.

### Extensibility
- Feature flags per device and scenario systems.
- ScriptableObjects for scenarios/devices; JSON schema for model I/O.


