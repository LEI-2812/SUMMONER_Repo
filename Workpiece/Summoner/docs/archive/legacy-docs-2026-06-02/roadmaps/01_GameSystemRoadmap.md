# GameSystem Roadmap

## 1. Role

GameSystem은 시작 화면, 새 게임, 이어하기, 저장 데이터, 스테이지 선택, 전투 클리어 후 진행 저장을 담당한다.
옵션 UI 자체는 `05_OptionSystemRoadmap.md`에서 관리하고, 옵션 값 보존 여부만 GameSystem 경계에서 확인한다.

현재 QA 상태나 버그 우선순위는 이 문서에 두지 않고 `qa/current.md`에서 관리한다.

## 2. Ownership

| 영역 | 주요 파일/폴더 | 책임 |
|---|---|---|
| 저장 데이터 | `Assets/Script/Save` | 저장값 모델, PlayerPrefs 저장소, 저장 읽기/쓰기 |
| 시작 화면 | `Assets/Script/Start Screen` | 새 게임, 이어하기, 시작 화면 버튼 흐름 |
| 스테이지 선택 | `Assets/Script/Stage` | 스테이지 버튼, 잠금/해금 표시, 선택한 스테이지 저장 |
| 전투 진행 저장 | `Assets/Script/Battle/Flow` | 전투 결과 이후 진행도 저장과 다음 흐름 연결 |

## 3. Current Structure

- 저장 접근은 `GameSaveController`와 `PlayerPrefsSaveStore` 기준으로 모은다.
- 저장 데이터는 진행도와 옵션 값을 섞지 않는 방향으로 유지한다.
- 이어하기 가능 여부는 저장된 진행도 기준으로 판단한다.
- 스테이지 선택 UI는 저장된 진행도보다 앞선 스테이지를 열지 않아야 한다.
- 전투 클리어 후 진행 저장은 BattleCore 결과 흐름과 연결되지만 저장 정책은 GameSystem에서 정의한다.

## 4. Refactoring Direction

- PlayerPrefs 키 직접 접근을 줄이고 저장소 API를 통해 읽고 쓴다.
- 새 게임, 이어하기, 스테이지 선택, 전투 클리어 저장을 같은 진행도 규칙으로 맞춘다.
- 저장값 검증과 fallback을 저장 계층 가까이에 둔다.
- UI View는 화면 표시와 버튼 이벤트 전달만 맡기고 저장 정책 판단은 controller로 옮긴다.

## 5. Constraints

- 새 게임은 진행 데이터만 초기화하고 오디오, 비디오, 게임플레이 옵션 값을 지우지 않는다.
- 저장된 진행도보다 높은 스테이지로 직접 진입하지 않는다.
- `savedStage`, `playingStage` 값이 비정상이면 안전한 기본값으로 복구해야 한다.
- 씬 이름과 Stage 번호 매핑을 바꿀 때는 StorySystem과 BattleCore 이동 흐름을 함께 확인한다.

## 6. Next Work Candidates

| ID | 작업 | 목적 |
|---|---|---|
| GS-REF-01 | StageFlow 중복 제거 | 새 게임, 이어하기, 스테이지 선택의 이동 규칙을 한곳에서 관리 |
| GS-REF-02 | 저장값 검증 위치 정리 | `savedStage`, `playingStage` 범위 방어를 저장 계층에 고정 |
| GS-REF-03 | 스테이지 UI와 저장 정책 분리 | Stage Select View가 저장 규칙을 직접 판단하지 않게 정리 |

## 7. Done Criteria

- 저장 없음 상태와 저장 있음 상태의 시작 화면 동작이 명확하다.
- 새 게임은 진행도만 초기화하고 옵션 설정은 유지한다.
- Stage Select는 저장된 진행도 범위만 선택 가능하게 한다.
- 전투 클리어 후 다음 진행도가 일관되게 저장된다.
- 비정상 저장값에도 잠긴 스테이지로 진입하지 않는다.

## 8. QA Links

- 현재 QA 큐: `qa/current.md`
- 큰 변경 후 전체 확인: `qa/full-check.md`
- 기능별 QA 종합보고서: `QAReports/01_GameSystem_QA.md`, `QAReports/01_GameSystem_QA.csv`
