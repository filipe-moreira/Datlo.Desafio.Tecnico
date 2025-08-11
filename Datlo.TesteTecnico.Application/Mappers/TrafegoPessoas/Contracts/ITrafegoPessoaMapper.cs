using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts
{
    /// <summary>
    /// Interface base para todos os mappers de tráfego de pessoas.    
    /// </summary>
    public interface ITrafegoPessoaMapper
    {
        /// <summary>
        /// Tipo de arquivo que este mapper processa.
        /// </summary>
        TipoArquivo TipoArquivo { get; }

        /// <summary>
        /// Identificador único do cliente (ex: "ClienteA", "ClienteB").
        /// </summary>
        string ClienteId { get; }

        void Validate(TrafegoPessoaDto dto);
    }
}
