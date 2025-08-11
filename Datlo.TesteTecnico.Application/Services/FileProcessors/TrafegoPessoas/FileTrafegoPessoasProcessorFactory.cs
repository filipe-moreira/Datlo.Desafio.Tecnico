using Datlo.TesteTecnico.Application.Services.FileProcessors.Contracts;
using Datlo.TesteTecnico.Application.Services.FileProcessors.TrafegoPessoas;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors
{
    /// <summary>
    /// Factory que retorna o processador apropriado baseado no tipo de arquivo.    
    /// </summary>
    public sealed class FileTrafegoPessoasProcessorFactory : IFileTrafegoPessoasProcessorFactory
    {
        private readonly Dictionary<TipoArquivo, IFileTrafegoPessoasProcessor> _processors;

        public FileTrafegoPessoasProcessorFactory(IEnumerable<IFileTrafegoPessoasProcessor> processors)
        {
            _processors = processors.ToDictionary(p => p.TipoArquivo);
        }

        public IFileTrafegoPessoasProcessor GetProcessor(TipoArquivo tipoArquivo)
        {
            if (_processors.TryGetValue(tipoArquivo, out var processor))
            {
                return processor;
            }

            throw new NotSupportedException($"Tipo de arquivo não suportado: {tipoArquivo}.");
        }
    }
}
