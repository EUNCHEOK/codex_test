# Daily Check-In Journal

매일 짧은 개발 체크인을 남기는 가벼운 저널 프로젝트입니다.

이 저장소는 GitHub 기여 그래프를 꾸미기 위한 가짜 커밋이 아니라, 실제로 그날의 생각이나 진행 상황을 한 줄이라도 남기는 습관을 돕기 위해 사용합니다.

## 기록 방식

체크인 파일은 `logs/YYYY-MM-DD.md` 형식으로 저장합니다.

각 파일은 짧게 유지합니다.

```md
# YYYY-MM-DD

- Check-in: 오늘의 한 줄 메모
- Note: 선택 메모
```

## 새 체크인 만들기

PowerShell에서 다음 명령을 실행합니다.

```powershell
.\scripts\new-checkin.ps1 -CheckIn "오늘 한 줄 메모"
```

선택 메모가 있으면 `-Note`를 함께 사용할 수 있습니다.

```powershell
.\scripts\new-checkin.ps1 -CheckIn "작은 기능을 정리했다" -Note "내일은 README를 다듬기"
```

스크립트는 오늘 날짜의 파일이 이미 있으면 덮어쓰지 않고 멈춥니다.

## EXE로 실행하기

Windows에서 바로 실행할 수 있는 `dist/checkin.exe`를 사용할 수 있습니다. 파일을 더블클릭하면 작은 입력 창이 열립니다.

창에서 오늘의 한 줄 체크인과 선택 메모를 입력한 뒤 `Save & Commit`을 누르면 `logs/YYYY-MM-DD.md` 파일이 생성되고, 해당 파일만 자동으로 커밋됩니다. 입력칸에는 예시 문구가 회색으로 표시되고, 칸을 누르면 사라집니다. 이미 오늘 파일이 있으면 덮어쓰지 않습니다.

EXE를 다시 빌드하려면 다음 명령을 실행합니다.

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\build-exe.ps1
```

## 커밋 규칙

EXE는 체크인 파일을 만든 뒤 다음 형식으로 자동 커밋합니다.

```powershell
git add logs/YYYY-MM-DD.md
git commit -m "Add check-in for YYYY-MM-DD"
```

## Daily Reminder

Codex가 매일 오후 6시(Asia/Seoul)에 이 스레드에서 체크인 알림을 보냅니다.

자동화는 파일을 임의로 만들거나 커밋하지 않습니다. 사용자가 그날의 메모를 입력하거나 승인한 뒤에만 기록을 남깁니다.
