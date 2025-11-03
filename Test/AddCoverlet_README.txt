1. Install packages: 
dotnet add package coverlet.collector
dotnet add package coverlet.msbuild

2. Running the Code Coverage:
dotnet test --collect:"XPlat Code Coverage"

Using MSBuild Integration: 
dotnet test /p:CollectCoverage=true

with output format options:
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover