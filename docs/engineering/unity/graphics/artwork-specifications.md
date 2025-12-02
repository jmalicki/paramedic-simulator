# Artwork Specifications — Paramedic Simulator

## Overview

This document specifies the artwork pipeline for the Paramedic Simulator. The game uses a **cel-based 2D presentation** with **3D source assets** that are rendered to 2D sprites. This approach provides flexibility for re-rendering assets at different angles, poses, and animation states.

---

## Art Style

### Visual Direction
- **Style**: Cartoonish but recognizable medical equipment and characters
- **Rendering**: Cel-shaded with bold outlines (2-4px black/dark outlines)
- **Colors**: Flat or limited gradient fills, medical-appropriate color palette
- **Tone**: Approachable and clear, prioritizing readability over realism
- **Inspiration**: Games like Trauma Center, Two Point Hospital, or classic point-and-click adventures

### Color Palette

#### Primary Colors
| Use | Hex | Description |
|-----|-----|-------------|
| Medical Blue | `#2E86AB` | Equipment, uniforms |
| Clinical White | `#F5F5F5` | Sheets, gauze, clean items |
| Emergency Red | `#E63946` | Alerts, blood, urgency indicators |
| Safety Orange | `#F4A261` | Warning elements, stretcher accents |
| Skin Base Light | `#FFDAB9` | Character skin (light) |
| Skin Base Medium | `#D2956D` | Character skin (medium) |
| Skin Base Dark | `#8D5524` | Character skin (dark) |

#### Medical Equipment Colors
| Equipment | Primary | Secondary | Accent |
|-----------|---------|-----------|--------|
| Stethoscope | `#2D2D2D` (black) | `#C0C0C0` (silver) | `#4A90D9` (tubing) |
| Monitor | `#3D3D3D` (housing) | `#1A1A2E` (screen) | `#00FF00` (vitals) |
| IV Bag | `#E8F4F8` (clear) | `#87CEEB` (fluid) | `#FFFFFF` (label) |
| Defibrillator | `#FF6B35` (orange) | `#2D2D2D` (black) | `#00FF00` (ready) |
| O2 Tank | `#228B22` (green) | `#C0C0C0` (valve) | `#FFFFFF` (label) |
| Stretcher | `#FF8C00` (frame) | `#FFFFFF` (pad) | `#2D2D2D` (wheels) |

---

## Asset Pipeline

### Workflow: 3D → 2D Sprites

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│  3D Modeling    │ ──▶ │  Cel-Shade      │ ──▶ │  2D Sprite      │
│  (Blender)      │     │  Render         │     │  Export (PNG)   │
└─────────────────┘     └─────────────────┘     └─────────────────┘
        │                       │                       │
   Low-poly models        Toon shader +           Final sprites
   with clean topology    outline render          for Unity
```

### Source Files (3D)
- **Format**: `.blend` (Blender) as primary DCC
- **Location**: `Art/Source/3D/`
- **Naming**: `[Category]_[AssetName].blend`
  - Example: `Prop_Stethoscope.blend`, `Char_Patient.blend`

### Rendered Sprites (2D)
- **Format**: PNG with transparency
- **Location**: `Art/Sprites/[Category]/`
- **Naming**: `Spr_[AssetName]_[Angle]_[Frame].png`
  - Example: `Spr_Stethoscope_Front_01.png`

### Render Settings (Blender)
- **Engine**: EEVEE or Cycles with Freestyle
- **Outline**: Freestyle lines, 2-4px, black or dark variant of base color
- **Shading**: Toon shader with 2-3 color bands
- **Background**: Transparent (RGBA)
- **Resolution**: 2x target size, downscale for anti-aliasing

---

## Asset Specifications

### Vehicles

#### Ambulance
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_Ambulance_Exterior` | 3-5k tris | 512x256 | Side, Rear, 3/4 |
| `Prop_Ambulance_Interior` | 5-8k tris | 1024x512 | Front-facing (background) |
| `Prop_Ambulance_Doors` | 1k tris | 256x256 | Open, Closed |

**Design Notes**:
- Recognizable Type II/III ambulance shape
- "AMBULANCE" text and star of life emblem
- Interior shows bench, cabinets, stretcher mount point

---

### Medical Equipment — Hero Props

These are primary interaction items, rendered at higher detail.

#### Stethoscope
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_Stethoscope` | 2-3k tris | 128x128 | Flat, Hanging, In-use |

**Design Notes**:
- Classic dual-tube design with diaphragm and bell
- Earpieces clearly visible
- Tubing shows slight curve/drape

#### BVM (Bag Valve Mask)
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_BVM` | 2-3k tris | 128x128 | Idle, Squeezed (2 frames) |

**Design Notes**:
- Blue/clear bag, clear mask
- Visible valve mechanism
- Animation: bag compression for ventilation

#### Cardiac Monitor / Defibrillator
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_Monitor_Defib` | 4-6k tris | 192x192 | Front, With-pads |
| `Prop_Defib_Pads` | 500 tris | 96x64 | Pair, On-patient |

**Design Notes**:
- Combined monitor/defib unit (like Zoll or Philips)
- Screen shows ECG waveform, vitals readout
- Defibrillator pads with cables
- Screen states: Off, Normal sinus, V-fib, Asystole, Charging

#### Laryngoscope
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_Laryngoscope` | 1-2k tris | 96x64 | Closed, Open/lit |

**Design Notes**:
- Metal handle with curved blade
- Light at blade tip (emissive when open)

---

### IV / Line Equipment

#### IV Bag & Line
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_IV_Bag` | 1k tris | 64x128 | Full, Half, Empty |
| `Prop_IV_Line` | 500 tris | 32x128 | Straight, Curved |
| `Prop_IV_Pole` | 800 tris | 48x192 | With bag, Empty |

**Design Notes**:
- Clear bag with visible fluid level
- Drip chamber visible in line
- Roller clamp on tubing

#### IV Start Kit / Catheter
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_IV_Catheter` | 500 tris | 64x32 | Packaged, Ready |
| `Prop_IV_StartKit` | 1.5k tris | 128x96 | Open kit view |

**Design Notes**:
- Catheter with flash chamber
- Kit includes: catheter, tegaderm, alcohol wipes, tape

#### IO Drill
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_IO_Drill` | 1.5k tris | 96x96 | Idle, With needle |

**Design Notes**:
- Recognizable EZ-IO style drill
- Needle attachment visible

---

### Airway & Oxygen Equipment

#### Oxygen Mask
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_O2_Mask` | 1k tris | 96x96 | Flat, On-face |
| `Prop_O2_NonRebreather` | 1.2k tris | 96x96 | With reservoir bag |

#### Nasal Cannula
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_O2_NasalCannula` | 500 tris | 96x48 | Flat, On-face |

#### Oxygen Tank
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_O2_Tank` | 1k tris | 64x128 | Upright, Laying |

**Design Notes**:
- Green cylinder (US standard)
- Regulator and flowmeter visible
- Gauge showing pressure

#### OPA / NPA Airways
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_OPA` | 300 tris | 64x32 | Multiple sizes (color-coded) |
| `Prop_NPA` | 300 tris | 64x32 | With lubricant |

---

### Monitoring Equipment

#### BP Cuff
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_BP_Cuff` | 1k tris | 96x64 | Flat, On-arm |
| `Prop_BP_Bulb` | 500 tris | 48x64 | Squeezed, Relaxed |

#### Pulse Oximeter
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_PulseOx` | 500 tris | 48x48 | Open, On-finger |

**Design Notes**:
- Finger clip style
- Small LED display showing SpO2 and pulse

#### ECG Leads
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_ECG_Leads` | 800 tris | 128x64 | 3-lead set, 12-lead set |
| `Prop_ECG_Electrode` | 100 tris | 32x32 | Single pad |

---

### Large Equipment

#### Stretcher / Gurney
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_Stretcher` | 4-6k tris | 256x128 | Flat, Raised-head, Folded |

**Design Notes**:
- Orange/yellow frame (high visibility)
- White mattress pad
- Visible wheels and rails
- Restraint straps

#### Backboard
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_Backboard` | 500 tris | 192x64 | With straps, With head blocks |

#### Jump Bag
| Asset | 3D Poly Budget | Sprite Size | Views/Frames |
|-------|----------------|-------------|--------------|
| `Prop_JumpBag` | 2k tris | 128x96 | Closed, Open (contents visible) |

**Design Notes**:
- Red or orange medical bag
- Multiple compartments when open
- Cross or star of life emblem

---

### Small Props (Inventory Items)

These are lower-detail items, often shown in inventory or during specific actions.

| Asset | 3D Poly Budget | Sprite Size | Frames |
|-------|----------------|-------------|--------|
| `Prop_Syringe_3ml` | 300 tris | 64x24 | Empty, Full |
| `Prop_Syringe_10ml` | 300 tris | 80x24 | Empty, Full |
| `Prop_MedVial` | 200 tris | 32x48 | Generic, Labeled variants |
| `Prop_MedAmpule` | 150 tris | 24x40 | Intact, Broken |
| `Prop_Epi_Auto` | 400 tris | 48x64 | Capped, Deployed |
| `Prop_Tape_Roll` | 200 tris | 48x48 | Full, Partial |
| `Prop_Gloves_Box` | 300 tris | 64x48 | Closed, Open |
| `Prop_Gloves_Pair` | 200 tris | 48x32 | Flat |
| `Prop_Gauze_Pack` | 150 tris | 48x32 | Sealed, Open |
| `Prop_Bandage_Roll` | 200 tris | 40x40 | Rolled |
| `Prop_Scissors_Trauma` | 400 tris | 64x32 | Open, Closed |
| `Prop_Penlight` | 200 tris | 48x16 | Off, On |
| `Prop_Thermometer` | 200 tris | 56x16 | - |
| `Prop_Glucometer` | 400 tris | 48x64 | With strip |
| `Prop_Tourniquet` | 300 tris | 64x32 | Flat, Applied |
| `Prop_CCollar` | 600 tris | 96x48 | Flat, Applied |

---

### Characters

Characters are rigged 3D models rendered to sprite sheets for various poses and states.

#### Patient
| Asset | 3D Poly Budget | Sprite Size | Poses/States |
|-------|----------------|-------------|--------------|
| `Char_Patient_Male` | 5-8k tris | 256x192 | See below |
| `Char_Patient_Female` | 5-8k tris | 256x192 | See below |
| `Char_Patient_Child` | 4-6k tris | 192x128 | See below |
| `Char_Patient_Elderly` | 5-8k tris | 256x192 | See below |

**Patient Poses**:
- Lying supine (default)
- Lying with head elevated
- Sitting up
- Recovery position
- Standing (ambulatory patient)

**Patient States** (texture/shader variants):
- Normal skin tone
- Pale (shock, blood loss)
- Cyanotic (blue tint - hypoxia)
- Flushed (fever, anaphylaxis)
- Diaphoretic (sweat overlay)

**Clothing Variants**:
- Casual clothes
- Nightwear/pajamas
- Work clothes
- Hospital gown

#### Paramedic / EMT
| Asset | 3D Poly Budget | Sprite Size | Poses |
|-------|----------------|-------------|-------|
| `Char_Paramedic_Male` | 4-6k tris | 128x256 | See below |
| `Char_Paramedic_Female` | 4-6k tris | 128x256 | See below |

**Paramedic Poses**:
- Standing idle
- Kneeling at patient
- Performing CPR (2-frame loop)
- Carrying equipment
- Using radio

**Uniform**:
- Navy/dark blue EMS uniform
- Reflective strips
- Badge and patches
- Gloves (optional overlay)

#### Bystander / NPC
| Asset | 3D Poly Budget | Sprite Size | Poses |
|-------|----------------|-------------|-------|
| `Char_Bystander_A` | 3-4k tris | 96x192 | Standing, Pointing |
| `Char_Bystander_B` | 3-4k tris | 96x192 | Standing, Worried |
| `Char_Bystander_C` | 3-4k tris | 96x192 | Standing, On phone |

#### Player Hands (First-Person Actions)
| Asset | 3D Poly Budget | Sprite Size | Poses |
|-------|----------------|-------------|-------|
| `Char_Hands_Gloved` | 2k tris | 128x96 | Open, Gripping, Pointing, Action-specific |

---

### Backgrounds / Environments

Backgrounds are larger scene renders, potentially layered for parallax.

| Asset | 3D Poly Budget | Sprite Size | Variants |
|-------|----------------|-------------|----------|
| `BG_Ambulance_Interior` | 15-20k tris | 1920x1080 | Day, Night |
| `BG_Street_Residential` | 10-15k tris | 1920x1080 | Day, Night, Rain |
| `BG_Street_Urban` | 10-15k tris | 1920x1080 | Day, Night |
| `BG_Apartment_LivingRoom` | 8-12k tris | 1920x1080 | Clean, Messy |
| `BG_Apartment_Bedroom` | 8-12k tris | 1920x1080 | - |
| `BG_ER_Bay` | 12-18k tris | 1920x1080 | Empty, Staffed |

---

### UI Elements

UI icons rendered from 3D at small sizes with extra-bold outlines.

| Asset | Source | Size | Notes |
|-------|--------|------|-------|
| `UI_Icon_Stethoscope` | From 3D | 64x64 | Inventory icon |
| `UI_Icon_BVM` | From 3D | 64x64 | |
| `UI_Icon_IV` | From 3D | 64x64 | |
| `UI_Icon_Monitor` | From 3D | 64x64 | |
| `UI_Icon_O2` | From 3D | 64x64 | |
| `UI_Icon_Defib` | From 3D | 64x64 | |
| `UI_Icon_Meds` | From 3D | 64x64 | |
| `UI_Icon_Airway` | From 3D | 64x64 | |
| `UI_Icon_Assessment` | 2D | 64x64 | Clipboard/checklist |
| `UI_Icon_Radio` | From 3D | 64x64 | |

---

## Animation Guidelines

### Limited Animation Approach
- **Frame count**: 2-4 frames per animation (not fluid, stylized)
- **Timing**: Hold frames longer for snappy, cartoon feel
- **Loops**: Breathing (2 frames), Equipment use (2-3 frames)

### Key Ambient Animations

| Animation | Frames | Loop |
|-----------|--------|------|
| Patient breathing (normal) | 2 | Yes |
| Patient breathing (labored) | 3 | Yes |
| Patient unconscious/still | 1 | No |
| BVM squeeze | 2 | No |
| CPR compression | 2 | Yes |
| Monitor beep pulse | 2 | Yes |
| Defibrillator charge | 3 | No |
| IV drip | 2 | Yes |

---

## Procedural Animation Sequences

Medical interventions require multi-step animated sequences showing the procedure being performed. These are more complex than ambient animations and show the player's hands interacting with equipment and patient.

### Animation Sequence Format

Each procedural sequence is rendered as a **sprite sheet** with accompanying **JSON metadata** defining:
- Frame dimensions and count
- Timing per frame (ms)
- Key event triggers (sound cues, state changes)
- Composite layer information

### Directory Structure for Sequences

```
Art/Sprites/Sequences/
├── IV_Start/
│   ├── Seq_IV_Start_Sheet.png       # Sprite sheet (all frames)
│   ├── Seq_IV_Start.json            # Animation metadata
│   └── Seq_IV_Start_Preview.gif     # Preview (for reference)
├── Monitor_Hookup/
├── Defib_Shock/
├── BVM_Ventilation/
└── ...
```

### Sequence Metadata Format (JSON)

```json
{
  "name": "IV_Start",
  "description": "Starting an IV line on patient's arm",
  "spriteSheet": "Seq_IV_Start_Sheet.png",
  "frameWidth": 320,
  "frameHeight": 240,
  "framesPerRow": 4,
  "totalFrames": 12,
  "defaultFrameMs": 500,
  "frames": [
    { "index": 0, "duration": 600, "event": null, "label": "select_site" },
    { "index": 1, "duration": 400, "event": "sfx_alcohol_wipe", "label": "clean_site" },
    { "index": 2, "duration": 400, "event": null, "label": "clean_site" },
    { "index": 3, "duration": 500, "event": "sfx_tourniquet", "label": "apply_tourniquet" },
    { "index": 4, "duration": 300, "event": null, "label": "prep_catheter" },
    { "index": 5, "duration": 600, "event": null, "label": "insert_needle" },
    { "index": 6, "duration": 400, "event": "sfx_flash", "label": "flash_confirm" },
    { "index": 7, "duration": 400, "event": null, "label": "advance_catheter" },
    { "index": 8, "duration": 300, "event": null, "label": "remove_needle" },
    { "index": 9, "duration": 400, "event": "sfx_click", "label": "connect_tubing" },
    { "index": 10, "duration": 500, "event": null, "label": "secure_site" },
    { "index": 11, "duration": 400, "event": "sfx_success", "label": "complete" }
  ],
  "layers": [
    { "name": "patient_arm", "zIndex": 0 },
    { "name": "equipment", "zIndex": 1 },
    { "name": "hands", "zIndex": 2 }
  ],
  "outcomes": {
    "success": { "gotoFrame": 11, "nextState": "iv_running" },
    "failure": { "gotoFrame": 6, "nextState": "iv_failed", "event": "sfx_miss" }
  }
}
```

### Required Procedural Sequences

#### Assessment & Monitoring

| Sequence | Frames | Duration | Description |
|----------|--------|----------|-------------|
| `Seq_PulseOx_Apply` | 4 | 2s | Clipping pulse ox onto finger |
| `Seq_BP_Cuff_Apply` | 6 | 3s | Wrapping cuff, positioning, inflating |
| `Seq_Stethoscope_Listen` | 5 | 3s | Placing stethoscope, listening positions |
| `Seq_Monitor_Hookup` | 10 | 5s | Applying ECG leads, connecting cables |
| `Seq_Pupils_Check` | 4 | 2s | Penlight check, pupil response |

#### Airway Management

| Sequence | Frames | Duration | Description |
|----------|--------|----------|-------------|
| `Seq_OPA_Insert` | 6 | 3s | Measuring, inserting oral airway |
| `Seq_NPA_Insert` | 6 | 3s | Measuring, lubricating, inserting nasal airway |
| `Seq_BVM_Setup` | 5 | 2.5s | Connecting mask, oxygen, positioning |
| `Seq_BVM_Ventilate` | 4 | 2s | Head tilt, mask seal, squeeze (loopable) |
| `Seq_Suction_Oral` | 6 | 3s | Suctioning airway |
| `Seq_O2_Mask_Apply` | 4 | 2s | Placing mask, adjusting strap, setting flow |
| `Seq_Intubation` | 12 | 8s | Laryngoscopy and tube placement (advanced) |

#### Circulation & IV Access

| Sequence | Frames | Duration | Description |
|----------|--------|----------|-------------|
| `Seq_IV_Start` | 12 | 6s | Full IV start procedure |
| `Seq_IV_Flush` | 4 | 2s | Flushing line, checking patency |
| `Seq_IV_Med_Push` | 6 | 3s | Drawing med, pushing through line |
| `Seq_IO_Insert` | 8 | 4s | IO drill placement |
| `Seq_Tourniquet_Apply` | 4 | 2s | Applying tourniquet for bleeding |
| `Seq_Bleeding_Control` | 6 | 3s | Direct pressure, packing wound |

#### Cardiac Care

| Sequence | Frames | Duration | Description |
|----------|--------|----------|-------------|
| `Seq_Defib_Pads_Apply` | 6 | 3s | Placing defibrillator pads |
| `Seq_Defib_Analyze` | 4 | 3s | Analyzing rhythm (monitor screen focus) |
| `Seq_Defib_Shock` | 5 | 2s | Charging, clearing, delivering shock |
| `Seq_CPR_Cycle` | 6 | 4s | Compression cycle (30 compressions) |
| `Seq_CPR_Position` | 4 | 2s | Hand positioning for compressions |

#### Medications

| Sequence | Frames | Duration | Description |
|----------|--------|----------|-------------|
| `Seq_Med_Draw_Vial` | 6 | 3s | Drawing medication from vial |
| `Seq_Med_Draw_Ampule` | 6 | 3s | Breaking ampule, drawing medication |
| `Seq_Epi_Auto_Inject` | 5 | 2.5s | EpiPen administration |
| `Seq_Narcan_IN` | 4 | 2s | Intranasal Narcan administration |
| `Seq_Nebulizer_Setup` | 5 | 2.5s | Setting up nebulizer treatment |

#### Trauma & Immobilization

| Sequence | Frames | Duration | Description |
|----------|--------|----------|-------------|
| `Seq_CCollar_Apply` | 8 | 4s | Sizing and applying cervical collar |
| `Seq_Splint_Apply` | 8 | 4s | Applying splint to extremity |
| `Seq_Backboard_Log` | 10 | 6s | Log roll onto backboard |
| `Seq_Bandage_Wrap` | 6 | 3s | Wrapping bandage around wound |

### Sequence Sprite Sheet Layout

```
┌────────────────────────────────────────────────────────┐
│  Frame 0  │  Frame 1  │  Frame 2  │  Frame 3  │        │  Row 1
├───────────┼───────────┼───────────┼───────────┤        │
│  Frame 4  │  Frame 5  │  Frame 6  │  Frame 7  │        │  Row 2
├───────────┼───────────┼───────────┼───────────┤        │
│  Frame 8  │  Frame 9  │  Frame 10 │  Frame 11 │        │  Row 3
└───────────┴───────────┴───────────┴───────────┴────────┘
```

**Standard Sequence Sizes:**
- Small (hands + small equipment): 256x192 per frame
- Medium (patient interaction): 320x240 per frame
- Large (full body procedures): 480x320 per frame

### Compositing Approach

For flexibility, some sequences may use **layered compositing** instead of flat sprite sheets:

```
Layer Stack (bottom to top):
├── Background (patient body part or surface)
├── Equipment Layer (IV catheter, leads, etc.)
├── Hands Layer (gloved hands performing action)
└── Overlay Effects (blood flash, success indicator)
```

This allows:
- Swapping patient skin tones without re-rendering
- Reusing hand animations across similar procedures
- Adding equipment variants (different IV catheter brands)

### Animation Rendering Workflow (Blender)

1. **Scene Setup**: Position camera for consistent framing across sequence
2. **Keyframe Actions**: Animate hands and equipment through procedure steps
3. **Render Frames**: Output individual frames with transparency
4. **Assemble Sheet**: Combine frames into sprite sheet (use TexturePacker or similar)
5. **Generate Metadata**: Create JSON with timing and events
6. **Export**: PNG sprite sheet + JSON to `Art/Sprites/Sequences/[Name]/`

### Sequence Priority (Implementation Order)

**Phase 1 — Core Assessment:**
1. `Seq_PulseOx_Apply`
2. `Seq_BP_Cuff_Apply`
3. `Seq_Stethoscope_Listen`
4. `Seq_Monitor_Hookup`

**Phase 2 — Airway & Breathing:**
1. `Seq_O2_Mask_Apply`
2. `Seq_BVM_Setup`
3. `Seq_BVM_Ventilate`
4. `Seq_OPA_Insert`

**Phase 3 — Circulation:**
1. `Seq_IV_Start`
2. `Seq_IV_Med_Push`
3. `Seq_Defib_Pads_Apply`
4. `Seq_Defib_Shock`
5. `Seq_CPR_Cycle`

**Phase 4 — Advanced & Polish:**
1. All remaining sequences
2. Failure state variants
3. Speed/skill level variants

---

## Rendering Specifications

### Blender Cel-Shade Setup

```
Material Setup:
├── Shader to RGB node (captures lighting)
├── Color Ramp (2-3 stops for toon bands)
│   ├── Shadow color (darker variant)
│   ├── Mid tone (base color)
│   └── Highlight (lighter variant)
└── Mix with base texture

Freestyle Settings:
├── Line Thickness: 2-4px (scale with sprite size)
├── Color: Black or dark variant of object color
├── Crease Angle: 120-140°
└── Silhouette: Enabled
```

### Export Checklist
- [ ] Transparent background (RGBA PNG)
- [ ] Render at 2x target resolution
- [ ] Apply slight Gaussian blur before downscale (anti-aliasing)
- [ ] Verify outline integrity at final size
- [ ] Check silhouette readability at 50% zoom

---

## Directory Structure

```
Art/
├── Source/
│   └── 3D/
│       ├── Props/
│       │   ├── Medical/
│       │   ├── Vehicles/
│       │   └── Environment/
│       ├── Characters/
│       │   ├── Patient/
│       │   ├── Paramedic/
│       │   └── NPC/
│       └── Backgrounds/
├── Sprites/
│   ├── Props/
│   ├── Characters/
│   ├── Backgrounds/
│   └── UI/
├── Textures/
│   └── Shared/
└── References/
    ├── MedicalEquipment/
    └── Uniforms/
```

---

## Asset Priority (Implementation Order)

### Phase 1 — Core Playable
1. `Char_Patient_Male` (lying, basic states)
2. `Prop_Stethoscope`
3. `Prop_BP_Cuff`
4. `Prop_PulseOx`
5. `Prop_Monitor_Defib`
6. `BG_Ambulance_Interior`
7. UI Icons for above

### Phase 2 — Airway & Circulation
1. `Prop_BVM`
2. `Prop_O2_Mask`
3. `Prop_O2_Tank`
4. `Prop_IV_Bag`, `Prop_IV_Line`, `Prop_IV_Catheter`
5. `Char_Hands_Gloved`

### Phase 3 — Full Scenario Support
1. `Prop_Stretcher`
2. `Prop_JumpBag`
3. `Char_Paramedic_Male/Female`
4. `BG_Street_Residential`
5. `BG_Apartment_LivingRoom`
6. Additional patient variants

### Phase 4 — Polish & Variety
1. All remaining props
2. Bystander characters
3. Additional backgrounds
4. Animation refinements
5. State variants (cyanosis, pallor, etc.)

---

## Technical Notes

### Unity Import Settings
- **Sprites**:
  - Texture Type: Sprite (2D and UI)
  - Filter Mode: Bilinear
  - Compression: High Quality (or uncompressed for important assets)
  - Pixels Per Unit: 100 (adjust based on game scale)

### Sprite Atlasing
- Group sprites by category for efficient atlasing
- Keep frequently-used sprites in same atlas
- Maximum atlas size: 2048x2048

---

## Revision History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | 2024-XX-XX | Initial 2D cel-based specification |
