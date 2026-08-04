using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Whisper.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task InitializeAsync();
}
