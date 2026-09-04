using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace HardWalk;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BasePlugin
{
    public const string PluginGuid = "com.hardwalk.bigwalk";
    public const string PluginName = "Hard Walk";
    public const string PluginVersion = "0.1.0";
    internal static ManualLogSource Logger = null!;
    private Harmony? _harmony;

    public override void Load()
    {
        Logger = base.Log;
        GameModeConfig.Bind(Config);
        Puzzles.HoopTossMovingTarget.Bind(Config);
        Puzzles.CannonTimerWeakLaunch.Bind(Config);
        Puzzles.RedTowerKeyLaunch.Bind(Config);
        Puzzles.PuzzleContainerBabyLaunch.Bind(Config);
        Puzzles.GreenMinefieldStandsExpansion.Bind(Config);
        _harmony = new Harmony(PluginGuid);
        _harmony.PatchAll(); // Includes Green Tower moving train coordinate target behavior.
        Logger.LogInfo($"{PluginName} {PluginVersion} loaded. Hard Walk mode requires 4+ players.");
    }

    internal static bool AreHardWalkMechanicsEnabled(int playerCount) =>
        GameModeConfig.IsHardWalkActive(playerCount);
}
