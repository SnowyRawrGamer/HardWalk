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
        _harmony = new Harmony(PluginGuid);
        _harmony.PatchAll(); // Includes the Puzzles.KeyGrinderScatter patch.
        Logger.LogInfo($"{PluginName} {PluginVersion} loaded.");
    }
}
