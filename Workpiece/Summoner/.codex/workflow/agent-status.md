# Agent Status

다음에 실행할 에이전트 하나만 `Ready`로 둔다.

| Agent | 상태 | 현재 초점 | 다음 행동 |
|---|---|---|---|
| CoordinatorAgent | Waiting | DEV-068 범위 정정 완료 | DevAgent 제안 결과 수렴 |
| DevAgent | Ready | DEV-068 예측용 얕은 복사 계약 고정 | `MemberwiseClone()` 유지와 예측 비파괴 조정 회귀 기준 제시 |
| VerificationAgent | Waiting | 대기 | `Summon` public API/상태 컨트롤러/Unity 생명주기 변경 시 호출 |
| DocumentationAgent | Waiting | 대기 | 기능 완료/handoff/release note 필요 시 사용 |
| ReleaseAgent | Waiting | 대기 | 릴리즈 요청 시 사용 |

## 운영 메모

- Ready를 둘 이상 두지 않는다.
- CoordinatorAgent는 사용자 판단이 필요한 내용만 보여주고, 문서 갱신과 다음 에이전트 인계는 바로 처리한다.
- DevAgent가 기본 개발 흐름이다.
- DevAgent의 코드 변경은 적용 예정 diff와 진행 승인이 필요하다.
- 문서 상태 기록은 사용자 진행 승인 없이 바로 수행한다.
- VerificationAgent는 필요한 순간에만 호출한다.
- DocumentationAgent는 상시 기록 담당이 아니다.
- 완료 이력은 여기에 누적하지 않는다.
