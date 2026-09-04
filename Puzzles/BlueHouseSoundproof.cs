// Evidence status: DISABLED. Public source confirms Dissonance voice concepts, but does not
// verify a Big Walk type named ProximityVoiceChat, CanHear, or a Blue House volume query.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

[HarmonyPatch]
internal static class BlueHouseSoundproof
{
    [HarmonyPrefix]
    [HarmonyPatch("ProximityVoiceChat", "CanHear")]
    private static bool CanHearPrefix(Vector3 listenerPosition, Vector3 speakerPosition, ref bool __result)
    {
        __result = false;
        return false;
    }
}
#endif
