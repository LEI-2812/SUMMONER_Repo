# Auto Refactor Mode

## 1. Purpose

이 문서는 사용자가 자동 리팩토링을 원할 때 사용하는 진행 규칙이다.
평소에는 `00_RefactoringOverview.md`의 코드 변경 전 승인 규칙을 따른다.

사용자가 아래처럼 명시한 경우에만 이 문서가 우선한다.

```text
자동 리팩토링 모드로 진행해
AutoRefactorMode 기준으로 진행해
로드맵 확인부터 수정, 테스트 코드 작성, 기록까지 자동으로 반복해
```

## 2. Automatic Flow

자동 모드에서는 아래 흐름을 사용자 승인 대기 없이 진행한다.

```text
1. Assets/RefactRoadMap 문서 확인
2. 다음 리팩토링 후보 선정
3. 관련 코드와 기존 동작 흐름 확인
4. 작은 기능 단위로 코드 수정
5. 기존 로직 흐름을 확인할 테스트 코드 또는 QA 요청 항목 작성
6. 정적 검색과 파일 기반 확인 수행
7. FileStateIndex, QAReports, WorkLogs 갱신
8. Roadmap의 Current Phase, Active Tasks, Latest Summary만 갱신
9. 다음 후보가 있으면 사용자에게 보고하지 않고 같은 흐름 반복
```

한 번에 하나의 기능만 수정한다.
파일 하나가 아니라 실제 플레이 흐름 하나를 기준으로 수정한다.
사용자가 인터럽트하거나, 이 문서의 중단 기준에 걸리기 전까지 계속 반복한다.

## 3. Agent Role Split

리팩토링 에이전트와 QA 에이전트의 책임을 분리한다.

리팩토링 에이전트가 맡는 일:

```text
- 로드맵에서 다음 리팩토링 후보 선정
- 관련 코드 흐름 확인
- 코드 리팩토링 적용
- 필요한 EditMode/PlayMode 테스트 코드 작성 또는 갱신
- 테스트로 다루기 어려운 부분은 QAReports에 수동 QA 요청 항목 추가
- FileStateIndex와 WorkLogs에 변경 내용 기록
- 정적 검색, 파일 내용 확인, 컴파일 위험 확인
```

리팩토링 에이전트가 맡지 않는 일:

```text
- Unity Test Runner 실행
- MCP run_tests 실행
- PlayMode/씬 실행 QA
- Inspector 연결 수동 확인
- QA 통과/실패 최종 판정
```

QA 에이전트가 맡는 일:

```text
- 리팩토링 에이전트가 작성한 테스트 코드 확인
- Unity Test Runner 실행
- MCP run_tests 또는 Unity Editor 직접 QA 실행
- PlayMode와 씬 이동 확인
- Inspector 연결 상태 확인
- QAReports에 Pass/Fail/Blocked 기록
- 실패 시 재현 조건, 실패 메시지, 관련 스크린/씬/파일 기록
```

리팩토링 에이전트는 테스트 코드를 작성해 QA 에이전트가 바로 실행할 수 있게 넘긴다.
QA 에이전트가 테스트나 수동 QA에서 Fail을 기록하면, 그 실패 수정은 다음 리팩토링 후보보다 항상 우선한다.

## 4. Efficient Agent Loop

두 에이전트는 `QAReports/*.csv`를 공유 큐로 사용한다.
Roadmap은 방향과 우선순위만 보여주고, WorkLogs는 세부 기록만 보관한다.

권장 루프:

```text
1. Refactor Agent
   -> 작은 기능 하나 리팩토링
   -> 관련 테스트 코드 작성/갱신
   -> 테스트로 다루기 어려운 항목은 QAReports에 Pending QA 등록
   -> WorkLogs에 변경 요약과 실행할 테스트 이름 기록

2. QA Agent
   -> Pending QA만 읽음
   -> 리팩토링 에이전트가 작성한 테스트 실행
   -> 필요한 씬/Inspector/PlayMode 확인
   -> QAReports에 Pass / Fail / Blocked 기록

3. Refactor Agent
   -> Fail이 있으면 새 작업보다 실패 수정 우선
   -> Fail이 없으면 다음 작은 리팩토링 진행
```

상태값은 아래 값만 사용한다.

```text
Refactor status:
- Todo
- In Progress
- Ready For QA
- Fixing QA Fail
- Done

QA status:
- Pending QA
- Running
- Pass
- Fail
- Blocked
```

효율 규칙:

```text
- 리팩토링 단위는 작게 유지한다.
- EditMode 테스트는 가능하면 묶어서 실행한다.
- PlayMode 테스트는 씬 로드가 무거우므로 기능별/씬별로 나눠 실행한다.
- Fail은 즉시 다음 리팩토링 우선순위 최상단으로 올린다.
- MCP, Unity Editor 상태, 외부 실행 문제는 Blocked로 기록하고 코드 실패와 구분한다.
- Roadmap에는 Ready For QA, Fail, Done 같은 현재 상태만 반영한다.
```

## 5. Documentation Update Rule

로드맵 문서는 누적 작업 로그가 아니다.

자동 모드에서 작업 후 기록 위치:

```text
Roadmap
- Current Phase
- Active Tasks
- Done Criteria
- Latest Summary
- 다음 작업 후보

WorkLogs/YYYY-MM-DD.md
- 수정 대상
- 어떻게 수정했는가
- 왜 그렇게 수정했는가
- 검증 방법
- 실패 또는 미검증 사유

CompletedArchive.md
- 완료된 작업 요약
- 나중에 다시 볼 필요가 있는 결정

FileStateIndex.md
- 파일 상태 변경
- 실제 연결 상태
- Deprecated/Removed 후보

QAReports/*.csv
- QA 요청, QA 실행 결과, 실패/보류 상태
```

금지:

```text
- 같은 날짜의 변경 기록을 Roadmap에 계속 추가
- 완료된 작업 상세를 Active Tasks 아래에 누적
- 긴 테스트 출력이나 rg 결과를 Roadmap에 붙여넣기
- 이미 처리한 View 이름 변경 후보를 다음 작업 후보에 계속 남기기
```

`Latest Summary`는 최대 3줄로 유지한다.
같은 항목을 반복 추가하지 말고 기존 task row의 Status/Notes를 갱신한다.

## 6. Automatically Allowed Work

아래 조건을 모두 만족하면 바로 수정한다.

```text
- 로드맵에 이미 후보로 기록된 작업이다.
- 기존 동작을 유지하는 리팩토링이다.
- 수정 범위가 명확하고 작다.
- 씬/프리팹 연결을 대량으로 바꾸지 않는다.
- 저장 키, 저장 데이터 구조, 씬 이름을 바꾸지 않는다.
- 삭제보다 이름 변경, 참조 정리, 책임 분리, 테스트 추가에 가깝다.
```

예시:

```text
- View 후보 파일 하나 이름 정리
- 불필요한 using 제거
- PlayerPrefs 직접 읽기를 이미 있는 저장 컨트롤러 경유로 변경
- 중복된 작은 메서드 추출
- 기존 흐름을 검증하는 PlayMode/EditMode 테스트 코드 추가
- QAReports에 QA 요청 또는 Pending QA 상태 갱신
```

## 7. Stop And Ask

아래 경우에는 자동 모드여도 사용자 확인을 받는다.

```text
- 씬 또는 프리팹 연결을 여러 개 대량 수정한다.
- 여러 클래스/파일을 한 번에 rename한다.
- 저장 데이터 구조, PlayerPrefs 키, 세이브 호환성을 바꾼다.
- 기존 게임 동작을 의도적으로 바꾼다.
- 삭제, 대량 이동, git reset 같은 되돌리기 어려운 작업이 필요하다.
- QA 실행 결과가 기존 동작과 충돌해서 제품 의도 판단이 필요하다.
- 로드맵에 없는 새 구조를 크게 도입한다.
```

멈출 때는 아래 형식으로 보고한다.

```text
자동 진행 중단 이유:
- 이유

확인이 필요한 결정:
- 사용자에게 필요한 선택

추천 방향:
- 권장 선택과 이유
```

## 8. QA Handoff Rule

코드 수정 후 리팩토링 에이전트는 기존 로직 흐름을 확인하는 테스트 코드를 작성한다.
테스트 코드로 다루기 어려운 씬 조작, Inspector 연결, 실제 플레이 흐름은 QA 요청으로 남긴다.
실제 QA 실행과 결과 기록은 QA 에이전트가 수행한다.

리팩토링 에이전트의 우선순위:

```text
1. 자동 테스트가 가능한 경우 PlayMode/EditMode 테스트 코드 추가 또는 갱신
2. 자동 테스트가 과하면 QAReports CSV에 수동 QA 요청 항목 추가 또는 갱신
3. 변경한 기능, 예상 기존 동작, 실행해야 할 테스트 이름을 WorkLogs에 기록
4. QA 실행이 필요한 항목은 Pending QA로 남기고 다음 코드 작업으로 넘어간다
```

QA 에이전트의 실행 결과가 Fail이면 리팩토링 에이전트는 다른 리팩토링 후보보다 해당 실패 수정을 우선한다.
Unity Editor 실행, batchmode 실행, MCP run_tests 실행 여부는 QA 에이전트가 결정한다.
씬 실행, Inspector 연결, PlayMode 확인이 필요한 변경은 QA 요청 항목으로 남긴다.

## 9. Current Default Candidates

현재 로드맵 기준 자동 진행 후보는 아래 순서로 본다.

```text
1. QA 에이전트가 기록한 실패 항목 수정
2. StorySystem InteractionController 책임 분리
3. StorySystem 대사 진행/이미지 표시 흐름 테스트 코드 또는 QA 요청 추가
4. BattleCore의 Summon 상태이상 책임 분리
5. BattleContent의 소환수 뽑기 흐름 작은 단위 분리
```

QA 실패 기록이 있으면 위 기본 후보보다 실패 수정이 항상 먼저다.
