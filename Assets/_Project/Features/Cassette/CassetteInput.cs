using UnityEngine;
using UnityEngine.InputSystem;

namespace RetroRescue.Features.Cassette
{
    [RequireComponent(typeof(CassetteController))]
    public class CassetteInput : MonoBehaviour
    {
        public Transform leftGearCenter;
        public Transform rightGearCenter;
        public float activationRadius = 80f; // píxeles

        private CassetteController cassette;
        private Camera mainCam;
        private int activeFingerId = -1;
        private Vector2 prevScreenPos;
        private float prevAngle;
        private Transform activeAxisCenter;

        void Awake()
        {
            cassette = GetComponent<CassetteController>();
            mainCam = Camera.main;
        }

        void Update()
        {
            if (Touchscreen.current == null) return;
            var touches = Touchscreen.current.touches;
            for (int i = 0; i < touches.Count; i++)
            {
                var t = touches[i];
                if (!t.press.isPressed) continue;
                var touchId = i; // index as id (approximate for simple polling)
                var pos = t.position.ReadValue();

                if (activeFingerId == -1 && t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                {
                    // comprobar distancia a ejes
                    if (IsWithinRadius(pos, leftGearCenter))
                    {
                        ActivateAxis(leftGearCenter, pos);
                    }
                    else if (IsWithinRadius(pos, rightGearCenter))
                    {
                        ActivateAxis(rightGearCenter, pos);
                    }
                }

                if (activeFingerId == touchId && t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    var angle = ScreenAngle(activeAxisCenter, pos);
                    var delta = Mathf.DeltaAngle(prevAngle, angle);
                    // Convención: consideramos giro horario como delta < 0 (si no, invertir)
                    if (delta < 0f)
                    {
                        cassette.ApplyDeltaTheta(Mathf.Abs(delta));
                    }
                    prevAngle = angle;
                    prevScreenPos = pos;
                }

                if (activeFingerId == touchId && (t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended || t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled))
                {
                    activeFingerId = -1;
                    activeAxisCenter = null;
                }
            }
        }

        private bool IsWithinRadius(Vector2 screenPos, Transform axis)
        {
            if (axis == null) return false;
            var axisScreen = mainCam.WorldToScreenPoint(axis.position);
            var dist = Vector2.Distance(screenPos, axisScreen);
            return dist <= activationRadius;
        }

        private void ActivateAxis(Transform axis, Vector2 screenPos)
        {
            activeFingerId = 0; // simplificación: primer dedo
            activeAxisCenter = axis;
            prevScreenPos = screenPos;
            prevAngle = ScreenAngle(axis, screenPos);
            cassette.ActivateInput();
            GameEvents.RaiseFeedbackTriggered("EV_TAP_SNAP");
        }

        private float ScreenAngle(Transform axis, Vector2 screenPos)
        {
            var axisScreen = mainCam.WorldToScreenPoint(axis.position);
            var dir = screenPos - (Vector2)axisScreen;
            return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        }
    }
}
