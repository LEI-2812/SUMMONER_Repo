# Issue Board

## Source Rule

이 문서는 열린 문제의 요약 인덱스다.
QA 상세 상태의 원본은 `.codex/qa/active-qa.md`다.
QA 항목 상태가 달라지면 `active-qa.md`를 먼저 갱신하고, 이 문서는 요약만 맞춘다.
이 문서는 전체 QA 목록이 아니라 지금 우선 처리할 상위 이슈만 담는다.
Done 항목은 `.codex/workflow/change-summary.md`나 `.codex/dev-log/YYYY-MM-DD.md`에 남기고 이 표에서는 제거한다.

| ID | 상태 | 우선순위 | 담당 에이전트 | 제목 | 다음 행동 |
|---|---|---|---|---|---|
| QA-004 | Fixed | High | QAAgent | Only Mouse 옵션 재검증 | ON/OFF와 클릭 입력 유지 확인 |
| QA-006 | Fixed | High | QAAgent | 오디오 볼륨 0 재검증 | 사용자 수동 확인 완료, done archive 이동 대기 |
| QA-007 | Fixed | Medium | QAAgent | 비디오 저장 인덱스 재검증 | 실제 옵션 UI 조작 확인 |
| MCP-001 | Hold | High | TestAgent | Unity MCP timeout | 안정화 후 Console/EditMode/PlayMode 재시도 |

## 상태값

- Open: 아직 해결 안 됨
- Fixed: DevAgent가 수정했고 QA 재검증 대기
- Retest: 코드 수정 없이 다시 확인 필요
- Hold: 환경이나 실행 조건 때문에 보류
- Done: 확인 완료
