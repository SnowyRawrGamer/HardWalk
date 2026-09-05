using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// v1.0 verified symbol: PeckSwitch. Existing beach-house behavior is retained as a v1.1 skeleton.
[HarmonyPatch]
internal static class GreenBeachHouseNoTelescopeBabyLaunch
{
    internal const string VerifiedPuzzleSymbol = "PeckSwitch";
    // v1.1 reference skeleton intentionally retained; target signatures remain dump-dependent.
    [HarmonyPostfix]
    [HarmonyPatch("GreenBeachHousePuzzle", "Initialize")]
    private static void InitializePostfix(Transform beachHouseRoot) { }
}
