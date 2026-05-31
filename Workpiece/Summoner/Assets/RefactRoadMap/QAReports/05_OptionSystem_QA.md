# OptionSystem QA

## 1. 현재 상태 요약

| 항목 | 내용 |
|---|---|
| QA 범위 | 설정창, 오디오, 비디오, 게임플레이 옵션, 옵션 저장 유지 |
| 현재 상태 | QA 항목 정리 완료, 실제 확인 필요 |
| 마지막 정리일 | 2026-05-31 |
| 다음 확인 | 옵션 변경 후 씬 이동/재시작 유지 확인 |

## 2. QA 체크 항목

| ID | 기능 | 확인 내용 | 상태 |
|---|---|---|---|
| OP-001 | 설정창 | Start Screen에서 설정창이 열리고 닫히는가 | Pending |
| OP-002 | 설정창 | 비디오/오디오/게임플레이 탭 전환이 정상 동작하는가 | Pending |
| OP-003 | 오디오 | Master Volume 변경값이 저장되고 재시작 후 유지되는가 | Pending |
| OP-004 | 오디오 | BGM Volume 변경값이 저장되고 재시작 후 유지되는가 | Pending |
| OP-005 | 오디오 | SFX Volume 변경값이 저장되고 재시작 후 유지되는가 | Pending |
| OP-006 | 오디오 | 볼륨 0 설정 시 에러 없이 음소거되는가 | Pending |
| OP-007 | 비디오 | 해상도 선택값이 저장되고 재시작 후 유지되는가 | Pending |
| OP-008 | 비디오 | 화면 모드 선택값이 저장되고 재시작 후 유지되는가 | Pending |
| OP-009 | 게임플레이 | 스토리 스킵 설정이 저장되고 재시작 후 유지되는가 | Pending |
| OP-010 | 게임플레이 | 마우스 전용 조작 설정이 저장되고 재시작 후 유지되는가 | Pending |
| OP-011 | 새 게임 | 새 게임 시작 시 옵션 설정값은 초기화되지 않는가 | Pending |
| OP-012 | 이어하기 | 이어하기 흐름에서도 기존 옵션 설정값이 유지되는가 | Pending |
| OP-013 | 자동 검사 | 새 게임/진행 초기화 시 옵션 유지 테스트가 추가되었는가 | Fixed |
| OP-014 | 설정창 | 옵션창을 닫았다 다시 열어도 현재 탭/값 표시가 깨지지 않는가 | Pending |
| OP-015 | 오디오 | Master/BGM/SFX 조합 볼륨이 실제 AudioMixer에 반영되는가 | Pending |
| OP-016 | 비디오 | 같은 토글을 다시 눌러도 모든 토글이 꺼지는 상태가 되지 않는가 | Pending |
| OP-017 | 비디오 | 잘못된 저장 인덱스가 있어도 기본 해상도/화면 모드로 복구되는가 | Pending |
| OP-018 | 게임플레이 | Story Skip 옵션이 실제 스토리 진입/스킵 동작에 반영되는가 | Pending |
| OP-019 | 게임플레이 | Only Mouse 옵션이 실제 Story 입력 처리에 반영되는가 | Pending |
| OP-020 | 직접 진입 | 옵션 UI가 없는 씬에서 설정 관련 싱글톤 누락으로 에러가 발생하지 않는가 | Pending |

## 3. 상세 QA 기록

### OP-001. 설정창 - 열기/닫기

- 우선순위: 높음
- 확인 내용: Start Screen의 설정 버튼을 누르면 SettingPanel이 열리고 다시 누르면 닫히는지 확인
- 기대 결과: 설정창 열기/닫기 중 NullReference가 발생하지 않는다
- 현재 결과: StartScreenView.openOption에서 MenuCanvas/SettingPanel을 찾아 토글하는 구조 확인
- 상태: Pending
- 다음 조치: Start Screen에서 설정 버튼 반복 클릭 확인
- 관련 파일: StartScreenView.cs, SettingPanelView.cs

---

### OP-002. 설정창 - 탭 전환

- 우선순위: 중간
- 확인 내용: 비디오, 오디오, 게임플레이 탭 클릭 시 해당 패널만 활성화되는지 확인
- 기대 결과: 한 번에 하나의 설정 패널만 보이고 버튼 강조 상태가 갱신된다
- 현재 결과: SettingPanelView.ShowPanel과 UpdateButtonColors 구조 확인
- 상태: Pending
- 다음 조치: 각 탭 클릭 후 panels 활성 상태 확인
- 관련 파일: SettingPanelView.cs

---

### OP-003~OP-005. 오디오 - 볼륨 저장 유지

- 우선순위: 높음
- 확인 내용: Master/BGM/SFX 슬라이더 값이 PlayerPrefs에 저장되고 게임 재시작 후 복원되는지 확인
- 기대 결과: 변경한 볼륨 값이 씬 이동과 재시작 후에도 유지된다
- 현재 결과: AudioSettingView가 MasterVolume, BGMVolume, SFXVolume 키로 저장/로드하는 구조 확인
- 상태: Pending
- 다음 조치: 각 슬라이더 값을 변경한 뒤 Start Screen 재진입과 재시작 후 값 확인
- 관련 파일: AudioSettingView.cs

---

### OP-006. 오디오 - 볼륨 0 처리

- 우선순위: 높음
- 확인 내용: 볼륨을 0으로 설정했을 때 AudioMixer에 잘못된 값이 들어가거나 에러가 발생하지 않는지 확인
- 기대 결과: 볼륨 0은 음소거로 처리되고 콘솔 에러가 없어야 한다
- 현재 결과: Mathf.Log10(adjustedVolume) 구조상 adjustedVolume이 0이면 음수 무한대가 될 수 있어 확인 필요
- 상태: Pending
- 다음 조치: Master/BGM/SFX를 각각 0으로 낮춘 뒤 콘솔 에러와 실제 음소거 확인
- 관련 파일: AudioSettingView.cs

---

### OP-007~OP-008. 비디오 - 해상도/화면 모드 저장 유지

- 우선순위: 중간
- 확인 내용: 해상도와 화면 모드 토글 선택값이 저장되고 재시작 후 유지되는지 확인
- 기대 결과: 선택한 해상도와 화면 모드가 PlayerPrefs에 저장되고 다시 열었을 때 같은 토글이 켜진다
- 현재 결과: VideoSettingView가 resolutionIndex, screenModeIndex 키로 저장/로드하는 구조 확인
- 상태: Pending
- 다음 조치: 각 토글 선택 후 재시작하여 토글 상태와 실제 Screen 설정 확인
- 관련 파일: VideoSettingView.cs

---

### OP-009~OP-010. 게임플레이 - Story Skip / Only Mouse

- 우선순위: 높음
- 확인 내용: 스토리 스킵과 마우스 전용 조작 설정이 저장되고 재시작 후 유지되는지 확인
- 기대 결과: IsStorySkip, IsOnlyMouse 값이 PlayerPrefs에 저장되고 설정창을 다시 열 때 같은 상태로 표시된다
- 현재 결과: GameplaySettingView가 저장/로드 구조를 갖고 있으나 실제 Story/Input 흐름과 연결 여부는 추가 확인 필요
- 상태: Pending
- 다음 조치: 옵션 값을 바꾼 뒤 Story Scene과 입력 처리에서 실제 반영 여부 확인
- 관련 파일: GameplaySettingView.cs, StoryScenarioControllerBase.cs

---

### OP-011. 새 게임 - 옵션 유지

- 우선순위: 높음
- 확인 내용: 새 게임 시작 시 진행 데이터만 초기화되고 옵션 설정은 유지되는지 확인
- 기대 결과: savedStage/playingStage는 1로 초기화되지만 볼륨/해상도/게임플레이 옵션은 유지된다
- 현재 결과: GameSaveController.StartNewGame은 진행 데이터만 저장하는 구조 확인
- 상태: Pending
- 다음 조치: 옵션 변경 후 새 게임 시작, Start Screen 재진입 후 옵션 값 유지 확인
- 관련 파일: GameSaveController.cs, PlayerPrefsSaveStore.cs

---

### OP-012. 이어하기 - 옵션 유지

- 우선순위: 중간
- 확인 내용: 이어하기로 Stage Select/Story/Fight에 진입해도 옵션 설정이 유지되는지 확인
- 기대 결과: 저장된 옵션 값이 씬 이동 후에도 바뀌지 않는다
- 현재 결과: 옵션 값은 진행 저장 데이터와 별도 PlayerPrefs 키를 사용함
- 상태: Pending
- 다음 조치: 옵션 변경 후 이어하기, 씬 이동 후 설정창 또는 실제 적용 상태 확인
- 관련 파일: StartScreenView.cs, AudioSettingView.cs, VideoSettingView.cs, GameplaySettingView.cs

---

### OP-013. 자동 검사 - 옵션 유지 테스트 추가

- 우선순위: 높음
- 확인 내용: 새 게임 시작과 진행 데이터 초기화가 옵션 PlayerPrefs 값을 삭제하지 않는지 자동 테스트로 검증하는가
- 기대 결과: StartNewGame과 ResetGameProgress 이후에도 MasterVolume, BGMVolume, SFXVolume, resolutionIndex, screenModeIndex, IsStorySkip, IsOnlyMouse 값이 유지된다
- 현재 결과: GameSaveProgressReadTests에 옵션 유지 테스트 추가, 스크립트 재컴파일 통과. 2026-05-31 PlayMode run_tests는 timeout으로 실행 차단
- 상태: Fixed
- 다음 조치: MCP 연결 복구 후 PlayMode 테스트 재실행
- 관련 파일: GameSaveProgressReadTests.cs, GameSaveController.cs, PlayerPrefsSaveStore.cs

---

### OP-014. 설정창 - 닫기/다시 열기 표시 유지

- 우선순위: 중간
- 확인 내용: 옵션창을 닫았다 다시 열었을 때 슬라이더/토글/탭 상태가 깨지지 않는지 확인
- 기대 결과: 저장된 값 또는 현재 선택값이 UI에 그대로 표시된다
- 현재 결과: 실제 UI 재오픈 QA 필요
- 상태: Pending
- 다음 조치: 설정값 변경 후 옵션창 닫기/열기 반복 확인
- 관련 파일: SettingPanelView.cs, AudioSettingView.cs, VideoSettingView.cs, GameplaySettingView.cs

---

### OP-015. 오디오 - 조합 볼륨 AudioMixer 반영

- 우선순위: 높음
- 확인 내용: Master Volume과 BGM/SFX Volume 조합이 AudioMixer에 실제 반영되는지 확인
- 기대 결과: BGM 출력은 master * bgm, SFX 출력은 master * sfx 기준으로 바뀐다
- 현재 결과: ApplyVolumes에서 곱셈 후 dB 변환 구조 확인. 실제 mixer 값 QA 필요
- 상태: Pending
- 다음 조치: Master/BGM/SFX 값을 조합해 변경하고 AudioMixer 파라미터 확인
- 관련 파일: AudioSettingView.cs, Audio.mixer

---

### OP-016~OP-017. 비디오 - 토글 안정성/잘못된 저장 인덱스

- 우선순위: 중간
- 확인 내용: 토글 그룹이 항상 하나의 선택값을 유지하고, 저장된 인덱스가 범위를 벗어나도 복구되는지 확인
- 기대 결과:
  - 같은 토글을 다시 눌러도 모든 해상도/화면 모드 토글이 꺼지지 않는다
  - resolutionIndex 또는 screenModeIndex가 범위를 벗어나면 기본값으로 복구된다
- 현재 결과: 저장 인덱스를 그대로 리스트 접근에 사용하므로 범위 초과 저장값 방어 확인 필요
- 상태: Pending
- 다음 조치: PlayerPrefs에 -1, 99를 저장한 뒤 옵션창 진입 확인
- 관련 파일: VideoSettingView.cs

---

### OP-018~OP-019. 게임플레이 - 실제 흐름 반영

- 우선순위: 높음
- 확인 내용: Story Skip과 Only Mouse 설정값이 저장만 되는 것이 아니라 실제 스토리/입력 흐름에 반영되는지 확인
- 기대 결과:
  - Story Skip ON: 스토리 진입 또는 Skip 동작이 설정대로 처리된다
  - Only Mouse ON: 키보드 입력은 무시되고 마우스 클릭만 유효하다
- 현재 결과: 저장 구조는 있으나 실제 Story/Input 연결 여부 미확인
- 상태: Pending
- 다음 조치: 옵션값 변경 후 Story Screen에서 진입/입력 동작 확인
- 관련 파일: GameplaySettingView.cs, StoryScenarioControllerBase.cs, StorySkipView.cs

---

### OP-020. 직접 진입 - 옵션 UI 없는 씬 안정성

- 우선순위: 중간
- 확인 내용: Fight/Story 씬처럼 옵션 UI가 없거나 MenuCanvas가 없는 상태에서 설정 관련 참조 누락 에러가 발생하지 않는지 확인
- 기대 결과: 옵션 UI가 없는 씬에서는 설정창 접근이 없거나 명확히 무시된다
- 현재 결과: StartScreenView는 MenuCanvas를 직접 찾음. 다른 씬 직접 진입 시 옵션 참조 영향 확인 필요
- 상태: Pending
- 다음 조치: Story/Fight 씬 단독 Play 후 옵션 관련 콘솔 에러 확인
- 관련 파일: StartScreenView.cs, SettingPanelView.cs

## 4. 발견한 버그

| ID | 날짜 | 내용 | 상태 |
|---|---|---|---|
| BUG-OP-001 | 2026-05-31 | `AudioSettingView`에서 볼륨 0일 때 `Mathf.Log10(0)` 호출 가능 | Open Fail |
| BUG-OP-002 | 2026-05-31 | Story Skip은 `StorySkipView`에 정적 연결 확인, Only Mouse 옵션은 Story 입력 흐름에 미반영 | Open Fail |
| BUG-OP-003 | 2026-05-31 | MCP PlayMode `GameSaveProgressReadTests`가 timeout으로 옵션 유지 자동 테스트 실행 불가 | Open Blocked |
| BUG-OP-004 | 2026-05-31 | `VideoSettingView`가 잘못된 저장 인덱스를 기본값으로 복구하지 않아 모든 토글이 꺼질 수 있음 | Open Fail |

## 5. QA 히스토리

| 날짜 | 확인 범위 | 결과 | 메모 |
|---|---|---|---|
| 2026-05-31 | OptionSystem QA 항목 정리 | 대기 | 설정 저장, 재시작 유지, 새 게임/이어하기 유지 기준 추가 |
| 2026-05-31 | 옵션 유지 자동 테스트 추가 | 조건부 통과 | 컴파일 통과, 2026-05-31 PlayMode 실행은 timeout으로 차단 |
| 2026-05-31 | OptionSystem 예외 QA 항목 확장 | 대기 | UI 재오픈, 조합 볼륨, 토글 안정성, 저장 인덱스, 실제 흐름 반영 기준 추가 |
| 2026-05-31 | QA 인덱스 재실행 | 부분 실패 | `recompile_scripts` warning 0, PlayMode `GameSaveProgressReadTests`는 timeout; 볼륨 0, 잘못된 비디오 인덱스, Only Mouse 정적 실패 확인 |
