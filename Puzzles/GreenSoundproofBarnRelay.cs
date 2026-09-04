using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Green Tower Soundproof Barn: Room 1 -> Room 2 -> Room 3 relay.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class GreenSoundproofBarnRelay
{
    private const string Room1Name = "GreenBarn_Room1_SoundproofCode";
    private const string Room2Name = "GreenBarn_Room2_Relay";
    private const string Room3Name = "GreenBarn_Room3_Pegboard";
    private const string DividerName = "HardWalk_GreenBarn_Room1Room3Divider";

    [HarmonyPostfix]
    [HarmonyPatch("GreenSoundproofBarnPuzzle", "Initialize")]
    private static void InitializePostfix(Transform barnRoot)
    {
        if (barnRoot == null) return;
        var room1 = barnRoot.Find(Room1Name);
        var room2 = barnRoot.Find(Room2Name);
        var room3 = barnRoot.Find(Room3Name);
        if (room1 == null || room2 == null || room3 == null) return;

        // Room 1 is behind soundproof glass: Room 2 can observe it, but Room 1 cannot speak out.
        SetVoiceChannel(room1, "Room1ToRoom2", false);
        SetVoiceChannel(room1, "Room1ToRoom3", false);
        SetVoiceChannel(room2, "Room2ToRoom3", true);
        SetVoiceChannel(room2, "Room2ToRoom1", false);

        // Remove any old visual-mask placeholder and enforce sight separation structurally instead.
        RemoveLegacyBlindfolds(room3);
        SetDirectView(room3, false);
        SpawnRoomDivider(barnRoot, room1, room2, room3);
    }

    [HarmonyPostfix]
    [HarmonyPatch("GreenSoundproofBarnPuzzle", "OnPlayerEnteredRoom")]
    private static void PlayerEnteredRoomPostfix(Component player, Transform barnRoot)
    {
        // No mask is equipped. The authored divider and room layout provide the occlusion.
        if (player == null || barnRoot == null) return;
        SetDirectView(barnRoot.Find(Room3Name), false);
    }

    private static void SpawnRoomDivider(Transform barnRoot, Transform room1, Transform room2, Transform room3)
    {
        if (barnRoot.Find(DividerName) != null) return;

        var divider = GameObject.CreatePrimitive(PrimitiveType.Cube);
        divider.name = DividerName;
        divider.transform.SetParent(barnRoot, true);
        divider.transform.position = Vector3.Lerp(room1.position, room3.position, 0.5f);
        divider.transform.localScale = new Vector3(2.5f, 3.5f, 0.35f);
        divider.transform.rotation = Quaternion.LookRotation((room2.position - divider.transform.position).normalized, Vector3.up);
        // The collider is intentionally solid. Room 2 remains on the relay side with sightlines to both rooms.
    }

    private static void SetVoiceChannel(Transform room, string channelName, bool enabled)
    {
        var channel = room?.Find(channelName);
        if (channel != null) channel.gameObject.SetActive(enabled);
    }

    private static void SetDirectView(Transform room3, bool enabled)
    {
        if (room3 == null) return;
        var directView = room3.Find("DirectViewToRoom1");
        if (directView != null) directView.gameObject.SetActive(enabled);
    }

    private static void RemoveLegacyBlindfolds(Transform room3)
    {
        if (room3 == null) return;
        foreach (var child in room3.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "HardWalk_GreenBarn_Blindfold") Object.Destroy(child.gameObject);
        }
    }
}
