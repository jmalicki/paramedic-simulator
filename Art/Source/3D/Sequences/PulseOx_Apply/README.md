# Pulse Oximeter Application Sequence

This directory contains the source files for the `Seq_PulseOx_Apply` animation sequence.

## Files

| File | Description |
|------|-------------|
| `create_pulseox_sequence.py` | Blender Python script that creates 3D models and animation |
| `assemble_spritesheet.py` | Python script to combine rendered frames into sprite sheet |
| `renders/` | Directory for rendered frames (created by Blender) |

## Quick Start

### Step 1: Create the Blender Scene

1. Open **Blender** (version 2.8 or later)
2. Go to **Scripting** workspace (top tabs)
3. Click **Open** and select `create_pulseox_sequence.py`
4. Click **Run Script** (or press Alt+P)

The script will:

- Create a finger, pulse oximeter, and gloved hand model
- Apply cel-shaded materials with outlines
- Set up a 4-frame animation
- Configure camera and lighting for sprite rendering

### Step 2: Render the Animation

1. Press **Ctrl+F12** (Render > Render Animation)
2. Wait for all 4 frames to render
3. Frames will be saved to `renders/pulseox_apply_0001.png` through `0004.png`

### Step 3: Assemble the Sprite Sheet

```bash
# Install Pillow if needed
pip install Pillow

# Run the assembly script
python assemble_spritesheet.py
```

This creates:

- `Seq_PulseOx_Apply_Sheet.png` - The sprite sheet (4 frames in a row)
- `Seq_PulseOx_Apply.json` - Animation metadata

### Step 4: Copy to Game Assets

Copy the outputs to the game's sprite directory:

```bash
cp Seq_PulseOx_Apply_Sheet.png ../../../Sprites/Sequences/PulseOx_Apply/
cp Seq_PulseOx_Apply.json ../../../Sprites/Sequences/PulseOx_Apply/
```

## Animation Frames

| Frame | Duration | Event | Description |
|-------|----------|-------|-------------|
| 0 | 500ms | - | Hand approaching with pulse oximeter |
| 1 | 400ms | sfx_click | Opening pulse oximeter clip |
| 2 | 500ms | - | Placing on finger |
| 3 | 600ms | sfx_beep | Clipped on, complete |

**Total duration: 2 seconds**

## Customization

### Changing Colors

Edit `COLORS` dictionary in `create_pulseox_sequence.py`:

```python
COLORS = {
    'skin_light': (1.0, 0.855, 0.725, 1.0),  # RGBA 0-1 range
    'glove_blue': (0.42, 0.545, 0.643, 1.0),
    # ...
}
```

### Changing Frame Timing

Edit `FRAME_DATA` in `assemble_spritesheet.py`:

```python
FRAME_DATA = [
    {"index": 0, "duration": 500, ...},  # Change duration here
    ...
]
```

### Changing Resolution

Edit `CONFIG` in both scripts:

```python
CONFIG = {
    'frame_width': 256,   # Sprite width
    'frame_height': 192,  # Sprite height
    ...
}
```

## Troubleshooting

### "No module named 'bpy'"

The Blender script must be run inside Blender, not from command line.

### "Pillow not installed"

Run: `pip install Pillow`

### Renders are black

Check that lights are enabled in Blender. The script creates Sun lights but they may be disabled.

### Outlines not showing

Outlines use a solidify modifier. Check that modifiers are enabled in render.

## Output Preview

After assembly, the sprite sheet will look like:

```
┌─────────┬─────────┬─────────┬─────────┐
│ Frame 0 │ Frame 1 │ Frame 2 │ Frame 3 │
│ 256x192 │ 256x192 │ 256x192 │ 256x192 │
└─────────┴─────────┴─────────┴─────────┘
Total: 1024 x 192 pixels
```
