using Datlo.TesteTecnico.Application.Services.FileProcessors.DemografiaLocal;
using Datlo.TesteTecnico.Application.Services.FileProcessors.TrafegoPessoas;
using Datlo.TesteTecnico.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors
{
    public sealed class FileProcessorFactory : IFileProcessorFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public FileProcessorFactory(IServiceScopeFactory scopeFactory)
        {
            ArgumentNullException.ThrowIfNull(scopeFactory, nameof(scopeFactory));

            _scopeFactory = scopeFactory;
        }

        public IFileProcessor GetProcessor(ArquivosS3 arquivo)
        {
            ArgumentNullException.ThrowIfNull(arquivo);

            var scope = _scopeFactory.CreateScope();
            var sp = scope.ServiceProvider;

            return arquivo.Modelo switch
            {
                ModeloArquivo.TrafegoPessoas => sp.GetRequiredService<ProcessarTrafegoPessoaUseCase>(),
                ModeloArquivo.DemografiaLocal => sp.GetRequiredService<ProcessarDemografiaLocalUseCase>(),
                _ => throw new NotSupportedException($"Modelo de arquivo não suportado: {arquivo.Modelo}")
            };
        }
    }
}
