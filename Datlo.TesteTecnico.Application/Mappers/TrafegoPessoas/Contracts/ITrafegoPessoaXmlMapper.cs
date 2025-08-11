using Datlo.TesteTecnico.Application.Dto;
using System.Xml;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts
{
    /// <summary>
    /// Interface para mappers de XML.
    /// Define o contrato para conversão de elementos XML em DTOs de tráfego de pessoas.
    /// </summary>
    public interface ITrafegoPessoaXmlMapper : ITrafegoPessoaMapper
    {
        /// <summary>
        /// Nome do elemento XML que contém os dados de tráfego
        /// </summary>
        string ElementName { get; }

        /// <summary>
        /// Converte um elemento XML no DTO intermediário de tráfego de pessoas.
        /// </summary>
        /// <param name="element">Elemento XML contendo os dados</param>
        /// <returns>DTO com os dados convertidos</returns>
        TrafegoPessoaDto Map(XmlElement element);
    }
}
