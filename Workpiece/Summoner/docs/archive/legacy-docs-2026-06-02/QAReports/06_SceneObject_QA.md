# SceneObject QA

## 1. 현재 상태 요약

| 항목 | 내용 |
|---|---|
| QA 범위 | `HUD.unity`, `Fight Screen_1Stage.unity` 1차 Hierarchy 정리 |
| 현재 상태 | EditMode 구조 QA 통과, PlayMode 런타임 QA는 MCP 연결 문제로 차단 |
| 마지막 정리일 | 2026-05-31 |
| 다음 확인 | Unity Editor에서 HUD 메뉴/설정/알림, Fight 1Stage 버튼/Alert 수동 Play 확인 |

## 2. QA 체크 항목

| ID | 기능 | 확인 내용 | 상태 |
|---|---|---|---|
| SO-001 | HUD 구조 | `MenuCanvas` 하위가 번호형 UI 그룹으로 정리되어 있는가 | Pass |
| SO-002 | Fight 1Stage 구조 | `BattleCanvas` 하위가 번호형 UI 그룹으로 정리되어 있는가 | Pass |
| SO-003 | Missing Script | `HUD`와 `Fight Screen_1Stage`에 missing script가 없는가 | Pass |
| SO-004 | 코드 경로 호환 | `StartScreenView.openOption`이 새 설정 패널 경로를 찾는가 | Pass |
| SO-005 | HUD 런타임 | ESC 메뉴, 설정, Alert, 클릭음이 실제 Play에서 동작하는가 | Blocked |
| SO-006 | Fight 1Stage 런타임 | 소환, 재소환, 턴 종료, Clear/Fail Alert가 실제 Play에서 동작하는가 | Fail |
| SO-007 | 적용 범위 | `Fight Screen_2Stage`~`7Stage`가 아직 구조 변경되지 않았는가 | Pass |

## 3. 상세 QA 기록

### SO-001. HUD 구조

- 확인 방법: `Summoner.EditModeTests.StageSceneConnectionEditModeTests.HudScene_HasOrganizedRootGroups`
- 기대 결과: `__UI/MenuCanvas/UI_00_Background`, `UI_10_Menu`, `UI_20_Settings`, `UI_90_Alerts`가 존재한다
- 현재 결과: 테스트 통과
- 상태: Pass

### SO-002. Fight 1Stage 구조

- 확인 방법: `Summoner.EditModeTests.StageSceneConnectionEditModeTests.FightSceneOne_HasOrganizedUiGroups`
- 기대 결과: `BattleCanvas` 하위에 `UI_00_Background`, `UI_10_BattleField`, `UI_20_TurnStatus`, `UI_30_PlayerCommands`, `UI_40_Mana`, `UI_50_SelectedUnitState`, `UI_60_SummonPicker`, `UI_90_ResultAlerts`가 존재한다
- 현재 결과: 테스트 통과
- 상태: Pass

### SO-003. Missing Script

- 확인 방법: `AssertNoMissingScriptsInOpenScene`
- 기대 결과: 확인 대상 씬의 모든 GameObject missing script count가 0이다
- 현재 결과: 테스트 통과. `HUD/MenuCanvas`의 비활성 레거시 `MenuView` missing script 참조는 제거 완료
- 상태: Pass

### SO-004. 코드 경로 호환

- 확인 방법: `StartScreenView.openOption` 코드 확인 및 `recompile_scripts`
- 기대 결과: 새 경로 `UI_20_Settings/Setting/SettingPanel`을 먼저 찾고 기존 `Setting/SettingPanel`도 fallback으로 유지한다
- 현재 결과: Unity MCP `recompile_scripts` warning 0
- 상태: Pass

### SO-005. HUD 런타임

- 확인 방법: Unity Play 수동 QA 또는 PlayMode 자동 QA
- 기대 결과: ESC 메뉴, 설정창, 메인 이동 Alert, 종료 Alert, Skip Alert, 클릭음/알림음이 동작한다
- 현재 결과: MCP PlayMode `run_tests`가 timeout 또는 `Connection failed: Unknown error`로 완료되지 않음. 2026-05-31 재시도에서도 단일 전투 테스트는 `Connection failed`, 단순 Start Screen PlayMode 테스트는 timeout
- 상태: Blocked
- 다음 조치: 열린 Unity Editor에서 수동 Play 확인

### SO-006. Fight 1Stage 런타임

- 확인 방법: Unity Play 수동 QA 또는 `StageRuntimeFlowPlayModeTests.BattleResultAlertView_FightScene_HasRequiredRuntimeControllers`
- 기대 결과: 1Stage 전투 진입, 소환, 재소환, 턴 종료, Clear/Fail Alert가 동작한다
- 현재 결과: MCP PlayMode 단일 테스트가 timeout 후 재시도에서 `Connection failed: Unknown error`. 2026-05-31 재시도 중 Unity Console에서 `Summon.Update()` line 77 `NullReferenceException` 확인
- 상태: Fail
- 다음 조치: `BUG-BC-002` 수정 후 Fight 1Stage PlayMode 재검증

### SO-007. 적용 범위 확인

- 확인 방법: 작업 범위와 테스트 경로 확인
- 기대 결과: 이번 1차 적용은 `HUD.unity`, `Fight Screen_1Stage.unity`만 구조 변경한다
- 현재 결과: `Fight Screen_2Stage`~`7Stage`, Start, Stage Select, Story 씬 구조 변경 없음
- 상태: Pass

## 4. 발견한 버그

| ID | 날짜 | 내용 | 상태 |
|---|---|---|---|
| BUG-SO-001 | 2026-05-31 | MCP PlayMode `run_tests`가 timeout 또는 `Connection failed: Unknown error`로 1차 SceneObject 런타임 QA를 완료하지 못함 | Open Blocked |
| BUG-BC-002 | 2026-05-31 | Fight 1Stage PlayMode 재시도 중 `Summon.Update()` line 77 `NullReferenceException` 발생. 상세 Dev Fix Request는 `03_BattleCore_QA.md`에 기록 | Open Fail |

## 5. QA 히스토리

| 날짜 | 확인 범위 | 결과 | 메모 |
|---|---|---|---|
| 2026-05-31 | SceneObject 1차 EditMode 구조 QA | 통과 | `recompile_scripts` warning 0, `StageSceneConnectionEditModeTests` 5/5 통과 |
| 2026-05-31 | SceneObject 1차 PlayMode 런타임 QA | 차단 | PlayMode 단일 테스트가 timeout 또는 `Connection failed: Unknown error` |
| 2026-05-31 | SceneObject PlayMode 런타임 QA 재시도 | 차단 | 전투 단일 테스트는 `Connection failed`, Start Screen 단일 PlayMode 테스트는 timeout. EditMode 구조 테스트는 5/5 재통과 |
| 2026-05-31 | SceneObject PlayMode 재시도 추가 확인 | 실패 | 전투 테스트 시도 후 Unity Console에서 `Summon.Update()` line 77 NRE 확인 |
