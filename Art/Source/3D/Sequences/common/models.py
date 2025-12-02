"""
Shared 3D model creation for procedural sequences.
"""

import bpy
import math
from .materials import create_toon_material, COLORS


def create_patient_arm(skin_tone="skin_light"):
    """
    Create a patient arm model (forearm + hand + fingers).

    Args:
        skin_tone: Key from COLORS dict for skin color

    Returns:
        bpy.types.Object: Root forearm object (other parts parented to it)
    """
    skin_color = COLORS.get(skin_tone, COLORS["skin_light"])
    skin_mat = create_toon_material(f"MAT_Skin_{skin_tone}", skin_color)

    # Forearm
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.04, depth=0.25,
        location=(0, 0, 0),
        rotation=(0, math.radians(90), 0)
    )
    forearm = bpy.context.object
    forearm.name = "PatientForearm"
    forearm.data.materials.append(skin_mat)

    # Hand
    bpy.ops.mesh.primitive_cube_add(size=0.08, location=(0.15, 0, 0))
    hand = bpy.context.object
    hand.name = "PatientHand"
    hand.scale = (1.2, 0.8, 0.4)
    hand.data.materials.append(skin_mat)
    hand.parent = forearm

    # Fingers
    finger_positions = [
        (0.22, -0.025, 0.01),   # Index
        (0.24, 0, 0.01),        # Middle
        (0.22, 0.025, 0.01),    # Ring
        (0.18, 0.045, 0.005),   # Pinky
    ]
    for i, pos in enumerate(finger_positions):
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.008, depth=0.06,
            location=pos,
            rotation=(0, math.radians(90), 0)
        )
        finger = bpy.context.object
        finger.name = f"PatientFinger_{i}"
        finger.data.materials.append(skin_mat)
        finger.parent = forearm

    # Thumb
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.01, depth=0.045,
        location=(0.17, -0.04, -0.015),
        rotation=(math.radians(30), math.radians(70), 0)
    )
    thumb = bpy.context.object
    thumb.name = "PatientThumb"
    thumb.data.materials.append(skin_mat)
    thumb.parent = forearm

    return forearm


def create_gloved_hand(name_suffix="L", position=(0, -0.15, 0.08), glove_color="glove_blue"):
    """
    Create a gloved hand model.

    Args:
        name_suffix: "L" or "R" for left/right
        position: Initial (x, y, z) position
        glove_color: Key from COLORS dict

    Returns:
        bpy.types.Object: Palm object (fingers parented to it)
    """
    color = COLORS.get(glove_color, COLORS["glove_blue"])
    glove_mat = create_toon_material(f"MAT_Glove_{glove_color}", color)

    # Palm
    bpy.ops.mesh.primitive_cube_add(size=0.08, location=position)
    palm = bpy.context.object
    palm.name = f"GlovedHand_{name_suffix}"
    palm.scale = (0.6, 1.0, 0.3)
    palm.data.materials.append(glove_mat)

    # Fingers
    finger_offsets = [-0.02, -0.007, 0.007, 0.02]
    for i, x_off in enumerate(finger_offsets):
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.008, depth=0.05,
            location=(position[0] + x_off, position[1] - 0.05, position[2]),
            rotation=(math.radians(90), 0, 0)
        )
        finger = bpy.context.object
        finger.name = f"GloveFinger_{name_suffix}_{i}"
        finger.data.materials.append(glove_mat)
        finger.parent = palm

    # Thumb
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.009, depth=0.04,
        location=(position[0] - 0.035, position[1] + 0.02, position[2] - 0.02),
        rotation=(math.radians(45), math.radians(30), 0)
    )
    thumb = bpy.context.object
    thumb.name = f"GloveThumb_{name_suffix}"
    thumb.data.materials.append(glove_mat)
    thumb.parent = palm

    return palm


def create_patient_upper_arm(skin_tone="skin_light"):
    """
    Create a patient upper arm (for BP cuff placement).

    Args:
        skin_tone: Key from COLORS dict

    Returns:
        bpy.types.Object: Upper arm cylinder
    """
    skin_color = COLORS.get(skin_tone, COLORS["skin_light"])
    skin_mat = create_toon_material(f"MAT_Skin_{skin_tone}", skin_color)

    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.05, depth=0.20,
        location=(-0.15, 0, 0),
        rotation=(0, math.radians(90), 0)
    )
    upper_arm = bpy.context.object
    upper_arm.name = "PatientUpperArm"
    upper_arm.data.materials.append(skin_mat)

    return upper_arm
