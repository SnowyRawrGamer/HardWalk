using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Adapt the target type/method names to the current Big Walk build.
[HarmonyPatch]
internal static class TelescopeShellGame
{
    private static readonly int[] BoxOrder = { 0, 1, 2 };
    private static int _targetBox;

    [HarmonyPostfix]
    [HarmonyPatch("TelescopeShellGameController", "StartRound")]
    private static void StartRoundPostfix() => _targetBox = Random.Range(0, BoxOrder.Length);

    [HarmonyPrefix]
    [HarmonyPatch("TelescopeShellGameController", "CheckButton")]
    private static bool CheckButtonPrefix(int buttonIndex, ref bool __result)
    {
        __result = buttonIndex == _targetBox;
        return false;
    }
}
