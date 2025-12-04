using System.IO;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ParamedicSimulator.Editor
{
    /// <summary>
    /// Batch mode entry point for headless Unity rendering.
    /// Called via: unity -batchmode -executeMethod ParamedicSimulator.Editor.BatchModeRecorder.RecordSequence
    ///
    /// Workflow:
    /// 1. Export FBX from Blender: blender --background --python compose.py -- --fbx
    /// 2. Copy FBX to UnityProject/Assets/Models/
    /// 3. Render with Unity: ./UnityProject/render.sh sequence_name
    /// </summary>
    [InitializeOnLoad]
    public static class BatchModeRecorder
    {
        private const string ModelsPath = "Assets/Models";
        private static RecorderController s_recorderController;
        private static string s_expectedOutputPath;

        // Session state keys for persisting across domain reload
        private const string kRecordingActiveKey = "BatchModeRecorder_Active";
        private const string kSequenceNameKey = "BatchModeRecorder_Sequence";
        private const string kOutputDirKey = "BatchModeRecorder_OutputDir";
        private const string kWidthKey = "BatchModeRecorder_Width";
        private const string kHeightKey = "BatchModeRecorder_Height";

        // Static constructor - runs after every domain reload
        static BatchModeRecorder()
        {
            // Check if we were in the middle of recording when domain reloaded
            if (
                SessionState.GetBool(kRecordingActiveKey, false)
                && EditorApplication.isPlayingOrWillChangePlaymode
            )
            {
                Debug.Log(
                    "[BatchModeRecorder] Domain reloaded during recording, re-initializing..."
                );
                EditorApplication.delayCall += ResumeRecording;
            }
        }

        private static void ResumeRecording()
        {
            if (!EditorApplication.isPlaying)
            {
                Debug.Log("[BatchModeRecorder] Waiting for play mode...");
                EditorApplication.delayCall += ResumeRecording;
                return;
            }

            string sequenceName = SessionState.GetString(kSequenceNameKey, "initial_assessment");
            string outputDir = SessionState.GetString(
                kOutputDirKey,
                "Art/Source/3D/Sequences/output"
            );
            int width = SessionState.GetInt(kWidthKey, 1280);
            int height = SessionState.GetInt(kHeightKey, 720);

            Debug.Log($"[BatchModeRecorder] Resuming recording for: {sequenceName}");

            // After domain reload, the scene content may need to be recreated
            // Check if we have a main camera and content
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.Log("[BatchModeRecorder] Main camera not found, recreating scene content...");
                RecreateSceneContent(sequenceName);
            }
            else
            {
                Debug.Log(
                    $"[BatchModeRecorder] Main camera found at {mainCamera.transform.position}"
                );

                // Verify there's something to render
                var renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
                Debug.Log($"[BatchModeRecorder] Found {renderers.Length} renderers in scene");

                if (renderers.Length == 0)
                {
                    Debug.Log(
                        "[BatchModeRecorder] No renderers found, recreating scene content..."
                    );
                    RecreateSceneContent(sequenceName);
                }
            }

            // Re-setup recorder after domain reload
            SetupRecorder(sequenceName, outputDir, width, height);
            StartRecording();
        }

        private static void RecreateSceneContent(string sequenceName)
        {
            // Find or create main camera
            var cameraGO = GameObject.Find("MainCamera");
            if (cameraGO == null)
            {
                cameraGO = new GameObject("MainCamera");
                var camera = cameraGO.AddComponent<Camera>();
                camera.tag = "MainCamera";
                camera.transform.position = new Vector3(0, 0.5f, -0.3f);
                camera.transform.rotation = Quaternion.Euler(45, 0, 0);
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = ToonMaterials.Colors.Background;
                camera.fieldOfView = 60f;
                camera.nearClipPlane = 0.01f;
                camera.farClipPlane = 100f;
                Debug.Log("[BatchModeRecorder] Created main camera");
            }

            // Check if lights exist
            if (Object.FindFirstObjectByType<Light>() == null)
            {
                CreateLights();
                Debug.Log("[BatchModeRecorder] Created lights");
            }

            // Load FBX or create procedural content
            bool fbxLoaded = TryLoadFbx(sequenceName, cameraGO);
            if (!fbxLoaded)
            {
                Debug.Log("[BatchModeRecorder] No FBX found, using procedural content");
                CreateProceduralContent(sequenceName, cameraGO);
            }

            Debug.Log($"[BatchModeRecorder] Scene content recreated for: {sequenceName}");
        }

        /// <summary>
        /// Main entry point for batch mode recording.
        /// Reads command line arguments and starts recording.
        /// </summary>
        public static void RecordSequence()
        {
            Debug.Log("====================================");
            Debug.Log("[BatchModeRecorder] Starting batch mode recording");
            Debug.Log("====================================");

            // Parse command line arguments
            string[] args = System.Environment.GetCommandLineArgs();
            string sequenceName = "initial_assessment";
            string outputDir = "Art/Source/3D/Sequences/output";
            int width = 1280;
            int height = 720;

            // Parse arguments
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--sequence" && i + 1 < args.Length)
                {
                    sequenceName = args[i + 1];
                }
                else if (args[i] == "--output" && i + 1 < args.Length)
                {
                    outputDir = args[i + 1];
                }
                else if (args[i] == "--resolution" && i + 1 < args.Length)
                {
                    string[] res = args[i + 1].Split('x');
                    if (res.Length == 2)
                    {
                        int.TryParse(res[0], out width);
                        int.TryParse(res[1], out height);
                    }
                }
            }

            Debug.Log($"[BatchModeRecorder] Sequence: {sequenceName}");
            Debug.Log($"[BatchModeRecorder] Output: {outputDir}");
            Debug.Log($"[BatchModeRecorder] Resolution: {width}x{height}");

            // Create or load the recording scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Set up the scene (without the SequenceRecorder component - we handle recording here)
            SetupRecordingScene(sequenceName, outputDir, width, height);

            // Save scene temporarily
            string tempScenePath = "Assets/Scenes/TempRecordingScene.unity";
            Directory.CreateDirectory(Path.GetDirectoryName(tempScenePath));
            EditorSceneManager.SaveScene(scene, tempScenePath);

            // Save session state so we can resume after domain reload
            SessionState.SetBool(kRecordingActiveKey, true);
            SessionState.SetString(kSequenceNameKey, sequenceName);
            SessionState.SetString(kOutputDirKey, outputDir);
            SessionState.SetInt(kWidthKey, width);
            SessionState.SetInt(kHeightKey, height);

            // Unity Recorder requires play mode to capture frames
            // Domain reload happens when entering play mode, so we save state above
            // and resume in the static constructor after reload
            Debug.Log("[BatchModeRecorder] Starting play mode for recording...");
            Debug.Log("[BatchModeRecorder] (Recording will resume after domain reload)");

            // Enter play mode - this triggers domain reload
            EditorApplication.isPlaying = true;
        }

        private static int s_warmupFrames = 0;
        private const int kWarmupFrameCount = 5; // Wait a few frames before recording

        private static void StartRecording()
        {
            Debug.Log("[BatchModeRecorder] Starting recorder...");

            // Log scene state before recording
            LogSceneState();

            if (s_recorderController != null)
            {
                s_recorderController.PrepareRecording();

                // Wait a few frames for the scene to stabilize before recording
                s_warmupFrames = 0;
                EditorApplication.update += WarmupBeforeRecording;
            }
            else
            {
                Debug.LogError("[BatchModeRecorder] RecorderController is null!");
                ClearSessionState();
                EditorApplication.Exit(1);
            }
        }

        private static void WarmupBeforeRecording()
        {
            s_warmupFrames++;
            if (s_warmupFrames < kWarmupFrameCount)
            {
                return; // Wait for more frames
            }

            EditorApplication.update -= WarmupBeforeRecording;

            Debug.Log(
                $"[BatchModeRecorder] Scene warmed up for {kWarmupFrameCount} frames, starting recording..."
            );
            LogSceneState();

            s_recorderController.StartRecording();
            Debug.Log("[BatchModeRecorder] Recording started!");

            // Check completion every frame
            EditorApplication.update += CheckRecordingComplete;
        }

        private static void LogSceneState()
        {
            Debug.Log("[BatchModeRecorder] === SCENE STATE DEBUG ===");

            // Check render pipeline
            var currentRP = UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline;
            Debug.Log(
                $"[BatchModeRecorder] Render Pipeline: {(currentRP != null ? currentRP.name : "NULL (using Built-in)")}"
            );

            var mainCamera = Camera.main;
            if (mainCamera != null)
            {
                Debug.Log(
                    $"[BatchModeRecorder] Camera: pos={mainCamera.transform.position}, rot={mainCamera.transform.eulerAngles}, fov={mainCamera.fieldOfView}"
                );
                Debug.Log(
                    $"[BatchModeRecorder] Camera: clearFlags={mainCamera.clearFlags}, bgColor={mainCamera.backgroundColor}"
                );
                Debug.Log(
                    $"[BatchModeRecorder] Camera: cullingMask={mainCamera.cullingMask}, nearClip={mainCamera.nearClipPlane}, farClip={mainCamera.farClipPlane}"
                );
                Debug.Log(
                    $"[BatchModeRecorder] Camera: enabled={mainCamera.enabled}, gameObject.active={mainCamera.gameObject.activeInHierarchy}"
                );
            }
            else
            {
                Debug.LogWarning("[BatchModeRecorder] No main camera found!");
            }

            var renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            Debug.Log($"[BatchModeRecorder] Scene has {renderers.Length} renderers");
            foreach (var r in renderers)
            {
                var mat = r.sharedMaterial;
                string matInfo =
                    mat != null
                        ? $"mat={mat.name}, shader={(mat.shader != null ? mat.shader.name : "NULL")}"
                        : "mat=NULL";
                Debug.Log(
                    $"[BatchModeRecorder]   - {r.gameObject.name}: bounds={r.bounds.center}, visible={r.isVisible}, enabled={r.enabled}, layer={r.gameObject.layer}, {matInfo}"
                );
            }

            var lights = Object.FindObjectsByType<Light>(FindObjectsSortMode.None);
            Debug.Log($"[BatchModeRecorder] Scene has {lights.Length} lights");
            foreach (var l in lights)
            {
                Debug.Log(
                    $"[BatchModeRecorder]   - {l.gameObject.name}: type={l.type}, intensity={l.intensity}, enabled={l.enabled}"
                );
            }

            Debug.Log("[BatchModeRecorder] === END SCENE STATE ===");
        }

        private static void ClearSessionState()
        {
            SessionState.SetBool(kRecordingActiveKey, false);
            SessionState.EraseBool(kRecordingActiveKey);
            SessionState.EraseString(kSequenceNameKey);
            SessionState.EraseString(kOutputDirKey);
            SessionState.EraseInt(kWidthKey);
            SessionState.EraseInt(kHeightKey);
        }

        private static void CheckRecordingComplete()
        {
            if (s_recorderController == null)
                return;

            s_frameCount++;

            // Show progress every 30 frames (1 second at 30fps)
            if (s_frameCount % 30 == 0 || s_frameCount == 1)
            {
                float progress = (float)s_frameCount / s_totalFrames;
                int barWidth = 30;
                int filled = (int)(progress * barWidth);
                string bar = new string('#', filled) + new string('-', barWidth - filled);
                float remainingSeconds = (s_totalFrames - s_frameCount) / 30f;

                string progressMsg =
                    $"[{bar}] {progress * 100:F0}% ({s_frameCount}/{s_totalFrames}) ETA: {remainingSeconds:F0}s";

                // Log progress - this will show in unity_render.log and stdout
                Debug.Log($"[BatchModeRecorder] {progressMsg}");
            }

            if (!s_recorderController.IsRecording())
            {
                Debug.Log("[BatchModeRecorder] Recording complete!");
                EditorApplication.update -= CheckRecordingComplete;

                // Clear session state
                ClearSessionState();

                // Verify output file exists
                if (File.Exists(s_expectedOutputPath))
                {
                    var fileInfo = new FileInfo(s_expectedOutputPath);
                    Debug.Log(
                        $"[BatchModeRecorder] Video saved: {s_expectedOutputPath} ({fileInfo.Length / 1024}KB)"
                    );
                }
                else
                {
                    Debug.LogWarning(
                        $"[BatchModeRecorder] Expected output not found: {s_expectedOutputPath}"
                    );
                }

                // Exit play mode first
                EditorApplication.isPlaying = false;

                // Then exit Unity (delayed to allow cleanup)
                if (Application.isBatchMode)
                {
                    Debug.Log("[BatchModeRecorder] Batch mode - exiting Unity");
                    EditorApplication.delayCall += () => EditorApplication.Exit(0);
                }
            }
        }

        private static int s_frameCount = 0;
        private static int s_totalFrames = 0;

        private static void SetupRecorder(
            string sequenceName,
            string outputDir,
            int width,
            int height
        )
        {
            float duration = ProcedureInfo.GetDuration(sequenceName);
            int frameRate = 30;
            s_totalFrames = (int)(duration * frameRate);
            s_frameCount = 0;

            // Create controller settings
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            controllerSettings.SetRecordModeToFrameInterval(0, s_totalFrames);
            controllerSettings.FrameRate = frameRate;

            // Create movie recorder settings
            var movieSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            movieSettings.name = $"{sequenceName}_recorder";
            movieSettings.Enabled = true;

            // Use WebM encoder (MP4/H.264 not supported on Linux)
            movieSettings.EncoderSettings = new UnityEditor.Recorder.Encoder.CoreEncoderSettings
            {
                Codec = UnityEditor.Recorder.Encoder.CoreEncoderSettings.OutputCodec.WEBM,
                EncodingQuality = UnityEditor
                    .Recorder
                    .Encoder
                    .CoreEncoderSettings
                    .VideoEncodingQuality
                    .High,
            };

            // Set output file path
            // outputDir may be absolute or relative - handle both cases
            string fullPath;
            if (Path.IsPathRooted(outputDir))
            {
                fullPath = outputDir;
            }
            else
            {
                // Relative path - go up from Assets to project root, then apply outputDir
                fullPath = Path.GetFullPath(Path.Combine(Application.dataPath, "..", outputDir));
            }

            Directory.CreateDirectory(fullPath);

            // Set output file (without extension - recorder adds it)
            string outputFile = Path.Combine(fullPath, sequenceName);
            movieSettings.OutputFile = outputFile;
            s_expectedOutputPath = outputFile + ".webm";

            Debug.Log($"[BatchModeRecorder] Output file will be: {s_expectedOutputPath}");

            // Set up camera input
            var cameraInput = new CameraInputSettings
            {
                Source = ImageSource.MainCamera,
                OutputWidth = width,
                OutputHeight = height,
                CaptureUI = false,
            };
            movieSettings.ImageInputSettings = cameraInput;

            // Set frame rate
            movieSettings.FrameRate = frameRate;
            movieSettings.FrameRatePlayback = FrameRatePlayback.Constant;

            // Add recorder to controller settings
            controllerSettings.AddRecorderSettings(movieSettings);

            // Create controller
            s_recorderController = new RecorderController(controllerSettings);

            Debug.Log($"[BatchModeRecorder] Recorder configured: {sequenceName}.webm");
            Debug.Log($"[BatchModeRecorder] Output path: {fullPath}");
            Debug.Log($"[BatchModeRecorder] Resolution: {width}x{height} @ {frameRate}fps");
            Debug.Log($"[BatchModeRecorder] Duration: {duration}s ({s_totalFrames} frames)");
        }

        private static void SetupRecordingScene(
            string sequenceName,
            string outputDir,
            int width,
            int height
        )
        {
            // === Camera Setup ===
            // Blender uses Y-forward, Z-up. Unity uses Z-forward, Y-up.
            // Blender camera: location=(0, -0.5, 0.3), rotation=(75, 0, 0)
            // Unity equivalent: position=(0, 0.3, -0.5), but rotation needs adjustment
            // With 75° X rotation in Blender looking along +Y, in Unity we look along +Z
            // So the camera should look DOWN at the scene which is around origin
            var cameraGO = new GameObject("MainCamera");
            var camera = cameraGO.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0, 0.5f, -0.3f);
            camera.transform.rotation = Quaternion.Euler(45, 0, 0);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = ToonMaterials.Colors.Background;
            camera.fieldOfView = 60f;
            camera.nearClipPlane = 0.01f; // Match Blender's clip_start
            camera.farClipPlane = 100f; // Match Blender's clip_end

            // === Lighting Setup ===
            CreateLights();

            // Recording is handled by BatchModeRecorder, not SequenceRecorder component

            // === Load FBX or Create Procedural Content ===
            bool fbxLoaded = TryLoadFbx(sequenceName, cameraGO);

            if (!fbxLoaded)
            {
                Debug.Log("[BatchModeRecorder] No FBX found, using procedural content");
                CreateProceduralContent(sequenceName, cameraGO);
            }

            Debug.Log($"[BatchModeRecorder] Scene setup complete for: {sequenceName}");
        }

        /// <summary>
        /// Try to load an FBX file for the sequence.
        /// </summary>
        private static bool TryLoadFbx(string sequenceName, GameObject camera)
        {
            string fbxPath = $"{ModelsPath}/{sequenceName}.fbx";

            // Check if FBX exists
            if (!File.Exists(Path.Combine(Application.dataPath, "..", fbxPath)))
            {
                Debug.Log($"[BatchModeRecorder] FBX not found: {fbxPath}");
                return false;
            }

            Debug.Log($"[BatchModeRecorder] Loading FBX: {fbxPath}");

            // Load the FBX prefab
            var fbxAsset = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);
            if (fbxAsset == null)
            {
                Debug.LogError($"[BatchModeRecorder] Failed to load FBX: {fbxPath}");
                return false;
            }

            // Instantiate the FBX
            var instance = Object.Instantiate(fbxAsset);
            instance.name = sequenceName;

            // Apply toon materials to all renderers
            ApplyToonMaterials(instance);

            // Play animation if present
            var animation = instance.GetComponent<Animation>();
            if (animation != null && animation.clip != null)
            {
                animation.playAutomatically = true;
                Debug.Log($"[BatchModeRecorder] Animation found: {animation.clip.name}");
            }

            // Also check for Animator component (Mecanim)
            var animator = instance.GetComponent<Animator>();
            if (animator != null)
            {
                Debug.Log("[BatchModeRecorder] Animator found on FBX");
            }

            Debug.Log($"[BatchModeRecorder] FBX loaded successfully: {sequenceName}");
            return true;
        }

        /// <summary>
        /// Apply toon materials to all renderers in the hierarchy.
        /// </summary>
        private static void ApplyToonMaterials(GameObject root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>();

            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                var newMaterials = new Material[materials.Length];

                for (int i = 0; i < materials.Length; i++)
                {
                    var mat = materials[i];
                    if (mat == null)
                    {
                        // Preserve null material slots to avoid rendering issues
                        newMaterials[i] = null;
                        continue;
                    }

                    // Map Blender material names to toon materials
                    Color color = GetColorFromMaterialName(mat.name);
                    newMaterials[i] = ToonMaterials.CreateToonMaterial(mat.name + "_Toon", color);
                }

                renderer.materials = newMaterials;
            }

            Debug.Log(
                $"[BatchModeRecorder] Applied toon materials to {renderers.Length} renderers"
            );
        }

        /// <summary>
        /// Map Blender material names to colors.
        /// </summary>
        private static Color GetColorFromMaterialName(string name)
        {
            // Blender material names from materials.py
            string lowerName = name.ToLower();

            if (lowerName.Contains("glove") && lowerName.Contains("blue"))
                return ToonMaterials.Colors.GloveBlue;
            if (lowerName.Contains("glove") && lowerName.Contains("purple"))
                return ToonMaterials.Colors.GlovePurple;
            if (lowerName.Contains("skin") && lowerName.Contains("light"))
                return ToonMaterials.Colors.SkinLight;
            if (lowerName.Contains("skin") && lowerName.Contains("medium"))
                return ToonMaterials.Colors.SkinMedium;
            if (lowerName.Contains("skin") && lowerName.Contains("dark"))
                return ToonMaterials.Colors.SkinDark;
            if (lowerName.Contains("pulseox") && lowerName.Contains("body"))
                return ToonMaterials.Colors.PulseOxBody;
            if (lowerName.Contains("pulseox") && lowerName.Contains("clip"))
                return ToonMaterials.Colors.PulseOxClip;
            if (lowerName.Contains("pulseox") && lowerName.Contains("screen"))
                return ToonMaterials.Colors.PulseOxScreenOn;
            if (lowerName.Contains("bp") && lowerName.Contains("cuff"))
                return ToonMaterials.Colors.BpCuffBlue;
            if (lowerName.Contains("bladder"))
                return ToonMaterials.Colors.BpCuffBladder;
            if (lowerName.Contains("gauge"))
                return ToonMaterials.Colors.BpGauge;
            if (lowerName.Contains("bulb"))
                return ToonMaterials.Colors.BpBulb;

            // Default: white
            Debug.Log($"[BatchModeRecorder] Unknown material name: {name}, using white");
            return ToonMaterials.Colors.White;
        }

        /// <summary>
        /// Create lighting setup matching Blender's cel-shaded look.
        /// </summary>
        private static void CreateLights()
        {
            // Key light
            var keyLightGO = new GameObject("KeyLight");
            var keyLight = keyLightGO.AddComponent<Light>();
            keyLight.type = LightType.Directional;
            keyLight.intensity = 1.5f;
            keyLight.color = Color.white;
            keyLightGO.transform.rotation = Quaternion.Euler(45, 30, 0);

            // Fill light
            var fillLightGO = new GameObject("FillLight");
            var fillLight = fillLightGO.AddComponent<Light>();
            fillLight.type = LightType.Directional;
            fillLight.intensity = 0.5f;
            fillLight.color = new Color(0.9f, 0.95f, 1.0f);
            fillLightGO.transform.rotation = Quaternion.Euler(60, -20, 0);
        }

        /// <summary>
        /// Create procedural content when no FBX is available.
        /// Fallback to generated geometry.
        /// </summary>
        private static void CreateProceduralContent(string sequenceName, GameObject camera)
        {
            Debug.Log("[BatchModeRecorder] Creating procedural content (no FBX)");

            // Create patient arm
            var patientArm = ProceduralModels.CreatePatientArm("skin_light");
            patientArm.transform.position = Vector3.zero;

            // Create gloved hand
            var hand = ProceduralModels.CreateGlovedHand(
                "L",
                new Vector3(0, 0.08f, -0.15f),
                "glove_blue"
            );

            switch (sequenceName)
            {
                case "pulseox_apply":
                    var pulseox = ProceduralModels.CreatePulseOximeter();
                    ProcedureAnimations.AnimatePulseOxApply(hand, pulseox, camera, 0f);
                    break;

                case "radial_pulse":
                    ProcedureAnimations.AnimateRadialPulse(hand, camera, 0f);
                    break;

                case "bp_cuff_apply":
                    ProceduralModels.CreatePatientUpperArm("skin_light");
                    var bpCuff = ProceduralModels.CreateBpCuff();
                    ProcedureAnimations.AnimateBpCuffApply(hand, bpCuff, camera, 0f);
                    break;

                case "initial_assessment":
                default:
                    var pulseoxEquip = ProceduralModels.CreatePulseOximeter();
                    ProcedureAnimations.AnimatePulseOxApply(hand, pulseoxEquip, camera, 0f);
                    break;
            }

            Debug.Log($"[BatchModeRecorder] Created procedural content for: {sequenceName}");
        }
    }
}
