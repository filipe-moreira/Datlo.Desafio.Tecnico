using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using System.Text.Json;
using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Json.ClienteA
{
    public sealed class TrafegoPessoaMapperClienteA : ITrafegoPessoaJsonMapper
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Json;
        public string ClienteId => "ID_CLIENTE_A";
        public string ItemPath => "$[*]"; // array na raiz do arquivo json

        public TrafegoPessoaDto Map(JsonElement item)
        {
            var data = DateTime.Parse(item.GetProperty("data").GetString()!);
            var qtd = item.GetProperty("pessoas_estimada").GetInt32();
            var lat = item.GetProperty("latitude").GetDouble();
            var lon = item.GetProperty("longitude").GetDouble();
            return new TrafegoPessoaDto(data, qtd, lat, lon);
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
