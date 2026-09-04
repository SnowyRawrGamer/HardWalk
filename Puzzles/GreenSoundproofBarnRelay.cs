using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Green Tower Soundproof Barn: three rooms enforce a one-way charades/voice relay.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class GreenSoundproofBarnRelay
{
    private const string Room1Name = "GreenBarn_Room1_SoundproofCode";
    private const string Room2Name = "GreenBarn_Room2_Relay";
    private const string Room3Name = "GreenBarn_Room3_Pegboard";
    private const string BlindfoldName = "HardWalk_GreenBarn_Blindfold";

    [HarmonyPostfix]
    [HarmonyPatch("GreenSoundproofBarnPuzzle", "Initialize")]
    private static void InitializePostfix(Transform barnRoot)
    {
        var room1 = barnRoot?.Find(Room1Name);
        var room2 = barnRoot?.Find(Room2Name);
        var room3 = barnRoot?.Find(Room3Name);
        if (room1 == null || room2 == null || room3 == null) return;

        // Room 1 is visually observable through glass but has no voice channel to either room.
        SetVoiceChannel(room1, "Room1ToRoom2", false);
        SetVoiceChannel(room1, "Room1ToRoom3", false);
        SetVoiceChannel(room2, "Room2ToRoom3", true);
        SetVoiceChannel(room2, "Room2ToRoom1", false);

        // Room 3's direct view of Room 2 is occluded; its player must rely on Room 2's callouts.
        SetVisualOcclusion(room3, room2, true);
        EnsureBlindfoldVisual(room3);
    }

    [HarmonyPostfix]
    [HarmonyPatch("GreenSoundproofBarnPuzzle", "OnPlayerEnteredRoom")]
    private static void PlayerEnteredRoomPostfix(Component player, string roomId, Transform barnRoot)
    {
        if (player == null || barnRoot == null) return;
        var room3 = barnRoot.Find(Room3Name);
        if (room3 != null && IsInside(player.transform, room3))
            EnsureBlindfoldVisual(room3, player.transform);
    }

    private static void SetVoiceChannel(Transform room, string channelName, bool enabled)
    {
        var channel = room.Find(channelName);
        if (channel != null) channel.gameObject.SetActive(enabled);
    }

    private static void SetVisualOcclusion(Transform room3, Transform room2, bool enabled)
    {
        var directView = room3.Find("DirectViewToRoom2");
        if (directView != null) directView.gameObject.SetActive(!enabled);
    }

    private static void EnsureBlindfoldVisual(Transform room, Transform? player = null)
    {
        var parent = player ?? room;
        if (parent.Find(BlindfoldName) != null) return;
        var blindfold = new GameObject(BlindfoldName);
        blindfold.transform.SetParent(parent, false);
        // Placeholder visual occlusion hook. Bind the game's mask/blindfold prefab here.
        blindfold.AddComponent<Canvas>();
    }

    private static bool IsInside(Transform player, Transform room)
    {
        var collider = room.GetComponent<Collider>();
        return collider != null && collider.bounds.Contains(player.position);
    }
}
