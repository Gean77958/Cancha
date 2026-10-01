var builder = WebApplication.CreateBuilder(args);

// Servicios de OpenAPI (solo documentación, se habilita en desarrollo más abajo)
builder.Services.AddOpenApi();

// CORS: permite que el frontend (otro dominio/puerto) consuma esta API
// desde el navegador. Sin esto, el navegador bloquea las peticiones del
// frontend aunque el backend responda correctamente.
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

// ------------------------------------------------------------------
// "Base de datos" en memoria. Se reinicia cada vez que el servidor se
// reinicia o se vuelve a desplegar en Render. Para el curso es
// suficiente, no hace falta una base de datos real.
// ------------------------------------------------------------------
var horarios = new List<Horario>
{
    new Horario { Id = 1,  Fecha = "2026-10-06", HoraInicio = "08:00", HoraFin = "09:00", TipoCancha = "Fútbol 5",  Estado = "Disponible", Precio = 60.00m, DuracionMinutos = 60 },
    new Horario { Id = 2,  Fecha = "2026-10-06", HoraInicio = "18:00", HoraFin = "19:00", TipoCancha = "Fútbol 7",  Estado = "Reservado",  Precio = 90.00m, DuracionMinutos = 60 },
    new Horario { Id = 3,  Fecha = "2026-10-06", HoraInicio = "19:00", HoraFin = "20:30", TipoCancha = "Fútbol 11", Estado = "Disponible", Precio = 150.00m, DuracionMinutos = 90 },
    new Horario { Id = 4,  Fecha = "2026-10-07", HoraInicio = "07:00", HoraFin = "08:00", TipoCancha = "Fútbol 5",  Estado = "Disponible", Precio = 60.00m, DuracionMinutos = 60 },
    new Horario { Id = 5,  Fecha = "2026-10-07", HoraInicio = "20:00", HoraFin = "21:00", TipoCancha = "Fútbol 7",  Estado = "Disponible", Precio = 90.00m, DuracionMinutos = 60 },
    new Horario { Id = 6,  Fecha = "2026-10-07", HoraInicio = "21:00", HoraFin = "22:00", TipoCancha = "Fútbol 7",  Estado = "Mantenimiento", Precio = 90.00m, DuracionMinutos = 60 },
    new Horario { Id = 7,  Fecha = "2026-10-08", HoraInicio = "17:00", HoraFin = "18:00", TipoCancha = "Fútbol 5",  Estado = "Reservado",  Precio = 65.00m, DuracionMinutos = 60 },
    new Horario { Id = 8,  Fecha = "2026-10-08", HoraInicio = "18:30", HoraFin = "20:00", TipoCancha = "Fútbol 11", Estado = "Disponible", Precio = 150.00m, DuracionMinutos = 90 },
    new Horario { Id = 9,  Fecha = "2026-10-09", HoraInicio = "09:00", HoraFin = "10:00", TipoCancha = "Fútbol 5",  Estado = "Disponible", Precio = 60.00m, DuracionMinutos = 60 },
    new Horario { Id = 10, Fecha = "2026-10-09", HoraInicio = "19:00", HoraFin = "20:00", TipoCancha = "Fútbol 7",  Estado = "Disponible", Precio = 85.00m, DuracionMinutos = 60 },
};

var servicios = new List<Servicio>
{
    new Servicio { Id = 1, Nombre = "Vestuarios",      Descripcion = "Vestuarios con duchas y casilleros incluidos.",            PrecioAdicional = 0.00m,  Disponible = true },
    new Servicio { Id = 2, Nombre = "Iluminación",      Descripcion = "Set de luces para partidos nocturnos.",                    PrecioAdicional = 15.00m, Disponible = true },
    new Servicio { Id = 3, Nombre = "Estacionamiento",  Descripcion = "Cochera privada vigilada, 10 espacios.",                   PrecioAdicional = 0.00m,  Disponible = true },
    new Servicio { Id = 4, Nombre = "Arbitraje",        Descripcion = "Árbitro certificado para partidos y campeonatos.",         PrecioAdicional = 40.00m, Disponible = true },
    new Servicio { Id = 5, Nombre = "Alquiler de balones", Descripcion = "Balones reglamentarios disponibles para alquiler.",      PrecioAdicional = 5.00m,  Disponible = true },
    new Servicio { Id = 6, Nombre = "Organización de campeonatos", Descripcion = "Armado de llaves, fixture y premiación.",        PrecioAdicional = 100.00m, Disponible = false },
};

var siguienteIdHorario = horarios.Count + 1;
var siguienteIdServicio = servicios.Count + 1;

// Ruta principal de prueba
app.MapGet("/", () => "API Cancha funcionando");

// =============== HORARIOS ===============

// GET /api/horarios -> listar todos
app.MapGet("/api/horarios", () => Results.Ok(horarios));

// GET /api/horarios/{id} -> obtener uno
app.MapGet("/api/horarios/{id:int}", (int id) =>
{
    var horario = horarios.FirstOrDefault(h => h.Id == id);
    return horario is not null ? Results.Ok(horario) : Results.NotFound();
});

// POST /api/horarios -> crear
app.MapPost("/api/horarios", (Horario nuevo) =>
{
    nuevo.Id = siguienteIdHorario++;
    horarios.Add(nuevo);
    return Results.Created($"/api/horarios/{nuevo.Id}", nuevo);
});

// PUT /api/horarios/{id} -> actualizar
app.MapPut("/api/horarios/{id:int}", (int id, Horario actualizado) =>
{
    var horario = horarios.FirstOrDefault(h => h.Id == id);
    if (horario is null) return Results.NotFound();

    horario.Fecha = actualizado.Fecha;
    horario.HoraInicio = actualizado.HoraInicio;
    horario.HoraFin = actualizado.HoraFin;
    horario.TipoCancha = actualizado.TipoCancha;
    horario.Estado = actualizado.Estado;
    horario.Precio = actualizado.Precio;
    horario.DuracionMinutos = actualizado.DuracionMinutos;

    return Results.Ok(horario);
});

// DELETE /api/horarios/{id} -> eliminar
app.MapDelete("/api/horarios/{id:int}", (int id) =>
{
    var horario = horarios.FirstOrDefault(h => h.Id == id);
    if (horario is null) return Results.NotFound();

    horarios.Remove(horario);
    return Results.NoContent();
});

// =============== SERVICIOS ===============

// GET /api/servicios -> listar todos
app.MapGet("/api/servicios", () => Results.Ok(servicios));

// GET /api/servicios/{id} -> obtener uno
app.MapGet("/api/servicios/{id:int}", (int id) =>
{
    var servicio = servicios.FirstOrDefault(s => s.Id == id);
    return servicio is not null ? Results.Ok(servicio) : Results.NotFound();
});

// POST /api/servicios -> crear
app.MapPost("/api/servicios", (Servicio nuevo) =>
{
    nuevo.Id = siguienteIdServicio++;
    servicios.Add(nuevo);
    return Results.Created($"/api/servicios/{nuevo.Id}", nuevo);
});

// PUT /api/servicios/{id} -> actualizar
app.MapPut("/api/servicios/{id:int}", (int id, Servicio actualizado) =>
{
    var servicio = servicios.FirstOrDefault(s => s.Id == id);
    if (servicio is null) return Results.NotFound();

    servicio.Nombre = actualizado.Nombre;
    servicio.Descripcion = actualizado.Descripcion;
    servicio.PrecioAdicional = actualizado.PrecioAdicional;
    servicio.Disponible = actualizado.Disponible;

    return Results.Ok(servicio);
});

// DELETE /api/servicios/{id} -> eliminar
app.MapDelete("/api/servicios/{id:int}", (int id) =>
{
    var servicio = servicios.FirstOrDefault(s => s.Id == id);
    if (servicio is null) return Results.NotFound();

    servicios.Remove(servicio);
    return Results.NoContent();
});

// Puerto: en Render, la variable de entorno PORT la asigna la plataforma
// automáticamente. En local, si no existe, usamos 5170.
var port = Environment.GetEnvironmentVariable("PORT") ?? "5170";

app.Run($"http://0.0.0.0:{port}");

// ------------------------------------------------------------------
// Modelos. ASP.NET Core serializa estas propiedades a JSON en camelCase
// por defecto (TipoCancha -> "tipoCancha"), así el frontend las recibe
// listas para usar sin transformar nada.
// ------------------------------------------------------------------
class Horario
{
    public int Id { get; set; }
    public string Fecha { get; set; } = "";
    public string HoraInicio { get; set; } = "";
    public string HoraFin { get; set; } = "";
    public string TipoCancha { get; set; } = "";
    public string Estado { get; set; } = "";
    public decimal Precio { get; set; }
    public int DuracionMinutos { get; set; }
}

class Servicio
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public decimal PrecioAdicional { get; set; }
    public bool Disponible { get; set; }
}
