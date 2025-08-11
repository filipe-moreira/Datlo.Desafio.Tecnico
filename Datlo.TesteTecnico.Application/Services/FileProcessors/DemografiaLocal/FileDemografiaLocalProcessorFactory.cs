using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal
{
    /// <summary>
    /// Factory que retorna o processador apropriado baseado no tipo de arquivo.    
    /// </summary>
    public sealed class FileDemografiaLocalProcessorFactory : IFileDemografiaLocalProcessorFactory
    {
        private readonly Dictionary<TipoArquivo, IFileDemografiaLocalProcessor> _processors;

        public FileDemografiaLocalProcessorFactory(IEnumerable<IFileDemografiaLocalProcessor> processors)
        {
            _processors = processors.ToDictionary(p => p.TipoArquivo);
        }

        public IFileDemografiaLocalProcessor GetProcessor(TipoArquivo tipoArquivo)
        {
            if (_processors.TryGetValue(tipoArquivo, out var processor))
            {
                return processor;
            }

            throw new NotSupportedException($"Tipo de arquivo não suportado: {tipoArquivo}.");
        }
    }
}
