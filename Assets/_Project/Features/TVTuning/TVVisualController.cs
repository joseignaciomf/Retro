using UnityEngine;
using RetroRescue.Architecture;

namespace RetroRescue.Features.TVTuning
{
    public class TVVisualController : MonoBehaviour
    {
        public TVTuningController tuningController;
        public Renderer screenRenderer; // material with shader that supports '_NoiseOpacity' or '_Clarity'

        void OnEnable()
        {
            GameEvents.OnMechanicProgressUpdated += OnProgress;
        }

        void OnDisable()
        {
            GameEvents.OnMechanicProgressUpdated -= OnProgress;
        }

        private void OnProgress(string id, float value)
        {
            if (tuningController == null) return;
            if (!string.IsNullOrEmpty(tuningController.levelId) && id != tuningController.levelId) return;
            float clarity = Mathf.Clamp01(value / 100f);
            if (screenRenderer != null && screenRenderer.material != null)
            {
                if (screenRenderer.material.HasProperty("_NoiseOpacity"))
                {
                    screenRenderer.material.SetFloat("_NoiseOpacity", 1f - clarity);
                }
                if (screenRenderer.material.HasProperty("_Clarity"))
                {
                    screenRenderer.material.SetFloat("_Clarity", clarity);
                }
            }
        }
    }
}
