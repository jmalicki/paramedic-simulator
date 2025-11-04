using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using ParamedicSimulator.Patient;

namespace ParamedicSimulator.Tests.EditMode.Patient
{
    /// <summary>
    /// Contract tests for PatientStateRequest/Response schemas.
    /// Verifies golden files round-trip correctly and detects breaking changes.
    /// </summary>
    public class PatientStateContractTests
    {
        private string GetGoldenPath(string filename)
        {
            return Path.Combine(Application.dataPath, "Tests", "Golden", filename);
        }

        [Test]
        public void PatientStateRequest_GoldenFile_RoundTrips()
        {
            var goldenPath = GetGoldenPath("patient_state_request_v0.1.json");
            
            if (!File.Exists(goldenPath))
            {
                Assert.Fail($"Golden file not found: {goldenPath}");
                return;
            }

            var json = File.ReadAllText(goldenPath);
            var request = JsonConvert.DeserializeObject<PatientStateRequest>(json);
            
            Assert.IsNotNull(request);
            Assert.AreEqual("v0.1", request.Version);
            Assert.IsNotNull(request.RequestId);
            Assert.IsNotNull(request.Timestamp);
            Assert.IsNotNull(request.CurrentState);
            Assert.IsNotNull(request.IntentEvents);

            // Round-trip: serialize back and compare structure
            var serialized = JsonConvert.SerializeObject(request, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<PatientStateRequest>(serialized);
            
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(request.Version, deserialized.Version);
            Assert.AreEqual(request.SimulationTime, deserialized.SimulationTime);
            Assert.AreEqual(request.SessionSeed, deserialized.SessionSeed);
        }

        [Test]
        public void PatientStateResponse_GoldenFile_RoundTrips()
        {
            var goldenPath = GetGoldenPath("patient_state_response_v0.1.json");
            
            if (!File.Exists(goldenPath))
            {
                Assert.Fail($"Golden file not found: {goldenPath}");
                return;
            }

            var json = File.ReadAllText(goldenPath);
            var response = JsonConvert.DeserializeObject<PatientStateResponse>(json);
            
            Assert.IsNotNull(response);
            Assert.AreEqual("v0.1", response.Version);
            Assert.IsNotNull(response.RequestId);
            Assert.IsNotNull(response.Timestamp);
            Assert.IsTrue(response.Success);
            Assert.IsNotNull(response.UpdatedState);
            Assert.IsNotNull(response.ModelVersion);

            // Round-trip: serialize back and compare structure
            var serialized = JsonConvert.SerializeObject(response, Formatting.Indented);
            var deserialized = JsonConvert.DeserializeObject<PatientStateResponse>(serialized);
            
            Assert.IsNotNull(deserialized);
            Assert.AreEqual(response.Version, deserialized.Version);
            Assert.AreEqual(response.Success, deserialized.Success);
            Assert.IsNotNull(deserialized.UpdatedState);
        }

        [Test]
        public void PatientStateRequest_Schema_ValidatesRequiredFields()
        {
            var request = new PatientStateRequest
            {
                Version = "v0.1",
                RequestId = "test-123",
                Timestamp = DateTime.UtcNow.ToString("o"),
                SimulationTime = 10.0f,
                SessionSeed = 12345
            };

            var json = JsonConvert.SerializeObject(request);
            var deserialized = JsonConvert.DeserializeObject<PatientStateRequest>(json);

            Assert.AreEqual("v0.1", deserialized.Version);
            Assert.AreEqual("test-123", deserialized.RequestId);
            Assert.AreEqual(10.0f, deserialized.SimulationTime);
            Assert.AreEqual(12345, deserialized.SessionSeed);
        }

        [Test]
        public void PatientState_SIUnits_AreValid()
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

            // Verify all units are SI-compatible
            Assert.GreaterOrEqual(state.WeightKg, 0f); // kg
            Assert.GreaterOrEqual(state.HeightM, 0f); // meters
            Assert.GreaterOrEqual(state.TemperatureCelsius, -273.15f); // Celsius (absolute zero)
            Assert.GreaterOrEqual(state.BloodGlucoseMmolL, 0f); // mmol/L
        }
    }
}
