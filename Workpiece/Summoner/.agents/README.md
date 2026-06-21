# Agents

이 폴더는 선택 호출형 에이전트 역할만 설명한다.
현재 작업 상태와 완료 이력은 두지 않는다.

## 빠른 라우팅

| 상황 | 읽을 문서 |
|---|---|
| 기능 슬라이스 선정 | `.agents/roles/CoordinatorAgent.md`, 관련 코드 |
| 코드 개발 | `.agents/roles/DevAgent.md`, 관련 코드 |
| 설계/QA 검증 | `.agents/roles/VerificationAgent.md`, 변경 코드/테스트 |
| 문서 정리 | `.agents/roles/DocumentationAgent.md`, 변경 요약 |
| 릴리즈 | `.agents/roles/ReleaseAgent.md`, dev-log/검증 상태 |

## 운영 원칙

- CoordinatorAgent가 필요한 에이전트만 선택 호출한다.
- CoordinatorAgent는 기능 슬라이스를 Ready로 두기 전에 완료조건을 정한다.
- CoordinatorAgent는 사용자 판단이 필요한 내용만 보여주고, 문서 갱신과 다음 에이전트 인계는 바로 처리한다.
- DevAgent가 기본 개발 흐름이다.
- DevAgent는 코드 변경 전에 적용 예정 diff와 완료조건을 보여주고 진행 여부를 확인한다.
- DevAgent는 완료조건을 만족하면 같은 기능의 추가 미세정리를 이어가지 않는다.
- 문서 상태 기록은 사용자 진행 승인 없이 바로 수행한다.
- VerificationAgent는 review mode 또는 QA mode가 필요할 때만 호출한다.
- DocumentationAgent는 상시 기록 담당이 아니다.
- 작은 변경마다 기존 기능 전체를 테스트하지 않는다.
- active 문서에는 현재 슬라이스와 다음 호출 대상만 둔다.

## 하지 말 것

- 에이전트를 고정 파이프라인으로 계속 이어가기
- DevAgent가 diff 승인 없이 코드 변경하기
- 완료조건 없이 넓은 리팩토링 후보 넘기기
- 기능 개발 중간마다 VerificationAgent 호출하기
- DocumentationAgent를 매 작업마다 붙이기
- 현재 판단에 필요 없는 문서까지 읽기
- `AgentModes.md`, `teams/`, `templates` 같은 참고 문서 다시 늘리기
