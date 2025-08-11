using Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts;

namespace Datlo.TesteTecnico.Application.Mappers.TrafegoPessoas.Contracts
{
    public sealed class TrafegoPessoaMapperRegistry : ITrafegoPessoaMapperRegistry
    {
        private readonly Dictionary<string, ITrafegoPessoaMapper> _mappers;

        public TrafegoPessoaMapperRegistry(IEnumerable<ITrafegoPessoaMapper> mappers)
        {
            _mappers = mappers.ToDictionary(m => m.ClienteId, StringComparer.OrdinalIgnoreCase);
        }

        public ITrafegoPessoaMapper Get(string clienteNome)
        {
            if (_mappers.TryGetValue(clienteNome, out var mapper))
            {
                return mapper;
            }

            throw new KeyNotFoundException($"Nenhum mapper encontrado para o cliente '{clienteNome}'.");
        }
    }
}
