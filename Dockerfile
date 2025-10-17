# Utiliza la imagen de SDK de .NET para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Instala las herramientas de SQL Server
RUN apt-get update && apt-get install -y curl apt-transport-https gnupg && \
    curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > /etc/apt/trusted.gpg.d/microsoft.gpg && \
    echo "deb [arch=amd64 signed-by=/etc/apt/trusted.gpg.d/microsoft.gpg] https://packages.microsoft.com/debian/11/prod bullseye main" > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    DEBIAN_FRONTEND=noninteractive ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev

# Copia los archivos de proyecto y restaura las dependencias
COPY *.sln .
COPY TaskMaster.Application/*.csproj ./TaskMaster.Application/
COPY TaskMaster.Domain/*.csproj ./TaskMaster.Domain/
COPY TaskMaster.Infrastructure/*.csproj ./TaskMaster.Infrastructure/
COPY TaskMaster.Presentation/*.csproj ./TaskMaster.Presentation/
COPY TaskMaster.Tests/*.csproj ./TaskMaster.Tests/
RUN dotnet restore
RUN dotnet tool install --global dotnet-ef

# Copia el resto del código fuente
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Utiliza la imagen de ASP.NET Core para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y curl apt-transport-https gnupg && \
    curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > /etc/apt/trusted.gpg.d/microsoft.gpg && \
    echo "deb [arch=amd64 signed-by=/etc/apt/trusted.gpg.d/microsoft.gpg] https://packages.microsoft.com/debian/11/prod bullseye main" > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    DEBIAN_FRONTEND=noninteractive ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev curl

# Copia dotnet-ef desde la etapa de compilación
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"

COPY --from=build /app/publish .

# Copia los scripts y los hace ejecutables
COPY --from=build /source/wait-for-db.sh .
COPY --from=build /source/entrypoint.sh .
RUN chmod +x wait-for-db.sh entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]