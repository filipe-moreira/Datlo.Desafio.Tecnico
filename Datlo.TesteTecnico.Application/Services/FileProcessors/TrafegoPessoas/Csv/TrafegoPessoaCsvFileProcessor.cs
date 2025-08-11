using CsvHelper;
using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using Datlo.TesteTecnico.Application.Services.FileProcessors.Contracts;
using Datlo.TesteTecnico.Domain.Entidades;
using Datlo.TesteTecnico.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.TrafegoPessoas.Csv
{
    /// <summary>
    /// Processador específico para arquivos CSV.
    /// Encapsula toda a lógica de processamento de arquivos CSV.
    /// </summary>
    public sealed class TrafegoPessoaCsvFileProcessor : IFileTrafegoPessoasProcessor
    {
        private readonly ILogger<TrafegoPessoaCsvFileProcessor> _logger;

        public TrafegoPessoaCsvFileProcessor(ILogger<TrafegoPessoaCsvFileProcessor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TipoArquivo TipoArquivo => TipoArquivo.Csv;

        public async IAsyncEnumerable<TrafegoPessoa> ProcessarAsync(
            Stream stream,
            ITrafegoPessoaMapper mapper,
            string clienteId,
            string identificadorArquivo,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(stream);
            ArgumentNullException.ThrowIfNull(mapper);
            ArgumentException.ThrowIfNullOrWhiteSpace(clienteId);
            ArgumentException.ThrowIfNullOrWhiteSpace(identificadorArquivo);

            if (mapper is not ITrafegoPessoaCsvMapper mapperCsv)
                throw new InvalidOperationException($"Mapper do cliente '{clienteId}' não implementa ITrafegoPessoaCsvMapper.");

            using var leitor = new StreamReader(stream);
            using var csv = new CsvReader(leitor, mapperCsv.ConfiguracaoCsv);

            // Cabeçalho
            if (!await csv.ReadAsync()) yield break;
            csv.ReadHeader();

            var linhaAtual = 1; // Começando a leitura do arquivo na linha 1 (após o cabeçalho)
            var registrosProcessados = 0;
            var registrosInvalidos = 0;

            while (await csv.ReadAsync())
            {
                linhaAtual++;

                TrafegoPessoa? entidade = null;

                try
                {
                    var dto = mapperCsv.Map(csv);
                    mapper.Validate(dto);

                    entidade = CriarEntidade(clienteId, identificadorArquivo, dto);
                    registrosProcessados++;
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    // Log de alerta para dados inválidos, mas continua o processamento do arquivo
                    registrosInvalidos++;
                    //_logger.LogWarning(
                    //    "Registro inválido encontrado na linha {Linha} do arquivo {Arquivo} (Cliente: {Cliente}). " +
                    //    "Erro: {Erro}. Continuando processamento...",
                    //    linhaAtual, identificadorArquivo, clienteId, ex.Message);
                }

                if (entidade != null)
                {
                    yield return entidade;
                }
            }

            // Log final com estatísticas do processamento
            _logger.LogInformation(
                "Processamento do arquivo {Arquivo} (Cliente: {Cliente}) concluído. " +
                "Registros processados: {ProcessadosCount}, Registros inválidos: {InvalidosCount}",
                identificadorArquivo, clienteId, registrosProcessados, registrosInvalidos);
        }

        private static TrafegoPessoa CriarEntidade(string clienteId, string identificadorArquivo, TrafegoPessoaDto dto) =>
            new(clienteId, identificadorArquivo, dto.DataRegistro, dto.Qtd, dto.Lat, dto.Lon);
    }
}
