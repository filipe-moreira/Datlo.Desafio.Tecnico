using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal
{
    /// <summary>
    /// Factory para criação de processadores de arquivo baseado no tipo de arquivo.
    /// </summary>
    public interface IFileDemografiaLocalProcessorFactory
    {
        /// <summary>
        /// Retorna o processador apropriado para o tipo de arquivo especificado.
        /// </summary>
        /// <param name="tipoArquivo">Tipo de arquivo a ser processado</param>
        /// <returns>Processador específico para o tipo de arquivo</returns>
        /// <exception cref="NotSupportedException">Quando o tipo de arquivo não é suportado</exception>
        IFileDemografiaLocalProcessor GetProcessor(TipoArquivo tipoArquivo);
    }
}
