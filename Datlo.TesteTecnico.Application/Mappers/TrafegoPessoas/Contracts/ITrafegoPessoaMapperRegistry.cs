namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts
{
    /// <summary>
    /// Interface para o registry de mappers.
    /// Define o contrato para localização de mappers por cliente.
    /// </summary>
    public interface ITrafegoPessoaMapperRegistry
    {
        /// <summary>
        /// Obtém o mapper apropriado para o cliente especificado.
        /// </summary>
        /// <param name="clienteId">ID do cliente</param>
        /// <returns>Mapper específico do cliente</returns>
        /// <exception cref="KeyNotFoundException">Quando nenhum mapper é encontrado para o cliente</exception>
        ITrafegoPessoaMapper Get(string clienteId);
    }
}
