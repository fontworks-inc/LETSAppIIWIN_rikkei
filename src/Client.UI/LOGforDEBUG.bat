@echo off
REM	デバッグログ切り替え

cd /d %~dp0

whoami /priv | find "SeDebugPrivilege" > nul
if %errorlevel% neq 0 (
	@powershell start-process %~0 -verb runas
	exit
)

echo Copy _nlog.config_debug
attrib -R nlog.config
copy /y _nlog.config_debug nlog.config
attrib +R nlog.config

echo ログをデバッグモードへ変更しました
echo LETSデスクトップアプリを再起動してください
pause