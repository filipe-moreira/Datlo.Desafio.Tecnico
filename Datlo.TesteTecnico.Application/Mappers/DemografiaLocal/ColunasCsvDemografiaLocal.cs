namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal
{
    public class ColunasCsvDemografiaLocal
    {

        /// <summary>
        /// Indice da coluna que contém a data de registro
        /// </summary>
        public required int DATA_REGISTRO { get; init; }

        /// <summary>
        /// Índice da coluna que contém a quantidade de pessoas
        /// </summary>
        public required int POPULACAO { get; init; }

        /// <summary>
        /// Índice da coluna que contém a renda média
        /// </summary>
        public required int RENDA_MEDIA { get; init; }

        /// <summary>
        /// Índice da coluna que contém a faixa etária predominante
        /// </summary>
        public required int FAIXA_ETARIA_PREDOMINANTE { get; init; }

        /// <summary>
        /// Índice da coluna que contém a latitude
        /// </summary>
        public required int LATITUDE { get; init; }

        /// <summary>
        /// Índice da coluna que contém a longitude
        /// </summary>
        public required int LONGITUDE { get; init; }
    }
}
