using CsvHelper.Configuration;
using System.Globalization;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Implementations.Csv.ClienteD
{
    /// <summary>
    /// Mapper específico para o "Cliente D" que processa arquivos CSV.
    /// Herda da classe base para reutilizar a lógica comum de processamento CSV.
    /// </summary>
    public sealed class TrafegoPessoaMapperClienteD : TrafegoPessoaMapperCsvBase
    {
        public override string ClienteId => "ID_CLIENTE_D";

        public override CsvConfiguration ConfiguracaoCsv => new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            Delimiter = "|",
            DetectColumnCountChanges = false,
            MissingFieldFound = null // ignora campos ausentes não usados
        };

        public override ColunasCsvTrafegoPessoa Colunas => new ColunasCsvTrafegoPessoa
        {
            LATITUDE = 0,
            LONGITUDE = 1,
            DATA_REGISTRO = 2,
            QTD_ESTIMADA_PESSOAS = 3,
        };        
    }
}
