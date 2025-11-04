using System;
using System.Threading;
using System.Threading.Tasks;

namespace ParamedicSimulator.Patient
{
    /// <summary>
    /// In-process patient adapter for direct model execution (no network).
    /// Implements fallback rules-based state transitions.
    /// </summary>
    public class InProcessPatientAdapter : IPatientAdapter
    {
        private readonly SeededRandom _rng;

        public bool IsAvailable => true;

        public InProcessPatientAdapter(SeededRandom rng)
        {
            _rng = rng ?? throw new ArgumentNullException(nameof(rng));
        }

        public Task<PatientStateResponse> RequestPatientStateUpdateAsync(
            PatientStateRequest request,
            CancellationToken cancellationToken = default)
        {
            // Simulate in-process model execution
            var updatedState = ApplyRulesBasedTransition(request.CurrentState, request.IntentEvents);

            var response = new PatientStateResponse
            {
                Version = "v0.1",
                RequestId = request.RequestId,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Success = true,
                UpdatedState = updatedState,
                ModelVersion = "in-process-rules-v0.1"
            };

            return Task.FromResult(response);
        }

        public Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        private PatientState ApplyRulesBasedTransition(PatientState currentState, System.Collections.Generic.List<IntentEvent> events)
        {
            // Simple rules-based fallback for essential vitals
            var updated = new PatientState
            {
                AgeYears = currentState?.AgeYears ?? 45f,
                WeightKg = currentState?.WeightKg ?? 70f,
                HeightM = currentState?.HeightM ?? 1.7f,
                HeartRateBpm = currentState?.HeartRateBpm ?? 70f,
                SystolicBpMmHg = currentState?.SystolicBpMmHg ?? 120f,
                DiastolicBpMmHg = currentState?.DiastolicBpMmHg ?? 80f,
                RespiratoryRateBpm = currentState?.RespiratoryRateBpm ?? 16f,
                SpO2Percent = currentState?.SpO2Percent ?? 98f,
                TemperatureCelsius = currentState?.TemperatureCelsius ?? 37f,
                GlasgowComaScale = currentState?.GlasgowComaScale ?? 15,
                BloodGlucoseMmolL = currentState?.BloodGlucoseMmolL ?? 5.5f,
                AdditionalParams = currentState?.AdditionalParams ?? new System.Collections.Generic.Dictionary<string, float>()
            };

            // Apply minimal transitions based on events (placeholder for actual model)
            foreach (var evt in events)
            {
                // Placeholder: actual model would apply sophisticated transitions
                if (evt.EventType == "intervention")
                {
                    // Example: intervention improves vitals slightly
                    updated.HeartRateBpm = Mathf.Max(60f, updated.HeartRateBpm - 5f);
                    updated.SpO2Percent = Mathf.Min(100f, updated.SpO2Percent + 2f);
                }
            }

            return updated;
        }
    }
}
