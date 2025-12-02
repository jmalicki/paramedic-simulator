#!/usr/bin/env python3
"""
Sequence Composer

Composes multiple reusable procedure modules into a single animated sequence.

Usage:
    # Render default initial assessment sequence
    blender --background --python compose.py

    # Render with custom procedure list
    blender --background --python compose.py -- --procedures pulseox_apply radial_pulse

    # Preview in Blender GUI
    blender --python compose.py

    # List available procedures
    blender --background --python compose.py -- --list

    # Custom output
    blender --background --python compose.py -- --output my_sequence --dir ./renders
"""

import bpy
import sys
import os
import argparse
import math

# Add this directory to path for imports
SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, SCRIPT_DIR)

from common.core import (
    CONFIG,
    clear_scene,
    setup_scene,
    create_camera,
    create_lights,
    seconds_to_frame,
    set_keyframe,
)
from common.models import create_patient_arm, create_gloved_hand
from procedures import PROCEDURES, get_procedure, list_procedures


# =============================================================================
# DEFAULT SEQUENCES
# =============================================================================

PREDEFINED_SEQUENCES = {
    "initial_assessment": {
        "name": "Initial Assessment",
        "description": "Pulse ox, radial pulse, and BP check",
        "procedures": ["pulseox_apply", "radial_pulse", "bp_cuff_apply"],
        "transition_time": 0.5,
    },
    "vital_signs": {
        "name": "Vital Signs",
        "description": "Complete vital signs assessment",
        "procedures": ["pulseox_apply", "radial_pulse", "bp_cuff_apply"],
        "transition_time": 0.5,
    },
}


# =============================================================================
# COMPOSER
# =============================================================================

class SequenceComposer:
    """Composes multiple procedures into a single animated sequence."""

    def __init__(self, procedure_names, transition_time=0.5):
        """
        Initialize the composer.

        Args:
            procedure_names: List of procedure module names to compose
            transition_time: Seconds between procedures for transitions
        """
        self.procedure_names = procedure_names
        self.transition_time = transition_time
        self.procedures = []
        self.context = {}
        self.total_duration = 0

        # Load procedure modules
        for name in procedure_names:
            proc = get_procedure(name)
            if proc is None:
                raise ValueError(f"Unknown procedure: {name}. Available: {list_procedures()}")
            self.procedures.append(proc)

        # Calculate total duration
        self._calculate_duration()

    def _calculate_duration(self):
        """Calculate total sequence duration."""
        self.total_duration = 0
        for i, proc in enumerate(self.procedures):
            self.total_duration += proc.DURATION
            if i < len(self.procedures) - 1:
                self.total_duration += self.transition_time

    def setup(self):
        """Set up the scene with shared models."""
        print("=" * 60)
        print("Sequence Composer")
        print("=" * 60)
        print(f"Procedures: {', '.join(self.procedure_names)}")
        print(f"Total duration: {self.total_duration:.1f}s")
        print("-" * 60)

        # Clear and setup scene
        print("Setting up scene...")
        clear_scene()
        setup_scene(self.total_duration + 0.5)

        # Create shared elements
        print("Creating camera and lights...")
        camera = create_camera()
        create_lights()

        print("Creating shared models...")
        patient_arm = create_patient_arm()
        hand = create_gloved_hand()

        # Initialize context with shared objects
        self.context = {
            'camera': camera,
            'patient_arm': patient_arm,
            'hand': hand,
        }

        # Create equipment for each procedure
        print("Creating procedure equipment...")
        for proc in self.procedures:
            equipment = proc.create_equipment()
            self.context.update(equipment)

        return self.context

    def animate(self):
        """Animate all procedures in sequence."""
        print("Creating animations...")

        current_time = 0
        camera = self.context.get('camera')

        for i, proc in enumerate(self.procedures):
            print(f"  [{i+1}/{len(self.procedures)}] {proc.DESCRIPTION} @ {current_time:.1f}s")

            # Animate this procedure
            end_time = proc.animate(self.context, current_time)

            # Add transition time before next procedure
            if i < len(self.procedures) - 1:
                # Animate camera transition
                if camera:
                    next_proc = self.procedures[i + 1]
                    transition_start = seconds_to_frame(end_time)
                    transition_end = seconds_to_frame(end_time + self.transition_time)

                    # Smooth camera move to next procedure's focus
                    if hasattr(next_proc, 'CAMERA_FOCUS'):
                        focus = next_proc.CAMERA_FOCUS
                        set_keyframe(camera, "location", transition_end, focus["location"])
                        set_keyframe(camera, "rotation_euler", transition_end, (
                            math.radians(focus["rotation_deg"][0]),
                            math.radians(focus["rotation_deg"][1]),
                            math.radians(focus["rotation_deg"][2]),
                        ))

                current_time = end_time + self.transition_time
            else:
                current_time = end_time

        # Smooth all keyframes
        print("Smoothing keyframes...")
        from common.core import smooth_all_keyframes
        smooth_all_keyframes = getattr(
            __import__('common.core', fromlist=['smooth_all_keyframes']),
            'smooth_all_keyframes',
            lambda: None
        )
        # Apply smooth interpolation
        for obj in bpy.data.objects:
            if obj.animation_data and obj.animation_data.action:
                for fcurve in obj.animation_data.action.fcurves:
                    for keyframe in fcurve.keyframe_points:
                        keyframe.interpolation = 'BEZIER'
                        keyframe.handle_left_type = 'AUTO_CLAMPED'
                        keyframe.handle_right_type = 'AUTO_CLAMPED'

    def render(self, output_name=None, output_dir=None):
        """Render the composed sequence."""
        if output_dir:
            CONFIG["output_dir"] = output_dir
        if output_name:
            CONFIG["output_filename"] = output_name

        output_path = os.path.join(
            CONFIG["output_dir"] or os.path.join(SCRIPT_DIR, "output"),
            CONFIG["output_filename"]
        )
        os.makedirs(os.path.dirname(output_path) or ".", exist_ok=True)

        bpy.context.scene.render.filepath = output_path

        print("-" * 60)
        print(f"Rendering to: {output_path}.mp4")
        print(f"Resolution: {CONFIG['resolution_x']}x{CONFIG['resolution_y']} @ {CONFIG['fps']}fps")
        print(f"Frames: {bpy.context.scene.frame_start} - {bpy.context.scene.frame_end}")

        if bpy.app.background:
            bpy.ops.render.render(animation=True)
            print("=" * 60)
            print("COMPLETE!")
            print(f"Output: {output_path}.mp4")
            print("=" * 60)
        else:
            print("Running in interactive mode.")
            print("Press Ctrl+F12 to render, or run with --background flag.")


# =============================================================================
# CLI INTERFACE
# =============================================================================

def parse_args():
    """Parse command line arguments."""
    # Find args after "--" (Blender passes args after --)
    try:
        idx = sys.argv.index("--")
        args = sys.argv[idx + 1:]
    except ValueError:
        args = []

    parser = argparse.ArgumentParser(
        description="Compose procedural animation sequences"
    )
    parser.add_argument(
        "--procedures", "-p",
        nargs="+",
        help="List of procedure names to compose"
    )
    parser.add_argument(
        "--sequence", "-s",
        choices=list(PREDEFINED_SEQUENCES.keys()),
        help="Use a predefined sequence"
    )
    parser.add_argument(
        "--output", "-o",
        default="composed_sequence",
        help="Output filename (without extension)"
    )
    parser.add_argument(
        "--dir", "-d",
        default=os.path.join(SCRIPT_DIR, "output"),
        help="Output directory"
    )
    parser.add_argument(
        "--transition", "-t",
        type=float,
        default=0.5,
        help="Transition time between procedures (seconds)"
    )
    parser.add_argument(
        "--list", "-l",
        action="store_true",
        help="List available procedures and exit"
    )
    parser.add_argument(
        "--list-sequences",
        action="store_true",
        help="List predefined sequences and exit"
    )

    return parser.parse_args(args)


def main():
    """Main entry point."""
    args = parse_args()

    # List procedures
    if args.list:
        print("Available procedures:")
        for name in list_procedures():
            proc = get_procedure(name)
            print(f"  {name}: {proc.DESCRIPTION} ({proc.DURATION}s)")
        return

    # List predefined sequences
    if args.list_sequences:
        print("Predefined sequences:")
        for name, seq in PREDEFINED_SEQUENCES.items():
            print(f"  {name}: {seq['description']}")
            print(f"    Procedures: {', '.join(seq['procedures'])}")
        return

    # Determine which procedures to use
    if args.sequence:
        seq = PREDEFINED_SEQUENCES[args.sequence]
        procedure_names = seq["procedures"]
        transition = seq.get("transition_time", args.transition)
        output_name = args.output if args.output != "composed_sequence" else args.sequence
    elif args.procedures:
        procedure_names = args.procedures
        transition = args.transition
        output_name = args.output
    else:
        # Default: initial assessment
        seq = PREDEFINED_SEQUENCES["initial_assessment"]
        procedure_names = seq["procedures"]
        transition = seq.get("transition_time", 0.5)
        output_name = "initial_assessment"

    # Compose and render
    composer = SequenceComposer(procedure_names, transition_time=transition)
    composer.setup()
    composer.animate()
    composer.render(output_name=output_name, output_dir=args.dir)


if __name__ == "__main__":
    main()
