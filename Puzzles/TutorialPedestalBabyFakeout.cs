using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Tutorial/start-island pedestal: the first complete placement is a fakeout; the second grants the key.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class TutorialPedestalBabyFakeout
{
    private static bool _firstCompletionTriggered;
    private static bool _keyGranted;

    [HarmonyPostfix]
    [HarmonyPatch("TutorialBabyPedestal", "Initialize")]
    private static void InitializePostfix()
    {
        _firstCompletionTriggered = false;
        _keyGranted = false;
    }

    [HarmonyPrefix]
    [HarmonyPatch("TutorialBabyPedestal", "OnAllBabiesSlotted")]
    private static bool OnAllBabiesSlottedPrefix(
        Transform[] babyContainers,
        Rigidbody[] slottedBabies,
        ref bool __result)
    {
        if (_keyGranted)
        {
            __result = true;
            return false;
        }

        if (!_firstCompletionTriggered)
        {
            _firstCompletionTriggered = true;
            EjectBabies(babyContainers, slottedBabies);
            __result = false;
            return false;
        }

        // The second complete placement follows the game's normal key-grant path.
        _keyGranted = true;
        __result = true;
        return true;
    }

    private static void EjectBabies(Transform[] containers, Rigidbody[] babies)
    {
        if (containers != null)
        {
            foreach (var container in containers)
            {
                if (container == null) continue;
                // Placeholder: replace with the game's open/unlock animation call.
                container.gameObject.SetActive(false);
            }
        }

        if (babies == null) return;
        foreach (var baby in babies)
        {
            if (baby == null) continue;
            baby.transform.SetParent(null, true);
            baby.isKinematic = false;
            baby.detectCollisions = true;
            baby.AddForce(Vector3.up * 1.5f + Random.insideUnitSphere * 0.5f, ForceMode.VelocityChange);
        }
    }
}
