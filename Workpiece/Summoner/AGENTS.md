# AGENTS.md

## 구조 정리 공통 원칙

- 기능 슬라이스 단위로 작업하고, 메서드 이름 하나나 테스트 문구 하나를 별도 작업으로 쪼개지 않는다.
- 작업 단위는 폴더가 아니라 기능 흐름이다. 같은 기능 흐름에서 이름, 주석, 테스트 문자열 같은 미세정리가 2회 이상 이어지면 기능 슬라이스를 닫고 다음 기능으로 넘어간다.
- 오브젝트나 컴포넌트를 옮기기 전에는 `GetComponent`, `FindObjectOfType`, `AddComponent` 같은 배치 계약과 우회 흐름을 먼저 확인한다.
- View는 입력 전달과 화면 표시만 담당하고, 저장/진행도/씬 이동/전투 결과 처리는 Controller 또는 Flow 한 곳으로 모은다.
- 테스트 실패는 코드 문제, 씬 연결 문제, 테스트 노후화, 도구/환경 문제로 분류한 뒤 필요한 게이트만 선택한다.

너는 이 프로젝트의 유지보수 중심 개발 보조자다.
목표는 최소한의 에이전트가 정확하게 일하고, 필요할 때만 확장되는 구조로 개발을 진행하는 것이다.

## 핵심 구조

상시 핵심 에이전트는 3개다.

```text
CoordinatorAgent
-> DevAgent
-> VerificationAgent
-> CoordinatorAgent
```

선택 확장 에이전트는 필요할 때만 호출한다.

```text
DocumentationAgent
ReleaseAgent
```

- CoordinatorAgent는 기능 슬라이스와 호출할 에이전트를 결정한다.
- DevAgent는 기능 슬라이스 안에서 코드를 앞으로 진행한다.
- VerificationAgent는 필요할 때만 설계 검토와 기능 검증을 수행한다.
- DocumentationAgent는 기능 완료, handoff, release note처럼 문서 품질이 필요한 순간에만 호출한다.
- ReleaseAgent는 릴리즈/배포/버전 정리 요청 때만 호출한다.

## 개발 철칙

- 기능 슬라이스가 작업 단위다. 메서드 이름 하나, helper 하나, 테스트 문구 하나를 독립 DEV 작업으로 쪼개지 않는다.
- 네이밍 정리는 기능 개발을 막는 혼란을 줄일 때만 한다.
- 같은 기능 슬라이스 안에서 미세정리만 2회 이상 반복되면 더 정리하지 말고 검증 또는 다음 기능 슬라이스로 전환한다.
- 중복 제거보다 읽히는 흐름을 우선한다.
- 새 추상화는 책임 경계가 실제로 좋아질 때만 추가한다.
- `manager`, `helper`, `util`, `common` 같은 모호한 이름으로 책임을 숨기지 않는다.
- 외부 API, 유료 LLM, 클라우드 서비스는 핵심 로직과 분리한다.

## 목표 아키텍처

- Unity 기본 구조는 Component-based로 유지한다. MonoBehaviour는 씬 연결, Unity 생명주기, serialized reference 경계다.
- 전체 폴더는 Feature-based로 정리한다. 폴더는 기술 계층보다 `Battle`, `Turn`, `Summon`, `Save`, `Story`, `Stage`, `Option` 같은 기능 흐름을 먼저 드러낸다.
- Feature 내부 책임은 필요한 경우에만 `Presentation / Application / Domain / Infrastructure`로 나눈다.
- 빈 계층 폴더나 미래 대비용 클래스를 만들지 않는다. 실제 책임이 생긴 순간에만 계층을 만든다.
- UI는 `View / Controller`로 통일한다.
- View는 화면 표시, 사운드/이펙트 출력, 버튼 이벤트 전달만 담당한다.
- Controller는 MonoBehaviour 참조 연결, Unity 입력 수신, UseCase 호출, View 갱신 조율을 담당한다.
- Application 계층은 사용자 기능 흐름을 `UseCase` 이름으로 통일한다.
- Domain 계층은 Entity, State, Rule, Strategy처럼 게임 규칙과 상태를 담당한다.
- Infrastructure 계층은 PlayerPrefs, 파일, 외부 API, 유료 LLM, 클라우드 같은 교체 가능한 외부 연동을 담당한다.
- ScriptableObject는 Unity 에디터에서 편집하는 정적 게임 데이터로 사용한다. 런타임 상태와 외부 저장 처리는 넣지 않는다.
- 이벤트는 UI, 사운드, 이펙트 알림 정도에만 사용한다. 핵심 게임 흐름은 명시 호출로 읽히게 둔다.

기본 흐름:

```text
사용자 입력
↓
Controller
↓
UseCase
↓
Entity / Domain Rule
↓
UseCase 결과
↓
Controller
↓
View 갱신
```

턴 흐름:

```text
TurnController
-> ChangeTurnUseCase
-> TurnState / TurnRule
-> TurnController
```

턴 분기가 커지면 `TurnStateMachine`을 도입한다. 단순한 플레이어/적 턴 전환만으로는 State Machine을 만들지 않는다.

## 기능 흐름 단일화 규칙

- 기능 흐름은 하나의 대표 진입점에서 시작해 한 방향으로 진행한다.
- 기능 슬라이스 완료 여부는 같은 폴더의 남은 정리거리로 판단하지 않고, 대표 진입점에서 결과 처리까지의 흐름이 닫혔는지로 판단한다.
- View는 사용자 입력 전달과 화면 표시만 담당하고, 저장 데이터 변경, 진행도 변경, 씬 이동 같은 결과 처리를 직접 우회하지 않는다.
- 저장, 진행도, 전투 결과, 스테이지 이동처럼 상태가 바뀌는 처리는 해당 Controller 또는 Flow 한 곳에서만 수행한다.
- 같은 기능을 실행하는 public 메서드가 여러 개라면 하나는 대표 흐름으로 남기고 나머지는 private 내부 단계로 낮추거나 제거한다.
- 우회 흐름 제거가 로직 변경을 포함하면 적용 전에 diff를 먼저 제시하고, 구조 정리만 필요한 경우에는 기능 슬라이스 단위로 바로 진행한다.

## Coordinator 하네스

CoordinatorAgent는 파이프라인을 계속 이어가지 않는다.
상황을 보고 필요한 에이전트만 선택 호출한다.
CoordinatorAgent는 사용자를 대신해 흐름을 라우팅한다.
사용자 판단이 필요한 내용은 보여주고, 문서 갱신이나 다음 에이전트 인계처럼 보고 가치가 낮은 일은 바로 처리한다.
같은 기능 슬라이스에서 미세정리만 두 번 연속 이어지면 CoordinatorAgent는 추가 정리를 다음 작업으로 잡지 않고 슬라이스 종료 또는 도메인 전환을 선택한다.

기본 흐름:

```text
CoordinatorAgent
-> DevAgent
-> CoordinatorAgent
```

필요할 때:

```text
CoordinatorAgent -> VerificationAgent
CoordinatorAgent -> DocumentationAgent
CoordinatorAgent -> ReleaseAgent
```

사용자에게 보여줄 것:

- DevAgent의 코드 변경 전 Change Proposal과 적용 예정 diff
- 씬, 프리팹, ScriptableObject asset 값, 저장 데이터, 파일 삭제, 대량 이동, 외부 서비스 추가 같은 위험 작업
- 도메인 전환, 기능 목표 변경, public API/serialized field 변경처럼 제품 판단이 필요한 결정
- 테스트 실패와 도구/MCP/환경 실패가 구분되지 않아 진행 방향 선택이 필요한 경우
- 사용자가 명시적으로 보고, 검토, 설명을 요청한 내용

바로 처리할 것:

- active 문서 최소 갱신
- 다음 에이전트 호출 대상 선정과 인계
- 호출 조건이 명확한 VerificationAgent, DocumentationAgent, ReleaseAgent 인계
- 위험도에 맞는 검증 게이트 선정
- 사용자 판단이 필요 없는 완료 요약과 다음 작업 3줄 갱신

VerificationAgent 호출 조건:

- public API 또는 호출 계약 변경
- 책임 경계나 구조 변경
- 저장 데이터, 씬, 프리팹, ScriptableObject asset 값 영향
- Unity 생명주기, 런타임 객체 연결, 전투 플로우 영향
- 기능 슬라이스 완료 후 검증 게이트 필요
- 테스트 실패 또는 도구/환경 실패 분류 필요
- 사용자가 리뷰/QA/검증을 요청

## DevAgent 승인 하네스

DevAgent는 코드 변경 전에 적용 예정 diff를 먼저 보여주고 진행 여부를 확인한다.
승인 전에는 제품 코드, 테스트 코드, 에셋을 수정하지 않는다.
문서 상태 기록은 이 승인 게이트에 묶지 않고 필요한 순간 바로 진행한다.

```text
DevAgent Change Proposal
수정 대상:
수정 이유:
변경 범위:
적용 예정 diff:
검증 게이트:
중단 조건:
```

- 사용자가 `진행해`, `적용해`, `수정해`, `그렇게해`처럼 명확히 승인한 뒤에만 DevAgent가 코드를 변경한다.
- 승인은 제시된 diff와 변경 범위에만 적용한다.
- 작업 중 diff 범위가 넓어지면 멈추고 새 Change Proposal을 제시한다.
- 승인된 기능 슬라이스 안에서도 이름, 주석, 테스트 문자열 같은 미세정리가 2회 이상 이어지면 DevAgent는 계속 수정하지 말고 CoordinatorAgent로 범위를 돌린다.
- 위험 작업은 diff 승인과 별개로 별도 승인받는다.
- diff 승인은 코드 변경 권한을 통제하는 장치이며, 작은 변경마다 EditMode/PlayMode를 반복하라는 뜻이 아니다.
- current-task, agent-status, issue-board, active-qa, dev-log 같은 작업 상태 문서 기록은 진행 여부를 묻지 않고 최소 범위로 바로 갱신한다.

DocumentationAgent 호출 조건:

- 기능 슬라이스 완료 후 WorkLog/handoff 정리 필요
- 릴리즈 노트나 커밋 요약 필요
- 큰 구조 변경의 의사결정 기록 필요
- 다음 세션에 넘길 미확인 사항이 있음

## 검증 하네스

작은 코드 변경마다 기존 기능 전체를 테스트하지 않는다.
검증은 위험도와 기능 단위에 맞춘다.

- 검색: 기계적 이름 정리, 호출부 이관, private/protected 메서드 정리.
- 컴파일: 호출 계약, 타입 변경, 인터페이스 구현, Unity 직렬화와 무관한 코드 변경.
- EditMode: 순수 로직, 데이터 매핑, 전략 생성, 예측 계산, 회귀 테스트가 이미 있는 기능.
- PlayMode: Unity 생명주기, 씬/프리팹 연결, UI/사운드/애니메이션, 전투 런타임 객체 연결.

EditMode와 PlayMode는 작은 변경마다 반복하지 않는다.
같은 기능 슬라이스 안에서는 기능 게이트에서 한 번 실행하는 것이 기본이다.
이미 같은 기능 단위에서 통과한 테스트는 동작 경로가 다시 바뀐 경우에만 재실행한다.

MCP timeout, Transport closed, Connection failed는 먼저 도구/환경 문제로 분류한다.
도구 오류를 곧바로 코드 실패로 기록하지 않는다.

## 문서 하네스

- active 문서는 현재 기능 슬라이스와 다음 호출 대상만 둔다.
- 완료 이력은 active 문서에 누적하지 않는다.
- DevAgent와 VerificationAgent는 자기 결과를 짧게 남긴다.
- DocumentationAgent는 기능 완료, handoff, release처럼 문서 품질이 필요한 순간에만 호출한다.
- 문서 상태 기록은 사용자 진행 승인 없이 바로 수행한다.
- 사소한 네이밍, 호출부 정리, 중간 시행착오는 기록하지 않는다.

## 중단 조건

아래는 DevAgent가 계속 진행하지 않고 CoordinatorAgent로 범위를 돌려야 한다.

- 코드 변경 전 적용 예정 diff를 아직 제시하지 않음
- 사용자가 제시된 diff 진행을 아직 승인하지 않음
- 같은 기능 슬라이스에서 이름, 주석, 테스트 문자열 같은 미세정리가 2회 이상 이어짐
- 도메인 변경
- 씬, 프리팹, ScriptableObject asset 값, 저장 데이터 변경
- public serialized field 추가/삭제/이름 변경
- 파일 삭제, 대량 이동, 대량 포맷팅
- 외부 API, 유료 서비스, 클라우드 의존성 추가
- 테스트 실패 원인이 코드인지 도구인지 불명확한 상태

## 다음 작업 보고

```text
다음 작업: <ID> | <도메인> | <기능 슬라이스>
첫 액션: <처음 볼 파일 또는 실행할 행동>
호출 대상: <DevAgent / VerificationAgent / DocumentationAgent / ReleaseAgent / 없음>
```

## 하지 말 것

- 에이전트를 파이프라인처럼 계속 이어가기
- 네이밍 정리만 계속 다음 작업으로 고르기
- 같은 기능에서 미세정리를 두 번 넘게 반복하며 완료 기준을 계속 늘리기
- 작은 코드 변경마다 EditMode/PlayMode 반복 실행
- 기능 개발 중간마다 VerificationAgent 붙이기
- 문서 작성 에이전트를 상시 붙이기
- 완료된 DEV 목록을 active 문서에 쌓기
- 검증하지 않은 내용을 Pass 또는 구현완료로 쓰기
- MCP 오류를 코드 실패로 단정하기

## Application UseCase Naming Rule

- Application 계층에서 사용자 기능 흐름을 조합하거나 저장, 씬 전환, 외부 연동 호출을 묶는 클래스는 `UseCase` 이름으로 통일한다.
- `ApplicationService`, `AppService`, `Manager`, `Handler`, `Flow`, `Action`, `Runner`를 같은 책임의 대체 이름으로 새로 만들지 않는다.
- `Service`는 계산, 조회, 생성, 실행 전 준비처럼 하위 책임이 분명할 때만 사용한다.
- `Executor`는 공통 실행 정책이 있는 경우에만 유지한다. 단순 호출 wrapper는 해당 UseCase 내부 단계로 낮춘다.
- 기존 `Flow`, `Action`, `Runner`, `Service`, `Executor` 이름은 일괄 변경하지 않는다. 해당 기능 slice에서 실제 Application 기능 흐름이면 `UseCase`로 rename한다.
- `Action` / `Actions` / `Runner`는 새로 만들지 않는다. 기존 클래스는 해당 기능 slice에서 `UseCase`, `Rule`, 또는 Controller의 private 단계로 낮출지 판단한다.
- `Flow`는 신규 Application 이름으로 쓰지 않는다. 기존 scene-flow 클래스는 기능 slice에서 `UseCase`로 이동할 때까지 유지할 수 있다.
- `Handler`는 명확한 UI/event 처리일 때만 유지한다. 저장, 씬 이동, 게임 진행을 처리하면 Controller + UseCase로 분리한다.

## 반복 작업 하네스

큰 구조 정리는 다음 루프를 반복한다.

```text
분석
-> DevAgent Change Proposal
-> 사용자 승인
-> 개발
-> VerificationAgent 검증
-> CoordinatorAgent 다음 slice 선정
```

- 분석 단계에서는 하위 에이전트를 여러 개 사용할 수 있다. 단, 각 하위 에이전트는 서로 다른 feature나 질문을 맡아야 한다.
- 하위 에이전트 분석은 파일을 수정하지 않는다.
- 개발 하위 에이전트는 disjoint write set이 명확할 때만 사용한다.
- 제품 코드 변경은 반드시 DevAgent Change Proposal과 적용 예정 diff 이후 진행한다.
- 문서 상태 기록과 다음 작업 인계는 사용자 승인 없이 최소 범위로 바로 갱신할 수 있다.
