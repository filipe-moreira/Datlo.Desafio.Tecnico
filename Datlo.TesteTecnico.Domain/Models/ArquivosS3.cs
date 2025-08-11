namespace Datlo.TesteTecnico.Domain.Models
{
    public class ArquivosS3
    {
        
        public ArquivosS3(string clienteId, 
                          string bucket, 
                          string chave, 
                          TipoArquivo tipo, 
                          ModeloArquivo modelo)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(clienteId, nameof(clienteId));
            ArgumentException.ThrowIfNullOrWhiteSpace(bucket, nameof(bucket));
            ArgumentException.ThrowIfNullOrWhiteSpace(chave, nameof(chave));            

            ClienteId = clienteId;
            Bucket = bucket;
            Chave = chave;
            Tipo = tipo;
            Modelo = modelo;
        }   

        /// <summary>
        /// Tipo do arquivo armazenado no S3 
        /// </summary>
        public TipoArquivo Tipo { get; private set; }

        /// <summary>
        /// Modelo do arquivo (ex: Trafego de Pessoas, Demografica Local, etc)
        /// </summary>
        public ModeloArquivo Modelo { get; private set; }

        /// <summary>
        /// ID do cliente associado ao arquivo
        /// </summary>
        public string ClienteId { get; private set; }

        /// <summary>
        /// Nome do bucket no S3 onde o arquivo está armazenado
        /// </summary>
        public string Bucket { get; private set; }

        /// <summary>
        /// Chave do arquivo no S3 (caminho completo dentro do bucket)
        /// </summary>
        public string Chave { get; private set; }

        /// <summary>
        /// Fila do Hangfire onde o job de processamento será enfileirado
        /// </summary>
        public string Queue => Modelo switch
        {
            ModeloArquivo.TrafegoPessoas => "trafego-pessoas",
            ModeloArquivo.DemografiaLocal => "demografia-local",
            _ => "default"
        };
        public override string ToString()
        {
            var modelo = Modelo.ToString();
            var tipo = Tipo.ToString();
            var cliente = ClienteId;

            return $"{modelo} - {tipo} | ClienteId: {cliente}";
        }
    }
}
