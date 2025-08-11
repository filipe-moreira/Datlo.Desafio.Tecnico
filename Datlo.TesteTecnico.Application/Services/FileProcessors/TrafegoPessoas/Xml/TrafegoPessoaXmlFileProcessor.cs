using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using Datlo.TesteTecnico.Application.Services.FileProcessors.Contracts;
using Datlo.TesteTecnico.Domain.Entidades;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors
{
    /// <summary>
    /// Exemplo de processador para arquivos XML.    
    /// </summary>
    public sealed class TrafegoPessoaXmlFileProcessor : IFileTrafegoPessoasProcessor
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Xml;

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
