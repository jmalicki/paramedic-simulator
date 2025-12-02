# Procedural Animation Sequences

[![Render Animations](https://github.com/jmalicki/paramedic-simulator/actions/workflows/render-animations.yml/badge.svg)](https://github.com/jmalicki/paramedic-simulator/actions/workflows/render-animations.yml)

Modular, reusable animation system for generating medical procedure videos.

## Architecture

```text
Sequences/
├── compose.py              # Main composer script
├── render.sh               # CLI wrapper
├── README.md               # This file
├── common/                 # Shared utilities
│   ├── __init__.py
│   ├── core.py             # Scene setup, timing, keyframes
│   ├── materials.py        # Cel-shading materials, color palette
│   └── models.py           # Shared models (hands, patient arm)
├── procedures/             # Reusable procedure modules
│   ├── __init__.py         # Procedure registry
│   ├── pulseox_apply.py    # Pulse oximeter application
│   ├── radial_pulse.py     # Radial pulse check
│   └── bp_cuff_apply.py    # Blood pressure cuff
└── output/                 # Rendered videos
```

## Quick Start

```bash
# Render the initial assessment sequence (pulse ox + radial pulse + BP)
./render.sh

# List available procedures
./render.sh --list

# Render specific procedures in order
./render.sh -p pulseox_apply radial_pulse

# Preview in Blender GUI
./render.sh --preview
```

## Usage

### Render Predefined Sequences

```bash
# Initial assessment (default)
./render.sh -s initial_assessment

# Or using Blender directly
blender --background --python compose.py -- --sequence initial_assessment
```

### Compose Custom Sequences

```bash
# Render just pulse ox and radial pulse
./render.sh -p pulseox_apply radial_pulse

# With custom output name
./render.sh -p pulseox_apply radial_pulse -o my_sequence

# Custom transition time (default 0.5s)
./render.sh -p pulseox_apply radial_pulse -t 1.0
```

### Preview in Blender

```bash
./render.sh --preview
# Then press Ctrl+F12 to render
```

## Procedure Module Interface

Each procedure module exports:

```python
# Metadata
NAME = "procedure_name"
DESCRIPTION = "Human-readable description"
DURATION = 2.0  # seconds

# Camera focus point (optional)
CAMERA_FOCUS = {
    "location": (x, y, z),
    "rotation_deg": (rx, ry, rz),
}

# Create procedure-specific equipment
def create_equipment() -> dict:
    """Returns dict of equipment objects"""
    pass

# Animate the procedure
def animate(context: dict, start_time: float) -> float:
    """
    Args:
        context: {'hand': Object, 'patient_arm': Object, 'camera': Object, ...}
        start_time: When to start (seconds)
    Returns:
        End time (seconds)
    """
    pass
```

## Adding New Procedures

1. Create `procedures/new_procedure.py`:

```python
"""
New Procedure Description
"""

import bpy
import math
import sys
import os

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from common.core import seconds_to_frame, set_keyframe
from common.materials import create_toon_material, COLORS

NAME = "new_procedure"
DESCRIPTION = "What this procedure does"
DURATION = 2.5

CAMERA_FOCUS = {
    "location": (0, -0.4, 0.25),
    "rotation_deg": (75, 0, 0),
}

def create_equipment():
    """Create any equipment specific to this procedure."""
    # Create your equipment here
    return {'equipment_name': equipment_object}

def animate(context, start_time):
    """Animate the procedure."""
    hand = context['hand']
    equipment = context.get('equipment_name')
    camera = context.get('camera')

    start_frame = seconds_to_frame(start_time)

    # Add your keyframes here
    set_keyframe(hand, "location", start_frame, (x, y, z))
    # ...

    return start_time + DURATION
```

1. Register in `procedures/__init__.py`:

```python
from . import new_procedure

PROCEDURES = {
    # ... existing ...
    'new_procedure': new_procedure,
}
```

1. Use it:

```bash
./render.sh -p pulseox_apply new_procedure bp_cuff_apply
```

## Configuration

Edit `common/core.py` to change defaults:

```python
CONFIG = {
    "resolution_x": 1280,
    "resolution_y": 720,
    "fps": 30,
}
```

Edit `common/materials.py` for the color palette:

```python
COLORS = {
    "glove_blue": (0.420, 0.545, 0.643, 1.0),
    "skin_light": (1.0, 0.855, 0.725, 1.0),
    # ...
}
```

## Technical Notes

- **Blender 4.0+** required (uses EEVEE Next)
- All timing is in seconds; use `seconds_to_frame()` for frame numbers
- Equipment models are created once and reused across the sequence
- Camera transitions are auto-generated between procedures
- Keyframes use Bezier interpolation for smooth motion

## Available Procedures

| Procedure       | Duration | Description                     |
| --------------- | -------- | ------------------------------- |
| `pulseox_apply` | 2.0s     | Clip pulse oximeter onto finger |
| `radial_pulse`  | 3.0s     | Palpate radial pulse at wrist   |
| `bp_cuff_apply` | 3.0s     | Wrap and inflate BP cuff        |

## Predefined Sequences

| Sequence             | Procedures            | Description                |
| -------------------- | --------------------- | -------------------------- |
| `initial_assessment` | pulseox + radial + bp | Complete initial vitals    |
| `vital_signs`        | pulseox + radial + bp | Same as initial_assessment |

## CI / GitHub Actions

Animations are automatically rendered in CI when changes are pushed to the `Art/Source/3D/Sequences/` directory.

### Automatic Rendering

- **On push/PR**: Renders `initial_assessment` sequence
- **On merge to main**: Creates a GitHub Release with the video attached

### Manual Rendering

Trigger renders manually from the Actions tab:

1. **Render Animations** - Render full sequences

   - Go to Actions → Render Animations → Run workflow
   - Select sequence and resolution

2. **Render Single Procedure** - Render individual procedures
   - Go to Actions → Render Single Procedure → Run workflow
   - Select procedure from dropdown

### Downloading Videos

- **From Pull Requests**: Download from the Artifacts section
- **From Releases**: Download from the Releases page (main branch only)

### Workflow Files

```text
.github/workflows/
├── render-animations.yml    # Full sequence rendering + releases
└── render-procedure.yml     # Single procedure rendering
```
