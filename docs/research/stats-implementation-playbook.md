# EMS Statistics Implementation Playbook (Per-Database, File-Level, Outputs)

This playbook specifies exactly what to extract from each data source, where scripts live, the filenames to write, and audit logs. It complements `statistics-research-plan.md` with operational details.

## Repository Layout (new paths)

- `research/scripts/`
  - `nemsis/`: NEMSIS ETL and measures
  - `benchmarks/`: CARES/OHCA, HCUP, WISQARS, State cross-check
  - `utils/`: shared helpers (io, ci-calcs, codebooks)
- `research/codebooks/`
  - `nemsis-elements.yaml`: element→meaning map
  - `case-definitions.yaml`: strict/proxy logic per condition
  - `codes-icd10.yaml`, `codes-snomed.yaml`, `codes-cpt.yaml`
  - `urbanicity-map.yaml`, `age-bands.yaml`
- `research/outputs/`
  - `nemsis/` (primary counts/rates per condition)
  - `benchmarks/` (CARES, HCUP, WISQARS, state)
  - `logs/` (ETL run logs, missingness reports)
- `docs/research/outputs/` (Markdown summaries linked in doc set)

## Naming Conventions

- CSV: `source__topic__year[_strata].csv`
- Markdown: `source__topic__summary.md`
- Logs: `YYYYMMDDTHHMM__pipeline-step.log`

---

## 1) NEMSIS v3 (2019–2024) — Primary Incidence

- Input (assumed access): Yearly parquet/CSV exports or state-provided extracts
  - Files: `nemsis_v3_{YEAR}.parquet` (preferred) or `nemsis_v3_{YEAR}.csv`
- Elements to read (minimum):
  - `eSituation.11` Primary Impression; `eSituation.12` Secondary Impression
  - `eSituation.09` Chief Complaint Anatomic Location; `eSituation.10` Chief Complaint Organ System
  - `eVitals.*` vital sets (RR, SBP, SpO2, GCS, EtCO2)
  - `eProcedures.03` Procedure; `eProcedures.07` Success; timestamps
  - `eMedications.03` Medication Given; `eMedications.05` Dose; timestamps
  - `eArrest.*` (for OHCA alignment when CARES not available)
  - `eDisposition.*` outcome; transported/not transported; destination
  - `ePatient.13` Age; `ePatient.15` Sex
  - `eScene.17` Incident Location Type; `eTimes.*` (for QA only)
- Case definitions: read from `research/codebooks/case-definitions.yaml`
- Scripts (Python suggested):
  - `research/scripts/nemsis/load_and_clean.py`
    - Reads yearly files, coerces types, flags implausible vitals; writes `research/outputs/logs/{stamp}__nemsis_clean.log`
  - `research/scripts/nemsis/select_cases.py`
    - Applies strict and proxy definitions per condition; writes per-year selections to temp parquet
  - `research/scripts/nemsis/compute_measures.py`
    - Denominators (total incidents), numerators (per condition), 95% CIs; stratifications
  - `research/scripts/utils/ci.py`
    - Poisson/Wilson CI helpers
- Outputs:
  - `research/outputs/nemsis/nemsis__denominators__{YEAR}.csv`
  - `research/outputs/nemsis/nemsis__chf_ape__{YEAR}__by_age_sex_urbanicity.csv`
  - `research/outputs/nemsis/nemsis__opioid_od__{YEAR}__by_age_sex_urbanicity.csv`
  - `research/outputs/nemsis/nemsis__tension_ptx_proxy__{YEAR}__by_age_sex_urbanicity.csv`
  - `research/outputs/nemsis/nemsis__ob_delivery_pph_nrp__{YEAR}__by_age_sex_urbanicity.csv`
  - Sensitivity tables:
    - `research/outputs/nemsis/nemsis__{condition}__{YEAR}__strict_vs_proxy.csv`
- Summaries (MD):
  - `docs/research/outputs/nemsis__{condition}__summary.md` (auto-generated tables + caveats)

---

## 2) CARES (OHCA) — Benchmarking (Utstein)

- Access: DUA preferred; otherwise use published annual statistics
- If DUA available — Inputs (CARES extract):
  - Files: `cares_ohca_{YEAR}.csv` (site export), with Utstein fields: witnessed, initial rhythm, bystander CPR/AED, ROSC, survival to discharge
- Scripts:
  - `research/scripts/benchmarks/cares_summarize.py`
    - Computes incidence and outcomes; aligns variable names to Utstein; writes logs
- Outputs:
  - `research/outputs/benchmarks/cares__ohca__{YEAR}__utstein.csv`
  - `docs/research/outputs/cares__ohca__summary.md`
- If only published stats:
  - Manual scrape/store:
    - `research/outputs/benchmarks/cares__ohca__annual_summary_{YEAR}.csv`
    - `docs/research/outputs/cares__ohca__annual_summary.md` with citations

---

## 3) State/Regional EMS Annual Reports — Cross-Check

- Inputs: PDFs/CSVs (e.g., TX GETAC, CA EMSA)
  - Save to `research/inputs/state_reports/{STATE}_{YEAR}.{pdf|csv}`
- Scripts:
  - `research/scripts/benchmarks/state_reports_catalog.py` (index metadata)
  - `research/scripts/benchmarks/state_reports_extract.py` (if CSV/Excel) or manual entry if PDF only
- Outputs:
  - `research/outputs/benchmarks/state__{STATE}__{YEAR}__key_metrics.csv` (naloxone admin counts, deliveries, arrests)
  - `docs/research/outputs/state__{STATE}__{YEAR}__key_metrics.md`

---

## 4) CDC WISQARS — Denominators (Trauma Mechanisms)

- Access: public web; export CSV from WISQARS
- Targets: mechanism counts, age/sex strata for population denominators
- Scripts:
  - `research/scripts/benchmarks/wisqars_download.py` (API/browser automation where available)
- Outputs:
  - `research/outputs/benchmarks/wisqars__injury_mechanisms__{YEAR}.csv`
  - `docs/research/outputs/wisqars__notes.md` (methods and caveats)

---

## 5) HCUP (NEDS/NIS) & NHAMCS — ED/Admissions Triangulation

- Access: published tables (public) or purchased microdata (if available)
- Targets: ED visit rates for CHF/APE, opioid OD, trauma chest injury proxies, obstetric emergencies
- Scripts:
  - `research/scripts/benchmarks/hcup_catalog.md` (manual table index with URLs)
  - `research/scripts/benchmarks/hcup_extract.py` (if CSV tables)
- Outputs:
  - `research/outputs/benchmarks/hcup__{dataset}__{topic}__{YEAR}.csv`
  - `docs/research/outputs/hcup__{topic}__summary.md`

---

## 6) ESO Data Collaborative — Trends (If Partner Access)

- Access: DUA/partner
- Targets: annual ESO EMS Index metrics relevant to our conditions
- Outputs (manual citation or partner extract):
  - `docs/research/outputs/eso__ems_index__notes.md`

---

## 7) ODMAP (HIDTA) & 8) SUDORS (CDC) — Overdose Context

- Access: agency/state
- Targets: OD counts/time-of-day, demographics (context, not incidence substitute)
- Outputs:
  - `docs/research/outputs/odmap__notes.md`
  - `docs/research/outputs/sudors__notes.md`

---

## 9) NTDB/TQIP (ACS) — Trauma Hospital Benchmarks

- Access: DUA
- Targets: chest trauma mechanisms/outcomes to contextualize PTX incidence
- Outputs:
  - `docs/research/outputs/ntdb_tqip__notes.md`

---

## 10) GWTG–Stroke / Mission: Lifeline — Cardiac/Neuro Benchmarks

- Access: site/DUA
- Targets: prehospital timestamps (if available), ED door metrics; triangulate STEMI/stroke incidence
- Outputs:
  - `docs/research/outputs/gwtg_mission__notes.md`

---

## 11) EMSC/PECARN — Pediatric EMS Incidence

- Access: publications/public datasets if available
- Targets: pediatric respiratory/cardiac/OB-related EMS encounters
- Outputs:
  - `docs/research/outputs/pecarn_emsc__notes.md`

---

## 12) NFIRS — Operational Denominators

- Access: public extracts
- Targets: EMS response counts by region/urbanicity
- Scripts:
  - `research/scripts/benchmarks/nfirs_extract.py`
- Outputs:
  - `research/outputs/benchmarks/nfirs__ems_responses__{YEAR}.csv`
  - `docs/research/outputs/nfirs__summary.md`

---

## Cross-Source Harmonization

- Mapping tables: `research/codebooks/urbanicity-map.yaml`, `age-bands.yaml`
- Common condition keys: `chf_ape`, `opioid_od`, `tension_ptx_proxy`, `ob_delivery_pph_nrp`, `ohca`
- CI calculations standardized via `research/scripts/utils/ci.py`

## Runbook (Phase 1)

1. Populate `case-definitions.yaml` and `nemsis-elements.yaml`
2. Implement `load_and_clean.py`, `select_cases.py`, `compute_measures.py`
3. Produce NEMSIS outputs for 2019–2024 (all four conditions)
4. Add CARES annual OHCA stats (or DUA extract) and write summary MD
5. Pull WISQARS/HCUP denominators; write notes
6. Assemble `docs/research/outputs/*summary.md` with tables/figures
7. Review with SME and iterate

## Logging & QA Artifacts

- `research/outputs/logs/{stamp}__nemsis_clean.log` — type coercions, exclusions, timing
- `research/outputs/logs/{stamp}__selection_{condition}.log` — case counts (strict/proxy)
- `research/outputs/nemsis/{condition}__missingness__{YEAR}.csv` — field NA rates

## Deliverables Checklist

- [ ] CSVs for counts/rates/CIs per condition×strata 2019–2024
- [ ] Sensitivity tables (strict vs proxy)
- [ ] Benchmark summaries (CARES, WISQARS/HCUP, state reports)
- [ ] Methods appendix with codebooks and logic
- [ ] SME-reviewed Markdown summaries in `docs/research/outputs/`
