using ProjectManager.Application.Common;

namespace ProjectManager.Infrastructure.Common;

public class FileStorageService : IFileStorageService
{
    private readonly string _rootPath = "/app/storage"; 

    public async Task<string> SaveFileAsync(
        Stream fileStream,
        string fileName,
        string projectFolder)
    {
        var projectPath = Path.Combine(_rootPath, projectFolder);

        Directory.CreateDirectory(projectPath);

        var fullPath = Path.Combine(projectPath, fileName);

        using var file = new FileStream(fullPath, FileMode.Create);
        await fileStream.CopyToAsync(file);

        return fullPath;
    }

    public Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }
}