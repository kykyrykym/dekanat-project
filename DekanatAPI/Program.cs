using Microsoft.EntityFrameworkCore;
using DekanatAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=Деканат;Trusted_Connection=True;TrustServerCertificate=True;"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:8080")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .WithExposedHeaders("X-Total-Count");
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowVueApp");

app.UseAuthorization();

app.MapControllers();

// ПРОСТОЙ PUT ДЛЯ СОХРАНЕНИЯ СТУДЕНТОВ (без [FromBody])
app.MapPut("/api/students/{id}", async (int id, UpdateStudentRequest request, ApplicationDbContext db) =>
{
    var student = await db.Студенты.FindAsync(id);
    if (student == null)
        return Results.NotFound();

    student.ФИО = request.ФИО;
    student.Пол = request.Пол;
    student.Дата_Рождения = request.Дата_Рождения;
    student.Номер_Зачетки = request.Номер_Зачетки;
    student.Средний_Балл = request.Средний_Балл;
    student.Статус = request.Статус;
    
    await db.SaveChangesAsync();
    Console.WriteLine($"✅ Студент {id} обновлён через Program.cs");
    
    return Results.Ok();
});

app.Run();

public class UpdateStudentRequest
{
    public string ФИО { get; set; }
    public string Пол { get; set; }
    public DateTime Дата_Рождения { get; set; }
    public string Номер_Зачетки { get; set; }
    public decimal Средний_Балл { get; set; }
    public int Статус { get; set; }
}