# Game System Refactoring Roadmap

## Current Phase

Phase 1 - GameSystem stabilization and QA handoff.

## Goal

저장, 설정, 메뉴, 씬 이동, 스테이지 진행도 흐름을 한 곳에서 이해할 수 있게 만든다.
게임 흐름 코드는 PlayerPrefs 키를 직접 알지 않게 하고, UI View와 실제 저장/씬 이동 책임을 분리한다.

## Scope

```text
Assets/Script/Start Screen
Assets/Script/Menu
Assets/Script/Option
Assets/Script/Stage
Assets/Script/Save
Assets/Script/Battle/BattleResultAlertView.cs
```

## Refactoring Rules

- 저장, 설정, 씬 이동은 기능 단위로만 수정한다.
- PlayerPrefs 키 직접 접근은 새로 늘리지 않는다.
- 기존 씬/프리팹 연결을 바꾸는 작업은 먼저 영향 범위를 확인한다.
- `StartScreenView`, `StageSelectView`, `BattleResultAlertView`처럼 UI와 연결된 파일은 View 책임과 흐름 제어 책임을 구분한다.
- 상세 변경 기록은 이 문서에 누적하지 않고 `WorkLogs/YYYY-MM-DD.md`에 기록한다.

## Current Structure

현재 흐름:

```text
StartScreenView
 -> 새 게임/이어하기 버튼
 -> GameSaveController
 -> 씬 이동

StageSelectView
 -> playingStage 저장
 -> Stage별 Story/Fight Screen 이동

BattleResultAlertView
 -> 클리어 시 savedStage, playingStage 저장
 -> 다음 Stage 또는 Epilogue 이동

Option Views
 -> Audio/Video/Gameplay 설정 적용
 -> 일부 PlayerPrefs 직접 접근 유지
```

목표 흐름:

```text
UI 입력
 -> Save/Setting/Scene 전용 코드
 -> 실제 저장/적용/이동 수행
```

## Active Tasks

| ID | Task | Status | Notes |
|---|---|---|---|
| GS-QA-00 | MCP QA 실행 복구 | Highest | 수동 QA 대신 MCP `run_tests` timeout 원인 수정 후 진행 |
| GS-QA-01 | 새 게임 시작 QA 요청 | Pending QA | QA 에이전트가 진행도만 초기화되고 설정값이 유지되는지 확인 |
| GS-QA-02 | 이어하기 QA 요청 | Pending QA | QA 에이전트가 저장 없음/있음 버튼 표시와 Stage Select 이동 확인 |
| GS-QA-03 | 스테이지 선택/전투 클리어 QA 요청 | Pending QA | QA 에이전트가 `playingStage`, `savedStage`, 다음 씬 이동 확인 |
| GS-SET-01 | 설정 View 내부 변수명 정리 검토 | Pending | `audioController`, `videoController`, `gamePlayController` 등 |
| GS-SCENE-01 | StageFlow 중복 제거 | Pending | StageController, StageSelectView, BattleResultAlertView |

## Completed

완료 상세는 `CompletedArchive.md`와 `WorkLogs/2026-05-30.md`에 보관한다.

요약:

- GameSystem UI 연결 클래스는 `*View` 이름으로 정리됨.
- 씬 UnityEvent 타입명은 현재 View 이름 기준으로 정리됨.
- 진행도 읽기 흐름은 `GameSaveController.GetGameSaveOrDefault()` 기준으로 정리됨.
- GameSystem 정적 회귀 테스트가 추가됨.

## Done Criteria

```text
1. 새 게임 시작 후 savedStage가 1인지 확인
2. 새 게임 시작 후 볼륨/해상도 설정이 유지되는지 확인
3. 이어하기 버튼이 savedStage 기준으로 정상 표시되는지 확인
4. 스테이지 선택 시 playingStage와 씬 이동이 정상인지 확인
5. 전투 클리어 후 다음 스테이지 저장이 정상인지 확인
6. ESC 메뉴, 메인 이동, 종료 Alert가 정상 동작하는지 확인
7. 관련 QAReports에 QA 요청 또는 결과 기록
8. FileStateIndex 갱신
```

## Latest Summary

2026-05-30: 진행도 읽기와 주요 View 이름 정리는 완료 상태다.
다음 리팩토링 작업은 설정 View 내부 변수명 정리 여부 판단이다.
다음 작업 최우선순위는 수동 QA가 아니라 MCP `run_tests` timeout 원인 수정과 GameSystem PlayMode 재실행이다.
