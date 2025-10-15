# TaskMaster (Proyecto Base del Curso)

Este proyecto es la plantilla inicial que se utiliza al comienzo del curso de Programación Avanzada en ASP.NET. Representa una estructura mínima de una solución .NET con la separación de capas básica de Clean Architecture, pero sin ninguna implementación específica.

## Estructura del Proyecto

La solución está dividida en 4 proyectos de biblioteca de clases, siguiendo los principios de Clean Architecture:

- **TaskMaster.Domain:** Destinado a contener las entidades de negocio y la lógica de dominio.
- **TaskMaster.Application:** Destinado a los casos de uso de la aplicación e interfaces.
- **TaskMaster.Infrastructure:** Destinado a las implementaciones de acceso a datos y servicios externos.
- **TaskMaster.Presentation:** El punto de entrada de la aplicación, una API de ASP.NET Core mínima.

Este proyecto no incluye base de datos, autenticación ni lógica de negocio avanzada. Su propósito es servir como punto de partida para las lecciones del curso.

---

## Cómo Clonar y Ejecutar el Proyecto

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### 1. Clonar el Repositorio

```bash
git --version
git clone https://github.com/rjonathan87/TaskMasterProjectCurso.git
cd https://github.com/rjonathan87/TaskMasterProjectCurso.git/curso/TaskMasterProjectCurso
```

### 2. Restaurar Dependencias

Desde la carpeta raíz del proyecto (`curso/TaskMasterProjectCurso`), restaura los paquetes NuGet necesarios:

```bash
dotnet restore
```

### 3. Ejecutar la Aplicación

Para iniciar la API, ejecuta el siguiente comando. Esto compilará y ejecutará el proyecto `TaskMaster.Presentation`.

```bash
dotnet run --project TaskMaster.Presentation
```

La API se iniciará y podrás ver las URLs en la consola (ej. `https://localhost:7202` y `http://localhost:5067`). Por defecto, puedes acceder a la documentación de Swagger en la URL de HTTPS, por ejemplo: `https://localhost:7202/swagger`.


## Insertar Data en Tasks
```
  {
    "title": "string 1",
    "description": "string 1",
    "projectId": "0b78d345-d851-420b-ac9b-709ff1e966fd",
    "assignedToId": "e9a115ab-87d7-4264-9020-4daa0d842f40",
    "dueDate": "2025-10-10T04:40:18.301Z"
  }
```