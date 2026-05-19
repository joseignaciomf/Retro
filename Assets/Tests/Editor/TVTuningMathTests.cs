using NUnit.Framework;
using UnityEngine;
using RetroRescue.Features.TVTuning;

namespace RetroRescue.Tests
{
    public class TVTuningMathTests
    {
        [Test]
        public void PerfectMatchReturnsOne()
        {
            float maxError = TVTuningMath.ComputeMaxError(15f, 165f);
            float c = TVTuningMath.ComputeClarity(10f, 20f, 30f, 10f, 20f, 30f, maxError);
            Assert.AreEqual(1f, c);
        }

        [Test]
        public void WorstMatchReturnsZero()
        {
            float maxError = TVTuningMath.ComputeMaxError(15f, 165f);
            float c = TVTuningMath.ComputeClarity(0f, 0f, 0f, 180f, 180f, 180f, maxError);
            Assert.AreEqual(0f, c);
        }

        [Test]
        public void PartialMatchBetween0and1()
        {
            float maxError = TVTuningMath.ComputeMaxError(15f, 165f);
            float c = TVTuningMath.ComputeClarity(15f, 15f, 0f, 165f, 165f, 180f, maxError);
            Assert.That(c, Is.GreaterThan(0f).And.LessThan(1f));
        }
    }
}
