using System;
using UnityEngine;

namespace ParamedicSimulator.Core
{
    /// <summary>
    /// Seeded random number generator for deterministic simulation.
    /// Serializes seed for replay/telemetry.
    /// </summary>
    public class SeededRandom
    {
        private System.Random _random;
        private int _seed;
        private ulong _callCount = 0;

        /// <summary>
        /// Current seed value
        /// </summary>
        public int Seed => _seed;

        /// <summary>
        /// Number of random calls made (for telemetry)
        /// </summary>
        public ulong CallCount => _callCount;

        /// <summary>
        /// Create a new seeded RNG with the given seed
        /// </summary>
        public SeededRandom(int seed)
        {
            _seed = seed;
            _random = new System.Random(seed);
        }

        /// <summary>
        /// Generate a random integer
        /// </summary>
        public int Next()
        {
            _callCount++;
            return _random.Next();
        }

        /// <summary>
        /// Generate a random integer less than maxValue
        /// </summary>
        public int Next(int maxValue)
        {
            _callCount++;
            return _random.Next(maxValue);
        }

        /// <summary>
        /// Generate a random integer in the range [minValue, maxValue)
        /// </summary>
        public int Next(int minValue, int maxValue)
        {
            _callCount++;
            return _random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Generate a random double between 0.0 and 1.0
        /// </summary>
        public double NextDouble()
        {
            _callCount++;
            return _random.NextDouble();
        }

        /// <summary>
        /// Generate a random float between 0.0f and 1.0f
        /// </summary>
        public float NextFloat()
        {
            _callCount++;
            return (float)_random.NextDouble();
        }

        /// <summary>
        /// Generate a random float in the range [min, max]
        /// </summary>
        public float Range(float min, float max)
        {
            _callCount++;
            return min + (float)_random.NextDouble() * (max - min);
        }

        /// <summary>
        /// Reset the RNG to its initial state
        /// </summary>
        public void Reset()
        {
            _random = new System.Random(_seed);
            _callCount = 0;
        }

        /// <summary>
        /// Get telemetry data for serialization
        /// </summary>
        public SeededRandomTelemetry GetTelemetry()
        {
            return new SeededRandomTelemetry
            {
                Seed = _seed,
                CallCount = _callCount
            };
        }
    }

    /// <summary>
    /// Telemetry data for seeded RNG
    /// </summary>
    [Serializable]
    public class SeededRandomTelemetry
    {
        public int Seed;
        public ulong CallCount;
    }
}
