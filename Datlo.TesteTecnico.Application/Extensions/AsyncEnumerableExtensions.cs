using System.Runtime.CompilerServices;

namespace Datlo.TesteTecnico.Application.Extensions
{
    public static class AsyncEnumerableExtensions
    {
        public static async IAsyncEnumerable<List<T>> Buffer<T>(
            this IAsyncEnumerable<T> source,
            int size,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size));

            var buffer = new List<T>(size);

            await foreach (var item in source.WithCancellation(ct))
            {
                buffer.Add(item);
                if (buffer.Count >= size)
                {
                    yield return buffer;
                    buffer = new List<T>(size);
                }
            }

            if (buffer.Count > 0)
                yield return buffer;
        }
    }
}
