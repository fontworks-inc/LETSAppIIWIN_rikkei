@echo OFF

REM �ꊇ�r���h�o�b�`
REM	BatchBuild.bat <�o�[�W�����ԍ�>
REM	�����Ńo�[�W�����ԍ����w�� (�����Ŏw�肵�Ȃ��ꍇ�͓��͂��Ă�������)
REM	��Developer Command Prompt for VS2019 �Ŏ��s���Ă�������
REM LETS-VerX.X.X�t�H���_���쐬���A���̒��� LETS-Installer.zip�ALETS-Ver1.0.0.zip ���o�͂��܂��B

if "%1" == "" (
	set /P Version="�o�[�W�����ԍ�(X.X.X)����͂��Ă��������F"
) else (
	set Version=%1
)

set VersionFolder=LETS-Ver%Version%

REM �o�[�W�����t�H���_�����݂��Ȃ���΍쐬����
if not exist %VersionFolder%\ ( 
	MD %VersionFolder%
)

REM �o�[�W�����ԍ��ҏW
REM Client.UI�o�[�W�����ҏW(LETS.exe�̃o�[�W����)
ClientUIVersionReplace.vbs ..\Client.UI\Client.UI.csproj %Version%

REM Installer�v���W�F�N�g �o�[�W�����ҏW(�C���X�g�[���̃o�[�W��������уC���X�g�[����t�H���_��)
LETS-InstallerVersionReplace.vbs ..\LETS-Installer\LETS-Installer.vdproj %Version%

REM �r���h���s
set LogFile=%VersionFolder%\build.log
devenv ..\LETS-Installer\LETS-Installer.vdproj /Clean "Release|x86" /out %LogFile%
devenv ..\LETS-Installer\LETS-Installer.vdproj /Rebuild "Release|x86" /out %LogFile%

if %ERRORLEVEL% neq 0 (
	echo �r���h�ŃG���[���������܂����B
	goto ERROREXIT
)

REM Installer�t�@�C���쐬
powershell Compress-Archive -Path ..\LETS-Installer\Release\LETS-Installer.msi,..\LETS-Installer\setup.exe -DestinationPath %VersionFolder%\LETS-Installer.zip -Force

REM Update�t�@�C���쐬
powershell Compress-Archive -Path ..\Client.UI\bin\Release\netcoreapp3.1\publish\* -DestinationPath %VersionFolder%\LETS-Ver%Version%.zip -Force


REM ���S�I�t���C���X�C�b�`�ҏW
OfflineSwitchReplace.vbs ..\Infrastructure\Infrastructure.csproj
OfflineSwitchReplace.vbs ..\LETSUpdater\LETSUpdater.csproj

REM �r���h���s
set LogFile=%VersionFolder%\build.log
REM	devenv ..\LETS-Installer\LETS-Installer.vdproj /Clean "Release|x86" /out %LogFile%
REM	devenv ..\LETS-Installer\LETS-Installer.vdproj /Rebuild "Release|x86" /out %LogFile%
devenv ..\LETS-Installer\LETS-Installer.vdproj /Build "Release|x86" /out %LogFile%

if %ERRORLEVEL% neq 0 (
	echo �r���h�ŃG���[���������܂����B
	goto ERROREXIT
)

move /y ..\Infrastructure\Infrastructure.csproj.bak ..\Infrastructure\Infrastructure.csproj
move /y ..\LETSUpdater\LETSUpdater.csproj.bak ..\LETSUpdater\LETSUpdater.csproj

REM Installer�t�@�C���쐬
powershell Compress-Archive -Path ..\LETS-Installer\Release\LETS-Installer.msi,..\LETS-Installer\setup.exe -DestinationPath %VersionFolder%\LETS-device-Installer.zip -Force

REM Update�t�@�C���쐬
powershell Compress-Archive -Path ..\Client.UI\bin\Release\netcoreapp3.1\publish\* -DestinationPath %VersionFolder%\LETS-device-Ver%Version%.zip -Force


:NORMALEND
EXIT /B 0

:ERROREXIT
EXIT /B -1
