using Amazon;
using Amazon.S3;
using Datlo.TesteTecnico.Domain.Interfaces;
using Datlo.TesteTecnico.Infrastructure.Services;

namespace Datlo.TesteTecnico.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura os serviços de armazenamento (S3)
    /// </summary>
    public static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            var region = RegionEndpoint.GetBySystemName(cfg["AWS:Region"]);
            var ak = cfg["AWS:AccessKeyId"];
            var sk = cfg["AWS:SecretAccessKey"];
            return new AmazonS3Client(ak, sk, region);
        });
        
        services.AddScoped<IS3Services, S3Services>();

        return services;
    }
}
