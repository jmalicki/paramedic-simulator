"""
Material definitions for cel-shaded rendering.
"""

import bpy

# =============================================================================
# COLOR PALETTE
# =============================================================================

COLORS = {
    # Gloves and hands
    "glove_blue": (0.420, 0.545, 0.643, 1.0),       # #6B8BA4
    "glove_purple": (0.502, 0.388, 0.569, 1.0),     # #806391

    # Skin tones
    "skin_light": (1.0, 0.855, 0.725, 1.0),          # #FFDAB9
    "skin_medium": (0.824, 0.584, 0.427, 1.0),       # #D2956D
    "skin_dark": (0.553, 0.333, 0.141, 1.0),         # #8D5524

    # Pulse Oximeter
    "pulseox_body": (0.910, 0.910, 0.910, 1.0),      # #E8E8E8
    "pulseox_clip": (0.290, 0.290, 0.290, 1.0),      # #4A4A4A
    "pulseox_screen_off": (0.102, 0.102, 0.180, 1.0),  # #1A1A2E
    "pulseox_screen_on": (0.0, 1.0, 0.3, 1.0),       # Green glow
    "pulseox_led_red": (1.0, 0.0, 0.0, 1.0),

    # BP Cuff
    "bp_cuff_blue": (0.180, 0.525, 0.670, 1.0),      # #2E86AB
    "bp_cuff_bladder": (0.290, 0.290, 0.290, 1.0),   # #4A4A4A
    "bp_bulb": (0.290, 0.290, 0.290, 1.0),           # #4A4A4A
    "bp_gauge": (0.910, 0.910, 0.910, 1.0),          # #E8E8E8

    # General
    "outline": (0.1, 0.1, 0.1, 1.0),
    "white": (1.0, 1.0, 1.0, 1.0),
    "black": (0.05, 0.05, 0.05, 1.0),
}


# =============================================================================
# MATERIAL CREATION
# =============================================================================

def create_toon_material(name, base_color, shadow_strength=0.3):
    """
    Create a cel-shaded material using shader nodes.

    Args:
        name: Material name
        base_color: RGBA tuple (0-1 range)
        shadow_strength: How dark shadows are (0-1)

    Returns:
        bpy.types.Material
    """
    # Check if material already exists
    if name in bpy.data.materials:
        return bpy.data.materials[name]

    mat = bpy.data.materials.new(name=name)
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links

    nodes.clear()

    # Output
    output = nodes.new('ShaderNodeOutputMaterial')
    output.location = (400, 0)

    # Diffuse BSDF
    diffuse = nodes.new('ShaderNodeBsdfDiffuse')
    diffuse.inputs['Color'].default_value = base_color
    diffuse.location = (0, 0)

    links.new(diffuse.outputs['BSDF'], output.inputs['Surface'])

    return mat


def create_emissive_material(name, color, strength=2.0):
    """
    Create an emissive material for screens/LEDs.

    Args:
        name: Material name
        color: RGBA tuple (0-1 range)
        strength: Emission strength

    Returns:
        bpy.types.Material
    """
    if name in bpy.data.materials:
        return bpy.data.materials[name]

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


def get_or_create_material(name, color, emissive=False, emission_strength=2.0):
    """
    Get existing material or create new one.

    Args:
        name: Material name
        color: RGBA tuple or key from COLORS dict
        emissive: Whether material should be emissive
        emission_strength: Strength if emissive

    Returns:
        bpy.types.Material
    """
    # Resolve color from palette if string
    if isinstance(color, str):
        color = COLORS.get(color, COLORS["white"])

    if emissive:
        return create_emissive_material(name, color, emission_strength)
    else:
        return create_toon_material(name, color)
