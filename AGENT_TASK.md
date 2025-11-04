## Lane D — Graphics Foundations + Asset Pipeline

Repo: https://github.com/jmalicki/paramedic-simulator
Branch: lane-d-graphics-pipeline

Context
- Build URP renderer, lighting templates, materials, decals, and import/validation pipeline per `docs/engineering/unity/graphics/graphics-spec.md` and `graphics/asset-pipeline.md`.

Tasks
- [ ] URP renderer asset, quality profiles, baseline post-FX (disabled by default)
- [ ] Lighting templates (key/fill/accent), probe setups, baked GI test scene
- [ ] Materials library (medical plastics/metals/fabrics); decal system MVP
- [ ] Import processors, validation gates, Addressables grouping

Constraints
- [ ] 60 FPS budget; draw calls/triangles per graphics-spec

Deliverables
- [ ] Renderer assets, scenes, validation scripts, docs

Definition of Done
- [ ] Profile scenes meet budgets; import rules enforced

References
- `docs/engineering/unity/graphics/graphics-spec.md`
- `docs/engineering/unity/graphics/asset-pipeline.md`

