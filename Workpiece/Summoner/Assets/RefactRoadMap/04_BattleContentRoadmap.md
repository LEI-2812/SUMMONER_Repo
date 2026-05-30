# Battle Content Refactoring Roadmap

## Current Phase

Phase 4 - Battle Content planning and first summon-pick split.

## Goal

소환수 뽑기, 소환수 데이터, 공격 전략, 플레이어 공격 예측, 적 AI 판단을 전투 규칙과 분리한다.
Battle Content는 후보 선택, 데이터 준비, AI 판단까지만 담당하고 실제 HP 감소나 상태 변경은 Battle Core로 넘긴다.

## Scope

```text
Assets/Script/Battle/BattleLogic/Controller/SummonController.cs
Assets/Script/Battle/BattleLogic/PickSummonPanelView.cs
Assets/Script/Battle/BattleLogic/Prediction
Assets/Script/Battle/BattleLogic/EnermyAlgorithm.cs
Assets/Script/Battle/BattleLogic/Controller/EnermyAttackController.cs
Assets/Script/Summons
```

## Refactoring Rules

- 소환수 뽑기, 데이터 적용, 공격 전략 생성, 예측, AI 판단을 한 번에 섞지 않는다.
- Prefab에 붙은 기존 소환수 스크립트는 바로 갈아엎지 않는다.
- 새 데이터 구조가 필요하면 기존 Prefab과 연결하는 Adapter 단계를 먼저 둔다.
- Battle Content 작업 중 Battle Core의 HP 감소, 상태 변경, 턴 종료 흐름을 직접 바꾸지 않는다.
- 상세 변경 기록은 이 문서에 누적하지 않고 `WorkLogs/YYYY-MM-DD.md`에 기록한다.

## Current Flow

```text
Player.OnSummonBtnClick()
 -> SummonController.StartSummon()
 -> randomTakeSummon()
 -> SummonRandomly()
 -> SelectSummonByRank()
 -> PickSummonPanelView에 후보 표시
 -> PickSummonPanelView.OnPointerClick()
 -> SummonController.OnSelectSummon()
 -> Plate.SummonPlaceOnPlate()

Enermy.startTurn()
 -> Enermy.takeAction()
 -> EnermyAlgorithm.getPlayerAttackPredictionsList()
 -> EnermyAttackController.EnermyAttackStart()
 -> EnermyAlgorithm.HandleReactPrediction()
 -> BattleController.SpecialAttackLogic() 또는 Summon.normalAttack()
```

## Target Flow

```text
소환 콘텐츠
 -> 후보 선택
 -> UI 표시
 -> Plate 배치

AI 콘텐츠
 -> 플레이어 공격 예측 생성
 -> 적 행동 판단
 -> Battle Core에 공격 명령 전달
```

## Active Tasks

| ID | Task | Status | Notes |
|---|---|---|---|
| BT-01 | SummonController 뽑기 확률 분리 | Next | `SummonPickProbability` 후보 |
| BT-02 | 후보 선택과 패널 표시 분리 | Pending | `SummonPickCandidateSelect`, `SummonPickPanelShow` 후보 |
| BT-03 | PickSummonPanelView 싱글톤 직접 호출 줄이기 | Pending | 선택 이벤트 흐름 분리 |
| BT-04 | 소환수 데이터 적용 한 종류부터 검증 | Pending | `SummonData`, `SummonDataApply` 후보 |
| BT-05 | EnermyAlgorithm 판단/실행 분리 | Pending | 예측 생성, 행동 결정, 공격 반응 분리 |

## Completed

완료 상세는 `CompletedArchive.md`와 `WorkLogs/2026-05-30.md`에 보관한다.

요약:

- Battle Content 로드맵을 소환수 뽑기, 선택/배치, 데이터 적용, 공격 전략, 공격 예측, AI 판단 기능 단위로 정리함.
- `PickSummonPanel -> PickSummonPanelView` 이름 변경 상태 기록.
- `SummonController`는 단순 View 이름 변경 대상에서 제외하고 책임 분리 대상으로 유지.

## Done Criteria

```text
1. 소환 버튼 클릭 시 후보 3개가 표시되는지 확인
2. 같은 후보가 중복 표시되지 않는지 확인
3. 후보 클릭 시 선택한 소환수가 Plate에 배치되는지 확인
4. 재소환 시 기존 공격 가능 상태가 유지되는지 확인
5. Cat, Rabbit, Wolf, Eagle, Snake, Fox의 기본 능력치가 기존과 같은지 확인
6. 적 턴에서 기존처럼 반응 공격이 실행되는지 확인
7. AI가 Heal, Shield, Stun, Curse를 기존 조건에 맞게 선택하는지 확인
8. 관련 QAReports에 QA 요청 또는 결과 기록
9. FileStateIndex 갱신
```

## Latest Summary

2026-05-30: Battle Content는 아직 본격 코드 분리 전 단계다.
다음 작업은 `SummonController`에서 뽑기 확률, 후보 선택, 패널 표시를 작은 단위로 분리하는 것이다.
