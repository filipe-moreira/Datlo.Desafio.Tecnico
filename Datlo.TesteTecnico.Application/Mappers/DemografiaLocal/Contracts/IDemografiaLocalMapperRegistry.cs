namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts
{
    public interface IDemografiaLocalMapperRegistry
    {
        /// <summary>
        /// Obtém o mapper apropriado para o cliente especificado.
        /// </summary>
        /// <param name="clienteNome">Nome do cliente</param>
        /// <returns>Mapper específico do cliente</returns>
        /// <exception cref="KeyNotFoundException">Quando nenhum mapper é encontrado para o cliente</exception>
        IDemografiaLocalMapper Get(string clienteNome);
    }
}
