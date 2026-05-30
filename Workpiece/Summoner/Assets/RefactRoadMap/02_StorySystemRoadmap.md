# Story System Refactoring Roadmap

## Current Phase

Phase 2 - StorySystem responsibility split.

## Goal

CSV 대사 로드, 대사 진행, 입력 처리, Stage별 연출, 스토리 종료 후 씬 이동을 서로 구분한다.
`InteractionController`에 몰린 책임을 기능 단위로 줄인다.

## Scope

```text
Assets/Script/Story/Dialogue
Assets/Script/Story/Progress
Assets/Script/Story/StageScenarioController
Assets/Script/Story/StoryImageView.cs
Assets/Script/Story/FadePanelView.cs
```

## Refactoring Rules

- CSV 파싱, 대사 진행, Stage 연출, 씬 이동을 한 번에 섞어 수정하지 않는다.
- Stage별 Controller는 하나를 먼저 정리하고 테스트 코드/QA 요청을 남긴 뒤 반복 적용한다.
- View 이름 변경만으로 책임이 명확해지지 않는 파일은 먼저 책임을 분리한다.
- 상세 변경 기록은 이 문서에 누적하지 않고 `WorkLogs/YYYY-MM-DD.md`에 기록한다.

## Current Flow

```text
DatabaseManager
 -> DialogueParser.Parse(csv_FileName)
 -> dialogueDic에 Dialogue 저장

InteractionEvent
 -> StoryStage.checkStage()
 -> DatabaseManager.instance.getDialogue(x, y)

InteractionController
 -> 대사 인덱스 관리
 -> Text UI 출력
 -> FadeOut
 -> StorySceneMove로 다음 씬 이동

Stage*_Controller
 -> Space/Click 입력 처리
 -> InteractionController.ShowNextLine()
 -> scenarioFlowCount 기준 연출 실행
```

## Target Flow

```text
CSV 로드
 -> CSV 파싱
 -> 현재 storyNum 대사 범위 선택
 -> 대사 진행
 -> 화면 출력
 -> Stage별 연출 실행
 -> 스토리 종료 후 다음 씬 이동
```

## Active Tasks

| ID | Task | Status | Notes |
|---|---|---|---|
| ST-02 | InteractionController 대사 진행 책임 분리 | Completed | `StoryProgressAdvance` 연결됨 |
| ST-03 | Text UI 출력 책임 분리 | Completed | `DialogueLineShow` 연결됨 |
| ST-04 | DialogueParser 로드/파싱 분리 | Completed | `DialogueCsvParse`, `DialogueDatabaseLoad` 연결됨 |
| ST-05 | Stage1 입력 공통화 | Pending QA | QA 에이전트가 실행 검증 |
| ST-06 | Stage2 입력 공통화 | Pending QA | QA 에이전트가 실행 검증 |
| ST-07 | Stage3 입력 공통화 | Pending QA | QA 에이전트가 실행 검증 |
| ST-08 | Stage5 입력 공통화 | Pending QA | QA 에이전트가 실행 검증 |
| ST-09 | Stage7 입력 공통화 | Pending QA | QA 에이전트가 실행 검증 |
| ST-QA-00 | MCP QA 실행 복구 | Highest | 수동 QA 대신 MCP `run_tests`/`recompile_scripts` timeout 원인 수정 |
| ST-QA-01 | StorySystem 정적 회귀 테스트 실행 요청 | Pending QA | QA 에이전트 담당 |

## Completed

완료 상세는 `CompletedArchive.md`와 `WorkLogs/2026-05-30.md`에 보관한다.

요약:

- `StoryImageView`, `FadePanelView`, `MainSceneButtonView`, `StorySkipView` 이름 정리 완료.
- `StorySceneMove`로 스토리 종료 후 다음 씬 결정 책임 분리 완료.
- `DialogueLineShow`로 대사 Text UI 출력 책임 분리 완료.
- `StoryProgressAdvance`로 Dialogue/Line 인덱스 진행 책임 분리 완료.
- `DialogueCsvParse`, `DialogueDatabaseLoad`로 CSV 로드와 파싱 책임 분리 완료.
- Story View 이름과 GUID 유지 검사용 정적 회귀 테스트 추가.

## Done Criteria

```text
1. Prologue Screen에서 첫 대사가 정상 출력되는지 확인
2. 클릭과 Space 입력으로 다음 대사가 진행되는지 확인
3. Stage별 이동/이미지/사운드 연출이 기존과 같은지 확인
4. 스토리 종료 후 Fight Screen_NStage로 이동하는지 확인
5. Epilogue는 Thank Screen으로 이동하는지 확인
6. Story Skip 버튼이 설정값에 따라 표시되는지 확인
7. 관련 QAReports에 QA 요청 또는 결과 기록
8. FileStateIndex 갱신
```

## Latest Summary

2026-05-30: 단순 Story View 이름 변경 후보, 스토리 종료 씬 이동, 대사 Text UI 출력, Dialogue/Line 인덱스 진행, DialogueParser 로드/파싱 분리는 완료했다.
Stage1, Stage2, Stage3, Stage5, Stage7 입력 공통화는 적용됐고 QA 요청 상태로 넘겼다.
다음 작업 최우선순위는 수동 QA가 아니라 MCP `run_tests`/`recompile_scripts` timeout 원인 수정과 StorySystem EditMode 재실행이다.
