# Battle Core Refactoring Roadmap

## Current Phase

Phase 3 - Battle Core planning and first responsibility split.

## Goal

턴, 마나, Plate, 공격 실행, 상태이상, 승패 판정을 읽기 쉬운 전투 규칙 단위로 나눈다.
전투 콘텐츠와 AI 판단은 `04_BattleContentRoadmap.md`에서 다루고, 이 문서는 실제 전투 상태 변경만 관리한다.

## Scope

```text
Assets/Script/Battle/BattleController.cs
Assets/Script/Battle/BattleLogic/Player.cs
Assets/Script/Battle/BattleLogic/Plate.cs
Assets/Script/Battle/BattleLogic/Controller/TurnController.cs
Assets/Script/Battle/BattleLogic/Controller/PlateController.cs
Assets/Script/Battle/BattleLogic/AttackLogic
Assets/Script/Summons/Summon.cs
```

## Refactoring Rules

- 전투 규칙은 기능 하나씩만 수정한다.
- 공격, 마나, 상태이상, 턴 종료를 한 작업에 섞지 않는다.
- 플레이어 행동과 적 행동에서 같은 규칙이 유지되는지 확인할 테스트 코드 또는 QA 요청을 남긴다.
- UI 표시 책임은 전투 규칙에서 분리할 수 있는지 먼저 판단한다.
- 상세 변경 기록은 이 문서에 누적하지 않고 `WorkLogs/YYYY-MM-DD.md`에 기록한다.

## Current Flow

```text
Player.OnAttackBtnClick()
 -> BattleController.attackStart()
 -> Summon.normalAttack()
 -> PlateController.CompactEnermyPlates()
 -> BattleResultAlertView.clearAlert()

Player.OnSpecialAttackBtnClick()
 -> BattleController.attackStart()
 -> Plate 선택 대기
 -> BattleController.SpecialAttackLogic()
 -> Summon.SpecialAttack()

TurnController.StartTurn()
 -> 상태이상 갱신
 -> 쿨타임 감소
 -> Plate 압축
 -> 승리 확인
 -> player.startTurn() 또는 enermy.startTurn()
```

## Target Flow

```text
턴 시작
 -> 상태/쿨타임 갱신
 -> 입력 또는 AI 행동 선택
 -> 공격 명령 생성
 -> 타겟 선택
 -> 공격 실행
 -> 결과 확인
 -> 턴 종료
```

## Active Tasks

| ID | Task | Status | Notes |
|---|---|---|---|
| BC-01 | Summon 상태이상 적용/갱신/표시 분리 | Next | `SummonStatusApply`, `SummonStatusTick`, `SummonStatusView` 후보 |
| BC-02 | BattleController 공격 실행 분리 | Pending | `BattleAttackCommand`, `BattleTargetSelect`, `BattleAttackExecute` 후보 |
| BC-03 | Player 마나와 입력 책임 분리 | Pending | 마나 UI와 공격 요청 분리 |
| BC-04 | PlateController 조회/표시/압축 분리 | Pending | `BattlePlateQuery`, `BattlePlateHighlight`, `BattlePlateCompact` 후보 |
| BC-05 | TurnController 시작/종료 단계 분리 | Pending | 상태 갱신, 쿨타임, 마나 회복, 결과 확인 |

## Completed

완료 상세는 `CompletedArchive.md`와 `WorkLogs/2026-05-30.md`에 보관한다.

요약:

- Battle Core 로드맵을 턴, 마나, 공격, 상태이상, 승패 판정 기능 단위로 정리함.
- `BattleAlert -> BattleResultAlertView`, `StatePanel -> SummonStatePanelView` 이름 변경 상태 기록.
- `Player`, `Plate`, `Summon`은 단순 View 이름 변경 대상에서 제외하고 책임 분리 후 재검토하기로 정리.

## Done Criteria

```text
1. FightScene에서 일반 소환이 정상인지 확인
2. 일반 공격 후 적 HP와 Plate 압축이 정상인지 확인
3. Heal, Shield, Upgrade가 아군에게 적용되는지 확인
4. Damage, Poison, Stun, Curse가 적에게 적용되는지 확인
5. 턴 종료 후 마나 회복과 쿨타임 감소가 정상인지 확인
6. Clear Turn 초과 시 패배가 정상인지 확인
7. 적 전멸 시 승리 Alert가 정상인지 확인
8. 관련 QAReports에 QA 요청 또는 결과 기록
9. FileStateIndex 갱신
```

## Latest Summary

2026-05-30: Battle Core는 아직 본격 코드 분리 전 단계다.
다음 작업은 `Summon`의 상태이상 적용, 턴 갱신, 표시 책임을 가장 작은 단위로 나누는 것이다.
