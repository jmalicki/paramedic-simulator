import os
import sys
import pandas as pd
from research.scripts.utils.yaml_loader import load_yaml
from research.scripts.utils.ci import poisson_ci
from research.scripts.utils.logging import write_log


def assign_age_band(age: float, bands: list[dict]) -> str:
    for b in bands:
        if age is None:
            return "unknown"
        if b["min"] <= age <= b["max"]:
            return b["name"]
    return "unknown"


def compute_for_year(year: int, conditions: list[str]):
    logs = []
    den_path = f"research/outputs/nemsis/clean__nemsis_v3_{year}.parquet"
    if not os.path.exists(den_path):
        logs.append(f"MISSING {den_path}")
        return logs
    df_all = pd.read_parquet(den_path)
    denom = len(df_all)
    bands = load_yaml("research/codebooks/age-bands.yaml")["bands"]
    urbmap = load_yaml("research/codebooks/urbanicity-map.yaml")["map"]

    def urbanicity(row):
        loc = str(row.get("eScene.17", "")).upper()
        if loc in (x.upper() for x in urbmap.get("urban", [])):
            return "urban"
        if loc in (x.upper() for x in urbmap.get("rural", [])):
            return "rural"
        return "unknown"

    for cond in conditions:
        sel_path = f"research/outputs/nemsis/select__{cond}__{year}.parquet"
        if not os.path.exists(sel_path):
            logs.append(f"MISSING {sel_path}")
            continue
        df = pd.read_parquet(sel_path)
        df["age_band"] = df.get("ePatient.13", pd.Series([None] * len(df))).apply(lambda a: assign_age_band(a, bands))
        df["sex"] = df.get("ePatient.15", pd.Series(["unknown"] * len(df))).fillna("unknown").str.lower()
        df["urbanicity"] = df.apply(urbanicity, axis=1)

        grp = df.groupby(["age_band", "sex", "urbanicity"], dropna=False).size().reset_index(name="count")
        grp["rate_per_1000"] = grp["count"].apply(lambda c: (c / denom) * 1000.0)
        grp["ci_lo"], grp["ci_hi"] = zip(*grp["count"].apply(lambda c: poisson_ci(c, denom)))
        grp["ci_lo"] = grp["ci_lo"] * 1000.0
        grp["ci_hi"] = grp["ci_hi"] * 1000.0

        out_csv = f"research/outputs/nemsis/nemsis__{cond}__{year}__by_age_sex_urbanicity.csv"
        os.makedirs(os.path.dirname(out_csv), exist_ok=True)
        grp.to_csv(out_csv, index=False)
        logs.append(f"WROTE {out_csv}")
    return logs


def main(years):
    conditions = ["chf_ape", "opioid_od", "tension_ptx_proxy", "ob_delivery_pph_nrp", "ohca"]
    logs = []
    for y in years:
        logs.extend(compute_for_year(y, conditions))
    write_log("nemsis_measures", logs)


if __name__ == "__main__":
    ys = [int(a) for a in sys.argv[1:]] if len(sys.argv) > 1 else list(range(2019, 2025))
    main(ys)

def main():
    # Placeholder: compute denominators, stratifications, CIs; write CSVs
    pass


if __name__ == "__main__":
    main()


