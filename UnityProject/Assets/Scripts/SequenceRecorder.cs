using UnityEngine;
using UnityEngine.Recorder;
using UnityEngine.Recorder.Input;
using System.IO;

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

        private RecorderController recorderController;
        private MovieRecorderSettings movieSettings;

        void Start()
        {
            // Set up recorder
            SetupRecorder();

            // Start recording immediately
            StartRecording();
        }

        void SetupRecorder()
        {
            // Create recorder controller
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            recorderController = new RecorderController(controllerSettings);

            // Create movie recorder settings
            movieSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            movieSettings.name = $"{sequenceName}_recorder";
            movieSettings.Enabled = true;

            // Set output format to MP4
            movieSettings.OutputFormat = MovieRecorderSettings.VideoRecorderOutputFormat.MP4;

            // Set output file
            string fullPath = Path.Combine(Application.dataPath, "..", outputDirectory);
            Directory.CreateDirectory(fullPath);
            movieSettings.OutputFile = Path.Combine(fullPath, sequenceName);

            // Configure video encoder
            movieSettings.VideoBitRateMode = VideoBitrateMode.High;

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

            // Add recorder to controller
            controllerSettings.AddRecorderSettings(movieSettings);
            controllerSettings.SetRecordModeToFrameInterval(0, (int)(recordDuration * frameRate));
            controllerSettings.FrameRate = frameRate;

            recorderController.Settings = controllerSettings;

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
            // Check if recording is complete
            if (recorderController != null && recorderController.IsRecording())
            {
                // Recording in progress
            }
            else if (recorderController != null && recorderController.Settings.FrameRate > 0)
            {
                // Recording finished
                OnRecordingComplete();
            }
        }

        void OnRecordingComplete()
        {
            Debug.Log("[SequenceRecorder] Recording complete!");
            Debug.Log($"[SequenceRecorder] Video saved: {movieSettings.OutputFile}.mp4");

            #if UNITY_EDITOR
            // In editor, just log completion
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            // In batch mode, quit application
            Application.Quit(0);
            #endif
        }

        void OnDestroy()
        {
            if (recorderController != null)
            {
                recorderController.StopRecording();
            }
        }
    }
}
