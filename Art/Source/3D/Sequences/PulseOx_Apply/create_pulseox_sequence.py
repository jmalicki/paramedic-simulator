"""
Blender Python Script: Pulse Oximeter Application Sequence
==========================================================
Creates a 4-frame animation sequence of applying a pulse oximeter to a patient's finger.

Usage:
1. Open Blender (2.8+ or 3.x/4.x)
2. Go to Scripting workspace
3. Open this script
4. Click "Run Script"
5. The scene will be created with animation
6. Render animation (Render > Render Animation) to output frames

Output: 4 PNG frames with transparency, ready for sprite sheet assembly
"""

import bpy
import math
from mathutils import Vector, Euler

# =============================================================================
# CONFIGURATION
# =============================================================================

CONFIG = {
    'output_path': '//renders/pulseox_apply_',  # Relative to .blend file
    'frame_width': 256,
    'frame_height': 192,
    'total_frames': 4,
    'samples': 64,  # Render samples (lower = faster, higher = cleaner)
}

# Color palette (matching artwork-specifications.md)
COLORS = {
    'skin_light': (1.0, 0.855, 0.725, 1.0),      # #FFDAB9
    'glove_blue': (0.42, 0.545, 0.643, 1.0),     # #6B8BA4
    'pulseox_white': (0.95, 0.95, 0.95, 1.0),    # Device body
    'pulseox_dark': (0.18, 0.18, 0.18, 1.0),     # Display/dark parts
    'led_red': (1.0, 0.0, 0.0, 1.0),             # IR LED
    'led_green': (0.0, 1.0, 0.0, 1.0),           # Display
    'outline': (0.176, 0.176, 0.176, 1.0),       # #2D2D2D
}


# =============================================================================
# UTILITY FUNCTIONS
# =============================================================================

def clear_scene():
    """Remove all objects from the scene."""
    bpy.ops.object.select_all(action='SELECT')
    bpy.ops.object.delete(use_global=False)

    # Clear orphan data
    for block in bpy.data.meshes:
        if block.users == 0:
            bpy.data.meshes.remove(block)
    for block in bpy.data.materials:
        if block.users == 0:
            bpy.data.materials.remove(block)


def create_cel_shade_material(name, base_color, shadow_factor=0.7, outline_thickness=0.02):
    """
    Create a cel-shaded material with toon shading.

    Args:
        name: Material name
        base_color: RGBA tuple (0-1 range)
        shadow_factor: How dark shadows are (0-1)
        outline_thickness: Solidify modifier thickness for outline
    """
    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links

    # Clear default nodes
    nodes.clear()

    # Create nodes
    output = nodes.new('ShaderNodeOutputMaterial')
    output.location = (400, 0)

    # Shader to RGB for toon effect
    diffuse = nodes.new('ShaderNodeBsdfDiffuse')
    diffuse.location = (-200, 0)
    diffuse.inputs['Color'].default_value = base_color

    shader_to_rgb = nodes.new('ShaderNodeShaderToRGB')
    shader_to_rgb.location = (0, 0)

    # Color ramp for toon bands
    ramp = nodes.new('ShaderNodeValToRGB')
    ramp.location = (200, 0)
    ramp.color_ramp.interpolation = 'CONSTANT'
    ramp.color_ramp.elements[0].position = 0.0
    ramp.color_ramp.elements[0].color = (
        base_color[0] * shadow_factor,
        base_color[1] * shadow_factor,
        base_color[2] * shadow_factor,
        1.0
    )
    ramp.color_ramp.elements[1].position = 0.5
    ramp.color_ramp.elements[1].color = base_color

    # Mix with emission for flat look
    emission = nodes.new('ShaderNodeEmission')
    emission.location = (200, -150)
    emission.inputs['Strength'].default_value = 0.1

    mix = nodes.new('ShaderNodeMixShader')
    mix.location = (400, -100)
    mix.inputs['Fac'].default_value = 0.9

    # Connect nodes
    links.new(diffuse.outputs['BSDF'], shader_to_rgb.inputs['Shader'])
    links.new(shader_to_rgb.outputs['Color'], ramp.inputs['Fac'])
    links.new(ramp.outputs['Color'], emission.inputs['Color'])
    links.new(shader_to_rgb.outputs['Color'], mix.inputs[1])
    links.new(emission.outputs['Emission'], mix.inputs[2])
    links.new(mix.outputs['Shader'], output.inputs['Surface'])

    return mat


def create_outline_material():
    """Create a solid black material for outlines."""
    mat = bpy.data.materials.new(name="Outline")
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links

    nodes.clear()

    output = nodes.new('ShaderNodeOutputMaterial')
    emission = nodes.new('ShaderNodeEmission')
    emission.inputs['Color'].default_value = COLORS['outline']
    emission.inputs['Strength'].default_value = 1.0

    links.new(emission.outputs['Emission'], output.inputs['Surface'])

    return mat


def add_outline_modifier(obj, thickness=0.02):
    """Add solidify modifier for outline effect."""
    # Duplicate for outline
    outline_obj = obj.copy()
    outline_obj.data = obj.data.copy()
    outline_obj.name = obj.name + "_outline"
    bpy.context.collection.objects.link(outline_obj)

    # Add solidify modifier
    solidify = outline_obj.modifiers.new(name="Outline", type='SOLIDIFY')
    solidify.thickness = thickness
    solidify.offset = 1.0
    solidify.use_flip_normals = True
    solidify.use_rim = False

    # Apply outline material
    outline_mat = create_outline_material()
    outline_obj.data.materials.clear()
    outline_obj.data.materials.append(outline_mat)

    # Parent to original
    outline_obj.parent = obj

    return outline_obj


# =============================================================================
# MODEL CREATION
# =============================================================================

def create_finger():
    """Create a simple finger model."""
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.15,
        depth=1.2,
        location=(0, 0, 0),
        rotation=(math.pi/2, 0, 0)
    )
    finger = bpy.context.active_object
    finger.name = "Finger"

    # Add subdivision for smoothness
    subsurf = finger.modifiers.new(name="Subsurf", type='SUBSURF')
    subsurf.levels = 2
    subsurf.render_levels = 2

    # Taper the finger (scale at tip)
    bpy.ops.object.mode_set(mode='EDIT')
    bpy.ops.mesh.select_all(action='DESELECT')
    bpy.ops.object.mode_set(mode='OBJECT')

    # Apply material
    mat = create_cel_shade_material("Skin", COLORS['skin_light'])
    finger.data.materials.append(mat)

    # Add outline
    add_outline_modifier(finger, 0.015)

    return finger


def create_pulse_oximeter():
    """Create a pulse oximeter model."""
    # Main body (clip shape)
    bpy.ops.mesh.primitive_cube_add(
        size=0.3,
        location=(0, 0, 0.5)
    )
    body = bpy.context.active_object
    body.name = "PulseOx_Body"
    body.scale = (0.8, 0.5, 0.4)

    # Round the edges
    bevel = body.modifiers.new(name="Bevel", type='BEVEL')
    bevel.width = 0.03
    bevel.segments = 3

    # Apply white material
    mat_body = create_cel_shade_material("PulseOx_White", COLORS['pulseox_white'])
    body.data.materials.append(mat_body)

    # Display screen
    bpy.ops.mesh.primitive_plane_add(
        size=0.15,
        location=(0.12, 0, 0.62)
    )
    screen = bpy.context.active_object
    screen.name = "PulseOx_Screen"
    screen.rotation_euler = (0, math.pi/2, 0)

    mat_screen = create_cel_shade_material("PulseOx_Screen", COLORS['pulseox_dark'])
    screen.data.materials.append(mat_screen)
    screen.parent = body

    # LED indicator (red)
    bpy.ops.mesh.primitive_uv_sphere_add(
        radius=0.02,
        location=(0.12, 0.08, 0.55)
    )
    led = bpy.context.active_object
    led.name = "PulseOx_LED"

    mat_led = bpy.data.materials.new(name="LED_Red")
    mat_led.use_nodes = True
    nodes = mat_led.node_tree.nodes
    nodes.clear()
    output = nodes.new('ShaderNodeOutputMaterial')
    emission = nodes.new('ShaderNodeEmission')
    emission.inputs['Color'].default_value = COLORS['led_red']
    emission.inputs['Strength'].default_value = 2.0
    mat_led.node_tree.links.new(emission.outputs['Emission'], output.inputs['Surface'])
    led.data.materials.append(mat_led)
    led.parent = body

    # Finger opening (bottom part that opens)
    bpy.ops.mesh.primitive_cube_add(
        size=0.25,
        location=(0, 0, 0.25)
    )
    clip_bottom = bpy.context.active_object
    clip_bottom.name = "PulseOx_Clip"
    clip_bottom.scale = (0.7, 0.45, 0.3)

    bevel2 = clip_bottom.modifiers.new(name="Bevel", type='BEVEL')
    bevel2.width = 0.02
    bevel2.segments = 2

    mat_clip = create_cel_shade_material("PulseOx_Clip", COLORS['pulseox_dark'])
    clip_bottom.data.materials.append(mat_clip)
    clip_bottom.parent = body

    # Add outline to main body
    add_outline_modifier(body, 0.012)

    return body


def create_gloved_hand():
    """Create a simplified gloved hand."""
    # Palm
    bpy.ops.mesh.primitive_cube_add(
        size=0.4,
        location=(-0.8, 0, 0.6)
    )
    palm = bpy.context.active_object
    palm.name = "Hand_Palm"
    palm.scale = (0.6, 0.3, 0.8)

    bevel = palm.modifiers.new(name="Bevel", type='BEVEL')
    bevel.width = 0.05
    bevel.segments = 3

    # Fingers (simplified as cylinders)
    finger_positions = [
        (-0.6, -0.08, 1.0),   # Index
        (-0.6, 0.0, 1.05),    # Middle
        (-0.6, 0.08, 1.0),    # Ring
    ]

    fingers = []
    for i, pos in enumerate(finger_positions):
        bpy.ops.mesh.primitive_cylinder_add(
            radius=0.05,
            depth=0.35,
            location=pos
        )
        finger = bpy.context.active_object
        finger.name = f"Hand_Finger_{i}"
        finger.parent = palm
        fingers.append(finger)

    # Thumb
    bpy.ops.mesh.primitive_cylinder_add(
        radius=0.05,
        depth=0.25,
        location=(-0.65, -0.15, 0.5),
        rotation=(0, math.pi/4, math.pi/6)
    )
    thumb = bpy.context.active_object
    thumb.name = "Hand_Thumb"
    thumb.parent = palm

    # Apply glove material to all parts
    mat_glove = create_cel_shade_material("Glove", COLORS['glove_blue'])
    palm.data.materials.append(mat_glove)
    for finger in fingers:
        finger.data.materials.append(mat_glove)
    thumb.data.materials.append(mat_glove)

    # Add outline
    add_outline_modifier(palm, 0.01)

    return palm


# =============================================================================
# ANIMATION
# =============================================================================

def setup_animation(finger, pulseox, hand):
    """
    Create 4-frame animation sequence:
    Frame 1: Hand approaching with pulse ox
    Frame 2: Pulse ox opening
    Frame 3: Placing on finger
    Frame 4: Clipped on, reading displayed
    """
    scene = bpy.context.scene
    scene.frame_start = 1
    scene.frame_end = 4

    # Frame 1: Hand approaching (pulse ox held, far from finger)
    scene.frame_set(1)
    hand.location = (-1.2, 0, 0.8)
    hand.rotation_euler = (0, 0, 0)
    hand.keyframe_insert(data_path="location")
    hand.keyframe_insert(data_path="rotation_euler")

    pulseox.location = (-0.7, 0, 0.7)
    pulseox.rotation_euler = (0, 0, 0)
    pulseox.keyframe_insert(data_path="location")
    pulseox.keyframe_insert(data_path="rotation_euler")

    # Get the clip child
    clip = None
    for child in pulseox.children:
        if "Clip" in child.name:
            clip = child
            break

    if clip:
        clip.rotation_euler = (0, 0, 0)
        clip.keyframe_insert(data_path="rotation_euler")

    # Frame 2: Pulse ox opening (clip rotates)
    scene.frame_set(2)
    hand.location = (-0.6, 0, 0.6)
    hand.keyframe_insert(data_path="location")

    pulseox.location = (-0.3, 0, 0.5)
    pulseox.keyframe_insert(data_path="location")

    if clip:
        clip.rotation_euler = (-0.4, 0, 0)  # Open ~23 degrees
        clip.keyframe_insert(data_path="rotation_euler")

    # Frame 3: Placing on finger
    scene.frame_set(3)
    hand.location = (-0.4, 0, 0.5)
    hand.keyframe_insert(data_path="location")

    pulseox.location = (0.3, 0, 0.5)  # Over finger
    pulseox.keyframe_insert(data_path="location")

    if clip:
        clip.rotation_euler = (-0.3, 0, 0)  # Still slightly open
        clip.keyframe_insert(data_path="rotation_euler")

    # Frame 4: Clipped on
    scene.frame_set(4)
    hand.location = (-0.5, 0, 0.4)  # Hand moving away
    hand.keyframe_insert(data_path="location")

    pulseox.location = (0.35, 0, 0.5)  # Slightly adjusted
    pulseox.keyframe_insert(data_path="location")

    if clip:
        clip.rotation_euler = (0, 0, 0)  # Closed on finger
        clip.keyframe_insert(data_path="rotation_euler")

    # Set interpolation to constant for snappy animation
    for obj in [hand, pulseox]:
        if obj.animation_data and obj.animation_data.action:
            for fcurve in obj.animation_data.action.fcurves:
                for keyframe in fcurve.keyframe_points:
                    keyframe.interpolation = 'CONSTANT'

    if clip and clip.animation_data and clip.animation_data.action:
        for fcurve in clip.animation_data.action.fcurves:
            for keyframe in fcurve.keyframe_points:
                keyframe.interpolation = 'CONSTANT'


# =============================================================================
# SCENE SETUP
# =============================================================================

def setup_camera():
    """Set up orthographic camera for sprite rendering."""
    bpy.ops.object.camera_add(
        location=(0, -3, 0.5),
        rotation=(math.pi/2, 0, 0)
    )
    camera = bpy.context.active_object
    camera.name = "SpriteCamera"
    camera.data.type = 'ORTHO'
    camera.data.ortho_scale = 1.5

    bpy.context.scene.camera = camera

    return camera


def setup_lighting():
    """Set up simple lighting for cel-shading."""
    # Key light
    bpy.ops.object.light_add(
        type='SUN',
        location=(2, -2, 3)
    )
    key_light = bpy.context.active_object
    key_light.name = "KeyLight"
    key_light.rotation_euler = (math.pi/4, 0, math.pi/4)
    key_light.data.energy = 3.0

    # Fill light (softer)
    bpy.ops.object.light_add(
        type='SUN',
        location=(-2, -1, 2)
    )
    fill_light = bpy.context.active_object
    fill_light.name = "FillLight"
    fill_light.rotation_euler = (math.pi/3, 0, -math.pi/4)
    fill_light.data.energy = 1.0


def setup_render_settings():
    """Configure render settings for sprite output."""
    scene = bpy.context.scene

    # Resolution
    scene.render.resolution_x = CONFIG['frame_width']
    scene.render.resolution_y = CONFIG['frame_height']
    scene.render.resolution_percentage = 100

    # Output settings
    scene.render.image_settings.file_format = 'PNG'
    scene.render.image_settings.color_mode = 'RGBA'
    scene.render.film_transparent = True  # Transparent background

    # Output path
    scene.render.filepath = CONFIG['output_path']

    # Use Eevee for fast cel-shaded rendering
    scene.render.engine = 'BLENDER_EEVEE'
    scene.eevee.taa_render_samples = CONFIG['samples']

    # For Blender 4.x compatibility
    try:
        scene.eevee.use_taa_reprojection = False
    except:
        pass


def setup_world():
    """Set up world background (transparent for sprites)."""
    world = bpy.data.worlds.new(name="SpriteWorld")
    bpy.context.scene.world = world
    world.use_nodes = True

    # Clear background
    nodes = world.node_tree.nodes
    nodes.clear()

    output = nodes.new('ShaderNodeOutputWorld')
    bg = nodes.new('ShaderNodeBackground')
    bg.inputs['Color'].default_value = (0, 0, 0, 0)  # Transparent
    bg.inputs['Strength'].default_value = 0

    world.node_tree.links.new(bg.outputs['Background'], output.inputs['Surface'])


# =============================================================================
# MAIN EXECUTION
# =============================================================================

def main():
    """Main function to create the complete sequence."""
    print("=" * 60)
    print("Creating Pulse Oximeter Application Sequence")
    print("=" * 60)

    # Clear existing scene
    print("Clearing scene...")
    clear_scene()

    # Setup environment
    print("Setting up world and lighting...")
    setup_world()
    setup_lighting()
    setup_camera()
    setup_render_settings()

    # Create models
    print("Creating finger model...")
    finger = create_finger()

    print("Creating pulse oximeter model...")
    pulseox = create_pulse_oximeter()

    print("Creating hand model...")
    hand = create_gloved_hand()

    # Setup animation
    print("Setting up animation...")
    setup_animation(finger, pulseox, hand)

    # Set to first frame
    bpy.context.scene.frame_set(1)

    print("=" * 60)
    print("Scene created successfully!")
    print("")
    print("To render the sequence:")
    print("1. Press F12 to render current frame")
    print("2. Or: Render > Render Animation (Ctrl+F12)")
    print(f"3. Frames will be saved to: {CONFIG['output_path']}")
    print("=" * 60)


# Run the script
if __name__ == "__main__":
    main()
