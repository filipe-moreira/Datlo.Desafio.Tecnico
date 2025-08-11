namespace Datlo.TesteTecnico.Application.Ports
{
    public interface IObjectStorage
    {
        /// <summary>
        /// Obtém um objeto da S3 como Stream
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Stream> GetObjectStreamAsync(string bucketName, string objectKey, 
            CancellationToken cancellationToken);
    }
}
