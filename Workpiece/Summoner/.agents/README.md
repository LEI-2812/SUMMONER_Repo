# Agents

이 폴더는 에이전트의 역할과 협업 기준을 정의한다.

`.agents`는 "누가 어떤 기준으로 일하는가"를 설명한다.
현재 작업 상태나 완료 이력은 이 폴더에 두지 않는다.

## 빠른 라우팅

모든 에이전트 문서를 매번 읽지 않는다.
현재 요청에 필요한 문서만 읽는다.

| 상황 | 읽을 문서 |
|---|---|
| 간단한 질문, 코드 설명, 명령 출력 | 추가 role 문서 없음 |
| 현재 상태 확인 | `.codex/workflow/current-task.md`, `.codex/workflow/agent-status.md` |
| 다음 기능 목표 선정 | `roles/FlowAgent.md`, `roles/DevAgent.md`, workflow 3종, 관련 코드 |
| 코드 변경 제안/적용 | `roles/DevAgent.md`, 관련 코드/테스트 |
| 검증/리뷰 | `roles/QAReviewAgent.md`, `.codex/qa/active-qa.md`, 관련 코드/테스트 |
| 자동 루프 | `AgentWorkflow.md`, `.codex/workflow/automation-rule.md`, 현재 `Ready` role |

## 역할 경계

- `.agents`: 에이전트 역할, 책임, 인계 기준
- `.codex/workflow`: 현재 진행 중인 작업판
- `.codex/qa`: 현재 확인이 필요한 QA
- `.codex/dev-log`: 날짜별 작업 요약
- `.codex/archive`: 완료된 상세 이력과 과거 기록

## 에이전트 문서

- `AgentWorkflow.md`: 전체 협업 흐름과 기록 기준
- `roles/FlowAgent.md`: 작업 범위와 인계 정리
- `roles/DevAgent.md`: 유지보수성 검토, diff 제안, 승인된 변경 적용
- `roles/QAReviewAgent.md`: 기능 목표 완료 후 검증, QA, 리뷰, 회귀 위험 검토

## 운영 기준

- 에이전트 문서는 작업 방식과 책임만 설명한다.
- 전체 흐름은 필요할 때만 `AgentWorkflow.md`를 기준으로 확인한다.
- 평소에는 현재 `Ready` 에이전트 문서 하나만 자세히 읽는다.
- 다음 작업 선정을 요청받으면 FlowAgent가 후보만 말하고 멈추지 않는다. 다음 DEV 작업을 정한 뒤 DevAgent 관점의 수정 대상, 수정 이유, 예상 변경 범위, diff 또는 변경 전/후 코드 제안까지 이어간다.
- 현재 작업 내용은 `.codex/workflow/current-task.md`에 둔다.
- 현재 QA 내용은 `.codex/qa/active-qa.md`에 둔다.
- 날짜별 작업 요약은 `.codex/dev-log/`에 둔다.
- 완료된 상세 작업 기록은 `.codex/archive/`에 둔다.
- 사용자 승인 전에는 코드, 씬, 에셋을 수정하지 않는다.
