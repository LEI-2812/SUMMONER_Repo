# Agent Workflow

이 문서는 전체 하네스만 둔다.
세부 기준은 현재 호출된 역할 문서 하나와 관련 코드로 판단한다.

## 기본 구조

```text
CoordinatorAgent
-> DevAgent
-> CoordinatorAgent
```

CoordinatorAgent가 필요하다고 판단할 때만 확장 에이전트를 호출한다.
CoordinatorAgent는 사용자를 대신해 흐름을 라우팅한다.
사용자 판단이 필요한 내용은 보여주고, 문서 갱신과 다음 에이전트 인계는 바로 처리한다.

```text
CoordinatorAgent -> VerificationAgent
CoordinatorAgent -> DocumentationAgent
CoordinatorAgent -> ReleaseAgent
```

## 역할

- CoordinatorAgent: 기능 슬라이스와 호출 대상 결정.
- DevAgent: 기능 슬라이스 안에서 코드 구현.
- VerificationAgent: review mode와 QA mode로 검증 게이트 수행.
- DocumentationAgent: 기능 완료, handoff, release note 문서 정리.
- ReleaseAgent: 릴리즈/배포/버전 정리.

## 아키텍처 기준

기본 구조는 `Feature -> Presentation / Application / Domain / Infrastructure`다.

```text
User Input
-> Controller
-> UseCase
-> Entity / Domain Rule
-> UseCase Result
-> Controller
-> View Update
```

- Unity는 Component-based로 유지한다.
- 폴더는 Feature-based로 정리한다.
- UI는 `View / Controller`만 사용한다.
- Application 흐름은 `UseCase` 이름으로 통일한다.
- Domain은 Entity, State, Rule, Strategy를 담당한다.
- Infrastructure는 PlayerPrefs, 파일, 외부 API, 유료 LLM, 클라우드처럼 교체 가능한 외부 연동을 담당한다.
- ScriptableObject는 정적 게임 데이터로 사용하고, 런타임 상태나 저장 흐름을 넣지 않는다.
- 이벤트는 UI, 사운드, 이펙트 알림에만 제한한다.
- 세부 기준은 `.codex/workflow/architecture-role-rule.md`를 우선한다.

## 상태 하네스

| 상태 | 통과 조건 | 다음 |
|---|---|---|
| Analysis | 전체 코드 또는 feature slice를 읽고 현재 책임/이름/위험을 분류 | Coordinator Ready |
| Coordinator Ready | 기능 슬라이스, 완료조건, 제외 범위, 호출 대상 확정 | DevAgent 또는 선택 에이전트 |
| Dev ChangeProposal | 수정 대상, 변경 범위, 적용 예정 diff, 완료조건, 검증 게이트 제시 | 사용자 승인 또는 Coordinator Ready |
| Dev InProgress | 승인된 diff 범위 안에서 기능 슬라이스 구현, 필요한 검색/컴파일 1차 확인 | Coordinator Ready |
| Verification Gate | review mode 또는 QA mode 결과 정리 | Coordinator Ready 또는 DevAgent |
| Documentation Gate | 필요한 문서 정리 | Coordinator Ready |
| Release Gate | 릴리즈 위험과 검증 상태 정리 | Coordinator Ready |

- Ready는 하나만 둔다.
- CoordinatorAgent는 보고 가치가 낮은 문서 갱신과 에이전트 인계를 사용자 승인 없이 진행한다.
- CoordinatorAgent는 코드 diff 승인, 위험 작업, 제품 판단, 환경/코드 실패 분류처럼 사용자 판단이 필요한 것만 보여준다.
- CoordinatorAgent는 기능 슬라이스를 Ready로 두기 전에 완료조건을 먼저 정한다.
- 큰 구조 정리는 `분석 -> Dev ChangeProposal -> 개발 -> Verification Gate`를 반복한다.
- 분석 단계에서는 여러 하위 에이전트를 병렬로 쓸 수 있다. 하위 에이전트는 서로 다른 feature나 질문을 맡고, 읽기 전용으로 현재 책임과 위험을 분류한다.
- 개발 하위 에이전트는 수정 파일 범위가 서로 겹치지 않을 때만 사용한다.
- CoordinatorAgent는 같은 기능 슬라이스에서 이름, 주석, 테스트 문자열 같은 미세정리가 두 번 연속 이어지면 추가 정리를 다음 작업으로 잡지 않는다.
- DevAgent는 코드 변경 전에 적용 예정 diff를 보여주고 진행 여부를 확인한다.
- 승인 전에는 제품 코드, 테스트 코드, 에셋을 수정하지 않는다.
- 문서 상태 기록은 승인 대기 없이 최소 범위로 바로 진행한다.
- 승인은 제시된 diff와 변경 범위에만 적용한다.
- 작업 중 diff 범위가 넓어지면 새 ChangeProposal로 되돌린다.
- DevAgent는 완료조건을 만족하면 같은 기능 슬라이스의 남은 이름/주석/미세 구조 정리를 이유로 계속 리팩토링하지 않는다.
- DevAgent는 승인된 기능 슬라이스 안에서도 미세정리만 2회 이상 반복되면 계속 수정하지 않고 Coordinator Ready로 되돌린다.
- DevAgent 뒤에 VerificationAgent를 자동으로 붙이지 않는다.
- VerificationAgent는 호출 조건이 있을 때만 사용한다.
- DocumentationAgent는 상시 기록 담당이 아니다.

## Verification 호출 조건

- public API, 호출 계약, 책임 경계, 구조 변경.
- 저장 데이터, 씬, 프리팹, ScriptableObject asset 값 영향.
- Unity 생명주기, UI/사운드/애니메이션, 전투 런타임 연결 영향.
- 기능 슬라이스 완료 후 검증 게이트 필요.
- 테스트 실패 또는 MCP/환경 실패 분류 필요.
- 사용자가 검증, QA, 리뷰를 요청.

## 검증 게이트

- 검색: 기계적 이름 정리, 호출부 이관, private/protected helper 정리.
- 컴파일: 타입/호출 계약 변경, 인터페이스 구현.
- EditMode: 순수 로직, 데이터 매핑, 전략 생성, 예측 계산.
- PlayMode: Unity 생명주기, 씬/프리팹 연결, UI/사운드/애니메이션, 전투 런타임 연결.

EditMode와 PlayMode는 작은 변경마다 반복하지 않는다.
같은 기능 슬라이스 안에서는 기능 게이트에서 한 번 실행하는 것이 기본이다.
같은 기능 슬라이스에서 이미 검증한 경로는 이름, 주석, 테스트 문자열 정리 때문에 반복 검증하지 않는다.

## 문서 최소화

- active 문서는 현재 기능 슬라이스와 다음 호출 대상만 둔다.
- 완료 상세는 active 문서에 누적하지 않는다.
- 의미 있는 완료만 `.codex/dev-log/YYYY-MM-DD.md`에 짧게 남긴다.
- DocumentationAgent는 기능 완료, handoff, release note 때만 호출한다.
- current-task, agent-status, issue-board, active-qa, dev-log 갱신은 사용자 진행 승인 없이 바로 수행한다.

## 하지 말 것

- 모든 기능을 Coordinator -> Dev -> Review -> QA 파이프라인으로 고정하기
- 네이밍 정리를 독립 작업으로 계속 고르기
- 같은 기능 슬라이스에서 미세정리만 2회 이상 반복하기
- 완료조건을 만족한 기능을 같은 세션에서 계속 확장하기
- 작은 변경마다 EditMode/PlayMode 반복 실행
- 기능 개발 중간마다 VerificationAgent 호출
- DocumentationAgent를 상시 기록 담당으로 붙이기
