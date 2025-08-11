using CsvHelper;
using CsvHelper.Configuration;
using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using Datlo.TesteTecnico.Domain.Models;
using System.Globalization;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Csv
{
    /// <summary>
    /// Classe base abstrata que implementa a lógica comum de mapeamento CSV para tráfego de pessoas.
    /// Permite reutilização do código de parsing e validação entre diferentes clientes.
    /// </summary>
    public abstract class TrafegoPessoaMapperCsvBase : ITrafegoPessoaCsvMapper
    {
        public TipoArquivo TipoArquivo => TipoArquivo.Csv;

        /// <summary>
        /// Nome do cliente. Deve ser implementado pelas classes filhas.
        /// </summary>
        public abstract string ClienteId { get; }

        /// <summary>
        /// Configuração específica do CSV para o cliente. Deve ser implementada pelas classes filhas.
        /// </summary>
        public abstract CsvConfiguration ConfiguracaoCsv { get; }

        /// <summary>
        /// Mapeamento das colunas do CSV. Deve ser implementado pelas classes filhas.
        /// </summary>
        public abstract ColunasCsvTrafegoPessoa Colunas { get; }

        /// <summary>
        /// Mapeia uma linha do CSV para o DTO de tráfego de pessoas.
        /// Implementação comum que pode ser reutilizada por todos os clientes CSV.
        /// </summary>
        public virtual TrafegoPessoaDto Map(IReaderRow row)
        {
            if (row is null) throw new ArgumentNullException(nameof(row));
            
            var latStr = row.GetField<string>(Colunas.LATITUDE);
            var lonStr = row.GetField<string>(Colunas.LONGITUDE);
            var dataStr = row.GetField<string>(Colunas.DATA_REGISTRO);
            var qtdStr = row.GetField<string>(Colunas.QTD_ESTIMADA_PESSOAS);

            if (!TryParseDate(dataStr, out var data))
                throw new InvalidOperationException($"Data inválida: '{dataStr}'. Esperado algo como 'yyyy-MM-dd HH:mm:ss'.");
            if (!int.TryParse(qtdStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var qtd))
                throw new InvalidOperationException($"Quantidade inválida: '{qtdStr}'.");
            if (!double.TryParse(latStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
                throw new InvalidOperationException($"Latitude inválida: '{latStr}'.");
            if (!double.TryParse(lonStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                throw new InvalidOperationException($"Longitude inválida: '{lonStr}'.");

            var dto = new TrafegoPessoaDto(data, qtd, lat, lon);
            Validate(dto);
            return dto;
        }

        /// <summary>
        /// Validação padrão do DTO. Pode ser sobrescrita pelas classes filhas para validações específicas.
        /// </summary>
        public virtual void Validate(TrafegoPessoaDto dto)
        {
            if (dto.DataRegistro == DateTime.MinValue) throw new ArgumentException("DataRegistro é obrigatório.");
            if (dto.Qtd < 0) throw new ArgumentException("QtdEstimadaPessoas não pode ser negativa.");
            if (dto.Lat is < -90 or > 90) throw new ArgumentException("Latitude inválida.");
            if (dto.Lon is < -180 or > 180) throw new ArgumentException("Longitude inválida.");
        }

        /// <summary>
        /// Método auxiliar para parsing de datas com múltiplos formatos.
        /// </summary>
        protected static bool TryParseDate(string input, out DateTime result)
        {
            var formats = new[]
            {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd'T'HH:mm:ss",
                "yyyy-MM-dd",
                "dd/MM/yyyy",
                "MM/dd/yyyy"
            };

            return DateTime.TryParseExact(
                input,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out result);
        }
    }
}
