# Paramedic Simulator Build System
#
# Usage:
#   just render                    # Render all sequences
#   just render initial_assessment # Render single sequence
#   just fbx                       # Export all FBX from Blender
#   just clean                     # Remove generated files
#   just help                      # Show all targets

# Configuration
blender := "blender"
unity := home_directory() / "Unity/Hub/Editor/6000.2.14f1/Editor/Unity"
resolution := "1280x720"

# Directories
project_root := justfile_directory()
blender_dir := project_root / "Art/Source/3D/Sequences"
unity_dir := project_root / "UnityProject"
models_dir := unity_dir / "Assets/Models"
output_dir := project_root / "Art/Source/3D/Sequences/output"

# All sequences
sequences := "pulseox_apply radial_pulse bp_cuff_apply initial_assessment"

# Default: show help
default:
    @just --list

# Show help
help:
    @echo "Paramedic Simulator Build System"
    @echo ""
    @echo "Usage: just <recipe> [arguments]"
    @echo ""
    @just --list

# =============================================================================
# Main Recipes
# =============================================================================

# Render all video sequences
render-all: ensure-dirs
    #!/usr/bin/env bash
    set -e
    for seq in {{sequences}}; do
        just render "$seq"
    done
    echo ""
    echo "========================================"
    echo "All sequences rendered!"
    echo "========================================"
    ls -la "{{output_dir}}"/*.webm 2>/dev/null || true

# Render a single sequence (Blender FBX → Unity video)
render sequence="initial_assessment": ensure-dirs
    #!/usr/bin/env bash
    set -e
    echo "========================================"
    echo "Rendering: {{sequence}}"
    echo "========================================"

    # Step 1: Export FBX from Blender (if blender available)
    if command -v {{blender}} &> /dev/null; then
        echo ""
        echo "[Step 1/2] Exporting FBX from Blender..."
        cd "{{blender_dir}}"
        {{blender}} --background --python compose.py -- --fbx --sequence "{{sequence}}" 2>&1 | \
            grep -E "^\[|^Exporting|^Saved|^Error|^Warning" || true

        if [ -f "{{blender_dir}}/output/{{sequence}}.fbx" ]; then
            cp "{{blender_dir}}/output/{{sequence}}.fbx" "{{models_dir}}/"
            echo "✓ FBX exported: {{models_dir}}/{{sequence}}.fbx"
        else
            echo "⚠ FBX not created, Unity will use procedural fallback"
        fi
    else
        echo "[Step 1/2] Skipping Blender (not installed)"
    fi

    # Step 2: Render with Unity
    echo ""
    echo "[Step 2/2] Rendering video with Unity..."
    cd "{{unity_dir}}"
    ./render.sh "{{sequence}}" --resolution "{{resolution}}" --output "{{output_dir}}"

# =============================================================================
# FBX Export Only
# =============================================================================

# Export all FBX files from Blender
fbx-all: ensure-dirs
    #!/usr/bin/env bash
    set -e
    for seq in {{sequences}}; do
        just fbx "$seq"
    done

# Export single FBX from Blender
fbx sequence="initial_assessment": ensure-dirs
    #!/usr/bin/env bash
    set -e
    echo "========================================"
    echo "Exporting FBX: {{sequence}}"
    echo "========================================"
    cd "{{blender_dir}}"
    {{blender}} --background --python compose.py -- --fbx --sequence "{{sequence}}"

    if [ -f "{{blender_dir}}/output/{{sequence}}.fbx" ]; then
        cp "{{blender_dir}}/output/{{sequence}}.fbx" "{{models_dir}}/"
        echo "✓ Created: {{models_dir}}/{{sequence}}.fbx"
    fi

# =============================================================================
# Unity Rendering Only
# =============================================================================

# Render video with Unity (skip Blender, use existing FBX or procedural)
unity sequence="initial_assessment": ensure-dirs
    #!/usr/bin/env bash
    set -e
    echo "========================================"
    echo "Unity Rendering: {{sequence}}"
    echo "========================================"
    cd "{{unity_dir}}"
    ./render.sh "{{sequence}}" --resolution "{{resolution}}" --output "{{output_dir}}"

# =============================================================================
# Cleaning
# =============================================================================

# Remove all generated files
clean: clean-videos clean-fbx
    @echo "✓ Cleaned all generated files"

# Remove only video files
clean-videos:
    rm -f "{{output_dir}}"/*.webm
    @echo "✓ Removed video files"

# Remove only FBX files
clean-fbx:
    rm -f "{{models_dir}}"/*.fbx
    rm -f "{{blender_dir}}/output"/*.fbx
    @echo "✓ Removed FBX files"

# =============================================================================
# Development Helpers
# =============================================================================

# Check all dependencies
check:
    #!/usr/bin/env bash
    echo "Checking dependencies..."
    echo ""

    # Blender
    if command -v {{blender}} &> /dev/null; then
        echo "✓ Blender: $({{blender}} --version 2>/dev/null | head -1)"
    else
        echo "✗ Blender: not found"
    fi

    # Unity
    if [ -f "{{unity}}" ]; then
        echo "✓ Unity: {{unity}}"
    else
        echo "✗ Unity: not found at {{unity}}"
    fi

    # ffprobe (optional)
    if command -v ffprobe &> /dev/null; then
        echo "✓ ffprobe: available"
    else
        echo "○ ffprobe: not found (optional, for video info)"
    fi

    # just
    echo "✓ just: $(just --version)"

# List all generated files
list:
    @echo "FBX files in {{models_dir}}:"
    @ls -la "{{models_dir}}"/*.fbx 2>/dev/null || echo "  (none)"
    @echo ""
    @echo "Video files in {{output_dir}}:"
    @ls -la "{{output_dir}}"/*.webm 2>/dev/null || echo "  (none)"

# Watch Blender files and re-export on change (requires entr)
watch:
    #!/usr/bin/env bash
    if ! command -v entr &> /dev/null; then
        echo "Install entr: sudo apt install entr"
        exit 1
    fi
    echo "Watching for changes in {{blender_dir}}..."
    find "{{blender_dir}}" -name "*.py" | entr -c just fbx-all

# Open Unity project in editor
open-unity:
    "{{unity}}" -projectPath "{{unity_dir}}" &

# =============================================================================
# Internal
# =============================================================================

# Ensure output directories exist
[private]
ensure-dirs:
    @mkdir -p "{{models_dir}}"
    @mkdir -p "{{output_dir}}"
