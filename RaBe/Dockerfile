FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV EMAIL_SENDER="test@test.test"
ENV EMAIL_SERVER="test.com"
ENV EMAIL_PORT=25

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-stretch AS build
WORKDIR /src
COPY ["RaBe/RaBe.csproj", "RaBe/"]
RUN dotnet restore "RaBe/RaBe.csproj"
COPY . .
WORKDIR "/src/RaBe"
RUN dotnet build "RaBe.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "RaBe.csproj" -c Release -o /app
COPY ./RaBe/RaBe.db /app/RaBe.db

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RaBe.dll"]