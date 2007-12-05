using System.Collections;
using Lucene.Net.Store;
using NHibernate.Search.Engine;
using NHibernate.Search.Impl;

namespace NHibernate.Search.Storage
{
	public interface IDirectoryProvider
	{
		void Initialize(string directoryProviderName, IDictionary indexProps, SearchFactory searchFactory);

		Directory Directory { get; }
	}
}