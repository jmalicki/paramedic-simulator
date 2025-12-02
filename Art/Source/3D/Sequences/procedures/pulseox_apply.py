"""
Pulse Oximeter Application Procedure

Animates clipping a pulse oximeter onto the patient's finger.
Duration: 2.0 seconds
"""

import bpy
import math
import sys
import os

# Add parent to path for imports
sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from common.core import seconds_to_frame, set_keyframe
from common.materials import create_toon_material, create_emissive_material, COLORS

# =============================================================================
# PROCEDURE METADATA
# =============================================================================

NAME = "pulseox_apply"
DESCRIPTION = "Apply pulse oximeter to patient's finger"
DURATION = 2.0  # seconds

# Camera focus point for this procedure
CAMERA_FOCUS = {
    "location": (0.15, -0.4, 0.25),
    "rotation_deg": (70, 0, 10),
}


# =============================================================================
# EQUIPMENT CREATION
# =============================================================================

def create_equipment():
    """
    Create the pulse oximeter model.

    Returns:
        dict: Equipment objects {'pulseox': Object}
    """
    # Body (top clip)
    bpy.ops.mesh.primitive_cube_add(size=0.03, location=(0.25, 0, 0.03))
    body = bpy.context.object
    body.name = "PulseOx_Body"
    body.scale = (2.0, 1.2, 0.8)

    body_mat = create_toon_material("MAT_PulseOx_Body", COLORS["pulseox_body"])
    body.data.materials.append(body_mat)

    # Screen
    bpy.ops.mesh.primitive_plane_add(size=0.025, location=(0.25, -0.02, 0.042))
    screen = bpy.context.object
    screen.name = "PulseOx_Screen"
    screen.scale = (1.5, 1.0, 1.0)

    screen_mat = create_emissive_material(
        "MAT_PulseOx_Screen",
        COLORS["pulseox_screen_on"],
        strength=1.0
    )
    screen.data.materials.append(screen_mat)
    screen.parent = body

    # Bottom clip
    bpy.ops.mesh.primitive_cube_add(size=0.025, location=(0.25, 0, 0.005))
    clip = bpy.context.object
    clip.name = "PulseOx_Clip"
    clip.scale = (2.0, 1.0, 0.5)

    clip_mat = create_toon_material("MAT_PulseOx_Clip", COLORS["pulseox_clip"])
    clip.data.materials.append(clip_mat)
    clip.parent = body

    # Start position (off-screen, held by hand)
    body.location = (0.3, -0.3, 0.15)

    return {'pulseox': body}


# =============================================================================
# ANIMATION
# =============================================================================

def animate(context, start_time):
    """
    Animate the pulse oximeter application.

    Args:
        context: dict with 'hand', 'patient_arm', 'camera', 'pulseox'
        start_time: Start time in seconds

    Returns:
        float: End time in seconds
    """
    hand = context['hand']
    pulseox = context['pulseox']
    camera = context.get('camera')

    start_frame = seconds_to_frame(start_time)

    # Frame timing
    f_start = start_frame
    f_approach = start_frame + seconds_to_frame(0.5)
    f_open = start_frame + seconds_to_frame(0.8)
    f_place = start_frame + seconds_to_frame(1.2)
    f_clip = start_frame + seconds_to_frame(1.5)
    f_release = start_frame + seconds_to_frame(1.8)
    f_done = start_frame + seconds_to_frame(DURATION)

    # === Pulse Oximeter Animation ===

    # Start: held in hand, approaching
    set_keyframe(pulseox, "location", f_start, (0.3, -0.25, 0.15))
    set_keyframe(pulseox, "rotation_euler", f_start, (0, 0, 0))

    # Approach finger
    set_keyframe(pulseox, "location", f_approach, (0.28, -0.15, 0.08))
    set_keyframe(pulseox, "rotation_euler", f_approach, (math.radians(-20), 0, 0))

    # Open clip (rotation simulates opening)
    set_keyframe(pulseox, "location", f_open, (0.26, -0.08, 0.04))
    set_keyframe(pulseox, "rotation_euler", f_open, (math.radians(-30), 0, 0))

    # Place on finger
    set_keyframe(pulseox, "location", f_place, (0.24, -0.01, 0.02))
    set_keyframe(pulseox, "rotation_euler", f_place, (0, 0, 0))

    # Clip closed
    set_keyframe(pulseox, "location", f_clip, (0.24, 0, 0.015))
    set_keyframe(pulseox, "rotation_euler", f_clip, (0, 0, 0))

    # Final position (stays on finger)
    set_keyframe(pulseox, "location", f_release, (0.24, 0, 0.015))
    set_keyframe(pulseox, "location", f_done, (0.24, 0, 0.015))

    # === Hand Animation ===

    # Start position
    set_keyframe(hand, "location", f_start, (0, -0.15, 0.08))
    set_keyframe(hand, "rotation_euler", f_start, (0, 0, 0))

    # Move with pulseox
    set_keyframe(hand, "location", f_approach, (0.05, -0.12, 0.06))
    set_keyframe(hand, "location", f_open, (0.12, -0.08, 0.04))
    set_keyframe(hand, "location", f_place, (0.15, -0.05, 0.03))
    set_keyframe(hand, "location", f_clip, (0.18, -0.04, 0.025))

    # Release and withdraw
    set_keyframe(hand, "location", f_release, (0.10, -0.10, 0.05))
    set_keyframe(hand, "location", f_done, (0, -0.20, 0.10))
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
    # Allow running this procedure standalone
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
