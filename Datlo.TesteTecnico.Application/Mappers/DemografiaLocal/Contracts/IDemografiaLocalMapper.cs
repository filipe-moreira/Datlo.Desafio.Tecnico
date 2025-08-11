using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts
{
    /// <summary>
    /// Interface base para todos os mappers de demografia local.
    /// </summary>
    public interface IDemografiaLocalMapper
    {
        /// <summary>
        /// Tipo de arquivo que este mapper processa.
        /// </summary>
        TipoArquivo TipoArquivo { get; }

        /// <summary>
        /// Identificador único do cliente (ex: "ClienteA", "ClienteB").
        /// </summary>
        string ClienteId { get; }

        /// <summary>
        /// Valida os dados do DTO de demografia local.
        /// </summary>
        /// <param name="dto">DTO a ser validado</param>
        /// <exception cref="ArgumentException">Quando os dados são inválidos</exception>
        void Validate(DemografiaLocalDto dto);
    }
}
