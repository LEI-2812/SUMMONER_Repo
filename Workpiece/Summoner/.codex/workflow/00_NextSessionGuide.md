# Next Session Guide

## 1. Last Stop Point

- 에이전트 문서 구조 정렬을 완료했다.
- Auto QA Mode를 추가했다.
- Auto Dev Mode는 이름만 예약했고 아직 사용하지 않는다.
- QA-006 정적 재검증과 HUD 참조 확인은 진행했다.
- 실제 슬라이더 0 음소거/복구 확인은 MCP Play 연결 오류로 완료하지 못했다.
- QA-006 후속 재시도에서도 Console error 조회, `Edit/Play`, 후속 씬 조회가 timeout으로 막혔다.
- 사용자 수동 확인으로 QA-006 볼륨 0 음소거와 복구 동작은 정상 확인됐다.

## 2. Start Protocol

새 세션을 시작하면 바로 코드를 수정하지 않는다.

1. `.codex/agents/AgentWorkflow.md`를 확인한다.
2. `.codex/workflow/current-task.md`와 `.codex/workflow/agent-status.md`를 확인한다.
3. `.codex/workflow/issue-board.md`에서 열린 이슈를 확인한다.
4. `.codex/workflow/change-summary.md`의 다음 추천 작업을 확인한다.
5. 사용자에게 다음 추천 작업을 보고하고 멈춘다.

## 3. Development Request Rule

사용자가 개발 작업을 요청하면 FlowAgent부터 시작한다.
DevAgent는 코드 수정 전에 관련 파일, 수정 대상, 수정 이유, 예상 diff를 먼저 보여주고 대기한다.
사용자 승인 전에는 코드 수정 단계로 넘어가지 않는다.

기존 기능 복구와 재검증을 요청하면 Auto QA Mode로 FlowAgent, DevAgent, TestAgent, QAAgent, ReviewAgent, FlowAgent 순서만 자동 진행할 수 있다.
Auto QA Mode의 DevAgent 수정은 기존 기능 복구에 필요한 최소 범위로 제한한다.

## 4. Next Work Priority

1. QA-006 done archive 이동
2. QA-003에 HUD Console/Play timeout 재현 근거 보강
3. QA-007 비디오 저장 인덱스 재검증 여부 결정
4. 최신 `.codex/dev-log/YYYY-MM-DD.md`의 남은 작업

## 5. Active Documents

| 문서 | 역할 |
|---|---|
| `.codex/agents/AgentWorkflow.md` | 전체 에이전트 플로우 |
| `.codex/workflow/current-task.md` | 현재 작업 범위와 상태 |
| `.codex/workflow/agent-status.md` | 에이전트별 상태 |
| `.codex/workflow/change-summary.md` | 현재 작업 결과 |
| `.codex/workflow/issue-board.md` | 열린 문제 |
| `.codex/qa/active-qa.md` | 현재 QA |
| `.codex/qa/regression-checklist.md` | 회귀 체크리스트 |
| `.codex/code-review/active-review.md` | 현재 코드리뷰 |
| `.codex/dev-log/YYYY-MM-DD.md` | 날짜별 작업 기록 |
