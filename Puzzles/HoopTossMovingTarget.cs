using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Moves the hoop target vertically to make the toss timing dynamic.
// Replace target type/method names with those from the game's IL2CPP dump.
[HarmonyPatch]
internal static class HoopTossMovingTarget
{
    internal static ConfigEntry<float> Speed { get; private set; } = null!;
    internal static ConfigEntry<float> Amplitude { get; private set; } = null!;
    private static Vector3 _baseLocalPosition;
    private static bool _initialized;

    internal static void Bind(ConfigFile config)
    {
        Speed = config.Bind("Hoop Toss", "VerticalSpeed", 1.25f, "Hoop oscillation speed in cycles per second.");
        Amplitude = config.Bind("Hoop Toss", "VerticalAmplitude", 0.75f, "Hoop vertical travel amplitude in metres.");
    }

    [HarmonyPostfix]
    [HarmonyPatch("HoopTossTarget", "Start")]
    private static void StartPostfix(Transform __instance)
    {
        _baseLocalPosition = __instance.localPosition;
        _initialized = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch("HoopTossTarget", "Update")]
    private static void UpdatePostfix(Transform __instance)
    {
        if (!_initialized) return;
        var position = _baseLocalPosition;
        position.y += Mathf.Sin(Time.time * Speed.Value * Mathf.PI * 2f) * Amplitude.Value;
        __instance.localPosition = position;
    }
}
