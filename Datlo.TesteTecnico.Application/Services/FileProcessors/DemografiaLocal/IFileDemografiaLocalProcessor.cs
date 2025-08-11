using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal
{
    /// <summary>
    /// Interface para processadores de arquivos de demografia local.
    /// Define o contrato para processamento de diferentes tipos de arquivo (CSV, JSON, XML).
    /// </summary>
    public interface IFileDemografiaLocalProcessor
    {
        /// <summary>
        /// Tipo de arquivo que este processador suporta.
        /// </summary>
        TipoArquivo TipoArquivo { get; }

        /// <summary>
        /// Processa um stream de arquivo usando o mapper fornecido e retorna as entidades processadas.
        /// </summary>
        /// <param name="stream">Stream do arquivo a ser processado</param>
        /// <param name="mapper">Mapper específico do cliente para conversão dos dados</param>
        /// <param name="clienteId">Cliente ID</param>
        /// <param name="identificadorArquivo">Identificador do arquivo para rastreamento</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Enumerável assíncrono das entidades processadas</returns>
        IAsyncEnumerable<Domain.Entidades.DemografiaLocal> ProcessarAsync(
            Stream stream, 
            IDemografiaLocalMapper mapper, 
            string clienteId, 
            string identificadorArquivo,
            CancellationToken cancellationToken);
    }
}
