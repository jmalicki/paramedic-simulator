"""
Pytest configuration for Blender script tests.
"""

import sys
from pathlib import Path

import pytest


# Add the Sequences directory to the path so tests can import modules
SEQUENCES_DIR = Path(__file__).parent.parent
sys.path.insert(0, str(SEQUENCES_DIR))


def pytest_configure(config):
    """Configure pytest markers."""
    config.addinivalue_line(
        "markers", "blender: marks tests that require Blender (deselect with '-m \"not blender\"')"
    )


@pytest.fixture
def sequences_dir():
    """Return the path to the Sequences directory."""
    return SEQUENCES_DIR


@pytest.fixture
def common_dir(sequences_dir):
    """Return the path to the common module directory."""
    return sequences_dir / "common"


@pytest.fixture
def procedures_dir(sequences_dir):
    """Return the path to the procedures module directory."""
    return sequences_dir / "procedures"
