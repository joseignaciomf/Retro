using UnityEngine;
using RetroRescue.Architecture;

namespace RetroRescue.Features.TVTuning
{
    public class TVTuningController : MonoBehaviour
    {
        public string levelId = "REQ-M2-L2-TV";

        [Header("References")]
        public Transform antennaL;
        public Transform antennaR;
        public Transform knobTransform;

        [Header("Ranges")]
        public float antennaMin = 15f;
        public float antennaMax = 165f;

        [Header("Tuning")]
        public float targetAntennaL;
        public float targetAntennaR;
        public float targetKnob;

        [Header("Behaviour")]
        public float holdTimeRequired = 2.0f;

        private float angleL = 90f;
        private float angleR = 90f;
        private float knobAngle = 0f;
        private float clarity = 0f;
        private float holdTimer = 0f;
        private float maxError;
        private int lastKnobTickIndex = -1;

        void Start()
        {
            maxError = TVTuningMath.ComputeMaxError(antennaMin, antennaMax);
            GenerateTargets();
            PublishProgress();
        }

        void Update()
        {
            clarity = TVTuningMath.ComputeClarity(angleL, angleR, knobAngle, targetAntennaL, targetAntennaR, targetKnob, maxError);

            PublishProgress();
            GameEvents.RaiseFeedbackTriggered("EV_STATIC_AUDIO");

            if (clarity >= 0.95f)
            {
                holdTimer += Time.deltaTime;
                if (holdTimer >= holdTimeRequired)
                {
                    GameEvents.RaiseLevelStateChanged(LevelState.Success);
                    GameEvents.RaiseFeedbackTriggered("EV_WIN_LEVEL");
                }
            }
            else
            {
                holdTimer = 0f;
            }

            int tickIndex = Mathf.FloorToInt(knobAngle / 10f);
            if (tickIndex != lastKnobTickIndex)
            {
                lastKnobTickIndex = tickIndex;
                GameEvents.RaiseFeedbackTriggered("EV_KNOB_TICK");
            }
        }

        public void GenerateTargets()
        {
            targetAntennaL = Random.Range(antennaMin, antennaMax);
            targetAntennaR = Random.Range(antennaMin, antennaMax);
            targetKnob = Random.Range(0f, 360f);
        }

        private void PublishProgress()
        {
            GameEvents.RaiseMechanicProgressUpdated(levelId, clarity * 100f);
        }

        public void SetAntennaL(float angle)
        {
            angleL = Mathf.Clamp(angle, antennaMin, antennaMax);
            if (antennaL != null) antennaL.localEulerAngles = new Vector3(0f, 0f, angleL);
        }

        public void SetAntennaR(float angle)
        {
            angleR = Mathf.Clamp(angle, antennaMin, antennaMax);
            if (antennaR != null) antennaR.localEulerAngles = new Vector3(0f, 0f, angleR);
        }

        public void SetKnob(float angle)
        {
            knobAngle = (angle % 360f + 360f) % 360f;
            if (knobTransform != null) knobTransform.localEulerAngles = new Vector3(0f, 0f, knobAngle);
        }

        public float GetClarity() => clarity;
    }
}
