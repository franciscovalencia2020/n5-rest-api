# Proyecto Backend API - .NET

Este proyecto es una API RESTful desarrollada en .NET que utiliza SQL Server como base de datos, Elasticsearch para búsqueda, y Kafka para mensajería. A continuación se detallan los pasos para ejecutar el proyecto usando Docker, así como la configuración para ejecutarlo localmente.

## Estructura del Proyecto

La estructura del proyecto es la siguiente:

    src/
    Api/
        Controllers/
        Middleware/
        Properties/
    Dockerfile
    appsettings.json
    Program.cs
    Data/
        Configurations/
        Helpers/
        Interfaces/
        Migrations/
        Models/
        Repositories/
        DbContext.cs
    Services/
        Commands/
        ElasticSearch/
        Handlers/
        Interfaces/
        Kafka/
        UserPermissionService.cs
        UserService.cs
    tests/
    Api.Tests/
    .dockerignore
    docker-compose.yml
    N5.postman_collection
    RestApiN5.sln

## Requisitos

- **Docker**: Necesitarás Docker y Docker Compose instalados en tu máquina para ejecutar el proyecto.
- **.NET 8.0**: Este proyecto está desarrollado con .NET 8.0.
- **Elasticsearch**: Se utiliza Elasticsearch para gestionar índices de búsqueda.
- **Kafka**: Kafka se usa para el manejo de mensajes asincrónicos.
- **SQL Server**: SQL Server se utiliza como base de datos principal.

## Configuración

### Variables de Entorno

Las siguientes son las variables de entorno requeridas:

- **SQL Server**:
  - **ConnectionStrings__N5Db**: La cadena de conexión a tu base de datos de SQL Server.
  - Ejemplo: `"ConnectionStrings__N5Db=Server=sqlserver;Database=N5Db;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"`

- **Elasticsearch**:
  - **ElasticSearch__Url**: La URL de tu servicio de Elasticsearch.
  - Ejemplo: `"ElasticSearch__Url=http://elasticsearch:9200"`

- **Kafka**:
  - **Kafka__BootstrapServers**: La dirección de tu servidor de Kafka.
  - Ejemplo: `"Kafka__BootstrapServers=kafka:9092"`

Para ejecutar el proyecto localmente, necesitarás modificar las variables de entorno en `appsettings.json` o configurarlas manualmente en el entorno de ejecución.

### Ejecutar el Proyecto con Docker

1. **Construir y correr los contenedores**:
   Asegúrate de tener Docker instalado y corre el siguiente comando para construir y levantar todos los contenedores:

   ```bash
   docker-compose up --build

2. **Acceso a la API**: La API estará disponible en http://localhost:5000. Puedes acceder a los endpoints definidos en el proyecto.

3. **Acceso a SQL Server**: La base de datos SQL Server estará corriendo en localhost:1433. Si necesitas conectarte desde otra aplicación o herramienta como SQL Server Management Studio, usa las siguientes credenciales:

**Usuario**: sa

**Contraseña**: YourStrong!Passw0rd

**Base de datos**: N5Db

4. **Acceso a Elasticsearch**: Elasticsearch estará corriendo en http://localhost:9200.

5. **Acceso a Kafka**: Kafka estará disponible en localhost:9092.

### Ejecutar el Proyecto Localmente (sin Docker)

Para ejecutar el proyecto sin Docker, debes asegurarte de tener los servicios de SQL Server, Elasticsearch y Kafka corriendo localmente. A continuación, realiza los siguientes pasos:

1. **Configura el archivo appsettings.json**: Modifica las variables de conexión en appsettings.json para apuntar a las instancias locales de SQL Server, Elasticsearch y Kafka. Un ejemplo de las configuraciones locales es el siguiente:

        "ConnectionStrings": {
        "N5Db": "Server=localhost;Database=N5Db;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;"
        },
        "ElasticSearch": {
        "Url": "http://localhost:9200"
        },
        "Kafka": {
        "BootstrapServers": "localhost:9092",
        "Topic": "permissions-topic"
        }

2. **Ejecuta la API localmente**: Desde Visual Studio o usando el siguiente comando en la terminal, corre la aplicación localmente:
    
    ```bash
    dotnet run

La API estará disponible en http://localhost:5000.

### Ejecución de Tests

1. **Restaurar los paquetes NuGet**: Asegúrate de que todos los paquetes necesarios estén restaurados. Puedes hacerlo ejecutando:

    ```bash
    dotnet restore

2. **Ejecutar los tests**: Para ejecutar los tests de la API, usa el siguiente comando:

    ```bash
    dotnet test

Este comando buscará todos los tests en el proyecto y los ejecutará, mostrando los resultados en la terminal.