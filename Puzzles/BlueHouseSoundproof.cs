// v1.0 verified symbol: PeckCombinator. The unverified soundproof implementation is retained for v1.1.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
using HarmonyLib;
using UnityEngine;
namespace HardWalk.Puzzles;
[HarmonyPatch]
internal static class BlueHouseSoundproof
{
    internal const string VerifiedPuzzleSymbol = "PeckCombinator";
    [HarmonyPrefix]
    [HarmonyPatch("ProximityVoiceChat", "CanHear")]
    private static bool CanHearPrefix(Vector3 listenerPosition, Vector3 speakerPosition, ref bool __result) { __result = false; return false; }
}
#endif
