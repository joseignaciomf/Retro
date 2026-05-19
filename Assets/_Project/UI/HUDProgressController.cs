using UnityEngine;
using UnityEngine.UI;
using RetroRescue.Architecture;

namespace RetroRescue.UI
{
    public class HUDProgressController : MonoBehaviour
    {
        public string levelId = "REQ-MEC-001";
        public Slider progressSlider;
        public Text progressText;
        public Text timerText;

        void OnEnable()
        {
            GameEvents.OnMechanicProgressUpdated += OnProgressUpdated;
            GameEvents.OnLevelTimerUpdated += OnTimerUpdated;
            GameEvents.OnLevelStateChanged += OnStateChanged;
        }

        void OnDisable()
        {
            GameEvents.OnMechanicProgressUpdated -= OnProgressUpdated;
            GameEvents.OnLevelTimerUpdated -= OnTimerUpdated;
            GameEvents.OnLevelStateChanged -= OnStateChanged;
        }

        private void OnProgressUpdated(string id, float progress)
        {
            if (!string.IsNullOrEmpty(levelId) && id != levelId) return;
            if (progressSlider != null) progressSlider.value = progress / 100f;
            if (progressText != null) progressText.text = $"{Mathf.RoundToInt(progress)}%";
        }

        private void OnTimerUpdated(float secondsRemaining)
        {
            if (timerText != null) timerText.text = secondsRemaining.ToString("F1") + "s";
        }

        private void OnStateChanged(LevelState state)
        {
            if (state == LevelState.Init || state == LevelState.IntroAnim)
            {
                if (progressSlider != null) progressSlider.value = 0f;
                if (progressText != null) progressText.text = "0%";
            }
        }
    }
}
