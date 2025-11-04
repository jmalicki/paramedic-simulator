using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using ParamedicSimulator.Core;

namespace ParamedicSimulator.Tests.EditMode.Core
{
    public class SimulationClockTests
    {
        private GameObject _gameObject;
        private SimulationClock _clock;
        private int _stepCount;
        private float _lastStepTime;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject("SimulationClock");
            _clock = _gameObject.AddComponent<SimulationClock>();
            _stepCount = 0;
            _lastStepTime = 0f;
            _clock.OnSimulationStep += OnStep;
        }

        [TearDown]
        public void TearDown()
        {
            if (_clock != null)
            {
                _clock.OnSimulationStep -= OnStep;
            }
            if (_gameObject != null)
            {
                UnityEngine.Object.DestroyImmediate(_gameObject);
            }
        }

        private void OnStep(float timestep, ulong tick)
        {
            _stepCount++;
            _lastStepTime = _clock.SimulationTime;
        }

        [Test]
        public void SimulationClock_InitialState_IsCorrect()
        {
            Assert.AreEqual(0f, _clock.SimulationTime);
            Assert.AreEqual(0UL, _clock.TickCount);
            Assert.AreEqual(1.0f, _clock.SpeedMultiplier);
            Assert.IsFalse(_clock.IsPaused);
        }

        [Test]
        public void SimulationClock_SetSpeed_AcceptsValidValues()
        {
            _clock.SetSpeed(0.5f);
            Assert.AreEqual(0.5f, _clock.SpeedMultiplier);

            _clock.SetSpeed(1.0f);
            Assert.AreEqual(1.0f, _clock.SpeedMultiplier);

            _clock.SetSpeed(2.0f);
            Assert.AreEqual(2.0f, _clock.SpeedMultiplier);
        }

        [Test]
        public void SimulationClock_Pause_StopsTime()
        {
            _clock.Pause();
            Assert.IsTrue(_clock.IsPaused);
        }

        [Test]
        public void SimulationClock_Resume_ContinuesTime()
        {
            _clock.Pause();
            _clock.Resume();
            Assert.IsFalse(_clock.IsPaused);
        }

        [Test]
        public void SimulationClock_TogglePause_ChangesState()
        {
            bool initialState = _clock.IsPaused;
            _clock.TogglePause();
            Assert.AreNotEqual(initialState, _clock.IsPaused);
        }

        [Test]
        public void SimulationClock_Reset_ResetsState()
        {
            _clock.Reset();
            Assert.AreEqual(0f, _clock.SimulationTime);
            Assert.AreEqual(0UL, _clock.TickCount);
        }

        [Test]
        public void SimulationClock_FixedTimestep_IsCorrect()
        {
            Assert.AreEqual(0.02f, SimulationClock.FixedTimestep);
        }
    }
}
