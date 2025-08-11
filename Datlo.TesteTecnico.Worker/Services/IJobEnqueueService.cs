using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Worker.Services
{
    /// <summary>
    /// Interface para enfileiramento de jobs de processamento
    /// </summary>
    public interface IJobEnqueueService
    {
        /// <summary>
        /// Enfileira um job de processamento usando Hangfire com pol�ticas de resili�ncia
        /// </summary>
        /// <param name="arquivo"></param>
        /// <returns></returns>
        string Enfileirar(ArquivosS3 arquivo);

        /// <summary>
        /// Enfileira m�ltiplos jobs de processamento usando Hangfire com pol�ticas de resili�ncia
        /// </summary>
        /// <param name="arquivo"></param>
        /// <returns></returns>
        IEnumerable<string> Enfileirar(IEnumerable<ArquivosS3> arquivo);       
    }
}
