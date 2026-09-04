// Evidence status: DISABLED. Public sources inspected do not verify a telescope shell-game
// controller or StartRound/CheckButton methods. Keep disabled until the Steam v39 dump proves
// the declaring type, parameter types, and return values.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
using HarmonyLib;

namespace HardWalk.Puzzles;

[HarmonyPatch]
internal static class TelescopeShellGame
{
    [HarmonyPostfix]
    [HarmonyPatch("TelescopeShellGameController", "StartRound")]
    private static void StartRoundPostfix() { }

    [HarmonyPrefix]
    [HarmonyPatch("TelescopeShellGameController", "CheckButton")]
    private static bool CheckButtonPrefix(int buttonIndex, ref bool __result) { __result = false; return false; }
}
#endif
