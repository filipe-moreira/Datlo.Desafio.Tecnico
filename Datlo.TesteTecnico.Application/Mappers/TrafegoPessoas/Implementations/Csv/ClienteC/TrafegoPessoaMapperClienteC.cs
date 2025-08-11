using CsvHelper.Configuration;
using System.Globalization;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Csv.ClienteC
{
    /// <summary>
    /// Mapper específico para o "Cliente C" que processa arquivos CSV.    
    /// </summary>
    public sealed class TrafegoPessoaMapperClienteC : TrafegoPessoaMapperCsvBase
    {
        public override string ClienteId => "ID_CLIENTE_C";

        public override CsvConfiguration ConfiguracaoCsv => new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = ";",
            DetectColumnCountChanges = false,
            MissingFieldFound = null // ignora campos ausentes não usados
        };

        public override ColunasCsvTrafegoPessoa Colunas => new ColunasCsvTrafegoPessoa
        {
            DATA_REGISTRO = 0,
            QTD_ESTIMADA_PESSOAS = 1,
            LATITUDE = 2,
            LONGITUDE = 3            
        };
    }
}
