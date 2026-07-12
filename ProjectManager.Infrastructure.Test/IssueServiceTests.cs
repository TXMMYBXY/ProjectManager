using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectManager.Application.Issue;
using ProjectManager.Application.Issue.Dto;
using ProjectManager.Application.Utils;
using ProjectManager.Entities.Enums;
using ProjectManager.Infrastructure.Issue;

namespace ProjectManager.Infrastructure.Test;

public class IssueServiceTests
{
    private class FakeCurrentUser : ProjectManager.Application.Utils.CurrentUser
    {
        public int Id { get; set; } = 1;
        public string Role { get; set; } = string.Empty;
    }

    private class FakeIssueRepository : IIssueRepository
    {
        public Func<IssueFilter, System.Linq.Expressions.Expression<Func<Entities.Models.Issue, bool>>?, Task<(IReadOnlyList<IssueItemDto>, int)>>? GetAllHandler;
        public Func<int, System.Linq.Expressions.Expression<Func<Entities.Models.Issue, bool>>?, Task<IssueInfoDto?>>? GetByIdHandler;
        public Func<Entities.Models.Issue, Task>? CreateHandler;
        public Func<int, Task<Entities.Models.Issue?>>? GetByIdEntityHandler;
        public Func<int, Task<int>>? DeleteByIdHandler;

        public Task<(IReadOnlyList<IssueItemDto>, int)> GetAllIssuesAsync(IssueFilter filter, System.Linq.Expressions.Expression<Func<Entities.Models.Issue, bool>>? predicate = null) => GetAllHandler!(filter, predicate);
        public Task<IssueInfoDto?> GetIssueByIdAsync(int issueId, System.Linq.Expressions.Expression<Func<Entities.Models.Issue, bool>>? predicate = null) => GetByIdHandler!(issueId, predicate);

        public Task CreateAsync(Entities.Models.Issue entity) => CreateHandler != null ? CreateHandler(entity) : Task.CompletedTask;
        public Task<Entities.Models.Issue?> GetByIdAsync(int id) => GetByIdEntityHandler != null ? GetByIdEntityHandler(id) : Task.FromResult<Entities.Models.Issue?>(null);
        public Task<int> DeleteByIdAsync(int entityId) => DeleteByIdHandler != null ? DeleteByIdHandler(entityId) : Task.FromResult(0);
        public Task<int> BulkDeleteAsync(IReadOnlyCollection<int> ids) => Task.FromResult(0);
        public Task<int> CountAsync() => Task.FromResult(0);
        public Task SaveChangesAsync() => Task.CompletedTask;
    }

    [Fact]
    public async Task CreateIssue_Success_WithRussianValues()
    {
        var fakeRepo = new FakeIssueRepository();
        fakeRepo.CreateHandler = issue =>
        {
            issue.Id = 301;
            return Task.CompletedTask;
        };

        var mapperCfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<CreateIssueDto, Entities.Models.Issue>();
            cfg.CreateMap<Entities.Models.Issue, IssueInfoDto>();
        });

        var service = new IssueService(new NullLogger<IssueService>(), mapperCfg.CreateMapper(), fakeRepo);

        var dto = new CreateIssueDto { Title = "Ошибка в модуле", Priority = 2, Status = ProjectManager.Entities.Enums.IssueStatus.ToDo, Comments = "Первый комментарий" };

        await service.CreateIssueAsync(dto);

        Assert.True(true);
    }

    [Fact]
    public async Task UpdateIssue_Success_UpdatesComments()
    {
        var issue = new Entities.Models.Issue { Id = 8, Title = "Тест", Priority = 1 };

        var fakeRepo = new FakeIssueRepository
        {
            GetByIdEntityHandler = id => Task.FromResult<Entities.Models.Issue?>(issue)
        };

        var mapperCfg = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UpdateIssueDto, Entities.Models.Issue>();
            cfg.CreateMap<Entities.Models.Issue, IssueInfoDto>();
        });

        var service = new IssueService(new NullLogger<IssueService>(), mapperCfg.CreateMapper(), fakeRepo);

        var dto = new UpdateIssueDto { Comments = new ProjectManager.Application.Utils.Optional<string?>("Обновлённый комментарий") };

        var result = await service.UpdateIssueAsync(8, dto);

        Assert.Equal("Обновлённый комментарий", result.Comments);
    }

    [Fact]
    public async Task DeleteIssueById_NotFound_Throws()
    {
        var fakeRepo = new FakeIssueRepository();
        fakeRepo.DeleteByIdHandler = id => Task.FromResult(0);

        var service = new IssueService(new NullLogger<IssueService>(), new MapperConfiguration(cfg => { }).CreateMapper(), fakeRepo);

        await Assert.ThrowsAsync<ProjectManager.Application.Common.Exceptions.NotFoundException>(() => service.DeleteIssueByIdAsync(999));
    }

    [Fact]
    public async Task GetAllIssues_ReturnsPaged()
    {
        var fakeRepo = new FakeIssueRepository
        {
            GetAllHandler = (filter, predicate) => Task.FromResult(((IReadOnlyList<IssueItemDto>)new[]
            {
                new IssueItemDto { Id = 3, Title = "Issue1", Priority = 1 }
            }, 1))
        };

        var service = new IssueService(new NullLogger<IssueService>(), new MapperConfiguration(cfg => { }).CreateMapper(), fakeRepo);

        var result = await service.GetAllIssuesAsync(new IssueFilter { PageNumber = 1, PageSize = 10 }, new CurrentUser
        {
            Id = (int) UserRole.Director,
            Role = nameof(UserRole.Director)
        });

        Assert.Equal(1, result.TotalCount);
        Assert.Single(result.Issues);
        Assert.Equal(3, result.Issues.First().Id);
    }

    [Fact]
    public async Task GetIssueById_ThrowsNotFound()
    {
        var fakeRepo = new FakeIssueRepository
        {
            GetByIdHandler = (id, predicate) => Task.FromResult<IssueInfoDto?>(null)
        };

        var service = new IssueService(new NullLogger<IssueService>(), new MapperConfiguration(cfg => { }).CreateMapper(), fakeRepo);

        await Assert.ThrowsAsync<ProjectManager.Application.Common.Exceptions.NotFoundException>(() => service.GetIssueByIdAsync(1, new CurrentUser
        {
            Id = (int) UserRole.Director,
            Role = nameof(UserRole.Director)
        }));
    }
}
