#!/usr/bin/env python3
"""
Sprite Sheet Assembly Script
============================
Combines individual rendered frames into a sprite sheet and generates JSON metadata.

Requirements:
    pip install Pillow

Usage:
    python assemble_spritesheet.py

Input:  renders/pulseox_apply_0001.png through pulseox_apply_0004.png
Output: Seq_PulseOx_Apply_Sheet.png and Seq_PulseOx_Apply.json
"""

import os
import json
from pathlib import Path

try:
    from PIL import Image
except ImportError:
    print("Error: Pillow not installed. Run: pip install Pillow")
    exit(1)

# =============================================================================
# CONFIGURATION
# =============================================================================

CONFIG = {
    'sequence_name': 'PulseOx_Apply',
    'input_dir': 'renders',
    'input_pattern': 'pulseox_apply_{:04d}.png',
    'output_sheet': 'Seq_PulseOx_Apply_Sheet.png',
    'output_json': 'Seq_PulseOx_Apply.json',
    'frame_count': 4,
    'frames_per_row': 4,
    'frame_width': 256,
    'frame_height': 192,
}

# Frame metadata (timing and events)
FRAME_DATA = [
    {
        "index": 0,
        "duration": 500,
        "event": None,
        "label": "approach",
        "description": "Hand approaching with pulse oximeter"
    },
    {
        "index": 1,
        "duration": 400,
        "event": "sfx_click",
        "label": "open_clip",
        "description": "Opening pulse oximeter clip"
    },
    {
        "index": 2,
        "duration": 500,
        "event": None,
        "label": "place_on_finger",
        "description": "Placing pulse oximeter on finger"
    },
    {
        "index": 3,
        "duration": 600,
        "event": "sfx_beep",
        "label": "complete",
        "description": "Pulse oximeter clipped on, reading displayed"
    }
]


# =============================================================================
# SPRITE SHEET ASSEMBLY
# =============================================================================

def load_frames(input_dir, pattern, count):
    """Load individual frame images."""
    frames = []
    for i in range(1, count + 1):
        filename = pattern.format(i)
        filepath = os.path.join(input_dir, filename)

        if not os.path.exists(filepath):
            print(f"Warning: Frame not found: {filepath}")
            # Create placeholder for missing frames
            placeholder = Image.new('RGBA', (CONFIG['frame_width'], CONFIG['frame_height']), (255, 0, 255, 128))
            frames.append(placeholder)
        else:
            img = Image.open(filepath)
            # Resize if needed
            if img.size != (CONFIG['frame_width'], CONFIG['frame_height']):
                img = img.resize((CONFIG['frame_width'], CONFIG['frame_height']), Image.LANCZOS)
            frames.append(img)
            print(f"Loaded: {filepath}")

    return frames


def create_spritesheet(frames, frames_per_row):
    """Combine frames into a sprite sheet."""
    frame_count = len(frames)
    rows = (frame_count + frames_per_row - 1) // frames_per_row

    sheet_width = CONFIG['frame_width'] * frames_per_row
    sheet_height = CONFIG['frame_height'] * rows

    sheet = Image.new('RGBA', (sheet_width, sheet_height), (0, 0, 0, 0))

    for i, frame in enumerate(frames):
        x = (i % frames_per_row) * CONFIG['frame_width']
        y = (i // frames_per_row) * CONFIG['frame_height']
        sheet.paste(frame, (x, y))

    return sheet


def generate_metadata():
    """Generate JSON metadata for the sequence."""
    rows = (CONFIG['frame_count'] + CONFIG['frames_per_row'] - 1) // CONFIG['frames_per_row']

    metadata = {
        "name": CONFIG['sequence_name'],
        "description": "Applying pulse oximeter to patient's finger",
        "spriteSheet": CONFIG['output_sheet'],
        "frameWidth": CONFIG['frame_width'],
        "frameHeight": CONFIG['frame_height'],
        "framesPerRow": CONFIG['frames_per_row'],
        "totalFrames": CONFIG['frame_count'],
        "sheetWidth": CONFIG['frame_width'] * CONFIG['frames_per_row'],
        "sheetHeight": CONFIG['frame_height'] * rows,
        "defaultFrameMs": 500,
        "totalDurationMs": sum(f["duration"] for f in FRAME_DATA),
        "frames": FRAME_DATA,
        "outcomes": {
            "success": {
                "gotoFrame": 3,
                "nextState": "monitoring_spo2",
                "event": "sfx_success"
            }
        },
        "patientVariants": [
            "skin_light",
            "skin_medium",
            "skin_dark"
        ]
    }

    return metadata


# =============================================================================
# MAIN
# =============================================================================

def main():
    """Main function."""
    print("=" * 60)
    print("Sprite Sheet Assembly")
    print("=" * 60)

    # Get script directory
    script_dir = Path(__file__).parent
    os.chdir(script_dir)

    # Check for renders directory
    input_dir = CONFIG['input_dir']
    if not os.path.exists(input_dir):
        print(f"\nRenders directory not found: {input_dir}")
        print("Please render the Blender animation first:")
        print("  1. Open Blender")
        print("  2. Run create_pulseox_sequence.py")
        print("  3. Render > Render Animation (Ctrl+F12)")
        print("")
        print("Creating placeholder sprite sheet for testing...")

        # Create placeholders
        os.makedirs(input_dir, exist_ok=True)
        frames = []
        colors = [(200, 100, 100), (100, 200, 100), (100, 100, 200), (200, 200, 100)]
        for i, color in enumerate(colors):
            img = Image.new('RGBA', (CONFIG['frame_width'], CONFIG['frame_height']), (*color, 200))
            frames.append(img)
            # Save placeholder frame
            img.save(os.path.join(input_dir, CONFIG['input_pattern'].format(i + 1)))
    else:
        # Load actual rendered frames
        frames = load_frames(input_dir, CONFIG['input_pattern'], CONFIG['frame_count'])

    # Create sprite sheet
    print("\nCreating sprite sheet...")
    sheet = create_spritesheet(frames, CONFIG['frames_per_row'])
    sheet.save(CONFIG['output_sheet'])
    print(f"Saved: {CONFIG['output_sheet']}")

    # Generate and save metadata
    print("\nGenerating metadata...")
    metadata = generate_metadata()

    with open(CONFIG['output_json'], 'w') as f:
        json.dump(metadata, f, indent=2)
    print(f"Saved: {CONFIG['output_json']}")

    print("\n" + "=" * 60)
    print("Done!")
    print(f"Sprite sheet: {CONFIG['output_sheet']}")
    print(f"Metadata: {CONFIG['output_json']}")
    print("=" * 60)


if __name__ == "__main__":
    main()
