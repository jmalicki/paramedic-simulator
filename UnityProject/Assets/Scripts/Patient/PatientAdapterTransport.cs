using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace ParamedicSimulator.Patient
{
    /// <summary>
    /// HTTP/JSON transport for Patient Adapter with timeout, retry, and circuit breaker.
    /// Targets localhost service by default.
    /// </summary>
    public class PatientAdapterTransport : IPatientAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly RetryPolicy _retryPolicy;
        private readonly CircuitBreaker _circuitBreaker;

        private const int DefaultTimeoutSeconds = 5;
        private const int MaxRetries = 3;
        private const float BaseBackoffSeconds = 0.1f;

        public bool IsAvailable => _circuitBreaker.State != CircuitBreakerState.Open;

        public PatientAdapterTransport(string baseUrl = "http://localhost:5000")
        {
            _baseUrl = baseUrl;
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(DefaultTimeoutSeconds)
            };

            _retryPolicy = new RetryPolicy
            {
                MaxRetries = MaxRetries,
                BaseBackoffSeconds = BaseBackoffSeconds
            };

            _circuitBreaker = new CircuitBreaker
            {
                FailureThreshold = 5,
                SuccessThreshold = 2,
                TimeoutSeconds = 30f
            };
        }

        public async Task<PatientStateResponse> RequestPatientStateUpdateAsync(
            PatientStateRequest request,
            CancellationToken cancellationToken = default)
        {
            if (!IsAvailable)
            {
                return CreateFallbackResponse(request, "Circuit breaker is open");
            }

            try
            {
                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    return await _circuitBreaker.ExecuteAsync(async () =>
                    {
                        var json = JsonConvert.SerializeObject(request);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var url = $"{_baseUrl}/api/v1/patient/state";

                        var response = await _httpClient.PostAsync(url, content, cancellationToken);
                        response.EnsureSuccessStatusCode();

                        var responseJson = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<PatientStateResponse>(responseJson);

                        return result ?? CreateFallbackResponse(request, "Invalid response format");
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Patient adapter request failed: {ex.Message}");
                _circuitBreaker.RecordFailure();
                return CreateFallbackResponse(request, ex.Message);
            }
        }

        public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var url = $"{_baseUrl}/api/v1/health";
                var response = await _httpClient.GetAsync(url, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private PatientStateResponse CreateFallbackResponse(PatientStateRequest request, string error)
        {
            // Fallback: return current state with minimal viable vitals
            return new PatientStateResponse
            {
                Version = "v0.1",
                RequestId = request.RequestId,
                Timestamp = DateTime.UtcNow.ToString("o"),
                Success = false,
                ErrorMessage = error,
                UpdatedState = request.CurrentState ?? CreateDefaultState(),
                ModelVersion = "fallback-v0.1"
            };
        }

        private PatientState CreateDefaultState()
        {
            return new PatientState
            {
                HeartRateBpm = 70f,
                SystolicBpMmHg = 120f,
                DiastolicBpMmHg = 80f,
                RespiratoryRateBpm = 16f,
                SpO2Percent = 98f,
                TemperatureCelsius = 37f,
                GlasgowComaScale = 15
            };
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    /// <summary>
    /// Retry policy with exponential backoff
    /// </summary>
    internal class RetryPolicy
    {
        public int MaxRetries { get; set; }
        public float BaseBackoffSeconds { get; set; }

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            Exception lastException = null;

            for (int attempt = 0; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    return await operation();
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    if (attempt < MaxRetries)
                    {
                        var backoff = BaseBackoffSeconds * Mathf.Pow(2f, attempt);
                        await Task.Delay(TimeSpan.FromSeconds(backoff));
                    }
                }
            }

            throw lastException ?? new Exception("Retry policy failed");
        }
    }

    /// <summary>
    /// Circuit breaker states
    /// </summary>
    internal enum CircuitBreakerState
    {
        Closed,
        Open,
        HalfOpen
    }

    /// <summary>
    /// Circuit breaker pattern implementation
    /// </summary>
    internal class CircuitBreaker
    {
        public int FailureThreshold { get; set; } = 5;
        public int SuccessThreshold { get; set; } = 2;
        public float TimeoutSeconds { get; set; } = 30f;

        private CircuitBreakerState _state = CircuitBreakerState.Closed;
        private int _failureCount = 0;
        private int _successCount = 0;
        private DateTime _lastFailureTime;

        public CircuitBreakerState State => _state;

        public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            if (_state == CircuitBreakerState.Open)
            {
                if (DateTime.UtcNow - _lastFailureTime > TimeSpan.FromSeconds(TimeoutSeconds))
                {
                    _state = CircuitBreakerState.HalfOpen;
                    _successCount = 0;
                }
                else
                {
                    throw new Exception("Circuit breaker is open");
                }
            }

            try
            {
                var result = await operation();
                RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                RecordFailure();
                throw;
            }
        }

        public void RecordSuccess()
        {
            _failureCount = 0;

            if (_state == CircuitBreakerState.HalfOpen)
            {
                _successCount++;
                if (_successCount >= SuccessThreshold)
                {
                    _state = CircuitBreakerState.Closed;
                }
            }
        }

        public void RecordFailure()
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;

            if (_failureCount >= FailureThreshold)
            {
                _state = CircuitBreakerState.Open;
            }
            else if (_state == CircuitBreakerState.HalfOpen)
            {
                _state = CircuitBreakerState.Open;
            }
        }
    }
}
