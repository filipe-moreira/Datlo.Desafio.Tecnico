using CsvHelper;
using CsvHelper.Configuration;
using Datlo.TesteTecnico.Application.Dto;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts
{
    /// <summary>
    /// Interface para mappers de arquivos CSV.    
    /// </summary>
    public interface ITrafegoPessoaCsvMapper : ITrafegoPessoaMapper
    {
        /// <summary>
        /// Configuração específica do CSV para o cliente (delimitadores, encoding, etc.).
        /// </summary>
        CsvConfiguration ConfiguracaoCsv { get; }

        /// <summary>
        /// Mapeamento das colunas do CSV para os campos do DTO.
        /// </summary>
        ColunasCsvTrafegoPessoa Colunas { get; }

        /// <summary>
        /// Converte uma linha do CSV no DTO intermediário de tráfego de pessoas.
        /// </summary>
        /// <param name="row">Linha do CSV a ser convertida</param>
        /// <returns>DTO com os dados convertidos</returns>
        TrafegoPessoaDto Map(IReaderRow row);
    }
}
