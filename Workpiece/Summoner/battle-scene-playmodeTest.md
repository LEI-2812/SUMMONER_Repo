# Battle Scene PlayMode Test Progress

## Scope

- Target scenes: `Assets/Screen/FightScene/Fight Screen_1Stage.unity` through `Fight Screen_7Stage.unity`.
- Goal: analyze battle-scene runtime features, add PlayMode tests, run them, and record completed feature gates here.
- Rule: do not rerun completed feature gates unless their related runtime path changes.

## Completed

| ID | Feature Gate | Test | Last Result |
| --- | --- | --- | --- |
| BPM-001 | Fight scene runtime wiring loads for stages 1-7 | `BattleScenePlayModeTests.FightScenes_LoadWithBattleRuntimeAndPlateBoard` | Passed |
| BPM-002 | Stage-specific battle scene plate counts, including stage 7 one enemy plate | `BattleScenePlayModeTests.FightScenes_HaveExpectedBattlePlateCounts` | Passed |
| BPM-003 | Player command methods are injected and callable in battle scene | `BattleScenePlayModeTests.PlayerCommands_AreCallableAfterBattleSceneLoad` | Passed |
| BPM-004 | End turn runs enemy turn and returns to player turn | `BattleScenePlayModeTests.PlayerEndTurn_ReturnsToPlayerTurnAndAdvancesRound` | Passed |
| BPM-005 | Stage enemy placement applies expected occupied enemy count | `BattleScenePlayModeTests.FightScenes_ApplyExpectedStageEnemyPlacement` | Passed |
| BPM-006 | Stage clear turn resolves per fight scene | `BattleScenePlayModeTests.FightScenes_ResolveExpectedClearTurn` | Passed |
| BPM-007 | Player summon command places selected summon on a player plate | `BattleScenePlayModeTests.PlayerSummonCommand_PlacesSelectedSummonOnPlayerPlate` | Passed |
| BPM-008 | Player normal attack command mutates an enemy summon or fails safely without target | `BattleScenePlayModeTests.PlayerNormalAttackCommand_ExecutesWithoutRuntimeError` | Passed |
| BPM-009 | Player special attack command executes in battle scene | `BattleScenePlayModeTests.PlayerSpecialAttackCommand_ExecutesWithoutRuntimeError` | Passed |
| BPM-010 | Player targeted special attack waits for target and applies after plate click | `BattleScenePlayModeTests.PlayerTargetedSpecialAttack_SelectsTargetPlateAndExecutes` | Passed |
| BPM-011 | Battle result controller starts clear/fail result safely | `BattleScenePlayModeTests.BattleResultController_StartsClearAndFailResultsSafely` | Passed |
| BPM-012 | Defeated summon clears its battle plate | `BattleScenePlayModeTests.SummonDeath_ClearsOccupiedBattlePlate` | Passed |

## In Progress

_None._

## Pending

- No pending PlayMode gate in this pass.
- Add a new BPM entry here only when a new battle-scene runtime path is changed or a missing scenario is found.
