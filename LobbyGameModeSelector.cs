using HarmonyLib;
using UnityEngine;

namespace HardWalk;

// UI/network method names are placeholders until confirmed against the game's IL2CPP dump.
// The host is authoritative: clients cannot activate Hard Walk independently.
[HarmonyLib.HarmonyPatch]
internal static class LobbyGameModeSelector
{
    internal const string BigWalkLabel = "Big Walk";
    internal const string HardWalkLabel = "Hard Walk";
    internal const string HardWalkRequirementText = "Requires 4+ players";

    [HarmonyLib.HarmonyPostfix]
    [HarmonyLib.HarmonyPatch("LobbyHostSettings", "OnPlayerCountChanged")]
    private static void PlayerCountChangedPostfix(int playerCount)
    {
        GameModeConfig.EnforcePlayerRequirement(playerCount);
        RefreshModeSelector(playerCount);
    }

    [HarmonyLib.HarmonyPrefix]
    [HarmonyLib.HarmonyPatch("LobbyHostSettings", "SetGameMode")]
    private static bool SetGameModePrefix(HardWalkGameMode requestedMode, int playerCount, ref bool __result)
    {
        if (requestedMode == HardWalkGameMode.HardWalk && !GameModeConfig.IsHardWalkAllowed(playerCount))
        {
            GameModeConfig.SelectedMode.Value = HardWalkGameMode.BigWalk;
            __result = false;
            return false;
        }

        GameModeConfig.SelectedMode.Value = requestedMode;
        __result = true;
        return false;
    }

    [HarmonyLib.HarmonyPostfix]
    [HarmonyLib.HarmonyPatch("LobbyHostSettings", "BuildModeOptions")]
    private static void BuildModeOptionsPostfix(int playerCount, ref object[] __result)
    {
        var hardWalkEnabled = GameModeConfig.IsHardWalkAllowed(playerCount);
        __result = new object[]
        {
            new { Id = HardWalkGameMode.BigWalk, Label = BigWalkLabel, Description = GameModeConfig.BigWalkDescription, Enabled = true },
            new { Id = HardWalkGameMode.HardWalk, Label = HardWalkLabel, Description = hardWalkEnabled ? GameModeConfig.HardWalkDescription : HardWalkRequirementText, Enabled = hardWalkEnabled }
        };
    }

    private static void RefreshModeSelector(int playerCount)
    {
        // The actual UI bridge should set the Hard Walk option interactable state and helper text.
        var selector = GameObject.Find("LobbyHostSettings/ModeSelector");
        if (selector == null) return;
        selector.SetActive(selector.activeSelf); // Keep a safe, no-op placeholder hook.
    }
}
