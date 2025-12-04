"""
Reusable procedural animation sequences.

Each procedure module exports:
- DURATION: float - Duration in seconds
- DESCRIPTION: str - Human-readable description
- create_equipment(): Creates procedure-specific equipment models
- animate(context, start_time): Animates the procedure starting at given time

The context dict contains shared objects:
- 'hand': Gloved hand object
- 'patient_arm': Patient arm object
- 'camera': Camera object
- Any equipment created by create_equipment()
"""

from . import pulseox_apply
from . import radial_pulse
from . import bp_cuff_apply

# Registry of all available procedures
PROCEDURES = {
    'pulseox_apply': pulseox_apply,
    'radial_pulse': radial_pulse,
    'bp_cuff_apply': bp_cuff_apply,
}

def get_procedure(name):
    """Get a procedure module by name."""
    return PROCEDURES.get(name)

def list_procedures():
    """List all available procedure names."""
    return list(PROCEDURES.keys())
