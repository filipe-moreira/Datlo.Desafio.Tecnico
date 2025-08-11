using Datlo.TesteTecnico.Domain.Models;
using Datlo.TesteTecnico.Worker.Services;

namespace Datlo.TesteTecnico.Worker;

/// <summary>
/// Worker que roda em background para processar arquivos da S3 usando Hangfire
/// </summary>
public class WorkerDeProcessamento : BackgroundService
{    
    private readonly IJobEnqueueService _jobEnqueueService;
    private readonly ILogger<WorkerDeProcessamento> _logger;
    private readonly IConfiguration _configuration;

    // Configurações do worker
    private readonly TimeSpan _intervaloVerificacao;
    private readonly int _maxArquivosSimultaneos;

    public WorkerDeProcessamento(        
        IJobEnqueueService jobEnqueueService,
        ILogger<WorkerDeProcessamento> logger,
        IConfiguration configuration)
    {        
        ArgumentNullException.ThrowIfNull(jobEnqueueService, nameof(jobEnqueueService));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        
        _jobEnqueueService = jobEnqueueService;
        _logger = logger;
        _configuration = configuration;

        _intervaloVerificacao = TimeSpan.FromMinutes(_configuration.GetValue<int>("Worker:IntervaloVerificacaoMinutos", 1));
        _maxArquivosSimultaneos = _configuration.GetValue<int>("Worker:MaxArquivosSimultaneos", 3);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker de processamento com Hangfire iniciado");
        _logger.LogInformation("Configurações: Intervalo={IntervaloMinutos}min, MaxSimultaneos={MaxSimultaneos}",
            _intervaloVerificacao.TotalMinutes, _maxArquivosSimultaneos);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Aqui seria implementada lógica para:
                // 1. Verificar novos arquivos no S3
                // 2. Enfileirar jobs automaticamente
                // 3. Monitorar status dos jobs

                // Por enquanto, apenas aguarda o próximo ciclo
                // Em uma implementação real, seria adicionada lógica para:
                // - Listar arquivos no S3
                // - Verificar se já foram processados
                // - Enfileirar novos jobs

                _logger.LogDebug("Worker verificando por novos arquivos...");

                var arquivosParaProcessar = new List<ArquivosS3>(); // Aqui implementaria a lógica para buscar arquivos do S3

                _jobEnqueueService.Enfileirar(arquivosParaProcessar);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante execução do worker de processamento");
            }

            // Aguarda o intervalo antes da próxima verificação
            try
            {
                await Task.Delay(_intervaloVerificacao, stoppingToken);
            }
            catch (OperationCanceledException)
            {                
                break;
            }
        }

        _logger.LogInformation("Worker de processamento finalizado");
    }
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Parando worker de processamento...");
        await base.StopAsync(cancellationToken);
        _logger.LogInformation("Worker de processamento parado");
    }
}
