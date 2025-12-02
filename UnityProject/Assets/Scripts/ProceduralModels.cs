using UnityEngine;

namespace ParamedicSimulator
{
    /// <summary>
    /// Procedural 3D model creation for medical simulation sequences.
    /// Ported from Blender: Art/Source/3D/Sequences/common/models.py
    /// </summary>
    public static class ProceduralModels
    {
        // =============================================================================
        // PATIENT ARM
        // =============================================================================

        /// <summary>
        /// Create a patient arm model (forearm + hand + fingers).
        /// </summary>
        /// <param name="skinTone">Skin tone key: "skin_light", "skin_medium", "skin_dark"</param>
        /// <returns>Root forearm GameObject (other parts parented to it)</returns>
        public static GameObject CreatePatientArm(string skinTone = "skin_light")
        {
            var skinColor = ToonMaterials.GetSkinColor(skinTone);
            var skinMat = ToonMaterials.CreateToonMaterial($"MAT_Skin_{skinTone}", skinColor);

            // Root object for the arm
            var armRoot = new GameObject("PatientArm");

            // Forearm (cylinder)
            var forearm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            forearm.name = "PatientForearm";
            forearm.transform.SetParent(armRoot.transform);
            forearm.transform.localPosition = Vector3.zero;
            forearm.transform.localRotation = Quaternion.Euler(0, 0, 90); // Horizontal
            forearm.transform.localScale = new Vector3(0.08f, 0.125f, 0.08f); // radius=0.04, depth=0.25
            forearm.GetComponent<Renderer>().material = skinMat;

            // Hand (cube)
            var hand = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hand.name = "PatientHand";
            hand.transform.SetParent(armRoot.transform);
            hand.transform.localPosition = new Vector3(0.15f, 0, 0);
            hand.transform.localScale = new Vector3(0.096f, 0.064f, 0.032f); // 0.08 * (1.2, 0.8, 0.4)
            hand.GetComponent<Renderer>().material = skinMat;

            // Fingers
            Vector3[] fingerPositions = new Vector3[]
            {
                new Vector3(0.22f, -0.025f, 0.01f),  // Index
                new Vector3(0.24f, 0f, 0.01f),       // Middle
                new Vector3(0.22f, 0.025f, 0.01f),   // Ring
                new Vector3(0.18f, 0.045f, 0.005f),  // Pinky
            };

            for (int i = 0; i < fingerPositions.Length; i++)
            {
                var finger = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                finger.name = $"PatientFinger_{i}";
                finger.transform.SetParent(armRoot.transform);
                finger.transform.localPosition = fingerPositions[i];
                finger.transform.localRotation = Quaternion.Euler(0, 0, 90); // Horizontal
                finger.transform.localScale = new Vector3(0.016f, 0.03f, 0.016f); // radius=0.008, depth=0.06
                finger.GetComponent<Renderer>().material = skinMat;
            }

            // Thumb
            var thumb = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            thumb.name = "PatientThumb";
            thumb.transform.SetParent(armRoot.transform);
            thumb.transform.localPosition = new Vector3(0.17f, -0.04f, -0.015f);
            thumb.transform.localRotation = Quaternion.Euler(30, 70, 0);
            thumb.transform.localScale = new Vector3(0.02f, 0.0225f, 0.02f); // radius=0.01, depth=0.045
            thumb.GetComponent<Renderer>().material = skinMat;

            return armRoot;
        }

        // =============================================================================
        // GLOVED HAND
        // =============================================================================

        /// <summary>
        /// Create a gloved hand model.
        /// </summary>
        /// <param name="nameSuffix">"L" or "R" for left/right</param>
        /// <param name="position">Initial position</param>
        /// <param name="gloveColor">Glove color key: "glove_blue", "glove_purple"</param>
        /// <returns>Palm GameObject (fingers parented to it)</returns>
        public static GameObject CreateGlovedHand(
            string nameSuffix = "L",
            Vector3? position = null,
            string gloveColor = "glove_blue")
        {
            var pos = position ?? new Vector3(0, -0.15f, 0.08f);
            var color = ToonMaterials.GetGloveColor(gloveColor);
            var gloveMat = ToonMaterials.CreateToonMaterial($"MAT_Glove_{gloveColor}", color);

            // Palm (cube)
            var palm = GameObject.CreatePrimitive(PrimitiveType.Cube);
            palm.name = $"GlovedHand_{nameSuffix}";
            palm.transform.position = pos;
            palm.transform.localScale = new Vector3(0.048f, 0.08f, 0.024f); // 0.08 * (0.6, 1.0, 0.3)
            palm.GetComponent<Renderer>().material = gloveMat;

            // Fingers
            float[] fingerOffsets = { -0.02f, -0.007f, 0.007f, 0.02f };
            for (int i = 0; i < fingerOffsets.Length; i++)
            {
                var finger = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                finger.name = $"GloveFinger_{nameSuffix}_{i}";
                finger.transform.SetParent(palm.transform);
                finger.transform.position = new Vector3(
                    pos.x + fingerOffsets[i],
                    pos.y - 0.05f,
                    pos.z
                );
                finger.transform.localRotation = Quaternion.Euler(90, 0, 0);
                finger.transform.localScale = new Vector3(0.016f, 0.025f, 0.016f); // radius=0.008, depth=0.05
                finger.GetComponent<Renderer>().material = gloveMat;
            }

            // Thumb
            var thumb = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            thumb.name = $"GloveThumb_{nameSuffix}";
            thumb.transform.SetParent(palm.transform);
            thumb.transform.position = new Vector3(
                pos.x - 0.035f,
                pos.y + 0.02f,
                pos.z - 0.02f
            );
            thumb.transform.localRotation = Quaternion.Euler(45, 30, 0);
            thumb.transform.localScale = new Vector3(0.018f, 0.02f, 0.018f); // radius=0.009, depth=0.04
            thumb.GetComponent<Renderer>().material = gloveMat;

            return palm;
        }

        // =============================================================================
        // UPPER ARM (for BP cuff)
        // =============================================================================

        /// <summary>
        /// Create a patient upper arm (for BP cuff placement).
        /// </summary>
        public static GameObject CreatePatientUpperArm(string skinTone = "skin_light")
        {
            var skinColor = ToonMaterials.GetSkinColor(skinTone);
            var skinMat = ToonMaterials.CreateToonMaterial($"MAT_Skin_{skinTone}", skinColor);

            var upperArm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            upperArm.name = "PatientUpperArm";
            upperArm.transform.position = new Vector3(-0.15f, 0, 0);
            upperArm.transform.rotation = Quaternion.Euler(0, 0, 90); // Horizontal
            upperArm.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // radius=0.05, depth=0.20
            upperArm.GetComponent<Renderer>().material = skinMat;

            return upperArm;
        }

        // =============================================================================
        // PULSE OXIMETER
        // =============================================================================

        /// <summary>
        /// Create a pulse oximeter model.
        /// </summary>
        /// <param name="startPosition">Initial position (off-screen, held by hand)</param>
        /// <returns>Root body GameObject</returns>
        public static GameObject CreatePulseOximeter(Vector3? startPosition = null)
        {
            var pos = startPosition ?? new Vector3(0.3f, -0.3f, 0.15f);

            // Body (top clip)
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "PulseOx_Body";
            body.transform.position = pos;
            body.transform.localScale = new Vector3(0.06f, 0.036f, 0.024f); // 0.03 * (2.0, 1.2, 0.8)

            var bodyMat = ToonMaterials.CreateToonMaterial("MAT_PulseOx_Body", ToonMaterials.Colors.PulseOxBody);
            body.GetComponent<Renderer>().material = bodyMat;

            // Screen
            var screen = GameObject.CreatePrimitive(PrimitiveType.Quad);
            screen.name = "PulseOx_Screen";
            screen.transform.SetParent(body.transform);
            screen.transform.localPosition = new Vector3(0, -0.02f / 0.036f, 0.5f); // Relative to scaled body
            screen.transform.localRotation = Quaternion.Euler(0, 0, 0);
            screen.transform.localScale = new Vector3(0.625f, 0.7f, 1f); // 0.025 * (1.5, 1.0) / body scale

            var screenMat = ToonMaterials.CreateEmissiveMaterial(
                "MAT_PulseOx_Screen",
                ToonMaterials.Colors.PulseOxScreenOn,
                1.0f
            );
            screen.GetComponent<Renderer>().material = screenMat;

            // Bottom clip
            var clip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            clip.name = "PulseOx_Clip";
            clip.transform.SetParent(body.transform);
            clip.transform.localPosition = new Vector3(0, 0, -0.7f); // Below body
            clip.transform.localScale = new Vector3(0.833f, 0.7f, 0.4f); // Relative scale

            var clipMat = ToonMaterials.CreateToonMaterial("MAT_PulseOx_Clip", ToonMaterials.Colors.PulseOxClip);
            clip.GetComponent<Renderer>().material = clipMat;

            return body;
        }

        // =============================================================================
        // BP CUFF
        // =============================================================================

        /// <summary>
        /// Create a blood pressure cuff model.
        /// </summary>
        public static GameObject CreateBpCuff(Vector3? startPosition = null)
        {
            var pos = startPosition ?? new Vector3(0.3f, -0.3f, 0.15f);

            // Cuff body
            var cuff = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cuff.name = "BpCuff_Body";
            cuff.transform.position = pos;
            cuff.transform.localScale = new Vector3(0.12f, 0.06f, 0.02f);

            var cuffMat = ToonMaterials.CreateToonMaterial("MAT_BpCuff_Body", ToonMaterials.Colors.BpCuffBlue);
            cuff.GetComponent<Renderer>().material = cuffMat;

            // Bladder (inside wrap)
            var bladder = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bladder.name = "BpCuff_Bladder";
            bladder.transform.SetParent(cuff.transform);
            bladder.transform.localPosition = new Vector3(0, 0, -0.6f);
            bladder.transform.localScale = new Vector3(0.8f, 0.8f, 0.5f);

            var bladderMat = ToonMaterials.CreateToonMaterial("MAT_BpCuff_Bladder", ToonMaterials.Colors.BpCuffBladder);
            bladder.GetComponent<Renderer>().material = bladderMat;

            // Gauge
            var gauge = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            gauge.name = "BpCuff_Gauge";
            gauge.transform.SetParent(cuff.transform);
            gauge.transform.localPosition = new Vector3(0.6f, 0, 0);
            gauge.transform.localScale = new Vector3(0.3f, 0.1f, 0.3f);

            var gaugeMat = ToonMaterials.CreateToonMaterial("MAT_BpCuff_Gauge", ToonMaterials.Colors.BpGauge);
            gauge.GetComponent<Renderer>().material = gaugeMat;

            // Bulb
            var bulb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bulb.name = "BpCuff_Bulb";
            bulb.transform.SetParent(cuff.transform);
            bulb.transform.localPosition = new Vector3(0.6f, -0.8f, 0);
            bulb.transform.localScale = new Vector3(0.2f, 0.3f, 0.2f);

            var bulbMat = ToonMaterials.CreateToonMaterial("MAT_BpCuff_Bulb", ToonMaterials.Colors.BpBulb);
            bulb.GetComponent<Renderer>().material = bulbMat;

            return cuff;
        }
    }
}
