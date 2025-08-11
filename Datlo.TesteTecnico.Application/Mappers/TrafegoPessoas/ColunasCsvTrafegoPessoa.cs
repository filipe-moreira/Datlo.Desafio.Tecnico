namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas
{
    /// <summary>
    /// Classe que define o mapeamento das colunas do CSV para os campos de tráfego de pessoas.
    /// Permite que diferentes clientes tenham diferentes ordenações de colunas.
    /// </summary>
    public class ColunasCsvTrafegoPessoa
    {
        /// <summary>
        /// Índice da coluna que contém a data de registro
        /// </summary>
        public required int DATA_REGISTRO { get; init; }
        
        /// <summary>
        /// Índice da coluna que contém a quantidade estimada de pessoas
        /// </summary>
        public required int QTD_ESTIMADA_PESSOAS { get; init; }
        
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
