using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Red Tower final key: send the key into a visible, recoverable 20-40 metre arc around the tower base.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class RedTowerKeyLaunch
{
    internal static ConfigEntry<float> LaunchImpulse { get; private set; } = null!;
    internal static ConfigEntry<float> VerticalArc { get; private set; } = null!;
    internal static ConfigEntry<float> DirectionRandomness { get; private set; } = null!;
    internal static ConfigEntry<Vector3> BaseDirection { get; private set; } = null!;

    internal static void Bind(ConfigFile config)
    {
        // Tuned down from the original extreme launch: with typical key mass this is intended
        // to land roughly 20-40m from the tower, rather than crossing the island or ocean.
        LaunchImpulse = config.Bind("Red Tower Key", "LaunchImpulse", 28f,
            "Recoverable launch impulse for the final key; target landing range is approximately 20-40m.");
        VerticalArc = config.Bind("Red Tower Key", "VerticalArc", 0.28f,
            "Vertical arc added to the launch direction. Keep modest so the key remains recoverable.");
        DirectionRandomness = config.Bind("Red Tower Key", "DirectionRandomness", 0.12f,
            "Small horizontal direction variance around the tower base.");
        BaseDirection = config.Bind("Red Tower Key", "BaseDirection", new Vector3(1f, 0f, 0f),
            "Horizontal direction away from the tower toward its island base area.");
    }

    [HarmonyPostfix]
    [HarmonyPatch("RedTowerBabyPedestal", "OnAllBabiesSlotted")]
    private static void OnAllBabiesSlottedPostfix(Rigidbody finalKey) => LaunchKey(finalKey);

    [HarmonyPostfix]
    [HarmonyPatch("RedTowerBabyPedestal", "SpawnFinalKey")]
    private static void SpawnFinalKeyPostfix(Rigidbody __result) => LaunchKey(__result);

    private static void LaunchKey(Rigidbody key)
    {
        if (key == null) return;
        var horizontal = new Vector3(BaseDirection.Value.x, 0f, BaseDirection.Value.z);
        if (horizontal.sqrMagnitude < 0.001f) horizontal = Vector3.forward;
        horizontal.Normalize();

        // Keep randomness horizontal so the key does not get an ocean-directed vertical shot.
        var random = Random.insideUnitCircle * Mathf.Max(0f, DirectionRandomness.Value);
        var direction = (horizontal + new Vector3(random.x, 0f, random.y)).normalized;
        direction.y = Mathf.Clamp(VerticalArc.Value, 0f, 0.8f);
        direction.Normalize();

        key.velocity = Vector3.zero;
        key.angularVelocity = Vector3.zero;
        key.AddForce(direction * Mathf.Clamp(LaunchImpulse.Value, 0f, 45f), ForceMode.Impulse);
        key.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
    }
}
