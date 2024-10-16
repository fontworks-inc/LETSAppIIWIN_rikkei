@echo off
REM	デバッグログ切り替え

cd /d %~dp0

whoami /priv | find "SeDebugPrivilege" > nul
if %errorlevel% neq 0 (
	@powershell start-process %~0 -verb runas
	exit
)

echo Copy _nlog.config_release
attrib -R nlog.config
copy /y _nlog.config_release nlog.config
attrib +R nlog.config

echo ログをリリースモードへ変更しました
echo LETSデスクトップアプリを再起動してください
pause