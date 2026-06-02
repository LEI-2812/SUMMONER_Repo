# <FeatureName> Roadmap

## 1. Role

이 시스템이 게임에서 맡는 역할을 짧게 적는다.
현재 QA 상태나 버그 우선순위는 이 문서에 두지 않고 `qa/current.md`에서 관리한다.

## 2. Ownership

| 영역 | 주요 파일/폴더 | 책임 |
|---|---|---|
|  | `Assets/...` |  |

## 3. Current Structure

- 현재 구조에서 유지해야 하는 핵심 흐름을 적는다.
- 주요 클래스가 게임 안에서 맡는 역할을 적는다.
- 이미 분리된 책임과 아직 섞여 있는 책임을 구분한다.

## 4. Refactoring Direction

- 앞으로 어떤 책임을 나눌지 적는다.
- 이름 기준, 의존성 방향, 테스트 가능한 단위를 적는다.
- 큰 방향만 적고, 오늘 처리할 QA 티켓은 적지 않는다.

## 5. Constraints

- 씬 이름, 저장 키, 프리팹 연결, UnityEvent, Inspector 참조처럼 깨면 안 되는 요소를 적는다.
- 기존 플레이 동작이 바뀌는 결정은 여기에서 명시한다.

## 6. Next Work Candidates

| ID | 작업 | 목적 |
|---|---|---|
| `<FEATURE>-REF-01` |  |  |

## 7. Done Criteria

- 이 시스템이 안정됐다고 볼 수 있는 기준을 적는다.
- QA 통과 여부가 아니라 구조와 동작 기준을 적는다.

## 8. QA Links

- 현재 QA 큐: `qa/current.md`
- 큰 변경 후 전체 확인: `qa/full-check.md`
- 기능별 QA 종합보고서: `QAReports/*`
