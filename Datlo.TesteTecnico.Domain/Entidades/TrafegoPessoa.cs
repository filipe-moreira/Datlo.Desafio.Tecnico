using System.ComponentModel.DataAnnotations.Schema;

namespace Datlo.TesteTecnico.Domain.Entidades
{
    public class TrafegoPessoa : Entity, IAggregateRoot
    {
        /// <summary>Construtor padrão para o Entity Framework</summary>        
        TrafegoPessoa() { }

        public TrafegoPessoa(string clienteId, 
                             string identificadorArquivo,
                             DateTime dataRegistro,
                             int qtdEstimadaPessoas,
                             double latitude,
                             double longitude)
        {
            ClienteId = clienteId;
            IdentificadorArquivo = identificadorArquivo;
            DataRegistro = dataRegistro;
            QtdEstimadaPessoas = qtdEstimadaPessoas;
            Latitude = latitude;
            Longitude = longitude;
        }

        [Column("cliente_id")]
        public string ClienteId { get; private set; }

        [Column("identificador_arquivo")]
        public string IdentificadorArquivo { get; private set; }

        [Column("data_registro")]
        public DateTime DataRegistro { get; private set; }

        [Column("qtd_estimada_pessoas")]
        public int QtdEstimadaPessoas { get; private set; }

        [Column("latitude")]
        public double Latitude { get; private set; }

        [Column("longitude")]   
        public double Longitude { get; private set; }
    }
}
