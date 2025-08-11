namespace Datlo.TesteTecnico.Application.Mappers.DemografiaLocal.Contracts
{
    public class DemografiaLocalMapperRegistry : IDemografiaLocalMapperRegistry
    {
        private readonly Dictionary<string, IDemografiaLocalMapper> _mappers;

        public DemografiaLocalMapperRegistry(IEnumerable<IDemografiaLocalMapper> mappers)
        {
            _mappers = mappers.ToDictionary(m => m.ClienteId, StringComparer.OrdinalIgnoreCase);
        }
        public IDemografiaLocalMapper Get(string clienteNome)
        {
            if (_mappers.TryGetValue(clienteNome, out var mapper))
            {
                return mapper;
            }

            throw new KeyNotFoundException($"Nenhum mapper encontrado para o cliente '{clienteNome}'.");
        }
    }
}
