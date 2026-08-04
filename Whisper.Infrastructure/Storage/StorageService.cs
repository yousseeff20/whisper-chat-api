using Microsoft.Extensions.Configuration;
using Supabase;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Whisper.Application.Common.Interfaces;

namespace Whisper.Infrastructure.Storage;

public class StorageService : IStorageService
{
    private readonly Client _supabaseClient;
    private readonly string _bucketName;

    public StorageService(IConfiguration configuration)
    {
        var url = configuration["Supabase:Url"] ?? "https://your-supabase-url.supabase.co";
        var key = configuration["Supabase:Key"] ?? "your-supabase-anon-key";
        _bucketName = configuration["Supabase:BucketName"] ?? "whisper-bucket";

        var options = new SupabaseOptions { AutoConnectRealtime = false };
        _supabaseClient = new Client(url, key, options);
    }

    public async Task InitializeAsync()
    {
        await _supabaseClient.InitializeAsync();
        
        try
        {
            var bucket = await _supabaseClient.Storage.GetBucket(_bucketName);
            if (bucket == null)
            {
                await _supabaseClient.Storage.CreateBucket(_bucketName, new Supabase.Storage.BucketUpsertOptions { Public = true });
            }
        }
        catch
        {
            // Usually occurs if bucket doesn't exist and we hit a 404, so we create it.
            try
            {
                await _supabaseClient.Storage.CreateBucket(_bucketName, new Supabase.Storage.BucketUpsertOptions { Public = true });
            }
            catch
            {
                // Ignored for environments without valid credentials yet
            }
        }
    }

    public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream, cancellationToken);
        var bytes = memoryStream.ToArray();

        await _supabaseClient.Storage.From(_bucketName).Upload(bytes, uniqueFileName, new Supabase.Storage.FileOptions { ContentType = contentType });

        var publicUrl = _supabaseClient.Storage.From(_bucketName).GetPublicUrl(uniqueFileName);
        return publicUrl;
    }

    public async Task DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;
        
        // Extract filename from URL
        var uri = new Uri(fileUrl);
        var fileName = Path.GetFileName(uri.LocalPath);

        await _supabaseClient.Storage.From(_bucketName).Remove(new List<string> { fileName });
    }
}
