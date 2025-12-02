# Research Pipeline Quickstart

Place NEMSIS files:

- research/inputs/nemsis/nemsis_v3_2019.parquet
- ... up to nemsis_v3_2024.parquet

Run steps:

1. Clean

   ```bash
   python -m research.scripts.nemsis.load_and_clean 2019 2020 2021 2022 2023 2024
   ```

1. Select cases

   ```bash
   python -m research.scripts.nemsis.select_cases 2019 2020 2021 2022 2023 2024
   ```

1. Compute measures

   ```bash
   python -m research.scripts.nemsis.compute_measures 2019 2020 2021 2022 2023 2024
   ```

Outputs will be written under `research/outputs/nemsis/` with logs under `research/outputs/logs/`.
