"""
Core utilities for Blender sequence generation.
"""

import bpy
import math
import os

# =============================================================================
# GLOBAL CONFIGURATION
# =============================================================================

CONFIG = {
    # Render settings
    "resolution_x": 1280,
    "resolution_y": 720,
    "fps": 30,

    # Default output
    "output_dir": None,  # Set by composer
    "output_filename": "sequence",
}


def get_output_dir():
    """Get or create the output directory."""
    if CONFIG["output_dir"] is None:
        CONFIG["output_dir"] = os.path.join(
            os.path.dirname(os.path.abspath(__file__)),
            "..", "output"
        )
    os.makedirs(CONFIG["output_dir"], exist_ok=True)
    return CONFIG["output_dir"]


# =============================================================================
# TIMING UTILITIES
# =============================================================================

def seconds_to_frame(seconds):
    """Convert seconds to frame number."""
    return int(seconds * CONFIG["fps"])


def frame_to_seconds(frame):
    """Convert frame number to seconds."""
    return frame / CONFIG["fps"]


# =============================================================================
# SCENE UTILITIES
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
    for block in bpy.data.armatures:
        if block.users == 0:
            bpy.data.armatures.remove(block)


def setup_scene(total_duration_seconds):
    """Configure render settings and world."""
    scene = bpy.context.scene

    # Render settings
    scene.render.resolution_x = CONFIG["resolution_x"]
    scene.render.resolution_y = CONFIG["resolution_y"]
    scene.render.fps = CONFIG["fps"]

    # Frame range
    scene.frame_start = 1
    scene.frame_end = seconds_to_frame(total_duration_seconds)

    # Output settings
    output_dir = get_output_dir()
    scene.render.filepath = os.path.join(output_dir, CONFIG["output_filename"])

    # Use FFmpeg for video output
    scene.render.image_settings.file_format = 'FFMPEG'
    scene.render.ffmpeg.format = 'MPEG4'
    scene.render.ffmpeg.codec = 'H264'
    scene.render.ffmpeg.constant_rate_factor = 'HIGH'
    scene.render.ffmpeg.audio_codec = 'NONE'

    # Render engine
    scene.render.engine = 'BLENDER_EEVEE_NEXT'

    # World background
    world = bpy.data.worlds.new("ToonWorld")
    world.use_nodes = True
    bg = world.node_tree.nodes["Background"]
    bg.inputs['Color'].default_value = (0.9, 0.92, 0.95, 1.0)
    bg.inputs['Strength'].default_value = 1.0
    scene.world = world

    return scene


# =============================================================================
# KEYFRAME UTILITIES
# =============================================================================

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
    elif data_path == "hide_viewport":
        obj.hide_viewport = value
        obj.keyframe_insert(data_path="hide_viewport", frame=frame)
    elif data_path == "hide_render":
        obj.hide_render = value
        obj.keyframe_insert(data_path="hide_render", frame=frame)


def smooth_keyframes(obj):
    """Apply smooth bezier interpolation to all keyframes on an object."""
    if obj.animation_data and obj.animation_data.action:
        for fcurve in obj.animation_data.action.fcurves:
            for keyframe in fcurve.keyframe_points:
                keyframe.interpolation = 'BEZIER'
                keyframe.handle_left_type = 'AUTO_CLAMPED'
                keyframe.handle_right_type = 'AUTO_CLAMPED'


def smooth_all_keyframes():
    """Apply smooth interpolation to all animated objects."""
    for obj in bpy.data.objects:
        smooth_keyframes(obj)


# =============================================================================
# CAMERA AND LIGHTING
# =============================================================================

def create_camera(location=(0, -0.5, 0.3), rotation_deg=(75, 0, 0)):
    """Create and position the main camera."""
    bpy.ops.object.camera_add(location=location)
    camera = bpy.context.object
    camera.name = "MainCamera"

    camera.rotation_euler = (
        math.radians(rotation_deg[0]),
        math.radians(rotation_deg[1]),
        math.radians(rotation_deg[2])
    )

    bpy.context.scene.camera = camera

    camera.data.lens = 35
    camera.data.clip_start = 0.01
    camera.data.clip_end = 100

    return camera


def create_lights():
    """Create lighting setup for cel-shaded look."""
    # Key light
    bpy.ops.object.light_add(type='SUN', location=(2, -2, 3))
    key_light = bpy.context.object
    key_light.name = "KeyLight"
    key_light.data.energy = 3.0
    key_light.rotation_euler = (math.radians(45), math.radians(20), math.radians(30))

    # Fill light
    bpy.ops.object.light_add(type='SUN', location=(-2, -1, 2))
    fill_light = bpy.context.object
    fill_light.name = "FillLight"
    fill_light.data.energy = 1.0
    fill_light.rotation_euler = (math.radians(60), math.radians(-20), math.radians(-20))

    return key_light, fill_light
