using Amazon;
using Amazon.S3;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Csv.ClienteC;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Csv.ClienteD;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Json.ClienteA;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Json.ClienteB;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Xml.ClienteE;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Csv.ClienteC;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Csv.ClienteD;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Json.ClienteA;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Json.ClienteB;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Xml.ClienteE;
using Datlo.TesteTecnico.Application.Ports;
using Datlo.TesteTecnico.Application.Repositories;
using Datlo.TesteTecnico.Application.Services.FileProcessors;
using Datlo.TesteTecnico.Application.Services.FileProcessors.Contracts;
using Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal;
using Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal.Csv;
using Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal.Json;
using Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal.Xml;
using Datlo.TesteTecnico.Application.Services.FileProcessors.TrafegoPessoas;
using Datlo.TesteTecnico.Application.Services.FileProcessors.TrafegoPessoas.Csv;
using Datlo.TesteTecnico.Infrastructure.Repositories;
using Datlo.TesteTecnico.Infrastructure.Services;
using Datlo.TesteTecnico.Worker.Jobs;
using Datlo.TesteTecnico.Worker.Options;
using Datlo.TesteTecnico.Worker.Services;
using Hangfire;
using Hangfire.PostgreSql;
using Npgsql;

namespace Datlo.TesteTecnico.Worker.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configura todas as dependências do Worker
    /// </summary>
    public static IServiceCollection AddWorkerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseServices(configuration);
        services.AddStorageServices(configuration);
        services.AddMapperServices();
        services.AddFileProcessorServices();
        services.AddUseCaseServices();
        services.AddHangfireServices(configuration);
        services.AddJobServices(configuration);
        services.AddWorkerHostedServices();
        services.AddLoggingServices();

        return services;
    }

    /// <summary>
    /// Configura os serviços de banco de dados
    /// </summary>
    private static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Conexão com banco de dados
        services.AddScoped(sp => new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));

        // Repositórios
        services.AddScoped<ITrafegoPessoaRepository, TrafegoPessoaRepository>();
        services.AddScoped<IDemografiaLocalRepository, DemografiaLocalRepository>();

        return services;
    }

    /// <summary>
    /// Configura os serviços de armazenamento (S3)
    /// </summary>
    private static IServiceCollection AddStorageServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAmazonS3>(sp =>
        {
            var cfg = sp.GetRequiredService<IConfiguration>();
            var region = RegionEndpoint.GetBySystemName(cfg["AWS:Region"]);
            var ak = cfg["AWS:AccessKeyId"];
            var sk = cfg["AWS:SecretAccessKey"];
            return new AmazonS3Client(ak, sk, region);
        });

        services.AddScoped<IObjectStorage, S3ObjectStorage>();

        return services;
    }

    /// <summary>
    /// Configura os mappers para TrafegoPessoas e DemografiaLocal
    /// </summary>
    private static IServiceCollection AddMapperServices(this IServiceCollection services)
    {
        // Mappers - TrafegoPessoas
        services.AddSingleton<ITrafegoPessoaMapper, TrafegoPessoaMapperClienteA>();
        services.AddSingleton<ITrafegoPessoaMapper, TrafegoPessoaMapperClienteB>();
        services.AddSingleton<ITrafegoPessoaMapper, TrafegoPessoaMapperClienteC>();
        services.AddSingleton<ITrafegoPessoaMapper, TrafegoPessoaMapperClienteD>();
        services.AddSingleton<ITrafegoPessoaMapper, TrafegoPessoaMapperClienteE>();
        services.AddSingleton<ITrafegoPessoaMapperRegistry, TrafegoPessoaMapperRegistry>();

        // Mappers - DemografiaLocal
        services.AddSingleton<IDemografiaLocalMapper, DemografiaLocalMapperClienteA>();
        services.AddSingleton<IDemografiaLocalMapper, DemografiaLocalMapperClienteB>();
        services.AddSingleton<IDemografiaLocalMapper, DemografiaLocalMapperClienteC>();
        services.AddSingleton<IDemografiaLocalMapper, DemografiaLocalMapperClienteD>();
        services.AddSingleton<IDemografiaLocalMapper, DemografiaLocalMapperClienteE>();
        services.AddSingleton<IDemografiaLocalMapperRegistry, DemografiaLocalMapperRegistry>();

        return services;
    }

    /// <summary>
    /// Configura os processadores de arquivo
    /// </summary>
    private static IServiceCollection AddFileProcessorServices(this IServiceCollection services)
    {
        // File Processors - TrafegoPessoas
        services.AddSingleton<IFileTrafegoPessoasProcessor, TrafegoPessoaJsonFileProcessor>();
        services.AddSingleton<IFileTrafegoPessoasProcessor, TrafegoPessoaCsvFileProcessor>();
        services.AddSingleton<IFileTrafegoPessoasProcessor, TrafegoPessoaXmlFileProcessor>();
        services.AddSingleton<IFileTrafegoPessoasProcessorFactory, FileTrafegoPessoasProcessorFactory>();

        // File Processors - DemografiaLocal
        services.AddSingleton<IFileDemografiaLocalProcessor, DemografiaLocalJsonFileProcessor>();
        services.AddSingleton<IFileDemografiaLocalProcessor, DemografiaLocalCsvFileProcessor>();
        services.AddSingleton<IFileDemografiaLocalProcessor, DemografiaLocalXmlFileProcessor>();
        services.AddSingleton<IFileDemografiaLocalProcessorFactory, FileDemografiaLocalProcessorFactory>();

        // Main File Processor Factory
        services.AddSingleton<IFileProcessorFactory, FileProcessorFactory>();

        return services;
    }

    /// <summary>
    /// Configura os casos de uso
    /// </summary>
    private static IServiceCollection AddUseCaseServices(this IServiceCollection services)
    {
        services.AddScoped<ProcessarTrafegoPessoaUseCase>();
        services.AddScoped<ProcessarDemografiaLocalUseCase>();

        return services;
    }

    /// <summary>
    /// Configura o Hangfire
    /// </summary>
    private static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        var hangfireConnectionString = configuration.GetConnectionString("HangfireConnection");
        
        var allConnectionStrings = configuration.GetSection("ConnectionStrings").GetChildren();
        
        if (string.IsNullOrEmpty(hangfireConnectionString))
        {
            throw new InvalidOperationException($"Connection string 'HangfireConnection' não encontrada no appsettings.json.");
        }

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(hangfireConnectionString, new PostgreSqlStorageOptions
            {
                QueuePollInterval = TimeSpan.FromSeconds(10),
                JobExpirationCheckInterval = TimeSpan.FromHours(1),
                CountersAggregateInterval = TimeSpan.FromMinutes(5),
                PrepareSchemaIfNecessary = true,
                TransactionSynchronisationTimeout = TimeSpan.FromMinutes(5),
                SchemaName = "hangfire"
            }));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = configuration.GetValue<int>("Hangfire:WorkerCount", 5);
            options.Queues = configuration.GetSection("Hangfire:Queues").Get<string[]>() ?? new[] { "default" };
        });

        return services;
    }

    /// <summary>
    /// Configura os jobs e serviços relacionados
    /// </summary>
    private static IServiceCollection AddJobServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ProcessamentoJobOptions>(configuration.GetSection("Jobs:ProcessamentoJob"));

        services.AddScoped<IProcessamentoJob, ProcessamentoJob>();
        services.AddSingleton<IJobEnqueueService, JobEnqueueService>();

        return services;
    }

    /// <summary>
    /// Configura os serviços hospedados (Workers)
    /// </summary>
    private static IServiceCollection AddWorkerHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<WorkerDeProcessamento>();

        return services;
    }

    /// <summary>
    /// Configura os serviços de logging
    /// </summary>
    private static IServiceCollection AddLoggingServices(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddConsole();
        });

        return services;
    }
}
