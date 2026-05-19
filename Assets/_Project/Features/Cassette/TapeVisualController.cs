using UnityEngine;
using RetroRescue.Architecture;

namespace RetroRescue.Features.Cassette
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class TapeVisualController : MonoBehaviour
    {
        public string levelId = "REQ-MEC-001";
        private SpriteRenderer sr;
        private Vector3 initialScale;
        private Color initialColor;

        void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            initialScale = transform.localScale;
            initialColor = sr.color;
        }

        void OnEnable()
        {
            GameEvents.OnMechanicProgressUpdated += OnProgress;
        }

        void OnDisable()
        {
            GameEvents.OnMechanicProgressUpdated -= OnProgress;
        }

        private void OnProgress(string id, float progress)
        {
            if (!string.IsNullOrEmpty(levelId) && id != levelId) return;
            float t = Mathf.Clamp01(progress / 100f);
            // Escala Y disminuye conforme prog aumenta
            transform.localScale = new Vector3(initialScale.x, Mathf.Lerp(initialScale.y, 0.01f, t), initialScale.z);
            // Alfa hacia 0
            var c = initialColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            sr.color = c;
        }
    }
}
