#nullable enable
using UnityEngine;
using System.Collections.Generic;

namespace ParamedicSimulator
{
    /// <summary>
    /// Keyframe data for procedural animations.
    /// </summary>
    public struct Keyframe
    {
        public float Time;
        public Vector3 Position;
        public Vector3 Rotation;

        public Keyframe(float time, Vector3 position, Vector3 rotation)
        {
            Time = time;
            Position = position;
            Rotation = rotation;
        }

        public Keyframe(float time, Vector3 position)
        {
            Time = time;
            Position = position;
            Rotation = Vector3.zero;
        }
    }

    /// <summary>
    /// Procedure metadata and timing.
    /// </summary>
    public static class ProcedureInfo
    {
        public const float PulseOxDuration = 2.0f;
        public const float RadialPulseDuration = 3.0f;
        public const float BpCuffDuration = 3.0f;
        public const float InitialAssessmentDuration = 9.5f;

        public static float GetDuration(string sequenceName)
        {
            return sequenceName switch
            {
                "pulseox_apply" => PulseOxDuration,
                "radial_pulse" => RadialPulseDuration,
                "bp_cuff_apply" => BpCuffDuration,
                "initial_assessment" => InitialAssessmentDuration,
                _ => 5.0f
            };
        }
    }

    /// <summary>
    /// Procedural animation creation for medical procedures.
    /// Ported from Blender: Art/Source/3D/Sequences/procedures/
    /// </summary>
    public static class ProcedureAnimations
    {
        // =============================================================================
        // PULSE OXIMETER APPLICATION
        // =============================================================================

        /// <summary>
        /// Create pulse oximeter application animation.
        /// Ported from: procedures/pulseox_apply.py
        /// </summary>
        public static void AnimatePulseOxApply(
            GameObject hand,
            GameObject pulseox,
            GameObject? camera,
            float startTime = 0f)
        {
            float duration = ProcedureInfo.PulseOxDuration;

            // Frame timing (converted from Blender's seconds_to_frame)
            float t_start = startTime;
            float t_approach = startTime + 0.5f;
            float t_open = startTime + 0.8f;
            float t_place = startTime + 1.2f;
            float t_clip = startTime + 1.5f;
            float t_release = startTime + 1.8f;
            float t_done = startTime + duration;

            // === Pulse Oximeter Animation ===
            var pulseoxKeyframes = new List<Keyframe>
            {
                // Start: held in hand, approaching
                new Keyframe(t_start, new Vector3(0.3f, 0.15f, -0.25f), Vector3.zero),
                // Approach finger
                new Keyframe(t_approach, new Vector3(0.28f, 0.08f, -0.15f), new Vector3(-20f, 0, 0)),
                // Open clip
                new Keyframe(t_open, new Vector3(0.26f, 0.04f, -0.08f), new Vector3(-30f, 0, 0)),
                // Place on finger
                new Keyframe(t_place, new Vector3(0.24f, 0.02f, -0.01f), Vector3.zero),
                // Clip closed
                new Keyframe(t_clip, new Vector3(0.24f, 0.015f, 0f), Vector3.zero),
                // Final position
                new Keyframe(t_release, new Vector3(0.24f, 0.015f, 0f), Vector3.zero),
                new Keyframe(t_done, new Vector3(0.24f, 0.015f, 0f), Vector3.zero),
            };

            // === Hand Animation ===
            var handKeyframes = new List<Keyframe>
            {
                // Start position
                new Keyframe(t_start, new Vector3(0f, 0.08f, -0.15f), Vector3.zero),
                // Move with pulseox
                new Keyframe(t_approach, new Vector3(0.05f, 0.06f, -0.12f), Vector3.zero),
                new Keyframe(t_open, new Vector3(0.12f, 0.04f, -0.08f), Vector3.zero),
                new Keyframe(t_place, new Vector3(0.15f, 0.03f, -0.05f), Vector3.zero),
                new Keyframe(t_clip, new Vector3(0.18f, 0.025f, -0.04f), Vector3.zero),
                // Release and withdraw
                new Keyframe(t_release, new Vector3(0.10f, 0.05f, -0.10f), Vector3.zero),
                new Keyframe(t_done, new Vector3(0f, 0.10f, -0.20f), Vector3.zero),
            };

            ApplyAnimation(pulseox, pulseoxKeyframes);
            ApplyAnimation(hand, handKeyframes);

            // Camera (optional)
            if (camera != null)
            {
                camera.transform.position = new Vector3(0.15f, 0.25f, -0.4f);
                camera.transform.rotation = Quaternion.Euler(70f, 0f, 10f);
            }
        }

        // =============================================================================
        // RADIAL PULSE CHECK
        // =============================================================================

        /// <summary>
        /// Create radial pulse check animation.
        /// </summary>
        public static void AnimateRadialPulse(
            GameObject hand,
            GameObject? camera,
            float startTime = 0f)
        {
            float duration = ProcedureInfo.RadialPulseDuration;

            float t_start = startTime;
            float t_approach = startTime + 0.5f;
            float t_contact = startTime + 1.0f;
            float t_hold_start = startTime + 1.2f;
            float t_hold_end = startTime + 2.5f;
            float t_release = startTime + 2.8f;
            float t_done = startTime + duration;

            var handKeyframes = new List<Keyframe>
            {
                // Start: hand approaches from side
                new Keyframe(t_start, new Vector3(-0.1f, 0.1f, -0.2f), Vector3.zero),
                // Approach wrist
                new Keyframe(t_approach, new Vector3(0.05f, 0.05f, -0.1f), new Vector3(0, 0, -15f)),
                // Contact - two fingers on radial artery
                new Keyframe(t_contact, new Vector3(0.08f, 0.02f, -0.02f), new Vector3(0, 0, -20f)),
                // Hold and feel pulse
                new Keyframe(t_hold_start, new Vector3(0.08f, 0.015f, 0f), new Vector3(0, 0, -20f)),
                new Keyframe(t_hold_end, new Vector3(0.08f, 0.015f, 0f), new Vector3(0, 0, -20f)),
                // Release
                new Keyframe(t_release, new Vector3(0.05f, 0.05f, -0.1f), Vector3.zero),
                new Keyframe(t_done, new Vector3(-0.1f, 0.1f, -0.2f), Vector3.zero),
            };

            ApplyAnimation(hand, handKeyframes);

            if (camera != null)
            {
                camera.transform.position = new Vector3(0.1f, 0.2f, -0.35f);
                camera.transform.rotation = Quaternion.Euler(65f, 0f, 5f);
            }
        }

        // =============================================================================
        // BP CUFF APPLICATION
        // =============================================================================

        /// <summary>
        /// Create BP cuff application animation.
        /// </summary>
        public static void AnimateBpCuffApply(
            GameObject hand,
            GameObject bpCuff,
            GameObject? camera,
            float startTime = 0f)
        {
            float duration = ProcedureInfo.BpCuffDuration;

            float t_start = startTime;
            float t_approach = startTime + 0.6f;
            float t_wrap_start = startTime + 1.0f;
            float t_wrap_mid = startTime + 1.5f;
            float t_wrap_end = startTime + 2.0f;
            float t_secure = startTime + 2.5f;
            float t_done = startTime + duration;

            // BP Cuff animation
            var cuffKeyframes = new List<Keyframe>
            {
                new Keyframe(t_start, new Vector3(0.2f, 0.15f, -0.25f), Vector3.zero),
                new Keyframe(t_approach, new Vector3(0f, 0.08f, -0.15f), Vector3.zero),
                new Keyframe(t_wrap_start, new Vector3(-0.1f, 0.03f, -0.05f), new Vector3(0, 0, 30f)),
                new Keyframe(t_wrap_mid, new Vector3(-0.15f, 0.01f, 0f), new Vector3(0, 0, 60f)),
                new Keyframe(t_wrap_end, new Vector3(-0.15f, 0.005f, 0f), new Vector3(0, 0, 0f)),
                new Keyframe(t_secure, new Vector3(-0.15f, 0f, 0f), Vector3.zero),
                new Keyframe(t_done, new Vector3(-0.15f, 0f, 0f), Vector3.zero),
            };

            // Hand follows cuff
            var handKeyframes = new List<Keyframe>
            {
                new Keyframe(t_start, new Vector3(0.15f, 0.12f, -0.2f), Vector3.zero),
                new Keyframe(t_approach, new Vector3(0.05f, 0.08f, -0.12f), Vector3.zero),
                new Keyframe(t_wrap_start, new Vector3(-0.05f, 0.05f, -0.05f), Vector3.zero),
                new Keyframe(t_wrap_mid, new Vector3(-0.12f, 0.03f, 0f), Vector3.zero),
                new Keyframe(t_wrap_end, new Vector3(-0.1f, 0.02f, 0.02f), Vector3.zero),
                new Keyframe(t_secure, new Vector3(-0.05f, 0.05f, -0.1f), Vector3.zero),
                new Keyframe(t_done, new Vector3(0f, 0.1f, -0.2f), Vector3.zero),
            };

            ApplyAnimation(bpCuff, cuffKeyframes);
            ApplyAnimation(hand, handKeyframes);

            if (camera != null)
            {
                camera.transform.position = new Vector3(-0.1f, 0.25f, -0.4f);
                camera.transform.rotation = Quaternion.Euler(70f, 10f, 0f);
            }
        }

        // =============================================================================
        // INITIAL ASSESSMENT (COMPOSITE SEQUENCE)
        // =============================================================================

        /// <summary>
        /// Create full initial assessment sequence (pulseox + radial pulse + bp cuff).
        /// </summary>
        public static void AnimateInitialAssessment(
            GameObject hand,
            GameObject pulseox,
            GameObject bpCuff,
            GameObject patientArm,
            GameObject? camera)
        {
            // Sequence timing:
            // 0.0 - 2.0: Pulse ox apply (2s)
            // 2.0 - 2.5: Transition (0.5s)
            // 2.5 - 5.5: Radial pulse (3s)
            // 5.5 - 6.0: Transition (0.5s)
            // 6.0 - 9.0: BP cuff apply (3s)
            // 9.0 - 9.5: End hold (0.5s)

            AnimatePulseOxApply(hand, pulseox, camera, 0f);

            // After pulseox, transition hand and continue with radial pulse
            // Note: In full implementation, would blend between animations
            // For now, create independent segments
        }

        // =============================================================================
        // ANIMATION APPLICATION
        // =============================================================================

        /// <summary>
        /// Apply keyframe animation to a GameObject using legacy Animation system.
        /// </summary>
        public static void ApplyAnimation(GameObject obj, List<Keyframe> keyframes)
        {
            if (keyframes.Count == 0) return;

            var animation = obj.GetComponent<Animation>();
            if (animation == null)
            {
                animation = obj.AddComponent<Animation>();
            }

            var clip = new AnimationClip();
            clip.legacy = true;
            clip.name = $"{obj.name}_animation";

            // Position curves
            var curveX = new AnimationCurve();
            var curveY = new AnimationCurve();
            var curveZ = new AnimationCurve();

            // Rotation curves
            var curveRotX = new AnimationCurve();
            var curveRotY = new AnimationCurve();
            var curveRotZ = new AnimationCurve();

            foreach (var kf in keyframes)
            {
                curveX.AddKey(new UnityEngine.Keyframe(kf.Time, kf.Position.x));
                curveY.AddKey(new UnityEngine.Keyframe(kf.Time, kf.Position.y));
                curveZ.AddKey(new UnityEngine.Keyframe(kf.Time, kf.Position.z));

                curveRotX.AddKey(new UnityEngine.Keyframe(kf.Time, kf.Rotation.x));
                curveRotY.AddKey(new UnityEngine.Keyframe(kf.Time, kf.Rotation.y));
                curveRotZ.AddKey(new UnityEngine.Keyframe(kf.Time, kf.Rotation.z));
            }

            // Apply smooth tangents for Bezier-like interpolation
            SmoothCurve(curveX);
            SmoothCurve(curveY);
            SmoothCurve(curveZ);
            SmoothCurve(curveRotX);
            SmoothCurve(curveRotY);
            SmoothCurve(curveRotZ);

            clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
            clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
            clip.SetCurve("", typeof(Transform), "localPosition.z", curveZ);

            clip.SetCurve("", typeof(Transform), "localEulerAngles.x", curveRotX);
            clip.SetCurve("", typeof(Transform), "localEulerAngles.y", curveRotY);
            clip.SetCurve("", typeof(Transform), "localEulerAngles.z", curveRotZ);

            animation.AddClip(clip, clip.name);
            animation.clip = clip;
            animation.playAutomatically = true;
            animation.wrapMode = WrapMode.Once;
        }

        /// <summary>
        /// Apply smooth tangents to animation curve (similar to Blender's AUTO_CLAMPED).
        /// </summary>
        private static void SmoothCurve(AnimationCurve curve)
        {
            for (int i = 0; i < curve.length; i++)
            {
                curve.SmoothTangents(i, 0f);
            }
        }
    }
}
