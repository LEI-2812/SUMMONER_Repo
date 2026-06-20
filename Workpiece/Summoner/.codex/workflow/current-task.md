# Current Task

## 상태

- 현재 도메인: Battle
- 현재 상태: Ready
- Ready 에이전트: DevAgent

## 현재 기능 슬라이스

다음 작업: COORD-136 | Battle SceneObject | FightScene UI 계층 정리 제안
첫 액션: FightScene 2~7의 BattleCanvas 하위 UI 계층을 1Stage 기준으로 맞출 수 있는지 Unity 연결 위험을 분리해 Change Proposal을 제시한다.
호출 대상: CoordinatorAgent

## 게이트

- DEV-100: FightScene 1~7에서 삭제된 `EnermyAlgorithm` serialized field, missing script MonoBehaviour, GameObject component 참조를 제거했다.
- DEV-100: 현재 `EnermyAttackController`에 필요한 `plateController`, `playerAttackPrediction`, `battleController` 연결을 1~7Stage에 유지했다.
- 검증: `enermyAlgorithm:` 및 삭제 GUID 검색 0건, `EnermyAttackController` 필드 연결 검색 확인, `git diff --check` 오류 없음.
- 다음 Batch 목표: Battle 전체 구조개선을 진행한다. EnemyAction/Prediction 하나에 계속 머무르지 않는다.
- Batch 단위 기준: 메서드 하나, 이름 하나, helper 하나로 DEV 작업을 쪼개지 않는다. Battle 도메인 안에서 다른 기능 영향이 최소화되는 범위로 묶어 진행한다.
- SceneObject 목표: 전투씬 오브젝트 계층, missing/stale component, Battle controller 연결 상태를 기능 영향이 작은 범위로 정리한다. 씬/프리팹/serialized field 변경은 적용 전 대상 씬과 diff를 먼저 제시한다.
- 첫 판단: EnemyAction/Prediction은 현재 정리 상태를 확인하고, 추가 작업이 필수인지 아니면 PlayerAction 또는 Turn Flow로 넘어갈지 결정한다.
- 다음 후보: PlayerAction 분리, TurnController 얇게 만들기, BattleController 공격 실행 책임 정리.
- 중단 조건: 로직 변경, public API 변경, serialized field 변경, 씬/프리팹/에셋 값 변경이 필요하면 멈춘다.
- DEV-101: `PlayerActionController`의 턴 자원 상태를 `PlayerTurnResourceState`로 분리했다.
- DEV-101 검증: 기존 `mana`, `usedMana`, private `hasSummonedThisTurn` 필드 잔존 검색 0건. `dotnet build`는 `project.assets.json` 누락, `dotnet restore`는 오류 0개 실패 상태로 코드 컴파일 전 중단.
- DEV-102: `Plate.cs`와 `.meta`를 `Assets/Script/Battle/Unit/Plate/`로 이동해 `Unit` 루트를 엔티티 폴더만 남긴 구조로 정리했다.
- DEV-102 검증: 기존 `Unit/Plate.cs`와 `.meta` 잔존 없음, `Plate.cs.meta` GUID 유지, `Assembly-CSharp.csproj` include 경로 갱신 확인.
- DEV-103: `Player`를 `PlayerController`로, `PlayerActionController`를 `PlayerActionFlow`로 rename해 역할 이름을 맞췄다.
- DEV-103 검증: 이전 클래스명/파일 경로 잔존 검색 0건, `.meta` GUID 유지 확인. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-104: `PlayerActionExecutor`를 추가해 소환 시작, 턴 종료, 리롤 마나 사용, 일반 공격, 즉시 특수 공격, 공격 후 처리를 `PlayerActionFlow`에서 분리했다.
- DEV-104 검증: 실행 메서드 이동 검색 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-105: `PlayerTurnActions`를 추가하고 `PlayerController`가 `PlayerTurnResourceState`와 `PlayerTurnActions`를 조립하도록 바꿨다. `PlayerActionFlow`와 `PlayerActionExecutor`의 `TurnController`/`BattleResultController` 직접 의존을 제거했다.
- DEV-105 검증: `PlayerActionFlow`의 `TurnController`/`BattleResultController` 직접 참조 0건, `PlayerActionExecutor`의 턴/결과 참조 0건, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-106: `PlayerSummonActions`와 `PlayerAttackActions`를 추가하고 `PlayerController`가 상태/Actions를 조립하도록 정리했다. `PlayerActionFlow`에서 소환 세부 판단, 재소환 실행, 공격 검증, 특수공격 분기, 타겟 선택 코루틴을 분리했다.
- DEV-106 검증: `PlayerActionFlow`의 소환/공격 private 세부 메서드 잔존 0건, 신규 Actions 생성은 `PlayerController`에만 존재, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-107: `PlayerTurnProgressState`를 추가하고 `PlayerTurnActions` 호출을 플레이어 턴 의도 중심(`StartPlayerTurn`, `TryEndPlayerTurn`, `CheckPlayerClearResult`, `CheckPlayerFailResult`)으로 정리했다.
- DEV-107 검증: Player 계층 호출부의 이전 턴 결과 메서드명 잔존 0건, 신규 턴 진행 상태 호출 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-108: 병렬 적용 후보 중 충돌이 없는 `SummonDrawService`와 `EnemyPredictionPlates` 범위를 적용했다. 소환 후보 계산은 후보 부족/중복 상황에서 멈추지 않도록 정리했고, 예측용 적 플레이트 상태는 `EnemyPredictionPlateState`로 이름을 맞췄다.
- DEV-108 검증: `EnemyPredictionPlates`/`RestoreEnemyPlates` 잔존 0건, `EnemyPredictionPlateState` 호출부 확인, `.meta` GUID 유지, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-109: `PlateSummonExecutor`를 추가해 `Plate.cs`의 소환수 생성, 파괴, 이동 실행을 분리했다. `SummonPlaceOnPlate`, `RemoveSummon`, `DirectMoveSummon` public API와 serialized field는 유지했다.
- DEV-109 검증: `Plate.cs`의 `Instantiate`/`Destroy`/`SetParent`/`localPosition` 실행 코드 잔존 0건, `PlateSummonExecutor` 호출 확인, `Assembly-CSharp.csproj` include 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-110: `PlateVisualView`를 추가해 개별 Plate의 강조 색상과 소환수 이미지 투명도 표시를 `Plate.cs`에서 분리했다. `Highlight`, `Unhighlight`, `SetSummonImageTransparency` public API와 serialized field는 유지했다.
- DEV-110 검증: `Plate.cs`의 직접 `.color` 변경 잔존 0건, `PlateVisualView` 호출 확인, `Assembly-CSharp.csproj` include 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-111: `PlateCompactExecutor`를 추가해 `PlateController.CompactEnermyPlates`의 소환수 이동 실행을 분리했다. `CompactEnermyPlates` public API와 `TurnController` 호출부는 유지했다.
- DEV-111 검증: `PlateController.cs`의 `DirectMoveSummon`/`RemoveSummon` 직접 호출 잔존 0건, `TurnController`의 `CompactEnermyPlates` 호출 유지, `Assembly-CSharp.csproj` include 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-112: `PlateQueryService`를 추가해 `PlateController`의 소환수 목록, 클리어 여부, 가장 가까운 점유 Plate, 소환수 수, 최저 체력 Plate 조회 계산을 분리했다. 외부 public API와 호출부는 유지했다.
- DEV-112 검증: `PlateController.cs`의 private 조회 계산 메서드 잔존 0건, 외부 public API 호출부 유지, `PlateQueryService` 메서드 확인, `Assembly-CSharp.csproj` include 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- COORD-113 판단: `PlateController`의 남은 표시 흐름은 `PlateView`에 실제 표시를 위임하고 player/enemy 목록만 선택하는 public 입구 역할로 충분하다. 추가 코드 분리는 하지 않는다.
- DEV-113: `SummonPickState`를 추가해 `SummonController`의 선택 소환수, 선택 플레이트, 소환 진행 상태 보관을 분리했다. 외부 직접 참조 보존을 위해 public `isSummoning`은 유지하고 `SummonPickState`와 동기화했다.
- DEV-113 검증: `SummonController.cs`의 private `selectedSummon`/`selectedPlateIndex` 필드 잔존 0건, public `isSummoning` 유지, `IsSummoning()` 반환 유지, `PlayerActionFlow`의 `summonController.isSummoning` 호출부 변경 없음, `Assembly-CSharp.csproj` include 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-114: `SummonPlaceExecutor`를 추가해 `SummonController`의 일반 소환/재소환 Plate 배치 실행과 플레이어 소환 상태 갱신을 분리했다. `StartSummon`, `StartRedraw`, `DrawSelection`, `RedrawSelection` 흐름과 public API는 유지했다.
- DEV-114 검증: `SummonController.cs`의 `SummonPlaceOnPlate`/`SetHasSummonedThisTurn` 직접 호출 잔존 0건, `SummonPlaceExecutor` 호출 확인, `Assembly-CSharp.csproj` include 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-115: `SummonPickView`를 추가해 `SummonController`의 draw/redraw 패널 열기, 후보 패널 채우기, 패널 닫기, 어두운 배경 표시를 분리했다. `PlateSelectionController`는 재소환 대상 Plate 표시만 담당하도록 줄였다.
- DEV-115 검증: `SummonController.cs`의 패널/배경 직접 `SetActive` 잔존 0건, 후보 패널 `SetAssignedSummon`/`SetSummonImage` 직접 호출 잔존 0건, `SummonPickView` 호출 확인, `Assembly-CSharp.csproj` include 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-116: `SummonController.OnSelectSummon`에 남은 플레이어 Plate 하이라이트/투명도 복구 루프를 `PlateSelectionController.RestorePlayerPlateSelectionView`로 이동했다. enemy plate 표시 상태는 기존 흐름과 동일하게 건드리지 않았다.
- DEV-116 검증: `SummonController.cs`의 직접 `GetPlayerPlates`/`Unhighlight`/`SetSummonImageTransparency` 호출 잔존 0건, `PlateSelectionController` 복구 메서드 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-117: `PlayerActionFlow.PlayerActionBlockedCheck`가 `SummonController.isSummoning` field를 직접 읽지 않고 `SummonController.IsSummoning()`을 호출하도록 변경했다. 호환을 위해 public `isSummoning` field는 유지했다.
- DEV-117 검증: 외부 `.isSummoning` 직접 참조 0건, `Plate`/`PlayerActionFlow`의 `IsSummoning()` 호출 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-118: `BattleSpecialAttackExecutor`를 추가해 `BattleController.SpecialAttackExecute`의 특수공격 전략 분기, 대상 Plate 선택, 적용 로그 생성을 분리했다. `SpecialAttackExecute` public API와 호출부는 유지했고, null/invalid special attack index일 때 reset하지 않는 기존 흐름도 유지했다.
- DEV-118 검증: `BattleController.cs`의 `HandleTargetedAttack`/`HandleAttackAll`/`HandleClosestEnemyAttack`/`SpecialAttackApply` 잔존 0건, `SpecialAttackExecute` 호출부 유지, `BattleSpecialAttackExecutor` include 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-119: `BattleAttackState`를 추가해 `BattleController`의 공격 중 여부, 공격 소환수, 현재 특수공격 정보 보관을 분리했다. public `isAttacking` field는 호환을 위해 유지하고 `BattleAttackState`와 동기화했다.
- DEV-119 검증: `BattleController.cs`의 `attackingSummon`/`currentSpecialAttackInfo` 직접 필드 잔존 0건, 외부 `.isAttacking` 직접 참조 0건, `GetIsAttacking`/`SetIsAttacking` 호출부 유지, `BattleAttackState` include 확인, `git diff --check` 통과. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- COORD-121 판단: `BattleController.AttackStart`의 `statePanel` 조회는 Unity scene/View 연결 진입점 성격이 강해 추가 Actions/Service로 분리하지 않는다. 대신 현재 특수공격 정보 조회 판단은 `BattleAttackState` 책임으로 이동하는 것이 타당하다고 판단했다.
- DEV-120: `BattleController`의 `HasCurrentSpecialAttackInfo`, `GetCurrentSpecialAttackInfoIndex`, `DoesCurrentSpecialAttackTargetPlayerPlate` 내부 판단을 `BattleAttackState`로 이동했다. 기존 public API와 호출부는 유지했다.
- DEV-120 검증: 외부 호출부가 기존 `BattleController` public API를 계속 호출하는 것 확인, `DoesAttackStrategyTargetPlayerPlate` 잔존 0건. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- COORD-122 판단: `TurnController`는 턴 흐름 전환 외에 턴 수/클리어 턴 UI 텍스트 표시를 직접 수행하고 있어 View 책임 분리가 타당하다. 턴 상태 이동은 외부 조회 계약과 연결되어 있어 다음 슬라이스로 미룬다.
- DEV-121: `TurnView`를 추가해 턴 수와 클리어 턴 UI 텍스트 표시를 분리했다. `TurnController`의 serialized `TextMeshProUGUI` field와 public API는 유지했다.
- DEV-121 검증: `UpdateTurnCountText`/`SetClearTurnText`/`TurnCountTextUpdate`/`ClearTurnTextSet` 잔존 0건, `TurnView` 호출 확인, `Assembly-CSharp.csproj` include 1건 확인, `git diff --check` 오류 없음. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-122: Turn 관련 파일을 `Assets/Script/Battle/Flow/Turn/` 폴더로 이동했다. `TurnController.cs.meta`, `TurnSummonStateUpdater.cs.meta`, `TurnView.cs.meta`를 함께 이동해 GUID를 유지했다.
- DEV-122 검증: 기존 `Battle/Flow/Turn*.cs*` 경로 잔존 0건, 이동 후 `TurnController` GUID `49357758035fd934b9a5c6931f6bc751` 유지, `Assembly-CSharp.csproj` Turn 경로 갱신 확인.
- DEV-123: `TurnProgressState`를 추가해 `TurnController`의 `currentTurn`, `turnCount` 보관/변경 책임을 분리했다. `clearTurn` serialized field와 `GetCurrentTurn`, `GetTurnCount`, `GetClearTurn` public API는 유지했다.
- DEV-123 검증: `TurnController.cs`의 `private Turn currentTurn`, `private int turnCount` 잔존 0건, `TurnProgressState` include 1건 확인, `git diff --check` 오류 없음. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- COORD-124 판단: `TurnController`의 턴 시작/종료 흐름은 소환수 상태 갱신, 플레이트 압축, 클리어 판정, 마나 지급, 플레이어/적 행동 시작을 함께 실행하므로 Controller에 남기기보다 Actions 분리가 타당하다.
- DEV-124: `TurnPhaseActions`를 추가해 `StartCurrentTurn`, `EndCurrentTurn`, 플레이어/적 턴 시작/종료 실행 흐름을 분리했다. `TurnController`의 public API, serialized field, 외부 호출부는 유지했다.
- DEV-124 검증: `TurnController.cs`의 `StartPlayerTurn`/`StartEnemyTurn`/`EndPlayerTurn`/`EndEnemyTurn`/`AddPlayerMana`/`TryClearBattle` 등 실행 private 메서드 잔존 0건, `TurnPhaseActions` include 확인, 외부 `TurnController` public API 호출부 유지, `git diff --check` 오류 없음. `dotnet build`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-125: `PlateInputActions`를 추가해 `Plate.cs`의 포인터 진입/이탈/클릭 입력 반응, 재소환 선택, 공격 대상 선택, 상태 패널 열기 흐름을 분리했다. `Plate`의 public API와 serialized field는 유지했다.
- DEV-125 검증: `Plate.cs`의 입력 처리 private 메서드 잔존 0건, `PlateInputActions` include 확인, `git diff --check` 오류 없음. `dotnet build --no-restore`는 `project.assets.json` 누락으로 코드 컴파일 전 중단.
- DEV-126: `SummonPlaceExecutor`의 일반 소환/재소환 배치 중복을 `PlacePlayerSummon`으로 모으고, `SummonController`의 일반 소환/재소환 종료 UI 정리를 `FinishPickFlow`로 모았다. public API와 serialized field는 유지했다.
- DEV-126 검증: `PlacePlayerSummon`/`FinishPickFlow` 호출 확인, `git diff --check` 오류 없음. `dotnet build Assembly-CSharp.csproj --no-restore`는 `Temp\obj\Assembly-CSharp\project.assets.json` 누락(NETSDK1004)으로 코드 컴파일 전 중단.
- DEV-127: `BattleResultController.ClearResultTry`가 승리 결과 시작 여부를 반환하도록 바꾸고, `TurnPhaseActions.StartPlayerTurn`이 승리 결과 시작 시 플레이어 턴 후속 처리(`UpdatePlayerSpecialCooldowns`, `PlayerTurnStart`)를 중단하도록 정리했다.
- DEV-127 검증: `ClearResultTry` 호출부 2곳 확인, `TurnPhaseActions` 중단 흐름 확인, `git diff --check` 오류 없음. `dotnet build Assembly-CSharp.csproj --no-restore`는 `Temp\obj\Assembly-CSharp\project.assets.json` 누락(NETSDK1004)으로 코드 컴파일 전 중단.
- DEV-128: `TurnPhaseActions.TryClearBattle`가 `player.clearTurn/player.currentTurn` 대신 `TurnController`에서 전달된 `clearTurn`과 `TurnProgressState.TurnCount`를 기준으로 승리 판정을 호출하도록 정리했다.
- DEV-128 검증: `TurnPhaseActions.cs`의 `player.clearTurn|player.currentTurn` 검색 0건, `git diff --check` 오류 없음. 변경 파일은 현재 git 기준 untracked 경로로 표시되어 일반 `git diff`에는 출력되지 않음.
- DEV-129: 공격 타겟 선택 인덱스를 `SummonController -> PlayerController -> PlayerAttackActions` 우회 전달에서 `BattleController -> BattleAttackState` 보관 흐름으로 이동했다. 상태 패널에서 공격자 Plate를 선택하는 `SummonController.SetPlayerSelectedIndex` 흐름은 유지했다.
- DEV-129 검증: 공격 타겟 클릭의 `BattleController.SelectTargetPlate` 호출 확인, `PlayerAttackActions`의 `GetSelectedTargetPlateIndex`/`ClearSelectedTargetPlate` 호출 확인, 변경 파일 대상 `git diff --check` 오류 없음. 전체 `git diff --check`는 기존 `UnityTestRunner_EditMode.log` trailing whitespace로 실패. `dotnet build Assembly-CSharp.csproj --no-restore`는 기존 `Temp\obj\Assembly-CSharp\project.assets.json` 누락(NETSDK1004)으로 코드 컴파일 전 중단.
- DEV-130: 공격자 Plate index 우회 갱신 경로(`PlateInputActions -> SummonController.SetPlayerSelectedIndex -> PlayerController -> PlayerActionFlow -> PlayerAttackActions`)를 제거했다. 공격자 index는 `SummonStatePanelView`가 패널을 연 플레이어 Plate index로 보관하고, `BattleController.AttackStart`가 `BattleAttackState.AttackingPlateIndex`로 넘긴다. `PlayerAttackActions`의 공격자 index 필드와 외부 setter는 제거했다.
- DEV-130 검증: 공격자 index 우회 setter 검색 0건. `PlayerAttackActions.cs`의 `selectedPlateIndex` 검색 0건. 변경 파일 대상 `git diff --check`는 whitespace 오류 없이 CRLF 경고만 출력. `dotnet build Assembly-CSharp.csproj --no-restore`는 기존 `Temp\obj\Assembly-CSharp\project.assets.json` 누락(NETSDK1004)으로 C# 컴파일 전 중단.
- DEV-131: `Plate.RestoreRedrawSummonState`의 재소환 후 상태 패널 갱신이 `selectedPlayerPlateIndex`를 기본값 `-1`로 덮지 않도록 현재 Plate의 player index를 명시 전달했다.
- DEV-131 검증: `SetStatePanel` 호출부 확인 결과 재소환 호출과 클릭 호출 모두 index를 명시 전달한다. `git diff --check -- Assets/Script/Battle/Unit/Plate/Plate.cs` 오류 없음. `dotnet build Assembly-CSharp.csproj --no-restore`는 기존 `Temp\obj\Assembly-CSharp\project.assets.json` 누락(NETSDK1004)으로 C# 컴파일 전 중단.
- DEV-132: `PlayerTargetSelectionActions`를 추가해 타겟형 특수공격의 타겟 선택 시작, 선택 대기, 외부 클릭 취소, 선택 완료 실행 흐름을 `PlayerAttackActions`에서 분리했다. `PlayerAttackActions`는 공격 가능 여부와 즉시/타겟 선택 분기만 담당한다.
- DEV-132 검증: `WaitForTargetPlateSelection`과 `MousePositionInsidePlates`는 `PlayerTargetSelectionActions.cs`에만 남았다. 변경 파일 대상 `git diff --check` 오류 없음. `dotnet build Assembly-CSharp.csproj --no-restore`는 기존 `Temp\obj\Assembly-CSharp\project.assets.json` 누락(NETSDK1004)으로 C# 컴파일 전 중단.
- DEV-133: `PlateTargetSelectionActions`를 추가해 공격 타겟 선택 중 Plate 클릭을 전투 타겟 선택 상태로 전달하는 책임을 `PlateInputActions`에서 분리했다. `PlateInputActions`는 클릭 라우팅만 유지하고, `BattleController.SelectTargetPlate` 직접 호출은 새 Actions로 이동했다.
- DEV-133 검증: `BattleController.SelectTargetPlate` 호출은 `PlateTargetSelectionActions.cs`에만 남았다. 변경 파일 대상 `git diff --check` 오류 없음. `dotnet build Assembly-CSharp.csproj --no-restore`는 기존 `Temp\obj\Assembly-CSharp\project.assets.json` 누락(NETSDK1004)으로 C# 컴파일 전 중단.
- COORD-134 판단: 실제 코드의 타겟 선택 API는 이미 `SelectSpecialAttackTargetPlate`, `GetSelectedSpecialAttackTargetPlateIndex`, `ClearSpecialAttackTargetSelection` 계열로 구체화되어 있다. target plate index는 `PlateTargetSelectionActions -> BattleController -> BattleAttackState -> PlayerTargetSelectionActions` 한 방향으로 흐르며, Plate 클릭이 공격 실행을 직접 우회하지 않는다. 따라서 COORD-134는 코드 변경 없이 닫고, 다음 판단은 `SummonStatePanelView`가 공격자 Plate index를 임시 보관하는 책임 경계로 이동한다.
- DEV-135: Battle Target Selection에서 공격자 source와 target plate index 흐름을 `BattleController -> BattleAttackState` 중심으로 정리했다. `SummonStatePanelView`의 공격자 Plate index 보관과 `BattleController`의 View 역참조를 제거했고, hover/click target 판정은 `PlateTargetSelectionActions`로 단일화했다.
- DEV-135 검증: `selectedPlayerPlateIndex`, `GetSelectedPlayerPlateIndex`, `GetStatePanelSummon` 잔존 검색 0건. `PlateInputActions`의 target 판정 중복 제거 확인. VerificationAgent가 공격 source View 우회 제거와 hover/click target 판정 중앙화를 확인했다. 변경 파일 대상 `git diff --check` 오류 없음. `dotnet build Assembly-CSharp.csproj --no-restore`는 기존 `Temp\obj\Assembly-CSharp\project.assets.json` 누락(NETSDK1004)으로 C# 컴파일 전 중단.

## 운영 메모

- 다음 코드 변경은 DevAgent Change Proposal과 적용 예정 diff를 먼저 제시한다.
- 다음 세션은 개별 예측 파일 보수 작업으로 바로 들어가지 않는다. 먼저 Battle Targeting의 선택 흐름 왕복을 정리한다.
- 씬 stale 연결 정리는 완료했다. 다음 코드 변경은 다시 Change Proposal을 제시한다.
- 다음 세션 첫 검토 영역: `Assets/Screen/FightScene/Fight Screen_1Stage.unity`부터 FightScene 1~7의 `BattleCanvas` 하위 UI 계층, 1Stage의 `UI_30_PlayerCommands`/`UI_90_ResultAlerts`와 2~7Stage 직속 버튼/텍스트 차이.
- 제외: 예측 확률 수치 변경, 주석/함수명만 반복 정리, 신규 공통 helper 추가, EnemyAction/Prediction에 장시간 머무는 세부 보수.
- Batch 진행 원칙: 큰 목표는 Battle 전체 단위로 잡고, 실제 diff는 승인 가능한 작은 슬라이스로 나눈다.
- 문서 상태 기록은 사용자 진행 승인 없이 바로 수행한다.
- active 문서에는 현재 슬라이스와 다음 호출 대상만 둔다.
