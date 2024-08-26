@echo off

dotnet build

REM Run the MatchmakingServer from the current working directory
start cmd /k "cd /d %cd%\MatchmakingServer\src && dotnet run"

REM Run the HostServer from the current working directory
start cmd /k "cd /d %cd%\HostServer\src && dotnet run"

REM Exit the original command prompt
exit
