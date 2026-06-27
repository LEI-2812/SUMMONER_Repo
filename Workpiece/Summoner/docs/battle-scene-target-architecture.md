# Battle Scene Target Architecture

이 문서는 전투씬을 새로 구축한다고 가정했을 때 사용할 목표 구조를 정리한다.

기준은 최신 FigJam 보드 `SUMMONER Battle Full Logic`이다.

Figma: https://www.figma.com/board/Ua223ttkCRLHu7q6GZlMDf

현재 소스 구조를 그대로 복사하기 위한 문서가 아니다. 현재 전투씬에서 보존해야 하는 로직을 빠뜨리지 않되, Controller와 UseCase를 과하게 늘리지 않는 목표 구조로 다시 정리한다.

핵심 기준:

```text
Controller는 Unity 입력 묶음만 담당한다.
UseCase는 기능 내부 단계가 아니라 사용자 행동 또는 전투 진행 흐름 단위만 담당한다.
Rule, Provider는 목표 클래스에서 제외한다.
Strategy는 공격 대상 선정 방식이 실제로 바뀌므로 유지한다.
Status는 AttackData에 붙는 공격 속성이고, 적용 후 SummonEntity.ActiveStatuses에 남는다.
```

## 1. 폴더 구조

목표 폴더는 기능 흐름을 먼저 드러낸다. 빈 폴더는 미리 만들지 않고, 실제 구현 slice가 시작될 때 필요한 폴더만 만든다.

```text
Assets/Script
├─ Battle
│  ├─ 0_Presentation
│  │  ├─ Runtime
│  │  ├─ Player
│  │  ├─ Board
│  │  ├─ Result
│  │  └─ View
│  ├─ 1_Application
│  │  ├─ Start
│  │  ├─ Turn
│  │  ├─ Player
│  │  ├─ Attack
│  │  ├─ Enemy
│  │  └─ Result
│  ├─ 2_Domain
│  │  ├─ Runtime
│  │  ├─ Turn
│  │  ├─ Board
│  │  ├─ Attack
│  │  ├─ Prediction
│  │  └─ Result
│  └─ 3_Infrastructure
│     └─ Store
├─ Summon
│  ├─ 0_Presentation
│  ├─ 2_Domain
│  └─ 3_Infrastructure
├─ Save
└─ Stage
```

이 문서의 목표 클래스 수는 일부러 줄인다.

```text
Controller: 4개
UseCase: 9개
StateMachine: TurnStateMachine 1개
State: 현재 입력 가능 단계 표현
Strategy: 공격 대상 선정 방식만 유지
Store: 저장, 정적 데이터, 리소스 제공만 유지
```

사용하지 않는 목표 이름:

```text
BattleRuntimeController
BattleState
Context
Provider
Rule
Action
Runner
Manager
Helper
RandomAttackStrategy
```

`RandomAttackStrategy`는 현재 전투 로직에 근거가 없으므로 만들지 않는다.

## 2. 핵심 흐름

### 전체 전투 흐름

```text
Fight Scene
→ BattleSceneRuntime
→ StartBattleUseCase
→ BattleRuntimeData 초기화
→ StageDataStore에서 스테이지 데이터 로드
→ 적 배치
→ ChangeTurnUseCase
→ TurnStateMachine
→ PlayerTurnState 또는 EnemyTurnState
→ Controller 입력 또는 EnemyTurnState 자동 진행
→ 행동별 UseCase
→ BattleRuntimeData / Domain 변경
→ View 표시
→ ResultState
→ BattleResultController
→ CompleteBattleResultUseCase
→ StageProgressSaveStore
```

### 플레이어 행동 흐름

```text
PlayerCommandController
→ HandlePlayerCommandUseCase
→ StartSummonSelectionUseCase
  또는 ExecutePlayerAttackUseCase
  또는 ChangeTurnUseCase
```

`HandlePlayerCommandUseCase`는 버튼 명령을 플레이어 행동 흐름으로 라우팅한다. 공격 계산, 소환 배치, 저장 처리를 직접 View나 Controller에 두지 않는다.

### 소환 / 재소환 흐름

```text
PlayerCommandController
→ HandlePlayerCommandUseCase
→ StartSummonSelectionUseCase
→ SummonSelectionState
→ SummonPickView
→ SummonSelectionController
→ SelectSummonUseCase
→ Player.CurrentMana 차감
→ BattleBoardData에 소환수 배치
→ PlayerTurnState
```

소환과 재소환은 별도 Controller를 만들지 않는다. `SummonSelectionController`가 후보 선택, 재뽑기, 선택 확정 입력을 받는다.

재소환 대상 plate 선택은 `BattleBoardInputController → SelectBoardTargetUseCase`에서 현재 State를 보고 처리한다.

### 플레이트 / 타깃 선택 흐름

```text
BattleBoardInputController
→ SelectBoardTargetUseCase
→ 현재 State 확인
→ 소환 배치
  또는 재소환 대상 확정
  또는 공격 타깃 확정
  또는 소환수 상태 표시
```

플레이트 클릭을 기능별 Controller로 나누지 않는다. Plate 입력은 `BattleBoardInputController` 하나로 시작한다.

### 플레이어 공격 흐름

```text
PlayerCommandController
→ HandlePlayerCommandUseCase
→ ExecutePlayerAttackUseCase
→ BattleAttackData / SelectedActionData 확인
→ AttackData 선택
→ AttackStrategy로 대상 선정
→ Damage 적용
→ AttackData.Status 적용
→ PredictionData 생성
→ ChangeTurnUseCase
```

일반공격과 특수공격을 UseCase로 더 쪼개지 않는다. 둘 다 플레이어가 공격을 실행하는 하나의 행동 흐름이므로 `ExecutePlayerAttackUseCase`가 담당한다.

대상 선택이 필요한 공격은 `TargetSelectionState`와 `SelectBoardTargetUseCase`가 `SelectedActionData`에 타깃을 기록한 뒤 `ExecutePlayerAttackUseCase`로 이어진다.

### 공격 Strategy 흐름

```text
AttackData
→ AttackStrategy
→ TargetedAttackStrategy
  또는 AttackAllEnemiesStrategy
  또는 ClosestEnemyAttackStrategy
→ 대상 SummonEntity 목록 반환
```

`AttackStrategy`는 공격 방식 전체가 아니라 대상 선정 방식을 담당한다.

`Status`는 Strategy가 아니라 `AttackData`의 구성 요소다.

### Status 흐름

```text
AttackData
→ Status
→ ExecutePlayerAttackUseCase / ExecuteEnemyTurnUseCase
→ SummonEntity.ActiveStatuses
→ ChangeTurnUseCase에서 턴 갱신
→ SummonStatePanelView 표시
```

Status는 공격에 붙는 속성이다. 공격과 별도 실행 흐름으로 만들지 않는다.

### 적 턴 흐름

```text
EnemyTurnState
→ ExecuteEnemyTurnUseCase
→ 행동 가능한 적 소환수 순회
→ CanAct 확인
→ 필요한 경우 Heal 우선 판단
→ PlayerAttackPredictionUseCase로 예측 생성
→ 예측 기반 반응 판단
→ AttackData / AttackStrategy 사용
→ Damage와 Status 적용
→ 반복 공격 가능 여부 판단
→ ChangeTurnUseCase
```

적 판단은 `Rule` 클래스로 새로 만들지 않는다. `ExecuteEnemyTurnUseCase` 내부 private 단계로 둔다.

### 예측 흐름

```text
ExecutePlayerAttackUseCase
또는 ExecuteEnemyTurnUseCase
→ PlayerAttackPredictionUseCase
→ PlayerAttackPrediction
→ CatAttackPrediction / EagleAttackPrediction / FoxAttackPrediction / RabbitAttackPrediction / SnakeAttackPrediction / WolfAttackPrediction
→ AttackPredictionData
→ AttackProbabilityData
→ PredictionBoardData
→ PredictionView
```

예측 알고리즘은 소환수별로 다르므로 예측 클래스는 유지한다. 단, `PredictionRule` 같은 추가 Rule 계층은 만들지 않는다.

### 결과 / 저장 흐름

```text
ResultState
→ BattleResultAlertView
→ BattleResultController
→ CompleteBattleResultUseCase
→ BattleRuntimeData 확인
→ StageDataStore에서 클리어 기준 확인
→ StageProgressSaveStore 저장
→ 다음 스테이지 / 스테이지 선택 / 재시도
```

PlayerPrefs 직접 호출은 `StageProgressSaveStore`에서만 한다.

## 3. 주요 코드

### 3.1 Controller

Controller는 Unity 입력만 받고 UseCase를 호출한다.

#### PlayerCommandController

역할: 플레이어 턴의 버튼 입력을 받는다.

입력:

```text
소환 버튼
재소환 버튼
일반공격 버튼
특수공격 버튼
턴 종료 버튼
```

참조:

```text
HandlePlayerCommandUseCase handlePlayerCommandUseCase
```

함수:

```text
OnClickSummon()
OnClickRedraw()
OnClickNormalAttack()
OnClickSpecialAttack(int attackIndex)
OnClickEndTurn()
```

하지 않는 일:

```text
마나 판단
공격 가능 판단
데미지 계산
소환 배치
턴 전환
View 직접 계산
```

#### SummonSelectionController

역할: 소환 후보 UI에서 발생하는 입력을 받는다.

입력:

```text
후보 소환수 클릭
후보 재뽑기 클릭
소환 선택 취소
```

참조:

```text
SelectSummonUseCase selectSummonUseCase
```

함수:

```text
OnClickSummonCandidate(int index)
OnClickRedrawCandidates()
OnClickCancel()
```

하지 않는 일:

```text
후보 확률 계산
마나 차감
plate 배치
플레이어 행동 상태 변경
```

#### BattleBoardInputController

역할: 전투 보드와 plate 클릭 입력을 받는다.

입력:

```text
player plate 클릭
enemy plate 클릭
빈 plate 클릭
plate hover
타깃 선택 중 외부 클릭
```

참조:

```text
SelectBoardTargetUseCase selectBoardTargetUseCase
PlateBoardView plateBoardView
```

함수:

```text
OnClickPlate(int plateIndex)
OnEnterPlate(int plateIndex)
OnExitPlate(int plateIndex)
OnClickOutsideBoard()
```

하지 않는 일:

```text
공격 대상 확정 규칙
재소환 가능 여부 판단
상태 패널 내용 구성
소환 배치 규칙
```

#### BattleResultController

역할: 전투 결과 alert의 입력을 받는다.

입력:

```text
확인 버튼
다음 스테이지 버튼
스테이지 선택 버튼
재시도 버튼
```

참조:

```text
CompleteBattleResultUseCase completeBattleResultUseCase
```

함수:

```text
OnClickConfirm()
OnClickNextStage()
OnClickStageSelect()
OnClickRetry()
```

하지 않는 일:

```text
클리어 판정
PlayerPrefs 저장
스테이지 규칙 계산
```

### 3.2 UseCase

UseCase는 전투 진행 흐름 또는 플레이어/적 행동 흐름을 담당한다.

#### StartBattleUseCase

역할: 전투 시작 흐름을 담당한다.

참조:

```text
BattleRuntimeData battleRuntimeData
StageDataStore stageDataStore
ChangeTurnUseCase changeTurnUseCase
```

함수:

```text
Execute(int stageNumber)
```

내부 단계:

```text
LoadStageData()
InitializeRuntimeData()
PlaceStageEnemies()
StartFirstTurn()
```

변경 데이터:

```text
BattleStageData
BattleBoardData
BattleAttackData
```

#### ChangeTurnUseCase

역할: 턴 전환과 턴 시작/종료 시 필요한 전투 상태 갱신을 담당한다.

참조:

```text
TurnStateMachine turnStateMachine
BattleRuntimeData battleRuntimeData
```

함수:

```text
StartPlayerTurn()
StartEnemyTurn()
StartResult()
EndCurrentTurn()
```

내부 단계:

```text
UpdateTurnCount()
UpdateActiveStatuses()
ResetTurnActions()
CheckBattleResult()
ChangeState()
```

#### HandlePlayerCommandUseCase

역할: 플레이어 버튼 명령을 현재 상태에 맞는 행동 흐름으로 연결한다.

참조:

```text
BattleRuntimeData battleRuntimeData
StartSummonSelectionUseCase startSummonSelectionUseCase
ExecutePlayerAttackUseCase executePlayerAttackUseCase
ChangeTurnUseCase changeTurnUseCase
```

함수:

```text
ExecuteSummon()
ExecuteRedraw()
ExecuteNormalAttack()
ExecuteSpecialAttack(int attackIndex)
ExecuteEndTurn()
```

내부 단계:

```text
CheckPlayerTurn()
CheckActionAvailable()
SelectCommand()
ShowFeedbackIfBlocked()
```

#### StartSummonSelectionUseCase

역할: 소환 또는 재소환 후보 선택 상태를 시작한다.

참조:

```text
BattleRuntimeData battleRuntimeData
SummonDataStore summonDataStore
TurnStateMachine turnStateMachine
```

함수:

```text
ExecuteSummon()
ExecuteRedraw()
```

내부 단계:

```text
CheckMana()
CheckRedrawTargetIfNeeded()
BuildSummonCandidates()
SaveSelectedActionData()
ChangeToSummonSelectionState()
```

#### SelectSummonUseCase

역할: 후보 중 선택된 소환수를 확정하고 배치 대기 또는 즉시 교체 흐름으로 연결한다.

참조:

```text
BattleRuntimeData battleRuntimeData
Player player
```

함수:

```text
Execute(int candidateIndex)
ExecuteRedraw()
Cancel()
```

내부 단계:

```text
GetSelectedSummonData()
SpendMana()
CreateSummonEntity()
PlaceOnBoardIfTargetReady()
ReturnToPlayerTurn()
```

#### SelectBoardTargetUseCase

역할: plate 클릭을 현재 State에 맞는 의미로 처리한다.

참조:

```text
BattleRuntimeData battleRuntimeData
ExecutePlayerAttackUseCase executePlayerAttackUseCase
```

함수:

```text
Execute(int plateIndex)
Hover(int plateIndex)
Cancel()
```

내부 단계:

```text
IfSummonSelectionPlaceSummon()
IfRedrawSelectionSelectOwnPlate()
IfTargetSelectionSelectAttackTarget()
IfPlayerTurnShowSummonStatePanel()
UpdateBoardView()
```

#### ExecutePlayerAttackUseCase

역할: 플레이어 공격 실행 흐름을 담당한다.

참조:

```text
BattleRuntimeData battleRuntimeData
ChangeTurnUseCase changeTurnUseCase
PlayerAttackPredictionUseCase playerAttackPredictionUseCase
```

함수:

```text
ExecuteNormalAttack()
ExecuteSpecialAttack()
ExecuteSelectedAttack()
```

내부 단계:

```text
GetAttacker()
GetAttackData()
SpendManaIfNeeded()
SelectTargetsByAttackStrategy()
ApplyDamage()
ApplyAttackStatus()
UpdateCooldown()
BuildPredictionData()
FinishPlayerAction()
```

#### ExecuteEnemyTurnUseCase

역할: 적 턴 전체 실행 흐름을 담당한다.

참조:

```text
BattleRuntimeData battleRuntimeData
PlayerAttackPredictionUseCase playerAttackPredictionUseCase
ChangeTurnUseCase changeTurnUseCase
```

함수:

```text
Execute()
```

내부 단계:

```text
CollectEnemySummons()
SkipIfStunned()
TryHealBeforeReaction()
BuildPlayerAttackPredictions()
ReactToPrediction()
ChooseEnemyAttack()
SelectTargetsByAttackStrategy()
ApplyDamage()
ApplyAttackStatus()
RepeatIfCanContinue()
FinishEnemyTurn()
```

#### CompleteBattleResultUseCase

역할: 전투 결과 확정, 저장, 씬 이동 결정을 담당한다.

참조:

```text
BattleRuntimeData battleRuntimeData
StageDataStore stageDataStore
StageProgressSaveStore stageProgressSaveStore
```

함수:

```text
ExecuteConfirm()
ExecuteNextStage()
ExecuteStageSelect()
ExecuteRetry()
```

내부 단계:

```text
ReadBattleResult()
CheckStageResultCondition()
SaveClearedStage()
SelectNextScene()
```

### 3.3 Runtime / State

#### BattleSceneRuntime

역할: Fight Scene의 실행 진입점이다. Unity 입력 Controller가 아니므로 `Controller` 이름을 쓰지 않는다.

참조:

```text
BattleRuntimeData battleRuntimeData
StartBattleUseCase startBattleUseCase
```

함수:

```text
Start()
BuildRuntime()
ConnectSceneReferences()
```

#### BattleRuntimeData

역할: 현재 전투 중 필요한 데이터 묶음이다.

변수:

```text
BattleStageData StageData
BattleAttackData AttackData
BattleBoardData BoardData
SelectedActionData SelectedAction
PredictionData Prediction
Player Player
Enemy Enemy
```

넣지 않는 것:

```text
PlayerPrefs 저장
현재 State 객체
UI 표시 로직
전체 데미지 계산
전체 예측 알고리즘
```

#### TurnStateMachine

역할: 현재 State 보관과 State 전환만 담당한다.

변수:

```text
ITurnState CurrentState
```

함수:

```text
ChangeState(ITurnState nextState)
```

전환 순서:

```text
CurrentState.Exit()
CurrentState = nextState
CurrentState.Enter()
```

#### ITurnState

역할: State 공통 규약이다. StateMachine에 필요한 최소 인터페이스로만 둔다.

함수:

```text
Enter()
Exit()
```

#### PlayerTurnState

역할: 플레이어 기본 행동 입력 가능 상태를 표현한다.

Enter:

```text
PlayerHudView 열기
ManaView 갱신
PlayerCommandController 입력 가능
```

Exit:

```text
PlayerCommandController 입력 잠금
필요한 임시 View 닫기
```

#### SummonSelectionState

역할: 소환 후보를 선택 중인 상태를 표현한다.

Enter:

```text
SummonPickView 열기
SummonSelectionController 입력 가능
```

Exit:

```text
SummonPickView 닫기
SummonSelectionController 입력 잠금
```

#### TargetSelectionState

역할: 공격 타깃 plate를 선택 중인 상태를 표현한다.

Enter:

```text
TargetSelectionView 열기
BattleBoardInputController target 선택 모드
```

Exit:

```text
TargetSelectionView 닫기
Plate highlight 초기화
```

#### EnemyTurnState

역할: 적 턴이 자동 실행되는 상태를 표현한다.

Enter:

```text
플레이어 입력 잠금
ExecuteEnemyTurnUseCase.Execute()
```

Exit:

```text
적 턴 임시 표시 제거
```

#### ResultState

역할: 전투 결과 표시 상태를 표현한다.

Enter:

```text
BattleResultAlertView 열기
BattleResultController 입력 가능
```

Exit:

```text
BattleResultAlertView 닫기
```

### 3.4 Battle Data

#### BattleStageData

역할: 현재 전투의 스테이지 진행 데이터를 담는다.

변수:

```text
int CurrentStage
int ClearTurnLimit
bool IsBossStage
```

#### BattleAttackData

역할: 현재 공격 선택 데이터를 담는다.

변수:

```text
SummonEntity Attacker
AttackData SelectedAttack
int SelectedSpecialAttackIndex
int SelectedTargetPlateIndex
bool IsWaitingTarget
```

#### BattleBoardData

역할: 전투 보드의 player/enemy plate 상태를 담는다.

변수:

```text
List<PlateData> PlayerPlates
List<PlateData> EnemyPlates
```

함수:

```text
GetPlate(int index)
PlaceSummon(int plateIndex, SummonEntity summon)
RemoveSummon(int plateIndex)
GetAlivePlayerSummons()
GetAliveEnemySummons()
```

#### SelectedActionData

역할: 현재 플레이어가 진행 중인 선택 행동을 담는다.

변수:

```text
PlayerActionType ActionType
SummonData SelectedSummonData
SummonEntity SelectedSummonEntity
int SelectedSourcePlateIndex
int SelectedTargetPlateIndex
int SelectedAttackIndex
```

#### PredictionData

역할: 예측 결과를 View와 적 판단 흐름에 전달한다.

변수:

```text
PredictionBoardData Board
List<AttackPredictionData> AttackPredictions
```

### 3.5 Domain

#### Player

역할: 전투 중 플레이어 상태를 담당한다.

변수:

```text
int CurrentMana
int MaxMana
```

함수:

```text
CanUseMana(int amount)
SpendMana(int amount)
RecoverMana(int amount)
ResetTurnMana()
```

마나는 Store에 두지 않는다.

#### Enemy

역할: 적 진영의 전투 상태를 담당한다.

변수:

```text
int CurrentActionIndex
```

함수:

```text
GetActingSummons()
CanContinueAttack(SummonEntity summon)
```

#### SummonEntity

역할: 현재 전투 중 소환수 상태값을 담당한다.

변수:

```text
string Name
SummonRank Rank
int CurrentHp
int MaxHp
int AttackPower
int Shield
bool CanAct
bool HasOnceInvincibility
List<Status> ActiveStatuses
AttackData NormalAttack
List<AttackData> SpecialAttacks
```

함수:

```text
TakeDamage(int amount)
Heal(int amount)
ApplyShield(int amount)
ApplyStatus(Status status)
UpdateStatuses(StatusTiming timing)
CanUseAttack(AttackData attackData)
```

#### SummonData

역할: 소환수 생성에 필요한 정적 데이터를 담는다.

변수:

```text
string Name
SummonRank Rank
int MaxHp
int AttackPower
int ManaCost
AttackData NormalAttack
List<AttackData> SpecialAttacks
```

#### PlateData

역할: plate 하나의 현재 상태를 담는다.

변수:

```text
int Index
bool IsPlayerSide
SummonEntity CurrentSummon
```

함수:

```text
IsEmpty()
HasSummon()
CanPlaceSummon()
```

### 3.6 Attack / Strategy / Status

#### AttackData

역할: 공격 하나의 정적/런타임 실행 정보를 담는다.

변수:

```text
string Name
int Damage
int ManaCost
int Cooldown
int CurrentCooldown
AttackType Type
AttackStrategy Strategy
Status Status
bool TargetsOwnSide
```

함수:

```text
CanUse()
StartCooldown()
ReduceCooldown()
```

#### AttackStrategy

역할: 공격 대상 선정 방식이다.

함수:

```text
SelectTargets(AttackTargetInput input)
```

데미지 적용과 Status 적용은 Strategy가 아니라 공격 실행 UseCase에서 한다.

#### TargetedAttackStrategy

역할: 선택한 plate의 소환수를 대상으로 삼는다.

대상:

```text
SelectedTargetPlateIndex
```

#### AttackAllEnemiesStrategy

역할: 상대 진영의 모든 살아있는 소환수를 대상으로 삼는다.

대상:

```text
All alive enemy summons
```

#### ClosestEnemyAttackStrategy

역할: 가장 가까운 적 소환수를 대상으로 삼는다.

대상:

```text
Closest alive enemy summon
```

#### AttackTargetInput

역할: Strategy가 대상을 고를 때 필요한 입력값 묶음이다.

변수:

```text
SummonEntity Attacker
BattleBoardData BoardData
int SelectedPlateIndex
bool IsPlayerAttack
```

`Context` 이름은 사용하지 않는다.

#### Status

역할: 공격에 붙는 상태 속성이고, 적용 후 소환수의 활성 상태로 남는다.

변수:

```text
StatusType Type
StatusTiming Timing
int Value
int Duration
bool IsExpired
```

함수:

```text
ApplyTo(SummonEntity target)
UpdateTurn(SummonEntity target)
Expire(SummonEntity target)
```

#### StatusType

역할: Status 종류를 표현한다.

값:

```text
Poison
Burn
LifeDrain
Heal
Shield
Upgrade
Curse
Stun
OnceInvincibility
```

#### StatusTiming

역할: Status 갱신 타이밍을 표현한다.

값:

```text
OnApply
TurnStart
TurnEnd
Damage
Upgrade
StunAndCurse
```

### 3.7 Prediction

#### PlayerAttackPredictionUseCase

역할: 플레이어 공격 예측 데이터를 만든다.

참조:

```text
BattleRuntimeData battleRuntimeData
PlayerAttackPrediction playerAttackPrediction
```

함수:

```text
Execute()
BuildPredictionBoard()
BuildAttackPredictions()
```

#### PlayerAttackPrediction

역할: 소환수별 예측 알고리즘을 찾아 실행한다.

참조:

```text
List<IAttackPrediction> predictions
```

함수:

```text
GetPrediction(SummonEntity summon)
```

#### IAttackPrediction

역할: 소환수별 예측 알고리즘 공통 규약이다.

함수:

```text
CanPredict(SummonEntity summon)
GetAttackPrediction(SummonEntity summon, PredictionInput input)
```

#### CatAttackPrediction

역할: Cat의 공격 선택과 타깃 예측을 담당한다.

판단 요소:

```text
가까운 적
처치 가능 여부
특수공격 사용 가능 여부
```

#### EagleAttackPrediction

역할: Eagle의 공격 선택과 타깃 예측을 담당한다.

판단 요소:

```text
체력 낮은 대상
특수공격 사용 가능 여부
```

#### FoxAttackPrediction

역할: Fox의 공격 선택과 타깃 예측을 담당한다.

판단 요소:

```text
강화/저주/상태 여부
특수공격 사용 가능 여부
```

#### RabbitAttackPrediction

역할: Rabbit의 공격 선택과 타깃 예측을 담당한다.

판단 요소:

```text
아군 체력 비율
가장 낮은 체력 대상
회복 판단
```

#### SnakeAttackPrediction

역할: Snake의 공격 선택과 타깃 예측을 담당한다.

판단 요소:

```text
독 상태 여부
적 수
적 체력 비율
특수공격 사용 가능 여부
```

#### WolfAttackPrediction

역할: Wolf의 공격 선택과 타깃 예측을 담당한다.

판단 요소:

```text
처치 가능 대상
가까운 적
특수공격 사용 가능 여부
```

#### AttackPredictionData

역할: 한 공격 예측 결과를 담는다.

변수:

```text
SummonEntity Attacker
int AttackerPlateIndex
AttackData PredictedAttack
int TargetPlateIndex
AttackProbabilityData Probability
```

#### AttackProbabilityData

역할: 예측 확률 정보를 담는다.

변수:

```text
float NormalAttackProbability
float SpecialAttackProbability
float TargetProbability
```

#### PredictionBoardData

역할: 보드 단위 예측 결과를 담는다.

변수:

```text
List<PredictionPlateData> PlayerPlates
List<PredictionPlateData> EnemyPlates
```

#### PredictionPlateData

역할: 예측용 plate 상태를 담는다.

변수:

```text
int PlateIndex
SummonEntity PredictedSummon
List<AttackPredictionData> IncomingAttacks
```

### 3.8 Store

Store는 저장, 리소스, 고정 데이터 제공만 담당한다.

#### StageDataStore

역할: 스테이지 규칙과 적 배치 데이터를 제공한다.

제공 데이터:

```text
StageResultData
StageEnemyPlacementData
```

함수:

```text
LoadStageResultData()
LoadStageEnemyPlacementData(int stageNumber)
```

#### SummonDataStore

역할: 소환 후보에 사용할 SummonData를 제공한다.

함수:

```text
LoadAll()
GetCandidates(int count)
GetRedrawCandidates(int count)
```

#### StatusDataStore

역할: Status 정적 데이터를 제공한다.

함수:

```text
Get(StatusType type)
```

#### StageProgressSaveStore

역할: PlayerPrefs 기반 스테이지 진행도 저장/로드를 담당한다.

함수:

```text
Load()
Save(GameSaveData data)
SaveClearedStage(int stage)
```

PlayerPrefs 직접 호출은 이 클래스에서만 한다.

### 3.9 Result Data

#### StageResultData

역할: 스테이지별 전투 결과 조건을 담는 ScriptableObject다.

변수:

```text
List<StageResultCondition> Conditions
```

#### StageResultCondition

역할: 한 스테이지의 결과 판정 조건을 담는다.

변수:

```text
int StageNumber
int ClearTurnLimit
bool UnlockNextStage
```

#### StageEnemyPlacementData

역할: 스테이지별 적 배치 정보를 담는 ScriptableObject다.

변수:

```text
List<StageEnemyPlacement> Placements
```

#### StageEnemyPlacement

역할: 한 스테이지의 적 plate 배치를 담는다.

변수:

```text
int StageNumber
List<EnemyPlacementSlot> Slots
```

#### EnemyPlacementSlot

역할: 적 소환수 하나의 배치 위치를 담는다.

변수:

```text
int PlateIndex
SummonData EnemySummonData
```

### 3.10 View

View는 UI 표시만 담당한다. UseCase를 직접 호출하지 않는다.

#### PlayerHudView

역할:

```text
플레이어 버튼 활성화 표시
플레이어 턴 HUD 열기/닫기
```

#### ManaView

역할:

```text
현재 마나 표시
마나 이미지/텍스트 갱신
```

#### SummonPickView

역할:

```text
소환 후보 표시
재뽑기 버튼 표시
후보 선택 UI 열기/닫기
```

#### PlateBoardView

역할:

```text
전체 plate 보드 표시
plate highlight 초기화
plate 투명도 갱신
```

#### PlateView

역할:

```text
plate 하나의 소환수 이미지, 상태, 선택 표시
```

#### PlateVisualView

역할:

```text
plate 색상
highlight
투명도
hover 표시
```

#### TargetSelectionView

역할:

```text
공격 타깃 선택 중인 UI 표시
선택 가능 plate 강조
취소 시 표시 복구
```

#### SummonStatePanelView

역할:

```text
소환수 상세 상태 표시
HP / 공격력 / Status 목록 표시
```

#### PredictionView

역할:

```text
예측 공격 표시
예측 확률 표시
위험 plate 표시
```

#### BattleResultAlertView

역할:

```text
승리/패배/클리어 결과 표시
다음 행동 버튼 표시
```

#### PlayerFeedbackView

역할:

```text
행동 불가 메시지
마나 부족 메시지
잘못된 타깃 메시지
```

## 4. 실행 방법

이 문서는 목표 구조 문서이므로 직접 실행 대상이 아니다.

구현 검증 순서:

```text
1. 문서 기준으로 현재 slice의 변경 대상만 고른다.
2. 이미 완료된 기능은 docs/refactor-done.md의 재작업 금지 조건을 확인한다.
3. Controller 변경은 UnityEvent와 scene serialized reference 영향을 확인한다.
4. UseCase 변경은 대표 입력부터 결과까지 한 방향으로 읽히는지 확인한다.
5. Strategy / Status 변경은 일반공격, 특수공격, 적 턴, 예측에 모두 영향이 있는지 확인한다.
6. Store 변경은 PlayerPrefs 직접 호출 위치가 StageProgressSaveStore로 제한되는지 확인한다.
```

수동 테스트 기준:

```text
1. 전투 시작 시 stage enemy placement가 적용된다.
2. 플레이어 턴 HUD와 마나가 표시된다.
3. 소환 버튼으로 후보 UI가 열리고 소환수가 빈 plate에 배치된다.
4. 재소환 흐름에서 player plate 선택 후 후보 선택과 교체가 된다.
5. 일반공격이 가장 가까운 적 또는 현재 전략 대상에게 적용된다.
6. 대상 지정 특수공격은 target selection 후 적용된다.
7. 전체 공격은 모든 유효 대상에게 적용된다.
8. 공격에 붙은 Status가 대상 SummonEntity.ActiveStatuses에 추가된다.
9. 턴 전환 시 Status 지속시간과 효과가 갱신된다.
10. 적 턴에서 예측, 회복 우선 판단, 반응 공격, 반복 공격이 기존 의도대로 실행된다.
11. Cat/Eagle/Fox/Rabbit/Snake/Wolf 예측이 PredictionView에 표시된다.
12. 승리/패배/턴 제한 결과가 BattleResultAlertView에 표시된다.
13. 스테이지 클리어 시 StageProgressSaveStore가 진행도를 저장한다.
```

## 5. 나중에 확장할 수 있는 부분 / 다음 작업

이 문서 기준으로 다음 구현 slice를 고를 때는 기능 흐름 단위로만 고른다.

우선순위 후보:

```text
다음 작업: BATTLE-DOC-TO-CODE-01 | Battle | Controller 4개 목표와 현재 Controller 차이 분석
첫 액션: PlayerInputController, SummonSelectionController, PlateBoardController, BattleResultController 호출 흐름 비교
호출 대상: DevAgent
```

```text
다음 작업: BATTLE-USECASE-MERGE-01 | Battle | 과분리 UseCase 병합 후보 분석
첫 액션: StartSummonUseCase / StartRedrawUseCase / PlayerAttackUseCase / ExecuteNormalAttackUseCase / ExecuteSpecialAttackUseCase 책임 비교
호출 대상: DevAgent
```

```text
다음 작업: BATTLE-ATTACK-STATUS-01 | Battle | AttackData와 Status 관계 정리
첫 액션: AttackStrategy, Status, SummonEntity.ActiveStatuses 연결 흐름 확인
호출 대상: DevAgent
```

주의:

```text
문서의 Enemy Decision inside UseCase, CanAct Check, Heal Before Reaction, Continue Attack Check는 새 클래스 이름이 아니다.
이 항목들은 ExecuteEnemyTurnUseCase 내부 private 단계로 표현한다.

Place Stage Enemies도 새 UseCase 이름이 아니다.
StartBattleUseCase 내부 단계로 표현한다.

Rule / Provider는 목표 클래스 목록에 넣지 않는다.
Strategy는 AttackStrategy 계열만 유지한다.
Status는 AttackData에 포함되고 SummonEntity.ActiveStatuses에 적용된다.
```
