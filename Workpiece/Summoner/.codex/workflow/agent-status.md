# Agent Status

| Agent | 상태 | 마지막 작업 | 다음 작업 |
|---|---|---|---|
| FlowAgent | Done | 에이전트 문서 구조 정렬 결과 취합 | 다음 작업 요청 대기 |
| MaintainabilityAgent | Done | 새 구조 추가 없이 기존 문서 역할 정렬 확인 | 다음 작업 요청 대기 |
| DevAgent | Done | 수정 대상과 승인 범위 확인 | 다음 작업 요청 대기 |
| TestAgent | Done | 코드 변경 없음 확인 | 다음 작업 요청 대기 |
| QAAgent | Done | 문서 역할 충돌 여부 확인 | 다음 작업 요청 대기 |
| ReviewAgent | Done | 중복 규칙과 역할 혼선 확인 | 다음 작업 요청 대기 |

상태값은 `Idle`, `Waiting`, `In Progress`, `Blocked`, `Done` 중 하나를 사용한다.

- `Idle`: 현재 맡은 작업 없음
- `Waiting`: 이전 단계 결과 대기
- `In Progress`: 해당 에이전트가 작업 중
- `Blocked`: 외부 조건이나 이전 단계 결과가 필요함
- `Done`: 해당 에이전트 책임 범위 완료
