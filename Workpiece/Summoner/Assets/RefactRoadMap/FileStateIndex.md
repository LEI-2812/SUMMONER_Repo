# Refactoring File State Index

## 0-A. 현재 리팩토링 운영 기준

앞으로 파일 상태를 기록할 때는 기존 코드 수정만 전제로 하지 않는다.
아래 두 흐름을 모두 정상적인 리팩토링 방식으로 기록한다.

```text
기존 코드 일부 교체
- 기존 파일이 계속 실제 게임 흐름에서 사용된다.
- 상태는 Original 또는 Bridge로 기록한다.

새 컴포넌트 생성 후 연결 이전
- 새 파일을 만들고 기존 오브젝트에 붙여 실제 흐름을 옮긴다.
- 새 파일은 New, 연결 중인 기존 파일은 Bridge로 기록한다.
- 기존 파일이 더 이상 호출되지 않으면 Deprecated로 바꾼다.
```

현재 반영 상태:

```text
GameSaveController
- Start Screen 씬에 오브젝트로 추가됨
- 새 게임 시작 저장 흐름에 연결됨
- 이어하기 저장 읽기 흐름에 연결됨

StartScreenEvent
- PlayerPrefs.DeleteAll 제거됨
- 새 게임 시작과 이어하기는 GameSaveController를 사용함
- LoadStage 제거됨
```

## 1. 폴더 구조

이 문서는 리팩토링 중 원본 코드, 새 코드, 임시 연결 코드, 삭제 예정 코드를 구분하기 위한 기준 문서다.
세션을 다시 시작해도 어떤 파일이 실제 게임 흐름에서 사용 중인지 바로 알 수 있게 유지한다.

위치:

```text
Assets/RefactRoadMap
  FileStateIndex.md
```

## 2. 핵심 흐름

리팩토링 중 새 파일을 만들거나 기존 파일의 역할을 바꾸면 이 문서에 반드시 기록한다.

파일 상태는 아래 값 중 하나만 사용한다.

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

## 3. 주요 코드

### 파일 상태 표

| 파일 | 상태 | 역할 | 연결된 기능 | 대체 대상 | 다음 처리 | 마지막 확인일 |
|---|---|---|---|---|---|---|
| Assets/Script/Save/GameSaveController.cs | New | 저장 기능의 새 진입점 | 새 게임 시작, 이어하기, 스테이지 선택 저장, 전투 클리어 저장 | PlayerPrefs 직접 접근 | 기존 호출부 연결 여부 확인 | 2026-05-30 |
| Assets/Script/Save/GameSaveData.cs | New | 저장 데이터 구조 | savedStage, playingStage | 개별 int 저장 흐름 | 필드 추가 시 저장소와 함께 수정 | 2026-05-30 |
| Assets/Script/Save/PlayerPrefsSaveStore.cs | New | PlayerPrefs 저장소 | 진행 저장 로드/저장/초기화 | PlayerPrefs 직접 접근 | 기존 저장 키와 호환성 확인 | 2026-05-30 |
| Assets/Script/Start Screen/StartScreenEvent.cs | Original | 시작 화면 새 게임/이어하기 | 새 게임 시작, 이어하기 | GameSaveController | DeleteAll 제거 후 Bridge로 전환 | 2026-05-30 |
| Assets/Script/Stage/StageSelecter.cs | Original | 스테이지 선택과 playingStage 저장 | 스테이지 선택 저장 | GameSaveController.SavePlayingStage | 저장 호출 교체 후 Bridge로 전환 | 2026-05-30 |
| Assets/Script/Battle/BattleAlert.cs | Original | 전투 결과 처리와 다음 스테이지 저장 | 전투 클리어 저장 | GameSaveController.SaveClearedStage | 저장 호출 교체 후 Bridge로 전환 | 2026-05-30 |
| Assets/Tests/PlayMode/PlayModeTests.asmdef | New | PlayMode 테스트 어셈블리 정의 | 이어하기 자동 QA | 수동 QA | Unity Test Runner 인식 확인 | 2026-05-30 |
| Assets/Tests/PlayMode/GameSaveContinueTests.cs | New | 이어하기 PlayMode 테스트 | 이어하기 | 수동 QA | MCP 또는 Unity Test Runner로 실행 | 2026-05-30 |

### 기능별 연결 상태

| 기능 | 현재 사용 코드 | 새 코드 | 연결 상태 | QA 상태 | 마지막 확인일 |
|---|---|---|---|---|---|
| 새 게임 시작 저장 | StartScreenEvent.NewStart | GameSaveController.StartNewGame | 미연결 | 대기 | 2026-05-30 |
| 이어하기 | StartScreenEvent.StartSavedStage | GameSaveController.GetGameSave | 부분 연결 | PlayMode 테스트 추가, MCP 실행 타임아웃 | 2026-05-30 |
| 스테이지 선택 저장 | StageSelecter.stageLoader | GameSaveController.SavePlayingStage | 미연결 | 대기 | 2026-05-30 |
| 전투 클리어 저장 | BattleAlert.clearAlert | GameSaveController.SaveClearedStage | 미연결 | 대기 | 2026-05-30 |

### 새 파일 상단 주석 예시

새 리팩토링 파일에는 필요한 경우 아래처럼 짧게 남긴다.
모든 파일에 강제로 붙이지 말고, 원본과 새 코드가 동시에 존재해서 헷갈릴 수 있는 파일에만 사용한다.

```csharp
// RefactorState: New
// Replaces: StartScreenEvent, StageSelecter, BattleAlert의 PlayerPrefs 직접 저장
// ConnectedFeature: 새 게임 시작, 스테이지 선택, 전투 클리어 저장
```

기존 파일의 제거 예정 코드 근처에는 짧은 TODO만 남긴다.

```csharp
// TODO Refactor: GameSaveController 연결 후 PlayerPrefs 직접 저장 제거
```

## 4. 실행 방법

리팩토링 작업자는 아래 순서로 기록한다.

```text
1. 새 파일을 만들기 전 FileStateIndex.md에 New 후보로 기록한다.
2. 기존 파일을 새 코드와 연결하면 해당 기존 파일 상태를 Bridge로 바꾼다.
3. 기존 기능이 새 코드로 완전히 대체되면 기존 파일이나 메서드를 Deprecated로 바꾼다.
4. 참조가 0개이고 QA가 끝난 뒤에만 Removed로 기록한다.
5. 삭제 전에는 반드시 참조 검색 결과와 대체 파일을 기록한다.
```

삭제 전 확인 기준:

```text
- Unity Console 컴파일 에러 없음
- 참조 검색 결과 0개 또는 의도한 참조만 남음
- 기능별 수동 테스트 완료
- QAAgent 검사 결과 치명적 회귀 없음
- 관련 로드맵 변경 기록 작성 완료
```

## 5. 나중에 확장할 수 있는 부분

파일이 많아지면 기능별로 표를 분리한다.

```text
GameSystemFileState.md
StorySystemFileState.md
BattleCoreFileState.md
BattleContentFileState.md
```

처음부터 나누지는 않는다.
현재는 한 문서에서 전체 상태를 보는 편이 세션 재시작 후 파악하기 쉽다.

## 6. 변경 기록

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/FileStateIndex.md

어떻게 수정했는가:
- 중복 테스트 어셈블리 정리 결과를 기록했다.
- 현재 남은 테스트 어셈블리가 `Assets/Tests/PlayMode/PlayModeTests.asmdef` 하나임을 확인했다.

왜 그렇게 수정했는가:
- `Tests.asmdef`가 여러 위치에 있으면 Unity Test Runner가 테스트를 잘못 분류하거나 컴파일 인식이 불안정해질 수 있다.
- 이어하기 PlayMode 테스트는 하나의 명확한 테스트 어셈블리에서 관리하는 편이 유지보수에 좋다.

검증 방법:
- `Assets` 아래 남은 asmdef가 `Assets/Tests/PlayMode/PlayModeTests.asmdef` 하나인지 확인
- MCP 또는 Unity Test Runner로 PlayMode 테스트 재실행

### 2026-05-30

수정 대상:
- Assets/Tests/PlayMode/PlayModeTests.asmdef
- Assets/RefactRoadMap/FileStateIndex.md

어떻게 수정했는가:
- PlayMode 테스트 어셈블리가 EditMode 테스트처럼 인식되던 설정을 수정했다.
- `includePlatforms: ["Editor"]`를 제거하고 `optionalUnityReferences: ["TestAssemblies"]`를 추가했다.

왜 그렇게 수정했는가:
- 기존 설정에서는 EditMode Test Runner가 PlayMode 테스트를 발견했고, `SceneManager.LoadScene()`이 PlayMode 전용 API라서 4개 테스트가 모두 실패했다.
- 이어하기 QA는 씬 로드와 MonoBehaviour 실행이 필요하므로 PlayMode 테스트 어셈블리로 인식되어야 한다.

검증 방법:
- MCP EditMode 실행에서 PlayMode 테스트가 잘못 실행되어 실패하는 현상을 확인
- asmdef 수정 후 MCP PlayMode 실행을 다시 시도했으나 현재 Test Runner 요청이 타임아웃됨

### 2026-05-30

수정 대상:
- Assets/Tests/PlayMode/PlayModeTests.asmdef
- Assets/Tests/PlayMode/GameSaveContinueTests.cs
- Assets/RefactRoadMap/FileStateIndex.md

어떻게 수정했는가:
- 이어하기 기능을 자동 QA할 수 있도록 PlayMode 테스트 어셈블리와 테스트 코드를 추가했다.
- 저장 없음, 저장 있음, savedStage 로드, savedStage 0 방어 조건을 검사하는 테스트를 준비했다.
- FileStateIndex.md에 새 테스트 파일과 이어하기 기능의 부분 연결 상태를 기록했다.

왜 그렇게 수정했는가:
- 이어하기 기능은 씬, 버튼, MonoBehaviour, PlayerPrefs가 함께 동작하므로 정적 검사만으로는 충분하지 않다.
- PlayMode 테스트를 추가하면 이후 MCP 또는 Unity Test Runner로 같은 QA를 반복 실행할 수 있다.

검증 방법:
- MCP `run_tests(testMode: "PlayMode")` 실행을 시도했으나 현재 요청이 타임아웃됨
- Unity Editor의 Test Runner에서 PlayMode 테스트 인식 여부 추가 확인 필요

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/FileStateIndex.md

어떻게 수정했는가:
- 리팩토링 중 원본 코드, 새 코드, 임시 연결 코드, 삭제 예정 코드를 구분하는 파일 상태 문서를 추가했다.
- `Original`, `New`, `Bridge`, `Deprecated`, `Removed` 상태 기준을 정의했다.
- 새로 추가된 저장 시스템 파일과 기존 저장 흐름 파일을 초기 추적 대상으로 기록했다.

왜 그렇게 수정했는가:
- 리팩토링 중 기존 코드와 새 코드가 동시에 존재하면 세션을 다시 시작했을 때 어떤 코드가 실제 흐름에서 사용 중인지 알기 어렵다.
- 파일 상태와 기능 연결 상태를 문서로 남기면 다음 작업자가 원본, 새 코드, 삭제 후보를 빠르게 구분할 수 있다.

검증 방법:
- FileStateIndex.md 생성 위치 확인
- 저장 시스템 관련 새 파일과 기존 연결 대상이 표에 기록됐는지 확인

### 2026-05-30

수정 대상
- Assets/RefactRoadMap/FileStateIndex.md

어떻게 수정했는가:
- UI와 직접 연결된 클래스명을 `View`로 바꾼 현재 상태를 파일 상태 인덱스에 추가했다.
- 기존 파일은 삭제 또는 이름 변경 대상으로 보고, 대응되는 새 `*View` 파일을 실제 사용 후보로 기록했다.
- UnityEvent 타입명 정리는 아직 코드/씬에 적용하지 않았으므로 다음 처리 항목으로 분리했다.

#### UI View 이름 변경 상태

| 기존 파일 | 현재 파일 | 상태 | 역할 | 다음 처리 |
|---|---|---|---|---|
| Assets/Script/Battle/BattleAlert.cs | Assets/Script/Battle/BattleResultAlertView.cs | New | 전투 결과 Alert 표시 | UnityEvent 타입명 잔여 여부 확인 |
| Assets/Script/Battle/BattleLogic/PickSummonPanel.cs | Assets/Script/Battle/BattleLogic/PickSummonPanelView.cs | New | 소환수 선택 패널 표시/클릭 | 선택 패널 동작 확인 |
| Assets/Script/Battle/BattleLogic/StatePanel.cs | Assets/Script/Battle/BattleLogic/SummonStatePanelView.cs | New | 소환수 상태 패널 표시 | HP/Shield/공격 버튼 표시 확인 |
| Assets/Script/Menu/Menu.cs | Assets/Script/Menu/MenuView.cs | New | ESC 메뉴 UI | UnityEvent 타입명 `Menu` 정리 필요 |
| Assets/Script/Option/Alert.cs | Assets/Script/Option/ConfirmAlertView.cs | New | Yes/No 확인 Alert UI | UnityEvent 타입명 `Alert` 정리 필요 |
| Assets/Script/Option/AudioController.cs | Assets/Script/Option/AudioSettingView.cs | New | 오디오 설정 UI | Inspector 참조명 추후 정리 |
| Assets/Script/Option/GamePlayController.cs | Assets/Script/Option/GameplaySettingView.cs | New | 게임플레이 설정 UI | UnityEvent 타입명 `GamePlayController` 정리 필요 |
| Assets/Script/Option/Setting.cs | Assets/Script/Option/SettingPanelView.cs | New | 설정 패널 전환 UI | UnityEvent 타입명 `Setting` 정리 필요 |
| Assets/Script/Option/VideoController.cs | Assets/Script/Option/VideoSettingView.cs | New | 비디오 설정 UI | Inspector 참조명 추후 정리 |
| Assets/Script/Stage/StageSelecter.cs | Assets/Script/Stage/StageSelectView.cs | New | 스테이지 선택 버튼 UI | UnityEvent 타입명 `StageSelecter` 정리 필요 |
| Assets/Script/Stage/StageText.cs | Assets/Script/Stage/StageTextView.cs | New | 시작 화면 스테이지 텍스트 UI | 시작 화면 표시 확인 |
| Assets/Script/Start Screen/StartScreenEvent.cs | Assets/Script/Start Screen/StartScreenView.cs | New | 시작 화면 버튼/Alert UI | 시작/이어하기/설정 버튼 확인 |

#### 검사 결과

| 검사 항목 | 결과 | 다음 처리 |
|---|---|---|
| C# 타입 참조 | 대부분 새 `*View` 타입으로 정리됨 | 컴파일로 최종 확인 |
| `.meta` GUID | 대부분 기존 GUID 유지 | `StageSelectView`, `StageTextView`는 씬이 새 GUID를 참조 중 |
| UnityEvent 타입명 | 이전 이름이 남아 있음 | 나중에 일괄 정리 |
| 추가 View 후보 | `StoryChangeImage`, `FadeController`, `ClicktoMain`, `Story` | 영향 범위 작은 파일부터 제안 후 변경 |

#### 나중에 정리할 UnityEvent 타입명

| 남은 타입명 | 변경 후보 | 확인된 개수 |
|---|---|---:|
| Alert | ConfirmAlertView | 114 |
| GamePlayController | GameplaySettingView | 34 |
| Menu | MenuView | 51 |
| Setting | SettingPanelView | 17 |
| StageSelecter | StageSelectView | 7 |

검증 방법:
- C# 클래스 선언 검색
- 이전 클래스명 참조 검색
- 기존/새 `.meta` GUID 비교
- 씬/프리팹 `m_TargetAssemblyTypeName` 검색

## 2026-05-30 통합 작업 요약

### 완료
- 저장 시스템 신규 파일과 테스트 파일 상태를 `FileStateIndex.md`에 기록했다.
- UI 연결 클래스의 View 이름 변경 상태를 한 표로 정리했다.
- 기존 파일과 현재 `*View` 파일의 대응 관계를 기록했다.
- UnityEvent 타입명 잔여 작업을 별도 표로 분리했다.

### View 이름 변경 완료 목록
| 기존 이름 | 현재 이름 | 영역 |
|---|---|---|
| BattleAlert | BattleResultAlertView | Battle |
| PickSummonPanel | PickSummonPanelView | BattleContent |
| StatePanel | SummonStatePanelView | BattleCore |
| Menu | MenuView | GameSystem |
| Alert | ConfirmAlertView | GameSystem |
| AudioController | AudioSettingView | GameSystem |
| VideoController | VideoSettingView | GameSystem |
| GamePlayController | GameplaySettingView | GameSystem |
| Setting | SettingPanelView | GameSystem |
| StageSelecter | StageSelectView | GameSystem |
| StageText | StageTextView | GameSystem |
| StartScreenEvent | StartScreenView | GameSystem |

### 남은 작업
| 남은 항목 | 처리 방향 |
|---|---|
| UnityEvent 타입명 잔여 문자열 | 씬/프리팹에서 일괄 교체 전 별도 diff 제안 |
| Inspector 변수명 `audioController`, `videoController`, `gamePlayController` | 동작 확인 후 View 기준 이름으로 정리 |
| Story UI 후보 | 영향 범위 작은 파일부터 제안 |
| Player/Plate/Summon UI 책임 | 별도 View 분리 후 이름 변경 검토 |
