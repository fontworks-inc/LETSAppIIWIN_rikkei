@echo OFF

REM 一括ビルドバッチ
REM	BatchBuild.bat <バージョン番号>
REM	引数でバージョン番号を指定 (引数で指定しない場合は入力してください)
REM	※Developer Command Prompt for VS2019 で実行してください
REM LETS-VerX.X.Xフォルダを作成し、その中に LETS-Installer.zip、LETS-Ver1.0.0.zip を出力します。

if "%1" == "" (
	set /P Version="バージョン番号(X.X.X)を入力してください："
) else (
	set Version=%1
)

set VersionFolder=LETS-Ver%Version%

REM バージョンフォルダが存在しなければ作成する
if not exist %VersionFolder%\ ( 
	MD %VersionFolder%
)

REM バージョン番号編集
REM Client.UIバージョン編集(LETS.exeのバージョン)
ClientUIVersionReplace.vbs ..\Client.UI\Client.UI.csproj %Version%

REM Installerプロジェクト バージョン編集(インストーラのバージョンおよびインストール先フォルダ名)
LETS-InstallerVersionReplace.vbs ..\LETS-Installer\LETS-Installer.vdproj %Version%

REM ビルド実行
set LogFile=%VersionFolder%\build.log
devenv ..\LETS-Installer\LETS-Installer.vdproj /Clean "Release|x86" /out %LogFile%
devenv ..\LETS-Installer\LETS-Installer.vdproj /Rebuild "Release|x86" /out %LogFile%

if %ERRORLEVEL% neq 0 (
	echo ビルドでエラーが発生しました。
	goto ERROREXIT
)

REM Installerファイル作成
powershell Compress-Archive -Path ..\LETS-Installer\Release\LETS-Installer.msi,..\LETS-Installer\setup.exe -DestinationPath %VersionFolder%\LETS-Installer.zip -Force

REM Updateファイル作成
powershell Compress-Archive -Path ..\Client.UI\bin\Release\netcoreapp3.1\publish\* -DestinationPath %VersionFolder%\LETS-Ver%Version%.zip -Force


REM 完全オフラインスイッチ編集
OfflineSwitchReplace.vbs ..\Infrastructure\Infrastructure.csproj
OfflineSwitchReplace.vbs ..\LETSUpdater\LETSUpdater.csproj

REM ビルド実行
set LogFile=%VersionFolder%\build.log
REM	devenv ..\LETS-Installer\LETS-Installer.vdproj /Clean "Release|x86" /out %LogFile%
REM	devenv ..\LETS-Installer\LETS-Installer.vdproj /Rebuild "Release|x86" /out %LogFile%
devenv ..\LETS-Installer\LETS-Installer.vdproj /Build "Release|x86" /out %LogFile%

if %ERRORLEVEL% neq 0 (
	echo ビルドでエラーが発生しました。
	goto ERROREXIT
)

move /y ..\Infrastructure\Infrastructure.csproj.bak ..\Infrastructure\Infrastructure.csproj
move /y ..\LETSUpdater\LETSUpdater.csproj.bak ..\LETSUpdater\LETSUpdater.csproj

REM Installerファイル作成
powershell Compress-Archive -Path ..\LETS-Installer\Release\LETS-Installer.msi,..\LETS-Installer\setup.exe -DestinationPath %VersionFolder%\LETS-device-Installer.zip -Force

REM Updateファイル作成
powershell Compress-Archive -Path ..\Client.UI\bin\Release\netcoreapp3.1\publish\* -DestinationPath %VersionFolder%\LETS-device-Ver%Version%.zip -Force


:NORMALEND
EXIT /B 0

:ERROREXIT
EXIT /B -1
