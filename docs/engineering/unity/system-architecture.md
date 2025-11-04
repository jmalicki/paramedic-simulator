## System Architecture — Runtime Topology & Data Flow

Purpose: describe how the Unity client interacts with the generative patient model, handles offline operation, and maintains determinism and observability.

### Topology Options
1) In‑Process Model Runner (single‑process)
   - Unity hosts a native/managed runner that loads the model locally.
   - Pros: lowest latency; simpler deployment; fewer moving parts.
   - Cons: memory/VRAM pressure inside editor/player; harder to sandbox; model updates require app updates.

2) Local Model Service (recommended baseline)
   - Unity talks to a localhost service over gRPC (preferred) or HTTP/JSON.
   - Pros: isolates model memory/VRAM usage; independent lifecycle; hot‑swap versions; easier observability; sandboxable.
   - Cons: slightly higher latency; requires a local daemon.

3) Remote Model Service (fallback/ops)
   - Same API, remote host; used for CI, shared labs, or heavy models.
   - Requires strict PII controls and offline capability locally.

We target Option 2 first, with a drop‑in Option 1 path if we later embed the runner.

### Data Flow (per fixed sim tick)
1. Input/intent events collected (interaction, devices, scenario triggers)
2. Patient Adapter builds `PatientStateRequest` from current state + intents + seed + timestamps (SI units)
3. Transport sends request (gRPC/HTTP) to model service; timeout + retries + circuit breaker
4. Response merged into authoritative `PatientState` in sim step (single writer)
5. Scenario evaluates objectives; UI updates; telemetry logs tick and I/O
6. If service unavailable: fallback rules table produces minimally viable vitals

### Contracts & Versioning
- Versioned JSON/proto with explicit units and timestamps
- Golden files for regression; schema migrations on breaking changes

### Determinism & Repro
- Fixed‑step loop; seeded RNG per session
- Replay consumes recorded inputs and replays adapter I/O
- Transport effects (latency) are bounded by timeouts; state updates only at sim steps

### Reliability & Security
- Transport policies: exponential backoff, max retries, circuit breaker, request idempotency
- Allowlist hosts (localhost by default); redact telemetry payloads; no PII

### Why a Client (HTTP/gRPC) Exists
- Even with a local GPU, running the model as a separate service improves isolation, versioning, and observability.
- The same adapter can speak to: in‑process runner (direct call), localhost service (gRPC), or remote (gRPC). The task in Sprint 1 establishes this transport layer so we can swap deployments without touching gameplay code.

### Offline Mode
- If model is down or not installed, adapter switches to rules‑based fallback for essential vitals; logs degraded mode and continues.


