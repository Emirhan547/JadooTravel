using Amazon.S3;
using Amazon.S3.Model;
using JadooTravel.UI.Options;
using Microsoft.Extensions.Options;

namespace JadooTravel.UI.Services;

public class AwsS3Service : IAwsS3Service
{
    private static readonly HashSet<string> AllowedImageContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif",
        "image/svg+xml"
    ];

    private readonly IAmazonS3 _amazonS3;
    private readonly AwsS3Options _awsS3Options;

    public AwsS3Service(IAmazonS3 amazonS3, IOptions<AwsS3Options> awsS3Options)
    {
        _amazonS3 = amazonS3;
        _awsS3Options = awsS3Options.Value;
    }

    public async Task<string> UploadImageAsync(IFormFile file, string? folder = null, CancellationToken cancellationToken = default)
    {
        if (file.Length <= 0)
            throw new InvalidOperationException("Boş dosya yüklenemez.");

        if (string.IsNullOrWhiteSpace(file.ContentType) || !AllowedImageContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            throw new InvalidOperationException("Sadece görsel dosyaları yüklenebilir.");

        if (string.IsNullOrWhiteSpace(_awsS3Options.BucketName))
            throw new InvalidOperationException("AWS S3 BucketName ayarı zorunludur.");

        var baseFolder = folder ?? _awsS3Options.UploadFolder;
        var sanitizedExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var objectKey = $"{baseFolder.Trim('/')}/{Guid.NewGuid():N}{sanitizedExtension}";

        await using var stream = file.OpenReadStream();
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = _awsS3Options.BucketName,
            Key = objectKey,
            InputStream = stream,
            ContentType = file.ContentType
        };

        await _amazonS3.PutObjectAsync(putObjectRequest, cancellationToken);

        if (!string.IsNullOrWhiteSpace(_awsS3Options.PublicBaseUrl))
            return $"{_awsS3Options.PublicBaseUrl.TrimEnd('/')}/{objectKey}";

        var regionSegment = string.IsNullOrWhiteSpace(_awsS3Options.Region) ? "us-east-1" : _awsS3Options.Region;
        return $"https://{_awsS3Options.BucketName}.s3.{regionSegment}.amazonaws.com/{objectKey}";
    }
}