:start
@echo off
SharpCry.exe --debug
set /a hexExit=errorlevel
echo [+] Program Exit Code: 0x%hexExit%
pause