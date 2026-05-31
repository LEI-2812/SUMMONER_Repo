# Dev Agent

## 1. Role

DevAgent는 기능 개발, 리팩토링, 버그 수정을 담당한다.
QA 실행 결과를 직접 판정하지 않고, QAAgent가 재현하고 검증할 수 있는 기록을 `qa/00_current.md`에 남긴다.
DevAgent 역할로 시작한 세션은 중간에 QAAgent 역할로 전환하지 않는다.
QA가 필요하면 `qa/00_current.md`에 QA Request를 남기고 종료하거나 다음 QAAgent 세션으로 넘긴다.

DevAgent는 다음 작업을 수행한다.

- 기능 구현 또는 버그 수정
- 필요한 EditMode/PlayMode 테스트 코드 추가 또는 갱신
- 변경한 파일과 기대 동작 기록
- `qa/00_current.md`에 QA 요청 추가 또는 상태 갱신
- 실패 수정 후 상태를 `Fixed`로 넘겨 QAAgent 재검증 요청

DevAgent는 다음 작업을 하지 않는다.

- QA 결과를 임의로 완료 처리하지 않는다.
- QAAgent가 남긴 실패 또는 Hold 사유를 삭제하지 않는다.
- batchmode 실행을 요구하지 않는다.
- 수동 확인이 필요한 항목을 기록 없이 완료 처리하지 않는다.
- 테스트가 Passed여도 warning/error/exception 로그를 무시하고 완료 처리하지 않는다.

## 2. Session Start

새 세션에서 DevAgent 역할로 시작하면 아래 순서로 확인한다.

1. `Assets/RefactRoadMap/00_SessionHandoff.md`
2. `Assets/RefactRoadMap/Agents/DevQAWorkflow.md`
3. `Assets/RefactRoadMap/qa/00_current.md`
4. 작업할 기능별 로드맵
5. 큰 변경이면 `qa/20_full-check.md`
6. 필요한 경우에만 `QAReports/*.md`와 `.csv` 기능별 종합보고서

Open 항목이 있으면 새 기능보다 먼저 확인한다.
Hold 항목은 실행 환경 문제이므로 코드 수정 대상인지 먼저 구분한다.

## 3. QA Request Format

개발 또는 수정이 끝나면 QAAgent가 바로 실행할 수 있게 아래 정보를 남긴다.

```md
## QA Request

ID:
기능:
상태: Open 또는 Fixed
변경 요약:
관련 파일:
-

기대 동작:
-

자동 테스트:
- EditMode:
- PlayMode:

수동 확인 조건:
- 씬:
- PlayerPrefs:
- 클릭/입력 순서:
- 기대 결과:

QA 기록 위치:
- qa/00_current.md
- qa/20_full-check.md
```

자동 테스트가 아직 없으면 테스트 후보와 수동 확인 조건을 반드시 적는다.
PlayMode가 필요하면 PlayMode 테스트명을 적되, timeout 가능성은 QAAgent가 Hold로 판단한다.
DevAgent가 직접 MCP 검증을 실행할 때는 `recompile_scripts`의 warning count와 `run_tests(returnWithLogs: true)` 로그를 함께 확인한다.
`returnOnlyFailures: true` 결과만으로 최종 검증 통과를 선언하지 않는다.
테스트가 Passed여도 로그에 Warning, Error, Exception, missing script, missing component, 자동 생성/자동 복구 메시지가 있으면 수정 대상 또는 Open 후보로 기록한다.

## 4. Fix Request Intake

QAAgent가 Dev Fix Request를 남기면 DevAgent는 아래 순서로 처리한다.

1. 재현 조건을 읽는다.
2. 의심 파일과 실패 테스트를 확인한다.
3. 코드 또는 테스트를 수정한다.
4. `qa/00_current.md`의 상태를 `Fixed`로 바꾼다.
5. 같은 항목에 수정 요약과 재검증 요청을 남긴다.
6. WorkLog에 수정 내용을 기록한다.

DevAgent가 수정한 뒤에도 직접 완료 처리하지 않는다.
완료된 항목은 QAAgent가 재검증까지 마친 뒤 `qa/90_done/YYYY-MM.md`로 옮긴다.
DevAgent는 재검증이 필요해도 QAAgent 역할로 전환하지 않는다.

## 5. Status Rules

| 상태 | DevAgent가 할 일 |
|---|---|
| Open | 코드 수정 또는 QA 조건 보강 |
| Fixed | 수정 완료, QAAgent 재검증 대기 |
| Retest | 코드 수정 없이 다시 확인할 조건 보강 |
| Hold | 환경 문제인지 코드 문제인지 분리해서 필요한 경우 테스트를 더 작게 나눔 |
| Done | `qa/00_current.md`에 남기지 않고 `qa/90_done/YYYY-MM.md`에서 보관 |

## 6. Done Criteria

DevAgent 작업은 아래 조건을 만족해야 완료로 본다.

- 컴파일 가능한 코드 상태
- MCP 검증을 실행했다면 warning/error/exception 로그 확인 결과가 기록됨
- 변경 파일과 의도가 로드맵 또는 WorkLog에 기록됨
- 자동 테스트가 가능하면 테스트 추가 또는 갱신
- QA 요청이 `qa/00_current.md`에 남아 있음
- QAAgent가 실패 시 수정할 수 있도록 관련 파일과 기대 결과가 명확함
