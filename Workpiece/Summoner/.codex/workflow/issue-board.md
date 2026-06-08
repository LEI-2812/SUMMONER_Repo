# Issue Board

현재 개발 판단에 필요한 이슈만 짧게 관리한다.

완료된 DEV/QA 이력은 `.codex/records/`의 CSV를 기준으로 확인한다.

| ID | 상태 | 우선순위 | 담당 | 이슈 | 다음 행동 |
|---|---|---|---|---|---|
| BACKLOG-001 | Done | 중 | FlowAgent | `PlateController`의 플레이어/적 플레이트 순회 메서드 중복 검토 | 기존 private 메서드 정리와 DEV-018~020 기록 기준으로 후속 후보에서 제외 |
| BACKLOG-002 | Done | 중 | FlowAgent | `Player` 특수공격 시작 흐름의 로그 문구와 상태 변경 순서 정리 검토 | 작은 private 리팩터링 기준으로 QAReviewAgent 생략. 컴파일 0 warning, Plate EditMode 22/22 통과 |
| BACKLOG-003 | In Progress | 낮음 | DevAgent | `SummonController`의 소문자 getter/setter 계열과 공격 예측 계열 네이밍 정리 | `AttackPrediction` getter/setter 네이밍 정리 진행. 예측 알고리즘 의미 변경과 `Summon.getAttackStrategy`는 제외 |
| BACKLOG-004 | Backlog | 낮음 | FlowAgent | 재소환 선택 흐름의 `PlateSelectionController` 분리 필요성 검토 | 인덱스 조회/빈 플레이트 확인 하위 작업 완료. 새 컨트롤러 분리는 새 파일/구조 변경 가능성이 있어 별도 승인 범위 필요 |

## 제외 항목

- 2~7스테이지 씬 수정은 당분간 진행하지 않는다.
- 예측 알고리즘과 공격 로직 수정은 다음 작업 선정 전까지 제외한다.

## 완료 이력

완료된 DEV/QA 이력은 아래 파일을 기준으로 확인한다.

- `.codex/records/dev-records.csv`
- `.codex/records/qa-records.csv`

## 운영 기준

- `Ready`: 다음 행동이 정해져 바로 착수 가능
- `Backlog`: 지금 당장 진행하지 않음
- `In Progress`: 현재 작업 중
- `Done`: 완료 요약만 남김
- `Pass`: QA 통과 요약만 남김
- `검증부족`: 사전에 정한 QA 깊이 기준 중 확인하지 못한 항목이 남음
- `Blocked`: 코드 문제와 도구/환경 문제를 구분해서 기록
- 완료 요약은 `issue-board.md`에 누적하지 않고 records CSV로 보낸다.
