using Datlo.TesteTecnico.Application.Dto;
using System.Text.Json;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts
{
    /// <summary>
    /// Interface para mappers de arquivos JSON de demografia local.    
    /// </summary>
    public interface IDemografiaLocalJsonMapper : IDemografiaLocalMapper
    {
        /// <summary>
        /// Caminho até o array de registros no JSON (ex.: "$[*]" para array raiz).
        /// Usado pelo enumerador para localizar os itens.
        /// </summary>
        string ItemPath { get; }

        /// <summary>
        /// Converte um elemento JSON no DTO intermediário de demografia local.
        /// </summary>
        /// <param name="item">Elemento JSON contendo os dados</param>
        /// <returns>DTO com os dados convertidos</returns>
        DemografiaLocalDto Map(JsonElement item);
    }
}
