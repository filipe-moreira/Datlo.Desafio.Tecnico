using Datlo.TesteTecnico.Application.Dto;
using System.Text.Json;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts
{
    /// <summary>
    /// Interface para mappers de arquivos JSON.    
    /// </summary>
    public interface ITrafegoPessoaJsonMapper : ITrafegoPessoaMapper
    {
        /// <summary>
        /// Caminho até o array de registros no JSON (ex.: "$[*]" para array raiz).
        /// Usado pelo enumerador para localizar os itens.
        /// </summary>
        string ItemPath { get; }

        /// <summary>
        /// Converte um elemento JSON no DTO intermediário de tráfego de pessoas.
        /// </summary>
        /// <param name="item">Elemento JSON contendo os dados</param>
        /// <returns>DTO com os dados convertidos</returns>
        TrafegoPessoaDto Map(JsonElement item);
    }
}
