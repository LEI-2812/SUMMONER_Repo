# Scene Object Organization Reference

이 문서는 활성 기능 로드맵이 아니다.
세션 시작 시 확인할 인계 문서는 `00_SessionHandoff.md`이고, 초기 리팩토링 정보는 기능별 5개 로드맵만 사용한다.

씬 계층, 루트 오브젝트, 네이밍, Inspector 연결 정리 기준이 필요할 때만 이 문서를 참고한다.

---

## 1. 목적

씬 Hierarchy의 가독성을 높이고, Inspector에서 필요한 오브젝트를 빠르게 찾을 수 있게 정리한다.

이 문서는 씬 오브젝트를 어떤 기준으로 묶고 이름 붙일지 정하는 기준 문서다.
코드 구조를 바꾸기 전에 씬 구조부터 안정적으로 정리하는 것을 목표로 한다.


## 2. 기본 원칙

| 원칙 | 내용 |
|---|---|
| 역할 기준 그룹화 | 오브젝트는 위치보다 역할 기준으로 묶는다 |
| 루트 최소화 | 씬 루트에는 큰 그룹만 남긴다 |
| 이름으로 역할 파악 | 이름만 보고 화면 요소인지, 시스템인지, 오디오인지 알 수 있게 한다 |
| Inspector 연결 유지 | 정리 중 컴포넌트 참조와 UnityEvent 연결을 끊지 않는다 |
| 한 번에 한 씬 | 먼저 대표 씬 하나를 정리하고 확인 후 같은 패턴을 반복 적용한다 |

## 3. 루트 오브젝트 기준

씬 루트는 아래 그룹을 기준으로 정리한다.

```text
__Systems
__Camera
__Audio
__UI
__Handlers
```

| 그룹 | 포함 대상 |
|---|---|
| `__Systems` | `GameSaveController`, `StageFlowController`, `StageController`, `EventSystem` |
| `__Camera` | `Main Camera` |
| `__Audio` | 씬 공통 클릭음, 알림음, BGM AudioSource |
| `__UI` | Canvas, 화면에 보이는 UI 그룹 |
| `__Handlers` | 메뉴/알림/설정 등 입력 처리용 MonoBehaviour 오브젝트 |

루트에 직접 둘 수 있는 예외는 임시 검증 중인 오브젝트뿐이다.
최종 정리 후에는 예외 오브젝트도 위 그룹 중 하나로 이동한다.

## 4. 공통 네이밍 기준

| 대상 | 권장 이름 |
|---|---|
| 배경 UI | `UI_Background` |
| 버튼 묶음 | `UI_Buttons` |
| 알림창 묶음 | `UI_Alerts` |
| 설정 UI | `UI_Settings` |
| 메뉴 UI | `UI_Menu` |
| 캐릭터 표시 영역 | `UI_Characters` |
| 전투 유닛 표시 영역 | `UI_BattleUnits` |
| 턴 정보 | `UI_Turn` |
| 마나 정보 | `UI_Mana` |
| 상태 패널 | `UI_State` |
| 소환 선택 패널 | `UI_SummonPick` |
| 전투 결과 알림 | `UI_ResultAlert` |

버튼 단일 오브젝트는 가능하면 동작 기준으로 이름 붙인다.

```text
NewGameButton
ContinueButton
SettingButton
EndTurnButton
ReSummonButton
StorySkipButton
```

## 5. 씬별 정리 기준

### 5.1 Start Screen

```text
__Systems
  GameSaveController
  StageFlowController
  EventSystem

__Camera
  Main Camera

__UI
  MainCanvas
    UI_Background
    UI_Title
    UI_Buttons
    UI_Alerts

__Audio
  ButtonClickAudio
```

정리 대상:

| 현재 이름 | 변경/이동 기준 |
|---|---|
| `MainCanvas/Background` | `MainCanvas/UI_Background` 아래 배치 |
| `MainCanvas/Title Image` | `MainCanvas/UI_Title` 아래 배치 |
| `MainCanvas/ButtonObjects` | `MainCanvas/UI_Buttons`로 이름 정리 |
| `Alert_firstplay` | `MainCanvas/UI_Alerts/NewGameAlert` |
| `Alert_saveplay` | `MainCanvas/UI_Alerts/ContinueAlert` |

### 5.2 Stage Select Screen

```text
__Systems
  GameSaveController
  StageController
  EventSystem

__Camera
  Main Camera

__UI
  StageCanvas
    UI_Background
    UI_Map
    UI_StageButtons
    UI_Description
    UI_Alerts
```

정리 대상:

| 현재 이름 | 변경/이동 기준 |
|---|---|
| `StageCanvas/Background Image` | `UI_Background` |
| `StageCanvas/Mapline Image` | `UI_Map` |
| `StageCanvas/Line Image1`, `Line Image2` | `UI_Map` |
| `StageCanvas/Button` | `UI_StageButtons` |
| `StageCanvas/Explain` | `UI_Description` |
| `Alert_nosave_quit` | `UI_Alerts/QuitAlert` |
| `Alert_nosave_toMain` | `UI_Alerts/ToMainAlert` |

### 5.3 Story Screens

스토리 씬은 `Story Screen_1Stage`, `2Stage`, `3Stage`, `5Stage`, `7Stage`에 같은 기준을 적용한다.

```text
__Systems
  EventSystem

__Camera
  Main Camera

__UI
  Canvas
    UI_Background
    UI_Characters
    UI_Dialogue
    UI_Effects
    UI_Fade
    UI_StoryControls
```

정리 대상:

| 현재 이름 | 변경/이동 기준 |
|---|---|
| `Background` | `UI_Background` |
| `Character_Player` | `UI_Characters` |
| `Dialog Box` | `UI_Dialogue` |
| `Confusebubble Image`, `Dotbubble Image` | `UI_Effects` |
| `FadePanel` | `UI_Fade` |
| `StoryAndTalkManager` | `UI_StoryControls` 또는 `__Systems` |

`StoryAndTalkManager`가 화면 표시보다 진행 제어를 담당하면 `__Systems`로 이동한다.

### 5.4 Fight Screens

전투 씬은 `Fight Screen_1Stage`를 먼저 정리하고, 같은 패턴을 `2Stage`부터 `7Stage`까지 반복 적용한다.

```text
__Systems
  GameSaveController
  EventSystem

__Camera
  Main Camera

__UI
  BattleCanvas
    UI_00_Background
    UI_10_BattleField
    UI_20_TurnStatus
    UI_30_PlayerCommands
    UI_40_Mana
    UI_50_SelectedUnitState
    UI_60_SummonPicker
    UI_90_ResultAlerts
```

정리 대상:

| 현재 이름 | 변경/이동 기준 |
|---|---|
| `BattleCanvas/Background` | `UI_00_Background` |
| `BattleCanvas/Player_Enermy` | `UI_10_BattleField` |
| `BattleCanvas/TurnTextUI`, `CurrentTurnText`, `ClearTurnText` | `UI_20_TurnStatus` |
| `BattleCanvas/SummonBtn`, `RedoBtn`, `TurnEndBtn` | `UI_30_PlayerCommands` |
| `BattleCanvas/MPcount Image` | `UI_40_Mana` |
| `BattleCanvas/StatePanel` | `UI_50_SelectedUnitState` |
| `TakeSummon(항상활성화)` | `BattleCanvas/UI_60_SummonPicker` |
| `Alert_clear`, `Alert_fail` | `BattleCanvas/UI_90_ResultAlerts` |

### 5.5 HUD

HUD 씬은 루트 구분선 오브젝트를 제거하고 실제 그룹명으로 대체한다.

```text
__Audio
  MenuClickAudio
  AlertClickAudio

__Handlers
  MenuHandler
  SettingHandler
  ToMainAlertHandler
  ToQuitAlertHandler
  SkipAlertHandler

__UI
  MenuCanvas
    UI_00_Background
      MenuBackgroundPanel
    UI_10_Menu
      MenuPanel
    UI_20_Settings
      Setting
        SettingPanel
    UI_90_Alerts
      AlertObject
      알림창
        Alert_clear
        Alert_fail
        Alert_Skip
```

정리 대상:

| 현재 이름 | 변경/이동 기준 |
|---|---|
| `-------메뉴관련 오브젝트---------` | 제거 또는 `__Handlers`로 대체 |
| `-------스토리관련 오브젝트--------` | 제거 또는 `__Handlers`로 대체 |
| `-------알림관련 오브젝트--------` | 제거 또는 `__UI/UI_Alerts`로 대체 |
| `-------오디오관련 오브젝트-------` | 제거 또는 `__Audio`로 대체 |
| `알림창` | `MenuCanvas/UI_90_Alerts` |
| `HandlerObjects` | `__Handlers` |
| `AlterClickAudio` | `__Audio/AlertClickAudio` |
| `SkipAlterHandler` | `__Handlers/SkipAlertHandler` |

## 6. 적용 순서

1. `HUD.unity` 정리
2. `Fight Screen_1Stage.unity` 정리
3. `Fight Screen_1Stage` 기준으로 테스트
4. `Fight Screen_2Stage` ~ `Fight Screen_7Stage` 반복 적용
5. `Start Screen.unity` 정리
6. `Stage Select Screen.unity` 정리
7. `Story Screen_1/2/3/5/7Stage.unity` 정리

## 7. 정리 후 확인 기준

씬 정리 후 아래 항목을 확인한다.

| 확인 항목 | 기준 |
|---|---|
| Missing Script | 씬/프리팹에 Missing Script가 없어야 한다 |
| Missing Reference | Inspector에 `None`으로 바뀐 필수 참조가 없어야 한다 |
| UI 위치 | 정리 전후 화면 위치가 달라지면 안 된다 |
| Button Event | Button OnClick 연결이 유지되어야 한다 |
| Alert 연결 | Clear/Fail/Confirm Alert 참조가 유지되어야 한다 |
| Audio 연결 | 클릭음/알림음 AudioSource 참조가 유지되어야 한다 |
| 테스트 | 스크립트 재컴파일 warning 0, 가능한 경우 StageSceneConnectionEditModeTests 실행 |

## 8. 자동 검사 후보

정리 후 아래 테스트를 추가하면 구조 회귀를 줄일 수 있다.

| 테스트 이름 후보 | 확인 내용 |
|---|---|
| `FightScenes_HaveRequiredUiGroups` | 모든 전투씬에 `UI_BattleUnits`, `UI_Turn`, `UI_PlayerAction`, `UI_ResultAlert` 존재 |
| `StoryScenes_HaveRequiredUiGroups` | 모든 스토리씬에 `UI_Background`, `UI_Characters`, `UI_Dialogue`, `UI_Fade` 존재 |
| `HudScene_HasOrganizedRootGroups` | HUD 루트에 `__UI`, `__Audio`, `__Handlers` 존재 |
| `StartScene_HasRequiredRootGroups` | Start Screen 루트에 `__Systems`, `__Camera`, `__UI` 존재 |

## 9. 주의 사항

- Transform 계층 이동은 RectTransform 값과 부모 기준 좌표에 영향을 줄 수 있다.
- Prefab Instance의 부모를 바꾸면 Override가 늘어날 수 있다.
- 씬 정리는 한 번에 전체 적용하지 않는다.
- 먼저 대표 씬 하나를 정리한 뒤 Play 화면에서 위치와 버튼 동작을 확인한다.
- 이름 변경은 참조를 끊지 않지만, 테스트나 에디터 스크립트가 이름으로 찾는 경우 영향을 줄 수 있다.

## 10. 1차 적용 기록

적용일: 2026-05-31

적용 범위:

- `Assets/Screen/HUD.unity`
- `Assets/Screen/FightScene/Fight Screen_1Stage.unity`
- `Assets/Tests/EditMode/Editor/StageSceneConnectionEditModeTests.cs`

적용 내용:

- `HUD.unity`는 구분선 루트 오브젝트를 제거하고 `__UI`, `__Audio`, `__Handlers` 루트 그룹으로 정리했다.
- `HUD.unity`의 `MenuCanvas`에 남아 있던 비활성 레거시 `MenuView` 컴포넌트 참조는 Unity missing script 검사에서 실패하여 제거했다. 현재 메뉴 처리는 `__Handlers/MenuHandler` 기준이다.
- `Fight Screen_1Stage.unity`는 루트를 `__UI`, `__Systems`, `__Camera`로 정리했다.
- `HUD/MenuCanvas` 하위 UI는 `UI_00_Background`, `UI_10_Menu`, `UI_20_Settings`, `UI_90_Alerts`로 번호순 정리했다.
- `BattleCanvas` 하위 UI는 `UI_00_Background`, `UI_10_BattleField`, `UI_20_TurnStatus`, `UI_30_PlayerCommands`, `UI_40_Mana`, `UI_50_SelectedUnitState`, `UI_60_SummonPicker`, `UI_90_ResultAlerts` 그룹 기준으로 정리했다.
- 1차 적용은 대표 씬 확인까지만 진행했다. `Fight Screen_2Stage`부터 `7Stage`, Start, Stage Select, Story 씬은 아직 적용하지 않았다.

검증 상태:

- `StageSceneConnectionEditModeTests`에 `HudScene_HasOrganizedRootGroups`, `FightSceneOne_HasOrganizedUiGroups`를 추가했다.
- 최초 테스트에서 `HUD/MenuCanvas` missing script 1건이 발견되어 레거시 컴포넌트 참조를 제거했다.
- 번호형 UI 구조 적용 후 Unity MCP `recompile_scripts`는 warning 0으로 통과했다.
- `Summoner.EditModeTests.StageSceneConnectionEditModeTests`는 5/5 통과했다.
