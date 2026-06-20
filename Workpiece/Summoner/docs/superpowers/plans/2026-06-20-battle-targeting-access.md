# Battle Targeting Access Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Stop attack source selection from being changed through `SummonController`, and keep attacker/target selection state inside the battle targeting flow.

**Architecture:** `SummonController` remains responsible only for summon/redraw picking. `SummonStatePanelView` records which player plate opened the panel, and `BattleController` starts an attack from that panel selection into `BattleAttackState`. `PlayerAttackActions` reads the selected attacker plate index from `BattleController` instead of storing a separate mutable copy.

**Tech Stack:** Unity C#, MonoBehaviour scene references, existing Battle controller/action/state classes.

---

### Task 1: Move Attacker Plate Selection Out Of SummonController

**Files:**
- Modify: `Assets/Script/Battle/View/SummonStatePanelView.cs`
- Modify: `Assets/Script/Battle/Unit/Plate/PlateInputActions.cs`
- Modify: `Assets/Script/Battle/Flow/BattleAttackState.cs`
- Modify: `Assets/Script/Battle/Flow/BattleController.cs`
- Modify: `Assets/Script/Battle/Unit/Player/PlayerAttackActions.cs`
- Modify: `Assets/Script/Battle/Unit/Player/PlayerActionFlow.cs`
- Modify: `Assets/Script/Battle/Unit/Player/PlayerController.cs`
- Modify: `Assets/Script/Battle/SummonPick/SummonController.cs`

- [ ] **Step 1: Extend state panel with selected player plate index**

Add `selectedPlayerPlateIndex` to `SummonStatePanelView`. Change `SetStatePanel` to accept the selected plate index and expose `GetSelectedPlayerPlateIndex()`.

```csharp
private int selectedPlayerPlateIndex = -1;

public void SetStatePanel(Summon stateSummon, bool isEnemyPlate, int selectedPlayerPlateIndex)
{
    statePanel.gameObject.SetActive(true);
    this.stateSummon = stateSummon;
    this.selectedPlayerPlateIndex = isEnemyPlate ? -1 : selectedPlayerPlateIndex;
    stateSummon.AddObserver(this);
    ...
}

public int GetSelectedPlayerPlateIndex()
{
    return selectedPlayerPlateIndex;
}
```

- [ ] **Step 2: Pass the plate index directly from PlateInputActions**

In `PlateInputActions.TryOpenSummonStatePanel`, remove the `summonController.SetPlayerSelectedIndex(plateIndex)` call and pass `plateIndex` into the state panel.

```csharp
int plateIndex = GetPlayerPlateIndex();
statePanelView.SetStatePanel(currentSummon, IsCurrentEnermyPlate(), plateIndex);
```

- [ ] **Step 3: Store attack source plate index in BattleAttackState**

Change `BattleAttackState.StartAttack` to accept the attacker plate index and expose it as read-only state.

```csharp
public int AttackingPlateIndex { get; private set; } = -1;

public void StartAttack(Summon attackingSummon, int attackingPlateIndex)
{
    AttackingSummon = attackingSummon;
    AttackingPlateIndex = attackingPlateIndex;
    CurrentSpecialAttackInfo = null;
}
```

Also reset `AttackingPlateIndex` to `-1` in `Reset()`.

- [ ] **Step 4: Start attacks from BattleController panel state**

In `BattleController.AttackStart`, read `statePanel.GetSelectedPlayerPlateIndex()` and pass it to `attackState.StartAttack(...)`.

```csharp
attackState.StartAttack(
    statePanel.GetStatePanelSummon(),
    statePanel.GetSelectedPlayerPlateIndex());
```

Add:

```csharp
public int GetAttackingPlateIndex()
{
    return attackState.AttackingPlateIndex;
}
```

- [ ] **Step 5: Remove PlayerAttackActions attacker index storage**

Remove `private int selectedPlateIndex = -1;` and `SetSelectedPlateIndex`. In normal and immediate special attack execution, read `battleController.GetAttackingPlateIndex()`.

```csharp
actionExecutor.ExecuteNormalAttack(
    attackSummon,
    plateController.GetEnermyPlates(),
    battleController.GetAttackingPlateIndex(),
    feedbackView);
```

```csharp
actionExecutor.ExecuteImmediateSpecialAttack(
    battleController,
    attackSummon,
    battleController.GetAttackingPlateIndex(),
    feedbackView);
```

- [ ] **Step 6: Remove public bypass methods**

Remove the now-unused pass-through methods:

```csharp
// PlayerActionFlow
public void SetSelectedPlateIndex(int selectedPlateIndex)

// PlayerController
public void SetSelectedPlateIndex(int selectedPlateIndex)

// SummonController
public void SetPlayerSelectedIndex(int index)
```

- [ ] **Step 7: Verify static references**

Run:

```powershell
Select-String -Path Assets\Script\Battle\**\*.cs -Pattern "SetPlayerSelectedIndex|SetSelectedPlateIndex"
```

Expected: no matches.

Run:

```powershell
Select-String -Path Assets\Script\Battle\Unit\Player\PlayerAttackActions.cs -Pattern "selectedPlateIndex"
```

Expected: no matches, except method parameter names in unrelated call sites are acceptable outside this file.

- [ ] **Step 8: Validate diff hygiene**

Run:

```powershell
git diff --check -- Assets/Script/Battle/View/SummonStatePanelView.cs Assets/Script/Battle/Unit/Plate/PlateInputActions.cs Assets/Script/Battle/Flow/BattleAttackState.cs Assets/Script/Battle/Flow/BattleController.cs Assets/Script/Battle/Unit/Player/PlayerAttackActions.cs Assets/Script/Battle/Unit/Player/PlayerActionFlow.cs Assets/Script/Battle/Unit/Player/PlayerController.cs Assets/Script/Battle/SummonPick/SummonController.cs
```

Expected: no whitespace errors in the changed code slice.

- [ ] **Step 9: Attempt compile gate**

Run:

```powershell
dotnet build Assembly-CSharp.csproj --no-restore
```

Expected in current workspace: may stop before C# compile with `NETSDK1004` if `Temp\obj\Assembly-CSharp\project.assets.json` is still missing. If it reaches C# compile, there should be no missing method errors for removed bypass methods.

