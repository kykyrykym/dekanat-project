using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DekanatAPI.Models;

[Table("Студенты")]
public class Student
{
    [Key]
    public int ID { get; set; }
    public string ФИО { get; set; }
    public string Пол { get; set; }
    public DateTime Дата_Рождения { get; set; }
    public int Код_Группы { get; set; }
    public DateTime Дата_Зачисления { get; set; }
    public string Номер_Зачетки { get; set; }
    public decimal Средний_Балл { get; set; }
    public int Статус { get; set; }
    public DateTime? Дата_Отчисления { get; set; }
    public DateTime? Дата_Академа { get; set; }
}