using Datlo.TesteTecnico.Domain.Models;

namespace Datlo.TesteTecnico.Application.Services.FileProcessors
{
    public interface IFileProcessor
    {
        Task ExecutarAsync(ArquivosS3 arquivo, CancellationToken ct);
    }
}
