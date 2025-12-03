using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
#endif

namespace ParamedicSimulator
{
    /// <summary>
    /// Controls Unity Recorder to capture animated sequences to video.
    /// </summary>
    public class SequenceRecorder : MonoBehaviour
    {
        [Header("Recording Settings")]
        [Tooltip("Output directory for rendered videos")]
        public string outputDirectory = "Art/Source/3D/Sequences/output";

        [Tooltip("Sequence name (e.g., 'initial_assessment')")]
        public string sequenceName = "initial_assessment";

        [Tooltip("Resolution width")]
        public int resolutionWidth = 1280;

        [Tooltip("Resolution height")]
        public int resolutionHeight = 720;

        [Tooltip("Frame rate for recording")]
        public int frameRate = 30;

        [Tooltip("Duration in seconds to record")]
        public float recordDuration = 9.5f;

#if UNITY_EDITOR
        private RecorderController recorderController;
        private MovieRecorderSettings movieSettings;
        private bool recordingCompleted = false;
#endif

        void Start()
        {
#if UNITY_EDITOR
            // Set up recorder
            SetupRecorder();

            // Start recording immediately
            StartRecording();
#else
            Debug.LogWarning("[SequenceRecorder] Unity Recorder only works in Editor mode");
#endif
        }

#if UNITY_EDITOR
        void SetupRecorder()
        {
            // Create recorder controller settings
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            controllerSettings.SetRecordModeToFrameInterval(0, (int)(recordDuration * frameRate));
            controllerSettings.FrameRate = frameRate;

            // Create movie recorder settings
            movieSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            movieSettings.name = $"{sequenceName}_recorder";
            movieSettings.Enabled = true;

            // Set output file path
            string fullPath = Path.Combine(Application.dataPath, "..", outputDirectory);
            Directory.CreateDirectory(fullPath);
            movieSettings.OutputFile = Path.Combine(fullPath, sequenceName);

            // Set up camera input
            var cameraInput = new CameraInputSettings
            {
                Source = ImageSource.MainCamera,
                OutputWidth = resolutionWidth,
                OutputHeight = resolutionHeight,
                CaptureUI = false
            };
            movieSettings.ImageInputSettings = cameraInput;

            // Set frame rate
            movieSettings.FrameRate = frameRate;
            movieSettings.FrameRatePlayback = FrameRatePlayback.Constant;

            // Add recorder to controller settings
            controllerSettings.AddRecorderSettings(movieSettings);

            // Create controller with settings
            recorderController = new RecorderController(controllerSettings);

            Debug.Log($"[SequenceRecorder] Recorder configured: {sequenceName}.mp4");
            Debug.Log($"[SequenceRecorder] Output: {fullPath}");
            Debug.Log($"[SequenceRecorder] Resolution: {resolutionWidth}x{resolutionHeight} @ {frameRate}fps");
            Debug.Log($"[SequenceRecorder] Duration: {recordDuration}s ({(int)(recordDuration * frameRate)} frames)");
        }

        public void StartRecording()
        {
            if (recorderController != null)
            {
                recorderController.PrepareRecording();
                recorderController.StartRecording();
                Debug.Log("[SequenceRecorder] Recording started");
            }
        }

        void LateUpdate()
        {
            // Guard against re-entry after recording completes
            if (recordingCompleted)
            {
                return;
            }

            // Check if recording is complete
            if (recorderController != null && recorderController.IsRecording())
            {
                // Recording in progress
            }
            else if (recorderController != null)
            {
                // Recording finished
                recordingCompleted = true;
                OnRecordingComplete();
            }
        }

        void OnRecordingComplete()
        {
            Debug.Log("[SequenceRecorder] Recording complete!");
            Debug.Log($"[SequenceRecorder] Video saved: {movieSettings.OutputFile}.mp4");

            // In editor, stop playing
            UnityEditor.EditorApplication.isPlaying = false;
        }

        void OnDestroy()
        {
            if (recorderController != null)
            {
                recorderController.StopRecording();
            }
        }
#endif
    }
}
