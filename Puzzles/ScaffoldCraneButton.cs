using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Scaffold Crane uses one press in Normal mode. In Hard Walk it requires all four hidden
// buttons to remain held simultaneously before the baby can be dropped.
[HarmonyPatch]
internal static class ScaffoldCraneButton
{
    internal const string VerifiedPuzzleSymbol = "PeckSwitch";
    internal const string VerifiedSignalSymbol = "PeckBus";
    internal const string VerifiedAnchorSymbol = "PropHome";
    internal const int HardWalkRequiredButtonCount = 4;

    internal static bool IsEnabled(LobbyPlayerCountMode mode, int playerCount) =>
        GameModeConfig.IsHardWalkActive(mode, playerCount);

    internal static bool ShouldDropBaby(
        bool hardWalkActive,
        bool normalButtonPressed,
        bool hiddenButton1Held,
        bool hiddenButton2Held,
        bool hiddenButton3Held,
        bool hiddenButton4Held)
    {
        if (!hardWalkActive)
            return normalButtonPressed;

        return hiddenButton1Held
            && hiddenButton2Held
            && hiddenButton3Held
            && hiddenButton4Held;
    }

    // Kept as the crane-motion gate used by the existing integration surface. The crane may
    // move only when the exact drop condition is satisfied.
    internal static bool ShouldMoveCrane(
        bool hardWalkActive,
        bool normalButtonPressed,
        bool hiddenButton1Held,
        bool hiddenButton2Held,
        bool hiddenButton3Held,
        bool hiddenButton4Held)
        => ShouldDropBaby(
            hardWalkActive,
            normalButtonPressed,
            hiddenButton1Held,
            hiddenButton2Held,
            hiddenButton3Held,
            hiddenButton4Held);

    internal static void ApplyCraneMotion(
        bool hardWalkActive,
        bool normalButtonPressed,
        bool hiddenButton1Held,
        bool hiddenButton2Held,
        bool hiddenButton3Held,
        bool hiddenButton4Held,
        Transform crane,
        Vector3 target)
    {
        if (crane != null && ShouldMoveCrane(
                hardWalkActive,
                normalButtonPressed,
                hiddenButton1Held,
                hiddenButton2Held,
                hiddenButton3Held,
                hiddenButton4Held))
        {
            crane.position = target;
        }
    }

    // v1.1+ reference skeleton; the shipped IL2CPP dump must confirm the concrete signatures.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
    [HarmonyPostfix, HarmonyPatch("ScaffoldButton", "Start")]
    private static void MoveButtonPostfix(Transform __instance, Transform? craneTransform) { }
#endif
}
