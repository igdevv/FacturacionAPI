# FacturacionAPI V1

Backend de aprendizaje para practicar C#, ASP.NET Core, Entity Framework Core y Clean Architecture con SQL Server. Gestiona productos, inventario, clientes y facturas internas, con reglas de negocio en el dominio y pruebas con xUnit.

**La emisión es interna y didáctica. No constituye facturación electrónica certificada por la DGII, no genera e-NCF y no tiene integración fiscal.** La tasa fija utilizada es una simplificación del ejercicio, no una validación tributaria.

## Tecnologías y requisitos

- .NET 10 y ASP.NET Core 10.
- Entity Framework Core 10.0.14 y proveedor SQL Server.
- SQL Server o SQL Server Express LocalDB para desarrollo en Windows.
- Swagger UI, OpenAPI y xUnit; sin bibliotecas de mediación, mapeo ni mocking.
- SDK .NET 10 instalado para los comandos de este documento.

El repositorio usa `net10.0` y una solución `.slnx`. Aunque el proyecto comenzó en Visual Studio 2022, para trabajar con .NET 10 en un IDE usa una versión compatible de Visual Studio 2026. También puedes usar la CLI `dotnet`. Consulta la [compatibilidad oficial de SDK, MSBuild y Visual Studio](https://learn.microsoft.com/dotnet/core/porting/versioning-sdk-msbuild-vs).

## Arquitectura

| Proyecto | Responsabilidad |
| --- | --- |
| `FacturacionAPI.Domain` | Entidades, cálculos, inventario y transiciones de estado. No depende de EF Core. |
| `FacturacionAPI.Application` | Casos de uso, DTOs e interfaces de repositorios. Depende de Domain. |
| `FacturacionAPI.Infrastructure` | `AppDbContext`, repositorios SQL Server, relaciones y migraciones. |
| `FacturacionAPI.API` | Controladores HTTP, configuración, DI y excepciones globales. Es el proyecto de inicio. |
| `FacturacionAPI.Domain.Tests` | Proyecto xUnit existente, ampliado con pruebas de dominio, aplicación, errores HTTP y persistencia. |

El proyecto web real se llama **FacturacionAPI.API**, no `FacturacionAPI.WebAPI`.

## Funcionalidades y decisiones de V1

- Productos: crear, listar, consultar por ID, buscar por nombre, cambiar precio, aumentar y descontar stock.
- Clientes: crear, listar y consultar por ID. Documento obligatorio y único, protegido también por un índice SQL.
- Facturas: crear en Borrador, listar con detalles, consultar por ID y emitir una única vez.
- Una factura requiere un cliente existente y al menos un producto existente con cantidad positiva. Cada producto se envía en una única línea; los duplicados se rechazan con 400.
- El servidor consulta todos los productos solicitados en una sola consulta y copia su precio a cada detalle. Precios, tasas y totales enviados como propiedades adicionales en el JSON se ignoran.
- ITBIS didáctico: `0.18` en todas las líneas, definido en `CrearFacturaService`. Fórmulas originales del dominio: `Subtotal = Cantidad × PrecioUnitario`, `Itbis = Subtotal × TasaItbis`, `Total = Subtotal + Itbis`. La factura suma sus líneas; no se añadió redondeo fiscal.
- El precio queda guardado en el detalle: cambiar el precio del producto posteriormente no cambia los importes de la factura. El nombre del producto y los datos del cliente se consultan del catálogo actual, sin copia histórica.
- `FechaEmision` conserva el comportamiento original: contiene la fecha UTC de creación del borrador y no se cambia al emitir. La materialización desde SQL conserva UTC.
- **Crear o emitir una factura no reserva ni descuenta stock.** El inventario se administra mediante sus endpoints propios. Es una separación deliberada de esta versión didáctica.
- Cabecera y detalles se guardan juntos mediante una única llamada a `SaveChangesAsync`, que utiliza una transacción de EF Core.
- `Stock` y `Precio` son tokens de concurrencia de producto; `Estado` es token de concurrencia de factura. Las modificaciones cargan entidades con seguimiento para conservar los valores originales. Una escritura sobre datos modificados devuelve 409; el cliente debe consultar de nuevo antes de decidir si reintenta.
- Las consultas usan `AsNoTracking()`. Las facturas incluyen cliente, detalles y productos sin carga diferida ni consultas por cada línea.

Se corrigió el acceso público a `_detalles`: ahora es un campo privado, configurado como backing field de EF, y se modifica a través de `AgregarDetalle`. Las fórmulas y las condiciones de emisión del dominio se conservaron. Se distinguieron argumentos inválidos de conflictos de negocio, se rechazaron precios que SQL redondearía o no podría almacenar en `decimal(18,2)`, y se evitó el desbordamiento del stock. El documento del cliente admite como máximo los 450 caracteres del índice existente.

## Configuración y ejecución local

Desde la raíz del repositorio, verifica las herramientas y restaura:

```powershell
dotnet --info
dotnet tool restore
dotnet restore
dotnet build
```

El manifiesto `dotnet-tools.json` fija `dotnet-ef` en 10.0.14, coincidente con EF Core del proyecto. Ejecuta los comandos EF desde la raíz para usar esa herramienta local.

`appsettings.json` contiene una configuración de LocalDB para desarrollo con autenticación integrada. Para elegir tu propia base sin editar ni publicar credenciales, configura una variable de entorno en PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\MSSQLLocalDB;Database=FacturacionDB;Trusted_Connection=True;TrustServerCertificate=True'
```

La instancia LocalDB debe existir y estar disponible para tu usuario. Si utilizas SQL Server Express, sustituye el servidor por tu instancia local, por ejemplo `.\SQLEXPRESS`, manteniendo el nombre de base que hayas elegido. `TrustServerCertificate=True` se muestra únicamente para desarrollo local. No publiques contraseñas ni cadenas privadas; los archivos locales de secretos y `appsettings.Development.json` están ignorados por Git.

Aplica las migraciones **solo después de verificar que la conexión apunta a tu base local**:

```powershell
dotnet ef migrations list --project FacturacionAPI.Infrastructure --startup-project FacturacionAPI.API
dotnet ef database update --project FacturacionAPI.Infrastructure --startup-project FacturacionAPI.API
```

La API no aplica migraciones automáticamente al iniciar. Sobre una base con datos, revisa primero los scripts y haz una copia de respaldo. La migración existente `DocumentoClienteUnico` requiere documentos únicos de hasta 450 caracteres: si hay duplicados o documentos más largos, deben revisarse antes de aplicarla, sin borrar registros automáticamente.

Para iniciar con HTTPS:

```powershell
dotnet dev-certs https --trust
dotnet run --project FacturacionAPI.API --launch-profile https
```

- Swagger UI: `https://localhost:7149/swagger`.
- Swagger JSON: `https://localhost:7149/swagger/v1/swagger.json`.
- OpenAPI: `https://localhost:7149/openapi/v1.json`.
- El perfil `http` también está disponible en `http://localhost:5188` para desarrollo local.

Swagger y OpenAPI solo se publican en el entorno `Development`.

## Migraciones y modelo relacional

| Migración | Contenido |
| --- | --- |
| `20261103201100_Inicial` | Productos. |
| `20261105002849_AgregarClientes` | Clientes; ya estaba en el trabajo pendiente del repositorio. |
| `20261105004300_DocumentoClienteUnico` | Índice único de documento; ya estaba en el trabajo pendiente. |
| `20261105232658_CompletarFacturasV1` | Facturas, detalles, claves foráneas e índices de consulta. Nueva en V1. |

Relaciones: Cliente 1:N Factura, Factura 1:N DetalleFactura y Producto 1:N DetalleFactura. SQL impide borrar clientes o productos referenciados. Borrar una factura en SQL eliminaría sus detalles en cascada; la API no expone endpoints de eliminación.

La colección `Detalles` usa el campo privado `_detalles`. EF materializa las entidades mediante sus constructores parametrizados y setters privados. Los totales se calculan en memoria y no se almacenan como columnas. `Precio` y `PrecioUnitario` usan `decimal(18,2)`; `TasaItbis`, `decimal(5,4)`. Los tokens de concurrencia aprovechan columnas existentes y no requieren una columna `rowversion` adicional.

Para comprobar el modelo o generar un script para revisión, sin aplicarlo:

```powershell
dotnet ef migrations has-pending-model-changes --project FacturacionAPI.Infrastructure --startup-project FacturacionAPI.API
dotnet ef migrations script --idempotent --project FacturacionAPI.Infrastructure --startup-project FacturacionAPI.API --output migraciones-v1.sql
```

No uses `EnsureDeleted`, `database drop` ni reinicies la base para actualizarla.

## Endpoints

| Método | Ruta | Resultado exitoso |
| --- | --- | --- |
| POST | `/api/productos` | 201 y producto creado; cabecera Location. |
| GET | `/api/productos` | 200 y lista. |
| GET | `/api/productos/{id}` | 200 y producto. |
| GET | `/api/productos/buscar?nombre=mouse` | 200 y coincidencias. |
| PATCH | `/api/productos/{id}/precio` | 204. |
| PATCH | `/api/productos/{id}/stock/aumentar` | 204. |
| PATCH | `/api/productos/{id}/stock/descontar` | 204. |
| POST | `/api/clientes` | 201 y cliente creado; cabecera Location. |
| GET | `/api/clientes` | 200 y lista. |
| GET | `/api/clientes/{id}` | 200 y cliente. |
| POST | `/api/facturas` | 201 y factura completa; cabecera Location. |
| GET | `/api/facturas` | 200 y lista de facturas completas. |
| GET | `/api/facturas/{id}` | 200 y factura completa. |
| PATCH | `/api/facturas/{id}/emitir` | 204, sin cuerpo de petición. |

Errores: 400 para datos inválidos; 404 para recursos inexistentes, incluido cliente o producto referenciado al crear factura; 409 para documento duplicado, inventario insuficiente, estado incompatible o concurrencia. Los errores usan `ProblemDetails`; los errores de validación del modelo pueden incluir `errors`. Un error inesperado conserva el estado 500, se registra y no expone detalles internos.

## Ejemplos JSON

Crear producto: `POST /api/productos`:

```json
{ "nombre": "Mouse", "precio": 100.00, "stock": 10 }
```

Respuesta 201, con ID ilustrativo:

```json
{ "id": 1, "nombre": "Mouse", "precio": 100.00, "stock": 10 }
```

Cambiar precio: `PATCH /api/productos/1/precio`:

```json
{ "nuevoPrecio": 125.50 }
```

Aumentar o descontar stock, mediante la ruta correspondiente:

```json
{ "cantidad": 2 }
```

Crear cliente: `POST /api/clientes`. Documento ilustrativo, sin validación fiscal del formato:

```json
{
  "nombre": "Cliente de ejemplo",
  "rncCedula": "DOCUMENTO-DE-PRUEBA-001",
  "correo": "cliente@example.com",
  "telefono": null
}
```

Respuesta 201:

```json
{
  "id": 1,
  "nombre": "Cliente de ejemplo",
  "rncCedula": "DOCUMENTO-DE-PRUEBA-001",
  "correo": "cliente@example.com",
  "telefono": null
}
```

Crear factura: `POST /api/facturas`. Usa IDs reales devueltos por tus altas anteriores:

```json
{
  "clienteId": 1,
  "productos": [
    { "productoId": 1, "cantidad": 2 },
    { "productoId": 2, "cantidad": 1 }
  ]
}
```

Ejemplo de respuesta 201, suponiendo un Mouse de 100.00 y un Teclado de 50.00 previamente creados. Los IDs y la fecha son ilustrativos; los GET devuelven la misma estructura:

```json
{
  "id": 1,
  "cliente": {
    "id": 1,
    "nombre": "Cliente de ejemplo",
    "rncCedula": "DOCUMENTO-DE-PRUEBA-001",
    "correo": "cliente@example.com",
    "telefono": null
  },
  "fechaEmision": "2026-11-05T12:00:00Z",
  "estado": "Borrador",
  "detalles": [
    {
      "id": 1, "productoId": 1, "nombreProducto": "Mouse",
      "cantidad": 2, "precioUnitario": 100.00, "tasaItbis": 0.18,
      "subtotal": 200.00, "itbis": 36.00, "total": 236.00
    },
    {
      "id": 2, "productoId": 2, "nombreProducto": "Teclado",
      "cantidad": 1, "precioUnitario": 50.00, "tasaItbis": 0.18,
      "subtotal": 50.00, "itbis": 9.00, "total": 59.00
    }
  ],
  "subtotal": 250.00,
  "itbis": 45.00,
  "total": 295.00,
  "cantidadTotalProductos": 3
}
```

Emitir: `PATCH /api/facturas/1/emitir`, sin cuerpo. Responde 204. Una consulta posterior muestra `"estado": "Emitida"`; una segunda emisión responde 409, por ejemplo:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflicto de negocio",
  "status": 409,
  "detail": "Solo se pueden emitir facturas en estado Borrador.",
  "instance": "/api/facturas/1/emitir"
}
```

`ProblemDetails` puede incluir metadatos adicionales como `traceId`.

## Pruebas

```powershell
dotnet test
```

Sin configuración SQL se ejecutan las pruebas independientes de base de datos y se marcan explícitamente como omitidas las 5 pruebas de integración SQL. Las pruebas cubren validaciones de producto, stock, desbordamientos, cálculos, colección privada, transición de estado, creación con referencias inexistentes, cantidades inválidas y respuestas del manejador global.

Para incluir SQL Server, utiliza **únicamente la base aislada local `FacturacionAPI_V1_Pruebas`**. Primero créala si no existe y aplica las migraciones:

```powershell
sqlcmd -S '(localdb)\MSSQLLocalDB' -E -C -b -Q "IF DB_ID(N'FacturacionAPI_V1_Pruebas') IS NULL CREATE DATABASE [FacturacionAPI_V1_Pruebas]"
$env:FACTURACION_TEST_SQLSERVER = 'Server=(localdb)\MSSQLLocalDB;Database=FacturacionAPI_V1_Pruebas;Trusted_Connection=True;TrustServerCertificate=True'
dotnet ef database update --project FacturacionAPI.Infrastructure --startup-project FacturacionAPI.API --connection "$env:FACTURACION_TEST_SQLSERVER"
dotnet test
Remove-Item Env:FACTURACION_TEST_SQLSERVER
```

Las pruebas SQL rechazan otras instancias o nombres de base. No crean ni migran la base por sí mismas y revierten todas sus escrituras mediante transacciones. Verifican materialización, grafo relacionado, precio histórico, emisión, stock concurrente, emisión concurrente, atomicidad ante una FK inválida y documento duplicado. Usan dos contextos con lecturas previas para reproducir escrituras con valores originales obsoletos. Las identidades SQL pueden avanzar aunque se reviertan las inserciones.

## Validación realizada de V1

- `dotnet restore`: correcto.
- `dotnet build`: correcto, 0 advertencias y 0 errores.
- `dotnet test` con SQL local: 57 correctas, 0 fallidas y 0 omitidas.
- Cuatro migraciones aplicadas y verificadas en `FacturacionAPI_V1_Pruebas`; modelo sin cambios pendientes.
- Verificación HTTP con la API real: Swagger, altas y consultas de productos/clientes, operaciones de stock/precio, creación y consulta de facturas, importes calculados a partir del catálogo, validación 400, inexistentes 404, conflictos 409 y emisión 204.
- `FacturacionDB` no fue migrada ni alterada durante esta implementación. La base aislada se conserva y contiene registros de la comprobación HTTP; las pruebas xUnit SQL usan rollback.
- Cambios existentes de clientes y sus dos migraciones conservados. Sin commits ni push automáticos.

## Límites y temas de estudio

V1 no incluye autenticación, paginación, anulación, actualización de borradores, borrado, validación fiscal de documentos, productos exentos ni integración DGII. `Anulada` permanece en el enum original, sin caso de uso expuesto. Los listados completos están pensados para el volumen de un proyecto de aprendizaje.

Para comprender los cambios, estudia encapsulación y agregados; DTOs y casos de uso; inyección de dependencias con lifetime Scoped; LINQ y async/await; seguimiento de entidades, Include y AsNoTracking; claves foráneas, precisión decimal y migraciones; transacciones y concurrencia optimista; middleware de excepciones, ProblemDetails y códigos HTTP; pruebas unitarias e integración con rollback.
