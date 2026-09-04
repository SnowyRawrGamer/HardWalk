// Evidence status: DISABLED. No public source inspected verifies GreenButton, its Update
// signature, or the requested Green Minefield stands/buttons/tether types.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

[HarmonyPatch]
internal static class GreenMovingButtons
{
    [HarmonyPostfix]
    [HarmonyPatch("GreenButton", "Update")]
    private static void UpdatePostfix(Transform __instance) { }
}
#endif
