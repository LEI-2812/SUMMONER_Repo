# Agent Status

다음에 실행할 에이전트 하나만 `Ready`로 둔다.

| Agent | 상태 | 현재 초점 | 다음 행동 |
|---|---|---|---|
| CoordinatorAgent | Ready | DEV-135 검증 반영 완료 | FightScene UI 계층 정리 제안 |
| DevAgent | Waiting | DEV-135 Battle Target Selection 우회 흐름 제거 적용 완료 | 다음 Dev 슬라이스 대기 |
| VerificationAgent | Waiting | DEV-135 책임 경계 검증 완료 | 추가 검증 요청 시 사용 |
| DocumentationAgent | Waiting | 대기 | 기능 완료/handoff/release note 필요 시 사용 |
| ReleaseAgent | Waiting | 대기 | 릴리즈 요청 시 사용 |

## 운영 메모

- Ready를 둘 이상 두지 않는다.
- CoordinatorAgent는 사용자 판단이 필요한 내용만 보여주고, 문서 갱신과 다음 에이전트 인계는 바로 처리한다.
- DevAgent가 기본 개발 흐름이다.
- DevAgent의 코드 변경은 적용 예정 diff를 먼저 제시한다. 저위험 반복 리팩토링은 현재 사용자 자동 진행 지시를 승인으로 본다.
- 문서 상태 기록은 사용자 진행 승인 없이 바로 수행한다.
- VerificationAgent는 필요한 순간에만 호출한다.
- DocumentationAgent는 상시 기록 담당이 아니다.
- 완료 이력은 여기에 누적하지 않는다.
