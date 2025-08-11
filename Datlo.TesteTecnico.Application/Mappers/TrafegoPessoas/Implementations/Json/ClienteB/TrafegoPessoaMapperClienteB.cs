using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using System.Text.Json;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Json.ClienteB
{
    public sealed class TrafegoPessoaMapperClienteB : ITrafegoPessoaJsonMapper
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Json;
        public string ClienteId => "ID_CLIENTE_B";
        public string ItemPath => "$.features[*]";

        public TrafegoPessoaDto Map(JsonElement item)
        {
            if (!item.TryGetProperty("properties", out var properties))
                throw new InvalidOperationException("Propriedades não encontradas no item.");
            
            if (!properties.TryGetProperty("qtd_estimada_pessoas", out var qtdProp) ||
                !properties.TryGetProperty("latitude", out var latProp) ||
                !properties.TryGetProperty("longitude", out var lonProp) ||
                !properties.TryGetProperty("data_registro", out var dataProp))
            {
                throw new InvalidOperationException("Propriedades necessárias não encontradas.");
            }
            
            var qtd = qtdProp.GetInt32();
            var lat = latProp.GetDouble();
            var lon = lonProp.GetDouble();
            var dataRegistro = dataProp.GetDateTime();

            return new TrafegoPessoaDto(dataRegistro, qtd, lat, lon);
        }

        public void Validate(TrafegoPessoaDto dto)
        {
            if (dto.DataRegistro == DateTime.MinValue) 
                throw new ArgumentException("DataRegistro é obrigatório.");
            if (dto.Qtd < 0) 
                throw new ArgumentException("QtdEstimadaPessoas não pode ser negativa.");
            if (dto.Lat is < -90 or > 90) 
                throw new ArgumentException("Latitude inválida.");
            if (dto.Lon is < -180 or > 180) 
                throw new ArgumentException("Longitude inválida.");
        }
    }
}
