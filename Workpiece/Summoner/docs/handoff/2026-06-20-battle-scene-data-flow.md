# Battle Scene Data Flow Handoff - 2026-06-20

## Status

Battle fight scene hierarchy and enemy placement data flow were refactored for Fight Screen 1~7.

The key rule now is:

```text
BattleStartController
-> BattleEnemyPlacementController
-> StageEnemyPlacementData
-> Plate.SummonPlaceOnPlate()
```

Enemy placement must not be driven by per-scene `EnemyPlate.summon` overrides anymore.

## Changed Files

- `Assets/Screen/FightScene/Fight Screen_1Stage.unity`
- `Assets/Screen/FightScene/Fight Screen_2Stage.unity`
- `Assets/Screen/FightScene/Fight Screen_3Stage.unity`
- `Assets/Screen/FightScene/Fight Screen_4Stage.unity`
- `Assets/Screen/FightScene/Fight Screen_5Stage.unity`
- `Assets/Screen/FightScene/Fight Screen_6Stage.unity`
- `Assets/Screen/FightScene/Fight Screen_7Stage.unity`
- `Assets/Script/5_Battle/0_Flow/4_Data/StageEnemyPlacementData.asset`
- `Assets/Script/5_Battle/0_Flow/4_Data/StageEnemyPlacementData.cs`
- `Assets/Script/5_Battle/0_Flow/4_Data/Editor/StageEnemyPlacementPropertyDrawer.cs`
- `Assets/Tests/EditMode/Editor/StageEnemyPlacementStaticRegressionTests.cs`
- `Assets/Tests/EditMode/Editor/StageSceneConnectionEditModeTests.cs`

## Scene Hierarchy Rule

Fight Screen 1~7 now use indexed top-level scene groups:

```text
0_Runtime
  EventSystem
  GameSaveController
  0_BattleFlow_Runtime
    0_Board_Runtime
    1_Turn_Runtime
    2_Stage_Runtime
  HUDLoader

1_Camera
  Main Camera

2_UI
  BattleCanvas
```

Core runtime ownership:

- `0_Board_Runtime`: `BattleController`, `PlateController`, attack prediction, `PlateView`
- `1_Turn_Runtime`: `TurnController`
- `2_Stage_Runtime`: `BattleStageContext`, `BattleEnemyPlacementController`, `BattleStartController`, `BattleResultController`, `BattleProgressController`, result alert/sound components

## Enemy Placement Data

Current `StageEnemyPlacementData.asset` placement:

```text
Stage 1: P0 Slime, P1 Slime
Stage 2: P0 Slime, P1 KingSlime
Stage 3: P0 Water Spirit, P1 Grass Spirit
Stage 4: P0 FireSpirit, P1 Water Spirit, P2 QueenSpirit
Stage 5: P0 Skeleton, P1 Skeleton, P2 LowDevil
Stage 6: P0 Skeleton, P1 LowDevil, P2 HighDevil
Stage 7: P0 DarkDragon
```

These values were restored from the previous scene `EnemyPlate.summon` prefab override values. Stage 1 had no direct scene override, so the existing Slime/Slime data was kept.

## Inspector Support

`StageEnemyPlacementPropertyDrawer` was added under an `Editor` folder.

Expected Inspector labels:

```text
Stage 4 enemy placements (3)
Plate 0 - FireSpirit
Plate 1 - Water Spirit
Plate 2 - QueenSpirit
```

This is editor-only and must stay under:

```text
Assets/Script/5_Battle/0_Flow/4_Data/Editor/
```

## Updated Tests

Updated static regression tests:

- `StageEnemyPlacementStaticRegressionTests`
  - checks restored stage 2~7 placement data
  - checks editor property drawer labels exist
  - checks Fight Screen 1~7 do not keep direct `propertyPath: summon` scene overrides
  - checks current runtime object names `0_BattleFlow_Runtime` and `2_Stage_Runtime`

- `StageSceneConnectionEditModeTests`
  - checks current scene hierarchy paths:
    - `0_Runtime`
    - `1_Camera`
    - `2_UI`
    - `0_BattleFlow_Runtime`
    - `0_Board_Runtime`
    - `1_Turn_Runtime`
    - `2_Stage_Runtime`

## Verified

Search/reference validation passed:

- Fight Screen 1~7 direct `propertyPath: summon` count is `0`.
- Fight Screen 1~7 have no `m_Script: {fileID: 0}`.
- Fight Screen 1~7 have no stale GUID `dd2e37e0a8a3a604a802e36f2693bc28`.
- Fight Screen 1~7 have no `battleResultController: {fileID: 0}`.
- Every `enemySummonPrefab` GUID/fileID in `StageEnemyPlacementData.asset` resolves to an existing summon prefab component.

## Not Verified

Unity/EditMode test execution was not completed in this session.

Known blocker:

```text
NETSDK1004: Temp/obj/Assembly-CSharp/project.assets.json is missing
```

Previous `dotnet restore .\Assembly-CSharp.csproj -v normal` ended with exit code 1 and no explicit errors.

## Next Session

Recommended first action:

```text
Run targeted Unity EditMode tests:
- StageEnemyPlacementStaticRegressionTests
- StageSceneConnectionEditModeTests
```

If Unity tests pass, continue with the next Battle cleanup slice. If they fail, fix only stale scene path/test expectation issues first.

## Continuation - Battle Targeting API

Continued the next Battle cleanup slice after the scene/data-flow handoff.

Targeting public API names were narrowed so the representative flow is clearer:

```text
PlateTargetSelectionActions
-> BattleController.SelectSpecialAttackTargetPlate()
-> BattleAttackState
-> PlayerTargetSelectionActions
-> BattleController.SpecialAttackExecute()
```

Changed code files:

- `Assets/Script/5_Battle/0_Flow/2_AttackFlow/BattleAttackState.cs`
- `Assets/Script/5_Battle/0_Flow/2_AttackFlow/BattleController.cs`
- `Assets/Script/5_Battle/1_Player/0_Turn/PlayerTurnFlow.cs`
- `Assets/Script/5_Battle/1_Player/2_Attack/PlayerTargetSelectionActions.cs`
- `Assets/Script/5_Battle/4_Plate/PlateInputActions.cs`
- `Assets/Script/5_Battle/4_Plate/PlateTargetSelectionActions.cs`

Renamed intent:

```text
GetIsAttacking()
-> IsSpecialAttackTargetSelectionActive()

SetIsAttacking(true)
-> StartSpecialAttackTargetSelection()

SetIsAttacking(false)
-> CancelSpecialAttackTargetSelection()

SelectTargetPlate()
-> SelectSpecialAttackTargetPlate()

GetSelectedTargetPlateIndex()
-> GetSelectedSpecialAttackTargetPlateIndex()

ClearSelectedTargetPlate()
-> ClearSpecialAttackTargetSelection()
```

No serialized fields, scene values, prefab values, or battle execution branches were changed in this slice.

Verified:

- Old targeting method calls are `0` across `Assets`.
- New targeting calls are limited to `BattleController`, `BattleAttackState`, `PlayerTargetSelectionActions`, `PlateInputActions`, `PlateTargetSelectionActions`, and `PlayerTurnFlow`.
- `git diff --check` passed for the changed targeting code files.

Still blocked:

```text
dotnet build .\Assembly-CSharp.csproj --no-restore
NETSDK1004: Temp\obj\Assembly-CSharp\project.assets.json is missing

dotnet restore .\Assembly-CSharp.csproj -v minimal
exit code 1 after "Determining projects to restore..." with no explicit error
```

Next recommended slice:

```text
COORD-135 | Battle Targeting | Move attack-start source away from SummonStatePanelView state reads
First action: inspect BattleController.AttackStart(), PlayerAttackActions.TryExecuteSpecialAttack(), SummonStatePanelView.SetStatePanel() call sites
Call target: DevAgent, then VerificationAgent after the slice
```
