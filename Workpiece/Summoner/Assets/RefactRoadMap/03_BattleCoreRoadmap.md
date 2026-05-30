# Battle Core Refactoring Roadmap

## 0-A. 전투 핵심 시스템 리팩토링 방식 선택

전투 핵심 시스템은 작은 변경도 턴, 공격, 상태이상, 승패 판정에 영향을 줄 수 있으므로 기능 단위로만 진행한다.

기존 코드 일부 교체가 적절한 경우:

- 승리 후 저장 호출처럼 외부 시스템 연결만 바꾼다.
- 공격 조건이나 상태이상 계산식 하나를 명확히 고친다.
- 기존 전투 UI 연결을 유지해야 위험이 낮다.

새 컴포넌트 생성이 적절한 경우:

- `Player`가 UI 입력, 마나, 소환, 공격, 승리 판정까지 동시에 처리한다.
- `BattleController`가 공격 대상 판단과 실제 공격 실행을 모두 담당한다.
- 턴 흐름, 공격 흐름, 결과 처리가 서로 강하게 얽혀 있다.

권장 분리 후보:

```text
BattleFlowController
- 전투 시작, 턴 진행, 승패 결과 흐름을 담당한다.

PlayerBattleInputController
- 버튼 입력과 입력 가능 여부만 담당한다.

BattleResultController
- 승리/패배 결과와 다음 흐름 연결만 담당한다.

AttackExecuteController
- 일반 공격과 특수 공격 실행만 담당한다.
```

현재 진행 기준:

- 전투 저장 연결처럼 외부 경계부터 작게 교체한다.
- `Player`와 `BattleController` 내부 분리는 QA 기준을 먼저 정한 뒤 진행한다.

## 0. 리팩토링 기본 전제

Battle Core 리팩토링은 전투 규칙을 읽기 쉽게 만드는 것을 최우선으로 한다.
턴, 마나, 공격, 타겟, 상태이상, 승패 판정은 이름만 보고 역할이 구분되어야 한다.

네이밍은 가능한 한 `대상 + 행동` 형태로 작성한다.

```text
BattleTurnStart
BattleTurnEnd
PlayerManaUse
PlayerManaRecover
BattleAttackStart
BattleAttackExecute
BattleTargetSelect
SummonStatusApply
SummonStatusTick
BattleResultCheck
```

피해야 할 이름:

```text
BattleManager
AttackHelper
StatusUtil
TurnCommon
```

판단 기준:

```text
- 이 코드는 전투 규칙인가, 화면 표시인가?
- 이 코드는 공격을 시작하는가, 실제로 실행하는가?
- 이 코드는 상태를 적용하는가, 턴마다 감소시키는가?
- 새 상태이상을 추가할 때 수정 위치가 명확한가?
- 기존 씬과 프리팹 참조를 불필요하게 깨지 않는가?
```

### 코드 변경 전 확인 규칙

Battle Core 코드를 수정하기 전에는 수정 대상 파일을 먼저 보여준다.
전투 규칙은 영향 범위가 크므로 턴, 마나, 공격, 타겟, 상태이상, 승패 판정을 한 번에 섞어서 수정하지 않는다.

작성 형식:

```text
수정 대상 파일:
- 파일 경로

수정 이유:
- 턴, 마나, 공격 실행, 타겟 선택, 상태이상, 승패 판정 중 어떤 책임을 정리하는지 설명한다.

예상 변경 범위:
- 어떤 전투 흐름과 어떤 메서드가 바뀌는지 설명한다.

새 파일/폴더 필요 여부:
- 새 Battle Core 파일이 필요하면 기존 파일에 넣지 않는 이유를 설명한다.

변경 전/후 또는 diff:
- 실제 적용 전에 변경 내용을 먼저 제안한다.
```

### 기능 단위 리팩토링 규칙

Battle Core는 전투 규칙을 담당하므로 파일 단위가 아니라 전투 행동 단위로 수정한다.
공격, 마나, 상태이상, 턴 종료는 서로 영향을 주므로 수정 전에 기능 흐름을 먼저 보여준다.

우선 기능 묶음:

```text
턴 시작
- TurnController.StartTurn()
- BattleTurnStart
- 상태이상 갱신
- 쿨타임 감소
- 승리 조건 확인

턴 종료
- TurnController.EndTurn()
- BattleTurnEnd
- 마나 회복
- 공격 가능 상태 복구
- 다음 턴 시작

마나 사용
- Player.OnSummonBtnClick()
- Player.OnSpecialAttackBtnClick()
- PlayerManaUse
- PlayerManaState
- UI 표시 확인

일반 공격
- Player.OnAttackBtnClick()
- BattleController.attackStart()
- Summon.normalAttack()
- Plate 정렬
- 승패 확인

특수 공격과 대상 선택
- Player.OnSpecialAttackBtnClick()
- BattleController.SpecialAttackLogic()
- BattleTargetSelect
- BattleAttackExecute
- 대상 Plate 선택 대기

상태이상 적용과 갱신
- Summon.ApplyStatusEffect()
- SummonStatusApply
- SummonStatusTick
- SummonStatusView

승패 판정
- BattleResultCheck
- BattleAlert.clearAlert()
- BattleAlert.failAlert()
```

각 기능을 수정할 때는 플레이어 행동과 적 행동에서 같은 규칙이 유지되는지 함께 확인한다.
전투 핵심 로직은 작은 변경도 게임 결과를 바꿀 수 있으므로 기능 하나마다 수동 테스트 항목을 남긴다.

## 1. 폴더 구조

Battle Core는 전투의 규칙과 상태 변경을 담당한다.
소환수 데이터, 뽑기 확률, 적 AI 성격은 `04_BattleContentRoadmap.md`에서 다룬다.

현재 관련 파일:

```text
Assets/Script/Battle
  BattleController.cs
  BattleAlert.cs
  BattleLogic
    Player.cs
    Enermy.cs
    Plate.cs
    StatePanel.cs
    Controller
      TurnController.cs
      PlateController.cs
    AttackLogic
      IAttackStrategy.cs
      TargetedAttackStrategy.cs
      AttackAllEnemiesStrategy.cs
      ClosestEnemyAttackStrategy.cs
      StatusEffect.cs
      SpecialAttackInfo.cs

Assets/Script/Summons
  Summon.cs
```

목표 구조:

```text
Assets/Script/Battle/Core
  Turn
    BattleTurnState.cs
    BattleTurnStart.cs
    BattleTurnEnd.cs
  Mana
    PlayerManaState.cs
    PlayerManaUse.cs
    PlayerManaRecover.cs
  Board
    BattlePlateQuery.cs
    BattlePlateHighlight.cs
    BattlePlateCompact.cs
  Attack
    BattleAttackCommand.cs
    BattleAttackStart.cs
    BattleAttackExecute.cs
    BattleTargetSelect.cs
  Status
    SummonStatusApply.cs
    SummonStatusTick.cs
    SummonStatusView.cs
  Result
    BattleResultCheck.cs
```

## 2. 핵심 흐름

현재 플레이어 공격 흐름:

```text
Player.OnAttackBtnClick()
 -> BattleController.attackStart()
 -> Summon.normalAttack()
 -> PlateController.CompactEnermyPlates()
 -> BattleAlert.clearAlert()
```

현재 특수공격 흐름:

```text
Player.OnSpecialAttackBtnClick()
 -> BattleController.attackStart()
 -> TargetedAttackStrategy면 Player 코루틴으로 Plate 선택 대기
 -> Plate.OnPointerClick()이 Player.selectedPlateIndex 변경
 -> BattleController.SpecialAttackLogic()
 -> Summon.SpecialAttack()
```

현재 턴 흐름:

```text
TurnController.StartTurn()
 -> 상태이상 갱신
 -> 쿨타임 감소
 -> Plate 압축
 -> 승리 확인
 -> player.startTurn() 또는 enermy.startTurn()

TurnController.EndTurn()
 -> currentTurn 변경
 -> 마나 회복
 -> 공격 가능 상태 복구
 -> StartTurn()
```

개선 목표:

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

## 3. 주요 코드

### 1단계: 빌드 위험 using 제거

수정 대상:

```text
Assets/Script/Battle/BattleAlert.cs
Assets/Script/Summons/Summon.cs
```

현재 문제:

```text
BattleAlert.cs
- using UnityEditor.SceneManagement;

Summon.cs
- using UnityEditor.Experimental.GraphView;
```

수정 이유:

- 런타임 전투 스크립트에 Editor 전용 using이 있다.
- 실제 사용하지 않으므로 제거한다.

### 2단계: Summon 상태이상 책임 분리

수정 대상:

```text
Assets/Script/Summons/Summon.cs
Assets/Script/Battle/Core/Status/SummonStatusApply.cs
Assets/Script/Battle/Core/Status/SummonStatusTick.cs
Assets/Script/Battle/Core/Status/SummonStatusView.cs
```

현재 문제:

- `Summon.cs`가 상태이상 적용, 턴마다 피해, Stun/Curse 처리, Upgrade 복구, 색상 깜빡임을 모두 처리한다.
- HP/Shield 변경과 UI 색상/사운드/애니메이션이 같은 클래스에 있다.

분리 순서:

```text
1. ApplyStatusEffect 분기 일부를 SummonStatusApply로 이동
2. UpdateDamageStatusEffects, UpdateStunAndCurseStatus, UpdateUpgradeStatus를 SummonStatusTick으로 이동
3. ApplyStatusEffectBlink, SetColorByStatus를 SummonStatusView로 이동
```

### 3단계: BattleController 공격 실행 분리

수정 대상:

```text
Assets/Script/Battle/BattleController.cs
Assets/Script/Battle/Core/Attack/BattleAttackCommand.cs
Assets/Script/Battle/Core/Attack/BattleTargetSelect.cs
Assets/Script/Battle/Core/Attack/BattleAttackExecute.cs
```

현재 문제:

- `BattleController.SpecialAttackLogic()`이 공격 전략 타입 판별, 아군/적 Plate 선택, 유효성 검사, 공격 실행을 모두 처리한다.
- `HandleTargetedAttack`, `HandleAttackAll`, `HandleClosestEnemyAttack`에 비슷한 아군/적 분기가 반복된다.

예상 구조:

```csharp
public class BattleAttackCommand
{
    public Summon attacker;
    public int targetPlateIndex;
    public int attackIndex;
    public bool isPlayerAttack;
}
```

### 4단계: Player 입력과 마나 분리

수정 대상:

```text
Assets/Script/Battle/BattleLogic/Player.cs
Assets/Script/Battle/Core/Mana/PlayerManaState.cs
Assets/Script/Battle/Core/Mana/PlayerManaUse.cs
Assets/Script/Battle/Core/Attack/PlayerAttackRequest.cs
```

현재 문제:

- `Player`가 마나 UI, 소환 버튼, 재소환 버튼, 일반 공격, 특수 공격, 타겟 선택 코루틴, 승리 체크를 모두 처리한다.
- `WaitForEnermyPlateSelection`, `WaitForPlayerPlateSelection`은 대상만 다르고 구조가 거의 같다.

분리 순서:

```text
1. mana, usedMana, AddMana, UpdateManaUI를 PlayerManaState로 이동
2. 소환/재소환 마나 차감은 PlayerManaUse로 이동
3. 공격 버튼은 PlayerAttackRequest로 이동
4. 타겟 선택 코루틴은 BattleTargetSelect 또는 BattleTargetWait로 이동
```

### 5단계: PlateController 책임 분리

수정 대상:

```text
Assets/Script/Battle/BattleLogic/Controller/PlateController.cs
Assets/Script/Battle/Core/Board/BattlePlateQuery.cs
Assets/Script/Battle/Core/Board/BattlePlateHighlight.cs
Assets/Script/Battle/Core/Board/BattlePlateCompact.cs
```

현재 문제:

- Plate 조회, 적 Plate 압축, 하이라이트, 투명도, 숨김/표시, 타겟 인덱스 찾기가 한 클래스에 있다.
- `getClosestEnermyPlatesIndex()` 내부에서 playerPlates를 순회하는 등 혼동 위험이 있다.

분리 순서:

```text
1. getPlayerSummons, getEnermySummons, Count, Index 조회를 BattlePlateQuery로 이동
2. Highlight, Hide, Show, Transparency를 BattlePlateHighlight로 이동
3. CompactEnermyPlates를 BattlePlateCompact로 이동
```

### 6단계: 턴 진행 단계 명확화

수정 대상:

```text
Assets/Script/Battle/BattleLogic/Controller/TurnController.cs
Assets/Script/Battle/Core/Turn/BattleTurnStart.cs
Assets/Script/Battle/Core/Turn/BattleTurnEnd.cs
Assets/Script/Battle/Core/Result/BattleResultCheck.cs
```

현재 문제:

- `StartTurn()` 안에서 상태이상, 쿨타임, Plate 압축, 승리 체크, 실제 플레이어/적 턴 시작이 함께 실행된다.
- `EndTurn()` 안에서 턴 변경, 마나 회복, 공격 가능 상태 복구가 함께 실행된다.

변경 방향:

```text
BattleTurnStart
- 현재 턴 주체에 맞는 상태/쿨타임 갱신

BattleTurnEnd
- 턴 전환, 마나 회복, 공격 가능 상태 복구

BattleResultCheck
- 승리/패배 조건만 판단
```

## 4. 실행 방법

적용 순서:

```text
1. Editor 전용 using 제거
2. Summon 상태이상 처리 일부 분리
3. BattleController 특수공격 타겟 선택 분리
4. Player 마나 처리 분리
5. PlateController 조회/표시/압축 분리
6. TurnController 시작/종료 단계 분리
```

검증 방법:

```text
1. FightScene에서 일반 소환이 정상인지 확인
2. 일반 공격 후 적 HP와 Plate 압축이 정상인지 확인
3. Heal, Shield, Upgrade가 아군에게 적용되는지 확인
4. Damage, Poison, Stun, Curse가 적에게 적용되는지 확인
5. 턴 종료 후 마나 회복과 쿨타임 감소가 정상인지 확인
6. Clear Turn 초과 시 패배가 정상인지 확인
7. 적 전멸 시 승리 Alert가 정상인지 확인
```

## 5. 나중에 확장할 수 있는 부분

### 테스트 후보

```text
SummonStatusApplyTests
- Poison 적용 시 HP 감소
- Shield 적용 시 shield 증가
- Upgrade 만료 시 공격력 복구

BattleTargetSelectTests
- 플레이어 Heal은 playerPlates 반환
- 플레이어 Damage는 enermyPlates 반환
- 적 Heal은 enermyPlates 반환

PlayerManaUseTests
- 마나가 충분하면 차감 성공
- 마나가 부족하면 차감 실패
```

### 네이밍 정리

아래 이름 변경은 마지막 단계에서 한다.

```text
Enermy -> Enemy
getAttakingSummon -> GetAttackingSummon
setIsAttaking -> SetIsAttacking
ApplayMultiple -> ApplyMultiple
SpecialAttackLogic -> ExecuteSpecialAttack
```

## 6. 변경 기록

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/03_BattleCoreRoadmap.md

어떻게 수정했는가:
- Battle Core 리팩토링을 턴 시작, 턴 종료, 마나 사용, 일반 공격, 특수 공격과 대상 선택, 상태이상, 승패 판정 기능 단위로 진행하도록 규칙을 추가했다.
- 전투 규칙 변경 전 플레이어 행동과 적 행동 양쪽에서 같은 규칙이 유지되는지 확인하도록 지시했다.

왜 그렇게 수정했는가:
- 전투 핵심 로직은 여러 클래스가 같은 상태를 바꾸므로 파일 하나만 고치면 마나, 공격, 턴, 승패 조건 중 다른 흐름이 깨질 수 있다.
- 기능 단위로 검증해야 리팩토링 후에도 실제 전투 결과가 기존과 같은지 확인할 수 있다.

검증 방법:
- Battle Core 로드맵에 기능 단위 작업 묶음이 추가됐는지 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/03_BattleCoreRoadmap.md

어떻게 수정했는가:
- 실제 전투 코드 기준으로 Summon, BattleController, Player, PlateController, TurnController의 분리 순서를 재작성했다.
- 빌드 위험 using 제거와 구체 검증 항목을 추가했다.

왜 그렇게 수정했는가:
- 전투 핵심 로직은 현재 여러 MonoBehaviour에 흩어져 있어, 상태 변경과 UI 입력을 먼저 나눠야 이후 소환수/AI 리팩토링이 가능하다.

검증 방법:
- BattleController, Player, PlateController, TurnController, Summon, StatePanel, AttackLogic 구조 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/03_BattleCoreRoadmap.md

어떻게 수정했는가:
- Battle Core 리팩토링의 기본 전제로 쉬운 전투 규칙 흐름과 명확한 `대상 + 행동` 네이밍을 추가했다.

왜 그렇게 수정했는가:
- 전투 규칙은 버그가 생기면 영향이 크므로, 공격 시작/실행/타겟 선택/상태 적용을 이름부터 명확히 구분해야 한다.

검증 방법:
- 문서 상단에 기본 전제 섹션 추가 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/03_BattleCoreRoadmap.md

어떻게 수정했는가:
- Battle Core 코드 변경 전에 수정 대상 파일과 변경 범위를 먼저 보여주도록 확인 규칙을 추가했다.

왜 그렇게 수정했는가:
- 전투 규칙은 작은 변경도 턴, 공격, 상태이상에 영향을 줄 수 있으므로 수정 전 범위를 명확히 해야 한다.

검증 방법:
- 문서 상단에 코드 변경 전 확인 규칙 추가 확인

### 2026-05-30

수정 대상
- Assets/Script/Battle/BattleResultAlertView.cs
- Assets/Script/Battle/BattleLogic/SummonStatePanelView.cs
- Assets/Script/Battle/BattleLogic/Plate.cs
- Assets/Script/Battle/BattleLogic/Player.cs
- Assets/RefactRoadMap/03_BattleCoreRoadmap.md

어떻게 수정했는가:
- 전투 UI와 직접 연결된 결과 Alert, 상태 패널 View 이름 변경 상태를 로드맵에 반영했다.
- `BattleAlert`는 `BattleResultAlertView`, `StatePanel`은 `SummonStatePanelView`로 정리된 것으로 기록했다.
- `Plate`, `Player`는 UI 컴포넌트를 직접 조작하지만 전투 규칙과 입력 처리가 섞여 있어 단순 View 이름 변경 대상에서 제외했다.

완료 표시:
- `BattleAlert` -> `BattleResultAlertView`
- `StatePanel` -> `SummonStatePanelView`

나중에 처리할 내용:
- `Plate`의 하이라이트/투명도/상태 패널 표시 책임을 분리한 뒤 `PlateView` 여부 검토
- `Player`의 마나 UI와 버튼 색상 표시를 분리한 뒤 `PlayerManaView`, `PlayerActionButtonView` 같은 작은 View 후보 검토
- 씬 UnityEvent에 남은 이전 Alert 타입명 정리

검증 방법:
- 전투 클리어/실패 Alert 표시 확인
- 소환수 상태 패널의 이미지, HP, Shield, 공격 버튼 표시 확인
- Plate hover/click 하이라이트와 공격 대상 선택 확인

## 2026-05-30 통합 작업 요약

### 완료
- 전투 핵심 로드맵을 턴, 마나, Plate, 공격, 상태이상, 승패 판정 기능 단위로 정리했다.
- 전투 UI 연결 클래스 중 `BattleResultAlertView`, `SummonStatePanelView` 변경 상태를 완료로 기록했다.

### 완료된 View 이름 변경
| 기존 이름 | 현재 이름 |
|---|---|
| BattleAlert | BattleResultAlertView |
| StatePanel | SummonStatePanelView |

### 보류
- `Plate`는 UI 하이라이트와 전투 대상 선택이 섞여 있어 이름만 `View`로 바꾸지 않는다.
- `Player`는 마나 UI, 버튼 UI, 공격 요청, 승패 체크가 섞여 있어 UI 책임 분리 후 View 후보를 정한다.
