#!/bin/bash
#
# Render Procedural Animation Sequences
#
# Usage:
#   ./render.sh                              # Render initial_assessment sequence
#   ./render.sh --sequence vital_signs       # Render predefined sequence
#   ./render.sh --procedures pulseox radial  # Render custom procedure list
#   ./render.sh --list                       # List available procedures
#   ./render.sh --preview                    # Open in Blender for preview
#

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_SCRIPT="$SCRIPT_DIR/compose.py"
OUTPUT_DIR="$SCRIPT_DIR/output"

# Check for Blender
if ! command -v blender &> /dev/null; then
    echo "Error: Blender not found in PATH"
    echo ""
    echo "Install options:"
    echo "  Ubuntu/Debian: sudo apt install blender"
    echo "  macOS:         brew install --cask blender"
    echo "  Download:      https://www.blender.org/download/"
    exit 1
fi

BLENDER_VERSION=$(blender --version | head -n1)
echo "Using: $BLENDER_VERSION"
echo ""

# Parse arguments
case "$1" in
    --preview|-P)
        echo "Opening in Blender for preview..."
        shift
        blender --python "$COMPOSE_SCRIPT" -- "$@"
        ;;
    --list|-l)
        blender --background --python "$COMPOSE_SCRIPT" -- --list
        ;;
    --list-sequences|-L)
        blender --background --python "$COMPOSE_SCRIPT" -- --list-sequences
        ;;
    --help|-h)
        echo "Render Procedural Animation Sequences"
        echo ""
        echo "Usage:"
        echo "  ./render.sh                                 Render initial_assessment"
        echo "  ./render.sh -s vital_signs                  Render predefined sequence"
        echo "  ./render.sh -p pulseox_apply radial_pulse   Render specific procedures"
        echo "  ./render.sh --preview                       Open in Blender for preview"
        echo "  ./render.sh --list                          List available procedures"
        echo "  ./render.sh --list-sequences                List predefined sequences"
        echo ""
        echo "Options:"
        echo "  -p, --procedures   List of procedure names to compose"
        echo "  -s, --sequence     Use a predefined sequence"
        echo "  -o, --output       Output filename (default: sequence name)"
        echo "  -d, --dir          Output directory (default: ./output)"
        echo "  -t, --transition   Transition time in seconds (default: 0.5)"
        echo ""
        echo "Output: $OUTPUT_DIR/"
        ;;
    *)
        echo "Rendering sequence..."
        mkdir -p "$OUTPUT_DIR"
        blender --background --python "$COMPOSE_SCRIPT" -- "$@"
        echo ""
        echo "Output directory: $OUTPUT_DIR/"
        ls -la "$OUTPUT_DIR/" 2>/dev/null | tail -5
        ;;
esac
