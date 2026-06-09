# QA Summary - 2026-06

## 완료 QA

| ID | 상태 | 완료일 | 요약 |
|---|---|---|---|
| QA-002 | Pass | 2026-06-04 | 전투 결과 Alert 연결, 진행 저장, 다음 씬 이동 PlayMode 흐름 확인 |
| QA-003 | Pass | 2026-06-04 | MCP PlayMode 연결 유지 및 PlayMode 전체 테스트 25/25 통과 |
| QA-004 | Pass | 2026-06-04 | Only Mouse Space 차단 코드, 클릭 UnityEvent, Gameplay 토글 연결 확인 |
| QA-005 | Pass | 2026-06-04 | Story Skip 버튼, `StorySkipView.SkipBtn`, `StorySkipView.skipAlert` UnityEvent 연결 확인 |
| QA-007 | Pass | 2026-06-04 | 비디오 저장 인덱스 복구 테스트와 HUD 비디오 토글 6개 연결 확인 |
| QA-008 | Pass | 2026-06-04 | 저장 진행도와 스테이지 흐름 관련 PlayMode 테스트 통과 |
| QA-010 | Pass | 2026-06-04 | Fight 1Stage의 6종 소환수 참조, Plate 3개, Draw/ReDraw 패널 연결 확인 |
| QA-012 | Pass | 2026-06-04 | Console warning 없음, error는 MCP WebSocket만 확인 |
| QA-013 | Pass | 2026-06-05 | Slime/Skeleton SummonData 연결 확인 |
| QA-014 | Pass | 2026-06-05 | LowDevil/HighDevil SummonData 연결 확인 |
| QA-015 | Pass | 2026-06-05 | KingSlime SummonData 연결 확인 |
| QA-017 | Pass | 2026-06-06 | DEV-007B 1스테이지 PlayMode Smoke에서 Slime 2회 배치와 첫 플레이어 턴 시작 확인 |
| QA-022 | Pass | 2026-06-08 | FireSpirit SummonData 값과 prefab 참조 확인, Unity compile 0 warning, SummonDataRegressionTests 8/8 통과 |
| QA-023 | Pass | 2026-06-08 | QueenSpirit SummonData 값과 3개 특수공격 prefab 참조 확인, Unity compile 0 warning, SummonDataRegressionTests 8/8 통과 |
| QA-024 | Pass | 2026-06-08 | DarkDragon SummonData 값과 4개 특수공격 prefab 참조 확인, Unity compile 0 warning, SummonDataRegressionTests 8/8 통과 |
| QA-025 | Pass | 2026-06-08 | BattleController 특수공격 대상 plate 선택 helper 분리 확인, Unity compile 0 warning, PlateSummonDrawStaticRegressionTests 24/24 통과 |
| QA-026 | Pass | 2026-06-08 | EnermyAlgorithm 적 일반공격/강공격 helper 분리 확인, Unity compile 0 warning, PlateSummonDrawStaticRegressionTests 25/25 통과 |
| QA-027 | Pass | 2026-06-08 | EnermyAlgorithm 적 특수공격 실행/로그 helper 분리 확인, Unity compile 0 warning, PlateSummonDrawStaticRegressionTests 26/26 통과 |

## 운영 기준

- 완료 QA는 `.codex/qa/active-qa.md`에서 제거한다.
- 현재 판단에 필요한 요약만 `.codex/workflow/issue-board.md`에 남긴다.
- 상세 기록은 이 파일에 누적하거나 날짜별 파일로 나눈다.
