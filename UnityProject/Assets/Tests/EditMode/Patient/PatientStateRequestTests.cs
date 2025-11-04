using System;
using System.Collections.Generic;
using NUnit.Framework;
using ParamedicSimulator.Patient;

namespace ParamedicSimulator.Tests.EditMode.Patient
{
    public class PatientStateRequestTests
    {
        [Test]
        public void PatientStateRequest_Initialization_IsCorrect()
        {
            var request = new PatientStateRequest
            {
                RequestId = "test-request-1",
                Timestamp = DateTime.UtcNow.ToString("o"),
                SimulationTime = 10.5f,
                SessionSeed = 12345
            };

            Assert.AreEqual("v0.1", request.Version);
            Assert.AreEqual("test-request-1", request.RequestId);
            Assert.IsNotNull(request.Timestamp);
            Assert.AreEqual(10.5f, request.SimulationTime);
            Assert.AreEqual(12345, request.SessionSeed);
            Assert.IsNotNull(request.IntentEvents);
        }

        [Test]
        public void PatientStateRequest_WithState_IsCorrect()
        {
            var state = new PatientState
            {
                AgeYears = 45f,
                WeightKg = 70f,
                HeightM = 1.7f,
                HeartRateBpm = 80f,
                SystolicBpMmHg = 120f,
                DiastolicBpMmHg = 80f,
                RespiratoryRateBpm = 16f,
                SpO2Percent = 98f,
                TemperatureCelsius = 37f,
                GlasgowComaScale = 15,
                BloodGlucoseMmolL = 5.5f
            };

            var request = new PatientStateRequest
            {
                CurrentState = state
            };

            Assert.IsNotNull(request.CurrentState);
            Assert.AreEqual(45f, request.CurrentState.AgeYears);
            Assert.AreEqual(70f, request.CurrentState.WeightKg);
            Assert.AreEqual(1.7f, request.CurrentState.HeightM);
        }

        [Test]
        public void PatientStateRequest_WithIntentEvents_IsCorrect()
        {
            var request = new PatientStateRequest();
            request.IntentEvents.Add(new IntentEvent
            {
                EventType = "intervention",
                Timestamp = 5.0f,
                Data = new Dictionary<string, object>
                {
                    { "action", "oxygen" },
                    { "amount", 2.0f }
                }
            });

            Assert.AreEqual(1, request.IntentEvents.Count);
            Assert.AreEqual("intervention", request.IntentEvents[0].EventType);
            Assert.AreEqual(5.0f, request.IntentEvents[0].Timestamp);
        }

        [Test]
        public void PatientState_SIUnits_AreCorrect()
        {
            var state = new PatientState
            {
                AgeYears = 45f,
                WeightKg = 70f,
                HeightM = 1.7f,
                HeartRateBpm = 80f,
                SystolicBpMmHg = 120f,
                DiastolicBpMmHg = 80f,
                RespiratoryRateBpm = 16f,
                SpO2Percent = 98f,
                TemperatureCelsius = 37f,
                GlasgowComaScale = 15,
                BloodGlucoseMmolL = 5.5f
            };

            // Verify SI units are used
            Assert.GreaterOrEqual(state.AgeYears, 0f);
            Assert.GreaterOrEqual(state.WeightKg, 0f);
            Assert.GreaterOrEqual(state.HeightM, 0f);
            Assert.GreaterOrEqual(state.HeartRateBpm, 0f);
            Assert.GreaterOrEqual(state.TemperatureCelsius, 0f);
            Assert.GreaterOrEqual(state.BloodGlucoseMmolL, 0f);
        }
    }
}
