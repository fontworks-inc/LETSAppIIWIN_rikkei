@echo OFF
REM �A�b�v�f�[�^�쐬�o�b�`
REM updaterzip.bat <�o�[�W����>
set /P Version="�o�[�W�����ԍ�(X.X.X)����͂��Ă��������F"
powershell Compress-Archive -Path ..\Client.UI\bin\Release\netcoreapp3.1\publish\* -DestinationPath LETS-Ver%Version%.zip -Force