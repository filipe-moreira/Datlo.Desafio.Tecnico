using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Worker.Jobs
{
    /// <summary>
    /// Interface para jobs de processamento executados pelo Hangfire.
    /// Define o contrato para processamento assíncrono de arquivos.
    /// </summary>
    public interface IProcessamentoJob
    {
        /// <summary>
        /// Executa o processamento de um arquivo específico.
        /// </summary>
        /// <param name="arquivo"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ExecutarAsync(ArquivosS3 arquivo, CancellationToken cancellationToken);
    }
}
