using Datlo.TesteTecnico.Domain.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Datlo.TesteTecnico.Infrastructure.Data;

/// <summary>
/// Conteúdo do banco de dados usando Entity Framework Core
/// Apenas para ilustrar conhecimento em EF Core    
/// </summary>
public class DatloContext : DbContext
{
    public DatloContext(DbContextOptions<DatloContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    public DbSet<DemografiaLocal> DemografiaLocal { get; set; }
    public DbSet<TrafegoPessoa> TrafegoPessoas { get; set; }
}
