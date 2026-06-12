# Current Task

## 상태

- 현재 도메인: Battle/EnemyAction
- 현재 상태: Ready
- Ready 에이전트: DevAgent

## 현재 기능 슬라이스

다음 작업: DEV-068 | Battle/EnemyAction | 예측용 얕은 복사 계약 고정
첫 액션: `Summon.Clone()`의 `MemberwiseClone()` 얕은 복사는 원본 HP에 영향을 주지 않기 위한 예측 계약으로 유지하고, `EnermyAlgorithm.ApplyEnermyStatus()`가 예측 중 원본 상태 컨트롤러를 변경하지 않도록 `SetNowHP()` 기반의 비파괴 조정만 사용한다는 회귀 기준을 제안한다.
호출 대상: DevAgent

## 게이트

- DevAgent: 코드/테스트 변경 전 적용 예정 diff를 먼저 제시한다.
- 검증: `Summon.Clone()`이 `MemberwiseClone()` 계약을 유지하는지, 예측 상태 적용이 `DamageTake()`/`StatusTurnUpdate()`처럼 원본 참조 상태를 바꿀 수 있는 경로를 쓰지 않는지 정적 회귀로 확인한다.
- VerificationAgent: `Summon` public API, 상태 효과 컨트롤러, Unity 생명주기 또는 런타임 객체 연결 변경이 필요하면 호출한다.
- 파일 삭제: Git 추적 파일, Unity 생성 폴더, 에셋/저장 데이터는 별도 승인 전까지 제외한다.

## 운영 메모

- 다이빙 정리만 독립 작업으로 새로 만들지 않는다.
- 작은 호출부/이름 보정은 현재 기능 슬라이스 안에서 처리한다.
- CoordinatorAgent는 사용자 판단이 필요한 내용만 보여주고, 문서 갱신과 다음 에이전트 인계는 바로 처리한다.
- DevAgent의 코드 변경은 적용 예정 diff와 진행 승인이 필요하다.
- 문서 상태 기록은 사용자 진행 승인 없이 바로 수행한다.
- EditMode/PlayMode는 작은 변경마다 반복하지 않는다.
- active 문서에는 현재 슬라이스와 다음 호출 대상만 둔다.
