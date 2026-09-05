using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// v1.0 verified HouseHouse symbols: PeckSwitch drives a PeckBus signal; the crane remains the
// normal Scaffold/PropHome presentation. The old guessed ScaffoldButton.Start patch is v1.1+ only.
[HarmonyPatch]
internal static class ScaffoldCraneButton
{
    internal const string VerifiedPuzzleSymbol = "PeckSwitch";
    internal const string VerifiedSignalSymbol = "PeckBus";
    internal const string VerifiedAnchorSymbol = "PropHome";

    internal static bool IsEnabled(LobbyPlayerCountMode mode, int playerCount) => GameModeConfig.IsHardWalkActive(mode, playerCount);

    internal static bool ShouldMoveCrane(bool hardWalkActive, bool switchPressed, bool busSignal)
        => hardWalkActive && switchPressed && busSignal;

    internal static void ApplyCraneMotion(bool hardWalkActive, bool switchPressed, bool busSignal, Transform crane, Vector3 target)
    {
        if (crane != null && ShouldMoveCrane(hardWalkActive, switchPressed, busSignal)) crane.position = target;
    }

    // v1.1+ reference skeleton; the shipped IL2CPP dump must confirm the concrete signatures.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
    [HarmonyPostfix, HarmonyPatch("ScaffoldButton", "Start")]
    private static void MoveButtonPostfix(Transform __instance, Transform? craneTransform) { }
#endif
}
