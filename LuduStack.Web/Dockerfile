#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["LuduStack.Web/LuduStack.Web.csproj", "LuduStack.Web/"]
COPY ["LuduStack.Application/LuduStack.Application.csproj", "LuduStack.Application/"]
COPY ["LuduStack.Domain/LuduStack.Domain.csproj", "LuduStack.Domain/"]
COPY ["LuduStack.Domain.Core/LuduStack.Domain.Core.csproj", "LuduStack.Domain.Core/"]
COPY ["LuduStack.Infra.Data/LuduStack.Infra.Data.csproj", "LuduStack.Infra.Data/"]
COPY ["LuduStack.Infra.CrossCutting.Identity/LuduStack.Infra.CrossCutting.Identity.csproj", "LuduStack.Infra.CrossCutting.Identity/"]
COPY ["LuduStack.Infra.CrossCutting.Abstractions/LuduStack.Infra.CrossCutting.Abstractions.csproj", "LuduStack.Infra.CrossCutting.Abstractions/"]
COPY ["LuduStack.Infra.Data.MongoDb/LuduStack.Infra.Data.MongoDb.csproj", "LuduStack.Infra.Data.MongoDb/"]
COPY ["LuduStack.Infra.CrossCutting.IoC/LuduStack.Infra.CrossCutting.IoC.csproj", "LuduStack.Infra.CrossCutting.IoC/"]
COPY ["LuduStack.Infra.CrossCutting.Notifications/LuduStack.Infra.CrossCutting.Notifications.csproj", "LuduStack.Infra.CrossCutting.Notifications/"]
COPY ["LuduStack.Infra.Data.Cache/LuduStack.Infra.Data.Cache.csproj", "LuduStack.Infra.Data.Cache/"]
RUN dotnet restore "LuduStack.Web/LuduStack.Web.csproj"
COPY . .
WORKDIR "/src/LuduStack.Web"
RUN dotnet build "LuduStack.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LuduStack.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LuduStack.Web.dll"]
