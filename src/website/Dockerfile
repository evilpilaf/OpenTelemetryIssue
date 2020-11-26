FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/website/otel-test-1.csproj", "src/website/"]
RUN dotnet restore "src/website/otel-test-1.csproj"
COPY . .
WORKDIR "/src/src/website"
RUN dotnet build "otel-test-1.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "otel-test-1.csproj" -c Release -o /app/publish --runtime linux-x64 --self-contained false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "otel-test-1.dll"]
