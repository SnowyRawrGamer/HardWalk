using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// v1.0 verified HouseHouse anchor: PropHome. The tutorial pedestal completion
// hook is kept isolated so normal mode continues through the vanilla path.
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
        // Normal Walk must retain the game's ordinary completion behavior.
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
        if (containers != null)
            foreach (var container in containers)
                if (container != null) container.gameObject.SetActive(false);

        if (babies == null) return;
        foreach (var baby in babies) if (baby != null)
        {
            baby.transform.SetParent(null, true);
            baby.isKinematic = false;
            baby.detectCollisions = true;
            baby.velocity = Vector3.zero;
            baby.AddForce(Vector3.up * 1.5f + Random.insideUnitSphere * 0.5f, ForceMode.VelocityChange);
        }
    }

    [HarmonyPostfix, HarmonyPatch("TutorialBabyPedestal", "Initialize")]
    private static void InitializePostfix()
    {
        _firstCompletionTriggered = false;
        _keyGranted = false;
    }

    [HarmonyPrefix, HarmonyPatch("TutorialBabyPedestal", "OnAllBabiesSlotted")]
    private static bool OnAllBabiesSlottedPrefix(Transform[] c, Rigidbody[] b, ref bool __result)
    {
        // The tutorial pedestal is only a four-player puzzle; the mode gate
        // makes the prefix a no-op in Normal Walk and other lobby sizes.
        var hardWalkActive = IsEnabled(GameModeConfig.PlayerCountMode.Value, 4);
        return TryHandleCompletion(hardWalkActive, c, b, ref __result);
    }
}
