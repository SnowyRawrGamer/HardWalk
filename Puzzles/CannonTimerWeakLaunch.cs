using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Red Tower cannon timer puzzle: reduce the launch so the timer drops into a difficult recovery spot.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class CannonTimerWeakLaunch
{
    internal static ConfigEntry<float> LaunchForceMultiplier { get; private set; } = null!;

    internal static void Bind(ConfigFile config)
    {
        LaunchForceMultiplier = config.Bind(
            "Cannon Timer",
            "LaunchForceMultiplier",
            0.22f,
            "Multiplier applied to the cannon timer projectile launch impulse. Lower values create a weak, awkward launch.");
    }

    [HarmonyPrefix]
    [HarmonyPatch("CannonTimerPuzzle", "LaunchTimerProjectile")]
    private static bool LaunchTimerProjectilePrefix(Rigidbody projectile, Vector3 launchVelocity)
    {
        if (projectile == null) return false;

        projectile.velocity = Vector3.zero;
        projectile.angularVelocity = Vector3.zero;
        projectile.AddForce(launchVelocity * LaunchForceMultiplier.Value, ForceMode.VelocityChange);
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("CannonTimerPuzzle", "ApplyLaunchImpulse")]
    private static void ApplyLaunchImpulsePrefix(ref Vector3 impulse)
    {
        impulse *= LaunchForceMultiplier.Value;
    }
}
