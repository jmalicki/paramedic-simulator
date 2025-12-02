using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

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
    public static class BatchModeRecorder
    {
        private const string ModelsPath = "Assets/Models";

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

            // Set up the scene
            SetupRecordingScene(sequenceName, outputDir, width, height);

            // Save scene temporarily
            string tempScenePath = "Assets/Scenes/TempRecordingScene.unity";
            Directory.CreateDirectory(Path.GetDirectoryName(tempScenePath));
            EditorSceneManager.SaveScene(scene, tempScenePath);

            // Enter play mode to start recording
            Debug.Log("[BatchModeRecorder] Entering play mode to start recording...");
            EditorApplication.isPlaying = true;
        }

        private static void SetupRecordingScene(string sequenceName, string outputDir, int width, int height)
        {
            // === Camera Setup ===
            var cameraGO = new GameObject("MainCamera");
            var camera = cameraGO.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.position = new Vector3(0, 0.3f, -0.5f);
            camera.transform.rotation = Quaternion.Euler(75, 0, 0);
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = ToonMaterials.Colors.Background;
            camera.fieldOfView = 35f;

            // === Lighting Setup ===
            CreateLights();

            // === Recorder Controller ===
            var recorderGO = new GameObject("SequenceRecorder");
            var recorder = recorderGO.AddComponent<SequenceRecorder>();
            recorder.sequenceName = sequenceName;
            recorder.outputDirectory = outputDir;
            recorder.resolutionWidth = width;
            recorder.resolutionHeight = height;
            recorder.recordDuration = ProcedureInfo.GetDuration(sequenceName);

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
                    if (mat == null) continue;

                    // Map Blender material names to toon materials
                    Color color = GetColorFromMaterialName(mat.name);
                    newMaterials[i] = ToonMaterials.CreateToonMaterial(mat.name + "_Toon", color);
                }

                renderer.materials = newMaterials;
            }

            Debug.Log($"[BatchModeRecorder] Applied toon materials to {renderers.Length} renderers");
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
            var hand = ProceduralModels.CreateGlovedHand("L", new Vector3(0, 0.08f, -0.15f), "glove_blue");

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
