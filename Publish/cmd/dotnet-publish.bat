@echo off

::启用变量延迟
setlocal EnableDelayedExpansion


echo %~n0.bat start...


::(x.1)获取basePath
set curPath=%cd%
cd /d "%~dp0"
cd /d ../..
set basePath=%cd%
set publishPath=%basePath%/Publish/release/release/publish




::(x.2)查找所有需要发布的项目并发布
for /f "delims=" %%f in ('findstr /M /s /i "<publish>" *.csproj') do (
	::get publishName
	for /f "tokens=3 delims=><" %%a in ('type "%basePath%\%%f"^|findstr "<publish>.*publish"') do set publishName=%%a

	echo publish !publishName!

	::publish
	cd /d "%basePath%\%%f\.."
	dotnet build --configuration Release
	dotnet publish --configuration Release --output "%publishPath%\!publishName!"
	@if errorlevel 1 (echo . & echo .  & echo 出错，请排查！& pause) 
)






::(x.3)copy bat
xcopy "%basePath%\Publish\ReleaseFile\publish" "%publishPath%" /e /i /r /y







echo %~n0.bat 执行成功！

cd /d "%curPath%"