using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

[HarmonyPatch]
internal static class ScaffoldCraneButton
{
    private static readonly Vector3 CraneButtonOffset = new(0f, 0.15f, 0.8f);

    [HarmonyPostfix]
    [HarmonyPatch("ScaffoldButton", "Start")]
    private static void MoveButtonPostfix(Transform __instance, Transform? craneTransform)
    {
        if (craneTransform != null) __instance.position = craneTransform.TransformPoint(CraneButtonOffset);
    }
}
