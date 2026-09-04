using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

[HarmonyPatch]
internal static class GreenMovingButtons
{
    private const float Amplitude = 0.18f;
    private const float Frequency = 2f;

    [HarmonyPostfix]
    [HarmonyPatch("GreenButton", "Update")]
    private static void UpdatePostfix(Transform __instance)
    {
        var position = __instance.localPosition;
        position.y += Mathf.Sin(Time.time * Frequency) * Amplitude * Time.deltaTime;
        __instance.localPosition = position;
    }
}
