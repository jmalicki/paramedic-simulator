import os
from datetime import datetime


def log_path(step: str) -> str:
    ts = datetime.utcnow().strftime("%Y%m%dT%H%M%S")
    os.makedirs("research/outputs/logs", exist_ok=True)
    return f"research/outputs/logs/{ts}__{step}.log"


def write_log(step: str, lines: list[str]):
    path = log_path(step)
    with open(path, "w", encoding="utf-8") as f:
        for line in lines:
            f.write(str(line) + "\n")
    return path
