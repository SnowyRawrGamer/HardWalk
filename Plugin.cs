using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace HardWalk;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Plugin : BasePlugin
{
    public const string PluginGuid = "com.hardwalk.bigwalk";
    public const string PluginName = "Hard Walk";
    public const string PluginVersion = "1.0.0";
    internal static ManualLogSource Logger = null!;
    private Harmony _harmony;

    public override void Load()
    {
        Logger = base.Log;
        GameModeConfig.Bind(Config);
        _harmony = new Harmony(PluginGuid);

        // Puzzle patch classes currently contain speculative IL2CPP targets. Harmony's
        // PatchAll() discovers them by attribute and throws on the first missing target;
        // never let one optional patch prevent the plugin (and verified patches) loading.
        try
        {
            _harmony.PatchAll();
            Logger.LogInfo("Harmony patch registration completed.");
        }
        catch (Exception exception)
        {
            Logger.LogWarning($"One or more optional Harmony patches were skipped: {exception}");
        }

        try
        {
            if (LobbyGameModeSelector.TryPatch(_harmony))
                Logger.LogInfo("Lobby player-count patch installed.");
            else
                Logger.LogWarning("Lobby player-count API was not found; skipping optional lobby UI patch.");
        }
        catch (Exception exception)
        {
            Logger.LogWarning($"Lobby player-count patch failed; continuing without it: {exception}");
        }

        Logger.LogInfo($"{PluginName} {PluginVersion} loaded; optional patch failures do not abort plugin load.");
    }

    // Shared mode gate for world objects, including the v1.0.0 spawn wooden sign.
    internal static bool AreHardWalkMechanicsEnabled(int playerCount)
    {
        return GameModeConfig.IsHardWalkActive(GameModeConfig.PlayerCountMode.Value, playerCount);
    }
}
