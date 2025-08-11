using Datlo.TesteTecnico.Application.Extensions;
using Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts;
using Datlo.TesteTecnico.Application.Ports;
using Datlo.TesteTecnico.Application.Repositories;
using Datlo.TesteTecnico.Domain.Models;
using Microsoft.Extensions.Logging;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal
{
    public sealed class ProcessarDemografiaLocalUseCase : IFileProcessor
    {
        private const int TAMANHO_LOTE = 50_000;

        private readonly IObjectStorage _objectStorage;
        private readonly IDemografiaLocalMapperRegistry _mappers;
        private readonly IDemografiaLocalRepository _repositorio;
        private readonly IFileDemografiaLocalProcessorFactory _fileProcessorFactory;
        private readonly ILogger<ProcessarDemografiaLocalUseCase> _logger;

        public ProcessarDemografiaLocalUseCase(
            IObjectStorage objectStorage,
            IDemografiaLocalMapperRegistry registradorMappers,
            IDemografiaLocalRepository repositorio,
            IFileDemografiaLocalProcessorFactory fileProcessorFactory,
            ILogger<ProcessarDemografiaLocalUseCase> logger)
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
            var cliente = arquivo.ClienteId; 
            var bucket = arquivo.Bucket; 
            var chave = arquivo.Chave;

            ArgumentException.ThrowIfNullOrEmpty(cliente);
            ArgumentException.ThrowIfNullOrEmpty(bucket);
            ArgumentException.ThrowIfNullOrEmpty(chave);

            var mapper = _mappers.Get(cliente);
            var processor = _fileProcessorFactory.GetProcessor(mapper.TipoArquivo);

            await using var stream = await _objectStorage.GetObjectStreamAsync(bucket, chave, ct);

            await foreach (var lote in processor.ProcessarAsync(
                stream, mapper, cliente, chave, ct).Buffer(TAMANHO_LOTE, ct))
            {
                ct.ThrowIfCancellationRequested();
                await _repositorio.BulkInsertAsync(lote, ct);                
            }
        }
    }
}
