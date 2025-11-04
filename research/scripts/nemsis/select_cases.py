import os
import sys
import pandas as pd
from research.scripts.utils.yaml_loader import load_yaml
from research.scripts.utils.logging import write_log


def contains_any(text: str, terms: list[str]) -> bool:
    if pd.isna(text):
        return False
    t = str(text).lower()
    return any(term.lower() in t for term in terms)


def select_condition(df: pd.DataFrame, condition_key: str, defs: dict) -> tuple[pd.Series, pd.Series]:
    d = defs["conditions"][condition_key]
    # Minimal, heuristic implementation using available columns; adapt to real schemas
    def logic_strict(row):
        if condition_key == "chf_ape":
            imp = str(row.get("eSituation.11", "")).upper()
            has_cp = str(row.get("eProcedures.03", "")).upper()
            med = str(row.get("eMedications.03", "")).upper()
            cc = str(row.get("eSituation.10", ""))
            return (
                ("CHF" in imp or "PULMONARY EDEMA" in imp)
                or (("CPAP" in has_cp or "NIV" in has_cp) and contains_any(cc, ["respiratory", "dyspnea"]) and "NITRO" in med)
            )
        if condition_key == "opioid_od":
            med = str(row.get("eMedications.03", "")).upper()
            rr = row.get("RR", None)
            imp = str(row.get("eSituation.11", "")).upper()
            return ("NALOXONE" in med and (pd.notna(rr) and rr <= 10)) or ("OPIOID" in imp)
        if condition_key == "tension_ptx_proxy":
            proc = str(row.get("eProcedures.03", "")).upper()
            imp = str(row.get("eSituation.11", "")).upper()
            mech = str(row.get("MECHANISM", "")).upper()
            return ("NEEDLE" in proc and "THORAC" in proc and "PNEUMOTHORAX" in imp and ("BLUNT" in mech or "PENETRAT" in mech))
        if condition_key == "ob_delivery_pph_nrp":
            proc = str(row.get("eProcedures.03", "")).upper()
            nrp = bool(row.get("NRP_STEPS", False))
            pph = bool(row.get("PPH_FLAG", False))
            return (("DELIVERY" in proc) or nrp) and pph
        if condition_key == "ohca":
            arrest = bool(row.get("eArrest.01", False))
            return arrest
        return False

    def logic_proxy(row):
        if condition_key == "chf_ape":
            med = str(row.get("eMedications.03", "")).upper()
            proc = str(row.get("eProcedures.03", "")).upper()
            cc = str(row.get("eSituation.10", ""))
            return ("CPAP" in proc or "NIV" in proc) or ("NITRO" in med and contains_any(cc, ["shortness of breath", "dyspnea"]))
        if condition_key == "opioid_od":
            med = str(row.get("eMedications.03", "")).upper()
            return "NALOXONE" in med
        if condition_key == "tension_ptx_proxy":
            proc = str(row.get("eProcedures.03", "")).upper()
            mech = str(row.get("MECHANISM", "")).upper()
            return ("NEEDLE" in proc and "THORAC" in proc) and ("BLUNT" in mech or "PENETRAT" in mech)
        if condition_key == "ob_delivery_pph_nrp":
            proc = str(row.get("eProcedures.03", "")).upper()
            nrp = bool(row.get("NRP_STEPS", False))
            return ("DELIVERY" in proc) or nrp
        if condition_key == "ohca":
            return bool(row.get("eArrest.01", False))
        return False

    strict = df.apply(logic_strict, axis=1)
    proxy = df.apply(logic_proxy, axis=1)
    return strict, proxy


def main(years):
    defs = load_yaml("research/codebooks/case-definitions.yaml")
    logs = []
    conditions = ["chf_ape", "opioid_od", "tension_ptx_proxy", "ob_delivery_pph_nrp", "ohca"]
    for y in years:
        in_path = f"research/outputs/nemsis/clean__nemsis_v3_{y}.parquet"
        if not os.path.exists(in_path):
            logs.append(f"MISSING {in_path}")
            continue
        df = pd.read_parquet(in_path)
        for cond in conditions:
            strict, proxy = select_condition(df, cond, defs)
            sel = df.loc[strict | proxy].copy()
            sel["is_strict"] = strict[strict | proxy]
            out_path = f"research/outputs/nemsis/select__{cond}__{y}.parquet"
            os.makedirs(os.path.dirname(out_path), exist_ok=True)
            sel.to_parquet(out_path, index=False)
            logs.append(f"{y}/{cond}: strict={strict.sum()} proxy_only={(proxy & ~strict).sum()} total={(strict | proxy).sum()}")
    write_log("nemsis_select", logs)


if __name__ == "__main__":
    ys = [int(a) for a in sys.argv[1:]] if len(sys.argv) > 1 else list(range(2019, 2025))
    main(ys)
def main():
    # Placeholder: implement strict/proxy selection per codebooks
    pass


if __name__ == "__main__":
    main()


