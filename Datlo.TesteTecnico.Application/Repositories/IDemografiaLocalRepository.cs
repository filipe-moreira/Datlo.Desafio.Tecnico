using Datlo.TesteTecnico.Domain.Entidades;

namespace Datlo.TesteTecnico.Application.Repositories
{
    public interface IDemografiaLocalRepository
    {
        Task BulkInsertAsync(IEnumerable<DemografiaLocal> entidades, CancellationToken ct);
    }
}
