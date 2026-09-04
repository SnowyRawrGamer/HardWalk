using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace HardWalk.Puzzles;

// Green Tower coordinates puzzle: the target is the moving Blue Train caboose.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class GreenCoordsMovingTrainTarget
{
    private const string TrainName = "BlueTrain";
    private const string CabooseButtonName = "HardWalk_CabooseReleaseButton";
    private const string BoardText = "Back of Blue Train";

    [HarmonyPostfix]
    [HarmonyPatch("GreenCoordinatesPuzzle", "Initialize")]
    private static void InitializePostfix(Transform puzzleHub, Transform blueTrain)
    {
        if (blueTrain == null) return;
        var caboose = FindRearCaboose(blueTrain);
        if (caboose == null) return;

        var button = caboose.Find(CabooseButtonName);
        if (button == null)
        {
            button = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            button.name = CabooseButtonName;
            button.SetParent(caboose, false);
            button.localPosition = Vector3.back * 1.5f + Vector3.up * 1.1f;
            button.localScale = new Vector3(0.45f, 0.25f, 0.12f);
        }

        var trigger = button.GetComponent<TrainReleaseButton>() ?? button.gameObject.AddComponent<TrainReleaseButton>();
        trigger.ReleasePuzzle = puzzleHub?.GetComponent<Component>();
        UpdateBoard(puzzleHub);
    }

    [HarmonyPostfix]
    [HarmonyPatch("BlueTrain", "Update")]
    private static void TrainUpdatePostfix(Transform __instance)
    {
        var board = GameObject.Find("GreenCoordinatesPuzzle/CoordinateDisplayBoard");
        UpdateBoard(board?.transform);
    }

    private static Transform? FindRearCaboose(Transform train)
    {
        var caboose = train.Find("RearCaboose") ?? train.Find("Caboose") ?? train.Find("Cars/Rear");
        return caboose ?? train;
    }

    private static void UpdateBoard(Transform board)
    {
        if (board == null) return;
        foreach (var text in board.GetComponentsInChildren<Text>(true)) text.text = BoardText;
        foreach (var text in board.GetComponentsInChildren<TMPro.TMP_Text>(true)) text.text = BoardText;
    }

    private sealed class TrainReleaseButton : MonoBehaviour
    {
        internal Component? ReleasePuzzle;
        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player") || ReleasePuzzle == null) return;
            var method = ReleasePuzzle.GetType().GetMethod("OnTrainButtonPressed");
            method?.Invoke(ReleasePuzzle, null);
        }
    }
}
