# EMS Condition Frequency and Epidemiology — Research Plan

## Objectives

- Quantify how common key conditions/presentations are in EMS encounters (overall and stratified).
- Produce comparable rates per 1,000 EMS incidents and per 10,000 population, with 95% CIs.
- Provide system-relevant breakdowns (age group, sex, region/urbanicity, chief complaint vs final impression).

## Scope (Initial Conditions)

- Medical: Acute decompensated heart failure (CHF/APE), opioid overdose.
- Trauma: Tension pneumothorax (as proxy via procedure + indicators).
- Special: Prehospital delivery, postpartum hemorrhage, neonatal resuscitation triggers.

## Data Sources (Priority Order)

1. NEMSIS v3 (2019–2024): national EMS dataset; variables for eSituation, eInjury, eArrest, eVitals, eProcedures, eMedications, eOutcome.
2. CARES (Cardiac Arrest Registry to Enhance Survival): OHCA incidence, Utstein variables, outcomes; for benchmarking simulation incidence/algorithms (subject to data-use agreements).
3. State/Regional EMS annual reports (e.g., Texas, California) for cross-check.
4. NHAMCS/HCUP-NIS/HCUP-NEDS: ED/Admissions rates to triangulate prevalence where EMS-specific coding is ambiguous.
5. CDC WISQARS: trauma mechanisms for denominators and comparative risk.
6. CDC WONDER mortality for opioid overdose trend context (not EMS-specific but supportive).
7. Published PEC/JEMS/NAEMSP studies with EMS-specific incidence where available.
8. ESO Data Collaborative: multi-agency EMS clinical/ops data; ESO EMS Index provides yearly trends; Access: partner/DUA.
9. NEMSQA Metrics: national EMS quality measures for definitions/benchmarks; Access: public specs (data via agencies).
10. ODMAP (HIDTA): near-real-time overdose mapping submitted by agencies; Access: agency participation portal.
11. SUDORS (CDC): state unintentional drug overdose reporting; tox/circumstances; Access: state/public summaries.
12. NTDB/TQIP (ACS): hospital trauma registry for mechanisms/outcomes; trauma denominators/trends; Access: DUA.
13. Get With The Guidelines–Stroke / Mission: Lifeline (AHA): hospital registries with some prehospital timestamps; STEMI/stroke incidence/outcomes; Access: site/DUA.
14. EMSC/PECARN: pediatric EMS research/surveys; pediatric incidence and care gaps; Access: publications/public datasets where available.
15. Air Medical registries (AAMS/AMPA): HEMS utilization and clinical metrics; Access: org/DUA.
16. NFIRS: fire incident data including many EMS responses; operational denominators; Access: public extracts.

## Case Definitions (Operational)

- CHF/Acute Pulmonary Edema: any of
  - Provider impression includes CHF/APE; AND/OR
  - CPAP/NIV administered AND diuretic/nitrate given AND respiratory distress primary; AND/OR
  - Narrative codes/ICD-10 I50.x when available.
- Opioid Overdose: any of
  - Medication: naloxone given; AND respiratory depression documented; AND/OR
  - Provider impression/toxidrome: opioid; AND/OR
  - ICD-10 T40.x exposure/poisoning fields when present.
- Tension Pneumothorax: proxy
  - Procedure: needle thoracostomy; AND traumatic mechanism; AND/OR
  - Impression: pneumothorax + severe respiratory distress/hypotension.
- Prehospital Delivery/PPH/NRP
  - Procedure codes indicating delivery; newborn assessment; excessive maternal bleeding coded; neonatal resuscitation steps performed.

## Variables to Extract

- Demographics: age group (peds/adult/geriatric), sex
- Call info: urbanicity, region/state (if available), response type
- Clinical: chief complaint, provider impression, vitals at arrival, procedures, meds
- Timers: on-scene time, transport time (for context)
- Outcomes: transport/non-transport, destination type, ED disposition (if linked)

## Denominators and Rates

- Primary denominator: total EMS incidents meeting inclusion criteria (per 1,000 incidents)
- Secondary: population-based rates where state population denominators available (per 10,000 pop)
- Report both raw counts and rates with 95% CIs (Wilson or Poisson exact depending on counts)

## Stratifications

- Age: 0–17, 18–64, 65+
- Sex: male, female (as recorded)
- Geography: census region if available; urban vs rural classification
- Temporal: year, quarter; time-of-day for OD sensitivity

## Data Quality & Bias Handling

- Handle missing/imputed values transparently (report % missing).
- Sensitivity analyses with alternative definitions (e.g., naloxone-only vs naloxone+RR<10 for OD).
- Weighting: if NEMSIS provides state participation variability, include sensitivity to coverage.

## Methodological Principles for Prehospital Data

- Use proxy definitions when documentation is incomplete (procedures/meds/vitals-based logic) and report both proxy and strict estimates.
- Prefer incident-based denominators; avoid double-counting by collapsing multiple assessments per incident.
- Flag implausible vitals/time stamps and apply conservative cleaning rules; report exclusion counts.
- Quantify and display missingness per critical field (impression, procedures, medications, vitals).
- Stratify by urbanicity and year to account for system mix changes and protocol drift.
- Triangulate incidence with state EMS reports and ED datasets for face validity; do not force reconciliation where definitions differ.
- For cardiac arrest statistics, use CARES definitions (Utstein) when available; reconcile with NEMSIS eArrest elements where CARES linkage is not accessible.

### First-Pass Sources (for this project phase)

- Primary: NEMSIS v3 (2019–2024) extracts.
- Benchmarking OHCA: CARES annual statistics (or DUA access if feasible in timeline).
- Denominators/triangulation: CDC WISQARS and HCUP-NEDS/NIS/NHAMCS published stats.
- Cross-checks: State EMS annual reports.

## Analysis Methods

- Descriptive statistics: frequencies, proportions, rates with 95% CIs.
- Trend analysis: yearly rate ratios; segmented regression if notable policy changes.
- Cross-check: compare EMS-derived rates with ED datasets (NHAMCS/HCUP) for face validity.

## Outputs

- Tables (CSV + Markdown): counts and rates per condition × stratification.
- Figures: bar charts of rates by age/sex; line plots for yearly trends.
- Methods appendix: exact code lists, variable maps, and case-definition logic.

## Tooling & Reproducibility

- Data processing: Python (pandas) or R (tidyverse); scripts committed under `research/scripts/`.
- Version control of code lists (ICD-10, SNOMED, NEMSIS element IDs) in YAML.
- Document limitations and data-use agreements; avoid row-level PHI.

## Timeline (initial pass)

- Week 1: Finalize definitions; acquire NEMSIS access; prototype extracts.
- Week 2: Compute national-level rates; produce first tables/figures.
- Week 3: Stratifications, sensitivity analyses; documentation.

## Citations Plan

- NEMSIS data dictionaries and annual reports.
- CARES annual report and methodology documentation (Utstein definitions).
- PEC/JEMS articles reporting EMS NIV, naloxone, prehospital delivery incidence.
- CDC WISQARS, HCUP/NHCS methodology pages for denominators and comparators.

## Next Actions

- Confirm condition list and definitions with SME.
- Request/prepare NEMSIS extracts.
- Build reproducible pipeline skeleton and table templates.
- Assess feasibility of CARES access (DUA); alternatively, use published CARES annual statistics for benchmarking OHCA incidence/outcomes.

## Implementation Checklist (Detailed Steps)

### Access & Governance

- [ ] Confirm final condition list for phase 1 (CHF/APE, opioid OD, tension PTX proxy, OB delivery/PPH/NRP, OHCA benchmarking)
- [ ] Identify data owners and submit access requests (NEMSIS state permissions as needed)
- [ ] Determine CARES access path (DUA) or confirm use of published annual stats
- [ ] Document data-use agreements and PHI handling (no row-level PHI in outputs)

### Case Definitions & Codebooks

- [ ] Draft strict and proxy definitions per condition (logic trees)
- [ ] Map NEMSIS v3 elements (eSituation, eImpression, eVitals, eProcedures, eMedications, eArrest, eDisposition)
- [ ] Enumerate ICD-10/SNOMED/CPT/code lists; save in YAML under `research/codebooks/`
- [ ] Peer review of definitions with SME and revise

### Extraction Pipeline

- [ ] Create project structure `research/scripts/` (Python/R) and `research/outputs/`
- [ ] Implement extract module to load/filter NEMSIS years 2019–2024
- [ ] Implement per-condition selectors (strict vs proxy)
- [ ] Compute denominators (total incidents) and numerators by condition
- [ ] Generate primary stratifications (age, sex, urbanicity, year)
- [ ] Calculate rates per 1,000 incidents and 95% CIs (Poisson/Wilson)

### Quality Assurance

- [ ] Build missingness tables for key fields (impression, procedures, meds, vitals)
- [ ] Apply outlier/implausible value filters (document thresholds)
- [ ] Sensitivity analyses (strict vs proxy) — produce comparison tables
- [ ] Cross-check counts/rates against state EMS annual reports
- [ ] Triangulate selected metrics with HCUP/NEDS or WISQARS where applicable

### OHCA Benchmarking (CARES/NEMSIS eArrest)

- [ ] Align variables with Utstein definitions (witnessed, initial rhythm, bystander CPR, ROSC)
- [ ] Produce OHCA incidence and outcome summary for benchmarking
- [ ] Document reconciliation approach when CARES not directly accessible

### Outputs & Reporting

- [ ] Export CSV tables: counts, rates, CIs per condition × stratification
- [ ] Create Markdown summaries with key findings and caveats in `docs/research/outputs/`
- [ ] Generate figure stubs (bar/line plots) or Vega-Lite specs
- [ ] Write short methods appendix (variable maps, logic, exclusions)

### Reproducibility & Review

- [ ] Commit scripts, codebooks, and documentation to repo
- [ ] Tag data version and code hash for reproducibility
- [ ] SME review of results and definitions; incorporate feedback
- [ ] Finalize v1 dataset and documentation

## Master Detailed Checklist (Granular Execution Steps)

### A. Repository and Environment Setup

- [ ] Create directories: `research/scripts/{nemsis,benchmarks,utils}`, `research/{outputs,inputs,codebooks}`, `docs/research/outputs`, `research/outputs/logs`, `research/outputs/nemsis`, `research/outputs/benchmarks`, `research/inputs/state_reports`
- [ ] Initialize Python environment (3.11+), `requirements.txt` with pandas, pyarrow, numpy, scipy, pyyaml, tqdm
- [ ] Create `research/scripts/utils/io.py` with helpers for reading CSV/Parquet, writing CSV with schema
- [ ] Create `research/scripts/utils/ci.py` for Poisson/Wilson CI functions with unit tests
- [ ] Create `research/scripts/utils/logging.py` for timestamped logfile writer
- [ ] Add `.gitattributes` for CSV/MD text normalization; `.gitkeep` files for empty dirs

### B. Codebooks and Definitions

- [ ] Draft `research/codebooks/nemsis-elements.yaml` mapping required elements to descriptions and types
- [ ] Draft `research/codebooks/case-definitions.yaml` with strict & proxy logic trees for: `chf_ape`, `opioid_od`, `tension_ptx_proxy`, `ob_delivery_pph_nrp`, `ohca`
- [ ] Draft `research/codebooks/codes-icd10.yaml` with I50.x (HF), T40.x (opioids), S27.0 (pneumothorax), O72.x (PPH)
- [ ] Draft `research/codebooks/codes-snomed.yaml` for common impressions if used by agencies
- [ ] Draft `research/codebooks/codes-cpt.yaml` for procedure crosswalks (needle thoracostomy)
- [ ] Draft `research/codebooks/age-bands.yaml` with cutpoints 0–17, 18–64, 65+
- [ ] Draft `research/codebooks/urbanicity-map.yaml` to map NEMSIS location types to urban/rural
- [ ] SME review of `case-definitions.yaml`; capture revisions

### C. Data Acquisition (NEMSIS)

- [ ] Confirm legal access path (national/state portal or local extracts)
- [ ] For each YEAR=2019..2024: obtain `nemsis_v3_{YEAR}.parquet` (preferred) or CSV
- [ ] Save raw files under `research/inputs/nemsis/nemsis_v3_{YEAR}.parquet`
- [ ] Verify file integrity: size, row count, quick schema dump to `research/outputs/logs/{stamp}__nemsis_schema_{YEAR}.log`
- [ ] Document any state participation/coverage notes per year in `docs/research/outputs/nemsis_coverage_notes.md`

### D. Load and Clean (NEMSIS)

- [ ] Implement `research/scripts/nemsis/load_and_clean.py`
- [ ] Read YEAR files with pyarrow for types; coerce dates/times to UTC-naive
- [ ] Normalize categorical codes to upper-case strings
- [ ] Validate required columns exist; log missing columns per YEAR
- [ ] Flag implausible vitals (e.g., RR<4 or >60 adults; SBP<50 or >300; SpO2>100) per `thresholds.yaml`
- [ ] Drop duplicate incident IDs; log duplicates
- [ ] Write cleaned Parquet `research/outputs/nemsis/clean__nemsis_v3_{YEAR}.parquet`
- [ ] Emit cleaning report `research/outputs/logs/{stamp}__nemsis_clean_{YEAR}.log`

### E. Denominators and Incident Collapsing

- [ ] Define unique incident key (eScene + eTimes or provided incident number)
- [ ] Collapse multiple vitals/assessments per incident as needed (first-on-scene vs highest acuity rule)
- [ ] Compute per-YEAR total incident denominators; write `nemsis__denominators__{YEAR}.csv`

### F. Case Selection (Per Condition)

- [ ] Implement `research/scripts/nemsis/select_cases.py`
- [ ] Encode strict logic for CHF/APE (impression CHF/APE OR CPAP+resp distress+NTG)
- [ ] Encode proxy logic for CHF/APE (CPAP OR NTG with resp distress)
- [ ] Encode strict logic for opioid OD (naloxone + hypoventilation OR impression opioid toxicity)
- [ ] Encode proxy logic for opioid OD (naloxone administered)
- [ ] Encode strict logic for tension PTX (needle thoracostomy + trauma + pneumothorax impression)
- [ ] Encode proxy logic for tension PTX (needle thoracostomy + trauma)
- [ ] Encode strict logic for OB (delivery procedure OR newborn assessment + maternal bleeding O72.x for PPH)
- [ ] Encode proxy logic for OB (delivery OR neonatal resuscitation steps)
- [ ] Write per-condition per-YEAR temp selections to Parquet
- [ ] Log counts for strict vs proxy per condition and YEAR

### G. Stratification and Measures

- [ ] Implement `research/scripts/nemsis/compute_measures.py`
- [ ] Derive age bands from `age-bands.yaml`
- [ ] Map urbanicity from `urbanicity-map.yaml`
- [ ] For each condition & YEAR: compute counts by age, sex, urbanicity
- [ ] Calculate rates per 1,000 incidents using denominators
- [ ] Compute 95% CIs (Poisson exact if counts<100, Wilson otherwise)
- [ ] Write CSVs: `nemsis__{condition}__{YEAR}__by_age_sex_urbanicity.csv`

### H. Sensitivity Analyses

- [ ] For each condition & YEAR: build strict vs proxy comparison table (counts, rates, deltas)
- [ ] Write `nemsis__{condition}__{YEAR}__strict_vs_proxy.csv`
- [ ] Summarize differences and likely bias directions in MD summaries

### I. Missingness and Outliers

- [ ] Compute NA rates for impression, procedures, medications, vitals per YEAR
- [ ] Write `nemsis/{condition}__missingness__{YEAR}.csv`
- [ ] Document applied filters (thresholds, exclusions) in `methods_missingness.md`

### J. CARES (OHCA)

- [ ] Determine DUA feasibility and timeline
- [ ] If DUA: obtain `cares_ohca_{YEAR}.csv` with Utstein fields
- [ ] Implement `benchmarks/cares_summarize.py` to compute incidence, ROSC, survival
- [ ] Write `cares__ohca__{YEAR}__utstein.csv`
- [ ] If no DUA: capture published annual stats into `cares__ohca__annual_summary_{YEAR}.csv`
- [ ] Write MD summary `cares__ohca__summary.md`

### K. WISQARS / HCUP / NHAMCS (Benchmarks)

- [ ] Download WISQARS mechanism CSVs for target years; save to `research/outputs/benchmarks`
- [ ] Record query parameters used (age, mechanism, geography) in `wisqars__notes.md`
- [ ] Identify HCUP/NHAMCS tables for CHF/APE, opioid OD, chest trauma, OB emergencies
- [ ] Extract published table values to CSV with sources and URLs in `hcup__{topic}__summary.md`

### L. State EMS Annual Reports

- [ ] Collect PDFs/CSVs for target states/years; save to `research/inputs/state_reports`
- [ ] Catalog metadata with `state_reports_catalog.py` (state, year, URL, metrics available)
- [ ] Where CSV available, parse key metrics into `state__{STATE}__{YEAR}__key_metrics.csv`
- [ ] For PDFs, manually transcribe key tables and cite page numbers in MD summaries

### M. Outputs and Documentation

- [ ] Generate per-condition MD summaries under `docs/research/outputs/nemsis__{condition}__summary.md`
- [ ] Include: definition used, denominators, counts, rates, CIs, sensitivity results, data quality notes
- [ ] Create overview `docs/research/outputs/phase1_summary.md` aggregating all conditions
- [ ] Store figure-ready CSVs and Vega-Lite specs if used

### N. Reproducibility

- [ ] Commit codebooks and scripts with version tags
- [ ] Record data file hashes (SHA256) in `research/outputs/logs/data_hashes_{stamp}.txt`
- [ ] Pin Python package versions in `requirements.txt`
- [ ] Add `README.md` in `research/scripts` explaining run order

### O. Review and Sign-off

- [ ] Internal QA pass on numbers (spot-check vs raw)
- [ ] SME review meeting; collect action items
- [ ] Apply fixes and re-run impacted steps; update logs and summaries
- [ ] Final sign-off and tag release `stats-phase1-v1`

## Immediate Next Steps (Execution Checklist)

1. Element mapping/codebook
   - Map NEMSIS v3 elements to each case definition (e.g., eInjury, eSituation, eVitals, eProcedures, eMedications, eDisposition).
   - Enumerate ICD-10/SNOMED codes used when available; store in YAML.
2. Prototype extraction
   - Build scripts to compute national counts and rates (2019–2024) for CHF/APE, opioid OD, tension PTX (proxy), OB delivery/PPH/NRP.
   - Generate 95% CIs (Poisson/Wilson) and primary stratifications (age, sex, urbanicity, year).
3. Sensitivity analyses
   - Produce strict vs proxy definition tables per condition; document differences.
4. Data-quality appendix
   - Missingness tables, outlier filters applied, coverage caveats (state participation).
5. Deliverables
   - Markdown + CSV tables, figure stubs, and a short memo summarizing key rates and caveats.
