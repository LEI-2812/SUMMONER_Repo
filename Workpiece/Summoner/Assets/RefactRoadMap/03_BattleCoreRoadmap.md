# BattleCore Roadmap

## 1. Role

BattleCore는 전투 규칙의 핵심 흐름을 담당한다.
턴 시작/종료, 마나, 공격 실행, 상태이상, Plate 전투 칸, 승리/패배 판정이 이 문서의 범위다.

소환 후보 뽑기, 후보 패널, Redraw, 소환수 데이터와 적 AI 콘텐츠 흐름은 `04_BattleContentRoadmap.md`에서 관리한다.
현재 QA 상태나 버그 우선순위는 이 문서에 두지 않고 `qa/00_current.md`에서 관리한다.

## 2. Ownership

| 영역 | 주요 파일/폴더 | 책임 |
|---|---|---|
| 전투 흐름 | `Assets/Script/Battle/Flow` | 턴 진행, 승패 판정, 결과 Alert 연결 |
| 공격 실행 | `Assets/Script/Battle/Attack` | 대상 선택 결과를 받아 공격 효과 실행 |
| 상태이상 | `Assets/Script/Battle/Status` | 상태 적용, 턴 갱신, 만료 처리 |
| 전투 유닛 | `Assets/Script/Battle/Unit`, `Assets/Script/Summons` | 플레이어, 적, 소환수, Plate 데이터 |
| Plate 제어 | `Assets/Script/Battle/PlateControl` | Plate 조회, 선택, 이동/정렬, 표시 연결 |
| 전투 View | `Assets/Script/Battle/View` | 전투 UI 표시, 이미지/상태/사운드 표시 |

## 3. Current Structure

- 전투 결과 처리는 `BattleResultController`, `BattleProgressController`, `BattleStageContext` 기준으로 분리한다.
- Plate는 소환수가 배치되는 전투 칸이며, 조회/대상 선택/이동/표시 책임이 아직 강하게 연결되어 있다.
- 상태이상 계산은 `Battle/Status`에 두고, 표시와 사운드는 View 쪽으로 분리하는 방향을 유지한다.
- 공격 전략은 대상 목록을 받아 효과를 실행하는 구조를 목표로 한다.
- 턴 흐름은 상태 갱신, 쿨타임 감소, 마나 회복, 승패 확인 순서를 명확히 해야 한다.

## 4. Refactoring Direction

- BattleController가 모든 전투 규칙을 직접 처리하지 않도록 흐름, 대상 선택, 공격 실행을 나눈다.
- PlateController는 조회, 이동/정렬, target query, View 표시 책임을 단계적으로 분리한다.
- 상태이상은 계산과 표시를 분리하고, 턴 갱신 타이밍을 한곳에서 관리한다.
- 승리/패배 Alert는 전투씬 단독으로 필요한 context를 받아 표시되게 유지한다.

### PlateController Split Plan

| 이름 | 역할 | 목적 |
|---|---|---|
| `PlateFinder` | player/enemy Plate와 소환수 상태 조회 | 리스트 직접 접근을 줄임 |
| `PlateMover` | 소환수 이동, 사망 후 Plate 정렬 | 이동/압축 규칙을 분리 |
| `PlateTargetFinder` | 공격/회복 대상 Plate 인덱스 찾기 | 대상 선택 기준을 테스트 가능하게 분리 |
| `PlateView` | Plate 숨김, 표시, 투명도, 하이라이트 | 화면 표시 책임을 View로 제한 |

## 5. Constraints

- 플레이어와 적 행동에서 같은 전투 규칙이 유지되어야 한다.
- 소환수 이미지, 색상, 효과음 표시가 상태 계산 변경 때문에 깨지면 안 된다.
- Plate 인덱스와 target 선택 기준을 바꿀 때는 공격/회복 대상 규칙을 먼저 고정해야 한다.
- 승리/패배 Alert는 전투씬에 없는 StageController에 직접 의존하지 않아야 한다.
- NullReferenceException을 막기 위한 자동 생성/자동 복구가 기존 씬 연결 문제를 숨기면 안 된다.

## 6. Next Work Candidates

| ID | 작업 | 목적 |
|---|---|---|
| BC-REF-01 | 소환수 View 초기화 경로 정리 | 전투 진입 중 `Summon.Update()` null 참조 방지 |
| BC-REF-02 | BattleController 공격 실행 분리 | 공격 명령, 대상 선택, 실행 책임 분리 |
| BC-REF-03 | Player 마나와 입력 책임 분리 | 마나 UI와 공격 요청 결합 축소 |
| BC-REF-04 | Plate 조회 API 정리 | Plate list 직접 반환을 query API로 대체 |
| BC-REF-05 | TurnController 단계 분리 | 상태 갱신, 쿨타임, 마나 회복, 결과 확인 순서 명시 |

## 7. Done Criteria

- 턴 시작/종료 흐름이 명확히 분리된다.
- 상태이상 적용, 턴 갱신, 표시 책임이 섞이지 않는다.
- 공격 대상 선택과 공격 실행이 분리된다.
- Plate 조회, 이동/정렬, 화면 표시 책임이 분리된다.
- 승리/패배 Alert가 전투씬 단독 흐름에서 NullReference 없이 표시된다.

## 8. QA Links

- 현재 QA 큐: `qa/00_current.md`
- 큰 변경 후 전체 확인: `qa/20_full-check.md`
- 기능별 QA 종합보고서: `QAReports/03_BattleCore_QA.md`, `QAReports/03_BattleCore_QA.csv`
