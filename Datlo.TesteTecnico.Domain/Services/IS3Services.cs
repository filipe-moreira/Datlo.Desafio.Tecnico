using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Domain.Interfaces;

/// <summary>
/// Interface para operações com Amazon S3
/// </summary>
public interface IS3Services
{
    /// <summary>
    /// Gera uma URL pré-assinada para upload de um arquivo no S3
    /// </summary>
    /// <param name="clienteId"></param>
    /// <param name="tipoArquivo"></param>
    /// <param name="modeloArquivo"></param>
    /// <param name="tempoExpiracaoMinutos"></param>
    /// <returns></returns>
    Task<string> GerarUrlAssinada(string clienteId, TipoArquivo tipoArquivo, ModeloArquivo modeloArquivo, int tempoExpiracaoMinutos = 60);

    /// <summary>
    /// Remove um arquivo da S3
    /// </summary>
    /// <param name="caminhoArquivo">Caminho do arquivo na S3</param>
    Task RemoverArquivo(string caminhoArquivo);
}
