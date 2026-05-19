using UnityEngine;

namespace RetroRescue.Data
{
    public enum MechanicType { CASSETTE, TV_TUNING }

    [CreateAssetMenu(fileName = "NewLevelData", menuName = "RetroRescue/Level Data")]
    public class LevelDataSO : ScriptableObject
    {
        [Header("QA & Trazabilidad")]
        public string levelID; // Ejemplo: "REQ-M2-L5-CASSETTE"
        public int orderIndex;

        [Header("Configuración de Nivel")]
        public GameObject levelPrefab;
        public float levelDurationSeconds = 15f;

        [Header("Mecánica Específica")]
        public MechanicType mechanicType;
        public float difficultyModifier = 1.0f;

        [Header("Audio Events")]
        public string mainAudioEventPath;
    }
}
