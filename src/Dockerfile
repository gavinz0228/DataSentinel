FROM microsoft/dotnet:2.2-sdk
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app

ENTRYPOINT ["dotnet","out/DataSentinel.dll"]
