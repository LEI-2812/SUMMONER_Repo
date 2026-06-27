# Refactor Paused

Base target document: `docs/battle-scene-target-architecture.md`
Last updated: 2026-06-26

There is no intentionally paused code edit at this point. The latest battle-scene target structure has been applied to code and this file no longer contains old-architecture next steps.

## Current State

- Target Controllers present:
  - `PlayerCommandController`
  - `SummonSelectionController`
  - `BattleBoardInputController`
  - `BattleResultController`
- Target UseCases present:
  - `StartBattleUseCase`
  - `ChangeTurnUseCase`
  - `HandlePlayerCommandUseCase`
  - `StartSummonSelectionUseCase`
  - `SelectSummonUseCase`
  - `SelectBoardTargetUseCase`
  - `ExecutePlayerAttackUseCase`
  - `ExecuteEnemyTurnUseCase`
  - `CompleteBattleResultUseCase`
- `AttackData` owns attack status/effect/cooldown data.
- `IAttackStrategy` owns target selection only.
- Stage progress saving goes through `StageProgressSaveStore`.

## Next Work

Next work: BATTLE-MANUAL-QA | Battle | Manual Fight Scene run
First action: run a Fight Scene from battle start through result save using the checklist in `docs/refactor-done.md`.
Call target: VerificationAgent if manual QA finds a bug; otherwise none.

## Cautions

- Do not recreate old names such as `PlayerTurnUseCase`, `PlateCellController`, `PlateBoardController`, or `EnemyTurnController`.
- Do not split target UseCases into smaller step UseCases unless a real bug or required feature connection forces it.
- Do not introduce Rule or Provider target classes for battle logic.
- Option setting stores are outside the battle-scene save target; do not refactor them as part of battle cleanup.