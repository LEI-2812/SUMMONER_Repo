# Issue Board

현재 판단이 필요한 항목만 둔다.
오래된 Done 목록은 여기에 누적하지 않는다.
상세 후보 큐는 `.codex/workflow/refactor-candidates.md`를 본다.

| ID | 상태 | 도메인 | 기능 슬라이스 | 완료조건 | 다음 행동 | 호출 대상 |
|---|---|---|---|---|---|---|
| TURN-ENEMY-01 | Verification Blocked | Battle/Turn | 적 공격 시작 실패와 턴 종료 결정 계약 명확화 | 적 공격 시작 실패 여부가 `IEnemyAttackFlow` 계약으로 드러나고, `EnemyTurnProgressUseCase`가 턴 종료 결정을 명시적으로 처리한다 | MCP timeout 해소 후 `recompile_scripts`와 관련 정적 회귀 테스트를 재실행한다 | VerificationAgent |
| BATTLE-ATTACK-04 | Candidate | Battle/Attack | Enemy 공격 UseCase의 BattleController 의존 축소 | Enemy reaction/turn UseCase가 필요한 공격 실행 계약만 보도록 축소한다 | BATTLE-ATTACK-03 이후 enemy 호출부에 같은 계약을 적용할지 판단 | CoordinatorAgent |
| CTRL-001 | Candidate | Summon | plain C# PlateSelectionController 이름/책임 정리 | 씬에 붙지 않은 `PlateSelectionController`가 Controller 이름을 쓰지 않고 redraw plate selection 책임을 드러낸다 | TURN-ENEMY-01 이후 controller 최소화 첫 slice로 검토한다 | CoordinatorAgent |

## 보류/제외

- 전체 후보 목록은 `refactor-candidates.md`에 둔다.
- 구조 기준은 `Feature -> Presentation / Application / Domain / Infrastructure`로 갱신했다.
- 기본 흐름은 `Controller -> UseCase -> Entity/Domain Rule -> UseCase Result -> Controller -> View`다.
- Turn 목표는 `TurnController + ChangeTurnUseCase`이며, 상태 분기가 커질 때만 State Machine을 제안한다.
- `Action`, `Actions`, `Runner`, Application성 `Flow`는 새로 만들지 않는다.
- 첫 제품 코드 slice는 Save로 둔다. Turn/Summon/Menu/Story는 scene, prefab, ScriptableObject, runtime 연결 위험 때문에 후속 slice로 분리한다.
- BATTLE-ATTACK-03: `IBattleAttackExecution`, `IBattleAttackTargetSelection`을 도입해 Player 공격/타겟 선택 Application의 `BattleController` 구체 타입 의존을 줄였다. 씬 연결은 유지했고 `recompile_scripts` 0 warning, 관련 정적 회귀 테스트 1/1 통과.
- SAVE-APP-001: `GameSaveController` public API와 scene 연결을 유지하고 저장 조작 흐름을 `GameSaveUseCase`로 분리했다. `GameSystemStaticRegressionTests`, `GameSaveProgressReadTests`, `GameSaveContinueTests` 통과.
- TURN-APP-001: `TurnPhaseActions`를 `ChangeTurnUseCase`로 이관했다. StateMachine은 아직 도입하지 않았고, `TurnController` public/internal entrypoint와 serialized field는 유지했다.
- `PlateSummonDrawStaticRegressionTests` 전체 클래스 실패 6건은 TURN-APP-001 변경 원인이 아니라 기존 Player/Enemy/Prediction 문자열 기대값 노후화로 분류했다.
- TEST-STATIC-001: stale 문자열 기대값을 현재 코드 기준으로 갱신했고, `PlateSummonDrawStaticRegressionTests` 36/36 통과.
- ENEMY-ACTION-001: `EnemyAttackExecutor.ExecuteSpecialAttack()`의 bool 결과를 적 반응/턴 호출부가 반영하도록 수정했고, 관련 정적/PlayMode 검증 통과.
- ENEMY-ACTION-001 보강: `SpecialAttackExecutor.Execute()`의 알 수 없는 전략 fallback도 `false`로 맞췄다.
- TURN-APP-002: `TryClearBattle()`을 `TryStopTurnForClearResult()`로 바꾸고, 실패 판정 이동은 제품 동작 변경 가능성이 있어 제외했다.
- ENEMY-ACTION-002 일부: `SpecialAttackUseCase`와 `TryExecuteSpecialAttack` 이름 정리를 완료했다.
- PLAYER-EXEC-001: `PlayerActionExecutor` 제거 완료. `PlayerSummonActions` 내부 private 단계로 흡수했다.
- PLAYER-01: `PlayerSummonActions`를 `PlayerSummonUseCase`로 rename했다.
- PLAYER-02: `PlayerAttackActions`를 `PlayerAttackUseCase`로 rename했다.
- ENEMY-TURN-001: `EnemyTurnActionRunner.RunEnemyAction()`을 `EnemyTurnUseCase.ExecuteEnemyTurn()`으로 rename했다.
- PLATE-APP-001: `PlateInputActions`의 클릭 의도 흐름을 `PlateClickUseCase`로 추출했다.
- COORD-147: fallback-only 소환수 클래스 삭제/통합은 16개 prefab `m_Script` GUID와 Fight Screen 소환 리스트 연결 때문에 별도 prefab migration 승인 전까지 보류한다.
- COORD-152: Prediction 매칭에서 옛 소환 타입 계약을 제거했다.
- COORD-161: `Summon` 런타임 타입 필드/API와 fallback 타입 인자는 제거했다.
- COORD-156: `PlayerAttackCondition` 과분리는 제거했다. 공격 조건은 `PlayerAttackActions` private guard로 둔다.
- COORD-163: `SummonType` enum, `SummonData.summonType`, asset YAML `summonType:` 값은 제거했다.
- COORD-164: `TurnSummonStateUpdater`의 턴 시작 상태효과 갱신 단계를 분리하고 일반 공격 쿨타임 갱신 null 방어를 추가했다.
- COORD-154: C# 외부 직접 `isAttacking` 호출부는 없지만 FightScene 1~7에 serialized `isAttacking:` 값이 남아 field 제거는 별도 scene 영향 승인 전까지 보류한다.
- COORD-155: 일반 소환의 빈 플레이어 Plate 조회를 `PlateController.GetFirstEmptyPlayerPlateIndex()`로 좁혔다. `GetPlayerPlates()` 전체 제거는 공격/예측 흐름 영향이 있어 이번 슬라이스에서 제외했다.
- COORD-157: `StoryImageView`가 `InteractionController`에서 현재 대사 인덱스를 직접 조회하지 않고 `InteractionController.OnPointerClick()`에서 인덱스를 전달받도록 좁혔다.
- COORD-159: `StorySkipView`가 `SettingPanelView`를 직접 조회하지 않고 `GameplaySettingStore.LoadStorySkipEnabled()`로 스킵 버튼 활성 여부를 판단하도록 좁혔다.
- COORD-158: 씬 연결이 없는 상태를 고려해 `GameObject.Find("MenuCanvas")`는 유지하고, 옵션 패널 계층 문자열 탐색만 `FindSettingPanelTransform()`으로 모았다.
- SCENE-WIRING-001: `Start Screen`의 중복 `StartScreenView` 컴포넌트를 제거하고, FightScene 1~7의 `TurnController.plateController`를 `PlateController` fileID `893681172`로 명시 연결했다.
- 연결 테스트 후속 처리: `CurrentTurnText`와 `ClearTurnText` 기대 경로를 실제 `UI_20_TurnStatus` 하위 구조에 맞췄고 `StageSceneConnectionEditModeTests`는 5/5 통과했다.
- COORD-160: `PlateCompactExecutor`가 기존 Plate를 먼저 비운 뒤 새 Plate에 summon을 이동하도록 정리했다. 직접 관련 경계 테스트는 통과했다.
- 테스트 이슈 정리: `PlateSummonDrawStaticRegressionTests`의 `TargetedAttackStrategy` 문자열 기대값을 현재 `effectValue` 기준으로 갱신했고, 전체 클래스는 36/36 통과했다.
- COORD-151: 플레이어/적 소환수 초기화 대표 흐름이 `Plate.SummonPlaceOnPlate()` -> `Summon.SummonInitialize()`임을 확인했고, 초기화 완료 알림을 `SummonInitialize()` 한 곳으로 모았다. `SummonDataRegressionTests`는 통과했다.
- AttackRule 미세정리는 같은 기능 반복 제한 규칙에 따라 다음 작업으로 잡지 않는다. 컴파일 실패가 있으면 실패 수습만 최소 범위로 처리한다.
- `MenuView`/`MenuHandler` 통합은 씬 연결 영향이 있어 자동 적용하지 않는다.
- `Summon` 대분리는 영향 범위가 커서 지금 바로 시작하지 않는다.
- 새 helper/service/common 계층 추가는 제외한다.
- Scene, prefab, ScriptableObject 값 변경은 별도 승인 전까지 제외한다.
- COORD-165: Story Screen 1/2/3/5/7의 `StorySkipView` UnityEvent 메서드명을 현재 public 메서드 `SkipAlert`에 맞췄다. `StorySystemStaticRegressionTests`는 26/26 통과했다.
- COORD-SCAN: 멀티에이전트 구조/연결 후보 재점검을 완료했다. 이후 BTL-SCENE-03은 테스트-only 연결 검증으로 처리했고, 현재 Ready는 SCENE-NULL-01이다.
- BTL-SCENE-03: `StageSceneConnectionEditModeTests`에 Plate runtime 연결과 Plate prefab `spawnTransform` 검증을 추가했다. `StageSceneConnectionEditModeTests` 7/7, 전체 EditMode 146/146, 전체 PlayMode 26/26 통과.
- Runtime null 1차 수습: Start Screen HUD additive 로드 경로와 `MenuCanvas` 지연 조회, FightScene 1~7 `PlayerController` UnityEvent 타입, PlayMode stale expectation을 정리했다.
- SCENE-NULL-01: Build Settings enabled scene과 `Assets/Prefabs` 전체 missing script 스캔, 소환수 prefab `shieldImage` 연결 계약을 추가했다. `Grass Spirit.prefab`, `HighDevil.prefab`의 기존 `ShieldImage` GameObject를 `Summon.shieldImage`에 연결했다. `StageSceneConnectionEditModeTests` 9/9, 전체 EditMode 148/148, 전체 PlayMode 26/26 통과.
- BTL-RUNTIME-01: `BattleProgressController`의 runtime `AddComponent<BattleStageContext>()` fallback을 제거했고, FightScene 1~7 battle runtime controller 필수 연결 계약을 추가했다. 전체 EditMode 149/149, 전체 PlayMode 26/26 통과.
- RUNTIME-WIRING-02: `StartGameFlow`의 `StageController` 의존 제거, Fight/Thank UnityEvent 직렬화 교정, `StorySkipView` 지연 핸들러 조회, Story 3 Fox 애니메이션 중복 제거, HUD `StageController` 제거, `MenuLoader` MonoScript import 복구를 적용했다. 전체 EditMode 155/155, 전체 PlayMode 28/28, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-03: Start Screen 설정 버튼 런타임 연결, HUD 중복 로드 방지, Story ESC 중복/파괴 참조 방어, Fight 결과창 시작 비활성화, 일반 소환 3옵션 fallback, prefab `Summon.GetImage()` 지연 확보를 적용했다. 전체 EditMode 155/155, 전체 PlayMode 32/32, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-05: 1Stage 전투 UI의 턴/명령/결과 알림 배치를 2Stage 기준으로 정리했고, 2~7Stage `UI_60_SummonPicker` zero-scale과 `PlayerPlateActions` 시작 활성 차이를 수습했다. `StageSceneConnectionEditModeTests` 15/15, 2Stage PlayMode 소환 UI/턴 교환 테스트 통과, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-06: `SummonStatusView`가 상태색/피격색을 덮기 전 원래 Image 색상을 캐싱하고, 상태 해제 시 원래 색으로 복구하도록 수정했다. `SummonStatusViewTests` 12/12, Unity Console error/warning 0건을 확인했다.
- RUNTIME-QA-07: 1~2Stage 전투 진입, 소환 선택 UI, 턴 교환, 결과창 시작 비활성 상태를 PlayMode 경로로 재점검했다. `StageRuntimeFlowPlayModeTests` 17/17, 전체 PlayMode 34/34, 전체 EditMode 158/158, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-09: FightScene 1~7의 소환 선택 옵션 판넬을 `400x580`에서 `380x550`으로 줄였고, 적 배치 직후 현재 전투 배수를 적 HP/공격력에 적용하도록 수정했다. `StageSceneConnectionEditModeTests` 15/15, `StageEnemyPlacementStaticRegressionTests` 11/11, `StageRuntimeFlowPlayModeTests` 18/18, 전체 PlayMode 35/35, 전체 EditMode 158/158, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-10: FightScene 1~7의 소환/재소환 3옵션 `RedrawPanel`을 `360x520` cell, 3열 고정 GridLayout, 넓은 panel 폭으로 맞췄고 1Stage `TurnTextUI` 프레임 배치를 텍스트와 맞췄다. 정적 scene YAML 확인과 `git diff --check`는 통과했지만 Unity 실행은 `LicenseClient` IPC timeout으로 결과 XML 없이 중단됐다.
- RUNTIME-HOTFIX-11: FightScene 1~7의 1개짜리 `DrawPanel/DrawOption_1` 오브젝트와 `SummonController`의 `drawPanel/drawOptionPanels` 직렬화 경로를 제거했다. 일반 소환과 재소환은 모두 공용 `RedrawPanel` 3옵션을 사용한다. 정적 scene YAML 확인과 `git diff --check`는 통과했지만 Unity 실행은 `LicenseClient` IPC timeout으로 결과 XML 없이 중단됐다.
- RUNTIME-HOTFIX-12: FightScene 1~7의 Unity YAML 헤더 누락으로 발생한 `File may be corrupted or was serialized with a newer version of Unity` 씬 로드 에러를 복구했다. 전체 `.unity` 헤더 검사, FightScene 로컬 fileID 참조 검사, `git diff --check`, Unity MCP `load_scene` 1~7Stage 확인을 통과했다.
- RUNTIME-TRIAGE-08: 남은 런타임 후보 재선정을 완료했다. 자동 검증 기준으로 `StageSceneConnectionEditModeTests` 15/15, `PlateSummonDrawStaticRegressionTests` 36/36, `StageRuntimeFlowPlayModeTests` 19/19가 통과했고 Unity Console error/warning 0건을 확인했다. 남은 확인은 직접 플레이 스모크 검증으로 넘긴다.
- Backlog 후보: STORY-01 StorySkipView 씬 이동 책임, STAGE-02 StageSelectView controller 연결, VHO-03 SettingPanel 직접 호출 우회.
- 보류 후보: STAGE-01 StageFlowController fallback 제거, STORY-02 InteractionController 종료 처리 분리, STORY-03 Story component wiring serialized 전환, BTL-RUNTIME-02 runtime AddComponent 제거, BTL-PRED-04 prediction 구성 위치 검증, VHO-01 StartScreen 대분리, VHO-02 MenuHandler 입력 정책 분리, VHO-04 Option View 적용/저장 분리, VHO-05 구형 MenuView 잔존 연결.
- 운영 규칙: 테스트 실행은 자동 진행한다. 테스트 결과 확인 뒤 10분 동안 사용자 응답이 없으면 코드/씬/에셋 수정이 필요 없는 다음 테스트 또는 문서/후보 정리 작업으로 이어간다. 코드/씬/프리팹/ScriptableObject 변경은 기존처럼 Change Proposal과 diff 승인 후 적용한다.
- 2026-06-22 구조 진행 완료: `TurnStateMachine`, `AttackStateMachine`, `BattleResultUseCase`, 공격 시작/일반공격/타겟선택 UseCase를 도입했다. 다음 작업은 공격 Application의 구체 컨트롤러 의존 축소다.
