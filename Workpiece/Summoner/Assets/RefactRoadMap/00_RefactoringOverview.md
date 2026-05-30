# SUMMONER Refactoring Overview

## 0-A. 리팩토링 방식 선택 기준

리팩토링은 기존 코드를 억지로 유지하는 방식만 사용하지 않는다.
기능 단위로 판단해서 아래 두 방식 중 유지보수성이 더 좋은 쪽을 선택한다.

```text
1. 기존 코드 일부 교체
2. 새 컴포넌트 생성 후 기존 오브젝트 연결 이전
```

기존 코드 일부 교체가 적절한 경우:

- 변경 범위가 작다.
- Unity Inspector 연결을 거의 건드리지 않는다.
- 기존 클래스의 책임이 비교적 명확하다.
- 한두 메서드만 바꿔도 기능 흐름이 충분히 정리된다.

새 컴포넌트 생성이 적절한 경우:

- 한 클래스 안에 UI, 저장, 씬 이동, Alert, 사운드가 섞여 있다.
- 기존 클래스가 기능 이름과 맞지 않는 책임을 많이 갖고 있다.
- 같은 흐름이 여러 씬이나 컨트롤러에 반복된다.
- 새 컴포넌트를 기존 오브젝트에 붙이고 참조만 옮기는 편이 더 명확하다.

진행 규칙:

- 한 번에 클래스 전체를 갈아엎지 않는다.
- 기능 하나를 기준으로 기존 코드 수정과 새 컴포넌트 생성을 비교한다.
- 새 파일을 만들 경우 기존 파일에 넣지 않는 이유와 연결할 오브젝트를 먼저 기록한다.
- 기존 오브젝트 연결을 바꾸는 경우 `FileStateIndex.md`에 실제 사용 흐름을 갱신한다.

## 0. 리팩토링 기본 전제

모든 리팩토링은 유지보수성과 읽기 쉬운 흐름을 최우선으로 진행한다.
확장성은 필요할 때 바로 확장할 수 있을 정도만 준비하고, 처음부터 과한 구조를 만들지 않는다.

네이밍은 초등학생도 역할을 짐작할 수 있을 만큼 쉽게 작성한다.
가능한 한 `대상 + 행동` 형태를 사용한다.

좋은 이름 예시:

```text
GameProgressSave
GameSettingLoad
StageFlowSelect
DialogueCsvParse
StoryLineShow
BattleTurnStart
BattleAttackExecute
BattleTargetSelect
SummonStatusApply
SummonPickSelect
EnemyActionDecide
```

피해야 할 방향:

```text
Manager
Helper
Util
Common
DataController
SystemHandler
ProcessThing
```

기본 판단 기준:

```text
1. 이 이름만 보고 무엇을 하는지 알 수 있는가?
2. 이 파일을 처음 보는 사람이 흐름을 따라갈 수 있는가?
3. 수정했을 때 영향 범위가 작게 유지되는가?
4. 새 기능을 추가할 때 어디를 수정해야 하는지 분명한가?
5. Unity 씬과 프리팹 연결을 불필요하게 깨지 않는가?
```

### 코드 변경 전 확인 규칙

실제 코드 파일을 수정하기 전에는 반드시 수정 대상 파일을 먼저 보여준다.
사용자가 `적용해`, `수정해`, `진행해`라고 승인하기 전까지는 코드 파일을 변경하지 않는다.

작성 형식:

```text
수정 대상 파일:
- 파일 경로

수정 이유:
- 왜 이 파일을 바꾸는지 설명한다.

예상 변경 범위:
- 어떤 함수, 클래스, 책임이 바뀌는지 설명한다.

새 파일/폴더 필요 여부:
- 필요하면 왜 기존 파일에 넣지 않고 새로 만드는지 설명한다.

변경 전/후 또는 diff:
- 실제 적용 전에 변경 내용을 먼저 제안한다.
```

### 기능 단위 리팩토링 규칙

이 프로젝트의 리팩토링은 파일 단위가 아니라 기능 단위로 진행한다.
파일 하나만 깨끗하게 고치는 것보다, 사용자가 실제로 지나가는 기능 흐름이 깨지지 않는 것이 더 중요하다.

작업 전에는 반드시 아래 내용을 먼저 제시한다.

```text
수정할 기능 이름:
- 예: 새 게임 시작 저장, 스테이지 선택 저장, 전투 클리어 저장

기능 흐름:
- 사용자가 누르는 버튼이나 자동으로 실행되는 시작점
- 중간에 지나가는 주요 메서드
- 마지막으로 바뀌는 저장값, 화면, 전투 상태

수정할 파일:
- 해당 기능이 실제로 지나가는 파일 목록

수정 이유:
- 각 파일을 왜 바꾸는지 설명한다.

예상 변경 범위:
- 바뀌는 클래스, 메서드, 저장 키, 씬 이동, UI 표시를 적는다.

유지해야 할 기존 동작:
- 리팩토링 후에도 반드시 같아야 하는 동작을 적는다.

검증 방법:
- 수동 테스트
- 필요하면 자동 테스트 후보
- 필요하면 QAAgent 검사 항목
```

예를 들어 저장 시스템을 수정할 때는 `GameSaveController`만 보지 않는다.
새 게임 시작, 이어하기, 스테이지 선택, 전투 클리어, 스테이지 표시 화면까지 같은 기능 흐름으로 묶어서 확인한다.

한 번에 여러 기능을 고치지 않는다.
기능 하나를 끝내고 검증한 뒤 다음 기능으로 넘어간다.

### 파일 상태 추적 규칙

리팩토링 중 새 파일을 만들거나 기존 파일의 역할을 바꾸면 `Assets/RefactRoadMap/FileStateIndex.md`에 반드시 기록한다.
기존 코드와 새 코드가 동시에 존재하면 어떤 파일이 실제 게임 흐름에서 사용 중인지 표시한다.

파일 상태는 아래 값만 사용한다.

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

삭제 예정 코드는 참조 검색과 QA가 끝나기 전까지 바로 삭제하지 않는다.
먼저 `Deprecated`로 표시하고, 대체 파일과 검증 방법을 기록한다.

## 1. 폴더 구조

이 로드맵은 실제 코드 흐름을 기준으로 작성한다.
문서는 하나로 합치지 않고, 유지보수 책임에 따라 아래 5개로 나눈다.

```text
Assets/RefactRoadMap
  00_RefactoringOverview.md
  FileStateIndex.md
  01_GameSystemRoadmap.md
  02_StorySystemRoadmap.md
  03_BattleCoreRoadmap.md
  04_BattleContentRoadmap.md
```

각 문서의 책임은 다음과 같다.

```text
00_RefactoringOverview.md
- 전체 우선순위
- 공통 작업 규칙
- 날짜별 변경 기록 규칙

FileStateIndex.md
- 기존 코드와 새 코드 상태 추적
- Original, New, Bridge, Deprecated, Removed 구분
- 기능별 실제 연결 상태 기록

01_GameSystemRoadmap.md
- 저장
- 설정
- 메뉴
- 씬 이동
- 스테이지 진행도

02_StorySystemRoadmap.md
- CSV 대사 로드와 파싱
- 대사 진행
- 스토리 입력
- Stage별 연출
- 스토리 종료 후 씬 이동

03_BattleCoreRoadmap.md
- 턴 진행
- 마나 사용
- Plate 상태
- 공격 실행
- 상태이상
- 승패 판정

04_BattleContentRoadmap.md
- 소환수 뽑기
- 재소환
- 소환수 데이터
- 공격 전략
- 플레이어 공격 예측
- 적 AI 판단
```

## 2. 핵심 흐름

전체 리팩토링은 아래 순서로 진행한다.

```text
1. 빌드 위험 요소 제거
2. 저장/설정/씬 이동 기준 통일
3. 스토리 대사 진행과 연출 분리
4. 전투 핵심 규칙 분리
5. 소환수 데이터와 AI 판단 분리
6. 네이밍 정리
```

가장 중요한 기준은 "동작을 유지하면서 책임을 줄이는 것"이다.
기존 씬과 프리팹 연결이 많은 Unity 프로젝트이므로, 클래스명 변경이나 파일 이동은 마지막에 한다.

## 3. 주요 코드

### 현재 가장 큰 위험 지점

```text
Assets/Script/Summons/Summon.cs
- 886줄
- HP, Shield, 공격, 특수공격, 상태이상, 색상, 사운드, 애니메이션, Observer를 모두 처리한다.

Assets/Script/Battle/BattleLogic/EnermyAlgorithm.cs
- 696줄
- 플레이어 공격 예측, 적 반응 판단, 적 공격 실행이 함께 있다.

Assets/Script/Battle/BattleLogic/Player.cs
- 473줄
- 마나, 버튼 입력, 공격 실행 요청, 타겟 선택 코루틴, 승리 체크가 함께 있다.

Assets/Script/Battle/BattleLogic/Controller/PlateController.cs
- 422줄
- Plate 조회, 이동, UI 하이라이트, 투명도, 타겟 인덱스 조회가 함께 있다.

Assets/Script/Story/StageScenarioController/Stage*_Controller.cs
- Space 입력, 클릭 입력, 대사 중복 방지, scenarioFlowCount 증가가 Stage마다 반복된다.

Assets/Script/Stage/StageController.cs
Assets/Script/Stage/StageSelecter.cs
- 스테이지별 씬 이동과 Summon.multiple 설정이 중복된다.
```

### 먼저 고칠 빌드 위험 요소

런타임 스크립트에 `UnityEditor` 네임스페이스가 섞여 있다.
빌드 대상에서 문제가 될 수 있으므로 가장 먼저 제거한다.

```text
Assets/Script/Battle/BattleAlert.cs
- using UnityEditor.SceneManagement;

Assets/Script/Stage/StageController.cs
- using UnityEditor.SceneManagement;

Assets/Script/Story/Dialogue/DatabaseManager.cs
- using UnityEditor.Search;

Assets/Script/Story/Dialogue/InteractionEvent.cs
- using UnityEditor.Build;

Assets/Script/Summons/Summon.cs
- using UnityEditor.Experimental.GraphView;
```

### 공통 작업 규칙

- 한 번에 여러 시스템을 같이 고치지 않는다.
- 코드 변경 전에는 관련 로드맵에 수정 대상과 이유를 먼저 적는다.
- 기존 클래스명 변경은 마지막 단계에서 한다.
- 새 파일명은 `대상 + 행동` 형태로 짓는다.
- `manager`, `helper`, `util`, `common` 같은 모호한 이름은 새로 만들지 않는다.
- 중복 제거보다 읽히는 흐름을 우선한다.

### 날짜별 변경 기록 규칙

코드를 수정할 때는 관련 로드맵의 `변경 기록`에 아래 형식으로 기록한다.

```text
### YYYY-MM-DD

수정 대상:
- 파일 경로

어떻게 수정했는가:
- 실제 변경한 구조, 함수, 클래스, 책임을 적는다.

왜 그렇게 수정했는가:
- 유지보수성, 가독성, 책임 분리 관점의 이유를 적는다.

검증 방법:
- Unity 실행 확인, 컴파일 확인, 수동 테스트, 테스트 코드 실행 등을 적는다.
```

## 4. 실행 방법

권장 적용 순서:

```text
Phase 0. 빌드 위험 요소 제거
- 런타임 스크립트의 UnityEditor using 제거
- 사용하지 않는 using 정리

Phase 1. GameSystem 정리
- PlayerPrefs 키 상수화
- StageController와 StageSelecter의 스테이지 분기 중복 제거
- NewStart에서 PlayerPrefs.DeleteAll이 설정값까지 지우는 문제 분리

Phase 2. StorySystem 정리
- DialogueParser를 로드와 파싱으로 분리
- InteractionController에서 대사 진행과 씬 이동 분리
- Stage*_Controller 공통 입력 흐름 분리

Phase 3. BattleCore 정리
- Summon 상태이상 처리 분리
- BattleController 공격 실행 분리
- Player 마나 처리 분리
- TurnController 턴 갱신 단계 분리

Phase 4. BattleContent 정리
- SummonController 뽑기 확률 분리
- Summon 개별 클래스의 능력치 데이터화 준비
- EnermyAlgorithm 판단과 실행 분리
```

각 Phase 완료 기준:

```text
1. Unity Console 컴파일 에러 없음
2. 관련 씬에서 기존 동작 유지
3. 변경 기록 작성
4. 다음 Phase로 넘어가기 전 영향 범위 확인
```

## 5. 나중에 확장할 수 있는 부분

문서가 커지면 아래 문서를 추가한다.

```text
05_TestingRoadmap.md
06_DataMigrationRoadmap.md
07_NamingCleanupRoadmap.md
```

현재는 5개 문서가 적절하다.
전투를 `BattleCore`와 `BattleContent`로 나누면 턴 규칙과 소환수/AI 확장을 서로 덜 흔들리게 만들 수 있다.

## 6. 변경 기록

### 2026-05-30

수정 대상:
- Assets/Script/Battle/BattleAlert.cs
- Assets/Script/Battle/BattleLogic/EnermyAlgorithm.cs
- Assets/Script/Battle/BattleLogic/AttackLogic/StatusEffect.cs
- Assets/Script/Stage/StageController.cs
- Assets/Script/Story/Dialogue/DatabaseManager.cs
- Assets/Script/Summons/Summon.cs

어떻게 수정했는가:
- UTF-8 strict 디코딩에 실패하던 C# 파일 6개를 UTF-8로 변환했다.
- `StageController.cs`는 혼합 인코딩으로 일부 주석과 로그 문구가 깨져 있어 한글 문구를 복구했다.

왜 그렇게 수정했는가:
- Script 파일 안의 한글 주석과 Unity Inspector Header 문자열이 환경에 따라 깨져 보일 수 있었다.
- 전체 `Assets/Script` C# 파일을 UTF-8로 통일해야 Visual Studio, Unity, Git diff에서 인코딩 문제가 줄어든다.

검증 방법:
- `Assets/Script/**/*.cs` 전체 strict UTF-8 디코딩 확인
- 대표 파일 `BattleAlert.cs`, `StageController.cs`, `DatabaseManager.cs` 한글 출력 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/00_RefactoringOverview.md
- Assets/RefactRoadMap/FileStateIndex.md
- Assets/RefactRoadMap/FileStateIndex.md.meta

어떻게 수정했는가:
- 리팩토링 중 원본 코드와 새 코드를 구분하기 위한 `FileStateIndex.md`를 추가했다.
- 공통 로드맵 폴더 구조와 작업 규칙에 파일 상태 기록 의무를 추가했다.
- 기존 코드와 새 코드가 동시에 있을 때 실제 게임 흐름에서 어떤 파일이 사용 중인지 기록하도록 지시했다.

왜 그렇게 수정했는가:
- 리팩토링 세션이 끊기면 새로 만든 파일과 아직 사용 중인 기존 파일을 구분하기 어렵다.
- 파일 상태를 `Original`, `New`, `Bridge`, `Deprecated`, `Removed`로 기록하면 다음 작업자가 삭제 후보와 실제 사용 코드를 빠르게 파악할 수 있다.

검증 방법:
- FileStateIndex.md와 meta 파일 생성 확인
- 공통 작업 규칙에 파일 상태 기록 규칙이 들어갔는지 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/00_RefactoringOverview.md
- Assets/RefactRoadMap/01_GameSystemRoadmap.md
- Assets/RefactRoadMap/02_StorySystemRoadmap.md
- Assets/RefactRoadMap/03_BattleCoreRoadmap.md
- Assets/RefactRoadMap/04_BattleContentRoadmap.md

어떻게 수정했는가:
- 리팩토링을 파일 단위가 아니라 기능 단위로 진행하도록 공통 규칙을 추가했다.
- 작업 전 기능 이름, 기능 흐름, 수정 파일, 수정 이유, 예상 변경 범위, 유지할 기존 동작, 검증 방법을 먼저 제시하도록 지시했다.
- 각 세부 로드맵에 시스템별 기능 묶음 예시를 추가했다.

왜 그렇게 수정했는가:
- 저장, 스토리, 전투 기능은 여러 파일을 지나가므로 파일 하나만 고치면 기존 흐름이 깨질 수 있다.
- refactoring Codex가 수정 전에 기능 흐름을 먼저 보여주면 사용자가 원하는 방향과 다른 수정이 들어가는 것을 줄일 수 있다.

검증 방법:
- 로드맵 5개 문서에 기능 단위 리팩토링 규칙이 들어갔는지 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/00_RefactoringOverview.md

어떻게 수정했는가:
- 실제 코드 분석 결과를 기준으로 전체 리팩토링 우선순위를 다시 작성했다.
- 큰 파일, 중복 흐름, 런타임 빌드 위험 요소를 먼저 정리했다.

왜 그렇게 수정했는가:
- 기존 로드맵은 구조 제안 중심이었고, 실제 코드의 우선 위험 지점이 충분히 드러나지 않았다.
- 먼저 위험도가 높은 파일과 시스템 경계를 정해야 단계별 리팩토링이 가능하다.

검증 방법:
- Assets/Script 전체 C# 파일 목록 확인
- 주요 시스템 파일 내용 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/00_RefactoringOverview.md
- Assets/RefactRoadMap/01_GameSystemRoadmap.md
- Assets/RefactRoadMap/02_StorySystemRoadmap.md
- Assets/RefactRoadMap/03_BattleCoreRoadmap.md
- Assets/RefactRoadMap/04_BattleContentRoadmap.md

어떻게 수정했는가:
- 모든 로드맵에 유지보수성, 읽기 쉬운 흐름, 쉬운 네이밍을 리팩토링 기본 전제로 추가했다.
- `대상 + 행동` 네이밍 기준과 피해야 할 모호한 이름 기준을 문서화했다.

왜 그렇게 수정했는가:
- 이후 실제 코드 변경이 단순 분리 작업으로 끝나지 않고, 읽기 쉬운 구조와 명확한 이름을 기준으로 진행되게 하기 위해서다.

검증 방법:
- 로드맵 5개 문서에 공통 전제가 들어갔는지 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/00_RefactoringOverview.md
- Assets/RefactRoadMap/01_GameSystemRoadmap.md
- Assets/RefactRoadMap/02_StorySystemRoadmap.md
- Assets/RefactRoadMap/03_BattleCoreRoadmap.md
- Assets/RefactRoadMap/04_BattleContentRoadmap.md

어떻게 수정했는가:
- 코드 변경 전에는 수정 대상 파일, 수정 이유, 예상 변경 범위, 새 파일 필요 여부, 변경 전/후 또는 diff를 먼저 보여주도록 규칙을 추가했다.

왜 그렇게 수정했는가:
- 리팩토링 중 코드가 예상보다 크게 바뀌는 것을 막고, 사용자가 변경 범위를 먼저 확인한 뒤 승인할 수 있게 하기 위해서다.

검증 방법:
- 로드맵 5개 문서에 코드 변경 전 확인 규칙이 들어갔는지 확인

### 2026-05-30

수정 대상
- Assets/RefactRoadMap/00_RefactoringOverview.md
- Assets/RefactRoadMap/FileStateIndex.md
- Assets/RefactRoadMap/01_GameSystemRoadmap.md
- Assets/RefactRoadMap/02_StorySystemRoadmap.md
- Assets/RefactRoadMap/03_BattleCoreRoadmap.md
- Assets/RefactRoadMap/04_BattleContentRoadmap.md

어떻게 수정했는가:
- UI와 직접 연결된 클래스명을 `View` 계열로 변경한 현재 상태를 점검하고 로드맵에 기록했다.
- C# 코드의 타입 참조는 대부분 새 `*View` 타입으로 정리된 것으로 확인했다.
- Unity `.meta` GUID는 대부분 유지되었고, `StageSelectView`, `StageTextView`는 새 GUID가 씬에 연결된 상태로 확인했다.
- 씬/프리팹 UnityEvent `m_TargetAssemblyTypeName`에는 예전 타입명이 남아 있어 다음 작업으로 분리했다.

완료 표시:
- UI 컴포넌트 중심 클래스명 변경 상태 검사
- 변경된 `*View` 클래스 목록 문서화
- `.meta` GUID 유지 여부 확인 결과 문서화

나중에 처리할 내용:
- 씬/프리팹 UnityEvent에 남은 이전 타입명 정리
- 추가 View 후보인 `StoryChangeImage`, `FadeController`, `ClicktoMain`, `Story` 이름 변경 검토
- `Player`, `Plate`, `Summon`처럼 UI와 게임 로직이 섞인 클래스는 책임 분리 후 재검토

검증 방법:
- `Assets/Script` 하위 C# 클래스 선언 검색
- 이전 클래스명 참조 검색
- 기존/새 `.meta` GUID 비교
- 씬/프리팹 `m_TargetAssemblyTypeName` 검색

## 2026-05-30 통합 작업 요약

### 완료
- 리팩토링 작업 방식 문서화: 코드 변경 전 수정 대상, 이유, 예상 변경 범위, diff 제안 규칙을 로드맵에 반영했다.
- 파일 상태 추적 기준 추가: `Original`, `New`, `Bridge`, `Deprecated`, `Removed` 상태 기준을 정리했다.
- 기능 단위 리팩토링 원칙 추가: 파일 하나만 보지 않고 실제 사용자 흐름 기준으로 수정 범위를 정하도록 정리했다.
- UI 연결 클래스 View 이름 변경 상태 검사 완료: C# 타입 참조, `.meta` GUID, 씬/프리팹 UnityEvent 타입명을 확인했다.
- 현재 완료된 View 이름 변경 목록과 남은 작업을 각 로드맵에 반영했다.

### 확인 결과
- C# 코드 타입 참조는 대부분 새 `*View` 타입으로 정리된 상태다.
- 대부분의 `.meta` GUID는 기존 값을 유지했다.
- `StageSelectView`, `StageTextView`는 새 GUID가 씬에 연결된 상태로 확인했다.
- 씬/프리팹 UnityEvent에는 이전 타입명이 아직 남아 있어 다음 작업으로 분리했다.

### 다음 작업
- UnityEvent 타입명 정리: `Alert`, `Menu`, `Setting`, `GamePlayController`, `StageSelecter`.
- 추가 View 후보 검토: `StoryChangeImage`, `FadeController`, `ClicktoMain`, `Story`.
- UI와 전투/스토리 로직이 섞인 `Player`, `Plate`, `Summon`, `InteractionController`, `Stage*_Controller`는 이름만 바꾸지 말고 책임 분리 후 재검토한다.
