using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Domain.Models;
using System.Globalization;
using System.Xml;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Xml.ClienteE
{
    /// <summary>
    /// Mapper específico para o "Cliente E" que processa arquivos XML de demografia local.    
    /// </summary>
    public sealed class DemografiaLocalMapperClienteE : IDemografiaLocalXmlMapper
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Xml;
        public string ClienteId => "ID-CLIENTE-E";
        public string ElementName => "demografia"; // nome do elemento XML que contém os dados

        /// <summary>
        /// Mapeia um elemento XML para o DTO de demografia local.
        /// Formato esperado do XML:
        /// </summary>
        public DemografiaLocalDto Map(XmlElement element)
        {
            var dataStr = element.GetAttribute("data");
            var latStr = element.GetAttribute("lat");
            var lonStr = element.GetAttribute("lon");
            var populacaoStr = element.GetAttribute("populacao");
            var rendaStr = element.GetAttribute("renda");
            var idade = element.GetAttribute("idade");

            if (!DateTime.TryParse(dataStr, out var data))
                throw new InvalidOperationException($"Data inválida: {dataStr}");

            if (!double.TryParse(latStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
                throw new InvalidOperationException($"Latitude inválida: {latStr}");

            if (!double.TryParse(lonStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                throw new InvalidOperationException($"Longitude inválida: {lonStr}");

            if (!int.TryParse(populacaoStr, out var populacao))
                throw new InvalidOperationException($"População inválida: {populacaoStr}");

            if (!double.TryParse(rendaStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var renda))
                throw new InvalidOperationException($"Renda inválida: {rendaStr}");

            return new DemografiaLocalDto(data, lat, lon, populacao, renda, idade);
        }

        /// <summary>
        /// Validação específica para o "Cliente E".
        /// </summary>
        public void Validate(DemografiaLocalDto dto)
        {
            if (dto.DataRegistro == DateTime.MinValue)
                throw new InvalidOperationException("Data de registro é obrigatória.");

            if (dto.Populacao <= 0)
                throw new InvalidOperationException("População deve ser maior que zero.");

            if (dto.Lat < -90 || dto.Lat > 90)
                throw new InvalidOperationException("Latitude deve estar entre -90 e 90.");

            if (dto.Lon < -180 || dto.Lon > 180)
                throw new InvalidOperationException("Longitude deve estar entre -180 e 180.");

            if (dto.RendaMedia < 0)
                throw new InvalidOperationException("Renda média não pode ser negativa.");
        }
    }
}
