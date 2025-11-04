using System;
using UnityEngine;

namespace ParamedicSimulator.Core
{
    /// <summary>
    /// Fixed-step simulation clock with pause, speed control (0.5×/1×/2×), and single-writer updates.
    /// Deterministic: uses fixed timestep instead of Time.deltaTime.
    /// </summary>
    public class SimulationClock : MonoBehaviour
    {
        /// <summary>
        /// Fixed timestep in seconds (50 Hz = 0.02s per step)
        /// </summary>
        public const float FixedTimestep = 0.02f;

        /// <summary>
        /// Speed multiplier: 0.5×, 1.0×, or 2.0×
        /// </summary>
        private float _speedMultiplier = 1.0f;

        /// <summary>
        /// Whether the simulation is paused
        /// </summary>
        private bool _isPaused = false;

        /// <summary>
        /// Accumulated time since last update
        /// </summary>
        private float _accumulator = 0f;

        /// <summary>
        /// Current simulation time in seconds
        /// </summary>
        private float _simulationTime = 0f;

        /// <summary>
        /// Current simulation tick count
        /// </summary>
        private ulong _tickCount = 0;

        /// <summary>
        /// Event fired once per fixed simulation step
        /// </summary>
        public event Action<float, ulong> OnSimulationStep;

        /// <summary>
        /// Current simulation time in seconds
        /// </summary>
        public float SimulationTime => _simulationTime;

        /// <summary>
        /// Current tick count
        /// </summary>
        public ulong TickCount => _tickCount;

        /// <summary>
        /// Current speed multiplier
        /// </summary>
        public float SpeedMultiplier => _speedMultiplier;

        /// <summary>
        /// Whether the simulation is paused
        /// </summary>
        public bool IsPaused => _isPaused;

        private void Update()
        {
            if (_isPaused)
                return;

            // Accumulate real-time delta scaled by speed multiplier
            _accumulator += Time.unscaledDeltaTime * _speedMultiplier;

            // Process fixed steps
            while (_accumulator >= FixedTimestep)
            {
                _accumulator -= FixedTimestep;
                _simulationTime += FixedTimestep;
                _tickCount++;

                // Fire single update event per step
                OnSimulationStep?.Invoke(FixedTimestep, _tickCount);
            }
        }

        /// <summary>
        /// Set simulation speed (0.5×, 1.0×, or 2.0×)
        /// </summary>
        public void SetSpeed(float multiplier)
        {
            if (multiplier == 0.5f || multiplier == 1.0f || multiplier == 2.0f)
            {
                _speedMultiplier = multiplier;
            }
            else
            {
                Debug.LogWarning($"Invalid speed multiplier: {multiplier}. Must be 0.5, 1.0, or 2.0");
            }
        }

        /// <summary>
        /// Pause the simulation
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Resume the simulation
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Toggle pause state
        /// </summary>
        public void TogglePause()
        {
            _isPaused = !_isPaused;
        }

        /// <summary>
        /// Reset the simulation clock
        /// </summary>
        public void Reset()
        {
            _simulationTime = 0f;
            _tickCount = 0;
            _accumulator = 0f;
        }
    }
}
