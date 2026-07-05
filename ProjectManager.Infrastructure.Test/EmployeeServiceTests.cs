using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Employee;
using ProjectManager.Application.Employee.Dto;
using ProjectManager.Infrastructure.Employee;

namespace ProjectManager.Infrastructure.Test;

public class EmployeeServiceTests
{
    private class FakeEmployeeRepository : IEmployeeRepository
    {
        public Func<EmployeeFilter, Task<(IReadOnlyList<EmployeeItemDto>, int)>>? GetAllHandler;
        public Func<int, Task<EmployeeInfoDto?>>? GetByIdHandler;
        public Func<Entities.Models.Employee, Task>? CreateHandler;
        public Func<string, Task<bool>>? IsEmailExistsHandler;
        public Func<int, Task<bool>>? EmployeeExistsHandler;
        public Func<int, Task<Entities.Models.Employee?>>? GetByIdEntityHandler;

        public Task<(IReadOnlyList<EmployeeItemDto>, int)> GetAllEmployeesAsync(EmployeeFilter filter) =>
            GetAllHandler!(filter);

        public Task<EmployeeInfoDto?> GetEmployeeByIdAsync(int employeeId) => GetByIdHandler != null ? GetByIdHandler(employeeId) : Task.FromResult<EmployeeInfoDto?>(null);

        public Task<bool> IsEmailExists(string email) => IsEmailExistsHandler != null ? IsEmailExistsHandler(email) : Task.FromResult(false);
        public Task<bool> EmployeeExistsAsync(int id) => EmployeeExistsHandler != null ? EmployeeExistsHandler(id) : Task.FromResult(true);
        public Task<bool> HasManagedProjects(int employeeId) => Task.FromResult(false);
        public Task<bool> HasIssues(int employeeId) => Task.FromResult(false);
        public Task<IReadOnlyList<int>> GetEmployeesWithProjectsAsync(IReadOnlyCollection<int> ids) => Task.FromResult((IReadOnlyList<int>)Array.Empty<int>());
        public async Task<Entities.Models.Employee?> GetEntityByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task CreateAsync(Entities.Models.Employee entity) => CreateHandler != null ? CreateHandler(entity) : Task.CompletedTask;
        public Task<Entities.Models.Employee?> GetByIdAsync(int id) => GetByIdEntityHandler != null ? GetByIdEntityHandler(id) : Task.FromResult<Entities.Models.Employee?>(null);
        public Task<int> DeleteByIdAsync(int entityId) => Task.FromResult(0);
        public Task<int> BulkDeleteAsync(IReadOnlyCollection<int> ids) => Task.FromResult(0);
        public Task<int> CountAsync() => Task.FromResult(0);
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    [Fact]
    public async Task CreateEmployee_Success_WithRussianValues()
    {
        var fakeRepo = new FakeEmployeeRepository
        {
            GetAllHandler = filter => Task.FromResult(((IReadOnlyList<EmployeeItemDto>)Array.Empty<EmployeeItemDto>(), 0))
        };

        var fakeEp = new FakeEmployeeProjectRepository();

        var mapperCfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateEmployeeDto, Entities.Models.Employee>();
            cfg.CreateMap<Entities.Models.Employee, EmployeeInfoDto>();
        });

        var mapper = mapperCfg.CreateMapper();

        var created = false;
        fakeRepo.CreateHandler = entity =>
        {
            entity.Id = 101;
            created = true;
            return Task.CompletedTask;
        };

        var service = new EmployeeService(new NullLogger<EmployeeService>(), mapper, fakeRepo, fakeEp);

        var dto = new CreateEmployeeDto { FirstName = "Иван", LastName = "Петров", Patronymic = "Дмитриевич", Email = "ivan.petrov@example.ru" };

        await service.CreateEmployeeAsync(dto);

        Assert.True(created);
    }

    [Fact]
    public async Task AddProjectToEmployee_Conflict_Throws()
    {
        var fakeRepo = new FakeEmployeeRepository
        {
            GetByIdHandler = id => Task.FromResult<EmployeeInfoDto?>(new EmployeeInfoDto { Id = id })
        };

        var fakeEp = new FakeEmployeeProjectRepository();
        fakeEp.ExistsHandler = (emp, proj) => Task.FromResult(true);

        var mapper = new MapperConfiguration(cfg => { cfg.CreateMap<Entities.Models.Employee, EmployeeInfoDto>(); }).CreateMapper();

        var service = new EmployeeService(new NullLogger<EmployeeService>(), mapper, fakeRepo, fakeEp);

        await Assert.ThrowsAsync<ProjectManager.Application.Common.Exceptions.ConflictException>(() => service.AddProjectToEmployeeAsync(1, 1));
    }

    [Fact]
    public async Task BulkInsertProjectsToEmployee_EmployeeNotExists_ThrowsConflict()
    {
        var fakeRepo = new FakeEmployeeRepository
        {
            EmployeeExistsHandler = id => Task.FromResult(false)
        };

        var fakeEp = new FakeEmployeeProjectRepository();

        var mapper = new MapperConfiguration(cfg => { cfg.CreateMap<CreateEmployeeDto, Entities.Models.Employee>(); }).CreateMapper();

        var service = new EmployeeService(new NullLogger<EmployeeService>(), mapper, fakeRepo, fakeEp);

        await Assert.ThrowsAsync<ProjectManager.Application.Common.Exceptions.ConflictException>(() => service.BulkInsertProjectsToEmployeeAsync(new List<int> { 1, 2 }, 5));
    }

    [Fact]
    public async Task UpdateEmployee_Success_ChangesApplied()
    {
        var employee = new Entities.Models.Employee { Id = 5, FirstName = "Алексей", LastName = "Иванов", Email = "a@b.ru" };

        var fakeRepo = new FakeEmployeeRepository
        {
            GetByIdHandler = id => Task.FromResult<EmployeeInfoDto?>(null),
            // implement GetByIdAsync for entity retrieval
        };

        // Provide GetByIdAsync for BaseRepository usage in service
        fakeRepo.GetByIdEntityHandler = id => Task.FromResult<Entities.Models.Employee?>(employee);

        var fakeEp = new FakeEmployeeProjectRepository();

        var mapperCfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UpdateEmployeeDto, Entities.Models.Employee>();
            cfg.CreateMap<Entities.Models.Employee, EmployeeInfoDto>();
        });

        var mapper = mapperCfg.CreateMapper();

        var service = new EmployeeService(new NullLogger<EmployeeService>(), mapper, fakeRepo, fakeEp);

        var dto = new UpdateEmployeeDto { FirstName = "Иван", LastName = "Сидоров", Email = "ivan.sidorov@example.ru" };

        var result = await service.UpdateEmployeeAsync(5, dto);

        Assert.Equal("Иван", result.FirstName);
        Assert.Equal("Сидоров", result.LastName);
    }

    [Fact]
    public async Task DeleteEmployeeById_HasLinks_ThrowsConflict()
    {
        var fakeRepo = new FakeEmployeeRepository();
        var fakeEp = new FakeEmployeeProjectRepository();
        fakeEp.HasAnyLinksForEmployeeHandler = id => Task.FromResult(true);

        var mapper = new MapperConfiguration(cfg => { cfg.CreateMap<Entities.Models.Employee, EmployeeInfoDto>(); }).CreateMapper();

        var service = new EmployeeService(new NullLogger<EmployeeService>(), mapper, fakeRepo, fakeEp);

        await Assert.ThrowsAsync<ProjectManager.Application.Common.Exceptions.ConflictException>(() => service.DeleteEmployeeByIdAsync(1));
    }

    private class FakeEmployeeProjectRepository : ProjectManager.Application.Common.IEmployeeProjectRepository
    {
        public Func<int,int,Task<bool>>? ExistsHandler;
        public Func<int,Task<bool>>? HasAnyLinksForEmployeeHandler;
        public Func<int,Task<bool>>? HasAnyLinksForProjectHandler;

        public Task AddAsync(int employeeId, int projectId) => Task.CompletedTask;
        public Task AddRangeToProjectAsync(int projectId, IReadOnlyList<int> employeesIds) => Task.CompletedTask;
        public Task AddRangeToEmployeeAsync(int employeeId, IReadOnlyList<int> projectIds) => Task.CompletedTask;
        public Task<int> RemoveAsync(int employeeId, int projectId) => Task.FromResult(0);
        public Task<int> DeleteRangeProjectsAsync(int employeeId, IReadOnlyList<int> projectIds) => Task.FromResult(0);
        public Task<int> DeleteRangeEmployeesAsync(int projectId, IReadOnlyList<int> employeesIds) => Task.FromResult(0);
        public Task<bool> ExistsAsync(int employeeId, int projectId) => ExistsHandler != null ? ExistsHandler(employeeId, projectId) : Task.FromResult(false);
        public Task<bool> HasAnyLinksForEmployeeAsync(int employeeId) => HasAnyLinksForEmployeeHandler != null ? HasAnyLinksForEmployeeHandler(employeeId) : Task.FromResult(false);
        public Task<bool> HasAnyLinksForProjectAsync(int projectId) => HasAnyLinksForProjectHandler != null ? HasAnyLinksForProjectHandler(projectId) : Task.FromResult(false);

        public Task CreateAsync(Entities.Models.EmployeeProject entity) => Task.CompletedTask;
        public Task<Entities.Models.EmployeeProject?> GetByIdAsync(int id) => Task.FromResult<Entities.Models.EmployeeProject?>(null);
        public Task<int> DeleteByIdAsync(int entityId) => Task.FromResult(0);
        public Task<int> BulkDeleteAsync(IReadOnlyCollection<int> ids) => Task.FromResult(0);
        public Task<int> CountAsync() => Task.FromResult(0);
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    [Fact]
    public async Task GetAllEmployees_ReturnsPaged()
    {
        var fakeRepo = new FakeEmployeeRepository
        {
            GetAllHandler = filter => Task.FromResult(((IReadOnlyList<EmployeeItemDto>)new[]
            {
                new EmployeeItemDto { Id = 1, FirstName = "John", LastName = "Doe", Email = "a@b.com" }
            }, 1))
        };

        var service = new EmployeeService(new NullLogger<EmployeeService>(), new MapperConfiguration(cfg => { }).CreateMapper(), fakeRepo, new FakeEmployeeProjectRepository());

        var result = await service.GetAllEmployeesAsync(new EmployeeFilter { PageNumber = 1, PageSize = 10 });

        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Employees);
        Assert.Equal(1, result.Employees.First().Id);
    }

    [Fact]
    public async Task GetEmployeeById_ThrowsNotFound()
    {
        var fakeRepo = new FakeEmployeeRepository
        {
            GetByIdHandler = id => Task.FromResult<EmployeeInfoDto?>(null)
        };

        var service = new EmployeeService(new NullLogger<EmployeeService>(), new MapperConfiguration(cfg => { }).CreateMapper(), fakeRepo, new FakeEmployeeProjectRepository());

        await Assert.ThrowsAsync<ProjectManager.Application.Common.Exceptions.NotFoundException>(() => service.GetEmployeeByIdAsync(1));
    }
}
