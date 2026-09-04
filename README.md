# Hard Walk

A BepInEx 6 IL2CPP mod skeleton for Big Walk. This revision is a reverse-engineering audit, not a claim that the puzzle patches are ready for the shipped game.

Target evidence was checked against these public sources:

- QuestPackageManager/bigwalk-cordl-rs: generated CORDL bindings repository, https://github.com/QuestPackageManager/bigwalk-cordl-rs
- dougwithseismic/bigwalk-mods README: https://github.com/dougwithseismic/bigwalk-mods/blob/main/README.md
- dougwithseismic/bigwalk-mods modding guide: https://github.com/dougwithseismic/bigwalk-mods/blob/main/docs/modding-guide.md

The modding guide explicitly targets Big Walk Steam app 1478500, Unity 6000.3.17f1, IL2CPP, and metadata version 39. CORDL synthetic identifiers must not be treated as human-readable gameplay names without corroboration.

## Audit table

| Classification | Names and evidence | HardWalk status |
|---|---|---|
| VERIFIED REAL NAMES | Dissonance Voip; MirrorIgnorancePlayer; VoiceProximityBroadcastTrigger; VoiceProximityReceiptTrigger; BaseProximityTrigger<T>; BaseProximityTrigger._range; get_Size; Grid.CellPos; Grid.GenerateName; GridProximityChat; VoicePlayback.UpdatePositionalPlayback; PlaybackOptions.IsPositional; PlaybackOptions.AmplitudeMultiplier; PlayerCheater; PlayerCheater.Update; PlayerCheater.CheckForCheat; CameraCheatMover; CameraCheatMover.Detach/Attach; SpawnEmCheat.Spawn; TrainCheater.SetDistance; DevMenuRow.Assign. These are named in the public modding guide, whose stated evidence is the shipped build/disassembly. | None of the existing puzzle files target these names. |
| PUBLIC-SOURCE CONFIRMED BUT SIGNATURE NEEDS STEAM VERIFICATION | Dissonance range/grid behavior and the shared Range wire-format constraint are confirmed conceptually. The guide does not provide enough C# interop declarations to safely write Harmony signatures for HardWalk. The guide also mentions PlayerCheater fields _voiceToggle, _voice2DToggle, _voice2DSet, ghostMovementScalar, and LockedRay, but field types/accessors still require the local v39 dump. | No patch added. |
| STILL PLACEHOLDER / UNKNOWN | ProximityVoiceChat.CanHear; GreenButton.Update; ScaffoldButton.Start; TelescopeShellGameController.StartRound; TelescopeShellGameController.CheckButton; Blue House room/volume query; Hoop Toss moving targets/sports/mini-games; Cannon, TimerBall, PropGroup.CannonFireable, PropGroup.TimerBall; BigKey, PropGroup.BigKey, BigKeyComplete; puzzle-container Baby launch; Green Minefield stands/buttons/tether; Telescope shell game/beach house; Train moving targets/Green tower coordinates; PitchDetector; GourdParcel. These were not verified in the public text inspected, and CORDL-generated synthetic names alone are insufficient evidence. | All related source files are compile-safe but excluded by HARDWALK_ENABLE_UNVERIFIED_PATCHES. |

## Safety decision

The original Harmony attributes used placeholder target names and guessed parameter lists. They could silently fail or patch the wrong IL2CPP method. They are now disabled at compile time. Do not define HARDWALK_ENABLE_UNVERIFIED_PATCHES until a Steam dump for the installed build proves each declaring type, method, parameter list, return type, and relevant field/property.

The one actionable public finding is voice proximity: Big Walk uses Dissonance grid rooms. The cell size is Range * 2, and room names encode grid coordinates. Changing Range on one client alone isolates that client; any range change must be coordinated across the lobby or implemented through a verified host relay. This finding is documented only and is not patched here.

## Build

Install the .NET SDK and set GameDir to the Big Walk installation directory:

```text
dotnet build HardWalk.csproj -p:GameDir="/path/to/Big Walk"
```

The checked-in project targets netstandard2.1 and net6.0 and uses BepInEx.Unity.IL2CPP plus HarmonyX. The execution environment used for this audit did not have dotnet installed, so a local build with the user's game references remains required before release.

## Next verification steps

1. Generate Cpp2IL v39 dummy assemblies from the installed Steam build.
2. Search the generated C# and clean CORDL exports for each concept above.
3. Confirm exact declaring types and signatures, including IL2CPP value/reference types and networking authority.
4. Add one patch at a time, guarded by an explicit feature flag, and test in a disposable save/lobby.

Unofficial educational project; not affiliated with House House or the publisher.
