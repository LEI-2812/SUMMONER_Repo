# OptionSystem Roadmap

## 1. Role

OptionSystem은 설정창 UI, 오디오, 비디오, 게임플레이 옵션, 옵션 저장 유지 흐름을 담당한다.
스토리 입력에 옵션이 실제 반영되는지는 `02_StorySystemRoadmap.md`와 함께 확인하고, 새 게임/이어하기에서 옵션이 유지되는지는 `01_GameSystemRoadmap.md`와 함께 확인한다.

현재 QA 상태나 버그 우선순위는 이 문서에 두지 않고 `qa/00_current.md`에서 관리한다.

## 2. Ownership

| 영역 | 주요 파일/폴더 | 책임 |
|---|---|---|
| 설정 패널 | `Assets/Script/Option/SettingPanelView.cs` | 설정창 열기/닫기, 탭 전환 |
| 오디오 | `Assets/Script/Option/AudioSettingView.cs` | Master/BGM/SFX 값 적용과 저장 |
| 비디오 | `Assets/Script/Option/VideoSettingView.cs` | 해상도, 화면 모드 적용과 저장 |
| 게임플레이 | `Assets/Script/Option/GameplaySettingView.cs` | Story Skip, Only Mouse 값 저장 |
| 저장 연동 | `Assets/Script/Save` | 옵션 PlayerPrefs 값 유지와 진행도 저장 분리 |

## 3. Current Structure

- 설정 UI는 오디오, 비디오, 게임플레이 탭으로 나뉜다.
- 오디오 옵션은 UI 값을 AudioMixer 값으로 변환해 적용한다.
- 비디오 옵션은 저장된 인덱스를 토글 상태와 실제 화면 설정으로 반영한다.
- 게임플레이 옵션은 Story Skip, Only Mouse 값을 저장하고 StorySystem이 실제 입력/스킵 정책에 반영한다.
- 옵션 저장은 진행도 저장과 분리되어 새 게임에서도 유지되어야 한다.

## 4. Refactoring Direction

- 옵션 View는 표시와 사용자 입력 전달만 맡고, 저장값 검증과 적용 정책을 명확히 분리한다.
- 오디오 볼륨 값은 dB 변환 전에 최소값과 음소거 정책을 고정한다.
- 비디오 저장 인덱스는 UI 적용 전에 범위를 검증하고 fallback을 제공한다.
- 게임플레이 옵션은 저장과 실제 사용 위치를 구분한다.

## 5. Constraints

- 볼륨 0에서도 `Mathf.Log10(0)` 같은 에러가 없어야 한다.
- 잘못된 비디오 저장 인덱스가 있어도 토글 중 하나는 항상 유효하게 선택되어야 한다.
- 새 게임은 옵션 값을 지우지 않는다.
- Story Skip과 Only Mouse는 저장만으로 끝나지 않고 StorySystem 실제 흐름에 반영되어야 한다.
- 설정창을 반복해서 열고 닫아도 Inspector 참조 누락이 없어야 한다.

## 6. Next Work Candidates

| ID | 작업 | 목적 |
|---|---|---|
| OP-REF-01 | 오디오 볼륨 변환 정책 정리 | 볼륨 0과 음소거 처리를 안전하게 고정 |
| OP-REF-02 | 비디오 저장 인덱스 fallback 정리 | 범위 밖 저장값에도 UI와 실제 설정을 정상 복구 |
| OP-REF-03 | 게임플레이 옵션 사용처 명시화 | Story Skip, Only Mouse 저장과 실제 반영 위치 연결 |
| OP-REF-04 | 설정 View 변수명과 책임 정리 | UI 표시, 저장, 적용 책임 구분 |

## 7. Done Criteria

- 설정창을 반복해서 열고 닫아도 참조 누락과 표시 깨짐이 없다.
- 오디오, 비디오, 게임플레이 옵션이 저장되고 재시작 후 유지된다.
- 새 게임은 진행 데이터만 초기화하고 옵션 값을 지우지 않는다.
- 이어하기와 씬 이동 후에도 옵션 적용 상태가 유지된다.
- Story Skip과 Only Mouse가 실제 Story 입력/진입 흐름에 반영된다.
- 잘못된 저장값이 있어도 기본값으로 복구된다.

## 8. QA Links

- 현재 QA 큐: `qa/00_current.md`
- 큰 변경 후 전체 확인: `qa/20_full-check.md`
- 기능별 QA 종합보고서: `QAReports/05_OptionSystem_QA.md`, `QAReports/05_OptionSystem_QA.csv`
