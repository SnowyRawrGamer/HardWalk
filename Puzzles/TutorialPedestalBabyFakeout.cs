using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// v1.0 verified HouseHouse anchor: PropHome. The guessed v1.1 patch skeleton remains below.
[HarmonyPatch]
internal static class TutorialPedestalBabyFakeout
{
    internal const string VerifiedPuzzleSymbol = "PropHome";
    internal const string VerifiedMechanic = "first placement ejects the babies; the next valid placement completes";
    private static bool _firstCompletionTriggered;
    private static bool _keyGranted;

    internal static bool IsEnabled(LobbyPlayerCountMode mode, int playerCount) =>
        GameModeConfig.IsHardWalkActive(mode, playerCount);

    internal static bool TryHandleCompletion(bool hardWalkActive, Transform[] containers, Rigidbody[] babies, ref bool result)
    {
        if (!hardWalkActive) return true;
        if (_keyGranted) { result = true; return false; }
        if (!_firstCompletionTriggered)
        {
            _firstCompletionTriggered = true;
            EjectBabies(containers, babies);
            result = false;
            return false;
        }
        _keyGranted = true;
        result = true;
        return true;
    }

    private static void EjectBabies(Transform[] containers, Rigidbody[] babies)
    {
        if (containers != null) foreach (var container in containers) if (container != null) container.gameObject.SetActive(false);
        if (babies == null) return;
        foreach (var baby in babies) if (baby != null)
        {
            baby.transform.SetParent(null, true);
            baby.isKinematic = false;
            baby.detectCollisions = true;
            baby.AddForce(Vector3.up * 1.5f + Random.insideUnitSphere * 0.5f, ForceMode.VelocityChange);
        }
    }

    // v1.1+ reference skeleton: exact TutorialBabyPedestal signatures remain dump-dependent.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
    [HarmonyPostfix, HarmonyPatch("TutorialBabyPedestal", "Initialize")]
    private static void InitializePostfix() { _firstCompletionTriggered = false; _keyGranted = false; }
    [HarmonyPrefix, HarmonyPatch("TutorialBabyPedestal", "OnAllBabiesSlotted")]
    private static bool OnAllBabiesSlottedPrefix(Transform[] c, Rigidbody[] b, ref bool __result) => TryHandleCompletion(true, c, b, ref __result);
#endif
}
