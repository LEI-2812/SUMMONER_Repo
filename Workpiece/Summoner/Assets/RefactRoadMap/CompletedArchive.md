# Completed Refactor Archive

## Purpose

완료된 리팩토링 작업을 Roadmap 밖에 보관한다.
Roadmap은 현재 상태와 다음 작업만 보여주고, 완료된 작업의 누적 요약은 이 문서에서 관리한다.

## 2026-05-30

### Documentation

- 리팩토링 운영 규칙을 정리했다.
- 코드 변경 전 확인 규칙, 기능 단위 리팩토링 규칙, 파일 상태 추적 규칙을 문서화했다.
- 자동 리팩토링 모드 규칙을 추가했다.
- Roadmap과 WorkLogs의 역할을 분리했다.

### GameSystem

- GameSystem UI 연결 클래스는 `*View` 이름 기준으로 정리했다.
- 씬 UnityEvent 타입명은 현재 View 클래스명 기준으로 정리했다.
- 진행도 읽기 흐름은 `GameSaveController.GetGameSaveOrDefault()` 기준으로 정리했다.
- 저장 관련 PlayMode 테스트와 정적 회귀 테스트를 추가했다.

### StorySystem

- `StoryImageView`, `FadePanelView`, `MainSceneButtonView`, `StorySkipView` 이름 정리를 완료했다.
- `StorySceneMove`를 추가해 스토리 종료 후 다음 씬 결정 책임을 분리했다.
- `DialogueLineShow`를 추가해 대사 Text UI 출력 책임을 분리했다.
- `StoryProgressAdvance`를 추가해 Dialogue/Line 인덱스 진행 책임을 분리했다.
- `DialogueDatabaseLoad`, `DialogueCsvParse`를 추가해 CSV 로드와 파싱 책임을 분리했다.
- StorySystem 정적 회귀 테스트를 추가했다.

### BattleCore

- Battle Core 리팩토링 대상을 턴, 마나, 공격, 상태이상, 승패 판정으로 정리했다.
- `BattleResultAlertView`, `SummonStatePanelView` 이름 변경 상태를 기록했다.

### BattleContent

- Battle Content 리팩토링 대상을 소환수 뽑기, 데이터, 공격 전략, 예측, 적 AI로 정리했다.
- `PickSummonPanelView` 이름 변경 상태를 기록했다.

## Archive Rule

새 완료 항목은 날짜별로 이 문서에 짧게 추가한다.
세부 수정 파일, 검증 출력, 실패 로그는 `WorkLogs/YYYY-MM-DD.md`에만 남긴다.
