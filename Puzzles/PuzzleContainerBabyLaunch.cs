using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// v1.0 verified symbol: PeckCondition. The existing release patches remain as v1.1 reference skeletons.
[HarmonyPatch]
internal static class PuzzleContainerBabyLaunch
{
    internal const string VerifiedPuzzleSymbol = "PeckCondition";
    internal static ConfigEntry<float> LaunchForce { get; private set; } = null!;
    internal static ConfigEntry<float> UpwardArc { get; private set; } = null!;
    internal static ConfigEntry<float> ForwardArc { get; private set; } = null!;
    internal static ConfigEntry<float> DirectionRandomness { get; private set; } = null!;

    internal static void Bind(ConfigFile config)
    {
        LaunchForce = config.Bind("Puzzle Baby Release", "LaunchForce", 5f, "Impulse applied when a completed puzzle container releases a baby.");
        UpwardArc = config.Bind("Puzzle Baby Release", "UpwardArc", 0.8f, "Upward component of the release arc.");
        ForwardArc = config.Bind("Puzzle Baby Release", "ForwardArc", 1.2f, "Forward component of the release arc.");
        DirectionRandomness = config.Bind("Puzzle Baby Release", "DirectionRandomness", 0.12f, "Direction variance.");
    }

    // v1.1 reference skeleton: exact ReleaseBaby signatures remain intentionally preserved.
    [HarmonyPostfix, HarmonyPatch("PuzzleCompletionContainer", "ReleaseBaby")]
    private static void ReleaseBabyPostfix(Rigidbody baby, Transform releasePoint) => Launch(baby, releasePoint);

    private static void Launch(Rigidbody baby, Transform releasePoint)
    {
        if (baby == null) return;
        var basis = releasePoint != null ? releasePoint : baby.transform;
        var direction = basis.up * Mathf.Max(0f, UpwardArc.Value) + basis.forward * Mathf.Max(0f, ForwardArc.Value) + Random.insideUnitSphere * Mathf.Max(0f, DirectionRandomness.Value);
        baby.transform.SetParent(null, true); baby.isKinematic = false; baby.detectCollisions = true; baby.velocity = Vector3.zero;
        baby.AddForce(direction.normalized * Mathf.Max(0f, LaunchForce.Value), ForceMode.Impulse);
    }
}
