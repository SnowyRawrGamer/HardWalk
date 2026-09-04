// Evidence status: DISABLED. ScaffoldButton, Start, and the craneTransform parameter are
// placeholders; no public evidence establishes this signature.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

[HarmonyPatch]
internal static class ScaffoldCraneButton
{
    [HarmonyPostfix]
    [HarmonyPatch("ScaffoldButton", "Start")]
    private static void MoveButtonPostfix(Transform __instance, Transform? craneTransform) { }
}
#endif
