# TaskMasterProject/Dockerfile

# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["TaskMaster.sln", "."]
COPY ["TaskMaster.Presentation/TaskMaster.Presentation.csproj", "TaskMaster.Presentation/"]
COPY ["TaskMaster.Application/TaskMaster.Application.csproj", "TaskMaster.Application/"]
COPY ["TaskMaster.Domain/TaskMaster.Domain.csproj", "TaskMaster.Domain/"]
COPY ["TaskMaster.Infrastructure/TaskMaster.Infrastructure.csproj", "TaskMaster.Infrastructure/"]
RUN dotnet restore "TaskMaster.Presentation/TaskMaster.Presentation.csproj"

# Copiar el resto del código fuente
COPY . .

# Construir el proyecto de presentación
WORKDIR "/src/TaskMaster.Presentation"
RUN dotnet build "TaskMaster.Presentation.csproj" -c Release -o /app/build

# Etapa de publicación
FROM build AS publish
RUN dotnet publish "TaskMaster.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final de ejecución
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y postgresql-client && rm -rf /var/lib/apt/lists/*
COPY --from=publish /app/publish .
COPY ["TaskMaster.Presentation/wait-for-db.sh", "./wait-for-db.sh"]
COPY ["TaskMaster.Presentation/entrypoint.sh", "./entrypoint.sh"]
RUN chmod +x ./wait-for-db.sh ./entrypoint.sh
ENTRYPOINT ["./entrypoint.sh"]