FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/CadastroPessoas.API/CadastroPessoas.API.csproj", "src/CadastroPessoas.API/"]
COPY ["src/CadastroPessoas.Application/CadastroPessoas.Application.csproj", "src/CadastroPessoas.Application/"]
COPY ["src/CadastroPessoas.Domain/CadastroPessoas.Domain.csproj", "src/CadastroPessoas.Domain/"]
COPY ["src/CadastroPessoas.Infrastructure/CadastroPessoas.Infrastructure.csproj", "src/CadastroPessoas.Infrastructure/"]
RUN dotnet restore "src/CadastroPessoas.API/CadastroPessoas.API.csproj"

COPY . .
WORKDIR "/src/src/CadastroPessoas.API"
RUN dotnet build "CadastroPessoas.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CadastroPessoas.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CadastroPessoas.API.dll"]
