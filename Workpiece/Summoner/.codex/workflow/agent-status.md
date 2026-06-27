# Agent Status

다음에 실행할 에이전트 하나만 `Ready`로 둔다.

| Agent | 상태 | 현재 초점 | 다음 행동 |
|---|---|---|---|
| CoordinatorAgent | Waiting | PLAYER-CONTROLLER-DELETE-002 선정 완료 | 다음 세션에서 DevAgent로 바로 진행한다 |
| DevAgent | Ready | PLAYER-CONTROLLER-DELETE-002 | `PlayerController` 삭제를 한 slice로 처리한다 |
| VerificationAgent | Waiting | BATTLE-RESULT-CONTROLLER-BOUNDARY-002 검증 완료 | 다음 구조 변경 대기 |
| DocumentationAgent | Waiting | handoff 기록 완료 | 다음 완료 slice 발생 시 `.codex` 문서만 갱신 |
| ReleaseAgent | Waiting | 대기 | 릴리즈 요청 시 사용 |

## 운영 메모

- CoordinatorAgent는 현재 사용자 목표를 우선한다. 전투 Controller 책임 축소가 닫히기 전까지 Option/Stage/Story/Menu 후보를 Ready로 올리지 않는다.
- DevAgent는 코드 변경 전 적용 예정 diff를 먼저 보여주고, 사용자가 승인한 자동 진행 범위 안에서 적용한다.
- BATTLE-RESULT-CONTROLLER-001은 처리했다. `BattleResultProgressUseCase`가 저장 진행도 변경과 결과 후 씬 이동 결정을 맡고, `BattleProgressController`는 위임 경계로 얇아졌다. 보강으로 남아 있던 `GameSaveController.cs/.meta`와 FightScene 1~7/Stage Select의 빈 scene object도 제거했다. 이번 보강 검증은 제품 코드/씬/prefab/resource 검색 0건, 삭제된 script GUID 검색 0건, `Assets/Refresh`, `recompile_scripts` 0 warning으로 닫았고 TestRunner는 사용자 요청대로 실행하지 않았다.
- BATTLE-START-CONTROLLER-001은 처리했다. `BattleStartUseCase`가 적 배치 후 턴 시작 순서를 맡고, `BattleStartController`는 Unity 생명주기, serialized reference 확보, 중복 시작 방지만 맡는다. 검증은 검색, FightScene 1~7 serialized field 유지 확인, `Assets/Refresh`, `recompile_scripts` 0 warning으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-ENEMY-PLACEMENT-001은 처리했다. `BattleEnemyPlacementUseCase`가 stage data 순회, plate 검증, summon 배치, stage multiplier 적용을 맡고, `BattleEnemyPlacementController`는 scene reference 확보와 위임 경계로 얇아졌다. 검증은 검색, `Assets/Refresh`, `recompile_scripts` 0 warning으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-ENEMY-ATTACK-CONTROLLER-001은 처리했다. `EnemyAttackUseCase`가 player attack prediction 생성과 enemy plate 순회 실행을 맡고, `EnermyAttackController`는 scene reference 확인과 UseCase 호출 경계로 얇아졌다. 검증은 검색, `Assets/Refresh`, `recompile_scripts` 0 warning으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-PLATE-CONTROLLER-001은 처리했다. `PlateCompactUseCase`가 enemy plate 압축 정책을 맡고, `PlateController`는 scene plate list와 위임 경계로 얇아졌다. 검증은 검색, `Assets/Refresh`, `recompile_scripts` 0 warning으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-RESULT-PROGRESS-002는 처리했다. `BattleProgressController`와 `BattleStageContext`에서 `PlayerPrefsSaveStore` 직접 생성을 제거했고, 현재 stage fallback과 결과 후 저장/씬 이동 조립은 `BattleResultProgressUseCase`가 맡는다. `Battle/0_Presentation/Result` 저장/씬 이동 검색 0건, `recompile_scripts` 0 warning으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-PROGRESS-CONTROLLER-DELETE-001은 처리했다. `BattleProgressController.cs/.meta`와 FightScene 1~7의 해당 component를 제거했고, 결과 alert callback 후 저장/씬 이동 처리는 `BattleResultController -> BattleResultProgressUseCase`로 직접 이어진다. 검색 결과 `BattleProgressController` 이름/GUID/fileID 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, FightScene 1~7 Unity `load_scene` 성공으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-START-CONTROLLER-DELETE-001은 처리했다. `BattleStartController.cs/.meta`와 단순 wrapper였던 `BattleStartUseCase.cs/.meta`를 제거했고, 적 배치 후 턴 시작 흐름은 `TurnRuntime`의 시작 흐름으로 흡수했다. 검색 결과 `BattleStartController` 이름/GUID/fileID 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, FightScene 1~7 Unity `load_scene` 성공으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-ENEMY-PLACEMENT-CONTROLLER-DELETE-001은 처리했다. `BattleEnemyPlacementController.cs/.meta`를 제거했고, `StageEnemyPlacementData` 참조와 적 배치 UseCase 호출은 `TurnRuntime` 시작 흐름으로 흡수했다. 검색 결과 `BattleEnemyPlacementController` 이름/GUID/fileID 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, FightScene 1~7 Unity `load_scene` 성공으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-ENEMY-ATTACK-CONTROLLER-DELETE-001은 처리했다. `EnermyAttackController.cs/.meta`를 제거했고, `EnemyAttackUseCase` 실행은 기존 `Enermy` runtime component가 직접 맡는다. 검색 결과 `EnermyAttackController` 이름/GUID/호출 메서드 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, FightScene 1~7 Unity `load_scene` 성공으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-REMAINING-CONTROLLER-REVIEW-001은 처리했다. 남은 Battle Controller는 `PlateController`, `PlayerController`, `BattleResultController` 3개이며, 각각 보드 registry/view highlight, Unity button event/PlayerView wiring, result alert callback/`IBattleResultFlow` 경계로 확인되어 삭제 대상에서 제외한다.
- ATTACK-STATE-MACHINE-BOUNDARY-001은 처리했다. `AttackStateMachine`에서 `PlateController` 생성자 의존과 `ResetAllPlateHighlight()` 호출을 제거했고, highlight reset은 `PlayerAttackUseCase`, Enemy reaction/usecase, `PlayerTurnState` 호출자 쪽으로 옮겼다. `AttackStateMachine`의 Presentation 의존 검색 0건, Unity `recompile_scripts` 0 warning으로 닫았다.
- PLAYER-CONTROLLER-PUBLIC-API-001은 처리했다. scene UnityEvent나 코드 호출이 없는 `PlayerController.HasSummonedThisTurn()`, `UpdateManaUI()`, `AddMana()` public wrapper를 제거했고, Unity `recompile_scripts` 0 warning으로 닫았다.
- PLATE-CONTROLLER-APPLICATION-DEPENDENCY-001 1차는 처리했다. `EnemyPredictionPlateState`가 `PlateController` 전체 대신 player/enemy plate 목록만 받아 snapshot을 만들도록 낮췄고, Unity `recompile_scripts` 0 warning으로 닫았다.
- PLATE-PREDICTION-BUILDER-DEPENDENCY-001은 처리했다. `PlayerAttackPredictionListBuilder`가 `PlateController` 전체 대신 player/enemy plate 목록을 받아 prediction snapshot을 만들도록 낮췄고, Unity `recompile_scripts` 0 warning으로 닫았다.
- SPECIAL-ATTACK-PLATE-LISTS-001은 처리했다. `SpecialAttackUseCase`가 `PlateController` 전체 대신 player/enemy plate 목록만 받아 특수공격 대상 plate를 고르도록 낮췄고, Unity `recompile_scripts` 0 warning으로 닫았다.
- PLATE-CONTROLLER-QUERY-API-001은 처리했다. 호출이 없던 `PlateController` public API를 제거하고 내부 단계로만 쓰이던 일부 메서드를 private로 낮췄다. 삭제/축소 대상 검색 0건, Unity `recompile_scripts` 0 warning으로 닫았다.
- PLATE-LIST-EXPOSURE-SCAN-001은 처리했다. `GetPlayerPlates()`/`GetEnermyPlates()` 호출부를 기능 흐름별로 나눴고, 가장 작은 후속 축소 대상으로 `EnemyNormalAttackReactionUseCase`를 선정했다.
- ENEMY-NORMAL-REACTION-PLATE-LISTS-001은 처리했다. `EnemyNormalAttackReactionUseCase`가 player plate 목록을 생성 시 한 번만 받고 반복 조회를 제거했으며, `PlateController.GetPlayerSummonCount()` public API도 제거했다. Unity `recompile_scripts` 0 warning으로 닫았다.
- ENEMY-ATTACK-PLATE-LISTS-001은 처리했다. `EnemyAttackUseCase`가 `PlateController` field를 들고 실행 중 plate 목록을 다시 조회하지 않고, 생성 시 확보한 player/enemy plate 목록을 사용하게 했다. Unity `recompile_scripts` 0 warning으로 닫았다.
- ENEMY-SPECIAL-TURN-PLATE-LISTS-001은 처리했다. `EnemySpecialAttackReactionUseCase`와 `EnemyTurnUseCase`의 `SpecialAttackUseCase` 생성 목록 조회를 생성자 local 단계로 제한했고, Unity `recompile_scripts` 0 warning으로 닫았다.
- PLAYER-ATTACK-TARGET-PLATE-LISTS-001은 처리했다. `PlayerAttackUseCase`와 `PlayerTargetSelectionUseCase`의 실행 중 plate 목록 재조회를 생성자 초기화 목록 사용으로 낮췄다. 첫 Unity MCP 재컴파일은 연결 실패였고, 재시도 0 warning으로 닫았다.
- SUMMON-CONTROLLER-PLATE-LISTS-001은 처리했다. `SummonController`가 player plate 목록을 `Awake()`에서 보관하고 소환 배치/재소환 표시 복구에서 반복 조회하지 않도록 낮췄다. Unity `recompile_scripts` 0 warning으로 닫았다.
- PLATE-LIST-EXPOSURE-REVIEW-003은 처리했다. 남은 plate 목록 조회는 UseCase 생성자 초기화, `SummonController.Awake()` 초기화, `TurnRuntime.ApplyEnemyPlacement()` scene boundary로 분류했다.
- GAME-SAVE-CONTROLLER-STALE-SCENE-001은 처리했다. `Fight Screen_1Stage.unity`에 남아 있던 `GameSaveController` scene object와 해당 fileID/script GUID 참조를 제거했다. 이름/GUID/fileID 검색 0건, Unity `recompile_scripts` 0 warning, 1Stage scene load 성공으로 닫았다.
- BATTLE-CONTROLLER-FINAL-REVIEW-001은 처리했다. Battle 폴더의 남은 Controller 파일은 `PlateController`, `PlayerController`, `BattleResultController` 3개뿐이고, 삭제한 Controller/GameSaveController 이름은 Assets 코드/scene/prefab 검색 0건이다. 남은 3개는 scene/UI boundary라 유지한다.
- COORD-SCAN-002는 처리했다. 멀티 에이전트 3개로 Battle/Turn, Summon/Player, 전체 role-rule 후보를 읽기 전용 점검했고, 다음 code-only 후보를 `TURN-APP-001`로 선정했다. Option 후보는 전투 목표 밖이라 후순위로 보류했다.
- TURN-APP-001은 처리했다. `PlayerTurnState`에서 `IBattleResultFlow`와 `clearTurn` 직접 의존을 제거하고, 클리어 결과 판정은 `PlayerTurnUseCase -> PlayerTurnProgressUseCase`로 이동했다. Unity `recompile_scripts` 재시도 0 warning으로 닫았다.
- SUM-PLAYER-001은 처리했다. `SummonController`의 소환 완료 후 턴 상태 변경 우회는 `PlayerSummonUseCase -> PlayerTurnStateMachine`으로 단일화했고, 호출 없는 `PlayerController`/`PlayerTurnUseCase` wrapper와 `SummonController` 미사용 public API/serialized `player` 참조도 제거했다. 검증은 관련 검색 0건, Unity `recompile_scripts` 0 warning, FightScene 1~7 `load_scene` 성공으로 닫았다. TestRunner는 실행하지 않았다.
- PLATE-CONTROLLER-PUBLIC-API-002는 처리했다. 호출 없는 `PlateController.GetPlayerSummons()`, `GetEnermySummons()`, `GetPlateIndex(Plate)`를 제거했고, `TurnSummonStateUpdater`는 plate 목록에서 현재 Summon을 뽑는 private 단계로 바꿨다. 관련 API 검색 0건, Unity `recompile_scripts` 0 warning으로 닫았다. TestRunner는 실행하지 않았다.
- BATTLE-RESULT-CONTROLLER-BOUNDARY-002는 처리했다. 호출 없는 `BattleResultController.Clear()`와 `Fail()`을 제거했고, 결과 Controller public 계약은 `IBattleResultFlow.ClearResultTry()`/`FailResultTry()`만 남겼다. UnityEvent 메서드명 검색 0건, Unity `recompile_scripts` 0 warning으로 닫았다. TestRunner는 실행하지 않았다.
- 현재 Ready 후보는 `PLAYER-CONTROLLER-DELETE-002` 하나다. 다음 세션에서는 workflow 문서 목표 기준으로 `PlayerController` 삭제를 한 번에 처리한다. TestRunner는 실행하지 않고 검색, Unity `recompile_scripts`, FightScene 1~7 `load_scene`로 확인한다.
- 이번 방향은 기능 흐름을 먼저 보고, 계층은 feature 내부 책임이 실제로 있을 때만 사용한다.
- UI는 `View / Controller`, Application 흐름은 `UseCase`로 통일한다.
- `StateMachine`은 상태 전이만 맡고 UseCase를 생성하거나 실행하지 않는다.
- `Rule`은 판단만 맡고 실행은 UseCase가 맡는다.
- `Action`, `Actions`, `Runner`, Application성 `Flow`는 새로 만들지 않는다.
- 분석 단계에서는 하위 에이전트를 feature별로 병렬 사용하고, 제품 코드 수정 전에는 DevAgent Change Proposal과 적용 예정 diff를 제시한다.
- ATTACK-STRUCTURE-001은 처리했다. `AttackStateMachine`에서 일반 공격 실행 소유를 제거하고, `PlayerAttackUseCase`와 `EnemyNormalAttackReactionRule`이 `NormalAttackUseCase`를 직접 사용한다. 검증은 `recompile_scripts` 0 warning, `TurnStateMachineStructureTests` 9/9, `PlateSummonDrawStaticRegressionTests` 36/36 통과로 닫았다.
- PLAYER-CONTROLLER-001은 처리했다. `PlayerViewBehaviour`를 `PlayerController`로 rename하고, `PlayerTurnState`가 `PlayerTurnUseCase`를 직접 호출하도록 왕복 흐름을 줄였다. FightScene 1~7 UnityEvent 타입명도 `PlayerController`로 갱신했다. 검증은 `recompile_scripts` 0 warning, `TurnStateMachineStructureTests` 9/9, `StageSceneConnectionEditModeTests` 15/15, `PlateSummonDrawStaticRegressionTests` 36/36, 2Stage 턴 교환 PlayMode 1/1 통과로 닫았다.
- PLAYER-ATTACK-STRUCTURE-001은 처리했다. `AttackStateMachine`에서 `SpecialAttackUseCase` 실행 소유를 제거하고, `PlayerAttackUseCase`와 Enemy Application 흐름이 직접 특수공격을 실행한다. `AttackTargetSelectionUseCase` 얇은 wrapper도 제거했다. 검증은 `recompile_scripts` 0 warning, `TurnStateMachineStructureTests` 9/9, `PlateSummonDrawStaticRegressionTests` 36/36, 2Stage 턴 교환 PlayMode 1/1 통과로 닫았다.
- ENEMY-RULE-STRUCTURE-001은 처리했다. 실행과 로그를 포함하던 Enemy reaction 클래스 2개를 `UseCase` 이름으로 바꾸고, 판단 전용 이름은 `EnemyAttackDecisionRule`에만 남겼다. 검증은 `Assets/Refresh`, `recompile_scripts` 0 warning, 관련 EditMode 2개, 2Stage 턴 교환 PlayMode 통과로 닫았다.
- OPTION-STRUCTURE-001은 처리했다. `GameplaySettingUseCase`를 추가해 `GameplaySettingView`가 `GameplaySettingStore`를 직접 생성하지 않고 `View -> UseCase -> Store` 흐름으로 읽히게 했다. scene/prefab serialized field는 변경하지 않았다. 검증은 `Assets/Refresh`, `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 20/20, Option PlayerPrefs 보존 PlayMode 1/1 통과로 닫았다.
- MENU-STRUCTURE-001은 처리했다. `MenuHandler`의 public 알림 진입점은 유지하고, 반복되던 클릭음/handler null 검사/`ShowAlert` 실행 조립을 private `ShowMenuAlert()`와 `PlayMenuClick()`으로 모았다. `MenuView` 삭제나 scene UnityEvent 변경은 제외했다. 검증은 `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 21/21 통과로 닫았다.
- STORY-01은 처리했다. `StorySkipView`의 취소 분기에서 중복 alert hide 호출을 제거하고, `SkipAlertHandler`의 base 호출 wrapper를 제거했다. 직접 검증은 `StorySkipView_DelegatesSkipFlowToUseCase` 1/1 통과로 닫았다. `StorySystemStaticRegressionTests` 전체 클래스 실패 15개는 현재 Story 파일명과 맞지 않는 stale 정적 테스트 경로 기대값으로 분류했다.
- STAGE-02는 처리했다. `StageSelectView`는 이미 `StageSelectUseCase`를 직접 사용하고 `StageController`/`FindObjectOfType` 의존이 없었다. scene UnityEvent가 쓰는 `stageLoader` alias는 유지하고, 미사용 public alias `SendStage(int)`만 제거했다. 검증은 검색, `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 21/21 통과로 닫았다.
- TEST-STORY-STATIC-001은 처리했다. `StorySystemStaticRegressionTests`의 stale Scenario 경로와 기대 class 이름을 현재 `StoryScenarioBase`, `PrologueScenario`, `EpilogueScenario`, `Stage1Scenario` 계열 파일명으로 갱신했다. 제품 코드는 변경하지 않았다. 검증은 `recompile_scripts` 0 warning, `StorySystemStaticRegressionTests` 45/45 통과로 닫았다.
- VHO-03은 처리했다. `SettingHandler.OpenSettings()`가 `SettingPanelView.OpenOption()` 토글을 호출하지 않고 직접 setting panel을 열게 했고, HUD의 직접 `SettingPanelView.OpenOption` UnityEvent를 `SettingHandler.CloseSettings`로 돌렸다. 검증은 검색, `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 22/22, `HudScene_HasOrganizedRootGroups` 1/1 통과로 닫았다.
- BTL-PRED-04는 처리했다. `StageSceneConnectionEditModeTests`에 FightScene 1~7의 `PlayerAttackPrediction` GameObject가 `Cat/Eagle/Fox/Rabbit/Snake/WolfAttackPrediction` 컴포넌트를 함께 가지는지 확인하는 씬 연결 테스트를 추가했다. 제품 코드와 scene/prefab은 변경하지 않았다. 검증은 `recompile_scripts` 0 warning, 추가 테스트 1/1, `StageSceneConnectionEditModeTests` 16/16 통과로 닫았다.
- MENU-SCENE-001은 처리했다. Prologue/Epilogue/Thank scene의 기존 `MenuView` UnityEvent는 유지하되, `MenuView`를 scene 호환 adapter로 낮추고 실제 메뉴 토글/알림/설정 실행은 런타임 구성된 `MenuHandler`, `BaseAlertHandler`, `SettingHandler` 흐름으로 위임하게 했다. scene/prefab은 변경하지 않았다. 검증은 `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 23/23, Prologue PlayMode 1/1, UnityEvent target EditMode 1/1 통과로 닫았다.
- MENU-SCENE-002는 처리했다. Prologue/Epilogue/Thank scene의 `MenuCanvas`에 `MenuHandler`, `ToMainAlertHandler`, `ToQuitAlertHandler`, `SettingHandler`를 직렬화하고 버튼 UnityEvent target을 `MenuView`에서 `MenuHandler`로 이관했다. `MenuView` component와 파일은 아직 삭제하지 않았다. 검증은 `MenuView` UnityEvent target 검색 0건, `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 24/24, scene 연결 EditMode/Prologue PlayMode 통과로 닫았다.
- MENU-SCENE-003은 처리했다. Prologue/Epilogue/Thank scene의 `MenuView` component와 `MenuView.cs/.meta`를 제거했고, `Assembly-CSharp.csproj` stale include도 제거했다. 검증은 `MenuView`/GUID 검색 0건, `Assets/Refresh`, `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests` 24/24, `StageSceneConnectionEditModeTests` 16/16, Prologue PlayMode 1/1 통과로 닫았다.
- 현재 Ready 작업은 `COORD-SCAN-002`다. 첫 액션은 `View`, `Handler`, `UseCase`, `Rule`, `StateMachine` 이름과 scene UnityEvent 직접 연결을 검색해 다음 변경 영향이 작은 후보를 고르는 것이다.
- BATTLE-ATTACK-03은 처리했다. Player 공격/타겟 선택 Application은 `BattleController` 구체 타입 대신 `IBattleAttackExecution`/`IBattleAttackTargetSelection` 계약을 사용한다. 검증은 `recompile_scripts` 0 warning과 관련 정적 회귀 테스트 1/1 통과로 닫았다.
- TURN-ENEMY-01은 코드 적용 후 검증 보류 상태다. 정적 검색과 수정 범위 `git diff --check`는 통과했지만 Unity MCP `run_tests`, `recompile_scripts`, `Assets/Refresh`가 timeout으로 중단됐다.
- 첫 제품 코드 slice는 `SAVE-APP-001`이다. Save는 scene/prefab/ScriptableObject asset 값을 건드리지 않고 기존 `GameSaveController` 연결을 유지할 수 있어 가장 안전하다.
- SAVE-APP-001은 처리했다. Save는 `Application/GameSaveUseCase`, `Domain/GameSaveData`, `Infrastructure/PlayerPrefsSaveStore` 구조로 정리했고 기존 `GameSaveController` public API는 유지했다.
- SAVE-APP-001 검증은 `recompile_scripts` 0 warning, `GameSystemStaticRegressionTests`, `GameSaveProgressReadTests`, `GameSaveContinueTests` 통과로 닫았다.
- TURN-APP-001은 처리했다. `TurnPhaseActions`를 `ChangeTurnUseCase`로 이관했고 `TurnController`의 serialized field와 public/internal entrypoint는 유지했다.
- TURN-APP-001 검증은 `recompile_scripts` 0 warning, 관련 EditMode 3개 클래스, PlayMode 턴 교환 2개 테스트 통과로 닫았다.
- TEST-STATIC-001은 처리했다. `PlateSummonDrawStaticRegressionTests`는 현재 코드 책임 경계와 clone 기반 예측 스냅샷을 검증하며 36/36 통과했다.
- ENEMY-ACTION-001은 처리했다. 적 특수공격 실행 실패가 성공처럼 처리되지 않도록 bool 반환 계약을 반영했고 관련 정적/PlayMode 게이트가 통과했다.
- ENEMY-ACTION-001 보강도 처리했다. `SpecialAttackExecutor.Execute()`의 알 수 없는 전략 fallback이 `false`를 반환하도록 맞췄다.
- TURN-APP-002는 처리했다. 실패 판정 이동은 동작 변경 위험 때문에 제외하고, 클리어 결과 게이트 이름만 `TryStopTurnForClearResult()`로 명확히 했다.
- ENEMY-ACTION-002 일부는 처리했다. bool 반환 특수공격 경로를 `TryExecuteSpecialAttack`으로 맞추고 `SpecialAttackUseCase`로 rename했다.
- PLAYER-EXEC-001은 처리했다. `PlayerActionExecutor` 단순 wrapper를 제거하고 소환 흐름 내부 private 단계로 흡수했다.
- PLAYER-01은 처리했다. `PlayerSummonActions`를 `PlayerSummonUseCase`로 rename했고 소환 시작 Application 흐름 이름을 맞췄다.
- PLAYER-02는 처리했다. `PlayerAttackActions`를 `PlayerAttackUseCase`로 rename했고 공격 시작 Application 흐름 이름을 맞췄다.
- ENEMY-TURN-001은 처리했다. `EnemyTurnActionRunner`를 `EnemyTurnUseCase`로 rename하고 진입 메서드를 `ExecuteEnemyTurn()`으로 맞췄다.
- PLATE-APP-001은 처리했다. `PlateInputActions`에서 클릭 의도 흐름을 `PlateClickUseCase`로 추출했고, hover/input 전달 책임은 기존 파일에 남겼다.
- BATTLE-FOLDER-001은 처리했다. Turn의 Presentation/Domain 계층을 만들고, Battle UseCase 파일을 `Assets/Script/5_Battle/Application/{Turn,Attack,Player,Enemy,Plate}` 아래로 모았다.
- FEATURE-FOLDER-001은 처리했다. `Turn`과 `Save`를 최상위 feature 폴더로 옮기고 내부를 `Presentation/Application/Domain/Infrastructure` 기준으로 맞췄다.
- FEATURE-FOLDER-002는 처리했다. `Summon` 관련 Domain/Application/Presentation 파일과 ScriptableObject 데이터를 최상위 `Assets/Script/Summon` feature 폴더로 옮겼다.
- FEATURE-FOLDER-003은 처리했다. Battle의 AttackRule과 Status 순수 도메인 코드를 `Assets/Script/5_Battle/Domain/Attack`, `Assets/Script/5_Battle/Domain/Status`로 옮겼다.
- FEATURE-FOLDER-004~005는 처리했다. `Assets/Script` 최상위 번호 폴더를 제거하고 `Battle`, `Core`, `Menu`, `Option`, `Save`, `Stage`, `StartScreen`, `Story`, `Summon`, `Turn` feature 폴더로 정리했다. 최소 검증은 오래된 경로 검색 0건, 번호 폴더 검색 0건, `recompile_scripts` 0 warning으로 닫았다.
- NAMING-APP-001~003은 처리했다. Core/Story `*Flow`는 `*UseCase`로, Stage 전환은 `StageTransitionController` + `StageTransitionUseCase`로, Battle Player/Plate의 `Actions` 이름은 `UseCase` 또는 하위 `Service`로 정리했다. `Actions/Runner/Flow` 파일명은 0건이고 `recompile_scripts` 0 warning이다.
- FOLDER-INDEX-001은 처리했다. feature 폴더 내부 계층은 `0_Presentation`, `1_Application`, `2_Domain`, `3_Infrastructure`로 정렬했고, Domain 내부의 `Attack`, `Status`, `Data` 같은 세부 폴더에는 인덱스를 붙이지 않았다.
- NAMING-APP-004는 처리했다. 모든 `Executor` 파일/클래스/문자열을 제거했고, 단일 사용 `PlateTargetSelectionService`는 UseCase 내부 단계로 흡수했다. `EnemyPredictionReactionService`는 UseCase, enemy reaction service 2개는 Rule로 rename했다.
- 현재 남은 `Service`는 `PlateQueryService`, `SummonDrawService` 2개다. 전자는 Plate 조회 공통 기능, 후자는 소환 후보 생성 규칙이라 유지 기준에 맞는다.
- Turn 목표는 `TurnController + ChangeTurnUseCase`이고, 상태 분기가 커진 경우에만 State Machine을 제안한다.
- Summon은 대분리하지 않고, enum 의존과 중복 fallback 초기화부터 줄인다.
- COORD-165는 Story Screen 1/2/3/5/7의 `StorySkipView` 버튼 이벤트 메서드명을 현재 public 메서드 `SkipAlert`에 맞추는 연결 수습이다.
- COORD-165는 적용 완료했다. `StorySystemStaticRegressionTests`는 26/26 통과했다.
- COORD-SCAN은 완료했다. Ready 후보는 BTL-SCENE-03 하나만 둔다.
- BTL-SCENE-03은 처리했다. `StageSceneConnectionEditModeTests` 7/7, 전체 EditMode 146/146, 전체 PlayMode 26/26 통과.
- SCENE-NULL-01은 처리했다. `Grass Spirit.prefab`, `HighDevil.prefab`의 기존 `ShieldImage` GameObject를 `Summon.shieldImage`에 연결했고, `StageSceneConnectionEditModeTests` 9/9, 전체 EditMode 148/148, 전체 PlayMode 26/26 통과.
- BTL-RUNTIME-01은 처리했다. `BattleProgressController`의 runtime `AddComponent<BattleStageContext>()` fallback을 제거했고, FightScene 1~7 battle runtime controller 필수 연결 계약을 추가했다. 전체 EditMode 149/149, 전체 PlayMode 26/26 통과.
- RUNTIME-WIRING-02는 처리했다. 시작/이어하기, HUD, Stage Select, Fight, Story/Thank 화면의 runtime wiring 오류와 콘솔 warning을 수습했고 전체 EditMode 155/155, 전체 PlayMode 28/28, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-03은 처리했다. Start Screen 설정 버튼, Story ESC, Fight 결과창 시작 비활성화, 일반 소환 3옵션 fallback과 `Summon.GetImage()` prefab 지연 확보를 수습했고 전체 EditMode 155/155, 전체 PlayMode 32/32, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-05는 처리했다. 1Stage 전투 UI 배치를 2Stage 기준으로 정리했고, 2~7Stage 소환 선택 UI zero-scale과 PlayerPlateActions 시작 활성 차이를 수습했다. `StageSceneConnectionEditModeTests` 15/15, 2Stage PlayMode 소환 UI/턴 교환 테스트는 통과했다.
- RUNTIME-HOTFIX-06은 처리했다. `SummonStatusView`가 상태색/피격색을 덮기 전 원래 Image 색상을 캐싱하고 상태 해제 시 원래 색으로 복구한다. `SummonStatusViewTests` 12/12 통과, Unity Console error/warning 0건을 확인했다.
- RUNTIME-QA-07은 처리했다. `StageRuntimeFlowPlayModeTests` 17/17, 전체 PlayMode 34/34, 전체 EditMode 158/158 통과했고 Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-09는 처리했다. FightScene 1~7의 소환 선택 옵션 판넬을 `380x550`으로 줄였고, 적 배치 직후 현재 전투 배수를 적 HP/공격력에 적용한다. 전체 PlayMode 35/35, 전체 EditMode 158/158, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-10은 처리했다. FightScene 1~7의 소환/재소환 3옵션 `RedrawPanel`을 3열 고정 GridLayout으로 맞추고 1Stage `TurnTextUI` 프레임 배치를 수습했다. Unity 실행은 `LicenseClient` IPC timeout으로 결과 XML 없이 중단됐고, 정적 scene YAML 확인과 `git diff --check`만 완료했다.
- RUNTIME-HOTFIX-11은 처리했다. FightScene 1~7의 1개짜리 `DrawPanel/DrawOption_1` 경로와 `SummonController`의 `drawPanel/drawOptionPanels` 직렬화 경로를 제거하고, 일반 소환과 재소환 모두 공용 `RedrawPanel` 3옵션으로 통일했다. Unity 실행은 `LicenseClient` IPC timeout으로 결과 XML 없이 중단됐고, 정적 scene YAML 확인과 `git diff --check`만 완료했다.
- RUNTIME-HOTFIX-12는 처리했다. FightScene 1~7의 Unity YAML 헤더 누락을 복구했고, Unity MCP `load_scene`으로 1~7Stage 모두 정상 로드되는 것을 확인했다.
- RUNTIME-TRIAGE-08은 처리했다. `StageSceneConnectionEditModeTests` 15/15, `PlateSummonDrawStaticRegressionTests` 36/36, `StageRuntimeFlowPlayModeTests` 19/19 통과 및 Unity Console error/warning 0건을 확인했다.
- 현재 반복 루프는 RUNTIME-QA-13이다. 자동 테스트로 닫힌 항목을 실제 플레이 흐름에서 다시 확인하고, 재현되는 오류가 있으면 코드/씬/도구 문제로 분류한다.
- 테스트 실행은 자동 진행한다. 테스트 결과 확인 뒤 10분 동안 사용자 응답이 없으면 코드/씬/에셋 수정이 필요 없는 다음 테스트 또는 문서/후보 정리 작업으로 이어간다.
- `dotnet build .\Assembly-CSharp.csproj --no-restore`는 기존 `NETSDK1004` project.assets.json 누락으로 중단됐다.
- AttackRule 미세정리는 같은 기능 반복 제한 규칙에 따라 다음 작업으로 잡지 않는다.
- 문서 상태 기록은 사용자 승인 없이 최소 범위로 바로 갱신한다.
- 2026-06-22 handoff: Turn은 StateMachine 구조로 전환했고 Attack도 `AttackStateMachine`을 도입했다. 다음 세션은 `BATTLE-ATTACK-03`부터 이어간다.
- PLAYER-TURN-STATE-001은 처리했다. `PlayerTurnActionResult`로 플레이어 행동 Try 결과를 명시하고, `PlayerTurnUseCase`가 타겟 선택 대기만 상태 유지하며 실패/완료는 Idle로 되돌린다. 검증은 `recompile_scripts` 0 warning, `TurnStateMachineStructureTests` 7/7, `PlateSummonDrawStaticRegressionTests` 36/36, Unity Console error 0건으로 닫았다.
- 사용자 요청으로 `Assets/Tests` 전체와 테스트 프로젝트 참조를 제거했다. 앞으로 이 작업 흐름에서는 TestRunner를 호출하지 않고, 검색과 Unity `recompile_scripts`를 기본 확인 게이트로 둔다.
