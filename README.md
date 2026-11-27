# Productos con Clean Architecture y CQRS

Práctica basada en el proyecto del bootcamp. Usa .NET 10, Minimal API y un catálogo en memoria que empieza con **Notebook** y **Mouse**. Al reiniciar la API, se restablece el catálogo.

## Ejecutar

Abrí `BootcampCLTNoviembre.slnx` en Visual Studio con soporte para .NET 10. Seleccioná `BootcampCLTNoviembre.Api` como proyecto de inicio y el perfil `http`. También podés ejecutar este comando desde la carpeta de la solución con el SDK de .NET 10 instalado.

```sh
dotnet run --project src/BootcampCLTNoviembre.Api --launch-profile http
```

Abrí [Scalar](http://localhost:5234/scalar/v1) o usá `src/BootcampCLTNoviembre.Api/BootcampCLTNoviembre.http`. En ese archivo, ajustá `@productId` al ID del producto creado si difiere del valor indicado.

## Qué hay en cada proyecto

| Proyecto bajo `src/` | Archivos para seguir |
|---|---|
| `BootcampCLTNoviembre.Api` | `Program.cs` registra componentes. `Endpoints/ProductsEndpoints.cs` define las rutas y convierte resultados a HTTP. |
| `BootcampCLTNoviembre.Application` | `Products/Commands/` contiene las operaciones de escritura. `Products/Queries/` contiene las consultas. `Products/Responses.cs` define el DTO de salida. |
| `BootcampCLTNoviembre.Domain` | `Products/Product.cs` protege las reglas de la entidad. `Products/IProductRepository.cs` define las operaciones que necesitan los comandos. |
| `BootcampCLTNoviembre.Infrastructure` | `Persistence/ProductRepository.cs` maneja entidades, `ReadModels/ProductsReadStore.cs` proyecta los datos a DTOs y `Persistence/InMemoryProducts.cs` guarda el catálogo compartido. |

Cada proyecto tiene su propio `.csproj`. `Application` referencia a `Domain`. `Infrastructure` tiene una referencia directa a `Application` y accede a `Domain` por esa referencia. `Api` referencia a `Application` e `Infrastructure` para exponer los casos de uso y registrar sus implementaciones. `Domain` es independiente de los otros proyectos.

## Recorrido recomendado

1. Abrí `ProductsEndpoints.cs` y buscá `MapPost`. El cuerpo JSON se convierte en un `CreateProductCommand`. El endpoint llama a `ISender.Send`.
2. Abrí `Products/Commands/CreateProduct/CreateProductCommand.cs`. Ahí están el mensaje, el validador y el handler. El validador revisa los campos. El handler comprueba que el nombre sea único, genera el ID, llama a `Product.Create`, guarda la entidad y devuelve un `ProductResponse`. La API responde `201 Created` y la cabecera `Location` apunta a la consulta por ID.
3. Abrí `Products/Queries/GetProductById.cs`. El handler le pide a `IProductsReadStore` un `ProductResponse`. Infrastructure resuelve la lectura desde el mismo catálogo que usa el repositorio de escritura.
4. Revisá `Product.cs`. `Create` y `Update` comparten `SetData`. Ahí se protegen reglas como permitir stock cero y rechazar stock negativo. La regla de nombre único necesita consultar otros productos, por eso se comprueba desde Application.
5. Revisá `Program.cs` y los dos `DependencyInjection.cs`. `AddApplication()` registra el sender, los handlers y los validadores. `AddInfrastructure()` conecta cada interfaz con su implementación en memoria.

`LoggingBehavior` mide la operación. `ValidationBehavior` revisa el mensaje antes de que llegue al handler. `InMemoryAccessBehavior` mantiene protegida la colección compartida durante cada operación, incluso cuando dos peticiones intentan usar el mismo nombre a la vez.

## Operaciones disponibles

Todas las rutas arrancan en `/v1/api/products`.

| Petición | Caso de uso | Respuesta exitosa |
|---|---|---|
| `GET /v1/api/products?search=notebook` | Listar, con filtro de nombre opcional | 200 |
| `GET /v1/api/products/{id}` | Buscar por ID | 200 |
| `POST /v1/api/products` | Crear | 201 |
| `PUT /v1/api/products/{id}` | Reemplazar los datos editables | 200 |
| `PATCH /v1/api/products/{id}` | Modificar campos puntuales | 200 |
| `DELETE /v1/api/products/{id}` | Eliminar | 204 |

`GlobalExceptionHandler` devuelve 400 si hay datos inválidos o un nombre duplicado, y 404 si no encuentra el producto solicitado. En el PATCH de este ejercicio, un campo omitido o con `null` conserva su valor. Enviar una descripción vacía la limpia.

## Comparación con el repo del profe

Se conservan la ruta, los cuatro proyectos, los comandos, las consultas, el mediador propio y las validaciones. En esta práctica el catálogo en memoria asigna el ID y los cambios se aplican directamente en el diccionario. En el repo del profe, EF Core confirma cambios con `IUnitOfWork.SaveChangesAsync` y PostgreSQL genera el ID. Los endpoints y los handlers muestran la misma separación entre lectura y escritura.
