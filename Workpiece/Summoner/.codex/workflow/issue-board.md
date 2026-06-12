# Issue Board

현재 판단이 필요한 항목만 둔다.
오래된 Done 목록은 남기지 않는다.

| ID | 상태 | 도메인 | 기능 슬라이스 | 다음 행동 | 호출 대상 |
|---|---|---|---|---|---|
| DEV-068 | Ready | Battle/EnemyAction | 예측용 얕은 복사 계약 고정 | `MemberwiseClone()` 유지와 예측 중 원본 상태 비변경 회귀 기준 제시 | DevAgent |

## 보류/제외

- 다이빙 정리만 독립 작업으로 분리하지 않는다.
- 씬, 프리팹, ScriptableObject asset 값, 저장 데이터 변경은 별도 승인 전까지 제외한다.
- Git 추적 중인 루트 로그/리포트/엑셀/업그레이드 로그 삭제는 별도 승인 전까지 제외한다.
- Git 미추적 루트 로그 `UnityTest-DEV-061.log`, `UnityTest-DEV-062.log`는 삭제를 시도했으나 현재 세션 권한/정책 문제로 남아 있다.
