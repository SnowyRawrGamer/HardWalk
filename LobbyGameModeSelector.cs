using HarmonyLib;
using UnityEngine;

namespace HardWalk;

// UI/network method names are placeholders until confirmed against the game's IL2CPP dump.
// Big Walk's player-count selector remains the top-level choice. Hard Walk is nested under 4+.
[HarmonyPatch]
internal static class LobbyGameModeSelector
{
    internal const string TwoPlayersLabel = "2 Players";
    internal const string ThreePlayersLabel = "3 Players";
    internal const string FourPlusPlayersLabel = "4+ Players";
    internal const string BigWalkLabel = "Big Walk";
    internal const string HardWalkLabel = "Hard Walk";

    [HarmonyPostfix]
    [HarmonyPatch("LobbyHostSettings", "OnPlayerCountChanged")]
    private static void PlayerCountChangedPostfix(LobbyPlayerCountMode mode, int playerCount)
    {
        GameModeConfig.SelectPlayerCount(mode, playerCount);
        RefreshModeSelector(mode, playerCount);
    }

    [HarmonyPrefix]
    [HarmonyPatch("LobbyHostSettings", "SetPlayerCountMode")]
    private static bool SetPlayerCountModePrefix(LobbyPlayerCountMode mode, int playerCount, ref bool __result)
    {
        GameModeConfig.SelectPlayerCount(mode, playerCount);
        RefreshModeSelector(mode, playerCount);
        __result = true;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("LobbyHostSettings", "SetGameMode")]
    private static bool SetGameModePrefix(HardWalkGameMode requestedMode, LobbyPlayerCountMode playerMode, int playerCount, ref bool __result)
    {
        if (requestedMode == HardWalkGameMode.HardWalk && !GameModeConfig.IsHardWalkAllowed(playerMode, playerCount))
        {
            GameModeConfig.SelectedMode.Value = HardWalkGameMode.BigWalk;
            __result = false;
            return false;
        }

        GameModeConfig.SelectedMode.Value = requestedMode;
        __result = true;
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch("LobbyHostSettings", "BuildPlayerCountOptions")]
    private static void BuildPlayerCountOptionsPostfix(ref object[] __result)
    {
        __result = new object[]
        {
            new { Id = LobbyPlayerCountMode.TwoPlayers, Label = TwoPlayersLabel, Enabled = true },
            new { Id = LobbyPlayerCountMode.ThreePlayers, Label = ThreePlayersLabel, Enabled = true },
            new { Id = LobbyPlayerCountMode.FourPlusPlayers, Label = FourPlusPlayersLabel, Enabled = true }
        };
    }

    [HarmonyPostfix]
    [HarmonyPatch("LobbyHostSettings", "BuildGameModeOptions")]
    private static void BuildGameModeOptionsPostfix(LobbyPlayerCountMode playerMode, int playerCount, ref object[] __result)
    {
        if (!GameModeConfig.IsFourPlus(playerMode))
        {
            __result = System.Array.Empty<object>();
            return;
        }

        __result = new object[]
        {
            new { Id = HardWalkGameMode.BigWalk, Label = BigWalkLabel, Description = GameModeConfig.BigWalkDescription, Enabled = true },
            new { Id = HardWalkGameMode.HardWalk, Label = HardWalkLabel, Description = GameModeConfig.HardWalkDescription, Enabled = GameModeConfig.IsHardWalkAllowed(playerMode, playerCount) }
        };
    }

    private static void RefreshModeSelector(LobbyPlayerCountMode playerMode, int playerCount)
    {
        var selector = GameObject.Find("LobbyHostSettings/ModeSelector");
        if (selector == null) return;
        // Replace with the game's UI bridge: hide the nested mode selector for 2/3 Players,
        // and show/disable the Hard Walk option based on the 4+ player requirement.
        selector.SetActive(GameModeConfig.IsFourPlus(playerMode));
    }
}
