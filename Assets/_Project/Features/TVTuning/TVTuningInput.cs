using UnityEngine;

namespace RetroRescue.Features.TVTuning
{
    [RequireComponent(typeof(TVTuningController))]
    public class TVTuningInput : MonoBehaviour
    {
        public Transform antennaLPivot;
        public Transform antennaRPivot;
        public Transform knobCenter;
        public float activationRadius = 80f;

        private TVTuningController controller;
        private Camera mainCam;
        private int activeFingerId = -1;
        private string activeControl = null; // "L","R","K"

        void Awake()
        {
            controller = GetComponent<TVTuningController>();
            mainCam = Camera.main;
        }

        void Update()
        {
            if (Touchscreen.current != null)
            {
                var touches = Touchscreen.current.touches;
                for (int i = 0; i < touches.Count; i++)
                {
                    var t = touches[i];
                    if (!t.press.isPressed) continue;
                    var touchId = i;
                    var pos = t.position.ReadValue();

                    if (activeFingerId == -1 && t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        if (IsWithinRadius(pos, antennaLPivot)) { Activate("L", pos); }
                        else if (IsWithinRadius(pos, antennaRPivot)) { Activate("R", pos); }
                        else if (IsWithinRadius(pos, knobCenter)) { Activate("K", pos); }
                    }

                    if (activeFingerId == touchId && t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Moved)
                    {
                        if (activeControl == "K")
                        {
                            var angle = ScreenAngle(knobCenter, pos);
                            controller.SetKnob(angle);
                        }
                        else if (activeControl == "L")
                        {
                            var angle = ScreenAngle(antennaLPivot, pos);
                            // clamp to 0..360 then remap to antenna range
                            controller.SetAntennaL(angle);
                        }
                        else if (activeControl == "R")
                        {
                            var angle = ScreenAngle(antennaRPivot, pos);
                            controller.SetAntennaR(angle);
                        }
                    }

                    if (activeFingerId == touchId && (t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Ended || t.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Canceled))
                    {
                        activeFingerId = -1; activeControl = null;
                    }
                }
            }
            else
            {
                // fallback mouse controls for editor testing
                if (Input.GetMouseButtonDown(0))
                {
                    var pos = Input.mousePosition;
                    if (IsWithinRadius(pos, antennaLPivot)) { activeControl = "L"; }
                    else if (IsWithinRadius(pos, antennaRPivot)) { activeControl = "R"; }
                    else if (IsWithinRadius(pos, knobCenter)) { activeControl = "K"; }
                }
                if (Input.GetMouseButton(0) && activeControl != null)
                {
                    var pos = (Vector2)Input.mousePosition;
                    if (activeControl == "K") controller.SetKnob(ScreenAngle(knobCenter, pos));
                    if (activeControl == "L") controller.SetAntennaL(ScreenAngle(antennaLPivot, pos));
                    if (activeControl == "R") controller.SetAntennaR(ScreenAngle(antennaRPivot, pos));
                }
                if (Input.GetMouseButtonUp(0)) activeControl = null;
            }
        }

        private bool IsWithinRadius(Vector2 screenPos, Transform t)
        {
            if (t == null) return false;
            var axisScreen = mainCam.WorldToScreenPoint(t.position);
            return Vector2.Distance(screenPos, axisScreen) <= activationRadius;
        }

        private void Activate(string control, Vector2 pos)
        {
            activeControl = control;
            activeFingerId = 0; // simplificación
        }

        private float ScreenAngle(Transform axis, Vector2 screenPos)
        {
            var axisScreen = mainCam.WorldToScreenPoint(axis.position);
            var dir = screenPos - (Vector2)axisScreen;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (angle < 0f) angle += 360f;
            return angle;
        }
    }
}
