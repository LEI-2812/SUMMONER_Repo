# SUMMONER Refactoring Overview

## 1. Purpose

This folder is the working map for SUMMONER refactoring.
Roadmap files show the current direction and the next decisions.
They are not cumulative work logs.

## 2. Documentation Rules

Keep each document focused on one job.

```text
00_RefactoringOverview.md
- 전체 우선순위
- 공통 리팩토링 규칙
- 문서 관리 규칙
- 현재 통합 상태

01_GameSystemRoadmap.md
- 저장, 설정, 메뉴, 씬 이동, 스테이지 진행도

02_StorySystemRoadmap.md
- CSV 대사, 대사 진행, 입력, 연출, 스토리 씬 이동

03_BattleCoreRoadmap.md
- 턴, 마나, Plate, 공격 실행, 상태이상, 승패 판정

04_BattleContentRoadmap.md
- 소환수 뽑기, 소환수 데이터, 공격 전략, 예측, 적 AI

FileStateIndex.md
- 파일별 현재 사용 상태
- Original, New, Bridge, Deprecated, Removed 구분

QAReports/*.csv
- Refactor Agent와 QA Agent의 공유 작업 큐
- Pending QA, Running, Pass, Fail, Blocked 상태 기록

AutoRefactorMode.md
- 리팩토링 에이전트 실행 규칙
- 코드 수정, 테스트 코드 작성, 기록 기준
- QA 에이전트가 실행할 테스트/QA 요청 작성 기준

Agents/QAAgent.md
- QA 실행 전담 역할
- 리팩토링 에이전트가 작성한 테스트 코드 실행
- Unity Test Runner, PlayMode, 씬/Inspector 확인
- QAReports 결과 기록과 실패 우선순위 등록 기준

WorkLogs/YYYY-MM-DD.md
- 실제 작업 세부 로그
- 수정 파일, 변경 내용, 검증 결과, 도구 오류

CompletedArchive.md
- 완료된 작업의 누적 보관
```

로드맵에는 아래 내용만 남긴다.

```text
- Current Phase
- Goal
- Active Tasks
- Done Criteria
- Latest Summary
- Log Links
```

로드맵에 넣지 않는다.

```text
- 개별 파일 수정 세부 내역
- 긴 테스트 출력
- 같은 날짜의 반복 변경 기록
- 이미 완료된 작업의 장문 설명
```

세부 기록은 `WorkLogs/YYYY-MM-DD.md`에 남긴다.
완료된 작업은 `CompletedArchive.md`에 요약해서 보관한다.
QA 실행 대기/결과 상태는 `QAReports/*.csv`에만 기록한다.

## 3. Refactoring Principles

모든 리팩토링은 유지보수성과 읽기 쉬운 흐름을 우선한다.
확장성은 필요할 때 바로 확장할 수 있을 정도만 준비한다.

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

## 4. Naming Rules

네이밍은 초등학생도 역할을 짐작할 수 있을 만큼 쉽게 작성한다.
가능한 한 `대상 + 행동` 형태를 사용한다.

Good examples:

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

Avoid:

```text
Manager
Helper
Util
Common
DataController
SystemHandler
ProcessThing
```

## 5. Code Change Rules

기본 모드에서는 실제 코드 파일을 수정하기 전에 수정 대상 파일을 먼저 보여준다.
사용자가 `적용해`, `수정해`, `진행해`라고 승인하기 전까지는 코드 파일을 변경하지 않는다.

단, 사용자가 자동 진행을 명시한 경우에는 `AutoRefactorMode.md`를 우선 적용한다.
자동 모드에서는 로드맵 확인, 코드 수정, 테스트 코드 작성, QA 요청 기록을 반복 진행하되 위험 작업은 다시 확인한다.
실제 QA 실행은 `Agents/QAAgent.md` 기준의 QA 에이전트가 수행한다.

작업 전에는 아래 내용을 확인한다.

```text
수정할 기능 이름
기능 흐름
수정할 파일
수정 이유
예상 변경 범위
유지해야 할 기존 동작
검증 방법 또는 QA 요청
```

한 번에 여러 기능을 고치지 않는다.
기능 하나를 끝내면 테스트 코드와 QA 요청을 남긴 뒤 다음 기능으로 넘어간다.
QA 에이전트가 Fail로 기록한 항목은 다음 작업 우선순위 최상단에 둔다.

## 6. Current Priority

전체 적용 순서:

```text
Phase 0. 빌드 위험 요소 제거
Phase 1. GameSystem 정리
Phase 2. StorySystem 정리
Phase 3. BattleCore 정리
Phase 4. BattleContent 정리
```

현재 통합 상태:

| Area | Status | Next |
|---|---|---|
| Documentation | 역할 분리 완료 | 리팩토링 에이전트와 QA 에이전트 책임 유지 |
| GameSystem | 진행도 읽기 흐름 정리됨 | 수동 QA 대신 MCP PlayMode 검증 복구/실행 |
| StorySystem | View 이름, StorySceneMove, DialogueLineShow, StoryProgressAdvance, DialogueParser 분리됨, Stage 입력 공통화 적용 | 수동 QA 대신 MCP EditMode 검증 복구/실행 |
| BattleCore | 계획 정리됨 | Summon 상태이상, Player, Plate 책임 분리 |
| BattleContent | 계획 정리됨 | SummonController 뽑기 흐름 분리 |

## 7. High Risk Files

```text
Assets/Script/Summons/Summon.cs
- HP, Shield, 공격, 특수공격, 상태이상, 색상, 사운드, 애니메이션, Observer를 모두 처리한다.

Assets/Script/Battle/BattleLogic/EnermyAlgorithm.cs
- 플레이어 공격 예측, 적 반응 판단, 적 공격 실행이 함께 있다.

Assets/Script/Battle/BattleLogic/Player.cs
- 마나, 버튼 입력, 공격 실행 요청, 타겟 선택 코루틴, 승리 체크가 함께 있다.

Assets/Script/Battle/BattleLogic/Controller/PlateController.cs
- Plate 조회, 이동, UI 하이라이트, 투명도, 타겟 인덱스 조회가 함께 있다.

Assets/Script/Story/Dialogue/InteractionController.cs
- 대사 진행, Text UI 출력, FadeOut, 스토리 종료 씬 이동이 함께 있다.
```

## 8. Active Tasks

| ID | Task | Area | Status | Notes |
|---|---|---|---|---|
| QA-MCP-00 | MCP run_tests/recompile_scripts timeout 원인 수정 | QA | Highest | 다음 작업 최우선, 수동 QA 대신 MCP로 진행 |
| GS-QA-01 | 새 게임/이어하기/스테이지/전투 클리어 저장 QA 요청 | GameSystem | Pending QA | QA 에이전트 담당 |
| ST-02 | InteractionController 대사 진행과 화면 출력 책임 분리 | StorySystem | Completed | `StoryProgressAdvance`, `DialogueLineShow` 연결됨 |
| ST-04 | DialogueParser 로드/파싱 분리 | StorySystem | Completed | `DialogueCsvParse`, `DialogueDatabaseLoad` 연결됨 |
| ST-05 | Stage1 입력 공통화 | StorySystem | Pending QA | QA 에이전트가 실행 검증 |
| ST-06 | Stage2 입력 공통화 | StorySystem | Pending QA | QA 에이전트가 실행 검증 |
| ST-07 | Stage3 입력 공통화 | StorySystem | Pending QA | QA 에이전트가 실행 검증 |
| ST-08 | Stage5 입력 공통화 | StorySystem | Pending QA | QA 에이전트가 실행 검증 |
| ST-09 | Stage7 입력 공통화 | StorySystem | Pending QA | QA 에이전트가 실행 검증 |
| BC-01 | Summon 상태이상 적용/갱신/표시 분리 | BattleCore | Next | QA 기준 먼저 확인 |
| BT-01 | SummonController 뽑기 확률/후보/패널 표시 분리 | BattleContent | Next | Battle Core 상태 변경은 건드리지 않음 |

## 9. Done Criteria

각 Phase 완료 기준:

```text
1. Unity Console 컴파일 에러 없음
2. 관련 씬에서 기존 동작 유지
3. 테스트 코드 또는 QAReports 요청 항목 갱신
4. FileStateIndex.md 갱신
5. WorkLogs/YYYY-MM-DD.md에 상세 기록
6. Roadmap에는 최신 요약 3줄 이하만 유지
```

## 10. Latest Summary

2026-05-30: StorySystem의 단순 View 이름 변경, 스토리 종료 씬 이동, 대사 Text UI 출력, Dialogue/Line 진행 책임 분리를 완료했다.
리팩토링 에이전트는 코드 수정, 테스트 코드 작성, FileStateIndex/WorkLogs/QA 요청 기록까지 담당한다.
QA 에이전트는 그 테스트와 QA 요청을 실행하고, 실패를 다음 리팩토링 우선순위로 기록한다.
두 에이전트의 공유 큐는 `QAReports/*.csv`이며, Roadmap은 현재 우선순위만 유지한다.
