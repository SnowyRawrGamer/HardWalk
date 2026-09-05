using HarmonyLib;
using UnityEngine;

namespace HardWalk.Global;

// Creates the Hard Walk welcome/warning sign at the indoor RedRoom/Gym spawn.
// The sign is deliberately self-contained so it works with the game's existing scene objects.
[HarmonyPatch]
internal static class SpawnHardWalkSign
{
    private const string SignName = "HardWalk_WelcomeWarningSign";
    private const string BoardName = "HardWalk_WoodenSign";
    private const string SignText = "HARD WALK\n\nA harder version of Big Walk\nfor 4+ players.\n\nBeat Normal Walk first.\nGood luck — you are going to need it.";
    private const string InteractionText = "HARD WALK\n\n4+ players • harder puzzles\n\nNormal Walk is recommended first.\nClick the sign to read this warning again.";

    [HarmonyPostfix]
    [HarmonyPatch("SpawnArea", "Initialize")]
    private static void InitializePostfix(Transform spawnArea, int playerCount)
    {
        if (spawnArea == null || !Plugin.AreHardWalkMechanicsEnabled(playerCount)) return;
        CreateOrUpdateSign(spawnArea);
    }

    private static void CreateOrUpdateSign(Transform spawnArea)
    {
        var spawn = FindRedRoomGymSpawn(spawnArea);
        if (spawn == null) return;

        var existing = spawnArea.Find(SignName);
        if (existing != null)
        {
            existing.SetParent(spawn, false);
            return;
        }

        var sign = new GameObject(SignName);
        sign.transform.SetParent(spawn, false);
        sign.transform.localPosition = new Vector3(0f, 1.35f, 2.5f);
        sign.transform.localRotation = Quaternion.identity;

        var post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        post.name = "Post";
        post.transform.SetParent(sign.transform, false);
        post.transform.localPosition = new Vector3(0f, -1.35f, 0f);
        post.transform.localScale = new Vector3(0.12f, 1.35f, 0.12f);

        var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = BoardName;
        board.transform.SetParent(sign.transform, false);
        board.transform.localPosition = Vector3.zero;
        board.transform.localScale = new Vector3(3.2f, 1.5f, 0.12f);
        board.GetComponent<Renderer>().material.color = new Color(0.28f, 0.12f, 0.035f);

        var textObject = new GameObject("WarningText");
        textObject.transform.SetParent(sign.transform, false);
        textObject.transform.localPosition = new Vector3(0f, 0f, -0.08f);
        textObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        textObject.transform.localScale = Vector3.one * 0.08f;
        var text = textObject.AddComponent<TextMesh>();
        text.text = SignText;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.fontSize = 48;
        text.characterSize = 0.08f;
        text.color = Color.yellow;

        var interaction = board.AddComponent<HardWalkSignInteraction>();
        interaction.SetText(text);
        Plugin.Logger.LogInfo("Hard Walk v1.0.0 spawn wooden sign created at the RedRoom/Gym spawn.");
    }

    private static Transform? FindRedRoomGymSpawn(Transform root)
    {
        return root.Find("RedRoom/Gym/PlayerSpawn")
            ?? root.Find("RedRoom/GymSpawn")
            ?? root.Find("RedRoom/RedRoomSpawn")
            ?? root.Find("Gym/PlayerSpawn")
            ?? root.Find("Gym/GymSpawn")
            ?? root.Find("RedRoomSpawn")
            ?? root.Find("GymSpawn")
            ?? root.Find("PlayerStart");
    }

    private sealed class HardWalkSignInteraction : MonoBehaviour
    {
        private TextMesh? _text;
        private bool _showingInteractionText;

        internal void SetText(TextMesh text) => _text = text;

        private void OnMouseDown()
        {
            if (_text == null) return;
            _showingInteractionText = !_showingInteractionText;
            _text.text = _showingInteractionText ? InteractionText : SignText;
        }
    }
}
