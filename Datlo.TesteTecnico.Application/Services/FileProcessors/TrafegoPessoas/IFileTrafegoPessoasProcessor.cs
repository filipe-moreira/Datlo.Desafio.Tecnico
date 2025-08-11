using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using Datlo.TesteTecnico.Domain.Entidades;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.Contracts
{
    /// <summary>
    /// Interface para processadores de arquivo que encapsulam a lógica específica 
    /// de processamento para diferentes tipos de arquivo (JSON, CSV, etc.).
    /// </summary>
    public interface IFileTrafegoPessoasProcessor
    {
        /// <summary>
        /// Tipo de arquivo que este processador é capaz de processar.
        /// </summary>
        TipoArquivo TipoArquivo { get; }

        /// <summary>
        /// Processa um stream de arquivo usando o mapper fornecido e retorna as entidades processadas.
        /// </summary>
        /// <param name="stream">Stream do arquivo a ser processado</param>
        /// <param name="mapper">Mapper específico do cliente para conversão dos dados</param>
        /// <param name="clienteId">Identificação do cliente</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Enumerável assíncrono das entidades processadas</returns>
        IAsyncEnumerable<TrafegoPessoa> ProcessarAsync(
            Stream stream, 
            ITrafegoPessoaMapper mapper, 
            string clienteId, 
            string identificadorArquivo,
            CancellationToken cancellationToken);
    }
}
