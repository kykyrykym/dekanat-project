using Microsoft.EntityFrameworkCore;
using DekanatAPI.Models;

namespace DekanatAPI.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Group> Все_Группы { get; set; }
    public DbSet<Faculty> Факультеты { get; set; }
    public DbSet<StudyForm> ФормаОбучения { get; set; }
    public DbSet<EducationLevel> Уровень_образования { get; set; }
    public DbSet<Student> Студенты { get; set; }  // ← ЭТО ДОБАВИТЬ
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string connectionString = "Server=localhost\\SQLEXPRESS;Database=Деканат;Trusted_Connection=True;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}