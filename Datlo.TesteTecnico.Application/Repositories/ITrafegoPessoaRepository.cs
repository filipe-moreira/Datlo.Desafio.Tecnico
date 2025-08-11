using Datlo.TesteTecnico.Domain.Entidades;

namespace Datlo.TesteTecnico.Application.Repositories
{
    public interface ITrafegoPessoaRepository 
    {
        Task BulkInsertAsync(IEnumerable<TrafegoPessoa> entidades, CancellationToken ct);
    }
}
