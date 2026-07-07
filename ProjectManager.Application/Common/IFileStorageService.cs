namespace ProjectManager.Application.Common;

public interface IFileStorageService
{
    /// <summary>
    /// Saving file on server
    /// </summary>
    /// <returns>Path to the file</returns>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string projectFolder);
    
    /// <summary>
    /// Deleting file from server
    /// </summary>
    Task DeleteFileAsync(string filePath);
}