using UnityEngine;

namespace ParamedicSimulator
{
    /// <summary>
    /// Color palette and material creation for cel-shaded rendering.
    /// Ported from Blender: Art/Source/3D/Sequences/common/materials.py
    /// </summary>
    public static class ToonMaterials
    {
        // =============================================================================
        // COLOR PALETTE
        // =============================================================================

        public static class Colors
        {
            // Gloves and hands
            public static readonly Color GloveBlue = new Color(0.420f, 0.545f, 0.643f, 1.0f);    // #6B8BA4
            public static readonly Color GlovePurple = new Color(0.502f, 0.388f, 0.569f, 1.0f);  // #806391

            // Skin tones
            public static readonly Color SkinLight = new Color(1.0f, 0.855f, 0.725f, 1.0f);      // #FFDAB9
            public static readonly Color SkinMedium = new Color(0.824f, 0.584f, 0.427f, 1.0f);   // #D2956D
            public static readonly Color SkinDark = new Color(0.553f, 0.333f, 0.141f, 1.0f);     // #8D5524

            // Pulse Oximeter
            public static readonly Color PulseOxBody = new Color(0.910f, 0.910f, 0.910f, 1.0f);       // #E8E8E8
            public static readonly Color PulseOxClip = new Color(0.290f, 0.290f, 0.290f, 1.0f);       // #4A4A4A
            public static readonly Color PulseOxScreenOff = new Color(0.102f, 0.102f, 0.180f, 1.0f);  // #1A1A2E
            public static readonly Color PulseOxScreenOn = new Color(0.0f, 1.0f, 0.3f, 1.0f);         // Green glow
            public static readonly Color PulseOxLedRed = new Color(1.0f, 0.0f, 0.0f, 1.0f);

            // BP Cuff
            public static readonly Color BpCuffBlue = new Color(0.180f, 0.525f, 0.670f, 1.0f);   // #2E86AB
            public static readonly Color BpCuffBladder = new Color(0.290f, 0.290f, 0.290f, 1.0f); // #4A4A4A
            public static readonly Color BpBulb = new Color(0.290f, 0.290f, 0.290f, 1.0f);       // #4A4A4A
            public static readonly Color BpGauge = new Color(0.910f, 0.910f, 0.910f, 1.0f);      // #E8E8E8

            // General
            public static readonly Color Outline = new Color(0.1f, 0.1f, 0.1f, 1.0f);
            public static readonly Color White = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            public static readonly Color Black = new Color(0.05f, 0.05f, 0.05f, 1.0f);

            // Background
            public static readonly Color Background = new Color(0.9f, 0.92f, 0.95f, 1.0f);
        }

        // =============================================================================
        // MATERIAL CREATION
        // =============================================================================

        /// <summary>
        /// Create a basic toon material with the given color.
        /// </summary>
        public static Material CreateToonMaterial(string name, Color baseColor)
        {
            // Use Standard shader for now, can be replaced with custom toon shader later
            var material = new Material(Shader.Find("Standard"));
            material.name = name;
            material.color = baseColor;

            // Make it more matte/toon-like by reducing metallic and smoothness
            material.SetFloat("_Metallic", 0f);
            material.SetFloat("_Glossiness", 0.1f);

            return material;
        }

        /// <summary>
        /// Create an emissive material for screens/LEDs.
        /// </summary>
        public static Material CreateEmissiveMaterial(string name, Color color, float strength = 2.0f)
        {
            var material = new Material(Shader.Find("Standard"));
            material.name = name;
            material.color = color;

            // Enable emission
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", color * strength);
            material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            return material;
        }

        /// <summary>
        /// Get skin color by name.
        /// </summary>
        public static Color GetSkinColor(string skinTone)
        {
            return skinTone switch
            {
                "skin_light" => Colors.SkinLight,
                "skin_medium" => Colors.SkinMedium,
                "skin_dark" => Colors.SkinDark,
                _ => Colors.SkinLight
            };
        }

        /// <summary>
        /// Get glove color by name.
        /// </summary>
        public static Color GetGloveColor(string gloveColor)
        {
            return gloveColor switch
            {
                "glove_blue" => Colors.GloveBlue,
                "glove_purple" => Colors.GlovePurple,
                _ => Colors.GloveBlue
            };
        }
    }
}
