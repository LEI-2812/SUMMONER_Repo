# Active QA

현재 개발 판단에 필요한 QA만 관리한다.

완료된 QA 상세는 `.codex/archive/qa/`로 보낸다.

## 운영 기준

- `Ready`: 개발 변경 후 바로 확인할 QA
- `Needs Fix`: 코드 문제로 기대 동작이 깨진 QA
- `Blocked`: PlayMode, MCP, Console, 입력 조작 같은 환경 문제로 확인이 막힌 QA
- `검증부족`: 사전에 정한 QA 깊이 기준 중 확인하지 못한 항목이 남은 QA
- `Pass`: active 목록에서 제거하고 archive로 이동

MCP timeout, 501, WebSocket error만 있으면 게임 코드 실패로 보지 않는다.
같은 MCP 오류는 2회까지만 재시도하고, 반복되면 `Blocked`로 기록한다.
Unity가 메인 프로세스가 아니어서 발생한 Test Time exceeded는 코드 실패로 보지 않고, 같은 원인을 긴 이슈로 반복 기록하지 않는다.

## 현재 항목

| ID | 상태 | 대상 | 핵심 확인 | 다음 행동 |
|---|---|---|---|---|
| - | - | - | 현재 진행 중인 QA 항목 없음 | 다음 기능 목표가 QA필요 상태가 되면 QAReviewAgent가 새 QA를 등록 |

## 최근 완료 QA

완료 QA 상세는 `.codex/archive/qa/`를 기준으로 확인한다.
현재 파일에는 진행 중 QA만 둔다.

## 작성 형식

```md
## QA Smoke Result

대상:
기대 동작:
확인 방법:
QA 깊이 기준:
결과: Pass / 검증부족 / Needs Fix / Blocked

확인한 내용:
-

문제:
-

다음 요청:
-
```
