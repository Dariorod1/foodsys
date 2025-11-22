FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE $PORT

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["backend/CafeteriaApi/CafeteriaApi.csproj", "backend/CafeteriaApi/"]
RUN dotnet restore "backend/CafeteriaApi/CafeteriaApi.csproj"
COPY . .
WORKDIR "/src/backend/CafeteriaApi"
RUN dotnet build "CafeteriaApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CafeteriaApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:$PORT
ENTRYPOINT ["dotnet", "CafeteriaApi.dll"]