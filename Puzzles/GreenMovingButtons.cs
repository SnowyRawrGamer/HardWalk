using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// v1.0 verified HouseHouse symbols: PeckSwitch and PeckBus. Movement is evaluated only while
// Hard Walk is active; guessed GreenButton.Update interop is retained as a v1.1+ skeleton.
[HarmonyPatch]
internal static class GreenMovingButtons
{
    internal const string VerifiedPuzzleSymbol = "PeckSwitch";
    internal const string VerifiedSignalSymbol = "PeckBus";

    internal static bool IsEnabled(LobbyPlayerCountMode mode, int playerCount) => GameModeConfig.IsHardWalkActive(mode, playerCount);

    internal static Vector3 GetButtonPosition(bool hardWalkActive, bool busSignal, Vector3 normal, Vector3 hardWalk)
        => hardWalkActive && busSignal ? hardWalk : normal;

    internal static void ApplyButtonPosition(bool hardWalkActive, bool busSignal, Transform button, Vector3 normal, Vector3 hardWalk)
    {
        if (button != null) button.position = GetButtonPosition(hardWalkActive, busSignal, normal, hardWalk);
    }

    // v1.1+ reference skeleton; exact GreenButton.Update remains dump-dependent.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
    [HarmonyPostfix, HarmonyPatch("GreenButton", "Update")]
    private static void UpdatePostfix(Transform __instance) { }
#endif
}
