using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// v1.0 uses the verified HouseHouse symbol PropHome as the tutorial puzzle anchor.
// The original guessed Baby-pedestal implementation is retained as a v1.1 reference skeleton.
[HarmonyPatch]
internal static class TutorialPedestalBabyFakeout
{
    internal const string VerifiedPuzzleSymbol = "PropHome";
    private static bool _firstCompletionTriggered;
    private static bool _keyGranted;

    // v1.1 skeleton retained: the shipped IL2CPP signature still needs confirmation.
    [HarmonyPostfix]
    [HarmonyPatch("TutorialBabyPedestal", "Initialize")]
    private static void InitializePostfix() { _firstCompletionTriggered = false; _keyGranted = false; }

    [HarmonyPrefix]
    [HarmonyPatch("TutorialBabyPedestal", "OnAllBabiesSlotted")]
    private static bool OnAllBabiesSlottedPrefix(Transform[] babyContainers, Rigidbody[] slottedBabies, ref bool __result)
    {
        if (_keyGranted) { __result = true; return false; }
        if (!_firstCompletionTriggered) { _firstCompletionTriggered = true; EjectBabies(babyContainers, slottedBabies); __result = false; return false; }
        _keyGranted = true; __result = true; return true;
    }

    private static void EjectBabies(Transform[] containers, Rigidbody[] babies)
    {
        if (containers != null) foreach (var container in containers) if (container != null) container.gameObject.SetActive(false);
        if (babies == null) return;
        foreach (var baby in babies) if (baby != null) { baby.transform.SetParent(null, true); baby.isKinematic = false; baby.detectCollisions = true; baby.AddForce(Vector3.up * 1.5f + Random.insideUnitSphere * 0.5f, ForceMode.VelocityChange); }
    }
}
