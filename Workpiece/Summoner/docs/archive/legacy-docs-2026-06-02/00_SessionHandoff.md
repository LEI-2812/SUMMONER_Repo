# SUMMONER Refactoring Session Handoff

이 문서는 세션을 끄고 다시 켰을 때 이어서 보기 위한 인계 문서다.
초기 리팩토링 정보는 기능별 로드맵 5개에 둔다.
현재 QA 우선순위와 버그 우선순위는 `qa/current.md`에 둔다.
`QAReports` 문서는 기능별 QA 종합보고서와 이전 상세 기록 보관용으로 유지한다. 현재 QA 큐는 관리하지 않는다.
개발과 QA는 `Agents/DevQAWorkflow.md` 기준으로 DevAgent와 QAAgent 역할을 분리한다.
한 세션에서 DevAgent와 QAAgent는 절대 서로 전환하지 않는다.
전환되는 것은 QA 항목의 상태와 담당 단계뿐이다.

## 1. Last Session Summary

| 영역 | 작업 내용 | 현재 상태 |
| --- | --- | --- |
| 문서 구조 | 세션 인계 문서와 기능별 로드맵 5개로 정리 | Done |
| 문서 구조 | 6번/7번 Plate 세부 로드맵을 BattleCore/BattleContent로 흡수 | Done |
| GameSystem | 저장 접근을 `GameSaveController`, `PlayerPrefsSaveStore` 기준으로 정리 | Retest |
| GameSystem | 이어하기 저장 있음 판단을 `savedStage >= 1` 기준으로 정리 | Retest |
| StorySystem | `StorySceneMove`, `DialogueLineShow`, `StoryProgressAdvance`, `DialogueDatabaseLoad`, `DialogueCsvParse` 연결 | Retest |
| StorySystem | Stage1/2/3/5/7 입력 공통화 적용 | Retest |
| BattleCore | QA 중 적 전멸 승리 Alert 경로 문제 발견 | Open |
| BattleContent | 소환수 뽑기, 후보, 패널 표시 분리 후보 정리 | Next |
| OptionSystem | 옵션 저장 유지와 Story Skip/Only Mouse 실제 반영 QA 항목 정리 | Retest |

## 2. Next Session Start

1. `qa/current.md`의 현재 QA 목록을 먼저 확인한다.
2. 이번 세션 역할이 DevAgent인지 QAAgent인지 정한다.
3. DevAgent면 `Agents/DevAgent.md`, QAAgent면 `Agents/QAAgent.md`를 읽는다.
4. 선택한 역할은 세션 중 유지한다. 다른 역할의 작업이 필요하면 요청과 상태만 남긴다.
5. QA 항목의 상태와 담당 단계 전환은 `Agents/DevQAWorkflow.md`를 따른다.
6. Open 버그가 있으면 새 리팩토링보다 먼저 처리한다.
7. 작업할 기능별 로드맵 하나를 연다.
8. 큰 변경이면 `qa/full-check.md`를 확인하고, 필요한 경우 `QAReports` 기능별 종합보고서를 조회한다.
9. 작업 후에는 이 문서의 Last Session Summary와 Next Session Start만 짧게 갱신한다.

## 3. Active Roadmaps

| 문서 | 역할 |
| --- | --- |
| `01_GameSystemRoadmap.md` | 저장, 새 게임, 이어하기, 스테이지 선택 |
| `02_StorySystemRoadmap.md` | 대사, 스토리 입력, 스토리 씬 이동 |
| `03_BattleCoreRoadmap.md` | 턴, 마나, 공격, 상태이상, 승패 |
| `04_BattleContentRoadmap.md` | 소환, 소환수 데이터, 예측, 적 AI |
| `05_OptionSystemRoadmap.md` | 설정 UI, 오디오, 비디오, 게임플레이 옵션 |
| `RoadmapTemplate.md` | 기능별 로드맵 공통 템플릿 |

Roadmap은 기능별 설계 방향, 소유 범위, 리팩토링 기준을 기록한다.
현재 QA 상태와 버그 우선순위는 Roadmap에 두지 않고 `qa/current.md`에서 관리한다.

## 3.1 QA Documents

| 문서 | 역할 |
| --- | --- |
| `qa/current.md` | 현재 수정 중이거나 다시 확인해야 하는 QA만 관리 |
| `qa/full-check.md` | 큰 수정 후 전체 회귀 확인 목록 |
| `qa/done/YYYY-MM.md` | 완료된 QA 보관 |
| `QAReports/*.md` / `.csv` | 기능별 QA 종합보고서와 이전 상세 기록. 현재 큐 관리 금지 |

## 4. Working Rules

- 리팩토링 단위는 작게 유지한다.
- 기존 동작을 바꾸는 작업은 먼저 사용자에게 확인한다.
- 씬, 프리팹, 저장 키, 저장 데이터 구조를 대량 변경하지 않는다.
- 자동 테스트가 가능하면 EditMode 또는 PlayMode 테스트를 추가하거나 갱신한다.
- 테스트로 다루기 어려운 내용은 `qa/current.md`에 Open 또는 Retest로 남긴다.
- MCP, Unity Editor, 외부 실행 문제는 코드 실패와 구분해서 Hold로 기록한다.
- 긴 테스트 출력과 검색 결과는 로드맵에 붙이지 않는다.
- `WorkLogs/YYYY-MM-DD.md`는 날짜별 공용 로그이므로 섹션 제목을 반드시 `DevAgent - 작업명` 또는 `QAAgent - 작업명`으로 시작한다.
- WorkLog에 다른 역할 기록이 이미 있어도 역할 전환으로 보지 않는다. 현재 에이전트는 자신의 역할 접두사가 붙은 새 섹션만 추가한다.
- WorkLog는 상세 이력 저장소가 아니라 요약 문서다. 프로젝트를 처음 보는 사람도 이해할 수 있게 `작업 요약`, `작업 배경`, `작업 내용`, `확인한 내용`, `발생한 문제와 해결`, `남은 작업`, `참고 메모` 구조로 기록한다.
- 최소한 Unity와 C#을 배운 학부생이 읽고 기능의 목적, 기존 문제, 변경 후 구조, 테스트 결과, 남은 작업을 이해할 수 있어야 한다.
- 내부 약어를 제목에 쓰더라도 본문 첫 줄에서는 그 작업이 게임에서 어떤 기능과 관련되는지 풀어서 설명한다.
- 클래스명만 나열하지 말고, 그 클래스가 게임 안에서 맡는 역할을 함께 적는다.
- `Open`, `Fixed`, `Retest`, `Hold` 같은 QA 상태는 왜 그렇게 판단했는지 한 줄로 설명한다.
- WorkLog는 날짜별 파일 하나만 사용한다. 새 파일이 필요하면 `Assets/RefactRoadMap/WorkLogs/YYYY-MM-DD.md` 형식으로 만들고, 작업명이나 티켓 ID를 파일명에 붙인 `YYYY-MM-DD_<작업명>.md` 파일은 만들지 않는다.
- 새 날짜별 WorkLog를 작성할 때는 `Assets/RefactRoadMap/WorkLogs/WorkLogTemplate.md` 양식을 먼저 복사해 사용한다.
- 변경 파일 전체 목록, 긴 로그, 구현 세부 단계는 WorkLog에 누적하지 않는다. 필요한 경우 QAReports나 기능별 로드맵에 추적 항목만 남긴다.

## 5. Recording Rules

| 위치 | 기록 내용 |
| --- | --- |
| `00_SessionHandoff.md` | 이전 세션 요약, 다음 세션 시작 절차 |
| `Agents/DevQAWorkflow.md` | DevAgent와 QAAgent 역할 고정, QA 상태와 담당 단계 전환, 실패 리포트 규칙 |
| `Agents/DevAgent.md` | 개발 에이전트 작업 규칙과 QA 요청 형식 |
| `Agents/QAAgent.md` | QA 에이전트 실행 규칙과 결과 기록 형식 |
| 기능별 로드맵 5개 | 초기 리팩토링 정보, 현재 구조, 활성 작업, Done Criteria |
| `qa/current.md` | 현재 QA/버그 우선순위 |
| `qa/full-check.md` | 전체 회귀 확인 기준 |
| `qa/done/YYYY-MM.md` | 완료된 QA 요약 보관 |
| 기능별 `QAReports` | 기능별 QA 종합보고서, 이전 상세 항목, 실행 결과, 발견 버그 보관. 현재 큐 관리 금지 |
| `WorkLogs/YYYY-MM-DD.md` | 날짜별 공용 작업 로그. 모든 섹션 제목에 DevAgent/QAAgent 접두사 필수 |
| `FileStateIndex.md` | 파일 상태 추적 |

## 6. Removed Documents

| 문서 | 처리 |
| --- | --- |
| `AutoRefactorMode.md` | 핵심 규칙을 이 문서의 Working Rules로 흡수하고 제거 |
| `CompletedArchive.md` | 완료 요약을 기능별 로드맵과 이 문서의 Last Session Summary로 흡수하고 제거 |
| `06_PlateSummonDrawRefactorPlan.md` | 핵심 내용을 `03_BattleCoreRoadmap.md`와 `04_BattleContentRoadmap.md`로 흡수하고 제거 |
| `07_PlateControllerAnalysis.md` | PlateController 분리 요약을 `03_BattleCoreRoadmap.md`로 흡수하고 제거 |
