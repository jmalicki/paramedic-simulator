using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using ParamedicSimulator.Core;
using ParamedicSimulator.Patient;

namespace ParamedicSimulator.Tests.EditMode.Patient
{
    public class PatientAdapterTests
    {
        private SeededRandom _rng;
        private InProcessPatientAdapter _adapter;

        [SetUp]
        public void SetUp()
        {
            _rng = new SeededRandom(12345);
            _adapter = new InProcessPatientAdapter(_rng);
        }

        [Test]
        public void InProcessPatientAdapter_IsAvailable_ReturnsTrue()
        {
            Assert.IsTrue(_adapter.IsAvailable);
        }

        [Test]
        public async Task InProcessPatientAdapter_RequestUpdate_ReturnsResponse()
        {
            var request = new PatientStateRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow.ToString("o"),
                SimulationTime = 0f,
                SessionSeed = 12345,
                CurrentState = new PatientState
                {
                    HeartRateBpm = 80f,
                    SpO2Percent = 95f
                }
            };

            var response = await _adapter.RequestPatientStateUpdateAsync(request);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual("v0.1", response.Version);
            Assert.IsNotNull(response.UpdatedState);
        }

        [Test]
        public async Task InProcessPatientAdapter_HealthCheck_ReturnsTrue()
        {
            var result = await _adapter.HealthCheckAsync();
            Assert.IsTrue(result);
        }

        [Test]
        public async Task InProcessPatientAdapter_AdapterIsSwappable()
        {
            // Test that adapter interface allows swapping implementations
            IPatientAdapter adapter1 = new InProcessPatientAdapter(_rng);
            IPatientAdapter adapter2 = new InProcessPatientAdapter(new SeededRandom(67890));

            var request = new PatientStateRequest
            {
                RequestId = Guid.NewGuid().ToString(),
                Timestamp = DateTime.UtcNow.ToString("o"),
                SimulationTime = 0f,
                SessionSeed = 12345
            };

            var response1 = await adapter1.RequestPatientStateUpdateAsync(request);
            var response2 = await adapter2.RequestPatientStateUpdateAsync(request);

            Assert.IsNotNull(response1);
            Assert.IsNotNull(response2);
            // Both should succeed, demonstrating swappability
            Assert.IsTrue(response1.Success);
            Assert.IsTrue(response2.Success);
        }
    }
}
