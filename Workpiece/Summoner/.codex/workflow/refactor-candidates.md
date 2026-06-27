# Refactor Candidates

CoordinatorAgent가 다음 개발 후보를 고를 때 보는 후보 목록이다.
완료 이력은 누적하지 않고, 실제로 진행할 후보와 보류 이유만 둔다.

## 선정 기준

1. 새 클래스를 추가하기보다 얇은 래퍼를 줄인다.
2. public 계약을 줄여 수정 영향 범위를 줄인다.
3. View가 저장, 씬 이동, 결과 처리, 입력 정책을 직접 우회하는 흐름을 줄인다.
4. 큰 클래스는 바로 쪼개지 않고 대표 흐름 하나를 좁힌다.
5. Scene, prefab, serialized field 영향이 있으면 적용 전 별도 승인한다.
6. 후보를 Ready로 올릴 때 완료조건을 먼저 정하고, 완료조건을 만족하면 같은 기능의 미세정리를 다음 작업으로 잡지 않는다.

## 구조 목표 달성 계획

목표는 `.codex/workflow/architecture-role-rule.md`의 `Source Structure Direction`을 실제 코드에 점진적으로 맞추는 것이다.

```text
기능 흐름 먼저
-> Controller / Runtime
-> UseCase
-> StateMachine / Rule
-> Entity / Store
-> View 갱신
```

진행 순서:

0. `Battle/Result`: 전투 Controller에 남은 저장 진행도 변경, 씬 이동, 결과 후처리 결정을 `UseCase` 중심으로 줄인다. 이 목표가 완료되기 전까지 다른 구조 후보로 우회하지 않는다.
1. `Battle/Attack`: `AttackStateMachine`이 UseCase를 만들거나 실행하지 않게 한다.
2. `Turn / PlayerTurn`: `TurnRuntime -> PlayerTurnState -> PlayerController -> PlayerTurnUseCase` 왕복 흐름을 한 방향으로 줄인다.
3. `Battle/PlayerAttack`: 공격 준비, 타겟 선택 대기, 실행, 피드백 책임을 대표 흐름 기준으로 정리한다.
4. `Battle/Enemy`: `Rule`은 판단만, 실행은 `EnemyTurnUseCase`가 맡도록 위치와 이름을 맞춘다.
5. `Option / Menu / Stage / Story`: View/Handler가 저장, 씬 이동, 종료, 진행도 처리를 직접 하는 우회 흐름을 줄인다.

각 단계는 하나의 기능 slice로 닫는다. 폴더 대이동, scene/prefab/ScriptableObject 값 변경, public serialized field 변경은 별도 위험 작업으로 분리한다.

## Ready

| ID | 도메인 | 기능 슬라이스 | 목표 | 완료조건 | 첫 액션 | 호출 대상 |
|---|---|---|---|---|---|---|
| PLAYER-CONTROLLER-DELETE-002 | Battle/Player | PlayerController 삭제를 한 번에 처리 | 전투 입력 경계에서 불필요한 PlayerController component를 제거한다 | FightScene 1~7의 버튼 UnityEvent와 serialized reference가 `PlayerController` 없이 동작하는 구조로 이관되고, `PlayerController.cs/.meta`와 scene component 참조가 제거된다. 새 Controller는 만들지 않는다 | FightScene 1~7의 `PlayerController` GUID, Button UnityEvent 메서드, `battleResultController`/`summonController`/`turnRuntime` serialized 참조를 검색해 이관 대상 목록을 확정한다 | DevAgent |

## Backlog

| ID | 도메인 | 기능 슬라이스 | 목표 | 완료조건 | 보류 이유 |
|---|---|---|---|---|---|
| COORD-SCAN-002 | Structure | 다음 구조 slice 후보 점검 | 남은 구조 혼선 후보를 분류하고 다음 Ready 후보 하나만 고른다 | 코드/scene/test 검색으로 후보가 분류되고 Ready 후보 하나가 선정된다 | 전투 Controller 책임 축소 목표가 끝나기 전까지 우회하지 않는다 |

## 다음 세션 목표

- 외부 시각화 산출물은 다음 작업 기준으로 사용하지 않는다. 목표와 완료조건은 이 문서와 `current-task.md`만 본다.
- `PLAYER-CONTROLLER-DELETE-002`는 하나의 큰 slice로 처리한다. 버튼 UnityEvent 이관, UseCase 생성 위치 이관, view 연결 위치 이관, scene component 제거, 파일 삭제, stale serialized field 제거, 검색 검증, Unity 재컴파일, FightScene 1~7 load 확인까지 묶는다.
- 새 Controller를 만들지 않는다. 우선 기존 `TurnRuntime` 또는 명확한 기존 runtime 경계로 흡수하고, 구조가 불가능할 때만 사용자에게 보고한다.
- TestRunner는 실행하지 않는다. 검증 게이트는 검색, `recompile_scripts`, FightScene 1~7 `load_scene`이다.

## 최근 처리

- COORD-141: `SummonPlaceExecutor` 얇은 위임을 `SummonController` private 단계로 통합했다.
- COORD-142: `IAttackPrediction` public 계약을 dispatcher가 쓰는 메서드로 축소했다.
- COORD-143: 씬 연결을 건드리지 않는 범위에서 `ToMainAlertHandler`, `ToQuitAlertHandler`의 빈 override를 제거했다. `MenuView`/`MenuHandler` 통합은 씬 영향이 있어 보류한다.
- COORD-144: `PlayerTargetSelectionActions`는 대상 선택/취소/선택 완료 콜백만 맡고, 타겟형 특수공격 실행과 후처리는 `PlayerAttackActions`가 맡도록 정리했다.
- COORD-145: `GameplaySettingView`의 효과 없는 입력 차단 `Update()`를 제거했다. `OnlyMouse`의 실제 적용은 `StoryScenarioControllerBase`의 `GameplaySettingStore.LoadOnlyMouseEnabled()` 경로가 맡는다.
- COORD-146: `PlayerActionExecutor`의 공격 실행/후처리 얇은 래퍼를 제거하고, 해당 실행 흐름을 `PlayerAttackActions`로 돌렸다. `PlayerActionExecutor`는 현재 소환 실행 역할만 남았다.
- COORD-147: fallback-only 소환수 클래스는 고유 행동이 적어도 16개 prefab `m_Script` GUID와 직접 연결되어 있어 삭제/통합을 보류했다.
- COORD-148: `Summon.NotifyObservers()` private 축소는 `UpdateStateObserver` 인터페이스 구현 계약을 깨므로 복구했다. 알림 public 계약 정리는 인터페이스 재설계 없이는 진행하지 않는다.
- COORD-149: 외부 호출 없는 `Summon` setter 5개를 제거했다. `SetNowHP`, `SetAttackPower`, `SetOnceInvincibility`, `SetIsAttack` 등 실제 호출 계약은 유지했다.
- COORD-152: Prediction 매칭에서 옛 소환 타입 비교를 제거하고, 각 예측기가 `CanPredict(Summon)`으로 자기 대상 여부를 판단하도록 바꿨다.
- COORD-161: `Summon`의 런타임 타입 field/API와 fallback 타입 인자를 제거했다.
- COORD-156: `PlayerAttackCondition`의 두 조건을 `PlayerAttackActions` private 메서드로 되돌리고 별도 조건 파일을 제거했다.
- COORD-163: `SummonType` enum, `SummonData.summonType`, `SummonData.GetSummonType()`, 16개 `*SummonData.asset`의 `summonType:` 값을 제거했다.
- COORD-164: `TurnSummonStateUpdater`의 턴 시작 상태효과 갱신 단계를 분리하고 일반 공격 쿨타임 갱신 null 방어를 추가했다.
- COORD-154: C# 외부 직접 `isAttacking` field 호출부는 없고 대표 기준은 `BattleAttackState`로 정리되어 있다. 다만 FightScene 1~7에 `isAttacking:` serialized 값이 남아 field 제거는 별도 scene 영향 승인 전까지 보류한다.
- COORD-155: 일반 소환의 빈 플레이어 Plate 조회를 `PlateController.GetFirstEmptyPlayerPlateIndex()`로 좁혔다. `GetPlayerPlates()` 전체 제거와 공격/예측 흐름 재설계는 이번 슬라이스에서 제외했다.
- COORD-157: `StoryImageView`가 `InteractionController`에서 현재 대사 인덱스를 직접 조회하지 않고 `InteractionController.OnPointerClick()`에서 인덱스를 전달받도록 좁혔다.
- COORD-159: `StorySkipView`가 `SettingPanelView`를 직접 조회하지 않고 `GameplaySettingStore.LoadStorySkipEnabled()`로 스킵 버튼 활성 여부를 판단하도록 좁혔다.
- COORD-158: 씬 연결이 없는 상태를 고려해 `GameObject.Find("MenuCanvas")`는 유지하고, 옵션 패널 계층 문자열 탐색만 `FindSettingPanelTransform()`으로 모았다.
- COORD-160: `Summon.Die()`는 Plate를 직접 찾지 않는 현재 구조를 유지했고, `PlateCompactExecutor`가 기존 Plate를 먼저 비운 뒤 새 Plate에 summon을 이동하도록 순서를 바로잡았다.
- COORD-151: 플레이어/적 소환수 초기화 대표 흐름이 `Plate.SummonPlaceOnPlate()` -> `Summon.SummonInitialize()`임을 확인했고, 초기화 완료 알림을 `SummonInitialize()` 한 곳으로 모았다.
- 테스트 기대값 정리: `TargetedAttackStrategy` 정적 검사 기대값을 현재 `effectValue` 흐름에 맞췄고 `PlateSummonDrawStaticRegressionTests` 전체 클래스가 통과했다.
- COORD-165: Story Screen 1/2/3/5/7의 `StorySkipView` UnityEvent 메서드명을 현재 public 메서드 `SkipAlert`에 맞췄고, Story 정적 회귀 테스트에 재발 방지 검사를 추가했다.
- COORD-SCAN: Story/Stage, Battle runtime, View/Option/HUD를 병렬 explorer 3개로 수정 없이 점검했고, Ready 후보를 BTL-SCENE-03으로 좁혔다.
- BTL-SCENE-03: `StageSceneConnectionEditModeTests`에 FightScene 1~7 Plate runtime 연결과 `PlayerPlates`/`EnemyPlate` prefab `spawnTransform` 검증을 추가했다. `StageSceneConnectionEditModeTests` 7/7, 전체 EditMode 146/146, 전체 PlayMode 26/26 통과.
- Runtime null 1차 수습: Start Screen HUD additive 로드 경로와 `MenuCanvas` 지연 조회, FightScene 1~7 `PlayerController` UnityEvent 타입, PlayMode stale expectation을 정리했다.
- SCENE-NULL-01: Build Settings enabled scene과 `Assets/Prefabs` 전체 missing script 스캔, 소환수 prefab `shieldImage` 연결 계약을 추가했다. `Grass Spirit.prefab`, `HighDevil.prefab`의 기존 `ShieldImage` GameObject를 `Summon.shieldImage`에 연결했다. `StageSceneConnectionEditModeTests` 9/9, 전체 EditMode 148/148, 전체 PlayMode 26/26 통과.
- BATTLE-RESULT-CONTROLLER-001: `BattleResultProgressUseCase`를 추가해 저장 진행도 변경과 전투 결과 후 씬 이동 결정을 Application으로 옮겼다. `BattleProgressController`는 FightScene component 경계와 위임만 맡는다. 보강으로 `GameSaveController.cs/.meta`와 FightScene 1~7/Stage Select의 빈 scene object를 제거했고, 저장/스테이지 진행도 조회는 `GameSaveUseCase -> PlayerPrefsSaveStore`로 통일했다. `Assets/Refresh`, `recompile_scripts` 0 warning. TestRunner는 사용자 요청대로 실행하지 않았다.
- BATTLE-START-CONTROLLER-001: `BattleStartUseCase`를 추가해 적 배치 후 턴 시작 순서를 Application으로 옮겼다. `BattleStartController`는 Unity 생명주기, serialized reference 확보, 중복 시작 방지만 맡는다. FightScene 1~7의 기존 Controller GUID와 `turnController` serialized field 호환은 유지했다. `Assets/Refresh`, `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- BATTLE-ENEMY-PLACEMENT-001: `BattleEnemyPlacementUseCase`를 추가해 stage data 순회, plate index 검증, summon prefab 검증, 적 소환 배치, stage multiplier 적용을 Application으로 옮겼다. `BattleEnemyPlacementController`는 scene reference 확보와 enemy plate 목록 전달만 맡는다. `Assets/Refresh`, `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- BATTLE-ENEMY-ATTACK-CONTROLLER-001: `EnemyAttackUseCase`를 추가해 player attack prediction 생성, enemy plate 순회, `EnemyTurnUseCase.ExecuteEnemyTurn()` 반복 실행을 Application으로 옮겼다. `EnermyAttackController`는 scene reference 확인과 UseCase 호출 경계로 남겼다. `Assets/Refresh`, `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- BATTLE-PLATE-CONTROLLER-001: `PlateCompactUseCase`를 추가해 enemy plate 압축 정책을 Application으로 옮겼다. `PlateController`는 `CompactEnermyPlates()` public entrypoint와 scene plate list 전달만 맡는다. `Assets/Refresh`, `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- BATTLE-RESULT-PROGRESS-002: `BattleProgressController`와 `BattleStageContext`에서 `PlayerPrefsSaveStore` 직접 생성을 제거했다. 현재 stage fallback과 결과 후 저장/씬 이동 조립은 `BattleResultProgressUseCase`가 맡는다. `Battle/0_Presentation/Result` 저장/씬 이동 검색 0건, `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- BATTLE-PROGRESS-CONTROLLER-DELETE-001: `BattleProgressController.cs/.meta`와 FightScene 1~7의 해당 component를 제거했다. 결과 alert callback 후 저장/씬 이동 처리는 `BattleResultController -> BattleResultProgressUseCase`로 직접 이어진다. 검색 결과 `BattleProgressController` 이름/GUID/fileID 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, FightScene 1~7 Unity `load_scene` 성공. TestRunner는 실행하지 않았다.
- BATTLE-START-CONTROLLER-DELETE-001: `BattleStartController.cs/.meta`와 단순 wrapper였던 `BattleStartUseCase.cs/.meta`를 제거했다. 적 배치 후 턴 시작 흐름은 `TurnRuntime.Start() -> StartBattleFlow() -> EnemyPlacementApply() -> StartTurnFlow()`로 흡수했다. 검색 결과 `BattleStartController` 이름/GUID/fileID 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, FightScene 1~7 Unity `load_scene` 성공. TestRunner는 실행하지 않았다.
- BATTLE-ENEMY-PLACEMENT-CONTROLLER-DELETE-001: `BattleEnemyPlacementController.cs/.meta`를 제거했다. `StageEnemyPlacementData` scene 참조와 적 배치 UseCase 호출은 `TurnRuntime` 시작 흐름으로 흡수했다. 검색 결과 `BattleEnemyPlacementController` 이름/GUID/fileID 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, FightScene 1~7 Unity `load_scene` 성공. TestRunner는 실행하지 않았다.
- BATTLE-ENEMY-ATTACK-CONTROLLER-DELETE-001: `EnermyAttackController.cs/.meta`를 제거했다. `EnemyAttackUseCase` 실행은 기존 `Enermy` runtime component가 직접 맡는다. 검색 결과 `EnermyAttackController` 이름/GUID/호출 메서드 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, FightScene 1~7 Unity `load_scene` 성공. TestRunner는 실행하지 않았다.
- BATTLE-REMAINING-CONTROLLER-REVIEW-001: 남은 `PlateController`, `PlayerController`, `BattleResultController`를 검토했다. 각각 scene plate list registry와 plate highlight, Unity button event와 PlayerView wiring, result alert callback과 `IBattleResultFlow` 구현 경계라 얇은 wrapper가 아니다. 이번 Controller 삭제 목표에서는 유지한다.
- ATTACK-STATE-MACHINE-BOUNDARY-001: `AttackStateMachine`에서 `PlateController` 생성자 의존과 `ResetAllPlateHighlight()` 호출을 제거했다. Domain 상태머신은 공격 상태만 reset하고, plate highlight reset은 `PlateController`를 이미 가진 호출자가 처리한다. `AttackStateMachine`의 Presentation 의존 검색 0건, Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- PLAYER-CONTROLLER-PUBLIC-API-001: scene UnityEvent나 코드 호출이 없는 `PlayerController.HasSummonedThisTurn()`, `UpdateManaUI()`, `AddMana()` public wrapper를 제거했다. `SetHasSummonedThisTurn`, button entrypoint, `GetPlayerTurnUseCase`는 실제 호출부가 있어 유지했다. 제거 wrapper 외부 호출 검색 0건, Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- PLATE-CONTROLLER-APPLICATION-DEPENDENCY-001 1차: `EnemyPredictionPlateState`가 `PlateController` 전체 대신 `IReadOnlyList<Plate>` player/enemy 목록만 받아 snapshot을 만들도록 낮췄다. `EnemyPredictionPlateState`의 `PlateController` 직접 의존 검색 0건, Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- PLATE-PREDICTION-BUILDER-DEPENDENCY-001: `PlayerAttackPredictionListBuilder`가 `PlateController` 전체 대신 player/enemy plate 목록을 받아 prediction snapshot을 만들도록 낮췄다. builder의 `PlateController`/plate 조회 직접 의존 검색 0건, Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- SPECIAL-ATTACK-PLATE-LISTS-001: `SpecialAttackUseCase`가 `PlateController` 전체 대신 player/enemy plate 목록만 받아 특수공격 대상 plate를 고르도록 낮췄다. `SpecialAttackUseCase`의 `PlateController` 직접 의존 검색 0건, 옛 생성 호출 검색 0건, Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- PLATE-CONTROLLER-QUERY-API-001: 호출이 없던 `PlateController` public 조회/표시 API를 제거하고, 내부 단계로만 쓰이던 일부 표시/타겟 이름 계산 메서드를 private로 낮췄다. 삭제/축소 대상 검색 0건, Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- ENEMY-NORMAL-REACTION-PLATE-LISTS-001: `EnemyNormalAttackReactionUseCase`가 player plate 목록을 생성 시 한 번만 받고 반복 조회를 제거했다. `PlateController.GetPlayerSummonCount()` public API도 제거했다. `GetPlayerSummonCount` 검색 0건, Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- ENEMY-ATTACK-PLATE-LISTS-001: `EnemyAttackUseCase`가 `PlateController` field를 들고 실행 중 plate 목록을 다시 조회하지 않고, 생성 시 확보한 player/enemy plate 목록을 사용하게 했다. Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- ENEMY-SPECIAL-TURN-PLATE-LISTS-001: `EnemySpecialAttackReactionUseCase`와 `EnemyTurnUseCase`는 `SpecialAttackUseCase` 생성에 필요한 plate 목록만 생성자에서 local로 받아 넘기고, 실행 단계 목록 조회를 남기지 않았다. Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- PLAYER-ATTACK-TARGET-PLATE-LISTS-001: `PlayerAttackUseCase`와 `PlayerTargetSelectionUseCase`의 실행 중 plate 목록 재조회를 생성자 초기화 목록 사용으로 낮췄다. 첫 Unity `recompile_scripts`는 MCP 연결 실패였고, 재시도에서 0 warning으로 통과했다. TestRunner는 실행하지 않았다.
- SUMMON-CONTROLLER-PLATE-LISTS-001: `SummonController`가 player plate 목록을 `Awake()`에서 보관하고 소환 배치/재소환 표시 복구에서 반복 조회하지 않도록 낮췄다. Unity `recompile_scripts` 0 warning. TestRunner는 실행하지 않았다.
- PLATE-LIST-EXPOSURE-REVIEW-003: 남은 plate 목록 조회는 UseCase 생성자 초기화, `SummonController.Awake()` 초기화, `TurnRuntime.ApplyEnemyPlacement()` scene boundary로 분류했다.
- GAME-SAVE-CONTROLLER-STALE-SCENE-001: `Fight Screen_1Stage.unity`에 남아 있던 `GameSaveController` scene object와 해당 fileID/script GUID 참조를 제거했다. 이름/GUID/fileID 검색 0건, Unity `recompile_scripts` 0 warning, 1Stage scene load 성공. TestRunner는 실행하지 않았다.
- BATTLE-CONTROLLER-FINAL-REVIEW-001: Battle 폴더의 남은 Controller 파일은 `PlateController`, `PlayerController`, `BattleResultController` 3개뿐이다. 삭제한 Controller/GameSaveController 이름은 Assets 코드/scene/prefab 검색 0건이며, 남은 3개는 scene/UI boundary라 유지한다.
- COORD-SCAN-002: 멀티 에이전트 3개로 Battle/Turn, Summon/Player, 전체 role-rule 후보를 읽기 전용 점검했고, 다음 code-only 후보를 `TURN-APP-001`로 선정했다. Option 후보는 전투 목표 밖이라 후순위로 보류했다.
- TURN-APP-001: `PlayerTurnState`에서 `IBattleResultFlow`와 `clearTurn` 직접 의존을 제거하고, 클리어 결과 판정은 `PlayerTurnUseCase -> PlayerTurnProgressUseCase`로 이동했다. 첫 Unity `recompile_scripts`는 MCP 연결 실패였고, 재시도에서 0 warning으로 통과했다. TestRunner는 실행하지 않았다.
- SUM-PLAYER-001: `SummonController`의 소환 완료 후 `PlayerController.SetHasSummonedThisTurn()` 우회 호출을 제거하고, 완료 처리는 `PlayerSummonUseCase -> PlayerTurnStateMachine`으로 단일화했다. 호출 없는 `PlayerController`/`PlayerTurnUseCase` wrapper와 `SummonController`의 미사용 public API/serialized `player` 참조도 제거했다. 관련 검색 0건, Unity `recompile_scripts` 0 warning, FightScene 1~7 `load_scene` 성공으로 닫았다. TestRunner는 실행하지 않았다.
- PLATE-CONTROLLER-PUBLIC-API-002: 호출 없는 `PlateController.GetPlayerSummons()`, `GetEnermySummons()`, `GetPlateIndex(Plate)`를 제거했고, `TurnSummonStateUpdater`는 plate 목록에서 현재 Summon을 뽑는 private 단계로 바꿨다. 관련 API 검색 0건, Unity `recompile_scripts` 0 warning으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-RESULT-CONTROLLER-BOUNDARY-002: 호출 없는 `BattleResultController.Clear()`와 `Fail()`을 제거했고, 결과 Controller public 계약은 `IBattleResultFlow.ClearResultTry()`/`FailResultTry()`만 남겼다. UnityEvent 메서드명 검색 0건, Unity `recompile_scripts` 0 warning으로 닫았다. TestRunner는 실행하지 않았다.
- ATTACK-STRUCTURE-001: `AttackStateMachine`에서 일반 공격 실행 소유를 제거했다. `PlayerAttackUseCase`와 `EnemyNormalAttackReactionRule`이 `NormalAttackUseCase`를 직접 사용하고, `AttackStateMachine`은 일반 공격 실행 public API를 노출하지 않는다. `recompile_scripts` 0 warning, `TurnStateMachineStructureTests` 9/9, `PlateSummonDrawStaticRegressionTests` 36/36 통과.
- PLAYER-CONTROLLER-001: `PlayerViewBehaviour`를 `PlayerController`로 rename하고, `PlayerTurnState`가 `PlayerTurnUseCase`를 직접 호출하도록 턴 시작 왕복 흐름을 줄였다. FightScene 1~7 UnityEvent 타입명도 `PlayerController`로 갱신했다. `recompile_scripts` 0 warning, `TurnStateMachineStructureTests` 9/9, `StageSceneConnectionEditModeTests` 15/15, `PlateSummonDrawStaticRegressionTests` 36/36, 2Stage 턴 교환 PlayMode 1/1 통과.
- PLAYER-ATTACK-STRUCTURE-001: `AttackStateMachine`에서 `SpecialAttackUseCase` 실행 소유를 제거하고, Player/Enemy Application 흐름이 특수공격 실행을 직접 맡게 했다. 얇은 `AttackTargetSelectionUseCase`도 제거했다. `recompile_scripts` 0 warning, `TurnStateMachineStructureTests` 9/9, `PlateSummonDrawStaticRegressionTests` 36/36, 2Stage 턴 교환 PlayMode 1/1 통과.
- ENEMY-RULE-STRUCTURE-001: 실행과 로그를 포함하던 `EnemyNormalAttackReactionRule`, `EnemySpecialAttackReactionRule`을 각각 `EnemyNormalAttackReactionUseCase`, `EnemySpecialAttackReactionUseCase`로 rename했다. 판단 전용 이름은 `EnemyAttackDecisionRule`에만 남겼다. `Assets/Refresh`, `recompile_scripts` 0 warning, `PlateSummonDrawStaticRegressionTests` 36/36, `TurnStateMachineStructureTests` 9/9, 2Stage 턴 교환 PlayMode 1/1 통과.
- OPTION-STRUCTURE-001: `GameplaySettingUseCase`를 추가해 `GameplaySettingView`의 직접 Store 생성/저장 흐름을 `View -> UseCase -> Store`로 바꿨다. scene/prefab serialized field는 변경하지 않았다. `Assets/Refresh`, `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 20/20, Option PlayerPrefs 보존 PlayMode 1/1 통과.
- MENU-STRUCTURE-001: `MenuHandler`의 `ShowToMainAlert`, `ShowToQuitAlert`, `ShowSkipAlert` public 진입점은 유지하고, 반복되던 클릭음/handler null 검사/`ShowAlert` 실행 조립을 private `ShowMenuAlert()`와 `PlayMenuClick()`으로 모았다. `MenuView` 삭제와 scene UnityEvent 변경은 제외했다. `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 21/21 통과.
- STORY-01: `StorySkipView`의 취소 분기에서 중복 `SkipAlertHandler.HideAlert()` 호출을 제거하고, base 호출만 하던 `SkipAlertHandler.ShowAlert()` wrapper도 제거했다. `StorySkipView_DelegatesSkipFlowToUseCase` 1/1 통과. `StorySystemStaticRegressionTests` 전체 클래스의 15개 실패는 stale 테스트 경로 기대값으로 분류했다.
- STAGE-02: `StageSelectView`는 이미 `StageSelectUseCase`를 직접 사용하고 `StageController`/`FindObjectOfType` 의존이 없었다. scene UnityEvent가 쓰는 `stageLoader` alias는 유지하고, 미사용 public alias `SendStage(int)`만 제거했다. `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 21/21 통과.
- TEST-STORY-STATIC-001: `StorySystemStaticRegressionTests`의 stale Scenario 경로와 기대 class 이름을 현재 `StoryScenarioBase`, `PrologueScenario`, `EpilogueScenario`, `Stage1Scenario` 계열 파일명으로 갱신했다. 제품 코드는 변경하지 않았다. `recompile_scripts` 0 warning, `StorySystemStaticRegressionTests` 45/45 통과.
- VHO-03: `SettingHandler.OpenSettings()`가 `SettingPanelView.OpenOption()` 토글을 호출하지 않고 직접 setting panel을 열게 했고, HUD의 직접 `SettingPanelView.OpenOption` UnityEvent를 `SettingHandler.CloseSettings`로 돌렸다. `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 22/22, `HudScene_HasOrganizedRootGroups` 1/1 통과.
- BTL-PRED-04: `StageSceneConnectionEditModeTests`에 FightScene 1~7의 `PlayerAttackPrediction` GameObject가 `Cat/Eagle/Fox/Rabbit/Snake/WolfAttackPrediction` 컴포넌트를 함께 가지는지 확인하는 씬 연결 테스트를 추가했다. 제품 코드와 scene/prefab은 변경하지 않았다. `recompile_scripts` 0 warning, 추가 테스트 1/1, `StageSceneConnectionEditModeTests` 16/16 통과.
- MENU-SCENE-001: Prologue/Epilogue/Thank scene의 기존 `MenuView` UnityEvent는 유지하되, `MenuView`를 scene 호환 adapter로 낮추고 실제 메뉴 토글/알림/설정 실행은 런타임 구성된 `MenuHandler`, `BaseAlertHandler`, `SettingHandler` 흐름으로 위임하게 했다. scene/prefab은 변경하지 않았다. `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 23/23, Prologue PlayMode 1/1, UnityEvent target EditMode 1/1 통과.
- MENU-SCENE-002: Prologue/Epilogue/Thank scene의 `MenuCanvas`에 `MenuHandler`, `ToMainAlertHandler`, `ToQuitAlertHandler`, `SettingHandler`를 직렬화하고 버튼 UnityEvent target을 `MenuView`에서 `MenuHandler`로 이관했다. `MenuView` component와 파일은 아직 삭제하지 않았다. `MenuView` UnityEvent target 검색 0건, `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 24/24, `StageSceneConnectionEditModeTests` 16/16, Prologue PlayMode 1/1 통과.
- MENU-SCENE-003: Prologue/Epilogue/Thank scene의 `MenuView` component와 `MenuView.cs/.meta`를 제거했고, `Assembly-CSharp.csproj` stale include도 제거했다. `MenuView`/GUID 검색 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 24/24, `StageSceneConnectionEditModeTests` 16/16, Prologue PlayMode 1/1 통과.
- 테스트 코드 제거: 사용자 요청에 따라 `Assets/Tests` 전체, 테스트 asmdef, 루트 테스트 csproj, `Summoner.sln` 테스트 프로젝트 참조를 제거했다. 이후 후보 검증은 TestRunner 없이 검색과 `recompile_scripts` 중심으로 기록한다.

## 유지/보류 판단

- `SummonController`가 삭제된 `SummonPlaceExecutor` 책임을 private 단계로 가진 현재 방향은 유지한다. 소환 배치 한두 줄을 별도 executor로 다시 분리하지 않는다.
- `Stage*_Controller`의 개별 스테이지 연출은 타임라인처럼 읽혀야 하므로 Effect 클래스로 과분리하지 않는다. 문자열 `Invoke` 같은 명확한 런타임 오류만 작은 diff로 고친다.

## 제외

- 이름만 바꾸는 작업.
- helper/common/util 추가.
- 기능 흐름 하나를 여러 파일로 더 쪼개는 작업.
- 소환수별 Prediction 로직을 공용 base/helper로 숨기는 작업.
- Scene/prefab/ScriptableObject 값을 바로 변경하는 작업.
- 검증 없이 완료 처리하는 작업.
