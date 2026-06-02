# Current Task

## 작업명

- 에이전트 문서 구조 정렬

## 현재 상태

- Done

## 작업 모드

- Default Mode

## 목표

- 기존 에이전트 문서 구조를 늘리지 않고, 각 문서의 역할과 상태 기준을 서로 맞춘다.

## 작업 범위

- `AgentWorkflow.md` 상태값 기준 정렬
- `current-task.md` 현재 작업 기준 정렬
- `issue-board.md` 열린 이슈 요약 기준 정렬
- `change-summary.md` 이번 작업 결과 기준 정렬
- `00_NextSessionGuide.md` 다음 세션 시작 기준 정렬

## 제외 범위

- 새 에이전트 추가
- Auto Dev Mode 추가
- 코드 파일 수정
- QA-006 런타임 재검증

## 멈춘 지점

- 에이전트 문서 구조 정렬 완료
- 다음 개발/QA 작업은 `issue-board.md`의 우선순위에서 다시 선택

## 담당 에이전트 순서

1. FlowAgent
2. MaintainabilityAgent - 사전 검토
3. DevAgent - 수정 전 분석
4. FlowAgent - 문서 구조 정리
5. MaintainabilityAgent - 사후 검토
6. TestAgent
7. QAAgent
8. ReviewAgent
9. FlowAgent - 최종 정리

## 완료 기준

- 현재 작업 결과가 `.codex/workflow/change-summary.md`에 기록됨
- 각 에이전트 상태가 `.codex/workflow/agent-status.md`에 기록됨
- TestAgent 결과 또는 미확인 사유가 기록됨
- QAAgent 결과 또는 미확인 사유가 기록됨
- ReviewAgent 결과 또는 미확인 사유가 기록됨
- 코드 수정 작업이면 수정 전 파일 확인, 예상 diff, 사용자 승인, 실제 diff가 기록됨

## 다음 추천 작업

- QA-006 TestAgent 결과 기록
- QA-006 QAAgent 재검증 결과 기록
- QA-007 비디오 저장 인덱스 재검증 여부 결정

## 다음 작업 시작 시

- 이 파일은 이전 작업을 유지하지 않고 새 작업 기준으로 덮어쓴다.
- 이 파일을 새 작업 기준으로 초기화한다.
- 완료된 작업 요약은 `.codex/workflow/change-summary.md`와 `.codex/dev-log/YYYY-MM-DD.md`에 남긴다.
