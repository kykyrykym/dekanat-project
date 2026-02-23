using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DekanatAPI.Models;

[Table("ФормаОбучения", Schema = "dbo")]
public class StudyForm
{
    [Key]
    [Column("Код")]
    public int Code { get; set; }
    
    [Column("ФормаОбучения")]
    public string? FormName { get; set; }
}