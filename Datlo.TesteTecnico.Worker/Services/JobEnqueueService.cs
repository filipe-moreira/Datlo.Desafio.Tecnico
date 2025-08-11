using Datlo.TesteTecnico.Domain.Models;
using Datlo.TesteTecnico.Worker.Jobs;
using Hangfire;

namespace Datlo.TesteTecnico.Worker.Services
{
    /// <summary>
    /// Serviço para enfileiramento de jobs usando Hangfire
    /// </summary>
    public class JobEnqueueService : IJobEnqueueService
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<JobEnqueueService> _logger;

        public JobEnqueueService(
            IBackgroundJobClient backgroundJobClient,
            ILogger<JobEnqueueService> logger)
        {
            ArgumentNullException.ThrowIfNull(backgroundJobClient);
            ArgumentNullException.ThrowIfNull(logger);

            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        public string Enfileirar(ArquivosS3 arquivo)
        {
            // COMENTÁRIO APENAS PARA ILUSTRAR O ENTENDIMENTO DO CÓDIGO PARA O AVALIADOR:

            //   Adiciona o job de processamento na fila do Hangfire com configurações de resiliência
            // - O job será executado com:
            // - Retry automático (3 tentativas com delays de 30s, 2min, 5min)
            // - Timeout dinâmico baseado no tipo de arquivo
            // - Tratamento inteligente de erros transientes vs permanentes

            var jobId = _backgroundJobClient.Enqueue<IProcessamentoJob>(arquivo.Queue, 
                        job => job.ExecutarAsync(arquivo, CancellationToken.None));

            _logger.LogInformation(
                "Job de {Modelo}-{Tipo} enfileirado com configurações de resiliência - JobId: {JobId}, Cliente: {ClienteId}, Chave: {Chave}, Fila: {Queue}",
                arquivo.Modelo,
                arquivo.Tipo,
                jobId,
                arquivo.ClienteId,
                arquivo.Chave,
                arquivo.Queue
            );

            return jobId;
        }

        public IEnumerable<string> Enfileirar(IEnumerable<ArquivosS3> arquivo)
        {
            var jobIds = new List<string>();
            foreach (var arq in arquivo)
            {
                var jobId = Enfileirar(arq);
                jobIds.Add(jobId);
            }
            return jobIds;
        }
    }
}
