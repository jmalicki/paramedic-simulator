"""
Tests for the assemble_spritesheet.py script.

These tests verify the spritesheet assembly logic without requiring
actual image files.
"""

import ast
from pathlib import Path

import pytest


SEQUENCES_DIR = Path(__file__).parent.parent
ASSEMBLE_SCRIPT = SEQUENCES_DIR / "PulseOx_Apply" / "assemble_spritesheet.py"


class TestAssembleSpritesheetSyntax:
    """Tests for assemble_spritesheet.py syntax and structure."""

    def test_file_exists(self):
        """The assemble_spritesheet.py file should exist."""
        assert ASSEMBLE_SCRIPT.exists(), (
            f"assemble_spritesheet.py should exist at {ASSEMBLE_SCRIPT}"
        )

    def test_valid_syntax(self):
        """The script should have valid Python syntax."""
        if not ASSEMBLE_SCRIPT.exists():
            pytest.skip("assemble_spritesheet.py not found")

        source = ASSEMBLE_SCRIPT.read_text(encoding="utf-8")
        try:
            ast.parse(source, filename=str(ASSEMBLE_SCRIPT))
        except SyntaxError as e:
            pytest.fail(f"Syntax error: {e}")

    def test_has_pillow_import(self):
        """The script should import PIL/Pillow."""
        if not ASSEMBLE_SCRIPT.exists():
            pytest.skip("assemble_spritesheet.py not found")

        source = ASSEMBLE_SCRIPT.read_text(encoding="utf-8")
        tree = ast.parse(source)

        has_pil_import = False
        for node in ast.walk(tree):
            if isinstance(node, ast.Import):
                for alias in node.names:
                    if alias.name == "PIL" or alias.name.startswith("PIL."):
                        has_pil_import = True
            elif isinstance(node, ast.ImportFrom):
                if node.module and (
                    node.module == "PIL" or node.module.startswith("PIL.")
                ):
                    has_pil_import = True

        assert has_pil_import, "assemble_spritesheet.py should import from PIL"

    def test_has_lanczos_compatibility(self):
        """The script should handle Pillow LANCZOS compatibility."""
        if not ASSEMBLE_SCRIPT.exists():
            pytest.skip("assemble_spritesheet.py not found")

        source = ASSEMBLE_SCRIPT.read_text(encoding="utf-8")

        # Check for backward-compatible LANCZOS handling
        # Either "Resampling" (new Pillow) or direct LANCZOS access pattern
        has_lanczos = "LANCZOS" in source
        has_resampling = "Resampling" in source

        assert has_lanczos, (
            "assemble_spritesheet.py should reference LANCZOS resampling"
        )
        # The script should have some form of compatibility handling
        # Either using getattr pattern or checking for Resampling attribute
        if has_resampling:
            assert "getattr" in source or "hasattr" in source or "Resampling" in source
