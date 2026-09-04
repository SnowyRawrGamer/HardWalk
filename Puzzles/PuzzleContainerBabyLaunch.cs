using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Applies a consistent upward/forward pop when a puzzle container releases a baby.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class PuzzleContainerBabyLaunch
{
    internal static ConfigEntry<float> LaunchForce { get; private set; } = null!;
    internal static ConfigEntry<float> UpwardArc { get; private set; } = null!;
    internal static ConfigEntry<float> ForwardArc { get; private set; } = null!;
    internal static ConfigEntry<float> DirectionRandomness { get; private set; } = null!;

    internal static void Bind(ConfigFile config)
    {
        LaunchForce = config.Bind("Puzzle Baby Release", "LaunchForce", 5f,
            "Impulse applied when a completed puzzle container releases a baby.");
        UpwardArc = config.Bind("Puzzle Baby Release", "UpwardArc", 0.8f,
            "Upward component of the release arc.");
        ForwardArc = config.Bind("Puzzle Baby Release", "ForwardArc", 1.2f,
            "Forward component of the release arc relative to the container.");
        DirectionRandomness = config.Bind("Puzzle Baby Release", "DirectionRandomness", 0.12f,
            "Small random direction variance applied to each released baby.");
    }

    [HarmonyPostfix]
    [HarmonyPatch("PuzzleCompletionContainer", "ReleaseBaby")]
    private static void ReleaseBabyPostfix(Rigidbody baby, Transform releasePoint)
        => Launch(baby, releasePoint);

    [HarmonyPostfix]
    [HarmonyPatch("PuzzleCompletionCage", "OpenAndRelease")]
    private static void CageReleasePostfix(Rigidbody baby, Transform releasePoint)
        => Launch(baby, releasePoint);

    [HarmonyPostfix]
    [HarmonyPatch("BabyDispenser", "DispenseBaby")]
    private static void DispenserReleasePostfix(Rigidbody baby, Transform releasePoint)
        => Launch(baby, releasePoint);

    private static void Launch(Rigidbody baby, Transform releasePoint)
    {
        if (baby == null) return;
        var basis = releasePoint != null ? releasePoint : baby.transform;
        var random = Random.insideUnitSphere * Mathf.Max(0f, DirectionRandomness.Value);
        var direction = (basis.up * Mathf.Max(0f, UpwardArc.Value))
                      + (basis.forward * Mathf.Max(0f, ForwardArc.Value))
                      + random;
        if (direction.sqrMagnitude < 0.001f) direction = Vector3.up;

        baby.transform.SetParent(null, true);
        baby.isKinematic = false;
        baby.detectCollisions = true;
        baby.velocity = Vector3.zero;
        baby.angularVelocity = Vector3.zero;
        baby.AddForce(direction.normalized * Mathf.Max(0f, LaunchForce.Value), ForceMode.Impulse);
    }
}
