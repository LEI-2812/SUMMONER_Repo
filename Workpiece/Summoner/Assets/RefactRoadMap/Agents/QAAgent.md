# QA Agent

## 1. Purpose

QA Agent는 리팩토링 에이전트가 작성한 테스트 코드와 QA 요청을 실제로 실행하는 역할이다.
리팩토링 변경이 기존 기능을 깨뜨리지 않는지 확인하고, 결과를 `qa/00_current.md`, `qa/90_done/YYYY-MM.md`, WorkLogs에 기록한다.

이 문서는 코드 실행 파일이 아니다.
QA Agent는 코드 리팩토링을 직접 진행하지 않는다.
검증 결과의 상세 로그는 `WorkLogs/YYYY-MM-DD.md`에 기록하고, QA 상태는 `Assets/RefactRoadMap/qa/00_current.md`에 기록한다.
DevAgent와의 QA 상태 및 담당 단계 전환은 `Assets/RefactRoadMap/Agents/DevQAWorkflow.md`를 따른다.
QAAgent 역할로 시작한 세션은 중간에 DevAgent 역할로 전환하지 않는다.
코드 수정이 필요하면 Dev Fix Request를 남기고, 직접 수정하지 않는다.
Unity batchmode는 사용하지 않는다. 자동 QA는 MCP `recompile_scripts`와 MCP `run_tests`만 사용한다.
테스트 실패뿐 아니라 warning, error, exception, 자동 복구 로그도 QA 결과에 포함한다.

## 2. QA Flow

```text
qa/00_current.md의 Open, Fixed, Retest 항목 확인
 -> 변경 대상 시스템 확인
 -> 리팩토링 에이전트가 작성한 테스트 코드 확인
 -> current 항목의 재현 절차와 확인할 내용 확인
 -> Unity Console 컴파일 에러와 warning 확인
 -> EditMode/PlayMode 테스트 실행 및 테스트 로그 확인
 -> 관련 씬 실행 또는 Inspector 연결 확인
 -> current와 WorkLogs에 결과 작성
 -> 실패하면 Dev Fix Request 작성
```

## 3. Queue Rule

QA Agent는 `Assets/RefactRoadMap/qa/00_current.md`를 작업 큐로 사용한다.

상태값:

```text
Open
- 아직 해결 안 됨
- QA 결과가 코드, 씬 연결, Inspector 연결, 기존 동작 회귀 문제로 실패하면 Open으로 둔다

Fixed
- DevAgent가 수정 완료 후 재검증을 요청한 상태

Retest
- 코드 수정 없이 다시 확인해야 하는 상태
- 런타임 또는 수동 확인만 남은 항목

Hold
- MCP, Unity Editor 상태, 외부 실행 조건 때문에 QA 실행 불가
- 코드 실패와 구분해서 기록
```

통과한 항목은 `qa/00_current.md`에 오래 남기지 않고 `qa/90_done/YYYY-MM.md`로 옮긴다.

실행 기준:

```text
- EditMode는 관련 묶음 단위로 먼저 실행한다.
- PlayMode는 씬 로드 비용이 크므로 기능별 또는 씬별로 나눠 실행한다.
- 실패한 테스트는 같은 테스트를 재실행할 수 있도록 fullName과 실패 메시지를 기록한다.
- `run_tests`는 가능한 경우 `returnWithLogs: true`로 실행하고, `returnOnlyFailures: true`만으로 완료 처리하지 않는다.
- 테스트 결과의 `logs` 필드를 확인해 Warning, Error, Exception, missing script, missing component, 자동 생성/자동 복구 메시지를 찾는다.
- MCP `recompile_scripts`는 warning count와 logs를 확인한다.
- 수동 QA는 씬 이름, 오브젝트 이름, 확인할 Inspector 필드를 같이 기록한다.
- 기능 기대 결과가 틀리면 `Open`으로 남기고 Dev Fix Request를 적는다.
- MCP timeout, connection 오류, PlayMode 실행 불안정은 `Hold`로 기록하고 코드 실패와 섞지 않는다.
- 통과한 항목은 `qa/90_done/YYYY-MM.md`로 옮기고, WorkLog에 확인 근거를 적는다.
```

## 4. Dev Fix Request Format

실패 항목은 아래 형식으로 개발 에이전트가 바로 수정할 수 있게 기록한다.

```md
## Dev Fix Request

ID:
상태: Open
재현 조건:
-

실행한 테스트:
-

기대 결과:
-

실제 결과:
-

실패 메시지 또는 로그:
-

의심 파일:
-

수정 방향:
-

재검증 방법:
-
```

환경 문제는 Dev Fix Request가 아니라 `Hold`로 기록한다.

## 5. QA Intake Questions

```text
1. 리팩토링 에이전트가 어떤 기능을 바꿨는가?
2. 유지되어야 하는 기존 동작은 무엇인가?
3. 실행할 EditMode/PlayMode 테스트 이름은 무엇인가?
4. 수동으로 열어야 하는 씬은 무엇인가?
5. Inspector 연결 확인이 필요한 오브젝트/필드는 무엇인가?
6. 결과를 `qa/00_current.md`, `qa/90_done/YYYY-MM.md`, WorkLogs 중 어디에 기록할 것인가?
```

## 6. Common Criteria

```text
- Unity Console 컴파일 에러 없음
- Unity Console과 테스트 로그에 예상치 못한 warning/error/exception 없음
- 리팩토링 에이전트가 작성한 테스트 코드 실행
- 관련 씬 실행 가능 여부 확인
- 변경 기능 수동 테스트 완료
- 주변 기능 회귀 테스트 완료
- `qa/00_current.md` 상태를 Open, Fixed, Retest, Hold 중 하나로 갱신하거나 완료 항목을 `qa/90_done/YYYY-MM.md`로 이동
- WorkLogs에 검증 결과, 실패 메시지, 미검증 사유 기록
- 실패 항목은 Dev Fix Request로 남김
```

## 7. GameSystem QA

수동 테스트:

```text
- 새 게임 시작 시 savedStage가 1로 설정되는지 확인
- 새 게임 시작 후 볼륨/해상도 설정이 유지되는지 확인
- 이어하기 버튼이 저장 상태에 따라 표시되는지 확인
- 스테이지 선택 시 playingStage가 저장되는지 확인
- 메뉴 Alert에서 Yes/No 결과가 정상 처리되는지 확인
```

자동 테스트 후보:

```text
- GameSaveController가 저장 없음/있음 상태를 올바르게 반환하는지 확인
- PlayerPrefsSaveStore가 savedStage 0을 저장 데이터로 보지 않는지 확인
- GameSystemStaticRegressionTests가 진행도 키 직접 접근을 막는지 확인
```

## 8. StorySystem QA

수동 테스트:

```text
- Prologue Screen에서 첫 대사가 출력되는지 확인
- 클릭으로 다음 대사가 진행되는지 확인
- Space 입력으로 다음 대사가 진행되는지 확인
- Stage별 이동/이미지/사운드 연출이 정상 실행되는지 확인
- 스토리 종료 후 Fight Screen_NStage로 이동하는지 확인
- Epilogue 종료 후 Thank Screen으로 이동하는지 확인
- Skip 설정에 따라 Skip 버튼이 표시되는지 확인
```

자동 테스트 후보:

```text
- StorySceneMove가 storyNum별 다음 씬 이름을 반환하는지 확인
- StorySystemStaticRegressionTests가 View 이름과 GUID 유지 여부를 확인하는지 검증
- DialogueCsvParse가 CSV 문자열을 Dialogue 배열로 변환하는지 확인
```

## 9. BattleCore QA

수동 테스트:

```text
- 전투 씬에서 소환이 가능한지 확인
- 일반 공격으로 적 HP가 감소하는지 확인
- 특수 공격으로 Heal, Shield, Upgrade가 아군에게 적용되는지 확인
- 특수 공격으로 Damage, Poison, Stun, Curse가 적에게 적용되는지 확인
- 턴 종료 후 마나가 회복되는지 확인
- 턴 종료 후 쿨타임이 감소하는지 확인
- 상태이상이 턴마다 적용되고 만료되는지 확인
- 적 전멸 시 승리 Alert가 표시되는지 확인
- Clear Turn 초과 시 패배 Alert가 표시되는지 확인
```

자동 테스트 후보:

```text
- BattleTargetSelect가 공격 타입에 맞는 Plate 목록을 반환하는지 확인
- PlayerManaUse가 마나 부족 시 실패하는지 확인
- SummonStatusApply가 Poison, Shield, Upgrade를 적용하는지 확인
- BattleResultCheck가 승리/패배 조건을 판단하는지 확인
```

## 10. BattleContent QA

수동 테스트:

```text
- 소환 버튼 클릭 시 후보 3개가 표시되는지 확인
- 후보 3개가 중복되지 않는지 확인
- 후보 클릭 시 선택한 소환수가 Plate에 배치되는지 확인
- 재소환 시 기존 소환수가 교체되는지 확인
- 재소환 후 기존 공격 가능 상태가 유지되는지 확인
- 적 턴에서 AI가 공격을 실행하는지 확인
- AI가 Heal, Shield, Stun, Curse를 기존 조건에 맞게 선택하는지 확인
```

자동 테스트 후보:

```text
- SummonPickProbability가 Low, Medium, High 등급을 기존 확률대로 반환하는지 확인
- SummonDataApply가 능력치를 기존 값과 동일하게 적용하는지 확인
- EnemyActionDecide가 공격 명령을 반환하는지 확인
- PlayerAttackPredictionBuild가 소환수별 예측 목록을 생성하는지 확인
```

## 11. Latest Summary

2026-05-30: QA Agent를 실제 QA 실행 전담 역할로 분리했다.
리팩토링 에이전트가 작성한 테스트 코드와 QA 요청을 실행하고, 실패는 다음 리팩토링 우선순위로 기록한다.
2026-05-31: DevAgent/QAAgent 분리 운영에 맞춰 실패는 Dev Fix Request, 환경 문제는 Hold, 통과 항목은 done 보관으로 기록한다.
2026-05-31: 테스트 Passed여도 warning/error/exception 또는 자동 복구 로그가 있으면 완료로 보지 않고 Open/Hold로 분리해 기록한다.
