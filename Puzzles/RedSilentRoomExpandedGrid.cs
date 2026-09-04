using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Red Tower silent-room challenge: expand both communicating boards and lengthen the sequence.
// Replace target names/signatures with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class RedSilentRoomExpandedGrid
{
    private const int GridSize = 5;
    private const int SequenceLength = 25;

    [HarmonyPostfix]
    [HarmonyPatch("RedSilentRoomPuzzle", "Initialize")]
    private static void InitializePostfix(Transform symbolMatrix, Transform outsideButtonBoard)
    {
        ExpandBoard(symbolMatrix, "Symbol", GridSize);
        ExpandBoard(outsideButtonBoard, "Button", GridSize);
    }

    [HarmonyPrefix]
    [HarmonyPatch("RedSilentRoomPuzzle", "BuildSequence")]
    private static bool BuildSequencePrefix(ref int[] __result)
    {
        var sequence = new int[SequenceLength];
        for (var i = 0; i < sequence.Length; i++) sequence[i] = Random.Range(0, GridSize * GridSize);
        __result = sequence;
        return false;
    }

    private static void ExpandBoard(Transform board, string childPrefix, int size)
    {
        // Uses authored row/column anchors when present; otherwise lays out a safe 5x5 grid.
        var spacing = 0.75f;
        for (var index = 0; index < size * size; index++)
        {
            var cell = board.Find($"{childPrefix}_{index}");
            if (cell == null) continue;
            var row = index / size;
            var column = index % size;
            cell.localPosition = new Vector3((column - (size - 1) / 2f) * spacing, 0f,
                (row - (size - 1) / 2f) * spacing);
            cell.gameObject.SetActive(true);
        }
    }
}
