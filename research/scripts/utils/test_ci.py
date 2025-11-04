"""Unit tests for confidence interval calculations in ci.py"""
import pytest
import sys
from pathlib import Path

# Add parent directory to path for imports
sys.path.insert(0, str(Path(__file__).parent))

from ci import poisson_ci, wilson_ci


def test_poisson_ci_zero_count():
    """Test Poisson CI with zero count returns (0, 0)"""
    result = poisson_ci(0, 100.0)
    assert result == (0.0, 0.0)


def test_poisson_ci_zero_exposure():
    """Test Poisson CI with zero exposure returns (0, 0)"""
    result = poisson_ci(10, 0.0)
    assert result == (0.0, 0.0)


def test_poisson_ci_positive():
    """Test Poisson CI with positive count and exposure"""
    # count=10, exposure=1000, rate=0.01
    result = poisson_ci(10, 1000.0)
    assert len(result) == 2
    assert result[0] >= 0.0
    assert result[1] >= result[0]
    assert result[0] <= 0.01 <= result[1]


def test_poisson_ci_large_count():
    """Test Poisson CI with large count"""
    result = poisson_ci(1000, 10000.0)
    assert len(result) == 2
    assert result[0] >= 0.0
    assert result[1] >= result[0]
    assert result[0] <= 0.1 <= result[1]


def test_wilson_ci_zero_n():
    """Test Wilson CI with zero n returns (0, 0)"""
    result = wilson_ci(5, 0)
    assert result == (0.0, 0.0)


def test_wilson_ci_zero_successes():
    """Test Wilson CI with zero successes"""
    result = wilson_ci(0, 100)
    assert len(result) == 2
    assert result[0] >= 0.0
    assert result[1] >= result[0]
    assert result[1] <= 1.0


def test_wilson_ci_all_successes():
    """Test Wilson CI with all successes"""
    result = wilson_ci(100, 100)
    assert len(result) == 2
    assert result[0] >= 0.0
    assert result[1] >= result[0]
    assert result[1] <= 1.0
    assert result[0] <= 1.0


def test_wilson_ci_positive():
    """Test Wilson CI with positive successes and n"""
    # 50 successes out of 100, proportion = 0.5
    result = wilson_ci(50, 100)
    assert len(result) == 2
    assert result[0] >= 0.0
    assert result[1] >= result[0]
    assert result[1] <= 1.0
    assert result[0] <= 0.5 <= result[1]


def test_wilson_ci_small_sample():
    """Test Wilson CI with small sample"""
    result = wilson_ci(1, 10)
    assert len(result) == 2
    assert result[0] >= 0.0
    assert result[1] >= result[0]
    assert result[1] <= 1.0


def test_wilson_ci_rare_event():
    """Test Wilson CI with rare event (low proportion)"""
    result = wilson_ci(1, 1000)
    assert len(result) == 2
    assert result[0] >= 0.0
    assert result[1] >= result[0]
    assert result[1] <= 1.0
    assert result[0] <= 0.001 <= result[1]

