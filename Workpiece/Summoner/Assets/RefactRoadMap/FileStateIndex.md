# Refactoring File State Index

## 1. Purpose

이 문서는 리팩토링 중 원본 코드, 새 코드, 임시 연결 코드, 삭제 예정 코드를 구분하기 위한 기준 문서다.
세션을 다시 시작해도 어떤 파일이 실제 게임 흐름에서 사용 중인지 바로 알 수 있게 유지한다.

## 2. State Values

```text
Original
- 기존 원본 코드
- 아직 실제 게임 흐름에서 사용 중

New
- 새로 만든 리팩토링 코드
- 아직 연결 전이거나 일부만 연결됨

Bridge
- 기존 코드와 새 코드를 이어주는 중간 코드
- 리팩토링 완료 후 줄이거나 삭제 예정

Deprecated
- 더 이상 사용하지 않는 기존 코드
- 삭제 후보지만 아직 참조 확인 필요

Removed
- 삭제 완료
- 삭제 이유와 대체 파일을 기록
```

기존 코드 일부 교체와 새 컴포넌트 생성 후 연결 이전은 모두 정상적인 리팩토링 방식이다.
실제 연결 상태가 바뀌면 이 문서의 표만 갱신하고, 상세 작업 로그는 `WorkLogs/YYYY-MM-DD.md`에 남긴다.
작업명이나 티켓 ID를 붙인 별도 WorkLog 파일은 만들지 않는다.

## 3. File State Table

| 파일 | 상태 | 역할 | 연결된 기능 | 대체 대상 | 다음 처리 | 마지막 확인일 |
|---|---|---|---|---|---|---|
| Assets/Script/Save/GameSaveController.cs | New | 저장 기능의 새 진입점 | 새 게임, 이어하기, 스테이지 선택, 전투 클리어, 진행도 읽기 | PlayerPrefs 직접 접근 | QA 에이전트 기능별 수동 QA 요청 | 2026-05-30 |
| Assets/Script/Save/GameSaveData.cs | New | 저장 데이터 구조 | savedStage, playingStage | 개별 int 저장 흐름 | 필드 추가 시 저장소와 함께 수정 | 2026-05-30 |
| Assets/Script/Save/PlayerPrefsSaveStore.cs | New | PlayerPrefs 저장소 | 진행 저장 로드/저장/초기화 | PlayerPrefs 직접 접근 | 기존 저장 키와 호환성 확인 | 2026-05-30 |
| Assets/Script/Start Screen/StartScreenView.cs | Bridge | 시작 화면 새 게임/이어하기/설정 패널 열기 | 새 게임 시작, 이어하기, HUD 설정 패널 경로 fallback | GameSaveController | QA 에이전트 시작 화면 QA 요청 | 2026-05-31 |
| Assets/Script/Stage/StageSelectView.cs | Bridge | 스테이지 선택과 playingStage 저장 | 스테이지 선택 저장 | GameSaveController.SavePlayingStage | QA 에이전트 스테이지 선택 QA 요청 | 2026-05-30 |
| Assets/Script/Battle/BattleResultAlertView.cs | Bridge | 전투 결과 처리와 다음 스테이지 저장 | 전투 클리어 저장 | GameSaveController.SaveClearedStage | QA 에이전트 전투 클리어 QA 요청 | 2026-05-30 |
| Assets/Tests/PlayMode/PlayModeTests.asmdef | New | PlayMode 테스트 어셈블리 정의 | 이어하기 자동 QA | 수동 QA | QA 에이전트가 Unity Test Runner 인식 확인 | 2026-05-30 |
| Assets/Tests/PlayMode/GameSaveContinueTests.cs | New | 이어하기 PlayMode 테스트 | 이어하기 | 수동 QA | QA 에이전트가 MCP 또는 Unity Test Runner로 실행 | 2026-05-30 |
| Assets/Tests/PlayMode/GameSaveProgressReadTests.cs | New | 진행도 읽기 PlayMode 테스트 | GetGameSaveOrDefault, StageTextView, StageController | 수동 QA | QA 에이전트가 실행 | 2026-05-30 |
| Assets/Tests/EditMode/Editor/GameSystemStaticRegressionTests.cs | New | GameSystem 정적 회귀 테스트 | PlayerPrefs 접근 위치, 진행도 키 직접 접근, LoadStage 재등장 | 수동 rg 검사 | QA 에이전트가 EditMode 실행 | 2026-05-30 |
| Assets/Script/Story/StoryImageView.cs | New | 스토리 이미지 표시 UI | Prologue/Epilogue 대사 이미지 표시 | StoryChangeImage | QA 에이전트 Prologue/Epilogue QA 요청 | 2026-05-30 |
| Assets/Script/Story/FadePanelView.cs | New | 스토리 Fade 패널 표시 UI | Story Fade In/Out 표시 | FadeController | QA 에이전트 스토리 씬 QA 요청 | 2026-05-30 |
| Assets/Script/Story/Dialogue/MainSceneButtonView.cs | New | 메인 씬 이동 버튼 UI | Thank Screen에서 Start Screen 이동 | ClicktoMain | EditMode 정적 테스트 통과 | 2026-05-30 |
| Assets/Script/Story/StageScenarioController/StorySkipView.cs | New | 스토리 스킵 버튼 UI | Story Scene 스킵 버튼 표시, 스킵 Alert, 전투 씬 이동 | Story | EditMode 정적 테스트 통과 | 2026-05-30 |
| Assets/Script/Story/Progress/StorySceneMove.cs | New | 스토리 종료 후 다음 씬 결정 | Prologue 종료, Story 종료, Epilogue 종료 | InteractionController의 씬 이동 분기 | EditMode 정적 테스트 통과 | 2026-05-30 |
| Assets/Script/Story/Dialogue/DialogueLineShow.cs | New | 대사 Text UI 출력 | 캐릭터 이름 표시, 대사 본문 줄 결합 | InteractionController의 UI 문자열 조립 | EditMode 정적 테스트 통과 | 2026-05-30 |
| Assets/Script/Story/Progress/StoryProgressAdvance.cs | New | 대사 진행 인덱스 관리 | Dialogue 인덱스, Line 인덱스 진행 | InteractionController의 진행 상태 관리 | EditMode 정적 테스트 통과 | 2026-05-30 |
| Assets/Script/Story/Dialogue/DialogueDatabaseLoad.cs | New | CSV TextAsset 로드 | Story CSV 리소스 로드 | DialogueParser의 Resources.Load | EditMode 정적 테스트 통과 | 2026-05-30 |
| Assets/Script/Story/Dialogue/DialogueCsvParse.cs | New | CSV 문자열 파싱 | Dialogue 배열 생성, 대사 줄 결합 준비 | DialogueParser의 CSV 파싱 | EditMode 정적 테스트 통과 | 2026-05-30 |
| Assets/Script/Story/StageScenarioController/StoryScenarioControllerBase.cs | New | Stage별 공통 입력 흐름 | Space 입력, Pointer 클릭, 대사 ID 중복 방지 | Stage*_Controller 중복 입력 코드 | QA 에이전트 테스트 실행 요청 | 2026-05-30 |
| Assets/Script/Story/StageScenarioController/Stage1_Controller.cs | Bridge | Stage1 전용 연출 실행 | Stage1 이동/이미지/대사 박스 연출 | StoryScenarioControllerBase | QA 에이전트 실행 QA 요청 | 2026-05-30 |
| Assets/Script/Story/StageScenarioController/Stage2_Controller.cs | Bridge | Stage2 전용 연출 실행 | Stage2 이미지/대사 박스/Yellow 연출 | StoryScenarioControllerBase | QA 에이전트 실행 QA 요청 | 2026-05-30 |
| Assets/Script/Story/StageScenarioController/Stage3_Controller.cs | Bridge | Stage3 전용 연출 실행 | Stage3 이동, Fox 등장, Angry 연출 | StoryScenarioControllerBase | QA 에이전트 실행 QA 요청 | 2026-05-30 |
| Assets/Script/Story/StageScenarioController/Stage5_Controller.cs | Bridge | Stage5 전용 연출 실행 | Stage5 이동, Bang 효과음, 적 등장 연출 | StoryScenarioControllerBase | QA 에이전트 실행 QA 요청 | 2026-05-30 |
| Assets/Script/Story/StageScenarioController/Stage7_Controller.cs | Bridge | Stage7 전용 연출 실행 | Stage7 이동, 대사 박스 연출 | StoryScenarioControllerBase | QA 에이전트 실행 QA 요청 | 2026-05-30 |
| Assets/Tests/EditMode/Editor/StorySystemStaticRegressionTests.cs | New | StorySystem 정적 회귀 테스트 | Story View 이름, GUID, 씬 이동, 대사 출력/진행, CSV 로드/파싱, Stage 입력 공통화 | 수동 rg 검사 | QA 에이전트 EditMode 실행 요청 | 2026-05-30 |
| Assets/Screen/HUD.unity | Bridge | HUD 씬 루트/번호형 UI 그룹 정리 1차 적용 | 메뉴 UI, 설정 UI, 알림 UI, 메뉴 오디오, 메뉴 핸들러 | 구분선 루트 오브젝트, 비활성 레거시 MenuView 컴포넌트 | 수동 UI 위치/버튼 QA | 2026-05-31 |
| Assets/Screen/FightScene/Fight Screen_1Stage.unity | Bridge | 전투 1스테이지 씬 루트/번호형 전투 UI 그룹 정리 1차 적용 | BattleCanvas, EventSystem, GameSaveController, Main Camera | 평면 BattleCanvas 하위 구조 | 수동 전투 UI 위치/버튼 QA 후 2~7Stage 반복 적용 판단 | 2026-05-31 |
| Assets/Tests/EditMode/Editor/StageSceneConnectionEditModeTests.cs | New | GameSystem/Scene 연결 및 Scene Object 구조 회귀 테스트 | Stage 씬 연결, HUD 번호형 구조, Fight 1Stage 번호형 구조 | 수동 hierarchy 확인 | MCP EditMode 5/5 통과 | 2026-05-31 |
| Assets/RefactRoadMap/QAReports/06_SceneObject_QA.md | Report | SceneObject 1차 QA 종합보고서 | HUD/Fight 1Stage 구조 QA, PlayMode 차단 기록 | 분산된 WorkLog 기록 | 현재 큐는 qa/00_current.md에서 관리 | 2026-05-31 |
| Assets/RefactRoadMap/QAReports/06_SceneObject_QA.csv | Report | SceneObject 1차 QA 표 데이터 | SO-001~SO-007 상태 | md 리포트 수동 집계 | 현재 큐는 qa/00_current.md에서 관리 | 2026-05-31 |
| Assets/RefactRoadMap/00_SessionHandoff.md | New | 세션 인계 문서 | 이전 세션 요약, 다음 세션 시작 절차 | 00_RefactoringOverview 역할 혼재 | 세션 재시작 시 먼저 확인 | 2026-05-31 |
| Assets/RefactRoadMap/Agents/DevQAWorkflow.md | New | DevAgent/QAAgent 분리 운영 규칙 | 역할 고정, QA 상태와 담당 단계 전환, QA 자동화 경로, Dev Fix Request 형식 | 역할 혼재 | 세션 시작 시 역할 분리 기준으로 확인 | 2026-05-31 |
| Assets/RefactRoadMap/Agents/DevAgent.md | New | 개발 에이전트 운영 규칙 | 개발, 테스트 추가, QA 요청, Fixed 전달 | QAAgent 단독 문서 | DevAgent 세션 시작 시 확인 | 2026-05-31 |
| Assets/RefactRoadMap/Agents/QAAgent.md | Bridge | QA 에이전트 운영 규칙 | MCP QA 실행, Fail/Blocked/Closed 기록, Dev Fix Request 작성 | DevQAWorkflow | QAAgent 세션 시작 시 확인 | 2026-05-31 |
| Assets/RefactRoadMap/AutoRefactorMode.md | Removed | 제거됨 | 핵심 규칙은 00_SessionHandoff Working Rules로 흡수 | 별도 자동 모드 문서 | 사용 안 함 | 2026-05-31 |
| Assets/RefactRoadMap/WorkLogs/2026-05-30.md | New | 날짜별 상세 작업 로그 | 로드맵에서 분리한 작업 기록 | Roadmap 변경 기록 누적 | 날짜별로 추가 | 2026-05-30 |
| Assets/RefactRoadMap/RoadmapTemplate.md | New | 기능별 Roadmap 공통 템플릿 | Roadmap을 설계/방향 문서로 고정 | QA 상태와 작업 큐 혼재 | 새 Roadmap 작성 시 이 구조 사용 | 2026-05-31 |
| Assets/RefactRoadMap/CompletedArchive.md | Removed | 제거됨 | 완료 요약은 기능별 로드맵과 00_SessionHandoff로 흡수 | 별도 완료 아카이브 | 사용 안 함 | 2026-05-31 |

## 4. Feature Connection State

| 기능 | 현재 사용 코드 | 새 코드 | 연결 상태 | QA 상태 | 마지막 확인일 |
|---|---|---|---|---|---|
| 새 게임 시작 저장 | StartScreenView.NewStart | GameSaveController.StartNewGame | 연결됨 | 수동 QA 필요 | 2026-05-30 |
| 이어하기 | StartScreenView.StartSavedStage | GameSaveController.GetGameSave | 연결됨 | PlayMode 테스트 코드 추가, QA 에이전트 실행 요청 | 2026-05-30 |
| 스테이지 선택 저장 | StageSelectView.stageLoader | GameSaveController.SavePlayingStage | 연결됨 | 수동 QA 필요 | 2026-05-30 |
| 전투 클리어 저장 | BattleResultAlertView.clearAlert | GameSaveController.SaveClearedStage | 연결됨 | 수동 QA 필요 | 2026-05-30 |
| 진행도 읽기 | StageTextView, StageController, StorySkipView, Player | GameSaveController.GetGameSaveOrDefault | 연결됨 | 수동 QA 필요 | 2026-05-30 |
| 스토리 종료 씬 이동 | InteractionController.EndDialogue | StorySceneMove.GetNextSceneName, StorySceneMove.LoadNextScene | 연결됨 | EditMode 정적 테스트 통과 | 2026-05-30 |
| 대사 Text UI 출력 | InteractionController.ShowNextLine | DialogueLineShow.Show, DialogueLineShow.BuildCombinedDialogue | 연결됨 | EditMode 정적 테스트 통과 | 2026-05-30 |
| 대사 진행 | InteractionController.StartDialogue, InteractionController.ShowNextLine | StoryProgressAdvance.Start, StoryProgressAdvance.TryGetNextLine | 연결됨 | EditMode 정적 테스트 통과 | 2026-05-30 |
| CSV 대사 로드와 파싱 | DialogueParser.Parse | DialogueDatabaseLoad.LoadText, DialogueCsvParse.Parse | 연결됨 | EditMode 정적 테스트 통과 | 2026-05-30 |
| Stage1 입력 공통화 | Stage1_Controller | StoryScenarioControllerBase | 연결됨 | 정적 확인, QA 에이전트 실행 요청 | 2026-05-30 |
| Stage2 입력 공통화 | Stage2_Controller | StoryScenarioControllerBase | 연결됨 | 정적 확인, QA 에이전트 실행 요청 | 2026-05-30 |
| Stage3 입력 공통화 | Stage3_Controller | StoryScenarioControllerBase | 연결됨 | 정적 확인, QA 에이전트 실행 요청 | 2026-05-30 |
| Stage5 입력 공통화 | Stage5_Controller | StoryScenarioControllerBase | 연결됨 | 정적 확인, QA 에이전트 실행 요청 | 2026-05-30 |
| Stage7 입력 공통화 | Stage7_Controller | StoryScenarioControllerBase | 연결됨 | 정적 확인, QA 에이전트 실행 요청 | 2026-05-30 |

## 5. UI View Rename State

| 기존 파일 | 현재 파일 | 상태 | 역할 | 다음 처리 |
|---|---|---|---|---|
| Assets/Script/Battle/BattleAlert.cs | Assets/Script/Battle/BattleResultAlertView.cs | New | 전투 결과 Alert 표시 | QA 에이전트 씬 QA 요청 |
| Assets/Script/Battle/SummonPick/DrawOptionPanelView.cs | Assets/Script/Battle/SummonPick/DrawOptionPanelView.cs | New | 소환수 Draw 옵션 패널 표시/클릭 | 선택 패널 동작 확인 |
| Assets/Script/Battle/BattleLogic/StatePanel.cs | Assets/Script/Battle/BattleLogic/SummonStatePanelView.cs | New | 소환수 상태 패널 표시 | HP/Shield/공격 버튼 표시 확인 |
| Assets/Script/Menu/Menu.cs | Assets/Script/Menu/MenuView.cs | New | ESC 메뉴 UI | QA 에이전트 씬 QA 요청 |
| Assets/Script/Option/Alert.cs | Assets/Script/Option/ConfirmAlertView.cs | New | Yes/No 확인 Alert UI | QA 에이전트 씬 QA 요청 |
| Assets/Script/Option/AudioController.cs | Assets/Script/Option/AudioSettingView.cs | New | 오디오 설정 UI | Inspector 참조명 추후 정리 |
| Assets/Script/Option/GamePlayController.cs | Assets/Script/Option/GameplaySettingView.cs | New | 게임플레이 설정 UI | QA 에이전트 씬 QA 요청 |
| Assets/Script/Option/Setting.cs | Assets/Script/Option/SettingPanelView.cs | New | 설정 패널 전환 UI | QA 에이전트 씬 QA 요청 |
| Assets/Script/Option/VideoController.cs | Assets/Script/Option/VideoSettingView.cs | New | 비디오 설정 UI | Inspector 참조명 추후 정리 |
| Assets/Script/Stage/StageSelecter.cs | Assets/Script/Stage/StageSelectView.cs | New | 스테이지 선택 버튼 UI | QA 에이전트 씬 QA 요청 |
| Assets/Script/Stage/StageText.cs | Assets/Script/Stage/StageTextView.cs | New | 시작 화면 스테이지 텍스트 UI | 시작 화면 표시 확인 |
| Assets/Script/Start Screen/StartScreenEvent.cs | Assets/Script/Start Screen/StartScreenView.cs | New | 시작 화면 버튼/Alert UI | 시작/이어하기/설정 버튼 확인 |
| Assets/Script/Story/StoryChangeImage.cs | Assets/Script/Story/StoryImageView.cs | New | 스토리 이미지 표시 UI | Prologue/Epilogue 이미지 표시 확인 |
| Assets/Script/Story/FadeController.cs | Assets/Script/Story/FadePanelView.cs | New | 스토리 Fade 패널 표시 UI | EditMode 정적 회귀 테스트 통과 |
| Assets/Script/Story/Dialogue/ClicktoMain.cs | Assets/Script/Story/Dialogue/MainSceneButtonView.cs | New | 메인 씬 이동 버튼 UI | QA 에이전트 테스트 실행 요청 |
| Assets/Script/Story/StageScenarioController/Story.cs | Assets/Script/Story/StageScenarioController/StorySkipView.cs | New | 스토리 스킵 버튼 UI | QA 에이전트 테스트 실행 요청 |

## 6. Removed From This Document

긴 날짜별 변경 기록은 이 문서에서 제거했다.
상세 작업 기록은 `WorkLogs/2026-05-30.md` 이후 날짜별 로그에 있고, 세션 인계 요약은 `00_SessionHandoff.md`에 있다.
