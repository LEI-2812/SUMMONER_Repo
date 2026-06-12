# Workflow

이 폴더는 현재 판단판이다.
완료 이력을 쌓는 곳이 아니다.

## 파일 역할

- `current-task.md`: 현재 기능 슬라이스와 다음 호출 대상
- `issue-board.md`: Ready / In Progress / Blocked만
- `agent-status.md`: 다음 Ready 에이전트 하나
- `automation-rule.md`: 자동 루프를 켰을 때만 읽는 기준

## active 문서 규칙

- current-task는 현재 슬라이스와 다음 호출 대상만 둔다.
- issue-board는 오래된 Done 목록을 남기지 않는다.
- agent-status는 Ready 하나만 둔다.
- CoordinatorAgent가 DevAgent, VerificationAgent, DocumentationAgent, ReleaseAgent 중 필요한 에이전트를 선택 호출한다.
- CoordinatorAgent는 사용자 판단이 필요한 내용만 보여주고, 문서 갱신과 다음 에이전트 인계는 바로 처리한다.
- DevAgent가 기본 개발 흐름이다.
- DevAgent는 코드 변경 전에 적용 예정 diff를 보여주고 진행 여부를 확인한다.
- 승인 전에는 제품 코드, 테스트 코드, 에셋을 수정하지 않는다.
- 문서 상태 기록은 사용자 진행 승인 없이 바로 수행한다.
- VerificationAgent는 review mode 또는 QA mode가 필요한 경우에만 호출한다.
- DocumentationAgent는 기능 완료, handoff, release note 때만 호출한다.
- 작은 변경마다 EditMode/PlayMode를 반복하지 않는다.
- 의미 있는 완료만 `../dev-log/YYYY-MM-DD.md`에 날짜별로 1~3줄 남긴다.
- 사소한 질문, 단순 확인, 문구 수정, 중간 시행착오는 기록하지 않는다.

## 하지 말 것

- 완료 Handoff 전문을 current-task에 누적하기
- 완료된 DEV 목록을 issue-board에 계속 쌓기
- 모든 에이전트를 고정 파이프라인처럼 순서대로 호출하기
- DevAgent가 diff 승인 없이 코드 변경하기
- 기능 개발 중간마다 VerificationAgent 호출하기
- DocumentationAgent를 상시 기록 담당으로 붙이기
- Ready를 둘 이상 두기
- 자동 루프가 아닌데 automation-rule을 매번 읽기
