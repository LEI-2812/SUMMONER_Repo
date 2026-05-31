# DevAgent and QAAgent Workflow

## 1. Purpose

이 문서는 세션을 새로 시작해도 개발과 QA가 같은 방식으로 분리되어 동작하게 하는 운영 규칙이다.

- DevAgent: 개발, 리팩토링, 버그 수정, QA 요청 작성
- QAAgent: MCP 기반 자동 QA 실행, 결과 기록, 실패 리포트 작성

한 세션에서 DevAgent와 QAAgent 역할은 절대 서로 전환하지 않는다.
이 문서의 전환 규칙은 QA 항목의 상태와 담당 단계가 바뀌는 규칙이며, 에이전트 역할 자체를 바꾸는 규칙이 아니다.

batchmode는 사용하지 않는다.
QA 자동화는 MCP `recompile_scripts`와 MCP `run_tests`만 사용한다.
검증 결과는 실패 여부뿐 아니라 warning, error, exception, 자동 복구 로그까지 확인한다.

## 2. Source of Truth

| 문서 | 역할 |
|---|---|
| `00_SessionHandoff.md` | 세션 시작 인계 |
| `Agents/DevAgent.md` | 개발 에이전트 규칙 |
| `Agents/QAAgent.md` | QA 에이전트 규칙 |
| `Agents/DevQAWorkflow.md` | DevAgent와 QAAgent 역할 고정, QA 상태와 담당 단계 전환 규칙 |
| `qa/00_current.md` | 현재 수정 중이거나 다시 확인해야 하는 QA 큐 |
| `qa/20_full-check.md` | 큰 수정 후 전체 회귀 확인 목록 |
| `qa/90_done/YYYY-MM.md` | 완료된 QA 보관 |
| `QAReports/*.md` / `.csv` | 기능별 QA 종합보고서와 이전 상세 기록. 현재 큐 관리 금지 |
| `WorkLogs/YYYY-MM-DD.md` | 날짜별 공용 실행 로그. 섹션 제목에 역할 접두사 필수 |

## 3. Normal Flow

1. DevAgent가 기능을 개발하거나 버그를 수정한다.
2. DevAgent가 테스트 코드와 QA 요청을 남긴다.
3. DevAgent가 `qa/00_current.md`에 `Open` 또는 `Fixed` 항목을 만든다.
4. QAAgent가 `qa/00_current.md` 우선순위대로 MCP 테스트를 실행한다.
5. QAAgent가 실패하면 `Open`, 환경 문제면 `Hold`, 재확인 필요면 `Retest`로 기록한다.
6. 실패하면 QAAgent가 Dev Fix Request를 작성한다.
7. DevAgent가 Fix Request를 보고 수정한 뒤 `Fixed`로 넘긴다.
8. QAAgent가 재검증하고 통과하면 `qa/90_done/YYYY-MM.md`로 옮긴다.

## 4. QA Execution Rules

QAAgent는 아래 순서로만 자동화한다.

1. MCP `recompile_scripts`
2. MCP `run_tests` EditMode 클래스 단위 실행
3. MCP `run_tests` PlayMode 클래스 단위 실행
4. PlayMode timeout 또는 connection 오류가 나면 `Hold`로 기록

`recompile_scripts`는 warning count와 logs를 확인한다.
`run_tests`는 가능하면 `returnWithLogs: true`로 실행하고, `returnOnlyFailures: true`만으로 완료 판정하지 않는다.
테스트가 모두 Passed여도 로그에 Warning, Error, Exception, missing script, missing component, 자동 생성/자동 복구 메시지가 있으면 완료로 기록하지 않는다.
해당 로그가 코드나 씬 연결 문제면 `Open`, MCP/Unity 연결 문제면 `Hold`로 분리한다.

금지 사항:

- Unity batchmode 사용 금지
- QA 실패를 근거 없이 코드 수정으로 바로 처리 금지
- 전체 PlayMode를 무리하게 반복 실행 금지
- `Hold`를 코드 실패와 섞어 기록 금지

## 5. Status and Owner Transition

아래 표는 QA 항목의 상태와 다음 담당 단계만 정의한다.
DevAgent가 QAAgent로 바뀌거나 QAAgent가 DevAgent로 바뀌는 역할 전환은 허용하지 않는다.

| 이전 상태 | 담당 | 다음 상태 | 조건 |
|---|---|---|---|
| Open | QAAgent | Open | 기능 기대 결과 불일치. Dev Fix Request 추가 |
| Open | QAAgent | Hold | MCP, Unity Editor, PlayMode timeout 등 환경 문제 |
| Open | QAAgent | Done 보관 | 자동 또는 수동 QA 통과, 관련 로그에 예상치 못한 warning/error/exception 없음 |
| Open | DevAgent | Fixed | 수정 완료, 재검증 요청 |
| Fixed | QAAgent | Done 보관 | 재검증 통과, 관련 로그에 예상치 못한 warning/error/exception 없음 |
| Fixed | QAAgent | Open | 재검증 실패. Dev Fix Request 추가 |
| Retest | QAAgent | Done 보관 | 재확인 통과 |
| Retest | QAAgent | Open | 재확인 실패 |
| Hold | QAAgent | Retest | 실행 경로가 복구되어 재시도 가능 |

완료 항목을 `qa/90_done/YYYY-MM.md`로 옮기는 작업은 QAAgent만 한다.
DevAgent는 수정 완료 후 `Fixed`까지만 기록한다.
각 에이전트는 자신의 역할 범위 밖 작업이 필요하면 상태와 요청을 남기고, 다른 역할의 작업을 대신 수행하지 않는다.

## 6. Dev Fix Request Format

QAAgent는 실패 시 개발자가 바로 수정할 수 있게 아래 형식으로 남긴다.

```md
## Dev Fix Request

ID:
상태: Open
재현 조건:
-

실행한 테스트:
-

기대 결과:
-

실제 결과:
-

실패 메시지 또는 로그:
-

의심 파일:
-

수정 방향:
-

재검증 방법:
-
```

환경 문제는 Dev Fix Request가 아니라 `Hold` 기록으로 남긴다.
다만 테스트를 더 작게 쪼개야 하는 경우에는 DevAgent에게 테스트 구조 개선 요청을 남길 수 있다.

## 7. WorkLog Role Prefix Rule

`WorkLogs/YYYY-MM-DD.md`는 날짜별 공용 로그다.
같은 날짜 파일 안에 DevAgent 기록과 QAAgent 기록이 함께 들어갈 수 있다.
이것은 역할 전환을 의미하지 않는다.
WorkLog는 날짜별 파일 하나만 사용한다.
새 파일이 필요하면 `Assets/RefactRoadMap/WorkLogs/YYYY-MM-DD.md` 형식으로만 만들고, `Assets/RefactRoadMap/WorkLogs/WorkLogTemplate.md` 형식을 따른다.
작업명이나 티켓 ID를 파일명에 붙인 `YYYY-MM-DD_<작업명>.md` 파일은 만들지 않는다.

WorkLog는 프로젝트를 모르는 사람이 빠르게 읽는 요약 문서다.
프로젝트 내부 약어만 쓰지 말고, 해당 기능이 게임에서 무엇을 하는지 먼저 설명한다.
최소한 Unity와 C#을 배운 학부생이 읽고 기능의 목적, 기존 문제, 변경 후 구조, 테스트 결과, 남은 작업을 이해할 수 있어야 한다.
상세 파일 목록, 긴 테스트 출력, 구현 세부 단계, 검색 결과는 WorkLog에 붙이지 않는다.

작성 기준:

- 내부 약어와 프로젝트 고유 명칭은 처음 등장할 때 풀어쓴다.
- 클래스명만 쓰지 말고 게임 안에서의 역할을 함께 설명한다.
- “정리”, “분리”, “수정” 같은 표현만 쓰지 말고 변경 전후 차이를 적는다.
- 테스트 결과는 통과 숫자와 함께 무엇을 확인했는지 적는다.
- 미확인 항목은 완료처럼 기록하지 않는다.

날짜별 WorkLog 안의 큰 섹션 제목은 아래 형식 중 하나로 시작해야 한다.

```md
## DevAgent - 작업명
## QAAgent - 작업명
```

날짜별 WorkLog는 아래 7개 섹션을 사용한다.

1. 작업 요약
2. 작업 배경
3. 작업 내용
4. 확인한 내용
5. 발생한 문제와 해결
6. 남은 작업
7. 참고 메모

각 작업 항목에는 역할 고정 여부를 적는다.
DevAgent 작업이면 `DevAgent 작업 기록. QAAgent 역할 전환 아님.`을 적는다.
QAAgent 작업이면 `QAAgent 고정. 코드 수정 없음.`을 적는다.

기존 날짜 파일에 다른 역할의 기록이 이미 있더라도, 현재 에이전트는 자신의 역할 접두사가 붙은 새 섹션만 추가한다.
작업별 세부 기록이 필요하면 별도 파일을 만들지 말고 해당 날짜 파일의 `작업 내용` 또는 `참고 메모`에 짧게 요약한다.

## 8. QA Request Quality Gate

DevAgent의 QA 요청은 아래 항목 중 최소 하나를 포함해야 한다.

- 실행할 EditMode 테스트 fullName 또는 class name
- 실행할 PlayMode 테스트 fullName 또는 class name
- 수동 QA 씬 이름과 클릭/입력 순서
- PlayerPrefs 초기값과 기대 저장값
- 실패 재현 조건과 의심 파일

아무 실행 조건이 없는 QA 요청은 QAAgent가 `Hold` 또는 `Needs QA Spec` 메모로 되돌린다.

## 9. Current Project Policy

현재 SUMMONER 프로젝트에서는 다음 정책을 적용한다.

- EditMode 테스트는 MCP로 우선 실행한다.
- PlayMode 테스트는 MCP로 클래스 단위 실행만 시도한다.
- PlayMode timeout은 QA 인프라 차단으로 보고 `Hold` 처리한다.
- 런타임 씬 조작이 필요한 항목은 가능한 한 PlayMode 테스트 후보로 쪼갠다.
- 수동 QA가 필요한 항목도 `qa/00_current.md`에 재현 조건과 기대 결과를 남긴다.
