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
| BC-010 | 승패 판정 | 적 전멸 시 승리 Alert가 표시되는가 | Fixed |
| BC-011 | 승패 판정 | Clear Turn 초과 시 패배 Alert가 표시되는가 | Fixed |
| BC-012 | 상태이상 표시 | 소환수 이미지, 색, 효과음이 분리된 View에서 처리되는가 | Fail |

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
- 현재 결과: DevAgent가 `BattleResultController`, `BattleProgressController`, `BattleStageContext`를 추가하고 `TurnController`의 `StageController` 의존을 제거했다. QAAgent 재확인에서 MCP `recompile_scripts` warning 0, `Summoner.EditModeTests.StageSceneConnectionEditModeTests` 5/5 통과. PlayMode `Summoner.PlayModeTests.StageRuntimeFlowPlayModeTests.BattleResultAlertView_FightScene_HasRequiredRuntimeControllers`는 timeout, Console 로그 조회도 timeout으로 런타임 승리 Alert 확인은 차단됨.
- 상태: Fixed/Blocked
- 다음 조치: DevAgent가 `BUG-BC-002`의 `Summon.Update()` NRE 의심 원인을 수정한 뒤 QAAgent가 Fight Screen에서 적 전멸 시 승리 Alert 표시, retry, 다음 스테이지 이동 및 저장 진행을 런타임 재검증
- 관련 파일: TurnController.cs, Player.cs, BattleResultController.cs, BattleProgressController.cs, BattleStageContext.cs, BattleResultAlertView.cs, BattleResultFlowStaticRegressionTests.cs

---

### BC-011. 승패 판정 - 패배 Alert

- 우선순위: 높음
- 확인 내용: Clear Turn을 초과했을 때 패배 Alert가 표시되는지 확인
- 기대 결과: 패배 Alert가 표시되고 재시도 또는 이동 흐름이 동작한다
- 현재 결과: Alert prefab과 BattleResultAlertView 필드 연결은 정적 확인됨. Clear Turn 초과 런타임 흐름은 재확인이 필요함
- 상태: Fixed
- 다음 조치: Clear Turn 초과 상황에서 패배 Alert 표시와 Stage Select 이동 확인
- 관련 파일: BattleResultAlertView.cs

---

### BC-012. 상태이상 표시 - 소환수 피드백 분리

- 우선순위: 중간
- 확인 내용: 소환수 이미지 접근, 상태 색, 효과음이 상태 타입/행동별로 분리된 View에서 처리되는지 확인
- 기대 결과: 이미지와 sprite 접근은 `SummonImageView`, 상태/순간 색은 `SummonStatusView`, 공격/버프/디버프 효과음 재생은 `SummonSoundView`가 담당한다
- 현재 결과: EditMode 구조 테스트는 통과하지만 PlayMode 전투 테스트 재시도 중 `Summon.Update()`에서 `statusView` null로 보이는 `NullReferenceException` 발생. `Summon.Awake()`가 `private`이고 일부 파생 소환수 클래스가 자체 `private Awake()`를 선언해 base 초기화가 실행되지 않는 구조가 의심됨
- 상태: Fail
- 다음 조치: DevAgent가 `Summon` View 초기화 경로를 파생 클래스 Awake와 충돌하지 않게 수정한 뒤 PlayMode 전투 테스트 재실행
- 관련 파일: SummonImageView.cs, SummonStatusView.cs, SummonSoundView.cs, StatusEffectController.cs, SummonStatusViewTests.cs, SummonFeedbackSeparationTests.cs

## 4. 발견한 버그

| ID | 날짜 | 내용 | 상태 |
|---|---|---|---|
| BUG-BC-001 | 2026-05-31 | TurnController가 전투씬에 없는 StageController를 참조해 승리 판정 시 NullReference가 발생할 수 있음. 구조 수정 후 정적 연결은 통과했으나 PlayMode timeout으로 런타임 재검증은 차단됨 | Fixed/Blocked |
| BUG-BC-002 | 2026-05-31 | PlayMode 전투 진입 중 `Summon.Update()` line 77에서 `NullReferenceException`. 파생 소환수 `Awake()`가 base `Summon.Awake()` 초기화를 우회해 `SummonStatusView` 참조가 null이 되는 것으로 의심됨 | Open Fail |

## Dev Fix Request

ID: BUG-BC-002
상태: Fail
재현 조건:
- MCP PlayMode 테스트 실행
- `Summoner.PlayModeTests.StageRuntimeFlowPlayModeTests.BattleResultAlertView_FightScene_HasRequiredRuntimeControllers`

실행한 테스트:
- `Summoner.PlayModeTests.StageRuntimeFlowPlayModeTests.StartScreen_LoadsSaveAndStageFlowControllers`
- `Summoner.PlayModeTests.StageRuntimeFlowPlayModeTests.BattleResultAlertView_FightScene_HasRequiredRuntimeControllers`
- `Summoner.EditModeTests.StageSceneConnectionEditModeTests`

기대 결과:
- Fight Screen_1Stage로 진입하고 `BattleResultAlertView`, `GameSaveController`, `StageFlowController`가 런타임에서 정상 확인된다.

실제 결과:
- PlayMode 전투 테스트가 timeout 또는 `Connection failed`로 끝나고, Unity Console에 `NullReferenceException`이 기록된다.
- EditMode `StageSceneConnectionEditModeTests`는 5/5 통과한다.

실패 메시지 또는 로그:
- `NullReferenceException: Object reference not set to an instance of an object`
- `Summon.Update () (at Assets/Script/Summons/Summon.cs:77)`

의심 파일:
- `Assets/Script/Summons/Summon.cs`
- `Assets/Script/Summons/Slime.cs`
- `Assets/Script/Summons/WaterSpirit.cs`
- `Assets/Script/Summons/FireSpirit.cs`
- `Assets/Script/Summons/GrassSpirit.cs`
- `Assets/Script/Summons/QueenSpirit.cs`
- `Assets/Script/Summons/KingSlime.cs`
- `Assets/Script/Summons/Skeleton.cs`
- `Assets/Script/Summons/LowDevil.cs`
- `Assets/Script/Summons/HighDevil.cs`
- `Assets/Script/Summons/DarkDragon.cs`

수정 방향:
- `Summon`의 `imageView`, `statusView`, `soundView`, `statusEffectController` 초기화가 모든 파생 소환수에서 보장되게 한다.
- 후보: `Summon.Awake()`를 `protected virtual`로 변경하고 파생 클래스 `Awake()`에서 `base.Awake()` 호출, 또는 `Summon.Update()`/View 접근 메서드에서 공통 초기화 보장 메서드 호출.
- 수정 시 파생 클래스별 스탯 초기화 순서와 `nowHP = maxHP` 흐름이 유지되어야 한다.

재검증 방법:
- MCP `recompile_scripts` warning 0
- MCP `Summoner.EditModeTests.StageSceneConnectionEditModeTests` 통과
- MCP PlayMode `Summoner.PlayModeTests.StageRuntimeFlowPlayModeTests.BattleResultAlertView_FightScene_HasRequiredRuntimeControllers` 재실행
- Unity Console에 `Summon.Update()` NRE가 없는지 확인

## 5. QA 히스토리

| 날짜 | 확인 범위 | 결과 | 메모 |
|---|---|---|---|
| 2026-05-31 | BattleCore QA 항목 정리 | 대기 | 실제 QA는 아직 진행 전 |
| 2026-05-31 | BattleResult Alert 정적 재검토 | 부분 실패 | Inspector 참조는 정상, TurnController 승리 판정 경로에 NullReference 후보 발견 |
| 2026-05-31 | 소환수 상태 피드백 분리 EditMode 확인 | 조건부 통과 | 이미지/색/효과음/상태 리스트 분리 자동 테스트와 Prefab 참조 마이그레이션 및 브리지 필드 제거 완료, EditMode 전체 테스트 통과, 실제 전투씬 표시는 런타임 재확인 필요 |
| 2026-05-31 | QA 인덱스 재확인 | 실패 후보 유지 | `TurnController`가 `StageController`를 찾아 `stageController.stageNum`을 넘기는 경로가 남아 있어 전투씬 승리 판정 NullReference 위험 유지 |
| 2026-05-31 | BattleResult 구조 분리 Dev 수정 | Fixed | `BattleResultAlertView` UI 전용화, `BattleResultController`/`BattleProgressController`/`BattleStageContext` 추가, `TurnController`의 `StageController` 의존 제거. Unity recompile warning 0, EditMode 55/55 passed. QAAgent 런타임 재검증 필요 |
| 2026-05-31 | SceneObject PlayMode 재시도 중 전투 진입 확인 | 실패 | `Summon.Update()` line 77 NRE 발생. `BUG-BC-002` Dev Fix Request 작성 |
| 2026-05-31 | BUG-BC-001 QAAgent 재검증 | 차단 | MCP `recompile_scripts` warning 0, `StageSceneConnectionEditModeTests` 5/5 통과. PlayMode `BattleResultAlertView_FightScene_HasRequiredRuntimeControllers`는 timeout, Console 로그 조회도 timeout. `BUG-BC-002` 수정 후 재검증 필요 |
