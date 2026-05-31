# GameSystem QA

## 1. 현재 상태 요약

| 항목 | 내용 |
|---|---|
| QA 범위 | 시작 화면, 새 게임, 이어하기, 저장 데이터, 스테이지 잠금/해금, 스테이지 진행 |
| 현재 상태 | 스테이지별 진행 QA 항목 확장 완료, 런타임 확인 필요 |
| 마지막 정리일 | 2026-05-31 |
| 다음 확인 | Unity Test Runner PlayMode 재실행 |

## 2. QA 체크 항목

| ID | 기능 | 확인 내용 | 상태 |
|---|---|---|---|
| GS-001 | 이어하기 | 저장 시스템 연결이 GameSaveController를 통해 동작하는가 | Pass |
| GS-002 | 이어하기 | savedStage가 1 이상일 때 저장 있음으로 판단하는가 | Pass |
| GS-003 | 이어하기 | 저장 데이터가 없으면 이어하기 버튼이 숨겨지는가 | Pending |
| GS-004 | 이어하기 | 저장 데이터가 있으면 이어하기 버튼이 표시되는가 | Blocked |
| GS-005 | 이어하기 | 이어하기 클릭 시 savedStage를 로드하는가 | Blocked |
| GS-006 | 새 게임 시작 | 새 게임 시작 시 기존 옵션 설정을 유지하는가 | Pending |
| GS-007 | 스테이지 선택 | 선택한 playingStage가 저장되는가 | Pending |
| GS-008 | 전투 클리어 | 클리어한 savedStage가 저장되는가 | Fixed |
| GS-009 | 자동 검사 | PlayerPrefs 직접 접근 위치가 다시 늘어나지 않는가 | Pass |
| GS-010 | 스테이지 잠금 | savedStage=1이면 1스테이지만 선택 가능한가 | Pending |
| GS-011 | 스테이지 잠금 | savedStage=2이면 1~2스테이지만 선택 가능한가 | Pending |
| GS-012 | 스테이지 잠금 | savedStage=3이면 1~3스테이지만 선택 가능한가 | Pending |
| GS-013 | 스테이지 잠금 | savedStage=4이면 1~4스테이지만 선택 가능한가 | Pending |
| GS-014 | 스테이지 잠금 | savedStage=5이면 1~5스테이지만 선택 가능한가 | Pending |
| GS-015 | 스테이지 잠금 | savedStage=6이면 1~6스테이지만 선택 가능한가 | Pending |
| GS-016 | 스테이지 잠금 | savedStage=7이면 1~7스테이지가 선택 가능한가 | Pending |
| GS-017 | 이어하기 | 1스테이지 클리어 후 재시작하면 2스테이지 진행 상태인가 | Pending |
| GS-018 | 이어하기 | 2스테이지 클리어 후 재시작하면 3스테이지 진행 상태인가 | Pending |
| GS-019 | 이어하기 | 3스테이지 클리어 후 재시작하면 4스테이지 진행 상태인가 | Pending |
| GS-020 | 이어하기 | 4스테이지 클리어 후 재시작하면 5스테이지 진행 상태인가 | Pending |
| GS-021 | 이어하기 | 5스테이지 클리어 후 재시작하면 6스테이지 진행 상태인가 | Pending |
| GS-022 | 이어하기 | 6스테이지 클리어 후 재시작하면 7스테이지 진행 상태인가 | Pending |
| GS-023 | 이어하기 | 7스테이지 클리어 후 재시작하면 Epilogue 또는 완료 상태로 이어지는가 | Pending |
| GS-024 | 진행도 보호 | 이전 스테이지를 다시 클리어해도 savedStage가 낮아지지 않는가 | Pending |
| GS-025 | 씬 흐름 | Start Screen에서 생성된 GameSaveController와 StageFlowController가 씬 이동 후 유지되는가 | Pending |
| GS-026 | 자동 검사 | 스테이지별 저장/잠금/다음 씬 이동 테스트가 추가되었는가 | Fixed |
| GS-027 | 저장값 방어 | savedStage가 0 이하이면 이어하기가 비활성화되는가 | Pending |
| GS-028 | 저장값 방어 | savedStage가 8 이상이면 Stage Select 표시가 7스테이지로 제한되는가 | Pending |
| GS-029 | 저장값 방어 | playingStage가 savedStage보다 커도 잠긴 스테이지로 진입하지 않는가 | Pending |
| GS-030 | 새 게임 시작 | 새 게임 확인 Alert에서 No를 누르면 진행 데이터가 바뀌지 않는가 | Pending |
| GS-031 | 이어하기 | 이어하기 확인 Alert에서 No를 누르면 씬 이동하지 않는가 | Pending |
| GS-032 | 중복 입력 | 새 게임/이어하기 버튼을 빠르게 여러 번 눌러도 Alert와 씬 이동이 중복 실행되지 않는가 | Pending |
| GS-033 | 직접 진입 | Stage Select Screen을 단독 실행해도 저장 컨트롤러 누락으로 진행이 깨지지 않는가 | Pending |
| GS-034 | 빌드 세팅 | Start/Stage/Story/Fight/Epilogue/Thank/HUD 씬이 Build Settings에 등록되어 있는가 | Pending |

## 3. 상세 QA 기록

### GS-001. 이어하기 - 저장 시스템 연결

- 우선순위: 높음
- 확인 내용: StartSavedStage가 GameSaveController를 사용하고, HasGameSave 조건이 강화되어 있는지 확인
- 기대 결과: 이어하기 흐름이 저장 시스템을 통해 동작한다
- 현재 결과: StartSavedStage가 GameSaveController를 사용하고 있음
- 상태: Pass
- 다음 조치: 추가 조치 없음
- 관련 파일: StartScreenView.cs

---

### GS-002. 이어하기 - 저장 있음 판단

- 우선순위: 높음
- 확인 내용: savedStage가 1 이상일 때만 저장 데이터가 있다고 판단하는지 확인
- 기대 결과: savedStage가 0이면 저장 없음, 1 이상이면 저장 있음
- 현재 결과: HasGameSave가 savedStage >= 1 기준으로 판단함
- 상태: Pass
- 다음 조치: 추가 조치 없음
- 관련 파일: PlayerPrefsSaveStore.cs

---

### GS-003. 이어하기 - 저장 없음 버튼 숨김

- 우선순위: 중간
- 확인 내용: 저장 데이터가 없을 때 이어하기 버튼이 숨겨지는지 확인
- 기대 결과: 이어하기 버튼이 보이지 않는다
- 현재 결과: PlayMode 테스트는 추가되었지만 아직 실행 전
- 상태: Pending
- 다음 조치: Unity Test Runner에서 GameSaveContinueTests 실행
- 관련 파일: GameSaveContinueTests.cs

---

### GS-004. 이어하기 - 저장 있음 버튼 표시

- 우선순위: 중간
- 확인 내용: 저장 데이터가 있을 때 이어하기 버튼이 표시되는지 확인
- 기대 결과: 이어하기 버튼이 보인다
- 현재 결과: MCP PlayMode timeout으로 확인하지 못함
- 상태: Blocked
- 다음 조치: Unity Test Runner에서 PlayMode 테스트 재실행
- 관련 파일: GameSaveContinueTests.cs

---

### GS-005. 이어하기 - savedStage 로드

- 우선순위: 중간
- 확인 내용: 이어하기 클릭 시 저장된 savedStage로 이동하는지 확인
- 기대 결과: 저장된 스테이지로 이동한다
- 현재 결과: MCP PlayMode timeout으로 확인하지 못함
- 상태: Blocked
- 다음 조치: Unity Test Runner에서 PlayMode 테스트 재실행
- 관련 파일: GameSaveContinueTests.cs

---

### GS-006. 새 게임 시작 - 옵션 유지

- 우선순위: 높음
- 확인 내용: 새 게임 시작 시 저장 데이터는 초기화하되 옵션 값은 유지되는지 확인
- 기대 결과: 새 게임은 Stage1부터 시작하고 옵션 설정은 유지된다
- 현재 결과: StartNewGame 연결은 확인됨
- 상태: Pending
- 다음 조치: DeleteAll 잔존 여부 확인
- 관련 파일: StartScreenEvent.cs

---

### GS-007. 스테이지 선택 - playingStage 저장

- 우선순위: 중간
- 확인 내용: 스테이지 선택 시 playingStage가 저장되는지 확인
- 기대 결과: 선택한 스테이지 번호가 저장된다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: SavePlayingStage 연결 후 검사
- 관련 파일: StageSelectView.cs

---

### GS-008. 전투 클리어 - savedStage 저장

- 우선순위: 높음
- 확인 내용: 전투 클리어 후 savedStage 진행 값이 저장되는지 확인
- 기대 결과: 클리어한 스테이지 기준으로 진행도가 저장된다
- 현재 결과: BattleResultAlertView에서 SaveClearedStage(stageNum + 1) 호출을 정적 확인함. Alert 클릭 후 런타임 재확인이 필요함
- 상태: Fixed
- 다음 조치: Fight Screen에서 승리 Alert 확인 후 savedStage 증가 검사
- 관련 파일: BattleResultAlertView.cs

---

### GS-009. 자동 검사 - PlayerPrefs 접근 위치 회귀

- 우선순위: 높음
- 확인 내용: 저장 기능이 PlayerPrefs에 직접 접근하지 않고 저장 계층을 통해 접근하는지 확인
- 기대 결과: PlayerPrefs 직접 접근 위치가 다시 늘어나지 않는다
- 현재 결과: GameSystemStaticRegressionTests 추가됨
- 상태: Pass
- 다음 조치: EditMode 테스트 재실행 시 함께 확인
- 관련 파일: GameSystemStaticRegressionTests.cs

---

### GS-010~GS-016. 스테이지 잠금 - savedStage별 선택 가능 범위

- 우선순위: 높음
- 확인 내용: 저장된 savedStage 값에 따라 Stage Select 버튼의 interactable 범위가 맞는지 확인
- 기대 결과:
  - savedStage=1: 1스테이지만 선택 가능, 2~7스테이지 선택 불가
  - savedStage=2: 1~2스테이지 선택 가능, 3~7스테이지 선택 불가
  - savedStage=3: 1~3스테이지 선택 가능, 4~7스테이지 선택 불가
  - savedStage=4: 1~4스테이지 선택 가능, 5~7스테이지 선택 불가
  - savedStage=5: 1~5스테이지 선택 가능, 6~7스테이지 선택 불가
  - savedStage=6: 1~6스테이지 선택 가능, 7스테이지 선택 불가
  - savedStage=7: 1~7스테이지 선택 가능
- 현재 결과: StageSelectView.ButtonInteractivity 구조상 savedStage 개수만큼 버튼을 활성화하는 흐름은 확인됨. 실제 씬 버튼 배열 순서 검증 필요
- 상태: Pending
- 다음 조치: Stage Select Screen에서 savedStage별 버튼 활성 상태 확인
- 관련 파일: StageSelectView.cs, StageController.cs

---

### GS-017~GS-023. 이어하기 - 스테이지 클리어 후 재시작 진행 상태

- 우선순위: 높음
- 확인 내용: 각 스테이지 클리어 후 게임을 껐다 켰을 때 이어하기 기준 진행도가 다음 스테이지로 유지되는지 확인
- 기대 결과:
  - 1스테이지 클리어 후 재시작: 이어하기 시 2스테이지 진행 가능
  - 2스테이지 클리어 후 재시작: 이어하기 시 3스테이지 진행 가능
  - 3스테이지 클리어 후 재시작: 이어하기 시 4스테이지 진행 가능
  - 4스테이지 클리어 후 재시작: 이어하기 시 5스테이지 진행 가능
  - 5스테이지 클리어 후 재시작: 이어하기 시 6스테이지 진행 가능
  - 6스테이지 클리어 후 재시작: 이어하기 시 7스테이지 진행 가능
  - 7스테이지 클리어 후 재시작: Epilogue 또는 완료 상태 흐름 확인
- 현재 결과: GameSaveController.SaveClearedStage가 savedStage와 playingStage를 nextStage로 저장하는 구조는 확인됨. 실제 재시작 QA 필요
- 상태: Pending
- 다음 조치: 각 스테이지 클리어 직후 PlayerPrefs 저장값과 재시작 후 Stage Select 상태 확인
- 관련 파일: GameSaveController.cs, PlayerPrefsSaveStore.cs, BattleResultAlertView.cs, StageFlowController.cs

---

### GS-024. 진행도 보호 - 이전 스테이지 재클리어

- 우선순위: 높음
- 확인 내용: 5스테이지까지 열린 상태에서 2스테이지를 다시 클리어해도 savedStage가 3으로 내려가지 않는지 확인
- 기대 결과: savedStage는 기존 최고 진행도보다 낮아지지 않는다
- 현재 결과: SaveClearedStage에서 Mathf.Max(currentSaveData.savedStage, nextStage)를 사용함
- 상태: Pending
- 다음 조치: 높은 진행도 저장 후 낮은 스테이지 재클리어 시 savedStage 유지 확인
- 관련 파일: GameSaveController.cs

---

### GS-025. 씬 흐름 - 비파괴 컨트롤러 유지

- 우선순위: 높음
- 확인 내용: Start Screen에서 생성된 GameSaveController와 StageFlowController가 Stage Select, Story, Fight 씬 이동 후 유지되는지 확인
- 기대 결과: 씬 이동 후에도 저장/스테이지 흐름 컨트롤러 인스턴스가 유지되고 중복 생성되지 않는다
- 현재 결과: 두 컨트롤러 모두 DontDestroyOnLoad와 중복 제거 구조가 있음
- 상태: Pending
- 다음 조치: Start Screen부터 실제 흐름으로 진입해 씬 이동마다 인스턴스 유지 확인
- 관련 파일: GameSaveController.cs, StageFlowController.cs

---

### GS-026. 자동 검사 - 스테이지 진행 테스트 추가

- 우선순위: 높음
- 확인 내용: 스테이지별 저장, Stage Select 잠금, 전투 클리어 후 다음 씬 이동을 자동 테스트로 검증할 수 있는지 확인
- 기대 결과: PlayMode 테스트가 savedStage/playingStage와 StageFlowController 흐름을 검증한다
- 현재 결과: GameSaveProgressReadTests와 StageRuntimeFlowPlayModeTests에 테스트 추가, 스크립트 재컴파일 통과. MCP PlayMode run_tests는 501 응답으로 실행 차단
- 상태: Fixed
- 다음 조치: MCP 연결 복구 후 PlayMode 테스트 재실행
- 관련 파일: GameSaveProgressReadTests.cs, StageRuntimeFlowPlayModeTests.cs

---

### GS-027~GS-029. 저장값 방어 - 비정상 진행 데이터

- 우선순위: 높음
- 확인 내용: PlayerPrefs에 비정상 진행 값이 들어 있어도 이어하기/스테이지 선택이 잘못 열리지 않는지 확인
- 기대 결과:
  - savedStage <= 0: 저장 없음으로 판단하고 이어하기 버튼이 숨겨진다
  - savedStage >= 8: Stage Select UI는 최대 7스테이지까지만 표시하거나 선택 가능하다
  - playingStage > savedStage: 잠긴 스테이지로 직접 진행하지 않는다
- 현재 결과: HasGameSave는 savedStage >= 1만 확인함. 상한값과 playingStage 불일치 방어는 실제 QA 필요
- 상태: Pending
- 다음 조치: PlayerPrefs 값을 직접 조작한 뒤 Start Screen과 Stage Select 동작 확인
- 관련 파일: PlayerPrefsSaveStore.cs, StageSelectView.cs, StageTextView.cs

---

### GS-030~GS-031. Alert 취소 흐름 - 새 게임/이어하기

- 우선순위: 높음
- 확인 내용: 새 게임 또는 이어하기 Alert에서 No를 눌렀을 때 저장값과 현재 씬이 유지되는지 확인
- 기대 결과:
  - 새 게임 No: 기존 savedStage/playingStage가 바뀌지 않고 Start Screen에 남는다
  - 이어하기 No: Stage Select로 이동하지 않고 Start Screen에 남는다
- 현재 결과: StartScreenView에서 No일 때 Alert만 비활성화하는 구조 확인. 런타임 저장값 검증 필요
- 상태: Pending
- 다음 조치: Alert No 클릭 전후 PlayerPrefs와 현재 씬 확인
- 관련 파일: StartScreenView.cs, ConfirmAlertView.cs

---

### GS-032. 중복 입력 - Alert/씬 이동 중복 실행

- 우선순위: 중간
- 확인 내용: 새 게임/이어하기 버튼을 빠르게 여러 번 눌렀을 때 Coroutine이나 씬 이동이 중복 실행되지 않는지 확인
- 기대 결과: Alert는 하나만 처리되고, 씬 이동은 한 번만 발생한다
- 현재 결과: 중복 클릭 방어 상태 미확인
- 상태: Pending
- 다음 조치: 버튼 연타 후 콘솔 로그, Alert 상태, 씬 이동 횟수 확인
- 관련 파일: StartScreenView.cs

---

### GS-033. 직접 진입 - Stage Select 단독 실행

- 우선순위: 중간
- 확인 내용: Start Screen을 거치지 않고 Stage Select Screen을 직접 Play해도 저장/스테이지 선택 흐름이 깨지지 않는지 확인
- 기대 결과: 저장값 fallback을 사용하거나 명확한 에러 없이 테스트 가능한 상태로 동작한다
- 현재 결과: StageController는 GameSaveController.GetGameSaveOrDefault를 사용하지만 StageSelectView.stageLoader는 GameSaveController.instance가 없으면 진행하지 않음
- 상태: Pending
- 다음 조치: Stage Select Screen 단독 Play 후 버튼 표시와 클릭 동작 확인
- 관련 파일: StageController.cs, StageSelectView.cs, GameSaveController.cs

---

### GS-034. 빌드 세팅 - 필수 씬 등록

- 우선순위: 높음
- 확인 내용: 런타임에 LoadScene으로 호출되는 모든 씬이 Build Settings에 등록되어 있는지 확인
- 기대 결과: Start Screen, Stage Select Screen, Story Screen_1/2/3/5/7Stage, Fight Screen_1~7Stage, Prologue Screen, Epilogue Screen, Thank Screen, HUD가 등록되어 있다
- 현재 결과: 정적 확인 필요
- 상태: Pending
- 다음 조치: ProjectSettings/EditorBuildSettings.asset과 실제 씬 파일 목록 비교
- 관련 파일: EditorBuildSettings.asset

## 4. 발견한 버그

| ID | 날짜 | 내용 | 상태 |
|---|---|---|---|
| BUG-GS-001 | 2026-05-31 | MCP PlayMode timeout으로 이어하기 버튼 표시와 로드 검증 불가 | Open |
| BUG-GS-002 | 2026-05-31 | MCP PlayMode run_tests가 501 응답으로 자동 테스트 실행 불가 | Open |
| BUG-GS-003 | 2026-05-31 | savedStage 상한값과 playingStage 불일치 저장값 방어 여부 미확인 | Open |

## 5. QA 히스토리

| 날짜 | 확인 범위 | 결과 | 메모 |
|---|---|---|---|
| 2026-05-31 | 이어하기 저장 조건, 저장 시스템 연결 | 조건부 통과 | PlayMode 실행은 timeout으로 일부 미확인 |
| 2026-05-31 | 스테이지별 진행 QA 항목 확장 | 대기 | 1~7스테이지 잠금/해금, 재시작 후 이어하기 기준 추가 |
| 2026-05-31 | 스테이지 진행 자동 테스트 추가 | 조건부 통과 | 컴파일 통과, PlayMode 실행은 MCP 501로 차단 |
| 2026-05-31 | 시스템 예외 QA 항목 확장 | 대기 | 비정상 저장값, Alert 취소, 중복 입력, 직접 진입, 빌드 세팅 기준 추가 |
