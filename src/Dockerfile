FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
FROM microsoft/dotnet:2.2-sdk
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

ENTRYPOINT ["dotnet","out/DataSentinel.dll"]
