# Paramedic Simulator Build System
#
# Usage:
#   make render                    # Render all sequences
#   make render-initial_assessment # Render single sequence
#   make fbx                       # Export all FBX from Blender
#   make clean                     # Remove generated files
#   make help                      # Show all targets

.PHONY: all render fbx clean help install-deps

# Configuration
BLENDER := blender
UNITY := $(HOME)/Unity/Hub/Editor/6000.2.14f1/Editor/Unity
RESOLUTION := 1280x720

# Directories
PROJECT_ROOT := $(shell pwd)
BLENDER_DIR := $(PROJECT_ROOT)/Art/Source/3D/Sequences
UNITY_DIR := $(PROJECT_ROOT)/UnityProject
MODELS_DIR := $(UNITY_DIR)/Assets/Models
OUTPUT_DIR := $(PROJECT_ROOT)/Art/Source/3D/Sequences/output

# Sequences
SEQUENCES := pulseox_apply radial_pulse bp_cuff_apply initial_assessment

# Generated files
FBX_FILES := $(addprefix $(MODELS_DIR)/,$(addsuffix .fbx,$(SEQUENCES)))
VIDEO_FILES := $(addprefix $(OUTPUT_DIR)/,$(addsuffix .webm,$(SEQUENCES)))

# Default target
all: render

# Help
help:
	@echo "Paramedic Simulator Build System"
	@echo ""
	@echo "Targets:"
	@echo "  make render                     - Render all video sequences"
	@echo "  make render-<sequence>          - Render single sequence"
	@echo "  make fbx                        - Export all FBX from Blender"
	@echo "  make fbx-<sequence>             - Export single FBX"
	@echo "  make clean                      - Remove generated files"
	@echo "  make clean-videos               - Remove only video files"
	@echo "  make clean-fbx                  - Remove only FBX files"
	@echo ""
	@echo "Sequences: $(SEQUENCES)"
	@echo ""
	@echo "Options (override with make VAR=value):"
	@echo "  RESOLUTION=$(RESOLUTION)"
	@echo ""
	@echo "Examples:"
	@echo "  make render-initial_assessment"
	@echo "  make render RESOLUTION=1920x1080"
	@echo "  make fbx-pulseox_apply"

# Create directories
$(MODELS_DIR) $(OUTPUT_DIR):
	mkdir -p $@

# =============================================================================
# FBX Export (Blender)
# =============================================================================

fbx: $(FBX_FILES)

# Pattern rule: Export FBX from Blender
# Depends on compose.py and procedure files
$(MODELS_DIR)/%.fbx: $(BLENDER_DIR)/compose.py $(BLENDER_DIR)/procedures/*.py | $(MODELS_DIR)
	@echo "========================================"
	@echo "Exporting FBX: $*"
	@echo "========================================"
	cd $(BLENDER_DIR) && $(BLENDER) --background --python compose.py -- --fbx --sequence $*
	@if [ -f "$(BLENDER_DIR)/output/$*.fbx" ]; then \
		cp "$(BLENDER_DIR)/output/$*.fbx" "$@"; \
		echo "Created: $@"; \
	else \
		echo "Warning: Blender did not create $*.fbx"; \
	fi

# Individual FBX targets
fbx-%: $(MODELS_DIR)/%.fbx
	@echo "FBX export complete: $*"

# =============================================================================
# Video Rendering (Unity)
# =============================================================================

render: $(VIDEO_FILES)
	@echo ""
	@echo "========================================"
	@echo "All sequences rendered!"
	@echo "========================================"
	@ls -la $(OUTPUT_DIR)/*.webm 2>/dev/null || true

# Pattern rule: Render video with Unity
# Depends on FBX file (will use procedural fallback if FBX doesn't exist)
$(OUTPUT_DIR)/%.webm: $(MODELS_DIR)/%.fbx | $(OUTPUT_DIR)
	@echo "========================================"
	@echo "Rendering: $*"
	@echo "========================================"
	cd $(UNITY_DIR) && ./render.sh $* --resolution $(RESOLUTION) --output $(OUTPUT_DIR)

# Render without requiring FBX (procedural fallback)
render-%: | $(OUTPUT_DIR)
	@echo "========================================"
	@echo "Rendering: $*"
	@echo "========================================"
	cd $(UNITY_DIR) && ./render.sh $* --resolution $(RESOLUTION) --output $(OUTPUT_DIR)

# =============================================================================
# Cleaning
# =============================================================================

clean: clean-videos clean-fbx
	@echo "Cleaned all generated files"

clean-videos:
	rm -f $(OUTPUT_DIR)/*.webm
	@echo "Removed video files"

clean-fbx:
	rm -f $(MODELS_DIR)/*.fbx
	rm -f $(BLENDER_DIR)/output/*.fbx
	@echo "Removed FBX files"

# =============================================================================
# Development helpers
# =============================================================================

# Check dependencies
check-deps:
	@echo "Checking dependencies..."
	@command -v $(BLENDER) >/dev/null 2>&1 && echo "✓ Blender: $$($(BLENDER) --version | head -1)" || echo "✗ Blender not found"
	@[ -f "$(UNITY)" ] && echo "✓ Unity: $(UNITY)" || echo "✗ Unity not found at $(UNITY)"
	@command -v ffprobe >/dev/null 2>&1 && echo "✓ ffprobe: available" || echo "✗ ffprobe not found (optional)"

# Watch for changes and rebuild (requires entr)
watch:
	@command -v entr >/dev/null 2>&1 || { echo "Install entr: sudo apt install entr"; exit 1; }
	find $(BLENDER_DIR) -name "*.py" | entr -c make fbx

# List all generated files
list:
	@echo "FBX files:"
	@ls -la $(MODELS_DIR)/*.fbx 2>/dev/null || echo "  (none)"
	@echo ""
	@echo "Video files:"
	@ls -la $(OUTPUT_DIR)/*.webm 2>/dev/null || echo "  (none)"
