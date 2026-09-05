using HarmonyLib;
using UnityEngine;

namespace HardWalk;

// UI integration point for the Host a Game -> 4+ players screen. The selector is deliberately
// data-driven so the game's menu bridge can render it as native menu choices rather than a world
// object or an in-lobby physical button.
[HarmonyPatch]
internal static class LobbyGameModeSelector
{
    internal const string TwoPlayersLabel = "2 Players";
    internal const string ThreePlayersLabel = "3 Players";
    internal const string FourPlusPlayersLabel = "4+ Players";
    internal const string NormalWalkLabel = "Normal Walk";
    internal const string HardWalkLabel = "Hard Walk";

    internal static object[] BuildVerifiedModeOptions(LobbyPlayerCountMode playerMode, int playerCount)
    {
        if (!GameModeConfig.IsFourPlus(playerMode)) return System.Array.Empty<object>();
        return new object[]
        {
            new { Id = HardWalkGameMode.NormalWalk, Label = NormalWalkLabel, Description = GameModeConfig.NormalWalkDescription, Enabled = true },
            new { Id = HardWalkGameMode.HardWalk, Label = HardWalkLabel, Description = GameModeConfig.HardWalkDescription, Enabled = GameModeConfig.IsHardWalkAllowed(playerMode, playerCount) }
        };
    }

    // Host UI callback: called when the host selects a player-count option. The mode choices are
    // shown only after 4+ players is selected, and leaving that screen safely resets to Normal.
    [HarmonyPostfix]
    [HarmonyPatch("LobbyHostSettings", "OnPlayerCountChanged")]
    private static void PlayerCountChangedPostfix(LobbyPlayerCountMode mode, int playerCount)
    {
        GameModeConfig.SelectPlayerCount(mode, playerCount);
        var selector = GameObject.Find("LobbyHostSettings/ModeSelector");
        if (selector != null) selector.SetActive(GameModeConfig.IsFourPlus(mode));
    }

    // Host UI callback: selecting a native Normal Walk/Hard Walk option updates the active mode
    // before the room is created. Hard Walk can never leak into 2/3-player rooms.
    internal static bool SelectModeFromHostMenu(HardWalkGameMode mode, LobbyPlayerCountMode playerMode, int playerCount)
    {
        return GameModeConfig.SelectMode(mode, playerMode, playerCount);
    }
}
