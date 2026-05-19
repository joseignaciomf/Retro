using UnityEngine;

namespace RetroRescue.Features.TVTuning
{
    public static class TVTuningMath
    {
        public static float ComputeMaxError(float antennaMin, float antennaMax)
        {
            return (antennaMax - antennaMin) * 2f + 360f;
        }

        public static float ComputeClarity(float angleL, float angleR, float knobAngle, float targetL, float targetR, float targetKnob, float maxError)
        {
            float diffTotal = Mathf.Abs(Mathf.DeltaAngle(angleL, targetL)) + Mathf.Abs(Mathf.DeltaAngle(angleR, targetR)) + Mathf.Abs(Mathf.DeltaAngle(knobAngle, targetKnob));
            float c = 1f - (Mathf.Abs(diffTotal) / maxError);
            return Mathf.Clamp01(c);
        }
    }
}
