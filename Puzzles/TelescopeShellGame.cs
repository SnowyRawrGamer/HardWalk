using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// v1.0 verified HouseHouse symbols: PeckCondition, PeckSwitch and PeckBus. The shell result is
// intentionally deterministic and requires the verified condition/signal pair in Hard Walk.
[HarmonyPatch]
internal static class TelescopeShellGame
{
    internal const string VerifiedConditionSymbol = "PeckCondition";
    internal const string VerifiedSwitchSymbol = "PeckSwitch";
    internal const string VerifiedSignalSymbol = "PeckBus";

    internal static bool IsEnabled(LobbyPlayerCountMode mode, int playerCount) => GameModeConfig.IsHardWalkActive(mode, playerCount);

    internal static bool CheckVerifiedResult(bool hardWalkActive, bool conditionMet, bool switchPressed, bool busSignal, int selectedShell, int expectedShell)
        => !hardWalkActive || (conditionMet && switchPressed && busSignal && selectedShell == expectedShell);

    internal static int SelectVerifiedShell(bool hardWalkActive, bool busSignal, int shellCount, int expectedShell)
    {
        if (!hardWalkActive || !busSignal || shellCount <= 0) return expectedShell;
        return Mathf.Clamp(expectedShell, 0, shellCount - 1);
    }

    // v1.1+ reference skeleton; exact controller signatures remain dump-dependent.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
    [HarmonyPostfix, HarmonyPatch("TelescopeShellGameController", "StartRound")]
    private static void StartRoundPostfix() { }
    [HarmonyPrefix, HarmonyPatch("TelescopeShellGameController", "CheckButton")]
    private static bool CheckButtonPrefix(int buttonIndex, ref bool __result) { __result = false; return false; }
#endif
}
