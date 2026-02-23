using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DekanatAPI.Models;

[Table("Все_Группы", Schema = "dbo")]
public class Group
{
    [Key]
    [Column("Код")]
    public int Id { get; set; }
    
    [Column("Название")]
    public string GroupName { get; set; } = string.Empty;
    
    [Column("Курс")]
    public int Course { get; set; }
    
    [Column("УчебныйГод")]
    public string AcademicYear { get; set; } = string.Empty;
    
    [Column("Специальность")]
    public string? Specialty { get; set; }
    
    [Column("Код_Факультета")]
    public int? FacultyCode { get; set; }
    
    [Column("Форма_Обучения")]
    public int? StudyFormCode { get; set; }
    
    [Column("Уровень")]
    public int? EducationLevelCode { get; set; }
    
    [Column("Удалена")]
    public bool? IsDeleted { get; set; }
}