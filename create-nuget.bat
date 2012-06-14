@ECHO OFF

nuget pack SQUnit\SQUnit.csproj -Build -Sym -Prop Configuration=Release
IF ERRORLEVEL 1 GOTO error

echo Done.
echo DO NOT FORGET TO INCREMENT THE VERSION IN SQUnit\Properties\AssemblyInfo.cs AND COMMIT THE CHANGE TO GIT
GOTO end

:error
ECHO Failed.
GOTO end

:end
pause