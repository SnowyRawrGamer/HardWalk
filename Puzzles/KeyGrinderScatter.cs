using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Skeleton for relocating the key grinder route across the starting island.
// Replace placeholder type/method names and route markers with names from the IL2CPP dump.
[HarmonyPatch]
internal static class KeyGrinderScatter
{
    private static readonly Vector3[] RouteFallback =
    {
        new(-8f, 0f, -12f), // Near spawn.
        new(18f, 0f, -6f),
        new(36f, 0f, 14f),
        new(54f, 0f, 32f)
    };

    private static Transform[] _grinders = System.Array.Empty<Transform>();
    private static Transform[] _guidanceArrows = System.Array.Empty<Transform>();
    private static int _activeStep;

    [HarmonyPostfix]
    [HarmonyPatch("StartingIslandKeyPuzzle", "Initialize")]
    private static void InitializePostfix(Transform[] grinders, Transform[] guidanceArrows)
    {
        _grinders = grinders ?? System.Array.Empty<Transform>();
        _guidanceArrows = guidanceArrows ?? System.Array.Empty<Transform>();
        _activeStep = 0;

        for (var i = 0; i < _grinders.Length; i++)
        {
            var position = ResolveReachablePosition(i);
            _grinders[i].position = position;
            _grinders[i].gameObject.SetActive(true);
        }

        UpdateGuidanceArrows();
    }

    [HarmonyPostfix]
    [HarmonyPatch("StartingIslandKeyPuzzle", "OnGrinderCompleted")]
    private static void OnGrinderCompletedPostfix(int grinderIndex)
    {
        if (_grinders.Length == 0) return;
        _activeStep = Mathf.Clamp(Mathf.Max(_activeStep, grinderIndex + 1), 0, _grinders.Length - 1);
        UpdateGuidanceArrows();
    }

    private static Vector3 ResolveReachablePosition(int index)
    {
        // Prefer authored island route markers. The fallback keeps the skeleton usable while
        // the game's real marker component/type is identified.
        var marker = GameObject.Find($"KeyGrinderRoute_{index}");
        if (marker != null) return marker.transform.position;

        var fallback = RouteFallback[Mathf.Clamp(index, 0, RouteFallback.Length - 1)];
        if (Physics.Raycast(fallback + Vector3.up * 25f, Vector3.down, out var hit, 50f))
            return hit.point;
        return fallback;
    }

    private static void UpdateGuidanceArrows()
    {
        if (_grinders.Length == 0) return;
        var destination = _grinders[Mathf.Clamp(_activeStep, 0, _grinders.Length - 1)];

        foreach (var arrow in _guidanceArrows)
        {
            if (arrow == null) continue;
            arrow.gameObject.SetActive(true);
            var direction = destination.position - arrow.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.001f)
                arrow.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }
    }
}
