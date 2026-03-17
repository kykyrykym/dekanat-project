using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DekanatAPI.Data;

namespace DekanatAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GroupsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public GroupsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Тестовый endpoint для проверки подключения к БД
    [HttpGet("test")]
    public async Task<ActionResult<string>> TestConnection()
    {
        var count = await _context.Все_Группы.CountAsync();
        return Ok($"✅ Подключение успешно! Найдено групп: {count}");
    }

    // Основной метод получения групп с фильтрацией и пагинацией
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetGroups(
        [FromQuery] string? faculty,
        [FromQuery] int? studyForm,
        [FromQuery] int? educationLevel,
        [FromQuery] string? courses,
        [FromQuery] string? academicYear,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        // Базовый запрос с JOIN для получения названий вместо кодов
        var query = from g in _context.Все_Группы
                    join f in _context.Факультеты on g.FacultyCode equals f.Code into fj
                    from facultyItem in fj.DefaultIfEmpty()
                    join sf in _context.ФормаОбучения on g.StudyFormCode equals sf.Code into sfj
                    from studyFormItem in sfj.DefaultIfEmpty()
                    select new
                    {
                        g.Id,
                        g.GroupName,
                        g.Course,
                        g.AcademicYear,
                        g.Specialty,
                        FacultyName = facultyItem != null ? facultyItem.FacultyName : "Не указан",
                        StudyFormName = studyFormItem != null ? studyFormItem.FormName : "Не указана",
                        g.FacultyCode,
                        g.StudyFormCode,
                        g.EducationLevelCode
                    };

        // Применение фильтров
        if (!string.IsNullOrEmpty(faculty))
            query = query.Where(g => g.FacultyName == faculty);

        if (studyForm.HasValue)
            query = query.Where(g => g.StudyFormCode == studyForm);

        if (educationLevel.HasValue)
            query = query.Where(g => g.EducationLevelCode == educationLevel);

        if (!string.IsNullOrEmpty(academicYear))
            query = query.Where(g => g.AcademicYear == academicYear);

        // Фильтр по нескольким курсам
        if (!string.IsNullOrEmpty(courses))
        {
            var courseList = courses.Split(',').Select(int.Parse).ToList();
            query = query.Where(g => courseList.Contains(g.Course));
        }

        // Получаем общее количество для пагинации
        var totalCount = await query.CountAsync();

        // Применяем сортировку, пагинацию и выполняем запрос
        var groups = await query
            .OrderBy(g => g.Course)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Добавляем общее количество в заголовок ответа
        Response.Headers.Add("X-Total-Count", totalCount.ToString());

        return Ok(groups);
    }

    // Получение списка факультетов для фильтра
    [HttpGet("faculties")]
    public async Task<ActionResult<IEnumerable<string>>> GetFaculties()
    {
        var faculties = await _context.Факультеты
            .Where(f => f.FacultyName != null)
            .Select(f => f.FacultyName)
            .ToListAsync();
        
        return Ok(faculties);
    }

    // Получение списка форм обучения для фильтра (возвращаем объекты с code и name)
    [HttpGet("studyforms")]
    public async Task<ActionResult<IEnumerable<object>>> GetStudyForms()
    {
        var forms = await _context.ФормаОбучения
            .Select(f => new { 
                Code = f.Code, 
                Name = f.FormName 
            })
            .ToListAsync();
        
        return Ok(forms);
    }

    // Получение списка уровней образования для фильтра (возвращаем объекты с code и name)
    [HttpGet("educationlevels")]
    public async Task<ActionResult<IEnumerable<object>>> GetEducationLevels()
    {
        var levels = await _context.Уровень_образования
            .Select(l => new { 
                Code = l.Code, 
                Name = l.LevelName 
            })
            .ToListAsync();
        
        return Ok(levels);
    }

    // Получение списка учебных годов для фильтра
    [HttpGet("academicyears")]
    public async Task<ActionResult<IEnumerable<string>>> GetAcademicYears()
    {
        var years = await _context.Все_Группы
            .Where(g => g.AcademicYear != null)
            .Select(g => g.AcademicYear)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();
        
        return Ok(years);
    }
}
