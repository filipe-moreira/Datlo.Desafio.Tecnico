using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Domain.Models;
using System.Text.Json;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Json.ClienteB
{
    /// <summary>
    /// Mapper específico para o "Cliente B" que processa arquivos JSON de demografia local.    
    /// </summary>
    public sealed class DemografiaLocalMapperClienteB : IDemografiaLocalJsonMapper
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Json;
        public string ClienteId => "ID_CLIENTE_B";
        public string ItemPath => "$.features[*]"; // features dentro de um GeoJSON
      
        public DemografiaLocalDto Map(JsonElement item)
        {
            if (!item.TryGetProperty("properties", out var properties))
                throw new InvalidOperationException("Propriedades não encontradas no item.");
            
            if (!item.TryGetProperty("geometry", out var geometry))
                throw new InvalidOperationException("Geometria não encontrada no item.");

            if (!properties.TryGetProperty("data_coleta", out var dataProp) ||
                !properties.TryGetProperty("total_habitantes", out var populacaoProp) ||
                !properties.TryGetProperty("renda_familiar_media", out var rendaProp) ||
                !properties.TryGetProperty("idade_media", out var idadeProp))
            {
                throw new InvalidOperationException("Propriedades necessárias não encontradas.");
            }

            if (!geometry.TryGetProperty("coordinates", out var coordinates))
                throw new InvalidOperationException("Coordenadas não encontradas na geometria.");

            var coordArray = coordinates.EnumerateArray().ToArray();
            if (coordArray.Length < 2)
                throw new InvalidOperationException("Coordenadas insuficientes.");

            var dataRegistro = dataProp.GetDateTime();
            var populacao = populacaoProp.GetInt32();
            var rendaMedia = rendaProp.GetDouble();
            var faixaEtaria = idadeProp.GetString();
            var lon = coordArray[0].GetDouble(); // GeoJSON usa [longitude, latitude]
            var lat = coordArray[1].GetDouble();

            return new DemografiaLocalDto(dataRegistro, lat, lon, populacao, rendaMedia, faixaEtaria);
        }

        /// <summary>
        /// Validação específica para o "Cliente B".
        /// </summary>
        public void Validate(DemografiaLocalDto dto)
        {
            if (dto.DataRegistro == DateTime.MinValue) 
                throw new ArgumentException("DataRegistro é obrigatório.");
            if (dto.Populacao <= 0) 
                throw new ArgumentException("População deve ser maior que zero para Cliente B.");
            if (dto.Lat is < -90 or > 90) 
                throw new ArgumentException("Latitude inválida.");
            if (dto.Lon is < -180 or > 180) 
                throw new ArgumentException("Longitude inválida.");
            if (dto.RendaMedia <= 0) 
                throw new ArgumentException("Renda média deve ser maior que zero para Cliente B.");            
        }
    }
}
