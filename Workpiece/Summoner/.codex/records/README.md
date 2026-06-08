# Records

완료된 DEV/QA 이력을 CSV로 관리한다.

## 파일 역할

- `dev-records.csv`: 완료된 개발 이슈 인덱스
- `qa-records.csv`: 완료된 QA 이슈 인덱스

## 운영 기준

- `issue-board.md`에는 현재 이슈와 Backlog만 둔다.
- DEV 완료는 `dev-records.csv`에 한 줄 추가한다.
- QA 완료는 `qa-records.csv`에 한 줄 추가한다.
- 긴 결정 이유는 `dev-log/YYYY-MM-DD.md`에 짧게 남긴다.
- 긴 QA 상세가 필요하면 `.codex/archive/qa/`에 둔다.
