#!/bin/bash
# Render Unity sequence in batch mode

set -e

# Default values
SEQUENCE="${1:-initial_assessment}"
RESOLUTION="1280x720"
OUTPUT_DIR="Art/Source/3D/Sequences/output"

# Parse additional arguments
shift || true
while [[ $# -gt 0 ]]; do
    case $1 in
        --resolution)
            RESOLUTION="$2"
            shift 2
            ;;
        --output)
            OUTPUT_DIR="$2"
            shift 2
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Detect Unity path
if command -v unity &> /dev/null; then
    UNITY_PATH="unity"
elif [ -f "$HOME/unity-editor/Editor/Unity" ]; then
    UNITY_PATH="$HOME/unity-editor/Editor/Unity"
elif [ -f "/Applications/Unity/Hub/Editor/2022.3.51f1/Unity.app/Contents/MacOS/Unity" ]; then
    UNITY_PATH="/Applications/Unity/Hub/Editor/2022.3.51f1/Unity.app/Contents/MacOS/Unity"
elif [ -f "/opt/Unity/Editor/Unity" ]; then
    UNITY_PATH="/opt/Unity/Editor/Unity"
elif [ -f "C:/Program Files/Unity/Hub/Editor/2022.3.51f1/Editor/Unity.exe" ]; then
    UNITY_PATH="C:/Program Files/Unity/Hub/Editor/2022.3.51f1/Editor/Unity.exe"
else
    echo "Error: Unity not found. Please install Unity 2022.3 LTS or add it to PATH"
    exit 1
fi

# Get project root
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_PATH="$SCRIPT_DIR"

echo "======================================"
echo "Unity Sequence Renderer"
echo "======================================"
echo "Sequence:    $SEQUENCE"
echo "Resolution:  $RESOLUTION"
echo "Output:      $OUTPUT_DIR"
echo "Project:     $PROJECT_PATH"
echo "Unity:       $UNITY_PATH"
echo "======================================"

# Create output directory
mkdir -p "../$OUTPUT_DIR"

# Run Unity in batch mode
"$UNITY_PATH" \
    -batchmode \
    -nographics \
    -projectPath "$PROJECT_PATH" \
    -executeMethod ParamedicSimulator.Editor.BatchModeRecorder.RecordSequence \
    --sequence "$SEQUENCE" \
    --output "$OUTPUT_DIR" \
    --resolution "$RESOLUTION" \
    -logFile unity_render.log \
    -quit

# Check if rendering succeeded
if [ $? -eq 0 ]; then
    echo "======================================"
    echo "✓ Rendering complete!"
    echo "Video: ../$OUTPUT_DIR/$SEQUENCE.mp4"
    echo "======================================"

    # Show video info if ffprobe is available
    if command -v ffprobe &> /dev/null; then
        echo ""
        echo "Video info:"
        ffprobe -v quiet -print_format json -show_streams "../$OUTPUT_DIR/$SEQUENCE.mp4" | grep -E '"width"|"height"|"duration"|"codec_name"' || true
    fi
else
    echo "======================================"
    echo "✗ Rendering failed! Check unity_render.log"
    echo "======================================"
    tail -n 50 unity_render.log
    exit 1
fi
