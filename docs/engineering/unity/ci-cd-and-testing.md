## CI/CD, Build Targets, and Testing Strategy

### CI/CD (GitHub Actions)

- Triggers: PRs to feature branches, PRs to `main`, tags `v*` for releases.
- Jobs:
  - Lint & Static Analysis: dotnet analyzers, code style check, asset GUID consistency.
  - EditMode Tests: run on Linux (Mono backend) headless.
  - PlayMode Tests: run on Windows/macOS runners (batchmode + nographics).
  - Build Matrix: Win64, macOS (Intel/Apple Silicon), dev configuration with symbols.
  - Artifact Upload: zipped builds, test reports (JUnit), coverage, telemetry samples.

### Required Secrets

- UNITY_LICENSE (or ULF), APPLE_NOTARIZATION creds (later), SENTRY_DSN (optional).

### Example GitHub Action (conceptual)

```yaml
name: unity-ci
on: [push, pull_request]
jobs:
  tests:
    uses: game-ci/unity-test-runner/.github/workflows/test.yml@v4
    with:
      unityVersion: 6000.0.XXf1
      testMode: all
  build:
    needs: tests
    uses: game-ci/unity-builder/.github/workflows/build.yml@v4
    with:
      unityVersion: 6000.0.XXf1
      targetPlatform: StandaloneWindows64, StandaloneOSX
```

### Testing Strategy

- Unit (EditMode): pure sim logic, adapters, rubric evaluation.
- Integration (PlayMode): interaction flows, device UIs, scenario progression.
- Contract Tests: JSON/proto schemas; golden files for vitals and device I/O.
- Performance: automated frame time budget checks (Profile Analyzer baselines).

### Build Targets

- Dev builds with debug UI and telemetry; Release builds with IL2CPP, managed stripping.
- Versioning: SemVer from tags; embed build metadata in `About` panel.

### Quality Gates

- All tests green; coverage threshold for Simulation and Scenario modules.
- No new allocations above baseline in hot paths; size budget for builds.
