# Issue Board

현재 개발 판단에 필요한 이슈만 짧게 관리한다.

완료된 DEV/QA 이력은 `.codex/records/`의 CSV를 기준으로 확인한다.

| ID | 상태 | 우선순위 | 담당 | 이슈 | 다음 행동 |
|---|---|---|---|---|---|
| BACKLOG-001 | Done | 중 | FlowAgent | `PlateController`의 플레이어/적 플레이트 순회 메서드 중복 검토 | 기존 private 메서드 정리와 DEV-018~020 기록 기준으로 후속 후보에서 제외 |
| BACKLOG-002 | Done | 중 | FlowAgent | `Player` 특수공격 시작 흐름의 로그 문구와 상태 변경 순서 정리 검토 | 작은 private 리팩터링 기준으로 QAReviewAgent 생략. 컴파일 0 warning, Plate EditMode 22/22 통과 |
| BACKLOG-003 | Done | 낮음 | FlowAgent | `SummonController`의 소문자 getter/setter 계열과 공격 예측 계열 네이밍 정리 | `next-session-note-2026-06-07.md` 기준 메서드 네이밍 완료. 재개하지 않음 |
| BACKLOG-004 | Done | 낮음 | FlowAgent | 재소환 선택 흐름의 `PlateSelectionController` 분리 필요성 검토 | `PlateSelectionController` 분리 확인. Unity compile 0 warning, Plate EditMode 23/23 통과 |
| DEV-022 | Pass | 중 | FlowAgent | `FireSpirit` SummonData 적용 | QA-022 Pass. FlowAgent가 완료 정리 후 다음 기능 목표 선정 |
| DEV-023 | Pass | 중 | FlowAgent | `QueenSpirit` SummonData 적용 | QA-023 Pass. FlowAgent가 완료 정리 후 다음 기능 목표 선정 |
| DEV-024 | Done | 중 | FlowAgent | `DarkDragon` SummonData 적용 | QA-024 Pass. SummonData 누락 대상 없음 확인 |
| DEV-025 | Pass | 중 | FlowAgent | 전투 공격 실행 흐름 분리 분석 | QA-025 Pass. FlowAgent가 완료 정리 후 다음 작은 구현 후보 선정 |
| DEV-026 | Pass | 중 | FlowAgent | 적 반응 일반공격 실행 중복 분리 | QA-026 Pass. FlowAgent가 완료 정리 후 다음 작은 구현 후보 선정 |
| DEV-027 | Pass | 중 | FlowAgent | 적 반응 특수공격 실행 로그 중복 분리 | QA-027 Pass. FlowAgent가 완료 정리 후 다음 작은 구현 후보 선정 |
| DEV-028 | Done | 중 | FlowAgent | 소환수 도메인 잔여 구조 정리 | DEV-028O Cat 전용 우회 위치 재조정까지 완료. 다음은 중기 damage source 정리 후보 검토 |
| DEV-029 | Done | 중 | FlowAgent | 공격 전략 damage source 정리 | QA-029 Pass 기준 완료. 실제 근접 피해는 현재 공격력 기준, 전략 damage는 예측/표시/데이터 기준으로 정리 |
| DEV-030 | Done | 중 | FlowAgent | Cat 예측 damage 기준 정합성 검토 | 실제 Cat 일반/특수공격 피해 기준과 예측 기준이 일치해 코드 변경 없이 완료 |
| DEV-031 | Done | 낮음 | FlowAgent | Cat 예측 처치 가능 인덱스 중복 계산 정리 | 지역 변수로 반복 계산 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-032 | Done | 낮음 | FlowAgent | Rabbit 예측 회복 대상 인덱스 반복 계산 정리 | 지역 변수로 반복 계산 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-033 | Done | 낮음 | FlowAgent | Fox 예측 인덱스 반복 계산 정리 | 지역 변수로 반복 계산 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-034 | Done | 낮음 | FlowAgent | Eagle 예측 인덱스 반복 계산 정리 | 지역 변수로 반복 계산 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-035 | Done | 낮음 | FlowAgent | Wolf 예측 인덱스 반복 계산 정리 | 지역 변수로 반복 계산 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-036 | Done | 낮음 | FlowAgent | Rabbit 후순위 회복 대상 인덱스 반복 계산 정리 | 지역 변수로 반복 계산 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-037 | Done | 낮음 | FlowAgent | Snake 일반공격 확정 중복 분기 정리 | 같은 일반공격 예측 생성 분기 통합. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-038 | Done | 낮음 | FlowAgent | Fox 미사용 AttackPrediction 대입 정리 | 최종 반환 전에 덮어쓰는 중간 대입 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-039 | Done | 낮음 | FlowAgent | Cat/Eagle/Wolf 미사용 AttackPrediction 초기 대입 정리 | 최종 반환 전에 덮어쓰는 초기 대입 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-040 | Done | 낮음 | FlowAgent | Rabbit/Snake 분기별 AttackPrediction 직접 반환 정리 | 분기별 대입 후 마지막 반환 구조를 직접 반환으로 정리. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-041 | Done | 낮음 | FlowAgent | PlayerAttackPrediction fallback plate index 반복 계산 정리 | 일반 공격 fallback 예측 생성에서 기존 attackSummonPlateIndex를 사용. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-042 | Done | 낮음 | FlowAgent | Battle/Prediction 미사용 Burst using 제거 | PlayerAttackPrediction과 IAttackPrediction의 미사용 Unity.Burst.Intrinsics using 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-043 | Done | 낮음 | FlowAgent | CatAttackPrediction 죽은 null 방어 분기 제거 | GetMostDamageAttack의 foreach 내부 죽은 null 체크 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-044 | Done | 낮음 | FlowAgent | Eagle/Wolf 미사용 damage 대입 제거 | 특수공격 선택 직전 사용되지 않는 maxDamage 대입 제거. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-045 | Done | 낮음 | FlowAgent | Eagle/Wolf damage 비교 기준 변수명 정리 | 일반공격 피해 기준값을 normalAttackDamage로 명명. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |
| DEV-046 | Done | 낮음 | FlowAgent | Eagle/Wolf damage 비교 주석 정리 | 특수공격 선택 조건 주석을 현재 코드 의미에 맞게 정리. compile warning 0, PlateSummonDrawStaticRegressionTests 28/28 통과 |

## 제외 항목

- 2~7스테이지 씬 수정은 당분간 진행하지 않는다.
- 비메서드 네이밍 잔여 정리는 이번 기능 개발에서 제외한다.
- DEV-029는 Summons 잔여 정리 완료 후 사용자가 지정한 다음 작업이다.
- DEV-029에서 예측 알고리즘 전면 수정은 제외하고 damage source 기준 분리만 다룬다.

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
