using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal.Json
{
    /// <summary>
    /// Processador específico para arquivos JSON de demografia local.
    /// Encapsula toda a lógica de processamento de arquivos JSON.
    /// </summary>
    public sealed class DemografiaLocalJsonFileProcessor : IFileDemografiaLocalProcessor
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Json;

        public IAsyncEnumerable<Domain.Entidades.DemografiaLocal> ProcessarAsync(Stream stream, IDemografiaLocalMapper mapper, string nomeCliente, string identificadorArquivo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
