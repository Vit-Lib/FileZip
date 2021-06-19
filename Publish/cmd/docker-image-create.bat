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
set dockerPath=%basePath%/Publish/release/release/docker-image


rd /s /q "%dockerPath%"

::(x.2)copy dir
xcopy "%basePath%/Publish/ReleaseFile/docker-image" "%dockerPath%" /e /i /r /y



::(x.3)查找所有需要发布的项目并发布
for /f "delims=" %%f in ('findstr /M /s /i "<docker>" *.csproj') do (
	::get publishName
	for /f "tokens=3 delims=><" %%a in ('type "%basePath%\%%f"^|findstr "<publish>.*publish"') do set publishName=%%a
	::get dockerName
	for /f "tokens=3 delims=><" %%a in ('type "%basePath%\%%f"^|findstr "<docker>.*docker"') do set dockerName=%%a

	echo create !dockerName!

	::copy file
	xcopy "%publishPath%/!publishName!" "%dockerPath%/!dockerName!/app" /e /i /r /y
)





echo %~n0.bat 执行成功！

cd /d "%curPath%"