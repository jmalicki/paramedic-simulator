"""
Blood Pressure Cuff Application Procedure

Animates wrapping and inflating a BP cuff on the patient's arm.
Duration: 3.0 seconds
"""

import bpy
import math
import sys
import os

sys.path.insert(0, os.path.dirname(os.path.dirname(os.path.abspath(__file__))))

from common.core import seconds_to_frame, set_keyframe
from common.materials import create_toon_material, COLORS

# =============================================================================
# PROCEDURE METADATA
# =============================================================================

NAME = "bp_cuff_apply"
DESCRIPTION = "Apply blood pressure cuff to patient's arm"
DURATION = 3.0  # seconds

# Camera focus for this procedure
CAMERA_FOCUS = {
    "location": (-0.08, -0.45, 0.25),
    "rotation_deg": (75, 0, -5),
}


# =============================================================================
# EQUIPMENT CREATION
# =============================================================================

def create_equipment():
    """
    Create the blood pressure cuff model.

    Returns:
        dict: Equipment objects {'bp_cuff': Object, 'bp_bulb': Object}
    """
    # Cuff (wraps around arm)
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.055, depth=0.12,
        location=(-0.08, 0, 0),
        rotation=(0, math.radians(90), 0)
    )
    cuff = bpy.context.object
    cuff.name = "BP_Cuff"

    cuff_mat = create_toon_material("MAT_BP_Cuff", COLORS["bp_cuff_blue"])
    cuff.data.materials.append(cuff_mat)

    # Bulb (squeeze bulb for inflation)
    bpy.ops.mesh.primitive_uv_sphere_add(radius=0.025, location=(-0.08, -0.15, -0.05))
    bulb = bpy.context.object
    bulb.name = "BP_Bulb"
    bulb.scale = (0.8, 1.2, 1.0)

    bulb_mat = create_toon_material("MAT_BP_Bulb", COLORS["bp_bulb"])
    bulb.data.materials.append(bulb_mat)
    bulb.parent = cuff

    # Tubing connecting cuff to bulb
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.004, depth=0.12,
        location=(-0.08, -0.08, -0.03),
        rotation=(math.radians(90), 0, 0)
    )
    tube = bpy.context.object
    tube.name = "BP_Tube"
    tube.data.materials.append(bulb_mat)
    tube.parent = cuff

    # Gauge (pressure indicator)
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.02, depth=0.01,
        location=(-0.08, -0.08, 0.06),
        rotation=(math.radians(90), 0, 0)
    )
    gauge = bpy.context.object
    gauge.name = "BP_Gauge"
    gauge_mat = create_toon_material("MAT_BP_Gauge", COLORS["bp_gauge"])
    gauge.data.materials.append(gauge_mat)
    gauge.parent = cuff

    # Start hidden (off-screen)
    cuff.location = (-0.08, -0.30, 0.10)
    cuff.hide_viewport = True
    cuff.hide_render = True

    return {'bp_cuff': cuff, 'bp_bulb': bulb}


# =============================================================================
# ANIMATION
# =============================================================================

def animate(context, start_time):
    """
    Animate the BP cuff application.

    Args:
        context: dict with 'hand', 'patient_arm', 'camera', 'bp_cuff', 'bp_bulb'
        start_time: Start time in seconds

    Returns:
        float: End time in seconds
    """
    hand = context['hand']
    cuff = context['bp_cuff']
    bulb = context.get('bp_bulb')
    camera = context.get('camera')

    start_frame = seconds_to_frame(start_time)

    # Frame timing
    f_start = start_frame
    f_show = start_frame + seconds_to_frame(0.1)
    f_approach = start_frame + seconds_to_frame(0.4)
    f_wrap = start_frame + seconds_to_frame(1.0)
    f_secure = start_frame + seconds_to_frame(1.5)
    f_grab_bulb = start_frame + seconds_to_frame(1.8)
    f_squeeze1 = start_frame + seconds_to_frame(2.2)
    f_squeeze2 = start_frame + seconds_to_frame(2.5)
    f_done = start_frame + seconds_to_frame(DURATION)

    # === Show cuff (un-hide) ===
    cuff.hide_viewport = True
    cuff.hide_render = True
    set_keyframe(cuff, "hide_viewport", f_start, True)
    set_keyframe(cuff, "hide_render", f_start, True)

    cuff.hide_viewport = False
    cuff.hide_render = False
    set_keyframe(cuff, "hide_viewport", f_show, False)
    set_keyframe(cuff, "hide_render", f_show, False)

    # === Cuff Animation ===

    # Start position (held, approaching)
    set_keyframe(cuff, "location", f_start, (-0.08, -0.25, 0.10))
    set_keyframe(cuff, "rotation_euler", f_start, (0, math.radians(90), math.radians(45)))

    # Approach arm
    set_keyframe(cuff, "location", f_approach, (-0.08, -0.12, 0.05))
    set_keyframe(cuff, "rotation_euler", f_approach, (0, math.radians(90), math.radians(20)))

    # Wrap around arm
    set_keyframe(cuff, "location", f_wrap, (-0.08, -0.02, 0))
    set_keyframe(cuff, "rotation_euler", f_wrap, (0, math.radians(90), 0))

    # Secure in place
    set_keyframe(cuff, "location", f_secure, (-0.08, 0, 0))
    set_keyframe(cuff, "rotation_euler", f_secure, (0, math.radians(90), 0))

    # Stay in place during inflation
    set_keyframe(cuff, "location", f_grab_bulb, (-0.08, 0, 0))
    set_keyframe(cuff, "location", f_squeeze1, (-0.08, 0, 0))
    set_keyframe(cuff, "location", f_squeeze2, (-0.08, 0, 0))
    set_keyframe(cuff, "location", f_done, (-0.08, 0, 0))

    # Cuff inflation (scale increase to simulate)
    set_keyframe(cuff, "scale", f_secure, (1.0, 1.0, 1.0))
    set_keyframe(cuff, "scale", f_squeeze1, (1.05, 1.0, 1.05))
    set_keyframe(cuff, "scale", f_squeeze2, (1.1, 1.0, 1.1))
    set_keyframe(cuff, "scale", f_done, (1.1, 1.0, 1.1))

    # === Hand Animation ===

    # Get current hand position
    current_loc = hand.location.copy() if hand.location else (-0.05, -0.15, 0.08)

    # Start
    set_keyframe(hand, "location", f_start, tuple(current_loc))
    set_keyframe(hand, "rotation_euler", f_start, (0, 0, 0))

    # Holding cuff, approaching
    set_keyframe(hand, "location", f_approach, (-0.06, -0.12, 0.06))
    set_keyframe(hand, "rotation_euler", f_approach, (math.radians(10), 0, math.radians(-10)))

    # Wrapping cuff
    set_keyframe(hand, "location", f_wrap, (-0.10, -0.05, 0.03))
    set_keyframe(hand, "rotation_euler", f_wrap, (math.radians(20), 0, math.radians(-15)))

    # Securing
    set_keyframe(hand, "location", f_secure, (-0.12, -0.03, 0.02))
    set_keyframe(hand, "rotation_euler", f_secure, (math.radians(15), 0, math.radians(-10)))

    # Move to grab bulb
    set_keyframe(hand, "location", f_grab_bulb, (-0.08, -0.12, -0.02))
    set_keyframe(hand, "rotation_euler", f_grab_bulb, (math.radians(-20), 0, 0))

    # Squeezing bulb
    set_keyframe(hand, "location", f_squeeze1, (-0.08, -0.14, -0.04))
    set_keyframe(hand, "rotation_euler", f_squeeze1, (math.radians(-30), 0, 0))

    set_keyframe(hand, "location", f_squeeze2, (-0.08, -0.13, -0.03))
    set_keyframe(hand, "rotation_euler", f_squeeze2, (math.radians(-25), 0, 0))

    # Done
    set_keyframe(hand, "location", f_done, (-0.08, -0.12, -0.02))
    set_keyframe(hand, "rotation_euler", f_done, (math.radians(-20), 0, 0))

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
