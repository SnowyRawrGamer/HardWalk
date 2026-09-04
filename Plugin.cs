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
        // All game-targeting patches are intentionally disabled until matched against the
        // shipped Unity 6000.3.17f1 / metadata-v39 Steam dump. PatchAll is safe here because
        // unverified patch classes are excluded with HARDWALK_ENABLE_UNVERIFIED_PATCHES.
        _harmony.PatchAll();
        Logger.LogInfo($"{PluginName} {PluginVersion} loaded; no unverified game patches enabled.");
    }
}
