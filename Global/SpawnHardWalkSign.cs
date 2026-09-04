using HarmonyLib;
using UnityEngine;

namespace HardWalk.Global;

// Adds a physical welcome sign to the true starting RedRoom/Gym spawn when Hard Walk is active.
// TODO: Verify the exact RedRoom/Gym identifiers and spawn transform against dumped IL2CPP metadata.
[HarmonyPatch]
internal static class SpawnHardWalkSign
{
    private const string SignName = "HardWalk_WelcomeWarningSign";
    private const string SignText = "Welcome to Hard Walk. This is a harder version of Big Walk designed for 4+ players by SnowyRawrGamer. It’s recommended to beat the normal game before playing Hard Walk. Good luck, you are going to need it.";

    [HarmonyPostfix]
    [HarmonyPatch("SpawnArea", "Initialize")]
    private static void InitializePostfix(Transform spawnArea, int playerCount)
    {
        if (!HardWalk.Plugin.AreHardWalkMechanicsEnabled(playerCount) || spawnArea == null) return;
        if (spawnArea.Find(SignName) != null) return;

        // The sign belongs at the indoor RedRoom/Gym starting area, not at the distant beach.
        // These candidate paths are placeholders until IL2CPP metadata confirms the real hierarchy.
        var gymSpawn = FindRedRoomGymSpawn(spawnArea);
        if (gymSpawn == null) return;

        var sign = new GameObject(SignName);
        sign.transform.SetParent(gymSpawn, false);
        sign.transform.localPosition = new Vector3(0f, 1.35f, 2.5f);
        sign.transform.localRotation = Quaternion.identity;

        var post = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        post.name = "Post";
        post.transform.SetParent(sign.transform, false);
        post.transform.localPosition = new Vector3(0f, -1.35f, 0f);
        post.transform.localScale = new Vector3(0.12f, 1.35f, 0.12f);

        var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
        board.name = "WarningBoard";
        board.transform.SetParent(sign.transform, false);
        board.transform.localPosition = Vector3.zero;
        board.transform.localScale = new Vector3(3.2f, 1.5f, 0.12f);

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
    }

    // Candidate identifiers to verify once IL2CPP metadata is available:
    // RedRoom, RedRoomSpawn, Gym, GymSpawn, StartRoom, PlayerStart, and RedRoom/Gym/PlayerSpawn.
    private static Transform? FindRedRoomGymSpawn(Transform spawnArea)
    {
        return spawnArea.Find("RedRoom/Gym/PlayerSpawn")
            ?? spawnArea.Find("RedRoom/GymSpawn")
            ?? spawnArea.Find("RedRoom/RedRoomSpawn")
            ?? spawnArea.Find("Gym/PlayerSpawn")
            ?? spawnArea.Find("Gym/GymSpawn")
            ?? spawnArea.Find("RedRoomSpawn")
            ?? spawnArea.Find("GymSpawn")
            ?? spawnArea.Find("PlayerStart");
    }
}
