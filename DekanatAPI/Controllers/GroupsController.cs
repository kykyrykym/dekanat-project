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

    // GET: api/groups/test - проверка подключения
    [HttpGet("test")]
    public async Task<ActionResult<string>> TestConnection()
    {
        var count = await _context.Все_Группы.CountAsync();
        return Ok($"✅ Подключение успешно! Найдено групп: {count}");
    }

    // GET: api/groups - все группы
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetGroups()
    {
        var groups = await _context.Все_Группы
            .Select(g => new
            {
                g.Id,
                g.GroupName,
                g.Course,
                g.AcademicYear,
                g.Specialty
            })
            .ToListAsync();

        return Ok(groups);
    }

    // GET: api/groups/faculties - список факультетов
    [HttpGet("faculties")]
    public async Task<ActionResult<IEnumerable<string>>> GetFaculties()
    {
        var faculties = await _context.Факультеты
            .Where(f => f.FacultyName != null)
            .Select(f => f.FacultyName)
            .ToListAsync();
        
        return Ok(faculties);
    }

    // GET: api/groups/studyforms - список форм обучения
    [HttpGet("studyforms")]
    public async Task<ActionResult<IEnumerable<string>>> GetStudyForms()
    {
        var forms = await _context.ФормаОбучения
            .Select(f => f.FormName)
            .ToListAsync();
        
        return Ok(forms);
    }

    // GET: api/groups/educationlevels - список уровней образования
    [HttpGet("educationlevels")]
    public async Task<ActionResult<IEnumerable<string>>> GetEducationLevels()
    {
        var levels = await _context.Уровень_образования
            .Select(l => l.LevelName)
            .ToListAsync();
        
        return Ok(levels);
    }

    // GET: api/groups/academicyears - список учебных годов
    [HttpGet("academicyears")]
    public async Task<ActionResult<IEnumerable<string>>> GetAcademicYears()
    {
        var years = await _context.Все_Группы
            .Where(g => g.AcademicYear != null)
            .Select(g => g.AcademicYear)
            .Distinct()
            .ToListAsync();
        
        return Ok(years);
    }

}
