using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Transfers the yellow mask to the current pass-the-ball holder.
// Replace target types/signatures with those from the game's IL2CPP dump.
[HarmonyPatch]
internal static class PassTheBallYellowMask
{
    private static GameObject? _maskInstance;
    private static Component? _currentHolder;

    [HarmonyPostfix]
    [HarmonyPatch("PassTheBallRelay", "OnBallPickedUp")]
    private static void OnBallPickedUpPostfix(Component holder, GameObject yellowMaskPrefab)
    {
        RemoveMask();
        if (holder == null || yellowMaskPrefab == null) return;

        _currentHolder = holder;
        _maskInstance = Object.Instantiate(yellowMaskPrefab, holder.transform);
        _maskInstance.name = "HardWalk_UnremovableYellowMask";
        _maskInstance.transform.localPosition = Vector3.zero;
        _maskInstance.transform.localRotation = Quaternion.identity;

        // Prevent normal inventory/drop logic from removing the challenge mask.
        var removable = _maskInstance.GetComponent<Collider>();
        if (removable != null) removable.enabled = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch("PassTheBallRelay", "OnBallDropped")]
    private static void OnBallDroppedPostfix() => RemoveMask();

    [HarmonyPostfix]
    [HarmonyPatch("PassTheBallRelay", "OnBallPassed")]
    private static void OnBallPassedPostfix(Component newHolder, GameObject yellowMaskPrefab)
        => OnBallPickedUpPostfix(newHolder, yellowMaskPrefab);

    private static void RemoveMask()
    {
        if (_maskInstance != null) Object.Destroy(_maskInstance);
        _maskInstance = null;
        _currentHolder = null;
    }
}
