# QAReports

> 이 문서는 현재 QA 작업표가 아니다.
> 현재 할 일은 `Assets/RefactRoadMap/qa/current.md` 하나만 본다.
> 이 문서는 기능별 QAReports를 찾기 위한 보관용 인덱스다.

> 새 QA 운영 기준은 `Assets/RefactRoadMap/qa` 폴더를 사용한다.
> `qa/current.md`에는 현재 수정 중이거나 다시 확인해야 하는 항목만 남긴다.
> 전체 회귀 확인은 `qa/full-check.md`, 완료된 QA는 `qa/done/YYYY-MM.md`에 둔다.
> 이 `QAReports` 폴더의 md/csv는 기능별 QA 종합보고서와 이전 상세 근거 보관용으로 유지한다.

현재 작업 큐는 `qa/current.md`다.
전체 회귀 확인은 `qa/full-check.md`, 완료된 QA는 `qa/done/YYYY-MM.md`에 둔다.
기존 CSV 파일은 이전 기록 보관용으로 유지한다.
현재 할 일과 상태 전환은 `Assets/RefactRoadMap/qa` 폴더를 기준으로 관리한다.
이 폴더는 현재 큐가 아니라 기능별 QA 결과를 한 번에 보는 종합보고서다.

## 1. 문서 목록

| 문서 | 범위 | 사용 목적 |
|---|---|---|
| 01_GameSystem_QA.md | 시작 화면, 저장, 이어하기, 스테이지 이동 | 게임 시작과 진행 저장 확인 |
| 02_StorySystem_QA.md | CSV 대사, 대사 진행, 스토리 연출, 씬 이동 | 스토리 흐름 확인 |
| 03_BattleCore_QA.md | 턴, 마나, 공격, 상태이상, 승패 판정 | 전투 핵심 규칙 확인 |
| 04_BattleContent_QA.md | 소환수 뽑기, 배치, 공격 전략, 적 AI | 전투 콘텐츠 동작 확인 |
| 05_OptionSystem_QA.md | 오디오, 비디오, 게임플레이 옵션, 설정 저장 | 설정값 적용과 재시작 후 유지 확인 |
| 06_SceneObject_QA.md | 씬 Hierarchy, UI 그룹, missing script | SceneObject 정리 회귀 확인 |

## 2. 운영 규칙

- 새 QA 항목은 `qa/current.md`에만 작성한다.
- 완료된 QA는 `qa/done/YYYY-MM.md`로 옮긴다.
- 큰 변경 후 확인 목록은 `qa/full-check.md`를 사용한다.
- `QAReports`는 기능별로 QA 범위, 실행 결과, 발견 버그, 남은 위험을 요약하는 종합보고서로 사용한다.
- `QAReports`는 큰 QA 정리 시점에 요약 갱신할 수 있지만, 오늘 처리할 큐나 상태 전환은 관리하지 않는다.

## 3. 이전 우선순위 스냅샷

아래 표는 이전 QAReports 기준의 스냅샷이다.
현재 우선순위는 `qa/current.md`에서 관리한다.

| 순서 | 우선순위 | ID | 문서 | 상태 | 다음 확인 |
|---|---|---|---|---|---|
| 1 | 최우선 | QA-MCP-00 | 01_GameSystem_QA.md; 02_StorySystem_QA.md; 05_OptionSystem_QA.md; 06_SceneObject_QA.md | Partial Pass/Blocked | MCP `recompile_scripts` warning 0, EditMode 관련 테스트 통과. PlayMode `run_tests`는 timeout 또는 `Connection failed: Unknown error` |
| 2 | 높음 | BUG-BC-001 | 03_BattleCore_QA.md | Fixed/Blocked | `recompile_scripts` warning 0, `StageSceneConnectionEditModeTests` 5/5 통과. PlayMode `BattleResultAlertView_FightScene_HasRequiredRuntimeControllers`는 timeout, Console 로그 조회도 timeout. `BUG-BC-002` 수정 후 런타임 재검증 필요 |
| 2.5 | 높음 | BUG-BC-002 | 03_BattleCore_QA.md | Open Fail | Fight 1Stage PlayMode 재시도 중 `Summon.Update()` line 77 `NullReferenceException` 발생 |
| 3 | 높음 | BUG-GS-002 | 01_GameSystem_QA.md | Open Blocked | MCP PlayMode `GameSaveProgressReadTests`는 timeout, `GameSaveContinueTests`는 `Connection failed: Unknown error` |
| 4 | 높음 | BUG-ST-001 | 02_StorySystem_QA.md | Closed | `StorySystemStaticRegressionTests` 25/25 통과로 Stage 입력 공통화 EditMode 재검증 완료 |
| 5 | 높음 | BUG-ST-003 | 02_StorySystem_QA.md | Open Blocked | MCP PlayMode `StageRuntimeFlowPlayModeTests`가 timeout으로 실행 불가 |
| 6 | 높음 | BUG-OP-003 | 05_OptionSystem_QA.md | Open Blocked | MCP PlayMode `GameSaveProgressReadTests`가 timeout으로 옵션 유지 자동 테스트 실행 불가 |
| 7 | 높음 | BUG-GS-003 | 01_GameSystem_QA.md | Open | `savedStage` 8 이상 표시/버튼은 정적 방어 확인, `playingStage > savedStage` 런타임 흐름은 미확인 |
| 8 | 높음 | BUG-ST-002 | 02_StorySystem_QA.md | Open Runtime QA | Story Skip 옵션은 `StorySkipView`에 정적 연결 확인, 실제 Story 진행 흐름은 런타임 미확인 |
| 9 | 높음 | BUG-ST-004 | 02_StorySystem_QA.md | Open Fail | `StoryScenarioControllerBase`가 `IsOnlyMouse`를 확인하지 않고 Space 입력을 처리함 |
| 10 | 높음 | BUG-OP-001 | 05_OptionSystem_QA.md | Open Fail | `AudioSettingView`가 볼륨 0에서 `Mathf.Log10(0)`을 호출할 수 있음 |
| 11 | 높음 | BUG-OP-002 | 05_OptionSystem_QA.md | Open Fail | Story Skip은 정적 연결, Only Mouse는 Story 입력 처리에 미반영 |
| 12 | 중간 | BUG-OP-004 | 05_OptionSystem_QA.md | Open Fail | 잘못된 비디오 저장 인덱스가 기본값으로 복구되지 않고 모든 토글이 꺼질 수 있음 |
| 13 | 높음 | GS-QA | 01_GameSystem_QA.md | Pending QA | 이어하기, 스테이지 잠금/해금, 재시작 후 이어하기 확인 |
| 14 | 높음 | ST-QA | 02_StorySystem_QA.md | Pending QA | 스테이지별 스토리 진입 조건과 종료 후 전투 이동 확인 |
| 15 | 높음 | OP-QA | 05_OptionSystem_QA.md | Pending QA | 옵션 변경 후 씬 이동/재시작 유지 확인 |
| 16 | 중간 | BC-QA | 03_BattleCore_QA.md | Pending QA | 턴 시작, 턴 종료, 공격 흐름 확인 |
| 17 | 중간 | BT-QA | 04_BattleContent_QA.md | Pending QA | 소환 후보 표시와 배치 확인 |
| 18 | 중간 | SO-QA | 06_SceneObject_QA.md | Partial Pass/Fail | HUD/Fight 1Stage 구조 QA는 Pass, Fight 1Stage 런타임은 `BUG-BC-002`로 Fail |
