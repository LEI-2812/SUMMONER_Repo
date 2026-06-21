# Architecture Role Rule

## Purpose

Prefer structure that reduces reading hops and change impact. Do not split classes just because another role name exists.

Default roles:

```text
View -> Controller/Flow -> Service -> Action -> State/Data
```

Create a new role only when it shortens the runtime flow or reduces real reasons to change.

## Role Rules

| Role | Responsibility | Why Keep It | Avoid When |
|---|---|---|---|
| View | UI wiring, button event forwarding, screen display | Keeps Unity UI away from game rules | It saves data, loads scenes, or handles battle result state |
| Controller | MonoBehaviour reference wiring and representative entry point | Keeps scene object contracts visible | It accumulates detailed combat rules or calculations |
| Flow | Orders multiple execution steps | Lets a feature read top to bottom | The feature has only one or two steps |
| Service | Calculation, query, creation, preparation before execution | Keeps probability, candidate creation, board query, and setup separate from state changes | It only wraps one method call |
| Action | Executes a game state change | Makes attacks, summons, turn end, and similar behavior explicit | It only calls another Executor or Helper |
| State/Data | Stores values | Makes turn state, selection state, save data, and settings explicit | The value has one obvious owner and can stay as a field |

## Names To Restrict

- `Manager`: usually hides ownership.
- `Helper`: usually hides the reason to change.
- `Util`: removes domain meaning.
- `Common`: often means reuse was chosen before responsibility.
- `Executor`: use only when there is a shared execution policy. If it is a thin call wrapper, keep it inside an Action or private Controller method.
- `Handler`: use only for a clear input or event handling responsibility. Do not create empty subclasses that only call `base`.

## Decision Rules

### Completion Criteria

- Every refactor candidate must define completion criteria before it becomes Ready.
- Completion criteria should name the representative entry point, the contract to shrink or keep, the risk boundary, and the verification gate.
- When completion criteria are met, do not keep refactoring the same feature because nearby names, comments, or tiny structure issues remain.
- If the work needs scene, prefab, ScriptableObject, saved data, file deletion, or broad movement, close the current slice with a hold reason and require separate approval.

### Keep As-Is

- One feature flow is readable in one file.
- The order is clear even if the file is a little long.
- Splitting only adds file navigation.
- Domain rules are clearer when kept by case, such as summon-specific prediction files.

### Merge

- A class only forwards one or two calls.
- It has no policy or state of its own.
- Its subclass only calls `base`.
- Reading one feature requires jumping through many tiny files.

### Shrink

- A public method or interface is wider than actual callers need.
- A View directly handles save, scene movement, battle result, or input policy.
- A Controller owns calculation, state mutation, and UI display together.
- An Action owns input wait, UI feedback, execution, and post-processing together.

### Split

- The reasons to change are genuinely different.
- Unity scene wiring and pure logic must be separated.
- Save, progress, battle result, or scene movement state changes are scattered.
- The same preparation calculation repeats across multiple execution flows and merging it reduces reading hops.

## Current Audit Candidates

### Merge Candidates

- `PlayerActionExecutor`: many thin execution wrappers.
- `SummonPlaceExecutor`: placement execution can likely read better as a private step in `SummonController`.
- `ToMainAlertHandler`, `ToQuitAlertHandler`: no unique behavior beyond base calls.

### Shrink Candidates

- `PlayerTargetSelectionActions`: input wait, UI, cancel, attack execution, and post-processing are mixed.
- `IAttackPrediction`: forces helper methods that the dispatcher does not need.
- `BattleController`: attack public API can grow too wide.
- `Summon`: combat state, attack, status, UI, sound, and death handling are large; do not big-bang split.
- `GameplaySettingView`: setting UI and input policy attempt are mixed.

## Next Work Selection

Pick the next slice in this order:

1. Reduce thin wrappers to lower file hops.
2. Shrink wide public contracts to lower change impact.
3. Move View bypasses for save, scene movement, and results into Flow or Controller.
4. For large classes, narrow one representative flow before splitting.

Before code edits, DevAgent must show a Change Proposal, expected diff, and completion criteria.
