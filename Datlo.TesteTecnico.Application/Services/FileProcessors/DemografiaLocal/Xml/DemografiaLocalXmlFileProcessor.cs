using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal.Xml
{
    /// <summary>
    /// Processador específico para arquivos XML de demografia local.
    /// Encapsula toda a lógica de processamento de arquivos XML.
    /// </summary>
    public sealed class DemografiaLocalXmlFileProcessor : IFileDemografiaLocalProcessor
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Xml;

        public IAsyncEnumerable<Domain.Entidades.DemografiaLocal> ProcessarAsync(Stream stream, IDemografiaLocalMapper mapper, string nomeCliente, string identificadorArquivo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
