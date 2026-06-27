# NEXT_SESSION_HANDOFF

## Current Goal

Task: PLAYER-TURN-ENTRY-002 | Battle/PlayerTurn | Complete first simple PlayerTurn flow

Purpose:
- Stop new sessions from following old workflow records and restarting broad refactor loops.
- Do not start from deleting `PlayerController`.
- Finish one PlayerTurn flow as the reference model before applying the same structure to other battle features.
- Keep the completed model simple: Controller -> UseCase -> StateMachine/Domain -> concrete View.
- Prefer simple, readable runtime flow over extra abstraction.

## First Action

Read these files and propose the code diff before editing product code:

- `Assets/Script/Turn/0_Presentation/TurnRuntime.cs`
- `Assets/Script/Battle/0_Presentation/Player/PlayerController.cs`
- `Assets/Script/Battle/1_Application/Player/PlayerTurnUseCase.cs`

Agent: DevAgent

## Done When

- `TurnRuntime` owns `PlayerTurnUseCase` creation.
- `PlayerController` public button methods only receive input and call `PlayerTurnUseCase`.
- PlayerTurn UI and feedback use concrete `PlayerView` and `PlayerFeedbackView`.
- No single-implementation View/Feedback interface remains in the PlayerTurn core flow.
- No new thin interface is added just to hide a concrete class.

## Explicit Exclusions

- Do not delete `PlayerController.cs/.meta` in this slice.
- Do not migrate FightScene 1-7 UnityEvent targets in this slice.
- Do not move `BattleRuntimeData` in this slice.
- Do not shrink `PlateController` in this slice.
- Do not create `BattleState`, `Context`, or `ActionContext`.
- Do not move mana into a Store.
- Do not add `Helper`, `Util`, `Common`, or thin wrapper layers.

## Structure Rules

- Controller receives input and calls UseCase.
- UseCase owns game rule orchestration and processing flow.
- TurnStateMachine owns transitions only.
- State opens/closes the current step view and controls whether input is allowed.
- View displays UI only.
- Store provides saved data, resources, or fixed data only.
- `BattleRuntimeData` groups data needed only during the current battle.

## Interface Rule

Avoid interface sprawl.

Use a concrete type when there is only one local implementation and the direct flow is readable.
Add an interface only when there are multiple real implementations, a replaceable external adapter, or a test seam that removes meaningful coupling.

## Verification

Default gate:
- Search for `PlayerController.GetPlayerTurnUseCase` call sites.
- Search `TurnRuntime` player-turn creation flow.
- Unity `recompile_scripts`.
- FightScene 1-7 `load_scene` only if scene references or serialized fields are touched.

Do not run TestRunner by default.

## Write Barrier

`.codex/workflow` and `.agents` are not writable in this session. Windows ACL returns `Access denied` even though files are not read-only.
This root handoff is the active reset record until the workflow docs become writable.

Current implementation state:

- `PlayerTurnUseCaseFactory.cs/.meta` was removed.
- `TurnRuntime` now creates and owns `PlayerTurnUseCase`.
- `PlayerController` receives the created `PlayerTurnUseCase` and uses it from button handlers.
- Product code no longer calls `PlayerController.GetPlayerTurnUseCase()`.
- `IPlayerTurnResourceView`, `IPlayerAttackCompletionView`, and `IPlayerFeedbackView` were removed.
- `PlayerTurnUseCase`, attack/summon/target-selection use cases, and attack completion now depend directly on `PlayerView` or `PlayerFeedbackView`.
- `IPlayerAttackCompletionBoard` was removed.
- `PlayerAttackCompletionUseCase` now depends directly on `PlayerAttackCompletionBoard`.
- `IPlayerTargetSelectionInput` and `IPlayerTargetSelectionView` were removed.
- `PlayerAttackUseCase` and `PlayerTargetSelectionUseCase` now depend directly on `PlayerTargetSelectionInputController` and `PlayerTargetSelectionView`.

Next task report:

```text
Next task: PLAYER-TURN-SUMMON-001 analysis | Battle/PlayerTurn | Review summon-selection controller boundary
First action: inspect `PlayerSummonUseCase`, `PlateInputUseCase`, `PlateClickUseCase`, and `PlayerSummonSelectionController` before proposing any diff.
Agent: VerificationAgent
```
