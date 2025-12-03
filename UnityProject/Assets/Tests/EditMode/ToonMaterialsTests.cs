using NUnit.Framework;
using UnityEngine;
using ParamedicSimulator;

namespace ParamedicSimulator.Tests.EditMode
{
    /// <summary>
    /// Tests for ToonMaterials color palette and utility methods.
    /// These tests verify basic functionality without requiring actual shader compilation.
    /// </summary>
    [TestFixture]
    public class ToonMaterialsTests
    {
        [Test]
        public void Colors_AllColorsHaveValidAlpha()
        {
            // All defined colors should have alpha = 1.0 (fully opaque)
            Assert.AreEqual(1.0f, ToonMaterials.Colors.GloveBlue.a, "GloveBlue alpha");
            Assert.AreEqual(1.0f, ToonMaterials.Colors.GlovePurple.a, "GlovePurple alpha");
            Assert.AreEqual(1.0f, ToonMaterials.Colors.SkinLight.a, "SkinLight alpha");
            Assert.AreEqual(1.0f, ToonMaterials.Colors.SkinMedium.a, "SkinMedium alpha");
            Assert.AreEqual(1.0f, ToonMaterials.Colors.SkinDark.a, "SkinDark alpha");
            Assert.AreEqual(1.0f, ToonMaterials.Colors.PulseOxBody.a, "PulseOxBody alpha");
            Assert.AreEqual(1.0f, ToonMaterials.Colors.Background.a, "Background alpha");
        }

        [Test]
        public void Colors_SkinTonesAreDistinct()
        {
            // Verify skin tones are meaningfully different from each other
            Assert.AreNotEqual(ToonMaterials.Colors.SkinLight, ToonMaterials.Colors.SkinMedium);
            Assert.AreNotEqual(ToonMaterials.Colors.SkinMedium, ToonMaterials.Colors.SkinDark);
            Assert.AreNotEqual(ToonMaterials.Colors.SkinLight, ToonMaterials.Colors.SkinDark);
        }

        [Test]
        public void Colors_GloveColorsAreDistinct()
        {
            Assert.AreNotEqual(ToonMaterials.Colors.GloveBlue, ToonMaterials.Colors.GlovePurple);
        }

        [Test]
        public void GetSkinColor_ReturnsCorrectColorForKnownKeys()
        {
            Assert.AreEqual(ToonMaterials.Colors.SkinLight, ToonMaterials.GetSkinColor("skin_light"));
            Assert.AreEqual(ToonMaterials.Colors.SkinMedium, ToonMaterials.GetSkinColor("skin_medium"));
            Assert.AreEqual(ToonMaterials.Colors.SkinDark, ToonMaterials.GetSkinColor("skin_dark"));
        }

        [Test]
        public void GetSkinColor_ReturnsDefaultForUnknownKey()
        {
            // Unknown keys should return SkinLight as default
            Assert.AreEqual(ToonMaterials.Colors.SkinLight, ToonMaterials.GetSkinColor("unknown_key"));
            Assert.AreEqual(ToonMaterials.Colors.SkinLight, ToonMaterials.GetSkinColor(""));
        }

        [Test]
        public void GetGloveColor_ReturnsCorrectColorForKnownKeys()
        {
            Assert.AreEqual(ToonMaterials.Colors.GloveBlue, ToonMaterials.GetGloveColor("glove_blue"));
            Assert.AreEqual(ToonMaterials.Colors.GlovePurple, ToonMaterials.GetGloveColor("glove_purple"));
        }

        [Test]
        public void GetGloveColor_ReturnsDefaultForUnknownKey()
        {
            // Unknown keys should return GloveBlue as default
            Assert.AreEqual(ToonMaterials.Colors.GloveBlue, ToonMaterials.GetGloveColor("unknown_key"));
            Assert.AreEqual(ToonMaterials.Colors.GloveBlue, ToonMaterials.GetGloveColor(""));
        }

        [Test]
        public void Colors_RGBValuesAreInValidRange()
        {
            // All color components should be in [0, 1] range
            AssertColorInRange(ToonMaterials.Colors.GloveBlue, "GloveBlue");
            AssertColorInRange(ToonMaterials.Colors.GlovePurple, "GlovePurple");
            AssertColorInRange(ToonMaterials.Colors.SkinLight, "SkinLight");
            AssertColorInRange(ToonMaterials.Colors.SkinMedium, "SkinMedium");
            AssertColorInRange(ToonMaterials.Colors.SkinDark, "SkinDark");
            AssertColorInRange(ToonMaterials.Colors.PulseOxBody, "PulseOxBody");
            AssertColorInRange(ToonMaterials.Colors.PulseOxClip, "PulseOxClip");
            AssertColorInRange(ToonMaterials.Colors.PulseOxScreenOff, "PulseOxScreenOff");
            AssertColorInRange(ToonMaterials.Colors.PulseOxScreenOn, "PulseOxScreenOn");
            AssertColorInRange(ToonMaterials.Colors.BpCuffBlue, "BpCuffBlue");
            AssertColorInRange(ToonMaterials.Colors.Background, "Background");
        }

        private void AssertColorInRange(Color color, string name)
        {
            Assert.IsTrue(color.r >= 0 && color.r <= 1, $"{name}.r should be in [0,1] range, was {color.r}");
            Assert.IsTrue(color.g >= 0 && color.g <= 1, $"{name}.g should be in [0,1] range, was {color.g}");
            Assert.IsTrue(color.b >= 0 && color.b <= 1, $"{name}.b should be in [0,1] range, was {color.b}");
            Assert.IsTrue(color.a >= 0 && color.a <= 1, $"{name}.a should be in [0,1] range, was {color.a}");
        }
    }
}
