using System;
using System.Collections.Generic;

namespace ParamedicSimulator.Patient
{
    /// <summary>
    /// Request to update patient state. All units in SI (meters, seconds, kilograms, etc.)
    /// Version: v0.1
    /// </summary>
    [Serializable]
    public class PatientStateRequest
    {
        /// <summary>
        /// Protocol version
        /// </summary>
        public string Version { get; set; } = "v0.1";

        /// <summary>
        /// Request ID for idempotency
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Timestamp of the request (ISO 8601, UTC)
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// Simulation time in seconds
        /// </summary>
        public float SimulationTime { get; set; }

        /// <summary>
        /// Session seed for determinism
        /// </summary>
        public int SessionSeed { get; set; }

        /// <summary>
        /// Current patient state (all values in SI units)
        /// </summary>
        public PatientState CurrentState { get; set; }

        /// <summary>
        /// Intent events since last update (interactions, device inputs, scenario triggers)
        /// </summary>
        public List<IntentEvent> IntentEvents { get; set; } = new List<IntentEvent>();
    }

    /// <summary>
    /// Current patient state with SI units
    /// </summary>
    [Serializable]
    public class PatientState
    {
        /// <summary>
        /// Age in years
        /// </summary>
        public float AgeYears { get; set; }

        /// <summary>
        /// Weight in kilograms
        /// </summary>
        public float WeightKg { get; set; }

        /// <summary>
        /// Height in meters
        /// </summary>
        public float HeightM { get; set; }

        /// <summary>
        /// Heart rate in beats per minute
        /// </summary>
        public float HeartRateBpm { get; set; }

        /// <summary>
        /// Systolic blood pressure in mmHg
        /// </summary>
        public float SystolicBpMmHg { get; set; }

        /// <summary>
        /// Diastolic blood pressure in mmHg
        /// </summary>
        public float DiastolicBpMmHg { get; set; }

        /// <summary>
        /// Respiratory rate in breaths per minute
        /// </summary>
        public float RespiratoryRateBpm { get; set; }

        /// <summary>
        /// Oxygen saturation as percentage (0-100)
        /// </summary>
        public float SpO2Percent { get; set; }

        /// <summary>
        /// Temperature in Celsius
        /// </summary>
        public float TemperatureCelsius { get; set; }

        /// <summary>
        /// Glasgow Coma Scale (3-15)
        /// </summary>
        public int GlasgowComaScale { get; set; }

        /// <summary>
        /// Blood glucose in mmol/L
        /// </summary>
        public float BloodGlucoseMmolL { get; set; }

        /// <summary>
        /// Additional clinical parameters (key-value pairs)
        /// </summary>
        public Dictionary<string, float> AdditionalParams { get; set; } = new Dictionary<string, float>();
    }

    /// <summary>
    /// Intent event (interaction, device input, scenario trigger)
    /// </summary>
    [Serializable]
    public class IntentEvent
    {
        /// <summary>
        /// Event type identifier
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Timestamp relative to simulation start (seconds)
        /// </summary>
        public float Timestamp { get; set; }

        /// <summary>
        /// Event data (key-value pairs)
        /// </summary>
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    }
}
