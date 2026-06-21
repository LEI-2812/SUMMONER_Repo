# Current Task

## 상태

- 현재 도메인: Battle Runtime
- 현재 상태: Ready
- Ready 에이전트: VerificationAgent

## 현재 기능 슬라이스

다음 작업: RUNTIME-QA-13 | Battle Runtime | 직접 플레이 스모크 검증
완료조건: Start Screen 설정 버튼, Story ESC, FightScene 1~2 일반 소환/재소환 3옵션, 결과창 시작 비활성화, Stage 5 적 배수 적용을 직접 플레이 또는 PlayMode 경로로 확인하고 Unity Console error/warning 0건을 기록한다
첫 액션: Unity에서 Start Screen부터 1Stage 전투 진입까지 직접 플레이 흐름을 확인하고, 재현되는 오류가 있으면 코드/씬/도구 문제로 분류한다
호출 대상: VerificationAgent

## 메모

- COORD-141~146, COORD-149~150은 적용 완료했다.
- COORD-148의 `NotifyObservers()` private 축소는 `UpdateStateObserver` 인터페이스 구현 계약을 깨므로 복구했다.
- COORD-147은 prefab `m_Script` GUID 연결 때문에 자동 삭제/통합을 보류했다.
- 수정 후보 큐는 `.codex/workflow/refactor-candidates.md`에 정리했다.
- 구조 기준은 `.codex/workflow/architecture-role-rule.md`에 정리했다.
- 기본 계층은 `View / Controller / Flow / Service / Action / State/Data`로 본다.
- Ready 작업은 완료조건을 먼저 걸고, 충족하면 같은 기능의 추가 미세정리로 확장하지 않는다.
- 새 helper/service/common 계층 추가보다 얇은 래퍼 축소와 public 계약 축소를 우선한다.
- 소환수 클래스는 prefab/component 연결 위험이 있으므로 삭제나 통합 전 반드시 scene/prefab GUID 연결을 확인한다.
- 이번 목표는 `Summon` 로직을 크게 분리하는 것이 아니라, 과분리와 중복을 줄이면서 데이터/상태 의존을 줄이는 것이다.
- 1차 범위는 코드-only 변경을 우선한다. prefab `m_Script`, scene, ScriptableObject 값 변경은 별도 승인 전까지 제외한다.
- 예측 로직은 소환수별 파일에 남겨 읽히게 하고, 공용 helper/base 추출로 숨기지 않는다.
- COORD-152는 Prediction 매칭에서 `SummonType` 계약을 제거했다. `IAttackPrediction`은 `CanPredict(Summon)`으로 자기 예측 대상 여부를 판단한다.
- COORD-161은 적용 완료했다. 이후 COORD-163에서 남아 있던 `SummonData` serialized `summonType`까지 제거했다.
- COORD-156은 적용 완료했다. 조건은 `PlayerAttackActions` private 메서드로 되돌렸고 별도 조건 파일은 제거했다.
- COORD-163은 적용 완료했다. `SummonType` enum, `SummonData.summonType`, asset YAML `summonType:` 값을 제거했다.
- COORD-164는 현재 코드에 없는 `StatusEffectController` 이름을 따라가지 않는다. 실제 대상은 `StatusEffectState`와 `Summon`의 상태효과 턴 갱신 흐름이다.
- COORD-164는 적용 완료했다. `TurnSummonStateUpdater`의 턴 시작 상태효과 갱신 단계를 private 메서드로 분리하고 일반 공격 쿨타임 갱신 null 방어를 추가했다.
- COORD-154는 code-only 변경 없이 보류로 닫았다. C# 외부 직접 `isAttacking` field 호출부는 없고 대표 기준은 `BattleAttackState`지만, FightScene 1~7에 `isAttacking:` serialized 값이 남아 field 제거는 scene 영향이 있다.
- COORD-155는 적용 완료했다. 일반 소환의 빈 플레이어 Plate 조회를 `PlateController.GetFirstEmptyPlayerPlateIndex()`로 좁혔다. `GetPlayerPlates()` 전체 제거와 공격/예측 흐름 재설계는 제외했다.
- COORD-157은 적용 완료했다. `StoryImageView`가 `InteractionController`에서 현재 대사 인덱스를 직접 조회하지 않고 `InteractionController.OnPointerClick()`에서 인덱스를 전달받도록 좁혔다.
- COORD-159는 적용 완료했다. `StorySkipView`가 `SettingPanelView`를 직접 조회하지 않고 `GameplaySettingStore.LoadStorySkipEnabled()`로 스킵 버튼 활성 여부를 판단하도록 좁혔다.
- COORD-158은 적용 완료했다. 씬 연결이 없는 상태를 고려해 `GameObject.Find("MenuCanvas")`는 유지하고, 옵션 패널 계층 문자열 탐색만 `FindSettingPanelTransform()`으로 모았다.
- SCENE-WIRING-001은 적용 완료했다. `Start Screen`의 중복 `StartScreenView` 컴포넌트를 제거하고, FightScene 1~7의 `TurnController.plateController`를 `PlateController` fileID `893681172`로 명시 연결했다.
- `StageSceneConnectionEditModeTests`는 5/5 통과했다. `CurrentTurnText`와 `ClearTurnText` 기대 경로를 실제 `UI_20_TurnStatus` 하위 구조에 맞췄다.
- COORD-160은 적용 완료했다. `Summon.Die()`는 Plate를 직접 찾지 않는 현재 구조를 유지했고, `PlateCompactExecutor`는 기존 Plate를 먼저 비운 뒤 새 Plate에 이동하도록 순서를 바꿔 death handler가 최종 Plate 기준으로 남게 했다.
- COORD-160 관련 경계 테스트 `Plate_RemoveSummonAlwaysClearsSlotState`, `Summon_ReportsDeathWithoutFindingPlate`는 통과했다.
- `PlateSummonDrawStaticRegressionTests` 전체 실행은 36/36 통과했다. 기존 `TargetedAttackStrategy` 문자열 기대값은 `effectValue` 기준으로 갱신했다.
- COORD-151은 적용 완료했다. 플레이어/적 소환수 모두 `Plate.SummonPlaceOnPlate()`에서 clone 생성 후 `Summon.SummonInitialize()`로 초기화되는 대표 흐름을 확인했고, `TryApplyAssignedSummonData()` 내부 중복 `NotifyObservers()`를 제거해 초기화 완료 알림을 `SummonInitialize()` 한 곳으로 모았다.
- COORD-151 검증: `recompile_scripts` 0 warning, `SummonDataRegressionTests` 통과.
- `dotnet build .\Assembly-CSharp.csproj --no-restore`는 기존 `NETSDK1004` project.assets.json 누락으로 C# 컴파일 전 중단됐다.
- 같은 특수공격/AttackRule 미세정리는 다음 세션 작업으로 잡지 않는다. 필요하면 컴파일 실패 수습만 최소로 처리한다.
- COORD-165는 적용 완료했다. Story Screen 1/2/3/5/7의 `StorySkipView` UnityEvent 메서드명을 `SkipAlert`로 맞췄고 `StorySystemStaticRegressionTests`는 26/26 통과했다.
- 테스트 실행은 사용자 추가 응답 없이 자동 진행한다. 테스트 결과 확인 뒤 10분 동안 사용자 응답이 없으면, 코드/씬/에셋 수정이 필요 없는 다음 테스트 또는 문서/후보 정리 작업으로 이어간다. 코드/씬/프리팹/ScriptableObject 변경은 기존처럼 Change Proposal과 diff 승인 후 적용한다.
- Runtime null 1차 수습은 통과했다. `GameSceneFlow.LoadHudAdditive()`는 Build Settings의 `HUD` 씬명을 사용하고, `StartScreenView.Start()`는 HUD additive 로드를 보장하며 옵션 열기 시 `MenuCanvas`를 지연 조회한다.
- FightScene 1~7 `PlayerTurnOverBtn` UnityEvent 타입은 `PlayerController`로 맞췄고, `StageRuntimeFlowPlayModeTests`는 stale `Player` 타입 기대와 과도한 로그 순서 기대를 제거했다.
- Plate 연결 검증을 추가했다. FightScene 1~7의 `Plate.spawnTransform`, `statePanel`, `statePanelScript`와 `PlayerPlates`/`EnemyPlate` prefab `spawnTransform`은 `StageSceneConnectionEditModeTests`로 검증한다.
- 검증 결과: `recompile_scripts` 0 warning, `StageSceneConnectionEditModeTests` 7/7 통과, 전체 EditMode 146/146 통과, 전체 PlayMode 26/26 통과, `git diff --check` 통과.
- SCENE-NULL-01은 적용 완료했다. Build Settings enabled scene과 `Assets/Prefabs` 전체 missing script 스캔, 소환수 prefab `shieldImage` 연결 계약을 `StageSceneConnectionEditModeTests`에 추가했다.
- `Grass Spirit.prefab`, `HighDevil.prefab`의 기존 `ShieldImage` GameObject를 `Summon.shieldImage`에 연결했다.
- 검증 결과: `recompile_scripts` 0 warning, `StageSceneConnectionEditModeTests` 9/9 통과, 전체 EditMode 148/148 통과, 전체 PlayMode 26/26 통과, Unity Console error 0건, `git diff --check` 통과.
- BTL-RUNTIME-01은 적용 완료했다. `BattleProgressController`가 `BattleStageContext`를 runtime `AddComponent`로 만들지 않고 누락 시 에러를 남기도록 바꿨다.
- FightScene 1~7의 주요 battle runtime controller 필수 연결 계약을 `StageSceneConnectionEditModeTests`에 추가했다.
- 검증 결과: `BattleResultFlowStaticRegressionTests` 5/5, `StageSceneConnectionEditModeTests` 10/10, 관련 PlayMode 1/1, 전체 EditMode 149/149, 전체 PlayMode 26/26, Unity Console error 0건, `git diff --check` 통과.
- RUNTIME-WIRING-02는 적용 완료했다. `StartGameFlow`의 Start/Continue `StageController` 의존을 제거했고, FightScene 1~7 `Player` UnityEvent 타입명, Thank Screen 메서드 대소문자, `StorySkipView` 시작 시점 경고, Story 3 Fox 중복 `Animation`, HUD의 잘못된 `StageController`, `MenuLoader` MonoScript import 끊김을 수습했다.
- 검증 결과: `recompile_scripts` 0 warning, 전체 EditMode 155/155 통과, 전체 PlayMode 28/28 통과, Unity Console error 0건/warning 0건, `git diff --check` 통과.
- RUNTIME-HOTFIX-03은 적용 완료했다. Start Screen 설정 버튼 런타임 연결, HUD 중복 로드 방지, Story ESC 중복/파괴 참조 방어, Fight 결과창 시작 비활성화, 일반 소환 3옵션 fallback, prefab `Summon.GetImage()` 지연 확보를 수습했다.
- 검증 결과: `recompile_scripts` 0 warning, 전체 PlayMode 32/32 통과, 전체 EditMode 155/155 통과, Unity Console error 0건/warning 0건, `git diff --check` 통과.
- RUNTIME-HOTFIX-05는 적용 완료했다. 1Stage 전투 UI의 턴/명령/결과 알림 배치를 2Stage식 full-stretch layout으로 맞췄고, 2~7Stage의 `UI_60_SummonPicker` zero-scale 상태를 1Stage식 visible layout으로 복구했다. `PlayerPlateActions`는 1~7Stage 모두 시작 비활성 상태로 맞췄다.
- 검증 결과: `recompile_scripts` 0 warning, `StageSceneConnectionEditModeTests` 15/15 통과, 2Stage PlayMode 소환 UI/턴 교환 테스트 각각 통과, Unity Console error 0건/warning 0건, `git diff --check` 공백 오류 없음. 전체 PlayMode 클래스 실행은 MCP 요청 timeout으로 끊겨 새 2Stage 테스트만 분리 검증했다.
- RUNTIME-HOTFIX-06은 적용 완료했다. `SummonStatusView`가 상태색/피격색을 덮기 전 원래 Image 색상을 캐싱하고, 상태가 없어지면 원래 색으로 복구하도록 좁혔다.
- 검증 결과: `recompile_scripts` 0 warning, `SummonStatusViewTests` 12/12 통과, Unity Console error 0건/warning 0건, `git diff --check` 공백 오류 없음.
- RUNTIME-QA-07은 완료했다. `StageRuntimeFlowPlayModeTests` 전체 17/17, 전체 PlayMode 34/34, 전체 EditMode 158/158 통과했고 Unity Console error 0건/warning 0건을 확인했다.
- RUNTIME-HOTFIX-09는 적용 완료했다. 소환 선택 옵션 판넬 4개를 FightScene 1~7에서 `400x580`에서 `380x550`으로 줄였고, 적 배치 직후 현재 전투 배수를 적 HP/공격력에 적용하도록 했다.
- 검증 결과: `recompile_scripts` 0 warning, `StageSceneConnectionEditModeTests` 15/15 통과, `StageEnemyPlacementStaticRegressionTests` 11/11 통과, `StageRuntimeFlowPlayModeTests` 18/18 통과, 전체 PlayMode 35/35 통과, 전체 EditMode 158/158 통과, Unity Console error 0건/warning 0건.
- RUNTIME-HOTFIX-10은 적용 완료했다. FightScene 1~7의 소환/재소환 3옵션 공용 `RedrawPanel`을 넓히고 GridLayout을 3열 고정으로 바꿔 세 옵션이 한 줄에 나오도록 했다. 1Stage `TurnTextUI` 프레임도 텍스트 위치와 맞췄다.
- 검증 결과: scene YAML 정적 확인은 FightScene 1~7 모두 `RedrawPanel` 1개, 3열 GridLayout 1개, `360x520` 옵션 4개로 일치했다. `git diff --check`는 공백 오류 없이 CRLF 경고만 있었다. Unity EditMode 실행은 `LicenseClient` IPC timeout으로 결과 XML 없이 중단됐고, `dotnet build/restore`는 `project.assets.json`/restore 환경 문제로 중단되어 코드 실패로 분류하지 않는다.
- RUNTIME-HOTFIX-11은 적용 완료했다. FightScene 1~7의 1개짜리 `DrawPanel/DrawOption_1` 오브젝트와 `SummonController`의 `drawPanel/drawOptionPanels` 직렬화 경로를 제거하고, 일반 소환과 재소환 모두 공용 `RedrawPanel`의 3개 선택지로 표시되도록 통일했다.
- 검증 결과: FightScene 1~7 정적 scene YAML 확인에서 `DrawPanel`/삭제 fileID/옛 직렬화 필드는 0건이고, `RedrawPanel` 3옵션 구조는 유지됐다. `git diff --check`는 공백 오류 없이 CRLF 경고만 있었다. Unity EditMode 실행은 `LicenseClient` IPC timeout으로 결과 XML 없이 중단됐고, `dotnet build/restore`는 `project.assets.json`/restore 환경 문제로 중단되어 코드 실패로 분류하지 않는다.
- RUNTIME-HOTFIX-12는 적용 완료했다. FightScene 1~7의 누락된 Unity YAML 헤더 `%YAML 1.1`, `%TAG !u! tag:unity3d.com,2011:`를 복구해 `File may be corrupted or was serialized with a newer version of Unity` 씬 로드 에러를 수습했다.
- 검증 결과: 전체 `.unity` 씬 헤더 검사 통과, FightScene 1~7 로컬 fileID 참조 누락 0건, `git diff --check -- Assets/Screen/FightScene` 통과, Unity MCP `load_scene`으로 FightScene 1~7 모두 로드 성공.
- RUNTIME-TRIAGE-08은 완료했다. `StageSceneConnectionEditModeTests` 15/15, `PlateSummonDrawStaticRegressionTests` 36/36, `StageRuntimeFlowPlayModeTests` 19/19가 통과했고 Unity Console error/warning 0건을 확인했다.
