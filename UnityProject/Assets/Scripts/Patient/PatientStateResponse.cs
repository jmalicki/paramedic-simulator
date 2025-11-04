using System;

namespace ParamedicSimulator.Patient
{
    /// <summary>
    /// Response from patient model service. All units in SI.
    /// Version: v0.1
    /// </summary>
    [Serializable]
    public class PatientStateResponse
    {
        /// <summary>
        /// Protocol version
        /// </summary>
        public string Version { get; set; } = "v0.1";

        /// <summary>
        /// Request ID this response corresponds to
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Timestamp of the response (ISO 8601, UTC)
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// Whether the response is successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if Success is false
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Updated patient state (all values in SI units)
        /// </summary>
        public PatientState UpdatedState { get; set; }

        /// <summary>
        /// Model version used for generation
        /// </summary>
        public string ModelVersion { get; set; }
    }
}
