## Pre-commit Setup

### Install

- Install Python 3.x
- `pip install pre-commit`
- In repo root: `pre-commit install --install-hooks` and `pre-commit install -t commit-msg`

### What runs

- Basic file checks (EOL, trailing whitespace, YAML/JSON)
- markdownlint, yamllint
- cspell with project dictionary
- Prettier for docs/configs
- Commitizen (commit message conventional commit check)

### CI

- GitHub Actions runs `pre-commit` on PRs. Fix locally before pushing for faster iteration.
