using HarmonyLib;
using UnityEngine;

namespace HardWalk.Global;

// Kept as a harmless compatibility patch for older installations. Mode selection is now entirely
// in Host a Game -> 4+ Players; no physical in-world button or sign is required.
[HarmonyPatch]
internal static class SpawnHardWalkSign
{
    [HarmonyPostfix]
    [HarmonyPatch("SpawnArea", "Initialize")]
    private static void InitializePostfix(Transform spawnArea, int playerCount)
    {
        // Intentionally empty: the host menu is the only mode-selection surface.
    }
}
