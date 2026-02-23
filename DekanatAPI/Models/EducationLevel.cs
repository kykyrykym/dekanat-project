using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DekanatAPI.Models;

[Table("Уровень_образования", Schema = "dbo")]
public class EducationLevel
{
    [Key]
    [Column("Код_записи")]
    public int Code { get; set; }
    
    [Column("Уровень")]
    public string? LevelName { get; set; }
}