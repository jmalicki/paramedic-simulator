#!/bin/bash
#
# Render Initial Assessment Sequence
#
# Usage:
#   ./render.sh              # Render with default settings
#   ./render.sh --preview    # Open in Blender for preview (no render)
#   ./render.sh --frames     # Render as PNG frames instead of video
#

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PYTHON_SCRIPT="$SCRIPT_DIR/generate_initial_assessment.py"
OUTPUT_DIR="$SCRIPT_DIR/output"

# Check for Blender
if ! command -v blender &> /dev/null; then
    echo "Error: Blender not found in PATH"
    echo "Please install Blender or add it to your PATH"
    echo ""
    echo "Install options:"
    echo "  Ubuntu/Debian: sudo apt install blender"
    echo "  macOS:         brew install --cask blender"
    echo "  Or download:   https://www.blender.org/download/"
    exit 1
fi

BLENDER_VERSION=$(blender --version | head -n1)
echo "Using: $BLENDER_VERSION"
echo ""

# Parse arguments
case "$1" in
    --preview)
        echo "Opening in Blender for preview..."
        blender --python "$PYTHON_SCRIPT"
        ;;
    --frames)
        echo "Rendering as PNG frames..."
        mkdir -p "$OUTPUT_DIR/frames"
        # Use heredoc with proper quoting to handle paths with spaces
        blender --background --python-expr "$(cat <<PYTHON
import bpy
import sys
sys.path.insert(0, r'''${SCRIPT_DIR}''')
import generate_initial_assessment as gen
gen.CONFIG['output_dir'] = r'''${OUTPUT_DIR}/frames'''
gen.main()
bpy.context.scene.render.image_settings.file_format = 'PNG'
bpy.context.scene.render.filepath = r'''${OUTPUT_DIR}/frames/frame_'''
bpy.ops.render.render(animation=True)
PYTHON
)"
        echo ""
        echo "Frames saved to: $OUTPUT_DIR/frames/"
        ;;
    --help|-h)
        echo "Render Initial Assessment Sequence"
        echo ""
        echo "Usage:"
        echo "  ./render.sh              Render as MP4 video"
        echo "  ./render.sh --preview    Open in Blender for preview"
        echo "  ./render.sh --frames     Render as PNG frame sequence"
        echo "  ./render.sh --help       Show this help"
        echo ""
        echo "Output: $OUTPUT_DIR/"
        ;;
    *)
        echo "Rendering video..."
        mkdir -p "$OUTPUT_DIR"
        blender --background --python "$PYTHON_SCRIPT"
        echo ""
        if [ -f "$OUTPUT_DIR/initial_assessment.mp4" ]; then
            echo "Success! Video saved to:"
            echo "  $OUTPUT_DIR/initial_assessment.mp4"
            echo ""
            # Show file info if ffprobe is available
            if command -v ffprobe &> /dev/null; then
                ffprobe -v quiet -show_format -show_streams "$OUTPUT_DIR/initial_assessment.mp4" 2>/dev/null | grep -E "(duration|width|height|codec_name)" | head -6
            fi
        else
            echo "Note: Output may be at $OUTPUT_DIR/initial_assessment0001-0285.mp4"
            echo "      (Blender appends frame range to filename)"
            ls -la "$OUTPUT_DIR/" 2>/dev/null
        fi
        ;;
esac
