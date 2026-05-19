using UnityEngine;
using RetroRescue.Architecture;

namespace RetroRescue.Features.Cassette
{
    public class CassetteController : MonoBehaviour
    {
        public string levelId = "REQ-MEC-001";
        public float progress = 0f; // 0..100
        public float rewindK = 0.5f; // K_rewind multiplier
        public float activationRadius = 80f; // pixels (tune in editor)
        public float levelDuration = 15f;

        private float timeRemaining;
        private bool inputActive = false;

        void Start()
        {
            timeRemaining = levelDuration;
        }

        void Update()
        {
            if (!inputActive) return;
            timeRemaining -= Time.deltaTime;
            // publicar timer para HUD
            GameEvents.RaiseLevelTimerUpdated(Mathf.Max(0f, timeRemaining));
            if (timeRemaining <= 0f)
            {
                inputActive = false;
                if (progress >= 100f)
                {
                    GameEvents.RaiseLevelStateChanged(LevelState.Success);
                }
                else
                {
                    GameEvents.RaiseLevelStateChanged(LevelState.Failure);
                }
            }
        }

        // Llamar desde el sistema de input (New Input System) con delta theta positivo
        public void ApplyDeltaTheta(float deltaTheta)
        {
            if (deltaTheta <= 0f) return; // solo sentido horario
            float deltaProg = Mathf.Abs(deltaTheta) * rewindK;
            progress = Mathf.Clamp(progress + deltaProg, 0f, 100f);
            GameEvents.RaiseMechanicProgressUpdated(levelId, progress);
            if (progress >= 100f)
            {
                inputActive = false;
                GameEvents.RaiseLevelStateChanged(LevelState.Success);
                GameEvents.RaiseFeedbackTriggered("EV_WIN_LEVEL");
            }
            else
            {
                GameEvents.RaiseFeedbackTriggered("EV_TAPE_LOOP");
            }
        }

        public void ActivateInput()
        {
            inputActive = true;
            timeRemaining = levelDuration;
            GameEvents.RaiseLevelStateChanged(LevelState.GameplayInput);
            GameEvents.RaiseLevelTimerUpdated(timeRemaining);
        }

        public void DeactivateInput()
        {
            inputActive = false;
        }
    }
}
