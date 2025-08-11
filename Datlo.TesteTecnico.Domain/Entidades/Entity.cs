using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Datlo.TesteTecnico.Domain.Entidades
{
    public abstract class Entity
    {
        protected Entity()
        {
            Id = Guid.NewGuid();
            DataInclusao = DateTime.Now;
        }
        
        [Key]
        [Column("id")]
        public Guid Id { get; private set; }
        [Column("data_inclusao")]
        public DateTime DataInclusao { get; private set; }
    }
}
