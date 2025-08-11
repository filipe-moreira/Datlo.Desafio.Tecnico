using Datlo.TesteTecnico.Application.Services.FileProcessors;
using Datlo.TesteTecnico.Domain.Models;
using Datlo.TesteTecnico.Worker.Options;
using Hangfire;
using Microsoft.Extensions.Options;
using System.ComponentModel;
using System.Net.Sockets;

namespace Datlo.TesteTecnico.Worker.Jobs
{
    /// <summary>
    /// Implementação do job de processamento para o Hangfire.
    /// Responsável por executar o processamento de arquivos de forma assíncrona.
    /// Configurado com políticas de resiliência, timeout e retry.
    /// </summary>
    public class ProcessamentoJob : IProcessamentoJob
    {
        private readonly ILogger<ProcessamentoJob> _logger;
        private readonly IFileProcessorFactory _fileProcessorFactory;        
        private readonly ProcessamentoJobOptions _options;

        public ProcessamentoJob(
            IFileProcessorFactory fileProcessorFactory,
            ILogger<ProcessamentoJob> logger,
            IConfiguration configuration,
            IOptions<ProcessamentoJobOptions> options)
        {
            ArgumentNullException.ThrowIfNull(fileProcessorFactory);
            ArgumentNullException.ThrowIfNull(logger);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(options);

            _fileProcessorFactory = fileProcessorFactory;
            _logger = logger;            
            _options = options.Value;
        }

        /// <summary>
        /// Executa o processamento de um arquivo específico com configurações de resiliência.
        /// Configurado com timeout dinâmico, retry automático e tratamento de erros específicos.
        /// </summary>
        /// <param name="arquivo">Arquivo a ser processado</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns></returns>
        [DisplayName("Processamento de Arquivo - {0}")]
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new int[] { 30, 120, 300 })]
        public async Task ExecutarAsync(ArquivosS3 arquivo, CancellationToken cancellationToken)
        {
            var timeoutMinutos = ObterTimeoutPorTipoArquivo(arquivo.Modelo);

            _logger.LogInformation(
                "Iniciando processamento do arquivo - Cliente: {Cliente}, Bucket: {Bucket}, Chave: {Chave}, Timeout: {Timeout}min",
                arquivo.ClienteId, arquivo.Bucket, arquivo.Chave, timeoutMinutos);

            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMinutes(timeoutMinutos));
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            try
            {
                // Determina o tipo de processamento baseado na chave do arquivo
                var processor = _fileProcessorFactory.GetProcessor(arquivo);

                if (processor == null)
                {
                    var errorMsg = $"Nenhum processador encontrado para o arquivo - Cliente: {arquivo.ClienteId}, Chave: {arquivo.Chave}";
                    _logger.LogError(errorMsg);
                    throw new InvalidOperationException(errorMsg); // Erro não transiente - não deve fazer retry
                }

                // Executa o processamento com timeout
                await processor.ExecutarAsync(arquivo, combinedCts.Token);

                _logger.LogInformation(
                    "Processamento concluído com sucesso - Cliente: {Cliente}, Bucket: {Bucket}, Chave: {Chave}",
                    arquivo.ClienteId, arquivo.Bucket, arquivo.Chave);
            }
            catch (OperationCanceledException) when (timeoutCts.Token.IsCancellationRequested)
            {
                var errorMsg = $"Processamento excedeu timeout de {timeoutMinutos} minutos";
                _logger.LogError(errorMsg + " - Cliente: {Cliente}, Bucket: {Bucket}, Chave: {Chave}",
                    arquivo.ClienteId, arquivo.Bucket, arquivo.Chave);
                throw new TimeoutException(errorMsg); // Erro transiente - pode fazer retry
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Processamento cancelado externamente - Cliente: {Cliente}, Bucket: {Bucket}, Chave: {Chave}",
                    arquivo.ClienteId, arquivo.Bucket, arquivo.Chave);
                throw; // Não deve fazer retry se foi cancelado externamente
            }
            catch (InvalidOperationException ex)
            {
                // Erros de configuração/validação - não devem fazer retry
                _logger.LogError(ex,
                    "Erro de configuração durante processamento - Cliente: {Cliente}, Bucket: {Bucket}, Chave: {Chave}",
                    arquivo.ClienteId, arquivo.Bucket, arquivo.Chave);
                throw; // Não faz retry
            }
            catch (Exception ex) when (IsTransientError(ex))
            {
                // Erros transientes - podem fazer retry
                _logger.LogWarning(ex,
                    "Erro transiente durante processamento - Cliente: {Cliente}, Bucket: {Bucket}, Chave: {Chave}. Será feito retry.",
                    arquivo.ClienteId, arquivo.Bucket, arquivo.Chave);
                throw; // Hangfire fará retry automaticamente
            }
            catch (Exception ex)
            {
                // Outros erros - log como erro crítico mas ainda permite retry
                _logger.LogError(ex,
                    "Erro crítico durante processamento - Cliente: {Cliente}, Bucket: {Bucket}, Chave: {Chave}",
                    arquivo.ClienteId, arquivo.Bucket, arquivo.Chave);
                throw;
            }
        }

        /// <summary>
        /// Obtém o timeout apropriado baseado no tipo de arquivo
        /// </summary>
        private int ObterTimeoutPorTipoArquivo(ModeloArquivo modelo) => modelo switch
        {
            ModeloArquivo.TrafegoPessoas => _options.TimeoutPorTipoArquivo.TrafegoPessoas,
            ModeloArquivo.DemografiaLocal => _options.TimeoutPorTipoArquivo.DemografiaLocal,
            _ => _options.TimeoutPorTipoArquivo.Default
        };

        /// <summary>
        /// Determina se um erro é transiente e deve ser reprocessado
        /// </summary>
        private static bool IsTransientError(Exception ex)
        {
            return ex switch
            {
                TimeoutException => true,
                HttpRequestException => true,
                TaskCanceledException => true,
                SocketException => true,
                _ when ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) => true,
                _ when ex.Message.Contains("network", StringComparison.OrdinalIgnoreCase) => true,
                _ when ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase) => true,
                _ => false
            };
        }
    }
}