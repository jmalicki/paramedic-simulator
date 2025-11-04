---
title: Paramedic Simulator: Comprehensive Design Document
author: Project Team
date: 2025-11-04
---

# Paramedic Simulator: Comprehensive Design Document

[Download this document as Markdown](#) · [Convert to PDF](#)

---

## Table of Contents

1. [Introduction](#1-introduction)
2. [Project Overview](#2-project-overview)
3. [Gameplay Overview](#3-gameplay-overview)
4. [Medically Realistic Scenarios](#4-medically-realistic-scenarios)
5. [Generative Patients](#5-generative-patients)
6. [Statistical Models](#6-statistical-models)
7. [Bayesian Networks](#7-bayesian-networks)
8. [PyTorch Implementation](#8-pytorch-implementation)
9. [Game Development](#9-game-development)
10. [User Interface](#10-user-interface)
11. [User Experience](#11-user-experience)
12. [Testing and Quality Assurance](#12-testing-and-quality-assurance)
13. [Deployment and Maintenance](#13-deployment-and-maintenance)
14. [Austin-Travis County Protocol Integration](#14-austin-travis-county-protocol-integration)
15. [Research Plan for Statistical Models](#15-research-plan-for-statistical-models)
16. [Conclusion](#16-conclusion)

---

## 1. Introduction

### 1.1 Project Vision

The Paramedic Simulator is an advanced educational tool designed to bridge the gap between classroom learning and real-world emergency medical response. This simulation-based training platform provides paramedic students with a medically accurate, immersive environment to practice critical decision-making, patient assessment, and treatment protocols in pre-hospital care scenarios. Unlike existing training tools that often rely on static case studies or limited mannequin-based simulations, this platform leverages cutting-edge statistical modeling and machine learning to generate dynamic, responsive patient scenarios that evolve based on user interventions.

### 1.2 Problem Statement

Current paramedic training faces significant challenges:

- Limited exposure to rare but critical cases during clinical rotations
- Inconsistent quality and variety of simulation training
- Difficulty in providing immediate, specific feedback on clinical decisions
- High costs associated with maintaining physical simulation labs
- Inability to safely expose students to high-risk scenarios

According to a 2022 study published in Prehospital Emergency Care, paramedic students require approximately 30–50 patient encounters to achieve competency in basic assessment skills, but real-world field experiences often fail to provide this breadth of exposure to diverse medical conditions (Smith et al., 2022).

### 1.3 Solution Overview

The Paramedic Simulator addresses these challenges through:

- A generative patient system using Bayesian networks to create medically accurate, dynamic patient scenarios
- Real-time physiological modeling that responds authentically to user interventions
- Comprehensive feedback system aligned with current evidence-based guidelines
- Scalable architecture supporting diverse deployment environments (desktop, VR, classroom)
- Integration with existing paramedic curriculum standards

### 1.4 Target Audience

- Primary: Paramedic students (EMT-P level) in accredited training programs
- Secondary: Current paramedics seeking continuing education
- Tertiary: EMS instructors for classroom demonstration and assessment
- Quaternary: Medical students and residents in emergency medicine

---

## 2. Project Overview

### 2.1 Core Objectives

1. **Educational Efficacy**: Improve student competency in patient assessment and treatment by 25% compared to traditional training methods
2. **Medical Accuracy**: Ensure all scenarios and responses align with current evidence-based guidelines (ACLS, PHTLS, AMLS)
3. **Scenario Diversity**: Generate >500 unique patient presentations covering the full spectrum of pre-hospital emergencies
4. **Adaptive Learning**: Provide personalized feedback and difficulty adjustment based on user performance
5. **Accessibility**: Support deployment across multiple platforms with minimal hardware requirements

### 2.2 Key Performance Indicators

- Student knowledge retention (measured through pre/post-simulation testing)
- Clinical decision accuracy (compared to expert paramedic benchmarks)
- User engagement metrics (session duration, scenario completion rates)
- Transfer of learning to real-world performance (tracked through field evaluations)
- Instructor satisfaction with integration into existing curriculum

### 2.3 Scope Definition

#### In Scope

- Full implementation of medical algorithms based on ACLS 2020, PALS 2023, PHTLS 10th Edition, and AMLS 3rd Edition
- Dynamic physiological modeling of 15+ critical conditions
- Generation of patient demographics, histories, and presentations reflecting real-world epidemiology
- Real-time vital sign monitoring and response to interventions
- Comprehensive assessment and feedback system
- Integration with learning management systems (LMS)

#### Out of Scope

- Certification or official testing functionality
- Replacement for hands-on clinical skills training
- Direct patient care or diagnostic recommendations in real emergencies
- Mobile app development for initial release (desktop first)

### 2.4 Regulatory and Compliance Considerations

- Adherence to medical simulation standards set by the Society for Simulation in Healthcare
- Compliance with HIPAA if integrating any real patient data (not planned for v1)
- Alignment with NAEMT curriculum standards
- Accessibility compliance (WCAG 2.1) for users with disabilities
- FERPA compliance for student performance data

---

## 3. Gameplay Overview

### 3.1 Core Gameplay Loop

1. Call Dispatch → 2. Scene Assessment → 3. Primary Survey → 4. History Taking → 5. Intervention Phase → 6. Reassessment → 7. Transport Decision → 8. Debriefing

### 3.2 Scenario Types

#### Medical Emergencies (60%)

- Cardiac: STEMI, arrhythmias, cardiac arrest, acute decompensated heart failure (CHF)
- Respiratory: Asthma, COPD, pulmonary edema, tension pneumothorax
- Neurological: Stroke, seizure, altered mental status
- Endocrine: Diabetic emergencies, adrenal crisis
- Toxicological: Opioid overdose (naloxone), other overdoses/poisoning
- Environmental: Hypothermia, heat stroke

#### Trauma Emergencies (30%)

- Blunt trauma: MVC, falls, assaults
- Penetrating trauma: Gunshot wounds, stabbings
- Orthopedic: Fractures, dislocations
- Head/Spine: TBI, spinal injury
- Multisystem trauma

#### Special Populations (10%)

- Pediatric: Respiratory distress, febrile seizures, dehydration
- Geriatric: Falls, polypharmacy issues, atypical presentations
- OB/GYN: Normal delivery, postpartum hemorrhage, shoulder dystocia, neonatal resuscitation, ectopic pregnancy
- Psychiatric: Acute psychosis, suicidal ideation

### 3.3 Progression System

- Skill Tiers: Novice → Intermediate → Advanced
- Competency Badges by domain (airway, cardiac, trauma, pediatric)
- Case complexity scales with performance
- Analytics on timing, accuracy, protocol adherence
- Remediation paths for skill gaps

### 3.4 Realism Features

- Dynamic physiological modeling responsive to interventions
- Real-time clock and urgency based on condition
- Resource constraints reflecting field realities
- Environmental factors (weather, lighting, hazards)
- Patient variability (age, weight, comorbidities)

---

## 4. Medically Realistic Scenarios

### 4.1 Medical Foundation

All scenarios are grounded in current evidence-based guidelines:

#### Cardiac Scenarios

- AHA ACLS 2020 Guidelines
- STEMI care per ACC/AHA guidance
- Arrhythmia algorithms per AHA
- Cardiac arrest with objective CPR feedback

#### Trauma Scenarios

- PHTLS 10th Edition
- TCCC hemorrhage control principles
- ATLS adapted survey approach
- MOI analysis using trauma registry patterns

#### Medical Scenarios

- AMLS 3rd Edition assessment framework
- PALS 2023 pediatric protocols
- CDC infectious disease guidelines
- Evidence-based treatment from peer-reviewed literature

### 4.2 Patient Presentation Modeling

Vital signs evolve via differential-equation-inspired dynamics:

```
BP(t) = BP_base + f(cardiac_output, SVR, blood_volume) + noise
cardiac_output = heart_rate × stroke_volume
SVR = systemic vascular resistance
```

```
SpO2 = f(hemoglobin, PaO2, V/Q ratio, shunt fraction)
```

Condition-specific parameters are applied (e.g., asthma, hemorrhagic shock, CHF, tension pneumothorax, opioid overdose) with realistic ranges and responses.

Example condition parameterizations:

**Acute Decompensated Heart Failure (CHF/Pulmonary Edema)**

- Preload: elevated; Afterload: elevated
- Lung water: increasing over time without intervention
- Response to CPAP: ↑SpO2, ↓work of breathing within minutes
- Response to nitrates: ↓SVR/afterload → improved CO and BP (if hypertensive)

**Tension Pneumothorax**

- Intrathoracic pressure: rising → ↓venous return → hypotension
- Breath sounds: unilateral decrease; tracheal deviation (late)
- Needle decompression: immediate improvement in BP and SpO2; risk of recurrence

**Opioid Overdose**

- Respiratory drive: depressed (RR ↓, EtCO2 ↑, SpO2 ↓)
- Pinpoint pupils common
- Naloxone: rapid reversal (RR/mental status improve); watch for re-sedation and acute withdrawal

### 4.3 Treatment Response Modeling

- Each medication includes onset, peak, duration, dose-response, and adverse profile
- Non-pharmacologic interventions (CPR, BVM, tourniquet) have modeled effects and constraints

### 4.4 Complication Modeling

Complications depend on elapsed time, intervention correctness, patient risk factors, and calibrated randomness.

---

## 5. Generative Patients

### 5.1 Patient Generation Framework

Components: Demographic Generator, Medical History, Presentation, Condition, and Response Profile generators, informed by NEMSIS/NHANES/CDC/AHRQ and literature.

### 5.2 Demographic Generation

Age, gender, and socioeconomic distributions follow EMS epidemiology with tunable regional priors.

### 5.3 Medical History Generation

Conditional probabilities produce comorbidities, medication regimens, and allergies aligned with prevalence data.

### 5.4 Presentation Generation

Chief complaint frequencies and severities reflect NEMSIS-derived patterns with tunable severity priors.

### 5.5 Patient Response Variability

Patient-specific parameters (e.g., baseline HR/SBP, pain tolerance, metabolism) drive heterogeneous responses.

---

## 6. Statistical Models

### 6.1 Overall Architecture

Three-layer system: Patient Generator, Disease Modeler, Response Simulator, coupled via a dynamic Bayesian network (DBN).

### 6.2 Bayesian Network Structure

Hierarchical nodes: Patient → Disease → Presentation (observables). CPTs encode conditional relationships.

Example subgraph (cardiac): demographics → comorbidities → CAD/rupture → STEMI → ST-elevation → hemodynamics.

### 6.3 Dynamic State Modeling

```
State(t) = [HR, SBP, DBP, RR, SpO2, EtCO2, GCS, Pain, Glucose, ...]
State(t+Δt) = f(State(t), Interventions, Time, Patient Factors) + Noise
```

Example hypovolemia dynamics:

```
d(BP)/dt = k1*HR - k2*BloodVolume - k3*SVR
d(HR)/dt = k4*(NormalBP - BP) + k5*Pain
d(BloodVolume)/dt = -BleedRate + k6*FluidAdministered
```

Additional examples:

```
# Acute decompensated heart failure (simplified)
d(AlveolarFluid)/dt = LeakRate(PCWP) - Clearance(CPAP, diuretics)
d(SpO2)/dt = g(Ventilation, AlveolarFluid, FiO2) - h(Shunt)

# Tension pneumothorax (simplified)
d(IntrathoracicPressure)/dt = AirLeakRate - DecompressionEffect
d(Preload)/dt = -m*IntrathoracicPressure
d(SBP)/dt = p*CO*SVR - q*Preload

# Opioid overdose (simplified)
d(RespDrive)/dt = -r*OpioidEffect + s*NaloxoneEffect
d(RR)/dt = u*RespDrive - v*Sedation
d(EtCO2)/dt = w*(CO2Production - AlveolarVentilation)
```

### 6.4 Treatment Response Modeling

PK/PD for drugs and procedural success as functions of skill, patient, and environment.

---

## 7. Bayesian Networks

### 7.1 Network Architecture

Three tiers with ~250 nodes total, average 3–5 edges/node, updating at up to 10 Hz.

### 7.2 Parameter Estimation

Combines NEMSIS data, literature, and expert elicitation using MLE, Bayesian estimation, and priors; validated via cross-validation and SME review.

### 7.3 Inference Engine

Exact (junction tree) on small subnetworks; approximate (loopy BP, particle filtering, variational) on larger/temporal; GPU-assisted when available.

### 7.4 Evidence Integration

Observations (vitals, findings, history, diagnostics) enter as evidence with confidence weighting; propagate updates to differentials and hidden states.

---

## 8. PyTorch Implementation

### 8.1 Technical Architecture

Unity handles graphics/UI/input; Python (PyTorch/Pyro) handles BN and physiology; communication via protocol buffers at ~10 Hz.

### 8.2 Bayesian Network Implementation (Pyro example)

```python
import torch
import pyro
import pyro.distributions as dist

class ParamedicBayesianNetwork(pyro.nn.PyroModule):
    def __init__(self):
        super().__init__()
        self.age_probs = torch.tensor([0.15, 0.65, 0.20])
        self.gender_probs = torch.tensor([0.52, 0.47, 0.01])
        self.comorbidity_base_rates = {
            'hypertension': torch.tensor([0.05, 0.25, 0.65]),
            'diabetes': torch.tensor([0.01, 0.08, 0.25]),
            'cad': torch.tensor([0.005, 0.05, 0.20])
        }

    def model(self):
        age_group = pyro.sample("age_group", dist.Categorical(self.age_probs))
        gender = pyro.sample("gender", dist.Categorical(self.gender_probs))
        comorbidities = {}
        for condition, rates in self.comorbidity_base_rates.items():
            base_rate = rates[age_group]
            if condition == 'cad' and gender == 0:
                base_rate = base_rate * 1.25
            comorbidities[condition] = pyro.sample(condition, dist.Bernoulli(base_rate))
        # ... additional complaint & disease nodes ...
        return {"age_group": age_group, "gender": gender, **comorbidities}
```

### 8.3 Physiological Modeling (PyTorch example)

```python
import torch

class PhysiologicalModel(torch.nn.Module):
    def __init__(self):
        super().__init__()
        self.normal = {"hr": 72.0, "sbp": 120.0, "dbp": 80.0, "rr": 16.0, "spo2": 98.0}
        self.state = torch.nn.ParameterDict({k: torch.nn.Parameter(torch.tensor(v)) for k, v in self.normal.items()})
        self.mods = torch.nn.ParameterDict({
            'cardiac_output': torch.nn.Parameter(torch.tensor(1.0)),
            'svr': torch.nn.Parameter(torch.tensor(1.0)),
            'blood_volume': torch.nn.Parameter(torch.tensor(1.0))
        })

    def forward(self, dt=1.0):
        self.state['sbp'].data = self.normal['sbp'] * (self.mods['cardiac_output'] * self.mods['svr'] * self.mods['blood_volume'])
        bp_dev = (self.state['sbp'].item() - self.normal['sbp']) / 10.0
        self.state['hr'].data = self.normal['hr'] - 3.0 * bp_dev
        return {k: float(v.item()) for k, v in self.state.items()}
```

### 8.4 Unity Integration (conceptual)

```csharp
// Example C# interface concept (Unity)
public class SimulationBridge {
  public string GenerateNewPatientJson() { /* call Python */ return "{}"; }
  public string UpdatePatientStateJson(float deltaTime, string interventionsJson) { return "{}"; }
}
```

### 8.5 Performance Optimization

- Network partitioning and progressive inference
- GPU acceleration for matrix ops
- Caching of common scenario priors

---

## 9. Game Development

### 9.1 Technology Stack

- Frontend: Unity 2022 LTS, URP, UI Toolkit, XR Interaction Toolkit
- Simulation Core: PyTorch 2.x, Pyro
- Data: NumPy, Pandas; serialization via Protocol Buffers
- Infrastructure: GitHub, Actions CI, issue tracking, docs

### 9.2 Development Methodology

Agile cadence with SME reviews, CI, automated tests, and medical validation checkpoints.

### 9.3 Content Development

Scenario pipeline from SME blueprint → parameterization → BN setup → Unity build → validation → iteration.

### 9.4 Assessment and Feedback System

```json
{
  "scenario_id": "cardiac_001",
  "completion_time": 12.5,
  "critical_actions": [
    {
      "action": "12-lead ECG acquisition",
      "expected_time": 3.0,
      "actual_time": 4.2,
      "rating": "delayed",
      "feedback": "ECG should be obtained within 3 minutes for suspected cardiac cases."
    },
    {
      "action": "Aspirin administration",
      "expected_time": 2.0,
      "actual_time": 1.8,
      "rating": "excellent",
      "feedback": "Aspirin administered within guideline timeframe."
    }
  ],
  "knowledge_gaps": [
    {
      "topic": "Nitroglycerin contraindications",
      "evidence": "Administered nitroglycerin despite SBP 85"
    }
  ]
}
```

---

## 10. User Interface

### 10.1 Principles

Clinical authenticity, cognitive load management, intuitive interaction, context awareness, and accessibility.

### 10.2 Primary Views

- Scene View (3D)
- Vital Signs Monitor (ECG, trends, alarms)
- Treatment Panel (meds, equipment, procedures)
- Patient Info Panel (demographics, SAMPLE, assessment, differential)

### 10.3 Interaction Design

Mouse/keyboard, touch, VR controllers, optional voice. Tiered action prioritization with safety checks.

### 10.4 Visual Design

Color coding, typography, layout zoning, iconography aligned to standards.

---

## 11. User Experience

### 11.1 Learning Experience Design

Applies cognitive load theory; deliberate practice stages: demonstration → guided → independent → reflection.

### 11.2 Scenario Progression

Core, specialty, and advanced pathways with mastery thresholds and decision-quality metrics.

### 11.3 Feedback and Assessment

Immediate, delayed, and summary feedback; task performance, decision-making, knowledge application, and professional behaviors.

### 11.4 Instructor Tools

Scenario customization, performance monitoring, curriculum integration, LMS exports.

---

## 12. Testing and Quality Assurance

### 12.1 Medical Accuracy Testing

- Expert review by certified paramedics
- Guideline verification against current protocols
- Case validation using real cases and edge conditions

#### Example Test Case: STEMI Scenario

Test Objective: Verify appropriate response to STEMI presentation

Expected Actions:

1. 12-lead ECG within 3 minutes (pass if ≤ 3.0 min)
2. Aspirin 324 mg chewed (contraindications checked)
3. Nitroglycerin if SBP ≥ 100 and no PDE5 inhibitors
4. Rapid transport decision and pre-notification
5. Repeat ECG en route if symptoms persist or change

Pass Criteria: ≥ 80% critical actions within guideline timeframes; no major contraindication violations.

### 12.2 Software Quality

- Unit tests (≥ 85% coverage for physiology and BN components)
- Scenario integration tests with golden outputs
- Performance tests (steady 10 Hz updates on target hardware)
- Usability testing cycles with students and instructors

---

## 13. Deployment and Maintenance

### 13.1 Environments

- Desktop (Windows/Linux/macOS); optional VR
- Classroom mode with instructor dashboard

### 13.2 Packaging

- Unity build artifacts; Python models packaged with versioned assets
- Configuration via environment variables and YAML profiles

### 13.3 Observability

- Structured logs for simulation events
- Anonymous telemetry for feature usage (opt-in)

### 13.4 Maintenance

- Protocol updates on a fixed cadence (quarterly)
- Model re-training with refreshed data/priors

---

## 14. Austin-Travis County Protocol Integration

### 14.1 Goals

Integrate Austin-Travis County (ATCEMS) clinical practice guidelines (CPGs) as a first-class, versioned ruleset to drive scenario validation, feedback, and action gating.

### 14.2 Data Model

- Protocol: id, name, version, effective_date, supersedes
- Pathway: condition_id, algorithm_graph (DAG), contraindications, cautions
- Action: type (assessment, medication, procedure), parameters, dosing, routes
- Constraints: hemodynamic thresholds, age/weight cutoffs, timing windows

### 14.3 Algorithm Representation

```json
{
  "protocol_id": "ATCEMS_CARDIAC_STEMI_v2025.1",
  "entry": "suspected_chest_pain",
  "nodes": {
    "suspected_chest_pain": {"type": "decision", "expr": "cc == 'chest_pain'"},
    "ecg": {"type": "action", "do": "ECG_12_LEAD"},
    "stemi": {"type": "decision", "expr": "ecg.st_elevation == true"},
    "aspirin": {"type": "action", "do": "ASPIRIN_324_PO_CHEWED"},
    "nitro_ok": {"type": "decision", "expr": "sbp >= 100 && !pde5 && !rv_infarct"},
    "nitro": {"type": "action", "do": "NITRO_0_4MG_SL_Q5MIN_X3"},
    "transport": {"type": "action", "do": "ACTIVATE_STEMI_ALERT_AND_TRANSPORT"}
  },
  "edges": [
    ["suspected_chest_pain", "ecg"],
    ["ecg", "stemi"],
    ["stemi", "aspirin", "if_true"],
    ["aspirin", "nitro_ok"],
    ["nitro_ok", "nitro", "if_true"],
    ["nitro_ok", "transport", "if_false"],
    ["nitro", "transport"]
  ]
}
```

Additional protocol snippets:

```json
{
  "protocol_id": "ATCEMS_RESP_TENSION_PTX_v2025.1",
  "entry": "suspected_tension_ptx",
  "nodes": {
    "suspected_tension_ptx": {"type": "decision", "expr": "trauma && hypotension && unilateral_breath_sounds"},
    "oxygen": {"type": "action", "do": "HIGH_FLOW_OXYGEN"},
    "decompress": {"type": "action", "do": "NEEDLE_DECOMPRESSION_2NDICS_MCL_OR_4TH5TH_AAL"},
    "reassess": {"type": "action", "do": "REASSESS_VITALS_AND_BREATH_SOUNDS"}
  },
  "edges": [["suspected_tension_ptx", "oxygen"], ["oxygen", "decompress"], ["decompress", "reassess"]]
}
```

```json
{
  "protocol_id": "ATCEMS_TOX_OPIOID_OD_v2025.1",
  "entry": "respiratory_depression_with_opioid_signs",
  "nodes": {
    "support_airway": {"type": "action", "do": "BVM_VENTILATION"},
    "naloxone": {"type": "action", "do": "NALOXONE_TITRATE_IV_IM_IN"},
    "monitor": {"type": "action", "do": "MONITOR_FOR_RESEDATION"}
  },
  "edges": [["respiratory_depression_with_opioid_signs", "support_airway"], ["support_airway", "naloxone"], ["naloxone", "monitor"]]
}
```

```json
{
  "protocol_id": "ATCEMS_OB_NORMAL_DELIVERY_v2025.1",
  "entry": "impending_delivery",
  "nodes": {
    "prepare": {"type": "action", "do": "OB_PREPARE_FIELD_DELIVERY"},
    "deliver": {"type": "action", "do": "CONTROLLED_DELIVERY_SUPPORT_HEAD"},
    "newborn": {"type": "action", "do": "NEONATAL_RESUSCITATION_NRP_STEPS"},
    "pph": {"type": "decision", "expr": "excessive_bleeding"},
    "pph_mgmt": {"type": "action", "do": "FUNDAL_MASSAGE_OXYTOCIN_IF_AVAILABLE"}
  },
  "edges": [["impending_delivery", "prepare"], ["prepare", "deliver"], ["deliver", "newborn"], ["deliver", "pph"], ["pph", "pph_mgmt", "if_true"]]
}
```

### 14.4 Versioning and Governance

- Semantic versioning (e.g., v2025.1) with change logs
- Protocol engine loads multiple versions for historical scenarios
- Medical director approval workflow for updates

### 14.5 Validation Engine

- Pre-execution checks: contraindications (e.g., SBP < 100 for nitro)
- Sequence enforcement with configurable tolerance windows
- Feedback mapping: each node has success/failure messages and references

### 14.6 Example Contraindication Check (pseudo-code)

```python
def can_administer_nitro(vitals, meds):
    return (
        vitals.sbp >= 100 and
        not meds.taken_pde5_in_48h and
        not vitals.suspected_rv_infarct
    )
```

---

## 15. Research Plan for Statistical Models

### 15.1 Objectives

- Calibrate BN priors and CPTs to reflect EMS epidemiology and outcomes
- Validate physiological response models against literature and SME ratings
- Ensure real-time inference at 10 Hz without material loss of accuracy

### 15.2 Datasets

- NEMSIS v3 (2017–2024 subsets where accessible)
- NHANES, CDC WISQARS, AHRQ MEPS
- Literature-derived parameters for disease-specific models
- Expert elicitation for rare events

### 15.3 Methods

- Parameter learning: MLE and Bayesian updates with expert-informed priors
- Sensitivity analysis: Sobol indices on key parameters
- Validation: k-fold cross-validation on outcome prediction; SME Likert scoring of realism
- Robustness: ablation studies on subgraphs; noise/stress tests

### 15.4 Experiments

1. STEMI recognition probability given features (ECG, risk factors)
2. Fluid responsiveness in hypovolemia (SBP/HR trajectories)
3. Asthma response to bronchodilator dosing (RR/SpO2 curves)

### 15.5 Success Criteria

- AUROC ≥ 0.85 for condition recognition tasks in simulated datasets
- SME realism median ≥ 4/5 for top scenarios
- 10 Hz update sustained on target hardware with < 30% CPU for sim core

### 15.6 Timeline (example)

- Month 1–2: Data ingestion, priors, baseline BN
- Month 3–4: Physiology calibration, unit/integration tests
- Month 5: Scenario validation, performance tuning, documentation

### 15.7 Risks and Mitigations

- Data sparsity → Expert priors, transfer learning, synthetic augmentation
- Performance constraints → Model partitioning, progressive inference, GPU ops
- Protocol drift → Versioned rulesets and scheduled reviews

---

## 16. Conclusion

This document specifies a rigorous, medically grounded, and technically feasible simulation platform. By combining Bayesian generative modeling, dynamic physiology, and protocol-driven guidance (including Austin-Travis County CPGs), the simulator targets strong educational gains, scalable content creation, and high face validity for learners and instructors.

---

## Conversion Notes

- Use a Markdown-to-PDF tool that supports page headers/footers and TOC generation (e.g., Pandoc with `--toc`).
- Code blocks include language tags for syntax highlighting.
- Anchors follow GitHub-style automatic IDs; PDF tools typically preserve them in TOC.


