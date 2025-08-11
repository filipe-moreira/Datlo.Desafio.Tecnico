using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using Datlo.TesteTecnico.Application.Services.FileProcessors.Contracts;
using Datlo.TesteTecnico.Domain.Entidades;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors
{
    /// <summary>
    /// Processador específico para arquivos JSON.
    /// Encapsula toda a lógica de processamento de arquivos JSON.
    /// </summary>
    public sealed class TrafegoPessoaJsonFileProcessor : IFileTrafegoPessoasProcessor
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Json;

        public IAsyncEnumerable<TrafegoPessoa> ProcessarAsync(
            Stream stream, 
            ITrafegoPessoaMapper mapper, 
            string clienteId, 
            string identificadorArquivo, 
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
