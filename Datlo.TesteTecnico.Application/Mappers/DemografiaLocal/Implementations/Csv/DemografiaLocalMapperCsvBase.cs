using CsvHelper;
using CsvHelper.Configuration;
using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Domain.Models;
using System.Globalization;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Csv
{
    /// <summary>
    /// Classe base abstrata que implementa a lógica comum de mapeamento CSV para demografia local.
    /// Permite reutilização do código de parsing e validação entre diferentes clientes.
    /// </summary>
    public abstract class DemografiaLocalMapperCsvBase : IDemografiaLocalCsvMapper
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
        public abstract ColunasCsvDemografiaLocal Colunas { get; }

        /// <summary>
        /// Mapeia uma linha do CSV para o DTO de demografia local.
        /// Implementação comum que pode ser reutilizada por todos os clientes CSV.
        /// </summary>
        public virtual DemografiaLocalDto Map(IReaderRow row)
        {
            if (row is null) throw new ArgumentNullException(nameof(row));
            
            var latStr = row.GetField<string>(Colunas.LATITUDE);
            var lonStr = row.GetField<string>(Colunas.LONGITUDE);
            var dataStr = row.GetField<string>(Colunas.DATA_REGISTRO);
            var populacaoStr = row.GetField<string>(Colunas.POPULACAO);
            var rendaMediaStr = row.GetField<string>(Colunas.RENDA_MEDIA);
            var faixaEtaria = row.GetField<string>(Colunas.FAIXA_ETARIA_PREDOMINANTE);

            if (!TryParseDate(dataStr, out var data))
                throw new InvalidOperationException($"Data inválida: '{dataStr}'. Esperado algo como 'yyyy-MM-dd HH:mm:ss'.");
            if (!int.TryParse(populacaoStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out var populacao))
                throw new InvalidOperationException($"População inválida: '{populacaoStr}'.");
            if (!double.TryParse(latStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lat))
                throw new InvalidOperationException($"Latitude inválida: '{latStr}'.");
            if (!double.TryParse(lonStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var lon))
                throw new InvalidOperationException($"Longitude inválida: '{lonStr}'.");
            if (!double.TryParse(rendaMediaStr, NumberStyles.Float, CultureInfo.InvariantCulture, out var rendaMedia))
                throw new InvalidOperationException($"Renda média inválida: '{rendaMediaStr}'.");
            
            var dto = new DemografiaLocalDto(data, lat, lon, populacao, rendaMedia, faixaEtaria);
            Validate(dto);
            return dto;
        }

        /// <summary>
        /// Validação padrão do DTO. Pode ser sobrescrita pelas classes filhas para validações específicas.
        /// </summary>
        public virtual void Validate(DemografiaLocalDto dto)
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

        /// <summary>
        /// Tenta fazer o parse de uma string de data em vários formatos comuns.
        /// </summary>
        protected static bool TryParseDate(string dateStr, out DateTime date)
        {
            date = default;
            if (string.IsNullOrWhiteSpace(dateStr)) return false;

            // Formatos comuns de data
            string[] formats = {
                "yyyy-MM-dd HH:mm:ss",
                "yyyy-MM-dd",
                "dd/MM/yyyy HH:mm:ss",
                "dd/MM/yyyy",
                "MM/dd/yyyy HH:mm:ss",
                "MM/dd/yyyy",
                "yyyy/MM/dd HH:mm:ss",
                "yyyy/MM/dd"
            };

            return DateTime.TryParseExact(dateStr, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}
