## Lane B — CI/CD + Project Bootstrap

Repo: https://github.com/jmalicki/paramedic-simulator
Branch: lane-b-ci

Context
- Establish CI quality gates and Unity build/test skeleton per `docs/engineering/unity/ci-cd-and-testing.md` and `docs/engineering/pre-commit-setup.md`.

Tasks
- [ ] Ensure pre-commit workflow runs on PRs; add status badges to README
- [ ] Add Unity CI stubs: EditMode tests, PlayMode tests (batchmode), build matrix (Win/macOS)
- [ ] Cache optimization; upload artifacts (JUnit, coverage)
- [ ] Document required secrets (UNITY_LICENSE, etc.)

Constraints
- [ ] Non-interactive CI; no PII in logs

Deliverables
- [ ] CI workflows + docs

Definition of Done
- [ ] CI runs on PRs; artifacts uploaded; gates documented

References
- `docs/engineering/unity/ci-cd-and-testing.md`
- `docs/engineering/pre-commit-setup.md`

