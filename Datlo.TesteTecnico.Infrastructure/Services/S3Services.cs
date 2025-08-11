using Amazon.S3;
using Amazon.S3.Model;
using Datlo.TesteTecnico.Domain.Interfaces;
using Datlo.TesteTecnico.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Datlo.TesteTecnico.Infrastructure.Services;

public class S3Services : IS3Services
{
    private readonly string _nomeBucket;
    private readonly IAmazonS3 _clienteS3;
    private readonly ILogger<S3Services> _logger;
    public S3Services(IAmazonS3 clienteS3, 
                      IConfiguration configuration, 
                      ILogger<S3Services> logger)
    {
        ArgumentNullException.ThrowIfNull(clienteS3);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(configuration);

        var bucketName = configuration["AWS:S3:BucketName"];
        if (string.IsNullOrWhiteSpace(bucketName))
            throw new ArgumentException("Configuração AWS:BucketName não foi definida.", nameof(configuration));

        _nomeBucket = bucketName;


        _clienteS3 = clienteS3;
        _nomeBucket = bucketName;                      
        _logger = logger;
    }

    public async Task<string> GerarUrlAssinada(
        string clienteId,
        TipoArquivo tipoArquivo,
        ModeloArquivo modeloArquivo,
        int tempoExpiracaoMinutos = 60)
    {
        try
        {
            _logger.LogInformation(
                "Gerando URL pré-assinada para arquivo: {TipoArquivo}, {ModeloArquivo}",
                tipoArquivo, modeloArquivo);

            // Determinar extensão e content-type
            string extensao;
            string tipoMime;
            switch (tipoArquivo)
            {
                case TipoArquivo.Json:
                    extensao = ".json";
                    tipoMime = "application/json";
                    break;
                case TipoArquivo.Csv:
                    extensao = ".csv";
                    tipoMime = "text/csv";
                    break;
                case TipoArquivo.Xml:
                    extensao = ".xml";
                    tipoMime = "application/xml";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tipoArquivo), tipoArquivo, null);
            }

            // Nome final do arquivo com extensão
            var nomeArquivo = $"{tipoArquivo.ToString().ToLowerInvariant()}-{modeloArquivo}{extensao}";

            // Caminho no bucket
            var chaveArquivo = $"uploads/{clienteId}/{DateTime.UtcNow:yyyy-MM-dd}-{Guid.NewGuid()}-{nomeArquivo}";

            var requisicao = new GetPreSignedUrlRequest
            {
                BucketName = _nomeBucket,
                Key = chaveArquivo,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(tempoExpiracaoMinutos),
                ContentType = tipoMime
            };

            var urlAssinada = await _clienteS3.GetPreSignedURLAsync(requisicao);

            _logger.LogInformation("URL pré-assinada gerada com sucesso para: {ChaveArquivo}", chaveArquivo);

            return urlAssinada;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar URL pré-assinada para arquivo: {ModeloArquivo}", modeloArquivo);
            throw;
        }
    }


    /// <summary>
    /// Remove um arquivo da S3
    /// </summary>
    public async Task RemoverArquivo(string caminhoArquivo)
    {
        try
        {
            _logger.LogInformation("Removendo arquivo da S3: {CaminhoArquivo}", caminhoArquivo);

            var requisicao = new DeleteObjectRequest
            {
                BucketName = _nomeBucket,
                Key = caminhoArquivo
            };

            await _clienteS3.DeleteObjectAsync(requisicao);

            _logger.LogInformation("Arquivo removido com sucesso: {CaminhoArquivo}", caminhoArquivo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover arquivo da S3: {CaminhoArquivo}", caminhoArquivo);
            throw;
        }
    }
}
