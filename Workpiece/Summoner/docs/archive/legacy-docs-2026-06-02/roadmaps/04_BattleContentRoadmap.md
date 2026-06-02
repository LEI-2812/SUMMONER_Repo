# BattleContent Roadmap

## 1. Role

BattleContent는 전투 규칙 위에서 동작하는 콘텐츠 흐름을 담당한다.
소환 후보 뽑기, Redraw, 후보 패널 표시, 선택 후 Plate 배치, 소환수 데이터, 공격 예측, 적 AI 판단이 이 문서의 범위다.

턴, 마나, 공격 실행, 상태이상 규칙 자체는 `03_BattleCoreRoadmap.md`에서 관리한다.
현재 QA 상태나 버그 우선순위는 이 문서에 두지 않고 `qa/current.md`에서 관리한다.

## 2. Ownership

| 영역 | 주요 파일/폴더 | 책임 |
|---|---|---|
| Draw/Redraw 흐름 | `Assets/Script/Battle/SummonPick` | 후보 생성, 후보 선택, Redraw 버튼 흐름 |
| 후보 패널 | `Assets/Script/Battle/SummonPick/DrawOptionPanelView.cs` | 후보 표시와 선택 이벤트 전달 |
| 소환수 데이터 | `Assets/Script/Summons` | 소환수 기본 능력치, 이미지, 색상, 사운드 연결 |
| 공격 예측 | `Assets/Script/Battle/Prediction` | 소환수별 공격 예측 목록 생성 |
| 적 AI | `Assets/Script/Battle/EnemyAction` | 적 행동 판단과 실행 요청 |

## 3. Current Structure

- Draw는 소환 후보를 뽑는 흐름이고, Redraw는 이미 나온 후보를 다시 뽑는 흐름이다.
- 후보 패널 View는 `DrawOptionPanelView` 이름을 기준으로 유지한다.
- `DrawOptionPanelView`는 선택 이벤트를 전달하고, controller가 선택 결과를 처리하는 방향으로 유지한다.
- 소환수 데이터 적용은 능력치, 이미지, 색상, 사운드가 함께 엮여 있어 단계적으로 분리해야 한다.
- 적 AI는 예측 생성, 행동 결정, 공격 실행 요청을 나누는 방향으로 정리한다.

## 4. Refactoring Direction

- Draw 확률 계산, 후보 생성, 후보 표시, 후보 선택 처리를 분리한다.
- 소환수 배치 책임을 Plate 자체에서 분리해 `SummonPlacementController` 같은 별도 흐름으로 옮긴다.
- 소환수 데이터 적용은 한 종류씩 기준값을 고정한 뒤 공통 적용 구조로 묶는다.
- 공격 예측과 적 AI는 결정 로직과 실행 로직을 분리한다.

### Naming Rule

| 의미 | 현재 이름 |
|---|---|
| 소환 후보 뽑기 | Draw |
| 후보 다시 뽑기 | Redraw |
| 후보 패널 | Draw Option Panel |
| 후보 패널 View | `DrawOptionPanelView` |
| 재소환 버튼 handler | `OnRedrawButtonClick` |

## 5. Constraints

- 후보는 기존 UI 흐름처럼 3개가 표시되어야 한다.
- 후보 선택 후 Plate 배치 흐름은 기존 조작감과 동일해야 한다.
- Redraw는 후보 재생성만 담당하고 Plate 상태를 임의로 깨면 안 된다.
- 소환수 기본 능력치, 이미지, 색상, 사운드가 리팩토링 중 바뀌면 안 된다.
- BattleCore의 전투 진입 문제가 열려 있으면 BattleContent 런타임 QA도 막힐 수 있다.

## 6. Next Work Candidates

| ID | 작업 | 목적 |
|---|---|---|
| BT-REF-01 | Draw 확률 계산 분리 | 후보 생성 규칙을 테스트 가능한 단위로 분리 |
| BT-REF-02 | 후보 선택과 패널 표시 분리 | View는 표시/이벤트만 맡고 선택 처리는 controller가 담당 |
| BT-REF-03 | 소환수 데이터 적용 검증 구조 마련 | 능력치 회귀를 한 종류씩 확인 가능하게 정리 |
| BT-REF-04 | Enemy AI 판단/실행 분리 | 예측 생성, 행동 결정, 공격 실행 요청을 나눔 |
| BT-REF-05 | 소환수 배치 책임 분리 | Plate와 Draw 흐름의 직접 결합 축소 |

진행 메모:
- 데이터화 전에 대표 소환수 기준값을 EditMode 테스트로 고정한다.
- 데이터화 작업자는 테스트가 통과하는 한 구조 변경으로 보고, 실패하면 값 변경 여부를 먼저 확인한다.
- 1차 데이터 구조로 `SummonData`와 `SummonAttackData`를 추가한다.
- 런타임 소환 흐름은 아직 데이터 기반으로 전환하지 않고, 대표 소환수 1종 전환 전까지 기존 파생 클래스 구조를 유지한다.
- 플레이어 소환수 6종(`Cat`, `Rabbit`, `Snake`, `Wolf`, `Eagle`, `Fox`)은 각 `SummonData` 에셋을 만들고 프리팹에 연결했다.
- 적/정령 계열은 현재 `SummonType` 의미와 섞지 않고, 이후 `EnemyType` 같은 별도 타입 확장을 검토한 뒤 데이터화한다.

## 7. Done Criteria

- Draw/Redraw 후보 생성, 후보 선택, 패널 표시 책임이 분리된다.
- 후보 표시와 선택 후 Plate 배치가 기존 UI 흐름과 동일하다.
- 소환수 데이터 적용 후 기존 능력치가 바뀌지 않는다.
- 공격 예측 결과가 소환수별 기존 동작과 일치한다.
- 적 AI 판단과 실행이 분리되어 테스트 가능하다.

## 8. QA Links

- 현재 QA 큐: `qa/current.md`
- 큰 변경 후 전체 확인: `qa/full-check.md`
- 기능별 QA 종합보고서: `QAReports/04_BattleContent_QA.md`, `QAReports/04_BattleContent_QA.csv`
