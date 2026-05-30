# Game System Refactoring Roadmap

## 0-A. 게임 시스템 리팩토링 방식 선택

게임 시스템은 저장, 설정, 메뉴, 씬 이동이 섞이기 쉬우므로 기존 코드 수정만 고집하지 않는다.

기존 코드 일부 교체가 적절한 경우:

- `PlayerPrefs` 호출 한두 개를 `GameSaveController` 호출로 바꾸는 수준이다.
- 버튼 클릭 흐름과 Alert 연결은 그대로 유지해도 충분히 읽기 쉽다.
- 변경 후 Inspector 연결을 바꾸지 않아도 된다.

새 컴포넌트 생성이 적절한 경우:

- `StartScreenEvent`처럼 새 게임, 이어하기, 설정 열기, 종료, Alert 대기가 한 클래스에 섞여 있다.
- 씬 이동 규칙이 여러 클래스에 중복된다.
- 저장 흐름과 UI 흐름을 분리하면 오브젝트 연결만 바꿔도 코드가 단순해진다.

권장 분리 후보:

```text
StartScreenEvent
- 버튼 이벤트만 받는다.

StartGameFlowController
- 새 게임 시작과 이어하기 흐름을 처리한다.

GameSaveController
- 저장 데이터만 관리한다.

SceneMoveController
- 씬 이름 생성과 이동만 담당한다.
```

현재 진행 기준:

- `StartScreenEvent`는 기능 단위로 먼저 정리한다.
- 새 게임/이어하기 저장 흐름이 안정되면 `StartGameFlowController`로 분리할지 다시 판단한다.

## 0. 리팩토링 기본 전제

게임 시스템 리팩토링은 유지보수성과 쉬운 네이밍을 기본으로 한다.
저장, 설정, 씬 이동은 여러 화면에서 함께 쓰이므로 이름만 보고 역할이 보여야 한다.

네이밍은 가능한 한 `대상 + 행동` 형태로 작성한다.

```text
GameProgressSave
GameProgressLoad
GameSettingSave
GameSettingLoad
AudioSettingApply
VideoSettingApply
StageFlowSelect
GameSceneLoad
AlertResultWait
```

피해야 할 이름:

```text
SaveManager
SettingHelper
SceneUtil
CommonController
```

판단 기준:

```text
- 저장하는 파일인지, 불러오는 파일인지 이름으로 구분되는가?
- UI를 여는 코드와 실제 설정을 적용하는 코드가 섞이지 않는가?
- PlayerPrefs 키 변경 시 수정 위치가 작게 유지되는가?
- 초등학생이 읽어도 "스테이지 저장", "설정 적용" 정도의 역할을 짐작할 수 있는가?
```

### 코드 변경 전 확인 규칙

게임 시스템 코드를 수정하기 전에는 수정 대상 파일을 먼저 보여준다.
특히 저장, 설정, 씬 이동은 여러 화면에 영향을 줄 수 있으므로 변경 범위를 작게 나눠 제안한다.

작성 형식:

```text
수정 대상 파일:
- 파일 경로

수정 이유:
- 저장, 설정, 씬 이동 중 어떤 책임을 정리하는지 설명한다.

예상 변경 범위:
- PlayerPrefs 키, 설정 적용, 씬 이동, Alert 대기 중 무엇이 바뀌는지 설명한다.

새 파일/폴더 필요 여부:
- 새 GameSystem 파일이 필요하면 기존 파일에 넣지 않는 이유를 설명한다.

변경 전/후 또는 diff:
- 실제 적용 전에 변경 내용을 먼저 제안한다.
```

### 기능 단위 리팩토링 규칙

게임 시스템은 저장, 설정, 씬 이동이 서로 이어져 있으므로 기능 단위로 수정한다.
`PlayerPrefs`를 쓰는 파일 하나만 바꾸지 말고, 사용자가 실제로 지나가는 흐름 전체를 먼저 확인한다.

우선 기능 묶음:

```text
새 게임 시작 저장
- StartScreenEvent.NewStart()
- GameSaveController.StartNewGame()
- PlayerPrefsSaveStore.SaveGameSave()
- Prologue Screen 이동

이어하기
- StartScreenEvent.ActiveSavedBtn()
- StartScreenEvent.StartSavedStage()
- GameSaveController.HasGameSave()
- GameSaveController.GetGameSave()
- Stage Select Screen 이동

스테이지 선택 저장
- StageSelecter.stageLoader()
- GameSaveController.SavePlayingStage()
- Stage별 Story/Fight Screen 이동

전투 클리어 후 진행 저장
- BattleAlert.clearAlert()
- GameSaveController.SaveClearedStage()
- 다음 Stage 또는 Epilogue 이동

설정 저장과 적용
- AudioController
- VideoController
- GamePlayController
- 설정 PlayerPrefs 키 유지
```

각 기능을 수정하기 전에는 기능이 지나가는 파일 목록을 먼저 보여준다.
저장 기능을 고칠 때 설정값이 지워지지 않는지, 씬 이동 기능을 고칠 때 스테이지 번호가 바뀌지 않는지 함께 확인한다.

## 1. 폴더 구조

게임 시스템은 전투나 스토리 내부 규칙이 아니라, 게임 전체에서 공통으로 쓰이는 흐름을 담당한다.
현재는 저장, 설정, 메뉴, 씬 이동, 스테이지 진행도가 여러 파일에 흩어져 있다.

현재 관련 파일:

```text
Assets/Script/Start Screen
  StartScreenEvent.cs

Assets/Script/Menu
  Menu.cs

Assets/Script/Option
  Alert.cs
  Setting.cs
  AudioController.cs
  VideoController.cs
  GamePlayController.cs

Assets/Script/Stage
  StageController.cs
  StageSelecter.cs
  StageText.cs

Assets/Script/Battle
  BattleAlert.cs
```

목표 구조:

```text
Assets/Script/GameSystem
  Save
    GameSaveKey.cs
    GameProgressLoad.cs
    GameProgressSave.cs
    GameSettingLoad.cs
    GameSettingSave.cs
  Setting
    GameSettingPanelShow.cs
    AudioSettingApply.cs
    VideoSettingApply.cs
    GamePlaySettingApply.cs
  Stage
    StageFlowData.cs
    StageFlowSelect.cs
    StageProgressSave.cs
  Scene
    GameSceneLoad.cs
    GameSceneName.cs
  Alert
    AlertResultWait.cs
```

## 2. 핵심 흐름

현재 저장 흐름:

```text
StartScreenEvent
 -> PlayerPrefs.DeleteAll()
 -> savedStage 저장
 -> Prologue Screen 이동

StageSelecter
 -> playingStage 저장
 -> stage별 Summon.multiple 설정
 -> Story 또는 Fight 씬 이동

BattleAlert
 -> 클리어 시 savedStage, playingStage 저장
 -> 다음 스테이지 또는 Stage Select Screen 이동
```

현재 설정 흐름:

```text
Setting
 -> 옵션 패널 전환
 -> AudioController, VideoController, GamePlayController 참조 제공

AudioController
 -> Slider 값 변경
 -> AudioMixer 적용
 -> PlayerPrefs 저장

VideoController
 -> Toggle 값 변경
 -> Screen 해상도/모드 적용
 -> PlayerPrefs 저장
```

개선 목표:

```text
UI 입력
 -> Save/Setting/Scene 전용 클래스에 요청
 -> 실제 저장/적용/이동 수행
```

## 3. 주요 코드

### 1단계: PlayerPrefs 키 상수화

수정 대상:

```text
Assets/Script/Start Screen/StartScreenEvent.cs
Assets/Script/Stage/StageController.cs
Assets/Script/Stage/StageSelecter.cs
Assets/Script/Stage/StageText.cs
Assets/Script/Battle/BattleAlert.cs
Assets/Script/Option/AudioController.cs
Assets/Script/Option/VideoController.cs
```

현재 직접 사용하는 키:

```text
savedStage
playingStage
MasterVolume
BGMVolume
SFXVolume
resolutionIndex
screenModeIndex
```

수정 이유:

- 저장 키가 문자열로 흩어져 있어 오타와 변경 비용이 크다.
- `StartScreenEvent.NewStart()`의 `PlayerPrefs.DeleteAll()`이 진행도뿐 아니라 옵션 설정까지 지운다.

예상 코드:

```csharp
public static class GameSaveKey
{
    public const string SavedStage = "savedStage";
    public const string PlayingStage = "playingStage";
    public const string MasterVolume = "MasterVolume";
    public const string BgmVolume = "BGMVolume";
    public const string SfxVolume = "SFXVolume";
    public const string ResolutionIndex = "resolutionIndex";
    public const string ScreenModeIndex = "screenModeIndex";
}
```

### 2단계: 새 게임 시작 저장 처리 분리

수정 대상:

```text
Assets/Script/Start Screen/StartScreenEvent.cs
Assets/Script/GameSystem/Save/GameProgressSave.cs
```

수정 이유:

- 새 게임 시작은 진행도만 초기화해야 한다.
- 볼륨, 해상도, 조작 설정까지 지우는 구조는 사용자 경험이 좋지 않다.

변경 방향:

```text
PlayerPrefs.DeleteAll()
 -> GameProgressSave.ResetProgress()
 -> savedStage만 1로 초기화
```

### 3단계: 스테이지 흐름 중복 제거

수정 대상:

```text
Assets/Script/Stage/StageController.cs
Assets/Script/Stage/StageSelecter.cs
Assets/Script/Battle/BattleAlert.cs
Assets/Script/GameSystem/Stage/StageFlowData.cs
Assets/Script/GameSystem/Stage/StageFlowSelect.cs
```

현재 문제:

- `StageController.SendStage()`, `StageController.SendFightStage()`, `StageSelecter.SendStage()`가 비슷한 switch를 가진다.
- stage별 `Summon.multiple` 값이 여러 곳에 중복된다.
- 씬 이름 생성 규칙이 여러 메서드에 흩어져 있다.

예상 구조:

```csharp
[System.Serializable]
public class StageFlowData
{
    public int stageNumber;
    public bool hasStoryScene;
    public double storyBattleMultiplier;
    public double directBattleMultiplier;
}
```

### 4단계: 설정 적용과 저장 분리

수정 대상:

```text
Assets/Script/Option/AudioController.cs
Assets/Script/Option/VideoController.cs
Assets/Script/Option/GamePlayController.cs
Assets/Script/GameSystem/Setting/AudioSettingApply.cs
Assets/Script/GameSystem/Setting/VideoSettingApply.cs
```

수정 이유:

- UI 이벤트, 실제 적용, 저장이 같은 메서드에 있다.
- 설정 UI가 바뀌어도 AudioMixer/Screen 적용 로직은 유지되어야 한다.

### 5단계: Alert 결과 대기 공통화

수정 대상:

```text
Assets/Script/Start Screen/StartScreenEvent.cs
Assets/Script/Menu/Menu.cs
Assets/Script/Battle/BattleAlert.cs
Assets/Script/Story/StageScenarioController/Story.cs
Assets/Script/GameSystem/Alert/AlertResultWait.cs
```

수정 이유:

- `WaitForAlertResult` 코루틴이 여러 파일에 반복된다.
- Alert UI 자체는 유지하고, 결과 대기 흐름만 공통화한다.

## 4. 실행 방법

적용 순서:

```text
1. GameSaveKey 추가
2. PlayerPrefs 직접 문자열을 GameSaveKey로 교체
3. NewStart의 DeleteAll 제거 후 진행도 초기화만 수행
4. StageFlowData로 스테이지 배율과 씬 종류 정리
5. StageController와 StageSelecter가 같은 StageFlowSelect를 사용하게 변경
6. Audio/Video 설정 적용과 저장 분리
7. AlertResultWait 공통화
```

검증 방법:

```text
1. 새 게임 시작 후 savedStage가 1인지 확인
2. 새 게임 시작 후 볼륨/해상도 설정이 유지되는지 확인
3. 이어하기 버튼이 savedStage 기준으로 정상 표시되는지 확인
4. 스테이지 선택 시 playingStage와 씬 이동이 정상인지 확인
5. 전투 클리어 후 다음 스테이지 저장이 정상인지 확인
6. ESC 메뉴, 메인 이동, 종료 Alert가 정상 동작하는지 확인
```

## 5. 나중에 확장할 수 있는 부분

### JSON 저장

처음에는 PlayerPrefs를 유지한다.
저장 데이터가 늘어나면 JSON 저장으로 교체한다.

```csharp
public interface GameProgressSaveProvider
{
    void SaveProgress(int savedStage);
    int LoadSavedStage();
}
```

### 외부 서비스와 비용

현재 단계에서는 외부 서비스가 필요 없다.
월 3만원 이상의 운영 비용도 발생하지 않는다.

추후 랭킹, 클라우드 저장을 붙인다면 아래 순서가 적절하다.

```text
1. 로컬 JSON
2. Firebase 무료/저비용 플랜
3. 자체 서버 API
```

## 6. 변경 기록

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/01_GameSystemRoadmap.md

어떻게 수정했는가:
- 게임 시스템 리팩토링을 저장, 이어하기, 스테이지 선택, 전투 클리어 저장, 설정 저장과 적용 기능 단위로 진행하도록 규칙을 추가했다.
- 각 기능이 지나가는 기존 파일과 새 저장 코드의 연결 지점을 함께 보도록 지시했다.

왜 그렇게 수정했는가:
- 저장 로직은 `StartScreenEvent`, `StageSelecter`, `BattleAlert`, `StageText`처럼 여러 파일에 흩어져 있어 한 파일만 바꾸면 기존 진행 흐름이 깨질 수 있다.
- 기능 단위로 묶어야 `savedStage`, `playingStage`, 설정 PlayerPrefs 키가 서로 충돌하지 않는지 검사할 수 있다.

검증 방법:
- 게임 시스템 로드맵에 기능 단위 작업 묶음이 추가됐는지 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/01_GameSystemRoadmap.md

어떻게 수정했는가:
- 실제 코드 기준으로 저장 키, 설정 컨트롤러, 스테이지 이동, Alert 중복을 정리했다.
- `PlayerPrefs.DeleteAll`, 스테이지 switch 중복, Alert 코루틴 반복을 우선 개선 대상으로 지정했다.

왜 그렇게 수정했는가:
- 저장/설정/씬 이동은 여러 시스템에서 동시에 사용되므로 먼저 기준을 통일해야 이후 전투와 스토리 리팩토링이 흔들리지 않는다.

검증 방법:
- StartScreenEvent, Menu, Setting, AudioController, VideoController, StageController, StageSelecter, BattleAlert 구조 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/01_GameSystemRoadmap.md

어떻게 수정했는가:
- 게임 시스템 리팩토링의 기본 전제로 유지보수성, 쉬운 네이밍, `대상 + 행동` 이름 규칙을 추가했다.

왜 그렇게 수정했는가:
- 저장, 설정, 씬 이동은 프로젝트 전체에서 쓰이므로 모호한 이름이 생기면 이후 기능 추가와 수정이 어려워진다.

검증 방법:
- 문서 상단에 기본 전제 섹션 추가 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/01_GameSystemRoadmap.md

어떻게 수정했는가:
- 게임 시스템 코드 변경 전에 수정 대상 파일과 변경 범위를 먼저 보여주도록 확인 규칙을 추가했다.

왜 그렇게 수정했는가:
- 저장, 설정, 씬 이동은 여러 화면에 영향을 주므로 변경 전 범위를 먼저 확인해야 안전하다.

검증 방법:
- 문서 상단에 코드 변경 전 확인 규칙 추가 확인

### 2026-05-30

수정 대상
- Assets/Script/Menu/MenuView.cs
- Assets/Script/Option/ConfirmAlertView.cs
- Assets/Script/Option/AudioSettingView.cs
- Assets/Script/Option/VideoSettingView.cs
- Assets/Script/Option/GameplaySettingView.cs
- Assets/Script/Option/SettingPanelView.cs
- Assets/Script/Stage/StageSelectView.cs
- Assets/Script/Stage/StageTextView.cs
- Assets/Script/Start Screen/StartScreenView.cs
- Assets/RefactRoadMap/01_GameSystemRoadmap.md

어떻게 수정했는가:
- 게임 시스템 쪽 UI 연결 클래스가 `View` 이름으로 정리된 상태를 로드맵에 반영했다.
- `Menu`, `Alert`, `Setting`, `AudioController`, `VideoController`, `GamePlayController`, `StageSelecter`, `StageText`, `StartScreenEvent`의 새 이름을 기록했다.
- 현재 C# 코드 타입 참조는 새 `*View` 이름 기준으로 정리된 것으로 확인했다.

완료 표시:
- `Menu` -> `MenuView`
- `Alert` -> `ConfirmAlertView`
- `AudioController` -> `AudioSettingView`
- `VideoController` -> `VideoSettingView`
- `GamePlayController` -> `GameplaySettingView`
- `Setting` -> `SettingPanelView`
- `StageSelecter` -> `StageSelectView`
- `StageText` -> `StageTextView`
- `StartScreenEvent` -> `StartScreenView`

나중에 처리할 내용:
- 씬/프리팹 UnityEvent의 `m_TargetAssemblyTypeName`에 남은 `Alert`, `Menu`, `Setting`, `GamePlayController`, `StageSelecter` 문자열 정리
- Inspector 변수명 `audioController`, `videoController`, `gamePlayController`는 동작 확인 후 `audioSettingView`, `videoSettingView`, `gameplaySettingView`로 정리 검토

검증 방법:
- 시작 화면: 새 게임, 이어하기, 설정 버튼 확인
- 메뉴: ESC 열기/닫기, 메인 이동 Alert, 종료 Alert 확인
- 설정: 오디오/비디오/게임플레이 탭 전환 및 값 변경 확인
- 스테이지 선택: 저장된 스테이지 기준 버튼 활성화와 씬 이동 확인

## 2026-05-30 통합 작업 요약

### 완료
- 게임 시스템 리팩토링 원칙 정리: 저장, 설정, 메뉴, 씬 이동, 스테이지 선택 흐름을 기능 단위로 보도록 정리했다.
- UI 연결 클래스명을 View 계열로 정리한 상태를 로드맵에 반영했다.
- `StartScreenView`, `MenuView`, `SettingPanelView`, `ConfirmAlertView`, `AudioSettingView`, `VideoSettingView`, `GameplaySettingView`, `StageSelectView`, `StageTextView`를 현재 사용 이름으로 기록했다.

### 남은 작업
- 씬/프리팹 UnityEvent 타입명에 남은 `Alert`, `Menu`, `Setting`, `GamePlayController`, `StageSelecter` 정리.
- 설정 패널 내부 변수명은 View 이름 기준으로 추후 정리.
- 시작 화면, 메뉴, 설정, 스테이지 선택 버튼 수동 QA 필요.
