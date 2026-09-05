using HarmonyLib;
using UnityEngine;

namespace HardWalk;

// v1.0 UI data for the 4-player mode choice. The original target-method skeletons remain below
// as reference until the installed IL2CPP dump confirms the game's concrete UI bridge.
[HarmonyPatch]
internal static class LobbyGameModeSelector
{
    internal const string TwoPlayersLabel = "2 Players";
    internal const string ThreePlayersLabel = "3 Players";
    internal const string FourPlusPlayersLabel = "4 Players";
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

    // v1.1 reference skeleton: keep these placeholder patches until signatures are verified.
    [HarmonyPostfix]
    [HarmonyPatch("LobbyHostSettings", "OnPlayerCountChanged")]
    private static void PlayerCountChangedPostfix(LobbyPlayerCountMode mode, int playerCount)
    {
        GameModeConfig.SelectPlayerCount(mode, playerCount);
        var selector = GameObject.Find("LobbyHostSettings/ModeSelector");
        if (selector != null) selector.SetActive(GameModeConfig.IsFourPlus(mode));
    }
}
