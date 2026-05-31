# 현재 QA

현재 수정 중이거나 다시 확인해야 하는 문제만 둔다.
끝난 항목은 `90_done/YYYY-MM.md`로 옮기고, 오래된 상세 근거는 필요할 때만 `QAReports`에서 찾는다.

## 상태 기준

- `Open`: 아직 해결 안 됨
- `Fixed`: 수정했지만 재확인 전
- `Retest`: 코드 수정 없이 다시 테스트 필요
- `Hold`: 환경이나 실행 조건 때문에 지금은 보류

## 우선순위

| ID | 상태 | 우선순위 | 기능 | 제목 | 다음 행동 |
|---|---|---|---|---|---|
| QA-001 | Open | High | BattleCore | Fight 1Stage 진입 중 `Summon.Update()` NullReferenceException | 소환수 초기화 경로 수정 후 Fight 1Stage 재확인 |
| QA-002 | Fixed | High | BattleCore | 적 전멸 시 승리 Alert 런타임 재검증 필요 | QA-001 해결 후 승리 Alert, Retry, 진행 저장 확인 |
| QA-003 | Hold | High | QAInfrastructure | MCP PlayMode 테스트 timeout 또는 connection error | PlayMode 단일 테스트와 Console 조회 안정성 확인 |
| QA-004 | Open | High | StorySystem | Only Mouse 옵션이 켜져도 Space 입력으로 대사가 진행됨 | Story 입력 공통 처리에서 `IsOnlyMouse` 반영 |
| QA-005 | Retest | High | StorySystem | Story Skip 옵션 런타임 흐름 확인 필요 | 실제 Story 진행 중 Skip 표시와 씬 이동 확인 |
| QA-006 | Open | High | OptionSystem | 오디오 볼륨 0에서 `Mathf.Log10(0)` 호출 가능 | 볼륨 0 방어 후 음소거/복구 확인 |
| QA-007 | Open | Medium | OptionSystem | 잘못된 비디오 저장 인덱스가 기본값으로 복구되지 않음 | 저장 인덱스 범위 방어 후 토글 상태 확인 |
| QA-008 | Retest | High | GameSystem | `playingStage > savedStage` 런타임 흐름 확인 필요 | 이어하기와 스테이지 잠금/해금 런타임 확인 |

---

## QA-001

> status: Open / priority: High / feature: BattleCore / updated: 2026-05-31

- 제목: Fight 1Stage 진입 중 `Summon.Update()` NullReferenceException 발생
- 문제: 전투 PlayMode 테스트에서 Fight 1Stage 진입 중 `Summon.Update()`가 NullReferenceException을 낸다. 이 문제 때문에 승리 Alert 런타임 QA도 막힌다.
- 재현: MCP PlayMode에서 `StageRuntimeFlowPlayModeTests.BattleResultAlertView_FightScene_HasRequiredRuntimeControllers` 실행 후 Fight 1Stage 로그와 Console error 확인
- 기대: Fight 1Stage가 NullReferenceException 없이 로드되고 전투 런타임 컨트롤러를 확인할 수 있어야 한다.
- 실제: `Summon.Update()` line 77에서 NullReferenceException 발생
- 의심 원인: 일부 파생 소환수의 `Awake()`가 base `Summon` 초기화를 보장하지 않아 `SummonStatusView` 같은 View 참조가 null이 되는 구조
- 수정: 미수정
- 확인:
  - [x] 재현되는지 확인
  - [ ] 수정 후 Fight 1Stage가 NullReferenceException 없이 열리는지 확인
  - [ ] 기존 소환수 이미지, 색상, 효과음 표시가 유지되는지 확인
  - [ ] Battle 승리 Alert 재검증이 가능해지는지 확인
- 관련 파일: `Assets/Script/Summons/Summon.cs`, `Assets/Script/Summons/*.cs`, `Assets/Script/Battle/View/SummonStatusView.cs`, `Assets/Script/Battle/View/SummonImageView.cs`, `Assets/Script/Battle/View/SummonSoundView.cs`
- 메모: 기존 QAReports의 `BUG-BC-002`를 새 current QA로 옮긴 항목

---

## QA-002

> status: Fixed / priority: High / feature: BattleCore / updated: 2026-05-31

- 제목: 적 전멸 시 승리 Alert 런타임 재검증 필요
- 문제: 전투 결과 처리 구조는 수정됐지만, Fight Scene에서 실제로 적이 모두 사망했을 때 승리 Alert가 표시되는지는 PlayMode timeout 때문에 확인하지 못했다.
- 재현: QA-001 해결 후 Fight 1Stage를 PlayMode로 실행하고 적 전멸 흐름을 만든다.
- 기대: 적 전멸 시 승리 Alert가 표시되고 다음 진행 또는 Retry 흐름이 정상 동작해야 한다.
- 실제: 정적 연결 테스트는 통과했지만 PlayMode timeout으로 런타임 결과는 미확인
- 원인: 기존 StageController 참조 위험은 `BattleResultController`, `BattleProgressController`, `BattleStageContext` 분리로 수정했지만 런타임 확인이 QA-001과 PlayMode timeout에 막혀 있다.
- 수정: `BattleResultController`, `BattleProgressController`, `BattleStageContext` 구조로 분리 완료
- 확인:
  - [x] 정적 씬 연결 테스트 통과
  - [ ] 실제 Fight Scene에서 승리 Alert 표시 확인
  - [ ] Retry 동작 확인
  - [ ] 다음 스테이지 진행 저장 확인
- 관련 파일: `Assets/Script/Battle/Flow/BattleResultController.cs`, `Assets/Script/Battle/Flow/BattleProgressController.cs`, `Assets/Script/Battle/Flow/BattleStageContext.cs`, `Assets/Script/Battle/View/BattleResultAlertView.cs`
- 메모: 기존 QAReports의 `BUG-BC-001`을 새 current QA로 옮긴 항목

---

## QA-003

> status: Hold / priority: High / feature: QAInfrastructure / updated: 2026-05-31

- 제목: MCP PlayMode 테스트 timeout 또는 connection error
- 문제: 여러 PlayMode 테스트가 timeout 또는 `Connection failed: Unknown error`로 끝난다.
- 재현: MCP `recompile_scripts` 실행 후 PlayMode 테스트를 클래스 단위로 실행한다.
- 기대: PlayMode 테스트가 완료되고 결과와 로그를 확인할 수 있어야 한다.
- 실제: 일부 PlayMode 테스트가 timeout 또는 connection error로 끝난다.
- 원인: MCP/Unity Editor 연결 또는 PlayMode 실행 안정성 문제로 분류
- 수정: 미수정
- 확인:
  - [x] EditMode 테스트는 클래스 단위로 실행 가능
  - [ ] PlayMode 단일 테스트가 안정적으로 완료되는지 확인
  - [ ] timeout 시 Console error를 별도로 조회할 수 있는지 확인
- 관련 파일: `Assets/Tests/PlayMode`, `LocalPackages/com.gamelovers.mcp-unity`
- 메모: 환경 문제는 코드 실패와 섞지 않고 `Hold`로 관리

---

## QA-004

> status: Open / priority: High / feature: StorySystem / updated: 2026-05-31

- 제목: Only Mouse 옵션이 켜져도 Space 입력으로 대사가 진행됨
- 문제: Story 입력 공통 처리에서 `IsOnlyMouse` 옵션을 확인하지 않고 Space 입력을 처리한다.
- 재현: Only Mouse 옵션을 켠 뒤 Story Scene에서 Space 키를 누른다.
- 기대: Only Mouse 옵션이 켜져 있으면 Space 키로 대사가 진행되지 않아야 한다.
- 실제: `StoryScenarioControllerBase`가 Space 입력을 처리할 수 있다.
- 원인: `StoryScenarioControllerBase`의 입력 처리에서 `IsOnlyMouse` 옵션 확인이 빠진 것으로 보인다.
- 수정: 미수정
- 확인:
  - [ ] Only Mouse ON에서 Space 입력이 막히는지 확인
  - [ ] Only Mouse OFF에서 Space 입력이 유지되는지 확인
  - [ ] 클릭 입력은 기존처럼 동작하는지 확인
- 관련 파일: `Assets/Script/Story/Scenario/StoryScenarioControllerBase.cs`, `Assets/Script/Option/GameplaySettingView.cs`
- 메모: 기존 `BUG-ST-004`, `BUG-OP-002`와 연결

---

## QA-005

> status: Retest / priority: High / feature: StorySystem / updated: 2026-05-31

- 제목: Story Skip 옵션 런타임 흐름 확인 필요
- 문제: `StorySkipView`가 Story Skip 옵션을 읽는 정적 연결은 확인됐지만, 실제 Story 진행 중 Skip 버튼 표시와 이동 흐름은 확인하지 못했다.
- 재현: Story Skip 옵션을 켜고 Story Scene에 진입한 뒤 Skip 버튼 표시와 클릭 후 이동 흐름을 확인한다.
- 기대: 옵션에 따라 Skip 버튼이 표시되고, 클릭 시 의도한 씬으로 이동해야 한다.
- 실제: 정적 연결만 확인했고 런타임은 미확인
- 원인: 런타임 PlayMode QA가 timeout으로 막혀 확인하지 못했다.
- 수정: 미수정
- 확인:
  - [x] 정적 연결 확인
  - [ ] 옵션 ON/OFF에 따른 Skip 버튼 표시 확인
  - [ ] Skip 클릭 후 씬 이동 확인
- 관련 파일: `Assets/Script/Story/View/StorySkipView.cs`, `Assets/Script/Story/Progress/StorySceneMove.cs`
- 메모: 기존 `BUG-ST-002`를 새 current QA로 옮긴 항목

---

## QA-006

> status: Open / priority: High / feature: OptionSystem / updated: 2026-05-31

- 제목: 오디오 볼륨 0에서 `Mathf.Log10(0)` 호출 가능
- 문제: 오디오 설정에서 볼륨 값이 0이 되면 `Mathf.Log10(0)`이 호출될 수 있다.
- 재현: 설정 창을 열고 오디오 볼륨을 0으로 낮춘 뒤 Console warning/error와 실제 오디오 값을 확인한다.
- 기대: 볼륨 0에서도 에러 없이 음소거 값으로 처리되어야 한다.
- 실제: `Mathf.Log10(0)` 호출 가능성이 있다.
- 원인: 볼륨 값을 dB로 변환하기 전에 0 또는 최소값 방어가 없다.
- 수정: 미수정
- 확인:
  - [ ] 볼륨 0 설정 시 Console error 없음
  - [ ] 음소거 상태 정상 적용
  - [ ] 볼륨을 다시 올렸을 때 정상 복구
- 관련 파일: `Assets/Script/Option/AudioSettingView.cs`
- 메모: 기존 `BUG-OP-001`을 새 current QA로 옮긴 항목

---

## QA-007

> status: Open / priority: Medium / feature: OptionSystem / updated: 2026-05-31

- 제목: 잘못된 비디오 저장 인덱스가 기본값으로 복구되지 않음
- 문제: 잘못된 비디오 설정 인덱스가 저장되어 있으면 모든 토글이 꺼질 수 있다.
- 재현: 비디오 설정 저장값을 잘못된 인덱스로 만든 뒤 설정 창에서 해상도 또는 화면 옵션 토글 상태를 확인한다.
- 기대: 잘못된 저장값은 기본값으로 복구되어야 한다.
- 실제: 모든 토글이 꺼질 수 있다.
- 원인: 저장된 인덱스를 UI에 적용하기 전에 유효 범위 검사가 부족하다.
- 수정: 미수정
- 확인:
  - [ ] 잘못된 저장 인덱스가 기본값으로 복구되는지 확인
  - [ ] 토글 중 하나가 항상 선택되는지 확인
  - [ ] 정상 저장값은 유지되는지 확인
- 관련 파일: `Assets/Script/Option/VideoSettingView.cs`
- 메모: 기존 `BUG-OP-004`를 새 current QA로 옮긴 항목

---

## QA-008

> status: Retest / priority: High / feature: GameSystem / updated: 2026-05-31

- 제목: `playingStage > savedStage` 런타임 흐름 확인 필요
- 문제: `savedStage` 8 이상 표시와 버튼 활성화 경로는 정적 방어가 확인됐지만, `playingStage`가 `savedStage`보다 큰 런타임 흐름은 확인하지 못했다.
- 재현: 저장 데이터에서 `savedStage`와 `playingStage` 값을 다르게 만든 뒤 시작 화면 또는 스테이지 선택 화면에서 이어하기와 스테이지 버튼 상태를 확인한다.
- 기대: 저장된 진행도보다 큰 플레이 중 스테이지 값이 잘못 사용되지 않아야 한다.
- 실제: 런타임 흐름 미확인
- 원인: PlayMode 테스트 timeout으로 확인하지 못했다.
- 수정: 미수정
- 확인:
  - [x] 정적 방어 확인
  - [ ] 런타임 이어하기 흐름 확인
  - [ ] 스테이지 잠금/해금 상태 확인
- 관련 파일: `Assets/Script/Save/GameSaveController.cs`, `Assets/Script/Save/PlayerPrefsSaveStore.cs`, `Assets/Script/Stage/StageSelectView.cs`
- 메모: 기존 `BUG-GS-003`을 새 current QA로 옮긴 항목
