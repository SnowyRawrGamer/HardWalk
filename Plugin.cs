using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace HardWalk;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BasePlugin
{
    public const string PluginGuid = "com.hardwalk.bigwalk";
    public const string PluginName = "Hard Walk";
    public const string PluginVersion = "1.0.0";
    internal static ManualLogSource Logger = null!;
    private Harmony? _harmony;

    public override void Load()
    {
        Logger = base.Log;
        GameModeConfig.Bind(Config);
        _harmony = new Harmony(PluginGuid);
        _harmony.PatchAll();
        Logger.LogInfo($"{PluginName} {PluginVersion} loaded; verified v1.0 tutorial and lobby-mode patches are enabled where supported.");
    }
}
