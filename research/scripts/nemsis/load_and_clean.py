import sys
import os
import pandas as pd
from research.scripts.utils.logging import write_log


def main(years):
    logs = []
    for y in years:
        in_path = f"research/inputs/nemsis/nemsis_v3_{y}.parquet"
        if not os.path.exists(in_path):
            logs.append(f"MISSING {in_path}")
            continue
        df = pd.read_parquet(in_path)
        logs.append(f"{y}: rows={len(df)} cols={len(df.columns)}")
        out_path = f"research/outputs/nemsis/clean__nemsis_v3_{y}.parquet"
        os.makedirs(os.path.dirname(out_path), exist_ok=True)
        df.to_parquet(out_path, index=False)
        logs.append(f"WROTE {out_path}")
    write_log("nemsis_clean", logs)


if __name__ == "__main__":
    ys = [int(a) for a in sys.argv[1:]] if len(sys.argv) > 1 else list(range(2019, 2025))
    main(ys)


