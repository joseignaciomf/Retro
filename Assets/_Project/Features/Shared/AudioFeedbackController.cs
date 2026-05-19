using UnityEngine;

#if FMOD
using FMODUnity;
#endif

namespace RetroRescue.Features.Audio
{
    public class AudioFeedbackController : MonoBehaviour
    {
        private void OnEnable()
        {
            RetroRescue.Architecture.GameEvents.OnFeedbackTriggered += HandleFeedback;
        }

        private void OnDisable()
        {
            RetroRescue.Architecture.GameEvents.OnFeedbackTriggered -= HandleFeedback;
        }

        private void HandleFeedback(string eventId)
        {
            switch (eventId)
            {
                case "EV_TAP_SNAP":
#if FMOD
                    RuntimeManager.PlayOneShot("event:/FX/Bic_Snap");
#else
                    Debug.Log("Audio: EV_TAP_SNAP");
#endif
                    break;
                case "EV_TAPE_LOOP":
#if FMOD
                    RuntimeManager.PlayOneShot("event:/FX/Tape_Roll");
#else
                    Debug.Log("Audio: EV_TAPE_LOOP");
#endif
                    break;
                case "EV_WIN_LEVEL":
#if FMOD
                    RuntimeManager.PlayOneShot("event:/Stingers/Win_Jingle");
#else
                    Debug.Log("Audio: EV_WIN_LEVEL");
#endif
                    break;
                default:
                    Debug.Log($"Unknown audio event: {eventId}");
                    break;
            }
        }
    }
}
