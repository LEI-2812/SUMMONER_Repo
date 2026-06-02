# Auto QA Report

이 파일은 Auto QA Mode에서 진행한 QA 내역을 누적 기록한다.
최신 기록을 위에 추가한다.

## 기록 템플릿

```text
## YYYY-MM-DD HH:mm

[Auto QA 상태]

진행한 QA:
- 완료한 검증 또는 수정 사항

진행 중인 QA:
- 현재 확인 중인 실패, 로그, 테스트, 코드 흐름

다음 진행할 QA:
- 다음에 실행할 테스트 또는 확인할 파일

남은 이슈:
- 아직 처리하지 못한 항목
- 실행 실패한 테스트나 외부 도구 문제

검증 결과:
- 실행한 테스트 또는 정적 검사 결과
```

## 2026-06-02 22:04

[Auto QA 상태]

진행한 QA:
- `git diff --check`를 다시 실행해 공백 오류가 없는 것을 확인했다.
- Unity MCP로 EditMode 전체 테스트를 다시 실행했다.
- 현재 변경된 Gameplay 옵션 저장소 분리 흐름을 diff와 파일 내용 기준으로 확인했다.

진행 중인 QA:
- 없음.

다음 진행할 QA:
- PlayMode 테스트는 Unity MCP 테스트 실행 연결이 안정화되면 다시 실행한다.
- `IsStorySkip`, `IsOnlyMouse`, `PlayerPrefs.GetInt`, `PlayerPrefs.SetInt` 전체 검색은 셸 도구가 안정화되면 다시 실행한다.

남은 이슈:
- PlayMode 테스트 실행이 `Connection failed: Unexpected server response: 501`로 실패했다.
- PlayMode 개별 필터 `GameSaveContinueTests` 실행은 MCP 요청 타임아웃으로 실패했다.
- `rg`, `Get-ChildItem`, `Select-String` 일부 실행이 `windows sandbox: spawn setup refresh` 도구 오류로 실패했다.
- `git diff --check`에서 LF가 CRLF로 바뀔 수 있다는 줄바꿈 경고가 남았다.

검증 결과:
- `git diff --check`: 통과. 공백 오류 없음.
- Unity EditMode 전체 테스트: `83/83 passed`, 실패 0개.
- Unity PlayMode 전체 테스트: 실행 실패. MCP 연결 오류 501.
- Unity PlayMode 개별 테스트 `GameSaveContinueTests`: 실행 실패. MCP 요청 타임아웃.

## 2026-06-02 21:45

[Auto QA 상태]

진행한 QA:
- Gameplay 옵션의 `IsStorySkip`, `IsOnlyMouse` PlayerPrefs 직접 접근을 `GameplaySettingStore`로 분리했다.
- `GameplaySettingView`에서 토글 값을 저장소를 통해 읽고 저장하도록 수정했다.
- `StoryScenarioControllerBase`에서 마우스 전용 여부를 `GameplaySettingStore.LoadOnlyMouseEnabled()`로 확인하도록 유지했다.
- 정적 회귀 테스트 기대값을 새 저장소 흐름에 맞게 갱신했다.

진행 중인 QA:
- 없음.

다음 진행할 QA:
- PlayMode 테스트는 MCP 연결이 안정화되면 다시 실행한다.

남은 이슈:
- PlayMode 테스트 실행 중 MCP 연결 오류가 반복됐다.
- 오류: `Connection failed: Unexpected server response: 501`
- Unity 콘솔에는 `[MCP Unity] WebSocket error: An error has occurred in sending data.`가 남았다.

검증 결과:
- `git diff --check`: 통과.
- Unity EditMode 전체 테스트: `83/83 passed`, 실패 0개.
