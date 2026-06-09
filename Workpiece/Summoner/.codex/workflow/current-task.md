# Current Task

## 기능 구현 상태

- 현재 도메인: Battle/Prediction
- 기능 상태: 동작완료
- 설계 품질 상태: 검토완료
- QA 깊이 상태: 검증완료
- 기능 목표: `Eagle/WolfAttackPrediction`의 damage 비교 주석을 현재 코드 의미에 맞게 정리한다.
- 기능 완료 조건:
  - `GetTypeOfMoreAttackDamage`, `GetMostDamageAttack`에서 더 이상 값을 갱신하지 않는 “업데이트” 주석을 실제 선택 의미로 바꾼다.
  - 일반/특수 공격 damage 비교 조건과 반환 의미를 바꾸지 않는다.
  - 씬, 프리팹, 저장 데이터 구조는 변경하지 않는다.
  - 예측 알고리즘 전면 수정은 하지 않는다.
  - 씬, 프리팹, 저장 데이터 구조는 변경하지 않는다.
- 설계 품질 기준:
  - Eagle/Wolf의 damage 비교 예측 책임은 각 Prediction 클래스에 유지한다.
  - 주석이 실제 코드 동작을 설명하게 한다.
  - 새 클래스나 공통 helper를 만들지 않는다.
- QA 깊이 기준:
  - 작은 private 리팩터링이므로 Unity compile warning 0을 확인한다.
  - 가능하면 `PlateSummonDrawStaticRegressionTests`를 재실행한다.
- QA 호출 조건:
  - 예측 결과 의미 변경, public API 변경, 테스트 추가, 관련 테스트 실패가 있으면 QAReviewAgent로 전환한다.

## 기능 상태 기준

- 시작전: 기능 목표만 정해졌고 개발을 시작하지 않음
- 진행중: DevAgent가 기능 목표 달성을 위해 개발 중
- 동작완료: 기능 동작은 연결됐지만 설계 품질 또는 QA 깊이 확인이 남음
- 검증대기: 기능 개발 중간 단계라 아직 QAReviewAgent로 넘기지 않음
- QA필요: 기능 구현과 설계 품질 기준을 충족했고 QAReviewAgent 검증 대기
- QA진행중: QAReviewAgent가 컴파일, 테스트, 회귀 위험을 확인 중
- QA완료: QAReviewAgent 검증 통과, FlowAgent 정리 대기
- 구현완료: 기능 동작, 설계 품질, QA 깊이 기준, FlowAgent 정리가 모두 끝남
- 보류: 사용자 결정이나 외부 조건 때문에 멈춘 상태
- 후속작업대기: 현재 진행 중인 기능이 없고 다음 작업 선정 대기

## 작업

DEV-046 Eagle/Wolf damage 비교 주석 정리

## 상태

동작완료

## 목표

`EagleAttackPrediction.GetTypeOfMoreAttackDamage`, `WolfAttackPrediction.GetMostDamageAttack`에서 특수공격 선택 조건 주석을 현재 코드 의미에 맞게 정리한다.

## 최근 완료 요약

- DEV-031은 `CatAttackPrediction.GetAttackPrediction`의 일반/특수공격 처치 가능 인덱스 반복 계산을 지역 변수로 정리했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- 다음 작은 후보는 같은 Battle/Prediction 도메인의 `RabbitAttackPrediction.GetAttackPrediction` 반복 계산 정리다.
- DevAgent가 `RabbitAttackPrediction.GetAttackPrediction`에서 `GetIndexOfLowerHealthIfDifferenceOver30`, `GetIndexOfLowerHealthIfAllDown30` 결과를 지역 변수로 한 번씩만 계산하도록 정리했다.
- 예측 확률 조정, 선택 plate index, 반환 `AttackPrediction` 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-033은 `FoxAttackPrediction.GetAttackPrediction`에서 `GetIndexOfSummonWithCurseStatus`, `IsAnyEnemyHealthDown30Percent`, `GetIndexOfNormalAttackCanKill` 반복 계산을 지역 변수로 정리했다.
- 예측 확률 조정, 선택 plate index, 반환 `AttackPrediction` 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-034는 `EagleAttackPrediction.GetAttackPrediction`에서 `IsEnermyHealthDifferenceOver30`, `CanNormalAttack`, `AreEnermyHealthWithin10Percent`, `GetIndexOfNormalAttackCanKill`, `GetSpecialAttackKillIndex` 반복 계산을 지역 변수로 정리했다.
- 예측 확률 조정, 선택 plate index, 반환 `AttackPrediction` 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-035는 `WolfAttackPrediction.GetAttackPrediction`에서 `IsEnermyHealthDifferenceOver30`, `GetIndexOfNormalAttackCanKill` 반복 계산을 지역 변수로 정리했다.
- 예측 확률 조정, 선택 plate index, 반환 `AttackPrediction` 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-036은 `RabbitAttackPrediction.GetAttackPrediction`에서 앞선 회복 우선 조건 실패 후 공통으로 사용하는 `GetIndexOfLowestHealthSummon` 결과를 지역 변수로 정리했다.
- `GetIndexOfNormalAttackCanKill` 호출은 기존처럼 모든 플레이어 소환수 70% 이상 조건이 실패한 뒤에만 수행되도록 유지했다.
- 예측 확률 조정, 선택 plate index, 반환 `AttackPrediction` 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-037은 `SnakeAttackPrediction.GetAttackPrediction`에서 중독 상태와 특수공격 불가 상태의 같은 일반공격 예측 생성 분기를 `||` 단락 평가 조건으로 합쳤다.
- 중독 상태일 때 `CanUseSpecialAttack`을 추가 호출하지 않는 기존 조건 순서는 유지했다.
- 예측 확률 조정, 선택 plate index, 반환 `AttackPrediction` 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-038은 `FoxAttackPrediction.GetAttackPrediction`에서 최종 반환 전에 다시 덮어쓰는 미사용 `AttackPrediction` 중간 대입을 제거했다.
- 저주 상태 조기 반환과 마지막 반환의 target plate, plate index, 확률 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-039는 `CatAttackPrediction`, `EagleAttackPrediction`, `WolfAttackPrediction`에서 최종 반환 전에 다시 덮어쓰는 미사용 `AttackPrediction` 초기 대입을 제거했다.
- 최종 반환의 target plate, plate index, 확률 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-040은 `RabbitAttackPrediction`, `SnakeAttackPrediction`에서 분기별 `AttackPrediction` 대입 후 마지막에 반환하는 구조를 직접 반환으로 정리했다.
- 각 분기에서 생성하는 attack strategy, target plate, plate index, 확률 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-041은 `PlayerAttackPrediction.GetPlayerAttackPredictionList`에서 일반 공격 fallback 예측 생성 시 `plateController.GetPlateIndex(plate)`를 다시 호출하지 않고 기존 `attackSummonPlateIndex`를 사용하게 정리했다.
- 생성되는 `AttackPrediction`의 plate index 의미와 특수 예측 위임 흐름은 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-042는 `PlayerAttackPrediction`, `IAttackPrediction`에서 사용하지 않는 `Unity.Burst.Intrinsics` using을 제거했다.
- 예측 알고리즘, interface 멤버, 호출부 의미는 바꾸지 않았다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-043은 `CatAttackPrediction.GetMostDamageAttack`에서 `GetAvailableSpecialAttacks()`의 빈 배열 반환 계약과 맞지 않는 `foreach` 내부 null 체크를 제거했다.
- 일반/특수 공격 damage 비교와 반환 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-044는 `EagleAttackPrediction.GetTypeOfMoreAttackDamage`, `WolfAttackPrediction.GetMostDamageAttack`에서 `return AttackType.SpecialAttack` 직전의 미사용 `maxDamage` 대입을 제거했다.
- 일반/특수 공격 damage 비교 조건과 반환 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-045는 `EagleAttackPrediction.GetTypeOfMoreAttackDamage`, `WolfAttackPrediction.GetMostDamageAttack`의 일반공격 피해 기준값 변수명을 `normalAttackDamage`로 정리했다.
- 일반/특수 공격 damage 비교 조건과 반환 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.
- DEV-046은 `EagleAttackPrediction.GetTypeOfMoreAttackDamage`, `WolfAttackPrediction.GetMostDamageAttack`에서 “업데이트”라고 되어 있던 특수공격 선택 조건 주석을 현재 코드 의미에 맞게 정리했다.
- 일반/특수 공격 damage 비교 조건과 반환 의미는 유지했다.
- 검증: Unity MCP `recompile_scripts` warning 0, EditMode `Summoner.EditModeTests.PlateSummonDrawStaticRegressionTests` 28/28 통과.

## 다음 작업

- FlowAgent가 Battle/Prediction 도메인의 damage 비교 예측 정리 완료 여부를 확인한다.

## 제외 범위

- 예측 알고리즘 전면 수정
- 다른 소환수 예측 로직 공통화
- 새 클래스/helper 추가
- 씬, 프리팹, 저장 데이터 구조 변경
