using BepInEx.Configuration;

namespace HardWalk;

public enum HardWalkGameMode
{
    BigWalk,
    HardWalk
}

internal static class GameModeConfig
{
    internal const int MinimumHardWalkPlayers = 4;
    internal const string BigWalkDescription = "Big Walk (Vanilla) — play the standard puzzle mechanics.";
    internal const string HardWalkDescription = "Hard Walk (Modded Challenge) — enables Hard Walk puzzle mechanics; requires 4+ players.";

    internal static ConfigEntry<HardWalkGameMode> SelectedMode { get; private set; } = null!;

    internal static void Bind(ConfigFile config)
    {
        SelectedMode = config.Bind("Lobby", "GameMode", HardWalkGameMode.BigWalk,
            $"Host-selected mode. {BigWalkDescription} {HardWalkDescription}");
    }

    internal static bool IsHardWalkAllowed(int playerCount) => playerCount >= MinimumHardWalkPlayers;

    internal static bool IsHardWalkActive(int playerCount) =>
        SelectedMode.Value == HardWalkGameMode.HardWalk && IsHardWalkAllowed(playerCount);

    internal static void EnforcePlayerRequirement(int playerCount)
    {
        if (!IsHardWalkAllowed(playerCount) && SelectedMode.Value == HardWalkGameMode.HardWalk)
        {
            SelectedMode.Value = HardWalkGameMode.BigWalk;
            Plugin.Logger.LogInfo($"Hard Walk mode disabled: {MinimumHardWalkPlayers}+ players are required (current count: {playerCount}).");
        }
    }
}
