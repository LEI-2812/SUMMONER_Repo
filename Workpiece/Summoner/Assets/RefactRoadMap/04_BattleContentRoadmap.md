# Battle Content Refactoring Roadmap

## 0-A. 전투 콘텐츠 리팩토링 방식 선택

전투 콘텐츠는 소환수, 공격 전략, AI 판단이 계속 늘어날 수 있으므로 새 파일과 데이터 분리를 허용한다.

기존 코드 일부 교체가 적절한 경우:

- 특정 소환수의 수치나 스킬 조건만 고친다.
- 기존 전략 클래스의 버그를 좁은 범위에서 수정한다.
- Prefab 연결을 바꾸면 위험이 더 큰 경우다.

새 컴포넌트 또는 새 데이터 생성이 적절한 경우:

- 소환수별 클래스가 같은 초기화/스킬 구조를 반복한다.
- 공격 전략이 늘어날수록 조건문이 길어진다.
- AI 판단 데이터와 실행 코드가 섞여 있다.

권장 분리 후보:

```text
SummonDefinition
- 소환수 기본 수치와 표시 데이터를 담는다.

AttackStrategy
- 공격 방식 하나를 명확히 담당한다.

EnemyActionDecide
- 적이 어떤 행동을 할지 판단한다.

EnemyActionExecute
- 결정된 행동을 실제 전투에 적용한다.
```

현재 진행 기준:

- Prefab에 붙은 기존 소환수 스크립트는 바로 갈아엎지 않는다.
- 새 데이터 구조가 필요하면 기존 Prefab과 연결하는 Adapter 단계를 먼저 둔다.

## 0. 리팩토링 기본 전제

Battle Content 리팩토링은 새 소환수, 새 공격, 새 AI 행동을 추가하기 쉬운 구조를 목표로 한다.
다만 처음부터 복잡한 데이터 시스템을 만들지 않고, 현재 코드 흐름을 유지하면서 작은 책임부터 분리한다.

네이밍은 가능한 한 `대상 + 행동` 형태로 작성한다.

```text
SummonPickSelect
SummonPickProbability
SummonPickPanelShow
SummonDataApply
SummonBattleScaleApply
AttackStrategyCreate
PlayerAttackPredictionBuild
EnemyActionDecide
EnemyAttackReact
EnemyPredictionBuild
```

피해야 할 이름:

```text
SummonManager
AIHelper
ContentUtil
DataCommon
```

판단 기준:

```text
- 이 코드는 후보를 고르는가, UI에 보여주는가, Plate에 배치하는가?
- 이 코드는 AI가 판단하는가, 공격을 실제 실행하는가?
- 새 소환수 추가 시 능력치와 공격 전략 위치가 명확한가?
- 확률 조정이 SummonController 안에 숨어 있지 않은가?
- 초등학생이 읽어도 "소환수 뽑기", "적 행동 결정" 정도를 이해할 수 있는가?
```

### 코드 변경 전 확인 규칙

Battle Content 코드를 수정하기 전에는 수정 대상 파일을 먼저 보여준다.
소환수 뽑기, 소환수 데이터, 공격 전략, 예측, AI 판단은 서로 다른 책임이므로 한 번에 섞어서 수정하지 않는다.

작성 형식:

```text
수정 대상 파일:
- 파일 경로

수정 이유:
- 소환수 뽑기, 데이터 적용, 공격 전략 생성, 예측, AI 판단 중 어떤 책임을 정리하는지 설명한다.

예상 변경 범위:
- 후보 선택, UI 표시, 소환수 초기화, AI 행동 결정 중 무엇이 바뀌는지 설명한다.

새 파일/폴더 필요 여부:
- 새 Battle Content 파일이 필요하면 기존 파일에 넣지 않는 이유를 설명한다.

변경 전/후 또는 diff:
- 실제 적용 전에 변경 내용을 먼저 제안한다.
```

### 기능 단위 리팩토링 규칙

Battle Content는 소환수, 뽑기, 공격 전략, 예측, AI처럼 계속 늘어나는 콘텐츠를 담당한다.
따라서 클래스 종류별로 한꺼번에 정리하지 말고, 플레이 중 보이는 기능 흐름 단위로 수정한다.

우선 기능 묶음:

```text
소환수 뽑기
- Player.OnSummonBtnClick()
- SummonController.StartSummon()
- SummonPickProbability
- SummonPickCandidateSelect
- PickSummonPanel 표시

소환수 선택과 배치
- PickSummonPanel.OnPointerClick()
- SummonController.OnSelectSummon()
- SummonResummonSelect
- Plate.SummonPlaceOnPlate()
- 기존 공격 가능 상태 유지

소환수 데이터 적용
- Summons/*.cs
- SummonData
- SummonDataApply
- SummonBattleScaleApply
- Stage별 배율 확인

공격 전략 생성
- Summons/*.cs
- AttackLogic/*.cs
- AttackStrategyCreate
- 대상 종류, 피해량, 쿨타임, 상태이상 시간 확인

플레이어 공격 예측
- PlayerAttackPrediction
- SummonPredictionRule
- PlayerAttackPredictionBuild
- 소환수별 예측 결과 확인

적 AI 행동 결정
- EnermyAlgorithm
- EnemyPredictionBuild
- EnemyActionDecide
- EnemyAttackReact
- Battle Core로 공격 명령 전달
```

각 기능을 수정할 때는 Battle Core의 실제 상태 변경 로직을 직접 바꾸지 않는다.
Battle Content는 후보 선택, 데이터 준비, AI 판단까지만 담당하고, HP 감소나 상태 변경은 Battle Core 흐름으로 넘긴다.

## 1. 폴더 구조

Battle Content는 전투 규칙 위에서 동작하는 콘텐츠를 담당한다.
소환수 뽑기, 재소환, 소환수 능력치, 공격 전략, 플레이어 공격 예측, 적 AI 판단이 여기에 들어간다.

현재 관련 파일:

```text
Assets/Script/Battle/BattleLogic
  Controller
    SummonController.cs
    EnermyAttackController.cs
  Prediction
    AttackPrediction.cs
    IAttackPrediction.cs
    PlayerAttackPrediction.cs
    CatAttackPrediction.cs
    EagleAttackPrediction.cs
    FoxAttackPrediction.cs
    RabbitAttackPrediction.cs
    SnakeAttackPrediction.cs
    WolfAttackPrediction.cs
  EnermyAlgorithm.cs
  PickSummonPanel.cs

Assets/Script/Summons
  Summon.cs
  Cat.cs
  Rabbit.cs
  Wolf.cs
  Eagle.cs
  Snake.cs
  Fox.cs
  Slime.cs
  Skeleton.cs
  LowDevil.cs
  HighDevil.cs
  DarkDragon.cs
  FireSpirit.cs
  GrassSpirit.cs
  WaterSpirit.cs
  QueenSpirit.cs
  KingSlime.cs
```

목표 구조:

```text
Assets/Script/Battle/Content
  SummonPick
    SummonPickProbability.cs
    SummonPickCandidateSelect.cs
    SummonPickPanelShow.cs
    SummonResummonSelect.cs
  SummonData
    SummonData.cs
    SummonDataApply.cs
    SummonBattleScaleApply.cs
  AttackStrategy
    AttackStrategyData.cs
    AttackStrategyCreate.cs
  Prediction
    AttackPrediction.cs
    PlayerAttackPredictionBuild.cs
    SummonAttackPredict.cs
    SummonPredictionRule.cs
  Enemy
    EnemyPredictionBuild.cs
    EnemyActionDecide.cs
    EnemyAttackReact.cs
    EnemyAttackProbability.cs
```

## 2. 핵심 흐름

현재 소환수 뽑기 흐름:

```text
Player.OnSummonBtnClick()
 -> SummonController.StartSummon()
 -> randomTakeSummon()
 -> SummonRandomly()
 -> SelectSummonByRank()
 -> PickSummonPanel에 후보 표시
 -> PickSummonPanel.OnPointerClick()
 -> SummonController.OnSelectSummon()
 -> Plate.SummonPlaceOnPlate()
```

현재 적 AI 흐름:

```text
Enermy.startTurn()
 -> Enermy.takeAction()
 -> EnermyAlgorithm.getPlayerAttackPredictionsList()
 -> EnermyAttackController.EnermyAttackStart()
 -> EnermyAlgorithm.HandleReactPrediction()
 -> BattleController.SpecialAttackLogic() 또는 Summon.normalAttack()
```

개선 목표:

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

Battle Content는 직접 HP를 깎거나 턴을 넘기지 않는다.
실제 상태 변경은 Battle Core의 공격 실행 흐름으로 보낸다.

## 3. 주요 코드

### 1단계: SummonController 뽑기 확률 분리

수정 대상:

```text
Assets/Script/Battle/BattleLogic/Controller/SummonController.cs
Assets/Script/Battle/Content/SummonPick/SummonPickProbability.cs
Assets/Script/Battle/Content/SummonPick/SummonPickCandidateSelect.cs
```

현재 문제:

- `SummonController`가 패널 열기, 랜덤 후보 선택, 등급 확률, 선택 대기 코루틴, Plate 배치를 모두 처리한다.
- 등급 확률이 코드에 직접 들어 있다.

현재 확률:

```text
Low: 50%
Medium: 35%
High: 15%
```

예상 코드:

```csharp
public sealed class SummonPickProbability
{
    public SummonRank PickSummonRank(float randomValue)
    {
        if (randomValue <= 50f) return SummonRank.Low;
        if (randomValue <= 85f) return SummonRank.Medium;
        return SummonRank.High;
    }
}
```

### 2단계: 소환수 배치와 UI 표시 분리

수정 대상:

```text
Assets/Script/Battle/BattleLogic/Controller/SummonController.cs
Assets/Script/Battle/BattleLogic/PickSummonPanel.cs
Assets/Script/Battle/BattleLogic/Plate.cs
Assets/Script/Battle/Content/SummonPick/SummonPickPanelShow.cs
Assets/Script/Battle/Content/SummonPick/SummonResummonSelect.cs
```

현재 문제:

- `PickSummonPanel`이 `SummonController.Instance`를 직접 호출한다.
- `Plate.SummonPlaceOnPlate()`가 Instantiate, 기존 소환수 제거, Observer 연결, 상태 유지까지 처리한다.

변경 방향:

```text
SummonPickPanelShow
- 후보 이미지와 assignedSummon 설정

SummonResummonSelect
- 재소환할 Plate 선택
- 기존 공격 가능 상태 유지

Plate
- 실제 배치만 담당
```

### 3단계: 소환수 데이터 정리

수정 대상:

```text
Assets/Script/Summons/*.cs
Assets/Script/Summons/Summon.cs
Assets/Script/Battle/Content/SummonData/SummonData.cs
Assets/Script/Battle/Content/SummonData/SummonDataApply.cs
```

현재 문제:

- 각 소환수 클래스의 `summonInitialize()`에 HP, 공격력, 등급, 타입, 공격 전략 생성이 직접 들어 있다.
- 일부 플레이어 소환수는 `ApplayMultiple(multiple)`을 호출하지만, 일부 적 소환수는 호출 방식이 다르다.
- `Summon.multiple`은 StageController와 StageSelecter에서 직접 바뀐다.

변경 방향:

```text
1. 능력치 값만 SummonData로 분리
2. SummonDataApply가 Summon에 값 적용
3. Stage 배율 적용은 SummonBattleScaleApply로 분리
4. ScriptableObject 전환은 나중에 진행
```

예상 데이터:

```csharp
public class SummonData
{
    public string summonName;
    public SummonRank summonRank;
    public SummonType summonType;
    public double maxHP;
    public double attackPower;
    public double heavyAttackPower;
}
```

### 4단계: 공격 전략 생성 분리

수정 대상:

```text
Assets/Script/Summons/*.cs
Assets/Script/Battle/BattleLogic/AttackLogic/*.cs
Assets/Script/Battle/Content/AttackStrategy/AttackStrategyCreate.cs
```

현재 문제:

- 소환수 클래스마다 `new TargetedAttackStrategy`, `new AttackAllEnemiesStrategy`, `new ClosestEnemyAttackStrategy`를 직접 호출한다.
- 전략 생성 위치가 흩어져 있어 쿨타임, 상태시간, 데미지 조정이 어렵다.

변경 방향:

```text
AttackStrategyCreate
- StatusType
- damage
- cooltime
- statusTime
- target type
```

### 5단계: PlayerAttackPrediction 중복 정리

수정 대상:

```text
Assets/Script/Battle/BattleLogic/Prediction/*.cs
Assets/Script/Battle/Content/Prediction/SummonPredictionRule.cs
Assets/Script/Battle/Content/Prediction/PlayerAttackPredictionBuild.cs
```

현재 문제:

- `PlayerAttackPrediction`은 플레이어 Plate를 돌며 소환수 타입에 맞는 예측 클래스를 찾는다.
- `EagleAttackPrediction`, `WolfAttackPrediction`, `FoxAttackPrediction` 등은 체력 비교, 처치 가능 여부, 확률 조정 로직이 반복된다.
- 예측 클래스가 MonoBehaviour로 붙어 있어 테스트하기 어렵다.

변경 방향:

```text
1. AttackPrediction은 데이터 객체로 유지
2. 체력 비교, 가장 가까운 적, 처치 가능 여부 계산만 먼저 공통화
3. 각 소환수별 성격은 SummonPredictionRule로 유지
4. MonoBehaviour 의존 제거는 후순위
```

### 6단계: EnermyAlgorithm 판단과 실행 분리

수정 대상:

```text
Assets/Script/Battle/BattleLogic/EnermyAlgorithm.cs
Assets/Script/Battle/BattleLogic/Controller/EnermyAttackController.cs
Assets/Script/Battle/Content/Enemy/EnemyPredictionBuild.cs
Assets/Script/Battle/Content/Enemy/EnemyActionDecide.cs
Assets/Script/Battle/Content/Enemy/EnemyAttackReact.cs
```

현재 문제:

- `EnermyAlgorithm.HandleReactPrediction()`은 판단과 실행을 같이 한다.
- `reactSpecialFromPoison`, `reactSpecialFromTargetNone`, `reactSpecialFromAllNone` 등은 BattleController를 직접 호출한다.
- 적 AI가 Battle Core 상태 변경까지 직접 실행한다.

변경 방향:

```text
EnemyPredictionBuild
- 플레이어 공격 예측 목록 생성

EnemyActionDecide
- 현재 적 소환수가 어떤 행동을 할지 결정

EnemyAttackReact
- 플레이어 예측에 대응할 특수공격 선택

BattleAttackCommand
- 실제 실행은 Battle Core로 전달
```

## 4. 실행 방법

적용 순서:

```text
1. SummonPickProbability 분리
2. SummonController에서 후보 선택 로직만 교체
3. PickSummonPanel의 직접 싱글톤 호출 제거
4. SummonData 초안 추가
5. 플레이어 소환수 1개만 SummonDataApply로 전환
6. 정상 확인 후 나머지 소환수 반복 적용
7. Prediction 공통 계산 함수 분리
8. EnermyAlgorithm이 BattleAttackCommand를 반환하도록 변경
```

검증 방법:

```text
1. 소환 버튼 클릭 시 후보 3개가 표시되는지 확인
2. 같은 후보가 중복 표시되지 않는지 확인
3. 재소환 시 기존 공격 가능 상태가 유지되는지 확인
4. Cat, Rabbit, Wolf, Eagle, Snake, Fox의 기본 능력치가 기존과 같은지 확인
5. 적 턴에서 기존처럼 반응 공격이 실행되는지 확인
6. AI가 Heal, Shield, Stun, Curse를 기존 조건에 맞게 선택하는지 확인
```

## 5. 나중에 확장할 수 있는 부분

### ScriptableObject 데이터화

구조가 안정되면 `SummonData`, `AttackStrategyData`, `EnemyAiProfile`을 ScriptableObject로 옮긴다.

```text
SummonData.asset
AttackStrategyData.asset
EnemyAiProfile.asset
```

이 단계가 되면 소환수 밸런싱을 코드 수정 없이 Inspector에서 조정할 수 있다.

### AI 난이도 분리

현재 AI는 규칙 기반이다.
나중에 난이도별 프로필을 만들 수 있다.

```text
EnemyAiProfile
- specialAttackChance
- continueAttackChance
- preferHeal
- preferShield
- preferKillTarget
```

## 6. 변경 기록

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/04_BattleContentRoadmap.md

어떻게 수정했는가:
- Battle Content 리팩토링을 소환수 뽑기, 선택과 배치, 데이터 적용, 공격 전략 생성, 플레이어 공격 예측, 적 AI 행동 결정 기능 단위로 진행하도록 규칙을 추가했다.
- 콘텐츠 코드는 판단과 데이터 준비까지만 담당하고 실제 HP 감소와 상태 변경은 Battle Core로 넘기도록 지시했다.

왜 그렇게 수정했는가:
- 소환수와 AI는 앞으로 가장 많이 늘어날 영역이라 파일 단위 정리만으로는 새 콘텐츠 추가 흐름을 안정적으로 만들기 어렵다.
- Battle Content와 Battle Core의 책임을 기능 단위로 나눠야 AI 판단 코드가 전투 상태를 직접 바꾸는 문제를 줄일 수 있다.

검증 방법:
- Battle Content 로드맵에 기능 단위 작업 묶음이 추가됐는지 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/04_BattleContentRoadmap.md

어떻게 수정했는가:
- 실제 소환수 뽑기, 소환수 초기화, 공격 예측, 적 AI 코드를 기준으로 로드맵을 재작성했다.
- `SummonController`, `PickSummonPanel`, `Summons/*.cs`, `Prediction/*.cs`, `EnermyAlgorithm`의 구체 분리 순서를 추가했다.

왜 그렇게 수정했는가:
- 전투 콘텐츠는 새 소환수와 AI 행동을 추가할 때 가장 자주 바뀌므로, 전투 규칙과 분리해야 유지보수가 쉬워진다.

검증 방법:
- SummonController, PickSummonPanel, Summons, Prediction, EnermyAlgorithm, EnermyAttackController 구조 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/04_BattleContentRoadmap.md

어떻게 수정했는가:
- Battle Content 리팩토링의 기본 전제로 쉬운 네이밍, 작은 책임 분리, 필요한 만큼의 확장성을 추가했다.

왜 그렇게 수정했는가:
- 소환수, 공격 전략, AI는 계속 늘어날 수 있으므로 후보 선택, 데이터 적용, AI 판단, 공격 실행을 이름부터 분리해야 한다.

검증 방법:
- 문서 상단에 기본 전제 섹션 추가 확인

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/04_BattleContentRoadmap.md

어떻게 수정했는가:
- Battle Content 코드 변경 전에 수정 대상 파일과 변경 범위를 먼저 보여주도록 확인 규칙을 추가했다.

왜 그렇게 수정했는가:
- 소환수, 공격 전략, AI는 확장 지점이 많아 수정 전 대상과 이유를 먼저 확인해야 변경 범위가 커지는 것을 막을 수 있다.

검증 방법:
- 문서 상단에 코드 변경 전 확인 규칙 추가 확인

### 2026-05-30

수정 대상
- Assets/Script/Battle/BattleLogic/PickSummonPanelView.cs
- Assets/Script/Battle/BattleLogic/Controller/SummonController.cs
- Assets/RefactRoadMap/04_BattleContentRoadmap.md

어떻게 수정했는가:
- 소환수 선택 패널이 `PickSummonPanelView`로 정리된 상태를 로드맵에 반영했다.
- `SummonController`가 소환수 선택 패널 표시와 랜덤 후보 선정까지 함께 처리하고 있어, 이름만 `View`로 바꾸지 않고 추후 책임 분리 대상으로 남겼다.

완료 표시:
- `PickSummonPanel` -> `PickSummonPanelView`

나중에 처리할 내용:
- `SummonController`에서 패널 열기/닫기와 후보 이미지 세팅을 `SummonPickView` 계열로 분리할지 검토
- 랜덤 소환수 후보 선정은 View가 아니라 선택 규칙 쪽으로 분리 검토
- 일반 소환/재소환 패널 동작을 PlayMode 또는 수동 QA로 확인

검증 방법:
- 일반 소환 패널에 3개 후보가 표시되는지 확인
- 후보 hover 색상 변경과 click 선택 확인
- 재소환 패널이 기존 소환수 교체 흐름과 충돌하지 않는지 확인

## 2026-05-30 통합 작업 요약

### 완료
- 전투 콘텐츠 로드맵을 소환수 뽑기, 선택과 배치, 데이터 적용, 공격 전략, 공격 예측, AI 판단 기능 단위로 정리했다.
- 소환수 선택 패널 이름 변경 상태를 완료로 기록했다.

### 완료된 View 이름 변경
| 기존 이름 | 현재 이름 |
|---|---|
| PickSummonPanel | PickSummonPanelView |

### 보류
- `SummonController`는 패널 표시와 랜덤 후보 선정이 섞여 있어 View 이름으로 바꾸지 않는다.
- 후보 이미지 세팅/패널 열기 닫기는 나중에 `SummonPickView` 계열로 분리할지 검토한다.
