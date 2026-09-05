using System;
using HarmonyLib;
using UnityEngine;

namespace HardWalk;

// Optional UI integration point. The game has changed the lobby selector type/signature between
// builds, so this patch is installed dynamically and skipped when the target is unavailable.
internal static class LobbyGameModeSelector
{
    internal const string TwoPlayersLabel = "2 Players";
    internal const string ThreePlayersLabel = "3 Players";
    internal const string FourPlusPlayersLabel = "4+ Players";
    internal const string NormalWalkLabel = "Normal Walk";
    internal const string HardWalkLabel = "Hard Walk";

    internal static object[] BuildVerifiedModeOptions(LobbyPlayerCountMode playerMode, int playerCount)
    {
        if (!GameModeConfig.IsFourPlus(playerMode)) return Array.Empty<object>();
        return new object[]
        {
            new { Id = HardWalkGameMode.NormalWalk, Label = NormalWalkLabel, Description = GameModeConfig.NormalWalkDescription, Enabled = true },
            new { Id = HardWalkGameMode.HardWalk, Label = HardWalkLabel, Description = GameModeConfig.HardWalkDescription, Enabled = GameModeConfig.IsHardWalkAllowed(playerMode, playerCount) }
        };
    }

    internal static bool TryPatch(Harmony harmony)
    {
        var targetType = AccessTools.TypeByName("LobbyHostSettings")
            ?? AccessTools.TypeByName("HardWalk.LobbyHostSettings");
        var target = targetType == null ? null : AccessTools.Method(targetType, "OnPlayerCountChanged");
        if (target == null) return false;

        harmony.Patch(target, postfix: new HarmonyMethod(typeof(LobbyGameModeSelector), nameof(PlayerCountChangedPostfix)));
        return true;
    }

    // Use object for the mode because the game's enum is not part of the mod assembly and has
    // changed across builds. This avoids a hard signature dependency while retaining the callback.
    private static void PlayerCountChangedPostfix(object mode, int playerCount)
    {
        if (!Enum.TryParse(mode?.ToString(), ignoreCase: true, out LobbyPlayerCountMode parsedMode)) return;
        GameModeConfig.SelectPlayerCount(parsedMode, playerCount);
        var selector = GameObject.Find("LobbyHostSettings/ModeSelector");
        if (selector != null) selector.SetActive(GameModeConfig.IsFourPlus(parsedMode));
    }

    internal static bool SelectModeFromHostMenu(HardWalkGameMode mode, LobbyPlayerCountMode playerMode, int playerCount)
    {
        return GameModeConfig.SelectMode(mode, playerMode, playerCount);
    }
}
