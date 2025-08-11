using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Api.Models
{
    public class GerarUrlAssinadaRequest
    {
        public required TipoArquivo TipoArquivo { get; init; }
        public required ModeloArquivo ModeloArquivo { get; init; }
    }
}
