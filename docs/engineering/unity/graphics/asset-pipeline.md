## Asset Pipeline (DCC → Unity)

### Source of Truth
- DCC: Blender/Maya for modeling/rigging; Substance Painter/Designer or open equivalents for texturing
- Versioning: Binary assets in LFS (if used) or per‑commit archives; sidecar JSON with provenance and generative parameters

### Naming
- `Env_`, `Prop_`, `Char_`, `Fx_` prefixes; materials `MAT_`, textures `T_Albedo/Rough/Metal/Normal/Mask`
- States: `_Clean`, `_Used`, `_Contam`

### Modeling & UV
- Target poly budgets per graphics spec; enforce consistent texel density
- UV islands straightened for trim sheets; second UV channel for lightmaps if baked

### LODs
- LOD0/1/2 for >5k tris; screen‑space and hysteresis tuned in author scenes

### Textures
- PBR: Albedo (sRGB), Metallic (R), Roughness (G), AO (B) packed; Normal (DXTnm)
- Sizes: 512–2k; power‑of‑two; tileable where applicable; decal atlases

### Import Settings (Unity)
- Textures: `sRGB` for albedo/emissive; `Linear` for masks; compression ASTC/BCn based on platform
- Models: `Optimize Mesh`, `Read/Write` off for shipped assets; scale 1.0; normals/tangents calculated consistently
- Animation: Humanoid rig; root motion off by default; compression keyframe reduction

### Addressables
- Group by scene/module; local catalog in dev, remote for release; labels for `Device`, `Env`, `Char`, `UI`

### Validation
- Pre‑commit checks: missing maps, size overages, non‑POT textures, incorrect color space, naming violations
- Runtime checks: frame budget profiler scenes; material/shader variant stripping report

### Generative Hooks
- Texture steps call ComfyUI graphs with locked prompts and seeds; outputs auto‑named and placed in `Art/Generated` before human curation
- Decal sheet generation uses locked LoRAs and color/roughness clamps; auto‑tile test


