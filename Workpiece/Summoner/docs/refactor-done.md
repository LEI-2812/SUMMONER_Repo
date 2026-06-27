# Refactor Done List

Base target document: `docs/battle-scene-target-architecture.md`
Last updated: 2026-06-26

This document is the current battle-scene refactor baseline. Older records that used the wider Controller/UseCase structure were removed from the active baseline so later sessions do not follow stale architecture.

## [Done] Battle Scene Target Structure

- Done date: 2026-06-26
- Modified files:
  - `Assets/Script/Battle/0_Presentation/Runtime/BattleSceneRuntime.cs`
  - `Assets/Script/Battle/0_Presentation/Player/PlayerCommandController.cs`
  - `Assets/Script/Summon/0_Presentation/SummonSelectionController.cs`
  - `Assets/Script/Battle/0_Presentation/Plate/BattleBoardInputController.cs`
  - `Assets/Script/Battle/0_Presentation/Result/BattleResultController.cs`
  - `Assets/Script/Battle/1_Application/**`
  - `Assets/Script/Turn/1_Application/**`
  - `Assets/Script/Battle/2_Domain/**`
  - `Assets/Script/Summon/2_Domain/**`
  - `Assets/Script/Save/3_Infrastructure/StageProgressSaveStore.cs`
  - `Assembly-CSharp.csproj`
- Change summary:
  - Battle Controllers now match the target input group list: `PlayerCommandController`, `SummonSelectionController`, `BattleBoardInputController`, `BattleResultController`.
  - Battle/Turn UseCases now match the target flow list: `StartBattleUseCase`, `ChangeTurnUseCase`, `HandlePlayerCommandUseCase`, `StartSummonSelectionUseCase`, `SelectSummonUseCase`, `SelectBoardTargetUseCase`, `ExecutePlayerAttackUseCase`, `ExecuteEnemyTurnUseCase`, `CompleteBattleResultUseCase`.
  - Obsolete fine-grained UseCases and old Controller names were removed or folded into the target flow.
  - `EnemyTurnRuntime` and `BattleSceneRuntime` remain scene runtime bridges, not input Controllers.
- Final flow:
  BattleSceneRuntime
  -> StartBattleUseCase
  -> ChangeTurnUseCase
  -> PlayerTurnState / EnemyTurnState
  -> PlayerCommandController / SummonSelectionController / BattleBoardInputController / BattleResultController
  -> target UseCase
  -> BattleRuntimeData / SummonEntity / AttackData / StatusData
  -> View / Store
- Completion conditions:
  - Controller class list matches the 4 target Controllers.
  - Battle/Turn UseCase class list matches the 9 target UseCases.
  - Controller classes only receive Unity input and call target UseCases.
  - Store classes provide save/resource/static data only.
  - View classes handle display only.
  - No battle target `Rule` or `Provider` class remains.
  - No `BattleState`, `Context`, or `ActionContext` name is used in battle/turn/summon target code.
  - Unity `Assets/Refresh` and `recompile_scripts` pass with 0 warnings.
- Verification:
  1. `rg -n "class .*Controller" Assets/Script/Battle/0_Presentation Assets/Script/Summon/0_Presentation`
  2. `rg -n "class .*UseCase" Assets/Script/Battle/1_Application Assets/Script/Turn/1_Application`
  3. `rg -n "class .*Rule|class .*Provider|BattleState|ActionContext|class .*Context" Assets/Script/Battle Assets/Script/Summon Assets/Script/Turn Assembly-CSharp.csproj`
  4. Unity `Assets/Refresh`
  5. Unity `recompile_scripts` with 0 warnings
  6. Load Fight Scenes 1-7 to check scene script references.
- Rework guard:
  Do not recreate old Controllers or removed fine-grained UseCases unless a compile error, runtime error, failed feature test, or required feature connection proves it is necessary.

## [Done] AttackData / AttackStrategy / Status Flow

- Done date: 2026-06-26
- Modified files:
  - `Assets/Script/Battle/2_Domain/Attack/AttackData.cs`
  - `Assets/Script/Battle/2_Domain/Attack/IAttackStrategy.cs`
  - `Assets/Script/Battle/2_Domain/Attack/TargetedAttackStrategy.cs`
  - `Assets/Script/Battle/2_Domain/Attack/AttackAllEnemiesStrategy.cs`
  - `Assets/Script/Battle/2_Domain/Attack/ClosestEnemyAttackStrategy.cs`
  - `Assets/Script/Battle/1_Application/Player/ExecutePlayerAttackUseCase.cs`
  - `Assets/Script/Battle/1_Application/Attack/EnemySpecialAttackExecution.cs`
  - `Assets/Script/Battle/1_Application/Enemy/ExecuteEnemyTurnUseCase.cs`
  - `Assets/Script/Battle/1_Application/Enemy/EnemyNormalAttackReaction.cs`
  - `Assets/Script/Battle/1_Application/Enemy/EnemySpecialAttackReaction.cs`
  - `Assets/Script/Battle/2_Domain/Prediction/AttackPredictionData.cs`
  - `Assets/Script/Battle/0_Presentation/Prediction/**`
  - `Assets/Script/Summon/2_Domain/Summon.cs`
  - `Assets/Script/Summon/2_Domain/SummonEntity.cs`
  - `Assets/Script/Summon/2_Domain/Data/SummonAttackData.cs`
  - `Assets/Script/Summon/2_Domain/Data/SummonData.cs`
- Change summary:
  - `AttackData` now holds status type, effect value, status time, cooldown state, and the target-selection strategy.
  - `IAttackStrategy` now only exposes target selection.
  - `TargetedAttackStrategy`, `AttackAllEnemiesStrategy`, and `ClosestEnemyAttackStrategy` no longer expose damage/status/cooldown APIs.
  - Player attack, enemy attack, enemy prediction reaction, and summon prediction code now read attack properties from `AttackData`.
  - Status execution remains in player/enemy attack UseCases and applies `StatusData` into `SummonEntity` active statuses through `Summon.ApplyStatus`.
- Final flow:
  SummonAttackData / fallback summon setup
  -> AttackData(strategy, status, effect, cooldown, statusTime)
  -> ExecutePlayerAttackUseCase / ExecuteEnemyTurnUseCase
  -> AttackData.SelectTargets -> IAttackStrategy.SelectTargets
  -> Damage / StatusData creation
  -> Summon.ApplyStatus
  -> SummonEntity.StatusStore active statuses
- Completion conditions:
  - AttackStrategy classes select targets only.
  - Status is attack data, not a strategy responsibility.
  - Cooldown is attack data, not a strategy responsibility.
  - `TargetedAttackStrategy`, `AttackAllEnemiesStrategy`, and `ClosestEnemyAttackStrategy` are preserved.
  - Cat, Eagle, Fox, Rabbit, Snake, Wolf prediction logic still compiles against `AttackData`.
  - Unity `recompile_scripts` passes with 0 warnings.
- Verification:
  1. `rg -n "\bIAttackStrategy\b|GetStatusType\(|GetSpecialDamage\(|GetStatusTime\(|GetCooltime\(|GetCurrentCooldown\(|ApplyCooldown\(|ReduceCooldown\(" Assets/Script/Battle/2_Domain/Attack Assets/Script/Battle/1_Application Assets/Script/Summon/2_Domain`
  2. Confirm non-`AttackData` attack property calls do not appear in strategy classes.
  3. Unity `Assets/Refresh`
  4. Unity `recompile_scripts` with 0 warnings
  5. Manual Fight Scene test: normal attack, targeted special attack, all-enemy special attack, heal/shield/status application, cooldown reduction.
- Rework guard:
  Do not add Rule or Provider classes for attack judgment. Do not move damage/status execution into Strategy, Store, or View.

## [Done] Stage Progress Save Store

- Done date: 2026-06-26
- Modified files:
  - `Assets/Script/Save/3_Infrastructure/StageProgressSaveStore.cs`
  - `Assets/Script/Battle/1_Application/Result/CompleteBattleResultUseCase.cs`
- Change summary:
  - Battle result completion saves stage progress through `CompleteBattleResultUseCase`.
  - Stage progress `PlayerPrefs` access is centralized in `StageProgressSaveStore`.
  - Option setting stores may still use `PlayerPrefs` outside the battle-scene target scope.
- Final flow:
  BattleResultController
  -> CompleteBattleResultUseCase
  -> StageProgressSaveStore
  -> PlayerPrefs SetInt/Save for stage progress
- Completion conditions:
  - Battle result saving does not call PlayerPrefs directly.
  - Store does not manipulate UI.
  - Save timing is decided by the UseCase.
- Verification:
  1. `rg -n "PlayerPrefs" Assets/Script/Battle Assets/Script/Save`
  2. Clear a stage manually and confirm next stage progress is saved.
- Rework guard:
  Do not move battle progress save calls into View, Controller, or runtime data.

## [Rework] Fight Scene Player Command UnityEvent Fix

- Rework date: 2026-06-26
- Rework reason:
  - Fight Screen_1Stage still had the normal attack button wired to the removed method `OnAttackBtnClick`.
  - This rework is allowed by the rework guard because it fixes a scene connection error after the Controller rename.
- Impact files:
  - `Assets/Screen/FightScene/Fight Screen_1Stage.unity`
- Change content:
  - Changed the normal attack button UnityEvent to `PlayerCommandController.OnClickNormalAttack`.
  - Verified all Fight Scene player command button UnityEvents now call the target `PlayerCommandController` methods.
- Rechecked completion conditions:
  - No `OnAttackBtnClick`, old `PlayerController` UnityEvent, `PlateCellController`, `PlateBoardController`, or `EnemyTurnController` references remain in Fight Scene YAML.
  - Unity `Assets/Refresh` succeeded.
  - Unity `recompile_scripts` passed with 0 warnings.
  - Unity Console error query returned no errors.

## [Rework] Fight Screen 7 Enemy Placement Stage Fix

- Rework date: 2026-06-26
- Rework reason:
  - Fight Screen_7Stage used `BattleRuntimeData.defaultStage: 1`, so enemy placement read stage 1 data instead of stage 7 data.
  - This rework is allowed by the rework guard because it fixes a scene runtime data connection error.
- Impact files:
  - `Assets/Screen/FightScene/Fight Screen_7Stage.unity`
- Change content:
  - Changed `BattleRuntimeData.defaultStage` from `1` to `7`.
  - Connected `BattleSceneRuntime.battleRuntimeData` directly to the scene `BattleRuntimeData` component.
- Rechecked completion conditions:
  - Fight Screen_7Stage now reads stage 7 enemy placement data.
  - Unity `Assets/Refresh` succeeded.
  - Unity `recompile_scripts` passed with 0 warnings.
  - Unity Console error query returned no errors.

## Manual Test Checklist

1. Load `Assets/Screen/FightScene/Fight Screen_1Stage.unity`.
2. Start battle and confirm enemies are placed from stage data.
3. Click summon and select a summon; confirm mana, plate placement, and summon choice UI.
4. Click redraw/resummon and select a replacement plate.
5. Execute normal attack and confirm target selection/damage.
6. Execute targeted and all-enemy special attacks; confirm status application and cooldown.
7. End turn and confirm enemy heal-first/repeated attack/prediction response behavior.
8. Clear or fail battle and confirm result UI and stage progress save.

## Rework Policy

Completed structure should not be reopened for naming taste or folder preference. Rework is allowed only for compile errors, runtime errors, failed feature tests, required new feature connection, or direct duplicate logic removal.
