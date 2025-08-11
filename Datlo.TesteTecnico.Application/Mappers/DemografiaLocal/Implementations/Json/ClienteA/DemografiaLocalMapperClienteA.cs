using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Domain.Models;
using System.Text.Json;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Json.ClienteA
{
    /// <summary>
    /// Mapper específico para o "Cliente A" que processa arquivos JSON de demografia local.
    /// Formato esperado: array de objetos na raiz do JSON.
    /// </summary>
    public sealed class DemografiaLocalMapperClienteA : IDemografiaLocalJsonMapper
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Json;
        public string ClienteId => "ID_CLIENTE_A";
        public string ItemPath => "$[*]"; // array na raiz do arquivo json

        public DemografiaLocalDto Map(JsonElement item)
        {
            var data = DateTime.Parse(item.GetProperty("data_registro").GetString()!);
            var lat = item.GetProperty("latitude").GetDouble();
            var lon = item.GetProperty("longitude").GetDouble();
            var populacao = item.GetProperty("populacao").GetInt32();
            var rendaMedia = item.GetProperty("renda_media").GetDouble();
            var faixaEtaria = item.GetProperty("faixa_etaria_predominante").GetString();
            
            return new DemografiaLocalDto(data, lat, lon, populacao, rendaMedia, faixaEtaria);
        }

        /// <summary>
        /// Validação específica para o ClienteA.
        /// </summary>
        public void Validate(DemografiaLocalDto dto)
        {
            if (dto.DataRegistro == DateTime.MinValue) 
                throw new ArgumentException("DataRegistro é obrigatório.");
            if (dto.Populacao < 0) 
                throw new ArgumentException("População não pode ser negativa.");
            if (dto.Lat is < -90 or > 90) 
                throw new ArgumentException("Latitude inválida.");
            if (dto.Lon is < -180 or > 180) 
                throw new ArgumentException("Longitude inválida.");
            if (dto.RendaMedia < 0) 
                throw new ArgumentException("Renda média não pode ser negativa.");            
        }
    }
}
