# Agent Workflow

## 1. Purpose

SUMMONER 프로젝트의 개발 작업을 역할별로 분리해서 진행한다.
한 번에 하나의 기능 또는 하나의 버그만 처리한다.

## 2. Core Rules

- 코드는 DevAgent만 수정한다.
- 코드 수정은 C# 코드, 테스트 코드, Unity 씬, 프리팹, 에셋 변경을 포함한다.
- 문서 기록은 각 Agent가 자기 책임 결과를 남길 때만 허용한다.
- 문서 전체 구조와 최종 정리는 FlowAgent가 담당한다.
- 개발 철칙 검사는 MaintainabilityAgent가 한다.
- 테스트 검증은 TestAgent가 한다.
- 기능 품질 확인은 QAAgent가 한다.
- 코드 리뷰는 ReviewAgent가 한다.
- 기본 모드는 Default Mode다.
- Auto QA Mode는 기존 기능 복구에 필요한 최소 수정과 검증을 자동 진행할 때 사용할 수 있다.
- Auto Dev Mode는 아직 사용하지 않는다.

## 3. Required Documents

모든 에이전트는 자기 역할을 시작하기 전에 아래 문서를 확인한다.

- `.codex/workflow/00_NextSessionGuide.md`
- `.codex/workflow/current-task.md`
- `.codex/workflow/agent-status.md`
- `.codex/workflow/change-summary.md`
- `.codex/workflow/issue-board.md`

각 에이전트는 시작 시 `agent-status.md`에서 자기 상태를 `In Progress`로 바꾼다.
끝나면 `Done`, 막히면 `Blocked`로 바꾸고 이유를 남긴다.
문서 갱신을 못 하면 다음 단계로 넘어가지 않는다.

## 4. Development Request

사용자가 기능 개발, 버그 수정, 리팩토링을 요청하면 바로 코드를 수정하지 않는다.

1. FlowAgent가 작업명, 목표, 범위, 제외 범위를 정리한다.
2. MaintainabilityAgent가 사전 검토를 한다.
3. DevAgent가 관련 파일을 열고 수정 전 분석을 한다.
4. DevAgent가 확인한 파일, 수정 대상, 수정 이유, 수정 계획, 예상 diff를 보여준다.
5. 사용자 승인 전에는 코드 수정 단계로 넘어가지 않는다.

사용자가 `진행해`, `수정해`, `승인`, `이대로 적용해`처럼 명확히 승인한 뒤에만 DevAgent가 코드를 수정한다.

## 5. Agent Flow

| 순서 | Agent | 책임 | 주요 기록 위치 |
|---|---|---|---|
| 1 | FlowAgent | 작업명, 목표, 범위, 제외 범위 정리 | `current-task.md`, `agent-status.md` |
| 2 | MaintainabilityAgent | 개발 철칙 기준 사전 검토 | `change-summary.md`, `agent-status.md` |
| 3 | DevAgent | 수정 전 파일 확인과 예상 diff 제시 | 사용자 보고, `agent-status.md` |
| 4 | DevAgent | 승인된 범위 안에서 코드 수정 | `change-summary.md` |
| 5 | MaintainabilityAgent | 수정 결과 사후 검토 | `change-summary.md` |
| 6 | TestAgent | 컴파일, Console, EditMode/PlayMode 확인 | `change-summary.md` |
| 7 | QAAgent | 재현 절차, 기대 결과, 실제 결과 확인 | `active-qa.md`, `change-summary.md` |
| 8 | ReviewAgent | 네이밍, 책임 분리, 중복, 변경 범위 리뷰 | `active-review.md`, `change-summary.md` |
| 9 | FlowAgent | 결과 취합, 남은 이슈, 다음 추천 작업 정리 | `change-summary.md`, `issue-board.md`, `00_NextSessionGuide.md` |

## 6. Finish Rule

FlowAgent는 작업이 끝나면 다음 추천 작업을 1~3개 제시한다.
하지만 다음 작업을 자동으로 시작하지 않는다.
사용자가 새 작업을 명시적으로 요청할 때만 다음 작업을 시작한다.

## 7. Automation Modes

### Default Mode

기본 모드다.
DevAgent는 코드 수정 전에 관련 파일, 수정 대상, 수정 이유, 예상 diff를 보여주고 사용자 승인을 기다린다.

### Auto QA Mode

기존 기능이 다시 동작할 정도의 최소 수정과 검증을 자동 진행하는 모드다.
FlowAgent, DevAgent, TestAgent, QAAgent, ReviewAgent, FlowAgent 순서로 진행한다.
코드는 DevAgent만 수정한다.
QAAgent, TestAgent, ReviewAgent, FlowAgent는 직접 코드를 수정하지 않는다.
테스트 실패, MCP timeout, Console error가 있으면 통과로 처리하지 않고 `Blocked`, `Retest`, `Hold` 중 맞는 상태로 기록한다.

Auto QA Mode에서는 새 기능 추가, 대규모 구조 변경, 리팩토링 목적 수정은 하지 않는다.
수정 범위가 기존 기능 복구를 넘으면 Default Mode 또는 Auto Dev Mode 검토로 넘긴다.

### Auto Dev Mode

개발까지 자동화하는 모드 이름으로 예약한다.
현재는 사용하지 않는다.
사용자가 Auto Dev Mode를 요청해도 현재는 Default Mode로 전환한다.
Auto Dev Mode가 준비되기 전에는 새 기능 개발을 자동으로 시작하지 않는다.

## 8. Source of Truth

| 문서 | 역할 |
|---|---|
| `.codex/workflow/00_NextSessionGuide.md` | 다음 세션 시작 기준과 마지막 중단 지점 |
| `.codex/workflow/current-task.md` | 현재 작업명, 상태, 범위, 완료 기준 |
| `.codex/workflow/agent-status.md` | 에이전트별 현재 상태 |
| `.codex/workflow/issue-board.md` | 열린 문제, 우선순위, 담당 에이전트 |
| `.codex/workflow/change-summary.md` | 현재 작업의 변경 내용, 테스트 결과, QA 결과, 리뷰 결과 |
| `.codex/qa/active-qa.md` | 현재 QA 항목 |
| `.codex/qa/regression-checklist.md` | 큰 수정 후 회귀 확인 |
| `.codex/qa/features/*.md` | 기능별 확인 기준 |
| `.codex/code-review/active-review.md` | 현재 코드리뷰 결과 |
| `.codex/dev-log/YYYY-MM-DD.md` | 날짜별 누적 작업 기록 |

## 9. Status Values

| 상태 | 의미 |
|---|---|
| Idle | 현재 맡은 작업 없음 |
| Waiting | 아직 작업하지 않음 |
| In Progress | 해당 에이전트가 작업 중 |
| Blocked | 외부 조건이나 이전 단계 결과가 필요함 |
| Done | 해당 에이전트 책임 범위 완료 |

QA 항목은 기존 상태값을 유지한다.

| 상태 | 의미 |
|---|---|
| Open | 아직 해결 안 됨 |
| Fixed | DevAgent가 수정했고 QA 재검증 대기 |
| Retest | 코드 수정 없이 다시 확인 필요 |
| Hold | 환경이나 실행 조건 때문에 보류 |
| Done | QAAgent가 통과 확인 후 archive로 보관 가능 |
