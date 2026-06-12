# QA

이 폴더는 현재 개발 판단에 영향을 주는 QA만 관리한다.

완료된 QA 상세 기록은 이 폴더에 오래 두지 않고 `.codex/archive/qa/`로 옮긴다.

## 파일 역할

- `active-qa.md`: 진행 중, 실패, 보류, 재확인이 필요한 QA
- `regression-checklist.md`: 반복 검증에 사용하는 회귀 체크리스트

## 운영 기준

- 통과한 QA 상세는 `active-qa.md`에서 제거한다.
- 현재 판단에 필요한 통과 요약만 `.codex/workflow/issue-board.md`에 남긴다.
- 완료된 QA 상세는 `.codex/archive/qa/`에 보관한다.
- QA 완료 요약은 필요할 때 `.codex/dev-log/YYYY-MM-DD.md`에 1~3줄만 남긴다.
- 코드 실패와 도구/환경 실패를 분리해서 기록한다.
- QA 실행, 판정, 설계 검토는 필요할 때 호출된 VerificationAgent가 mode를 나눠 담당한다.
- VerificationAgent는 상시 단계가 아니라 기능 게이트나 검증 요청 때만 호출한다.

