#!/usr/bin/env python3
"""
Generate Initial Assessment Sequence Video

This script creates a complete animated video of the initial assessment sequence:
1. Pulse Oximeter Application
2. Radial Pulse Check
3. Blood Pressure Cuff Application

Run headless (no UI):
    blender --background --python generate_initial_assessment.py

Run with preview (opens Blender):
    blender --python generate_initial_assessment.py

Output: renders to ./output/initial_assessment.mp4 (or .avi on some systems)
"""

import bpy
import math
import os
from mathutils import Vector, Euler

# =============================================================================
# CONFIGURATION
# =============================================================================

CONFIG = {
    # Output settings
    "output_dir": os.path.join(os.path.dirname(os.path.abspath(__file__)), "output"),
    "output_filename": "initial_assessment",
    "resolution_x": 1280,
    "resolution_y": 720,
    "fps": 30,

    # Timing (in seconds)
    "procedures": {
        "pulseox": {"start": 0, "duration": 2.0},
        "radial_pulse": {"start": 2.5, "duration": 3.0},  # 0.5s transition
        "bp_cuff": {"start": 6.0, "duration": 3.0},       # 0.5s transition
    },
    "total_duration": 9.5,  # includes transitions

    # Colors (cel-shading palette)
    "colors": {
        "glove_blue": (0.420, 0.545, 0.643, 1.0),      # #6B8BA4
        "skin_light": (1.0, 0.855, 0.725, 1.0),         # #FFDAB9
        "pulseox_body": (0.910, 0.910, 0.910, 1.0),     # #E8E8E8
        "pulseox_clip": (0.290, 0.290, 0.290, 1.0),     # #4A4A4A
        "pulseox_screen": (0.102, 0.102, 0.180, 1.0),   # #1A1A2E
        "bp_cuff_blue": (0.180, 0.525, 0.670, 1.0),     # #2E86AB
        "bp_cuff_bulb": (0.290, 0.290, 0.290, 1.0),     # #4A4A4A
        "outline": (0.1, 0.1, 0.1, 1.0),
    },
}

# =============================================================================
# UTILITY FUNCTIONS
# =============================================================================

def clear_scene():
    """Remove all objects from the scene."""
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete()

    # Clear orphan data
    for block in bpy.data.meshes:
        if block.users == 0:
            bpy.data.meshes.remove(block)
    for block in bpy.data.materials:
        if block.users == 0:
            bpy.data.materials.remove(block)


def create_toon_material(name, base_color, shadow_strength=0.3):
    """Create a cel-shaded material using shader nodes."""
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links

    # Clear default nodes
    nodes.clear()

    # Create nodes
    output = nodes.new('ShaderNodeOutputMaterial')
    output.location = (400, 0)

    # Diffuse BSDF for base color
    diffuse = nodes.new('ShaderNodeBsdfDiffuse')
    diffuse.inputs['Color'].default_value = base_color
    diffuse.location = (0, 0)

    # For simple toon look, just use diffuse
    # In a full setup, we'd use Shader to RGB + ColorRamp
    links.new(diffuse.outputs['BSDF'], output.inputs['Surface'])

    return mat


def create_emissive_material(name, color, strength=2.0):
    """Create an emissive material for screens/LEDs."""
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links

    nodes.clear()

    output = nodes.new('ShaderNodeOutputMaterial')
    output.location = (400, 0)

    emission = nodes.new('ShaderNodeEmission')
    emission.inputs['Color'].default_value = color
    emission.inputs['Strength'].default_value = strength
    emission.location = (0, 0)

    links.new(emission.outputs['Emission'], output.inputs['Surface'])

    return mat


def seconds_to_frame(seconds):
    """Convert seconds to frame number."""
    return int(seconds * CONFIG["fps"])


def set_keyframe(obj, data_path, frame, value):
    """Set a keyframe on an object property."""
    if data_path == "location":
        obj.location = value
        obj.keyframe_insert(data_path="location", frame=frame)
    elif data_path == "rotation_euler":
        obj.rotation_euler = value
        obj.keyframe_insert(data_path="rotation_euler", frame=frame)
    elif data_path == "scale":
        obj.scale = value
        obj.keyframe_insert(data_path="scale", frame=frame)


# =============================================================================
# SCENE SETUP
# =============================================================================

def setup_scene():
    """Configure render settings and world."""
    scene = bpy.context.scene

    # Render settings
    scene.render.resolution_x = CONFIG["resolution_x"]
    scene.render.resolution_y = CONFIG["resolution_y"]
    scene.render.fps = CONFIG["fps"]

    # Frame range
    scene.frame_start = 1
    scene.frame_end = seconds_to_frame(CONFIG["total_duration"])

    # Output settings
    os.makedirs(CONFIG["output_dir"], exist_ok=True)
    scene.render.filepath = os.path.join(
        CONFIG["output_dir"],
        CONFIG["output_filename"]
    )

    # Use FFmpeg for video output
    scene.render.image_settings.file_format = 'FFMPEG'
    scene.render.ffmpeg.format = 'MPEG4'
    scene.render.ffmpeg.codec = 'H264'
    scene.render.ffmpeg.constant_rate_factor = 'HIGH'
    scene.render.ffmpeg.audio_codec = 'NONE'

    # Render engine (use Eevee for speed, or Cycles for quality)
    scene.render.engine = 'BLENDER_EEVEE_NEXT'

    # World background
    world = bpy.data.worlds.new("ToonWorld")
    world.use_nodes = True
    bg = world.node_tree.nodes["Background"]
    bg.inputs['Color'].default_value = (0.9, 0.92, 0.95, 1.0)  # Light clinical blue-white
    bg.inputs['Strength'].default_value = 1.0
    scene.world = world


def create_camera():
    """Create and position the main camera."""
    bpy.ops.object.camera_add(location=(0, -0.5, 0.3))
    camera = bpy.context.object
    camera.name = "MainCamera"

    # Point camera at scene center
    camera.rotation_euler = (math.radians(75), 0, 0)

    # Set as active camera
    bpy.context.scene.camera = camera

    # Camera settings
    camera.data.lens = 35  # Focal length
    camera.data.clip_start = 0.01
    camera.data.clip_end = 100

    return camera


def create_lights():
    """Create lighting setup for cel-shaded look."""
    # Key light (main light source)
    bpy.ops.object.light_add(type='SUN', location=(2, -2, 3))
    key_light = bpy.context.object
    key_light.name = "KeyLight"
    key_light.data.energy = 3.0
    key_light.rotation_euler = (math.radians(45), math.radians(20), math.radians(30))

    # Fill light (softer, from opposite side)
    bpy.ops.object.light_add(type='SUN', location=(-2, -1, 2))
    fill_light = bpy.context.object
    fill_light.name = "FillLight"
    fill_light.data.energy = 1.0
    fill_light.rotation_euler = (math.radians(60), math.radians(-20), math.radians(-20))

    return key_light, fill_light


# =============================================================================
# MODEL CREATION
# =============================================================================

def create_patient_arm():
    """Create a simplified patient arm model."""
    # Forearm
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.04, depth=0.25,
        location=(0, 0, 0),
        rotation=(0, math.radians(90), 0)
    )
    forearm = bpy.context.object
    forearm.name = "PatientForearm"

    # Apply skin material
    skin_mat = create_toon_material("MAT_Skin", CONFIG["colors"]["skin_light"])
    forearm.data.materials.append(skin_mat)

    # Hand (simple box for now)
    bpy.ops.mesh.primitive_cube_add(
        size=0.08,
        location=(0.15, 0, 0)
    )
    hand = bpy.context.object
    hand.name = "PatientHand"
    hand.scale = (1.2, 0.8, 0.4)
    hand.data.materials.append(skin_mat)

    # Fingers (simple cylinders)
    fingers = []
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
        fingers.append(finger)

    # Thumb
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.01, depth=0.045,
        location=(0.17, -0.04, -0.015),
        rotation=(math.radians(30), math.radians(70), 0)
    )
    thumb = bpy.context.object
    thumb.name = "PatientThumb"
    thumb.data.materials.append(skin_mat)

    # Parent all to forearm
    for obj in [hand, thumb] + fingers:
        obj.parent = forearm

    return forearm


def create_gloved_hand(name_suffix="L", x_offset=0):
    """Create a simplified gloved hand model."""
    # Palm
    bpy.ops.mesh.primitive_cube_add(
        size=0.08,
        location=(x_offset, -0.15, 0.08)
    )
    palm = bpy.context.object
    palm.name = f"GlovedHand_{name_suffix}"
    palm.scale = (0.6, 1.0, 0.3)

    # Apply glove material
    glove_mat = create_toon_material("MAT_Glove", CONFIG["colors"]["glove_blue"])
    palm.data.materials.append(glove_mat)

    # Fingers
    fingers = []
    finger_offsets = [-0.02, -0.007, 0.007, 0.02]
    for i, x_off in enumerate(finger_offsets):
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.008, depth=0.05,
            location=(x_offset + x_off, -0.20, 0.08),
            rotation=(math.radians(90), 0, 0)
        )
        finger = bpy.context.object
        finger.name = f"GloveFinger_{name_suffix}_{i}"
        finger.data.materials.append(glove_mat)
        finger.parent = palm
        fingers.append(finger)

    # Thumb
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.009, depth=0.04,
        location=(x_offset - 0.035, -0.13, 0.06),
        rotation=(math.radians(45), math.radians(30), 0)
    )
    thumb = bpy.context.object
    thumb.name = f"GloveThumb_{name_suffix}"
    thumb.data.materials.append(glove_mat)
    thumb.parent = palm

    return palm


def create_pulse_oximeter():
    """Create a pulse oximeter model."""
    # Body (top clip)
    bpy.ops.mesh.primitive_cube_add(
        size=0.03,
        location=(0.25, 0, 0.03)
    )
    body = bpy.context.object
    body.name = "PulseOx_Body"
    body.scale = (2.0, 1.2, 0.8)

    body_mat = create_toon_material("MAT_PulseOx_Body", CONFIG["colors"]["pulseox_body"])
    body.data.materials.append(body_mat)

    # Screen
    bpy.ops.mesh.primitive_plane_add(
        size=0.025,
        location=(0.25, -0.02, 0.042)
    )
    screen = bpy.context.object
    screen.name = "PulseOx_Screen"
    screen.scale = (1.5, 1.0, 1.0)

    screen_mat = create_emissive_material(
        "MAT_PulseOx_Screen",
        (0.0, 1.0, 0.3, 1.0),  # Green glow
        strength=1.0
    )
    screen.data.materials.append(screen_mat)
    screen.parent = body

    # Bottom clip
    bpy.ops.mesh.primitive_cube_add(
        size=0.025,
        location=(0.25, 0, 0.005)
    )
    clip = bpy.context.object
    clip.name = "PulseOx_Clip"
    clip.scale = (2.0, 1.0, 0.5)

    clip_mat = create_toon_material("MAT_PulseOx_Clip", CONFIG["colors"]["pulseox_clip"])
    clip.data.materials.append(clip_mat)
    clip.parent = body

    # Initially position off-screen (will animate in)
    body.location = (0.3, -0.3, 0.15)

    return body


def create_bp_cuff():
    """Create a blood pressure cuff model."""
    # Cuff (cylinder wrapped around arm)
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.055, depth=0.12,
        location=(-0.08, 0, 0),
        rotation=(0, math.radians(90), 0)
    )
    cuff = bpy.context.object
    cuff.name = "BP_Cuff"

    cuff_mat = create_toon_material("MAT_BP_Cuff", CONFIG["colors"]["bp_cuff_blue"])
    cuff.data.materials.append(cuff_mat)

    # Bulb (squeeze bulb)
    bpy.ops.mesh.primitive_uv_sphere_add(
        radius=0.025,
        location=(-0.08, -0.15, -0.05)
    )
    bulb = bpy.context.object
    bulb.name = "BP_Bulb"
    bulb.scale = (0.8, 1.2, 1.0)

    bulb_mat = create_toon_material("MAT_BP_Bulb", CONFIG["colors"]["bp_cuff_bulb"])
    bulb.data.materials.append(bulb_mat)
    bulb.parent = cuff

    # Tubing
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.004, depth=0.12,
        location=(-0.08, -0.08, -0.03),
        rotation=(math.radians(90), 0, 0)
    )
    tube = bpy.context.object
    tube.name = "BP_Tube"
    tube.data.materials.append(bulb_mat)
    tube.parent = cuff

    # Initially hide (will animate in)
    cuff.location = (-0.08, 0, -0.3)
    cuff.hide_viewport = True
    cuff.hide_render = True

    return cuff


# =============================================================================
# ANIMATION
# =============================================================================

def animate_pulseox_apply(hand, pulseox, start_time):
    """Animate pulse oximeter application sequence."""
    start_frame = seconds_to_frame(start_time)

    # Frame timing within sequence
    f_approach = start_frame
    f_open = start_frame + seconds_to_frame(0.5)
    f_place = start_frame + seconds_to_frame(1.0)
    f_clip = start_frame + seconds_to_frame(1.5)
    f_done = start_frame + seconds_to_frame(2.0)

    # Pulse oximeter animation
    # Start position (in hand, approaching)
    set_keyframe(pulseox, "location", f_approach, (0.3, -0.25, 0.15))
    set_keyframe(pulseox, "rotation_euler", f_approach, (0, 0, 0))

    # Move toward finger
    set_keyframe(pulseox, "location", f_open, (0.28, -0.15, 0.08))
    set_keyframe(pulseox, "rotation_euler", f_open, (math.radians(-20), 0, 0))

    # Position on finger
    set_keyframe(pulseox, "location", f_place, (0.24, -0.01, 0.02))
    set_keyframe(pulseox, "rotation_euler", f_place, (0, 0, 0))

    # Clipped on (slight adjustment)
    set_keyframe(pulseox, "location", f_clip, (0.24, 0, 0.015))

    # Final position
    set_keyframe(pulseox, "location", f_done, (0.24, 0, 0.015))

    # Hand animation
    set_keyframe(hand, "location", f_approach, (0, -0.15, 0.08))
    set_keyframe(hand, "location", f_open, (0.05, -0.12, 0.06))
    set_keyframe(hand, "location", f_place, (0.15, -0.08, 0.04))
    set_keyframe(hand, "location", f_clip, (0.18, -0.05, 0.03))
    set_keyframe(hand, "location", f_done, (0, -0.20, 0.10))  # Hand withdraws


def animate_radial_pulse(hand, start_time):
    """Animate radial pulse check sequence."""
    start_frame = seconds_to_frame(start_time)

    # Frame timing
    f_start = start_frame
    f_approach = start_frame + seconds_to_frame(0.5)
    f_position = start_frame + seconds_to_frame(1.0)
    f_palpate1 = start_frame + seconds_to_frame(1.5)
    f_palpate2 = start_frame + seconds_to_frame(2.5)
    f_release = start_frame + seconds_to_frame(3.0)

    # Hand moves to wrist area
    set_keyframe(hand, "location", f_start, (0, -0.20, 0.10))
    set_keyframe(hand, "rotation_euler", f_start, (0, 0, 0))

    # Approach wrist
    set_keyframe(hand, "location", f_approach, (-0.05, -0.12, 0.05))
    set_keyframe(hand, "rotation_euler", f_approach, (math.radians(20), 0, math.radians(-10)))

    # Position fingers on radial artery
    set_keyframe(hand, "location", f_position, (-0.02, -0.04, 0.02))
    set_keyframe(hand, "rotation_euler", f_position, (math.radians(30), 0, math.radians(-15)))

    # Palpating (slight movements to simulate feeling pulse)
    set_keyframe(hand, "location", f_palpate1, (-0.02, -0.035, 0.018))
    set_keyframe(hand, "location", f_palpate2, (-0.02, -0.038, 0.022))

    # Release
    set_keyframe(hand, "location", f_release, (-0.05, -0.15, 0.08))
    set_keyframe(hand, "rotation_euler", f_release, (0, 0, 0))


def animate_bp_cuff(hand, cuff, start_time):
    """Animate blood pressure cuff application."""
    start_frame = seconds_to_frame(start_time)

    # Frame timing
    f_start = start_frame
    f_show_cuff = start_frame + seconds_to_frame(0.3)
    f_wrap = start_frame + seconds_to_frame(1.0)
    f_secure = start_frame + seconds_to_frame(1.5)
    f_inflate = start_frame + seconds_to_frame(2.0)
    f_done = start_frame + seconds_to_frame(3.0)

    # Show cuff
    cuff.hide_viewport = False
    cuff.hide_render = False
    cuff.keyframe_insert(data_path="hide_viewport", frame=f_start)
    cuff.keyframe_insert(data_path="hide_render", frame=f_start)

    # Cuff animation
    set_keyframe(cuff, "location", f_start, (-0.08, -0.20, 0.10))
    set_keyframe(cuff, "rotation_euler", f_start, (0, math.radians(90), 0))

    set_keyframe(cuff, "location", f_show_cuff, (-0.08, -0.10, 0.05))

    set_keyframe(cuff, "location", f_wrap, (-0.08, 0, 0))
    set_keyframe(cuff, "rotation_euler", f_wrap, (0, math.radians(90), 0))

    set_keyframe(cuff, "location", f_secure, (-0.08, 0, 0))
    set_keyframe(cuff, "location", f_inflate, (-0.08, 0, 0))
    set_keyframe(cuff, "location", f_done, (-0.08, 0, 0))

    # Hand animation
    set_keyframe(hand, "location", f_start, (-0.05, -0.15, 0.08))
    set_keyframe(hand, "location", f_show_cuff, (-0.08, -0.12, 0.06))
    set_keyframe(hand, "location", f_wrap, (-0.10, -0.05, 0.04))
    set_keyframe(hand, "location", f_secure, (-0.12, -0.03, 0.02))

    # Hand moves to bulb for inflation
    set_keyframe(hand, "location", f_inflate, (-0.08, -0.15, 0.00))
    set_keyframe(hand, "rotation_euler", f_inflate, (math.radians(-30), 0, 0))

    # Squeezing bulb animation
    set_keyframe(hand, "location", f_done, (-0.08, -0.12, -0.02))


def animate_camera(camera, start_time_pulseox, start_time_radial, start_time_bp):
    """Animate camera to follow the action."""
    # Camera positions for each procedure

    # Pulse ox - focus on finger
    f_pulseox = seconds_to_frame(start_time_pulseox)
    set_keyframe(camera, "location", f_pulseox, (0.15, -0.4, 0.25))
    set_keyframe(camera, "rotation_euler", f_pulseox, (math.radians(70), 0, math.radians(10)))

    # Transition to radial pulse - focus on wrist
    f_radial = seconds_to_frame(start_time_radial)
    set_keyframe(camera, "location", f_radial, (0, -0.45, 0.25))
    set_keyframe(camera, "rotation_euler", f_radial, (math.radians(75), 0, 0))

    # Transition to BP - focus on upper arm
    f_bp = seconds_to_frame(start_time_bp)
    set_keyframe(camera, "location", f_bp, (-0.08, -0.45, 0.25))
    set_keyframe(camera, "rotation_euler", f_bp, (math.radians(75), 0, math.radians(-5)))

    # End position
    f_end = seconds_to_frame(CONFIG["total_duration"])
    set_keyframe(camera, "location", f_end, (-0.08, -0.45, 0.25))


# =============================================================================
# MAIN EXECUTION
# =============================================================================

def main():
    print("=" * 60)
    print("Generating Initial Assessment Sequence")
    print("=" * 60)

    # Clear and setup
    print("Setting up scene...")
    clear_scene()
    setup_scene()

    # Create elements
    print("Creating camera and lights...")
    camera = create_camera()
    create_lights()

    print("Creating models...")
    patient_arm = create_patient_arm()
    gloved_hand = create_gloved_hand("L", 0)
    pulse_ox = create_pulse_oximeter()
    bp_cuff = create_bp_cuff()

    # Get timing from config
    pulseox_start = CONFIG["procedures"]["pulseox"]["start"]
    radial_start = CONFIG["procedures"]["radial_pulse"]["start"]
    bp_start = CONFIG["procedures"]["bp_cuff"]["start"]

    # Animate
    print("Creating animations...")
    animate_pulseox_apply(gloved_hand, pulse_ox, pulseox_start)
    animate_radial_pulse(gloved_hand, radial_start)
    animate_bp_cuff(gloved_hand, bp_cuff, bp_start)
    animate_camera(camera, pulseox_start, radial_start, bp_start)

    # Set interpolation to smooth
    for obj in bpy.data.objects:
        if obj.animation_data and obj.animation_data.action:
            for fcurve in obj.animation_data.action.fcurves:
                for keyframe in fcurve.keyframe_points:
                    keyframe.interpolation = 'BEZIER'
                    keyframe.handle_left_type = 'AUTO_CLAMPED'
                    keyframe.handle_right_type = 'AUTO_CLAMPED'

    # Render
    print(f"Rendering to: {CONFIG['output_dir']}/{CONFIG['output_filename']}.mp4")
    print(f"Resolution: {CONFIG['resolution_x']}x{CONFIG['resolution_y']} @ {CONFIG['fps']}fps")
    print(f"Duration: {CONFIG['total_duration']}s ({seconds_to_frame(CONFIG['total_duration'])} frames)")
    print("-" * 60)

    # Check if running in background mode
    if bpy.app.background:
        print("Rendering animation...")
        bpy.ops.render.render(animation=True)
        print("=" * 60)
        print("COMPLETE!")
        print(f"Output: {CONFIG['output_dir']}/{CONFIG['output_filename']}.mp4")
        print("=" * 60)
    else:
        print("Running in interactive mode.")
        print("Use Render > Render Animation (Ctrl+F12) to render.")
        print("Or run with: blender --background --python generate_initial_assessment.py")


if __name__ == "__main__":
    main()
