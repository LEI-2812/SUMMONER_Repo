# StorySystem QA

## 1. 현재 상태 요약

| 항목 | 내용 |
|---|---|
| QA 범위 | CSV 대사 파싱, 대사 진행, Stage 연출, 스토리 진입 조건, 스토리 종료 |
| 현재 상태 | 정적 확인 항목과 스테이지별 런타임 QA 항목 정리 완료 |
| 마지막 정리일 | 2026-05-31 |
| 다음 확인 | PlayMode StageRuntimeFlowPlayModeTests timeout 해소 후 런타임 재검증 |

## 2. QA 체크 항목

| ID | 기능 | 확인 내용 | 상태 |
|---|---|---|---|
| ST-001 | CSV 대사 파싱 | CSV 로드 책임이 분리되어 있는가 | Pass |
| ST-002 | CSV 대사 파싱 | CSV 문자열 파싱 규칙이 유지되는가 | Pass |
| ST-003 | 대사 진행 | 대사 Text UI 출력이 정상 동작하는가 | Pass |
| ST-004 | 대사 진행 | Dialogue/Line 인덱스가 정상 진행되는가 | Pass |
| ST-005 | 스토리 종료 | storyNum에 맞는 다음 씬 이름을 결정하는가 | Pass |
| ST-006 | 스토리 종료 | InteractionController가 씬 이동을 위임하는가 | Pass |
| ST-007 | Stage 연출 | Stage1 입력 공통화가 기존 흐름을 유지하는가 | Pending |
| ST-008 | Stage 연출 | Stage2 입력 공통화가 기존 흐름을 유지하는가 | Pending |
| ST-009 | Stage 연출 | Stage3 입력 공통화가 기존 흐름을 유지하는가 | Pending |
| ST-010 | Stage 연출 | Stage5 입력 공통화가 기존 흐름을 유지하는가 | Pending |
| ST-011 | Stage 연출 | Stage7 입력 공통화가 기존 흐름을 유지하는가 | Pending |
| ST-012 | 자동 검사 | StorySystemStaticRegressionTests가 다시 통과하는가 | Pass |
| ST-013 | 스토리 진입 | 1스테이지 미클리어 상태에서 2스테이지 스토리로 갈 수 없는가 | Pending |
| ST-014 | 스토리 진입 | 2스테이지 미클리어 상태에서 3스테이지 스토리로 갈 수 없는가 | Pending |
| ST-015 | 스토리 진입 | 3스테이지 미클리어 상태에서 5스테이지 스토리로 갈 수 없는가 | Pending |
| ST-016 | 스토리 진입 | 5스테이지 미클리어 상태에서 7스테이지 스토리로 갈 수 없는가 | Pending |
| ST-017 | 스토리 종료 | Stage1 Story 종료 후 Fight Screen_1Stage로 이동하는가 | Pending |
| ST-018 | 스토리 종료 | Stage2 Story 종료 후 Fight Screen_2Stage로 이동하는가 | Pending |
| ST-019 | 스토리 종료 | Stage3 Story 종료 후 Fight Screen_3Stage로 이동하는가 | Pending |
| ST-020 | 스토리 종료 | Stage5 Story 종료 후 Fight Screen_5Stage로 이동하는가 | Pending |
| ST-021 | 스토리 종료 | Stage7 Story 종료 후 Fight Screen_7Stage로 이동하는가 | Pending |
| ST-022 | 스토리 없는 스테이지 | Stage4와 Stage6은 Stage Select에서 바로 전투 씬으로 이동하는가 | Pending |
| ST-023 | 스토리 옵션 | 스토리 스킵 옵션이 켜져 있을 때 스토리 진행 흐름이 의도대로 처리되는가 | Pending |
| ST-024 | 자동 검사 | StageFlowController의 스토리/전투 다음 씬 이동 테스트가 추가되었는가 | Fixed |
| ST-025 | 직접 진입 | Story Screen을 단독 실행해도 필수 참조 누락 없이 시작되는가 | Pending |
| ST-026 | 입력 제한 | Only Mouse 옵션이 켜져 있으면 Space 입력으로 대사가 진행되지 않는가 | Pending |
| ST-027 | 입력 제한 | Only Mouse 옵션이 꺼져 있으면 Space 입력과 클릭 입력이 모두 동작하는가 | Pending |
| ST-028 | 대사 데이터 | storyNum별 CSV 대사 범위가 비어 있지 않은가 | Pending |
| ST-029 | 대사 데이터 | CSV에 빈 줄/쉼표가 포함되어도 파싱이 깨지지 않는가 | Pending |
| ST-030 | 연출 중 입력 | 캐릭터 이동/연출 중 중복 입력으로 대사가 두 번 진행되지 않는가 | Pending |
| ST-031 | 스킵 버튼 | 스토리 스킵 버튼 클릭 시 현재 스테이지의 목적지 씬으로 이동하는가 | Pending |
| ST-032 | Epilogue | Epilogue 종료 후 Thank Screen 또는 Main 흐름으로 정상 이동하는가 | Pending |

## 3. 상세 QA 기록

### ST-001. CSV 대사 파싱 - 로드 책임 분리

- 우선순위: 높음
- 확인 내용: Resources TextAsset 로드 책임이 DialogueDatabaseLoad로 분리되어 있는지 확인
- 기대 결과: DialogueParser가 직접 로드 책임을 갖지 않는다
- 현재 결과: DialogueDatabaseLoad 분리 테스트 통과
- 상태: Pass
- 다음 조치: 추가 조치 없음
- 관련 파일: DialogueDatabaseLoad.cs

---

### ST-002. CSV 대사 파싱 - 문자열 파싱

- 우선순위: 높음
- 확인 내용: CSV 행 분리와 따옴표 제거 규칙이 유지되는지 확인
- 기대 결과: 기존 대사 CSV가 Dialogue 배열로 정상 변환된다
- 현재 결과: DialogueCsvParse 테스트 통과
- 상태: Pass
- 다음 조치: 추가 조치 없음
- 관련 파일: DialogueCsvParse.cs

---

### ST-003. 대사 진행 - Text UI 출력

- 우선순위: 높음
- 확인 내용: DialogueLineShow가 대사 줄을 UI Text에 출력하는지 확인
- 기대 결과: 기존 2줄 단위 결합 규칙이 유지된다
- 현재 결과: DialogueLineShow 분리와 줄 결합 테스트 통과
- 상태: Pass
- 다음 조치: 추가 조치 없음
- 관련 파일: DialogueLineShow.cs

---

### ST-004. 대사 진행 - 인덱스 진행

- 우선순위: 높음
- 확인 내용: Dialogue/Line 인덱스 진행이 StoryProgressAdvance로 위임되어 있는지 확인
- 기대 결과: 다음 대사 이동 흐름이 기존과 동일하다
- 현재 결과: StoryProgressAdvance 위임 정적 테스트 통과
- 상태: Pass
- 다음 조치: 추가 조치 없음
- 관련 파일: StoryProgressAdvance.cs

---

### ST-005. 스토리 종료 - 다음 씬 이름 결정

- 우선순위: 높음
- 확인 내용: storyNum에 맞는 다음 씬 이름을 반환하는지 확인
- 기대 결과: 일반 스토리는 Fight Screen_NStage로 이동하고, Epilogue는 Thank Screen으로 이동한다
- 현재 결과: StorySceneMove.GetNextSceneName 테스트 통과
- 상태: Pass
- 다음 조치: 추가 조치 없음
- 관련 파일: StorySceneMove.cs

---

### ST-006. 스토리 종료 - 씬 이동 위임

- 우선순위: 중간
- 확인 내용: InteractionController가 FadeOut 이후 씬 이동을 StorySceneMove에 위임하는지 확인
- 기대 결과: InteractionController에 씬 이름 결정 책임이 남지 않는다
- 현재 결과: EndDialogue FadeOut 콜백 위임 테스트 통과
- 상태: Pass
- 다음 조치: 추가 조치 없음
- 관련 파일: InteractionController.cs

---

### ST-007. Stage1 입력 공통화

- 우선순위: 중간
- 확인 내용: Stage1_Controller가 StoryScenarioControllerBase를 상속하고 입력 메서드를 중복 구현하지 않는지 확인
- 기대 결과: Space/Click/중복 대사 ID 흐름이 기존과 동일하다
- 현재 결과: 정적 확인 완료, Unity 재실행 필요
- 상태: Pending
- 다음 조치: Unity Test Runner EditMode 재실행
- 관련 파일: Stage1_Controller.cs

---

### ST-008. Stage2 입력 공통화

- 우선순위: 중간
- 확인 내용: Stage2_Controller가 StoryScenarioControllerBase를 상속하고 입력 메서드를 중복 구현하지 않는지 확인
- 기대 결과: Space/Click/중복 대사 ID 흐름이 기존과 동일하다
- 현재 결과: 정적 확인 완료, Unity 재실행 필요
- 상태: Pending
- 다음 조치: Unity Test Runner EditMode 재실행
- 관련 파일: Stage2_Controller.cs

---

### ST-009. Stage3 입력 공통화

- 우선순위: 중간
- 확인 내용: Stage3_Controller가 StoryScenarioControllerBase를 상속하고 입력 메서드를 중복 구현하지 않는지 확인
- 기대 결과: Space/Click/중복 대사 ID 흐름이 기존과 동일하다
- 현재 결과: 정적 확인 완료, Unity 재실행 필요
- 상태: Pending
- 다음 조치: Unity Test Runner EditMode 재실행
- 관련 파일: Stage3_Controller.cs

---

### ST-010. Stage5 입력 공통화

- 우선순위: 중간
- 확인 내용: Stage5_Controller가 StoryScenarioControllerBase를 상속하고 입력 메서드를 중복 구현하지 않는지 확인
- 기대 결과: Space/Click/중복 대사 ID 흐름이 기존과 동일하다
- 현재 결과: 정적 확인 완료, Unity 재실행 필요
- 상태: Pending
- 다음 조치: Unity Test Runner EditMode 재실행
- 관련 파일: Stage5_Controller.cs

---

### ST-011. Stage7 입력 공통화

- 우선순위: 중간
- 확인 내용: Stage7_Controller가 StoryScenarioControllerBase를 상속하고 입력 메서드를 중복 구현하지 않는지 확인
- 기대 결과: Space/Click/중복 대사 ID 흐름이 기존과 동일하다
- 현재 결과: 정적 확인 완료, Unity 재실행 필요
- 상태: Pending
- 다음 조치: Unity Test Runner EditMode 재실행
- 관련 파일: Stage7_Controller.cs

---

### ST-012. 자동 검사 - StorySystem 재실행

- 우선순위: 높음
- 확인 내용: StorySystemStaticRegressionTests가 현재 상태에서 다시 통과하는지 확인
- 기대 결과: EditMode 테스트가 모두 통과한다
- 현재 결과: MCP `run_tests` 클래스 단위 재실행으로 25/25 통과
- 상태: Pass
- 다음 조치: 추가 조치 없음
- 관련 파일: StorySystemStaticRegressionTests.cs

---

### ST-013~ST-016. 스토리 진입 - 스테이지 잠금 연동

- 우선순위: 높음
- 확인 내용: 이전 스테이지를 클리어하지 않은 상태에서 다음 스토리 스테이지로 진입할 수 없는지 확인
- 기대 결과:
  - savedStage=1: 2스테이지 Story 진입 불가
  - savedStage=2: 3스테이지 Story 진입 불가
  - savedStage=3: 5스테이지 Story 진입 불가
  - savedStage=5: 7스테이지 Story 진입 불가
- 현재 결과: Stage Select 버튼 잠금이 savedStage 기준으로 동작하는 구조는 확인됨. 실제 씬 진입 QA 필요
- 상태: Pending
- 다음 조치: savedStage별 Stage Select 버튼 잠금과 직접 진입 방어 범위 확인
- 관련 파일: StageSelectView.cs, StageFlowController.cs

---

### ST-017~ST-021. 스토리 종료 - 전투 씬 이동

- 우선순위: 높음
- 확인 내용: 각 스토리 씬 종료 후 같은 번호의 전투 씬으로 이동하는지 확인
- 기대 결과:
  - Story Screen_1Stage 종료 후 Fight Screen_1Stage 이동
  - Story Screen_2Stage 종료 후 Fight Screen_2Stage 이동
  - Story Screen_3Stage 종료 후 Fight Screen_3Stage 이동
  - Story Screen_5Stage 종료 후 Fight Screen_5Stage 이동
  - Story Screen_7Stage 종료 후 Fight Screen_7Stage 이동
- 현재 결과: StorySceneMove.GetNextSceneName 정적 테스트는 통과 기록이 있음. 실제 연출 종료 타이밍 QA 필요
- 상태: Pending
- 다음 조치: 각 Story Screen에서 마지막 대사/연출 이후 씬 이동 확인
- 관련 파일: StorySceneMove.cs, InteractionController.cs, Stage*_Controller.cs

---

### ST-022. 스토리 없는 스테이지 - Stage4/Stage6 직접 전투

- 우선순위: 높음
- 확인 내용: 4스테이지와 6스테이지는 Stage Select에서 스토리 없이 바로 전투 씬으로 이동하는지 확인
- 기대 결과: Stage4 선택 시 Fight Screen_4Stage, Stage6 선택 시 Fight Screen_6Stage로 이동한다
- 현재 결과: StageSelectView와 StageFlowController에서 4/6스테이지는 SendFight를 호출하는 구조 확인
- 상태: Pending
- 다음 조치: savedStage=4, savedStage=6 상태에서 Stage Select 버튼 클릭 후 실제 씬 이동 확인
- 관련 파일: StageSelectView.cs, StageFlowController.cs

---

### ST-023. 스토리 옵션 - 스토리 스킵

- 우선순위: 중간
- 확인 내용: 스토리 스킵 옵션이 켜져 있을 때 스토리 진행이 의도대로 생략되거나 빠르게 진행되는지 확인
- 기대 결과: 스토리 스킵 설정이 켜진 상태에서 스토리 씬 진행이 멈추지 않고 다음 목적지로 이동한다
- 현재 결과: GameplaySettingView에 IsStorySkip 저장 구조는 있으나 Story 진행 코드와의 연결 여부 확인 필요
- 상태: Pending
- 다음 조치: IsStorySkip=1 저장 후 Stage1/2/3/5/7 Story 진입 결과 확인
- 관련 파일: GameplaySettingView.cs, StoryScenarioControllerBase.cs, InteractionController.cs

---

### ST-024. 자동 검사 - 스테이지별 다음 씬 이동

- 우선순위: 높음
- 확인 내용: 전투 클리어 후 다음 스토리/전투/Epilogue 씬 이동 규칙이 자동 테스트로 검증되는지 확인
- 기대 결과: Stage1 클리어 후 Stage2 Story, Stage3 클리어 후 Stage4 Fight, Stage5 클리어 후 Stage6 Fight, Stage7 클리어 후 Epilogue로 이동한다
- 현재 결과: StageRuntimeFlowPlayModeTests에 StageFlowController_SendNextStageAfterBattle_LoadsExpectedNextScene 테스트 추가, 스크립트 재컴파일 통과. 2026-05-31 PlayMode run_tests는 timeout으로 실행 차단
- 상태: Fixed
- 다음 조치: MCP 연결 복구 후 PlayMode 테스트 재실행
- 관련 파일: StageRuntimeFlowPlayModeTests.cs, StageFlowController.cs

---

### ST-025. 직접 진입 - Story Screen 단독 실행

- 우선순위: 중간
- 확인 내용: Start Screen/Stage Select를 거치지 않고 각 Story Screen을 직접 Play해도 필수 참조가 빠지지 않는지 확인
- 기대 결과: StoryStage, InteractionController, FadePanelView, StorySkipView 참조가 유효하고 시작 시 NullReference가 발생하지 않는다
- 현재 결과: 직접 진입 QA 미실행
- 상태: Pending
- 다음 조치: Story Screen_1/2/3/5/7Stage를 각각 단독 Play 확인
- 관련 파일: StoryStage.cs, InteractionController.cs, Stage*_Controller.cs

---

### ST-026~ST-027. 입력 제한 - Only Mouse 옵션

- 우선순위: 높음
- 확인 내용: Only Mouse 옵션에 따라 Space 입력과 클릭 입력 허용 범위가 맞는지 확인
- 기대 결과:
  - IsOnlyMouse=1: 클릭 입력만 대사를 진행하고 Space 입력은 무시된다
  - IsOnlyMouse=0: 클릭 입력과 Space 입력이 모두 대사를 진행한다
- 현재 결과: GameplaySettingView에 IsOnlyMouse 저장 구조는 있으나 StoryScenarioControllerBase 입력 처리와의 연결 여부 확인 필요
- 상태: Pending
- 다음 조치: 옵션 저장값을 바꾼 뒤 Story Screen에서 Space/Click 입력 테스트
- 관련 파일: GameplaySettingView.cs, StoryScenarioControllerBase.cs

---

### ST-028~ST-029. 대사 데이터 - CSV 안정성

- 우선순위: 중간
- 확인 내용: storyNum별 대사 데이터가 비어 있지 않고, 빈 줄/쉼표/따옴표가 포함되어도 파싱이 깨지지 않는지 확인
- 기대 결과: 각 스토리 번호별 Dialogue 배열이 존재하고, 문장 내부 쉼표가 대사 줄을 잘못 분리하지 않는다
- 현재 결과: CSV 파싱 책임 분리 테스트는 있으나 실제 데이터 전체 검증은 필요
- 상태: Pending
- 다음 조치: Resources CSV 전체를 대상으로 storyNum별 대사 수와 파싱 오류 확인
- 관련 파일: DialogueCsvParse.cs, DialogueParser.cs, SummonContext.csv

---

### ST-030. 연출 중 입력 - 중복 진행 방지

- 우선순위: 중간
- 확인 내용: 캐릭터 이동, 페이드, 이미지 전환 같은 연출 중 입력을 반복해도 대사가 두 번 넘어가지 않는지 확인
- 기대 결과: 연출이 끝나기 전 중복 입력은 무시되거나 한 번만 처리된다
- 현재 결과: 연출 중 입력 잠금 여부 미확인
- 상태: Pending
- 다음 조치: 각 Stage 연출 중 Space/Click 연타 후 대사 인덱스와 연출 상태 확인
- 관련 파일: StoryScenarioControllerBase.cs, Stage*_Controller.cs, PlayerMove.cs

---

### ST-031. 스킵 버튼 - 목적지 씬 이동

- 우선순위: 높음
- 확인 내용: 스토리 스킵 버튼을 누르면 현재 스테이지 번호 기준 목적지 씬으로 이동하는지 확인
- 기대 결과: Stage1/2/3/5/7 Story에서 Skip 시 같은 번호 Fight Scene으로 이동한다
- 현재 결과: StorySkipView 참조 유지 정적 확인 기록은 있으나 런타임 이동 QA 필요
- 상태: Pending
- 다음 조치: 각 Story Screen에서 Skip 클릭 후 현재 씬 확인
- 관련 파일: StorySkipView.cs, StorySceneMove.cs

---

### ST-032. Epilogue - 종료 흐름

- 우선순위: 중간
- 확인 내용: Epilogue 종료 후 Thank Screen 또는 Main 흐름으로 정상 이동하는지 확인
- 기대 결과: 7스테이지 이후 엔딩 흐름이 끊기지 않고 완료 화면으로 이어진다
- 현재 결과: StageFlowController는 Epilogue Screen으로 이동함. Epilogue 이후 흐름은 QA 필요
- 상태: Pending
- 다음 조치: Epilogue Screen 마지막 입력 후 목적지 씬 확인
- 관련 파일: StageFlowController.cs, StorySceneMove.cs, MainSceneButtonView.cs

## 4. 발견한 버그

| ID | 날짜 | 내용 | 상태 |
|---|---|---|---|
| BUG-ST-001 | 2026-05-31 | `StorySystemStaticRegressionTests` 25/25 통과로 Stage 입력 공통화 이후 EditMode 재검증 완료 | Closed |
| BUG-ST-002 | 2026-05-31 | 스토리 스킵 옵션 저장값은 `StorySkipView`에 정적 연결 확인, 실제 Story 진행 흐름은 미확인 | Open Runtime QA |
| BUG-ST-003 | 2026-05-31 | MCP PlayMode `StageRuntimeFlowPlayModeTests`가 timeout으로 StageFlowController 자동 테스트 실행 불가 | Open Blocked |
| BUG-ST-004 | 2026-05-31 | Only Mouse 옵션이 Story 입력 처리에 반영되지 않음. `StoryScenarioControllerBase`가 Space 입력에서 옵션값을 확인하지 않음 | Open Fail |

## 5. QA 히스토리

| 날짜 | 확인 범위 | 결과 | 메모 |
|---|---|---|---|
| 2026-05-31 | StorySceneMove, DialogueLineShow, DialogueCsvParse | 통과 | 주요 분리 로직 테스트 통과 |
| 2026-05-31 | Stage 입력 공통화 | 조건부 통과 | 정적 확인 완료, Unity 재실행 필요 |
| 2026-05-31 | 스테이지별 Story 진입/종료 QA 항목 확장 | 대기 | 잠금, 종료 후 전투 이동, 4/6 직접 전투 기준 추가 |
| 2026-05-31 | StageFlowController 다음 씬 이동 테스트 추가 | 조건부 통과 | 컴파일 통과, 2026-05-31 PlayMode 실행은 timeout으로 차단 |
| 2026-05-31 | Story 예외 QA 항목 확장 | 대기 | 직접 진입, 입력 옵션, CSV 데이터, 연출 중 입력, Skip, Epilogue 기준 추가 |
| 2026-05-31 | QA 인덱스 재실행 | 부분 완료 | `recompile_scripts` warning 0, `StorySystemStaticRegressionTests` 25/25 통과, `StageRuntimeFlowPlayModeTests` PlayMode는 timeout; Only Mouse 정적 실패 확인 |
