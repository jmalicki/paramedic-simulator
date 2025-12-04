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

# Detect Unity path - check Unity Hub locations first, then fallback paths
UNITY_PATH=""

# Linux Unity Hub - prefer the version matching project
if [ -d "$HOME/Unity/Hub/Editor" ]; then
    # First check for exact version match from ProjectVersion.txt
    PROJECT_VERSION=$(grep "m_EditorVersion:" "$(dirname "${BASH_SOURCE[0]}")/ProjectSettings/ProjectVersion.txt" 2>/dev/null | sed 's/.*: //')
    if [ -n "$PROJECT_VERSION" ] && [ -f "$HOME/Unity/Hub/Editor/$PROJECT_VERSION/Editor/Unity" ]; then
        UNITY_PATH="$HOME/Unity/Hub/Editor/$PROJECT_VERSION/Editor/Unity"
    else
        # Fall back to any installed Unity 6 version
        UNITY_PATH=$(ls -d "$HOME/Unity/Hub/Editor"/6*/Editor/Unity 2>/dev/null | sort -rV | head -1)
    fi
fi
# macOS Unity Hub
if [ -z "$UNITY_PATH" ] && [ -d "/Applications/Unity/Hub/Editor" ]; then
    UNITY_PATH=$(find "/Applications/Unity/Hub/Editor" -path "*/Unity.app/Contents/MacOS/Unity" -type f 2>/dev/null | head -1)
fi
# Windows Unity Hub (Git Bash / WSL)
if [ -z "$UNITY_PATH" ] && [ -d "/c/Program Files/Unity/Hub/Editor" ]; then
    UNITY_PATH=$(find "/c/Program Files/Unity/Hub/Editor" -maxdepth 3 -name "Unity.exe" -type f 2>/dev/null | head -1)
fi
# Fallback paths
if [ -z "$UNITY_PATH" ] && command -v unity &> /dev/null; then
    UNITY_PATH="unity"
elif [ -z "$UNITY_PATH" ] && [ -f "$HOME/unity-editor/Editor/Unity" ]; then
    UNITY_PATH="$HOME/unity-editor/Editor/Unity"
elif [ -z "$UNITY_PATH" ] && [ -f "/opt/Unity/Editor/Unity" ]; then
    UNITY_PATH="/opt/Unity/Editor/Unity"
fi

if [ -z "$UNITY_PATH" ]; then
    echo "Error: Unity not found. Please install Unity via Unity Hub or add it to PATH"
    echo "Checked locations:"
    echo "  - $HOME/Unity/Hub/Editor/*/Editor/Unity (Linux Hub)"
    echo "  - /Applications/Unity/Hub/Editor/*/Unity.app/Contents/MacOS/Unity (macOS Hub)"
    echo "  - /c/Program Files/Unity/Hub/Editor/*/Editor/Unity.exe (Windows Hub)"
    echo "  - unity command in PATH"
    echo "  - $HOME/unity-editor/Editor/Unity"
    echo "  - /opt/Unity/Editor/Unity"
    exit 1
fi

# Get project root
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_PATH="$SCRIPT_DIR"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Make output path absolute if it's relative
if [[ "$OUTPUT_DIR" != /* ]]; then
    OUTPUT_DIR_ABS="$PROJECT_ROOT/$OUTPUT_DIR"
else
    OUTPUT_DIR_ABS="$OUTPUT_DIR"
fi

echo "======================================"
echo "Unity Sequence Renderer"
echo "======================================"
echo "Sequence:    $SEQUENCE"
echo "Resolution:  $RESOLUTION"
echo "Output:      $OUTPUT_DIR_ABS"
echo "Project:     $PROJECT_PATH"
echo "Unity:       $UNITY_PATH"
echo "======================================"

# Create output directory
mkdir -p "$OUTPUT_DIR_ABS"

# Check for display (X11 or Wayland)
# Unity 6 supports both X11 (DISPLAY) and Wayland (WAYLAND_DISPLAY)
HAS_DISPLAY=""
if [[ -n "${WAYLAND_DISPLAY:-}" ]]; then
    echo "Using Wayland display: $WAYLAND_DISPLAY"
    HAS_DISPLAY="wayland"
elif [[ -n "${DISPLAY:-}" ]]; then
    echo "Using X11 display: $DISPLAY"
    HAS_DISPLAY="x11"
fi

if [[ -z "$HAS_DISPLAY" ]] && [[ "$(uname)" == "Linux" ]]; then
    echo "Warning: No display found (neither WAYLAND_DISPLAY nor DISPLAY set)."
    echo "Unity Recorder requires a display for GPU-accelerated video encoding."
    echo "Options:"
    echo "  1. Run from a desktop session (recommended)"
    echo "  2. Use headless EGL (NVIDIA GPUs with proper drivers)"
fi

# Run Unity in batch mode
# Note: We use batchmode but NOT nographics since we need GPU for video encoding
# Note: We don't use -quit because the recording happens asynchronously after play mode starts
#       BatchModeRecorder calls EditorApplication.Exit() when recording completes
# -force-vulkan ensures GPU rendering on Linux (alternatively use -force-glcore for OpenGL)

# Clear old log
> unity_render.log

# Start Unity in background
"$UNITY_PATH" \
    -batchmode \
    -force-vulkan \
    -projectPath "$PROJECT_PATH" \
    -executeMethod ParamedicSimulator.Editor.BatchModeRecorder.RecordSequence \
    --sequence "$SEQUENCE" \
    --output "$OUTPUT_DIR_ABS" \
    --resolution "$RESOLUTION" \
    -logFile unity_render.log &

UNITY_PID=$!

# Show progress from log file
echo "Starting Unity (PID: $UNITY_PID)..."
echo ""

# Wait for log file to be created
sleep 2

# Monitor the log file while Unity runs
# Use a background subshell that checks if Unity is still running
(
    # Wait for log file to have content
    while [ ! -s unity_render.log ] && kill -0 $UNITY_PID 2>/dev/null; do
        sleep 0.5
    done

    # Tail the log, but exit when Unity process dies
    tail -f unity_render.log 2>/dev/null &
    TAIL_INNER_PID=$!

    # Wait for Unity to exit, then kill tail
    while kill -0 $UNITY_PID 2>/dev/null; do
        sleep 1
    done

    kill $TAIL_INNER_PID 2>/dev/null
) | while read line; do
    # Show BatchModeRecorder messages
    if [[ "$line" == *"[BatchModeRecorder]"* ]]; then
        echo "$line" | sed 's/.*\[BatchModeRecorder\]/[Render]/'
    fi
done &

MONITOR_PID=$!

# Wait for Unity to finish
wait $UNITY_PID
UNITY_EXIT_CODE=$?

# Give the monitor a moment to flush output, then kill it
sleep 1
kill $MONITOR_PID 2>/dev/null
wait $MONITOR_PID 2>/dev/null

echo ""

# Check if rendering succeeded
if [ $UNITY_EXIT_CODE -eq 0 ]; then
    # Check for video file (WebM format on Linux)
    VIDEO_FILE="$OUTPUT_DIR_ABS/$SEQUENCE.webm"
    if [ -f "$VIDEO_FILE" ]; then
        echo "======================================"
        echo "Rendering complete!"
        echo "Video: $VIDEO_FILE"
        echo "======================================"

        # Show video info if ffprobe is available
        if command -v ffprobe &> /dev/null; then
            echo ""
            echo "Video info:"
            ffprobe -v quiet -print_format json -show_streams "$VIDEO_FILE" | grep -E '"width"|"height"|"duration"|"codec_name"' || true
        fi
    else
        echo "======================================"
        echo "Warning: Unity exited successfully but video not found"
        echo "Expected: $VIDEO_FILE"
        echo "======================================"
        echo "Check unity_render.log for details"
        tail -n 30 unity_render.log | grep -E "BatchModeRecorder|Error|error|Warning|warning" || true
        exit 1
    fi
else
    echo "======================================"
    echo "Rendering failed! Check unity_render.log"
    echo "======================================"
    tail -n 50 unity_render.log
    exit 1
fi
