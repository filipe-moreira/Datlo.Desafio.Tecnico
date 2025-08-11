using Amazon.S3;
using Datlo.TesteTecnico.Application.Ports;

namespace Datlo.TesteTecnico.Infrastructure.Services
{
    public sealed class S3ObjectStorage : IObjectStorage
    {
        private readonly IAmazonS3 _s3Client;

        public S3ObjectStorage(IAmazonS3 s3Client)
        {
            ArgumentNullException.ThrowIfNull(s3Client);

            _s3Client = s3Client;
        }

        /// <summary>
        /// Obtém um objeto da S3 como Stream
        /// </summary>
        /// <param name="bucketName"></param>
        /// <param name="objectKey"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<Stream> GetObjectStreamAsync(string bucketName, string objectKey,
                                                       CancellationToken cancellationToken)
        {
            var response = await _s3Client.GetObjectAsync(bucketName, objectKey, cancellationToken);
            return response.ResponseStream;
        }
    }

}
