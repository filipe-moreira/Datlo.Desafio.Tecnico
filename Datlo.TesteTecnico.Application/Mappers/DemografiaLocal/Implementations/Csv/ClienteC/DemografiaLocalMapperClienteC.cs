using CsvHelper.Configuration;
using System.Globalization;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Implementations.Csv.ClienteC
{
    /// <summary>
    /// Mapper específico para o "Cliente C" que processa arquivos CSV de demografia local.
    /// </summary>
    public sealed class DemografiaLocalMapperClienteC : DemografiaLocalMapperCsvBase
    {
        public override string ClienteId => "ID_CLIENTE_C";

        public override CsvConfiguration ConfiguracaoCsv => new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            DetectColumnCountChanges = false,
            MissingFieldFound = null // ignora campos ausentes não usados
        };

        public override ColunasCsvDemografiaLocal Colunas => new ColunasCsvDemografiaLocal
        {
            DATA_REGISTRO = 0,
            LATITUDE = 1,
            LONGITUDE = 2,
            POPULACAO = 3,
            RENDA_MEDIA = 4,
            FAIXA_ETARIA_PREDOMINANTE = 5
        };
    }
}
