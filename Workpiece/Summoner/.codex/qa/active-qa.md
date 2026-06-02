# 현재 QA

DevAgent와 QAAgent가 지금 봐야 하는 QA는 이 파일 하나에서 관리한다.
QA 항목 상세 상태의 원본은 이 파일이다.
`.codex/workflow/issue-board.md`는 요약 인덱스로만 사용한다.
새 요청, 수정 후 재검증, Hold 항목을 모두 여기에 둔다.
끝난 항목은 `docs/archive/legacy-docs-2026-06-02/qa/done/` 또는 이후 QA archive로 옮긴다.
이전 `QAReports`는 `docs/archive/legacy-docs-2026-06-02/QAReports/`에서 오래된 상세 근거가 필요할 때만 확인한다.

## 사용 규칙

- DevAgent는 새 QA 요청이나 수정 완료 항목을 이 파일에 남긴다.
- QAAgent는 이 파일의 우선순위 표를 보고 위에서부터 확인한다.
- QAAgent가 통과를 확인한 항목만 archive로 옮긴다.
- archive의 `QAReports/README.md`에는 현재 할 일을 새로 쓰지 않는다.

## QAAgent 작업 우선순위

QAAgent는 DevAgent가 `Fixed`로 넘긴 재검증 항목을 가장 먼저 처리한다.
`Fixed` 항목이 남아 있으면 관련 없는 기존 `Open` 항목을 먼저 훑지 않는다.

1. `Fixed`: DevAgent 수정 재검증
2. `Retest`: 코드 수정 없이 남은 런타임/수동 확인
3. `Hold`: Fixed/Retest를 막는 MCP, PlayMode, Console 차단 원인 분리
4. `Open`: 기존 실패 항목의 근거 보강과 Dev Fix Request 작성
5. `QAReports`: current 항목 판단에 필요한 과거 근거만 참고

같은 상태 안에서는 아래 우선순위 표의 위쪽 항목과 High 우선순위를 먼저 본다.

## 상태 기준

- `Open`: 아직 해결 안 됨
- `Fixed`: 수정했지만 재확인 전
- `Retest`: 코드 수정 없이 다시 테스트 필요
- `Hold`: 환경이나 실행 조건 때문에 지금은 보류

## 우선순위

| ID | 상태 | 우선순위 | 기능 | 제목 | 다음 행동 |
|---|---|---|---|---|---|
| QA-002 | Retest | High | BattleCore | 적 전멸 시 승리 Alert 런타임 재검증 필요 | 안정적인 PlayMode 또는 수동 Play로 승리 Alert, Retry, 진행 저장 확인 |
| QA-003 | Hold | High | QAInfrastructure | MCP PlayMode 테스트 timeout 또는 connection error | PlayMode timeout 후 Console 조회와 최소 재현 단위 기록 |
| QA-010 | Retest | Medium | BattleContent | 플레이어 소환수 6종 SummonData 에셋 연결 런타임 확인 | 안정적인 PlayMode 또는 수동 Play로 6종 소환 후 능력치/공격 표시 확인 |
| QA-012 | Open | Medium | QAInfrastructure | AudioSource obsolete property Console error 발생 | MCP 오브젝트 조회 또는 런타임 AudioSource 직렬화 경로 확인 |
| QA-004 | Fixed | High | StorySystem | Only Mouse 옵션이 켜져도 Space 입력으로 대사가 진행됨 | Only Mouse ON/OFF와 클릭 입력 유지 재검증 |
| QA-005 | Retest | High | StorySystem | Story Skip 옵션 런타임 흐름 확인 필요 | 실제 Story 진행 중 Skip 표시와 씬 이동 확인 |
| QA-006 | Fixed | High | OptionSystem | 오디오 볼륨 0에서 `Mathf.Log10(0)` 호출 가능 | 수정 후 볼륨 0 음소거/복구 확인 |
| QA-007 | Fixed | Medium | OptionSystem | 잘못된 비디오 저장 인덱스가 기본값으로 복구되지 않음 | 잘못된 저장 인덱스 기본값 복구와 토글 선택 상태 재검증 |
| QA-008 | Retest | High | GameSystem | `playingStage > savedStage` 런타임 흐름 확인 필요 | 이어하기와 스테이지 잠금/해금 런타임 확인 |

## QA-002

> status: Retest / priority: High / feature: BattleCore / updated: 2026-06-02

- 제목: 적 전멸 시 승리 Alert 런타임 재검증 필요
- 문제: 전투 결과 처리 구조는 수정됐지만, Fight Scene에서 실제로 적이 모두 사망했을 때 승리 Alert가 표시되는지는 PlayMode timeout 때문에 확인하지 못했다.
- 재현: QA-001 해결 후 Fight 1Stage를 PlayMode로 실행하고 적 전멸 흐름을 만든다.
- 기대: 적 전멸 시 승리 Alert가 표시되고 다음 진행 또는 Retry 흐름이 정상 동작해야 한다.
- 실제: 정적 연결 테스트는 통과했지만 PlayMode timeout으로 런타임 결과는 미확인
- 원인: 기존 StageController 참조 위험은 `BattleResultController`, `BattleProgressController`, `BattleStageContext` 분리로 수정했지만 런타임 확인이 QA-001과 PlayMode timeout에 막혀 있다.
- 수정: `BattleResultController`, `BattleProgressController`, `BattleStageContext` 구조로 분리 완료
- 확인:
  - [x] 정적 씬 연결 테스트 통과
  - [x] MCP EditMode `Summoner.EditModeTests.BattleResultFlowStaticRegressionTests` 5/5 통과
  - [x] MCP EditMode `StageSceneConnectionEditModeTests.FightScreens_HaveRequiredResultAlertConnections` 1/1 통과
  - [ ] 실제 Fight Scene에서 승리 Alert 표시 확인
  - [ ] Retry 동작 확인
  - [ ] 다음 스테이지 진행 저장 확인
- 관련 파일: `Assets/Script/Battle/Flow/BattleResultController.cs`, `Assets/Script/Battle/Flow/BattleProgressController.cs`, `Assets/Script/Battle/Flow/BattleStageContext.cs`, `Assets/Script/Battle/View/BattleResultAlertView.cs`
- 메모: 기존 QAReports의 `BUG-BC-001`을 새 current QA로 옮긴 항목
  - 2026-06-02 PlayMode `StageRuntimeFlowPlayModeTests.BattleResultAlertView_FightScene_HasRequiredRuntimeControllers` 재시도는 timeout이었다. 직후 Console error/warning은 0개였다.
  - 2026-06-02 Fight 1Stage에서 `UI_90_ResultAlerts`와 `Player`의 `BattleResultAlertView` 존재는 확인했지만, `Edit/Play` 진입 명령이 `Connection failed: Unknown error`로 실패해 실제 승리 Alert 표시/Retry/저장은 확인하지 못했다.
  - 2026-06-02 재시도에서 Console info에 `현재 플레이 스테이지 : 2`, `버튼 클릭`, `Fade In 중...`, `savedStage 값` 로그가 남아 실제 실행 흔적은 확인했다.
  - `UI_90_ResultAlerts`, `Alert_clear`, `Alert_fail`, `Player`의 `BattleResultAlertView`는 존재했고 Console error는 0개였다.
  - Console warning에 `The referenced script (Unknown) on this Behaviour is missing!` 1개가 남아 QA-002를 통과 처리하지 않는다.
  - `Assets/Screen`, `Assets/Prefabs` 검색에서는 `m_Script: {fileID: 0}` 항목을 찾지 못했고, 런타임 루트 계층 스캔은 MCP timeout으로 완료하지 못했다.

---

## QA-003

> status: Hold / priority: High / feature: QAInfrastructure / updated: 2026-06-02

- 제목: MCP PlayMode 테스트 timeout 또는 connection error
- 문제: 여러 PlayMode 테스트가 timeout 또는 `Connection failed: Unknown error`로 끝난다.
- 재현: MCP `recompile_scripts` 실행 후 PlayMode 테스트를 클래스 단위로 실행한다.
- 기대: PlayMode 테스트가 완료되고 결과와 로그를 확인할 수 있어야 한다.
- 실제: 2026-06-02 기준 `recompile_scripts`는 0 warning으로 완료되고 EditMode `SummonDataRegressionTests`는 4/4 통과했다. PlayMode `StageRuntimeFlowPlayModeTests.BattleResultAlertView_FightScene_HasRequiredRuntimeControllers`와 `GameSaveProgressReadTests.GetGameSaveOrDefault_LoadsPlayerPrefs_WhenControllerIsMissing`는 timeout으로 끝났다.
- 원인: MCP/Unity Editor 연결 또는 PlayMode 실행 안정성 문제로 분류
- 수정: 코드 수정 아님. Workflow에 PlayMode timeout triage와 Console 조회 기준을 추가했다.
- 확인:
  - [x] EditMode 테스트는 클래스 단위로 실행 가능
  - [ ] PlayMode 단일 테스트가 안정적으로 완료되는지 확인
  - [x] timeout 시 Console 로그를 별도로 조회할 수 있는지 확인
  - [x] Console에 유효한 코드 error가 없는 timeout을 QAInfrastructure Hold로 분류했는지 확인
- 관련 파일: `Assets/Tests/PlayMode`, `LocalPackages/com.gamelovers.mcp-unity`
- 메모: 환경 문제는 코드 실패와 섞지 않고 `Hold`로 관리한다. 2026-06-02 Console 조회 결과는 MCP WebSocket server 시작 로그만 확인됐고 PlayMode 실패 원인 error/warning 로그는 없었다.
  - Fight 1Stage 씬 로드 후 Console error 조회는 timeout으로 완료하지 못했다.
  - MCP `execute_menu_item("Edit/Play")`도 timeout으로 끝났고, 직후 `get_scene_info`와 Console error 조회도 timeout이었다.
  - 2026-06-02 재시도에서 `execute_menu_item("Edit/Play")`는 요청 timeout 후 뒤늦게 Fight 1Stage Play에 진입했다.
  - Play 진입 후 Console error는 0개, warning은 `DontDestroyOnLoad only works for root GameObjects or components on root GameObjects.` 1개였다.
  - 2026-06-02 전체 EditMode 테스트 실행은 timeout으로 완료되지 않았다. 직후 Console error/warning은 0개였다.
  - 2026-06-02 PlayMode `StageRuntimeFlowPlayModeTests.BattleResultAlertView_FightScene_HasRequiredRuntimeControllers` 재시도는 timeout으로 완료되지 않았다. 직후 Console error/warning은 0개였다.
  - 2026-06-02 `execute_menu_item("Edit/Play")`가 `Connection failed: Unknown error`로 실패했고, 직후 `get_scene_info`와 Console 조회 명령은 queue에서 60초 만료됐다.
  - 2026-06-02 Play 진입/종료 토글 명령은 MCP 응답이 `Connection failed: Unknown error`일 수 있으나, Console info 로그상 실제 실행 흔적은 남을 수 있다. 실제 상태는 별도 씬/Console 조회로 판단한다.
  - 2026-06-02 QA-006 후속 재시도에서 `get_scene_info`는 한 번 정상 응답해 HUD 씬, dirty false를 확인했다.
  - 2026-06-02 직후 Console error 조회는 timeout, `Edit/Play`도 timeout, 다시 실행한 `get_scene_info`도 timeout이었다.
  - 이번 결과는 기능 실패가 아니라 MCP Console/Play/queue 안정성 문제로 보며, QA-003 근거로 유지한다.

---

## QA-010

> status: Retest / priority: Medium / feature: BattleContent / updated: 2026-06-02

- 제목: 플레이어 소환수 6종 SummonData 에셋 연결 런타임 확인
- 문제: 플레이어 소환수 6종 프리팹을 각 `SummonData` 기반 초기화로 연결했지만, 실제 소환 흐름에서 표시와 공격 동작은 아직 런타임 확인이 필요하다.
- 재현: Fight Scene 또는 소환 테스트 흐름에서 `Cat`, `Rabbit`, `Snake`, `Wolf`, `Eagle`, `Fox`를 소환한다.
- 기대: 6종 모두 기존 기준값과 같은 이름, 체력, 공격력, 일반/특수 공격 기준으로 동작해야 한다.
- 실제: EditMode 기준값과 프리팹 참조는 확인했지만 런타임 소환 표시와 공격 동작은 미확인
- 수정: 플레이어 소환수 6종 `SummonData` 에셋을 추가하고 각 프리팹의 `summonData`에 연결했다.
- 확인:
  - [x] 6종 프리팹이 각 `SummonData` GUID를 참조하는지 EditMode 테스트로 확인
  - [x] MCP `recompile_scripts(returnWithLogs: true)` 0 warning
  - [x] MCP EditMode `Summoner.EditModeTests.SummonDataRegressionTests` 4/4 통과
  - [x] Fight 1Stage `SummonController`가 Cat, Rabbit, Wolf, Eagle, Snake, Fox 6종을 참조하는지 확인
  - [x] MCP EditMode `Summoner.EditModeTests.SummonDataRegressionTests` 4/4 재통과
  - [x] Fight 1Stage `PlayerPlates`에 First/Second/Third Plate와 Plate 컴포넌트가 있는지 확인
  - [x] Fight 1Stage `UI_60_SummonPicker`가 활성 상태이고 `SummonController`가 있는지 확인
  - [x] 조회 직후 Console error/warning 0개
  - [ ] 실제 소환 흐름에서 6종 능력치 표시 유지 확인
  - [ ] 일반 공격과 특수 공격 동작 유지 확인
- 관련 파일: `Assets/Script/Summons/Summon.cs`, `Assets/Script/Summons/Cat.cs`, `Assets/Script/Summons/Rabbit.cs`, `Assets/Script/Summons/Snake.cs`, `Assets/Script/Summons/Wolf.cs`, `Assets/Script/Summons/Eagle.cs`, `Assets/Script/Summons/Fox.cs`, `Assets/Script/Summons/Data/*SummonData.asset`, `Assets/Prefabs/SummonPrefab/*.prefab`
- 메모: 적/정령 계열은 `SummonType` 의미와 섞지 않고, 이후 `EnemyType` 같은 별도 타입 확장 검토 후 데이터화한다.
- 재검증 요청: QAAgent가 PlayMode 또는 수동 Fight Scene 확인으로 6종 소환 표시와 공격 동작을 확인한다.
  - 2026-06-02 MCP 조회에서 `UI_60_SummonPicker`는 활성 상태이고 `SummonController`가 붙어 있었다. 자식 후보 계층 조회는 timeout이라 실제 후보 표시와 소환 조작은 아직 확인하지 않았다.
  - 현재 자동 테스트에는 후보 클릭, Plate 배치, 일반/특수 공격 동작을 직접 검증하는 PlayMode 테스트가 없다.

---

## QA-012

> status: Open / priority: Medium / feature: QAInfrastructure / updated: 2026-06-02

- 제목: AudioSource obsolete property Console error 발생
- 문제: Fight 1Stage 런타임 확인 후 Console error 3개가 발생했다.
- 재현: Fight 1Stage Play 진입 후 MCP `get_gameobject`로 AudioSource가 포함된 오브젝트를 properties 포함 조회하거나 Console error 조회
- 기대: QA 조회 또는 런타임 중 AudioSource 관련 obsolete property error가 없어야 한다.
- 실제:
  - `maxVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.`
  - `minVolume is not supported anymore. Use min-, maxDistance and rolloffMode instead.`
  - `rolloffFactor is not supported anymore. Use min-, maxDistance and rolloffMode instead.`
- 원인: MCP 오브젝트 조회가 AudioSource의 지원 중단된 프로퍼티를 직렬화하는 과정에서 발생했을 가능성이 있다.
- 수정: 미수정
- 확인:
  - [x] Console error 3개 확인
  - [ ] 실제 게임 코드에서 해당 프로퍼티를 직접 쓰는지 확인
  - [ ] MCP 조회 과정에서만 발생하는지 재확인
  - [ ] `Main Camera` AudioSource 조회 비교는 MCP timeout으로 완료하지 못함
- 관련 파일: `Assets/Script/Battle/View/BattleResultAlertView.cs`, `Assets/Script/Battle/SummonPick/SummonController.cs`, `Assets/Script/Battle/Unit/Player.cs`, `Assets/Script/Battle/Unit/Plate.cs`, `Assets/Script/Battle/View/SummonSoundView.cs`
- 메모: `rg` 기준 프로젝트 코드에서 `maxVolume`, `minVolume`, `rolloffFactor` 직접 사용은 찾지 못했다.

---

## QA-004

> status: Fixed / priority: High / feature: StorySystem / updated: 2026-06-02

- 제목: Only Mouse 옵션이 켜져도 Space 입력으로 대사가 진행됨
- 문제: Story 입력 공통 처리에서 `IsOnlyMouse` 옵션을 확인하지 않고 Space 입력을 처리한다.
- 재현: Only Mouse 옵션을 켠 뒤 Story Scene에서 Space 키를 누른다.
- 기대: Only Mouse 옵션이 켜져 있으면 Space 키로 대사가 진행되지 않아야 한다.
- 실제: `StoryScenarioControllerBase`가 Space 입력을 처리할 수 있다.
- 정적 확인: `StoryScenarioControllerBase.Update()`가 `Input.GetKeyDown(KeyCode.Space)`에서 바로 `OnClickDialogue()`를 호출하고, `IsOnlyMouse` 또는 `PlayerPrefs` 조건을 확인하지 않는다.
- 원인: `StoryScenarioControllerBase`의 입력 처리에서 `IsOnlyMouse` 옵션 확인이 빠진 것으로 보인다.
- 수정: `StoryScenarioControllerBase.Update()`에서 Space 입력 시 `IsOnlyMouse` 옵션을 확인하도록 수정
- 확인:
  - [x] StoryScenarioControllerBase Space 입력 경로 정적 확인
  - [x] `StoryScenarioControllerBase.Update()`가 Space 입력 시 `IsOnlyMouse` 옵션을 확인하는지 정적 확인
  - [x] MCP EditMode `Summoner.EditModeTests.StorySystemStaticRegressionTests` 25/25 통과
  - [x] Story 1Stage `StoryAndTalkManager`에 `Stage1_Controller`가 연결된 것을 확인
  - [x] Console error/warning 0개 확인
  - [ ] Only Mouse ON에서 Space 입력이 막히는지 확인
  - [ ] Only Mouse OFF에서 Space 입력이 유지되는지 확인
  - [ ] 클릭 입력은 기존처럼 동작하는지 확인
- 관련 파일: `Assets/Script/Story/Scenario/StoryScenarioControllerBase.cs`, `Assets/Script/Option/GameplaySettingView.cs`
- 메모: 기존 `BUG-ST-004`, `BUG-OP-002`와 연결
  - 2026-06-02 QAAgent 재검증에서 정적 테스트와 Story 1Stage 연결은 확인했다.
  - 실제 Space/클릭 입력 조작은 MCP 입력 시뮬레이션 한계로 완료하지 못해 Done 처리하지 않는다.

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
  - [x] MCP EditMode `StorySkipView_KeepsOriginalStorySceneGuid` 통과
  - [x] MCP EditMode `StorySkipView_UnityEventsUseCurrentTypeName` 통과
  - [x] MCP EditMode `StorySkipView_ClassNameMatchesFileName` 통과
  - [ ] 옵션 ON/OFF에 따른 Skip 버튼 표시 확인
  - [ ] Skip 클릭 후 씬 이동 확인
- 관련 파일: `Assets/Script/Story/View/StorySkipView.cs`, `Assets/Script/Story/Progress/StorySceneMove.cs`
- 메모: 기존 `BUG-ST-002`를 새 current QA로 옮긴 항목

---

## QA-006

> status: Fixed / priority: High / feature: OptionSystem / updated: 2026-06-02

- 제목: 오디오 볼륨 0에서 `Mathf.Log10(0)` 호출 가능
- 문제: 오디오 설정에서 볼륨 값이 0이 되면 `Mathf.Log10(0)`이 호출될 수 있다.
- 재현: 설정 창을 열고 오디오 볼륨을 0으로 낮춘 뒤 Console warning/error와 실제 오디오 값을 확인한다.
- 기대: 볼륨 0에서도 에러 없이 음소거 값으로 처리되어야 한다.
- 실제: `Mathf.Log10(0)` 호출 가능성이 있다.
- 정적 확인: `AudioSettingView.ApplyVolumes()`가 조합 볼륨을 `VolumeToDecibel()`로 넘기고, `VolumeToDecibel()`가 0 이하 값을 `-80f`로 처리한다.
- 원인: 볼륨 값을 dB로 변환하기 전에 0 또는 최소값 방어가 없다.
- 수정: 볼륨 dB 변환 전에 0 이하 값을 `-80f` 음소거 dB로 처리
- 확인:
  - [x] `AudioSettingView.ApplyVolumes()` 볼륨 0 경로 정적 확인
  - [x] DevAgent 수정 완료
  - [x] MCP `recompile_scripts(returnWithLogs: true)` 0 warning
  - [x] MCP EditMode `Summoner.EditModeTests.GameSystemStaticRegressionTests` 6/6 통과
  - [x] MCP EditMode `AudioSettingView_GuardsZeroVolumeBeforeLog10` 단일 테스트 1/1 통과
  - [x] HUD 씬 `AudioPanel`의 `AudioSettingView`, `MasterSlider`, `BGMSlider`, `SFXSlider`, `Audio.mixer` 직렬화 참조 확인
  - [ ] 볼륨 0 설정 시 Console error 없음
  - [x] 음소거 상태 정상 적용
  - [x] 볼륨을 다시 올렸을 때 정상 복구
- 관련 파일: `Assets/Script/Option/AudioSettingView.cs`
- 메모: 기존 `BUG-OP-001`을 새 current QA로 옮긴 항목
  - 2026-06-02 `AudioSettingView_GuardsZeroVolumeBeforeLog10` 단일 실행은 timeout이었지만, 같은 클래스 실행은 6/6 통과했다.
  - 2026-06-02 직후 Console error/warning 조회 가능 범위에서는 0개였다.
  - `HUD` 씬 로드와 씬/Console 추가 조회는 MCP timeout으로 완료하지 못해 실제 슬라이더 0 설정, 음소거, 복구 확인은 남긴다.
  - 2026-06-02 재시도에서 `HUD` 씬은 열려 있었고 dirty 상태는 false였다.
  - `AudioSettingView`는 `AudioPanel`에 있으며 `MasterSlider`, `BGMSlider`, `SFXSlider`, `Audio.mixer` 참조가 정적으로 연결돼 있었다.
  - `AudioPanel` 정확 경로 조회는 timeout이었다.
  - MCP `Edit/Play`는 `Connection failed: Unknown error`로 실패했고, 직후 Console에는 MCP WebSocket error 1개가 남았다.
  - 이번 error는 게임 코드 error가 아니라 MCP 연결 error로 분류한다.
  - 2026-06-02 재검증 재시도에서 MCP `recompile_scripts(returnWithLogs: true)`는 0 warning으로 통과했다.
  - 2026-06-02 MCP EditMode `Summoner.EditModeTests.GameSystemStaticRegressionTests.AudioSettingView_GuardsZeroVolumeBeforeLog10`는 1/1 통과했다.
  - 2026-06-02 Play 전 Console error 조회는 0개였고 warning 조회는 timeout이었다.
  - 2026-06-02 `HUD` 씬 로드는 성공했고 dirty 상태는 false였다.
  - 2026-06-02 `AudioPanel`은 씬 파일 기준 비활성 상태지만 `AudioSettingView`와 세 슬라이더, `Audio.mixer` 참조가 연결돼 있었다.
  - 2026-06-02 MCP `Edit/Play`는 `Connection failed: Unknown error`로 실패했고, 이후 `get_scene_info`와 Console error 조회는 queue에서 60초 만료됐다.
  - 실제 슬라이더 0 조작, 음소거, 복구는 아직 확인하지 못했으므로 QA-006은 `Fixed` 상태를 유지한다.
  - 2026-06-02 후속 재시도에서 HUD 씬 상태 조회는 한 번 성공했지만 Console error 조회, `Edit/Play`, 후속 씬 조회가 timeout으로 끝났다.
  - QA-006 실제 슬라이더 0 조작은 계속 미확인이므로 `Fixed` 상태를 유지하고, 차단 원인은 QA-003에서 관리한다.
  - 2026-06-02 사용자 수동 확인으로 볼륨 0 음소거와 볼륨 복구가 정상 동작함을 확인했다.
  - Console error/warning 자동 확인은 MCP timeout 때문에 완료하지 못했지만, 기능 동작 기준으로 QA-006은 완료 후보가 됐다.
  - 다음 정리 단계에서 QA-006을 done archive로 이동한다.

---

## QA-007

> status: Fixed / priority: Medium / feature: OptionSystem / updated: 2026-06-02

- 제목: 잘못된 비디오 저장 인덱스가 기본값으로 복구되지 않음
- 문제: 잘못된 비디오 설정 인덱스가 저장되어 있으면 모든 토글이 꺼질 수 있다.
- 재현: 비디오 설정 저장값을 잘못된 인덱스로 만든 뒤 설정 창에서 해상도 또는 화면 옵션 토글 상태를 확인한다.
- 기대: 잘못된 저장값은 기본값으로 복구되어야 한다.
- 실제: 모든 토글이 꺼질 수 있다.
- 정적 확인: `VideoSettingView.Awake()`가 `resolutionIndex`, `screenModeIndex`를 `PlayerPrefs`에서 읽은 뒤 범위 검사 없이 `(i == savedIndex)`로 토글을 켠다. 저장값이 `-1` 또는 토글 수 이상이면 선택되는 토글이 없다.
- 원인: 저장된 인덱스를 UI에 적용하기 전에 유효 범위 검사가 부족하다.
- 수정: `VideoSettingView.Awake()`에서 저장 인덱스를 토글 수 범위 안으로 보정하고, 잘못된 값은 `0`으로 다시 저장하도록 수정
- 확인:
  - [x] `VideoSettingView.Awake()` 저장 인덱스 적용 경로 정적 확인
  - [x] MCP EditMode `Summoner.EditModeTests.GameSystemStaticRegressionTests` 4/4 통과
  - [x] `VideoSettingView_RepairsInvalidSavedIndexesBeforeToggleSetup` 단일 EditMode 테스트 1/1 통과
  - [x] `HUD` 씬 `VideoPanel`에 `VideoSettingView`, 해상도 토글 3개, 화면 모드 토글 3개, AudioSource 참조가 연결된 것을 정적으로 확인
  - [ ] 잘못된 저장 인덱스가 기본값으로 복구되는지 확인
  - [ ] 토글 중 하나가 항상 선택되는지 확인
  - [ ] 정상 저장값은 유지되는지 확인
- 관련 파일: `Assets/Script/Option/VideoSettingView.cs`
- 메모: 기존 `BUG-OP-004`를 새 current QA로 옮긴 항목
  - 2026-06-02 QAAgent 재검증에서 코드와 HUD 씬 연결 기준 복구 경로는 확인했다.
  - Console error 조회와 실제 옵션 UI 조작은 MCP timeout으로 완료하지 못해 Done 처리하지 않는다.

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
  - 2026-06-02 단일 PlayMode `Summoner.PlayModeTests.GameSaveProgressReadTests.GetGameSaveOrDefault_LoadsPlayerPrefs_WhenControllerIsMissing` 재시도는 timeout으로 완료되지 않았다. 직후 Console error/warning은 0개였다.
