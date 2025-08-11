using Datlo.TesteTecnico.Application.Extensions;
using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;
using Datlo.TesteTecnico.Application.Ports;
using Datlo.TesteTecnico.Application.Repositories;
using Datlo.TesteTecnico.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.TrafegoPessoas
{
    public sealed class ProcessarTrafegoPessoaUseCase : IFileProcessor
    {
        private const int TAMANHO_LOTE = 50_000;

        private readonly IObjectStorage _objectStorage;
        private readonly ITrafegoPessoaMapperRegistry _mappers;
        private readonly ITrafegoPessoaRepository _repositorio;
        private readonly IFileTrafegoPessoasProcessorFactory _fileProcessorFactory;
        private readonly ILogger<ProcessarTrafegoPessoaUseCase> _logger;

        public ProcessarTrafegoPessoaUseCase(
            IObjectStorage objectStorage,
            ITrafegoPessoaMapperRegistry registradorMappers,
            ITrafegoPessoaRepository repositorio,
            IFileTrafegoPessoasProcessorFactory fileProcessorFactory,
            ILogger<ProcessarTrafegoPessoaUseCase> logger)
        {
            ArgumentNullException.ThrowIfNull(objectStorage, nameof(objectStorage));
            ArgumentNullException.ThrowIfNull(registradorMappers, nameof(registradorMappers));
            ArgumentNullException.ThrowIfNull(repositorio, nameof(repositorio));
            ArgumentNullException.ThrowIfNull(fileProcessorFactory, nameof(fileProcessorFactory));
            ArgumentNullException.ThrowIfNull(logger, nameof(logger));

            _objectStorage = objectStorage;
            _mappers = registradorMappers;
            _repositorio = repositorio;
            _fileProcessorFactory = fileProcessorFactory;
            _logger = logger;
        }

        public async Task ExecutarAsync(ArquivosS3 arquivo, CancellationToken ct)
        {
            var clienteId = arquivo.ClienteId;
            var bucket = arquivo.Bucket;
            var chave = arquivo.Chave;

            var mapper = _mappers.Get(clienteId);
            var processor = _fileProcessorFactory.GetProcessor(mapper.TipoArquivo);

            await using var stream = await _objectStorage.GetObjectStreamAsync(bucket, chave, ct);

            await foreach (var lote in processor.ProcessarAsync(
                stream, mapper, clienteId, chave, ct).Buffer(TAMANHO_LOTE, ct))
            {
                ct.ThrowIfCancellationRequested();
                await _repositorio.BulkInsertAsync(lote, ct);
            }
        }
    }
}