using CsvHelper;
using Datlo.TesteTecnico.Application.Dto;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal.Csv
{
    /// <summary>
    /// Processador específico para arquivos CSV de demografia local.
    /// Encapsula toda a lógica de processamento de arquivos CSV.
    /// </summary>
    public sealed class DemografiaLocalCsvFileProcessor : IFileDemografiaLocalProcessor
    {
        private readonly ILogger<DemografiaLocalCsvFileProcessor> _logger;

        public DemografiaLocalCsvFileProcessor(ILogger<DemografiaLocalCsvFileProcessor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public TipoArquivo TipoArquivo => TipoArquivo.Csv;

        public async IAsyncEnumerable<Domain.Entidades.DemografiaLocal> ProcessarAsync(
            Stream stream,
            IDemografiaLocalMapper mapper,
            string nomeCliente,
            string identificadorArquivo,
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
        {
            if (mapper is not IDemografiaLocalCsvMapper mapperCsv)
                throw new InvalidOperationException($"Mapper do cliente '{nomeCliente}' não implementa IDemografiaLocalCsvMapper.");

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

                Domain.Entidades.DemografiaLocal? entidade = null;
                try
                {
                    var dto = mapperCsv.Map(csv);
                    mapper.Validate(dto);

                    entidade = CriarEntidade(nomeCliente, identificadorArquivo, dto);
                    registrosProcessados++;
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
                {
                    // Log de alerta para dados inválidos, mas continua processamento
                    registrosInvalidos++;
                    //_logger.LogWarning(
                    //    "Registro inválido encontrado na linha {Linha} do arquivo {Arquivo} (Cliente: {Cliente}). " +
                    //    "Erro: {Erro}. Continuando processamento...",
                    //    linhaAtual, identificadorArquivo, nomeCliente, ex.Message);
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
                identificadorArquivo, nomeCliente, registrosProcessados, registrosInvalidos);
        }

        private static Domain.Entidades.DemografiaLocal CriarEntidade(string nomeCliente, string identificadorArquivo, DemografiaLocalDto dto)
        {
            return new Domain.Entidades.DemografiaLocal(
                clienteId: nomeCliente,
                identificadorArquivo: identificadorArquivo,
                dataRegistro: dto.DataRegistro,
                populacao: dto.Populacao,
                latitude: dto.Lat,
                longitude: dto.Lon,
                rendaMedia: dto.RendaMedia,
                faixaEtariaPredominante: dto.FaixaEtariaPredominante);
        }
    }
}
