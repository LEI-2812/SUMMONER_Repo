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

## Ready

| ID | 도메인 | 기능 슬라이스 | 목표 | 완료조건 | 첫 액션 | 호출 대상 |
|---|---|---|---|---|---|---|
| BTL-RUNTIME-01 | Battle Runtime | runtime `FindObjectOfType` / `AddComponent` 우회 제거 후보 탐색 | 전투 시작/턴/결과 흐름에서 null을 숨기는 runtime 검색/동적 컴포넌트 생성 후보를 분류한다 | 전투 시작/턴/결과 흐름에서 null을 숨기는 runtime 검색/동적 컴포넌트 생성 후보를 분류한다; scene/prefab serialized 값 수정 또는 public serialized field 변경이 필요하면 사용자에게 보고한다; code-only로 목표 구조에 맞는 후보가 있으면 한 기능 슬라이스만 적용하고 같은 Battle runtime 미세정리로 확장하지 않는다 | 멀티 에이전트로 Battle runtime 우회 흐름과 scene 연결 계약을 읽기 전용 탐색 | CoordinatorAgent |

## Backlog

| ID | 도메인 | 기능 슬라이스 | 목표 | 완료조건 | 보류 이유 |
|---|---|---|---|---|---|
| STORY-01 | Story | StorySkipView skip flow 책임 분리 | View가 스킵 후 전투 씬 이동을 직접 처리하지 않게 한다 | `StorySkipView`에는 `LoadCurrentPlayingFightScene` 직접 호출이 없고, 스킵 후 이동 결정은 Story flow가 담당한다 | `SkipAlertHandler`가 Menu와 Story에서 공용으로 쓰이므로 Story 전용 이동 책임만 좁혀야 한다 |
| STAGE-02 | Stage | StageSelectView controller 연결 명시화 | StageSelectView의 `FindObjectOfType<StageController>()` 의존을 줄인다 | `StageSelectView`가 `StageController`를 명시 참조하고 EditMode가 `buttons`, `audioSource`, `stageController` 할당을 확인한다 | `Stage Select Screen.unity` serialized field 변경이 필요할 수 있다 |
| VHO-03 | HUD/Option | SettingPanel 직접 호출 우회 제거 | 설정 열기/닫기 대표 진입점을 Handler 한 곳으로 단일화한다 | HUD UnityEvent가 `SettingPanelView.OpenOption`을 직접 호출하지 않고, `SettingPanelView`는 탭/패널 표시만 담당한다 | HUD UnityEvent 타깃 변경이 필요하므로 씬 diff 승인이 필요하다 |
| BTL-PRED-04 | Battle Prediction | prediction 구성 위치 검증 | PlayerAttackPrediction과 예측 컴포넌트 배치 계약을 명확히 한다 | 7개 Fight scene에서 필요한 `IAttackPrediction` 컴포넌트 구성이 검증된다 | 최근 AttackRule 미세정리 반복 제한 때문에 내부 예측 규칙 정리로 번지면 안 된다 |

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
