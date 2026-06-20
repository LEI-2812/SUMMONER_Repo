# Issue Board

현재 판단이 필요한 항목만 둔다.
오래된 Done 목록은 남기지 않는다.

| ID | 상태 | 도메인 | 기능 슬라이스 | 다음 행동 | 호출 대상 |
|---|---|---|---|---|---|
| COORD-136 | Ready | Battle SceneObject | FightScene UI 계층 정리 제안 | FightScene 2~7의 BattleCanvas 하위 UI 계층을 1Stage 기준으로 맞출 수 있는지 위험도와 diff 범위를 제안 | CoordinatorAgent |

## 보류/제외

- DEV-100 씬 stale 연결 정리는 완료했다.
- 다음 세션은 파일 하나씩 오래 붙잡거나 메서드 하나 단위로 쪼개지 않는다.
- Battle 도메인 안에서 다른 기능 영향이 최소화되는 범위로 묶고, 전투씬 오브젝트 정리도 같은 Batch 판단 안에 포함한다.
- 다음은 Battle 전체 Batch 목표와 완료 조건을 먼저 확인한다.
- EnemyAction/Prediction은 마무리 상태만 판단하고, 필수 문제가 아니면 PlayerAction 분리 또는 Turn Flow 정리로 이동한다.
- 후보 순서: PlayerAction 분리 -> TurnController 얇게 만들기 -> BattleController 공격 실행 책임 정리.
- DEV-101은 PlayerAction 턴 자원 상태 분리까지 진행했다.
- DEV-102는 Unit 루트를 Player, Enemy, Plate 엔티티 폴더만 남긴 구조로 정리했다.
- DEV-103은 PlayerController와 PlayerActionFlow로 역할 이름을 맞췄다.
- DEV-104는 PlayerActionExecutor로 이미 결정된 행동 실행 책임을 분리했다.
- DEV-105는 PlayerTurnActions로 턴/결과 의존성을 PlayerActionFlow 밖으로 분리했다.
- DEV-106은 PlayerSummonActions와 PlayerAttackActions로 소환/공격 세부 흐름을 PlayerActionFlow 밖으로 분리했다.
- DEV-107은 PlayerTurnProgressState와 플레이어 턴 의도 중심 메서드로 턴 결과 호출 구조를 정리했다.
- DEV-108은 병렬 가능 범위로 SummonDrawService 계산 안정화와 EnemyPredictionPlateState 이름 정리를 적용했다.
- DEV-109는 PlateSummonExecutor로 Plate의 소환수 생성/파괴/이동 실행 책임을 분리했다.
- DEV-110은 PlateVisualView로 Plate의 개별 표시 책임을 분리했다.
- DEV-111은 PlateCompactExecutor로 PlateController의 적 플레이트 압축 실행 책임을 분리했다.
- DEV-112는 PlateQueryService로 PlateController의 조회 계산 책임을 분리했다.
- COORD-113은 PlateController의 남은 표시 흐름을 현재 PlateView 위임 구조로 충분하다고 판단해 추가 코드 변경 없이 닫았다.
- DEV-113은 SummonPickState로 SummonController의 선택 상태 보관 책임을 분리했다.
- DEV-114는 SummonPlaceExecutor로 SummonController의 일반 소환/재소환 배치 실행 책임을 분리했다.
- DEV-115는 SummonPickView로 SummonController의 패널/배경/후보 표시 책임을 분리했다.
- DEV-116은 PlateSelectionController로 소환 선택 완료 후 플레이어 Plate 표시 복구 책임을 이동했다.
- DEV-117은 PlayerActionFlow의 SummonController.isSummoning 직접 field 참조를 IsSummoning() 호출로 바꿨다.
- DEV-118은 BattleSpecialAttackExecutor로 BattleController의 특수공격 실행 책임을 분리했다.
- DEV-119는 BattleAttackState로 BattleController의 공격 상태 보관 책임을 분리했다.
- COORD-121은 BattleController.AttackStart를 현재 Controller 진입점으로 유지하고, 현재 특수공격 조회 판단만 BattleAttackState로 이동하는 것으로 닫았다.
- DEV-120은 BattleController의 현재 특수공격 조회 public API를 유지한 채 내부 판단을 BattleAttackState로 위임했다.
- COORD-122는 TurnController의 UI 표시 책임 분리를 우선하기로 판단했다.
- DEV-121은 TurnView로 턴 수와 클리어 턴 UI 텍스트 표시 책임을 분리했다.
- DEV-122는 Turn 관련 파일을 Battle/Flow/Turn 폴더로 이동하고 기존 .meta GUID를 유지했다.
- DEV-123은 TurnProgressState로 TurnController의 현재 턴과 턴 수 상태 보관 책임을 분리했다.
- DEV-124는 TurnPhaseActions로 TurnController의 턴 시작/종료 실행 흐름을 분리했다.
- DEV-130은 SummonController를 통한 공격자 Plate index 우회 갱신 경로를 제거하고, 상태 패널/BattleAttackState를 통해 공격자 index를 보관하도록 정리했다.
- DEV-131은 재소환 후 상태 패널 갱신 시 공격자 Plate index가 `-1`로 덮이지 않도록 `Plate.RestoreRedrawSummonState`에서 현재 player Plate index를 명시 전달했다.
- DEV-132는 PlayerTargetSelectionActions를 추가해 타겟형 특수공격의 선택 대기/취소/완료 실행 흐름을 PlayerAttackActions에서 분리했다.
- DEV-133은 PlateTargetSelectionActions를 추가해 Plate 클릭 입력에서 전투 타겟 선택 상태 변경 책임을 분리했다.
- COORD-134는 현재 target plate index 흐름이 한 방향으로 정리되어 있고 public API 이름도 `SpecialAttackTargetSelection` 계열로 충분히 구체적이라 코드 변경 없이 닫았다.
- DEV-135는 공격자 source를 View가 보관하지 않고 BattleController/BattleAttackState로 등록하도록 바꿨고, target hover/click 판정을 PlateTargetSelectionActions로 모았다.
- DEV-135 VerificationAgent는 View 우회 제거와 target 판정 중앙화를 확인했다. 추가로 상태 패널 연결 누락 방어를 반영했다.
- 추가 씬, 프리팹, ScriptableObject asset 값, 저장 데이터 변경은 별도 승인 전까지 제외한다.
- 제외: 예측 확률 수치 변경, 함수명/주석만 반복 정리, 신규 공통 helper 추가, EnemyAction/Prediction에 장시간 머무는 작업.
