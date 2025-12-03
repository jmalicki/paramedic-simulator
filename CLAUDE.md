# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

A paramedic training simulator with a Unity game client and a Python-based research pipeline for analyzing NEMSIS EMS data.

## Commands

### Pre-commit Setup

```bash
pip install pre-commit
pre-commit install
```

### Research Pipeline (NEMSIS data analysis)

Requires NEMSIS parquet files in `research/inputs/nemsis/`.

```bash
# Install dependencies
pip install -r requirements.txt

# Run pipeline steps (specify years)
python -m research.scripts.nemsis.load_and_clean 2019 2020 2021 2022 2023 2024
python -m research.scripts.nemsis.select_cases 2019 2020 2021 2022 2023 2024
python -m research.scripts.nemsis.compute_measures 2019 2020 2021 2022 2023 2024
```

### Blender Animation Rendering

```bash
# Render procedural sequences with Blender
cd Art/Source/3D/Sequences
blender --background --python compose.py
```

### Unity Sequence Rendering

```bash
# Render animation sequences via Unity batch mode
./UnityProject/render.sh initial_assessment
./UnityProject/render.sh pulseox_apply
./UnityProject/render.sh radial_pulse
./UnityProject/render.sh bp_cuff_apply
```

## Architecture

### Unity Client (`UnityProject/`)

- **Unity 6 (6000 LTS)** with URP, C# 10, nullable enabled
- **Key runtime modules** (under `Assets/Scripts/Runtime/`):
  - `Core/` - Game loop, time control, DI, services
  - `Simulation/` - Deterministic sim systems, patient gateway
  - `Scenario/` - Objectives, scoring, triggers, state machine
  - `Interaction/` - Input System package, raycasts, toolbelt
  - `Devices/` - Monitor, BP, ECG, oxygen (each encapsulates UI + logic)
  - `Persistence/` - Session recording, replay, telemetry (JSONL)
  - `Integrations/` - HTTP/gRPC client for patient model service

- **Determinism**: Fixed-step loop, single-writer patient state, seeded RNG, explicit SI units
- **Patient Model Adapter**: Talks to local or remote model service (gRPC preferred, HTTP/JSON fallback); falls back to rules table when offline

### Research Pipeline (`research/`)

- Python scripts for NEMSIS EMS data analysis
- `codebooks/` - YAML definitions for case selection, age bands, ICD-10 codes
- `scripts/nemsis/` - load_and_clean, select_cases, compute_measures
- Outputs to `research/outputs/nemsis/`

### Art Pipeline (`Art/`)

- `Source/3D/Sequences/` - Blender Python scripts for procedural medical animations
  - `common/` - Shared utilities (core, materials, models)
  - `procedures/` - Individual procedure animations (pulseox, radial pulse, bp cuff)
- `Sprites/` - SVG assets for characters, props, UI

## Code Style

- **Commits**: Use Conventional Commits (e.g., `feat(unity): add sim clock`)
- **C#**: 4 spaces indent, nullable enabled, Roslyn analyzers
- **Pre-commit hooks**: markdownlint, yamllint, prettier (md/json/yml)

## Testing

- **Unity EditMode tests**: Pure sim logic, adapters, rubric evaluation
- **Unity PlayMode tests**: Interaction flows, device UIs, scenario progression
- **CI**: GitHub Actions runs pre-commit on all PRs; Unity tests via game-ci

## Key Documentation

- `docs/engineering/unity/architecture.md` - Unity project structure and core systems
- `docs/engineering/unity/system-architecture.md` - Runtime topology and data flow
- `docs/engineering/unity/tech-stack-and-versions.md` - Packages and settings
- `docs/research/` - Medical condition research (CHF, opioid overdose, trauma)
