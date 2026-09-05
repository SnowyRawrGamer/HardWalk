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
    NormalWalk,
    HardWalk
}

internal static class GameModeConfig
{
    internal const int MinimumHardWalkPlayers = 4;
    internal const string NormalWalkDescription = "Standard Walk — play the game's standard puzzle mechanics.";
    internal const string HardWalkDescription = "Overhauled difficulty — extra buttons, fakeouts, and moving puzzles for 4+ player groups.";
    internal const string HardWalkRequirementText = "Hard Walk is available only for 4+ player games.";

    internal static ConfigEntry<LobbyPlayerCountMode> PlayerCountMode { get; private set; } = null!;
    internal static ConfigEntry<HardWalkGameMode> SelectedMode { get; private set; } = null!;

    internal static void Bind(ConfigFile config)
    {
        PlayerCountMode = config.Bind("Lobby", "PlayerCountMode", LobbyPlayerCountMode.FourPlusPlayers,
            "Vanilla lobby size: 2 Players, 3 Players, or 4+ Players.");
        SelectedMode = config.Bind("Lobby", "FourPlayerGameMode", HardWalkGameMode.NormalWalk,
            $"Mode used by 4+ player lobbies. {NormalWalkDescription} {HardWalkDescription}");
    }

    internal static bool IsFourPlus(LobbyPlayerCountMode mode) => mode == LobbyPlayerCountMode.FourPlusPlayers;
    internal static bool IsHardWalkAllowed(LobbyPlayerCountMode mode, int playerCount) => IsFourPlus(mode) && playerCount >= MinimumHardWalkPlayers;
    internal static bool IsHardWalkActive(LobbyPlayerCountMode mode, int playerCount) => SelectedMode.Value == HardWalkGameMode.HardWalk && IsHardWalkAllowed(mode, playerCount);

    internal static void SelectPlayerCount(LobbyPlayerCountMode mode, int playerCount)
    {
        PlayerCountMode.Value = mode;
        if (!IsHardWalkAllowed(mode, playerCount)) SelectedMode.Value = HardWalkGameMode.NormalWalk;
    }

    internal static bool SelectMode(HardWalkGameMode mode, LobbyPlayerCountMode playerMode, int playerCount)
    {
        if (mode == HardWalkGameMode.HardWalk && !IsHardWalkAllowed(playerMode, playerCount))
        {
            SelectedMode.Value = HardWalkGameMode.NormalWalk;
            return false;
        }

        SelectedMode.Value = mode;
        return true;
    }
}
