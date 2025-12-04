using NUnit.Framework;
using ParamedicSimulator;

namespace ParamedicSimulator.Tests.EditMode
{
    /// <summary>
    /// Tests for ProcedureInfo timing constants.
    /// </summary>
    [TestFixture]
    public class ProcedureInfoTests
    {
        [Test]
        public void DurationConstants_ArePositive()
        {
            Assert.Greater(ProcedureInfo.PulseOxDuration, 0f, "PulseOxDuration should be positive");
            Assert.Greater(
                ProcedureInfo.RadialPulseDuration,
                0f,
                "RadialPulseDuration should be positive"
            );
            Assert.Greater(ProcedureInfo.BpCuffDuration, 0f, "BpCuffDuration should be positive");
            Assert.Greater(
                ProcedureInfo.InitialAssessmentDuration,
                0f,
                "InitialAssessmentDuration should be positive"
            );
        }

        [Test]
        public void InitialAssessmentDuration_IsGreaterThanIndividualProcedures()
        {
            // Initial assessment combines all procedures, so should be longer
            Assert.Greater(
                ProcedureInfo.InitialAssessmentDuration,
                ProcedureInfo.PulseOxDuration,
                "InitialAssessment should be longer than PulseOx"
            );
            Assert.Greater(
                ProcedureInfo.InitialAssessmentDuration,
                ProcedureInfo.RadialPulseDuration,
                "InitialAssessment should be longer than RadialPulse"
            );
            Assert.Greater(
                ProcedureInfo.InitialAssessmentDuration,
                ProcedureInfo.BpCuffDuration,
                "InitialAssessment should be longer than BpCuff"
            );
        }

        [Test]
        public void GetDuration_ReturnsCorrectValuesForKnownSequences()
        {
            Assert.AreEqual(
                ProcedureInfo.PulseOxDuration,
                ProcedureInfo.GetDuration("pulseox_apply")
            );
            Assert.AreEqual(
                ProcedureInfo.RadialPulseDuration,
                ProcedureInfo.GetDuration("radial_pulse")
            );
            Assert.AreEqual(
                ProcedureInfo.BpCuffDuration,
                ProcedureInfo.GetDuration("bp_cuff_apply")
            );
            Assert.AreEqual(
                ProcedureInfo.InitialAssessmentDuration,
                ProcedureInfo.GetDuration("initial_assessment")
            );
        }

        [Test]
        public void GetDuration_ReturnsDefaultForUnknownSequence()
        {
            float defaultDuration = ProcedureInfo.GetDuration("unknown_sequence");
            Assert.AreEqual(
                5.0f,
                defaultDuration,
                "Unknown sequence should return default 5.0 seconds"
            );
        }

        [Test]
        public void DurationValues_AreReasonableForAnimations()
        {
            // Animations should be reasonable lengths (between 1 and 30 seconds)
            float minDuration = 1.0f;
            float maxDuration = 30.0f;

            Assert.IsTrue(
                ProcedureInfo.PulseOxDuration >= minDuration
                    && ProcedureInfo.PulseOxDuration <= maxDuration,
                $"PulseOxDuration ({ProcedureInfo.PulseOxDuration}s) should be between {minDuration}s and {maxDuration}s"
            );

            Assert.IsTrue(
                ProcedureInfo.RadialPulseDuration >= minDuration
                    && ProcedureInfo.RadialPulseDuration <= maxDuration,
                $"RadialPulseDuration ({ProcedureInfo.RadialPulseDuration}s) should be between {minDuration}s and {maxDuration}s"
            );

            Assert.IsTrue(
                ProcedureInfo.BpCuffDuration >= minDuration
                    && ProcedureInfo.BpCuffDuration <= maxDuration,
                $"BpCuffDuration ({ProcedureInfo.BpCuffDuration}s) should be between {minDuration}s and {maxDuration}s"
            );

            Assert.IsTrue(
                ProcedureInfo.InitialAssessmentDuration >= minDuration
                    && ProcedureInfo.InitialAssessmentDuration <= maxDuration,
                $"InitialAssessmentDuration ({ProcedureInfo.InitialAssessmentDuration}s) should be between {minDuration}s and {maxDuration}s"
            );
        }
    }
}
