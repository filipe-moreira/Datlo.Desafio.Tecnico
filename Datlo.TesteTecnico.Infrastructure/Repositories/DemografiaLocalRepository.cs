using Datlo.TesteTecnico.Application.Repositories;
using Datlo.TesteTecnico.Domain.Entidades;
using Npgsql;
using NpgsqlTypes;

namespace Datlo.TesteTecnico.Infrastructure.Repositories
{
    public class DemografiaLocalRepository : GenericRepository<DemografiaLocal>, IDemografiaLocalRepository
    {
        public DemografiaLocalRepository(NpgsqlConnection connection) : base(connection) { }

        public async Task BulkInsertAsync(IEnumerable<DemografiaLocal> entidades, CancellationToken ct)
        {
            if (Connection.State != System.Data.ConnectionState.Open)
            {
                await Connection.OpenAsync(ct);
            }

            await using var writer = await Connection.BeginBinaryImportAsync(@"
            COPY demografia_local 
            (
                id, 
                data_inclusao, 
                data_registro, 
                identificador_arquivo, 
                cliente_id, 
                populacao, 
                latitude, 
                longitude, 
                renda_media, 
                faixa_etaria_predominante
            ) FROM STDIN (FORMAT BINARY)", ct);

            foreach (var e in entidades)
            {
                ct.ThrowIfCancellationRequested();

                await writer.StartRowAsync(ct);
                await writer.WriteAsync(e.Id == Guid.Empty ? Guid.NewGuid() : e.Id, NpgsqlDbType.Uuid, ct);

                var dataInclusao = e.DataInclusao == default ? DateTime.UtcNow : e.DataInclusao;

                var di = e.DataInclusao.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(e.DataRegistro, DateTimeKind.Utc) : e.DataInclusao.ToUniversalTime();
                await writer.WriteAsync(di, NpgsqlDbType.TimestampTz, ct);

                var dt = e.DataRegistro.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(e.DataRegistro, DateTimeKind.Utc) : e.DataRegistro.ToUniversalTime();
                await writer.WriteAsync(dt, NpgsqlDbType.TimestampTz, ct);

                await writer.WriteAsync(e.IdentificadorArquivo, NpgsqlDbType.Varchar, ct);

                await writer.WriteAsync(e.ClienteId, NpgsqlDbType.Varchar, ct);

                await writer.WriteAsync(e.Populacao, NpgsqlDbType.Integer, ct);

                await writer.WriteAsync(e.Latitude, NpgsqlDbType.Double, ct);

                await writer.WriteAsync(e.Longitude, NpgsqlDbType.Double, ct);

                await writer.WriteAsync(e.RendaMedia, NpgsqlDbType.Double, ct);

                await writer.WriteAsync(e.FaixaEtariaPredominante, NpgsqlDbType.Varchar, ct);
            }

            await writer.CompleteAsync(ct);
        }
    }
}
