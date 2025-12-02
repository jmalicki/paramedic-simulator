## Tech Stack, Versions, and Packages

### Core

- Unity: 6000 LTS (Unity 6), URP.
- Scripting: C# 10; nullable enabled; Roslyn analyzers.
- Version Control: Git; follow Conventional Commits; small, single‑concern PRs.

### Platforms

- Desktop: Windows 10+, macOS 12+ (Apple Silicon supported).
- Later: VR (Quest 3/Pro), Linux; Multiplayer TBD.

### Unity Packages

- com.unity.inputsystem (actions‑based input)
- com.unity.addressables (content management)
- com.unity.cinemachine, com.unity.timeline
- com.unity.localization
- com.unity.textmeshpro
- com.unity.test-framework
- com.unity.performance.profile-analyzer (editor‑only)

### External Libraries

- HTTP/gRPC client: gRPC for .NET (Grpc.Net.Client), Newtonsoft.Json for robust JSON.
- Dependency Injection: Unity's built‑in Service Locator pattern minimal; consider Zenject only if complexity grows.
- Logging: Serilog‑style structured logging adapter (or Unity Logging + custom sink).

### Project Settings (Key)

- Color space: Linear; URP forward renderer.
- Scripting backend: IL2CPP for release, Mono for CI tests.
- API Compatibility: .NET Standard 2.1.
- Quality: fixed timestep 50 Hz; vSync off; 60 FPS target; incremental GC.

### Coding Standards

- Folder and namespace map 1:1; no God classes.
- Prefer composition over inheritance; interfaces for simulator services.
- Unit tests for pure sim logic; PlayMode tests for interaction and devices.

### Example `Packages/manifest.json` Snippet

```json
{
  "dependencies": {
    "com.unity.render-pipelines.universal": "^17.0.3",
    "com.unity.inputsystem": "^1.8.0",
    "com.unity.addressables": "^1.21.21",
    "com.unity.localization": "^2.5.3",
    "com.unity.timeline": "^1.8.6",
    "com.unity.cinemachine": "^2.9.7",
    "com.unity.test-framework": "^1.4.5",
    "com.unity.textmeshpro": "^3.0.8"
  }
}
```

### Integration Options for Generative Model

- Preferred: gRPC streaming for vitals/events; HTTP/JSON fallback.
- Contracts: versioned JSON with explicit units (SI), timestamps, and seeds.
- Timeouts/retries: exponential backoff, circuit breaker, offline FO rules.
