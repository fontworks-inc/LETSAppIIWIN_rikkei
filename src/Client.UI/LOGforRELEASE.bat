@echo off
REM	�f�o�b�O���O�؂�ւ�

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

echo ���O�������[�X���[�h�֕ύX���܂���
echo LETS�f�X�N�g�b�v�A�v�����ċN�����Ă�������
pause