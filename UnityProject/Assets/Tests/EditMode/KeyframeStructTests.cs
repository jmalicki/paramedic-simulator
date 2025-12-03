using NUnit.Framework;
using UnityEngine;
using ParamedicSimulator;

namespace ParamedicSimulator.Tests.EditMode
{
    /// <summary>
    /// Tests for the Keyframe struct used in procedural animations.
    /// </summary>
    [TestFixture]
    public class KeyframeStructTests
    {
        [Test]
        public void Constructor_WithPositionAndRotation_SetsAllValues()
        {
            var position = new Vector3(1f, 2f, 3f);
            var rotation = new Vector3(10f, 20f, 30f);

            var keyframe = new Keyframe(0.5f, position, rotation);

            Assert.AreEqual(0.5f, keyframe.Time);
            Assert.AreEqual(position, keyframe.Position);
            Assert.AreEqual(rotation, keyframe.Rotation);
        }

        [Test]
        public void Constructor_WithPositionOnly_SetsRotationToZero()
        {
            var position = new Vector3(1f, 2f, 3f);

            var keyframe = new Keyframe(1.0f, position);

            Assert.AreEqual(1.0f, keyframe.Time);
            Assert.AreEqual(position, keyframe.Position);
            Assert.AreEqual(Vector3.zero, keyframe.Rotation);
        }

        [Test]
        public void Keyframe_IsValueType()
        {
            // Keyframe is a struct, so it should be a value type
            Assert.IsTrue(typeof(Keyframe).IsValueType);
        }

        [Test]
        public void Keyframe_CanBeUsedInArray()
        {
            var keyframes = new Keyframe[]
            {
                new Keyframe(0f, Vector3.zero),
                new Keyframe(1f, Vector3.one),
                new Keyframe(2f, new Vector3(2f, 2f, 2f))
            };

            Assert.AreEqual(3, keyframes.Length);
            Assert.AreEqual(0f, keyframes[0].Time);
            Assert.AreEqual(1f, keyframes[1].Time);
            Assert.AreEqual(2f, keyframes[2].Time);
        }

        [Test]
        public void Keyframe_TimeCanBeNegative()
        {
            // Negative time might be used for pre-roll or offset animations
            var keyframe = new Keyframe(-1f, Vector3.zero);
            Assert.AreEqual(-1f, keyframe.Time);
        }

        [Test]
        public void Keyframe_RotationValuesCanExceed360()
        {
            // Rotation values can exceed 360 degrees for multiple rotations
            var rotation = new Vector3(720f, -180f, 450f);
            var keyframe = new Keyframe(0f, Vector3.zero, rotation);

            Assert.AreEqual(rotation, keyframe.Rotation);
        }
    }
}
