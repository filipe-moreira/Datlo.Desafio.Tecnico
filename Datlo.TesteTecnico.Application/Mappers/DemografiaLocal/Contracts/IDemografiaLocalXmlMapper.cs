using Datlo.TesteTecnico.Application.Dto;
using System.Xml;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts
{
    /// <summary>
    /// Interface para mappers de XML de demografia local.
    /// Define o contrato para conversão de elementos XML em DTOs de demografia local.
    /// </summary>
    public interface IDemografiaLocalXmlMapper : IDemografiaLocalMapper
    {
        /// <summary>
        /// Nome do elemento XML que contém os dados de demografia local
        /// </summary>
        string ElementName { get; }

        /// <summary>
        /// Converte um elemento XML no DTO intermediário de demografia local.
        /// </summary>
        /// <param name="element">Elemento XML contendo os dados</param>
        /// <returns>DTO com os dados convertidos</returns>
        DemografiaLocalDto Map(XmlElement element);
    }
}
