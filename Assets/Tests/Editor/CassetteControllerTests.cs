using NUnit.Framework;
using UnityEngine;
using RetroRescue.Features.Cassette;

namespace RetroRescue.Tests
{
    public class CassetteControllerTests
    {
        [Test]
        public void ApplyDeltaTheta_IncreasesProgress()
        {
            var go = new GameObject();
            var comp = go.AddComponent<CassetteController>();
            comp.rewindK = 0.5f;
            comp.progress = 0f;
            comp.ApplyDeltaTheta(10f);
            Assert.AreEqual(5f, comp.progress);
            Object.DestroyImmediate(go);
        }

        [Test]
        public void ApplyDeltaTheta_ClampsAt100()
        {
            var go = new GameObject();
            var comp = go.AddComponent<CassetteController>();
            comp.rewindK = 10f;
            comp.progress = 95f;
            comp.ApplyDeltaTheta(1f);
            Assert.AreEqual(100f, comp.progress);
            Object.DestroyImmediate(go);
        }
    }
}
