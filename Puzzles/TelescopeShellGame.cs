using HarmonyLib;
using UnityEngine;

namespace HardWalk.Puzzles;

// Telescope shell game behavior:
// Normal Walk: holding a button reveals the baby box with a red light.
// Hard Walk: three grouped buttons control three scattered boxes; all boxes have red
// lights, but exactly one box contains the baby, so the lights provide no answer.
[HarmonyPatch]
internal static class TelescopeShellGame
{
    internal const int ButtonCount = 3;
    internal const int BoxCount = 3;

    internal const string VerifiedConditionSymbol = "PeckCondition";
    internal const string VerifiedSwitchSymbol = "PeckSwitch";
    internal const string VerifiedSignalSymbol = "PeckBus";

    internal static bool IsEnabled(LobbyPlayerCountMode mode, int playerCount)
        => GameModeConfig.IsHardWalkActive(mode, playerCount);

    // Normal mode: the held button identifies the baby box and only that box is lit.
    internal static int GetNormalLitBox(bool buttonHeld, int babyBox)
        => buttonHeld ? Mathf.Clamp(babyBox, 0, BoxCount - 1) : -1;

    // Hard Walk always has three buttons and three boxes. All three lights are on,
    // regardless of which box was assigned the baby.
    internal static bool IsHardWalkLayout(bool hardWalkActive)
        => hardWalkActive;

    internal static bool IsButtonGrouped(bool hardWalkActive, int buttonIndex)
        => hardWalkActive && buttonIndex >= 0 && buttonIndex < ButtonCount;

    internal static bool IsBoxLit(bool hardWalkActive, bool buttonHeld, int boxIndex, int babyBox)
    {
        if (boxIndex < 0 || boxIndex >= BoxCount) return false;
        if (hardWalkActive) return true;
        return GetNormalLitBox(buttonHeld, babyBox) == boxIndex;
    }

    // Pick exactly one baby box at the start of a hard round. The answer is hidden by
    // the identical red lights and is only used when a player guesses a box.
    internal static int SelectBabyBox(bool hardWalkActive, int randomValue)
    {
        if (!hardWalkActive) return Mathf.Clamp(randomValue, 0, BoxCount - 1);
        return Mathf.Abs(randomValue) % BoxCount;
    }

    internal static bool CheckGuess(bool hardWalkActive, bool buttonHeld, int selectedBox, int babyBox)
    {
        if (selectedBox < 0 || selectedBox >= BoxCount) return false;
        return hardWalkActive
            ? selectedBox == babyBox
            : buttonHeld && selectedBox == babyBox;
    }

    internal static bool CheckVerifiedResult(bool hardWalkActive, bool conditionMet, bool switchPressed,
        bool busSignal, int selectedBox, int babyBox)
        => !hardWalkActive || (conditionMet && switchPressed && busSignal && CheckGuess(true, true, selectedBox, babyBox));

    // Kept behind the existing opt-in guard until controller signatures are available
    // from the target game's symbol dump.
#if HARDWALK_ENABLE_UNVERIFIED_PATCHES
    [HarmonyPostfix, HarmonyPatch("TelescopeShellGameController", "StartRound")]
    private static void StartRoundPostfix() { }

    [HarmonyPrefix, HarmonyPatch("TelescopeShellGameController", "CheckButton")]
    private static bool CheckButtonPrefix(int buttonIndex, ref bool __result)
    {
        __result = buttonIndex >= 0 && buttonIndex < ButtonCount;
        return false;
    }
#endif
}
