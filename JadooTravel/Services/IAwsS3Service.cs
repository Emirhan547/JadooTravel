using Microsoft.AspNetCore.Http;

namespace JadooTravel.UI.Services;

public interface IAwsS3Service
{
    Task<string> UploadImageAsync(IFormFile file, string? folder = null, CancellationToken cancellationToken = default);
}