@echo OFF
REM アップデータ作成バッチ
REM updaterzip.bat <バージョン>
set /P Version="バージョン番号(X.X.X)を入力してください："
powershell Compress-Archive -Path ..\Client.UI\bin\Release\netcoreapp3.1\publish\* -DestinationPath LETS-Ver%Version%.zip -Force