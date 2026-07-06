using System.Linq.Expressions;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Project;
using ProjectManager.Application.Project.Dto;
using ProjectManager.Infrastructure.Project;

namespace ProjectManager.Infrastructure.Test;

public class ProjectServiceTests
{
    private class FakeCurrentUser : ProjectManager.Application.Common.ICurrentUser
    {
        public int Id { get; set; } = 1;
        public bool IsInRole(string role) => false;
    }

    private class FakeProjectRepository : IProjectRepository
    {
        public Func<ProjectFilter, Expression<Func<Entities.Models.Project, bool>>?, Task<(IReadOnlyList<ProjectItemDto>, int)>>? GetAllHandler;
        public Func<int, Task<ProjectInfoDto?>>? GetByIdHandler;
        public Func<Entities.Models.Project, Task>? CreateHandler;
        public Func<int, Task<Entities.Models.Project?>>? GetByIdEntityHandler;
        public Func<int, Task<bool>>? ProjectExistsHandler;
        public Func<int, Task<bool>>? HasManagersHandler;
        public Func<int, Task<bool>>? HasIssuesHandler;

        public Task<(IReadOnlyList<ProjectItemDto>, int)> GetAllProjectsAsync(ProjectFilter filter, Expression<Func<Entities.Models.Project, bool>>? predicate = null) => GetAllHandler!(filter, predicate);
        public Task<ProjectInfoDto?> GetProjectByIdAsync(int projectId) => GetByIdHandler!(projectId);

        public Task CreateAsync(Entities.Models.Project entity) => CreateHandler != null ? CreateHandler(entity) : Task.CompletedTask;
        public Task<Entities.Models.Project?> GetByIdAsync(int id) => GetByIdEntityHandler != null ? GetByIdEntityHandler(id) : Task.FromResult<Entities.Models.Project?>(null);
        public Task<int> DeleteByIdAsync(int entityId) => Task.FromResult(0);
        public Task<int> BulkDeleteAsync(IReadOnlyCollection<int> ids) => Task.FromResult(0);
        public Task<int> CountAsync() => Task.FromResult(0);
        public Task SaveChangesAsync() => Task.CompletedTask;
        public Task<bool> ProjectExistsAsync(int id) => ProjectExistsHandler != null ? ProjectExistsHandler(id) : Task.FromResult(true);
        public Task<bool> HasManagers(int id) => HasManagersHandler != null ? HasManagersHandler(id) : Task.FromResult(false);
        public Task<bool> HasIssues(int id) => HasIssuesHandler != null ? HasIssuesHandler(id) : Task.FromResult(false);
    }

    [Fact]
    public async Task CreateProject_Success_WithRussianValues()
    {
        var fakeRepo = new FakeProjectRepository();
        fakeRepo.CreateHandler = p =>
        {
            p.Id = 201;
            return Task.CompletedTask;
        };

        var fakeEp = new FakeEmployeeProjectRepository();

        var mapperCfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateProjectDto, Entities.Models.Project>();
            cfg.CreateMap<Entities.Models.Project, ProjectInfoDto>();
        });

        var service = new ProjectService(new NullLogger<ProjectService>(), mapperCfg.CreateMapper(), new FakeCurrentUser(), fakeRepo, fakeEp);

        var dto = new CreateProjectDto { Title = "Проект А", CompanyCustomer = "Заказчик А", CompanyExecutor = "Исполнитель А", FinishDate = DateTime.UtcNow.AddDays(30), Priority = 1, ProjectManagerId = 1 };

        await service.CreateProjectAsync(dto);

        // if no exception — success
        Assert.True(true);
    }

    [Fact]
    public async Task AssignEmployeeToProject_Conflict_Throws()
    {
        var fakeRepo = new FakeProjectRepository
        {
            GetByIdHandler = id => Task.FromResult<ProjectInfoDto?>(new ProjectInfoDto { Id = id })
        };

        var fakeEp = new FakeEmployeeProjectRepository();
        fakeEp.ExistsHandler = (emp, proj) => Task.FromResult(true);

        var mapper = new MapperConfiguration(cfg => { cfg.CreateMap<Entities.Models.Project, ProjectInfoDto>(); }).CreateMapper();

        var service = new ProjectService(new NullLogger<ProjectService>(), mapper, new FakeCurrentUser(), fakeRepo, fakeEp);

        await Assert.ThrowsAsync<ProjectManager.Application.Common.Exceptions.ConflictException>(() => service.AssignEmployeeToProjectAsync(1, 1));
    }

    [Fact]
    public async Task DeleteProjectById_HasLinks_ThrowsConflict()
    {
        var fakeRepo = new FakeProjectRepository();
        var fakeEp = new FakeEmployeeProjectRepository();
        fakeEp.HasAnyLinksForProjectHandler = id => Task.FromResult(true);

        var service = new ProjectService(new NullLogger<ProjectService>(), new MapperConfiguration(cfg => { }).CreateMapper(), new FakeCurrentUser(), fakeRepo, fakeEp);

        await Assert.ThrowsAsync<ProjectManager.Application.Common.Exceptions.ConflictException>(() => service.DeleteProjectByIdAsync(1));
    }

    [Fact]
    public async Task UpdateProject_Success_ChangesApplied()
    {
        var project = new Entities.Models.Project { Id = 7, Title = "Old", CompanyCustomer = "X", CompanyExecutor = "Y" };

        var fakeRepo = new FakeProjectRepository
        {
            GetByIdEntityHandler = id => Task.FromResult<Entities.Models.Project?>(project)
        };

        var fakeEp = new FakeEmployeeProjectRepository();

        var mapperCfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UpdateProjectDto, Entities.Models.Project>();
            cfg.CreateMap<Entities.Models.Project, ProjectInfoDto>();
        });

        var service = new ProjectService(new NullLogger<ProjectService>(), mapperCfg.CreateMapper(), new FakeCurrentUser(), fakeRepo, fakeEp);

        var dto = new UpdateProjectDto { Title = "Проект Б", CompanyCustomer = "Заказчик Б", CompanyExecutor = "Исполнитель Б" };

        var result = await service.UpdateProjectAsync(7, dto);

        Assert.Equal("Проект Б", result.Title);
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
    public async Task GetAllProjects_ReturnsPaged()
    {
        var fakeRepo = new FakeProjectRepository
        {
            GetAllHandler = (filter, predicate) => Task.FromResult(((IReadOnlyList<ProjectItemDto>)new[]
            {
                new ProjectItemDto { Id = 2, Title = "P1", CompanyCustomer = "C", CompanyExecuter = "E", StartDate = DateTime.UtcNow }
            }, 1))
        };

        var service = new ProjectService(new NullLogger<ProjectService>(), new MapperConfiguration(cfg => { }).CreateMapper(), new FakeCurrentUser(), fakeRepo, new FakeEmployeeProjectRepository());

        var result = await service.GetAllProjectsAsync(new ProjectFilter { PageNumber = 1, PageSize = 10 });

        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Projects);
        Assert.Equal(2, result.Projects.First().Id);
    }

    [Fact]
    public async Task GetProjectById_ThrowsNotFound()
    {
        var fakeRepo = new FakeProjectRepository
        {
            GetByIdHandler = id => Task.FromResult<ProjectInfoDto?>(null)
        };

        var service = new ProjectService(new NullLogger<ProjectService>(), new MapperConfiguration(cfg => { }).CreateMapper(), new FakeCurrentUser(), fakeRepo, new FakeEmployeeProjectRepository());

        await Assert.ThrowsAsync<ProjectManager.Application.Common.Exceptions.NotFoundException>(() => service.GetProjectByIdAsync(1));
    }
}
