using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors
{
    public interface IFileProcessorFactory
    {
        /// <summary>
        /// Factory para criar o processador de arquivos baseado no tipo do arquivo.
        /// </summary>
        /// <param name="arquivo"></param>
        /// <returns>Instância do processador de arquivo correspondente.</returns>
        IFileProcessor GetProcessor(ArquivosS3 arquivo);
    }
}