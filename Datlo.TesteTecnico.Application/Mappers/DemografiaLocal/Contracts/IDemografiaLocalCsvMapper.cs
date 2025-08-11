using CsvHelper;
using CsvHelper.Configuration;
using Datlo.TesteTecnico.Application.Dto;

namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts
{
    /// <summary>
    /// Interface para mappers de arquivos CSV de demografia local.
    /// </summary>
    public interface IDemografiaLocalCsvMapper : IDemografiaLocalMapper
    {
        /// <summary>
        /// Configuração específica do CSV para o cliente (delimitadores, encoding, etc.).
        /// </summary>
        CsvConfiguration ConfiguracaoCsv { get; }

        /// <summary>
        /// Mapeamento das colunas do CSV para os campos do DTO.
        /// </summary>
        ColunasCsvDemografiaLocal Colunas { get; }

        /// <summary>
        /// Converte uma linha do CSV no DTO intermediário de demografia local.
        /// </summary>
        /// <param name="row">Linha do CSV a ser convertida</param>
        /// <returns>DTO com os dados convertidos</returns>
        DemografiaLocalDto Map(IReaderRow row);
    }
}
