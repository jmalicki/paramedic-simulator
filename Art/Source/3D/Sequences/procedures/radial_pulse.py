"""
Radial Pulse Check Procedure

Animates palpating the radial pulse at the patient's wrist.
Duration: 3.0 seconds
"""

import bpy
import math
import sys
import os

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from common.core import seconds_to_frame, set_keyframe
from common.materials import COLORS

# =============================================================================
# PROCEDURE METADATA
# =============================================================================

NAME = "radial_pulse"
DESCRIPTION = "Palpate radial pulse at patient's wrist"
DURATION = 3.0  # seconds

# Camera focus for this procedure
CAMERA_FOCUS = {
    "location": (0, -0.45, 0.25),
    "rotation_deg": (75, 0, 0),
}


# =============================================================================
# EQUIPMENT CREATION
# =============================================================================

def create_equipment():
    """
    Create any procedure-specific equipment.

    For radial pulse, no additional equipment is needed.

    Returns:
        dict: Empty dict (no equipment)
    """
    return {}


# =============================================================================
# ANIMATION
# =============================================================================

def animate(context, start_time):
    """
    Animate the radial pulse check.

    Args:
        context: dict with 'hand', 'patient_arm', 'camera'
        start_time: Start time in seconds

    Returns:
        float: End time in seconds
    """
    hand = context['hand']
    camera = context.get('camera')

    start_frame = seconds_to_frame(start_time)

    # Frame timing
    f_start = start_frame
    f_approach = start_frame + seconds_to_frame(0.5)
    f_position = start_frame + seconds_to_frame(1.0)
    f_palpate1 = start_frame + seconds_to_frame(1.5)
    f_palpate2 = start_frame + seconds_to_frame(2.0)
    f_palpate3 = start_frame + seconds_to_frame(2.5)
    f_release = start_frame + seconds_to_frame(2.8)
    f_done = start_frame + seconds_to_frame(DURATION)

    # === Hand Animation ===

    # Get current hand position as starting point
    current_loc = hand.location.copy() if hand.location else (0, -0.20, 0.10)

    # Start: from current position
    set_keyframe(hand, "location", f_start, tuple(current_loc))
    set_keyframe(hand, "rotation_euler", f_start, (0, 0, 0))

    # Approach wrist
    set_keyframe(hand, "location", f_approach, (-0.03, -0.12, 0.05))
    set_keyframe(hand, "rotation_euler", f_approach, (
        math.radians(15),
        0,
        math.radians(-10)
    ))

    # Position two fingers on radial artery (lateral wrist)
    set_keyframe(hand, "location", f_position, (-0.01, -0.04, 0.02))
    set_keyframe(hand, "rotation_euler", f_position, (
        math.radians(25),
        0,
        math.radians(-15)
    ))

    # Palpating - subtle movements simulating pulse detection
    set_keyframe(hand, "location", f_palpate1, (-0.01, -0.035, 0.018))
    set_keyframe(hand, "rotation_euler", f_palpate1, (
        math.radians(28),
        0,
        math.radians(-15)
    ))

    set_keyframe(hand, "location", f_palpate2, (-0.01, -0.038, 0.022))
    set_keyframe(hand, "rotation_euler", f_palpate2, (
        math.radians(23),
        0,
        math.radians(-15)
    ))

    set_keyframe(hand, "location", f_palpate3, (-0.01, -0.036, 0.019))
    set_keyframe(hand, "rotation_euler", f_palpate3, (
        math.radians(26),
        0,
        math.radians(-15)
    ))

    # Release
    set_keyframe(hand, "location", f_release, (-0.03, -0.10, 0.06))
    set_keyframe(hand, "rotation_euler", f_release, (
        math.radians(10),
        0,
        math.radians(-5)
    ))

    # Return to neutral
    set_keyframe(hand, "location", f_done, (-0.05, -0.15, 0.08))
    set_keyframe(hand, "rotation_euler", f_done, (0, 0, 0))

    # === Camera Animation (optional) ===
    if camera:
        set_keyframe(camera, "location", f_start, CAMERA_FOCUS["location"])
        set_keyframe(camera, "rotation_euler", f_start, (
            math.radians(CAMERA_FOCUS["rotation_deg"][0]),
            math.radians(CAMERA_FOCUS["rotation_deg"][1]),
            math.radians(CAMERA_FOCUS["rotation_deg"][2]),
        ))

    return start_time + DURATION


# =============================================================================
# STANDALONE EXECUTION
# =============================================================================

if __name__ == "__main__":
    from common.core import clear_scene, setup_scene, create_camera, create_lights, smooth_all_keyframes
    from common.models import create_patient_arm, create_gloved_hand

    print(f"Generating: {DESCRIPTION}")

    clear_scene()
    setup_scene(DURATION + 0.5)

    camera = create_camera()
    create_lights()

    patient_arm = create_patient_arm()
    hand = create_gloved_hand()
    equipment = create_equipment()

    context = {
        'hand': hand,
        'patient_arm': patient_arm,
        'camera': camera,
        **equipment,
    }

    animate(context, 0)
    smooth_all_keyframes()

    print(f"Scene ready. Duration: {DURATION}s")
