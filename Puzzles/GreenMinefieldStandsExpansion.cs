using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Green Tower dense minefield with a held remote button that powers the final green orb.
// Replace target type/method names with those from the current IL2CPP dump.
[HarmonyPatch]
internal static class GreenMinefieldStandsExpansion
{
    internal static ConfigEntry<int> GridWidth { get; private set; } = null!;
    internal static ConfigEntry<int> GridDepth { get; private set; } = null!;
    internal static ConfigEntry<float> StandSpacing { get; private set; } = null!;
    internal static ConfigEntry<float> RedStandChance { get; private set; } = null!;
    internal static ConfigEntry<float> KnockbackForce { get; private set; } = null!;

    private static bool _tetherHeld;
    private static Transform _remoteContainer;

    internal static void Bind(ConfigFile config)
    {
        GridWidth = config.Bind("Green Minefield", "GridWidth", 12, "Expanded minefield width.");
        GridDepth = config.Bind("Green Minefield", "GridDepth", 12, "Expanded minefield depth.");
        StandSpacing = config.Bind("Green Minefield", "StandSpacing", 1.5f, "Distance between minefield stands.");
        RedStandChance = config.Bind("Green Minefield", "RedStandChance", 0.65f, "Chance that an added stand is a red decoy/trap.");
        KnockbackForce = config.Bind("Green Minefield", "KnockbackForce", 8f, "Knockback impulse after touching a red stand.");
    }

    [HarmonyPostfix]
    [HarmonyPatch("GreenMinefieldPuzzle", "Initialize")]
    private static void InitializePostfix(Transform minefieldRoot, Transform remoteContainer)
    {
        _tetherHeld = false;
        _remoteContainer = remoteContainer;
        SpawnDenseField(minefieldRoot);
        SetRemotePower(false);
    }

    [HarmonyPostfix]
    [HarmonyPatch("GreenMinefieldPuzzle", "OnTetherButtonPressed")]
    private static void TetherPressedPostfix() { _tetherHeld = true; SetRemotePower(true); }

    [HarmonyPostfix]
    [HarmonyPatch("GreenMinefieldPuzzle", "OnTetherButtonReleased")]
    private static void TetherReleasedPostfix() { _tetherHeld = false; SetRemotePower(false); }

    [HarmonyPostfix]
    [HarmonyPatch("GreenMinefieldPuzzle", "OnStandTouched")]
    private static void StandTouchedPostfix(Rigidbody player, bool isRedStand)
    {
        if (!isRedStand || player == null) return;
        player.AddForce((Vector3.up + Random.insideUnitSphere * 0.35f).normalized * KnockbackForce.Value, ForceMode.VelocityChange);
        SetRemotePower(false);
    }

    private static void SpawnDenseField(Transform root)
    {
        if (root == null) return;
        var tetherIndex = Random.Range(0, Mathf.Max(1, GridWidth.Value * GridDepth.Value));
        for (var z = 0; z < GridDepth.Value; z++)
        for (var x = 0; x < GridWidth.Value; x++)
        {
            var stand = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            stand.name = x + z * GridWidth.Value == tetherIndex ? "HardWalk_TetherButtonStand" : (Random.value < RedStandChance.Value ? "HardWalk_RedTrapStand" : "HardWalk_GreenTargetStand");
            stand.transform.SetParent(root, false);
            stand.transform.localPosition = new Vector3(x * StandSpacing.Value, 0f, z * StandSpacing.Value);
            var trigger = stand.AddComponent<GreenMinefieldStandTrigger>();
            trigger.IsRed = stand.name.Contains("RedTrap");
            trigger.IsTether = stand.name.Contains("TetherButton");
        }
    }

    private static void SetRemotePower(bool powered)
    {
        if (_remoteContainer == null) return;
        _remoteContainer.gameObject.SetActive(true);
        var animator = _remoteContainer.GetComponent<Animator>();
        if (animator != null) animator.SetBool("Powered", powered);
        var collider = _remoteContainer.GetComponent<Collider>();
        if (collider != null) collider.enabled = powered;
    }

    private sealed class GreenMinefieldStandTrigger : MonoBehaviour
    {
        internal bool IsRed;
        internal bool IsTether;
        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            var body = other.attachedRigidbody;
            if (IsRed && body != null) body.AddForce(Vector3.up * KnockbackForce.Value, ForceMode.VelocityChange);
            if (IsTether) { _tetherHeld = true; SetRemotePower(true); }
        }
        private void OnTriggerExit(Collider other)
        {
            if (IsTether && other.CompareTag("Player")) { _tetherHeld = false; SetRemotePower(false); }
        }
    }
}
