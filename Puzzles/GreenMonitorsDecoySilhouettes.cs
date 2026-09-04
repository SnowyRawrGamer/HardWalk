using System;
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Blue Signal Monitors: injects misleading frames, while only standard black head silhouettes count.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class GreenMonitorsDecoySilhouettes
{
    private enum FrameKind { GenuineBlackHead, RedAnimalOutline, OrangeAnimalSilhouette, UpsideDownHead, Lookalike }
    private static int _genuineCount;

    [HarmonyPostfix]
    [HarmonyPatch("GreenMonitorPuzzle", "Initialize")]
    private static void InitializePostfix() => _genuineCount = 0;

    [HarmonyPrefix]
    [HarmonyPatch("BlueSignalMonitor", "BuildFlashSequence")]
    private static void BuildFlashSequencePrefix(ref object[] __result)
    {
        // Placeholder sequence hook: the production implementation should clone the game's
        // monitor-frame type and insert decoy visuals between genuine frames.
        if (__result == null || __result.Length == 0) return;
        var frames = new object[__result.Length * 2];
        for (var i = 0; i < __result.Length; i++)
        {
            frames[i * 2] = __result[i];
            frames[i * 2 + 1] = CreateDecoyFrame(i);
        }
        __result = frames;
    }

    [HarmonyPrefix]
    [HarmonyPatch("GreenMonitorPuzzle", "EvaluateFrame")]
    private static bool EvaluateFramePrefix(object frame, ref bool __result)
    {
        var kind = ClassifyFrame(frame);
        __result = kind == FrameKind.GenuineBlackHead;
        if (__result) _genuineCount++;
        // Suppress decoys from the target sum while allowing genuine frames through.
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("GreenMonitorPuzzle", "EvaluateTargetSum")]
    private static bool EvaluateTargetSumPrefix(ref int __result)
    {
        __result = _genuineCount;
        return false;
    }

    private static object CreateDecoyFrame(int index)
    {
        // Placeholder marker consumed by the eventual monitor-frame adapter.
        return new DecoyFrame((FrameKind)(index % 4 + 1));
    }

    private static FrameKind ClassifyFrame(object frame)
    {
        if (frame is DecoyFrame decoy) return decoy.Kind;
        return FrameKind.GenuineBlackHead;
    }

    private sealed class DecoyFrame
    {
        internal readonly FrameKind Kind;
        internal DecoyFrame(FrameKind kind) => Kind = kind;
    }
}
