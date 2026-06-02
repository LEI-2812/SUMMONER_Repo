# StorySystem Roadmap

## 1. Role

StorySystem은 CSV 대사 로드/파싱, 대사 출력, 입력 처리, Stage별 스토리 연출, 스토리 종료 후 씬 이동을 담당한다.
옵션의 Story Skip, Only Mouse 저장값은 `05_OptionSystemRoadmap.md`에서 관리하고, 실제 입력 반영은 StorySystem 경계에서 확인한다.

현재 QA 상태나 버그 우선순위는 이 문서에 두지 않고 `qa/current.md`에서 관리한다.

## 2. Ownership

| 영역 | 주요 파일/폴더 | 책임 |
|---|---|---|
| 대사 데이터 | `Assets/Script/Story/Dialogue` | CSV 로드, 파싱, 대사 데이터 제공 |
| 스토리 진행 | `Assets/Script/Story/Progress` | 대사 진행, 다음 씬 결정, 스토리 종료 처리 |
| Stage별 연출 | `Assets/Script/Story/Scenario` | Stage별 이미지, 사운드, 이벤트 연출 |
| 이동/입력 보조 | `Assets/Script/Story/Movement` | 스토리 씬의 캐릭터 이동과 입력 보조 |

## 3. Current Structure

- 대사 로드와 파싱은 `DialogueDatabaseLoad`, `DialogueCsvParse`처럼 데이터 책임으로 분리한다.
- 대사 표시와 진행은 UI 표시 책임과 진행 책임을 나눠 유지한다.
- Stage별 Controller는 각 Stage 연출 차이만 갖고, 공통 입력과 진행 규칙은 공통 계층으로 모은다.
- 스토리 종료 후 다음 씬 결정은 StorySystem 내부에서 명시적으로 관리한다.
- Story Skip과 Only Mouse는 옵션값을 읽되, 입력 처리 정책은 StorySystem에서 최종 반영한다.

## 4. Refactoring Direction

- Stage1/2/3/5/7 Controller의 중복 입력 처리와 종료 처리를 공통화한다.
- CSV 데이터 로드, 파싱, 표시, 진행을 서로 직접 침범하지 않게 유지한다.
- Story Scene 이동 규칙은 테스트 가능한 작은 API로 유지한다.
- Stage별 특수 연출은 공통 진행 흐름 위에 얹는 방식으로 제한한다.

## 5. Constraints

- 기존 CSV 대사 순서와 Stage별 연출 타이밍을 임의로 바꾸지 않는다.
- Story Skip은 의도한 목적지 씬으로 이동해야 하며 진행도 저장 흐름과 충돌하면 안 된다.
- Only Mouse가 켜져 있으면 Space 입력으로 대사가 진행되지 않아야 한다.
- Story 종료 후 Fight Scene 또는 Epilogue 이동 규칙은 Stage 번호와 일치해야 한다.

## 6. Next Work Candidates

| ID | 작업 | 목적 |
|---|---|---|
| ST-REF-01 | Stage별 Controller 잔여 중복 정리 | Stage별 연출 차이만 남기고 입력/진행 중복 제거 |
| ST-REF-02 | Story 입력 정책 명시화 | 클릭, Space, Only Mouse 조건을 한곳에서 판단 |
| ST-REF-03 | Story Skip 이동 정책 정리 | Skip 버튼 표시와 목적지 결정 흐름을 분리 |

## 7. Done Criteria

- CSV 대사가 기존 범위와 순서대로 로드된다.
- 클릭과 Space 입력이 옵션 조건에 맞게 대사를 진행한다.
- Stage별 기존 연출이 공통화 후에도 유지된다.
- 스토리 종료 후 올바른 Fight Scene 또는 Epilogue로 이동한다.
- Story Skip 버튼과 옵션 값이 목적지 씬 이동과 충돌하지 않는다.

## 8. QA Links

- 현재 QA 큐: `qa/current.md`
- 큰 변경 후 전체 확인: `qa/full-check.md`
- 기능별 QA 종합보고서: `QAReports/02_StorySystem_QA.md`, `QAReports/02_StorySystem_QA.csv`
