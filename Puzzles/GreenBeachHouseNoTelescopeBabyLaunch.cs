using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// v1.0 verified symbol: PeckSwitch.
// The original guessed implementation is intentionally retained as a v1.1 reference skeleton.
[HarmonyPatch]
internal static class GreenBeachHouseNoTelescopeBabyLaunch
{
    internal const string VerifiedPuzzleSymbol = "PeckSwitch";
    private static readonly Vector3 RemoteRockPosition = new(0f, -1f, 0f);
    private const string TelescopeName = "Telescope";
    private const string RockName = "HardWalk_BeachHouse_BabyRock";

    // v1.1 reference skeleton: replace target signatures after IL2CPP verification.
    [HarmonyPostfix]
    [HarmonyPatch("GreenBeachHousePuzzle", "Initialize")]
    private static void InitializePostfix(Transform beachHouseRoot)
    {
        if (beachHouseRoot == null) return;
        DisableTelescope(beachHouseRoot);
        SpawnRemoteRock(beachHouseRoot);
    }

    [HarmonyPostfix]
    [HarmonyPatch("GreenBeachHousePuzzle", "OnPuzzleCompleted")]
    private static void CompletedPostfix(Rigidbody baby, Transform beachHouseRoot)
    {
        if (baby == null) return;
        var target = beachHouseRoot?.Find(RockName);
        baby.transform.SetParent(null, true);
        if (target != null) baby.position = target.position + Vector3.up;
        baby.isKinematic = false; baby.detectCollisions = true; baby.velocity = Vector3.zero; baby.angularVelocity = Vector3.zero;
        baby.AddForce((Vector3.forward * 0.7f + Vector3.up * 1.1f).normalized * 24f, ForceMode.Impulse);
        baby.AddTorque(Random.insideUnitSphere * 8f, ForceMode.Impulse);
    }

    private static void DisableTelescope(Transform root)
    {
        var telescope = root.Find(TelescopeName) ?? root.Find("TelescopeInteractable") ?? root.Find("TelescopeCameraZoom");
        if (telescope != null) telescope.gameObject.SetActive(false);
    }

    private static void SpawnRemoteRock(Transform root)
    {
        if (root.Find(RockName) != null) return;
        var rock = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        rock.name = RockName; rock.transform.SetParent(root, true); rock.transform.position = RemoteRockPosition; rock.transform.localScale = new Vector3(4f, 0.5f, 4f);
    }
}
