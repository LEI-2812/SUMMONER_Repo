# BattleCore QA

## 1. 현재 상태 요약

| 항목 | 내용 |
|---|---|
| QA 범위 | 턴 시작, 턴 종료, 마나, 공격, 상태이상, 승패 판정 |
| 현재 상태 | Alert Inspector 정적 확인 완료, 승리 판정 런타임 버그 후보 발견 |
| 마지막 정리일 | 2026-05-31 |
| 다음 확인 | 턴 시작과 턴 종료 흐름부터 확인 |

## 2. QA 체크 항목

| ID | 기능 | 확인 내용 | 상태 |
|---|---|---|---|
| BC-001 | 턴 시작 | 상태이상이 턴 시작 시 갱신되는가 | Pending |
| BC-002 | 턴 시작 | 쿨타임이 턴 시작 시 감소하는가 | Pending |
| BC-003 | 턴 종료 | 턴 종료 후 마나가 회복되는가 | Pending |
| BC-004 | 일반 공격 | 공격 후 대상 HP가 감소하는가 | Pending |
| BC-005 | 일반 공격 | 적 사망 후 Plate가 정렬되는가 | Pending |
| BC-006 | 특수 공격 | Heal과 Damage 대상 선택이 구분되는가 | Pending |
| BC-007 | 상태이상 | Poison이 턴마다 HP를 감소시키는가 | Pending |
| BC-008 | 상태이상 | Shield가 방어막 수치를 증가시키는가 | Pending |
| BC-009 | 상태이상 | Stun과 Curse가 공격 가능 여부에 영향을 주는가 | Pending |
| BC-010 | 승패 판정 | 적 전멸 시 승리 Alert가 표시되는가 | Fail |
| BC-011 | 승패 판정 | Clear Turn 초과 시 패배 Alert가 표시되는가 | Fixed |

## 3. 상세 QA 기록

### BC-001. 턴 시작 - 상태이상 갱신

- 우선순위: 높음
- 확인 내용: 턴 시작 시 Poison, Shield, Stun, Curse 같은 상태가 갱신되는지 확인
- 기대 결과: 상태이상이 정해진 규칙대로 적용되거나 만료된다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: 턴 시작 테스트 작성
- 관련 파일: TurnController.cs

---

### BC-002. 턴 시작 - 쿨타임 감소

- 우선순위: 높음
- 확인 내용: 턴 시작 시 소환수 또는 스킬 쿨타임 값이 감소하는지 확인
- 기대 결과: 쿨타임이 1씩 감소하고 0 미만으로 내려가지 않는다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: 쿨타임 값 변화 확인
- 관련 파일: TurnController.cs

---

### BC-003. 턴 종료 - 마나 회복

- 우선순위: 높음
- 확인 내용: 플레이어 턴 종료 후 마나가 회복되는지 확인
- 기대 결과: 턴 종료 후 마나가 규칙에 맞게 증가한다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: 턴 종료 후 마나 확인
- 관련 파일: Player.cs

---

### BC-004. 일반 공격 - HP 감소

- 우선순위: 높음
- 확인 내용: 일반 공격 실행 후 대상 HP가 감소하는지 확인
- 기대 결과: 공격력만큼 대상 HP가 감소하고 UI가 갱신된다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: 일반 공격 결과 확인
- 관련 파일: BattleController.cs

---

### BC-005. 일반 공격 - Plate 정렬

- 우선순위: 중간
- 확인 내용: 적 사망 후 Plate 정렬이 정상 동작하는지 확인
- 기대 결과: 빈 Plate가 정리되고 남은 대상이 정상 배치된다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: 적 사망 후 Plate 정렬 확인
- 관련 파일: PlateController.cs

---

### BC-006. 특수 공격 - 대상 선택

- 우선순위: 높음
- 확인 내용: Heal, Damage 같은 특수 공격의 대상 선택이 구분되는지 확인
- 기대 결과: 회복은 아군, 피해는 적군을 대상으로 선택한다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: Heal/Damage 대상 구분 확인
- 관련 파일: BattleController.cs

---

### BC-007. 상태이상 - Poison 적용

- 우선순위: 높음
- 확인 내용: Poison 상태가 턴마다 HP를 감소시키는지 확인
- 기대 결과: Poison이 적용된 대상의 HP가 턴마다 감소한다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: 턴마다 HP 감소 확인
- 관련 파일: Summon.cs

---

### BC-008. 상태이상 - Shield 적용

- 우선순위: 높음
- 확인 내용: Shield 상태가 방어막 수치를 증가시키는지 확인
- 기대 결과: Shield 적용 후 피해를 먼저 방어막으로 막는다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: 방어막 증가 확인
- 관련 파일: Summon.cs

---

### BC-009. 상태이상 - Stun/Curse 적용

- 우선순위: 중간
- 확인 내용: Stun과 Curse가 공격 가능 여부 또는 공격 결과에 영향을 주는지 확인
- 기대 결과: 상태이상 규칙에 따라 공격 가능 여부가 달라진다
- 현재 결과: 아직 QA 전
- 상태: Pending
- 다음 조치: 공격 가능 여부 확인
- 관련 파일: Summon.cs

---

### BC-010. 승패 판정 - 승리 Alert

- 우선순위: 높음
- 확인 내용: 적이 모두 사망했을 때 승리 Alert가 표시되는지 확인
- 기대 결과: 승리 Alert가 표시되고 다음 진행이 가능하다
- 현재 결과: Alert prefab과 BattleResultAlertView 필드 연결은 확인됨. 다만 TurnController가 전투씬에 없는 StageController를 참조해 승리 Alert 호출 전에 NullReference가 발생할 수 있음
- 상태: Fail
- 다음 조치: TurnController의 StageController 의존 제거 후 적 전멸 시 Alert 표시 재확인
- 관련 파일: TurnController.cs, BattleResultAlertView.cs

---

### BC-011. 승패 판정 - 패배 Alert

- 우선순위: 높음
- 확인 내용: Clear Turn을 초과했을 때 패배 Alert가 표시되는지 확인
- 기대 결과: 패배 Alert가 표시되고 재시도 또는 이동 흐름이 동작한다
- 현재 결과: Alert prefab과 BattleResultAlertView 필드 연결은 정적 확인됨. Clear Turn 초과 런타임 흐름은 재확인이 필요함
- 상태: Fixed
- 다음 조치: Clear Turn 초과 상황에서 패배 Alert 표시와 Stage Select 이동 확인
- 관련 파일: BattleResultAlertView.cs

## 4. 발견한 버그

| ID | 날짜 | 내용 | 상태 |
|---|---|---|---|
| BUG-BC-001 | 2026-05-31 | TurnController가 전투씬에 없는 StageController를 참조해 승리 판정 시 NullReference가 발생할 수 있음 | Open |

## 5. QA 히스토리

| 날짜 | 확인 범위 | 결과 | 메모 |
|---|---|---|---|
| 2026-05-31 | BattleCore QA 항목 정리 | 대기 | 실제 QA는 아직 진행 전 |
| 2026-05-31 | BattleResult Alert 정적 재검토 | 부분 실패 | Inspector 참조는 정상, TurnController 승리 판정 경로에 NullReference 후보 발견 |
