using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using System.Globalization;
using System.Xml;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Xml.ClienteE
{
    public sealed class TrafegoPessoaMapperClienteE : ITrafegoPessoaXmlMapper
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Xml;
        public string ClienteId => "ID_CLIENTE_E";
        public string ElementName => "registro";

        public TrafegoPessoaDto Map(XmlElement element)
        {
            var dataStr = element.GetAttribute("data");
            var qtdStr = element.GetAttribute("quantidade");
            var latStr = element.GetAttribute("lat");
            var lonStr = element.GetAttribute("lon");

            if (!DateTime.TryParse(dataStr, out var data))
                throw new InvalidOperationException($"Data inválida: {dataStr}");

            if (!int.TryParse(qtdStr, out var qtd))
                throw new InvalidOperationException($"Quantidade inválida: {qtdStr}");

            if (!double.TryParse(latStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
                throw new InvalidOperationException($"Latitude inválida: {latStr}");

            if (!double.TryParse(lonStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                throw new InvalidOperationException($"Longitude inválida: {lonStr}");

            return new TrafegoPessoaDto(data, qtd, lat, lon);
        }

        public void Validate(TrafegoPessoaDto dto)
        {
            if (dto.Qtd <= 0)
                throw new InvalidOperationException("Quantidade deve ser maior que zero.");

            if (dto.Lat < -90 || dto.Lat > 90)
                throw new InvalidOperationException("Latitude deve estar entre -90 e 90.");

            if (dto.Lon < -180 || dto.Lon > 180)
                throw new InvalidOperationException("Longitude deve estar entre -180 e 180.");
        }
    }
}
