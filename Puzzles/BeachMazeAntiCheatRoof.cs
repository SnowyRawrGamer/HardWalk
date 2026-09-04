using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Red Tower beach maze: closes aerial/item-throw shortcuts while preserving the authored passages.
// Replace target names/signatures with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class BeachMazeAntiCheatRoof
{
    private const float RoofHeight = 8f;
    private const float RoofThickness = 0.25f;

    [HarmonyPostfix]
    [HarmonyPatch("BeachMazePuzzle", "Initialize")]
    private static void InitializePostfix(Transform mazeRoot, Bounds mazeBounds)
    {
        if (mazeRoot == null) return;
        var roof = new GameObject("HardWalk_BeachMazeAntiCheatRoof");
        roof.transform.SetParent(mazeRoot, false);
        roof.transform.position = new Vector3(mazeBounds.center.x, mazeBounds.max.y + RoofHeight, mazeBounds.center.z);

        var collider = roof.AddComponent<BoxCollider>();
        collider.size = new Vector3(mazeBounds.size.x, RoofThickness, mazeBounds.size.z);
        collider.isTrigger = false;

        var mesh = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mesh.name = "HardWalk_BeachMazeCeiling";
        mesh.transform.SetParent(roof.transform, false);
        mesh.transform.localScale = new Vector3(mazeBounds.size.x, mazeBounds.size.z, 1f);
        mesh.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        Object.Destroy(mesh.GetComponent<Collider>());
    }

    [HarmonyPrefix]
    [HarmonyPatch("BeachMazePuzzle", "CanFinish")]
    private static bool RequireMazeRoutePrefix(bool physicallyRoutedThroughPassages, ref bool __result)
    {
        __result = physicallyRoutedThroughPassages;
        return false;
    }
}
