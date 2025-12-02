# Initial Assessment Sequence Generator

Generates the Phase 0 test sequence video programmatically using Blender.

## Sequence Content

The initial assessment sequence composes three procedures:

| Step | Procedure | Duration | Description |
|------|-----------|----------|-------------|
| 1 | Pulse Oximeter Apply | 2.0s | Clip pulse ox onto patient's finger |
| 2 | Radial Pulse Check | 3.0s | Palpate radial pulse at wrist |
| 3 | BP Cuff Application | 3.0s | Wrap and inflate blood pressure cuff |

Total duration: ~9.5 seconds (including 0.5s transitions)

## Requirements

- **Blender 4.0+** (uses EEVEE Next renderer)
- No additional Python packages required (uses Blender's built-in modules)

### Installing Blender

```bash
# Ubuntu/Debian
sudo apt install blender

# macOS
brew install --cask blender

# Or download from https://www.blender.org/download/
```

## Usage

### Render Video (Headless)

```bash
# Using the wrapper script
./render.sh

# Or directly with Blender
blender --background --python generate_initial_assessment.py
```

Output: `output/initial_assessment.mp4`

### Render as Frame Sequence

```bash
./render.sh --frames
```

Output: `output/frames/frame_0001.png` through `frame_0285.png`

### Preview in Blender GUI

```bash
./render.sh --preview

# Or
blender --python generate_initial_assessment.py
```

This opens Blender with the scene set up. Use `Ctrl+F12` to render.

## Configuration

Edit `generate_initial_assessment.py` to modify:

```python
CONFIG = {
    "resolution_x": 1280,      # Video width
    "resolution_y": 720,       # Video height
    "fps": 30,                 # Frames per second
    "output_dir": "./output",  # Output directory
    ...
}
```

## Output Structure

```
Initial_Assessment/
├── generate_initial_assessment.py  # Main generation script
├── render.sh                       # Convenience wrapper
├── README.md                       # This file
└── output/
    ├── initial_assessment.mp4      # Rendered video
    └── frames/                     # Optional frame sequence
        ├── frame_0001.png
        ├── frame_0002.png
        └── ...
```

## Technical Notes

### Rendering

- Uses **EEVEE Next** for fast rendering (can switch to Cycles for higher quality)
- Cel-shaded materials with toon lighting
- H.264 codec for MP4 output

### Models

The script creates simplified placeholder geometry:
- Patient arm with hand and fingers
- Gloved paramedic hand
- Pulse oximeter with body, clip, and screen
- BP cuff with bulb and tubing

For production, replace with proper high-poly models from Blender artists.

### Animation

All animation is keyframed programmatically:
- Bezier interpolation for smooth motion
- Camera follows the action across procedures
- Transition frames blend between procedures

## Extending

To add new procedures, follow this pattern:

```python
def animate_new_procedure(hand, equipment, start_time):
    start_frame = seconds_to_frame(start_time)

    # Define keyframe times
    f_start = start_frame
    f_action1 = start_frame + seconds_to_frame(0.5)
    f_action2 = start_frame + seconds_to_frame(1.0)
    # ...

    # Set keyframes
    set_keyframe(hand, "location", f_start, (x, y, z))
    set_keyframe(hand, "location", f_action1, (x, y, z))
    # ...
```

Then add to `main()`:

```python
animate_new_procedure(gloved_hand, equipment, new_start_time)
```
