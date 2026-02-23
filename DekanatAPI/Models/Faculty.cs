using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DekanatAPI.Models;

[Table("Факультеты", Schema = "dbo")]
public class Faculty
{
    [Key]
    [Column("Код")]
    public int Code { get; set; }
    
    [Column("Факультет")]
    public string? FacultyName { get; set; }
}