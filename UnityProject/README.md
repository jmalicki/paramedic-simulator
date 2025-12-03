# Unity Sequence Renderer

Renders procedural animation sequences using Unity's recorder in batch mode. Uses FBX files exported from Blender for models and animations, with toon materials applied in Unity.

## Prerequisites

- Unity 2022.3 LTS or later (Unity 6 preferred)
- Unity Recorder package (installed via manifest.json)
- Blender 4.2+ (for FBX export)

## Quick Start: Blender FBX to Unity Video

```bash
# 1. Export FBX from Blender
cd Art/Source/3D/Sequences
blender --background --python compose.py -- --fbx --sequence initial_assessment

# 2. Copy FBX to Unity
cp output/initial_assessment.fbx ../../../UnityProject/Assets/Models/

# 3. Render with Unity
cd ../../../../
./UnityProject/render.sh initial_assessment
```

### Export Individual Procedures

```bash
# Export just pulse ox
blender --background --python compose.py -- --fbx --procedures pulseox_apply

# Export radial pulse
blender --background --python compose.py -- --fbx --procedures radial_pulse

# Export BP cuff
blender --background --python compose.py -- --fbx --procedures bp_cuff_apply
```

### Unity Rendering Options

```bash
# Render from project root
./UnityProject/render.sh initial_assessment

# Custom resolution
./UnityProject/render.sh initial_assessment --resolution 1920x1080
```

## Available Sequences

- `initial_assessment` - Complete assessment sequence (9.5s)
  - Pulse oximeter application
  - Radial pulse check
  - Blood pressure cuff application
- `pulseox_apply` - Pulse ox only (2s)
- `radial_pulse` - Radial pulse check (3s)
- `bp_cuff_apply` - BP cuff application (3s)

## Output

Videos are rendered to: `Art/Source/3D/Sequences/output/<sequence_name>.mp4`

- Format: MP4 (H.264)
- Resolution: 1280x720 (configurable)
- Frame rate: 30 fps
- Quality: High bitrate

## Project Structure

```
UnityProject/
├── Assets/
│   ├── Models/                             # FBX files from Blender
│   │   └── initial_assessment.fbx          # Exported animated sequence
│   ├── Scripts/
│   │   ├── SequenceRecorder.cs             # Main recording controller
│   │   ├── ToonMaterials.cs                # Color palette and materials
│   │   ├── ProceduralModels.cs             # Fallback procedural geometry
│   │   ├── ProcedureAnimations.cs          # Fallback procedural animations
│   │   └── Editor/
│   │       └── BatchModeRecorder.cs        # Batch mode entry point
│   ├── Scenes/
│   │   └── TempRecordingScene.unity        # Recording scene (auto-created)
│   └── Materials/                           # (Future: URP toon shaders)
├── Packages/
│   └── manifest.json                        # Package dependencies
├── ProjectSettings/
│   └── ProjectVersion.txt                   # Unity version
└── render.sh                                # Convenience script
```

## Workflow

### Primary: FBX Import

1. **Blender exports FBX** with models and baked animations
2. **Copy FBX to `Assets/Models/`** - Unity auto-imports
3. **BatchModeRecorder loads FBX** and applies toon materials
4. **Unity Recorder captures** to MP4

### Fallback: Procedural

If no FBX is found, Unity falls back to procedural geometry (ported from Blender):

- `ToonMaterials.cs` - Color palette matching Blender's `materials.py`
- `ProceduralModels.cs` - Geometry matching Blender's `models.py`
- `ProcedureAnimations.cs` - Animations matching Blender's `procedures/*.py`

## FBX Export Settings

The Blender `compose.py --fbx` uses these settings for Unity compatibility:

- **Coordinate system**: Y-up, -Z forward (Unity standard)
- **Scale**: FBX_SCALE_ALL
- **Animations**: Baked, all keyframes preserved
- **Meshes**: Triangulated, modifiers applied
- **Materials**: Names preserved for Unity material mapping

## Material Mapping

Unity maps Blender material names to toon colors:

| Blender Material Name  | Unity Color          |
| ---------------------- | -------------------- |
| `MAT_Glove_glove_blue` | Glove Blue (#6B8BA4) |
| `MAT_Skin_skin_light`  | Skin Light (#FFDAB9) |
| `MAT_PulseOx_Body`     | Light Gray (#E8E8E8) |
| `MAT_PulseOx_Screen`   | Green Emissive       |
| `MAT_BpCuff_Body`      | BP Blue (#2E86AB)    |

## Command Line Arguments

| Argument       | Description             | Default                          |
| -------------- | ----------------------- | -------------------------------- |
| `--sequence`   | Sequence name to render | `initial_assessment`             |
| `--output`     | Output directory        | `Art/Source/3D/Sequences/output` |
| `--resolution` | Video resolution (WxH)  | `1280x720`                       |

## Implementation Status

### Completed

- [x] FBX import from Blender
- [x] Automatic toon material application
- [x] Unity Recorder integration
- [x] Batch mode support
- [x] MP4 output to same path as Blender
- [x] Fallback procedural system

### Future Enhancements

- [ ] URP toon shader (Shader Graph implementation)
- [ ] Timeline-based sequencing
- [ ] Cinemachine camera control
- [ ] Post-processing effects

## Troubleshooting

### FBX not loading

- Ensure FBX is in `Assets/Models/` directory
- Check Unity Console for import errors
- Verify Blender export completed successfully

### Animations not playing

- FBX must have baked animations (not NLA strips)
- Check that Animation component is on root object
- Unity may need to reimport FBX after changes

### Unity not found

Add Unity to PATH or use full path in `render.sh`.

## CI Integration

The current CI uses Blender for video rendering. A future workflow will:

1. Run Blender to export FBX
2. Import into Unity
3. Render with Unity Recorder
4. Upload MP4 artifacts
