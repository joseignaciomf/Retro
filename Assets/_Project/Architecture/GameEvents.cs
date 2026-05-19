using System;
using UnityEngine;

namespace RetroRescue.Architecture
{
    public enum LevelState { Init, IntroAnim, GameplayInput, Success, Failure }

    public static class GameEvents
    {
        public static Action<LevelState> OnLevelStateChanged;
        public static Action<string, float> OnMechanicProgressUpdated;
        public static Action<string> OnFeedbackTriggered;
        public static Action<float> OnLevelTimerUpdated;

        public static void RaiseLevelStateChanged(LevelState newState) => OnLevelStateChanged?.Invoke(newState);
        public static void RaiseMechanicProgressUpdated(string levelId, float progress) => OnMechanicProgressUpdated?.Invoke(levelId, progress);
        public static void RaiseFeedbackTriggered(string eventId) => OnFeedbackTriggered?.Invoke(eventId);
        public static void RaiseLevelTimerUpdated(float secondsRemaining) => OnLevelTimerUpdated?.Invoke(secondsRemaining);
    }
}
