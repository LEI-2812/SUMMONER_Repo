# Agent Status

다음에 실행할 에이전트 하나만 `Ready`로 둔다.

| Agent | 상태 | 현재 초점 | 다음 행동 |
|---|---|---|---|
| CoordinatorAgent | Waiting | RUNTIME-TRIAGE-08 완료 | 직접 플레이 검증 뒤 새 오류가 있으면 다음 후보 선정 |
| DevAgent | Waiting | 수정 후보 대기 | 코드-only 수정 후보가 확정되면 Change Proposal 제시 |
| VerificationAgent | Ready | RUNTIME-QA-13 직접 플레이 스모크 검증 | Start Screen부터 FightScene 1~2 소환/재소환 흐름과 콘솔 error/warning 확인 |
| DocumentationAgent | Waiting | 최근 완료 기록 정리 대기 | 기능 완료 또는 handoff 필요 시 호출 |
| ReleaseAgent | Waiting | 대기 | 릴리즈 요청 시 사용 |

## 운영 메모

- CoordinatorAgent는 `refactor-candidates.md`에서 Ready 하나만 골라 DevAgent로 넘긴다.
- DevAgent는 코드 변경 전 적용 예정 diff를 먼저 보여주고, 사용자가 승인한 자동 진행 범위 안에서 적용한다.
- 이번 방향은 새 책임 분리가 아니라 얇은 래퍼 축소와 public 계약 축소를 우선한다.
- Summon은 대분리하지 않고, enum 의존과 중복 fallback 초기화부터 줄인다.
- COORD-165는 Story Screen 1/2/3/5/7의 `StorySkipView` 버튼 이벤트 메서드명을 현재 public 메서드 `SkipAlert`에 맞추는 연결 수습이다.
- COORD-165는 적용 완료했다. `StorySystemStaticRegressionTests`는 26/26 통과했다.
- COORD-SCAN은 완료했다. Ready 후보는 BTL-SCENE-03 하나만 둔다.
- BTL-SCENE-03은 처리했다. `StageSceneConnectionEditModeTests` 7/7, 전체 EditMode 146/146, 전체 PlayMode 26/26 통과.
- SCENE-NULL-01은 처리했다. `Grass Spirit.prefab`, `HighDevil.prefab`의 기존 `ShieldImage` GameObject를 `Summon.shieldImage`에 연결했고, `StageSceneConnectionEditModeTests` 9/9, 전체 EditMode 148/148, 전체 PlayMode 26/26 통과.
- BTL-RUNTIME-01은 처리했다. `BattleProgressController`의 runtime `AddComponent<BattleStageContext>()` fallback을 제거했고, FightScene 1~7 battle runtime controller 필수 연결 계약을 추가했다. 전체 EditMode 149/149, 전체 PlayMode 26/26 통과.
- RUNTIME-WIRING-02는 처리했다. 시작/이어하기, HUD, Stage Select, Fight, Story/Thank 화면의 runtime wiring 오류와 콘솔 warning을 수습했고 전체 EditMode 155/155, 전체 PlayMode 28/28, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-03은 처리했다. Start Screen 설정 버튼, Story ESC, Fight 결과창 시작 비활성화, 일반 소환 3옵션 fallback과 `Summon.GetImage()` prefab 지연 확보를 수습했고 전체 EditMode 155/155, 전체 PlayMode 32/32, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-05는 처리했다. 1Stage 전투 UI 배치를 2Stage 기준으로 정리했고, 2~7Stage 소환 선택 UI zero-scale과 PlayerPlateActions 시작 활성 차이를 수습했다. `StageSceneConnectionEditModeTests` 15/15, 2Stage PlayMode 소환 UI/턴 교환 테스트는 통과했다.
- RUNTIME-HOTFIX-06은 처리했다. `SummonStatusView`가 상태색/피격색을 덮기 전 원래 Image 색상을 캐싱하고 상태 해제 시 원래 색으로 복구한다. `SummonStatusViewTests` 12/12 통과, Unity Console error/warning 0건을 확인했다.
- RUNTIME-QA-07은 처리했다. `StageRuntimeFlowPlayModeTests` 17/17, 전체 PlayMode 34/34, 전체 EditMode 158/158 통과했고 Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-09는 처리했다. FightScene 1~7의 소환 선택 옵션 판넬을 `380x550`으로 줄였고, 적 배치 직후 현재 전투 배수를 적 HP/공격력에 적용한다. 전체 PlayMode 35/35, 전체 EditMode 158/158, Unity Console error/warning 0건을 확인했다.
- RUNTIME-HOTFIX-10은 처리했다. FightScene 1~7의 소환/재소환 3옵션 `RedrawPanel`을 3열 고정 GridLayout으로 맞추고 1Stage `TurnTextUI` 프레임 배치를 수습했다. Unity 실행은 `LicenseClient` IPC timeout으로 결과 XML 없이 중단됐고, 정적 scene YAML 확인과 `git diff --check`만 완료했다.
- RUNTIME-HOTFIX-11은 처리했다. FightScene 1~7의 1개짜리 `DrawPanel/DrawOption_1` 경로와 `SummonController`의 `drawPanel/drawOptionPanels` 직렬화 경로를 제거하고, 일반 소환과 재소환 모두 공용 `RedrawPanel` 3옵션으로 통일했다. Unity 실행은 `LicenseClient` IPC timeout으로 결과 XML 없이 중단됐고, 정적 scene YAML 확인과 `git diff --check`만 완료했다.
- RUNTIME-HOTFIX-12는 처리했다. FightScene 1~7의 Unity YAML 헤더 누락을 복구했고, Unity MCP `load_scene`으로 1~7Stage 모두 정상 로드되는 것을 확인했다.
- RUNTIME-TRIAGE-08은 처리했다. `StageSceneConnectionEditModeTests` 15/15, `PlateSummonDrawStaticRegressionTests` 36/36, `StageRuntimeFlowPlayModeTests` 19/19 통과 및 Unity Console error/warning 0건을 확인했다.
- 현재 반복 루프는 RUNTIME-QA-13이다. 자동 테스트로 닫힌 항목을 실제 플레이 흐름에서 다시 확인하고, 재현되는 오류가 있으면 코드/씬/도구 문제로 분류한다.
- 테스트 실행은 자동 진행한다. 테스트 결과 확인 뒤 10분 동안 사용자 응답이 없으면 코드/씬/에셋 수정이 필요 없는 다음 테스트 또는 문서/후보 정리 작업으로 이어간다.
- `dotnet build .\Assembly-CSharp.csproj --no-restore`는 기존 `NETSDK1004` project.assets.json 누락으로 중단됐다.
- AttackRule 미세정리는 같은 기능 반복 제한 규칙에 따라 다음 작업으로 잡지 않는다.
- 문서 상태 기록은 사용자 승인 없이 최소 범위로 바로 갱신한다.
