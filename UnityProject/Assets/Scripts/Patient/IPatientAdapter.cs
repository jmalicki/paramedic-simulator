using System.Threading;
using System.Threading.Tasks;

namespace ParamedicSimulator.Patient
{
    /// <summary>
    /// Interface for patient state adapter. Allows swapping implementations
    /// (in-process, gRPC, HTTP) without changing gameplay code.
    /// </summary>
    public interface IPatientAdapter
    {
        /// <summary>
        /// Request patient state update
        /// </summary>
        /// <param name="request">Patient state request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Patient state response</returns>
        Task<PatientStateResponse> RequestPatientStateUpdateAsync(
            PatientStateRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Whether the adapter is available (service reachable)
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Health check
        /// </summary>
        Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default);
    }
}
