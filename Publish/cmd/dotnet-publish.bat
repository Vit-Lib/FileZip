@echo off

::���ñ����ӳ�
setlocal EnableDelayedExpansion


echo %~n0.bat start...


::(x.1)��ȡbasePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ../..
set basePath=%cd%
set publishPath=%basePath%/Publish/release/release/publish




::(x.2)����������Ҫ��������Ŀ������
for /f "delims=" %%f in ('findstr /M /s /i "<publish>" *.csproj') do (
	::get publishName
	for /f "tokens=3 delims=><" %%a in ('type "%basePath%\%%f"^|findstr "<publish>.*publish"') do set publishName=%%a

	echo publish !publishName!

	::publish
	cd /d "%basePath%\%%f\.."
	dotnet build --configuration Release
	dotnet publish --configuration Release --output "%publishPath%\!publishName!"
	@if errorlevel 1 (echo . & echo .  & echo �������Ų飡& pause) 
)






::(x.3)copy bat
xcopy "%basePath%\Publish\ReleaseFile\publish" "%publishPath%" /e /i /r /y







echo %~n0.bat ִ�гɹ���

cd /d "%curPath%"