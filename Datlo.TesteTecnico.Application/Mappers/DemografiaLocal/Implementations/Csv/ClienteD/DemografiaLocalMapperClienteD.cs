using CsvHelper.Configuration;
using Datlo.TesteTecnico.Application.Dto;
using System.Globalization;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Csv.ClienteD
{
    /// <summary>
    /// Mapper específico para o "Cliente D" que processa arquivos CSV de demografia local.
    /// </summary>
    public sealed class DemografiaLocalMapperClienteD : DemografiaLocalMapperCsvBase
    {
        public override string ClienteId => "ID_CLIENTE_D";

        public override CsvConfiguration ConfiguracaoCsv => new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            DetectColumnCountChanges = false,
            MissingFieldFound = null // ignora campos ausentes não usados
        };

        public override ColunasCsvDemografiaLocal Colunas => new ColunasCsvDemografiaLocal
        {
            POPULACAO = 0,
            RENDA_MEDIA = 1,
            FAIXA_ETARIA_PREDOMINANTE = 2,
            DATA_REGISTRO = 3,
            LATITUDE = 4,
            LONGITUDE = 5
        };

        /// <summary>
        /// Validação específica para o ClienteD com regras mais rigorosas.
        /// </summary>
        public override void Validate(DemografiaLocalDto dto)
        {
            // Validação base
            base.Validate(dto);

            // Validações específicas do ClienteId D
            if (dto.Populacao < 1000) 
                throw new ArgumentException("Cliente D requer população mínima de 1000 habitantes.");
            if (dto.RendaMedia < 1000) 
                throw new ArgumentException("Cliente D requer renda média mínima de R$ 1000.");            
        }
    }
}
