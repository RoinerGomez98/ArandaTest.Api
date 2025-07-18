# ArandaTest.Api 🧠

API REST desarrollada en .NET 9 con arquitectura limpia, Entity Framework Core y despliegue dockerizado. Incluye integración con SQL Server y ejecución automática de migraciones en entorno de desarrollo.

## 🚀 Requisitos previos

- [Docker](https://www.docker.com/products/docker-desktop) instalado y corriendo (si se desea usar docker)
- [Git](https://git-scm.com/downloads) instalado
- Visual Studio 2022 o superior (opcional, para desarrollo local)

---

## 📥 Clonar el repositorio

=> git clone https://github.com/RoinerGomez98/ArandaTest.Api.git
UNA VEZ DESCARGADO EL PROYECTO =>
                                1-Cambia el appsetting el ConnectionStrings con tu servidor para la base de datos en la propiedad ConnectionAranda, actualmente esta mi servidor cambialo por el adecuado para poder ejecutar la migracion
                                2-ejecuta la migracion:
                                => dotnet ef database update --project ArandaTest.Infrastructure --startup-project ArandaTest.Api
NOTA: SI NO TE DESCARGA LA CARPETA 'MIGRATIONS' en ArandaTest.Infrastructure ejecuta el comando en la consola 'Package Manager Console' 
=> dotnet ef migrations add InitialMigration --project ArandaTest.Infrastructure --startup-project ArandaTest.Api

SI DESEAS EJECUTAR EL DOCKER VE A LA CARPETA RAIZ DONDE SE ENCUENTRA EL 'docker-compose' EJECUTA EL COMANDO 
=> docker-compose up --build

=======================SQL SERVER========================
Se crearon 3 tablas 
Products => para el crud de productos
Category => para las categorias de los productos tambien tiene su propio crud (aunque sin delete por seguridad)
Users => se creo un usuario por defecto para las pruebas de login y tokens e inicio de sesion
Mail => rstiven_98@hotmail.com
Pass => 1023970895

ESTRUCTURA DEL PROYECTO
<img width="561" height="710" alt="image" src="https://github.com/user-attachments/assets/b12ef7b1-69ac-43aa-a638-369934613152" />
ArandaTest.Infrastructure => contiene el repositorio generico, el dbContext para EF , las inyecciones de dependencias para los servicios y las migracions EF
ArandaTest.Domain => contiene las entidades de base de datos, la interfaz del repositorio generico y unas clases utiles para las respuestas al front y una clase para encriptar la contraseña al guardar el usuario y al consultarlo
ArandaTest.Application => contiene la logica donde estan todos los Dtos, las implementaciones de cada tabla las interfaces de cada tabla y lo mappers
ArandaTest.Api => contiene los controladores e informacion principal, dockerFile,Program.cs , los decoradores para filtros del token y filtros para capturar los errores de la aplicacion y guardarlos en un log
