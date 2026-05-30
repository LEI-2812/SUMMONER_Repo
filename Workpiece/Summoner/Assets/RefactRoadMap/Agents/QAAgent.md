# QA Agent

## 1. 폴더 구조

QA Agent는 리팩토링 변경이 기존 기능을 깨뜨리지 않는지 검사하는 기준 문서다.
실제 테스트 코드를 바로 작성하기 전에, 어떤 수동 테스트와 자동 테스트가 필요한지 먼저 제안한다.

위치:

```text
Assets/RefactRoadMap
  Agents
    QAAgent.md
```

이 문서는 코드 실행 파일이 아니다.
리팩토링할 때마다 테스트 항목을 정하고, 검증 결과를 로드맵 변경 기록에 남기기 위한 기준이다.

## 2. 핵심 흐름

QA Agent는 아래 순서로 동작하도록 사용한다.

```text
코드 변경 전
 -> 변경 대상 시스템 확인
 -> 유지되어야 할 기존 동작 확인
 -> 수동 테스트 항목 제안
 -> 자동 테스트 후보 제안
 -> 시뮬레이션 후보 제안

코드 변경 후
 -> Unity Console 컴파일 에러 확인
 -> 관련 씬 실행 확인
 -> 수동 테스트 수행
 -> 회귀 위험 확인
 -> 로드맵 변경 기록에 검증 결과 작성
```

## 3. 주요 코드

### 코드 변경 전 질문

리팩토링을 적용하기 전에는 아래 질문에 답해야 한다.

```text
1. 이 변경은 어떤 시스템에 영향을 주는가?
2. 반드시 유지되어야 하는 기존 동작은 무엇인가?
3. 수동 테스트가 필요한 씬은 무엇인가?
4. 자동 테스트로 만들 수 있는 핵심 로직은 무엇인가?
5. 실패하면 가장 위험한 기능은 무엇인가?
6. 변경 후 어떤 로드맵에 검증 결과를 기록할 것인가?
```

### 테스트 제안 형식

```text
수동 테스트:
- Unity에서 직접 확인할 항목

자동 테스트 후보:
- EditMode 또는 PlayMode 테스트로 만들 수 있는 항목

시뮬레이션 후보:
- 여러 번 반복 실행해 안정성을 확인할 항목

회귀 위험:
- 기존 기능이 깨질 가능성이 높은 항목
```

### 공통 검증 기준

```text
- Unity Console 컴파일 에러 없음
- 관련 씬 실행 가능
- 변경 기능 수동 테스트 완료
- 주변 기능 회귀 테스트 완료
- 로드맵 변경 기록에 검증 결과 작성
```

## 4. 실행 방법

### 게임 시스템 QA

수동 테스트:

```text
- 새 게임 시작 시 savedStage가 1로 설정되는지 확인
- 새 게임 시작 후 볼륨/해상도 설정이 유지되는지 확인
- 이어하기 버튼이 저장 상태에 따라 표시되는지 확인
- 스테이지 선택 시 playingStage가 저장되는지 확인
- 메뉴 Alert에서 Yes/No 결과가 정상 처리되는지 확인
```

자동 테스트 후보:

```text
- GameSaveKey가 모든 저장 키를 제공하는지 확인
- GameProgressSave가 savedStage만 초기화하는지 확인
- StageFlowSelect가 stage 번호에 맞는 씬 이름과 배율을 반환하는지 확인
```

시뮬레이션 후보:

```text
SaveLoadSimulation
- 새 게임 시작
- 저장값 확인
- 이어하기
- 설정값 유지 확인
```

### 스토리 시스템 QA

수동 테스트:

```text
- Prologue Screen에서 첫 대사가 출력되는지 확인
- 클릭으로 다음 대사가 진행되는지 확인
- Space 입력으로 다음 대사가 진행되는지 확인
- Stage별 이동/이미지/사운드 연출이 정상 실행되는지 확인
- 스토리 종료 후 Fight Screen_NStage로 이동하는지 확인
- Epilogue 종료 후 Thank Screen으로 이동하는지 확인
- Skip 설정에 따라 Skip 버튼이 표시되는지 확인
```

자동 테스트 후보:

```text
- DialogueCsvParse가 CSV 문자열을 Dialogue 배열로 변환하는지 확인
- DialogueRangeSelect가 storyNum에 맞는 대사 범위를 반환하는지 확인
- StoryProgressAdvance가 다음 대사 줄을 순서대로 반환하는지 확인
```

시뮬레이션 후보:

```text
StoryDialogueSimulation
- CSV 전체 파싱
- 빈 대사 확인
- 잘못된 ID 확인
- storyNum별 대사 범위 존재 확인
```

### 전투 핵심 QA

수동 테스트:

```text
- 전투 씬에서 소환이 가능한지 확인
- 일반 공격으로 적 HP가 감소하는지 확인
- 특수 공격으로 Heal, Shield, Upgrade가 아군에게 적용되는지 확인
- 특수 공격으로 Damage, Poison, Stun, Curse가 적에게 적용되는지 확인
- 턴 종료 후 마나가 회복되는지 확인
- 턴 종료 후 쿨타임이 감소하는지 확인
- 상태이상이 턴마다 적용되고 만료되는지 확인
- 적 전멸 시 승리 Alert가 표시되는지 확인
- Clear Turn 초과 시 패배 Alert가 표시되는지 확인
```

자동 테스트 후보:

```text
- BattleTargetSelect가 공격 타입에 맞는 Plate 목록을 반환하는지 확인
- PlayerManaUse가 마나 부족 시 실패하는지 확인
- SummonStatusApply가 Poison, Shield, Upgrade를 적용하는지 확인
- BattleResultCheck가 승리/패배 조건을 판단하는지 확인
```

시뮬레이션 후보:

```text
BattleSimulation
- 플레이어 소환수 3마리, 적 3마리 배치
- 10턴 자동 진행
- HP가 음수가 되지 않는지 확인
- 죽은 소환수가 Plate에 남지 않는지 확인
- 턴이 무한 루프에 빠지지 않는지 확인
```

### 전투 콘텐츠 QA

수동 테스트:

```text
- 소환 버튼 클릭 시 후보 3개가 표시되는지 확인
- 후보 3개가 중복되지 않는지 확인
- 후보 클릭 시 선택한 소환수가 Plate에 배치되는지 확인
- 재소환 시 기존 소환수가 교체되는지 확인
- 재소환 후 기존 공격 가능 상태가 유지되는지 확인
- 적 턴에서 AI가 공격을 실행하는지 확인
- AI가 Heal, Shield, Stun, Curse를 기존 조건에 맞게 선택하는지 확인
```

자동 테스트 후보:

```text
- SummonPickProbability가 Low, Medium, High 등급을 기존 확률대로 반환하는지 확인
- SummonDataApply가 능력치를 기존 값과 동일하게 적용하는지 확인
- EnemyActionDecide가 공격 명령을 반환하는지 확인
- PlayerAttackPredictionBuild가 소환수별 예측 목록을 생성하는지 확인
```

시뮬레이션 후보:

```text
SummonPickSimulation
- 1000번 뽑기 실행
- Low / Medium / High 비율 확인
- 후보 3개 중복 여부 확인

EnemyAiSimulation
- 플레이어 Plate와 적 Plate를 여러 상태로 구성
- 적 행동 결정 반복 실행
- 사용 불가능한 특수공격을 선택하지 않는지 확인
```

## 5. 나중에 확장할 수 있는 부분

### Unity 테스트 폴더 후보

나중에 자동 테스트를 추가할 때는 아래 구조를 사용한다.

```text
Assets/Tests
  EditMode
    GameSaveTests.cs
    DialogueCsvParseTests.cs
    BattleTargetSelectTests.cs
    SummonStatusApplyTests.cs
    SummonPickProbabilityTests.cs
  PlayMode
    StorySceneFlowTests.cs
    BattleTurnFlowTests.cs
```

### 자동화 우선순위

초기에는 수동 테스트를 우선한다.
자동 테스트는 Unity 씬 의존이 낮은 순수 로직부터 만든다.

우선순위:

```text
1. GameSaveKey, StageFlowSelect
2. DialogueCsvParse, DialogueRangeSelect
3. BattleTargetSelect, PlayerManaUse
4. SummonStatusApply
5. SummonPickProbability
6. EnemyActionDecide
```

## 6. 변경 기록

### 2026-05-30

수정 대상:
- Assets/RefactRoadMap/Agents/QAAgent.md

어떻게 수정했는가:
- 리팩토링 변경 전후에 수행할 QA 기준 문서를 추가했다.
- 게임 시스템, 스토리 시스템, 전투 핵심, 전투 콘텐츠별 수동 테스트와 자동 테스트 후보를 정리했다.
- 전투, 소환수 뽑기, 스토리 대사, 저장/불러오기 시뮬레이션 후보를 추가했다.

왜 그렇게 수정했는가:
- Unity 프로젝트는 컴파일이 되어도 씬 흐름이나 프리팹 연결이 깨질 수 있으므로, 리팩토링마다 검증 기준이 필요하다.
- 자동 테스트를 처음부터 무리하게 만들기보다 수동 테스트 기준과 자동화 후보를 함께 관리하는 편이 현실적이다.

검증 방법:
- QAAgent.md 생성 위치 확인
