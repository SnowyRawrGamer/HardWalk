using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

[HarmonyPatch]
internal static class BlueHouseSoundproof
{
    private const float OcclusionRadius = 6f;

    [HarmonyPrefix]
    [HarmonyPatch("ProximityVoiceChat", "CanHear")]
    private static bool CanHearPrefix(Vector3 listenerPosition, Vector3 speakerPosition, ref bool __result)
    {
        var distance = Vector3.Distance(listenerPosition, speakerPosition);
        __result = distance <= OcclusionRadius && !IsInsideBlueHouse(listenerPosition) ||
                   distance <= OcclusionRadius && !IsInsideBlueHouse(speakerPosition);
        return false;
    }

    private static bool IsInsideBlueHouse(Vector3 position) => Physics.OverlapSphere(position, 0.1f).Length > 0; // Replace with the game's room/volume query.
}
