using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Green Moving Buttons: static in Normal Walk, vertically animated at independently random
// speeds in Hard Walk. The motion is attached to each button so returning to Normal Walk
// restores the exact position captured when the button was first seen.
[HarmonyPatch]
internal static class GreenMovingButtons
{
    private const float MovementAmplitude = 0.35f;
    private const float MinimumSpeed = 0.65f;
    private const float MaximumSpeed = 1.8f;

    [HarmonyPostfix]
    [HarmonyPatch("GreenButton", "Update")]
    private static void UpdatePostfix(Transform __instance)
    {
        if (__instance == null) return;

        var motion = __instance.GetComponent<GreenButtonMotion>();
        if (motion == null) motion = __instance.gameObject.AddComponent<GreenButtonMotion>();

        // The mode gate is deliberately checked every frame: Normal Walk never moves the
        // buttons, while Hard Walk gives each button its own random vertical speed.
        var hardWalkActive = GameModeConfig.SelectedMode.Value == HardWalkGameMode.HardWalk
            && GameModeConfig.IsFourPlus(GameModeConfig.PlayerCountMode.Value);
        motion.SetHardWalkActive(hardWalkActive);
    }

    private sealed class GreenButtonMotion : MonoBehaviour
    {
        private Vector3 _normalLocalPosition;
        private float _speed;
        private float _phase;
        private bool _initialized;
        private bool _hardWalkActive;

        private void Awake()
        {
            _normalLocalPosition = transform.localPosition;
            _speed = Random.Range(MinimumSpeed, MaximumSpeed);
            _phase = Random.Range(0f, Mathf.PI * 2f);
            _initialized = true;
        }

        internal void SetHardWalkActive(bool active)
        {
            if (!_initialized) Awake();
            if (!active)
            {
                _hardWalkActive = false;
                transform.localPosition = _normalLocalPosition;
                return;
            }

            _hardWalkActive = true;
        }

        private void Update()
        {
            if (!_hardWalkActive) return;

            var position = _normalLocalPosition;
            position.y += Mathf.Sin((Time.time * _speed) + _phase) * MovementAmplitude;
            transform.localPosition = position;
        }
    }
}
