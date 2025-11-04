import os
import pandas as pd
import json
from pathlib import Path


def read_any(path: str) -> pd.DataFrame:
    """Read CSV or Parquet file, automatically detecting format."""
    if path.endswith(".parquet"):
        return pd.read_parquet(path)
    if path.endswith(".csv"):
        return pd.read_csv(path)
    raise ValueError(f"Unsupported file type: {path}")


def write_csv(df: pd.DataFrame, path: str, schema_path: str = None):
    """
    Write DataFrame to CSV with optional schema preservation.
    
    Args:
        df: DataFrame to write
        path: Output CSV path
        schema_path: Optional path to save schema JSON (if None, uses {path}.schema.json)
    """
    os.makedirs(os.path.dirname(path), exist_ok=True)
    df.to_csv(path, index=False)
    
    # Save schema information
    if schema_path is None:
        schema_path = str(Path(path).with_suffix('.csv.schema.json'))
    
    schema = {
        'columns': list(df.columns),
        'dtypes': {col: str(dtype) for col, dtype in df.dtypes.items()},
        'shape': list(df.shape),
        'null_counts': df.isnull().sum().to_dict()
    }
    
    with open(schema_path, 'w') as f:
        json.dump(schema, f, indent=2)


