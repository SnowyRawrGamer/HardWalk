using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Red Tower final key: launch the key off the tower after all babies are slotted.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class RedTowerKeyLaunch
{
    internal static ConfigEntry<float> LaunchImpulse { get; private set; } = null!;
    internal static ConfigEntry<float> DirectionRandomness { get; private set; } = null!;
    internal static ConfigEntry<Vector3> BaseDirection { get; private set; } = null!;

    internal static void Bind(ConfigFile config)
    {
        LaunchImpulse = config.Bind("Red Tower Key", "LaunchImpulse", 85f,
            "Extreme impulse applied to the final key after all babies are slotted.");
        DirectionRandomness = config.Bind("Red Tower Key", "DirectionRandomness", 0.35f,
            "Random horizontal/vertical direction variance applied to the key launch.");
        BaseDirection = config.Bind("Red Tower Key", "BaseDirection", new Vector3(0.8f, 1f, 0.2f),
            "Base launch direction, relative to the Red Tower.");
    }

    [HarmonyPostfix]
    [HarmonyPatch("RedTowerBabyPedestal", "OnAllBabiesSlotted")]
    private static void OnAllBabiesSlottedPostfix(Rigidbody finalKey)
    {
        LaunchKey(finalKey);
    }

    [HarmonyPostfix]
    [HarmonyPatch("RedTowerBabyPedestal", "SpawnFinalKey")]
    private static void SpawnFinalKeyPostfix(Rigidbody __result)
    {
        LaunchKey(__result);
    }

    private static void LaunchKey(Rigidbody key)
    {
        if (key == null) return;
        var direction = BaseDirection.Value.normalized;
        var randomOffset = Random.insideUnitSphere * Mathf.Max(0f, DirectionRandomness.Value);
        var launchDirection = (direction + randomOffset).normalized;

        key.velocity = Vector3.zero;
        key.angularVelocity = Vector3.zero;
        key.AddForce(launchDirection * Mathf.Max(0f, LaunchImpulse.Value), ForceMode.Impulse);
        key.AddTorque(Random.insideUnitSphere * Mathf.Max(0f, LaunchImpulse.Value * 0.15f), ForceMode.Impulse);
    }
}
