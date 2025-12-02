## Contributing Guide

### Branching & Commits

- Create a feature branch from `main`.
- Use Conventional Commits in PR titles and commits (e.g., `feat(unity): add sim clock`).

### Pre-commit Hooks

- Install: `pip install pre-commit` then `pre-commit install`.
- Hooks run on staged files: markdownlint, yamllint, cspell, prettier, basic file checks.

### Code Style

- Editor config enforced via `.editorconfig`.
- C# uses 4 spaces indent, nullable enabled, analyzers recommended.

### PRs

- Include tests when possible (EditMode/PlayMode for Unity).
- Update docs under `docs/engineering/unity/` when changing architecture or processes.

### Secrets & Privacy

- Never commit secrets or PII. Use `.env` locally and secret stores in CI.
