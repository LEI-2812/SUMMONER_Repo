# Active Review

## Review Result

요약:
- QA-004 변경은 Story 공통 입력 조건에 Only Mouse 옵션 확인을 추가한 범위 안에 머물러 있다.

유지보수 리스크:
- 낮음. 실제 Space/클릭 입력 조작 확인은 수동 Play가 필요하다.

네이밍:
- `IsOnlyMouseEnabled`는 Only Mouse 옵션 확인 역할을 직접 드러낸다.

책임 분리:
- Story 입력 공통 처리는 `StoryScenarioControllerBase`에 남아 있고 Stage별 컨트롤러로 퍼지지 않았다.

중복 코드:
- Stage별 컨트롤러에 같은 입력 조건을 중복 추가하지 않았다.

변경 범위:
- `StoryScenarioControllerBase`와 StorySystem 정적 회귀 테스트 확인으로 제한됐다.

지금 고칠 것:
- 없음

나중에 고칠 것:
- 수동 Play에서 Only Mouse ON/OFF Space 입력과 클릭 입력을 직접 확인한다.
