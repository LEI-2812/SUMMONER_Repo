# Agent Status

에이전트별 현재 초점과 연결 상태만 관리한다.

다음에 실행할 에이전트 하나만 `Ready`로 둔다.
나머지는 순서가 올 때까지 `Waiting` 또는 완료 상태로 둔다.

상세 수행 이력은 `.codex/archive/`에 보관한다.

| Agent | 상태 | 현재 초점 | 다음 행동 |
|---|---|---|---|
| FlowAgent | Done | BACKLOG-003 `AttackPrediction` 범위 선정 | DevAgent 결과를 받아 완료 정리 |
| DevAgent | Ready | `AttackPrediction` getter/setter 네이밍 정리 | 선언과 호출부를 대문자 시작 함수명으로 통일 |
| QAReviewAgent | Done | 작은 private 리팩터링 기준으로 호출 생략 | QA필요 작업이 생길 때까지 대기 |

## 상태 기준

- `Waiting`: 현재 직접 작업 없음
- `Ready`: 다음에 실행할 에이전트
- `In Progress`: 작업 중
- `Done`: 이번 역할 완료
- `검증대기`: 기능 개발이 아직 끝나지 않아 최종 QA를 미룬 상태
- `QA필요`: 기능 구현과 설계 품질 기준은 충족했고 QAReviewAgent로 넘겨야 하는 상태
- `Blocked`: 환경이나 외부 조건 때문에 진행 불가
- `PermissionRequired`: 권한 승인 필요
- `WaitingApproval`: 협업 모드에서 사용자 승인 대기

## 운영 기준

- 동시에 여러 에이전트를 `Ready`로 두지 않는다.
- 작업이 끝나면 FlowAgent가 다음 에이전트 하나만 `Ready`로 바꾼다.
- 검증하지 않은 내용을 `Pass`로 기록하지 않는다.
- 기능 개발 중간 결과는 `검증대기`로 기록한다.
- 2~7스테이지 씬 수정은 당분간 진행하지 않는다.
