# Agent Workflow

이 문서는 에이전트 협업 흐름의 최소 기준만 정의한다.
세부 실행 기준은 현재 `Ready`인 `roles/*.md` 하나만 확인한다.

## 기본 흐름

```text
FlowAgent
→ DevAgent
→ DevAgent 반복 가능
→ 기능 목표 완료 후 QAReviewAgent
→ FlowAgent
```

QAReviewAgent는 DevAgent 뒤에 매번 실행하지 않는다.
기능 목표가 완료됐거나 사용자가 `검증해`, `QA해`, `테스트해`처럼 요청했을 때 실행한다.

## 품질 게이트

구현완료는 아래 세 가지가 모두 충족될 때만 사용한다.

1. 기능이 요구대로 동작한다.
2. 설계 품질 상태가 `적정`이다.
3. QA 깊이 상태가 `검증완료`다.

FlowAgent는 개발 전에 `기능 완료 조건`, `설계 품질 기준`, `QA 깊이 기준`을 정리한다.
DevAgent는 책임분리, SOLID 최소 기준, 타 클래스 public 접근 영향을 확인한다.
QAReviewAgent는 사전 QA 깊이 기준을 만족하지 못하면 `Pass`가 아니라 `검증부족`으로 기록한다.

## 상태 기준

| 상태 | 의미 | 다음 에이전트 |
|---|---|---|
| 시작전 | 기능 목표만 정해짐 | FlowAgent 또는 DevAgent |
| 진행중 | DevAgent가 개발 중 | DevAgent |
| 동작완료 | 기능 동작은 연결됐지만 설계 품질 또는 QA 깊이 확인이 남음 | DevAgent 또는 QAReviewAgent |
| 검증대기 | 개발 중간 단계라 최종 QA 전 | DevAgent |
| QA필요 | 기능 구현과 설계 품질 기준을 충족했고 QA 대기 | QAReviewAgent |
| QA진행중 | QAReviewAgent 검증 중 | QAReviewAgent |
| QA완료 | QA 통과 후 FlowAgent 정리 대기 | FlowAgent |
| 구현완료 | 기능, 설계 품질, QA, 정리가 모두 끝남 | FlowAgent |
| 보류 | 사용자 결정이나 외부 조건 때문에 멈춤 | FlowAgent |
| 후속작업대기 | 진행 중인 기능 없음 | FlowAgent |

## 문서 원칙

- 현재 판단은 `.codex/workflow/`에 남긴다.
- 진행 중 QA는 `.codex/qa/active-qa.md`에만 둔다.
- 코드 리뷰는 QAReviewAgent 결과에 포함해 기록한다.
- 완료된 DEV/QA 이력 인덱스는 `.codex/records/`의 CSV에 남긴다.
- 완료 상세는 `.codex/archive/`와 `.codex/archive/qa/`로 보낸다.
- 완료 요약과 구조 결정은 `.codex/dev-log/YYYY-MM-DD.md`에 짧게 남긴다.
- active 문서에는 과거 이력을 길게 누적하지 않는다.

## 승인 기준

협업 모드에서는 실제 변경 전 아래를 먼저 제안한다.

1. 수정 대상 파일
2. 수정 이유
3. 예상 변경 범위
4. 책임 분리 관점의 개선 이유
5. diff 또는 변경 전/후 코드

사용자가 `적용해`, `수정해`, `진행해`, `그렇게해`처럼 명확히 승인한 뒤에만 실제 변경한다.

## 자동 진행 기준

- 자동 개발 모드와 자동 루프는 사용자가 명시적으로 요청했을 때만 사용한다.
- 자동 루프는 `.codex/workflow/automation-rule.md`를 기준으로 한다.
- `agent-status.md`의 `Ready` 에이전트를 실제 수행한 뒤 다음 에이전트로 넘긴다.
- 실제 작업 없이 상태만 넘기지 않는다.
- 위험 작업은 자동으로 진행하지 않고 멈춘다.

위험 작업:
- 파일 삭제
- 씬 변경
- 에셋 삭제 또는 대량 이동
- 저장 데이터 구조 변경
- 외부 API 또는 유료 서비스 연결
- 대규모 폴더 구조 변경
