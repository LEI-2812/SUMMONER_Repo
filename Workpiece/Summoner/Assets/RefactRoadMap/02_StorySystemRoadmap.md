# Story System Refactoring Roadmap

## 0-A. 스토리 시스템 리팩토링 방식 선택

스토리 시스템은 대화 출력, 입력 처리, 연출, 씬 이동이 섞여 있으므로 새 컴포넌트 분리를 적극적으로 고려한다.

기존 코드 일부 교체가 적절한 경우:

- 특정 Stage의 잘못된 연출 순서나 조건만 고친다.
- CSV 대화 범위 같은 작은 데이터 접근만 정리한다.
- 기존 오브젝트 연결을 유지하는 편이 더 안전하다.

새 컴포넌트 생성이 적절한 경우:

- `InteractionController`가 대화 출력, 클릭 입력, 이미지 변경, 페이드, 씬 이동을 모두 갖고 있다.
- `Stage*_Controller`들이 Space 입력, 클릭 처리, 중복 대화 ID 방지를 반복한다.
- Stage가 늘어날수록 복사 붙여넣기 코드가 증가한다.

권장 분리 후보:

```text
DialogueProgressController
- 대화 출력과 다음 줄 진행만 담당한다.

StoryScenarioControllerBase
- 공통 입력 처리와 시나리오 단계 증가를 담당한다.

StageNScenarioController
- Stage별 이동, 이펙트, 사운드 연출만 담당한다.

StorySceneMoveController
- 스토리 종료 후 다음 씬 이동만 담당한다.
```

현재 진행 기준:

- Stage 하나씩 기능 단위로 분리한다.
- 기존 Stage 오브젝트에 새 Scenario 컴포넌트를 붙일 수 있으면 새 컴포넌트 방식을 우선 검토한다.

## 0. 리팩토링 기본 전제

스토리 시스템 리팩토링은 대사 흐름과 연출 흐름을 읽기 쉽게 만드는 것을 우선한다.
CSV 파싱, 대사 출력, 입력 처리, 연출 실행은 이름만 보고 구분되어야 한다.

네이밍은 가능한 한 `대상 + 행동` 형태로 작성한다.

```text
DialogueCsvParse
DialogueDatabaseLoad
DialogueRangeSelect
DialogueLineShow
StoryInputRead
StoryProgressAdvance
StorySceneMove
StorySkipRequest
StoryScenarioPlay
StoryActorMove
```

피해야 할 이름:

```text
StoryManager
DialogueHelper
ScenarioUtil
CommonStory
```

판단 기준:

```text
- CSV를 읽는 코드와 대사를 보여주는 코드가 분리되는가?
- "다음 대사로 이동"과 "캐릭터를 이동"이 이름으로 구분되는가?
- Stage별 연출 파일에는 해당 Stage의 연출만 남는가?
- 새 스토리를 추가할 때 어디를 수정해야 하는지 바로 알 수 있는가?
```

### 코드 변경 전 확인 규칙

스토리 시스템 코드를 수정하기 전에는 수정 대상 파일을 먼저 보여준다.
CSV 파싱, 대사 진행, Stage별 연출은 서로 영향이 다르므로 한 번에 섞어서 수정하지 않는다.

작성 형식:

```text
수정 대상 파일:
- 파일 경로

수정 이유:
- CSV 파싱, 대사 출력, 입력 처리, 연출 실행 중 어떤 책임을 정리하는지 설명한다.

예상 변경 범위:
- 어떤 대사 흐름, Stage 연출, 씬 이동이 바뀌는지 설명한다.

새 파일/폴더 필요 여부:
- 새 Story 파일이 필요하면 기존 파일에 넣지 않는 이유를 설명한다.

변경 전/후 또는 diff:
- 실제 적용 전에 변경 내용을 먼저 제안한다.
```

### 기능 단위 리팩토링 규칙

스토리 시스템은 대사 데이터, 입력, 연출, 씬 이동이 한 흐름으로 이어진다.
따라서 CSV 파싱 파일 하나만 고치거나 Stage별 Controller 하나만 고치는 방식으로 진행하지 않는다.

우선 기능 묶음:

```text
CSV 대사 불러오기와 파싱
- DialogueParser
- DatabaseManager
- DialogueCsvParse
- DialogueDatabaseLoad
- storyNum별 대사 범위 확인

대사 진행
- InteractionEvent
- InteractionController
- DialogueLineShow
- StoryProgressAdvance
- 현재 대사 인덱스와 UI 출력 확인

스토리 입력 처리
- Stage*_Controller
- StoryScenarioBase
- 클릭 입력
- Space 입력
- 이동 중 입력 무시

Stage별 연출 실행
- Stage1_Controller, Stage2_Controller, Stage3_Controller, Stage5_Controller, Stage7_Controller
- PlayerMove, EnemyMove, FoxMove
- 이미지, 사운드, 이동 연출

스토리 종료 후 씬 이동
- InteractionController
- StorySceneMove
- Fight Screen_NStage
- Stage Select Screen
- Thank Screen
```

각 기능을 수정하기 전에는 대사 시작부터 종료 후 씬 이동까지 어떤 파일을 지나는지 먼저 보여준다.
Stage 하나를 고친 뒤 정상 확인하고, 같은 패턴을 다음 Stage에 반복 적용한다.

## 1. 폴더 구조

스토리 시스템은 대사 데이터, 대사 진행, 입력 처리, Stage별 연출, 스토리 종료 후 씬 이동을 담당한다.

현재 관련 파일:

```text
Assets/Script/Story
  Dialogue
    Dialogue.cs
    DialogueParser.cs
    DatabaseManager.cs
    InteractionEvent.cs
    InteractionController.cs
    ClicktoMain.cs
  StageScenarioController
    ScenarioBase.cs
    Story.cs
    Stage1_Controller.cs
    Stage2_Controller.cs
    Stage3_Controller.cs
    Stage5_Controller.cs
    Stage7_Controller.cs
  StoryStage.cs
  StoryChangeImage.cs
  FadeController.cs
  PlayerMove.cs
  EnemyMove.cs
  FoxMove.cs
```

목표 구조:

```text
Assets/Script/Story
  Dialogue
    Dialogue.cs
    DialogueCsvParse.cs
    DialogueDatabaseLoad.cs
    DialogueRangeSelect.cs
    DialogueLineShow.cs
  Progress
    StoryInputRead.cs
    StoryProgressAdvance.cs
    StorySceneMove.cs
    StorySkipRequest.cs
  Scenario
    StoryScenarioBase.cs
    Stage1ScenarioPlay.cs
    Stage2ScenarioPlay.cs
    Stage3ScenarioPlay.cs
    Stage5ScenarioPlay.cs
    Stage7ScenarioPlay.cs
  Actor
    StoryActorMove.cs
    StoryActorEffectPlay.cs
  View
    StoryImageChange.cs
    StoryFadePlay.cs
```

## 2. 핵심 흐름

현재 흐름:

```text
DatabaseManager
 -> DialogueParser.Parse(csv_FileName)
 -> dialogueDic에 Dialogue 저장

InteractionEvent
 -> StoryStage.checkStage()
 -> StoryStage의 x, y를 DialogueEvent에 넣음
 -> DatabaseManager.instance.getDialogue(x, y)

InteractionController
 -> InteractionEvent.getDialogue()
 -> 현재 대사 인덱스와 줄 인덱스 관리
 -> Text UI에 출력
 -> 대사가 끝나면 FadeOut 후 Fight 또는 Stage Select 또는 Thank Screen 이동

Stage*_Controller
 -> Space/Click 입력 처리
 -> InteractionController.ShowNextLine()
 -> scenarioFlowCount 기준으로 이동/이미지/사운드 연출 실행
```

개선 목표:

```text
CSV 로드
 -> CSV 파싱
 -> 현재 storyNum에 맞는 Dialogue 범위 선택
 -> 대사 진행
 -> Stage별 연출 실행
 -> 스토리 종료 후 다음 씬 이동
```

## 3. 주요 코드

### 1단계: 빌드 위험 using 제거

수정 대상:

```text
Assets/Script/Story/Dialogue/DatabaseManager.cs
Assets/Script/Story/Dialogue/InteractionEvent.cs
```

현재 문제:

```text
DatabaseManager.cs
- using UnityEditor.Search;

InteractionEvent.cs
- using UnityEditor.Build;
```

수정 이유:

- 런타임 스크립트에서 UnityEditor 네임스페이스를 참조하면 빌드에서 문제가 될 수 있다.
- 실제 코드에서 사용하지 않는 using이므로 먼저 제거한다.

### 2단계: Dialogue 로드와 파싱 분리

수정 대상:

```text
Assets/Script/Story/Dialogue/DialogueParser.cs
Assets/Script/Story/Dialogue/DatabaseManager.cs
Assets/Script/Story/Dialogue/DialogueCsvParse.cs
Assets/Script/Story/Dialogue/DialogueDatabaseLoad.cs
```

현재 문제:

- `DialogueParser`가 `Resources.Load<TextAsset>()`와 CSV 텍스트 파싱을 함께 한다.
- `DatabaseManager`가 싱글톤 생성, 파싱 실행, Dictionary 저장을 함께 한다.

변경 방향:

```text
DialogueDatabaseLoad
 -> CSV TextAsset 로드

DialogueCsvParse
 -> CSV 문자열을 Dialogue[]로 변환

DialogueDatabase
 -> Dialogue를 ID 기준으로 조회
```

### 3단계: StoryStage 범위 데이터화

수정 대상:

```text
Assets/Script/Story/StoryStage.cs
Assets/Script/Story/Dialogue/DialogueRangeSelect.cs
```

현재 문제:

- `StoryStage.checkStage()`가 storyNum별 x, y 범위를 switch로 관리한다.
- 새 스토리 구간이 추가될 때 코드 수정이 필요하다.

예상 구조:

```csharp
[System.Serializable]
public class DialogueRange
{
    public int storyNumber;
    public int startDialogueId;
    public int endDialogueId;
}
```

### 4단계: InteractionController 책임 분리

수정 대상:

```text
Assets/Script/Story/Dialogue/InteractionController.cs
Assets/Script/Story/Progress/StoryProgressAdvance.cs
Assets/Script/Story/Dialogue/DialogueLineShow.cs
Assets/Script/Story/Progress/StorySceneMove.cs
```

현재 문제:

- 대사 배열 보관, 현재 인덱스 관리, Text UI 출력, 클릭 처리, FadeOut 이후 씬 이동을 한 클래스가 처리한다.
- `isStory`는 이동/연출 중 대사 진행을 막는 역할인데 이름만 보면 의미가 불명확하다.

변경 방향:

```text
StoryProgressAdvance
- 현재 Dialogue 인덱스와 Line 인덱스 관리
- 다음 줄 반환

DialogueLineShow
- characterName, dialogueContext UI 표시

StorySceneMove
- storyNum에 따라 다음 씬 결정

InteractionController
- 기존 씬 연결을 유지하는 조립 역할
```

### 5단계: Stage별 Controller 공통 입력 분리

수정 대상:

```text
Assets/Script/Story/StageScenarioController/Stage1_Controller.cs
Assets/Script/Story/StageScenarioController/Stage2_Controller.cs
Assets/Script/Story/StageScenarioController/Stage3_Controller.cs
Assets/Script/Story/StageScenarioController/Stage5_Controller.cs
Assets/Script/Story/StageScenarioController/Stage7_Controller.cs
Assets/Script/Story/Scenario/StoryScenarioBase.cs
```

현재 반복되는 흐름:

```text
void Update()
- Space 입력 시 OnClickDialogue()

OnPointerClick()
- OnClickDialogue()

OnClickDialogue()
- playerMove.getIsMoving() 확인
- interactionController.ShowNextLine()
- scenarioFlow()

checkCSVDialogueID()
- 같은 대사 ID인지 확인
- scenarioFlowCount 증가
```

변경 방향:

```csharp
public abstract class StoryScenarioBase : MonoBehaviour, IPointerClickHandler
{
    protected abstract void PlayScenarioStep(int scenarioStep);
}
```

Stage별 파일에는 아래 내용만 남긴다.

```text
Stage1ScenarioPlay
- 어느 step에서 이동하는지
- 어느 step에서 이미지/사운드를 켜는지
- 해당 Stage에만 있는 연출
```

### 6단계: Actor 이동 공통화

수정 대상:

```text
Assets/Script/Story/PlayerMove.cs
Assets/Script/Story/EnemyMove.cs
Assets/Script/Story/FoxMove.cs
Assets/Script/Story/Actor/StoryActorMove.cs
```

수정 이유:

- PlayerMove, EnemyMove, FoxMove는 이동 목표, 속도, isMoving 상태, MoveTowards 흐름이 비슷하다.
- 단, UI RectTransform 이동과 Transform 이동이 다르므로 한 번에 강하게 추상화하지 않는다.
- 먼저 이름과 책임만 정리하고, 중복 제거는 후순위로 둔다.

## 4. 실행 방법

적용 순서:

```text
1. UnityEditor using 제거
2. DialogueParser에서 CSV 로드와 파싱 분리
3. StoryStage의 switch를 DialogueRange 배열로 이전
4. InteractionController에서 대사 진행과 화면 출력 분리
5. Stage1_Controller만 StoryScenarioBase 기반으로 먼저 변경
6. Stage1 정상 확인 후 Stage2, Stage3, Stage5, Stage7 순서로 반복 적용
7. PlayerMove, EnemyMove, FoxMove는 마지막에 정리
```

검증 방법:

```text
1. Prologue Screen에서 첫 대사가 정상 출력되는지 확인
2. 클릭과 Space 입력으로 다음 대사가 진행되는지 확인
3. Stage1 이동/이미지/사운드 연출이 기존과 같은지 확인
4. 스토리 종료 후 Fight Screen_NStage로 이동하는지 확인
5. Epilogue는 Thank Screen으로 이동하는지 확인
6. Story Skip 버튼이 설정값에 따라 표시되는지 확인
```

## 5. 나중에 확장할 수 있는 부분

### StoryStep 데이터화

Stage별 switch가 안정적으로 줄어든 뒤에 연출을 데이터화한다.

```text
StoryStep
- dialogueIndex
- dialogueBoxVisible
- actorMoveDistance
- actorMoveSpeed
- effectObject
- soundClip
- waitSeconds
```

현재 단계에서 바로 완전 데이터 기반으로 바꾸면 오히려 복잡해진다.
먼저 Stage별 중복 입력 처리와 대사 진행 책임을 분리한다.

## 6. 변경 기록

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/02_StorySystemRoadmap.md

어떻게 수정했는가:
- 스토리 시스템 리팩토링을 CSV 대사 불러오기, 대사 진행, 입력 처리, Stage별 연출, 스토리 종료 후 씬 이동 기능 단위로 진행하도록 규칙을 추가했다.
- Stage별 Controller를 한 번에 모두 고치지 말고 하나를 검증한 뒤 반복 적용하도록 지시했다.

왜 그렇게 수정했는가:
- 스토리 흐름은 CSV 데이터와 UI 출력, 입력, 연출, 씬 이동이 연결되어 있어 파일 단위 수정만으로는 실제 플레이 흐름을 보장하기 어렵다.
- 기능 단위로 묶어야 대사 파싱은 맞지만 스토리 종료 이동이 깨지는 식의 회귀를 줄일 수 있다.

검증 방법:
- 스토리 시스템 로드맵에 기능 단위 작업 묶음이 추가됐는지 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/02_StorySystemRoadmap.md

어떻게 수정했는가:
- 실제 스토리 코드 기준으로 CSV 파싱, Dialogue 범위, InteractionController, Stage별 Controller 중복을 세분화했다.
- Stage별 공통 입력 흐름과 Actor 이동 중복을 별도 단계로 분리했다.

왜 그렇게 수정했는가:
- 기존 스토리 구조는 대사 진행과 연출이 서로 직접 얽혀 있어 한 Stage를 고치면 입력/진행 흐름까지 같이 건드릴 가능성이 크다.

검증 방법:
- DialogueParser, DatabaseManager, InteractionEvent, InteractionController, StoryStage, Stage*_Controller, PlayerMove 구조 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/02_StorySystemRoadmap.md

어떻게 수정했는가:
- 스토리 시스템 리팩토링의 기본 전제로 읽기 쉬운 대사 흐름, 쉬운 네이밍, 책임 분리를 추가했다.

왜 그렇게 수정했는가:
- 스토리는 데이터, 출력, 입력, 연출이 쉽게 섞이므로 이름과 책임을 명확히 하지 않으면 Stage가 늘어날수록 유지보수가 어려워진다.

검증 방법:
- 문서 상단에 기본 전제 섹션 추가 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/02_StorySystemRoadmap.md

어떻게 수정했는가:
- 스토리 시스템 코드 변경 전에 수정 대상 파일과 변경 범위를 먼저 보여주도록 확인 규칙을 추가했다.

왜 그렇게 수정했는가:
- 스토리 코드는 대사 진행과 연출이 얽히기 쉬워, 수정 전 어떤 흐름을 건드리는지 먼저 확인해야 한다.

검증 방법:
- 문서 상단에 코드 변경 전 확인 규칙 추가 확인

### 2026-05-30

수정 대상
- Assets/Script/Story/StoryChangeImage.cs
- Assets/Script/Story/FadeController.cs
- Assets/Script/Story/Dialogue/ClicktoMain.cs
- Assets/Script/Story/StageScenarioController/Story.cs
- Assets/RefactRoadMap/02_StorySystemRoadmap.md

어떻게 수정했는가:
- 스토리 시스템에서 UI와 직접 연결되어 있지만 아직 `View` 이름이 아닌 후보를 기록했다.
- 이번 단계에서는 파일명 변경을 적용하지 않고, 나중에 영향 범위가 작은 파일부터 제안하는 항목으로 분리했다.

추가 View 후보:
- `StoryChangeImage` -> `StoryImageView`
- `FadeController` -> `FadePanelView`
- `ClicktoMain` -> `MainSceneButtonView`
- `Story` -> `StorySkipView`

보류한 이유:
- `InteractionController`는 Text UI를 직접 다루지만 대사 진행, 클릭 입력, 씬 이동까지 함께 처리하므로 단순히 `View`로 이름만 바꾸면 책임이 더 모호해진다.
- `Stage*_Controller`는 UI 오브젝트를 켜고 끄지만 스테이지 연출 흐름을 직접 제어하므로 `View`보다 `StageNScenarioController` 또는 `StageNScenarioView` 중 책임 분리 후 결정하는 편이 안전하다.

검증 방법:
- Prologue/Epilogue 대사 출력과 클릭 진행 확인
- FadeOut 후 씬 이동 확인
- 스토리 스킵 버튼과 Confirm Alert 확인
- Stage별 이미지/말풍선/대사 박스 표시 확인

## 2026-05-30 통합 작업 요약

### 완료
- 스토리 시스템 리팩토링 원칙 정리: CSV 로드, 대사 진행, 입력 처리, Stage별 연출, 씬 이동을 기능 단위로 나누도록 정리했다.
- UI와 직접 연결되어 있지만 아직 View 이름이 아닌 후보를 별도로 기록했다.

### View 후보
| 현재 이름 | 후보 이름 | 판단 |
|---|---|---|
| StoryChangeImage | StoryImageView | 이미지 UI 표시 책임이라 우선 후보 |
| FadeController | FadePanelView | Fade 패널 표시 책임이라 우선 후보 |
| ClicktoMain | MainSceneButtonView | 클릭 UI 이동 책임이라 우선 후보 |
| Story | StorySkipView | 스토리 스킵 UI 중심이라 후보 |

### 보류
- `InteractionController`는 대사 진행, 입력, UI 출력, 씬 이동이 섞여 있어 단순 View 변경 보류.
- `Stage*_Controller`는 연출 흐름 제어가 중심이라 책임 분리 후 이름 결정.
