# Workflow

이 폴더는 현재 작업판이다.

현재 개발 흐름에서 다음 행동을 판단하는 데 필요한 파일만 둔다.

## 파일 역할

- `issue-board.md`: 전체 이슈와 QA 요약 상태
- `current-task.md`: 현재 진행 중인 기능 하나의 구현 상태, 기능 목표, 완료 조건, 다음 행동, 주의점
- `agent-status.md`: 에이전트별 현재 초점과 인계 상태
- `automation-rule.md`: Codex가 Ready 에이전트를 실제 수행하며 반복하는 자동 루프 기준

## 기록 책임 요약

- FlowAgent는 작업 범위와 인계를 정리하고 `current-task.md`, `issue-board.md`, `agent-status.md`를 갱신한다.
- DevAgent는 개발 결과를 반영하고 `current-task.md`의 완료한 개발, 남은 개발, QA 전환 판단을 갱신한다.
- QAReviewAgent는 QA 설계와 결과를 `active-qa.md`, `current-task.md`, `issue-board.md`, `agent-status.md`에 갱신한다.
- 완료된 DEV/QA 이력은 `.codex/records/`의 CSV에 한 줄씩 추가한다.
- 기능 개발 완료, QA 완료, 구조 결정, 작업 정책 변경은 `.codex/dev-log/YYYY-MM-DD.md`에 짧게 남긴다.
- 파일 갱신 없이 대화 보고만 끝난 작업은 에이전트 완료로 보지 않는다.

## current-task 작성 기준

`current-task.md`는 기능 구현 상태를 가장 먼저 보여준다.

```md
## 기능 구현 상태

- 기능 상태: 시작전 / 진행중 / 동작완료 / 검증대기 / QA필요 / QA진행중 / QA완료 / 구현완료 / 보류 / 후속작업대기
- 설계 품질 상태: 미검토 / 적정 / 구조개선필요 / 보류
- QA 깊이 상태: 미정의 / 기준정의완료 / 검증대기 / 검증완료 / 검증부족
- 기능 목표:
- 기능 완료 조건:
- 설계 품질 기준:
- QA 깊이 기준:
- 현재 완료한 개발:
- 남은 개발:
- QA 호출 조건:
- QA 기준 범위:
- 다음 QA 확인 항목:
- QA 전환 판단:
```

상태 기준:

- `시작전`: 기능 목표만 정해졌고 개발을 시작하지 않은 상태
- `진행중`: DevAgent가 기능 목표 완료 전 개발을 진행 중
- `동작완료`: 기능 동작은 연결됐지만 설계 품질 또는 QA 깊이 기준 확인이 남은 상태
- `검증대기`: 기능 개발이 아직 끝나지 않아 QA를 미룬 상태
- `QA필요`: 기능 구현과 설계 품질 기준은 충족했고 QAReviewAgent로 넘겨야 하는 상태
- `QA진행중`: 기능 목표가 완료되어 QAReviewAgent가 검증 중
- `QA완료`: QAReviewAgent 검증은 끝났고 FlowAgent 정리 대기 상태
- `구현완료`: 기능 동작, 설계 품질, QA 깊이 기준, FlowAgent 정리가 모두 끝난 상태
- `보류`: 사용자 결정이나 외부 조건 때문에 멈춘 상태
- `후속작업대기`: 현재 진행 중인 기능이 없고 다음 작업 선정 대기

`구현완료`는 기능 상태, 설계 품질 상태, QA 깊이 상태가 모두 완료 기준을 만족할 때만 사용한다.
기능이 동작해도 설계 품질이 `구조개선필요`이거나 QA 깊이가 `검증부족`이면 구현완료로 보지 않는다.

## 운영 기준

- 긴 과거 이력은 이 폴더에 두지 않는다.
- 완료된 변경 요약은 `.codex/archive/`로 옮긴다.
- 완료된 QA 상세는 `.codex/archive/qa/`로 옮긴다.
- 완료된 DEV/QA 이력 인덱스는 `.codex/records/`에서 확인한다.
- `current-task.md`는 한 번에 하나의 작업만 다룬다.
- `current-task.md`의 완료 이력은 길게 누적하지 않고, 완료된 상세는 `.codex/archive/` 또는 `.codex/dev-log/`로 옮긴다.
- 진행 중 QA와 리뷰 결과는 `active-qa.md`에 함께 둔다.
- DevAgent가 QA로 넘기지 않으려면 `남은 개발`에 구체적인 항목을 남긴다.
- 남은 개발이 비어 있거나 불명확하면 상태를 `QA필요`로 바꾸거나 FlowAgent가 범위를 다시 정리한다.
- `issue-board.md`는 빠르게 훑을 수 있을 만큼 짧게 유지한다.
- 자동 루프는 `automation-rule.md`를 기준으로 하며, 실제 작업 없이 상태만 넘기지 않는다.
- `agent-status.md`에는 `Ready` 에이전트를 항상 하나만 둔다.
- 갱신할 내용이 없으면 불필요한 이력을 누적하지 않고 결과 보고에 `변경 없음`과 이유를 남긴다.
