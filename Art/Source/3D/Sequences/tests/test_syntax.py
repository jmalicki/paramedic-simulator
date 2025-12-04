"""
Tests for verifying Python syntax and basic structure of Blender scripts.

These tests can run without Blender installed - they only check for syntax errors
and basic Python code quality.
"""

import ast
import os
import sys
from pathlib import Path

import pytest


# Get the Sequences directory
SEQUENCES_DIR = Path(__file__).parent.parent


def get_python_files():
    """Get all Python files in the Sequences directory."""
    python_files = []
    for path in SEQUENCES_DIR.rglob("*.py"):
        # Skip test files and __pycache__
        if "tests" in path.parts or "__pycache__" in path.parts:
            continue
        python_files.append(path)
    return python_files


class TestPythonSyntax:
    """Tests that verify Python files have valid syntax."""

    @pytest.mark.parametrize(
        "python_file",
        get_python_files(),
        ids=lambda p: str(p.relative_to(SEQUENCES_DIR)),
    )
    def test_file_has_valid_syntax(self, python_file):
        """Each Python file should parse without syntax errors."""
        source = python_file.read_text(encoding="utf-8")
        try:
            ast.parse(source, filename=str(python_file))
        except SyntaxError as e:
            pytest.fail(f"Syntax error in {python_file}: {e}")

    @pytest.mark.parametrize(
        "python_file",
        get_python_files(),
        ids=lambda p: str(p.relative_to(SEQUENCES_DIR)),
    )
    def test_file_is_valid_utf8(self, python_file):
        """Each Python file should be valid UTF-8."""
        try:
            python_file.read_text(encoding="utf-8")
        except UnicodeDecodeError as e:
            pytest.fail(f"Invalid UTF-8 in {python_file}: {e}")

    @pytest.mark.parametrize(
        "python_file",
        get_python_files(),
        ids=lambda p: str(p.relative_to(SEQUENCES_DIR)),
    )
    def test_file_has_docstring(self, python_file):
        """Each Python file should have a module docstring."""
        source = python_file.read_text(encoding="utf-8")
        tree = ast.parse(source)

        docstring = ast.get_docstring(tree)
        assert docstring is not None, f"Missing module docstring in {python_file}"

    def test_common_package_exists(self):
        """The common package should exist."""
        common_dir = SEQUENCES_DIR / "common"
        assert common_dir.exists(), "common/ directory should exist"
        assert (common_dir / "__init__.py").exists(), "common/__init__.py should exist"

    def test_procedures_package_exists(self):
        """The procedures package should exist."""
        procedures_dir = SEQUENCES_DIR / "procedures"
        assert procedures_dir.exists(), "procedures/ directory should exist"
        assert (procedures_dir / "__init__.py").exists(), (
            "procedures/__init__.py should exist"
        )


class TestModuleStructure:
    """Tests for module structure and required definitions."""

    def test_core_has_config(self):
        """core.py should define CONFIG dict."""
        core_path = SEQUENCES_DIR / "common" / "core.py"
        source = core_path.read_text(encoding="utf-8")
        tree = ast.parse(source)

        config_found = False
        for node in ast.walk(tree):
            if isinstance(node, ast.Assign):
                for target in node.targets:
                    if isinstance(target, ast.Name) and target.id == "CONFIG":
                        config_found = True
                        break

        assert config_found, "core.py should define CONFIG"

    def test_materials_has_colors(self):
        """materials.py should define COLORS dict."""
        materials_path = SEQUENCES_DIR / "common" / "materials.py"
        source = materials_path.read_text(encoding="utf-8")
        tree = ast.parse(source)

        colors_found = False
        for node in ast.walk(tree):
            if isinstance(node, ast.Assign):
                for target in node.targets:
                    if isinstance(target, ast.Name) and target.id == "COLORS":
                        colors_found = True
                        break

        assert colors_found, "materials.py should define COLORS"

    def test_core_has_required_functions(self):
        """core.py should define required utility functions."""
        core_path = SEQUENCES_DIR / "common" / "core.py"
        source = core_path.read_text(encoding="utf-8")
        tree = ast.parse(source)

        required_functions = {
            "seconds_to_frame",
            "frame_to_seconds",
            "clear_scene",
            "setup_scene",
            "create_camera",
            "create_lights",
        }

        defined_functions = set()
        for node in ast.walk(tree):
            if isinstance(node, ast.FunctionDef):
                defined_functions.add(node.name)

        missing = required_functions - defined_functions
        assert not missing, f"core.py missing functions: {missing}"

    def test_materials_has_required_functions(self):
        """materials.py should define required material functions."""
        materials_path = SEQUENCES_DIR / "common" / "materials.py"
        source = materials_path.read_text(encoding="utf-8")
        tree = ast.parse(source)

        required_functions = {
            "create_toon_material",
            "create_emissive_material",
            "get_or_create_material",
        }

        defined_functions = set()
        for node in ast.walk(tree):
            if isinstance(node, ast.FunctionDef):
                defined_functions.add(node.name)

        missing = required_functions - defined_functions
        assert not missing, f"materials.py missing functions: {missing}"


class TestColorPalette:
    """Tests for the color palette defined in materials.py."""

    def get_colors_dict(self):
        """Parse and return the COLORS dict from materials.py."""
        materials_path = SEQUENCES_DIR / "common" / "materials.py"
        source = materials_path.read_text(encoding="utf-8")

        # Execute in a restricted namespace to extract COLORS
        # We can safely eval the dict literal from the source
        tree = ast.parse(source)

        for node in ast.walk(tree):
            if isinstance(node, ast.Assign):
                for target in node.targets:
                    if isinstance(target, ast.Name) and target.id == "COLORS":
                        # Safely evaluate the dict literal
                        try:
                            return ast.literal_eval(
                                ast.unparse(node.value)  # type: ignore
                            )
                        except (ValueError, SyntaxError):
                            # If it can't be evaluated as literal, skip this test
                            pytest.skip("COLORS is not a literal dict")
        return None

    def test_colors_has_required_entries(self):
        """COLORS should have all required color entries."""
        colors = self.get_colors_dict()
        if colors is None:
            pytest.skip("Could not parse COLORS dict")

        required_colors = [
            "glove_blue",
            "glove_purple",
            "skin_light",
            "skin_medium",
            "skin_dark",
            "pulseox_body",
            "bp_cuff_blue",
        ]

        for color_name in required_colors:
            assert color_name in colors, f"Missing required color: {color_name}"

    def test_colors_are_valid_rgba_tuples(self):
        """All colors should be valid RGBA tuples with values in [0, 1]."""
        colors = self.get_colors_dict()
        if colors is None:
            pytest.skip("Could not parse COLORS dict")

        for name, color in colors.items():
            assert isinstance(color, tuple), f"{name} should be a tuple"
            assert len(color) == 4, f"{name} should have 4 components (RGBA)"

            for i, component in enumerate(color):
                assert isinstance(
                    component, (int, float)
                ), f"{name}[{i}] should be numeric"
                assert (
                    0 <= component <= 1
                ), f"{name}[{i}] should be in [0, 1], got {component}"
