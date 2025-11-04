from math import sqrt


def poisson_ci(count: int, exposure: float, alpha: float = 0.05):
    """Approximate Poisson CI for rate = count/exposure."""
    if exposure <= 0:
        return (0.0, 0.0)
    rate = count / exposure
    se = sqrt(count) / exposure if count > 0 else 1.0 / exposure
    z = 1.96 if alpha == 0.05 else 1.96
    return (max(0.0, rate - z * se), max(0.0, rate + z * se))


def wilson_ci(successes: int, n: int, alpha: float = 0.05):
    """Wilson score interval for proportion successes/n."""
    if n <= 0:
        return (0.0, 0.0)
    z = 1.96 if alpha == 0.05 else 1.96
    p = successes / n
    denom = 1 + z * z / n
    center = (p + z * z / (2 * n)) / denom
    margin = (z * sqrt((p * (1 - p) + z * z / (4 * n)) / n)) / denom
    return (max(0.0, center - margin), min(1.0, center + margin))


