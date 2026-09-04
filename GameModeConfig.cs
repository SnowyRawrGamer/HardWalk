using BepInEx.Configuration;

namespace HardWalk;

public enum LobbyPlayerCountMode
{
    TwoPlayers,
    ThreePlayers,
    FourPlusPlayers
}

public enum HardWalkGameMode
{
    BigWalk,
    HardWalk
}

internal static class GameModeConfig
{
    internal const int MinimumHardWalkPlayers = 4;
    internal const string BigWalkDescription = "Big Walk (Vanilla) — play the standard puzzle mechanics.";
    internal const string HardWalkDescription = "Hard Walk (Modded Challenge) — enables Hard Walk puzzle mechanics for 4+ player lobbies.";
    internal const string HardWalkRequirementText = "Hard Walk is available only inside 4+ Players.";

    internal static ConfigEntry<LobbyPlayerCountMode> PlayerCountMode { get; private set; } = null!;
    internal static ConfigEntry<HardWalkGameMode> SelectedMode { get; private set; } = null!;

    internal static void Bind(ConfigFile config)
    {
        PlayerCountMode = config.Bind("Lobby", "PlayerCountMode", LobbyPlayerCountMode.FourPlusPlayers,
            "Vanilla lobby size: 2 Players, 3 Players, or 4+ Players.");
        SelectedMode = config.Bind("Lobby", "FourPlusGameMode", HardWalkGameMode.BigWalk,
            $"Mode used only by 4+ Players lobbies. {BigWalkDescription} {HardWalkDescription}");
    }

    internal static bool IsFourPlus(LobbyPlayerCountMode mode) => mode == LobbyPlayerCountMode.FourPlusPlayers;
    internal static bool IsHardWalkAllowed(LobbyPlayerCountMode mode, int playerCount) =>
        IsFourPlus(mode) && playerCount >= MinimumHardWalkPlayers;
    internal static bool IsHardWalkActive(LobbyPlayerCountMode mode, int playerCount) =>
        SelectedMode.Value == HardWalkGameMode.HardWalk && IsHardWalkAllowed(mode, playerCount);

    internal static void SelectPlayerCount(LobbyPlayerCountMode mode, int playerCount)
    {
        PlayerCountMode.Value = mode;
        if (!IsHardWalkAllowed(mode, playerCount) && SelectedMode.Value == HardWalkGameMode.HardWalk)
        {
            SelectedMode.Value = HardWalkGameMode.BigWalk;
            Plugin.Logger.LogInfo("Hard Walk disabled because the lobby is not a valid 4+ player lobby.");
        }
    }
}
