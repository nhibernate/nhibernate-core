#if !NET8_0_OR_GREATER
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NHibernate.Test.CacheTest;

internal static class TextReaderExtensions
{
	public static Task<string> ReadToEndAsync(this TextReader reader, CancellationToken cancellationToken) =>
		cancellationToken.IsCancellationRequested
			? Task.FromCanceled<string>(cancellationToken)
			: reader.ReadToEndAsync();
}
#endif
