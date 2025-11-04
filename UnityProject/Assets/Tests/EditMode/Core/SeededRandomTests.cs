using NUnit.Framework;
using ParamedicSimulator.Core;

namespace ParamedicSimulator.Tests.EditMode.Core
{
    public class SeededRandomTests
    {
        [Test]
        public void SeededRandom_SameSeed_ProducesSameSequence()
        {
            var rng1 = new SeededRandom(12345);
            var rng2 = new SeededRandom(12345);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(rng1.Next(), rng2.Next());
            }
        }

        [Test]
        public void SeededRandom_DifferentSeed_ProducesDifferentSequence()
        {
            var rng1 = new SeededRandom(12345);
            var rng2 = new SeededRandom(67890);

            bool different = false;
            for (int i = 0; i < 100; i++)
            {
                if (rng1.Next() != rng2.Next())
                {
                    different = true;
                    break;
                }
            }

            Assert.IsTrue(different, "Different seeds should produce different sequences");
        }

        [Test]
        public void SeededRandom_Reset_RestoresSequence()
        {
            var rng = new SeededRandom(12345);
            var first = rng.Next();
            var second = rng.Next();
            var third = rng.Next();

            rng.Reset();

            Assert.AreEqual(first, rng.Next());
            Assert.AreEqual(second, rng.Next());
            Assert.AreEqual(third, rng.Next());
        }

        [Test]
        public void SeededRandom_Next_ReturnsValidInteger()
        {
            var rng = new SeededRandom(12345);
            int value = rng.Next();
            Assert.GreaterOrEqual(value, 0);
        }

        [Test]
        public void SeededRandom_NextMaxValue_ReturnsValidRange()
        {
            var rng = new SeededRandom(12345);
            int value = rng.Next(100);
            Assert.GreaterOrEqual(value, 0);
            Assert.Less(value, 100);
        }

        [Test]
        public void SeededRandom_NextRange_ReturnsValidRange()
        {
            var rng = new SeededRandom(12345);
            int value = rng.Next(10, 20);
            Assert.GreaterOrEqual(value, 10);
            Assert.Less(value, 20);
        }

        [Test]
        public void SeededRandom_NextFloat_ReturnsValidRange()
        {
            var rng = new SeededRandom(12345);
            float value = rng.NextFloat();
            Assert.GreaterOrEqual(value, 0f);
            Assert.LessOrEqual(value, 1f);
        }

        [Test]
        public void SeededRandom_Range_ReturnsValidRange()
        {
            var rng = new SeededRandom(12345);
            float value = rng.Range(10f, 20f);
            Assert.GreaterOrEqual(value, 10f);
            Assert.LessOrEqual(value, 20f);
        }

        [Test]
        public void SeededRandom_Telemetry_ContainsSeed()
        {
            var rng = new SeededRandom(12345);
            rng.Next();
            rng.Next();

            var telemetry = rng.GetTelemetry();
            Assert.AreEqual(12345, telemetry.Seed);
            Assert.AreEqual(2UL, telemetry.CallCount);
        }
    }
}
