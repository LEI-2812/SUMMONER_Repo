# Current Task

## 기능 구현 상태

- 기능 상태: 진행중
- 설계 품질 상태: 검토중
- QA 깊이 상태: 컴파일 검증 필요
- 기능 목표: `AttackPrediction` 데이터 객체의 getter/setter 네이밍을 대문자 시작으로 통일한다.
- 기능 완료 조건:
  - `AttackPrediction`의 getter/setter 메서드를 대문자 시작 함수명으로 통일한다.
  - `AttackPrediction` 타입 호출부를 새 이름으로 변경한다.
  - `Summon.getAttackStrategy()`처럼 같은 이름을 가진 다른 타입 메서드는 변경하지 않는다.
  - 같은 기능을 하는 소문자 wrapper를 남기지 않는다.
  - 공격 로직, 예측 알고리즘, 씬/프리팹 구조를 변경하지 않는다.
- 설계 품질 기준:
  - 예측 결과 데이터 조회/수정 책임은 `AttackPrediction` 안에 유지한다.
  - 함수명은 대문자 시작 형태로 통일한다.
  - 새 클래스나 새 `MonoBehaviour` 없이 기존 구조를 유지한다.
- QA 깊이 기준:
  - Unity 컴파일 기준으로 public API 변경이 없는지 확인한다.
  - 가능하면 기존 Plate/Summon 관련 EditMode 테스트를 실행한다.
  - 작은 private 리팩터링은 수동 QA를 필수 통과 조건으로 두지 않는다.
- 현재 완료한 개발:
  - 없음
- 남은 개발:
  - `AttackPrediction` getter/setter 선언과 호출부를 대문자 시작 함수명으로 변경한다.
- QA 호출 조건:
  - DevAgent가 구현과 설계 품질 기준을 충족해 `QA필요` 상태로 전환했을 때
- QA 기준 범위:
  - `AttackPrediction` 데이터 객체 호출부
  - `EnermyAlgorithm`, `PlayerAttackPrediction`의 예측 결과 조회 경로
- 다음 QA 확인 항목:
  - 없음. 작은 private 리팩터링이라 컴파일/관련 테스트 확인으로 FlowAgent에 복귀한다.
- QA 전환 판단:
  - 코드 적용 후 컴파일/관련 테스트를 확인한다.

## 기능 상태 기준

- 시작전: 기능 목표만 정해졌고 개발을 시작하지 않음
- 진행중: DevAgent가 기능 목표 달성을 위해 개발 중
- 동작완료: 기능 동작은 연결됐지만 설계 품질 또는 QA 깊이 기준 확인이 남음
- 검증대기: 기능 개발 중간 단계라 아직 QAReviewAgent로 넘기지 않음
- QA필요: 기능 구현과 설계 품질 기준은 충족했고 QAReviewAgent 검증 대기
- QA진행중: QAReviewAgent가 컴파일, 테스트, 회귀 위험을 확인 중
- QA완료: QAReviewAgent 검증 통과, FlowAgent 정리 대기
- 구현완료: 기능 동작, 설계 품질, QA 깊이 기준, FlowAgent 정리가 모두 끝남
- 보류: 사용자 결정이나 외부 조건 때문에 멈춘 상태
- 후속작업대기: 현재 진행 중인 기능이 없고 다음 작업 선정 대기

## 작업

BACKLOG-003 일부: `AttackPrediction` getter/setter 네이밍 정리

## 상태

진행중

## 목표

`AttackPrediction` 데이터 객체의 getter/setter와 호출부를 대문자 시작 함수명으로 통일한다.

## 최근 완료 요약

완료 이력은 `.codex/records/`의 CSV와 `.codex/dev-log/`를 기준으로 확인한다.

## 다음 작업

- DevAgent가 `AttackPrediction` getter/setter 선언과 호출부를 변경한다.
- 적용 후 컴파일/관련 테스트를 확인한다.

## 제외 범위

- 당분간 2~7스테이지 씬 수정 금지
- 예측 알고리즘 수정
- 공격 로직 수정
- EnemyPlate prefab 구조 변경
- 적 배치 데이터 구조 변경
- 새 `MonoBehaviour` 생성과 씬/프리팹 컴포넌트 연결 변경
- Plate 관련 범위를 벗어난 전투 시스템 리팩터링

## 다음 작업 시작 시

- 새 기능 목표를 먼저 정리한다.
- 수정 대상, 이유, 예상 변경 범위를 설명한 뒤 diff를 제안한다.
