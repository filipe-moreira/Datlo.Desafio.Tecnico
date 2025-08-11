using System.ComponentModel.DataAnnotations.Schema;

namespace Datlo.TesteTecnico.Domain.Entidades
{
    public class DemografiaLocal : Entity, IAggregateRoot
    {

        /// <summary>Construtor padrão para o Entity Framework</summary>       
        DemografiaLocal() { }


        public DemografiaLocal(string clienteId,
                               string identificadorArquivo,
                               DateTime dataRegistro,
                               int populacao,
                               double latitude,
                               double longitude,
                               double rendaMedia,
                               string faixaEtariaPredominante)
        {
            ClienteId = clienteId;
            IdentificadorArquivo = identificadorArquivo;
            DataRegistro = dataRegistro;
            Populacao = populacao;
            Latitude = latitude;
            Longitude = longitude;
            RendaMedia = rendaMedia;
            FaixaEtariaPredominante = faixaEtariaPredominante;
        }

        [Column("cliente_id")]
        public string ClienteId { get; private set; }

        [Column("identificador_arquivo")]
        public string IdentificadorArquivo { get; private set; }

        [Column("data_registro")]
        public DateTime DataRegistro { get; private set; }

        [Column("populacao")]
        public int Populacao { get; private set; }

        [Column("latitude")]
        public double Latitude { get; private set; }

        [Column("longitude")]
        public double Longitude { get; private set; }

        [Column("renda_media")]
        public double RendaMedia { get; private set; }

        [Column("faixa_etaria_predominante")]
        public string FaixaEtariaPredominante { get; private set; }
    }
}