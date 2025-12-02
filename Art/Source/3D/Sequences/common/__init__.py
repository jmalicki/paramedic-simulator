"""
Common utilities for procedural sequence generation.
"""

from .core import (
    CONFIG,
    clear_scene,
    setup_scene,
    seconds_to_frame,
    frame_to_seconds,
    set_keyframe,
    create_camera,
    create_lights,
)

from .materials import (
    create_toon_material,
    create_emissive_material,
    COLORS,
)

from .models import (
    create_patient_arm,
    create_gloved_hand,
)

__all__ = [
    'CONFIG',
    'clear_scene',
    'setup_scene',
    'seconds_to_frame',
    'frame_to_seconds',
    'set_keyframe',
    'create_camera',
    'create_lights',
    'create_toon_material',
    'create_emissive_material',
    'COLORS',
    'create_patient_arm',
    'create_gloved_hand',
]
